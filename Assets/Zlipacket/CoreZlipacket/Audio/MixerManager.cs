using UnityEngine;
using UnityEngine.Audio;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.Audio
{
    public class MixerManager : Singleton<MixerManager>
    {
        public const float DEFAULT_VOLUME = 0.7f;
        
        [SerializeField] private AudioMixer audioMixer;
        
        public float GetVolume(MixerType type)
        {
            float volume = DEFAULT_VOLUME;
            
            if (PlayerPrefs.HasKey(type.ToString()))
                volume = PlayerPrefs.GetFloat(type.ToString());
            else
            {
                //No Saved Volume Data
                SetVolume(type, volume);
            }
            
            return volume;
        }

        public void SetVolume(MixerType type, float volume)
        {
            PlayerPrefs.SetFloat(type.ToString(), volume);
            audioMixer.SetFloat(type.ToString(), Mathf.Log10(volume) * 20);
        }
    }

    public enum MixerType
    {
        MasterVolume,
        MusicVolume,
        SfxVolume,
        VoiceVolume
    }
}