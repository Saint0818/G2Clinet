using JetBrains.Annotations;
using UI;
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
    public GameObject[] Inlays;

    [UsedImplicitly]
    private void Awake()
    {
        // 暫時關閉.
	    Amount.gameObject.SetActive(false);
        foreach(GameObject inlay in Inlays)
        {
            inlay.SetActive(false);
        }
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }

    public void Set(EquipItem item)
    {
        gameObject.SetActive(true);
        Picture.spriteName = item.Icon;
        Text.text = item.Name;
    }

    /// <summary>
    /// NGUI Event.
    /// </summary>
    [UsedImplicitly]
    private void OnClick()
    {
        if(OnClickListener != null)
            OnClickListener();
    }
}