using UnityEngine;
using System;
using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;

public class UIMall : UIBase {
	private static UIMall instance = null;
	private const string UIName = "UIMall";

	private List<TMallBox> mallBoxs ;

	private TPickCost choosePickCost;
	private int spendType;
	private int chooseIndex;

	private GameObject table;
	private GameObject skillCard;
	private GameObject itemIcon;

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
			if (isShow)
				Get.Show(isShow);	
	}

	public static UIMall Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMall;

			return instance;
		}
	}

	public void Hide () {
		if (instance) 
			instance.Show(false);
		else
			Get.Show(false);
	}

	void FixedUpdate () {
		for(int i=0; i<mallBoxs.Count; i++) {
			mallBoxs[i].UpdateFreeTimeCD();
		}
	}

	protected override void InitCom() {
		skillCard = Resources.Load(UIPrefabPath.ItemSkillCard) as GameObject;
		itemIcon = Resources.Load(UIPrefabPath.ItemAwardGroup) as GameObject;
		table = GameObject.Find(UIName + "/Center/Window/ScrollView/Table");
		SetBtnFun(UIName + "/BottomLeft/BackBtn", OnClose);
	}

	public void Show () {//420
		UIShow(true);
		mallBoxs = new List<TMallBox>();
		for (int i=0; i<GameData.DPickCost.Length; i++) {
			if(IsStart(GameData.DPickCost[i]) && !IsExpired(GameData.DPickCost[i])) {
				TMallBox mallBox = new TMallBox();
				GameObject prefab = Instantiate(Resources.Load("Prefab/UI/Items/" + GameData.DPickCost[i].Prefab)) as GameObject;
				setParentInit(prefab, table);
				mallBox.Init(prefab, new EventDelegate(OnOneBtn), new EventDelegate(OnFiveBtn), new EventDelegate(OnTenBtn));
				mallBox.UpdateView(GameData.DPickCost[i].Order, GameData.DPickCost[i]);
				if(GameData.DPickCost[i].ShowCard != null && GameData.DPickCost[i].ShowCard.Length > 0) {
					for(int j=0; j<GameData.DPickCost[i].ShowCard.Length; j++) {
						if(GameData.DItemData.ContainsKey(GameData.DPickCost[i].ShowCard[j])) {
							TActiveSkillCard activeSkillCard = new TActiveSkillCard();
							GameObject obj = Instantiate(skillCard) as GameObject;
							setParentInit(obj, mallBox.DiskScrollView);
							obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
							activeSkillCard.Init(obj,new EventDelegate(ShowSkillCardHint), false);
							activeSkillCard.UpdateViewItemData(GameData.DItemData[GameData.DPickCost[i].ShowCard[j]]);
							mallBox.UpdataCards(j, activeSkillCard.MySkillCard);
						}
					}
				}
				if(GameData.DPickCost[i].ShowItem != null && GameData.DPickCost[i].ShowItem.Length > 0) {
					for(int j=0; j<GameData.DPickCost[i].ShowItem.Length; j++) {
						if(GameData.DItemData.ContainsKey(GameData.DPickCost[i].ShowItem[j])) {
							ItemAwardGroup item = (Instantiate(itemIcon) as GameObject ).GetComponent<ItemAwardGroup>();
							setParentInit(item.gameObject, mallBox.ItemScrollView);
							item.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 1);
							item.gameObject.transform.localPosition = new Vector3(150 * j, 0, 0);
							item.Show(GameData.DItemData[GameData.DPickCost[i].ShowItem[j]]);
						}
					}
				}
				mallBox.SetIndex(i);
				mallBoxs.Add(mallBox);
			}
		}
	}

	private void setParentInit (GameObject obj, GameObject parent) {
		obj.transform.parent = parent.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localScale = Vector3.one;
	}

	private bool IsStart (TPickCost pickCost) {
		if(pickCost.StartTimeYear == 0 || pickCost.StartTimeMonth == 0 || pickCost.StartTimeDay == 0)
			return true;
		
		DateTime startTime = new DateTime(pickCost.StartTimeYear, pickCost.StartTimeMonth, pickCost.StartTimeDay);
		if(DateTime.UtcNow > startTime)
			return true;
		
		return false;
	}

	private bool IsExpired (TPickCost pickCost) {
		if(pickCost.FinishTimeYear == 0 || pickCost.FinishTimeMonth == 0 || pickCost.FinishTimeDay == 0) {
			return false;
		}

		DateTime finishTime = new DateTime(pickCost.FinishTimeYear, pickCost.FinishTimeMonth, pickCost.FinishTimeDay);
		if(DateTime.UtcNow > finishTime)
			return true;
		
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

	public void OnOneBtn () {
		int result = 0;
		if(int.TryParse(UIButton.current.name, out result)) {
			choosePickCost = mallBoxs[result].mPickCost;
			chooseIndex = mallBoxs[result].mIndex;
			spendType = EPickSpendType.ONE.GetHashCode();
			if(mallBoxs[result].IsPickFree)
				CheckDiamond(choosePickCost.OnePick, true, TextConst.S(4108), ConfirmUse);
			else 
				CheckDiamond(choosePickCost.OnePick, true, string.Format(TextConst.S(252), choosePickCost.OnePick), ConfirmUse);
		}
	}

	public void ConfirmUse () {
		SendPickLottery(choosePickCost.Order, spendType);
	}

	public void OnFiveBtn () {
		int result = 0;
		if(int.TryParse(UIButton.current.name, out result)) {
			choosePickCost = mallBoxs[result].mPickCost;
			spendType = EPickSpendType.FIVE.GetHashCode();
			CheckDiamond(choosePickCost.FivePick, true, string.Format(TextConst.S(252) , choosePickCost.FivePick), ConfirmUse);
		}
	}

	public void OnTenBtn () {
		int result = 0;
		if(int.TryParse(UIButton.current.name, out result)) {
			choosePickCost = mallBoxs[result].mPickCost;
			spendType = EPickSpendType.TEN.GetHashCode();
			CheckDiamond(choosePickCost.TenPick, true, string.Format(TextConst.S(252) , choosePickCost.TenPick), ConfirmUse);
		}
	}

	public void OnClose () {
		UIShow(false);
		UIMainLobby.Get.Show();
	}

	public void ShowSkillCardHint () {
		int result = -1;
		if(int.TryParse(UIButton.current.name,out result)) {
			if(GameData.DItemData.ContainsKey(result))
				UIItemHint.Get.OnShow(GameData.DItemData[result].ID);
		}
	}
	//order = 0 can used
	private void SendPickLottery(int order, int type)
	{
		WWWForm form = new WWWForm();
		form.AddField("Order", order);
		form.AddField("Type", type);
		form.AddField("Index", chooseIndex);
		SendHttp.Get.Command(URLConst.PickLottery, waitPickLottery, form);
	}

	private void waitPickLottery(bool ok, WWW www)
	{
		if(ok)
		{
			TPickLotteryResult result = (TPickLotteryResult)JsonConvert.DeserializeObject(www.text, typeof(TPickLotteryResult));
			GameData.Team.Items = result.Items;
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.Money = result.Money;
			GameData.Team.LotteryFreeTime = result.LotteryFreeTime;
			UIMainLobby.Get.UpdateUI();
			GameData.Team.InitSkillCardCount();

			if(result.ItemIDs != null) {
				TItemData[] getItemIDs = new TItemData[result.ItemIDs.Length];
				for(int i=0; i<result.ItemIDs.Length; i++) {
					if(GameData.DItemData.ContainsKey(result.ItemIDs[i]))
						getItemIDs[i] = GameData.DItemData[result.ItemIDs[i]];
				}
				OpenLottery(getItemIDs);
				for(int i=0; i<mallBoxs.Count; i++) {
					mallBoxs[i].Refresh();
				}
			}
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.PickLottery);
	}

	public void OpenLottery (TItemData[] itemDatas) {
		UIMainLobby.Get.HideAll();
		UI3DMainLobby.Get.Hide();
		Hide ();
		UIBuyStore.Get.ShowView(choosePickCost, chooseIndex, spendType, itemDatas);
		UI3DBuyStore.Get.Show();
	}
}
