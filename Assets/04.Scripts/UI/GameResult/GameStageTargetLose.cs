using UnityEngine;
using System.Collections;

public class GameStageTargetLose : MonoBehaviour {
	public bool IsComplete;

	public UILabel DescLabel;
	public UILabel GoalLabel;
	public GameObject FinTarget;
	public GameObject LoseTarget;
	
	public void Show()
	{
		gameObject.SetActive(true);
	}
	
	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void UpdateUI(string target, string desc, string current, string max, bool isComplete, bool isFinish = false)
	{
		DescLabel.text = desc;
		GoalLabel.text = "[FFDD33FF]"+ current +"[-]" + max;
		IsComplete = isComplete;
		FinTarget.SetActive(isFinish);
		LoseTarget.SetActive(isFinish);
	}
	
	public void UpdateFin (bool isComplete) {
		if(isComplete)
			FinTarget.SetActive(true);
		else 
			LoseTarget.SetActive(true);
	}
}
