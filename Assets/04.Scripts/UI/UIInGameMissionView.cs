using UnityEngine;
using System.Collections;

public class UIInGameMissionView : MonoBehaviour {

	public bool IsUse;
	public bool IsFinish;
	public Animator FinishAnimator;
	public UILabel DescLabel;
	public GameObject AddTarget;

	private bool isRecordUpdate = false;
	private bool isRecordFin = false;

	void OnEnable() {
		AddTarget.SetActive(false);
		if(isRecordUpdate) {
			StartCoroutine(targetInvisible ());
			isRecordUpdate = false;
		}

		if(isRecordFin) {
			FinishAnimator.SetTrigger("Finish");
			isRecordFin = false;
		}
	}

	public void Show()
	{
		IsUse = true;
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		IsUse = false;
		IsFinish = false;
		gameObject.SetActive(false);
	}

	public void UpdateUI(string desc)
	{
		if(!DescLabel.text.Equals (desc)) {
			if(gameObject.activeInHierarchy)
				StartCoroutine(targetInvisible ());
			else
				isRecordUpdate = true;
		}
		
		DescLabel.text = desc;

	}

	public void UpdateFin () {
		IsFinish = true;
		if(gameObject.activeInHierarchy)
			FinishAnimator.SetTrigger("Finish");
		else
			isRecordFin = true;
	}
		
	IEnumerator targetInvisible () {
		AddTarget.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		AddTarget.SetActive(false);
	}
}
