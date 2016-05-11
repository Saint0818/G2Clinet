﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;

public struct TFreshShopResult {
    public int Kind;
    public int Diamond;
    public DateTime FreshShopTime;
    public TDailyCount DailyCount;
    public TSellItem[] ShopItems1;
    public TSellItem[] ShopItems2;
    public TSellItem[] ShopItems3;
}

public class TShopItemObj {
    public int Index;
    public TSellItem Data;
    public GameObject Item;
    public GameObject UISuit;
    public GameObject UISoldout;
    public ItemAwardGroup AwardGroup;
    public UIButton ButtonAwardGroup;
    public UILabel LabelName;
    public UILabel LabelPrice;
    public UILabel LabelSuitCount;
    public UISprite SpriteSpendKind;
    public UISprite SpriteSuit; 
    public UIButton ButtonSuit; 
    public UIButton ButtonBuy;
}

public class UIShop : UIBase {
    private static UIShop instance = null;
    private const string UIName = "UIShop";

    private const int pageNum = 3;
    private int nowPage = 0;
    private int nowIndex = -1;
    private TAvatar equipAvatar = new TAvatar();

    private GameObject itemSellItem;
    private UILabel labelFreshTime;
    private UILabel labelFreshDiamond;
    private UIButton buttonFreshDiamond;
    private GameObject[] uiSuits = new GameObject[pageNum];
    private GameObject[] pageObjects = new GameObject[pageNum];
    private UIToggle[] pageToggle = new UIToggle[pageNum];
    private UIScrollView[] pageScrollViews = new UIScrollView[pageNum];
    private List<TShopItemObj>[] shopItemList = new List<TShopItemObj>[pageNum];

    private int diamondFresh = 0;

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

            if(value)
            {
                Statistic.Ins.LogScreen(15);
                Statistic.Ins.LogEvent(206);
            }
        }
    }

    public static UIShop Get
    {
        get {
            if (!instance) 
                instance = LoadUI(UIName) as UIShop;

            return instance;
        }
    }

    protected override void InitCom() {
        itemSellItem = Resources.Load("Prefab/UI/Items/ItemShop") as GameObject;
        SetBtnFun(UIName + "/BottomLeft/BackBtn", OnClose);
		SetBtnFun(UIName + "/BottomLeft/ResetBtn", OnFreshShop);

        for (int i = 0; i < pageNum; i++) {
            SetBtnFun(UIName + "/Center/Right/Tabs/" + i.ToString(), OnPage);
            uiSuits[i] = GameObject.Find(UIName + "/Center/Right/Tabs/" + i.ToString() + "/FittingIcon");
            pageObjects[i] = GameObject.Find(UIName + "/Center/Right/Pages/" + i.ToString());
            pageScrollViews[i] = GameObject.Find(UIName + "/Center/Right/Pages/" + i.ToString() + "/ItemList").GetComponent<UIScrollView>();
            pageToggle[i] = GameObject.Find(UIName + "/Center/Right/Tabs/" + i.ToString()).GetComponent<UIToggle>();

            uiSuits[i].SetActive(false);
            pageObjects[i].SetActive(false);
        }

		labelFreshTime = GameObject.Find(UIName + "/BottomLeft/WarningsLabel").GetComponent<UILabel>();
		labelFreshDiamond = GameObject.Find(UIName + "/BottomLeft/ResetBtn/PriceLabel").GetComponent<UILabel>();
        buttonFreshDiamond = GameObject.Find(UIName + "/BottomLeft/ResetBtn").GetComponent<UIButton>();
    }

    private void initList(int page) {
        int diamond = 0;
        if (GameData.Team.FreshShopTime.ToUniversalTime() > DateTime.UtcNow)
            diamond = 50 * (GameData.Team.DailyCount.FreshShop +1);

        bool flag = GameData.Team.CoinEnough(0, diamond);
        labelFreshDiamond.color = GameData.CoinEnoughTextColor(flag);
        buttonFreshDiamond.normalSprite = GameData.CoinEnoughSprite(flag, 1);

        if (shopItemList[page] == null)
            shopItemList[page] = new List<TShopItemObj>();

        switch (page) {
            case 0:
                if (GameData.Team.ShopItems1 != null) {
                    for (int i = 0; i < GameData.Team.ShopItems1.Length; i++)
                        if (GameData.DItemData.ContainsKey(GameData.Team.ShopItems1[i].ID))
                            addItem(page, i, GameData.Team.ShopItems1[i]);

                    if (GameData.Team.ShopItems1.Length < shopItemList[page].Count) 
                        for (int i = GameData.Team.ShopItems1.Length; i < shopItemList[page].Count; i++)
                            shopItemList[page][i].Item.SetActive(false);
                }

                break;
            case 1:
                if (GameData.Team.ShopItems2 != null) {
                    for (int i = 0; i < GameData.Team.ShopItems2.Length; i++)
                        addItem(page, i, GameData.Team.ShopItems2[i]);

                    if (GameData.Team.ShopItems2.Length < shopItemList[page].Count) 
                        for (int i = GameData.Team.ShopItems2.Length; i < shopItemList[page].Count; i++)
                            shopItemList[page][i].Item.SetActive(false);
                }

                break;
            case 2:
                if (GameData.Team.ShopItems3 != null) {
                    for (int i = 0; i < GameData.Team.ShopItems3.Length; i++)
                        addItem(page, i, GameData.Team.ShopItems3[i]);

                    if (GameData.Team.ShopItems3.Length < shopItemList[page].Count) 
                        for (int i = GameData.Team.ShopItems3.Length; i < shopItemList[page].Count; i++)
                            shopItemList[page][i].Item.SetActive(false);
                }

                break;
        }
    }

    private void addItem(int page, int index, TSellItem data) {
        if (!GameData.DItemData.ContainsKey(data.ID)) {
            Debug.Log("Shop item id error " + data.ID.ToString());
            return;
        }

        if (index >= shopItemList[page].Count) {
            TShopItemObj item = new TShopItemObj();
            item.Item = Instantiate(itemSellItem, Vector3.zero, Quaternion.identity) as GameObject;
            item.Item.GetComponent<UIDragScrollView>().scrollView = pageScrollViews[page];
            string name = index.ToString();
            item.Item.name = name;

            SetLabel(name + "/FittingIcon/Label", TextConst.S(4508));
            SetLabel(name + "/SoldOutIcon/Label", TextConst.S(4509));

            item.ButtonBuy = GameObject.Find(name + "/BuyBtn").GetComponent<UIButton>();
            if (item.ButtonBuy)
                SetBtnFun(ref item.ButtonBuy, OnBuy);

            item.LabelSuitCount = GameObject.Find(name + "/SuitItem/CountLabel").GetComponent<UILabel>();
            item.SpriteSuit = GameObject.Find(name + "/SuitItem").GetComponent<UISprite>();
            item.ButtonSuit = GameObject.Find(name + "/SuitItem").GetComponent<UIButton>();
            if (item.ButtonSuit)
                SetBtnFun(ref item.ButtonSuit, OnSuitItem);

            item.UISuit = GameObject.Find(name + "/FittingIcon");
            item.UISoldout = GameObject.Find(name + "/SoldOutIcon");
            GameObject obj = GameObject.Find(name + "/ItemAwardGroup");
            if (obj) {
                item.AwardGroup = obj.GetComponent<ItemAwardGroup>();
                item.ButtonAwardGroup = obj.GetComponent<UIButton>();
            }

            item.LabelName = GameObject.Find(name + "/ItemName").GetComponent<UILabel>();
            item.LabelPrice = GameObject.Find(name + "/BuyBtn/PriceLabel").GetComponent<UILabel>();
            item.SpriteSpendKind = GameObject.Find(name + "/BuyBtn/Icon").GetComponent<UISprite>();

            item.Item.transform.parent = pageScrollViews[page].gameObject.transform;

            int x = index / 2;
            int y = index % 2;

            item.Item.transform.localPosition = new Vector3(-310 + x * 215, 135 - y * 270, 0);
            item.Item.transform.localScale = Vector3.one;
            shopItemList[page].Add(item);
            index = shopItemList[page].Count-1;
        }

        shopItemList[page][index].Item.SetActive(true);
        shopItemList[page][index].Index = index;
        shopItemList[page][index].Data = data;
        shopItemList[page][index].LabelName.text = GameData.DItemData[data.ID].Name;
        shopItemList[page][index].UISoldout.SetActive(data.Num <= 0);
        shopItemList[page][index].UISuit.SetActive(false);
        shopItemList[page][index].LabelPrice.text = NumFormater.Convert(data.Price);
        shopItemList[page][index].SpriteSpendKind.spriteName = GameFunction.SpendKindTexture(data.SpendKind);
        shopItemList[page][index].ButtonSuit.name = data.ID.ToString();

        if (shopItemList[page][index].ButtonAwardGroup) {
            shopItemList[page][index].ButtonAwardGroup.onClick.Clear();
            if (GameData.DItemData[data.ID].Kind <= 7)
                SetBtnFun(ref shopItemList[page][index].ButtonAwardGroup, OnSuit);
			else if (GameData.DItemData[data.ID].Kind == 21)
				SetBtnFun(ref shopItemList[page][index].ButtonAwardGroup, OnSkillInfo);
			else
                SetBtnFun(ref shopItemList[page][index].ButtonAwardGroup, OnBuy);
        }

        bool flag = GameData.Team.CoinEnough(shopItemList[page][index].Data.SpendKind, shopItemList[page][index].Data.Price);
        shopItemList[page][index].ButtonBuy.normalSprite = GameData.CoinEnoughSprite(flag, 1);
        shopItemList[page][index].LabelPrice.color = GameData.CoinEnoughTextColor(flag, shopItemList[page][index].Data.SpendKind);
        SetSuitItem(data.ID, shopItemList[page][index].ButtonSuit, shopItemList[page][index].SpriteSuit, shopItemList[page][index].LabelSuitCount);

        if (GameData.DItemData.ContainsKey(data.ID)) {
            shopItemList[page][index].AwardGroup.Show(GameData.DItemData[data.ID]);
            if (data.Num > 1)
                shopItemList[page][index].AwardGroup.SetAmountText("X" + data.Num.ToString());
        }
    }

    private void SetSuitItem(int itemID, UIButton btn, UISprite sp, UILabel lab) {
        if (btn && sp && lab) {
            btn.gameObject.SetActive(true);
            if (GameData.DItemData[itemID].Kind < 8) {
                int id = GameData.DItemData[itemID].SuitItem;
                if (GameData.DSuitItem.ContainsKey(id))
                    lab.text = string.Format("{0}/{1}", GameData.Team.SuitItemCompleteCount(id), GameData.DSuitItem[id].ItemLength);
                else
                    btn.gameObject.SetActive(false);
            } else
            if (GameData.DItemData[itemID].Kind == 21) {
                sp.spriteName = "SuitLight";
                int id = GameData.DItemData[itemID].SuitCard;
                if (GameData.DSuitCard.ContainsKey(id)) 
					lab.text = string.Format("{0}/{1}", GameData.Team.SuitCardCompleteCount(id).ToString(), GameData.DSuitCard[id].ItemCount);
                else
                    btn.gameObject.SetActive(false);
            } else
                btn.gameObject.SetActive(false);

            if (btn.gameObject.activeInHierarchy) {
                if (!GameData.Team.IsGetItem(itemID)) {
                    btn.defaultColor = Color.gray;
                    btn.hover = Color.gray;
                    btn.pressed = Color.gray;
                } else {
                    btn.defaultColor = Color.white;
                    btn.hover = Color.white;
                    btn.pressed = Color.white; 
                }
            }
        }
    }

    void FixedUpdate() {
        if (GameData.Team.FreshShopTime.ToUniversalTime() > DateTime.UtcNow)
            labelFreshTime.text = TextConst.SecondString(GameData.Team.FreshShopTime.ToUniversalTime());
        else {
            labelFreshTime.text = TextConst.S(4510);
            labelFreshDiamond.text = "0";
        }
    }

    private void updateUI() {
        OpenPage(nowPage);
    }
   
    private void refreshShop(int kind) {
        WWWForm form = new WWWForm();
        form.AddField("Kind", kind.ToString());
        SendHttp.Get.Command(URLConst.RefreshShop, waitRefreshShop, form);
    }

    protected override void OnShow(bool isShow) {
        base.OnShow(isShow);

        if (isShow) {
            equipAvatar = GameData.Team.Player.Avatar;
            UIPlayerAvatar.Get.ShowUIPlayer(EUIPlayerMode.UIShop, GameData.Team.Player.BodyType, equipAvatar);
            if (GameData.Team.ShopItems1 == null || GameData.Team.FreshShopTime.ToUniversalTime().CompareTo(DateTime.UtcNow) < 0)
                refreshShop(0);

            OpenPage(nowPage);

            labelFreshTime.text = "";
            labelFreshDiamond.text = (50 * (GameData.Team.DailyCount.FreshShop +1)).ToString();
        } 
    }

    public void OnClose() {
        Visible = false;
        UIPlayerAvatar.Visible = false;
        if (!UIPVP.Visible)
            UIMainLobby.Get.Show();
    }

    public void OpenPage(int page) {
        for (int i = 0; i < pageObjects.Length; i++) {
            pageObjects[i].SetActive(false);
            pageToggle[i].value = false;
        }

        pageToggle[page].value = true;
        pageObjects[page].SetActive(true);
        nowPage = page;
        initList(page);
    }

    public void OnPage() {
        int index = -1;
        if (int.TryParse(UIButton.current.name, out index))
            OpenPage(index);
    }

    public void OnBuy() {
        if (UIButton.current.transform.parent.gameObject && 
            int.TryParse(UIButton.current.transform.parent.gameObject.name, out nowIndex) &&
			shopItemList[nowPage][nowIndex].Data.Num > 0)
		{
			if (GameData.DItemData.ContainsKey(shopItemList[nowPage][nowIndex].Data.ID)) 
            	UIItemHint.Get.OpenBuyUI(shopItemList[nowPage][nowIndex].Data, sendBuyItem);
			
		}
    }

	public void OnSkillInfo () {
		if (UIButton.current.transform.parent.gameObject && 
			int.TryParse(UIButton.current.transform.parent.gameObject.name, out nowIndex) &&
			shopItemList[nowPage][nowIndex].Data.Num > 0)
		{
			if (GameData.DItemData.ContainsKey(shopItemList[nowPage][nowIndex].Data.ID)) {
				TSkill skill = new TSkill();
				skill.ID = GameData.DItemData[shopItemList[nowPage][nowIndex].Data.ID].Avatar;
				skill.Lv = GameData.DItemData[shopItemList[nowPage][nowIndex].Data.ID].LV;
				UISkillInfo.Get.ShowFromNewCard (skill, closeSkillInfo);
			}
		}
	}

	private void closeSkillInfo () {
		UIResource.Get.Show(UIResource.EMode.PvpSocial);
	}

    public void OnSuitItem() {
        int id = -1;
        if (int.TryParse(UIButton.current.name, out id)) {
            if (GameData.DItemData.ContainsKey(id)) {
                if (GameData.DItemData[id].Kind == 21) {
					if(GameData.IsOpenUIEnableByPlayer(GameEnum.EOpenID.SuitCard))
						UISuitAvatar.Get.ShowView(1, 0, GameData.DItemData[id].SuitCard);
					else 
						UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)GameEnum.EOpenID.SuitCard)),LimitTable.Ins.GetLv(GameEnum.EOpenID.SuitCard)) , Color.black);
					
                    //Visible = false;
                    //UIPlayerAvatar.Visible = false;
                } else
                if (GameData.DItemData[id].Kind <= 7)
					if(GameData.IsOpenUIEnableByPlayer(GameEnum.EOpenID.SuitItem))
						UISuitAvatar.Get.ShowView(GameData.DItemData[id].SuitItem, 1);
					else 
						UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)GameEnum.EOpenID.SuitItem)),LimitTable.Ins.GetLv(GameEnum.EOpenID.SuitItem)) , Color.black);
            }
        }
    }

    private void checkOtherSuit(int kind, int page, int index) {
        for (int j = 0; j < shopItemList.Length; j ++) {
            if (shopItemList[j] != null) {
                for (int i = 0; i < shopItemList[j].Count; i++) {
                    if ((j != nowPage || i != index) && shopItemList[j][i].UISuit.activeInHierarchy) {
                        int id = shopItemList[j][i].Data.ID;
                        if (GameData.DItemData[id].Kind == kind) {
                            shopItemList[j][i].UISuit.SetActive(false);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void OnSuit() {
        if (UIButton.current.transform.parent.gameObject && 
            int.TryParse(UIButton.current.transform.parent.gameObject.name, out nowIndex)) {
            int id = shopItemList[nowPage][nowIndex].Data.ID;
            if (GameData.DItemData[id].Kind < GameData.Team.Player.Items.Length) {
                if (GameData.DItemData[id].Position == 3 || GameData.DItemData[id].Position == GameData.Team.Player.BodyType) {
                    shopItemList[nowPage][nowIndex].UISuit.SetActive(!shopItemList[nowPage][nowIndex].UISuit.activeInHierarchy);

                    if (shopItemList[nowPage][nowIndex].UISuit.activeInHierarchy) {
                        shopItemList[nowPage][nowIndex].UISoldout.SetActive(false);
                        checkOtherSuit(GameData.DItemData[id].Kind, nowPage, nowIndex);
                        switch(GameData.DItemData[id].Kind) {
                            case 0: equipAvatar.Body = GameData.DItemData[id].Avatar; break;
                            case 1: equipAvatar.Hair = GameData.DItemData[id].Avatar; break;
                            case 2: equipAvatar.MHandDress = GameData.DItemData[id].Avatar; break;
                            case 3: equipAvatar.Cloth = GameData.DItemData[id].Avatar; break;
                            case 4: equipAvatar.Pants = GameData.DItemData[id].Avatar; break;
                            case 5: equipAvatar.Shoes = GameData.DItemData[id].Avatar; break;
                            case 6: equipAvatar.AHeadDress = GameData.DItemData[id].Avatar; break;
                            case 7: equipAvatar.ZBackEquip = GameData.DItemData[id].Avatar; break;
                        } 
                    } else {
                        shopItemList[nowPage][nowIndex].UISoldout.SetActive(shopItemList[nowPage][nowIndex].Data.Num == 0);
                        switch(GameData.DItemData[id].Kind) {
                            case 0: equipAvatar.Body = GameData.Team.Player.Avatar.Body; break;
                            case 1: equipAvatar.Hair = GameData.Team.Player.Avatar.Hair; break;
                            case 2: equipAvatar.MHandDress = GameData.Team.Player.Avatar.MHandDress; break;
                            case 3: equipAvatar.Cloth = GameData.Team.Player.Avatar.Cloth; break;
                            case 4: equipAvatar.Pants = GameData.Team.Player.Avatar.Pants; break;
                            case 5: equipAvatar.Shoes = GameData.Team.Player.Avatar.Shoes; break;
                            case 6: equipAvatar.AHeadDress = GameData.Team.Player.Avatar.AHeadDress; break;
                            case 7: equipAvatar.ZBackEquip = GameData.Team.Player.Avatar.ZBackEquip; break;
                        }  
                    }

                    UIPlayerAvatar.Get.ChangeAvatar(GameData.Team.Player.BodyType, equipAvatar);
                } else 
                    UIHint.Get.ShowHint(TextConst.S(4514), Color.black);
            }
        }
    }

    public void OnFreshShop() {
        diamondFresh = 0;
        if (GameData.Team.FreshShopTime.ToUniversalTime() > DateTime.UtcNow)
            diamondFresh = 50 * (GameData.Team.DailyCount.FreshShop +1);
        
        CheckDiamond(diamondFresh, true, TextConst.S(4511) + diamondFresh.ToString(), doFreshShop, updateUI);
    }

    private void doFreshShop() {
        refreshShop(nowPage + 1);
    }

    private void sendBuyItem() {
        switch (shopItemList[nowPage][nowIndex].Data.SpendKind) {
            case 0:
                if (!CheckDiamond(shopItemList[nowPage][nowIndex].Data.Price, true, "", null, updateUI))
                    return;

                break;
            case 1:
                if (!CheckMoney(shopItemList[nowPage][nowIndex].Data.Price, true, null, updateUI))
                    return;

                break;
            case 2:
                if (GameData.Team.PVPCoin < shopItemList[nowPage][nowIndex].Data.Price) {
                    UIHint.Get.ShowHint(TextConst.S(4515), Color.black);
                    return;
                }

                break;
            case 3:
                if (GameData.Team.SocialCoin < shopItemList[nowPage][nowIndex].Data.Price) {
                    UIHint.Get.ShowHint(TextConst.S(4516), Color.black);
                    return;
                }

                break;
        }

        UIItemHint.Visible = false;
        WWWForm form = new WWWForm();
        form.AddField("Identifier", GameData.Team.Identifier);
        form.AddField("Kind", nowPage.ToString());
        form.AddField("Index", nowIndex.ToString());
        SendHttp.Get.Command(URLConst.BuyMyShop, waitBuy, form);
    }

    private void waitRefreshShop(bool ok, WWW www) {
        if (ok) {
            TFreshShopResult result = JsonConvertWrapper.DeserializeObject <TFreshShopResult>(www.text);
            GameData.Team.Diamond = result.Diamond;
            GameData.Team.FreshShopTime = result.FreshShopTime;
            GameData.Team.DailyCount.FreshShop = result.DailyCount.FreshShop;
            labelFreshDiamond.text = (50 * (GameData.Team.DailyCount.FreshShop +1)).ToString();
            GameData.Team.ShopItems1 = result.ShopItems1;
            GameData.Team.ShopItems2 = result.ShopItems2;
            GameData.Team.ShopItems3 = result.ShopItems3;

            initList(nowPage);
            Statistic.Ins.LogEvent(209, diamondFresh);
        }
    }

    private void waitBuy(bool ok, WWW www) {
        if (ok) {
            TTeam result = JsonConvertWrapper.DeserializeObject <TTeam>(www.text);

            //record GA event
            if (nowPage < shopItemList.Length && nowIndex < shopItemList[nowPage].Count) {
                string id = shopItemList[nowPage][nowIndex].Data.ID.ToString();
                int diamond = GameData.Team.Diamond - result.Diamond;
                int money = GameData.Team.Money - result.Money;
                int pvpCoin = GameData.Team.PVPCoin - result.PVPCoin;
                int socialCoin = GameData.Team.SocialCoin - result.SocialCoin;
                if (diamond > 0) 
                    Statistic.Ins.LogEvent(210, id, diamond);

                if (money > 0) 
                    Statistic.Ins.LogEvent(211, id, money);

                if (pvpCoin > 0) 
                    Statistic.Ins.LogEvent(212, id, pvpCoin);

                if (socialCoin > 0) 
                    Statistic.Ins.LogEvent(213, id, socialCoin);
            }

            GameData.Team.Diamond = result.Diamond;
            GameData.Team.Money = result.Money;
            GameData.Team.PVPCoin = result.PVPCoin;
            GameData.Team.SocialCoin = result.SocialCoin;
            GameData.Team.LifetimeRecord = result.LifetimeRecord;

            if (result.AvatarPotential > GameData.Team.AvatarPotential)
                GameData.Team.AvatarPotential = result.AvatarPotential;
            
            if (result.Items != null)
                GameData.Team.Items = result.Items;

            if (result.ValueItems != null)
                GameData.Team.ValueItems = result.ValueItems;

            if (result.MaterialItems != null)
                GameData.Team.MaterialItems = result.MaterialItems;

            if (result.SkillCards != null)
                GameData.Team.SkillCards = result.SkillCards;

            if (result.GotItemCount != null)
                GameData.Team.GotItemCount = result.GotItemCount;
            
            if (result.GotAvatar != null)
                GameData.Team.GotAvatar = result.GotAvatar;
            
            if (result.SkillCardCounts != null)
                GameData.Team.SkillCardCounts = result.SkillCardCounts;

            if (result.ShopItems1 != null)
                GameData.Team.ShopItems1 = result.ShopItems1;

            if (result.ShopItems2 != null)
                GameData.Team.ShopItems2 = result.ShopItems2;

            if (result.ShopItems3 != null)
                GameData.Team.ShopItems3 = result.ShopItems3;

            initList(nowPage);
        }
    }
}
