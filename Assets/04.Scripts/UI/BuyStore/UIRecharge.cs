using UnityEngine;
using System.Collections;
using GameStruct;

public enum ERechargeType {
	Diamond = 0,
	Coin = 1,
	Power = 2
}

public struct TItemRecharge {
	public GameObject mSelf;

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

	public void UpdateView (int index, TShop shop) {
		mSelf.name = shop.Order.ToString();
		PriceButton.name = shop.Order.ToString();
		mSelf.transform.localPosition = new Vector3(-250 + index * 250, 0, 0);
		PriceLabel.text = shop.Price;
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

	public  void UpdateViewForMall (int index, TMall mall) {
		PriceButton.name = mall.Order.ToString();
		mSelf.transform.localPosition = new Vector3(-250 + index * 250, 0, 0);
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

	private string getSaleText (int sale) {
		switch (sale) {
		case 0:
			return TextConst.S(4200);
		case 1:
			return TextConst.S(4201);
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
		int index = 0;
		for(int i=0; i<GameData.DShops.Length; i++) {
			if(GameData.DShops[i].Kind == 0) {
				if(GameData.DShops[i].Order >= BuyCoinLen) {
					Debug.LogError("Order is Wrong. Kind = " + GameData.DShops[i].Kind );
					break;
				}
				kindBuyCoin[GameData.DShops[i].Order].init(Instantiate(prefabKind[1]), scrollviews[1]);
				kindBuyCoin[GameData.DShops[i].Order].UpdateView(GameData.DShops[i].Order ,GameData.DShops[i]);
				if(GameData.DShops[i].SpendKind == 0)
					kindBuyCoin[GameData.DShops[i].Order].UpdateBtn(new EventDelegate(OnSpendGem));
				else if(GameData.DShops[i].SpendKind == 1) 
					kindBuyCoin[GameData.DShops[i].Order].UpdateBtn(new EventDelegate(OnSpendCoin));
			} else if(GameData.DShops[i].Kind == 1) {
				if(GameData.DShops[i].Order >= BuyStaminaLen) {
					Debug.LogError("Order is Wrong. Kind = " + GameData.DShops[i].Kind );
					break;
				}
				kindBuyStamina[GameData.DShops[i].Order].init(Instantiate(prefabKind[1]), scrollviews[2]);
				kindBuyStamina[GameData.DShops[i].Order].UpdateView(GameData.DShops[i].Order ,GameData.DShops[i]);
				if(GameData.DShops[i].SpendKind == 0)
					kindBuyStamina[GameData.DShops[i].Order].UpdateBtn(new EventDelegate(OnSpendGem));
				else if(GameData.DShops[i].SpendKind == 1) 
					kindBuyStamina[GameData.DShops[i].Order].UpdateBtn(new EventDelegate(OnSpendCoin));
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
			kindBuyDiamond[GameData.DMalls[i].Order].UpdateViewForMall(GameData.DMalls[i].Order, GameData.DMalls[i]);
			kindBuyDiamond[GameData.DMalls[i].Order].UpdateBtn(new EventDelegate(OnSpendRealMoney));
		}
	}

	public void OnSpendRealMoney () {
		int result = -1;
		if(int.TryParse(UIButton.current.name, out result)) {
			Debug.Log("Kind: 0 ");
			Debug.Log("OnSpendRealMoney Order:"+ result);
		}
	}

	public void OnSpendGem () {
		int result = -1;
		if(int.TryParse(UIButton.current.name, out result)) {
			Debug.Log("Kind: 1 ");
			Debug.Log("OnSpendGem Order:"+ result);
		}
	}

	public void OnSpendCoin () {
		int result = -1;
		if(int.TryParse(UIButton.current.name, out result)) {
			Debug.Log("Kind: 2 ");
			Debug.Log("OnSpendCoin Order:"+ result);
		}
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
}
