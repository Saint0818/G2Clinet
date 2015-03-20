using UnityEngine;
using System.Collections;

public class SpaceD_Notification : MonoBehaviour {

	private static SpaceD_Notification mInstance;

	public enum MarkType
	{
		None,
		Exclamation,
		Question,
	}

	public UIWidget container;
	public UILabel headline;
	public UILabel text;

	public UISprite mark;
	public MarkType markType = MarkType.Exclamation;
	[SerializeField] private bool markPulse = true;
	public float markPulseSpeed = 5f;
	private float markPulseTargetAlpha = 0f;

	public bool useFading = true;
	public float fadeDuration = 0.5f;

	public bool isVisible { get; private set; }

	void Awake()
	{
		if (mInstance == null)
			mInstance = this;
	}

	void OnDestroy()
	{
		mInstance = null;
	}

	void Start()
	{
		if (this.container == null)
			this.container = this.GetComponent<UIWidget>();

		this.isVisible = (this.container.alpha > 0f) ? true : false;

		if (this.mark == null)
			this.mark = this.GetComponentInChildren<UISprite>();

		this.SetMark(this.markType);
	}

	void Update()
	{
		if (!this.isVisible)
			return;

		if (this.markPulse && this.mark != null)
		{
			this.mark.alpha = Mathf.Lerp(this.mark.alpha, this.markPulseTargetAlpha, Time.deltaTime * this.markPulseSpeed);

			if (this.mark.alpha <= 0.01f)
			{
				this.markPulseTargetAlpha = 1f;
			}
			else if (this.mark.alpha >= 0.99f)
			{
				this.markPulseTargetAlpha = 0f;
			}
		}
	}

	public void SetMarkPulse(bool enable)
	{
		this.markPulse = enable;

		if (!enable && this.mark != null)
			this.mark.alpha = 1f;
	}

	public void SetMark(MarkType type)
	{
		if (this.mark == null)
			return;

		if (type == MarkType.None)
		{
			// Disable the sprite
			this.mark.enabled = false;
		}
		else
		{
			// Enable the sprite
			this.mark.enabled = true;

			// Switch the sprite
			switch (type)
			{
			case MarkType.Exclamation:
				this.mark.spriteName = "Notification_ExclamationMark";
				break;
			case MarkType.Question:
				this.mark.spriteName = "Notification_QuestionMark";
				break;
			}

			// Update rect
			this.mark.MakePixelPerfect();
		}

		// Update the mark type variable
		this.markType = type;
	}

	public void SetHeadline(string headline)
	{
		if (this.headline != null)
			this.headline.text = headline;
	}

	public void SetText(string text)
	{
		if (this.text != null)
			this.text.text = text;
	}

	[ContextMenu("Show")]
	public void Show()
	{
		this.isVisible = true;

		// Fade In
		this.StopCoroutine("FadeOut");
		this.StartCoroutine("FadeIn");
	}

	[ContextMenu("Hide")]
	public void Hide()
	{
		// Fade out
		this.StopCoroutine("FadeIn");
		this.StartCoroutine("FadeOut");
	}

	private float currentAlpha { get { return this.container.alpha; } }
	
	// Show / Hide fade animation coroutine
	private IEnumerator FadeIn()
	{
		// Get the timestamp
		float startTime = Time.time;
		
		// Calculate the time we need to fade in from the current alpha
		float internalDuration = (this.fadeDuration * (1f - this.currentAlpha));
		
		// Update the start time
		startTime -= (this.fadeDuration - internalDuration);
		
		// Fade In
		while (Time.time < (startTime + this.fadeDuration))
		{
			float RemainingTime = (startTime + this.fadeDuration) - Time.time;
			float ElapsedTime = this.fadeDuration - RemainingTime;
			
			// Update the alpha by the percentage of the time elapsed
			this.container.alpha = (ElapsedTime / this.fadeDuration);
			
			yield return 0;
		}
		
		// Make sure it's 1
		this.container.alpha = 1.0f;
	}
	
	// Show / Hide fade animation coroutine
	private IEnumerator FadeOut()
	{
		// Get the timestamp
		float startTime = Time.time;
		
		// Calculate the time we need to fade out from the current alpha
		float internalDuration = (this.fadeDuration * this.currentAlpha);
		
		// Update the start time
		startTime -= (this.fadeDuration - internalDuration);
		
		// Fade In
		while (Time.time < (startTime + this.fadeDuration))
		{
			float RemainingTime = (startTime + this.fadeDuration) - Time.time;
			
			// Update the alpha by the percentage of the time elapsed
			this.container.alpha = (RemainingTime / this.fadeDuration);
			
			yield return 0;
		}
		
		// Make sure it's 0
		this.container.alpha = 0f;
		this.isVisible = false;
	}
}
