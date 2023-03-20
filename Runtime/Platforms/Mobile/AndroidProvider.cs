/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

#if !ENABLE_METAQUEST && UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine;
using Interhaptics.HapticBodyMapping;
using System.Collections;

namespace Interhaptics.Platforms.Android
{
    public sealed class AndroidProvider : IHapticProvider
    {
#region HAPTIC CHARACTERISTICS FIELDS
        private const string DISPLAY_NAME = "Android";
        private const string DESCRIPTION = "Android device";
        private const string MANUFACTURER = "Google";
        private const string VERSION = "1.0";
        private const int SAMPLERATE = 100;
        private ulong lastBufferStartingTime = 0;
        private bool hapticPlaying= false;
        private float? expectedEndTimestamp = null;
        private BodyPartID Hand= BodyPartID.Bp_Left_palm;
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
       
        [UnityEngine.Scripting.Preserve]
        public bool Init()
        {
            Interhaptics.Platforms.Mobile.GenericAndroidHapticAbstraction.Initialize();
            Core.HAR.AddBodyPart(Perception.Vibration, Hand, 1, 1, 1, SAMPLERATE, false, false, false, false);
            UnityEngine.Debug.Log("Android haptic provider initialised");
            return true;
        }

        [UnityEngine.Scripting.Preserve]
        public bool IsPresent()
        {
            return Interhaptics.Platforms.Mobile.GenericAndroidHapticAbstraction.HasVibrator();
        }

        [UnityEngine.Scripting.Preserve]
        public bool Clean()
        {
            return true;
        }

        [UnityEngine.Scripting.Preserve]
        public void RenderHaptics()
        {
            ulong startingTime = Core.HAR.GetVectorStartingTime(Perception.Vibration, Hand, 0, 0, 0);
            //if something to update
            if (startingTime!= lastBufferStartingTime)
            {
                lastBufferStartingTime = startingTime;
                int size = Core.HAR.GetOutputBufferSize(Perception.Vibration, Hand, 0, 0, 0, BufferDataType.Amplitude);
                if (size > 0)
                {  
                    double[] outputBuffer = new double[size];

                    //getting haptic amplitude buffer to play
                    Core.HAR.GetOutputBuffer(outputBuffer, size, Perception.Vibration, Hand, 0, 0, 0, BufferDataType.Amplitude);
                    int[] AndroidOutputBuffer = new int[size];
                    long[] timeBuffer = new long[size];
                    long step = Mathf.RoundToInt(1000f / SAMPLERATE);
                    //creating pattern array and casting haptic data into the correct range (0 <-> 255)
                    for (int i = 0;i<size;i++)
                    {
                        AndroidOutputBuffer[i] = Mathf.RoundToInt((float)(outputBuffer[i] * 255));
                        timeBuffer[i] = step;
                    }

                    //sending haptic data to device
                    Interhaptics.Platforms.Mobile.GenericAndroidHapticAbstraction.Vibrate(timeBuffer, AndroidOutputBuffer,-1,true);
                    hapticPlaying = true;
                    expectedEndTimestamp = UnityEngine.Time.realtimeSinceStartup+(size / SAMPLERATE);
                }
            }
            else 
            {
                if (hapticPlaying && UnityEngine.Time.realtimeSinceStartup> expectedEndTimestamp)
                {
                    //Stop haptic at the end of the effect
                    Interhaptics.Platforms.Mobile.GenericAndroidHapticAbstraction.Cancel();
                    hapticPlaying = false;
                
                }
            }
        }
#endregion
    }
}
#endif