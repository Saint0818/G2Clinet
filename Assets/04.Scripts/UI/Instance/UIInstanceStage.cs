using System;
using System.Collections.Generic;
using GameStruct;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 副本章節內的關卡. 這對應到 UIInstanceStage.prefab.
/// </summary>
public class UIInstanceStage : MonoBehaviour
{
    public UILabel TitleLabel;
    public UILabel MoneyLabel;
    public UILabel ExpLabel;
    public UILabel RemainDailyLabel; // 關卡剩餘挑戰次數.
    public UIButton StartButton;
    public UILabel StartButtonLabel;
    public GameObject ClearIcon;
    public GameObject Mask;

    public GameObject PowerObj;
    [FormerlySerializedAs("StaminaLabel")]
    public UILabel PowerLabel;
    public GameObject DiamondObj;
    public UILabel DiamondLabel;

    public UILabel RewardTitle;
    public Transform[] RewardParents; // 獎勵圖示的位置.

    public enum EIcon
    {
        Power,
        Diamond
    }

    public class Data
    {
        public int ID;
        public string Title;
        public int Money;
        public int Exp;

        /// <summary>
        /// 還可以打幾次關卡, 也就是顯示還可以打幾次.
        /// </summary>
        public string RemainDailyCount;

        public UIStageVerification.EErrorCode ErrorCode;

        /// <summary>
        /// 不可進入關卡的錯誤訊息.
        /// </summary>
        public string ErrorMsg;

        public string StartButtonSprite;
        public string StartButtonText;

        /// <summary>
        /// 開始按鈕旁邊的圖示, 也就是按下開始按鈕後會扣的數值.
        /// </summary>
        public EIcon Icon;
        public int IconValue;

        /// <summary>
        /// true: 顯示半透明的遮照.(用在不能打的關卡上)
        /// </summary>
        public bool ShowMask;

        /// <summary>
        /// true: 右上角顯示 Clear 圖示.(表示關卡已通過)
        /// </summary>
        public bool ShowClear;

        /// <summary>
        /// 必定獲得 or 亂數獲得的文字.
        /// </summary>
        public string RewardTitle;

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
            if(mData.Icon == EIcon.Diamond)
            {
                UIMessage.Get.ShowMessage(TextConst.S(259), string.Format(TextConst.S(234), mData.IconValue), 
                () => UIInstance.Get.Main.NotifyStageStartClick(mData.ID, mData.ErrorCode, mData.ErrorMsg));
            }
            else
                UIInstance.Get.Main.NotifyStageStartClick(mData.ID, mData.ErrorCode, mData.ErrorMsg);
        }));
    }

    public void SetData(Data data)
    {
        mData = data;

        mHint.UpdateUI(data.ID);

        TitleLabel.text = data.Title;
        MoneyLabel.text = data.Money.ToString();
        ExpLabel.text = data.Exp.ToString();
        
        RemainDailyLabel.text = data.RemainDailyCount;

        StartButton.normalSprite = mData.StartButtonSprite;
        StartButton.GetComponent<UISprite>().spriteName = mData.StartButtonSprite;
        StartButtonLabel.text = mData.StartButtonText;

        PowerObj.SetActive(mData.Icon == EIcon.Power);
        PowerLabel.text = data.IconValue.ToString();
        DiamondObj.SetActive(mData.Icon == EIcon.Diamond);
        DiamondLabel.text = data.IconValue.ToString();

        ClearIcon.SetActive(data.ShowClear);
        Mask.SetActive(data.ShowMask);

        RewardTitle.text = data.RewardTitle;
        for (int i = 0; i < mRewardIcons.Count; i++)
        {
            if (data.RewardItems.Count > i)
                mRewardIcons[i].Show(data.RewardItems[i]);
            else
                mRewardIcons[i].Hide();
        }
    }
}