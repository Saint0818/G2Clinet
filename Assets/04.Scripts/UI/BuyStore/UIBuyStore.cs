﻿using GameStruct;
using Newtonsoft.Json;
using UnityEngine;
using System;
using System.Collections.Generic;

public struct TPickLotteryResult {
	public int[] ItemIDs;
	public TItem[] Items;
	public TSkill[] SkillCards;
	public int Diamond;
	public int Money;
	public DateTime[] LotteryFreeTime;
	public Dictionary<int, int> GotItemCount; //key: item id, value: got number
	public TMaterialItem[] MaterialItems;
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
	private bool isClickTouch = true;

//	public TSkill[] newSkillCard;
//	private bool isOneAware = true; 
	public Dictionary<int, int> newGetItemCount = new Dictionary<int, int>();

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
                RemoveUI(instance.gameObject);
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

		Invoke("delayTap", 2);
		UIEventListener.Get(GameObject.Find(UIName + "/Center/Touch")).onClick = StartDrawLottery;
		SetBtnFun(UIName + "/Center/ItemGet/AgainBt", OnAgain);
		SetBtnFun(UIName + "/Center/ItemGet/EnterBt", OnBack);
	}

	private void delayTap () {
		isClickTouch = false;
	}

	public void ShowView (TPickCost pick, int index, int type, TItemData[] itemDatas) {
		UIShow(true);
		mIndex = index;
		mPickCost = pick;
		mSpendType = type;
		updateView(type);
		mItemDatas = itemDatas;
		spritePay.spriteName = GameFunction.SpendKindTexture(pick.SpendKind);
		RefreshTextColor ();
	}

	public void SetNewSkillCard (Dictionary<int, int> gotItems) {
//		newSkillCard = skill;
		newGetItemCount = gotItems;
	}

	private void updateView (int type) {
		labelPay.text = getLabelPay(type);
	}

	public void RefreshTextColor () {
		labelPay.color = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(mPickCost.SpendKind, getLabelPayCount(mSpendType)), mPickCost.SpendKind);
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
		Invoke("ShowOneNew", 3f);
		UI3DBuyStore.Get.StartRaffle();
	}

	private void showFive(TItemData[] itemDatas) {
		fiveItem.Show(itemDatas);
		fiveItem.ShowAni(true);
		tenItem.ShowAni(false);
		animationBuy.SetTrigger("Five");
		UI3DBuyStore.Get.StartRaffle();
		GameData.Team.GotItemCount = newGetItemCount;
	}

	private void showTen (TItemData[] itemDatas) {
		tenItem.Show(itemDatas);
		tenItem.ShowAni(true);
		fiveItem.ShowAni(false);
		animationBuy.SetTrigger("Ten");
		UI3DBuyStore.Get.StartRaffle();
		GameData.Team.GotItemCount = newGetItemCount;
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
		AudioMgr.Get.PlaySound(SoundType.SD_Instant);
	}

	public void FinishDrawLottery () {
        UIResource.Get.Show();
		animationBuy.SetTrigger("Finish");
		GameData.Team.GotItemCount = newGetItemCount;
	}

	private void reset() {
		oneItem.Reset();
		fiveItem.Reset();
		tenItem.Reset();

        animationBuy.SetTrigger("Again");	
		UI3DBuyStore.Get.AgainRaffle();
		isClickTouch = false;
	}

	public void OnAgain() {
		if(mPickCost.SpendKind == 0) { // Diamond
			if(!CheckDiamond(howMuch(mPickCost, mSpendType), true, string.Format(TextConst.S(252), howMuch(mPickCost, mSpendType)), ConfirmUse, RefreshTextColor))
				AudioMgr.Get.PlaySound (SoundType.SD_Prohibit);
		} else if (mPickCost.SpendKind == 1) { // Money
			if(!CheckMoney(howMuch(mPickCost, mSpendType), true, string.Format(TextConst.S(253), howMuch(mPickCost, mSpendType)), ConfirmUse, RefreshTextColor))
				AudioMgr.Get.PlaySound (SoundType.SD_Prohibit);
		}
	}

	public void ConfirmUse () {
		UIResource.Get.Hide();
		SendPickLottery(mPickCost.Order, mSpendType);
	}

	private int getLabelPayCount (int spendType) {
		if(spendType == EPickSpendType.ONE.GetHashCode()) 
			return mPickCost.OnePick;
		else if(spendType == EPickSpendType.FIVE.GetHashCode()) 
			return mPickCost.FivePick;
		else if(spendType == EPickSpendType.TEN.GetHashCode()) 
			return mPickCost.TenPick;
		return 0;
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
		UIMall.Get.RefreshTextColor();
	}

	//order = 0 can used
	private void SendPickLottery(int order, int type)
	{
		WWWForm form = new WWWForm();
		form.AddField("Order", order);
		form.AddField("Type", type);
		form.AddField("Index", mIndex);
		SendHttp.Get.Command(URLConst.PickLottery, waitPickLottery, form);
	}

	private void waitPickLottery(bool ok, WWW www)
	{
		if(ok)
		{
            TPickLotteryResult result = JsonConvertWrapper.DeserializeObject<TPickLotteryResult>(www.text);
			GameData.Team.Items = result.Items;
			newGetItemCount = result.GotItemCount;
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.Money = result.Money;
			GameData.Team.LotteryFreeTime = result.LotteryFreeTime;
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.MaterialItems = result.MaterialItems;
			GameData.Team.InitSkillCardCount();
			RefreshTextColor ();
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