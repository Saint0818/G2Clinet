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

        public bool StartEnabled;

        /// <summary>
        /// 開始按鈕不能按, 但使用者按下時會出現的提示訊息.
        /// </summary>
        public string StartWarningMessage;

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

    private Data mData;

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
        mData = data;

        KindLabel.text = mData.KindName;
        NameLabel.text = mData.Name;

        ButtonSprite.spriteName = UIBase.ButtonBG(mData.StartEnabled);
    }

    private void onStartClick()
    {
        if(mData.StartEnabled)
        {
            if(mData.StartAction != null)
                mData.StartAction.Do();

            UIItemSource.Get.Hide();
        }
        else
            UIHint.Get.ShowHint(mData.StartWarningMessage, Color.white);

        if(mData.StartCallback != null)
            mData.StartCallback(mData.StartEnabled);
    }
}