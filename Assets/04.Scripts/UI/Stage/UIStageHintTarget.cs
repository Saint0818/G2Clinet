using JetBrains.Annotations;
using UnityEngine;

public class UIStageHintTarget : MonoBehaviour
{
    public UILabel TargetLabel;
    public UILabel DescLabel;
    public UILabel CurrentLabel;
    public UILabel GoalLabel;
	public GameObject FinTarget;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

	public void MoveCurrentLabel () {
		if(DescLabel.localSize.x >= 170)
			CurrentLabel.gameObject.transform.localPosition = new Vector3(70, -15, 0);
		else
			CurrentLabel.gameObject.transform.localPosition = new Vector3(40, -15, 0);

	}

	public void MoveGoalLabel () {
		GoalLabel.gameObject.transform.localPosition = new Vector3(125, -15, 0);
	}

    /// <summary>
    /// 常見都是用 current/max 的方式來顯示; 但這部份剛好相反, 主要的原因是排版會不好看.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="desc"></param>
    /// <param name="current"> 放在介面的右邊. </param>
    /// <param name="max"> 在介面的左邊. </param>
    /// <param name="isFinish"></param>
	public void UpdateUI(string target, string desc, string current, string max, bool isFinish = false)
    {
        TargetLabel.text = target;
        DescLabel.text = desc;
        CurrentLabel.text = current;
        GoalLabel.text = max;
		FinTarget.SetActive(isFinish);
    }

	public void UpdateFin (bool isFinish) {
		FinTarget.SetActive(isFinish);
	}
}