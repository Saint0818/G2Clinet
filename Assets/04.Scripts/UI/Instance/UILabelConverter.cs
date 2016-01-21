using UnityEngine;

/// <summary>
/// 自動轉換為遊戲內字串.
/// </summary>
[RequireComponent(typeof(UILabel))]
public class UILabelConverter : MonoBehaviour
{
    public int TextValue;

    private void Start()
    {
        GetComponent<UILabel>().text = TextConst.S(TextValue);
    }
}