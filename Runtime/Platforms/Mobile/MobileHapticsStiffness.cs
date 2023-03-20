/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;


namespace Interhaptics.Platforms.Mobile
{

    /// <summary>
    /// Allow users to play real-time haptics from a material.
    /// </summary>
    /// <remarks>The current script only works with IOS</remarks>
    [DisallowMultipleComponent]
    [AddComponentMenu("Interhaptics/Mobile/HapticsStiffness")]
    public class MobileHapticsStiffness : AMobileHapticsReader
    {

        #region Constantes
        #if !UNITY_EDITOR && UNITY_IOS
        private const float VALUE_VibrationTime = 0.1f;
        #endif

        private HapticsMaterialTuple? _currentMaterial = default;
        #endregion

        #region Publics
        /// <summary>
        /// Compute real-time haptics from a material.
        /// </summary>
        /// <param name="index">Index of the loaded material</param>
        /// <param name="distance">From 0 to 1</param>
        public void PlayStiffness(int index, float distance)
        {
            if (_currentMaterial == null || index != _currentMaterial.Value.index)
            {
                if (this.GetMaterialInfos(index, out MaterialInfos materialInfos))
                {
                    _currentMaterial = new HapticsMaterialTuple
                    {
                        index = index,
                        materialInfos = materialInfos
                    };
                }
            }

            if (_currentMaterial == null)
            {
                return;
            }

            float stiffnessAmplitude = (float)Core.HAR.GetStiffnessAmp(_currentMaterial.Value.materialInfos.id, distance);

            #if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)
            Debug.Log($"{this.GetType().Name}: (<br>Material =</br> {_currentMaterial.Value.materialInfos.material.name} | <br>Value =</br> {distance} | <br>Amplitude =</br> {stiffnessAmplitude}）", this);
            #elif UNITY_IOS
            UnityCoreHaptics.UnityCoreHapticsProxy.PlayContinuousHaptics(stiffnessAmplitude, 1, VALUE_VibrationTime);
            #endif
        }
        #endregion

    }

}
