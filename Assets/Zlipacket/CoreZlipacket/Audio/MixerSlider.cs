using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zlipacket.CoreZlipacket.Audio
{
    public class MixerSlider : MonoBehaviour, IPointerUpHandler
    {
        private Slider slider;
             
        public MixerType mixerType = MixerType.MasterVolume;
        
        public AudioClip sliderUpSound;
        
        private MixerManager mixer => MixerManager.Instance;
        
        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        private void Start()
        {
            if (slider == null)
                return;
            
            slider.value = mixer.GetVolume(mixerType);
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            mixer.SetVolume(mixerType, value);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (sliderUpSound != null)
                SfxManager.Instance.PlaySoundFX(sliderUpSound);
        }
    }
}