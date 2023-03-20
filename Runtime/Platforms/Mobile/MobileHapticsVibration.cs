/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;

namespace Interhaptics.Platforms.Mobile
{
 
    /// <summary>
    /// Allow the user to play the haptics vibration from a material.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Interhaptics/Mobile/HapticsVibration")]
    public class MobileHapticsVibration : AMobileHapticsReader
    {
    
        #region Publics
        /// <summary>
        /// Play the vibration from a material.
        /// </summary>
        /// <param name="index">Index of the loaded material</param>
        public virtual void PlayVibration(int index)
        {
            if (this.GetMaterialInfos(index, out MaterialInfos materialInfos))
            {
                #if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)
                Debug.Log($"{this.GetType().Name}: (<b>Material =</b> {materialInfos.material.name}", this);
                #elif UNITY_ANDROID
                GenericAndroidHapticAbstraction.Vibrate(materialInfos.timePattern, materialInfos.pattern);
                #elif UNITY_IOS
                UnityCoreHaptics.UnityCoreHapticsProxy.PlayHapticsFromJSON(Tools.iOSUtilities.BufferToAHAP(materialInfos.pattern, materialInfos.FreqPattern,materialInfos.TransientVibTimer,materialInfos.TransientVibGain, materialInfos.vibrationLength, AMobileHapticsReader.PATTERN_STEP));
                #endif
            }
        }

        #endregion

    }

}