/* ​
* Copyright © 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN) && (!ENABLE_METAQUEST) && (!UNITY_ANDROID)
using System.Runtime.InteropServices;


namespace Interhaptics.Platforms.XInput
{

    public sealed class XInputProvider : IHapticProvider
    {

        #region HAPTIC CHARACTERISTICS FIELDS
        private const string DISPLAY_NAME = "XInput";
        private const string DESCRIPTION = "Controller APIs communication layer for XInput";
        private const string MANUFACTURER = "Microsoft";
        private const string VERSION = "1.0";
        #endregion

        #region HAPTIC CHARACTERISTICS GETTERS
        [UnityEngine.Scripting.Preserve]
        public string DisplayName()
        {
            return DISPLAY_NAME;
        }

        [UnityEngine.Scripting.Preserve]
        public string Description()
        {
            return DESCRIPTION;
        }

        [UnityEngine.Scripting.Preserve]
        public string Manufacturer()
        {
            return MANUFACTURER;
        }

        [UnityEngine.Scripting.Preserve]
        public string Version()
        {
            return VERSION;
        }
        #endregion

        #region PROVIDER LOOP
        private static class XInputProviderNative
        {
            const string DLL_NAME = "Interhaptics.XInputProvider";

            [DllImport(DLL_NAME)]
            public static extern bool IHProviderXInput_Init();
            [DllImport(DLL_NAME)]
            public static extern bool IHProviderXInput_IsPresent();
            [DllImport(DLL_NAME)]
            public static extern bool IHProviderXInput_Clean();
            [DllImport(DLL_NAME)]
            public static extern void IHProviderXInput_RenderHaptics();
        }

        [UnityEngine.Scripting.Preserve]
        public bool Init()
        {
            bool res = XInputProviderNative.IHProviderXInput_Init();
            if (res)
            {
                UnityEngine.Debug.Log("XInput haptic provider initialised");
            }
            return res;
        }

        [UnityEngine.Scripting.Preserve]
        public bool IsPresent()
        {
            return XInputProviderNative.IHProviderXInput_IsPresent();
        }

        [UnityEngine.Scripting.Preserve]
        public bool Clean()
        {
            return XInputProviderNative.IHProviderXInput_Clean();
        }

        [UnityEngine.Scripting.Preserve]
        public void RenderHaptics()
        {
            XInputProviderNative.IHProviderXInput_RenderHaptics();
        }
        #endregion

    }

}
#endif //  (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN) && !ENABLE_METAQUEST