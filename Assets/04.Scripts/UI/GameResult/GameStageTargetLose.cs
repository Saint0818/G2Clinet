using UnityEngine;

public class GameStageTargetLose : MonoBehaviour {
	public bool IsComplete;

	public UILabel DescLabel;
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

	public void UpdateUI(string desc, bool isComplete, bool isFinish = false)
	{
		DescLabel.text = desc;
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
