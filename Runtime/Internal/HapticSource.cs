/* ​
* Copyright © 2022 Go Touch VR SAS. All rights reserved. ​
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
        private HapticMaterial hapticMaterial;
        private int hapticMaterialId;

        [SerializeField]
        private float vibrationOffset;
        [SerializeField]
        private float textureOffset;
        [SerializeField]
        private float stiffnessOffset;

        void Awake()
        {
            hapticMaterialId = Core.HAR.AddHM(hapticMaterial);
        }

        public void Play()
        {
            Core.HAR.PlayEvent(hapticMaterialId, -Time.realtimeSinceStartup + vibrationOffset, textureOffset, stiffnessOffset);
        }

        public void Stop()
        {
            Core.HAR.StopEvent(hapticMaterialId);
        }

        public void AddTarget(List<HapticBodyMapping.CommandData> Target)
        {
            Core.HAR.AddTargetToEvent(hapticMaterialId, Target);
        }

        public void RemoveTarget(List<HapticBodyMapping.CommandData> Target)
        {
            Core.HAR.RemoveTargetFromEvent(hapticMaterialId, Target);
        }

    }

}