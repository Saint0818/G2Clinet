using UnityEngine;
using System.Collections;
using GameStruct;
using Newtonsoft.Json;

public struct TPickLotteryResult {
	public int[] ItemIDs;
	public TTeam Team;
}

public enum EPickSpendType {
	ONE = 1,
	FIVE = 2,
	TEN = 3
}


public class UIBuyStore : UIBase {
	private static UIBuyStore instance = null;
	private const string UIName = "UIBuyStore";

	private TPickCost mPickCost;
	private int mSpendType;
	private TItemData[] mItemDatas;

	private Animator animationBuy;
	private GetOneItem oneItem;
	private GetFiveItem fiveItem;
	private GetTenItem tenItem;

	private UILabel labelPay;
	private UISprite spritePay;

//	private bool isOneAware = true; 

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
			if (isShow) {
				Get.Show(isShow);
			}
	}

	public static UIBuyStore Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIBuyStore;

			return instance;
		}
	}

	protected override void InitCom() {
		animationBuy = GetComponent<Animator>();
		oneItem = transform.FindChild("Center/ItemGet/GetItem_One").GetComponent<GetOneItem>();
		fiveItem = transform.FindChild("Center/ItemGet/GetItem_Five").GetComponent<GetFiveItem>();
		tenItem = transform.FindChild("Center/ItemGet/GetItem_Ten").GetComponent<GetTenItem>();

		oneItem.Reset();
		fiveItem.Reset();
		tenItem.Reset();

		labelPay = GameObject.Find(UIName + "/Center/ItemGet/AgainBt/PayLabel").GetComponent<UILabel>();
		spritePay = GameObject.Find(UIName + "/Center/ItemGet/AgainBt/PayIcon").GetComponent<UISprite>();

		UIEventListener.Get(GameObject.Find(UIName + "/Center/Touch")).onClick = StartDrawLottery;
		SetBtnFun(UIName + "/Center/ItemGet/AgainBt", OnAgain);
		SetBtnFun(UIName + "/Center/ItemGet/EnterBt", OnBack);
	}

	public void ShowView (TPickCost pick, int type, TItemData[] itemDatas) {
		UIShow(true);
		mPickCost = pick;
		mSpendType = type;
		updateView(type);
		mItemDatas = itemDatas;
	}

	/*
	0.台幣
	1.鑽石
	2.遊戲幣
	3.聯盟幣
	4.社群幣
	*/
	private void updateView (int type) {
		labelPay.text = getLabelPay(type);
		spritePay.spriteName = getCostName(type);
	}

	private bool checkCost (TPickCost pickCost, int spendType) {
		if(pickCost.Kind == 0)
			return false;
		else if(pickCost.Kind == 1)
			return (GameData.Team.Diamond >= howMuch(pickCost, spendType));
		else if(pickCost.Kind == 2)
			return (GameData.Team.Money >= howMuch(pickCost, spendType));
		else if(pickCost.Kind == 3)
			return false;
		else if(pickCost.Kind == 4) 
			return false;
		return false;
	}

	private int howMuch (TPickCost pickCost, int spendType) {
		if(spendType == EPickSpendType.ONE.GetHashCode())
			return pickCost.OnePick;
		else if(spendType == EPickSpendType.FIVE.GetHashCode())
			return pickCost.FivePick;
		else if(spendType == EPickSpendType.TEN.GetHashCode())
			return pickCost.TenPick;
		return -1;
	}

	private void showOne (TItemData itemData) {
		oneItem.Show(itemData);
		fiveItem.ShowAni(false);
		tenItem.ShowAni(false);
		animationBuy.SetTrigger("One");
		Invoke("FinishDrawLottery", 3.3f);
		UI3DBuyStore.Get.StartRaffle();
	}

	private void showFive(TItemData[] itemDatas) {
		fiveItem.Show(itemDatas);
		fiveItem.ShowAni(true);
		tenItem.ShowAni(false);
		animationBuy.SetTrigger("Five");
		UI3DBuyStore.Get.StartRaffle();
	}

	private void showTen (TItemData[] itemDatas) {
		tenItem.Show(itemDatas);
		tenItem.ShowAni(true);
		fiveItem.ShowAni(false);
		animationBuy.SetTrigger("Ten");
		UI3DBuyStore.Get.StartRaffle();
	}

	public void StartDrawLottery(GameObject go) {
		if(mItemDatas.Length == 1) {
			showOne(mItemDatas[0]);
		} else if(mItemDatas.Length == 5) {
			showFive(mItemDatas);
		} else if(mItemDatas.Length == 10) {
			showTen(mItemDatas);
		}
	}

	public void FinishDrawLottery () {
		UIMainLobby.Get.ShowForLottery(true);
		animationBuy.SetTrigger("Finish");
	}

	private void reset() {
		oneItem.Reset();
		fiveItem.Reset();
		tenItem.Reset();
		UIMainLobby.Get.ShowForLottery(false);
		animationBuy.SetTrigger("Again");	
		UI3DBuyStore.Get.AgainRaffle();
	}

	public void OnAgain() {
		if(checkCost(mPickCost, mSpendType) ) {
			SendPickLottery(mPickCost.Order, mPickCost.Kind, mSpendType);
		}
	}


	private string getLabelPay (int spendType) {
		if(spendType == EPickSpendType.ONE.GetHashCode()) 
			return mPickCost.OnePick.ToString();
		else if(spendType == EPickSpendType.FIVE.GetHashCode()) 
			return mPickCost.FivePick.ToString();
		else if(spendType == EPickSpendType.TEN.GetHashCode()) 
			return mPickCost.TenPick.ToString();
		return "0";
	}

	private string getCostName (int spendkind) {
		if(mPickCost.SpendKind == 0) 
			return "MallCoin";
		else if(mPickCost.SpendKind == 1) 
			return "MallGem";
		else if(mPickCost.SpendKind == 2) 
			return "MallCoin";
		else if(mPickCost.SpendKind == 3) 
			return "MallCoin";
		else if(mPickCost.SpendKind == 4) 
			return "MallCoin";
		return "";
	}

	public void OnBack () {
		UIShow(false);
		UI3DBuyStore.UIShow(false);
		UIMall.UIShow(true);
	}

	//order = 0 can used
	private void SendPickLottery(int order, int kind, int type)
	{
		if(order == 0) {
			WWWForm form = new WWWForm();
			form.AddField("Order", order);
			form.AddField("Kind", kind);
			form.AddField("Type", type);
			SendHttp.Get.Command(URLConst.PickLottery, waitPickLottery, form);
		}
	}

	private void waitPickLottery(bool ok, WWW www)
	{
		if(ok)
		{
			TPickLotteryResult result = (TPickLotteryResult)JsonConvert.DeserializeObject(www.text, typeof(TPickLotteryResult));
			GameData.Team.Items = result.Team.Items;
			GameData.Team.SkillCards = result.Team.SkillCards;
			GameData.Team.Diamond = result.Team.Diamond;

			if(result.ItemIDs != null) {
				mItemDatas = new TItemData[result.ItemIDs.Length];
				for(int i=0; i<result.ItemIDs.Length; i++) {
					if(GameData.DItemData.ContainsKey(result.ItemIDs[i]))
						mItemDatas[i] = GameData.DItemData[result.ItemIDs[i]];
				}
				reset();
			}
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.PickLottery);
	}
}