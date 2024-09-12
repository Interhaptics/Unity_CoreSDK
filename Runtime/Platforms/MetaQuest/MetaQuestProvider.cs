/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

#if ENABLE_METAQUEST && (UNITY_EDITOR || UNITY_ANDROID || UNITY_STANDALONE_WIN)
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
        
        private const int NUMBER_OF_BP = 2;
        private static readonly Perception[] PERCEPTIONS = { Perception.Vibration, Perception.Vibration };
        private static readonly BodyPartID[] BODYPARTS = { BodyPartID.Bp_Left_palm, BodyPartID.Bp_Right_palm };
        private static readonly int[] X_DIM = { 1, 1 };
        private static readonly int[] Y_DIM = { 1, 1 };
        private static readonly int[] Z_DIM = { 1, 1 };
        private static readonly double[] SAMPLERATE = { 500, 500 };
        private static readonly bool[] HD = { false, false };
        private static readonly bool[] SPLIT_FREQUENCY = { false, false };
        private static readonly bool[] SPLIT_TRANSIENTS = { false, false };
        private static readonly EProtocol[] PROTOCOL = { EProtocol.PCM, EProtocol.PCM };

        private int myAvatarID = -1;
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

            myAvatarID = Core.HAR.CreateBodyParts(2, PERCEPTIONS, BODYPARTS, X_DIM, Y_DIM, Z_DIM, SAMPLERATE, HD, SPLIT_FREQUENCY, SPLIT_TRANSIENTS, PROTOCOL);
            if (HapticManager.DebugSwitch)
            {
            UnityEngine.Debug.Log("Meta Quest haptic provider started.");
            }
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
            int size = Core.HAR.GetOutputBufferSize(myAvatarID, Perception.Vibration, BodyPartID.Bp_Left_palm, 0, 0, 0, BufferDataType.PCM);
            if (size > 0)
            {
                outputBuffer = new double[size];
                Core.HAR.GetOutputBuffer(outputBuffer, size, myAvatarID, Perception.Vibration, BodyPartID.Bp_Left_palm, 0, 0, 0, BufferDataType.PCM);
                UnityXRHapticAbstraction.VibrateLeft(Time.realtimeSinceStartup - Time.time, outputBuffer);
            }
            else
            {
                UnityXRHapticAbstraction.VibrateLeft(Time.realtimeSinceStartup - Time.time, null);
            }

            size = Core.HAR.GetOutputBufferSize(myAvatarID, Perception.Vibration, BodyPartID.Bp_Right_palm, 0, 0, 0, BufferDataType.PCM);
            if (size > 0)
            {
                outputBuffer = new double[size];
                Core.HAR.GetOutputBuffer(outputBuffer, size, myAvatarID, Perception.Vibration, BodyPartID.Bp_Right_palm, 0, 0, 0, BufferDataType.PCM);
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