using UnityEngine;
using System.Collections;

public class MaterialSlot : MonoBehaviour {
	public GameObject InputFX;
	public UIButton RemoveBtn;
	public Transform View;
	public GameObject EatFX;

	private float inputTime;
	private float eatTime;

	void Awake () {
		InputFX.SetActive(false);
		RemoveBtn.gameObject.SetActive(false);
		EatFX.SetActive(false);
	}

	void FixedUpdate () {
		if(inputTime > 0) {
			inputTime -= Time.deltaTime;
			if(inputTime <= 0) {
				inputTime = 0;
				InputFX.SetActive(false);
			}
		}

		if(eatTime > 0) {
			eatTime -= Time.deltaTime;
			if(eatTime <= 0) {
				eatTime = 0;
				EatFX.SetActive(false);
			}
		}
	}

	public void ShowInput () {
		inputTime = 0.3f;
		InputFX.SetActive(true);
	}

	public void ShowEatFX () {
		eatTime = 0.8f;
		EatFX.SetActive(true);
	}
}
