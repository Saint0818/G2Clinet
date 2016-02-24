using UnityEngine;

public class UIDailyLoginReward : IDailyLoginReward
{
    public UILabel DayLabel;
    public UILabel NameLabel;
    public GameObject Clear;
    public ItemAwardGroup ItemAward;
    public GameObject ReceiveButton;

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
        ReceiveButton.SetActive(status == EStatus.Receivable);
    }
}
