using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 道具出處介面主程式.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call SetMaterial() 設定道具資訊. </item>
/// </list>
[DisallowMultipleComponent]
public class UIItemSourceMain : MonoBehaviour
{
    /// <summary>
    /// 右上角的 X 按鈕按下.
    /// </summary>
    public event CommonDelegateMethods.Action OnCloseListener;

    public UIButton CloseButton;
    public UILabel NameLabel;

//    private HintAvatarView mHintAvatar;
    private HintInlayView mHintInlay;
//    private HintSkillView mHintSkill;

    [UsedImplicitly]
    private void Awake()
    {
        CloseButton.onClick.Add(new EventDelegate(onCloseClick));

//        mHintAvatar = GetComponentInChildren<HintAvatarView>();
//        mHintAvatar.gameObject.SetActive(false);
        mHintInlay = GetComponentInChildren<HintInlayView>();
        mHintInlay.gameObject.SetActive(false);
//        mHintSkill = GetComponentInChildren<HintSkillView>();
//        mHintSkill.gameObject.SetActive(false);
    }

    private void hideAllHint()
    {
        mHintInlay.Hide();
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

    public void ClearSources()
    {
        
    }

    public void AddSource(string kindTitle, string kindName, UIItemSourceElement.IAction action)
    {
        
    }

    private void onCloseClick()
    {
        if(OnCloseListener != null)
            OnCloseListener();
    }
}