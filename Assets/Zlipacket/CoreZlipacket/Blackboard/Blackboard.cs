using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Zlipacket.CoreZlipacket.Scene.System.IO;
using Zlipacket.CoreZlipacket.System.SaveLoad;
using Zlipacket.CoreZlipacket.Tools;
using Zlipacket.CoreZlipacket.Tools.Extensions;

namespace Zlipacket.CoreZlipacket.Blackboard {
    [Serializable]
    public readonly struct BlackboardKey : IEquatable<BlackboardKey> {
        readonly string name;
        readonly int hashedKey;

        public BlackboardKey(string name) {
            this.name = name;
            hashedKey = name.ComputeFNV1aHash();
        }
        
        public bool Equals(BlackboardKey other) => hashedKey == other.hashedKey;
        
        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => hashedKey;
        public override string ToString() => name;
        
        public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs.hashedKey == rhs.hashedKey;
        public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => !(lhs == rhs);
    }
    
    public interface IBlackboardEntry {
        object ValueObject { get; }
        Type ValueType { get; }
    }

    [Serializable]
    public class BlackboardEntry<T> : IBlackboardEntry {
        public BlackboardKey Key { get; }
        public T Value { get; }
        public Type ValueType { get; }
        
        object IBlackboardEntry.ValueObject => Value;

        public BlackboardEntry(BlackboardKey key, T value) {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }
        
        public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;
        public override int GetHashCode() => Key.GetHashCode();
    }
    
    [Serializable]
    public class Blackboard : ISaveLoad {
        Dictionary<string, BlackboardKey> keyRegistry = new();
        Dictionary<BlackboardKey, object> entries = new();
        
        public List<Action> PassedActions { get; } = new();

        public void AddAction(Action action) {
            Preconditions.CheckNotNull(action);
            PassedActions.Add(action);
        }
        
        public void ClearActions() => PassedActions.Clear();

        /*public IEnumerable<(BlackboardKey key, object value, Type type)> Entries {
            get {
                foreach (var kvp in entries) {
                    var entry = (IBlackboardEntry)kvp.Value;
                    yield return (kvp.Key, entry.ValueObject, entry.ValueType);
                }
            }
        }*/
        
        public readonly struct Entry {
            public readonly BlackboardKey Key;
            public readonly object Value;
            public readonly Type ValueType;

            public Entry(BlackboardKey key, object value, Type valueType) {
                Key = key;
                Value = value;
                ValueType = valueType;
            }
        }

        public IEnumerable<Entry> Entries {
            get {
                foreach (var kvp in entries) {
                    var entryType = kvp.Value.GetType();
                    var valueProp = entryType.GetProperty("Value");
                    var valueTypeProp = entryType.GetProperty("ValueType");

                    var value = valueProp.GetValue(kvp.Value);
                    var valueType = (Type)valueTypeProp.GetValue(kvp.Value);

                    yield return new Entry(kvp.Key, value, valueType);
                }
            }
        }
        
        public void Debug() {
            /*foreach (var entry in entries) {
                var entryType = entry.Value.GetType();

                if (entryType.IsGenericType && entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>)) {
                    var valueProperty = entryType.GetProperty("Value");
                    if (valueProperty == null) continue;
                    var value = valueProperty.GetValue(entry.Value);
                    UnityEngine.Debug.Log($"Key: {entry.Key}, Value: {value}");
                }
            }*/
            
            /*foreach (var kvp in entries) {
                var entry = (IBlackboardEntry)kvp.Value;
                UnityEngine.Debug.Log($"Key: {kvp.Key}, Value: {entry.ValueObject}");
            }*/
        }

        public bool TryGetValue<T>(BlackboardKey key, out T value) {
            if (entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry) {
                value = castedEntry.Value;
                return true;
            }
            
            value = default;
            return false;
        }
        
        public void SetValue<T>(BlackboardKey key, T value) {
            entries[key] = new BlackboardEntry<T>(key, value);
        }

        public BlackboardKey GetOrRegisterKey(string keyName) {
            Preconditions.CheckNotNull(keyName);

            if (!keyRegistry.TryGetValue(keyName, out BlackboardKey key)) {
                key = new BlackboardKey(keyName);
                keyRegistry[keyName] = key;
            }
            
            return key;
        }
        
        public bool ContainsKey(BlackboardKey key) => entries.ContainsKey(key);
        
        public void Remove(BlackboardKey key) => entries.Remove(key);
        
        public string SaveToJson(bool prettyPrint = true)
        {
            var snapshot = new BlackboardSnapshot();

            // Preserve all registered keys, even ones without a current value
            foreach (var keyName in keyRegistry.Keys) {
                snapshot.registeredKeys.Add(keyName);
            }

            foreach (var kvp in entries) {
                var boxedEntry = kvp.Value;
                var entryType = boxedEntry.GetType(); // BlackboardEntry<T>

                var valueProp = entryType.GetProperty("Value");
                var valueTypeProp = entryType.GetProperty("ValueType");

                var value = valueProp.GetValue(boxedEntry);
                var valueType = (Type)valueTypeProp.GetValue(boxedEntry);

                snapshot.entries.Add(new BlackboardEntrySnapshot {
                    key = kvp.Key.ToString(), // BlackboardKey.ToString() returns the name
                    typeName = valueType.AssemblyQualifiedName,
                    json = JsonConvert.SerializeObject(value)
                });
            }

            return JsonConvert.SerializeObject(snapshot, prettyPrint ? Formatting.Indented : Formatting.None);
        }

        public void LoadFromJson(string json)
        {
            var snapshot = JsonConvert.DeserializeObject<BlackboardSnapshot>(json);
            if (snapshot == null) return;

            keyRegistry.Clear();
            entries.Clear();

            foreach (var keyName in snapshot.registeredKeys) {
                GetOrRegisterKey(keyName);
            }

            var setValueMethod = typeof(Blackboard).GetMethod(nameof(SetValue));

            foreach (var entryData in snapshot.entries) {
                var valueType = Type.GetType(entryData.typeName);
                if (valueType == null) {
                    UnityEngine.Debug.LogWarning($"Blackboard: could not resolve type '{entryData.typeName}' for key '{entryData.key}', skipping.");
                    continue;
                }

                var key = GetOrRegisterKey(entryData.key);
                object value;
                try {
                    value = JsonConvert.DeserializeObject(entryData.json, valueType);
                } catch (Exception ex) {
                    UnityEngine.Debug.LogWarning($"Blackboard: failed to deserialize value for key '{entryData.key}': {ex.Message}");
                    continue;
                }

                var genericSetValue = setValueMethod.MakeGenericMethod(valueType);
                genericSetValue.Invoke(this, new[] { (object)key, value });
            }
        }

        public void SaveToFile(string relativePath)
        {
            string path = FilePath.GetPathToPersistantData(relativePath);
            
            UnityEngine.Debug.Log($"Saving to {path}");
            File.WriteAllText(path, SaveToJson());
        }

        public bool LoadFromFile(string relativePath)
        {
            string path = FilePath.GetPathToPersistantData(relativePath);
            
            if (!File.Exists(path)) {
                UnityEngine.Debug.Log($"Blackboard: no save file found at '{path}'.");
                return false;
            }
            LoadFromJson(File.ReadAllText(path));
            return true;
        }
    }
}