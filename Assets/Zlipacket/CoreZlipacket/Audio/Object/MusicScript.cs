using UnityEngine;

namespace Zlipacket.CoreZlipacket.Audio.Object
{
    public class MusicScript : MonoBehaviour
    {
        public MusicManager musicManager => MusicManager.Instance;
        
        public AudioClip audioClip;
        public float volume = 1f;
        public bool playOnStart = false;
        public float transitionDuration = 1f;
        
        private void Start()
        {
            if (playOnStart)
                PlaySong();
        }

        private void OnDisable()
        {
            if (musicManager != null)
                musicManager.FadingOut(transitionDuration / 2, volume);
        }

        public void PlaySong()
        {
            musicManager.PlayMusicWithTransition(audioClip, transitionDuration, volume);
        }
    }
}