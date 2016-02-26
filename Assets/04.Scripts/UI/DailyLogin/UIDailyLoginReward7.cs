using UnityEngine;

/// <summary>
/// 第 7 天的每日登入獎勵.
/// </summary>
public class UIDailyLoginReward7 : DailyLoginReward
{
    public UILabel DayLabel;
    public UILabel NameLabel;
    public GameObject Clear;
    public ItemAwardGroup ItemAward;
    public GameObject SkillObj;
    public UIButton ReceiveButton;

    private UIButton mSkillButton;

    private Data mData;

    private void Start()
    {
        var main = GetComponentInParent<UIDailyLoginMain>();
        ReceiveButton.onClick.Add(new EventDelegate(main.FireReceiveClick));

        mSkillButton = SkillObj.GetComponent<UIButton>();
        mSkillButton.onClick.Add(new EventDelegate(() =>
        {
            UIItemHint.Get.OnShow(mData.ItemData.ID);
        }));
    }

    public override void Set(Data data)
    {
        mData = data;

        DayLabel.text = data.Day;

        // 21 表示道具是技能卡.
        NameLabel.gameObject.SetActive(data.ItemData.Kind != 21);
        NameLabel.text = data.Name;

        updateStatus(data.Status);

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

    private void updateStatus(UIDailyLoginMain.EStatus status)
    {
        Clear.SetActive(status == UIDailyLoginMain.EStatus.Received);
        ReceiveButton.gameObject.SetActive(status == UIDailyLoginMain.EStatus.Receivable);
    }
}