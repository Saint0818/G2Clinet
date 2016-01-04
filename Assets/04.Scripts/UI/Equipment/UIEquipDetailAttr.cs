using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 裝備介面中, 畫面中間, 顯示某一個(一列)屬性的數值.
/// </summary>
public class UIEquipDetailAttr : MonoBehaviour
{
    public UISprite Icon;
    public UILabel ValueLabel;

    [UsedImplicitly]
    private void Awake()
    {
    }

    public void Set(string icon, int basicValue, int inlayValue)
    {
        gameObject.SetActive(true);

        Icon.spriteName = icon;

        ValueLabel.text = inlayValue > 0 
            ? string.Format("{0}[ABFF83FF]+{1}[-]", basicValue, inlayValue) 
            : string.Format("{0}", basicValue);
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}