/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Interhaptics
{

    [UnityEngine.AddComponentMenu("Interhaptics/HapticManager")]
    public sealed class HapticManager : Internal.Singleton<HapticManager>
    {
		[SerializeField] private bool debugSwitch = true;

		public bool DebugSwitch
		{
			get => debugSwitch;
			set => debugSwitch = value;
		}
        [HideInInspector]
        public bool monoScriptingBackend = false;
		private bool isRendering = false;

		public void Init()
        {

			Core.HAR.Init();
            Internal.HapticDeviceManager.DeviceInitLoop();
#if UNITY_ANDROID && !ENABLE_METAQUEST
            Platforms.Mobile.GenericAndroidHapticAbstraction.Initialize();
#elif UNITY_IPHONE
            UnityCoreHaptics.UnityCoreHapticsProxy.CreateEngine();
#endif
        }

        override protected void OnAwake()
        {
#if !UNITY_EDITOR_OSX
            Init();
#endif

        }

        private void Start()
        {
            isRendering = true;
            StartCoroutine(ComputeLoop());
        }

        public IEnumerator ComputeLoop()
        {
            while (isRendering)
            {
                //Compute all haptics event
                Core.HAR.ComputeAllEvents(UnityEngine.Time.realtimeSinceStartup);
                //Insert device rendering loop here
                Internal.HapticDeviceManager.DeviceRenderLoop();
                yield return new WaitForSeconds(0.030f);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
			// TODO pause the haptic playback
			if (!hasFocus)
			{
				Core.HAR.ClearActiveEvents();
			}
		}

        private void OnApplicationPause(bool pauseStatus)
        {
			// TODO pause the haptic playback
            if (pauseStatus)
            {
				Core.HAR.ClearActiveEvents();
			}
		}

        protected override void OnOnApplicationQuit()
        {
#if !UNITY_EDITOR_OSX
            Internal.HapticDeviceManager.DeviceCleanLoop();
            Core.HAR.ClearActiveEvents();
            Core.HAR.ClearInactiveEvents();
            Core.HAR.Quit();
#endif
        }
	}

}