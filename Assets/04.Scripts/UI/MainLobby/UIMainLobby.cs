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

    private GameObject mFullScreenBlock;

    public void Show()
    {
        Show(true);
    }

    public void Hide()
    {
        UI3DMainLobby.Get.Hide();
        RemoveUI(UIName);
    }

    /// <summary>
    /// Block ���ت��O�קK�ϥΪ��I������ UI ����.(�����ϥ�, �@��ϥΪ̤��n�ϥ�)
    /// </summary>
    /// <param name="enable"></param>
    public void EnableBlock(bool enable)
    {
        mFullScreenBlock.SetActive(enable);
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

    [UsedImplicitly]
    private void Awake()
    {
        mFullScreenBlock = GameObject.Find("FullScreenInvisibleWidget");
        mFullScreenBlock.SetActive(false);
    }
}