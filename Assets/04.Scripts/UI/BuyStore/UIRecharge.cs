using UnityEngine;
using System.Collections;
using GameStruct;
using Newtonsoft.Json;

public enum ERechargeType {
	Diamond = 0,
	Coin = 1,
	Power = 2
}

public struct TBuyDiamond {
	public int Diamond;
} 

public struct TBuyFromShop {
	public int Diamond;
	public int Money;
	public int Power;
}

public struct TItemRecharge {
	public GameObject mSelf;
	public int mIndex;
	public TShop mShop;

	public UIButton PriceButton;
	public UILabel PriceLabel;
	public UISprite PriceIcon;
	public UISprite ItemIcon;
	public UILabel ItemNameLabel;
	public UILabel SaleLabel;
	public UILabel ValueLabel;
	public UISprite ValueIcon;

	public void init (GameObject obj, GameObject parent) {
		mSelf = obj;
		obj.transform.parent = parent.transform;
		obj.transform.localScale = Vector3.one;
		PriceButton = obj.transform.FindChild("BuyBtn").GetComponent<UIButton>();
		PriceLabel = obj.transform.FindChild("BuyBtn/PriceLabel").GetComponent<UILabel>();
		if(obj.transform.FindChild("BuyBtn/Icon") != null)
			PriceIcon = obj.transform.FindChild("BuyBtn/Icon").GetComponent<UISprite>();
		ItemIcon = obj.transform.FindChild("ItemIcon").GetComponent<UISprite>();
		ItemNameLabel = obj.transform.FindChild("ItemNameLabel").GetComponent<UILabel>();
		SaleLabel = obj.transform.FindChild("SaleLabel").GetComponent<UILabel>();
		ValueLabel = obj.transform.FindChild("GetLabel").GetComponent<UILabel>();
		ValueIcon = obj.transform.FindChild("GetLabel/Icon").GetComponent<UISprite>();

		if(PriceLabel == null || ItemIcon == null || ItemNameLabel == null || 
			SaleLabel == null || ValueLabel == null || ValueIcon == null)
			Debug.LogError("TItemRecharge Init Fail.");

	}

	public void UpdateBtn (EventDelegate btn) {
		PriceButton.onClick.Add(btn);
	}

	public void UpdateView (int index, int order, TShop shop) {
		mIndex = index;
		mSelf.name = shop.Order.ToString();
		mShop = shop;
		PriceButton.name = shop.Order.ToString();
		mSelf.transform.localPosition = new Vector3(-250 + order * 250, 0, 0);
		PriceLabel.text = shop.Price.ToString();
		if(PriceIcon != null)
			PriceIcon.spriteName = iconName(shop.SpendKind, shop.Pic);

		if(shop.Sale > 0)
			SaleLabel.text = getSaleText(shop.Sale);
		else
			SaleLabel.gameObject.SetActive(false);
		
		if(shop.ItemID > 0 && GameData.DItemData.ContainsKey(shop.ItemID)) {
			ItemIcon.spriteName = itemiconName(GameData.DItemData[shop.ItemID].Kind);
			ItemNameLabel.text = GameData.DItemData[shop.ItemID].Name;
			ValueLabel.text =  GameData.DItemData[shop.ItemID].Value.ToString();
			ValueIcon.spriteName = itemiconName(GameData.DItemData[shop.ItemID].Kind);
		}
	}

	public  void UpdateViewForMall (int index, int order, TMall mall) {
		mIndex = index;
		PriceButton.name = mall.Order.ToString();
		mSelf.transform.localPosition = new Vector3(-250 + order * 250, 0, 0);
		PriceLabel.text = mall.Price;

		if(mall.Sale > 0)
			SaleLabel.text = getSaleText(mall.Sale);
		else
			SaleLabel.gameObject.SetActive(false);
		
		ItemIcon.spriteName = "MallGem1";
		ItemNameLabel.text = mall.Diamonds.ToString();
		ValueLabel.text = mall.Price;
		ValueIcon.gameObject.SetActive(false);
	}

	//31.錢
	//32.鑽石
	//34.體力
	private string itemiconName (int kind) {
		switch(kind) {
		case 31:
			return "MallCoin1";
		case 32:
			return "MallGem1";
		case 34:
			return "MallStamina1";
			
		}
		return "";
	}
	//1.4200 特賣
	//2.4201 熱門
	//3.4202 限時
	private string getSaleText (int sale) {
		switch (sale) {
		case 1:
			return TextConst.S(4200);
		case 2:
			return TextConst.S(4201);
		case 3:
			return TextConst.S(4202);
		}
		return "";
	}

	/*
	0.鑽石
	1.遊戲幣
	2.聯盟幣
	3.社群幣
	*/
	private string iconName (int kind, int pic) {
		switch(kind) {
		case 0:
			return "MallGem"+pic;
		case 1:
			return "MallCoin"+pic;
		}
		return "";
	}
}

public class UIRecharge : UIBase {
	private static UIRecharge instance = null;
	private const string UIName = "UIRecharge";

	private GameObject[] prefabKind = new GameObject[3];

	private GameObject[] tabSelects = new GameObject[3];
	private GameObject[] tabGos = new GameObject[3];
	private GameObject[] pages = new GameObject[3];
	private GameObject[] scrollviews = new GameObject[3];

	private TItemRecharge[] kindBuyCoin;
	private TItemRecharge[] kindBuyStamina;

	private TItemRecharge[] kindBuyDiamond;

	private bool isInit = false;
//	private int showText = 0;
	private int buyIndex = -1;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static void UIShow(bool isShow){
		if (instance)
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		else
			if (isShow) 
				Get.Show(isShow);
	}

	public static UIRecharge Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIRecharge;

			return instance;
		}
	}

	protected override void InitCom() {
		isInit = false;
		prefabKind[0] = Resources.Load(UIPrefabPath.ItemRechargeGems) as GameObject;
		prefabKind[1] = Resources.Load(UIPrefabPath.ItemRechargeMoney) as GameObject;
		prefabKind[2] = Resources.Load(UIPrefabPath.ItemRechargeStamina) as GameObject;
		for(int i=0; i<3; i++) {
			tabGos[i] = GameObject.Find(UIName + "/Center/Window/Tabs/" + i.ToString());
			tabSelects[i] = tabGos[i].transform.FindChild("Selected").gameObject;
			UIEventListener.Get(tabGos[i]).onClick = OnClickTab;
			pages[i] = GameObject.Find(UIName + "/Center/Window/Pages/" + i.ToString());
			scrollviews[i] = pages[i].transform.FindChild("Vertical/ScrollView").gameObject;
		}

		SetBtnFun(UIName + "/Center/Window/NoBtn", OnClose);
	}

	public void Show (int type) {
		UIShow(true);
		showTab(type);
		if(!isInit) {
			initMall ();
			initScroll();
		}
	}

	private void initScroll () {
		isInit = true;
		int BuyCoinLen = 0;
		int BuyStaminaLen = 0;
		for(int i=0; i<GameData.DShops.Length; i++) {
			if(GameData.DShops[i].Kind == 0) 
				BuyCoinLen ++;
			else if(GameData.DShops[i].Kind == 1)
				BuyStaminaLen ++;
		}

		kindBuyCoin = new TItemRecharge[BuyCoinLen];
		kindBuyStamina = new TItemRecharge[BuyStaminaLen];
		for(int i=0; i<GameData.DShops.Length; i++) {
			if(GameData.DShops[i].Kind == 0) {
				if(GameData.DShops[i].Order >= BuyCoinLen) {
					Debug.LogError("Order is Wrong. Kind = " + GameData.DShops[i].Kind );
					break;
				}
				kindBuyCoin[GameData.DShops[i].Order].init(Instantiate(prefabKind[1]), scrollviews[1]);
				kindBuyCoin[GameData.DShops[i].Order].UpdateView(i, GameData.DShops[i].Order ,GameData.DShops[i]);
				if(GameData.DShops[i].Kind == 0)
					kindBuyCoin[GameData.DShops[i].Order].UpdateBtn(new EventDelegate(OnBuyCoin));
				else if(GameData.DShops[i].Kind == 1) 
					kindBuyCoin[GameData.DShops[i].Order].UpdateBtn(new EventDelegate(OnBuyPower));
			} else if(GameData.DShops[i].Kind == 1) {
				if(GameData.DShops[i].Order >= BuyStaminaLen) {
					Debug.LogError("Order is Wrong. Kind = " + GameData.DShops[i].Kind );
					break;
				}
				kindBuyStamina[GameData.DShops[i].Order].init(Instantiate(prefabKind[2]), scrollviews[2]);
				kindBuyStamina[GameData.DShops[i].Order].UpdateView(i, GameData.DShops[i].Order ,GameData.DShops[i]);
				if(GameData.DShops[i].Kind == 0)
					kindBuyStamina[GameData.DShops[i].Order].UpdateBtn(new EventDelegate(OnBuyCoin));
				else if(GameData.DShops[i].Kind == 1) 
					kindBuyStamina[GameData.DShops[i].Order].UpdateBtn(new EventDelegate(OnBuyPower));
			}

		}
	}

	private void initMall () {
		kindBuyDiamond = new TItemRecharge[GameData.DMalls.Length];
		for (int i=0; i<GameData.DMalls.Length; i++) {
			if(GameData.DMalls[i].Order >=GameData.DMalls.Length) {
				Debug.LogError("Order is Wrong. Order = " + GameData.DMalls[i].Order );
				break;
			}
			kindBuyDiamond[GameData.DMalls[i].Order].init(Instantiate(prefabKind[0]), scrollviews[0]);
			kindBuyDiamond[GameData.DMalls[i].Order].UpdateViewForMall(i, GameData.DMalls[i].Order, GameData.DMalls[i]);
			kindBuyDiamond[GameData.DMalls[i].Order].UpdateBtn(new EventDelegate(OnBuyDiamond));
		}
	}

	public void OnBuyDiamond () {
		int result = -1;
		if(int.TryParse(UIButton.current.name, out result)) {
			if(result >= 0 && result < kindBuyDiamond.Length && result < GameData.DMalls.Length) {
				SendBuyDiamond(kindBuyDiamond[result].mIndex, GameData.DMalls[kindBuyDiamond[result].mIndex].Android);
			}
		}
	}

	public void OnBuyCoin () {
		int result = -1;
		if(int.TryParse(UIButton.current.name, out result)) {
			if(result >= 0 && result < kindBuyCoin.Length && GameData.DItemData.ContainsKey(kindBuyCoin[result].mShop.ItemID)) {
//				showText = 535;
				buyIndex = kindBuyCoin[result].mIndex;
				if(kindBuyCoin[result].mShop.SpendKind == 0 ) {
					CheckDiamond(kindBuyCoin[result].mShop.Price, true, string.Format(TextConst.S(250), kindBuyCoin[result].mShop.Price, GameData.DItemData[kindBuyCoin[result].mShop.ItemID].Name), ConfirmBuy);
				}else {
					CheckDiamond(kindBuyCoin[result].mShop.Price, true, string.Format(TextConst.S(251), kindBuyCoin[result].mShop.Price, GameData.DItemData[kindBuyCoin[result].mShop.ItemID].Name), ConfirmBuy);
				}
			}
		}
	}

	public void OnBuyPower () {
		int result = -1;
		if(int.TryParse(UIButton.current.name, out result)) {
			if(result >= 0 && result < kindBuyStamina.Length) {
				if(GameData.Team.Power < GameConst.Max_Power){
//					showText = 537;
					buyIndex = kindBuyStamina[result].mIndex;
					if(kindBuyStamina[result].mShop.SpendKind == 0) {
						CheckMoney(kindBuyStamina[result].mShop.Price, true, string.Format(TextConst.S(250), kindBuyStamina[result].mShop.Price, GameData.DItemData[kindBuyStamina[result].mShop.ItemID].Name), ConfirmBuy);
					} else if(kindBuyStamina[result].mShop.SpendKind == 1) {
						CheckMoney(kindBuyStamina[result].mShop.Price, true, string.Format(TextConst.S(251), kindBuyStamina[result].mShop.Price, GameData.DItemData[kindBuyStamina[result].mShop.ItemID].Name), ConfirmBuy);
					}

				} else
					UIHint.Get.ShowHint(TextConst.S(536), Color.blue);
			}
		}
	}

	public void ConfirmBuy () {
		SendBuyFromShop(buyIndex);
	}


	public void OnClickTab (GameObject go) {
		int result = -1;
		if(int.TryParse(go.name, out result)) {
			showTab(result);
		}
	}

	private void showTab (int type) {
		for (int i=0; i<tabSelects.Length; i++) {
			tabSelects[i].SetActive((type == i));
			pages[i].SetActive((type == i));
		}
	}

	public void OnClose () {
		UIShow(false);
	}

	private void SendBuyDiamond(int index, string receipt)
	{
		if(index >=0 && !string.IsNullOrEmpty(receipt) && !string.IsNullOrEmpty(GameData.Team.Identifier)) {
			WWWForm form = new WWWForm();
			form.AddField("Identifier", GameData.Team.Identifier);
			form.AddField("id", index);
			form.AddField("Receipt", receipt);
			SendHttp.Get.Command(URLConst.BuyDiamond, waitBuyDiamond, form);
		}
	}

	private void waitBuyDiamond(bool ok, WWW www)
	{
		if(ok)
		{
			TBuyDiamond result = (TBuyDiamond)JsonConvert.DeserializeObject(www.text, typeof(TBuyDiamond));
			GameData.Team.Diamond = result.Diamond;

			UIMainLobby.Get.UpdateUI();
//			UIHint.Get.ShowHint(TextConst.S(535), Color.blue);
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.BuyDiamond);
	}

	private void SendBuyFromShop(int index)
	{
		if(index >=0 && !string.IsNullOrEmpty(GameData.Team.Identifier)) {
			WWWForm form = new WWWForm();
			form.AddField("Identifier", GameData.Team.Identifier);
			form.AddField("Index", index);
			SendHttp.Get.Command(URLConst.BuyFromShop, waitBuyFromShop, form);
		}
	}

	private void waitBuyFromShop(bool ok, WWW www)
	{
		if(ok)
		{
			TBuyFromShop result = (TBuyFromShop)JsonConvert.DeserializeObject(www.text, typeof(TBuyFromShop));
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.Money = result.Money;
			GameData.Team.Power = result.Power;

			UIMainLobby.Get.UpdateUI();
//			UIHint.Get.ShowHint(TextConst.S(showText), Color.blue);
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.BuyDiamond);
	}
}
