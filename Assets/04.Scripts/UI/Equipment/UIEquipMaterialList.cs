using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這是裝備介面右邊的材料清單列表, 會列出數值裝可鑲嵌所需的材料.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Set() 設定要顯示的材料. </item>
/// </list>
public class UIEquipMaterialList : MonoBehaviour
{
    /// <summary>
    /// 呼叫時機: 列表上的按鈕點擊時. 
    /// 參數: int index, 哪一個被點擊.
    /// </summary>
    public event CommonDelegateMethods.Int1 OnClickListener;

    public Transform[] ItemParents;
    public GameObject WarningMsg;

    private readonly List<UIEquipMaterialItem> mMaterialItems = new List<UIEquipMaterialItem>();

    [UsedImplicitly]
    private void Awake()
    {
        for(int i = 0; i < ItemParents.Length; i++)
        {
            GameObject obj = UIPrefabPath.LoadUI(UIPrefabPath.EquipMaterialItem, ItemParents[i]);
            var materialItem = obj.GetComponent<UIEquipMaterialItem>();
            materialItem.Init(i);
            mMaterialItems.Add(materialItem);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"> 列表要顯示的數值裝材料. </param>
    public void Set(List<UIEquipMaterialItem.Data> data)
    {
        for(int i = 0; i < mMaterialItems.Count; i++)
        {
            if(i < data.Count)
            {
                // 有資料.
                mMaterialItems[i].gameObject.SetActive(true);
                mMaterialItems[i].UpdateUI(data[i]);
            }
            else
            {
                // 沒資料.
                mMaterialItems[i].gameObject.SetActive(false);
            }
        }

        WarningMsg.SetActive(data.Count == 0);
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