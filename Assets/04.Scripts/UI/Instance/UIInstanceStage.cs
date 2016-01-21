using System.Collections.Generic;
using GameStruct;
using UnityEngine;

/// <summary>
/// 副本章節內的關卡. 這對應到 UIInstanceStage.prefab.
/// </summary>
public class UIInstanceStage : MonoBehaviour
{
    public UILabel TitleLabel;
    public UILabel MoneyLabel;
    public UILabel ExpLabel;
    public UILabel StaminaLabel; // 體力.
    public UILabel RemainDailyLabel; // 關卡剩餘挑戰次數.
    public UIButton StartButton;

    public Transform[] RewardParents; // 獎勵圖示的位置.

    public class Data
    {
        public int ID;
        public string Title;
        public int Money;
        public int Exp;

        /// <summary>
        /// 進入關卡所需的體力.
        /// </summary>
        public int Stamina;

        /// <summary>
        /// 還可以打幾次關卡, 也就是顯示還可以打幾次.
        /// </summary>
        public string RemainDailyCount;

        /// <summary>
        /// 是否可以進入關卡.
        /// </summary>
        public bool StartEnable;

        /// <summary>
        /// 不可進入關卡的錯誤訊息.
        /// </summary>
        public string ErrorMsg;

        /// <summary>
        /// <para> 顯示該關卡會得到的獎勵. </para>
        /// todo 這邊暫時和遊戲資料耦合, 以後再改.
        /// </summary>
        public readonly List<TItemData> RewardItems = new List<TItemData>();
    }

    private readonly List<ItemAwardGroup> mRewardIcons = new List<ItemAwardGroup>();
    private UIStageHint mHint;
    private Data mData;

    private void Awake()
    {
        mHint = GetComponent<UIStageHint>();

        for (var i = 0; i < RewardParents.Length; i++)
        {
            var obj = UIPrefabPath.LoadUI(UIPrefabPath.ItemAwardGroup, RewardParents[i]);
            mRewardIcons.Add(obj.GetComponent<ItemAwardGroup>());
        }

        StartButton.onClick.Add(new EventDelegate(() =>
        {
            if(mData.StartEnable)
                UIInstance.Get.Main.NotifyStageStartClick(mData.ID);
            else
            {
                Debug.LogWarning(mData.ErrorMsg);
                UIHint.Get.ShowHint(mData.ErrorMsg, Color.green);
            }
        }));
    }

    public void SetData(Data data)
    {
        mData = data;

        mHint.UpdateUI(data.ID);

        TitleLabel.text = data.Title;
        MoneyLabel.text = data.Money.ToString();
        ExpLabel.text = data.Exp.ToString();
        StaminaLabel.text = data.Stamina.ToString();
        RemainDailyLabel.text = data.RemainDailyCount;

        StartButton.normalSprite = UIBase.ButtonBG(data.StartEnable);
        StartButton.GetComponent<UISprite>().spriteName = UIBase.ButtonBG(data.StartEnable);

        for (int i = 0; i < mRewardIcons.Count; i++)
        {
            if (data.RewardItems.Count > i)
                mRewardIcons[i].Show(data.RewardItems[i]);
            else
                mRewardIcons[i].Hide();
        }
    }
}