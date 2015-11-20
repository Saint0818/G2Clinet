using UnityEngine;
using System.Collections;

public class AwardBasket : MonoBehaviour {
	public Animator animatorAward;
	public GameObject FinishFX;

	private bool isChoosed = false;
	void Awake () {
		FinishFX.SetActive(false);
	}

	public void OnChoose () {
		if(!isChoosed) {
			animatorAward.SetTrigger("Finish");
			FinishFX.SetActive(true);
			isChoosed = true;
		}
	}
}
