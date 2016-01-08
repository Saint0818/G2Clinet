using System;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 道具出處介面主程式.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call SetXXXX() 設定道具資訊. </item>
/// <item> Call AddSources() 加入出處來源. </item>
/// </list>
[DisallowMultipleComponent]
public class UIItemSourceMain : MonoBehaviour
{
    /// <summary>
    /// 右上角的 X 按鈕按下.
    /// </summary>
    public event Action OnCloseListener;

    public UIButton CloseButton;
    public UILabel NameLabel;
    public Transform ElementParent;

    private HintAvatarView mHintAvatar;
    private HintInlayView mHintInlay;
    private HintSkillView mHintSkill;

    [UsedImplicitly]
    private void Awake()
    {
        CloseButton.onClick.Add(new EventDelegate(onCloseClick));

        mHintAvatar = GetComponentInChildren<HintAvatarView>();
        mHintInlay = GetComponentInChildren<HintInlayView>();
        mHintSkill = GetComponentInChildren<HintSkillView>();
    }

    private void hideAllHint()
    {
        mHintAvatar.Hide();
        mHintInlay.Hide();
        mHintSkill.Hide();
    }

    public void SetMaterial(TItemData item)
    {
        hideAllHint();

        NameLabel.text = item.Name;

        // 我發現呼叫 Awake() 的時機在 Unity 5.3.0 好像有改變.
        // 當我第一次呼叫 Show() 時, Awake() 居然沒有被呼叫.
        // 所以我必須要用呼叫兩次 Show(), 才可以真正的介面打開.(因為 Awake 會關閉介面)
        mHintInlay.Show();
        mHintInlay.Show(); 
        mHintInlay.UpdateUI(item);
    }

    public void SetAvatar(TItemData item)
    {
        hideAllHint();

        NameLabel.text = item.Name;

        // 我發現呼叫 Awake() 的時機在 Unity 5.3.0 好像有改變.
        // 當我第一次呼叫 Show() 時, Awake() 居然沒有被呼叫.
        // 所以我必須要用呼叫兩次 Show(), 才可以真正的介面打開.(因為 Awake 會關閉介面)
        mHintAvatar.Show();
        mHintAvatar.Show();
        mHintAvatar.UpdateUI(item);
    }

    public void SetSkill(TItemData item)
    {
        hideAllHint();

        NameLabel.text = item.Name;

        // 我發現呼叫 Awake() 的時機在 Unity 5.3.0 好像有改變.
        // 當我第一次呼叫 Show() 時, Awake() 居然沒有被呼叫.
        // 所以我必須要用呼叫兩次 Show(), 才可以真正的介面打開.(因為 Awake 會關閉介面)
        mHintSkill.Show();
        mHintSkill.Show();
        mHintSkill.UpdateUI(item);
    }

    private void clearSources()
    {
        UIItemSourceElement[] elements = GetComponentsInChildren<UIItemSourceElement>();
        foreach(UIItemSourceElement element in elements)
        {
            Destroy(element.gameObject);
        }
    }

    public void AddSources(UIItemSourceElement.Data[] data)
    {
        clearSources();

        Vector3 localPos = Vector3.zero;
        for(var i = 0; i < data.Length; i++)
        {
            var obj = UIPrefabPath.LoadUI(UIPrefabPath.ItemSourceElement, ElementParent, localPos);
            var element = obj.GetComponent<UIItemSourceElement>();
            element.Set(data[i]);

            localPos.y -= obj.GetComponent<UISprite>().height;
        }
    }

    private void onCloseClick()
    {
        if(OnCloseListener != null)
            OnCloseListener();
    }
}