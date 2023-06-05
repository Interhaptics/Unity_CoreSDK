/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/
#if ENABLE_OPENXR
using UnityEngine;
using Interhaptics.HapticBodyMapping;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;


[assembly: UnityEngine.Scripting.AlwaysLinkAssembly]
[assembly: UnityEngine.Scripting.Preserve]
namespace Interhaptics.Platforms.XR
{

    public sealed class OpenXRProvider : IHapticProvider
    {
        IH_HapticsInput m_IH_HapticsInput;
        float minimumDuration = 0.016f;
        int sampleRate = 150;

        #region HAPTIC CHARACTERISTICS FIELDS
        private const string DISPLAY_NAME = "OpenXR device";
        private const string DESCRIPTION = "XR controller for OpenXR device";
        private const string MANUFACTURER = "Unknown";
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
        [UnityEngine.Scripting.Preserve]
        public bool Init()
        {
            if (UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head) == null)
            {
                UnityEngine.Debug.Log("XR HMD not found");
                return false;
            }
            m_IH_HapticsInput = new IH_HapticsInput();

            Core.HAR.AddBodyPart(Perception.Vibration, BodyPartID.Bp_Left_palm, 1, 1, 1, sampleRate, false, false, false, true);
            Core.HAR.AddBodyPart(Perception.Vibration, BodyPartID.Bp_Right_palm, 1, 1, 1, sampleRate, false, false, false, true);
            UnityEngine.Debug.Log("OpenXR haptic provider initialised");
            return true;
        }

        [UnityEngine.Scripting.Preserve]
        public bool IsPresent()
        {
            UnityEngine.XR.HapticCapabilities caps = new UnityEngine.XR.HapticCapabilities();
            bool isPresent = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand).TryGetHapticCapabilities(out caps);
            isPresent |= UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand).TryGetHapticCapabilities(out caps);
            return isPresent;
        }

        [UnityEngine.Scripting.Preserve]
        public bool Clean()
        {
            return true;
        }

        [UnityEngine.Scripting.Preserve]
        public void RenderHaptics()
        {
            double[] outputBuffer;
            int size = Core.HAR.GetOutputBufferSize(Perception.Vibration, BodyPartID.Bp_Left_palm, 0, 0, 0, BufferDataType.PCM);
            if (size > 0)
            {
                outputBuffer = new double[size];
                Core.HAR.GetOutputBuffer(outputBuffer, size, Perception.Vibration, BodyPartID.Bp_Left_palm, 0, 0, 0, BufferDataType.PCM);
                OpenXRInput.SendHapticImpulse(m_IH_HapticsInput.HapticsXR.Left, (float)AverageFromBuffer(outputBuffer, size), minimumDuration, UnityEngine.InputSystem.XR.XRController.leftHand);
            }
            else
            {
                OpenXRInput.StopHaptics(m_IH_HapticsInput.HapticsXR.Left, UnityEngine.InputSystem.XR.XRController.leftHand);
            }

            size = Core.HAR.GetOutputBufferSize(Perception.Vibration, BodyPartID.Bp_Right_palm, 0, 0, 0, BufferDataType.PCM);
            if (size > 0)
            {
                outputBuffer = new double[size];
                Core.HAR.GetOutputBuffer(outputBuffer, size, Perception.Vibration, BodyPartID.Bp_Right_palm, 0, 0, 0, BufferDataType.PCM);
                OpenXRInput.SendHapticImpulse(m_IH_HapticsInput.HapticsXR.Right, (float)AverageFromBuffer(outputBuffer, size), minimumDuration, UnityEngine.InputSystem.XR.XRController.rightHand);
            }
            else
            {
                OpenXRInput.StopHaptics(m_IH_HapticsInput.HapticsXR.Right, UnityEngine.InputSystem.XR.XRController.rightHand);
            }
        }
        #endregion

        double AverageFromBuffer(double[] buffer, int bufferSize)
        {
            if (bufferSize <= 0)
            {
                return 0;
            }

            double average = 0;

            for (int i = 0; i < bufferSize; i++)
            {
                average += buffer[i];
            }

            return average / bufferSize;
        }

    }

}
#endif