using System;
using System.Collections.Generic;

namespace Zlipacket.CoreZlipacket.Blackboard
{
    [Serializable]
    public class BlackboardEntrySnapshot {
        public string key;
        public string typeName; // AssemblyQualifiedName
        public string json;     // serialized value
    }
    
    [Serializable]
    public class BlackboardSnapshot {
        public List<string> registeredKeys = new();
        public List<BlackboardEntrySnapshot> entries = new();
    }
}