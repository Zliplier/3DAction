using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.Audio
{
    public class MusicManager : Singleton<MusicManager>
    {
        [SerializeField] private AudioSource music;
        public bool isMusicPlaying => music.isPlaying;

        private Coroutine co_Transitioning;
        public bool isTransitioning => co_Transitioning != null;
        
        private void PlayMusic(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false)
        {
            if (music.name.Equals(clip.name))
                return;
            
            music.clip = clip;
            music.volume = volume;
            music.pitch = pitch;
            music.loop = loop;
            music.Play();
        }

        public void PlayMusicWithTransition(AudioClip clip, float duration = 1f, float volume = 1f, float pitch = 1f, bool loop = false)
        {
            if (music.name.Equals(clip.name))
                return;
            
            if (duration > 0f)
                co_Transitioning = StartCoroutine(TransitionMusic(clip, duration, volume, pitch, loop));
            else
            {
                PlayMusic(clip, volume, pitch, loop);
            }
        }

        private IEnumerator TransitionMusic(AudioClip clip, float duration, float volume = 1f, float pitch = 1f, bool loop = false)
        {
            if (isMusicPlaying)
                yield return Fading(true, volume, duration / 2);
            
            PlayMusic(clip, volume, pitch, loop);
            
            yield return Fading(false, volume, duration / 2);

            co_Transitioning = null;
        }

        public void StopMusic()
        {
            music.Stop();
        }

        public void FadingIn(float duration, float volume = 1f)
        {
            co_Transitioning = StartCoroutine(Fading(false, volume, duration));
        }

        public void FadingOut(float duration, float volume = 1f)
        {
            co_Transitioning = StartCoroutine(Fading(true, volume, duration));
        }
        
        private IEnumerator Fading(bool isFadingOut, float volume, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                if (isFadingOut)
                    music.volume = Mathf.Lerp(volume, 0f, elapsedTime / duration);
                else
                    music.volume = Mathf.Lerp(0f, volume, elapsedTime / duration);
                
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }

            if (isFadingOut) StopMusic();
            
            co_Transitioning = null;
        }
    }
}