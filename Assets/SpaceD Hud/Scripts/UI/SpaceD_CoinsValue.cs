using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel))]
[ExecuteInEditMode()]
public class SpaceD_CoinsValue : MonoBehaviour {

	public int coinsValue = 0;
	public int defaultLength = 4;

	private UILabel label;
	private int lastValue;

	private const string ZeroColor = "494f54";
	private const string OtherColor = "7c8389";

	void Start()
	{
		if (this.label == null)
			this.label = this.GetComponent<UILabel>();
	}

	void Update()
	{
		// Check if the value has been processed
		if (this.lastValue != this.coinsValue)
		{
			string text = "";
			int len = (this.coinsValue > 0) ? this.coinsValue.ToString().Length : 0;

			// Check if we're over our length
			if (len > this.defaultLength)
			{
				text += "[" + OtherColor + "]" + this.Formatted(this.coinsValue) + "[-]";
			}
			else
			{
				// Open up the color tag
				text += "[" + ZeroColor + "]";

				// Add the zeros
				for (int i = 0; i < (this.defaultLength - len); i++)
					text += "0";

				// Close the color tag
				text += "[-]";

				// Add the new value if there is any
				if (this.coinsValue > 0)
					text += "[" + OtherColor + "]" + this.coinsValue.ToString() + "[-]";
			}

			// Set the new text to the label
			this.label.text = text;

			// Save as processed
			this.lastValue = this.coinsValue;
		}
	}

	private string Formatted(int value)
	{
		if (value >= 1000000)
			return ((float)value / 1000000.0f).ToString("0.0") + "m";
		else if (value >= 1000)
			return ((float)value / 1000.0f).ToString("0.0") + "k";

		return value.ToString();
	}
}
