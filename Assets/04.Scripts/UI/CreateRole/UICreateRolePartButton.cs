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

    [UsedImplicitly]
    private void Awake()
    {
    }

    public void OnToggleChange()
    {
        if(UIToggle.current.value)
        {
            Button.localPosition = mOnPos;
            Window.UpdateData(Equipment);
        }
        else
            Button.localPosition = mOffPos;
    }

}