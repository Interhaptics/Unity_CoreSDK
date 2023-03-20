/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;
using System.Collections.Generic;


namespace Interhaptics.Internal
{

    [AddComponentMenu("Interhaptics/HapticSource")]
    public class HapticSource : MonoBehaviour
    {

        [SerializeField]
        public HapticMaterial hapticMaterial;

        [SerializeField]
        private float vibrationOffset;
        [SerializeField]
        private float textureOffset;
        [SerializeField]
        private float stiffnessOffset;
        [SerializeField]
        public bool debugMode;

        public int HapticMaterialId
        {
            get; set;
        }

        public void DebugMode(string debugMessage)
        {
            if (debugMode)
            {
                Debug.Log(debugMessage);
            }
        }

        protected virtual void Awake()
        {
            HapticMaterialId = Core.HAR.AddHM(hapticMaterial);
        }

        public virtual void Play()
        {
            Core.HAR.PlayEvent(HapticMaterialId, -Time.realtimeSinceStartup + vibrationOffset, textureOffset, stiffnessOffset);
        }

        public virtual void Stop()
        {
            Core.HAR.StopEvent(HapticMaterialId);
        }

        public void AddTarget(List<HapticBodyMapping.CommandData> Target)
        {
            Core.HAR.AddTargetToEvent(HapticMaterialId, Target);
        }

        public void RemoveTarget(List<HapticBodyMapping.CommandData> Target)
        {
            Core.HAR.RemoveTargetFromEvent(HapticMaterialId, Target);
        }

    }

}