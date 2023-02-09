/* ​
* Copyright © 2022 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.IO;

using Interhaptics.HapticBodyMapping;


namespace Interhaptics.Platforms.Dummy
{

    public sealed class CSVProvider : IHapticProvider
    {

        #region HAPTIC CHARACTERISTICS FIELDS
        private const string DISPLAY_NAME = "CSV generator";
        private const string DESCRIPTION = "Generate a CSV file with output";
        private const string MANUFACTURER = "Interhaptics";
        private const string VERSION = "1.0";
        #endregion


        #region HAPTIC CHARACTERISTICS GETTERS
        public string DisplayName()
        {
            return DISPLAY_NAME;
        }

        public string Description()
        {
            return DESCRIPTION;
        }
        public string Manufacturer()
        {
            return MANUFACTURER;
        }
        public string Version()
        {
            return VERSION;
        }
        #endregion


        #region PROVIDER LOOP
        public bool Init()
        {
#if UNITY_EDITOR
            if (!sourceFromCpp)
            {
                file = new StreamWriter("CSVOutput.csv", append: true);
            }
            Core.HAR.AddBodyPart(Perception.Vibration, BodyPartID.Bp_Left_palm, 1, 1, 1, 48000, true, false, false);
            Core.HAR.AddBodyPart(Perception.Vibration, BodyPartID.Bp_Right_palm, 1, 1, 1, 48000, true, false, false);
            return true;
#else
            return false;
#endif
        }

        public bool IsPresent()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        public bool Clean()
        {
#if UNITY_EDITOR
            if (!sourceFromCpp)
            {
                file.Close();
            }
#endif
            return true;
        }

        public void RenderHaptics()
        {
#if UNITY_EDITOR

            double[] outputBuffer;
            double[] halfBuffer;
            int size = Core.HAR.GetOutputBufferSize(Perception.Vibration, BodyPartID.Bp_Left_palm, 0, 0, 0, BufferDataType.PCM);
            if (size > 0)
            {
                outputBuffer = new double[size];
                Core.HAR.GetOutputBuffer(outputBuffer, size, Perception.Vibration, BodyPartID.Bp_Left_palm, 0, 0, 0, BufferDataType.PCM);

                halfBuffer = new double[size/* / 2*/];
                System.Array.Copy(outputBuffer, 0, halfBuffer, 0, halfBuffer.Length);
                WriteToFile(halfBuffer);
            }

            size = Core.HAR.GetOutputBufferSize(Perception.Vibration, BodyPartID.Bp_Right_palm, 0, 0, 0, BufferDataType.PCM);
            if (size > 0)
            {
                //outputBuffer = new double[size];
                //HAR.GetOutputBuffer(outputBuffer, size, Perception.Vibration, BodyPartID.Bp_Right_palm, 0, 0, 0, BufferDataType.PCM);
                //halfBuffer = new double[size / 2];
                //System.Array.Copy(outputBuffer, 0, halfBuffer, 0, halfBuffer.Length);
                //WriteToFile(halfBuffer);
            }
#endif
        }
        #endregion


        private StreamWriter file;
        private bool sourceFromCpp = false;

        private void WriteToFile(double[] buffer)
        {
            for (int i = 0; i < buffer.Length/2; i++)
            {
                string s = buffer[i] + "\n";
                file.Write(s);
            }
        }

    }

}