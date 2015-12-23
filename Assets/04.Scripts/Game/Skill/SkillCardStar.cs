using UnityEngine;
using System.Collections;

public class SkillCardStar : MonoBehaviour {
	public UISprite Star;
	public void Awake () {
		if(Star == null)
			Star = transform.FindChild("Star").GetComponent<UISprite>();
		Hide();
	}

	public void Show () {
		if(Star != null)
			Star.gameObject.SetActive(true);
	}

	public void Hide () {
		if(Star != null)
			Star.gameObject.SetActive(false);
	}

	public void SetQuality (int quality) {
		if(Star != null)
			Star.spriteName = "Staricon" + quality;
	}
}
