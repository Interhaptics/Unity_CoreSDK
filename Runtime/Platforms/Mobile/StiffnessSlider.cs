/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;
using UnityEngine.EventSystems;


namespace Interhaptics.Platforms.Mobile
{
    /// <summary>
    /// Example about how we can handle haptics stiffness by using a slider.
    /// </summary>
    public class StiffnessSlider : AHapticsSlider, IPointerDownHandler, IPointerUpHandler
    {

        #region Properties
        /// <summary>
        /// Return the MobileHapticsStiffness set in the inspector.
        /// </summary>
        /// <see cref="MobileHapticsStiffness"/>
        public MobileHapticsStiffness Handler { get { return mobileHapticsStiffness; } }
        #endregion

        #region Variables
        /// <summary>
        /// The specified MobileHapticsStiffness handler.
        /// </summary>
        /// <see cref="MobileHapticsStiffness"/>
        [SerializeField]
        private MobileHapticsStiffness mobileHapticsStiffness = null;

        protected float _currentValue = 0;
        protected bool _isInteracting = false;
        #endregion

        #region Life Cycle
        protected virtual void Update()
        {
            if (!_isInteracting)
            {
                return;
            }

            if (mobileHapticsStiffness)
            {
                mobileHapticsStiffness.PlayStiffness(MaterialIndex, _currentValue);
            }
        }
        #endregion

        #region Abstracts
        protected override void OnValueChanged(float value)
        {
            _currentValue = value;
        }
        #endregion

        #region Interfaces
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            _isInteracting = true;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            _isInteracting = false;
        }
        #endregion
    }
}
