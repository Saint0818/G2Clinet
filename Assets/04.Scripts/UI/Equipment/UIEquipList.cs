using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;

public delegate void ItemChangeCallback(int index, UIValueItemData item);

//Use in UISelectRole
public class UIEquipList : UIBase {
	private static UIEquipList instance = null;
	private const string UIName = "UIEquipList";
	private readonly Vector3 mStartPos = new Vector3(0, 110, 0);
	private ItemChangeCallback itemChangeCallback;

	private int mItemIndex;
	private UIValueItemData[] playerItems = new UIValueItemData[2];
	private List<UIValueItemData>[] storageItems = new List<UIValueItemData>[2];

	private UIScrollView scrollView;
	private List<UIEquipListButton> itemButtonList = new List<UIEquipListButton>();

	private int[] exchangeIndices = new int[8];
	private int[] stackIndices = new int[2];

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

		set {
			if (instance) {
				if (!value)
					RemoveUI(instance.gameObject);
				else
					instance.Show(value);
			} else
			if (value)
				Get.Show(value);
		}
	}

	public static UIEquipList Get {
		get {
			if (!instance)
				instance = LoadUI (UIName) as UIEquipList;

			return instance;
		}
	}

	void OnDestroy() {
		clear ();
		for (int i = 0; i < storageItems.Length; i++)
			storageItems [i].Clear ();

		playerItems = new UIValueItemData[0];
		storageItems = new List<UIValueItemData>[0];
	}

	private void clear() {
		for (int i = 0; i < itemButtonList.Count; i++)
			Destroy (itemButtonList[i].gameObject);

		itemButtonList.Clear ();
	}

	protected override void InitCom() {
		SetBtnFun (UIName + "/Center/NoBtn", OnClose);
		SetBtnFun (UIName + "/Center/DemountBtn", OnDemount);
		scrollView = GameObject.Find (UIName + "/Center/ScrollView").GetComponent<UIScrollView>();
	}

	public void InitItemData(int index, ItemChangeCallback changeCallback) {
		clear ();

		itemChangeCallback = changeCallback;
		mItemIndex = index;
		if (playerItems[0] == null) {
			for (int i = 0; i < playerItems.Length; i++) {
				playerItems [i] = findPlayerValueItemByKind (i + 17);
				storageItems [i] = findStorageItemsByKind (i + 17);
			}
		}

		var localPos = Vector3.zero;
		for (int i = 0; i < storageItems[index].Count; i++) {
			GameObject obj = UIPrefabPath.LoadUI(UIPrefabPath.UIEquipListButton, scrollView.transform, localPos);
			var element = obj.GetComponent<UIEquipListButton>();
			element.Set (storageItems [index][i], true);
			element.Init (OnEquip, i);
			itemButtonList.Add (element);
			localPos.y -= 130;
		}
	}

	private List<UIValueItemData> findStorageItemsByKind(int kind)
	{
		List<UIValueItemData> items = new List<UIValueItemData>();
		for(int i = 0; i < GameData.Team.ValueItems.Length; i++)
		{
			TValueItem storageItem = GameData.Team.ValueItems[i];
			if(!GameData.DItemData.ContainsKey(storageItem.ID))
			{
				Debug.LogErrorFormat("Can't find ItemData, {0}", storageItem);
				continue;
			}

			if(GameData.DItemData[storageItem.ID].Kind == kind)
			{
				UIValueItemData uiItem = UIValueItemDataBuilder.Build(
					GameData.DItemData[storageItem.ID], 
					storageItem.RevisionInlayItemIDs, storageItem.Num);
				
				uiItem.StorageIndex = i;
				items.Add(uiItem);
			}
		}

		return items;
	}

	private UIValueItemData findPlayerValueItemByKind(int kind)
	{
		if(GameData.Team.Player.ValueItems.ContainsKey(kind) &&
			GameData.DItemData.ContainsKey(GameData.Team.Player.ValueItems[kind].ID))
		{
			TItemData item = GameData.DItemData[GameData.Team.Player.ValueItems[kind].ID];
			int[] inlayItemIDs = GameData.Team.Player.ValueItems[kind].RevisionInlayItemIDs;
			return UIValueItemDataBuilder.Build(item, inlayItemIDs, GameData.Team.Player.ValueItems[kind].Num);
		}

		return UIValueItemDataBuilder.BuildEmpty();
	}

	public bool IsDataChanged()
	{
		for(var i = 0; i < playerItems.Length; i++)
			if(playerItems[i].StorageIndex != UIValueItemData.StorageIndexNone)
				return true;
		
		return false;
	}

	protected override void OnShow(bool isShow) {
		base.OnShow(isShow);
	}

	private void onChangeValueItem(bool ok)
	{
		
	}

	public void OnClose() {
		if (IsDataChanged ()) {
			int[] change = new int[8];
			int[] stackIndices = new int[2];

			for (int i = 0; i < change.Length; i++)
				change [i] = -2;
			
			for (int i = 0; i < stackIndices.Length; i++)
				stackIndices [i] = -2;

			for (int i = 0; i < playerItems.Length; i++)
				change [i + 6] = playerItems [i].StorageIndex;

			var protocol = new ValueItemExchangeProtocol();
			protocol.Send(change, stackIndices, onChangeValueItem);
		}

		Visible = false;
	}

	public void OnEquip(int index) {
		UIValueItemData item = playerItems [mItemIndex];
		playerItems [mItemIndex] = storageItems [mItemIndex][index];
		storageItems [mItemIndex][index] = item;

		InitItemData (mItemIndex, itemChangeCallback);
		if (itemChangeCallback != null)
			itemChangeCallback (mItemIndex, playerItems [mItemIndex]);
	}

	public void OnDemount() {
		if (playerItems [mItemIndex].Status != UIValueItemData.EStatus.CannotDemount) {
			storageItems [mItemIndex].Add (playerItems [mItemIndex]);
			playerItems [mItemIndex] = UIValueItemDataBuilder.BuildEmpty ();
			if (itemChangeCallback != null)
				itemChangeCallback (mItemIndex, playerItems [mItemIndex]);
		}
	}
}
