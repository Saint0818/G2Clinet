using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這是一個裝備道具. 會顯示裝備的圖示, 名稱, 數量, 鑲嵌資訊.(會用在裝備介面的左邊和中間)
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Set() or Clear() 設定道具資訊. </item>
/// <item> OnClickListener 註冊點擊事件. </item>
/// </list>
public class UIEquipItem : MonoBehaviour
{
    public event CommonDelegateMethods.Action OnClickListener;

    public UISprite Picture;
    public UILabel Text;
    public UILabel Amount;
    public GameObject RedPoint;
    public GameObject[] InlaySlots;
    public GameObject[] Inlays;

    [UsedImplicitly]
    private void Awake()
    {
        // 暫時關閉.
        Amount.gameObject.SetActive(false);
        Text.gameObject.SetActive(false);

        foreach(GameObject inlay in Inlays)
        {
            inlay.SetActive(false);
        }

        foreach(GameObject slot in InlaySlots)
        {
            slot.SetActive(false);
        }
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }

    private bool RedPointVisible
    {
        set { RedPoint.SetActive(value); }
    }

    public void Set(UIValueItemData item, bool showRedPoint)
    {
        gameObject.SetActive(true);
        Picture.spriteName = item.Icon;

        GetComponent<UISprite>().spriteName = item.Frame;
        // 我認為這是 NGUI 的問題, 其實我改 UISprite 後, UIButton 的 normal 也應該要改才對.
        GetComponent<UIButton>().normalSprite = item.Frame;

        Text.text = item.Name;

        RedPointVisible = showRedPoint;
    }

    /// <summary>
    /// NGUI Event.
    /// </summary>
    [UsedImplicitly]
    public void OnClick()
    {
        if(OnClickListener != null)
            OnClickListener();
    }
}