using UnityEngine;
using System.Collections;

public class UIInGameMissionView : MonoBehaviour {

	public bool IsUse;
	public bool IsFinish;
	public Animator FinishAnimator;
	public UILabel DescLabel;
	public UILabel CurrentLabel;
	public UILabel GoalLabel;

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

	public void UpdateUI(string desc, string current, string max)
	{
		DescLabel.text = desc;
		CurrentLabel.text = current;
		GoalLabel.text = max;
		CurrentLabel.gameObject.SetActive(!(max.Equals("0") || max.Equals("/0")));
		GoalLabel.gameObject.SetActive(!(max.Equals("0") || max.Equals("/0")));
	}

	public void UpdateFin () {
		IsFinish = true;
		FinishAnimator.SetTrigger("Finish");
	}
}
