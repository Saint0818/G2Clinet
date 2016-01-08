using UnityEngine;

public class UIInGameMissionView : MonoBehaviour {

	public bool IsUse;
	public bool IsFinish;
	public Animator FinishAnimator;
	public UILabel DescLabel;

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
		DescLabel.text = desc;
	}

	public void UpdateFin () {
		IsFinish = true;
		FinishAnimator.SetTrigger("Finish");
	}
}
