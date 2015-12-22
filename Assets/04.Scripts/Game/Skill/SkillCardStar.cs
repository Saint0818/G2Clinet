using UnityEngine;
using System.Collections;

public class SkillCardStar : MonoBehaviour {
	public GameObject Star;
	public void Awake () {
		Hide();
	}

	public void Show () {
		Star.SetActive(true);
	}

	public void Hide () {
		Star.SetActive(false);
	}

}
