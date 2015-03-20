using UnityEngine;
using System.Collections;

[AddComponentMenu("SpaceD UI/Step Bar")]
public class SpaceD_StepBar : MonoBehaviour {

	[System.Serializable]
	public class StepInfo
	{
		public float fillAmount;
		public UISprite separator;
	}

	public UIProgressBar progressbar;
	public UISprite bubble;
	public int startingStep = 0;
	public StepInfo[] steps;

	void Start()
	{
		if (this.progressbar == null)
		{
			Debug.LogWarning(this.GetType() + " requires a progressbar to be defined in order to work.", this);
			this.enabled = false;
			return;
		}

		this.SetStep(this.startingStep);
	}

	public void SetStep(int step)
	{
		if (this.progressbar == null || this.steps.Length == 0)
			return;

		if (step == 0)
		{
			this.progressbar.value = 0f;
			this.HideBubble();
		}
		else if (step > this.steps.Length)
		{
			this.progressbar.value = 1f;
			this.HideBubble();
		}
		else
		{
			StepInfo stepInfo = this.steps[step - 1];

			this.progressbar.value = stepInfo.fillAmount;

			// Move the buddle on the current step
			if (this.bubble != null && stepInfo.separator != null)
			{
				this.bubble.cachedTransform.localPosition = new Vector3(stepInfo.separator.cachedTransform.localPosition.x, this.bubble.cachedTransform.localPosition.y, this.bubble.cachedTransform.localPosition.z);

				UILabel bubbleLabel = this.bubble.GetComponentInChildren<UILabel>();

				if (bubbleLabel != null)
					bubbleLabel.text = step.ToString();

				this.ShowBubble();
			}
		}
	}

	private void HideBubble()
	{
		if (this.bubble != null)
		{
			this.bubble.alpha = 0f;
		}
	}

	private void ShowBubble()
	{
		if (this.bubble != null)
		{
			this.bubble.alpha = 1f;
		}
	}

	[ContextMenu("Test")]
	public void Test()
	{
		this.SetStep(this.startingStep);
	}
}
