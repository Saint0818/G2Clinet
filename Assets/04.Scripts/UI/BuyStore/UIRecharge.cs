﻿using UnityEngine;
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
	public TDailyCount DailyCount;
	public TTeamRecord LifetimeRecord;
}

public struct TItemRecharge {
	public GameObject mSelf;
	public int mIndex;
	public TShop mShop;
	public int mPrice;

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
		PriceButton = obj.transform.Find("BuyBtn").GetComponent<UIButton>();
		PriceLabel = obj.transform.Find("BuyBtn/PriceLabel").GetComponent<UILabel>();
		if(obj.transform.Find("BuyBtn/Icon") != null)
			PriceIcon = obj.transform.Find("BuyBtn/Icon").GetComponent<UISprite>();
		ItemIcon = obj.transform.Find("ItemIcon").GetComponent<UISprite>();
		ItemNameLabel = obj.transform.Find("ItemNameLabel").GetComponent<UILabel>();
		SaleLabel = obj.transform.Find("SaleLabel").GetComponent<UILabel>();
		ValueLabel = obj.transform.Find("GetLabel").GetComponent<UILabel>();
		ValueIcon = obj.transform.Find("GetLabel/Icon").GetComponent<UISprite>();

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
		mSelf.transform.localPosition = new Vector3(-400 + order * 275, 0, 0);

		if(PriceIcon != null)
			PriceIcon.spriteName = GameFunction.SpendKindTexture(shop.SpendKind);

		if(mShop.Sale > 0)
			SaleLabel.text = getSaleText(mShop.Sale);
		else
			SaleLabel.gameObject.SetActive(false);

		
		if(shop.ItemID > 0 && GameData.DItemData.ContainsKey(shop.ItemID)) {
			if(GameData.DItemData[shop.ItemID].Kind == 31)
				ItemIcon.spriteName = "MallCoin" + shop.Pic.ToString();
			else if(GameData.DItemData[shop.ItemID].Kind == 34)
				ItemIcon.spriteName = "MallStamina" + shop.Pic.ToString();
			ItemNameLabel.text = GameData.DItemData[shop.ItemID].Name;
			ValueLabel.text =  NumFormater.Convert(GameData.DItemData[shop.ItemID].Value);
			ValueIcon.spriteName = itemKindIconValueName(GameData.DItemData[shop.ItemID].Kind);
		}
		RefreshPrice();
	}

	public void RefreshTextColor () {
		PriceLabel.color = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(0, mPrice));
	}

	/// <summary>
	/// 主要重整有限制次數的UI
	/// </summary>
	public void RefreshPrice() {
		if(mShop.Limit != null) {
			if(mShop.Order == 0 && GameData.Team.DailyCount.BuyPowerOne >=0) { 
				if(GameData.Team.DailyCount.BuyPowerOne >= mShop.Limit.Length) {
					mPrice = mShop.Limit[mShop.Limit.Length - 1];
					showSoldOut ();
				} else
					mPrice = mShop.Limit[GameData.Team.DailyCount.BuyPowerOne];
			} else if (mShop.Order == 1 && GameData.Team.DailyCount.BuyPowerTwo >=0) {
				if(GameData.Team.DailyCount.BuyPowerTwo >= mShop.Limit.Length) {
					mPrice =  mShop.Limit[mShop.Limit.Length - 1];
					showSoldOut ();
				} else 
					mPrice =  mShop.Limit[GameData.Team.DailyCount.BuyPowerTwo];
			}
		} else 
			mPrice = mShop.Price;

		PriceLabel.text =  NumFormater.Convert(mPrice);
	}

	private void showSoldOut () {
		SaleLabel.gameObject.SetActive(true);
		SaleLabel.text = getSaleText(4);
	}

	public  void UpdateViewForMall (int index, int order, TMall mall) {
		mIndex = index;
		mSelf.name = order.ToString();
		PriceButton.name = mall.Order.ToString();
		mSelf.transform.localPosition = new Vector3(-400 + order * 275, 0, 0);
		PriceLabel.text = mall.Price;

		if(mall.Sale > 0)
			SaleLabel.text = getSaleText(mall.Sale);
		else
			SaleLabel.gameObject.SetActive(false);
		
		ItemIcon.spriteName = itemKindIconName(index);
		ItemNameLabel.text = mall.Name;
		ValueLabel.text = mall.Diamond.ToString();
	}

	//31.錢
	//32.鑽石
	//34.體力
	private string itemKindIconValueName (int kind) {
		switch(kind) {
		case 31:
			return "Icon_Coin";
		case 32:
			return "Icon_Gem";
		case 34:
			return "Icon_Stamina";
			
		}
		return "";
	}
	//1.4200 特賣
	//2.4201 熱門
	//3.4202 限時
	private string getSaleText (int sale) {
		return TextConst.S(4200 + (sale - 1));
	}

	private string itemKindIconName (int pic) {
		if(pic >= 0)
			return "MallGem"+(pic + 1);
        
		return "MallGem1";
	}
}

public class UIRecharge : UIBase {
	private static UIRecharge instance = null;
	private const string UIName = "UIRecharge";
    public static EventDelegate.Callback FreshUICallback = null;

	private GameObject[] prefabKind = new GameObject[3];

	private GameObject[] tabSelects = new GameObject[3];
	private GameObject[] tabGos = new GameObject[3];
	private GameObject[] pages = new GameObject[3];
	private GameObject[] scrollviews = new GameObject[3];

	private TItemRecharge[] kindBuyCoin;
	private TItemRecharge[] kindBuyStamina;

	private TItemRecharge[] kindBuyDiamond;

	private bool isInit = false;
	private int buyIndex = -1;
	private bool isBuyCoin = false;
	private int recordType = 0;
	private bool isRecord = false;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static void UIShow(bool isShow)
	{
	    if (instance) {
            if (!isShow) {
                FreshUICallback = null;
                RemoveUI(instance.gameObject);
            } else
				instance.Show(isShow);
        } else
		if (isShow) 
			Get.Show(isShow);

		if(isShow) 
			Statistic.Ins.LogScreen(17);
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
			tabSelects[i] = tabGos[i].transform.Find("Selected").gameObject;
			UIEventListener.Get(tabGos[i]).onClick = OnClickTab;
			pages[i] = GameObject.Find(UIName + "/Center/Window/Pages/" + i.ToString()); 
			scrollviews[i] = pages[i].transform.Find("Vertical/ScrollView").gameObject;
		}

		SetBtnFun(UIName + "/BottomLeft/BackBtn", OnClose);
	}

	public void ShowView (int type, EventDelegate.Callback uiUpdate = null ,bool isNeedRecord = true) {
		if(!UISkillReinforce.Visible) {
			if(IsNeedShowLobbyMenu)
				UIMainLobby.Get.Hide();
            
			if(IsNeedShowPlayer)
				UIPlayerAvatar.Get.Enable = false;
            
			if(UIShop.Visible)
			{
			    UIMainLobby.Get.Hide();
                UIResource.Get.Show(UIResource.EMode.PvpSocial);
			}
		}

		if(UIGymEngage.Visible)
			UIGymEngage.Get.CenterVisible = false;

		isRecord = isNeedRecord;
		if(!UIRecharge.Visible)
			recordType = type;
		
		UIShow(true);
		if (uiUpdate != null)
			UIRecharge.FreshUICallback = uiUpdate;

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
					UIEventListener.Get(kindBuyCoin[GameData.DShops[i].Order].PriceButton.gameObject).onClick = OnBuyCoin;
				else if(GameData.DShops[i].Kind == 1) 
					UIEventListener.Get(kindBuyCoin[GameData.DShops[i].Order].PriceButton.gameObject).onClick = OnPower;
			} else if(GameData.DShops[i].Kind == 1) {
				if(GameData.DShops[i].Order >= BuyStaminaLen) {
					Debug.LogError("Order is Wrong. Kind = " + GameData.DShops[i].Kind );
					break;
				}
				kindBuyStamina[GameData.DShops[i].Order].init(Instantiate(prefabKind[2]), scrollviews[2]);
				kindBuyStamina[GameData.DShops[i].Order].UpdateView(i, GameData.DShops[i].Order ,GameData.DShops[i]);
				kindBuyStamina[GameData.DShops[i].Order].RefreshTextColor();
				if(GameData.DShops[i].Kind == 0)
					UIEventListener.Get(kindBuyStamina[GameData.DShops[i].Order].PriceButton.gameObject).onClick = OnBuyCoin;
				else if(GameData.DShops[i].Kind == 1) 
					UIEventListener.Get(kindBuyStamina[GameData.DShops[i].Order].PriceButton.gameObject).onClick = OnPower;
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
			if(GameData.DMalls[i].Kind == 0) {
				kindBuyDiamond[GameData.DMalls[i].Order].init(Instantiate(prefabKind[0]), scrollviews[0]);
				kindBuyDiamond[GameData.DMalls[i].Order].UpdateViewForMall(i, GameData.DMalls[i].Order, GameData.DMalls[i]);
				UIEventListener.Get(kindBuyDiamond[GameData.DMalls[i].Order].PriceButton.gameObject).onClick = OnBuyDiamond;
			}
		}
	}

	public void OnBuyDiamond (GameObject go) {
        if (!SendHttp.Get.IAPinProcess) {
    		int result = -1;
    		if(int.TryParse(go.name, out result)) {
    			if(result >= 0 && result < kindBuyDiamond.Length && result < GameData.DMalls.Length) {
                    #if UNITY_IOS
                    if (FileManager.NowMode == VersionMode.Release)
                        UIHint.Get.ShowHint(TextConst.S(502), Color.black);
                    else
                        SendBuyDiamond(kindBuyDiamond[result].mIndex, GameData.DMalls[kindBuyDiamond[result].mIndex].Android);
                    #else
                    SendHttp.Get.SendIAP(kindBuyDiamond[result].mIndex, SendBuyDiamond);
                    #endif
    			}
    		}
        }
	}

	public void OnBuyCoin (GameObject go) {
		int result = -1;
		if(int.TryParse(go.name, out result)) {
			if(result >= 0 && result < kindBuyCoin.Length && GameData.DItemData.ContainsKey(kindBuyCoin[result].mShop.ItemID)) {
				buyIndex = kindBuyCoin[result].mIndex;
				isBuyCoin = true;
				if(kindBuyCoin[result].mShop.SpendKind == 0 ) 
					CheckDiamond(kindBuyCoin[result].mPrice, true, string.Format(TextConst.S(250), kindBuyCoin[result].mPrice, GameData.DItemData[kindBuyCoin[result].mShop.ItemID].Name), ConfirmBuy);
				else 
					CheckMoney(kindBuyCoin[result].mPrice, true, string.Format(TextConst.S(251), kindBuyCoin[result].mPrice, GameData.DItemData[kindBuyCoin[result].mShop.ItemID].Name), ConfirmBuy);
				
			}
		}
	}

	public void OnPower (GameObject go) {
		int result = -1;
		if(int.TryParse(go.name, out result)) {
			if(result >= 0 && result < kindBuyStamina.Length) {
				
				if(kindBuyStamina[result].mShop.Order == 0) {
					if (GameData.Team.DailyCount.BuyPowerOne >= kindBuyStamina[result].mShop.Limit.Length ) {
						UIHint.Get.ShowHint("over BuyPowerOne limit:"+ GameData.Team.DailyCount.BuyPowerOne, Color.red);
						return;
					}
				} else if(kindBuyStamina[result].mShop.Order == 1) {
					if (GameData.Team.DailyCount.BuyPowerTwo >= kindBuyStamina[result].mShop.Limit.Length ) {
						UIHint.Get.ShowHint("over BuyPowerTwo limit:"+ GameData.Team.DailyCount.BuyPowerTwo, Color.red);
						return;
					}
				}

				if(GameData.Team.Power < GameConst.Max_Power){
					buyIndex = kindBuyStamina[result].mIndex;
					isBuyCoin = false;
					if(kindBuyStamina[result].mShop.SpendKind == 0) 
						CheckDiamond(kindBuyStamina[result].mPrice, true, string.Format(TextConst.S(250), kindBuyStamina[result].mPrice, GameData.DItemData[kindBuyStamina[result].mShop.ItemID].Name), ConfirmBuy);
					else if(kindBuyStamina[result].mShop.SpendKind == 1) 
						CheckMoney(kindBuyStamina[result].mPrice, true, string.Format(TextConst.S(251), kindBuyStamina[result].mPrice, GameData.DItemData[kindBuyStamina[result].mShop.ItemID].Name), ConfirmBuy);
					
				} else
					UIHint.Get.ShowHint(TextConst.S(536), Color.blue);
			}
		}
	}

	public void ConfirmBuy () {
		SendBuyFromShop(buyIndex);
	}

	private void refreshPriceUI () {
		for(int i=0; i<GameData.DShops.Length; i++) {
			if(GameData.DShops[i].Kind == 0) 
				kindBuyCoin[GameData.DShops[i].Order].RefreshPrice();
			else 
            if(GameData.DShops[i].Kind == 1) 
				kindBuyStamina[GameData.DShops[i].Order].RefreshPrice();
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
		if(!GameData.IsMainStage && !GameData.IsInstance && !GameData.IsPVP)
		{
			if(!UISkillReinforce.Visible) {
				if(IsNeedShowLobbyMenu)
					UIMainLobby.Get.Show();
				if(IsNeedShowPlayer)
					UIPlayerAvatar.Get.Enable = true;
				if(UIShop.Visible)
				{
				    UIMainLobby.Get.Hide();
                    UIResource.Get.Show(UIResource.EMode.PvpSocial);
				}
			    if(UIMall.Visible)
					UIMall.Get.RefreshTextColor();
//				if(UIGym.Visible)
//					UIGym.Get.RefreshDiamondColor();

			}
		}

		if(UIGymEngage.Visible)
			UIGymEngage.Get.CenterVisible = true;

        if(UIInstance.Get.Visible)
            UIInstance.Get.ShowByChapter(UIInstance.Get.Main.CurrentChapter);

		UIMail.SetFocus (true);
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
            TBuyDiamond result = JsonConvertWrapper.DeserializeObject<TBuyDiamond>(www.text);
			GameData.Team.Diamond = result.Diamond;
			AudioMgr.Get.PlaySound (SoundType.SD_Buy);
			Statistic.Ins.LogEvent(401);

			UIMainLobby.Get.UpdateUI();
			refreshPriceUI ();

			if(isRecord)
				showTab(recordType);

            if (FreshUICallback != null)
                FreshUICallback();

            if (UIItemHint.Visible)
                UIItemHint.Get.FreshUI();

			if(GameData.IsMainStage || GameData.IsInstance || GameData.IsPVP)
                UIHint.Get.ShowHint(TextConst.S(535), Color.blue);
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
            TBuyFromShop result = JsonConvertWrapper.DeserializeObject<TBuyFromShop>(www.text);
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.Money = result.Money;
			GameData.Team.Power = result.Power;
			GameData.Team.DailyCount = result.DailyCount;
			GameData.Team.LifetimeRecord = result.LifetimeRecord;
			AudioMgr.Get.PlaySound (SoundType.SD_Buy);
			if(isBuyCoin)
				Statistic.Ins.LogEvent(402);
			else
				Statistic.Ins.LogEvent(403);

			UIMainLobby.Get.UpdateUI();
			if(isRecord)
				showTab(recordType);
			
			refreshPriceUI ();

            if (FreshUICallback != null)
                FreshUICallback();

            if (UIItemHint.Visible)
                UIItemHint.Get.FreshUI();

			if(GameData.IsMainStage || GameData.IsInstance || GameData.IsPVP)
				UIHint.Get.ShowHint(TextConst.S(535), Color.red);
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.BuyDiamond);
	}

	public bool IsNeedShowPlayer{
		get {
			return (UIPlayerInfo.Visible || UIShop.Visible || UIAvatarFitted.Visible);
		}
	}

	public bool IsNeedShowLobbyMenu {
		get {
			return !(UIMainStage.Get.Visible || UIGameLobby.Get.gameObject.activeInHierarchy || UIPVP.Visible || UIInstance.Get.Visible ||
				UISkillFormation.Visible || UISkillReinforce.Visible || UIPlayerInfo.Visible || UIMission.Visible || UIAvatarFitted.Visible ||
				UIEquipment.Get.Visible || UISocial.Visible || UIShop.Visible || UIMall.Visible || UIBuyStore.Visible || UIPlayerPotential.Visible || 
				UICreateRole.Visible || UIGymEngage.Visible || UIMail.Visible);
		}
	}
}
