using UnityEngine;
using System.Collections;

[AddComponentMenu("SpaceD UI/Star")]
[RequireComponent(typeof(UISprite))]
public class SpaceD_Star : MonoBehaviour {
	
	public const string normalSprite = "Star_Small";
	public const string emptySprite = "Star_Small_Empty";

	private UISprite sprite;
	public bool makePixelPerfect = true;
	public bool startEmpty = false;

	void Start()
	{
		this.sprite = this.GetComponent<UISprite>();

		// Apply starting state
		this.SetEmpty(this.startEmpty);
	}
	
	public void SetEmpty(bool state)
	{
		if (!this.enabled || this.sprite == null)
			return;

		this.sprite.spriteName = (state) ? emptySprite : normalSprite;

		if (this.makePixelPerfect)
			this.sprite.MakePixelPerfect();
	}
}
