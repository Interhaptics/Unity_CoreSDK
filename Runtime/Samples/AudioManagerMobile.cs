/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using Interhaptics.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Interhaptics.Samples
{
	public class AudioManagerMobile : MonoBehaviour
	{
		[SerializeField]
		private List<AudioHapticSource> audioHapticSources;

		public void StopPlayingAudioHapticSources()
		{
			foreach (AudioHapticSource audioHapticSource in audioHapticSources)
			{
				if (audioHapticSource.audioSource.isPlaying)
				{
					audioHapticSource.audioSource.Stop();
					break;
				}
			}
		}
	}
}
