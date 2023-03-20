/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

#if ENABLE_METAQUEST && (UNITY_EDITOR || UNITY_ANDROID)
using UnityEngine;

using Interhaptics.HapticBodyMapping;


[assembly: UnityEngine.Scripting.AlwaysLinkAssembly]
[assembly: UnityEngine.Scripting.Preserve]
namespace Interhaptics.Platforms.XR
{

    public sealed class MetaQuestProvider : IHapticProvider
    {

#region HAPTIC CHARACTERISTICS FIELDS
        private const string DISPLAY_NAME = "Meta Quest";
        private const string DESCRIPTION = "XR controller for Meta Quest";
        private const string MANUFACTURER = "Meta";
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
                return false;
            }

            Core.HAR.AddBodyPart(Perception.Vibration, BodyPartID.Bp_Left_palm, 1, 1, 1, 500, false, false, false, true);
            Core.HAR.AddBodyPart(Perception.Vibration, BodyPartID.Bp_Right_palm, 1, 1, 1, 500, false, false, false, true);
            UnityEngine.Debug.Log("Meta Quest haptic provider initialised");
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
                UnityXRHapticAbstraction.VibrateLeft(Time.realtimeSinceStartup - Time.time, outputBuffer);
            }
            else
            {
                UnityXRHapticAbstraction.VibrateLeft(Time.realtimeSinceStartup - Time.time, null);
            }

            size = Core.HAR.GetOutputBufferSize(Perception.Vibration, BodyPartID.Bp_Right_palm, 0, 0, 0, BufferDataType.PCM);
            if (size > 0)
            {
                outputBuffer = new double[size];
                Core.HAR.GetOutputBuffer(outputBuffer, size, Perception.Vibration, BodyPartID.Bp_Right_palm, 0, 0, 0, BufferDataType.PCM);
                UnityXRHapticAbstraction.VibrateRight(Time.realtimeSinceStartup - Time.time, outputBuffer);
            }
            else
            {
                UnityXRHapticAbstraction.VibrateRight(Time.realtimeSinceStartup - Time.time, null);
            }
        }
#endregion

    }

}
#endif