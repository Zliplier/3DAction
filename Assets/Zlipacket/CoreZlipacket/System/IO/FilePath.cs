using System.IO;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.Scene.System.IO
{
    public class FilePath
    {
        private const string HOME_DIRECTORY_SYMBOL = "~/"; 
        
        public static readonly string root = $"{Application.dataPath}/GameData/";
        
        public static readonly string resources_graphics = "Graphics/";
        public static readonly string resources_backgroundImages = $"{resources_graphics}BgImages/";
        public static readonly string resources_backgroundVideos = $"{resources_graphics}BgVideos/";
        public static readonly string resources_blendTextures = $"{resources_graphics}BlendTextures/";
        
        public static readonly string resources_audio = "Audio/";
        public static readonly string resources_sfx = $"{resources_audio}Sfx/";
        public static readonly string resources_voices = $"{resources_audio}Voices/";
        public static readonly string resources_music = $"{resources_audio}Music/";
        
        public static string GetPathToResources(string defaultPath, string resourceName)
        {
            if (resourceName.StartsWith(HOME_DIRECTORY_SYMBOL))
                return resourceName.Substring(HOME_DIRECTORY_SYMBOL.Length);
            
            return defaultPath + resourceName;
        }

        public static string GetPathToPersistantData(string dataName)
        {
            return Path.Combine(Application.persistentDataPath, dataName);
        }
    }
}