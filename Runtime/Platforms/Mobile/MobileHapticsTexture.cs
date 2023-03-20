/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;

using System.Collections.Generic;


namespace Interhaptics.Platforms.Mobile
{

    /// <summary>
    /// Allow users to play the haptics texture from a material.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Interhaptics/Mobile/HapticsTexture")]
    public class MobileHapticsTexture : AMobileHapticsReader
    {

        #region Structures
        private struct TextureInfo
        {
            public MaterialInfos MaterialInfos;

            #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            #if UNITY_ANDROID
            public int[] subPattern;
            #elif UNITY_IOS
            public float[] subPattern;
            #endif

            public long[] timePattern;
            public int dir, oldDirection;
            public bool inSubPattern;
            #endif
            public float oldValue;
        }
        #endregion

        #region Variables
        private Dictionary<int, TextureInfo> textureMaterials = new Dictionary<int, TextureInfo>();
        #endregion

        #region Publics
        /// <summary>
        /// Compute haptics texture from a material.
        /// </summary>
        /// <param name="index">Index of the loaded material</param>
        /// <param name="distance">Current distance on the texture (real distance 1 unit = 1 meter)</param>
        public void PlayTexture(int index, float distance)
        {
            if (!textureMaterials.ContainsKey(index))
            {
                MaterialInfos materialInfos;
                if (this.GetMaterialInfos(index, out materialInfos))
                {
                    TextureInfo textureInfo = new TextureInfo();
                    textureInfo.MaterialInfos = materialInfos;

                    textureMaterials.Add(index, textureInfo);
                }
            }

            if (!textureMaterials.TryGetValue(index, out TextureInfo currentTextureInfo))
            {
                return;
            }

            if (currentTextureInfo.MaterialInfos.textureLength <= 0 || distance == currentTextureInfo.oldValue)
            {
                return;
            }

            #if UNITY_EDITOR
            Debug.Log($"{this.GetType().Name}: (<b>Material =</b> {currentTextureInfo.MaterialInfos.material.name} | <b>Value =</b> {distance} | <b>Amplitude =</b> {Interhaptics.Core.HAR.GetTextureAmp(currentTextureInfo.MaterialInfos.id, distance)}", this);
            #elif (UNITY_ANDROID || UNITY_IOS)

            if (currentTextureInfo.MaterialInfos.globalPattern == null || currentTextureInfo.MaterialInfos.globalPattern.Length <= 0)
            {
                return;
            }

            //Distance computation
            #if UNITY_ANDROID
            int start = Mathf.RoundToInt((distance / currentTextureInfo.MaterialInfos.textureLength) * (currentTextureInfo.MaterialInfos.globalPattern.Length - 2));
            #elif UNITY_IOS
            int start = Mathf.RoundToInt((distance / currentTextureInfo.MaterialInfos.textureLength) * (currentTextureInfo.MaterialInfos.globalPattern.Length - 1));
            #endif

            int end = start;

            currentTextureInfo.dir = (int)Mathf.Sign(distance - currentTextureInfo.oldValue);

            if (currentTextureInfo.oldDirection != currentTextureInfo.dir)
            {
                #if UNITY_ANDROID
                this.CancelHaptic();
                #endif
                currentTextureInfo.inSubPattern = false;
                currentTextureInfo.oldDirection = currentTextureInfo.dir;
            }

            #if UNITY_ANDROID
            if (currentTextureInfo.inSubPattern && currentTextureInfo.MaterialInfos.globalPattern[start + 1] <= 0)
            #elif UNITY_IOS
            if (currentTextureInfo.inSubPattern && currentTextureInfo.MaterialInfos.globalPattern[start] <= 0)
            #endif
            {
                #if UNITY_ANDROID
                GenericAndroidHapticAbstraction.Cancel();
                #endif
                currentTextureInfo.inSubPattern = false;
            }

            #if UNITY_ANDROID
            if (!currentTextureInfo.inSubPattern && currentTextureInfo.MaterialInfos.globalPattern[start + 1] > 0)
            #elif UNITY_IOS
            if (!currentTextureInfo.inSubPattern && currentTextureInfo.MaterialInfos.globalPattern[start] > 0)
            #endif
            {
                currentTextureInfo.inSubPattern = true;

                #if UNITY_ANDROID
                for (int i = start; i < currentTextureInfo.MaterialInfos.globalPattern.Length; i += 2 * currentTextureInfo.dir)
                {
                    if ((i + currentTextureInfo.dir < 0) || (i + currentTextureInfo.dir >= currentTextureInfo.MaterialInfos.globalPattern.Length) || (currentTextureInfo.MaterialInfos.globalPattern[i + currentTextureInfo.dir] <= 0))
                    {
                        end = i;
                        break;
                    }
                }
                #elif UNITY_IOS
                for (int i = start; i < currentTextureInfo.MaterialInfos.globalPattern.Length; i += currentTextureInfo.dir)
                {
                    if ((i < 0) || (i >= currentTextureInfo.MaterialInfos.globalPattern.Length) || (currentTextureInfo.MaterialInfos.globalPattern[i] <= 0))
                    {
                        end = i;
                        break;
                    }
                }
                #endif

                int size = Mathf.Abs(end - start);

                if (size > 0)
                {
                    #if UNITY_ANDROID
                    currentTextureInfo.subPattern = new int[size];
                    #elif UNITY_IOS
                    currentTextureInfo.subPattern = new float[size];
                    #endif
                }
                else
                {
                    return;
                }

                Debug.Log(size);

                string debug = currentTextureInfo.MaterialInfos.material.name + "\n";
                for (int k = 0; k < currentTextureInfo.subPattern.Length; k++)
                {
                    if ((start + k * currentTextureInfo.dir < 0) || (start + k * currentTextureInfo.dir >= currentTextureInfo.MaterialInfos.globalPattern.Length))
                    {
                        break;
                    }

                    currentTextureInfo.subPattern[k] = currentTextureInfo.MaterialInfos.globalPattern[start + k * currentTextureInfo.dir];
                    #if UNITY_ANDROID
                    k++;
                    currentTextureInfo.subPattern[k] = currentTextureInfo.MaterialInfos.globalPattern[start + k * currentTextureInfo.dir];
                    #endif
                    debug += currentTextureInfo.subPattern[k] + " ";
                }
                Debug.Log(debug);

                #if UNITY_ANDROID
                currentTextureInfo.timePattern = new long[currentTextureInfo.subPattern.Length];

                for (int j = 0; j < currentTextureInfo.timePattern.Length; j++)
                {
                    currentTextureInfo.timePattern[j] = 0;
                    currentTextureInfo.timePattern[++j] = Mathf.RoundToInt(PATTERN_STEP * 1000);
                }

                GenericAndroidHapticAbstraction.Vibrate(currentTextureInfo.timePattern, currentTextureInfo.subPattern);
                #elif UNITY_IOS
                UnityCoreHaptics.UnityCoreHapticsProxy.PlayHapticsFromJSON(Tools.iOSUtilities.BufferToAHAP(currentTextureInfo.subPattern, currentTextureInfo.subPattern, new float[0], new float[0], currentTextureInfo.subPattern.Length * PATTERN_STEP, AMobileHapticsReader.PATTERN_STEP));
                #endif
            }
            #endif

            currentTextureInfo.oldValue = distance;
            textureMaterials[index] = currentTextureInfo;
        }

        /// <summary>
        /// Compute haptics texture from a material.
        /// </summary>
        /// <param name="index">Index of the loaded material</param>
        /// <param name="distance01">Current distance normalized between 0 and 1</param>
        public void PlayTexture01(int index, float distance01)
        {
            if (!textureMaterials.ContainsKey(index))
            {
                MaterialInfos materialInfos;
                if (this.GetMaterialInfos(index, out materialInfos))
                {
                    TextureInfo textureInfo = new TextureInfo();
                    textureInfo.MaterialInfos = materialInfos;

                    textureMaterials.Add(index, textureInfo);
                }
            }

            if (!textureMaterials.TryGetValue(index, out TextureInfo currentTextureInfo))
            {
                return;
            }

            float distance = currentTextureInfo.MaterialInfos.textureLength * distance01;

            this.PlayTexture(index, distance);
        }
        #endregion

    }

}
