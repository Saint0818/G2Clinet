﻿using System;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 道具出處介面中, 某一列出處介面. 比如顯示關卡的出處.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Set() 設定要顯示的資料. </item>
/// </list>
[DisallowMultipleComponent]
public class UIItemSourceElement : MonoBehaviour
{
    public class Data
    {
        public string KindName;
        public string Name;
    }

    public interface IAction
    {
        void Do();
    }

    public UILabel KindLabel;
    public UILabel NameLabel;
    public UIButton StartButton;

    private IAction mAction;

    [UsedImplicitly]
    private void Awake()
    {
        StartButton.onClick.Add(new EventDelegate(onStartClick));
    }

    public void Set(string kindTitle, string kindName, IAction action)
    {
        KindLabel.text = kindTitle;
        NameLabel.text = kindName;

        mAction = action;
    }

    private void onStartClick()
    {
        mAction.Do();
    }
}