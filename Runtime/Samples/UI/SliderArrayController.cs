using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Interhaptics.Samples
{
	public class SliderArrayController : MonoBehaviour
	{
		public Slider[] sliders;  // Array of Sliders
		private int currentSliderIndex = 0;  // Index of the currently selected slider
		public Color activeColor = Color.red;  // Color for the active slider
		public Color activeKnobColor = new Color(0, 0, 0, 0.5f);
		public Color inactiveColor = Color.white;  // Color for inactive sliders
		public Color inactiveKnobColor = Color.white;
		private bool isTransitioning = false;  // Flag to check if a transition is already happening
		public float transitionDelay = 0.5f;  // Delay for transitions

		void Start()
		{
			// Initialize all sliders to the inactive color
			foreach (var slider in sliders)
			{
				SetSliderColor(slider, inactiveColor);
				SetKnobColor(slider, inactiveKnobColor);
			}
			// Highlight the first slider
			SetSliderColor(sliders[currentSliderIndex], activeColor);
			SetKnobColor(sliders[currentSliderIndex], activeKnobColor);
		}

		void Update()
		{
			// Cycle through sliders with the right stick vertical input
			if (!isTransitioning && Input.GetAxis("rightstick1vertical") > 0)
			{
				StartCoroutine(TransitionToNextSlider());
			}
			else if (!isTransitioning && Input.GetAxis("rightstick1vertical") < 0)
			{
				StartCoroutine(TransitionToPreviousSlider());
			}

			// Control the current slider with the left stick vertical input
			if (sliders[currentSliderIndex] != null)
			{
				float verticalInput = Input.GetAxis("leftstick1vertical");
				sliders[currentSliderIndex].value += verticalInput * Time.deltaTime;
			}
		}

		private IEnumerator TransitionToNextSlider()
		{
			isTransitioning = true;

			// Change previous slider to inactive color
			SetSliderColor(sliders[currentSliderIndex], inactiveColor);
			SetKnobColor(sliders[currentSliderIndex], inactiveKnobColor);

			// Move to the next slider
			currentSliderIndex = (currentSliderIndex + 1) % sliders.Length;

			yield return new WaitForSeconds(transitionDelay);

			// Change new active slider to active color
			SetSliderColor(sliders[currentSliderIndex], activeColor);
			SetKnobColor(sliders[currentSliderIndex], activeKnobColor);

			isTransitioning = false;
		}

		private IEnumerator TransitionToPreviousSlider()
		{
			isTransitioning = true;

			// Change previous slider to inactive color
			SetSliderColor(sliders[currentSliderIndex], inactiveColor);
			SetKnobColor(sliders[currentSliderIndex], inactiveKnobColor);

			// Move to the previous slider
			currentSliderIndex = (currentSliderIndex - 1 + sliders.Length) % sliders.Length;

			yield return new WaitForSeconds(transitionDelay);

			// Change new active slider to active color
			SetSliderColor(sliders[currentSliderIndex], activeColor);
			SetKnobColor(sliders[currentSliderIndex], activeKnobColor);

			isTransitioning = false;
		}

		private void SetSliderColor(Slider slider, Color color)
		{
			// Assuming the slider has an Image component
			var sliderImage = slider.GetComponentInChildren<Image>();
			if (sliderImage != null)
			{
				sliderImage.color = color;
			}
		}

		private void SetKnobColor(Slider slider, Color color)
		{
			slider.handleRect.GetComponent<Image>().color = color;
		}
	}
}