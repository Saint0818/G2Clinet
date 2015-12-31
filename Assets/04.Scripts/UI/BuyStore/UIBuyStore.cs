using UnityEngine;
using System.Collections;
using GameStruct;
using Newtonsoft.Json;

public struct TPickOneResult {
	public int ItemID;
	public TTeam Team;
}

public struct TPickTenResult {
	public int[] ItemIDs;
	public TTeam Team;
}


public class UIBuyStore : UIBase {
	private static UIBuyStore instance = null;
	private const string UIName = "UIBuyStore";

	private Animator animationBuy;
	private GetOneItem oneItem;
	private GetTenItem tenItem;

	private UILabel labelPay;

	private bool isOneAware = true; 

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
		tenItem = transform.FindChild("Center/ItemGet/GetItem_Ten").GetComponent<GetTenItem>();
		labelPay = GameObject.Find(UIName + "/Center/ItemGet/AgainBt/PayLabel").GetComponent<UILabel>();

		UIEventListener.Get(GameObject.Find(UIName + "/Center/Touch")).onClick = StartDrawLottery;
		SetBtnFun(UIName + "/Center/ItemGet/AgainBt", OnAgain);
		SetBtnFun(UIName + "/Center/ItemGet/EnterBt", OnBack);
	}

	public void ShowView (bool isOne) {
		isOneAware = isOne;
		if(isOne)
			labelPay.text = "30";
		else
			labelPay.text = "250";
		UIShow(true);
	}

	private void showOne (TItemData itemData) {
		oneItem.Show(itemData);
		tenItem.ShowAni(false);
		animationBuy.SetTrigger("One");
		Invoke("FinishDrawLottery", 2.5f);
		UI3DBuyStore.Get.StartRaffle();
	}

	private void showTen (TItemData[] itemDatas) {
		tenItem.Show(itemDatas);
		tenItem.ShowAni(true);
		animationBuy.SetTrigger("Ten");
		UI3DBuyStore.Get.StartRaffle();
	}

	public void StartDrawLottery(GameObject go) {
		oneItem.Reset();
		tenItem.Reset();
		if(isOneAware) {
			SendPickOne();
		}  else {
			SendPickTen();
		}
	}

	public void FinishDrawLottery () {
		animationBuy.SetTrigger("Finish");
	}

	public void OnAgain() {
		if(isOneAware) {
			if(GameData.Team.Diamond <= 30) {
				UIHint.Get.ShowHint(TextConst.S(233), Color.white);
				return;
			}
		} else {
			if(GameData.Team.Diamond <= 250) {
				UIHint.Get.ShowHint(TextConst.S(233), Color.white);
				return;
			}
		}
		animationBuy.SetTrigger("Again");	
		UI3DBuyStore.Get.AgainRaffle();
	}

	public void OnBack () {
		UIShow(false);
		UI3DBuyStore.UIShow(false);
		UIMainLobby.Get.Show();
		UI3DMainLobby.Get.Show();
	}

	private void SendPickOne()
	{
		WWWForm form = new WWWForm();
		SendHttp.Get.Command(URLConst.PickOne, waitPickOne, form);
	}

	private void waitPickOne(bool ok, WWW www)
	{
		if(ok)
		{
			TPickOneResult result = (TPickOneResult)JsonConvert.DeserializeObject(www.text, typeof(TPickOneResult));
			GameData.Team.Items = result.Team.Items;
			GameData.Team.SkillCards = result.Team.SkillCards;
			GameData.Team.Diamond = result.Team.Diamond;

			if(GameData.DItemData.ContainsKey(result.ItemID))
				showOne(GameData.DItemData[result.ItemID]);
			else 
				OnBack();
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.PickOne);
	}

	private void SendPickTen()
	{
		WWWForm form = new WWWForm();
		SendHttp.Get.Command(URLConst.PickTen, waitPickTen, form);
	}

	private void waitPickTen(bool ok, WWW www)
	{
		if(ok)
		{
			TPickTenResult result = (TPickTenResult)JsonConvert.DeserializeObject(www.text, typeof(TPickTenResult));
			GameData.Team.Items = result.Team.Items;
			GameData.Team.SkillCards = result.Team.SkillCards;
			GameData.Team.Diamond = result.Team.Diamond;

			TItemData[] getItemIDs = new TItemData[result.ItemIDs.Length];
			for(int i=0; i<result.ItemIDs.Length; i++) {
				if(GameData.DItemData.ContainsKey(result.ItemIDs[i]))
					getItemIDs[i] = GameData.DItemData[result.ItemIDs[i]];
			}

			showTen(getItemIDs);
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.PickOne);
	}
}