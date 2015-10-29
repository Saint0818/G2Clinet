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
    public event CommonDelegateMethods.Action1 StartListener;

    public class Data
    {
        public string Name { set; get; }
        public string Description { set; get; }
        public string KindSpriteName { set; get; }
        public string KindName { set; get; }
        public string RewardSpriteName { set; get; }
        public string RewardName { set; get; }
        public int Stamina { set; get; }
    }

    public GameObject Window;
    public UILabel NameLabel; // 關卡名稱.
    public UILabel DescriptionLabel;
    public UISprite KindSprite;
    public UILabel KindLabel;
    public UISprite RewardSprite;
    public UILabel RewardLabel;
    public UILabel StaminaLabel;
    public Transform HintParent;
    private UIStageHint2 mHint;

    private int mStageID;

    [UsedImplicitly]
	void Awake()
    {
	    Hide();

        GameObject hintObj = Instantiate(Resources.Load<GameObject>("Prefab/UI/UIStageHint2"));
        hintObj.transform.parent = HintParent;
        hintObj.transform.localPosition = Vector3.zero;
        hintObj.transform.localRotation = Quaternion.identity;
        hintObj.transform.localScale = Vector3.one;
        mHint = hintObj.GetComponent<UIStageHint2>();
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
        DescriptionLabel.text = data.Description;
        KindSprite.spriteName = data.KindSpriteName;
        KindLabel.text = data.KindName;
        RewardSprite.spriteName = data.RewardSpriteName;
        RewardLabel.text = data.RewardName;
        StaminaLabel.text = string.Format("{0}", data.Stamina);
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
