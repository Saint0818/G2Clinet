using System;
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

        public bool EnableStartButton;

        /// <summary>
        /// Start 按鈕按下要做的事情.(通常都是只有開啟介面)
        /// </summary>
        [CanBeNull]
        public IAction StartAction;

        /// <summary>
        /// 呼叫時機: Start 按鈕按下.
        /// </summary>
        [CanBeNull]
        public Action<bool> StartCallback; 
    }

    public interface IAction
    {
        void Do();
    }

    public UILabel KindLabel;
    public UILabel NameLabel;
    public UILabel ButtonLabel;
    public UIButton StartButton;
    public UISprite ButtonSprite;

    [CanBeNull]
    private IAction mStartAction;

    /// <summary>
    /// bool: true 表示 Start 按鈕是 Enabled.
    /// </summary>
    [CanBeNull]
    private Action<bool> mStartCallback;

    private bool mStartEnabled;

    [UsedImplicitly]
    private void Awake()
    {
        StartButton.onClick.Add(new EventDelegate(onStartClick));
    }

    [UsedImplicitly]
    private void Start()
    {
        ButtonLabel.text = TextConst.S(int.Parse(ButtonLabel.text));
    }

    public void Set(Data data)
    {
        KindLabel.text = data.KindName;
        NameLabel.text = data.Name;

        mStartEnabled = data.EnableStartButton;
        mStartAction = data.StartAction;
        mStartCallback = data.StartCallback;

        ButtonSprite.spriteName = UIBase.ButtonBG(mStartEnabled);
    }

    private void onStartClick()
    {
        if(mStartEnabled)
        {
            if(mStartAction != null)
                mStartAction.Do();

            UIItemSource.Get.Hide();
        }

        if(mStartCallback != null)
            mStartCallback(mStartEnabled);
    }
}