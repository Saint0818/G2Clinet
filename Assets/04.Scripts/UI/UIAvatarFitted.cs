using UnityEngine;
using System.Collections;
using GameStruct;
using System;
using System.Collections.Generic;

public struct TItemAvatar
{
	public int Index;
	public int ID;
	public int Position;
	public string AbilityKind;
	public string AbilityValue;
	public GameObject gameobject;
	public Transform DisablePool;
	public Transform EnablePool;
	private bool isEquip;
	private bool isInit;

	private UILabel name;
	private UILabel usetime;
	private UILabel abilityValue;
	private UILabel price;
	private UISprite pic;
	private UIButton equipBtn;
	private UIButton buyBtn;

	public bool Enable
	{
		set{
			if(gameobject){
				gameobject.SetActive(value);
				gameobject.transform.parent = gameobject.activeSelf? EnablePool : DisablePool;
			}
		}
		get{
			if(gameobject)
				return gameobject.activeSelf;
			else
			{
				Debug.LogError("Must be Inited TItemAvatarPart.gameobject");
				return false;
			}
		}
	}

	public string Name
	{
		set{ if(name) name.text = value;}
		get{
			if(name)
				return name.text;
			else
				return "";
		}
	}

	public string Pic
	{
		set{
			if(pic)
				pic.spriteName = value;
		}
	}

	public DateTime UseTime
	{
		set{
			if(usetime)
				usetime.text = value.ToLongTimeString();
		}
//		get{
//			if(usetime)
//				return usetime.text;
//			else
//				return null;
//		}
	}

	public bool Equip
	{
		set{
			isEquip = value;
			equipBtn.defaultColor = (isEquip == true) ? new Color(0.431f, 0.976f, 0.843f,1) : Color.gray;
			equipBtn.hover = (isEquip == true) ? new Color(0.431f, 0.976f, 0.843f,1) : Color.gray;
		}
		get{
			return isEquip;
		}
	}

	public void Init()
	{
		if (!isInit) {
			if (gameobject) {
				name = gameobject.transform.FindChild ("ItemName").gameObject.GetComponent<UILabel> ();
				usetime = gameobject.transform.FindChild ("DeadlineLabel").gameObject.GetComponent<UILabel> ();
				abilityValue = gameobject.transform.FindChild ("BuyBtn/FinishLabel").gameObject.GetComponent<UILabel> ();
				pic = gameobject.transform.FindChild ("ItemPic").gameObject.GetComponent<UISprite> ();
				equipBtn = gameobject.transform.FindChild ("EquipBtn").gameObject.GetComponent<UIButton> ();
				equipBtn.name = gameobject.name;
				buyBtn = gameobject.transform.FindChild ("BuyBtn").gameObject.GetComponent<UIButton> ();
				buyBtn.name = gameobject.name;
				buyBtn.onClick.Add(new EventDelegate(UIAvatarFitted.Get.OnBuy));
				equipBtn.onClick.Add(new EventDelegate(UIAvatarFitted.Get.OnEquip));
			}
		}
		isInit = name && usetime && abilityValue && pic;
	}
}

public struct TEquip
{
	public int ID;
	public int Kind;
}

public class UIAvatarFitted : UIBase {
	private static UIAvatarFitted instance = null;
	private const string UIName = "UIAvatarFitted";
	private const int avatarPartCount = 6;
	private GameObject item;
	private TItemAvatar[] items;
	private bool isInit = false;
	private UIGrid grid;
	private UIScrollView scrollView;
	private GameObject disableGroup;

	private Dictionary<int, TEquip> Equips = new Dictionary<int, TEquip>();

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UIAvatarFitted Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIAvatarFitted;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}

	void FixedUpdate(){
		
	}

	private string[] btnPaths = new string[6];

	protected override void InitCom() {
		btnPaths [0] = UIName + "/MainView/Left/MainButton/HairBtn";
		btnPaths [1] = UIName + "/MainView/Left/MainButton/ClothesBtn";
		btnPaths [2] = UIName + "/MainView/Left/MainButton/PantsBtn";
		btnPaths [3] = UIName + "/MainView/Left/MainButton/ShoesBtn";
		btnPaths [4] = UIName + "/MainView/Left/MainButton/HandsBtn";
		btnPaths [5] = UIName + "/MainView/Left/MainButton/BacksBtn";

		for(int i = 0; i < btnPaths.Length; i++){
			SetBtnFunReName (btnPaths[i], DoAvatarTab, i.ToString());
		}

		SetBtnFun (UIName + "/MainView/BottomLeft/BackBtn", DoReturn);
		SetBtnFun (UIName + "/MainView/BottomRight/CheckBtn", DoSave);

		item = Resources.Load ("Prefab/UI/Items/ItemAvatarBtn") as GameObject;
		grid = GameObject.Find (UIName + "/MainView/Left/ItemList/UIGrid").GetComponent<UIGrid>();
		scrollView = GameObject.Find (UIName + "/MainView/Left/ItemList").GetComponent<UIScrollView>();

		disableGroup = new GameObject ();
		disableGroup.name = "disableGroup";
		disableGroup.transform.parent = scrollView.transform;

		InitEquips ();

		isInit = true;
	}

	private int avatarPart = 0;

	private int GetItemKind(int id)
	{
		if (GameData.DItemData.ContainsKey (id)) {
			return	GameData.DItemData[id].Kind;
		} else {
			Debug.LogError("Can not find ID in ItemData");
			return -1;
		}
	}

	private void InitEquips()
	{
		if (GameData.Team.Player.Items.Length > 0) {
			for(int i = 0; i < GameData.Team.Player.Items.Length;i++)
			{
				int kind = GetItemKind(GameData.Team.Player.Items[i].ID);

				if(kind > 0 && kind < 8 && !Equips.ContainsKey(GameData.Team.Player.Items[i].ID))
				{
					TEquip equip = new TEquip();
					equip.ID = GameData.Team.Player.Items[i].ID;
					equip.Kind = kind;
					Equips.Add(equip.ID, equip);
				}
			}
		}

	}

	public void DoAvatarTab()
	{
		if (int.TryParse (UIButton.current.name, out avatarPart)) {
			if(CheckItemCount()){
				InitItems();
				InitEquipState();
			}
		}
	}

	private bool CheckSameEquip()
	{
		if (GameData.Team.Player.Items.Length > 0) {
			if(Equips.Count > 0){
				for(int i = 0; i < GameData.Team.Player.Items.Length; i++)
					if(!Equips.ContainsKey(GameData.Team.Player.Items[i].ID))
						return false;
			}
		} else {
			if(Equips.Count > 0)
				return false;
		}
	
		return true;
	}

	private bool CheckItemCount()
	{
		if (isInit && GameData.Team.Items!= null) {
			if (GameData.Team.Items.Length > 0 && item && avatarPart < btnPaths.Length) {
				if(items == null){
					items = new TItemAvatar[GameData.Team.Items.Length];
				}
				else
				{
					if(GameData.Team.Items.Length == items.Length)
					{
						return true;
					}
					else if(GameData.Team.Items.Length > items.Length){
						Array.Resize(ref items, GameData.Team.Items.Length);
					}
				}
				return true;
			}
		}

		return false;
	}

	private void InitItems()
	{
		for(int i = 0; i < items.Length; i++){
			if(items[i].gameobject == null){
				items[i].gameobject = Instantiate(item) as GameObject;
				items[i].gameobject.transform.parent = grid.transform;
				items[i].gameobject.transform.localScale = Vector3.one;
				items[i].gameobject.name = i.ToString();
				items[i].DisablePool = disableGroup.gameObject.transform;
				items[i].EnablePool = grid.gameObject.transform;
				items[i].Init();
			}

			if(items[i].ID != GameData.Team.Items[i].ID)
			{
				items[i].ID = GameData.Team.Items[i].ID;
				items[i].Position = GameData.DItemData[items[i].ID].Position;
				items[i].UseTime = GameData.Team.Items[i].UseTime;
				items[i].Name =  GameData.DItemData[items[i].ID].Name;
				items[i].Pic = GameData.DItemData[items[i].ID].Icon;
				items[i].AbilityKind = GetItemKind(items[i].ID).ToString();
			}

			if(i >= GameData.Team.Items.Length)
			{
				items[i].Enable = false;
			}
			else {
				if(GameData.DItemData.ContainsKey(items[i].ID) && GetItemKind(items[i].ID) != avatarPart)
				{
					items[i].gameobject.transform.localPosition = Vector3.zero;
					items[i].Enable = false;
				}
				else
				{
					#if UIAvatarFitted_ShowAll
					items[i].Enable = true;
					#else
					if(items[i].Position != GameData.Team.Player.BodyType)
						items[i].Enable = false;
					else
						items[i].Enable = true;
					#endif
				}
			}
		}

		grid.Reposition ();
		grid.gameObject.SetActive (false);
		grid.gameObject.SetActive (true);

		scrollView.ResetPosition ();
		scrollView.enabled = false;
		scrollView.enabled = true;
	}

	public void OnBuy()
	{
		int index;

		if (int.TryParse (UIButton.current.name, out index)) {
					
		}
			Debug.Log ("Buy id : " + index);
	}

	public void InitEquipState()
	{
		UnEquipAll();

		if (GameData.Team.Player.Items.Length > 0) {
			for(int i = 0; i < GameData.Team.Player.Items.Length; i++)
			{
				if(GameData.DItemData.ContainsKey(GameData.Team.Player.Items[i].ID) && 
				   GetItemKind(GameData.Team.Player.Items[i].ID) == avatarPart){
					for(int y = 0; y < items.Length; y++)
					{
						if(GameData.Team.Player.Items[i].ID == items[y].ID)
							items[y].Equip = true;
					}
				}
			}
		} 
	}

	private void UnEquipAll()
	{
		for(int i = 0; i < items.Length;i++)
			items[i].Equip = false;
	}

	public void OnEquip()
	{
		int index;

		if (int.TryParse (UIButton.current.name, out index)) {
			if(index < items.Length)
			{
				if(!items[index].Equip){
					items[index].Equip = true;

					if(!Equips.ContainsKey(items[index].ID))
					{
						TEquip equip = new TEquip();
						equip.ID = items[index].ID;
						equip.Kind = GetItemKind(items[index].ID);
						Equips.Add(equip.ID, equip);
					}

					for(int i = 0; i < items.Length;i++)
					{
						//找出已裝備的Item
						if(items[index].Enable && i != index)
						{
							items[i].Equip = false;
							if(Equips.ContainsKey(items[i].ID))
								Equips.Remove(items[i].ID);
						}
					}
				}
				else
				{
					items[index].Equip = false;
				}
			}
		}
			Debug.Log ("Equip id : " + index);
	}

	private void DoReturn()
	{
		Show (false);
		UIMain.Visible = true;
	}

	private void DoSave()
	{
		DoReturn ();

		for (int i = 0; i < Equips.Count; i++) {
			Debug.Log("** Equips : " + GameData.DItemData[Equips[i].ID].Name);
		}
			
		if (!CheckSameEquip ()) {
			Debug.LogWarning("save serverdata");
		}
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

}
