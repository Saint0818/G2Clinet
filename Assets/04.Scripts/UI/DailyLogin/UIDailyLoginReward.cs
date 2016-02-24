using UnityEngine;

/// <summary>
/// 第 1 ~ 6 天的每日登入獎勵.
/// </summary>
public class UIDailyLoginReward : DailyLoginReward
{
    public UILabel DayLabel;
    public UILabel NameLabel;
    public GameObject Clear;
    public ItemAwardGroup ItemAward;
    public UIButton ReceiveButton;

    private void Start()
    {
        var main = GetComponentInParent<UIDailyLoginMain>();
        ReceiveButton.onClick.Add(new EventDelegate(main.FireReceiveClick));
    }

    public override void Set(Data data)
    {
        DayLabel.text = data.Day;
        NameLabel.text = data.Name;
        ItemAward.Show(data.ItemData);

        updateStatus(data.Status);
    }

    private void updateStatus(EStatus status)
    {
        Clear.SetActive(status == EStatus.Received);
        ReceiveButton.gameObject.SetActive(status == EStatus.Receivable);
    }
}
