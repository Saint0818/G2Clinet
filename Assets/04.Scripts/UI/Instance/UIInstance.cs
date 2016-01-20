using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 副本介面.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示副本. </item>
/// <item> Call Hide() 關閉副本. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIInstance : UIBase
{
    private static UIInstance instance;
    private const string UIName = "UIInstance";

    private UIInstanceMain mMain;

    [UsedImplicitly]
    private void Awake()
    {
        mMain = GetComponent<UIInstanceMain>();
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    /// <summary>
    /// 預設顯示尚未過關關卡所在的章節.
    /// </summary>
    public void Show()
    {
        Show(true);

        buildChapters();
    }

    private void buildChapters()
    {
        List<ChapterData> allData = ChapterTable.Ins.GetAllInstance();
        foreach(ChapterData data in allData)
        {
            UIInstanceChapter.Data uiData = UIInstanceBuilder.Build(data);
            mMain.AddChapter(uiData);
        }
    }

    public void Hide()
    {
        RemoveUI(UIName);
    }

    public static UIInstance Get
    {
        get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIInstance;
            }
			
            return instance;
        }
    }
}