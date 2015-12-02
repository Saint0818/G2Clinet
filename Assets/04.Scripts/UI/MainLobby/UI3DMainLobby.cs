using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 負責大廳 3D 相關的東西(模型等等).
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> Call Get 取得 instance. </item>
/// <item> Call Show() or Hide() 控制 UI 要不要顯示. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UI3DMainLobby : UIBase
{
    private static UI3DMainLobby instance = null;
    private const string UIName = "UI3DMainLobby";

    private UI3DMainLobbyImpl mImpl;

    [UsedImplicitly]
    private void Awake()
    {
        mImpl = GetComponent<UI3DMainLobbyImpl>();
    }

    [UsedImplicitly]
    private void Start()
    {
        var obj = GameObject.Find("UI3D/3DCamera");
        if(obj)
            obj.SetActive(false);
        else
            Debug.LogWarning("Can't find UI3D.3DCamera.");
    }

    public void Show()
    {
        Show(true);

        mImpl.Show();
    }

    public void Hide()
    {
        mImpl.Hide();

        RemoveUI(UIName);
    }

    public static UI3DMainLobby Get
    {
        get
        {
            if(!instance)
            {
                UI3D.UIShow(true);
                instance = Load3DUI(UIName) as UI3DMainLobby;
            }

            return instance;
        }
    }
}