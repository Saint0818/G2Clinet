using JetBrains.Annotations;
using UnityEngine;

public class UIStageInfo : MonoBehaviour
{
    /// <summary>
    /// <para> 呼叫時機: 進入關卡按鈕按下時. </para>
    /// <para> 參數:(int StageID) </para>
    /// </summary>
    public event CommonDelegateMethods.Action1 StartListener;

    public GameObject Window;

    private int mStageID;

    [UsedImplicitly]
	void Awake()
    {
	    Hide();
	}

    public void Show(int stageID)
    {
        Window.SetActive(true);
        mStageID = stageID;
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
