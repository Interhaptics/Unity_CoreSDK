/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interhaptics;

namespace Interhaptics
{
    [AddComponentMenu("Interhaptics/SpatialHapticSource")]
    [RequireComponent(typeof(Rigidbody))]
    public class SpatialHapticSource : Internal.HapticSource
    {
        public enum PlayMethod
        {
            Undefined,
            OnCollision,
            OnTrigger
        }

        public PlayMethod playMethod;
        
        [HideInInspector]
        public float timeHapticVibration;
        
        protected virtual void Start()
        {
            timeHapticVibration = (float)Interhaptics.Core.HAR.GetVibrationLength(HapticMaterialId);
        }

        /// <summary>
        /// Controls the vibration perception based on the full length of the haptic material; stops any residual haptics which might come from the controller after the haptic playback length
        /// </summary>
        /// <returns></returns>
        public IEnumerator ControlVibration()
        {
            DebugMode(string.Format("Started playing  haptics! + {0}",Time.time));
            Play();
            yield return new WaitForSeconds(timeHapticVibration);
            DebugMode(string.Format("Finished playing haptics at timestamp : + {0} at {1}", timeHapticVibration, Time.time));
        }

#if UNITY_EDITOR
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void Reset()
        {
            SphereCollider sphere = GetComponent<SphereCollider>();
            BoxCollider box = GetComponent<BoxCollider>();

            if (sphere == null && box == null)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Choose a Component", "You are missing one of the required componets. Please choose one to add", "SphereCollider", "BoxCollider"))
                {
                    gameObject.AddComponent<SphereCollider>().isTrigger = true;
                }
                else
                {
                    gameObject.AddComponent<BoxCollider>().isTrigger = true;
                }
            }
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        #endif

        public void AddTarget(GameObject target)
        {
            if (target.TryGetComponent(out HapticBodyPart hbp))
            {
                AddTarget(hbp.ToCommandData());
            }
        }

        public void RemoveTarget(GameObject target)
        {
            if (target.TryGetComponent(out HapticBodyPart hbp))
            {
                RemoveTarget(hbp.ToCommandData());
            }
        }

        protected virtual void OnCollisionEnter(Collision other)
        {
            if ((playMethod == PlayMethod.OnCollision)&&(other.gameObject.GetComponent<HapticBodyPart>() != null))
            {  
                ActivateHaptics(other.gameObject);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if ((playMethod == PlayMethod.OnTrigger) && (other.gameObject.GetComponent<HapticBodyPart>()!=null))
            { 
                ActivateHaptics(other.gameObject);
            }
        }

        private void ActivateHaptics(GameObject other)
        {
            AddTarget(other);
            if (timeHapticVibration > 0)
            {
                StartCoroutine(ControlVibration());
            }
            else
            {
                Play();
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            RemoveTarget(other.gameObject);
        }

        protected virtual void OnCollisionExit(Collision other)
        {
            RemoveTarget(other.gameObject);
        }

    }
}