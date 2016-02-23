using UnityEngine;

public class UIDailyLoginReward : IDailyLoginReward
{
    public UILabel DayLabel;
    public UILabel NameLabel;
    public GameObject Clear;
    public ItemAwardGroup ItemAward;

    public override void Set(Data data)
    {
        DayLabel.text = data.Day;
        NameLabel.text = data.Name;
        Clear.SetActive(data.ShowClear);
        ItemAward.Show(data.ItemData);
    }
}
