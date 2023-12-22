using UnityEngine;
using Interhaptics.Core;
using Interhaptics;
using Interhaptics.HapticBodyMapping;
using Interhaptics.Utils;

namespace Interhaptics.Samples
{
/// <summary>
/// Class to test the different haptic effects in the Interhaptics SDK triggered by key presses on the code side (no Monobehaviour)
/// </summary>
	public class HapticEffectCodeTester : MonoBehaviour
	{
		public HapticMaterial hapticMaterial; // Assign this in the Unity Inspector
		public double intensity = 1.0; // Intensity of the haptic effect
		double[] transientTester = { 0.0, 1.0, 1.0, 0.5, 1.0, 1.0, 1.0, 1.0, 1.0 }; // Transient pattern
																	 // Define your amplitude, pitch, and transient values
		double[] amplitude = { 0.0, 1.0, 2.5, 1.0 }; // Constant amplitude
		double[] pitch = { 0.0, 0.0, 1.5, 1.0, 2.5, 0.0 }; // Pitch pattern
		double[] transient = { 3.0, 1.0, 1.0, 4.0, 1.0, 1.0 , 4.5, 1.0, 1.0}; // Transient pattern

		void Update()
		{
			// Trigger the haptic effect based on a condition, for example, a button press
			if (Input.GetKeyDown(KeyCode.K))
			{
				TestParametricHapticEffect();
			}

			if (Input.GetKeyDown(KeyCode.I))
			{
				TestAmplitude();
			}

			if (Input.GetKeyDown(KeyCode.U))
			{
				TestAmplitudesTransients();
			}

			if (Input.GetKeyDown(KeyCode.J))
			{
				TestTransient();
			}

			if (Input.GetKeyDown(KeyCode.H))
			{
				TestTransients();
			}

			if (Input.GetKeyDown(KeyCode.M))
			{
				TestConstantRight();
			}

			if (Input.GetKeyDown(KeyCode.L))
			{
				// Play the haptic effect
				HAR.PlayHapticEffect(hapticMaterial, 0.5, 2, LateralFlag.Left);
				//StartCoroutine(HAR.PlayHapticEffectCoroutine(hapticMaterial, 1.0, LateralFlag.Left, true, 3));
			}
			if (Input.GetKeyDown(KeyCode.O))
			{
				TestHapticEffect();
			}
			if (Input.GetKeyDown(KeyCode.P))
			{
				TestTransients();
			}

			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				StopHapticEffect();
			}
			// Detect key presses for numbers 1 to 9 and play corresponding haptic effect
			for (int i = 1; i <= 9; i++)
			{
				if (Input.GetKeyDown(KeyCode.Alpha0 + i))
				{
					HapticPreset.Play((HapticPreset.PresetType)(i - 1));
				}
			}
		}
		public void TestConstant()
		{
			HAR.PlayConstant(1.0, 5.0);
		}
		public void TestConstantRight()
		{
			HAR.PlayConstant(1.0, 5.0, _controllerSide: LateralFlag.Right);
		}
		public void TestParametricHapticEffect()
		{
			HAR.PlayAdvanced(amplitude, pitch, HAR.DEFAULT_FREQ_MIN, HAR.DEFAULT_FREQ_MAX, transient, 1.0, 2); // Intensity set to 1.0
		}
		public void TestHapticEffect()
		{
			HAR.PlayHapticEffect(hapticMaterial, 1.0, 1, LateralFlag.Right);
		}
		public void TestTransient()
		{
			HAR.PlayTransient();
		}
		public void TestTransients()
		{
			HAR.PlayTransients(transientTester, 1);
		}
		public void TestAmplitude()
		{
			HAR.Play(amplitude);
		}
		public void TestAmplitudesTransients()
		{
			HAR.Play(amplitude, transient, 1.0, 1);
		}
		public void PlayStockHaptics(int i)
		{
			HapticPreset.Play((HapticPreset.PresetType)(i));
		}
		public void StopMobileHaptics()
		{
			HAR.MobileCancelHaptics();
		}
		public void StopHapticEffect()
		{
			HAR.StopCurrentHapticEffect();
			HAR.ClearEvent(HAR.CurrentRunner.CurrentHMaterialId);
			HAR.ClearActiveEvents();
		}
	}

}
