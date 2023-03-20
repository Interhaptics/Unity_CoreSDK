/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Interhaptics.HapticBodyMapping;

namespace Interhaptics.Utils
{
    [AddComponentMenu("Interhaptics/AudioHapticSource")]
    [RequireComponent(typeof(AudioSource))]
    public class AudioHapticSource : Internal.HapticSource
    {
        // AudioSource variables and properties
        [SerializeField]
        public AudioSource audioSource;

        [SerializeField]
        private HapticBodyPart[] hapticBodyParts;

        [SerializeField]
        private bool playOnAwake = false;
        public bool PlayOnAwake { get { return playOnAwake; } set { playOnAwake = value; } }

        protected override void Awake()
        {
            if (audioSource == null && !TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogError("AudioSource is not assigned, please assign one");
                return;
            }
            base.Awake();
            if (audioSource.playOnAwake)
            {
                audioSource.playOnAwake = false;
                playOnAwake = true;
            }
            if (playOnAwake)
            {
                Play();
            }
        }

        public override void Play()
        {
            AddTarget(hapticBodyParts.Select(hapticBodyPart => new CommandData(Operator.Plus, hapticBodyPart.BodyPart, hapticBodyPart.Side)).ToList());
            base.Play();
            audioSource.Play();
        }

        public override void Stop()
        {
            base.Stop();
            audioSource.Stop();
            RemoveTarget(hapticBodyParts.Select(hapticBodyPart => new CommandData(Operator.Plus, hapticBodyPart.BodyPart, hapticBodyPart.Side)).ToList());
        }
    }
}