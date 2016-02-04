using System;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 顯示道具出處的介面.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call ShowXXX() 顯示介面.. </item>
/// <item> Call Hide(). </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIItemSource : UIBase
{
    private static UIItemSource instance;
    private const string UIName = "UIItemSource";

    private UIItemSourceMain mMain;

    [UsedImplicitly]
    private void Awake()
    {
        mMain = GetComponent<UIItemSourceMain>();
        mMain.OnCloseListener += Hide;
    }

    [UsedImplicitly]
    private void Start()
    {
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startCallback"> [bool]:true 是前往按鈕按下, 呼叫此介面的介面, 要自己負責將自己關閉. </param>
    public void ShowMaterial(TItemData item, Action<bool> startCallback)
    {
        Show(true);

        // 如果不加上這段, 六角形的屬性值會在前面. 所以必須要整個 UI 往前移動, 讓六角形屬性值不會穿過介面.
        bringToFront();

        mMain.SetMaterial(item);
        mMain.AddSources(UIItemSourceBuilder.Build(item, startCallback));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startCallback"> [bool]:true 是前往按鈕按下, 呼叫此介面的介面, 要自己負責將自己關閉. </param>
    public void ShowAvatar(TItemData item, Action<bool> startCallback)
    {
        Show(true);

        // 如果不加上這段, 六角形的屬性值會在前面. 所以必須要整個 UI 往前移動, 讓六角形屬性值不會穿過介面.
        bringToFront();

        mMain.SetAvatar(item);
        mMain.AddSources(UIItemSourceBuilder.Build(item, startCallback));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startCallback"> [bool]:true 是前往按鈕按下, 呼叫此介面的介面, 要自己負責將自己關閉. </param>
    public void ShowSkill(TItemData item, Action<bool> startCallback)
    {
        Show(true);

        // 如果不加上這段, 六角形的屬性值會在前面. 所以必須要整個 UI 往前移動, 讓六角形屬性值不會穿過介面.
        bringToFront();

        mMain.SetSkill(item);
        mMain.AddSources(UIItemSourceBuilder.Build(item, startCallback));
    }

    private void bringToFront()
    {
        var pos = transform.localPosition;
        pos.z = -10;
        transform.localPosition = pos;
    }

    public void Hide()
    {
        RemoveUI(UIName);
    }

    public static UIItemSource Get
    {
        get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIItemSource;
            }

            return instance;
        }
    }
}