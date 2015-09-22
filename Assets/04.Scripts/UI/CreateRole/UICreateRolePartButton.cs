using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 創角的 StyleView 所使用, 控制左邊的按鈕行為(頭髮, 衣服, 褲子, 鞋子, 身體).
/// </summary>
[DisallowMultipleComponent]
public class UICreateRolePartButton : MonoBehaviour
{
    public UICreateRoleStyleView.EEquip Equipment;
    public UICreateRoleStyleViewPartsWindow Window;
    public Transform Button;

    private readonly Vector3 mOnPos = new Vector3(45, 0, 0);
    private readonly Vector3 mOffPos = new Vector3(-45, 0, 0);

    private TItemData[] mItems;

    [UsedImplicitly]
    private void Awake()
    {
    }

    public void SetData(TItemData[] data)
    {
        mItems = data;
    }

    public void OnToggleChange()
    {
        if(UIToggle.current.value)
        {
            Button.localPosition = mOnPos;
            Window.UpdateData(mItems);
        }
        else
            Button.localPosition = mOffPos;
    }

    public void SetSelected()
    {
        var toggle = GetComponent<UIToggle>();

        // 因為當 Toggle 是 true 時, 再設定為 true, 事件並不會送出, 所以才有這段特別的程式碼.
        if (toggle.value) 
            Window.UpdateData(mItems);
        else
            toggle.Set(true);
    }
}