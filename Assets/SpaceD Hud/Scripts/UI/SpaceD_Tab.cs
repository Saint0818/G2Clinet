using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIToggle))]
[AddComponentMenu("SpaceD UI/Tab Button")]
public class SpaceD_Tab : MonoBehaviour {

	public UISprite tabSprite;
	public UILabel tabLabel;
	public GameObject targetContent;
	public SpaceD_Tab linkWith;

	private UIToggle toggle;
	private bool currentState = false;

	public string normalSprite;
	public string hoverSprite;
	public string activeSprite;

	public Color inactiveLabelColor = Color.white;
	public Color activeLabelColor = Color.white;
	public Color hoverLabelColor = Color.white;

	void Start()
	{
		this.toggle = this.GetComponent<UIToggle>();

		if (this.tabSprite == null)
			this.tabSprite = this.GetComponent<UISprite>();

		if (this.tabSprite != null)
			this.normalSprite = this.tabSprite.spriteName;

		if (this.tabLabel != null)
			this.inactiveLabelColor = this.tabLabel.color;

		if (this.toggle == null)
		{
			Debug.LogWarning(this.GetType() + " requires that you define UIToggle in order to work.", this);
			this.enabled = false;
			return;
		}

		if (this.targetContent == null)
		{
			Debug.LogWarning(this.GetType() + " requires that you define target GameObject to toggle.", this);
			this.enabled = false;
			return;
		}

		// Trigger on change just in case the NGUI call fails
		this.OnChange();

		// Hook the on change event
		this.toggle.onChange.Add(new EventDelegate(OnChange));
	}

	private void OnChange()
	{
		// Check if this tab is linked to another
		if (this.linkWith != null)
			this.linkWith.SetState(this.toggle.value);

		// Handle state change
		this.SetState(this.toggle.value);
	}

	void OnHover(bool isOver)
	{
		if (isOver)
		{
			if (this.tabLabel != null)
				this.tabLabel.color = this.hoverLabelColor;

			if (this.tabSprite != null && !string.IsNullOrEmpty(this.hoverSprite))
				this.tabSprite.spriteName = this.hoverSprite;
		}
		else
		{
			if (this.tabLabel != null)
				this.tabLabel.color = (this.currentState) ? this.activeLabelColor : this.inactiveLabelColor;

			if (this.tabSprite != null && !string.IsNullOrEmpty((this.currentState) ? this.activeSprite : this.normalSprite))
				this.tabSprite.spriteName = (this.currentState) ? this.activeSprite : this.normalSprite;
		}

		this.tabLabel.Update();
	}

	public void SetState(bool state)
	{
		if (this.toggle == null || this.targetContent == null)
			return;

		// Force the state on the toggle if necessary
		if (this.toggle.value != state)
			this.toggle.value = state;

		// Handle state change
		if (state)
		{
			this.targetContent.SetActive(true);

			// Nozmalize the panels depths
			NGUITools.NormalizePanelDepths();

			if (this.tabLabel != null)
			{
				this.tabLabel.color = this.activeLabelColor;
				this.tabLabel.Update();
			}

			if (this.tabSprite != null && !string.IsNullOrEmpty(this.activeSprite))
				this.tabSprite.spriteName = this.activeSprite;
		}
		else
		{
			this.targetContent.SetActive(false);

			if (this.tabLabel != null)
			{
				this.tabLabel.color = this.inactiveLabelColor;
				this.tabLabel.Update();
			}

			if (this.tabSprite != null && !string.IsNullOrEmpty(this.normalSprite))
				this.tabSprite.spriteName = this.normalSprite;
		}

		this.currentState = state;
	}

	public void Activate()
	{
		this.SetState(true);
	}
}
