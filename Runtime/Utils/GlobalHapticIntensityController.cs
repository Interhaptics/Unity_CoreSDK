using Interhaptics.Utils;
using UnityEngine;
using UnityEngine.UI; 

namespace Interhaptics.Core
{
	public class GlobalHapticIntensityController : MonoBehaviour
	{
		[Tooltip("The global intensity for all haptic events.")]
		[Range(0, 1)] // Assuming the intensity range is between 0 and 1
		public double globalIntensity = 1.0;
		[Tooltip("Activate Debug Mode for Intensity Controller and for the HapticManager")]
		[SerializeField]
		public bool debugMode = false;
		[SerializeField]
		private bool stopHapticsOnFocusLoss = true;
		private double intensityBeforeMute;
		// Delegate to notify when the intensity changes
		public delegate void OnIntensityChange(double newIntensity);
		public event OnIntensityChange onIntensityChanged;

		private void Awake()
		{
			if (debugMode)
			{
				HapticManager.DebugSwitch = true;
			}
            if (!stopHapticsOnFocusLoss)
            {
				DebugMode("Haptics will not stop on focus loss");
				HapticManager.StopHapticsOnFocusLoss = stopHapticsOnFocusLoss;
            }
        }

		private void Start()
		{
			SetIntensity();
		}

		public void	StopAllHaptics()
		{
			intensityBeforeMute = globalIntensity;
			SetGlobalIntensity(0.0);
		}

		public void ResumeAllHaptics()
		{
			SetGlobalIntensity(intensityBeforeMute);
		}
		private void Update()
		{
			if (debugMode)
			{
				HapticManager.DebugSwitch = true;
			}
			if (globalIntensity!=HAR.GetGlobalIntensity())
			{
				SetGlobalIntensity(globalIntensity);
			}
			//* Debugging controls
			if (Input.GetKeyDown(KeyCode.R))
			{
				StopAllHaptics();
			}
			if (Input.GetKeyDown(KeyCode.Y))
			{
				ResumeAllHaptics();
			}
			//*/
		}

		public void DebugMode(string debugMessage) 
		{
			if (debugMode)
			{
				Debug.Log(debugMessage);
			}
		}

		// This method now only sets the global intensity and does not interact with any UI elements
		public void SetGlobalIntensity(double newIntensity)
		{
			globalIntensity = newIntensity;
			HAR.SetGlobalIntensity(globalIntensity);
			onIntensityChanged?.Invoke(globalIntensity); // Invoke the event to notify listeners
			DebugMode($"Global haptic intensity set to: {globalIntensity}");
		}

		// Call this method to set the global intensity for haptic events
		public void SetIntensity()
		{
			HAR.SetGlobalIntensity(globalIntensity);
			DebugMode($"Global haptic intensity set to: {globalIntensity}");
		}

		// Call this method to get the current global intensity for haptic events
		public double GetCurrentIntensity()
		{
			return HAR.GetGlobalIntensity();
		}

		// Example usage: You might want to call SetIntensity in response to a UI event, like a slider change
		public void OnIntensitySliderChanged(float newIntensity)
		{
			SetGlobalIntensity(newIntensity);
		}

		// Example usage: You might want to log the current global intensity at certain times
		public void LogCurrentIntensity()
		{
			double currentIntensity = GetCurrentIntensity();
			DebugMode($"Current global haptic intensity is: {currentIntensity}");
		}
	}
}