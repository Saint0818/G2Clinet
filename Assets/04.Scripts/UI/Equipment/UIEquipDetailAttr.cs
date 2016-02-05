using GameStruct;
using UnityEngine;

/// <summary>
/// 裝備介面中, 畫面中間, 顯示某一個(一列)屬性的數值.
/// </summary>
public class UIEquipDetailAttr : MonoBehaviour
{
    public UISprite Icon;
    public UILabel ValueLabel;

    private EBonus mBonus;

    private void Start()
    {
        var button = GetComponent<UIButton>();
        if(button != null)
            button.onClick.Add(new EventDelegate(() => UIAttributeHint.Get.UpdateView((int)mBonus)));
    }

    public void Set(UIValueItemData.BonusData data, int inlayValue)
    {
        gameObject.SetActive(true);

        mBonus = data.Bonus;

        Icon.spriteName = data.Icon;

        ValueLabel.text = inlayValue > 0 
            ? string.Format("{0}[ABFF83FF]+{1}[-]", data.Value, inlayValue) 
            : string.Format("{0}", data.Value);
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}