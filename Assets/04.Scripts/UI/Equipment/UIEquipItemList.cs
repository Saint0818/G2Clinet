using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這是裝備介面右邊的道具清單列表, 會列出玩家倉庫內的數值裝備.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Show() or Hide(). </item>
/// <item> (Optional) Visible 得知是否顯示中. </item>
/// </list>
public class UIEquipItemList : MonoBehaviour
{
    /// <summary>
    /// 呼叫時機: 列表上的按鈕點擊時. 
    /// 參數: int index, 哪一個被點擊.
    /// </summary>
    public event Action<int> OnClickListener;

    public GameObject Window;
    public UIScrollView ScrollView;
    public GameObject EmptyLabel;

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

    public bool Visible { get { return Window.activeSelf; } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerItem"> 玩家身上裝的裝備.(只是用來判斷要不要顯示紅點, 以後要改掉, 應該是傳遞哪個要道具顯示紅點的參數) </param>
    /// <param name="items"> 列表要顯示的數值裝. </param>
    /// <param name="resetPos"> true: 會將 ScrollView 底下的元件位置重置. </param>
    public void Show(UIValueItemData playerItem, List<UIValueItemData> items, bool resetPos)
    {
        Window.SetActive(true);

        buildUI(playerItem, items);

        if(resetPos)
            // 這個和 ScrollView 的 Content Origin 非常有關係, 這會影響重置的位置.
            ScrollView.ResetPosition(); 
        ScrollView.enabled = false;
        ScrollView.enabled = true;
    }

    private void buildUI(UIValueItemData playerItem, List<UIValueItemData> items)
    {
        clear();

        var localPos = mStartPos;
        for(var i = 0; i < items.Count; i++)
        {
            GameObject obj = UIPrefabPath.LoadUI(UIPrefabPath.UIEquipListButton, ScrollView.transform, localPos);
            var element = obj.GetComponent<UIEquipListButton>();
            mButtons.Add(element);
            localPos.y -= obj.GetComponent<UISprite>().height; // 因為每個元件的高度都相同, 所以可以這樣增加.

            element.Init(this, i);
            element.Set(items[i]);
        }

        if(items.Count > 0)
            bestItemShowRedPoint(playerItem, items);

        EmptyLabel.SetActive(items.Count <= 0);
    }

    private void bestItemShowRedPoint(UIValueItemData playerItem, List<UIValueItemData> items)
    {
        UIValueItemData bestItem;
        int index = findBestItem(items, out bestItem);
        if(bestItem.GetTotalPoints() > playerItem.GetTotalPoints())
            mButtons[index].RedPointVisible = true;
    }

    private int findBestItem(List<UIValueItemData> items, out UIValueItemData bestItem)
    {
        int maxTotalPoints = int.MinValue;
        bestItem = null;
        int bestItemIndex = -1;

        for(int i = 0; i < items.Count; i++)
        {
            UIValueItemData item = items[i];
            if(maxTotalPoints < item.GetTotalPoints())
            {
                maxTotalPoints = item.GetTotalPoints();
                bestItem = item;
                bestItemIndex = i;
            }
        }

        return bestItemIndex;
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
