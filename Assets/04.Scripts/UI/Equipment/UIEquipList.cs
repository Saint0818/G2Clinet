using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using UI;

/// <summary>
/// 這是裝備介面右邊的道具清單列表, 會列出玩家倉庫內的數值裝備.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Show() or Hide(). </item>
/// </list>
public class UIEquipList : MonoBehaviour
{
    public GameObject Window;
    public Transform ScrollView;

    private readonly List<UIEquipListButton> mButtons = new List<UIEquipListButton>();

    private readonly Vector3 mStartPos = new Vector3(0, 160, 0);

    [UsedImplicitly]
	private void Awake()
    {
	    Hide();
	}

    private void clear()
    {
        foreach(UIEquipListButton button in mButtons)
        {
            Destroy(button.gameObject);
        }
        mButtons.Clear();
    }

    public void Show(EquipItem[] items)
    {
        Window.SetActive(true);

        clear();

        var localPos = mStartPos;
        for(var i = 0; i < items.Length; i++)
        {
            GameObject obj = UIPrefabPath.LoadUI(UIPrefabPath.EquipListButton, ScrollView, localPos);
            var button = obj.GetComponent<UIEquipListButton>();
            mButtons.Add(button);
            localPos.y -= obj.GetComponent<UISprite>().height; // 因為每個元件的高度都相同, 所以可以這樣增加.

            button.Set(items[i]);
        }
    }

    public void Hide()
    {
        Window.SetActive(false);
    }
}
