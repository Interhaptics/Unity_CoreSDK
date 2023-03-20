/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.Collections.Generic;

using Interhaptics.HapticBodyMapping;


namespace Interhaptics.Core
{

    public static partial class HAR
    {

        #region Enums
        public enum HMaterial_VersionStatus
        {
            NoAnHapticsMaterial = 0,
            V3_NeedToBeReworked = 1,
            V4_Current = 2,
            UnknownVersion = 3
        }
        #endregion

        #region HM Managment
        private static string parseMaterial(UnityEngine.TextAsset _material)
        {
            if (_material == null)
            {
                return "";
            }
            return _material.text;
        }

        private static string parseMaterial(HapticMaterial _material)
        {
            if (_material == null)
            {
                return "";
            }
            return _material.text;
        }

        public static int AddHM(UnityEngine.TextAsset _material)
        {
            return AddHM(parseMaterial(_material));
        }

        public static int AddHM(HapticMaterial _material)
        {
            return AddHM(parseMaterial(_material));
        }

        public static bool UpdateHM(int _id, UnityEngine.TextAsset _material)
        {
            return UpdateHM(_id, parseMaterial(_material));
        }

        public static bool UpdateHM(int _id, HapticMaterial _material)
        {
            return UpdateHM(_id, parseMaterial(_material));
        }
        #endregion

        public static void AddTargetToEvent(int _hMaterialId, List<CommandData> _target)
        {
            AddTargetToEventMarshal(_hMaterialId, _target.ToArray(), _target.Count);
        }

        public static void UpdateEventPositions(int _hMaterialId, List<CommandData> _target, double _texturePosition, double _stiffnessPosition)
        {
            UpdateEventPositionsMarshal(_hMaterialId, _target.ToArray(), _target.Count, _texturePosition, _stiffnessPosition);
        }

        public static void RemoveTargetFromEvent(int _hMaterialId, List<CommandData> _target)
        {
            RemoveTargetFromEventMarshal(_hMaterialId, _target.ToArray(), _target.Count);
        }

    }

}
