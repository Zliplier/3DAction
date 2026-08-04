using UnityEngine;
using UnityEngine.Audio;

namespace Zlipacket.CoreZlipacket.Audio.Object
{
    public class SfxScript : MonoBehaviour
    {
        public AudioClip audioClip;
        public float volume = 1f;
        public float spatialBlend = 0f;
        public float pitch = 1f;
        public bool loop = false;
        public AudioMixerGroup mixer = null;

        public void PlaySfx()
        {
            SfxManager.Instance.PlaySoundFX(audioClip, volume, spatialBlend, pitch, loop, mixer);
        }
    }
}