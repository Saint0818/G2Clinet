using System;
using System.Collections.Generic;
using GameEnum;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class TItemAvatar
{
	private GameObject self;
	private Transform DisablePool;
	private Transform EnablePool;
	private int id;
	private int usekind;
	private bool isEquip;
	private bool isInitBtn;
	private bool isRental;
	private bool isSelect;
	private EAvatarMode mode;
	private UILabel name;
	private UILabel usetime;
	private UILabel getModeLabel;
	private UILabel PriceLabel;
	private UILabel BuyInfoLabel;
	private UILabel potentialLabel;
	private UISprite pic;
	private UISprite OutLine;
	private UIButton equipBtn;
	private UIButton sellBtn;
	private UIButton buyBtn;
	private UISprite TrimBottom;
	private UISprite SellSelect;
	private UISprite EquipedIcon;
	private TimeSpan currentTime;


	public int BackageSort; //-1 : Player.Item 
	public int Position;
	public int Kind;
	public bool IsInit = false;
	public DateTime EndUseTime;
	
	public EAvatarMode Mode
	{
		set{
			mode = value;
		}
		get{return mode;}
	}

	public int UseKind
	{
		// 永久性裝備 : -1, 時效性裝備 : 1, 已過期裝備 : 2
		set{
			usekind = value;
		}
		get{return usekind;}
	}

	public bool Enable
	{
		set{
			if(self){
				self.SetActive(value);
				if(self.activeSelf)
				{
					self.transform.parent = EnablePool;
				}
				else{
					self.transform.parent = DisablePool;
					self.transform.localPosition = Vector3.zero;
				}
			}
		}
		get{
			if(self)
				return self.activeSelf;
			else
			{
				Debug.LogError("Must be Inited TItemAvatarPart.gameobject");
				return false;
			}
		}
	}

	public int SellPrice
	{
		get{
			if(GameData.DItemData.ContainsKey(id))
			{
				return GameData.DItemData[id].Sell;
			}
			else
				return 0;
		}
	}

	public int ID{
		get{return id;}	
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

	public void UpdateBtnUseState()
	{
		if(Kind > 0 && Kind < 8 && id != 0){
			// 永久性裝備 : -1, 時效性裝備 : 1, 已過期裝備 : 2
			sellBtn.gameObject.SetActive(mode == EAvatarMode.Sell);
			buyBtn.gameObject.SetActive(!sellBtn.gameObject.activeSelf && UseKind != -1);
			switch(UseKind)
			{
			case 1: 
				int compare = TimeSpan.Compare(TimeSpan.FromTicks(DateTime.UtcNow.Ticks), UseTime);
				TrimBottom.gameObject.SetActive(compare > 0);
				break;
			case 2: 
				TrimBottom.gameObject.SetActive(true);
				break;
			default : 
				TrimBottom.gameObject.SetActive(false);
				break;
			}

			getModeLabel.gameObject.SetActive(!sellBtn.gameObject.activeSelf && GameData.DItemData[id].Potential > 0);
		}
	}

	public bool Equip
	{
		set{
			isEquip = value;
			EquipedIcon.enabled = isEquip;
		}
		get{
			return isEquip;
		}
	}

	public void Init(GameObject obj, Transform enablepool, Transform disablepool, int index)
	{
		if (!IsInit && obj) {
			self = obj;
			self.name = index.ToString();
			EnablePool = enablepool;
			DisablePool = disablepool;
			self.transform.parent = enablepool;
			self.transform.localScale = Vector3.one;

			name = self.transform.FindChild ("ItemName").gameObject.GetComponent<UILabel> ();
			usetime = self.transform.FindChild ("DeadlineLabel").gameObject.GetComponent<UILabel> ();
			getModeLabel = self.transform.FindChild ("GetModeLabel").gameObject.GetComponent<UILabel> ();
			pic = self.transform.FindChild ("ItemPic").gameObject.GetComponent<UISprite> ();
			OutLine = self.transform.FindChild ("ItemPic/OutLine").gameObject.GetComponent<UISprite> ();
			TrimBottom = self.transform.FindChild ("TrimBottom").gameObject.GetComponent<UISprite> ();
			sellBtn = self.transform.FindChild ("SellBtn").gameObject.GetComponent<UIButton> ();
			SellSelect = self.transform.FindChild ("SellSelect").gameObject.GetComponent<UISprite> ();
			Selected = false;
			EquipedIcon = self.transform.FindChild ("EquipedIcon").gameObject.GetComponent<UISprite> ();
			equipBtn = self.transform.GetComponent<UIButton> ();
			buyBtn = self.transform.FindChild ("BuyBtn").gameObject.GetComponent<UIButton> ();
				
			if(buyBtn){
				buyBtn.name = self.name;
				PriceLabel = buyBtn.transform.FindChild("PriceLabel").gameObject.GetComponent<UILabel>();
				BuyInfoLabel = buyBtn.transform.FindChild("InfoLabel").gameObject.GetComponent<UILabel>();
			}

			if(equipBtn && sellBtn){
				equipBtn.name = self.name;
				sellBtn.name = self.name;
			}
		}

		IsInit = self && name && usetime && getModeLabel && pic && OutLine && TrimBottom && sellBtn && SellSelect && EquipedIcon && equipBtn && buyBtn;
		Mode = EAvatarMode.Normal;
	}

	public void InitBtttonFunction(EventDelegate BuyFunc, EventDelegate EquipFunc, EventDelegate SellFunc)
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

		if (sellBtn) {
			sellBtn.onClick.Add (SellFunc);
		}

		isInitBtn = true;
	}

	public void UpdateView(int itemid, DateTime usetime, int usekind, int backageSort)
	{
		if (itemid > 0 && GameData.DItemData.ContainsKey(itemid)) 
		{
			id = itemid;
			Position = GameData.DItemData [id].Position;
			name.text = GameData.DItemData [id].Name;
			pic.spriteName = string.Format ("Item_{0}", GameData.DItemData [id].Icon);
			OutLine.spriteName = string.Format ("Equipment_{0}", GameData.DItemData [id].Quality);
			Kind = GameFunction.GetItemKind (id);
			EndUseTime = usetime;
			UseKind = usekind;
			BackageSort = backageSort; //-1 : player.items else team.items

			PriceLabel.text = GameData.DItemData[id].Buy.ToString();
			BuyInfoLabel.text = TextConst.StringFormat(8005, GameData.DItemData[id].Potential);
			getModeLabel.text = TextConst.StringFormat(8004, GameData.DItemData[id].Potential);
		
			UpdateBtnUseState();
		} else {
			id = 0;
			Enable = false;
		}
	}

	public void UpdateTimeCD()
	{
		if(Enable){
			switch (UseKind) {
				case 1:
					TimeSpan checktime;
					checktime = EndUseTime.ToUniversalTime().Subtract(DateTime.UtcNow);
					
					if(checktime.TotalSeconds > 0)
					{
						UseTime = checktime;
					}
					else{
						UseKind = 2;
						UpdateBtnUseState();
					}
					break;
				case 2:
					usetime.gameObject.SetActive(false);
					getModeLabel.enabled = false;
					break;
				default:
					usetime.gameObject.SetActive(false);
					getModeLabel.enabled = true;
					break;
			}
		}
	}

	public Vector3 LocalPosition
	{
		set{self.transform.localPosition = value;}
	}
}

public struct TEquip
{
	
	/// <summary>
	/// The kind : 部位 ID:ItemID 
	/// Index : 陣列索引, -1: 為player.Items 
	/// SortIndex : 陣列索引, 不分team.items 或playeritems
	/// </summary>
	public int Kind;
	public int ID;
	public int BackageSort;
	public int BackageSortNoLimit;
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
	private int BuyIndex = 0;
	
	private Dictionary<int, TEquip> Equips = new Dictionary<int, TEquip>();
	private Dictionary<int, TEquip> UnEquips = new Dictionary<int, TEquip>();
	private Dictionary<int, TEquip> ExpriedChangeEquips = new Dictionary<int, TEquip>();
	private TAvatar EquipsAvatar = new TAvatar();

	private TimeSpan checktime;
	private GameObject avatar;
	private string[] btnPaths = new string[avatarPartCount];
	private GameObject[] notice = new GameObject[avatarPartCount];
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

	void FixedUpdate()
	{
		if(backpackItems != null && backpackItems.Length > 0)
			for(int i = 0; i < backpackItems.Length; i++)
				if(backpackItems[i] != null)
					backpackItems[i].UpdateTimeCD();
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

		for (int i = 0; i < btnPaths.Length; i++) {
			notice[i] = GameObject.Find(btnPaths [i] + "/New");
			SetBtnFunReName (btnPaths [i], DoAvatarTab, i.ToString ());
		}

		SetBtnFun (UIName + "/MainView/BottomLeft/BackBtn", OnReturn);
		SetBtnFun (UIName + "/MainView/BottomRight/CheckBtn", OnSave);
		SetBtnFun (UIName + "/MainView/BottomLeft/SortBtn", OnSortMode);
		SetBtnFun (UIName + "/MainView/BottomLeft/SellBtn", OnSellMode);
		SetBtnFun (UIName + "/MainView/BottomLeft/SellBtn/SellCount/CancelBtn", OnCancelSell);
		SellCount = GameObject.Find (UIName + "/MainView/BottomLeft/SellBtn/SellCount");
		TotalPriceLabel = SellCount.transform.FindChild ("TotalPrice").gameObject.GetComponent<UILabel> ();
		SellCount.SetActive (false);
	
		item = Resources.Load ("Prefab/UI/Items/ItemAvatarBtn") as GameObject;
		enablePool = GameObject.Find (UIName + "/MainView/Left/ItemList").transform;
		scrollView = GameObject.Find (UIName + "/MainView/Left/ItemList").GetComponent<UIScrollView>();

		disableGroup = new GameObject ();
		disableGroup.name = "disableGroup";
		disableGroup.transform.parent = scrollView.transform;

		InitEquips ();
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
					equip.BackageSort = -1;
					equip.BackageSortNoLimit = i + GetAvatarCountInTeamItem();
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
			GameData.SetAvatarNotice(avatarPart, 0);
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

		for(int i = 0;i< notice.Length;i++)
			notice[i].SetActive(GameData.AvatarNoticeEnable(i+1));
	}

	private void UpdateSellMoney()
	{
		int total = 0;

		for (int i = 0; i < backpackItems.Length; i++)
			if (backpackItems [i].Selected)
				total += backpackItems [i].SellPrice;

		totalPrice = total;
		TotalPriceLabel.text = string.Format("Total : {0}", totalPrice);
	}
	
	private bool CheckSameEquip()
	{
		//檢查是否有裝備背包Item
		foreach (KeyValuePair<int, TEquip> item in Equips) {
			if(item.Value.BackageSort > 0)
				return false;
		}

		//檢查是否有脫掉裝備
		foreach (KeyValuePair<int, TEquip> item in UnEquips) {
			if(item.Value.ID > 0)
				return false;
		}
		return true;
	}

	private bool CheckExpiredItem()
	{
		bool result = false;

		//檢查是否有過期裝備
		foreach (KeyValuePair<int, TEquip> item in Equips) 
		{
			if(backpackItems[item.Value.BackageSortNoLimit].UseKind == 1)
			{
//				DateTime dt = Convert.ToDateTime(backpackItems[item.Value.BackageSortNoLimit].UseTime.ToString());
//				int compare = DateTime.Compare(dt, DateTime.UtcNow);
				int compare = TimeSpan.Compare(TimeSpan.FromTicks(DateTime.UtcNow.Ticks), backpackItems[item.Value.BackageSortNoLimit].UseTime);
				if(compare < 0)
					result = true;
			}
			else if(backpackItems[item.Value.BackageSortNoLimit].UseKind == 2)
				result = true;
		}


		return result;
	}

	/// <summary>
	/// 發現過期裝備，"先重置預設裝備"，再檢查本來的裝備是否有過期，有則替換成新手裝.
	/// </summary>
	private void ChangeExpiredItem()
	{
		ExpriedChangeEquips.Clear ();

		foreach (KeyValuePair<int, TEquip> item in Equips) {
			DateTime time = Convert.ToDateTime(backpackItems[item.Value.BackageSortNoLimit].UseTime.ToString());
			if (backpackItems [item.Value.BackageSortNoLimit].UseKind == 2 || 
			    (backpackItems [item.Value.BackageSortNoLimit].UseKind == 1 && DateTime.Compare (time, DateTime.UtcNow) < 0)) 
			{
				if(item.Value.Kind == 2 || item.Value.Kind == 6 || item.Value.Kind == 7)
				{
					//飾品不需要新手裝直接脫掉
					AddUnEquipItem(item.Value.Kind, item.Value);
				}
				else
				{
					AddUnEquipItem(item.Value.Kind, item.Value);
					for(int i = 0; i < backpackItems.Length;i ++)
					{
						if(item.Value.Kind == backpackItems[i].Kind && backpackItems[i].SellPrice == 0)
						{
							TEquip equip = new TEquip();
							equip.Kind = item.Value.Kind;
							equip.ID = backpackItems[i].ID;
							equip.BackageSort = i;
							equip.BackageSortNoLimit = i;
							AddExpiredChangeItem(item.Value.Kind, equip);
						}
					}
				}
			}
		}

		foreach (KeyValuePair<int, TEquip> item in ExpriedChangeEquips) {
			AddEquipItem(item.Key, item.Value);
		}
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
				
			if(backpackItems[i] == null)
				backpackItems[i] = new TItemAvatar();

			if(!backpackItems[i].IsInit){
				backpackItems[i].Init(Instantiate(item) as GameObject, enablePool, disableGroup.gameObject.transform, i);
				backpackItems[i].InitBtttonFunction(new EventDelegate(OnBuy), new EventDelegate(OnEquip), new EventDelegate(OnSellSelect));
			}

			//InitData
			if(GetAvatarCountInTeamItem() > 0 && i < GetAvatarCountInTeamItem())
			{
				//Team.Items
				backpackItems[i].UpdateView(GameData.Team.Items[i].ID, GameData.Team.Items[i].UseTime, GameData.Team.Items[i].UseKind, i);
			}
			else if(i >= GetAvatarCountInTeamItem() && i < GetAvatarCountInPlayerItem() + GetAvatarCountInTeamItem())
			{
				//Player.Items
				int playerItemIndex = i - GetAvatarCountInTeamItem();
				if(playerItemIndex < GameData.Team.Player.Items.Length){
					backpackItems[i].UpdateView(GameData.Team.Player.Items[playerItemIndex].ID, GameData.Team.Player.Items[playerItemIndex].UseTime, GameData.Team.Player.Items[playerItemIndex].UseKind, -1);
				}
			}
			else
			{
				backpackItems[i].UpdateView(0, DateTime.UtcNow, -1, -1);
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
			if(backpackItems[i].Kind == avatarPart)
			{
				backpackItems[i].Mode = Mode;
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
				backpackItems[i].Enable = false;
			}

			if(Mode == EAvatarMode.Sell && (backpackItems[i].Equip || backpackItems[i].SellPrice == 0))
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
					sortlist[i].LocalPosition = GetItemPos(count);
					count++;
				}
			break;
			case 1:
				sortlist.Sort((x, y) => { return x.EndUseTime.CompareTo(y.EndUseTime); });
				for(int i = 0; i< sortlist.Count;i++){
					sortlist[i].LocalPosition = GetItemPos(count);;
					count++;
				}
				break;
			default:
				for(int i = 0; i < backpackItems.Length; i++)
					if(backpackItems[i].Enable)
					{
						backpackItems[i].LocalPosition = GetItemPos(count);;
						count++;
					}
						break;
		}

		sortlist.Clear ();
		enablePool.gameObject.SetActive (false);
		enablePool.gameObject.SetActive (true);
		scrollView.ResetPosition ();
		scrollView.enabled = false;
		scrollView.enabled = true;
	}
	
	public void OnBuy()
	{
		int index;
		BuyIndex = -1;
		if (int.TryParse (UIButton.current.name, out index)) {
			BuyIndex = index;
			string ask = string.Format(TextConst.S(208), GameData.DItemData[backpackItems[index].ID].Buy,  GameData.DItemData[backpackItems[index].ID].Name);
			UIMessage.Get.ShowMessage(TextConst.S(201), ask, OnYesBuy);
		}
	}

	public void OnYesBuy(object obj)
	{
		if (BuyIndex != -1) {
			int from = 0;//0:team.items -1:player.Items
			int buyIndex = 0;
			if(backpackItems[BuyIndex].BackageSort == -1)
			{
				from = -1;
				buyIndex = avatarPart;
				
			}else
			{
				from = 0;
				buyIndex = BuyIndex;
			}
			
			WWWForm form = new WWWForm();
			form.AddField("From", from);
			form.AddField("Index", buyIndex);
			SendHttp.Get.Command(URLConst.BuyAvatarItem, waitBuyItem, form);
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
			if(backpackItems[i].BackageSort != -1 && backpackItems[i].Selected)
			{
				sells.Add(backpackItems[i].BackageSort);
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
				if(Equips.ContainsKey(avatarPart) && Equips[avatarPart].ID == backpackItems[i].ID && Equips[avatarPart].BackageSort == backpackItems[i].BackageSort)
				{
					backpackItems[i].Equip = true;
				}
			}
		}
	}

	private void UnEquipAll()
	{
		for(int i = 0; i < backpackItems.Length;i++)
			backpackItems[i].Equip = false;
	}

	public void OnSellSelect()
	{
		if (Mode == EAvatarMode.Sell){
			int index;
			if (int.TryParse (UIButton.current.name, out index)) {
				if(index < backpackItems.Length){
					if(backpackItems[index].Equip)
						return;
					else{
						backpackItems[index].Selected = !backpackItems[index].Selected;
						UpdateSellMoney ();
					}
				}
			}
		}
	}

	public void OnEquip()
	{
		if (Mode == EAvatarMode.Sell)
			return;

		int index;

		if (enableCount <= 1 && avatarPart < 5 && avatarPart != 2) {
			Debug.Log("need two Item");
			return;		
		}

		if (int.TryParse (UIButton.current.name, out index)) {
			if(index < backpackItems.Length)
			{
				TEquip equip = new TEquip();
				int kind = GameFunction.GetItemKind(backpackItems[index].ID);
				equip.ID = backpackItems[index].ID;
				equip.Kind = kind;
				equip.BackageSort = backpackItems[index].BackageSort;
				equip.BackageSortNoLimit = index;
				
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
			}
		}
	}

	private void AddExpiredChangeItem(int kind, TEquip item)
	{
		if (ExpriedChangeEquips.ContainsKey (kind))
			ExpriedChangeEquips[kind] = item;
		else
			ExpriedChangeEquips.Add(kind, item);
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
			if(UnEquips[kind].ID == item.ID && UnEquips[kind].BackageSort == item.BackageSort)
				UnEquips.Remove(kind);
	}

	private void AddUnEquipItem(int kind, TEquip item)
	{
		if (item.BackageSort == -1) {
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
		if (CheckExpiredItem ()) {
			//發現過期裝備，先重置預設裝備
			UpdateAvatar (true);
			ChangeExpiredItem();
			ItemIdTranslateAvatar();
			UIPlayerMgr.Get.ChangeAvatar(EquipsAvatar);	
		}

		if (!CheckSameEquip ()) {
			List<int> add = new List<int>();
			List<int> move = new List<int>();

			foreach (KeyValuePair<int, TEquip> item in Equips) {
				if(item.Value.ID > 0 && item.Value.BackageSort > 0){
					add.Add(item.Value.BackageSort);
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
		else
		{
			UpdateAvatar(true);
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

	private void waitBuyItem(bool ok, WWW www)
	{
		if(ok)
		{
			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			GameData.Team.Items = team.Items;
			GameData.Team.Player.Items = team.Player.Items;
			GameData.Team.Diamond = team.Diamond;
			GameData.Team.AvatarPotential = team.AvatarPotential;
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
		UIPlayerMgr.Get.ShowUIPlayer (EUIPlayerMode.UIAvatarFitted, ref GameData.Team);
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
