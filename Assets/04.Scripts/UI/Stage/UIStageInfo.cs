using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 關卡資訊視窗.
/// </summary>
public class UIStageInfo : MonoBehaviour
{
    /// <summary>
    /// <para> 呼叫時機: 進入關卡按鈕按下時. </para>
    /// <para> 參數:(int StageID) </para>
    /// </summary>
    public event CommonDelegateMethods.Int1 StartListener;

    public class Data
    {
        public string Name { set; get; }
        public string BgTextureName { set; get; } // 大張的背景圖名稱.
        public string Description { set; get; }
        public string KindSpriteName { set; get; }
        public string KindName { set; get; }

        /// <summary>
        /// <para> 顯示在右下角的獎勵. </para>
        /// todo 這邊暫時和遊戲資料耦合, 以後再改.
        /// </summary>
        public readonly List<TItemData> RewardItems = new List<TItemData>();

        public int Stamina { set; get; }

        /// <summary>
        /// 是否要顯示關卡打完的圖示.
        /// </summary>
        public bool ShowCompleted;

        /// <summary>
        /// 還可以打幾次關卡, 也就是顯示還可以打幾次.
        /// </summary>
        public int DailyCount;

        /// <summary>
        /// 開始按鈕可不可以點選.
        /// </summary>
        public bool StartEnable;
    }

    public GameObject Window;
    public UILabel NameLabel; // 關卡名稱.
    public UITexture BgTexture; // 背景的大圖片.
    public UILabel DescriptionLabel;
    public UISprite KindSprite;
    public UILabel KindLabel;
    public UILabel StaminaLabel;
    public Transform HintParent;
    public GameObject Completed; // 標示是否關卡打過的圖片.
    public UIButton StartButton; // 右下角的開始按鈕.
    public Transform[] RewardParents; // 獎勵圖示的位置.

    /// <summary>
    /// 每日關卡限制的圖片. [0]:最左邊, [1]:中間, [2]:最右邊.
    /// </summary>
    public UISprite[] DailyLimits;
    private UIStageHint mHint;

    private readonly List<ItemAwardGroup> mRewardIcons = new List<ItemAwardGroup>();

    private readonly string TexturePath = "Textures/Stage/StageKind/{0}";
    private readonly Color32 mDisableColor = new Color32(69, 69, 69, 255);

    private int mStageID;

    [UsedImplicitly]
	void Awake()
    {
        GameObject hintObj = Instantiate(Resources.Load<GameObject>("Prefab/UI/UIStageHint"));
        hintObj.transform.parent = HintParent;
        hintObj.transform.localPosition = Vector3.zero;
        hintObj.transform.localRotation = Quaternion.identity;
		hintObj.transform.localScale = new Vector3(0.9f, 0.9f, 1);
        mHint = hintObj.GetComponent<UIStageHint>();
        mHint.SetInterval(70);

        for(var i = 0; i < RewardParents.Length; i++)
        {
            var obj = UIPrefabPath.LoadUI(UIPrefabPath.ItemAwardGroup, RewardParents[i]);
            mRewardIcons.Add(obj.GetComponent<ItemAwardGroup>());
        }
    }

    public void Show(int stageID, Data data)
    {
        gameObject.SetActive(true);
        Window.SetActive(true);
        mStageID = stageID;

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

        for(int i = 0; i < mRewardIcons.Count; i++)
        {
            if(data.RewardItems.Count > i)
                mRewardIcons[i].Show(data.RewardItems[i]);
            else
                mRewardIcons[i].Hide();
        }

        StaminaLabel.text = string.Format("{0}", data.Stamina);
        Completed.SetActive(data.ShowCompleted);

        for(int i = 0; i < DailyLimits.Length; i++)
        {
            if(data.DailyCount > i)
                DailyLimits[i].color = Color.white;
            else
                DailyLimits[i].color = mDisableColor;
        }

        StartButton.isEnabled = data.StartEnable;
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    public void OnStartClick()
    {
        if(StartListener != null)
            StartListener(mStageID);
    }
}
