using UnityEngine;

/// <summary>
/// 獎勵圖片.
/// </summary>
public class UIStageRewardIcon : MonoBehaviour
{
    public UISprite Icon;
    public UILabel Text;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI(string spriteName, string text)
    {
        Icon.spriteName = string.Format("Item_{0}", spriteName);
        Text.text = text;
    }
}