/* ​
* Copyright © 2022 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using Interhaptics.Platforms.Mobile;

namespace Interhaptics
{

    [UnityEngine.AddComponentMenu("Interhaptics/HapticManager")]
    public sealed class HapticManager : Internal.Singleton<HapticManager>
    {

        public void Init()
        {
            Core.HAR.Init();
            Internal.HapticDeviceManager.DeviceInitLoop();
#if UNITY_ANDROID && !ENABLE_METAQUEST
            GenericAndroidHapticAbstraction.Initialize();
            GenericAndroidHapticAbstraction.m_timer = 30;
            GenericAndroidHapticAbstraction.pulse = 30;
            GenericAndroidHapticAbstraction.m_last_time = UnityEngine.Time.fixedTime;
#elif UNITY_IPHONE
            UnityCoreHaptics.UnityCoreHapticsProxy.CreateEngine();
#endif
        }

        override protected void OnAwake()
        {
            Init();
        }

        private void LateUpdate()
        {
#if !UNITY_IOS //TODO make sure it does not run on mobile ? Also we will change mobile to use that so idk
            //Compute all haptics event
            Core.HAR.ComputeAllEvents(UnityEngine.Time.realtimeSinceStartup);

            //Insert device rendering loop here
            Internal.HapticDeviceManager.DeviceRenderLoop();
#endif
        }

        protected override void OnOnApplicationQuit()
        {
            Internal.HapticDeviceManager.DeviceCleanLoop();
            Core.HAR.ClearActiveEvents();
            Core.HAR.ClearInactiveEvents();
            Core.HAR.Quit();
        }

    }

}