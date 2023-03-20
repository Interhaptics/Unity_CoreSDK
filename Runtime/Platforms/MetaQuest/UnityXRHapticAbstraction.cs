/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

namespace Interhaptics.Platforms.XR
{

    internal static class UnityXRHapticAbstraction
    {

        internal static void VibrateBoth(float seconds, double[] amplitude)
        {
            seconds = UnityEngine.Mathf.Max(UnityEngine.Time.fixedDeltaTime, seconds);
            System.Collections.Generic.List<UnityEngine.XR.XRNodeState> nodeStates = new System.Collections.Generic.List<UnityEngine.XR.XRNodeState>();
            UnityEngine.XR.InputTracking.GetNodeStates(nodeStates);
            foreach (UnityEngine.XR.XRNodeState nodeState in nodeStates)
            {
                if (nodeState.nodeType == UnityEngine.XR.XRNode.LeftHand)
                {
                    VibrateLeft(seconds, amplitude);
                }
                else if (nodeState.nodeType == UnityEngine.XR.XRNode.RightHand)
                {
                    VibrateRight(seconds, amplitude);
                }
            }
        }

        internal static bool VibrateLeft(float seconds, double[] amplitude)
        {
            seconds = UnityEngine.Mathf.Max(UnityEngine.Time.fixedDeltaTime, seconds);
            return VibrateXRNode(seconds, UnityEngine.XR.XRNode.LeftHand, amplitude);
        }

        internal static bool VibrateRight(float seconds, double[] amplitude)
        {
            seconds = UnityEngine.Mathf.Max(UnityEngine.Time.fixedDeltaTime, seconds);
            return VibrateXRNode(seconds, UnityEngine.XR.XRNode.RightHand, amplitude);
        }

        /// <summary>
        /// Vibrate controller at the Node
        /// </summary>
        /// <param name="seconds">Duration of signal \remark Oculus Touch Only</param>
        /// <param name="node">XRNode.RightHand or XRNode.LeftHand</param>
        /// <param name="amplitude">Amplitude of pulse</param>
        /// <returns></returns>
        internal static bool VibrateXRNode(float seconds, UnityEngine.XR.XRNode node, double[] amplitude)
        {
            UnityEngine.XR.HapticCapabilities caps = new UnityEngine.XR.HapticCapabilities();
            if (!UnityEngine.XR.InputDevices.GetDeviceAtXRNode(node).TryGetHapticCapabilities(out caps))
            {
                UnityEngine.Debug.LogWarning("HAR ERROR!: " + node.ToString() + " doesn't support haptic feedback or not connected to your system!");
                return false;
            }

            if (caps.supportsBuffer)
            {
                byte[] clip = { };
                return GenerateHapticClip(seconds, node, ref clip, amplitude) ? UnityEngine.XR.InputDevices.GetDeviceAtXRNode(node).SendHapticBuffer(0, clip) : false;
            }
            else if (caps.supportsImpulse)
            {
                return UnityEngine.XR.InputDevices.GetDeviceAtXRNode(node).SendHapticImpulse(0, (float)PulseFromBuffer(amplitude), seconds);
            }
            else
            {
                UnityEngine.Debug.LogWarning("HAR ERROR!: " + node.ToString() + " doesn't support buffer or impulse!");
                return false;
            }
        }

        internal static double PulseFromBuffer(double[] _buffer)
        {
            if ((_buffer == null) || (_buffer.Length == 0))
            {
                return 0;
            }

            double result = 0;
            for (int i = 0; i < _buffer.Length; i++)
            {
                result += _buffer[i];
            }

            return UnityEngine.Mathf.Clamp((float)(result / _buffer.Length), 0, 1);
        }


        //Generates a haptic clip of the proper size from an input buffer
        internal static bool GenerateHapticClip(float seconds, UnityEngine.XR.XRNode node, ref byte[] clip, double[] amplitudes)
        {
            UnityEngine.XR.HapticCapabilities caps = new UnityEngine.XR.HapticCapabilities();

            if (!UnityEngine.XR.InputDevices.GetDeviceAtXRNode(node).TryGetHapticCapabilities(out caps))
            {
                return false;
            }

            //create clips with proper size
            int clipCount = (int)((double)caps.bufferFrequencyHz * seconds);
            clip = new byte[clipCount];

            if (clip.Length <= 0)
            {
                return false;
            }

            if (clip.Length == 1)
            {
                clip[0] = (byte)(PulseFromBuffer(amplitudes) * (double)byte.MaxValue);
            }
            else
            {
                double i = 0;
                int j = 0;

                //fill clip with smoothed values
                for (i = 0, j = 0; UnityEngine.Mathf.CeilToInt((float)i) < amplitudes.Length && j < clip.Length; i += (amplitudes.Length - 1.0f) / (clip.Length - 1.0f), j++)
                {
                    double a = amplitudes[UnityEngine.Mathf.FloorToInt((float)i)]
                            + (j * (amplitudes.Length - 1.0f) / (clip.Length - 1.0f) - UnityEngine.Mathf.FloorToInt((float)i))
                            * (amplitudes[UnityEngine.Mathf.CeilToInt((float)i)] - amplitudes[UnityEngine.Mathf.FloorToInt((float)i)]);
                    clip[j] = (byte)(a * (double)byte.MaxValue);
                }
            }

            return true;
        }
    }
}
