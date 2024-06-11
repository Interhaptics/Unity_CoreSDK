using UnityEngine;
using UnityEngine.UI; 

namespace Interhaptics.Samples
{
	public class DisplayFPS : MonoBehaviour
	{
		public Text fpsText; // Change TextMesh to TMP_Text
		private float deltaTime;

		void Update()
		{
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			float msec = deltaTime * 1000.0f;
			float fps = 1.0f / deltaTime;
			fpsText.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps); // This line remains the same
		}
	}
}
