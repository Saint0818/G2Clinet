using System;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 道具出處介面中, 某一列出處介面. 比如顯示關卡的出處.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item>  </item>
/// </list>
[DisallowMultipleComponent]
public class UIItemSourceElement : MonoBehaviour
{
    public event CommonDelegateMethods.Action ClickListener;

    public class Data
    {
        public string KindName;
        public string Name;
    }

    public UILabel KindLabel;
    public UILabel NameLabel;
    public UIButton StartButton;

    [UsedImplicitly]
    private void Awake()
    {
        StartButton.onClick.Add(new EventDelegate(onButtonClick));
    }

    public void Set(Data data)
    {
        KindLabel.text = data.KindName;
        NameLabel.text = data.Name;
    }

    private void onButtonClick()
    {
        Debug.Log("UIItemSourceElement.onButtonClick");

        if(ClickListener != null)
            ClickListener();
    }
}