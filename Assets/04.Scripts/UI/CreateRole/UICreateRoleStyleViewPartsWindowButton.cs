using UnityEngine;
using JetBrains.Annotations;

/// <summary>
/// 創角 StyleView 右邊視窗的按鈕. 該按鈕顯示可換裝的裝備.
/// </summary>
[DisallowMultipleComponent]
public class UICreateRoleStyleViewPartsWindowButton : MonoBehaviour
{
    public UILabel NameLabel;
    public UISprite IconSprite;

    [UsedImplicitly]
	private void Start()
    {
	}

    public string Name
    {
        set { NameLabel.text = value; }
    }

    public string Icon
    {
        set { IconSprite.spriteName = value; }
    }

    public void SetSelected()
    {
        GetComponent<UIToggle>().Set(true);
    }
}
