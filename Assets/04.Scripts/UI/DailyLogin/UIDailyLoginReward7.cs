using UnityEngine;

public class UIDailyLoginReward7 : IDailyLoginReward
{
    public UILabel DayLabel;
    public UILabel NameLabel;
    public GameObject Clear;
    public ItemAwardGroup ItemAward;
    public GameObject SkillObj;

    public override void Set(Data data)
    {
        DayLabel.text = data.Day;
        NameLabel.text = data.Name;
        Clear.SetActive(data.ShowClear);

        ItemAward.gameObject.SetActive(data.ItemData.Kind != 21);
        SkillObj.SetActive(data.ItemData.Kind == 21);

        if(data.ItemData.Kind == 21)
        {
            var skillCard = new TActiveSkillCard();
            skillCard.Init(SkillObj);
            skillCard.UpdateViewItemData(data.ItemData);
        }
        else
            ItemAward.Show(data.ItemData);
    }
}