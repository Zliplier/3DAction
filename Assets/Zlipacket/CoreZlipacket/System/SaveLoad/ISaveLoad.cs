using System.IO;
using Zlipacket.CoreZlipacket.Scene.System.IO;

namespace Zlipacket.CoreZlipacket.System.SaveLoad
{
    public interface ISaveLoad
    {
        public string SaveToJson(bool prettyPrint = true);
        public void LoadFromJson(string json);

        public void SaveToFile(string relativePath);
        public bool LoadFromFile(string relativePath);
    }
}