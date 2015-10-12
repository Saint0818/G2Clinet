using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// �j�U�D�{��.
/// </summary>
/// <remarks>
/// �ϥΤ�k:
/// <list type="number">
/// <item> �� Get ���o instance. </item>
/// <item> Call Show() ��ܬY�ӭ���. </item>
/// <item> Call Hide() �N�j�U����. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIMainLobby : UIBase
{
    private static UIMainLobby instance;
    private const string UIName = "UIMainLobby";

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