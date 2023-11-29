/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

namespace Interhaptics.Utils
{
	using UnityEngine;

	public class ConditionalHideAttribute : PropertyAttribute
	{
		public string ConditionalSourceField { get; private set; }
		public bool HideInInspector { get; private set; }

		public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = false)
		{
			ConditionalSourceField = conditionalSourceField;
			HideInInspector = hideInInspector;
		}
	}
}
