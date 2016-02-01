using System;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 創角的 StyleView 所使用, 控制左邊的按鈕行為(頭髮, 衣服, 褲子, 鞋子, 身體).
/// </summary>
[DisallowMultipleComponent]
public class UICreateRolePartButton : MonoBehaviour
{
    public UICreateRole.EPart Part;
    public UICreateRoleStyleViewPartsWindow Window;
    public Transform Button;

    /// <summary>
    /// <para> 呼叫時機: 當按鈕被點選時. </para>
    /// </summary>
    public event Action<UICreateRole.EPart> SelectedListener;

    private readonly Vector3 mOnPos = new Vector3(45, 0, 0);
    private readonly Vector3 mOffPos = new Vector3(-45, 0, 0);

    private TItemData[] mItems;

    /// <summary>
    /// 右邊視窗的項目中, 哪一個被點選.
    /// </summary>
    public int SelectedIndex { set; get; }

    [UsedImplicitly]
    private void Awake()
    {
    }

    public void SetData(TItemData[] data)
    {
        SelectedIndex = 0;
        mItems = data;
    }

    public void OnToggleChange()
    {
        if(UIToggle.current.value)
        {
            Button.localPosition = mOnPos;
            Window.UpdateData(Part, mItems, SelectedIndex);
            notifySelected();
        }
        else
            Button.localPosition = mOffPos;
    }

    public void SetSelected()
    {
        var toggle = GetComponent<UIToggle>();

        // 因為當 Toggle 是 true 時, 再設定為 true, 事件並不會送出, 所以才有這段特別的程式碼.
        if (toggle.value)
        {
            Window.UpdateData(Part, mItems, SelectedIndex);
            notifySelected();
        }
        else
            toggle.Set(true);
    }

    public void OnPartItemSelected(UICreateRole.EPart part, int index, int itemID)
    {
//        Debug.LogFormat("Part:{0}, Index:{1}, ItemID:{2}", part, index, itemID);

        if(part == Part)
            SelectedIndex = index;
    }

    public TItemData RandomItemData()
    {
        return mItems[UnityEngine.Random.Range(0, mItems.Length - 1)];
    }

    private void notifySelected()
    {
        if(SelectedListener != null)
            SelectedListener(Part);
    }
}