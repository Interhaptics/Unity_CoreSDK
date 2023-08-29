/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interhaptics.Platforms.Mobile;

namespace Interhaptics.Samples
{
	public class MobileSceneGameManager : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] stiffnessSlider;
		[SerializeField]
		private MobileHapticsStiffness mobileHapticsStiffness;

		void Start()
		{
#if (!UNITY_IOS) || UNITY_EDITOR
			foreach (GameObject obj in stiffnessSlider)
			{
				obj.SetActive(false);
			}
			mobileHapticsStiffness.enabled = false;
#endif
		}
	}
}
