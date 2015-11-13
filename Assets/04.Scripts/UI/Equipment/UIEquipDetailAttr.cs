using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 裝備介面中間的數值.(某一列)
/// </summary>
public class UIEquipDetailAttr : MonoBehaviour
{
    public UISprite Icon;
    public UILabel ValueLabel;

    [UsedImplicitly]
    private void Awake()
    {
    }

    public void Set(string icon, int value)
    {
        gameObject.SetActive(true);

        Icon.spriteName = icon;
        ValueLabel.text = value.ToString();
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}