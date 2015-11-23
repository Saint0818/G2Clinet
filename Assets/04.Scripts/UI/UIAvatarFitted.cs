using System;
using System.Collections.Generic;
using GameEnum;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public struct TItemAvatar
{
	public int Index; //-1 : Player.Item 
	public int Position;
	public int Kind;
	public string AbilityKind;
	public string AbilityValue;
	public GameObject Self;
	public Transform DisablePool;
	public Transform EnablePool;
	private bool isEquip;
	private bool isInit;
	private bool isInitBtn;
	private bool isRental;
	private bool isSelect;
	private bool isEnableBuy;
	private int id;
	private int usekind;
	private EAvatarMode mode;

	private UILabel name;
	private UILabel usetime;
	private UILabel abilityValue;
	private UILabel PriceLabel;
	private UILabel InfoLabel;

	private UISprite pic;
	private UISprite OutLine;
	private UIButton equipBtn;
//	private UILabel equipLabel;
	private UIButton buyBtn;
	private UISprite TrimBottom;
	private UISprite SellSelect;
	private UISprite EquipedIcon;
//	private UISlider timeBar;
	public DateTime EndUseTime;
	public int UseTotalScenes;

	public EAvatarMode Mode
	{
		set{
			mode = value;

			if(mode == EAvatarMode.Sell)
			{
				if(!Equip){
					equipBtn.defaultColor = GColor.Red;
					equipBtn.hover = GColor.Red;
				}
			}
			else
			{
				if(equipBtn){
					equipBtn.defaultColor = (Equip == true) ? GColor.Equip : Color.white;
					equipBtn.hover = (Equip == true) ? GColor.Equip : Color.white;
				}
			}
		}
		get{return mode;}
	}

	public int ID 
	{
		set{
			id = value;
//			CheckItemUseKind();
		}
		get{return id;}
	}

	public int UseKind
	{
		// 永久性裝備 : -1, 時效性裝備 : 1, 已過期裝備 : 2
		set{
			usekind = value;
//			CheckItemUseKind();
		}
		get{return usekind;}
	}

	public bool Enable
	{
		set{
			CheckEquipBtnName();

			if(Self){
				Self.SetActive(value);
				Self.transform.parent = Self.activeSelf? EnablePool : DisablePool;
			}
		}
		get{
			if(Self)
				return Self.activeSelf;
			else
			{
				Debug.LogError("Must be Inited TItemAvatarPart.gameobject");
				return false;
			}
		}
	}

	public bool EnableBuy
	{
		set{
			isEnableBuy = value;
			PriceLabel.gameObject.SetActive(isEnableBuy);
			InfoLabel.gameObject.SetActive(!isEnableBuy);
		}

		get{return isEnableBuy;}
	}

	public bool Selected
	{
		set{
			isSelect = value;
			SellSelect.gameObject.SetActive(isSelect);
		}
		get {
			return isSelect;
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

	public int Quality
	{
		set{OutLine.spriteName = string.Format("Equipment_{0}", value);}
	}

//	public bool IsRental
//	{
//		set{
//			isRental = value;
//			if(isRental)
//				usetime.gameObject.SetActive(true);
//			else
//
//		}
//
//		get{return isRental;}
//	}

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
			currentTime = value;
			if(currentTime.TotalDays > 1)
				usetime.text = string.Format("{0} Day", currentTime.Days);
			else if(currentTime.TotalHours > 1 && currentTime.TotalDays < 1)
				usetime.text = string.Format("{0}H {1}M", currentTime.Hours, currentTime.Minutes);
			else
				usetime.text = string.Format("{0}M {1}S", currentTime.Minutes, currentTime.Seconds);
		}

		get{return currentTime;}
	}

	public void CheckEquipBtnName()
	{
		switch (UseKind) 
		{
			case 2:
				if(mode == EAvatarMode.Sell){
//					equipLabel.text = "SELL";
				}else{
					TrimBottom.enabled = true;
//					equipLabel.text = "FETTING";
				}
				break;
			default:
				if(mode == EAvatarMode.Sell){
//					if(Equip)
//						equipLabel.text = "EQUIPED";
//					else
//						equipLabel.text = "SELL";
				}else{
					TrimBottom.enabled = false;
					
//					if(Equip)
//						equipLabel.text = "EQUIPED";
//					else
//						equipLabel.text = "EQUIP";
				}
			break;
		}
	}

	public bool Equip
	{
		set{
			isEquip = value;
			EquipedIcon.enabled = isEquip;
			equipBtn.defaultColor = (isEquip == true) ? GColor.Equip : GColor.White;
			equipBtn.hover = (isEquip == true) ? GColor.Equip : GColor.White;
			CheckEquipBtnName();
		}
		get{
			return isEquip;
		}
	}

	public void Init()
	{
		if (!isInit) {
			if (Self) {
				Mode = EAvatarMode.Normal;
				name = Self.transform.FindChild ("ItemName").gameObject.GetComponent<UILabel> ();
				usetime = Self.transform.FindChild ("DeadlineLabel").gameObject.GetComponent<UILabel> ();
//				timeBar = usetime.transform.FindChild ("TimeBar").gameObject.GetComponent<UISlider> ();
				abilityValue = Self.transform.FindChild ("BuyBtn/InfoLabel").gameObject.GetComponent<UILabel> ();
				pic = Self.transform.FindChild ("ItemPic").gameObject.GetComponent<UISprite> ();
				OutLine = Self.transform.FindChild ("ItemPic/OutLine").gameObject.GetComponent<UISprite> ();
				TrimBottom = Self.transform.FindChild ("TrimBottom").gameObject.GetComponent<UISprite> ();
				SellSelect = Self.transform.FindChild ("SellSelect").gameObject.GetComponent<UISprite> ();
				Selected = false;
				EquipedIcon = Self.transform.FindChild ("EquipedIcon").gameObject.GetComponent<UISprite> ();
				equipBtn = Self.transform.GetComponent<UIButton> ();

				if(equipBtn){
					equipBtn.name = Self.name;
//					equipLabel = equipBtn.transform.FindChild("EquipLabel").gameObject.GetComponent<UILabel>();
				}

				buyBtn = Self.transform.FindChild ("BuyBtn").gameObject.GetComponent<UIButton> ();

				if(buyBtn){
					buyBtn.name = Self.name;
					PriceLabel = buyBtn.transform.FindChild("PriceLabel").gameObject.GetComponent<UILabel>();
					InfoLabel = buyBtn.transform.FindChild("InfoLabel").gameObject.GetComponent<UILabel>();
				}
			}
		}
		isInit = name && usetime && abilityValue && pic;
	}

//	public void CheckItemUseKind()
//	{
//		if(GameData.DItemData.ContainsKey(ID))
//		{
//			switch(usekind)
//			{
//				case -1:
//					usetime.gameObject.SetActive(false);
//					break;
//				case 1:
//					usetime.gameObject.SetActive(true);
//					break;
//			}
//			if(GameData.DItemData[ID].UseTime > 0)
//			{
//				IsRental = true;
//				PriceLabel.gameObject.SetActive(true);
//				FinishLabel.gameObject.SetActive(!PriceLabel.gameObject.activeSelf);
//			}
//			else
//			{
//				IsRental = false;
//				PriceLabel.gameObject.SetActive(false);
//				FinishLabel.gameObject.SetActive(!PriceLabel.gameObject.activeSelf);
//			}
//		}
//	}

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

	public void Update()
	{
		if(Enable){
			switch (UseKind) {
				case 1:
					TimeSpan checktime;
					checktime = EndUseTime - System.DateTime.UtcNow;
					
					if(checktime.TotalSeconds > 0)
					{
//						if(timeBar.enabled == false)
//							timeBar.enabled = true;
						
						EnableBuy = false;
//						timeBar.value = (float)checktime.TotalSeconds / (float)UseTotalScenes;
						UseTime = checktime;
					}
					else{
						UseKind = 2;
					}
					break;
				case 2:
//					timeBar.enabled = false;
					usetime.gameObject.SetActive(false);
					EnableBuy = true;
					abilityValue.enabled = false;
					break;
				default:
					usetime.gameObject.SetActive(false);
					EnableBuy = false;
//					timeBar.enabled = false;
					abilityValue.enabled = true;
					break;
			}
		}
	}
}

public struct TEquip
{
	public int Kind;
	public int ID;
	public int Index;
}

public enum EAvatarMode
{
	Sell,
	Sort,
	Normal
}

public class UIAvatarFitted : UIBase {
	private static UIAvatarFitted instance = null;
	private const string UIName = "UIAvatarFitted";
	private const int avatarPartCount = 7;
	private GameObject item;
	private TItemAvatar[] backpackItems;
	private UIGrid grid;
	private Transform enablePool;
	private UIScrollView scrollView;
	private GameObject disableGroup;
	private int enableCount = 0;
	private int avatarPart = 1;
	private GameObject SellCount;
	private UILabel TotalPriceLabel;
	private int totalPrice = 0;
	
	private Dictionary<int, TEquip> Equips = new Dictionary<int, TEquip>();
	private Dictionary<int, TEquip> UnEquips = new Dictionary<int, TEquip>();
	private TAvatar EquipsAvatar = new TAvatar();

	private TimeSpan checktime;
	private GameObject avatar;
	private string[] btnPaths = new string[avatarPartCount];
	public EAvatarMode Mode = EAvatarMode.Normal;

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
			if (!isShow){
				RemoveUI(UIName);
				UIPlayerMgr.Get.Enable = false;
			}
			else
				instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}

	void Update()
	{
		if(backpackItems.Length > 0)
			for(int i = 0; i < backpackItems.Length; i++)
				backpackItems[i].Update();
	}

	protected override void InitCom() {
		string mainBtnPath = UIName + "/MainView/Left/MainButton/";
		btnPaths [0] = mainBtnPath + "HairBtn";
		btnPaths [1] = mainBtnPath + "HandsBtn";
		btnPaths [2] = mainBtnPath + "ClothesBtn";
		btnPaths [3] = mainBtnPath + "PantsBtn";
		btnPaths [4] = mainBtnPath + "ShoesBtn";
		btnPaths [5] = mainBtnPath + "FaceBtn";
		btnPaths [6] = mainBtnPath + "BacksBtn";

		for(int i = 0; i < btnPaths.Length; i++)
			SetBtnFunReName (btnPaths[i], DoAvatarTab, i.ToString());

		SetBtnFun (UIName + "/MainView/BottomLeft/BackBtn", OnReturn);
		SetBtnFun (UIName + "/MainView/BottomRight/CheckBtn", OnSave);
		SetBtnFun (UIName + "/MainView/BottomLeft/SortBtn", OnSortMode);
		SetBtnFun (UIName + "/MainView/BottomLeft/SellBtn", OnSellMode);
		SetBtnFun (UIName + "/MainView/BottomLeft/SellBtn/SellCount/CancelBtn", OnCancelSell);
		SellCount = GameObject.Find (UIName + "/MainView/BottomLeft/SellBtn/SellCount");
		TotalPriceLabel = SellCount.transform.FindChild ("TotalPrice").gameObject.GetComponent<UILabel> ();
		SellCount.SetActive (false);
	
		item = Resources.Load ("Prefab/UI/Items/ItemAvatarBtn") as GameObject;
//		grid = GameObject.Find (UIName + "/MainView/Left/ItemList/UIGrid").GetComponent<UIGrid>();
		enablePool = GameObject.Find (UIName + "/MainView/Left/ItemList").transform;
		scrollView = GameObject.Find (UIName + "/MainView/Left/ItemList").GetComponent<UIScrollView>();

		disableGroup = new GameObject ();
		disableGroup.name = "disableGroup";
		disableGroup.transform.parent = scrollView.transform;

		InitEquips ();
//		isInit = true;
	}



	private void InitEquips()
	{
		if (GameData.Team.Player.Items.Length > 0) {
			for(int i = 0; i < GameData.Team.Player.Items.Length;i++){
				int kind = i;
				if(GameData.Team.Player.Items[i].ID > 0)
					kind = GameFunction.GetItemKind(GameData.Team.Player.Items[i].ID);

				if(kind < 8){
					TEquip equip = new TEquip();
					equip.ID = GameData.Team.Player.Items[i].ID;
					equip.Kind = kind;
					equip.Index = -1;
					AddEquipItem(equip.Kind, equip);
				}
			}
		}

		ItemIdTranslateAvatar();
	}

	public void DoAvatarTab()
	{
		int index;

		if (int.TryParse (UIButton.current.name, out index)) {
			//avatarPart 1:頭髮 2手飾 3上身 4下身 5鞋 6頭飾(共用）7背部(共用)
			avatarPart = index + 1;
			UpdateAvatar();
		}
	}

	public void UpdateAvatar(bool clearEquipsData = false)
	{
		if(clearEquipsData)
			InitEquips ();

		InitItemCount();
		InitItemsData();
		InitEquipState();
		UpdateView();
	}

	private void UpdateSellMoney()
	{
		int total = 0;

		for (int i = 0; i < backpackItems.Length; i++)
			if (backpackItems [i].Selected)
			{
				int id = backpackItems [i].ID;
				if(GameData.DItemData.ContainsKey(id))
					total += GameData.DItemData [id].Sell;
			}

		totalPrice = total;
		TotalPriceLabel.text = string.Format("Total : {0}", totalPrice);
	}
	
	private bool CheckSameEquip()
	{
		//檢查是否有裝備背包Item
		foreach (KeyValuePair<int, TEquip> item in Equips) {
			if(item.Value.Index > 0)
				return false;
		}

		//檢查是否有脫掉裝備
		foreach (KeyValuePair<int, TEquip> item in UnEquips) {
			if(item.Value.ID > 0)
				return false;
		}
		return true;
	}

	private int GetAvatarCountInTeamItem()
	{
		if (GameData.Team.Items != null)
			return GameData.Team.Items.Length;
		else
			return 0;
	}

	private int GetAvatarCountInPlayerItem()
	{
		if(GameData.Team.Player.Items != null)
			return GameData.Team.Player.Items.Length;
		else
			return 0;
	}

	private void InitItemCount()
	{
		int all =  GetAvatarCountInTeamItem() + GetAvatarCountInPlayerItem();

		if (backpackItems == null) {
			backpackItems = new TItemAvatar[all];
		}
		else
		{
			if(all > backpackItems.Length)//{
				Array.Resize(ref backpackItems, all);
		}
	}

	private void InitItemsData()
	{
		for(int i = 0; i < backpackItems.Length; i++){
			//InitCom
			if(backpackItems[i].Self == null){
				backpackItems[i].Self = Instantiate(item) as GameObject;
				backpackItems[i].Self.transform.parent = enablePool;
//					grid.transform;
				backpackItems[i].Self.transform.localScale = Vector3.one;
				backpackItems[i].Self.name = i.ToString();
				backpackItems[i].DisablePool = disableGroup.gameObject.transform;
				backpackItems[i].EnablePool = enablePool;
//					grid.gameObject.transform;
				backpackItems[i].Init();
				backpackItems[i].InitBtttonFunction(new EventDelegate(OnBuy), new EventDelegate(OnEquip));
			}

			//InitData
			if(GetAvatarCountInTeamItem() > 0 && i < GetAvatarCountInTeamItem())
			{
				//Team.Items
					backpackItems[i].ID = GameData.Team.Items[i].ID;
					backpackItems[i].Position = GameData.DItemData[backpackItems[i].ID].Position;
					backpackItems[i].EndUseTime = GameData.Team.Items[i].UseTime;
					backpackItems[i].Name =  GameData.DItemData[backpackItems[i].ID].Name;
					backpackItems[i].Pic = string.Format("Item_{0}", GameData.DItemData[backpackItems[i].ID].Icon);
					backpackItems[i].Quality = GameData.DItemData[backpackItems[i].ID].Quality;
					backpackItems[i].Kind = GameFunction.GetItemKind(backpackItems[i].ID);
					backpackItems[i].UseKind = GameData.Team.Items[i].UseKind;
					backpackItems[i].Index = i;
					backpackItems[i].UseTotalScenes = GameData.DItemData[backpackItems[i].ID].UseTime * 3600;
			}
			else if(i >= GetAvatarCountInTeamItem() && i < GetAvatarCountInPlayerItem() + GetAvatarCountInTeamItem())
			{
				//Player.Items
				int playerItemIndex = i - GetAvatarCountInTeamItem();

				if(playerItemIndex < GameData.Team.Player.Items.Length){

					int id = GameData.Team.Player.Items[playerItemIndex].ID;

					backpackItems[i].ID = id;

						if(id > 0){
							backpackItems[i].Position = GameData.DItemData[id].Position;
							backpackItems[i].EndUseTime = GameData.Team.Player.Items[playerItemIndex].UseTime;
							backpackItems[i].Name =  GameData.DItemData[id].Name;
							backpackItems[i].Pic = string.Format("Item_{0}", GameData.DItemData[id].Icon);
							backpackItems[i].Quality = GameData.DItemData[id].Quality;
							backpackItems[i].Kind = GameFunction.GetItemKind(id);
							backpackItems[i].UseKind = GameData.Team.Player.Items[playerItemIndex].UseKind;
							backpackItems[i].Index = -1;
						}
				}
			}
			else
			{
				backpackItems[i].ID = 0;
			}
		}
	}

	private Vector3 GetItemPos(int count)
	{
		return new Vector3 (210 * (int)(count / 2), (count % 2 == 0 ? 115 : -120), 0);
	}

	public void UpdateView()
	{
		enableCount = 0;
		int filter = 0;
		int sort = PlayerPrefs.GetInt(ESave.AvatarSort.ToString());

		if(PlayerPrefs.HasKey(ESave.AvatarFilter.ToString()))
			filter = PlayerPrefs.GetInt(ESave.AvatarFilter.ToString());
		else{
			filter = 2;
			PlayerPrefs.SetInt(ESave.AvatarFilter.ToString(), 2);
			PlayerPrefs.Save();
		}

		for (int i = 0; i < backpackItems.Length; i++) {
			//ItemVisable
			if(backpackItems[i].ID > 0 && GameData.DItemData.ContainsKey(backpackItems[i].ID) && backpackItems[i].Kind == avatarPart)
			{
				backpackItems[i].Mode = Mode;
				#if UIAvatarFitted_ShowAll
				items[i].Enable = true;
				#else
				if(backpackItems[i].Kind < 6)
				{
					if(backpackItems[i].Position != GameData.Team.Player.BodyType)
						backpackItems[i].Enable = false;
					else
					{
						backpackItems[i].Enable = true;
						enableCount++;
					}
				}
				else{
					backpackItems[i].Enable = true;
					enableCount++;
				}
				#endif

				switch(filter)
				{
					case 0:
						if(!backpackItems[i].Equip)
							backpackItems[i].Enable = true;
						else
							backpackItems[i].Enable = false;
						break;
					case 1:
						if(backpackItems[i].Equip)
							backpackItems[i].Enable = true;
						else
							backpackItems[i].Enable = false;
						break;
				}
			}
			else
			{
				backpackItems[i].Self.transform.localPosition = Vector3.zero;
				backpackItems[i].Enable = false;
			}

			if(Mode == EAvatarMode.Sell && backpackItems[i].Equip)
				backpackItems[i].Enable = false;
		}

		int count = 0;

		List<TItemAvatar> sortlist = new List<TItemAvatar> ();
		for (int i = 0; i < backpackItems.Length; i++)
			if (backpackItems [i].Enable)
				sortlist.Add (backpackItems [i]);
			
		switch(sort)
		{
			case 0:
				sortlist.Sort((x, y) => { return -x.EndUseTime.CompareTo(y.EndUseTime); });
				for(int i = 0; i< sortlist.Count;i++){
					sortlist[i].Self.transform.localPosition = GetItemPos(count);
					count++;
				}
			break;
			case 1:
				sortlist.Sort((x, y) => { return x.EndUseTime.CompareTo(y.EndUseTime); });
				for(int i = 0; i< sortlist.Count;i++){
					sortlist[i].Self.transform.localPosition = GetItemPos(count);;
					count++;
				}
				break;
			default:
				for(int i = 0; i < backpackItems.Length; i++)
					if(backpackItems[i].Enable)
					{
						backpackItems[i].Self.transform.localPosition = GetItemPos(count);;
						count++;
					}
						break;
		}

		sortlist.Clear ();
		enablePool.gameObject.SetActive (false);
		enablePool.gameObject.SetActive (true);
//		grid.Reposition ();
//		grid.gameObject.SetActive (false);
//		grid.gameObject.SetActive (true);
		scrollView.ResetPosition ();
		scrollView.enabled = false;
		scrollView.enabled = true;
	}

	public void OnBuy()
	{
		int index;

		if (int.TryParse (UIButton.current.name, out index)) {
			Debug.Log ("Buy id : " + index);
		}
	}

	public void OnSellMode()
	{
		if (Mode == EAvatarMode.Sort)
			return;
		else{
			UpdateSellMoney ();

			if(Mode == EAvatarMode.Normal){
				if(CheckSameEquip()){
					ChangeMode(EAvatarMode.Sell);
				}else{
					//ask need save?
					OnSave();//yes
//					UpdateAvatar(true);//No
					ChangeMode(EAvatarMode.Sell);
				}
			}
			else{
				//sell something
				if(totalPrice > 0)
					UIMessage.Get.ShowMessage(TextConst.S(201), TextConst.S(203), OnYesSell);
				else
					ChangeMode(EAvatarMode.Normal);
			}
		}
	}

	private void OnCancelSell()
	{
		for(int i = 0; i < backpackItems.Length; i++)
			backpackItems[i].Selected = false;

		UpdateSellMoney ();
		ChangeMode (EAvatarMode.Normal);
	}

	private void OnYesSell(object obj)
	{
		List<int> sells = new List<int>();

		for(int i = 0; i < backpackItems.Length; i++)
			if(backpackItems[i].Index != -1 && backpackItems[i].Selected)
			{
				sells.Add(backpackItems[i].Index);
				backpackItems[i].Enable = false;
				backpackItems[i].Selected = false;
			}

		sells.Sort((x, y) => { return x.CompareTo(y); });

		//SendtoServer
		WWWForm form = new WWWForm();
		form.AddField("RemoveIndexs", JsonConvert.SerializeObject(sells));
		SendHttp.Get.Command(URLConst.SellItem, waitSellItem, form);
	}

	public void InitEquipState()
	{
		UnEquipAll();

		if(backpackItems.Length < 1)
			return;

		for (int i = 0; i < backpackItems.Length; i++) {
			if(backpackItems[i].Kind == avatarPart)	
			{
				if(Equips.ContainsKey(avatarPart))
				{
				   if(Equips[avatarPart].ID == backpackItems[i].ID)
				   	if(Equips[avatarPart].Index == backpackItems[i].Index)
						backpackItems[i].Equip = true;
				}
//				backpackItems[i].CheckItemUseKind();
			}
		}
	}

	private void UnEquipAll()
	{
		for(int i = 0; i < backpackItems.Length;i++)
			backpackItems[i].Equip = false;
	}

	public void OnEquip()
	{
		int index;

		if (enableCount <= 1 && avatarPart < 5 && avatarPart != 2) {
			Debug.Log("need two Item");
			return;		
		}

		if (int.TryParse (UIButton.current.name, out index)) {
			if(index < backpackItems.Length)
			{
				switch(Mode)
				{
					case EAvatarMode.Sell:
						if(backpackItems[index].Equip)
							return;
						else{
							backpackItems[index].Selected = !backpackItems[index].Selected;
							UpdateSellMoney ();
						}
						break;
					default:
						TEquip equip = new TEquip();
						int kind = GameFunction.GetItemKind(backpackItems[index].ID);
						equip.ID = backpackItems[index].ID;
						equip.Kind = kind;
						equip.Index = backpackItems[index].Index;
						
						if(!backpackItems[index].Equip){
							
							//裝備Item
							backpackItems[index].Equip = true;
							
							//卸除已裝備的Item
							for(int i = 0; i < backpackItems.Length;i++)
								if(index != i && backpackItems[i].Kind == avatarPart)
									backpackItems[i].Equip = false;
							
							if(Equips.ContainsKey(kind)){
								//檢查同部位是否有裝裝備
								if(Equips[kind].ID > 0){ 
									AddUnEquipItem(kind, Equips[kind]);
									Equips[kind] = equip;
								}else{ 
									//此部位未裝備任何裝備
									AddEquipItem(kind, equip);
									Equips[kind] = equip;
								}
							}else{
								AddEquipItem(kind, equip);
								DeleteUnEquipItem(kind, equip);
							}
							
							ItemIdTranslateAvatar();
							UIPlayerMgr.Get.ChangeAvatar(EquipsAvatar);	
						}
						else
						{
							if(kind == 2 || kind == 6 || kind == 7){	
								AddUnEquipItem(kind, equip);
								backpackItems[index].Equip = false;
								equip.ID = 0;
								Equips[kind] = equip;
								ItemIdTranslateAvatar();
								UIPlayerMgr.Get.ChangeAvatar(EquipsAvatar);	
							}
						}
						break;
				}

			}
		}
	}

	private void AddEquipItem(int kind, TEquip item)
	{
		if (Equips.ContainsKey (kind))
			Equips[kind] = item;
		else
			Equips.Add(kind, item);
	}

	private void DeleteUnEquipItem(int kind, TEquip item)
	{
		if (UnEquips.ContainsKey (kind))
			if(UnEquips[kind].ID == item.ID && UnEquips[kind].Index == item.Index)
				UnEquips.Remove(kind);
	}

	private void AddUnEquipItem(int kind, TEquip item)
	{
		if (item.Index == -1) {
			if (UnEquips.ContainsKey (kind))
				UnEquips [kind] = item;
			else
				UnEquips.Add (kind, item);
		}
	}

	private void ItemIdTranslateAvatar()
	{
		foreach (KeyValuePair<int, TEquip> item in Equips) {
			int avatarIndex;

			if(item.Value.ID > 0)
				avatarIndex = GameData.DItemData[item.Value.ID].Avatar;
			else
				avatarIndex = 0;

			switch(item.Value.Kind)
			{
				case 0:
					EquipsAvatar.Body = GameData.Team.Player.Avatar.Body;
					break;
				
				case 1:
					EquipsAvatar.Hair = avatarIndex;
					break;
					
				case 2:
					EquipsAvatar.MHandDress = avatarIndex;//手飾
					break;
					
				case 3:
					EquipsAvatar.Cloth = avatarIndex;//上身
					break;
					
				case 4:
					EquipsAvatar.Pants = avatarIndex;//下身
					break;
					
				case 5:
					EquipsAvatar.Shoes = avatarIndex;//鞋
					break;
					
				case 6:
					EquipsAvatar.AHeadDress = avatarIndex;//頭飾(共用）
					break;
					
				case 7:
					EquipsAvatar.ZBackEquip = avatarIndex;//背部(共用)
					break;
			}
		}
	}

	private void OnReturn()
	{
		UIShow (false);
        UIMainLobby.Get.Show();
		if (UISort.Visible)
			UISort.UIShow (false);
	}

	public void ChangeMode(EAvatarMode mode)
	{
		Mode = mode;
		UpdateView ();

		switch (Mode) {
			case EAvatarMode.Normal:
				SellCount.SetActive (false);
				break;
			case EAvatarMode.Sell:
				SellCount.SetActive (true);
				break;
			case EAvatarMode.Sort:
				SellCount.SetActive (false);
				break;
		}
	}

	private void OnSortMode()
	{
		if (Mode == EAvatarMode.Sell) {
			return;
		} else {
			UISort.UIShow (!UISort.Visible, 1);
			if(!UISort.Visible)
				ChangeMode (EAvatarMode.Normal);
			else
				ChangeMode (EAvatarMode.Sort);
		}
	}

	private void OnSave()
	{
		if (!CheckSameEquip ()) {
			List<int> add = new List<int>();
			List<int> move = new List<int>();

			foreach (KeyValuePair<int, TEquip> item in Equips) {
				if(item.Value.ID > 0 && item.Value.Index > 0){
					add.Add(item.Value.Index);
				}
			}

			add.Sort((x, y) => { return x.CompareTo(y); });

			//找出脫掉裝備，不穿裝備的Item
			foreach (KeyValuePair<int, TEquip> item in UnEquips) {
				if(item.Value.ID > 0 && Equips.ContainsKey(item.Value.Kind) && Equips[item.Value.Kind].ID == 0){
					move.Add(item.Value.Kind);
				}
			}

			move.Sort((x, y) => { return x.CompareTo(y); });

			WWWForm form = new WWWForm();
			form.AddField("AddIndexs", JsonConvert.SerializeObject(add));
			form.AddField("RemoveIndexs", JsonConvert.SerializeObject(move));
			SendHttp.Get.Command(URLConst.ChangeAvatar, waitEquipPlayerItem, form);
		}
	}

	private void waitSellItem(bool ok, WWW www)
	{
		if(ok)
		{
			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			GameData.Team.Items = team.Items;
			GameData.Team.Money = team.Money;
			UIMainLobby.Get.UpdateUI();
			ChangeMode(EAvatarMode.Normal);
			UpdateAvatar();
		}
	}

	private void waitEquipPlayerItem(bool ok, WWW www)
	{
		if(ok)
		{
			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			GameData.Team.Items = team.Items;
			GameData.Team.Player.Items = team.Player.Items;
			GameFunction.ItemIdTranslateAvatar(ref GameData.Team.Player.Avatar, GameData.Team.Player.Items);
			UpdateAvatar(true);
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddItem);
	}

	protected override void InitData() {
		UpdateAvatar(true);
		UIPlayerMgr.Get.ShowUIPlayer (EUIPlayerMode.UIAvatarFitted);
	}

	private void changeLayersRecursively(Transform trans, string name){
		trans.gameObject.layer = LayerMask.NameToLayer(name);
		foreach(Transform child in trans)
		{            
			changeLayersRecursively(child, name);
		}
	}

	protected override void OnShow(bool isShow) {
		if(isShow)
		{

		}
	}
}
