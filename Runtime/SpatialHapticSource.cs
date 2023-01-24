/* ​
* Copyright © 2022 Go Touch VR SAS. All rights reserved. ​
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
        [HideInInspector]
        public bool playOnStart;
        [HideInInspector]
        public bool customBodyPart;
        [HideInInspector]
        public GameObject hapticBodyPartObject;
        [HideInInspector]
        public bool debugMode = false;
        [HideInInspector]
        public bool playOnCollision = true;
        [HideInInspector]
        public bool playOnTrigger = true;

        float timeHapticVibration;
        
        public void Start()
        {
            timeHapticVibration = (float)Interhaptics.Core.HAR.GetVibrationLength(HapticMaterialId);
            if (playOnStart)
            {
                AddTarget(hapticBodyPartObject);
                StartCoroutine(ControlVibration());
            }
            if (playOnCollision)
                { 
                    if (TryGetComponent<Collider>(out Collider collider))
                        {
                            collider.isTrigger = false;
                        }
                }
        }

        public void DebugMode(string debugMessage)
        {
            if (debugMode)
            {
                Debug.Log(debugMessage);
            }
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
                Stop();
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

        private void AddTarget(GameObject target)
        {
            if (target.TryGetComponent(out HapticBodyPart hbp))
            {
                AddTarget(hbp.ToCommandData());
            }
        }

        private void RemoveTarget(GameObject target)
        {
            if (target.TryGetComponent(out HapticBodyPart hbp))
            {
                RemoveTarget(hbp.ToCommandData());
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            AddTarget(other.gameObject);
            if (playOnCollision)
            {
                if (timeHapticVibration > 0)
                {
                    StartCoroutine(ControlVibration());
                }
                else
                {
                    Play();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            AddTarget(other.gameObject);
            if (playOnTrigger)
            {
                if (timeHapticVibration > 0)
                {
                    StartCoroutine(ControlVibration());
                }
                else
                {
                    Play();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            RemoveTarget(other.gameObject);
            if (playOnCollision)
            {
                Stop();
            }
        }

        private void OnCollisionExit(Collision other)
        {
            RemoveTarget(other.gameObject);
            if (playOnCollision)
            {
                Stop();
            }
        }

    }
}