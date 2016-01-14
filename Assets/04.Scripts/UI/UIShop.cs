using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;

public struct TBuyItemResult {
    public int Kind;
    public int Index;
    public int Diamond;
    public int Money;
    public TItem[] Items;
    public TValueItem[] ValueItems;
    public TMaterialItem[] MaterialItems;
    public TSkill[] SkillCards;
    public Dictionary<int, int> GotItemCount; //key: item id, value: got number
    public Dictionary<int, int> GotAvatar; //key: item id, value: 1 : got already
    public Dictionary<int, int> SkillCardCounts; //key: ID , value:num
}

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
    public UILabel LabelName;
    public UILabel LabelPrice;
    public UISprite SpriteSpendKind;
}

public class UIShop : UIBase {
    private static UIShop instance = null;
    private const string UIName = "UIShop";

    private const int pageNum = 3;
    private int nowPage = 0;

    private GameObject itemSellItem;
    private UILabel labelPVPCoin;
    private UILabel labelSocialCoin;
    private UILabel labelFreshTime;
    private UILabel labelFreshDiamond;
    private GameObject[] uiSuits = new GameObject[pageNum];
    private GameObject[] pageObjects = new GameObject[pageNum];
    private UIScrollView[] pageScrollViews = new UIScrollView[pageNum];
    private List<TShopItemObj>[] shopItemList = new List<TShopItemObj>[pageNum];

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
                    RemoveUI(UIName);
                else
                    instance.Show(value);
            } else
                if (value)
                    Get.Show(value);
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
        SetBtnFun(UIName + "/Center/BottomRight/ResetBtn", OnFreshShop);

        for (int i = 0; i < pageNum; i++) {
            uiSuits[i] = GameObject.Find(UIName + "/Center/Right/Tabs/" + i.ToString() + "/FittingIcon");
            pageObjects[i] = GameObject.Find(UIName + "/Center/Right/Pages/" + i.ToString());
            pageScrollViews[i] = GameObject.Find(UIName + "/Center/Right/Pages/" + i.ToString() + "/ItemList").GetComponent<UIScrollView>();
            SetBtnFun(UIName + "/Center/Right/Tabs/" + i.ToString(), OnPage);

            uiSuits[i].SetActive(false);
            pageObjects[i].SetActive(false);
        }

        labelPVPCoin = GameObject.Find(UIName + "/TopRight/PVPCoin/Label").GetComponent<UILabel>();
        labelSocialCoin = GameObject.Find(UIName + "/TopRight/SocialCoin/Label").GetComponent<UILabel>();
        labelFreshTime = GameObject.Find(UIName + "/Center/BottomRight/WarningsLabel").GetComponent<UILabel>();
        labelFreshDiamond = GameObject.Find(UIName + "/Center/BottomRight/ResetBtn/PriceLabel").GetComponent<UILabel>();
    }

    protected override void InitData() {

    }

    private void initList(int page) {
        if (shopItemList[page] == null)
            shopItemList[page] = new List<TShopItemObj>();

        for (int i = 0; i < shopItemList[page].Count; i++)
            shopItemList[page][i].Item.SetActive(false);

        switch (page) {
            case 0:
                if (GameData.Team.ShopItems1 != null) {
                    for (int i = 0; i < GameData.Team.ShopItems1.Length; i++)
                        if (GameData.DItemData.ContainsKey(GameData.Team.ShopItems1[i].ID))
                            addItem(page, i, GameData.Team.ShopItems1[i]);
                }

                break;
            case 1:
                if (GameData.Team.ShopItems2 != null) {
                    for (int i = 0; i < GameData.Team.ShopItems2.Length; i++)
                        addItem(page, i, GameData.Team.ShopItems2[i]);
                }

                break;
            case 2:
                if (GameData.Team.ShopItems3 != null) {
                    for (int i = 0; i < GameData.Team.ShopItems3.Length; i++)
                        addItem(page, i, GameData.Team.ShopItems3[i]);
                }

                break;
        }
    }

    private void addItem(int page, int index, TSellItem data) {
        if (index >= shopItemList[page].Count) {
            TShopItemObj item = new TShopItemObj();
            item.Item = Instantiate(itemSellItem, Vector3.zero, Quaternion.identity) as GameObject;
            item.Item.GetComponent<UIDragScrollView>().scrollView = pageScrollViews[page];
            string name = index.ToString();
            item.Item.name = name;

            SetLabel(name + "/FittingIcon/Label", TextConst.S(4508));
            SetLabel(name + "/SoldOutIcon/Label", TextConst.S(4509));
            SetBtnFun(name + "/BuyBtn", OnBuy);
            SetBtnFun(name, OnInfo);

            item.UISuit = GameObject.Find(name + "/FittingIcon");
            item.UISoldout = GameObject.Find(name + "/SoldOutIcon");
            GameObject obj = GameObject.Find(name + "/ItemAwardGroup");
            if (obj)
                item.AwardGroup = obj.GetComponent<ItemAwardGroup>();
            
            item.LabelName = GameObject.Find(name + "/ItemName").GetComponent<UILabel>();
            item.LabelPrice = GameObject.Find(name + "/BuyBtn/PriceLabel").GetComponent<UILabel>();
            item.SpriteSpendKind = GameObject.Find(name + "/BuyBtn/Icon").GetComponent<UISprite>();

            item.Item.transform.parent = pageScrollViews[page].gameObject.transform;

            int x = index / 2;
            int y = index % 2;

            item.Item.transform.localPosition = new Vector3(-300 + x * 220, 110 - y * 230, 0);
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
        shopItemList[page][index].LabelPrice.text = data.Price.ToString();
        if (data.SpendKind == 0) {
            shopItemList[page][index].SpriteSpendKind.spriteName = "Icon_Gem";
            shopItemList[page][index].LabelPrice.color = new Color(255, 0, 255, 255);
        } else {
            shopItemList[page][index].SpriteSpendKind.spriteName = "Icon_Coin";
            shopItemList[page][index].LabelPrice.color = Color.white;
        }

        if (GameData.DItemData.ContainsKey(data.ID))
            shopItemList[page][index].AwardGroup.Show(GameData.DItemData[data.ID]);
    }

    void FixedUpdate() {
        if (GameData.Team.FreshShopTime.ToUniversalTime() > DateTime.UtcNow)
            labelFreshTime.text = string.Format(TextConst.S(4507), TextConst.DeadlineString(GameData.Team.FreshShopTime.ToUniversalTime()));
        else {
            labelFreshTime.text = TextConst.S(4510);
            labelFreshDiamond.text = "0";
        }
    }

    private void refreshShop(int kind) {
        WWWForm form = new WWWForm();
        form.AddField("Kind", kind.ToString());
        SendHttp.Get.Command(URLConst.RefreshShop, waitRefreshShop, form);
    }

    protected override void OnShow(bool isShow) {
        base.OnShow(isShow);

        if (isShow) {
            if (GameData.Team.FreshShopTime.ToUniversalTime().CompareTo(DateTime.UtcNow) < 0)
                refreshShop(0);

            openPage(nowPage);
        }

        labelPVPCoin.text = "0";
        labelSocialCoin.text = "0";
        labelFreshTime.text = "";
        labelFreshDiamond.text = (50 * (GameData.Team.DailyCount.FreshShop +1)).ToString();
    }

    public void OnClose() {
        Visible = false;
        UIMainLobby.Get.Show();
    }

    public void openPage(int page) {
        for (int i = 0; i < pageObjects.Length; i++)
            pageObjects[i].SetActive(false);

        pageObjects[page].SetActive(true);
        nowPage = page;
        initList(page);
    }

    public void OnPage() {
        int index = -1;
        if (int.TryParse(UIButton.current.name, out index))
            openPage(index);
    }

    public void OnBuy() {
        int index = -1;
        if (UIButton.current.transform.parent.gameObject && 
            int.TryParse(UIButton.current.transform.parent.gameObject.name, out index)) {
            sendBuyItem(nowPage, index);
        }
    }

    public void OnInfo() {
        
    }

    public void OnFreshShop() {
        int diamond = 0;
        if (GameData.Team.FreshShopTime.ToUniversalTime() > DateTime.UtcNow)
            diamond = 50 * (GameData.Team.DailyCount.FreshShop +1);
        
        CheckDiamond(diamond, true, TextConst.S(4511) + diamond.ToString(), doFreshShop);
    }

    private void doFreshShop() {
        refreshShop(nowPage + 1);
    }

    private void sendBuyItem(int kind, int index) {
        WWWForm form = new WWWForm();
        form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
        form.AddField("Kind", kind.ToString());
        form.AddField("Index", index.ToString());
        SendHttp.Get.Command(URLConst.BuyMyShop, waitBuy, form);
    }

    private void waitRefreshShop(bool ok, WWW www) {
        if (ok) {
            TFreshShopResult result = JsonConvert.DeserializeObject <TFreshShopResult>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.Diamond = result.Diamond;
            GameData.Team.FreshShopTime = result.FreshShopTime;
            GameData.Team.DailyCount.FreshShop = result.DailyCount.FreshShop;
            labelFreshDiamond.text = (50 * (GameData.Team.DailyCount.FreshShop +1)).ToString();

            switch (result.Kind) {
                case 0:
                    GameData.Team.ShopItems1 = result.ShopItems1;
                    GameData.Team.ShopItems2 = result.ShopItems2;
                    GameData.Team.ShopItems3 = result.ShopItems3;
                    break;
                case 1:
                    GameData.Team.ShopItems1 = result.ShopItems1;
                    break;
                case 2:
                    GameData.Team.ShopItems2 = result.ShopItems2;
                    break;
                case 3:
                    GameData.Team.ShopItems3 = result.ShopItems3;
                    break;
            }

            initList(nowPage);
        }
    }

    private void waitBuy(bool ok, WWW www) {
        if (ok) {
            TTeam result = JsonConvert.DeserializeObject <TTeam>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.Diamond = result.Diamond;
            GameData.Team.Money = result.Money;

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
