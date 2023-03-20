/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;
using UnityEngine.UI;


namespace Interhaptics.Platforms.Mobile
{

    /// <summary>
    /// The abstract class to inherit to use the haptics with the Unity's slider.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public abstract class AHapticsSlider : MonoBehaviour
    {

        #region Properties
        public int MaterialIndex => materialIndex;
        #endregion

        #region Variables
        /// <summary>
        /// The index of the material.
        /// </summary>
        [SerializeField, Min(0)]
        private int materialIndex = 0;

        protected Slider _slider = null;
        #endregion

        #region Life Cycle
        protected virtual void Awake()
        {
            _slider = gameObject.GetComponent<Slider>();
            _slider.wholeNumbers = false;
        }

        protected virtual void OnEnable()
        {
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        protected virtual void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(OnValueChanged);
        }
        #endregion

        #region Abstracts
        protected abstract void OnValueChanged(float value);
        #endregion

    }

}