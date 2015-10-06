using UnityEngine;
using System.Collections;
using GameStruct;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public struct TItemAvatar
{
	public int Index; //-1 : Player.Item 
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
			CheckEquipBtnName();

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
					CheckEquipBtnName();
					isReantimeEnd = true;
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
			if (equipBtn) {
				equipBtn.defaultColor = (isEquip == true) ? Color.gray : new Color(0.431f, 0.976f, 0.843f,1);
				equipBtn.hover = (isEquip == true) ? Color.gray : new Color(0.431f, 0.976f, 0.843f,1);
				CheckEquipBtnName();
			}
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

				CheckItemKind();
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
	private TItemAvatar[] backpackItems;
	private TItemAvatar[] playerItems;
	private bool isInit = false;
	private UIGrid grid;
	private UIScrollView scrollView;
	private GameObject disableGroup;
	private int enableCount = 0;
	
	private Dictionary<int, TEquip> Equips = new Dictionary<int, TEquip>();
	private int [] AvatarKind = new int [7]{1,2,3,4,5,10,11};
	private TAvatar EquipsAvatar = new TAvatar();

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
		if(backpackItems != null)
			for(int i = 0; i < backpackItems.Length; i++){
				if(backpackItems[i].Enable){
					checktime = backpackItems[i].EndUseTime - System.DateTime.UtcNow;
					backpackItems[i].UseTime = checktime;
				}
			}
	}

	private string[] btnPaths = new string[avatarPartCount];

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
			for(int i = 0; i < GameData.Team.Player.Items.Length;i++){
				int kind = GetItemKind(GameData.Team.Player.Items[i].ID);

				if(kind > 0 && kind < 8 && !Equips.ContainsKey(GameData.Team.Player.Items[i].ID)){
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

	private bool IsAvatarKind(int kind)
	{
		for(int i = 0; i < AvatarKind.Length;i++)
			if(AvatarKind[i] == kind)
				return true;

		return false;

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
//		int result = 0;
//		if(GameData.Team.Player.Items.Length > 0)
//			for(int i = 0; i < GameData.Team.Player.Items.Length;i++)
//				if(GameData.Team.Player.Items[i].ID > 0)
//					result++;
//
//		return result;

		return GameData.Team.Player.Items.Length;
	}

	private bool CheckItemCount()
	{
		int all =  GetAvatarCountInTeamItem() + GetAvatarCountInPlayerItem();

		if (backpackItems == null) {
			backpackItems = new TItemAvatar[all];		
			return true;
		}
		else
		{
			if(all == backpackItems.Length)
				return true;
			else if(all > backpackItems.Length){
				Array.Resize(ref backpackItems, GameData.Team.Items.Length);
				return true;
			}
		}

		return false;
	}

	private void InitItems()
	{
		enableCount = 0;

		if (GameData.Team.Items != null) {
			for(int i = 0; i < backpackItems.Length; i++){
				if(backpackItems[i].gameobject == null){
					backpackItems[i].gameobject = Instantiate(item) as GameObject;
					backpackItems[i].gameobject.transform.parent = grid.transform;
					backpackItems[i].gameobject.transform.localScale = Vector3.one;
					backpackItems[i].gameobject.name = i.ToString();
					backpackItems[i].DisablePool = disableGroup.gameObject.transform;
					backpackItems[i].EnablePool = grid.gameObject.transform;
					backpackItems[i].Init();
					backpackItems[i].InitBtttonFunction(new EventDelegate(OnBuy), new EventDelegate(OnEquip));
				}

				if(i < GameData.Team.Items.Length)
				{
					//Team.Items
					if(backpackItems[i].ID != GameData.Team.Items[i].ID)
					{
						backpackItems[i].ID = GameData.Team.Items[i].ID;
						backpackItems[i].Position = GameData.DItemData[backpackItems[i].ID].Position;
						backpackItems[i].EndUseTime = GameData.Team.Items[i].UseTime;
						backpackItems[i].Name =  GameData.DItemData[backpackItems[i].ID].Name;
						backpackItems[i].Pic = GameData.DItemData[backpackItems[i].ID].Icon;
						backpackItems[i].AbilityKind = GetItemKind(backpackItems[i].ID).ToString();
						backpackItems[i].Index = i;
					}
				}
				else
				{
					int playerItemIndex = i - GameData.Team.Items.Length;
					if(playerItemIndex < GameData.Team.Player.Items.Length){
						if(backpackItems[i].ID !=  GameData.Team.Player.Items[playerItemIndex].ID)
						{
							backpackItems[i].ID = GameData.Team.Player.Items[playerItemIndex].ID;
							backpackItems[i].Position = GameData.DItemData[backpackItems[i].ID].Position;
							backpackItems[i].EndUseTime = GameData.Team.Player.Items[playerItemIndex].UseTime;
							backpackItems[i].Name =  GameData.DItemData[backpackItems[i].ID].Name;
							backpackItems[i].Pic = GameData.DItemData[backpackItems[i].ID].Icon;
							backpackItems[i].AbilityKind = GetItemKind(backpackItems[i].ID).ToString();
							backpackItems[i].Index = -1;
						}
					}
				}

				Debug.Log("backpackItems[i].ID :" + backpackItems[i].ID);
				//ItemVisable
				int kind = GetItemKind(backpackItems[i].ID);

				if(GameData.DItemData.ContainsKey(backpackItems[i].ID) && kind == avatarPart)
				{
					#if UIAvatarFitted_ShowAll
					items[i].Enable = true;
					#else
					if(kind < 10)
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
				}
				else
				{
					backpackItems[i].gameobject.transform.localPosition = Vector3.zero;
					backpackItems[i].Enable = false;
				}
					
			}

			grid.Reposition ();
			grid.gameObject.SetActive (false);
			grid.gameObject.SetActive (true);
			scrollView.ResetPosition ();
			scrollView.enabled = false;
			scrollView.enabled = true;
		}

		Debug.Log ("enableCount : " + enableCount);
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
					for(int y = 0; y < backpackItems.Length; y++)
					{
						if(GameData.Team.Player.Items[i].ID == backpackItems[y].ID)
							backpackItems[y].Equip = true;
					}
				}
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

		if (enableCount <= 1 && avatarPart < 10) {
			Debug.Log("need two Item");
			return;		
		}

		if (int.TryParse (UIButton.current.name, out index)) {
			if(index < backpackItems.Length)
			{
				if(!backpackItems[index].Equip){
					backpackItems[index].Equip = true;

					if(!Equips.ContainsKey(backpackItems[index].ID))
					{
						TEquip equip = new TEquip();
						equip.ID = backpackItems[index].ID;
						equip.Kind = GetItemKind(backpackItems[index].ID);
						Equips.Add(equip.ID, equip);
						ItemIdTranslateAvatar(equip.ID);
						ModelManager.Get.SetAvatarTexture(avatar, EquipsAvatar, GameData.Team.Player.BodyType, avatarPart,GameData.DItemData[backpackItems[index].ID].Avatar);
					}

					for(int i = 0; i < backpackItems.Length;i++)
					{
						//找出已裝備的Item
						if(backpackItems[index].Enable && i != index)
						{
							backpackItems[i].Equip = false;
							if(Equips.ContainsKey(backpackItems[i].ID))
								Equips.Remove(backpackItems[i].ID);
						}
					}
				}
				else
				{
					backpackItems[index].Equip = false;
				}
			}
		}
			Debug.Log ("Equip id : " + index);
	}

	private int GetTextureIndex(int avatarindex)
	{
		return avatarindex % 1000;
	}

	private void ItemIdTranslateAvatar(int id)
	{
		switch(avatarPart)
		{
			case 1:
				EquipsAvatar.Hair = id;
				break;

			case 2:
				EquipsAvatar.MHandDress = id;//手飾
				break;
				
			case 3:
				EquipsAvatar.Cloth = id;//上身
				break;
				
			case 4:
				EquipsAvatar.Pants = id;//下身
				break;
				
			case 5:
				EquipsAvatar.Shoes = id;//鞋
				break;
				
			case 10:
				EquipsAvatar.AHeadDress = id;//頭飾(共用）
				break;
				
			case 11:
				EquipsAvatar.ZBackEquip = id;//背部(共用)
				break;
		}
		
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
			WWWForm form = new WWWForm();
			form.AddField("Indexs", JsonConvert.SerializeObject(EquipsAvatar));
			SendHttp.Get.Command(URLConst.EquipPlayerItem, waitEquipPlayerItem, form);
			Debug.LogWarning("save serverdata");
		}
	}

	private void waitEquipPlayerItem(bool ok, WWW www)
	{
		if(ok)
		{
			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			GameData.Team.Items = team.Items;

			if(team.Items.Length > 0)
				for(int i = 0; i < team.Items.Length; i++)
					if(GameData.DItemData.ContainsKey(team.Items[i].ID))
						Debug.Log("item : " + GameData.DItemData[team.Items[i].ID].Name);
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddItem);
	}
	
	private GameObject avatar;

	protected override void InitData() {
		InitAvatarView (0);
		avatar = new GameObject ();
		avatar.name = "UIPlayer";
		avatar.transform.parent = gameObject.transform;
		avatar.transform.localScale = Vector3.one * 500;
		avatar.transform.localPosition = new Vector3 (-1160, -633, -2000);

		ModelManager.Get.SetAvatar(ref avatar, GameData.Team.Player.Avatar, GameData.Team.Player.BodyType, EAnimatorType.AvatarControl, false);
		changeLayersRecursively (avatar.transform, "UIPlayer");
	}

	private void changeLayersRecursively(Transform trans, string name){
		trans.gameObject.layer = LayerMask.NameToLayer(name);
		foreach(Transform child in trans)
		{            
			changeLayersRecursively(child, name);
		}
	}

	protected override void OnShow(bool isShow) {
		
	}
}
