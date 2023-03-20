/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;


namespace Interhaptics.Platforms.Mobile
{
    /// <summary>
    /// An example that shows how we can handle haptics texture with a slider.
    /// </summary>
    public class TextureSlider : AHapticsSlider
    {

        /// <summary>
        /// Return the MobileHapticsTexture set in the inspector.
        /// </summary>
        /// <see cref="MobileHapticsTexture"/>
        public MobileHapticsTexture Handler { get { return mobileHapticsTexture; } }

        #region Variables
        /// <summary>
        /// The specified MobileHapticsTexture handler.
        /// </summary>
        /// <see cref="MobileHapticsTexture"/>
        [SerializeField]
        private MobileHapticsTexture mobileHapticsTexture = null;

        private float _sliderLastValue = 0; 
        #endregion

        #region Abstracts
        protected override void OnValueChanged(float value) 
        {
            _sliderLastValue = value;
        }
        #endregion

        #region Life Cycle
        /// <summary>
        /// We refresh the texture of each frame.
        /// </summary>
        protected virtual void Update()
        {
            //We directly send the value from the slider to the MobileHapticsTexture script (selected in the inspector)
            if (mobileHapticsTexture)
            {
                mobileHapticsTexture.PlayTexture(MaterialIndex, _sliderLastValue);
            }
        }
        #endregion

    }
}
