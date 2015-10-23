using UnityEngine;
using JetBrains.Annotations;

/// <summary>
/// 小關卡, 也就是關卡介面上的小圓點, 點擊後, 玩家可以進入關卡.
/// </summary>
public class UIStageSmall : MonoBehaviour
{
    [Tooltip("StageTable 裡面的 ID. 控制要顯示哪一個關卡的資訊.")]
    public int StageID;

    private const string LockSpriteName = "Icon_lock";
//    private const string LockSpriteName = "Iconface";

    private UIMainStageImpl mImpl;

    [UsedImplicitly]
	private void Awake()
    {
        mImpl = GetComponentInParent<UIMainStageImpl>();
    }

    public void Show(string spriteName)
    {
        GetComponent<UISprite>().spriteName = spriteName;

        // 如果不加上這行, 當我滑鼠滑過圖片時, 圖片會變掉. 我認為這應該是 UIButton 的 Bug. 
        // 目前的解決辦法是以下程式碼.
        GetComponent<UIButton>().normalSprite = spriteName; 

        GetComponent<BoxCollider>().enabled = true;
    }

    public void ShowLock()
    {
        GetComponent<UISprite>().spriteName = LockSpriteName;

        // 如果不加上這行, 當我滑鼠滑過圖片時, 圖片會變掉. 我認為這應該是 UIButton 的 Bug. 
        // 目前的解決辦法是以下程式碼.
        GetComponent<UIButton>().normalSprite = LockSpriteName;

        GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// NGUI Event: 點擊時被呼叫.
    /// </summary>
    [UsedImplicitly]
    private void OnClick()
    {
        mImpl.Info.Show(StageID);
    }
}
