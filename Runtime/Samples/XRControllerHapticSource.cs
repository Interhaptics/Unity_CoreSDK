/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interhaptics;

namespace Interhaptics.Samples
{
    public class XRControllerHapticSource : SpatialHapticSource
    {
        [SerializeField]
        private bool playOnBothControllers;
        [SerializeField]
        private bool playLeftFirst;
        [SerializeField]
        private bool stopHapticsExitTriggerCollision;
        [SerializeField]
        private GameObject controllerLeft;
        [SerializeField]
        private GameObject controllerRight;

        private GameObject controller => playLeftFirst ? controllerLeft : controllerRight;

        protected override void Start()
        {
            base.Start();
        }

        public IEnumerator XRControlVibration(GameObject hbp)
        {
            DebugMode(string.Format("Started playing  haptics! + {0}", Time.time));
            Play();
            yield return new WaitForSeconds(timeHapticVibration);
            RemoveTarget(hbp);
            DebugMode(string.Format("Finished playing haptics at timestamp : + {0} at {1}", timeHapticVibration, Time.time));
        }

        protected override void OnCollisionEnter(Collision other)
        {
            DebugMode("Collision:" + timeHapticVibration + other.gameObject);
            ActivateHaptics(other.gameObject);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            DebugMode("Trigger:" + timeHapticVibration + other.gameObject);
            ActivateHaptics(other.gameObject);
        }

        private void ActivateHaptics(GameObject other)
        {
            if ((playOnBothControllers) || (other == controller))
            {
                AddTarget(other);
            }
            if (timeHapticVibration > 0)
            {
                StartCoroutine(XRControlVibration(other));
            }
            else
            {
                Play();
            }
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (stopHapticsExitTriggerCollision)
            {
                RemoveTarget(other.gameObject);
            }
        }

        protected override void OnCollisionExit(Collision other)
        {
            if (stopHapticsExitTriggerCollision)
            {
                RemoveTarget(other.gameObject);
            }
        }
    }
}

