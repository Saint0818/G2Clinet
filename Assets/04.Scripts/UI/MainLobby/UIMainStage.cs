using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 關卡頁面, 會顯示很多的小關卡.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示關卡. </item>
/// <item> Call Hide() 關閉關卡. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIMainStage : UIBase
{
    private static UIMainStage instance;
    private const string UIName = "UIMainStage";

    private UIMainStageImpl mImpl;

    [UsedImplicitly]
    private void Awake()
    {
        mImpl = GetComponent<UIMainStageImpl>();
        mImpl.BackListener += goToGameLobby;
    }

    public void Show()
    {
        Show(true);
    }

    public void Hide()
    {
        RemoveUI(UIName);
    }

    private void goToGameLobby()
    {
        UIGameLobby.Get.Show();
        Hide();
    }

    public static UIMainStage Get
    {
        get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIMainStage;
            }
			
            return instance;
        }
    }
}