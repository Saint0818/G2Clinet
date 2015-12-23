using UnityEngine;

public class UIStageHintTarget : MonoBehaviour
{
    public UILabel DescLabel;
	public GameObject FinTarget;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 常見都是用 current/max 的方式來顯示; 但這部份剛好相反, 主要的原因是排版會不好看.
    /// </summary>
    /// <param name="desc"></param>
    /// <param name="isFinish"></param>
	public void UpdateUI(string desc, bool isFinish = false)
    {
        DescLabel.text = desc;
        if (FinTarget)
		    FinTarget.SetActive(isFinish);
    }

	public void UpdateFin (bool isFinish) {
		FinTarget.SetActive(isFinish);
	}
}