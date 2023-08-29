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
		private const string ERROR_MESSAGE_MONO = "Interhaptics requires IL2CPP scripting backend for Android. Please change it in Player Settings. Haptics will not play on the Mono scripting backend on the Android platform.";

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
#if UNITY_ANDROID
			if (HapticManager.Instance.monoScriptingBackend)
			{
				DebugMode(ERROR_MESSAGE_MONO + "Haptic Source");
				return;
			}
            else
            {
                DebugMode("IL2CPP Haptic Source");
            }
#endif
            HapticMaterialId = Core.HAR.AddHM(hapticMaterial);
		}

		public virtual void Play()
        {
            Core.HAR.PlayEvent(HapticMaterialId, -Time.realtimeSinceStartup + vibrationOffset, textureOffset, stiffnessOffset);
        }

        public virtual void PlayTexture(float _distance)
        {
            Debug.Log("Playing texture! " + HapticMaterialId);
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