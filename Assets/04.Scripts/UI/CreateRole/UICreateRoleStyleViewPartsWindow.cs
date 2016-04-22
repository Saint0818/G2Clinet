﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 創角介面右邊顯示可換裝項目的視窗.
/// </summary>
[DisallowMultipleComponent]
public class UICreateRoleStyleViewPartsWindow : MonoBehaviour
{
    /// <summary>
    /// <para>呼叫時機: 視窗內某個裝備被點擊時.</para>
    /// 參數:
    /// <list type="number">
    /// <item> part </item>
    /// <item> index: 視窗中, 由上往下數, 哪一個被點選.(從 0 開始) </item>
    /// <item> itemID </item>
    /// </list>
    /// </summary>
    public event Action<UICreateRole.EPart, int, int> SelectListener;

    public GameObject ScrollView;

    private UIScrollView mScrollView;

    /// <summary>
    /// 換裝物品的起點位置, 單位: Pixel.
    /// </summary>
    private readonly Vector3 mUIStartPos = new Vector3(3, 200, 0);

    /// <summary>
    /// 每個項目的高度間隔, 單位: Pixel.
    /// </summary>
    private const int UIHeightInterval = 120;

    private UICreateRole.EPart mPart;

    private readonly List<GameObject> mButtons = new List<GameObject>();

    [UsedImplicitly]
	private void Awake()
    {
        mScrollView = ScrollView.GetComponent<UIScrollView>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="part"></param>
    /// <param name="items"></param>
    /// <param name="selectedIndex"> 預設哪一個要被選擇. </param>
    public void UpdateData(UICreateRole.EPart part, TItemData[] items, int selectedIndex)
    {
        mScrollView.ResetPosition();
        mPart = part;

        clear();

        for(int i = 0; i < items.Length; i++)
        {
            var localPos = mUIStartPos;
            localPos.y -= UIHeightInterval * i;
//            var btn = createBtn(localPos);
            var btn = UIPrefabPath.LoadUI(UIPrefabPath.UICreateRoleStyleViewPartsWindowButton, ScrollView.transform, localPos);
            mButtons.Add(btn);

            var partBtn = btn.GetComponent<UICreateRoleStyleViewPartsWindowButton>();
            partBtn.Name = items[i].Name;
            partBtn.Atlas = TextureManager.Get.ItemAtlas(items[i].Atlas);
            int iconValue;
            if(!int.TryParse(items[i].Icon, out iconValue))
                Debug.LogErrorFormat("Can't parse Icon to integer, Item:{0}", items[i]);
            partBtn.Icon = iconValue;

            partBtn.ItemID = items[i].ID;
            partBtn.Index = i;
            partBtn.ClickListener += onPartSelected;
        }

        if(mButtons.Count > selectedIndex)
            mButtons[selectedIndex].GetComponent<UIToggle>().Set(true);
        else
            Debug.LogErrorFormat("Button.Count:{0}, SelectedIndex:{1}", mButtons.Count, selectedIndex);

        // NGUI 的 ScrollView 似乎不能正常顯示, 所以我必需要將 GameObject 關閉後再打開, 才可以正常顯示.
        StartCoroutine(reActive());
    }

    private IEnumerator reActive()
    {
        ScrollView.SetActive(false);
        // try and error, 我發現要等久一點, UI 才可以正常顯示.
        yield return new WaitForSeconds(0.01f); 
        ScrollView.SetActive(true);
    }

    private void clear()
    {
        foreach(var obj in mButtons)
        {
            Destroy(obj);
        }
        mButtons.Clear();
    }

//    private GameObject createBtn(Vector3 localPos)
//    {
//        GameObject partObj = Instantiate(Resources.Load<GameObject>(PartPath));
//        partObj.transform.parent = ScrollView.transform;
//        partObj.GetComponent<UIDragScrollView>().scrollView = mScrollView;
//
//        partObj.transform.localPosition = localPos;
//        partObj.transform.localRotation = Quaternion.identity;
//        partObj.transform.localScale = Vector3.one;
//        return partObj;
//    }

    private void onPartSelected(int index, int itemID)
    {
//        Debug.LogFormat("Index:{0}, ItemID:{1}", index, itemID);

        if(SelectListener != null)
            SelectListener(mPart, index, itemID);
    }
}
