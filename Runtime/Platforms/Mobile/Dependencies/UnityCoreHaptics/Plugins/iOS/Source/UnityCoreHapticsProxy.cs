using System.Runtime.InteropServices;
using System;
using System.IO;
using UnityEngine;

namespace UnityCoreHaptics
{
    public static class UnityCoreHapticsProxy
    {
#if UNITY_IOS && !UNITY_EDITOR
            [DllImport("__Internal")]
            private static extern void _coreHapticsCreateEngine();

            [DllImport("__Internal")]
            private static extern void _coreHapticsStopEngine();

            [DllImport("__Internal")]
            private static extern bool _coreHapticsSupportsCoreHaptics();

            [DllImport("__Internal")]
            private static extern void _coreHapticsPlayTransientHaptic(float intensity, float sharpness);
            
            [DllImport("__Internal")]
            private static extern void _coreHapticsPlayContinuousHaptic(float intensity, float sharpness, float duration);

            [DllImport("__Internal")]
            private static extern void _coreHapticsPlayHapticsFromJSON(string str);

            [DllImport("__Internal")]
            private static extern void _coreHapticsPlayHapticsFromFile(string path);

            // Callbacks
            [DllImport("__Internal")]
            private static extern void _coreHapticsRegisterEngineCreated(Action callback);

            [DllImport("__Internal")]
            private static extern void _coreHapticsRegisterEngineError(Action callback);

            public static event Action OnEngineCreated;
            public static event Action OnEngineError;
#endif

        /// <summary>
        /// In the constructor, set up callbacks
        /// </summary>
        static UnityCoreHapticsProxy()
        {
            #if !UNITY_2019_3_OR_NEWER
                throw new Exception("[UnityCoreHaptics] plugin is only supported in Unity 2019.3 or later.");
            #endif
            #if UNITY_IOS && !UNITY_EDITOR
                _coreHapticsRegisterEngineCreated(OnEngineCreated);
                _coreHapticsRegisterEngineError(OnEngineError);
            #endif
        }

        /// <summary>
        /// Create engine to play haptics. It is recommended to create an engine before you
        /// actually play haptic effects. If you don't, an engine will automatically be
        /// created when you play haptic effects and may cause a frame spike or 
        /// performance issues.
        /// </summary>
        public static void CreateEngine() {
            #if UNITY_IOS && !UNITY_EDITOR
                _coreHapticsCreateEngine();
            #endif
        }

        /// <summary>
        /// Stops the haptic engine
        /// </summary>
        public static void StopEngine() {
            #if UNITY_IOS && !UNITY_EDITOR
                _coreHapticsStopEngine();
            #endif
        }


        /// <summary>
        /// Checks whether the hardware supports core haptics, which is available
        /// in iOS 13+ and on select iPhone devices only
        /// </summary>
        /// <returns></returns>
        public static bool SupportsCoreHaptics()
        {
            #if UNITY_IOS && !UNITY_EDITOR
                return _coreHapticsSupportsCoreHaptics();
            #else
                return false;
            #endif
        }

        /// <summary>
        /// Play a one-time haptic effect
        /// </summary>
        /// <param name="intensity">Intensity of haptic effect from 0 to 1</param>
        /// <param name="sharpness">Sharpness of haptic effect from 0 to 1</param>
        public static void PlayTransientHaptics(float intensity, float sharpness)
        {
            #if UNITY_IOS && !UNITY_EDITOR
                _coreHapticsPlayTransientHaptic(intensity, sharpness);
            #endif
        }

        /// <summary>
        /// Play a continuous haptic effect
        /// </summary>
        /// <param name="intensity">Intensity of haptic effect from 0 to 1</param>
        /// <param name="sharpness">Sharpness of haptic effect from 0 to 1</param>
        /// <param name="duration">Duration in seconds to play haptic effect</param>
        public static void PlayContinuousHaptics(float intensity, float sharpness, float duration)
        {
            #if UNITY_IOS && !UNITY_EDITOR
                _coreHapticsPlayContinuousHaptic(intensity, sharpness, duration);
            #endif
        }

        /// <summary>
        /// Play haptics using text in AHAP format. Does not support playing
        /// audio, if that is part of the AHAP text.
        /// </summary>
        /// <param name="str">AHAP text content (not file name)</param>
        public static void PlayHapticsFromJSON(string str)
        {
            #if UNITY_IOS && !UNITY_EDITOR
                _coreHapticsPlayHapticsFromJSON(str);
            #endif
        }

        /// <summary>
        /// Plays haptics from an AHAP file stored in the StreamingAssets folder
        /// See: https://docs.unity3d.com/Manual/StreamingAssets.html for more details on StreamingAssets
        /// </summary>
        /// <param name="path">Path to an AHAP file relative to the StreamingAssets folder</param>
        public static void PlayHapticsFromFile(string pathFromStreamingAssets)
        {
            string fullPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, pathFromStreamingAssets);
            #if UNITY_IOS && !UNITY_EDITOR
                _coreHapticsPlayHapticsFromFile(fullPath);
            #endif
        }
    }
}
