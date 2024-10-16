using System.Collections;
using UnityEngine;
using Interhaptics.Core;

//Tester class for Interhaptics XR scene

namespace Interhaptics.Samples
{
	public class OnTriggerAPICall : MonoBehaviour
	{
		[SerializeField]
		private HapticEffectCodeTester hapticEffectCodeTester;
		[SerializeField]
		private GlobalHapticIntensityController globalHapticIntensityController;
		[SerializeField]
		private int testCaseNumber = 0;

		private void OnTriggerEnter(Collider other)
		{
			if (testCaseNumber == 0)
			{
				hapticEffectCodeTester.TestParametricHapticEffect();
				Debug.Log("TestParametricHapticEffect");
			}
			if (testCaseNumber == 1)
			{
				hapticEffectCodeTester.TestHapticEffect();
			}
			if (testCaseNumber == 2)
			{
				globalHapticIntensityController.SetGlobalIntensity(0.1);
			}

			if (testCaseNumber == 3)
			{
				globalHapticIntensityController.SetGlobalIntensity(1);
			}

			if (testCaseNumber == 4)
			{
				globalHapticIntensityController.SetGlobalIntensity(0);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			hapticEffectCodeTester.StopHapticEffect();
		}
	}
}
