using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這是裝備介面右邊的道具清單列表, 會列出玩家倉庫內的數值裝備.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Show() or Hide(). </item>
/// </list>
public class UIEquipList : MonoBehaviour
{
    /// <summary>
    /// 呼叫時機: 列表上的按鈕點擊時. 
    /// 參數: int index, 哪一個被點擊.
    /// </summary>
    public event CommonDelegateMethods.Int1 OnClickListener;

    public GameObject Window;
    public UIScrollView ScrollView;

    private readonly List<UIEquipListButton> mButtons = new List<UIEquipListButton>();

    private readonly Vector3 mStartPos = new Vector3(0, 110, 0);

    [UsedImplicitly]
	private void Awake()
    {
	}

    private void clear()
    {
        foreach(UIEquipListButton button in mButtons)
        {
            Destroy(button.gameObject);
        }
        mButtons.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="items"> 列表要顯示的數值裝. </param>
    /// <param name="resetPos"> true: 會將 ScrollView 底下的元件位置重置. </param>
    public void Show(UIValueItemData[] items, bool resetPos)
    {
        Window.SetActive(true);

        clear();

        var localPos = mStartPos;

        for(var i = 0; i < items.Length; i++)
        {
            GameObject obj = UIPrefabPath.LoadUI(UIPrefabPath.EquipListButton, ScrollView.transform, localPos);
            var button = obj.GetComponent<UIEquipListButton>();
            mButtons.Add(button);
            localPos.y -= obj.GetComponent<UISprite>().height; // 因為每個元件的高度都相同, 所以可以這樣增加.

            button.Init(this, i);
            button.Set(items[i]);
        }

        if(resetPos)
            // 這個和 ScrollView 的 Content Origin 非常有關係, 這會影響重置的位置.
            ScrollView.ResetPosition(); 
        ScrollView.enabled = false;
        ScrollView.enabled = true;
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    /// <summary>
    /// 僅給 UIEquipListButton 使用.
    /// </summary>
    /// <param name="index"></param>
    public void NotifyClick(int index)
    {
        if(OnClickListener != null)
            OnClickListener(index);
    }
}
