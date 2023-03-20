/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

#if UNITY_IOS && !UNITY_EDITOR

using UnityEngine;
using Interhaptics.HapticBodyMapping;
using Interhaptics.Platforms.Mobile.Tools;
using System;

namespace Interhaptics.Platforms.IOS
{

    public sealed class IOSProvider : IHapticProvider
    {
#region HAPTIC CHARACTERISTICS FIELDS
        private const string DISPLAY_NAME = "iOS";
        private const string DESCRIPTION = "iOS device";
        private const string MANUFACTURER = "Apple";
        private const string VERSION = "1.0";
        private const int SAMPLERATE = 100;
        private ulong lastBufferStartingTime = 0;
        private BodyPartID hand = BodyPartID.Bp_Left_palm;
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
            Core.HAR.AddBodyPart(Perception.Vibration, hand, 1, 1, 1, SAMPLERATE, true, true, true, false);
            UnityEngine.Debug.Log("iOS haptic provider initialised");
            return true;
        }

        [UnityEngine.Scripting.Preserve]
        public bool IsPresent()
        {
            return true;
        }

        [UnityEngine.Scripting.Preserve]
        public bool Clean()
        {
            return true;
        }

        [UnityEngine.Scripting.Preserve]
        public void RenderHaptics()
        {
            ulong startingTime = Core.HAR.GetVectorStartingTime(Perception.Vibration, hand, 0, 0, 0);
            //if something to update
            if (startingTime == lastBufferStartingTime)
            {
                return;
            }

            UnityCoreHaptics.UnityCoreHapticsProxy.StopEngine();
            lastBufferStartingTime = startingTime;
            double[] outputBufferVibrationAmplitude = null;
            double[] outputBufferVibrationFrequency = null;
            double[] outputBufferVibrationTransient = null;

            int sizeAmpVibration = Core.HAR.GetOutputBufferSize(Perception.Vibration, hand, 0, 0, 0, BufferDataType.Amplitude);

            if (sizeAmpVibration <= 0)
            {
                return;
            }


            //if amplitude buffer not null

            int sizeFreqVibration = Core.HAR.GetOutputBufferSize(Perception.Vibration, hand, 0, 0, 0, BufferDataType.Frequency);
            int sizeTransientVibration = Core.HAR.GetOutputBufferSize(Perception.Vibration, hand, 0, 0, 0, BufferDataType.Transient);

            //getting amplitude haptic data
            outputBufferVibrationAmplitude = new double[sizeAmpVibration];
            Core.HAR.GetOutputBuffer(outputBufferVibrationAmplitude, sizeAmpVibration, Perception.Vibration, hand, 0, 0, 0, BufferDataType.Amplitude);

            //getting frequency haptic data
            outputBufferVibrationFrequency = new double[sizeFreqVibration];
            Core.HAR.GetOutputBuffer(outputBufferVibrationFrequency, sizeFreqVibration, Perception.Vibration, hand, 0, 0, 0, BufferDataType.Frequency);

            //geting transients haptic data
            outputBufferVibrationTransient = new double[sizeTransientVibration];
            Core.HAR.GetOutputBuffer(outputBufferVibrationTransient, sizeTransientVibration, Perception.Vibration, hand, 0, 0, 0, BufferDataType.Transient);

            double EffectLength = sizeAmpVibration / (float)SAMPLERATE;

            //sending haptic data to the device
            UnityCoreHaptics.UnityCoreHapticsProxy.PlayHapticsFromJSON(iOSUtilities.BufferToAHAP(outputBufferVibrationAmplitude, outputBufferVibrationFrequency, outputBufferVibrationTransient, EffectLength, (1.0f / (float)SAMPLERATE)));
        }
    }
#endregion
}
#endif