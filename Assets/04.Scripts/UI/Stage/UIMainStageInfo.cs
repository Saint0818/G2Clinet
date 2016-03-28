using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 主線關卡的資訊視窗.
/// </summary>
public class UIMainStageInfo : MonoBehaviour
{
    /// <summary>
    /// <para> 呼叫時機: 進入關卡按鈕按下時. </para>
    /// <para> 參數:(int StageID) </para>
    /// <para> 參數:(EErrorCode) </para>
    /// <para> 參數:(string errMsg) </para>
    /// </summary>
    public event Action<int, UIStageVerification.EErrorCode, string> StartListener;

    public class Data
    {
        public string Name { set; get; }
        public string BgTextureName { set; get; } // 大張的背景圖名稱.
        public string Description { set; get; }
        public string KindSpriteName { set; get; }
        public string KindName { set; get; }

        public string RewardTitle { set; get; }
        /// <summary>
        /// <para> 顯示該關卡會得到的獎勵. </para>
        /// todo 這邊暫時和遊戲資料耦合, 以後再改.
        /// </summary>
        public readonly List<TItemData> RewardItems = new List<TItemData>();

        public int Money { get; set; }

        /// <summary>
        /// 要不要顯示經驗值.
        /// </summary>
        public bool ExpVisible; 
        public int Exp { get; set; }

        public int Power { set; get; }
        public int Diamond { set; get; }

        /// <summary>
        /// 是否要顯示關卡打完的圖示.
        /// </summary>
        public bool ShowCompleted;

        /// <summary>
        /// 還可以打幾次關卡, 也就是顯示還可以打幾次.
        /// </summary>
        public string RemainDailyCount;

        public UIStageVerification.EErrorCode ErrorCode;
        public string ErrorMsg;

        public string StartButtonSprite;
        public string StartButtonText;

        public bool MissionVisible;
        public string MissionTitle;
        public string MissionDesc;
        public int MissionCurrentValue; // 任務目前的數值.
        public int MissionGoalValue; // 達成任務所需的數值.
        public Action MissionAction; // 任務的前往按鈕按下後會執行的行為.
    }

    public GameObject Window;
    public UILabel NameLabel; // 關卡名稱.
    public UITexture BgTexture; // 背景的大圖片.
    public UILabel DescriptionLabel;
    public UISprite KindSprite;
    public UILabel KindLabel;
    
    public GameObject Completed; // 標示是否關卡打過的圖片.
    public UIButton StartButton; // 右下角的開始按鈕.
    public UILabel StartButtonLabel; // 右下角的開始按鈕.
    public Transform[] RewardParents; // 獎勵圖示的位置.
    public UILabel RewardMoney;
    public GameObject ExpObj; // 控制經驗值要不要顯示.
    public UILabel RewardExp;
    public UILabel RewardTitle;
    public UILabel DailyCount; // 每日限制的數值.
    public GameObject MissionObj;
    public UILabel MissionTitle;
    public UILabel MissionDesc;
    public UILabel MissionValue;
    public UISlider MissionProgress;
    public UIButton MissionButton;

    // 開始按鈕旁邊圖示和數值.
    public GameObject PowerObj;
    [FormerlySerializedAs("StaminaLabel")]
    public UILabel PowerLabel;
    public GameObject DiamondObj;
    public UILabel DiamondLabel;

    private readonly List<ItemAwardGroup> mRewardIcons = new List<ItemAwardGroup>();

    private readonly string TexturePath = "Textures/Stage/StageKind/{0}";

    public int StageID { get { return mStageID; } }
    private int mStageID;
    private UIStageVerification.EErrorCode mErrorCode;
    private string mErrMsg;

    private UIStageHint mHint;
    private Action mMissionAction;

    [UsedImplicitly]
	void Awake()
    {
        mHint = GetComponent<UIStageHint>();

        for(var i = 0; i < RewardParents.Length; i++)
        {
            var obj = UIPrefabPath.LoadUI(UIPrefabPath.ItemAwardGroup, RewardParents[i]);
            mRewardIcons.Add(obj.GetComponent<ItemAwardGroup>());
        }

        StartButton.onClick.Add(new EventDelegate(() =>
        {
            if(StartListener != null)
                StartListener(mStageID, mErrorCode, mErrMsg);
        }));

        MissionButton.onClick.Add(new EventDelegate(() =>
        {
            if(mMissionAction != null)
                mMissionAction();
        }));
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    public void Show(int stageID, Data data)
    {
        gameObject.SetActive(true);
        Window.SetActive(true);

        mStageID = stageID;
        mErrorCode = data.ErrorCode;
        mErrMsg = data.ErrorMsg;

        updateUI(data);

        mHint.UpdateUI(mStageID);
    }

    private void updateUI(Data data)
    {
        NameLabel.text = data.Name;
        BgTexture.mainTexture = Resources.Load<Texture2D>(string.Format(TexturePath, data.BgTextureName));
        DescriptionLabel.text = data.Description;
        KindSprite.spriteName = data.KindSpriteName;
        KindLabel.text = data.KindName;

        RewardTitle.text = data.RewardTitle;
        for(int i = 0; i < mRewardIcons.Count; i++)
        {
            if(data.RewardItems.Count > i)
                mRewardIcons[i].Show(data.RewardItems[i]);
            else
                mRewardIcons[i].Hide();
        }

        PowerObj.SetActive(data.Power > 0);
        PowerLabel.text = string.Format("{0}", data.Power);
        DiamondObj.SetActive(data.Diamond > 0);
        DiamondLabel.text = string.Format("{0}", data.Diamond);

        Completed.SetActive(data.ShowCompleted);

        StartButton.normalSprite = data.StartButtonSprite;
        StartButton.GetComponent<UISprite>().spriteName = data.StartButtonSprite;
        StartButtonLabel.text = data.StartButtonText;

        RewardMoney.text = string.Format("{0}", data.Money);

        ExpObj.SetActive(data.ExpVisible);
        RewardExp.text = string.Format("{0}", data.Exp);

        DailyCount.text = data.RemainDailyCount;

        MissionObj.SetActive(data.MissionVisible);
        MissionTitle.text = data.MissionTitle;
        MissionDesc.text = data.MissionDesc;
        MissionValue.text = string.Format("{0}/{1}", data.MissionCurrentValue, data.MissionGoalValue);
        MissionProgress.value = data.MissionCurrentValue / (float)data.MissionGoalValue;
        mMissionAction = data.MissionAction;
    }

    public void Hide()
    {
        Window.SetActive(false);
    }
}
