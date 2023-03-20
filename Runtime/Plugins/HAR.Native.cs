/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.Runtime.InteropServices;


namespace Interhaptics.Core
{

    public static partial class HAR
    {

        #if UNITY_IOS && !UNITY_EDITOR
        const string DLL_NAME = "__Internal";
        #else
        const string DLL_NAME = "HAR";
        #endif

        [DllImport(DLL_NAME)]
        public static extern bool Init();
        [DllImport(DLL_NAME)]
        public static extern void Quit();
        [DllImport(DLL_NAME)]
        private static extern int AddHM(string _content);
        [DllImport(DLL_NAME)]
        private static extern bool UpdateHM(int _id, string _content);

        [DllImport(DLL_NAME)]
        public static extern double GetVibrationAmp(int _id, double _step, int _mode = 0);
        [DllImport(DLL_NAME)]
        public static extern double GetVibrationFreq(int _id, double _step);
        [DllImport(DLL_NAME)]
        public static extern double GetVibrationLength(int _id);
        [DllImport(DLL_NAME)]
        public static extern double GetTextureAmp(int _id, double _step, int _mode = 0);
        [DllImport(DLL_NAME)]
        public static extern double GetTextureFreq(int _id, double _step);
        [DllImport(DLL_NAME)]
        public static extern double GetTextureLength(int _id);
        [DllImport(DLL_NAME)]
        public static extern double GetStiffnessAmp(int _id, double _step);
        [DllImport(DLL_NAME)]
        public static extern double GetStiffnessFreq(int _id, double _step);
        [DllImport(DLL_NAME)]
        public static extern bool GetTransientsData(int _hMaterialId, int _perceptionIndex, double _startingTime, double _length, int _tabSize, double[] _timer, double[] _volume, double[] _frequency, int[] _type);
        [DllImport(DLL_NAME)]
        public static extern int GetNumberOfTransient(int _hMaterialId, int _perceptionIndex, double _startingTime, double _length);

        [DllImport(DLL_NAME)]
        public static extern int GetOutputBufferSize(Interhaptics.HapticBodyMapping.Perception _perception, Interhaptics.HapticBodyMapping.BodyPartID _bodyPartID, int _x, int _y, int _z, Interhaptics.HapticBodyMapping.BufferDataType _dataType);
        [DllImport(DLL_NAME)]
        public static extern void GetOutputBuffer(double[] _outputBuffer, int _bufferSize, Interhaptics.HapticBodyMapping.Perception _perception, Interhaptics.HapticBodyMapping.BodyPartID _bodyPartID, int _x, int _y, int _z, Interhaptics.HapticBodyMapping.BufferDataType _dataType);

        [DllImport(DLL_NAME)]
        public static extern ulong GetVectorStartingTime(Interhaptics.HapticBodyMapping.Perception _perception, Interhaptics.HapticBodyMapping.BodyPartID _bodyPartID, int _xDimension, int _yDimension, int _zDimension);
        
        [DllImport(DLL_NAME)]
        public static extern void AddBodyPart(Interhaptics.HapticBodyMapping.Perception _perception, Interhaptics.HapticBodyMapping.BodyPartID _bodyPartID, int _xDimension, int _yDimension, int _zDimension, double _sampleRate, bool _hd, bool _splitFrequency, bool _splitTransient, bool realTime);
        [DllImport(DLL_NAME)]
        public static extern void DeleteBodyPart(Interhaptics.HapticBodyMapping.Perception _perception, Interhaptics.HapticBodyMapping.BodyPartID _bodyPartID);

        [DllImport(DLL_NAME)]
        public static extern void PlayEvent(int _hMaterialId, double _vibrationOffset, double _textureOffset, double _stiffnessOffset);
        [DllImport(DLL_NAME)]
        public static extern void StopEvent(int _hMaterialId);
        [DllImport(DLL_NAME)]
        public static extern void ClearActiveEvents();
        [DllImport(DLL_NAME)]
        public static extern void ClearInactiveEvents();
        [DllImport(DLL_NAME)]
        public static extern void ClearEvent(int _hMaterialId);
        [DllImport(DLL_NAME)]
        public static extern void SetEventOffsets(int _hMaterialId, double _vibrationOffset, double _textureOffset, double _stiffnessOffset);
        [DllImport(DLL_NAME)]
        private static extern void UpdateEventPositionsMarshal(int _hMaterialId, Interhaptics.HapticBodyMapping.CommandData[] _target, int _size, double _texturePosition, double _stiffnessPosition);
        [DllImport(DLL_NAME)]
        public static extern void ComputeAllEvents(double _curTime);

        [DllImport(DLL_NAME)]
        private static extern void AddTargetToEventMarshal(int _hMaterialId, Interhaptics.HapticBodyMapping.CommandData[] _target, int _size);
        [DllImport(DLL_NAME)]
        private static extern void RemoveTargetFromEventMarshal(int _hMaterialId, Interhaptics.HapticBodyMapping.CommandData[] _target, int _size);
        [DllImport(DLL_NAME)]
        public static extern void RemoveAllTargetsFromEvent(int _hMaterialId);

    }

}