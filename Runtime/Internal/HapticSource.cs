/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;
using System.Collections.Generic;
using Interhaptics.Core;
using Interhaptics.Utils;
using System.Collections;
using System.IO;
using System;

namespace Interhaptics.Internal
{
    [AddComponentMenu("Interhaptics/HapticSource")]
    public class HapticSource : MonoBehaviour
    {
		[Header("Haptic Effect File")]
		[SerializeField]
        public HapticMaterial hapticMaterial;
		[Header("Haptic Source Settings")]
		[Range(0, 2)]
		[SerializeField]
		private float sourceIntensity = 1.0f;
		[Tooltip("Indicates the delay before playing the vibration.")]
		[SerializeField]
        public float vibrationOffset;
        private float textureOffset;
        private float stiffnessOffset;
        [SerializeField]
        public bool debugMode;
		[Tooltip("Indicates whether the effect should loop.")]
		[SerializeField]
		public bool isLooping;
		[ConditionalHide("isLooping", true)]
		[Tooltip("Maximum number of loops")] //TODO: -1 means no limit
		[SerializeField]
		public int maxLoops = 1;
		[ConditionalHide("isLooping", true)]
		[Tooltip("Maximum time for loops")]
		[SerializeField]
		public float maxLoopTime = 10f;
		[SerializeField]
		public double targetIntensity = 1.0;
		[SerializeField]
		public bool playAtStart = false;
		public bool isPlaying = false;
		[HideInInspector]
		public double hapticEffectDuration = 0;
		private float currentSourceIntensity;

		// Fields necessary for the loop control
		public Coroutine playingCoroutine = null; 
		[HideInInspector]
		public double loopStartTime = 0;

		public const string ERROR_MESSAGE_MONO = "Interhaptics requires IL2CPP scripting backend for Android. Please change it in Player Settings. Haptics will not play on the Mono scripting backend on the Android platform.";
		public int HapticMaterialId
        {
            get; set;
        }

		// Public property to get and set the source intensity
		public float SourceIntensity
		{
			get => sourceIntensity;
			set
			{
				sourceIntensity = value;
				DebugMode("Source intensity set to: " + sourceIntensity);
				ApplySourceIntensity(); // Apply the intensity immediately when set
			}
		}

		#region Lifecycle
		/// <summary>
		/// Add the haptic material to the when the object is created
		/// </summary>
		protected virtual void Awake()
		{
#if UNITY_ANDROID
			if (HapticManager.MonoScriptingBackend)
			{
				DebugMode(ERROR_MESSAGE_MONO + "Haptic Source");
				return;
			}
            else
            {
                DebugMode("IL2CPP Haptic Source");
            }
#endif
			if (hapticMaterial != null)
			{
				HapticMaterialId = Core.HAR.AddHM(hapticMaterial);
			}
			else
			{
			DebugMode("No haptic effect provided. Please assign a HapticMaterial in the inspector or provide a path to a haptic effect file in the StreamingAssets directory.");
			}
		}

		/// <summary>
		/// Initialize the haptic effect settings at the start of the game
		/// </summary>
		protected virtual void Start()
		{
			// Initialize the haptic effect at the start of the game
			hapticEffectDuration = HAR.GetVibrationLength(HapticMaterialId);
			currentSourceIntensity = sourceIntensity;
			if (hapticMaterial!=null)
			{
				DebugMode("Haptic effect duration: " + hapticEffectDuration + " " + hapticMaterial.name);
			}
			else
			{
				DebugMode("Parametric Haptic effect duration: " + hapticEffectDuration + " " + HapticMaterialId);
			}
			if (isLooping)
			{
				float maxComputedTimeInLoops = maxLoops * (float)hapticEffectDuration;
				if (maxComputedTimeInLoops < maxLoopTime)
				{
					maxLoopTime = maxComputedTimeInLoops;
				}
				else
				{
					maxLoops = (int)(maxLoopTime / (float)hapticEffectDuration);
					maxLoopTime = maxLoops * (float)hapticEffectDuration;
				}
			}
			else
			{ 
				maxLoopTime = (float)hapticEffectDuration;
				maxLoops = 1;
			}
			if (playAtStart)
			{
				playingCoroutine = StartCoroutine(ControlVibration());
			}
		}

		/// <summary>
		/// Update the haptic effect settings at every frame
		/// </summary>
		protected virtual void Update()
		{
			if (sourceIntensity != currentSourceIntensity)
			{
				ApplySourceIntensity();
				currentSourceIntensity = sourceIntensity;
				DebugMode("Source intensity changed to: " + sourceIntensity);	
			}
        }

#endregion

		public virtual void ApplyTargetIntensity()
		{
			//Empty for now
		}

		/// <summary>
		/// Call this method to apply the source intensity
		/// </summary>
		public void ApplySourceIntensity()
		{
			HAR.SetEventIntensity(HapticMaterialId, sourceIntensity);
		}
		/// <summary>
		/// Call this method to apply the looping state
		/// </summary>
		public void ApplyLooping(bool loopValue)
		{
			DebugMode("Applied looping: " + loopValue);
			HAR.SetEventLoop(HapticMaterialId, loopValue);
		}

		/// <summary>
		/// Debug method to print messages in the console only when debugMode is enabled
		/// </summary>
		/// <param name="debugMessage"></param>
		public void DebugMode(string debugMessage)
        {
            if (debugMode)
            {
                Debug.Log(debugMessage);
            }
        }

		/// <summary>
		/// Call this method to play the haptic effect
		/// </summary>
		public virtual void Play()
        {
			isPlaying = true;
            Core.HAR.PlayEvent(HapticMaterialId, -Time.realtimeSinceStartup + vibrationOffset, textureOffset, stiffnessOffset);
        }

		/// <summary>
		/// Call this method to stop the haptic effect
		/// </summary>
        public virtual void Stop()
        {
			isPlaying = false;
            Core.HAR.StopEvent(HapticMaterialId);
			if (playingCoroutine != null)
			{
				StopCoroutine(playingCoroutine);
				playingCoroutine = null;
			}
		}

		/// <summary>
		/// Call this method to add a target to the haptic effect
		/// </summary>
		/// <param name="Target"></param>
        public void AddTarget(List<HapticBodyMapping.CommandData> Target)
        {
            Core.HAR.AddTargetToEvent(HapticMaterialId, Target);
        }

		/// <summary>
		/// Call this method to remove a target from the haptic effect
		/// </summary>
		/// <param name="Target"></param>
        public void RemoveTarget(List<HapticBodyMapping.CommandData> Target)
        {
            Core.HAR.RemoveTargetFromEvent(HapticMaterialId, Target);
        }
		/// <summary>
		/// Method to start the coroutine from outside (if necessary). Plays the haptic effect after the vibrationOffset
		/// </summary>
		public virtual void PlayEventVibration()
		{
			if (playingCoroutine != null)
			{
				StopCoroutine(playingCoroutine);
			}
			playingCoroutine = StartCoroutine(ControlVibration());
		}

		/// <summary>
		/// Controls the vibration perception based on the full length of the haptic material; stops any residual haptics which might come from the controller after the haptic playback length
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator ControlVibration()
		{
			yield return new WaitForSeconds(vibrationOffset);
			DebugMode(string.Format("Started playing haptics! + {0}", Time.time));
#if (!ENABLE_METAQUEST && UNITY_ANDROID && !UNITY_EDITOR) || UNITY_IOS // Coroutine logic specific to Android and iOS
            int loopsPlayed = 0;
            float loopStartTime = Time.time;
            int maxComputedLoops = maxLoops > 0 ? maxLoops : int.MaxValue;

            while (loopsPlayed < maxComputedLoops)
            {
                Play(); // Play haptic effect
                loopsPlayed++;
                DebugMode($"Loop {loopsPlayed} start at {Time.time}");

                // Wait for the haptic effect duration to finish before restarting
                yield return new WaitForSeconds((float)hapticEffectDuration);

                // Check if the maxLoops condition has been met
                if (loopsPlayed >= maxComputedLoops)
                {
                    DebugMode($"Max loops reached: {loopsPlayed} loops at {Time.time}");
                    break;
                }
            }
			isPlaying = false;
			float totalTimePlayed = maxComputedLoops * (float)hapticEffectDuration;
			DebugMode($"Finished playing haptics at {Time.time} after {totalTimePlayed} seconds");
#else
			// Coroutine logic for other platforms
			if (isLooping)
			{
				ApplyLooping(isLooping);
			}
			Play();
			float effectStartTime = Time.time;
			float effectEndTime = effectStartTime + maxLoopTime;
			// Check if we have a valid maxLoopTime, otherwise keep playing indefinitely
			if (maxLoopTime > 0)
			{
				// Loop until the maxLoopTime is reached
				while (Time.time < effectEndTime)
				{
					yield return null; // Wait until the next frame
				}
				// Once the maxLoopTime is exceeded, stop the effect
				DebugMode($"Stopped playing haptics at {Time.time} after reaching max loop time of {maxLoopTime} seconds");
				if (isLooping)
				{
					ApplyLooping(false);
				}
				isPlaying = false;
			}
			else
			{
				// If maxLoopTime is not valid (e.g., -1 for indefinite), just wait for the duration of the effect
				yield return new WaitForSeconds((float)hapticEffectDuration);
			}
#endif 
			// Clean up
			playingCoroutine = null;
		}
	}
}
