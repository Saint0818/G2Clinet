using UnityEngine;
using System.Collections;
using GameStruct;
using Newtonsoft.Json;

public struct TPickLotteryResult {
	public int[] ItemIDs;
	public TTeam Team;
}

//public struct TPickTenResult {
//	public int[] ItemIDs;
//	public TTeam Team;
//}


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
		Invoke("FinishDrawLottery", 3.3f);
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
			SendPickLottery(1);
		}  else {
			SendPickLottery(2);
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

	private void SendPickLottery(int kind)
	{
		WWWForm form = new WWWForm();
		form.AddField("Kind", kind);
		SendHttp.Get.Command(URLConst.PickLottery, waitPickLottery, form);
	}

	private void waitPickLottery(bool ok, WWW www)
	{
		if(ok)
		{
			TPickLotteryResult result = (TPickLotteryResult)JsonConvert.DeserializeObject(www.text, typeof(TPickLotteryResult));
			GameData.Team.Items = result.Team.Items;
			GameData.Team.SkillCards = result.Team.SkillCards;
			GameData.Team.Diamond = result.Team.Diamond;

			if(result.ItemIDs.Length == 1) {
				if(GameData.DItemData.ContainsKey(result.ItemIDs[0]))
					showOne(GameData.DItemData[result.ItemIDs[0]]);
				else 
					OnBack();
			} else if(result.ItemIDs.Length == 10) {
				TItemData[] getItemIDs = new TItemData[result.ItemIDs.Length];
				for(int i=0; i<result.ItemIDs.Length; i++) {
					if(GameData.DItemData.ContainsKey(result.ItemIDs[i]))
						getItemIDs[i] = GameData.DItemData[result.ItemIDs[i]];
				}
	
				showTen(getItemIDs);
			}
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.PickLottery);
	}
}