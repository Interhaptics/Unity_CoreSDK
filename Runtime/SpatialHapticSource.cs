/* ​
* Copyright © 2022 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;


namespace Interhaptics
{

    [AddComponentMenu("Interhaptics/SpatialHapticSource")]
    [RequireComponent(typeof(Rigidbody))]
    public class SpatialHapticSource : Internal.HapticSource
    {

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
        }

        private void OnTriggerEnter(Collider other)
        {
            AddTarget(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            RemoveTarget(other.gameObject);
        }

        private void OnCollisionExit(Collision other)
        {
            RemoveTarget(other.gameObject);
        }

    }

}