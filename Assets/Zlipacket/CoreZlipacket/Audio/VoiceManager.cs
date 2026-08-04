using System;
using UnityEngine;
using UnityEngine.Audio;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.Audio
{
    public class VoiceManager : Singleton<VoiceManager>
    {
        [SerializeField] private AudioSource voice;
    }
}