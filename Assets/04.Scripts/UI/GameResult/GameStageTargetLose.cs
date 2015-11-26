using UnityEngine;
using System.Collections;

public class GameStageTargetLose : MonoBehaviour {
	public bool IsComplete;

	public UILabel TargetLabel;
	public UILabel DescLabel;
	public UILabel CurrentLabel;
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
		TargetLabel.text = target;
		DescLabel.text = desc;
		CurrentLabel.text = current;
		GoalLabel.text = max;
		IsComplete = isComplete;
		FinTarget.SetActive(isFinish);
		LoseTarget.SetActive(isFinish);
		CurrentLabel.gameObject.SetActive(!(max.Equals("0") || max.Equals("/0")));
		GoalLabel.gameObject.SetActive(!(max.Equals("0") || max.Equals("/0")));
	}
	
	public void UpdateFin (bool isComplete) {
		if(isComplete)
			FinTarget.SetActive(true);
		else 
			LoseTarget.SetActive(true);
	}
}
