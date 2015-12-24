using UnityEngine;
using System.Collections;

public class SkillCardStar : MonoBehaviour {
	public UISprite Star;
	public float XSize;
	public void Awake () {
		if(Star == null){
			Star = transform.FindChild("Star").GetComponent<UISprite>();
		} 
		if(Star != null) 
			XSize = Star.width;

		HideStar();
	}

	public void Show () {
		gameObject.SetActive(true);
	}

	public void Hide () {
		gameObject.SetActive(false);
	}

	public void ShowStar () {
		if(Star != null)
			Star.gameObject.SetActive(true);
	}

	public void HideStar () {
		if(Star != null)
			Star.gameObject.SetActive(false);
	}

	public void SetQuality (int quality) {
		if(Star != null)
			Star.spriteName = "Staricon" + quality;
	}
}
