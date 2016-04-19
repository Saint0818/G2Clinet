using UnityEngine;
using System.Collections;

public class UIInGameMissionView : MonoBehaviour {

	public bool IsUse;
	public bool IsFinish;
	public Animator FinishAnimator;
	public UILabel DescLabel;
	public GameObject AddTarget;

	void OnEnable() {
		AddTarget.SetActive(false);
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
		if(!DescLabel.text.Equals (desc))
			StartCoroutine(targetInvisible ());
		DescLabel.text = desc;

	}

	public void UpdateFin () {
		IsFinish = true;
		FinishAnimator.SetTrigger("Finish");
	}
		
	IEnumerator targetInvisible () {
		AddTarget.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		AddTarget.SetActive(false);
	}
}
