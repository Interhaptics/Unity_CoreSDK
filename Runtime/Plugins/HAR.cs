/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.Collections.Generic;
using UnityEngine;
using Interhaptics.HapticBodyMapping;
using UnityCoreHaptics;
using Interhaptics.Platforms.Mobile;
using System.Collections;

namespace Interhaptics.Core
{
    public static partial class HAR
    {

		#region Constants
		public const double DEFAULT_FREQ_MIN = 60.0;
		public const double DEFAULT_FREQ_MAX = 300.0;
		public const double DEFAULT_INTENSITY = 1.0;
		public const int  DEFAULT_LOOPS = 0;
		public const double DELAY_COMPENSATION = 0.02;
		public const LateralFlag DEFAULT_CONTROLLER_SIDE = LateralFlag.Global;
		#endregion

		// Flag to indicate if the haptic effect should be stopped
		private static bool stopHapticEffect = false;

		#region Enums
		public enum HMaterial_VersionStatus
        {
            NoAnHapticsMaterial = 0,
            V3_NeedToBeReworked = 1,
            V4_Current = 2,
            UnknownVersion = 3
        }
		#endregion

		#region Haptic Material/Effect Management
		private static string parseMaterial(UnityEngine.TextAsset _material)
        {
            if (_material == null)
            {
                return "";
            }
            return _material.text;
        }

		private static string parseMaterial(HapticMaterial _material)
        {
            if (_material == null)
            {
                return "";
            }
            return _material.text;
        }

		/// <summary>
		/// Adds a Haptic Material to the system.
		/// </summary>
		/// <param name="_material">Material in Unity TextAsset format.</param>
		/// <returns>ID of the added haptic material.</returns>
		public static int AddHM(UnityEngine.TextAsset _material)
        {
            return AddHM(parseMaterial(_material));
        }

		/// <summary>
		/// Adds a Haptic Material to the system.
		/// </summary>
		/// <param name="_material">Material in HapticMaterial format.</param>
		/// <returns>ID of the added haptic material.</returns>
		public static int AddHM(HapticMaterial _material)
        {
            return AddHM(parseMaterial(_material));
        }

		/// <summary>
		/// Updates a Haptic Material in the system.
		/// </summary>
		/// <param name="_id">ID of the haptic material to update.</param>
		/// <param name="_material">Updated material in Unity TextAsset format.</param>
		/// <returns>True if update is successful.</returns>
        public static bool UpdateHM(int _id, UnityEngine.TextAsset _material)
        {
            return UpdateHM(_id, parseMaterial(_material));
        }

		/// <summary>
		/// Updates a Haptic Material in the system.
		/// </summary>
		/// <param name="_id">ID of the haptic material to update.</param>
		/// <param name="_material">Updated material in HapticMaterial format.</param>
		/// <returns>True if update is successful.</returns>
		public static bool UpdateHM(int _id, HapticMaterial _material)
        {
            return UpdateHM(_id, parseMaterial(_material));
        }
		#endregion

		/// <summary>
		/// Adds a target to a specific haptic event.
		/// </summary>
		/// <param name="_hMaterialId">ID of the haptic material.</param>
		/// <param name="_target">List of CommandData representing the target.</param>

		public static void AddTargetToEvent(int _hMaterialId, List<CommandData> _target)
        {
            AddTargetToEventMarshal(_hMaterialId, _target.ToArray(), _target.Count);
        }

		/// <summary>
		/// Updates event positions for a specific haptic event.
		/// </summary>
		/// <param name="_hMaterialId">ID of the haptic effect.</param>
		/// <param name="_target">List of CommandData representing the target.</param>
		/// <param name="_texturePosition">New texture position.</param>
		/// <param name="_stiffnessPosition">New stiffness position.</param>
		public static void UpdateEventPositions(int _hMaterialId, List<CommandData> _target, double _texturePosition, double _stiffnessPosition)
        {
            UpdateEventPositionsMarshal(_hMaterialId, _target.ToArray(), _target.Count, _texturePosition, _stiffnessPosition);
        }

		/// <summary>
		/// Removes a target from a specific haptic event.
		/// </summary>
		/// <param name="_hMaterialId">ID of the haptic material.</param>
		/// <param name="_target">List of CommandData representing the target.</param>
		public static void RemoveTargetFromEvent(int _hMaterialId, List<CommandData> _target)
        {
            RemoveTargetFromEventMarshal(_hMaterialId, _target.ToArray(), _target.Count);
        }

		/// <summary>
		/// Sets the intensity for a specific target of a haptic event.
		/// </summary>
		/// <param name="_hMaterialId">ID of the haptic material.</param>
		/// <param name="_target">List of CommandData representing the target.</param>
		/// <param name="_intensity">Intensity value.</param>
		public static void SetTargetIntensity(int _hMaterialId, List<CommandData> _target, double _intensity)
        {
			SetTargetIntensityMarshal(_hMaterialId, _target.ToArray(), _target.Count, _intensity);
		}

		public static CoroutineRunner CurrentRunner { get; private set; }

		/// <summary>
		/// Coroutine runner class for managing haptic effects. 
		/// </summary>
		public class CoroutineRunner : MonoBehaviour
		{
			/// <summary>Current ID of the haptic material being used.</summary>
			public int CurrentHMaterialId { get; set; } = -1;
		}

		/// <summary>
		/// Debug method for printing messages in the console. Debug mode must be enabled in the HapticManager.
		/// </summary>
		/// <param name="message"></param>
		public static void DebugAPIMode(string message)
		{
			if (HapticManager.DebugSwitch)
			{ 
				Debug.Log(message);
			}
		}

		/// <summary>
		/// Plays a haptic effect using a specified haps file.
		/// </summary>
		/// <param name="_material">Haptic effect to be used.</param>
		/// <param name="_intensity">Intensity of the haptic effect.</param>
		/// <param name="controllerSide">Side of the controller for the haptic effect.</param>
		/// <param name="_loops">Number of loops to play. 0 means it is not looping, 1 or more is the number of repeats</param>
		/// <returns>Coroutine for playing the haptic effect.</returns>
		public static IEnumerator PlayHapticEffectCoroutine(HapticMaterial _material, double _intensity, int _loops, LateralFlag controllerSide)
		{
			// Set stopHapticEffect to false at the beginning of the coroutine
			stopHapticEffect = false;
			// Obtain hMaterialId and store it in the current CoroutineRunner instance
			int hMaterialId = AddHM(_material);
			if (CurrentRunner != null)
			{
				CurrentRunner.CurrentHMaterialId = hMaterialId;
			}
			if (hMaterialId == -1)
			{
				DebugAPIMode("PlayHapticEffect: Failed to add haptic effect.");
				yield break;
			}
			List<CommandData> targets = new List<CommandData>{new CommandData(Operator.Plus, GroupID.Hand, controllerSide)};
			AddTargetToEvent(hMaterialId, targets);
			double vibrationLength = GetVibrationLength(hMaterialId);
			DebugAPIMode("PlayHapticEffect: Vibration length retrieved: " + vibrationLength + "\nPlayHapticEffect: Playing event at " + Time.realtimeSinceStartup);
			SetEventIntensity(hMaterialId, _intensity);
			SetEventLoop(hMaterialId, _loops > 0);
			PlayEvent(hMaterialId, (double)-Time.realtimeSinceStartup, 0, 0);
			for (int i=0; i<=_loops;i++)
			{
				yield return new WaitForSeconds((float)vibrationLength);
				#if UNITY_ANDROID || UNITY_IOS
				// Check if the stop flag has been set
				if (stopHapticEffect)
				{
					Debug.Log("StopEffectReceived");
					break; // Exit the loop if stopHapticEffect is true
				}
				if (i<_loops && !stopHapticEffect)
				{
					PlayEvent(hMaterialId, (double)-Time.realtimeSinceStartup, 0, 0);
				}
				#endif
			}
			DebugAPIMode("Reached: " + (_loops+1) * (float)vibrationLength);
			SetEventLoop(hMaterialId, false);
			StopEvent(hMaterialId);
			DebugAPIMode("PlayHapticEffect: Event stopped.");
			if (CurrentRunner != null)
			{
				CurrentRunner.CurrentHMaterialId = -1;
			}
		}

		/// <summary>
		/// Plays a haptic effect using a specified haps file. 
		/// </summary>
		/// <param name="material"></param>
		/// <param name="intensity"></param>
		/// <param name="controllerSide"></param>
		/// <param name="loops"></param>
		public static void PlayHapticEffect(HapticMaterial material, double intensity = DEFAULT_INTENSITY, int loops = DEFAULT_LOOPS, LateralFlag controllerSide = DEFAULT_CONTROLLER_SIDE)
		{
			if (CurrentRunner != null)
			{
				// Stop and clean up the previous haptic effect and CoroutineRunner
				StopPreviousHapticEffect();
				GameObject.Destroy(CurrentRunner.gameObject);
			}

			GameObject runnerObject = new GameObject("CoroutineRunner");
			CurrentRunner = runnerObject.AddComponent<CoroutineRunner>();
			CurrentRunner.StartCoroutine(PlayHapticEffectCoroutine(material, intensity, loops, controllerSide));
		}

		/// <summary>
		/// Stops the previous haptic effect.
		/// </summary>
		private static void StopPreviousHapticEffect()
		{
#if UNITY_ANDROID || UNITY_IOS
				MobileCancelHaptics();
				// Set the stopHapticEffect flag to true
				stopHapticEffect = true;
#endif
			if (CurrentRunner != null && CurrentRunner.CurrentHMaterialId != -1)
			{
				SetEventLoop(CurrentRunner.CurrentHMaterialId, false);
				StopEvent(CurrentRunner.CurrentHMaterialId);
				DebugAPIMode("PlayHapticEffect: Event stopped." + CurrentRunner.CurrentHMaterialId);
			}
		}

		/// <summary>
		/// Plays a parametric haptic effect using specified amplitude, pitch, and transient parameters.
		/// </summary>
		/// <param name="_amplitude">Array of amplitude values formatted as Time - Value pairs, with values between 0 and 1.</param>
		/// <param name="_pitch">Array of pitch values formatted as Time - Value pairs, with values between 0 and 1.</param>
		/// <param name="_transient">Array of transient values formatted as Time - Amplitude - Frequency triples, with values between 0 and 1.</param>
		/// <param name="_intensity">Intensity of the haptic effect.</param>
		/// <param name="_loops">Number of loops to play.</param>
		public static IEnumerator PlayParametricHapticEffectCoroutine(double[] _amplitude, double[] _pitch, double _freqMin, double _freqMax, double[] _transient, double _intensity, int _loops, LateralFlag _controllerSide)
		{
			// Set stopHapticEffect to false at the beginning of the coroutine
			stopHapticEffect = false;
			if (_transient!=null)
			{
				if (_transient[0] == 0.0)
				{   // If the first transient is at time 0, add a small delay to compensate for the delay in the system
					_transient[0] = DELAY_COMPENSATION;
				}
			}
			//Default values for frequency min and max
			int hMaterialId = AddParametricEffect(
				_amplitude, _amplitude != null ? _amplitude.Length : 0,
				_pitch, _pitch != null ? _pitch.Length : 0,
				_freqMin, _freqMax,
				_transient, _transient != null ? _transient.Length : 0,
				_loops > 0
			);
			if (hMaterialId == -1)
			{
				DebugAPIMode("PlayParametricHapticEffect: Failed to create parametric effect.");
				yield break;
			}
			// Store the hMaterialId in the current CoroutineRunner instance
			if (CurrentRunner != null)
			{
				CurrentRunner.CurrentHMaterialId = hMaterialId;
			}

			List<CommandData> targets = new List<CommandData> { new CommandData(Operator.Plus, GroupID.Hand, _controllerSide) };
			AddTargetToEvent(hMaterialId, targets);
			double vibrationLength = GetVibrationLength(hMaterialId);
			DebugAPIMode("PlayParametricHapticEffect: Vibration length retrieved: " + vibrationLength);
			SetEventIntensity(hMaterialId, _intensity);
			PlayEvent(hMaterialId, (double)-Time.realtimeSinceStartup, 0, 0);
			for (int i = 0; i <= _loops; i++)
			{
				// Early check for stop flag
				if (stopHapticEffect)
				{
					DebugAPIMode("StopEffectReceived - Early");
					break;
				}
				yield return new WaitForSeconds((float)vibrationLength);
#if UNITY_ANDROID || UNITY_IOS
				if (stopHapticEffect)
				{
					Debug.Log("StopEffectReceived - Before Replay");
					break;
				}
				if (i < _loops)
				{
					PlayEvent(hMaterialId, (double)-Time.realtimeSinceStartup, 0, 0);
				}
#endif
			}
			StopEvent(hMaterialId);
			DebugAPIMode("PlayParametricHapticEffect: Event stopped.");

			if (CurrentRunner != null)
			{
				CurrentRunner.CurrentHMaterialId = -1;
			}
		}

		/// <summary>
		/// Plays a parametric haptic effect using specified amplitude, pitch, and transient parameters.
		/// </summary>
		/// <param name="_amplitude">Array of amplitude values formatted as Time - Value pairs, with values between 0 and 1.</param>
		/// <param name="_pitch">Array of pitch values formatted as Time - Value pairs, with values between 0 and 1.</param>
		/// <param name="_transient">Array of transient values formatted as Time - Amplitude - Frequency triples, with values between 0 and 1.</param>
		/// <param name="_freqMin">Minimum frequency of the haptic effect.</param>
		/// <param name="_freqMax">Maximum frequency of the haptic effect.</param>
		/// <param name="_intensity">Intensity of the haptic effect.</param>
		/// <param name="_loops">Number of loops to play.</param>
		/// <param name="_controllerSide">Side of the controller for the haptic effect.</param>
		public static void PlayAdvanced(double[] _amplitude, double[] _pitch, double _freqMin = DEFAULT_FREQ_MIN, double _freqMax = DEFAULT_FREQ_MAX, double[] _transient = null, double _intensity = DEFAULT_INTENSITY, int _loops = DEFAULT_LOOPS, LateralFlag _controllerSide = DEFAULT_CONTROLLER_SIDE)
		{

			if (CurrentRunner != null)
			{
				StopPreviousHapticEffect();
				GameObject.Destroy(CurrentRunner.gameObject);
			}

			GameObject runnerObject = new GameObject("CoroutineRunner");
			CurrentRunner = runnerObject.AddComponent<CoroutineRunner>();
			CurrentRunner.StartCoroutine(PlayParametricHapticEffectCoroutine(_amplitude, _pitch, _freqMin, _freqMax, _transient, _intensity, _loops, _controllerSide));
		}

		/// <summary>
		/// Plays haptic effects based on provided arrays of amplitudes and transient triplets.
		/// </summary>
		/// <param name="amplitudes">Array of amplitude values, with values between 0 and 1.</param>
		/// <param name="transients">Array of transient triplets formatted as Time - Amplitude - Frequency, with values between 0 and 1.</param>
		/// <param name="_intensity">Intensity of the haptic effect.</param>
		/// <param name="_loops">Number of times to loop the effect.</param>
		/// <param name="_controllerSide">Side of the controller for the haptic effect.</param>
		public static void Play(double[] amplitudes, double[] transients, double _intensity = DEFAULT_INTENSITY, int _loops = DEFAULT_LOOPS, LateralFlag _controllerSide = DEFAULT_CONTROLLER_SIDE)
		{
			DebugAPIMode("PlayAmplitudesTransients: Playing amplitudes and transients at " + Time.realtimeSinceStartup);

			PlayAdvanced(
				amplitudes, // The amplitude array
				null, // No pitch 
				DEFAULT_FREQ_MIN, // Default frequency min
				DEFAULT_FREQ_MAX, // Default frequency max
				transients, // The transient triplets
				_intensity,  // Intensity of the effects
				_loops, // Number of loops
				_controllerSide // Controller side
			);
		}

		/// <summary>
		/// Plays multiple amplitude-based haptic effects based on provided time-amplitudes pairs.
		/// </summary>
		/// <param name="amplitudes">Array of time - amplitude values, with amplitude values between 0 and 1.</param>
		/// <param name="_intensity">Intensity of the haptic effect.</param>
		/// <param name="_loops">Number of times to loop the effect.</param>
		public static void Play(double[] amplitudes, double _intensity = DEFAULT_INTENSITY, int _loops = DEFAULT_LOOPS, LateralFlag _controllerSide = DEFAULT_CONTROLLER_SIDE)
		{
			DebugAPIMode("Play: Playing amplitudes at " + Time.realtimeSinceStartup);

			Play(
				amplitudes, // The amplitude array
				null, // No transients 
				_intensity,  // Intensity of the effects
				_loops, // Number of loops
				_controllerSide // Controller side
			);
		}

		/// <summary>
		/// Plays multiple transient haptic effects based on provided triplets of time, amplitude, and frequency. (Loops number optional)
		/// </summary>
		/// <param name="transients">Array of transient triplets formatted as Time - Amplitude - Frequency, with values between 0 and 1.</param>
		/// <returns>A coroutine that plays the transient haptic effects.</returns>
		/// <example>
		/// <code>
		/// StartCoroutine(HapticEffectPlayer.PlayTransients(new double[] { 0.5, 0.8, 0.6, 1.0, 1.0, 0.5 }));
		/// </code>
		/// This would play two transient effects: the first half a second after the method is called with 0.8 amplitude and 0.6 frequency, and the second one second after the method is called with full amplitude and 0.5 frequency.
		/// </example>
		public static void PlayTransients(double[] transients, double _intensity = DEFAULT_INTENSITY, int _loops = DEFAULT_LOOPS, LateralFlag _controllerSide = DEFAULT_CONTROLLER_SIDE)
		{
			// Debug message with timestamp for when the transients play
			DebugAPIMode("PlayTransients: Playing transients at " + Time.realtimeSinceStartup);
			// This call should include only the transient array, as we're only interested in playing transients.
			PlayAdvanced(
				null, // No amplitude 
				null, // No pitch 
				DEFAULT_FREQ_MIN, // Default frequency min
				DEFAULT_FREQ_MAX, // Default frequency max
				transients, // The transient triplets
				_intensity,  // Intensity of the transient effects
				_loops, // Number of loops
				_controllerSide // Controller side
			);
		}

		/// <summary>
		/// Plays a single transient haptic effect at the specified time, amplitude, and frequency.
		/// </summary>
		/// <param name="time">The time at which the transient should occur, in seconds.</param>
		/// <param name="amplitude">The amplitude of the transient effect, between 0 and 1.</param>
		/// <param name="frequency">The frequency of the transient effect, between 0 and 1.</param>
		/// <returns>A coroutine that plays the transient haptic effect.</returns>
		public static void PlayTransient(double time = DELAY_COMPENSATION, double amplitude = 1.0, double frequency = 1.0, double _intensity = DEFAULT_INTENSITY, int _loops = DEFAULT_LOOPS, LateralFlag _controllerSide = DEFAULT_CONTROLLER_SIDE)
		{
			DebugAPIMode("PlayTransient: Playing transient at " + Time.realtimeSinceStartup);
			double[] transient = { time, amplitude, frequency };
#if UNITY_IOS
			transient = new double[] { time, amplitude, frequency, time + 0.1, amplitude, frequency };
#endif
			// Call the PlayParametricHapticEffect coroutine with the transient parameters
			PlayTransients(
				transient, // The transient parameters
				_intensity, 
				_loops, // intensity and loops optional
				_controllerSide
			);
		}

		/// <summary>
		/// Plays a single constant haptic effect at the specified amplitude and for a specified time duration.
		/// </summary>
		/// <param name="amplitude"></param>
		/// <param name="time"></param>
		public static void PlayConstant(double amplitude, double time, double _intensity = DEFAULT_INTENSITY, int _loops = DEFAULT_LOOPS, LateralFlag _controllerSide = DEFAULT_CONTROLLER_SIDE)
		{
			double[] amplitudes = { 0.0, amplitude, time, amplitude };
			Play(amplitudes, _intensity, _loops, _controllerSide);
		}

		private static int AndroidVersion
		{
			get
			{
				using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
				{
					return version.GetStatic<int>("SDK_INT");
				}
			}
		}
		public static void MobileCancelHaptics()
		{
			// Set the stopHapticEffect flag to true
			stopHapticEffect = true;
#if !UNITY_EDITOR && (UNITY_ANDROID)
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				var vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
				if (vibrator != null)
				{
					vibrator.Call("cancel");
					// For API Level 26 and above, use VibrationEffect with amplitude 0
					if (AndroidVersion >= 26)
					{
						AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
						int silentAmplitude = 0; // Amplitude 0 for silent vibration
						long[] pattern = { 0, 10 }; // Very short vibration
						int repeat = -1; // -1 for no repeat
						AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", pattern, new int[]{ silentAmplitude, silentAmplitude }, repeat);
						vibrator.Call("vibrate", vibrationEffect);
					}
					else
					{
						// For older versions, just cancel as you can't set amplitude
						// The cancel call above already stops the vibration
					}
					vibrator.Dispose();
				}
				// Obtain a new instance of the vibrator service
				vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

    // Now you can use the new instance of the vibrator service
			}
#endif
#if UNITY_IOS
            UnityCoreHapticsProxy.StopEngine();
#endif
		}

		/// <summary>
		/// Stops the current haptic effect.
		/// </summary>
		public static void StopCurrentHapticEffect()
		{
#if UNITY_ANDROID || UNITY_IOS
				MobileCancelHaptics();
#endif
			if (CurrentRunner != null && CurrentRunner.CurrentHMaterialId != -1)
			{
				// Logic to stop the haptic effect
				SetEventLoop(CurrentRunner.CurrentHMaterialId, false);
				StopEvent(CurrentRunner.CurrentHMaterialId);
				ClearEvent(CurrentRunner.CurrentHMaterialId);
				ClearActiveEvents();
				DebugAPIMode("Haptic effect stopped: " + CurrentRunner.CurrentHMaterialId);
				// Set the stopHapticEffect flag to true
				stopHapticEffect = true;
				// Reset the CurrentHMaterialId
				CurrentRunner.CurrentHMaterialId = -1;
			}
		}
	}
}
