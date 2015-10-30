using JetBrains.Annotations;
using UnityEngine;

public class UIStageHintTarget : MonoBehaviour
{
    public UILabel TargetLabel;
    public UILabel DescLabel;
    public UILabel CurrentLabel;
    public UILabel GoalLabel;
	public GameObject FinTarget;

    [UsedImplicitly]
    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

	public void UpdateUI(string target, string desc, string current, string max, bool isFinish = false)
    {
        TargetLabel.text = target;
        DescLabel.text = desc;
        CurrentLabel.text = current;
        GoalLabel.text = max;
		FinTarget.SetActive(isFinish);
    }
}