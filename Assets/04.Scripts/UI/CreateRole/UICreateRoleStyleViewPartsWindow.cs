using System.Collections;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 創角介面右邊顯示可換裝項目的視窗.
/// </summary>
[DisallowMultipleComponent]
public class UICreateRoleStyleViewPartsWindow : MonoBehaviour
{
    public GameObject ScrollView;

    private UIScrollView mScrollView;

    private const string PartPath = "Prefab/UI/Items/ItemEquipBtn";

    /// <summary>
    /// 項目的起點位置, 單位: Pixel.
    /// </summary>
    private readonly Vector3 mUIStartPos = new Vector3(15, 203, 0);

    /// <summary>
    /// 每個項目的高度間隔, 單位: Pixel.
    /// </summary>
    private const int UIHeightInterval = 100; 

    [UsedImplicitly]
	private void Awake()
    {
        mScrollView = ScrollView.GetComponent<UIScrollView>();
    }

    public void UpdateData(TItemData[] data)
    {
        foreach(TItemData item in data)
        {
            Debug.Log(item);
        }

        clear();

        for(int i = 0; i < data.Length; i++)
        {
            var localPos = mUIStartPos;
            localPos.y -= UIHeightInterval * i;
            Add(data[i], localPos);
        }

        // NGUI 的 ScrollView 似乎不能正常顯示, 所以我必需要將 GameObject 關閉後再打開, 才可以正常顯示.
        StartCoroutine(reActive());
    }

    private IEnumerator reActive()
    {
        ScrollView.SetActive(false);
//        yield return new WaitForEndOfFrame();
        // try and error, 我發現要等久一點, UI 才可以正常顯示.
        yield return new WaitForSeconds(0.01f); 
        ScrollView.SetActive(true);
    }

    private void clear()
    {
        foreach(Transform child in ScrollView.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Add(TItemData item, Vector3 localPos)
    {
        GameObject partObj = Instantiate(Resources.Load<GameObject>(PartPath));
        partObj.transform.parent = ScrollView.transform;
        partObj.GetComponent<UIDragScrollView>().scrollView = mScrollView;

        partObj.transform.localPosition = localPos;
        partObj.transform.localRotation = Quaternion.identity;
        partObj.transform.localScale = Vector3.one;

        partObj.GetComponent<UICreateRoleStyleViewPartsWindowButton>().Name = item.Name;
        partObj.GetComponent<UICreateRoleStyleViewPartsWindowButton>().Icon = item.Icon;
    }
}
