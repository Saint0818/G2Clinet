using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 道具出處介面主程式.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item>  </item>
/// </list>
[DisallowMultipleComponent]
public class UIItemSourceMain : MonoBehaviour
{
    /// <summary>
    /// 右上角的 X 按鈕按下.
    /// </summary>
    public event CommonDelegateMethods.Action OnCloseListener;

    public UIButton CloseButton;

    [UsedImplicitly]
    private void Awake()
    {
        CloseButton.onClick.Add(new EventDelegate(onCloseClick));
    }

    private void onCloseClick()
    {
        if(OnCloseListener != null)
            OnCloseListener();
    }
}