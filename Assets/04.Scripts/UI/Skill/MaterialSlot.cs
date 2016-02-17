using UnityEngine;
using System.Collections;

public class MaterialSlot : MonoBehaviour {
	public GameObject InputFX;
	public UIButton RemoveBtn;
	public Transform View;
	public GameObject EatFX;

	void Awake () {
		InputFX.SetActive(false);
		RemoveBtn.gameObject.SetActive(false);
		EatFX.SetActive(false);
	}

	public void ShowInput () {
		InputFX.SetActive(true);
	}

	public void HideInput () {
		InputFX.SetActive(false);
	}

	public void ShowEatFX () {
		EatFX.SetActive(true);
	}

	public void HideEatFX () {
		EatFX.SetActive(false);
	}

}
