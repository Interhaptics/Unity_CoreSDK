/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using Interhaptics.Internal;
using Interhaptics.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interhaptics.Samples
{
	public class EventHapticManagerMobile : MonoBehaviour
	{
		[SerializeField]
		private List<EventHapticSource> eventHapticSources;

		public void StopPlayingEventHapticSources()
		{
			foreach (EventHapticSource eventHapticSource in eventHapticSources)
			{
				if (eventHapticSource.isPlaying)
				{
					eventHapticSource.Stop();
					break;
				}
			}
		}
	}
}
