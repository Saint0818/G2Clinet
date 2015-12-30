using UnityEngine;
using System.Collections;
using GameStruct;

public class UIBuyStore : UIBase {
	private static UIBuyStore instance = null;
	private const string UIName = "UIBuyStore";

	private Animator animationBuy;
	private GetOneItem oneItem;
	private GetTenItem tenItem;

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

		UIEventListener.Get(GameObject.Find(UIName + "/Center/Touch")).onClick = StartDrawLottery;
		SetBtnFun(UIName + "/Center/ItemGet/AgainBt", OnAgain);
		SetBtnFun(UIName + "/Center/ItemGet/EnterBt", OnBack);
	}

	public void Show () {
		UIShow(true);
	}

	private void showOne (TItemData itemData) {
		isOneAware = true;
		oneItem.Show(itemData);
		tenItem.ShowAni(false);
	}

	private void showTen (TItemData[] itemDatas) {
		isOneAware = false;
		tenItem.Show(itemDatas);
		tenItem.ShowAni(true);
	}

	private void showitem () {
		oneItem.Reset();
		tenItem.Reset();
		int ran = Random.Range(0,2);
		if(ran == 1) {
			showOne(GameData.DItemData[21100]);
		}  else {
			TItemData[] itemdatas = new TItemData[10];
			for(int i=0; i<itemdatas.Length; i++) {
				itemdatas[i] = GameData.DItemData[21100];
			}
			showTen(itemdatas);
		}
	}

	public void StartDrawLottery(GameObject go) {
		showitem ();
		if(isOneAware) {
			animationBuy.SetTrigger("One");
			Invoke("FinishDrawLottery", 2.5f);
		} else {
			animationBuy.SetTrigger("Ten");
		}
		UI3DBuyStore.Get.StartRaffle();
	}

	public void FinishDrawLottery () {
		animationBuy.SetTrigger("Finish");
	}

	public void OnAgain() {
		animationBuy.SetTrigger("Again");	
		UI3DBuyStore.Get.AgainRaffle();
	}

	public void OnBack () {
		UIShow(false);
		UI3DBuyStore.UIShow(false);
		UIMainLobby.Get.Show();
		UI3DMainLobby.Get.Show();
	}
}