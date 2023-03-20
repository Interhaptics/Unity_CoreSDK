/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;
using UnityCoreHaptics;

using System.Collections.Generic;
using System;

using Interhaptics.Core;


namespace Interhaptics.Platforms.Mobile
{
    /// <summary>
    /// The abstract class that implements the haptics material reader methods.
    /// </summary>
    public abstract class AMobileHapticsReader : MonoBehaviour
    {
        
        #region Constants
        #if UNITY_ANDROID
        public const float PATTERN_STEP = 0.005f;
        #elif UNITY_IOS
        public const float PATTERN_STEP = 0.001f;
        #endif

        //TODO unite PATTERN_STEP

        private const string ERROR_NullMaterial = "Null material";
        private const string ERROR_CorruptedMaterial = "Corrupted material";
        #endregion

        #region Structures
        /// <summary>
        /// Structure to store the materials data.
        /// </summary>
        [Serializable]
        public struct MaterialInfos
        {
        
            /// <summary>
            /// The materials' stored ID (in the list of loaded materials).
            /// </summary>
            public int id;

            /// <summary>
            /// Material file.
            /// </summary>
            public HapticMaterial material;

            //Length
            /// <summary>
            /// Length of the vibration.
            /// </summary>
            public float vibrationLength;

            /// <summary>
            /// Length of the texture.
            /// </summary>
            public float textureLength;

            #if UNITY_ANDROID
            //Vibration
            /// <summary>
            /// Android pattern.
            /// </summary>
            public int[] pattern;
            public long[] timePattern;

            //Texture
            /// <summary>
            /// Android global pattern.
            /// </summary>
            public int[] globalPattern;
            #elif UNITY_IOS
            /// <summary>
            /// IOS pattern.
            /// </summary>
            public float[] pattern;

            /// <summary>
            /// IOS pattern.
            /// </summary>
            public float[] FreqPattern;

            public float[] TransientVibTimer;
            public float[] TransientVibGain;

            public float[] TransientFreqTimer;
            public float[] TransientFreqGain;            

            /// <summary>
            /// IOS global pattern.
            /// </summary>
            public float[] globalPattern;

            /// <summary>
            /// IOS global pattern.
            /// </summary>
            public float[] globalFreqPattern;
            #endif
        }

        [Serializable]
        protected struct HapticsMaterialTuple
        {
            /// <summary>
            /// The index of the material in the list of loaded materials.
            /// </summary>
            public int index;

            /// <summary>
            /// The corresponding material's data.
            /// </summary>
            public MaterialInfos materialInfos;
        }
        #endregion

        #region Variables
        /// <summary>
        /// The materials to load on Awake.
        /// </summary>
        [SerializeField] private HapticMaterial[] materials = null;

        protected List<MaterialInfos> _loadedMaterials = new List<MaterialInfos>();
        #endregion

        #region Life Cycle
        protected virtual void Awake()
        {
            #if !UNITY_EDITOR_OSX
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    this.AddMaterial(materials[i]);
                }
            }
            #endif
        }
        #endregion

        #region Privates
        /// <summary>
        /// Extract the data from the material by using the HARWrapper.
        /// </summary>
        /// <param name="id">The loaded material's ID</param>
        /// <param name="hmInfos">MaterialInfos to fill</param>
        /// <returns>The extracted data</returns>
        private MaterialInfos ExtractHapticsMaterialBuffer(int id, MaterialInfos hmInfos)
        {
            //Vibration informations loading
            hmInfos.vibrationLength = (float)HAR.GetVibrationLength(id);

            //Texture informations loading
            hmInfos.textureLength = (float)HAR.GetTextureLength(id);

            #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            //Vibration
            if(hmInfos.vibrationLength >= 0)
            {
                #if UNITY_ANDROID
                hmInfos.pattern = new int[(Mathf.RoundToInt(hmInfos.vibrationLength / PATTERN_STEP) + 1) * 2];

                hmInfos.timePattern = new long[hmInfos.pattern.Length];
                #elif UNITY_IOS
                hmInfos.pattern = new float[(Mathf.RoundToInt(hmInfos.vibrationLength / PATTERN_STEP) + 1)];
                hmInfos.FreqPattern = new float[(Mathf.RoundToInt(hmInfos.vibrationLength / PATTERN_STEP) + 1)];

                int TransVibSize = HAR.GetNumberOfTransient(id, 2,0.0,hmInfos.vibrationLength);

                hmInfos.TransientVibTimer = new float[TransVibSize];
                hmInfos.TransientVibGain = new float[TransVibSize];

                double[] tvt = new double[TransVibSize];
                double[] tvg = new double[TransVibSize];

                HAR.GetTransientsData(id,2,0.0,(double)hmInfos.vibrationLength,TransVibSize,tvt,tvg,new double[TransVibSize],new int[TransVibSize]);

                for(int i =0;i<TransVibSize;i++)
                {
                    hmInfos.TransientVibTimer[i] = (float)tvt[i];
                    hmInfos.TransientVibGain[i]= (float)tvg[i];
                }
                #endif

                for (int i = 0; i < hmInfos.pattern.Length; i++)
                {
                    #if UNITY_ANDROID
                    int v = Mathf.RoundToInt((float)HAR.GetVibrationAmp(id, ((i - 1) / 2) * PATTERN_STEP) * 255);
                    hmInfos.pattern[i] = v;
                    hmInfos.timePattern[i] = 0;
                    i += 1;
                    hmInfos.pattern[i] = v;
                    hmInfos.timePattern[i] = Mathf.RoundToInt(PATTERN_STEP * 1000);
                    #elif UNITY_IOS
                    hmInfos.pattern[i] = (float)HAR.GetVibrationAmp(id, (double)(i * PATTERN_STEP));
                    hmInfos.FreqPattern[i] = (float)HAR.GetVibrationFreq(id, (double)(i * PATTERN_STEP));
                    #endif
                }
            }
            //Texture
            if(hmInfos.textureLength >= 0)
            {
                #if UNITY_ANDROID
                hmInfos.globalPattern = new int[(Mathf.RoundToInt(hmInfos.textureLength / PATTERN_STEP) + 1) * 2];
                #elif UNITY_IOS

                //transients managed in texture

                hmInfos.globalPattern = new float[(Mathf.RoundToInt(hmInfos.textureLength / PATTERN_STEP) + 1)];
                hmInfos.globalFreqPattern = new float[(Mathf.RoundToInt(hmInfos.textureLength / PATTERN_STEP) + 1)];
                #endif
                for (int i = 0; i < hmInfos.globalPattern.Length; i++)
                {
                    #if UNITY_ANDROID
                    int v = Mathf.RoundToInt((float)HAR.GetTextureAmp(id, ((i - 1) / 2) * PATTERN_STEP) * 255);
                    hmInfos.globalPattern[i] = v;
                    hmInfos.globalPattern[++i] = v;
                    #elif UNITY_IOS
                    hmInfos.globalPattern[i] = (float)HAR.GetTextureAmp(id, (double)(i * PATTERN_STEP));
                    hmInfos.globalFreqPattern[i] = (float)HAR.GetTextureFreq(id, (double)(i * PATTERN_STEP));
                    #endif
                }
            }
            #endif

            return hmInfos;
        }
        #endregion

        #region Protected
        /// <summary>
        /// Add the material into the list of loaded materials.
        /// </summary>
        /// <param name="hapticsMaterial">The material file to load</param>
        /// <returns>True on success</returns>
        public bool AddMaterial(HapticMaterial hapticsMaterial)
        {
            bool success = false;

            if (hapticsMaterial != null)
            {
                int id = HAR.AddHM(hapticsMaterial);

                if (id == -1)
                {
                    throw new ArgumentException(ERROR_CorruptedMaterial);
                }

                MaterialInfos hapticsMaterialInfos = new MaterialInfos
                {
                    id = id,
                    material = hapticsMaterial
                };
                hapticsMaterialInfos = this.ExtractHapticsMaterialBuffer(id, hapticsMaterialInfos);

                _loadedMaterials.Add(hapticsMaterialInfos);

                success = true;
            }
            else
            {
                Debug.LogWarning(ERROR_NullMaterial, this);
            }

            return success;
        }
        #endregion

        #region Publics
        /// <summary>
        /// Get the material by using its order index.
        /// </summary>
        /// <param name="index">The material index corresponding to its order in the loaded materials list</param>
        /// <param name="materialInfos">The data of the corresponding material</param>
        /// <returns>True if the List of loaded materials contains the index</returns>
        public bool GetMaterialInfos(int index, out MaterialInfos materialInfos)
        {
            bool success = false;
            materialInfos = default;

            if (index >= 0 && index < _loadedMaterials.Count)
            {
                materialInfos = _loadedMaterials[index];
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Get the index of a loaded material.
        /// </summary>
        /// <param name="material">Material to find</param>
        /// <param name="index">The material index corresponding to its order in the loaded materials list</param>
        /// <returns>True if the material exists in the list of loaded materials</returns>
        public bool GetIndexFromTextAsset(HapticMaterial material, out int index)
        {
            bool success = false;
            index = -1;

            for (int i = 0; i < _loadedMaterials.Count; i++)
            {
                if (_loadedMaterials[i].material.Equals(material))
                {
                    index = i;
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// Allow the user to stop the vibration.
        /// </summary>
        /// <remarks>Only compatible with Android</remarks>
        public void CancelHaptic()
        {
            #if !UNITY_EDITOR && (UNITY_ANDROID)
            GenericAndroidHapticAbstraction.Cancel();
            #endif
            #if UNITY_IOS
            UnityCoreHapticsProxy.StopEngine();
            #endif
        }
        #endregion

    }
}