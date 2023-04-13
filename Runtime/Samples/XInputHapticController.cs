/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System;
using UnityEngine;
using UnityEngine.UI;
using Interhaptics;
using Interhaptics.Samples;
using Interhaptics.Internal;
using Interhaptics.Utils;
using Interhaptics.Platforms.XInput;

namespace Interhaptics.Platforms.XInput.Samples
{
    public class XInputHapticController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Add EventHapticSources for vibrations on button press or event calls")]
        private EventHapticSource[] eventHapticSources;
        [SerializeField]
        [Tooltip("Add AudioHapticSources for vibrations on button press or event calls")]
        private AudioHapticSource[] audioHapticSources;
        [SerializeField]
        [Tooltip("Add SpatialHapticSources for vibrations on button press or collision/trigger calls")]
        private SpatialHapticSource[] spatialHapticSources;
        [SerializeField]
        private TextMesh buttonPressed;
        [SerializeField]
        private TextMesh vibrationMaterialName;
        [SerializeField]
        private TextMesh vibrationType;
        [SerializeField]
        private int m_StickId = 1;
        [SerializeField]
        [Range(0, 6)]
        private int indexStiffness;
        [SerializeField]
        [Range(0, 2)]
        private int indexVibration;
        private bool canModifyVibrationType = true;

        private void Start()
        {
            VibrationHapticSourceGUI();
        }

        private void Update()
        {
            VibrationButtons();
            ButtonsUI();
        }

        private void VibrationButtons()
        {
            float dpadVertical = Input.GetAxis("dpad" + m_StickId + "_vertical");
            if (dpadVertical == 0)
            {
                canModifyVibrationType = true;
            }
            else
            {
                IncreaseIndexVibrations(dpadVertical > 0);
            }
            KeyCode[] joystickButtonCodes = {
        (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + m_StickId + "Button0", true),
        (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + m_StickId + "Button1", true),
        (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + m_StickId + "Button2", true),
        (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + m_StickId + "Button3", true)
            };
            for (int i = 0; i < joystickButtonCodes.Length; i++)
            {
                if (Input.GetKey(joystickButtonCodes[i]))
                {
                    HapticVibrationController(indexVibration, i);
                }
            }
        }

        private void HapticVibrationController(int indexVibration, int indexButton)
        {
            switch (indexVibration)
            {
                case 0:
                    PlayAudioVibrationController(indexButton);
                    break;
                case 1:
                    PlayEventVibrationController(indexButton);
                    break;
                case 2:
                    PlaySpatialVibrationController(indexButton);
                    break;
                default:
                    Debug.LogWarning("Invalid input for indexVibration.");
                    break;
            }
        }

        public void PlayAudioVibrationController(int indexButton)
        {
            audioHapticSources[indexButton].Play();
            vibrationMaterialName.text = audioHapticSources[indexButton].name;
        }

        public void PlayEventVibrationController(int indexButton)
        {
            eventHapticSources[indexButton].PlayEventVibration();
            vibrationMaterialName.text = eventHapticSources[indexButton].name;
        }

        public void PlaySpatialVibrationController(int indexButton)
        {
            spatialHapticSources[indexButton].GetComponent<ObjectTransform>().enabled = true;
            spatialHapticSources[indexButton].GetComponent<ObjectTransform>().MoveSpatialHapticSource();
            vibrationMaterialName.text = spatialHapticSources[indexButton].name;
        }

        public void IncreaseIndexVibrations(bool isIncrementing)
        {
            if (canModifyVibrationType)
            {
                indexVibration = isIncrementing ? (indexVibration + 1) % 3 : (indexVibration + 2) % 3;
                canModifyVibrationType = false;
            }
            VibrationHapticSourceGUI();
        }

        private void VibrationHapticSourceGUI()
        {
            switch (indexVibration)
            {
                case 0:
                    vibrationType.text = "Audio Haptic Source";
                    break;
                case 1:
                    vibrationType.text = "Event Haptic Source";
                    break;
                case 2:
                    vibrationType.text = "Spatial Haptic Source";
                    break;
                default:
                    Debug.LogWarning("Invalid input for Haptic Source.");
                    break;
            }
        }

        private void ButtonsUI()
        {
            string[] buttonNames = { "A | Cross", "B | Circle", "X | Square", "Y | Triangle", "Shoulder Left", "Shoulder Right" };
            string[] dpadNames = { "DPad left", "DPad right", "DPad down", "DPad up" };
            for (int i = 0; i < buttonNames.Length; i++)
            {
                if (Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + m_StickId + "Button" + i, true)))
                {
                    buttonPressed.text = buttonNames[i];
                    return;
                }
            }
            for (int i = 0; i < dpadNames.Length; i++)
            {
                if (i < 2 && Input.GetAxis("dpad" + m_StickId + "_horizontal") == (i == 0 ? -1 : 1))
                {
                    buttonPressed.text = dpadNames[i] + " " + Input.GetAxis("dpad" + m_StickId + "_horizontal");
                    return;
                }
                else if (i >= 2 && Input.GetAxis("dpad" + m_StickId + "_vertical") == (i == 2 ? -1 : 1))
                {
                    buttonPressed.text = dpadNames[i] + " " + Input.GetAxis("dpad" + m_StickId + "_vertical");
                    return;
                }
            }
        }
    }
}

