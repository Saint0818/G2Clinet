using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 小關卡, 也就是關卡介面上的小圓點, 點擊後, 玩家可以進入關卡.
/// </summary>
public class UIStageSmall : MonoBehaviour
{
    [Tooltip("StageTable 裡面的 ID. 控制要顯示哪一個關卡的資訊.")]
    public int StageID;

    public UISprite KindSprite;

    private const string OpenSpriteName = "StageButton01";
    private const string LockSpriteName = "StageButton02";

    private UIStageInfo.Data mData;

    private UIMainStageMain Main
    {
        get
        {
            if(mMain == null)
                mMain = GetComponentInParent<UIMainStageMain>();
            return mMain;
        }
    }
    private UIMainStageMain mMain;

    public void Show(UIStageInfo.Data data)
    {
        mData = data;

        GetComponent<UISprite>().spriteName = OpenSpriteName;
        KindSprite.spriteName = mData.KindSpriteName;

        // 如果不加上這行, 當我滑鼠滑過圖片時, 圖片會變掉. 我認為這應該是 UIButton 的 Bug. 
        // 目前的解決辦法是以下程式碼.
        GetComponent<UIButton>().normalSprite = OpenSpriteName; 

        GetComponent<BoxCollider>().enabled = true;
    }

    public void ShowLock(string kindSpriteName)
    {
        GetComponent<UISprite>().spriteName = LockSpriteName;
        KindSprite.spriteName = kindSpriteName;

        // 如果不加上這行, 當我滑鼠滑過圖片時, 圖片會變掉. 我認為這應該是 UIButton 的 Bug. 
        // 目前的解決辦法是以下程式碼.
        GetComponent<UIButton>().normalSprite = LockSpriteName;

        GetComponent<BoxCollider>().enabled = false;
    }

    public void NotifyClick()
    {
        Main.Info.Show(StageID, mData);
    }
}
