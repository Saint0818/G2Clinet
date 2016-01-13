using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 這是一個裝備道具. 會顯示裝備的圖示, 名稱, 數量, 鑲嵌資訊.(會用在裝備介面的左邊和中間)
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Set() 設定道具資訊. </item>
/// <item> OnClickListener 註冊點擊事件. </item>
/// </list>
public class UIEquipItem : MonoBehaviour
{
    public event Action OnClickListener;

    [FormerlySerializedAs("Picture")]
    public UISprite Icon;
    public GameObject EmptyIcon;

    public UILabel Text;
    public UILabel Amount;
    public GameObject RedPoint;
    public GameObject[] InlaySlots;
    public GameObject[] Inlays;

    [UsedImplicitly]
    private void Awake()
    {
        // 暫時關閉.
        Text.gameObject.SetActive(false);

        hideAllInlay();
    }

    private void hideAllInlay()
    {
        foreach(GameObject inlay in Inlays)
        {
            inlay.SetActive(false);
        }

        foreach(GameObject slot in InlaySlots)
        {
            slot.SetActive(false);
        }
    }

    public void Set(UIValueItemData data, bool showRedPoint)
    {
        gameObject.SetActive(true);
        
        updateIcon(data);

        Text.text = data.Name;

        RedPoint.SetActive(showRedPoint);

        Amount.gameObject.SetActive(data.Num >= 2);
        Amount.text = data.Num.ToString();

        updateInlay(data.Inlay);
    }

    private void updateIcon(UIValueItemData data)
    {
        if(data.IsValid())
        {
            EmptyIcon.SetActive(false);
            Icon.gameObject.SetActive(true);
            Icon.atlas = data.Atlas;
            Icon.spriteName = data.Icon;

            GetComponent<UISprite>().spriteName = data.Frame;
            // 我認為這是 NGUI 的問題, 其實我改 UISprite 後, UIButton 的 normal 也應該要改才對.
            GetComponent<UIButton>().normalSprite = data.Frame;
        }
        else
        {
            EmptyIcon.SetActive(true);
            Icon.gameObject.SetActive(false);

            GetComponent<UISprite>().spriteName = string.Empty;
            // 我認為這是 NGUI 的問題, 其實我改 UISprite 後, UIButton 的 normal 也應該要改才對.
            GetComponent<UIButton>().normalSprite = string.Empty;
        }
    }

    private void updateInlay(bool[] inlayStatus)
    {
        hideAllInlay();

        for(var i = 0; i < inlayStatus.Length; i++)
        {
            InlaySlots[i].SetActive(true);
            Inlays[i].SetActive(inlayStatus[i]);
        }
    }

    public void NotifyClick()
    {
        if(OnClickListener != null)
            OnClickListener();
    }
}