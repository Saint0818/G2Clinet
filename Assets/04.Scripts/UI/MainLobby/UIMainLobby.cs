using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 大廳主程式.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示某個頁面. </item>
/// <item> Call Hide() 將大廳關閉. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIMainLobby : UIBase
{
    private static UIMainLobby instance;
    private const string UIName = "UIMainLobby";

//    private UIMainLobbyImpl mImpl;

    [UsedImplicitly]
    private void Awake()
    {
//        mImpl = GetComponent<UIMainLobbyImpl>();
    }

    public void Show()
    {
        Show(true);

        UI3DMainLobby.Get.Show();
    }

    public void Hide()
    {
        UI3DMainLobby.Get.Hide();

        RemoveUI(UIName);
    }

    public static UIMainLobby Get
    {
        get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIMainLobby;
            }
			
            return instance;
        }
    }
}