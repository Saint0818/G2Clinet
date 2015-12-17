using JetBrains.Annotations;
using UnityEngine;

public class UIStageHint : MonoBehaviour
{
    public GameObject Window;
	public UIAnchor WindowAnchor;

    private UIStageHintTarget[] mTargets;

    [UsedImplicitly]
    private void Awake()
    {
        mTargets = GetComponentsInChildren<UIStageHintTarget>();
    }

    /// <summary>
    /// 設定目標的間隔.
    /// </summary>
    /// <param name="xInterval"> 單位: Pixel. </param>
    /// <param name="yInterval"> 單位: Pixel. </param>
	public void UpdateInterval(float xInterval, float yInterval)
    {
        // 因為介面的配置是左邊有 3 個目標, 右邊有 3 個目標, 所以才有以下的魔術數字 3 和 6.

        // 左邊 3 個目標.
        for(int i = 0; i < 3; i++)
        {
            var localPos = mTargets[0].transform.localPosition;
            localPos.y -= yInterval * i;
            mTargets[i].transform.localPosition = localPos;
        }

        // 右邊 3 個目標.
        for(int i = 3; i < 6; i++)
        {
            var localPos = mTargets[0].transform.localPosition;
            localPos.x += xInterval;
            localPos.y -= yInterval * (i - 3);
            mTargets[i].transform.localPosition = localPos;
        }

    }

	public void SetInterval(float startY, float yInterval)
	{
		// 因為介面的配置是左邊有 3 個目標, 右邊有 3 個目標, 所以才有以下的魔術數字 3 和 6.
		float y = 0;
		for (int i = 0; i < 3; i++)
		{
			var localPos = mTargets[i].transform.localPosition;
			if(i == 0) {
				y = startY;
				localPos.y = startY;
			} else
				localPos.y = y;
			mTargets[i].transform.localPosition = localPos;
			
			y -= yInterval;
		}
		
		y = 0;
		for (int i = 3; i < 6; i++)
		{
			var localPos = mTargets[i].transform.localPosition;
			if(i == 3) {
				y = startY;
				localPos.y = startY;
			} else
				localPos.y = y;
			mTargets[i].transform.localPosition = localPos;
			
			y -= yInterval;
		}
	}

    public void Show()
    {
        Window.SetActive(true);
    }

    public void Hide()
    {
        Window.SetActive(false);
    }
	//Stage
	public void UpdateUI (int stageID) {
		UIStageHintManager.UpdateHintNormal(stageID, ref mTargets);
	}
	public void UpdateValue(int stageID) {
		UIStageHintManager.UpdateHintInGame(stageID, ref mTargets);
	}
}