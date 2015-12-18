using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 創角 StyleView 右邊視窗的按鈕. 該按鈕顯示可換裝的裝備.
/// </summary>
[DisallowMultipleComponent]
public class UICreateRoleStyleViewPartsWindowButton : MonoBehaviour
{
    /// <summary>
    /// value1: Index, value2: ItemID.
    /// </summary>
    public event CommonDelegateMethods.Int2 ClickListener;

    public UILabel NameLabel;
    public UISprite IconSprite;

    [UsedImplicitly]
	private void Start()
    {
	}

    /// <summary>
    /// 這在視窗中, 是第幾個按鈕.
    /// </summary>
    public int Index { set; get; }

    public int ItemID { set; get; }

    public string Name
    {
        set { NameLabel.text = value; }
    }

	public UIAtlas Atlas
	{
		set {IconSprite.atlas = value;}
	}

    public int Icon
    {
        set { IconSprite.spriteName = string.Format("Item_{0}", value); }
    }

    public void SetSelected()
    {
        GetComponent<UIToggle>().Set(true);
    }

    /// <summary>
    /// NGUI Event.
    /// </summary>
    public void OnClick()
    {
        if(ClickListener != null)
            ClickListener(Index, ItemID);
    }
}
