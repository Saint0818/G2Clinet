using UnityEngine;
using System.Collections;

public class SpaceD_CastBar : MonoBehaviour {

	[System.Serializable]
	public class ColorStage
	{
		public Color fillColor;
		public Color titleColor;
	}

	public UISprite fill;
	public UILabel titleLabel;
	public UILabel timeLabel;

	public ColorStage normalColors;
	public ColorStage onInterruptColors;
	
	public float hideDelay = 0.3f;

	private bool isCasting = false;
	public bool IsCasting { get { return this.isCasting; } }

	private float currentCastDuration = 0f;
	private float currentCastEndTime = 0f;

	void Start()
	{
		if (this.fill == null)
		{
			Debug.LogWarning(this.GetType() + " requires a fill sprite to be defined in order to work.", this);
			this.enabled = false;
			return;
		}
	
		// Apply the normal colors
		this.ApplyColorStage(this.normalColors);

		// Hide the cast bar
		this.SetAlpha(0f);
	}

	public void ApplyColorStage(ColorStage stage)
	{
		if (this.fill != null)
			this.fill.color = stage.fillColor;

		if (this.titleLabel != null)
			this.titleLabel.color = stage.titleColor;
	}

	public void Show()
	{
		this.SetAlpha(1f);
	}

	public void Hide()
	{
		this.SetAlpha(0f);
	}

	private void SetAlpha(float alpha)
	{
		UIWidget me = this.GetComponent<UIWidget>();
		if (me != null) me.alpha = alpha;

		UIWidget[] widgets = this.GetComponentsInChildren<UIWidget>();
		foreach (UIWidget w in widgets)
			w.alpha = alpha;
	}

	IEnumerator AnimateCast()
	{
		// Update the text
		if (this.timeLabel != null)
			this.timeLabel.text = "0.0 sec";
		
		// Get the timestamp
		float startTime = (this.currentCastEndTime > 0f) ? (this.currentCastEndTime - this.currentCastDuration) : Time.time;
		
		// Fade In the notification
		while (Time.time < (startTime + this.currentCastDuration))
		{
			float RemainingTime = (startTime + this.currentCastDuration) - Time.time;
			float ElapsedTime = this.currentCastDuration - RemainingTime;
			float ElapsedTimePct = ElapsedTime / this.currentCastDuration;
			
			// Update the elapsed cast time value
			if (this.timeLabel != null)
				this.timeLabel.text = ElapsedTime.ToString("0.0") + " sec";

			// Update the fill sprite
			this.fill.fillAmount = ElapsedTimePct;

			yield return 0;
		}

		// Make sure it's maxed
		if (this.timeLabel != null)
			this.timeLabel.text = this.currentCastDuration.ToString("0.0") + " sec";
		
		// Call that we finished
		this.OnFinishedCasting();
		
		// Hide with a delay
		this.StartCoroutine("DelayHide");
	}

	IEnumerator DelayHide()
	{
		// Wait for the hide delay
		yield return new WaitForSeconds(this.hideDelay);
		
		// Do not show the casting anymore
		this.Hide();
	}

	public void StartCasting(SpellInfo spellInfo, float duration, float endTime)
	{
		// Make sure we can start casting it
		if (this.isCasting)
			return;
		
		// Stop the coroutine might be still running on the hide delay
		this.StopCoroutine("AnimateCast");
		this.StopCoroutine("DelayHide");

		// Apply the normal colors
		this.ApplyColorStage(this.normalColors);

		// Change the fill pct
		this.fill.fillAmount = 0f;

		// Set the spell name
		if (this.titleLabel != null)
			this.titleLabel.text = spellInfo.Name;

		// Set some info about the cast
		this.currentCastDuration = duration;
		this.currentCastEndTime = endTime;

		// Define that we start casting animation
		this.isCasting = true;

		// Show the cast bar
		this.Show();
		
		// Start the cast animation
		this.StartCoroutine("AnimateCast");
	}

	public void Interrupt()
	{
		if (this.isCasting)
		{
			// Stop the coroutine if it's assigned
			this.StopCoroutine("AnimateCast");

			// No longer casting
			this.isCasting = false;

			// Apply the interrupt colors
			this.ApplyColorStage(this.onInterruptColors);

			// Hide with a delay
			this.StartCoroutine("DelayHide");
		}
	}
	
	private void OnFinishedCasting()
	{
		// Define that we are no longer casting
		this.isCasting = false;
	}
}
