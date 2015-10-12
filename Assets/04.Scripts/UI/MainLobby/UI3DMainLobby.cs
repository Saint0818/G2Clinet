using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// �t�d�j�U 3D �������F��(�ҫ�����).
/// </summary>
/// <remarks>
/// �ϥΤ�k:
/// <list type="number">
/// <item> Call Get ���o instance. </item>
/// <item> Call Show() or Hide() ���� UI �n���n���. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UI3DMainLobby : UIBase
{
    private static UI3DMainLobby instance = null;
    private const string UIName = "UI3DMainLobby";


    [UsedImplicitly]
    private void Awake()
    {
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
    }

    public void Hide()
    {
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