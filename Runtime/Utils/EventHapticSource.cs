/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;
using System.Linq;
using Interhaptics.HapticBodyMapping;
using Interhaptics.Core;
using System.Collections.Generic;
using System.Collections;

namespace Interhaptics.Utils
{
	[AddComponentMenu("Interhaptics/EventHapticSource")]
	public class EventHapticSource : Internal.HapticSource
	{
		public HapticBodyPart[] hapticBodyParts;

#region Lifecycle
		protected override void Start()
		{
			//AddTarget(hapticBodyParts.Select(hapticBodyPart => new CommandData(Operator.Plus, hapticBodyPart.BodyPart, hapticBodyPart.Side)).ToList());
			base.Start();
		}

		protected override void Update()
		{
			base.Update();
			//DebugMode("Target intensity changed to: " + targetIntensity);
		}
#endregion

		public override void Play()
		{
			// Check if all HapticBodyParts have TargetIntensity set to 0, and if so, do not play.
			if (hapticBodyParts.All(hbp => hbp.TargetIntensity == 0))
			{
				DebugMode("All TargetIntensity values are set to 0, not playing haptic effect.");
				return; // Exit the method without playing.
			}
			AddTarget(hapticBodyParts.Select(hapticBodyPart => new CommandData(Operator.Plus, hapticBodyPart.BodyPart, hapticBodyPart.Side)).ToList());
			base.Play();
		}

		public override void Stop()
		{
#if !UNITY_PS5 //PS5 platform needs this commented out, otherwise residual haptics and no haptic playback until provider update - TODO: remove this when PS5 provider is updated
			base.Stop();
			//RemoveTarget(hapticBodyParts.Select(hapticBodyPart => new CommandData(Operator.Plus, hapticBodyPart.BodyPart, hapticBodyPart.Side)).ToList());
#endif
		}

		// Use the base class's coroutine for looping
		public override void PlayEventVibration()
		{
			base.PlayEventVibration();
		}

		public override IEnumerator ControlVibration()
		{
			yield return new WaitForSeconds(vibrationOffset);
			DebugMode(string.Format("Started playing haptics! + {0}", Time.time));
			int loopsPlayed = 0;
			float loopStartTime = Time.time;
			float totalTimePlayed = 0f;
			int maxComputedLoops = maxLoops > 0 ? maxLoops : int.MaxValue;
			Debug.Log ("maxComputedLoops: " + maxComputedLoops + " " + hapticMaterial.name);

			while (loopsPlayed < maxComputedLoops)
			{
				Play(); // Play audio and haptic effect
				loopsPlayed++;
				DebugMode($"Loop {loopsPlayed} start at {Time.time}");

				// Wait for the haptic effect duration to finish before restarting
				yield return new WaitForSeconds((float)hapticEffectDuration);
				totalTimePlayed = maxComputedLoops * (float)hapticEffectDuration;

				// Check if the maxLoops condition has been met
				if (loopsPlayed >= maxComputedLoops)
				{
					DebugMode($"Max loops reached: {loopsPlayed} loops at {Time.time}");
					break;
				}
			}
			isPlaying = false;
			DebugMode($"Finished playing haptics at {Time.time} after {totalTimePlayed} seconds");
			playingCoroutine = null;
		}
	}
}