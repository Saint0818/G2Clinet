using JetBrains.Annotations;
using UnityEngine;
using GameEnum;

/// <summary>
/// 負責創角時 3D 相關的東西(模型等等).
/// </summary>
/// <remarks>
/// <list type="number">
/// <item> Call Get 取得 instance. </item>
/// <item> Call ShowXXXX() or Hide() 控制 UI 要不要顯示. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UI3DCreateRole : UIBase
{
    private static UI3DCreateRole instance = null;
    private const string UIName = "UI3DCreateRole";

    public UI3DCreateRolePositionView PositionView
    {
        get { return mPositionView; }
    }
    private UI3DCreateRolePositionView mPositionView;

    public UI3DCreateRoleStyleView StyleView
    {
        get { return mStyleView; }
    }
    private UI3DCreateRoleStyleView mStyleView;

    [UsedImplicitly]
    private void Awake()
    {
        mPositionView = GetComponent<UI3DCreateRolePositionView>();
        mStyleView = GetComponent<UI3DCreateRoleStyleView>();
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

    /// <summary>
    /// 會顯示全部的球員.
    /// </summary>
    public void ShowPositionView()
    {
        Show(true);

        mPositionView.Show();
        mStyleView.Hide();
    }

    /// <summary>
    /// 僅顯示某一位球員.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="playerID"></param>
    public void ShowStyleView(EPlayerPostion pos, int playerID)
    {
        Show(true);

        mPositionView.Hide();
        mStyleView.Show(pos, playerID);
    }

    public void Hide()
    {
        RemoveUI(UIName);
    }

    public static UI3DCreateRole Get
    {
        get
        {
            if(!instance)
            {
                UI3D.UIShow(true);
                instance = Load3DUI(UIName) as UI3DCreateRole;
            }

            return instance;
        }
    }
}
