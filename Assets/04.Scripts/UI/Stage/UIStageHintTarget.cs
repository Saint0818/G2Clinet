using JetBrains.Annotations;
using UnityEngine;

public class UIStageHintTarget : MonoBehaviour
{
    public UILabel TargetLabel;
    public UILabel DescLabel;
    public UILabel CurrentLabel;
    public UILabel GoalLabel;

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

    public void UpdateUI(string target, string desc, string current, string max)
    {
        TargetLabel.text = target;
        DescLabel.text = desc;
        CurrentLabel.text = current;
        GoalLabel.text = max;
    }
}