using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Chronos.Example
{
	// Utility class to controll a Global clock's time scale with a slider.
	[RequireComponent(typeof(Slider))]
	public class ExampleSlider : MonoBehaviour
	{
		public GlobalClock clock;
		public Text text;

		Slider slider
		{
			get { return GetComponent<Slider>(); }
		}

		void Start()
		{
			slider.onValueChanged.AddListener(OnValueChanged);
		}

		public void OnValueChanged(float value)
		{
			// Change the global clock's time scale

			clock.localTimeScale = value;
		}

		void Update()
		{
			// Update the slider value

			slider.value = clock.localTimeScale;

			// Update the label text

			string sign;
			float value = clock.localTimeScale;

			if (clock.parent == null)
			{
				sign = "=";
			}
			else if (clock.parentBlend == ClockBlend.Multiplicative)
			{
				sign = "×";
			}
			else // if (clock.parentBlend == ClockBlend.Additive)
			{
				sign = clock.localTimeScale >= 0 ? "+" : "-";
				value = Mathf.Abs(value);
			}

			text.text = string.Format("{0} ({1} {2:0.0} = {3:0.0})", clock.key, sign, value, clock.timeScale);
		}
	}

}