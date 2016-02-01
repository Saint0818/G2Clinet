using GameStruct;
using Newtonsoft.Json;
using UnityEngine;
using System;

public struct TPickLotteryResult {
	public int[] ItemIDs;
	public TItem[] Items;
	public TSkill[] SkillCards;
	public int Diamond;
	public int Money;
	public DateTime[] LotteryFreeTime;
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
	private int mIndex;

	private Animator animationBuy;
	private GetOneItem oneItem;
	private GetFiveItem fiveItem;
	private GetTenItem tenItem;

	private UILabel labelPay;
	private UISprite spritePay;
	private bool isClickTouch = false;

	public TSkill[] newSkillCard;
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

	void FixedUpdate () {
		if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) {
			StartDrawLottery(null);
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

		isClickTouch = false;
		UIEventListener.Get(GameObject.Find(UIName + "/Center/Touch")).onClick = StartDrawLottery;
		SetBtnFun(UIName + "/Center/ItemGet/AgainBt", OnAgain);
		SetBtnFun(UIName + "/Center/ItemGet/EnterBt", OnBack);
	}

	public void ShowView (TPickCost pick, int index, int type, TItemData[] itemDatas) {
		UIShow(true);
		mIndex = index;
		mPickCost = pick;
		mSpendType = type;
		updateView(type);
		mItemDatas = itemDatas;
		spritePay.spriteName = GameFunction.SpendKindTexture(pick.SpendKind);
	}

	public void SetNewSkillCard (TSkill[] skill) {
		newSkillCard = skill;
	}

	private void updateView (int type) {
		labelPay.text = getLabelPay(type);
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
		Invoke("ShowOneNew", 3.3f);
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

	public void Gohead () {
		fiveItem.GoAhead();
		tenItem.GoAhead();
	}

	public void StartDrawLottery(GameObject go) {
		if(!isClickTouch) {
			if(mItemDatas.Length == 1) {
				showOne(mItemDatas[0]);
			} else if(mItemDatas.Length == 5) {
				showFive(mItemDatas);
			} else if(mItemDatas.Length == 10) {
				showTen(mItemDatas);
			}
			isClickTouch = true;
		}
	}

	public void ShowOneNew () {
		oneItem.ShowNew();
	}

	public void FinishDrawLottery () {
		UIMainLobby.Get.ShowForLottery(true);
		animationBuy.SetTrigger("Finish");
		GameData.Team.SkillCards = newSkillCard;
	}

	private void reset() {
		oneItem.Reset();
		fiveItem.Reset();
		tenItem.Reset();
		UIMainLobby.Get.ShowForLottery(false);
		animationBuy.SetTrigger("Again");	
		UI3DBuyStore.Get.AgainRaffle();
		isClickTouch = false;
	}

	public void OnAgain() {
		if(mPickCost.SpendKind == 0) { // Diamond
			if(!CheckDiamond(howMuch(mPickCost, mSpendType), true, string.Format(TextConst.S(252), howMuch(mPickCost, mSpendType)), ConfirmUse))
				AudioMgr.Get.PlaySound (SoundType.SD_Prohibit);
		} else if (mPickCost.SpendKind == 1) { // Money
			if(!CheckMoney(howMuch(mPickCost, mSpendType), true, string.Format(TextConst.S(253), howMuch(mPickCost, mSpendType)), ConfirmUse))
				AudioMgr.Get.PlaySound (SoundType.SD_Prohibit);
		}
	}

	public void ConfirmUse () {
		SendPickLottery(mPickCost.Order, mPickCost.ID, mSpendType);
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
			form.AddField("Type", type);
			form.AddField("Index", mIndex);
			SendHttp.Get.Command(URLConst.PickLottery, waitPickLottery, form);
		}
	}

	private void waitPickLottery(bool ok, WWW www)
	{
		if(ok)
		{
			TPickLotteryResult result = (TPickLotteryResult)JsonConvert.DeserializeObject<TPickLotteryResult>(www.text, SendHttp.Get.JsonSetting);
			GameData.Team.Items = result.Items;
			newSkillCard = result.SkillCards;
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.Money = result.Money;
			GameData.Team.LotteryFreeTime = result.LotteryFreeTime;
			UIMainLobby.Get.UpdateUI();
			GameData.Team.InitSkillCardCount();

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