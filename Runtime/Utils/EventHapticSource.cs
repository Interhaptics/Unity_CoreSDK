/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interhaptics;
using System.Linq;
using Interhaptics.HapticBodyMapping;

namespace Interhaptics.Internal
{
    [AddComponentMenu("Interhaptics/EventHapticSource")]
    public class EventHapticSource : Internal.HapticSource
    {
        public bool playOnStart;
        public HapticBodyPart[] hapticBodyParts;
        public float delayPlay = 0f;

        private float timeHapticVibration;

        public override void Play()
        {
            AddTarget(hapticBodyParts.Select(hapticBodyPart => new CommandData(Operator.Plus, hapticBodyPart.BodyPart, hapticBodyPart.Side)).ToList());
            base.Play();
        }

        public override void Stop()
        {
            foreach (var hapticBodyPart in hapticBodyParts)
            {
                RemoveTarget(hapticBodyPart.ToCommandData());
            }
         //   RemoveTarget(hapticBodyParts.Select(hapticBodyPart => new CommandData(Operator.Plus, hapticBodyPart.BodyPart, hapticBodyPart.Side)).ToList());
#if !UNITY_PS5 //PS5 platform needs this commented out, otherwise residual haptics and no haptic playback unitl provider update
            base.Stop();
#endif
        }

        private void Start()
        {
            timeHapticVibration = (float)Interhaptics.Core.HAR.GetVibrationLength(HapticMaterialId);
            if (playOnStart)
            {
                 StartCoroutine(ControlVibration());
            }
        }

        public void PlayEventVibration()
        {
            StartCoroutine(ControlVibration());
        }

        /// <summary>
        /// Controls the vibration perception based on the full length of the haptic material; stops any residual haptics which might come from the controller after the haptic playback length
        /// </summary>
        /// <returns></returns>
        public IEnumerator ControlVibration() //move to hapticsource
        {
            yield return new WaitForSeconds(delayPlay);
            DebugMode(string.Format("Started playing haptics! + {0}", Time.time));
            Play();
            yield return new WaitForSeconds(timeHapticVibration);
            DebugMode(string.Format("Finished playing haptics at timestamp : + {0} at {1}", timeHapticVibration, Time.time));
            Stop(); 
        }
    }

}
