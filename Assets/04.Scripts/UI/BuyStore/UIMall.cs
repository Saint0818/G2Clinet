using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;

public class UIMall : UIBase {
	private static UIMall instance = null;
	private const string UIName = "UIMall";

	private List<TMallBox> mallBoxs ;

	private TPickCost choosePickCost;
	private int spendType;

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
//		if()
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
			TMallBox mallBox = new TMallBox();
			GameObject prefab = Instantiate(Resources.Load("Prefab/UI/Items/" + GameData.DPickCost[i].Prefab)) as GameObject;
			setParentInit(prefab, table);
			mallBox.Init(prefab, new EventDelegate(OnOneBtn), new EventDelegate(OnFiveBtn), new EventDelegate(OnTenBtn));
			mallBox.UpdateView(i, GameData.DPickCost[i]);
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

			mallBoxs.Add(mallBox);
		}
	}

	private void setParentInit (GameObject obj, GameObject parent) {
		obj.transform.parent = parent.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localScale = Vector3.one;
	}

	/*
	0.台幣
	1.鑽石
	2.遊戲幣
	3.聯盟幣
	4.社群幣
	*/
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

	public void OnOneBtn () {
		int result = 0;
		if(int.TryParse(UIButton.current.name, out result)) {
			choosePickCost = mallBoxs[result].mPickCost;
			spendType = EPickSpendType.ONE.GetHashCode();
			SendPickLottery(choosePickCost.Order, choosePickCost.Kind, spendType);
		}
	}

	public void OnFiveBtn () {
		int result = 0;
		if(int.TryParse(UIButton.current.name, out result)) {
			choosePickCost = mallBoxs[result].mPickCost;
			spendType = EPickSpendType.FIVE.GetHashCode();
			SendPickLottery(choosePickCost.Order, choosePickCost.Kind, spendType);
		}
	}

	public void OnTenBtn () {
		int result = 0;
		if(int.TryParse(UIButton.current.name, out result)) {
			choosePickCost = mallBoxs[result].mPickCost;
			spendType = EPickSpendType.TEN.GetHashCode();
			SendPickLottery(choosePickCost.Order, choosePickCost.Kind, spendType);
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
				UIItemHint.Get.OnShow(GameData.DItemData[result]);
		}
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
				TItemData[] getItemIDs = new TItemData[result.ItemIDs.Length];
				for(int i=0; i<result.ItemIDs.Length; i++) {
					if(GameData.DItemData.ContainsKey(result.ItemIDs[i]))
						getItemIDs[i] = GameData.DItemData[result.ItemIDs[i]];
				}
				OpenLottery(getItemIDs);
			}
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.PickLottery);
	}

	public void OpenLottery (TItemData[] itemDatas) {
		if(checkCost(choosePickCost, spendType)) {
			UIMainLobby.Get.HideAll();
			UI3DMainLobby.Get.Hide();
			Hide ();
			UIBuyStore.Get.ShowView(choosePickCost, spendType, itemDatas);
			UI3DBuyStore.Get.Show();
		} else 
			UIHint.Get.ShowHint(TextConst.S(233), Color.red);
	}
}
