using System;
using UnityEngine;
using UnityEngine.Audio;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.Audio
{
    public class SfxManager : Singleton<SfxManager>
    {
        [SerializeField] private AudioSource sfxObject;
        
        public AudioSource PlaySoundFX(AudioClip clip, Vector3 position, float volume = 1f, float spatialBlend = 0f, float pitch = 1f, bool loop = false, AudioMixerGroup mixer = null)
        {
            AudioSource soundFX = PlaySoundFX(clip, volume, spatialBlend, pitch, loop, mixer);
            soundFX.transform.position = position;
            return soundFX;
        }
        
        public AudioSource PlaySoundFX(AudioClip clip, float volume = 1f, float spatialBlend = 0f, float pitch = 1f, bool loop = false, AudioMixerGroup mixer = null)
        {
            AudioSource soundFX;
            
            soundFX = Instantiate(sfxObject, transform);
            soundFX.name = clip.name;
            
            if (mixer != null)
                soundFX.outputAudioMixerGroup = mixer;
            soundFX.clip = clip;
            soundFX.volume = volume;
            soundFX.spatialBlend = spatialBlend;
            soundFX.pitch = pitch;
            soundFX.loop = loop;
            soundFX.Play();

            if (loop)
                Destroy(soundFX.gameObject, (clip.length / pitch) + 1);
            
            return soundFX;
        }

        public void StopSoundEffect(string soundName)
        {
            AudioSource[] sources = transform.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource source in sources)
            {
                if (String.Equals(source.name, soundName, (StringComparison)StringComparison.InvariantCultureIgnoreCase))
                {
                    Destroy(source.gameObject);
                    return;
                }
            }
        }
        
        public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);
    }
}