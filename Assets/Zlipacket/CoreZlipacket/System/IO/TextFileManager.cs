using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zlipacket.CoreZlipacket.Scene.System.IO;

namespace Zlipacket.CoreZlipacket.System.IO
{
    public class TextFileManager
    {
        public static List<string> ReadTextFile(string filepath, bool includeBlanklines = true)
        {
            if (!filepath.StartsWith('/'))
                filepath = FilePath.root + filepath;
            
            List<string> lines = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(filepath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (includeBlanklines || !string.IsNullOrWhiteSpace(line))
                            lines.Add(line);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.LogError($"File not found: '{ex.FileName}'");
            }
            
            return lines;
        }
        
        public static List<string> ReadTextAsset(string filepath, bool includeBlanklines = true)
        {
            TextAsset asset = Resources.Load<TextAsset>(filepath);
            if (asset == null)
            {
                Debug.LogError($"Asset not found: '{filepath}'");
                return null;
            }
            
            return ReadTextAsset(asset, includeBlanklines);
        }
        
        public static List<string> ReadTextAsset(TextAsset asset, bool includeBlanklines = true)
        {
            List<string> lines = new List<string>();
            
            using (StringReader sr = new StringReader(asset.text))
            {
                while (sr.Peek() > -1)
                {
                    string line = sr.ReadLine();
                    if (includeBlanklines || !string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }
            }
            
            return lines;
        }
    }
}