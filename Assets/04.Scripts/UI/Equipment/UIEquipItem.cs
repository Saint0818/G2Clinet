using JetBrains.Annotations;
using UI;
using UnityEngine;

/// <summary>
/// 這是一個裝備道具. 會顯示裝備的圖示, 名稱, 數量, 鑲嵌資訊.
/// </summary>
public class UIEquipItem : MonoBehaviour
{
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

    public void Set(EquipItem item)
    {
        Picture.spriteName = item.Icon;
        Text.text = item.Name;
    }
}