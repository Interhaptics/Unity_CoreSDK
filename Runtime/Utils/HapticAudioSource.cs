using UnityEngine;
using System.Collections.Generic;

namespace Interhaptics.Utils
{
    [AddComponentMenu("Interhaptics/HapticAudioSource")]
    [RequireComponent(typeof(AudioSource))]
    public class HapticAudioSource : Internal.HapticSource
    {
        // AudioSource variables and properties
        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private HapticBodyPart hapticBodyPart;
        public AudioSource AudioManager { get { return audioSource; } set { audioSource = value; } }

        [SerializeField]
        private bool playOnAwake = false;
        public bool PlayOnAwake { get { return playOnAwake; } set { playOnAwake = value; } }

        protected override void Awake()
        {
            base.Awake();
            if (TryGetComponent<AudioSource>(out AudioSource audioSource) == false)
            {
                Debug.LogError("AudioSource is not assigned, please assign one");
                return;
            }
            audioSource.playOnAwake = playOnAwake;
            if (playOnAwake)
            {
                Play();
            }
        }

        public override void Play()
        {
            AddTarget(hapticBodyPart.ToCommandData());
            base.Play();
            audioSource.Play();
        }

        public override void Stop()
        {
            base.Stop();
            audioSource.Stop();
            RemoveTarget(hapticBodyPart.ToCommandData());
        }
    }
}