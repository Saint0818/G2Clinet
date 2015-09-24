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
	private bool isInitBtn;
	private bool isRental;

	private UILabel name;
	private UILabel usetime;
	private UILabel abilityValue;
	private UILabel price;
	private UILabel PriceLabel;
	private UILabel FinishLabel;

	private UISprite pic;
	private UIButton equipBtn;
	private UILabel equipLabel;
	private UIButton buyBtn;
	private UISprite TrimBottom;
	public DateTime EndUseTime;

	public bool Enable
	{
		set{
			CheckItemKind();
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

	public bool IsRental
	{
		set{
			isRental = value;
			if(isRental)
				usetime.gameObject.SetActive(true);
			else
				usetime.gameObject.SetActive(false);
		}

		get{return isRental;}
	}

	public string Pic
	{
		set{
			if(pic)
				pic.spriteName = value;
		}
	}

	private TimeSpan currentTime;
	private bool isReantimeEnd;

	public TimeSpan UseTime
	{
		set{

			if(IsRental == false){
				return;
			}

			currentTime = value;

			if(usetime)
				usetime.text = currentTime.Days + " : " + currentTime.Hours + " : " + currentTime.Minutes;

			if(currentTime.TotalSeconds < 0){
				if(isReantimeEnd == false)
				{
					//End
					if(IsRental){
						usetime.gameObject.SetActive(false);
					}
				}
			}
		}

		get{return currentTime;}
	}

	public void CheckEquipBtnName()
	{
		if (IsRental) {
			if(currentTime.TotalSeconds > 0){

				if(TrimBottom.color != Color.white)
					TrimBottom.color = Color.white;

				if(Equip)
					equipLabel.text = "EQUIPED";
				else
					equipLabel.text = "EQUIP";
			}
			else{

				if(TrimBottom.color != Color.black)
					TrimBottom.color = Color.black;
				equipLabel.text = "FETTING";
			}
		} else {
			if(TrimBottom.color != Color.white)
				TrimBottom.color = Color.white;

			if(Equip)
				equipLabel.text = "EQUIPED";
			else
				equipLabel.text = "EQUIP";	
		}
	}

	public bool Equip
	{
		set{
			isEquip = value;
			equipBtn.defaultColor = (isEquip == true) ? Color.gray : new Color(0.431f, 0.976f, 0.843f,1);
			equipBtn.hover = (isEquip == true) ? Color.gray : new Color(0.431f, 0.976f, 0.843f,1);
			CheckEquipBtnName();
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
				TrimBottom = gameobject.transform.FindChild ("TrimBottom").gameObject.GetComponent<UISprite> ();

				equipBtn = gameobject.transform.FindChild ("EquipBtn").gameObject.GetComponent<UIButton> ();
				if(equipBtn){
					equipBtn.name = gameobject.name;
					equipLabel = equipBtn.transform.FindChild("EquipLabel").gameObject.GetComponent<UILabel>();
				}

				buyBtn = gameobject.transform.FindChild ("BuyBtn").gameObject.GetComponent<UIButton> ();

				if(buyBtn){
					buyBtn.name = gameobject.name;
					PriceLabel = buyBtn.transform.FindChild("PriceLabel").gameObject.GetComponent<UILabel>();
					FinishLabel = buyBtn.transform.FindChild("FinishLabel").gameObject.GetComponent<UILabel>();
				}
			}
		}
		isInit = name && usetime && abilityValue && pic;
	}

	private void CheckItemKind()
	{
		if(GameData.DItemData.ContainsKey(ID))
		{
			if(GameData.DItemData[ID].UseTime > 0)
			{
				IsRental = true;


				PriceLabel.gameObject.SetActive(true);
				FinishLabel.gameObject.SetActive(!PriceLabel.gameObject.activeSelf);
			}
			else
			{
				IsRental = false;
				equipLabel.text = "FITTING";
				TrimBottom.color = Color.black;
				PriceLabel.gameObject.SetActive(false);
				FinishLabel.gameObject.SetActive(!PriceLabel.gameObject.activeSelf);
			}
		}
	}

	public void InitBtttonFunction(EventDelegate BuyFunc, EventDelegate EquipFunc)
	{
		if (isInitBtn)
			return;

		if (buyBtn)
			buyBtn.onClick.Add (BuyFunc);
		else {
			isInitBtn = false;
			return;
		}

		if (equipBtn)
			equipBtn.onClick.Add(EquipFunc);
		else{
			isInitBtn = false;
			return;
		}

		isInitBtn = true;
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
	private const int avatarPartCount = 7;
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

	private TimeSpan checktime;

	void Update()
	{
		for(int i = 0; i < items.Length; i++)
		{
			if(items[i].Enable)
			{
				checktime = items[i].EndUseTime - System.DateTime.UtcNow;
				items[i].UseTime = checktime;

//				if(items[i].isTimeEnd == flase && checktime.TotalSeconds > 0)
//				{
//					items[i].UseTime = checktime.Days + " : " + checktime.Hours + ":" + checktime.Minutes;
//				}
//				else
//				{
//					items[i].isTimeEnd = true;
//				}




//				if(checktime.TotalSeconds > 0)
//				{
//					items[i].UseTime = (items[i].EndUseTime - System.DateTime.UtcNow);
//				}
			}

		}
	}

	void FixedUpdate(){
	}

	private string[] btnPaths = new string[avatarPartCount];
	private int test = 0;

	protected override void InitCom() {
		string mainBtnPath = UIName + "/MainView/Left/MainButton/";
		btnPaths [0] = mainBtnPath + "HairBtn";
		btnPaths [1] = mainBtnPath + "HandsBtn";
		btnPaths [2] = mainBtnPath + "ClothesBtn";
		btnPaths [3] = mainBtnPath + "PantsBtn";
		btnPaths [4] = mainBtnPath + "ShoesBtn";
		btnPaths [5] = mainBtnPath + "FaceBtn";
		btnPaths [6] = mainBtnPath + "BacksBtn";

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
		int index;

		if (int.TryParse (UIButton.current.name, out index)) {
			InitAvatarView(index);
		}

	}

	public void InitAvatarView(int index)
	{
		switch(index)
		{
		case 0:
			avatarPart = 1;//頭髮
			break;
		case 1:
			avatarPart = 2;//手飾
			break;
			
		case 2:
			avatarPart = 3;//上身
			break;
			
		case 3:
			avatarPart = 4;//下身
			break;
			
		case 4:
			avatarPart = 5;//鞋
			break;
			
		case 5:
			avatarPart = 10;//頭飾(共用）
			break;
			
		case 6:
			avatarPart = 11;//背部(共用)
			break;
			
		default:
			Debug.LogError("Can't found ItemKind");
			return;
		}
		
		if(CheckItemCount()){
			InitItems();
			InitEquipState();
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
			if (GameData.Team.Items.Length > 0 && item) {
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
				items[i].InitBtttonFunction(new EventDelegate(OnBuy), new EventDelegate(OnEquip));
			}

			if(items[i].ID != GameData.Team.Items[i].ID)
			{
				items[i].ID = GameData.Team.Items[i].ID;
				items[i].Position = GameData.DItemData[items[i].ID].Position;
				items[i].EndUseTime = GameData.Team.Items[i].UseTime;
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
			
		if (!CheckSameEquip ()) {
			Debug.LogWarning("save serverdata");
		}

		foreach (KeyValuePair<int, TEquip> item in Equips) {
			Debug.Log("** Equips : " + GameData.DItemData[item.Value.ID].Name);
		}
	}

	protected override void InitData() {
		InitAvatarView (0);
	}
	
	protected override void OnShow(bool isShow) {
		
	}
}
