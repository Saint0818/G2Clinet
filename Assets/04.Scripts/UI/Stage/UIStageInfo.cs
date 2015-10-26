using JetBrains.Annotations;
using UnityEngine;

public class UIStageInfo : MonoBehaviour
{
    /// <summary>
    /// <para> 呼叫時機: 進入關卡按鈕按下時. </para>
    /// <para> 參數:(int StageID) </para>
    /// </summary>
    public event CommonDelegateMethods.Action1 StartListener;

    public class Data
    {
        public string Description { set; get; }
        public string KindSpriteName { set; get; }
        public string KindName { set; get; }
        public string RewardSpriteName { set; get; }
        public string RewardName { set; get; }
    }

    public GameObject Window;
    public UILabel DescriptionLabel;
    public UISprite KindSprite;
    public UILabel KindLabel;
    public UISprite RewardSprite;
    public UILabel RewardLabel;
    public UIStageHint2 Hint;

    private int mStageID;

    [UsedImplicitly]
	void Awake()
    {
	    Hide();
	}

    public void Show(int stageID, Data data)
    {
        gameObject.SetActive(true);
        Window.SetActive(true);
        mStageID = stageID;

        updateUI(data);

        Hint.UpdateUI(mStageID);
    }

    private void updateUI(Data data)
    {
        DescriptionLabel.text = data.Description;
        KindSprite.spriteName = data.KindSpriteName;
        KindLabel.text = data.KindName;
        RewardSprite.spriteName = data.RewardSpriteName;
        RewardLabel.text = data.RewardName;
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
