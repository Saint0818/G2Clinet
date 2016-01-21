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

    public Transform[] RewardParents; // 獎勵圖示的位置.

    public class Data
    {
        public int ID;
        public string Title;
        public int Money;
        public int Exp;

        /// <summary>
        /// <para> 顯示該關卡會得到的獎勵. </para>
        /// todo 這邊暫時和遊戲資料耦合, 以後再改.
        /// </summary>
        public readonly List<TItemData> RewardItems = new List<TItemData>();
    }

    private readonly List<ItemAwardGroup> mRewardIcons = new List<ItemAwardGroup>();
    private UIStageHint mHint;
    private void Awake()
    {
        mHint = GetComponent<UIStageHint>();

        for (var i = 0; i < RewardParents.Length; i++)
        {
            var obj = UIPrefabPath.LoadUI(UIPrefabPath.ItemAwardGroup, RewardParents[i]);
            mRewardIcons.Add(obj.GetComponent<ItemAwardGroup>());
        }
    }

    public void SetData(Data data)
    {
        mHint.UpdateUI(data.ID);

        TitleLabel.text = data.Title;
        MoneyLabel.text = data.Money.ToString();
        ExpLabel.text = data.Exp.ToString();

        for(int i = 0; i < mRewardIcons.Count; i++)
        {
            if (data.RewardItems.Count > i)
                mRewardIcons[i].Show(data.RewardItems[i]);
            else
                mRewardIcons[i].Hide();
        }
    }
}