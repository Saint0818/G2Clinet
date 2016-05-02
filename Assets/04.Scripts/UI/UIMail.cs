using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStruct;
using Newtonsoft.Json;


public struct TMail
{
	public int Kind;
	public string Title;
	public string Contents;
	public DateTime Data;
	public bool isRead;
}

public class MailSubPage: MonoBehaviour {
	public GameObject redPoint;
	public GameObject pageObject;
	public bool isActive;

	public List<TMailInfo> MailList = new List<TMailInfo>();

	public virtual void HookUI(string UIName, int i)
	{
			redPoint = GameObject.Find(UIName + "/Window/Center/Group0/Tabs/" + i.ToString() + "/RedPoint");
			pageObject = GameObject.Find(UIName + "/Window/Center/Group0/Pages/" + i.ToString());

			UIBase.SetBtnFun (UIName + "/Window/Center/Group0/Tabs/" + i.ToString (), OnPage);
			redPoint.SetActive(false);
			pageObject.SetActive(false);
	}

	public virtual void OnPage() {
		UIMail.Get.HideAllPage ();
		SetActive (true);
	}

	public virtual void SetActive(bool a){
		isActive = a;
		pageObject.SetActive (a);
	}

	public virtual void ListMail(TMailInfo[] Mails)
	{			
	}
}

public class MailSubPageHtml : MailSubPage {
	public GameObject webViewGameObject;
	private bool loadComplete = false;
	public UniWebView webView;
	public MailSubPageHtml(string UIName, int i){
		HookUI (UIName, i);
	}

	void Destroy(){
		GameObject.Destroy (webViewGameObject);
	}

	public override void HookUI(string UIName, int i)
	{
		base.HookUI (UIName, i);

		webViewGameObject = GameObject.Find("WebView");
		if (webViewGameObject == null)
			webViewGameObject = new GameObject("WebView");

		webView = webViewGameObject.AddComponent<UniWebView>();
		webView.OnLoadComplete += OnLoadComplete;
		webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
		webView.toolBarShow = true;

		string host = "http://nicemarket.com.tw/";
		//string host = "http://localhost:3300/";
		string url = string.Format(host + "notic?game={0}&company={1}&os={2}&language={3}&version={4}", "g2", 
			GameData.Company, GameData.OS, GameData.Setting.Language.ToString(), GameData.SaveVersion);

		Debug.Log (url);
		webView.url = url;
		webView.Load();

	}

	public override void SetActive(bool a){
		base.SetActive (a);
		if (a == false)
			webView.Hide ();
		else {
			webView.Show();
		}
	}

	UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation) {


		float zoomRatioW = (float)UniWebViewHelper.screenWidth / UI2D.Get.RootWidth;
		float zoomRatioH = (float)UniWebViewHelper.screenHeight / UI2D.Get.RootHeight;
		//float zoomRatioW = (float)UniWebViewHelper.screenWidth / Screen.width;
		//float zoomRatioH = (float)UniWebViewHelper.screenHeight / Screen.height;
//		UIWidget webViewWidget;
		int top = (int)(160*zoomRatioH);//- webViewWidget.topAnchor.absolute * zoomRatio;
		int left = (int)(130*zoomRatioW);//webViewWidget.leftAnchor.absolute * zoomRatio;
		int bottom = (int)(70*zoomRatioH);//webViewWidget.bottomAnchor.absolute * zoomRatio;
		int right = (int)(130*zoomRatioW);//- webViewWidget.rightAnchor.absolute * zoomRatio;

		//return new UniWebViewEdgeInsets(top, left, bottom, right);
		return new UniWebViewEdgeInsets(top, left, bottom, right);
	}

	public void OnLoadComplete(UniWebView webView, bool success, string errorMessage) {
		if (success) {
			loadComplete = true;
			if(isActive)
				webView.Show();

		} else {
			Debug.Log("Something wrong in webview loading: " + errorMessage);
		}
	}
		
	public override void OnPage() {
		base.OnPage ();
		if(loadComplete)
			webView.Show();
	}

}
	
public class MailSubPagePrize : MailSubPage {
	private UIPanel pagePanel;
	private UIScrollView pageScrollView;
	public int totalCount;
	public int finishCount;

	public MailSubPagePrize(string UIName, int i){
		HookUI (UIName, i);
	}

	public override void HookUI(string UIName, int i)
	{
		base.HookUI (UIName, i);
		pageScrollView = GameObject.Find(UIName + "/Window/Center/Group0/Pages/" + i.ToString() + "/ScrollView").GetComponent<UIScrollView>();
		pagePanel = GameObject.Find(UIName + "/Window/Center/Group0/Pages/" + i.ToString() + "/ScrollView").GetComponent<UIPanel>();

	}

	public override void SetActive(bool a){
		base.SetActive (a);
	}

	public override void OnPage() {
		base.OnPage ();
		//		if (waitForAnimator)
		//			return;
		//
		//		for (int i = 0; i < pageObjects.Length; i++)
		//			pageObjects[i].SetActive(false);
		//
		//		int index = -1;
		//		if (int.TryParse(UIButton.current.name, out index)) {
		//			pageObjects[index].SetActive(true);
		//			nowPage = index;
		//
		//			initMissionList(index);
		//		}
		SendListMail(1);
	}

	public override void ListMail(TMailInfo[] Mails)
	{
		MailList.Clear ();
		for (int i = 0; i < Mails.Length; i++)
			MailList.Add (Mails [i]);
	}

	private void SendListMail (int mailKind) {
		WWWForm form = new WWWForm();
		form.AddField("Identifier", GameData.Team.Identifier);
		form.AddField("MailKind", 1);// 1=prize, 2=social
		SendHttp.Get.Command(URLConst.ListMail, waitListMail, form);
	}

	private void waitListMail(bool ok, WWW www) {
		if (ok) {
			TMailInfo[] result = JsonConvertWrapper.DeserializeObject <TMailInfo[]>(www.text); 
			//GameData.Team.GymBuild = result.GymBuild;
			ListMail(result);

		} else {
			Debug.LogError("text:"+www.text);
		} 
	}
}

public class MailSubPageSocial : MailSubPage {
	private UIPanel pagePanel;
	private UIScrollView pageScrollView;
	public int totalCount;
	public int finishCount;

	public MailSubPageSocial(string UIName, int i){
		HookUI (UIName, i);
	}
	public override void HookUI(string UIName, int i)
	{
		base.HookUI (UIName, i);
		pageScrollView = GameObject.Find(UIName + "/Window/Center/Group0/Pages/" + i.ToString() + "/ScrollView").GetComponent<UIScrollView>();
		pagePanel = GameObject.Find(UIName + "/Window/Center/Group0/Pages/" + i.ToString() + "/ScrollView").GetComponent<UIPanel>();

	}

	public override void SetActive(bool a){
		base.SetActive (a);
	}

	public override void OnPage() {
		base.OnPage ();
		//		if (waitForAnimator)
		//			return;
		//
		//		for (int i = 0; i < pageObjects.Length; i++)
		//			pageObjects[i].SetActive(false);
		//
		//		int index = -1;
		//		if (int.TryParse(UIButton.current.name, out index)) {
		//			pageObjects[index].SetActive(true);
		//			nowPage = index;
		//
		//			initMissionList(index);
		//		}
		SendListMail(2);
	}

	public override void ListMail(TMailInfo[] Mails)
	{
		MailList.Clear ();
		for (int i = 0; i < Mails.Length; i++)
			MailList.Add (Mails [i]);		
	}

	private void SendListMail (int mailKind) {
		WWWForm form = new WWWForm();
		form.AddField("Identifier", GameData.Team.Identifier);
		form.AddField("MailKind", 2);// 1=prize, 2=social
		SendHttp.Get.Command(URLConst.ListMail, waitListMail, form);
	}

	private void waitListMail(bool ok, WWW www) {
		if (ok) {
			TMailInfo[] result = JsonConvertWrapper.DeserializeObject <TMailInfo[]>(www.text); 
			//GameData.Team.GymBuild = result.GymBuild;
			ListMail(result);

		} else {
			Debug.LogError("text:"+www.text);
		} 
	}
}

public class UIMail : UIBase {
	
	private static UIMail instance = null;
	private const string UIName = "UIMail";

	// ui
	private GameObject itemBuild;// Resource.Load
	private GameObject window;
	private UIButton changeBtn;

	// group stage
	private int nowGroup = 0;

	// group0
	private const int pageNum = 3;
	private int nowPage = 0;
	private MailSubPage[] subPages = new MailSubPage[pageNum];

	// group 1
	private List<TITemGymObj> itemGymObjs = new List<TITemGymObj>();
	private int mItemGymObjIndex = 0;
	private UIScrollView decoScrollView;

	private int mAvatarIndex;
	private bool isRealChange = true; //false表示預覽而已，Back的時候要回復
	//
	void Destroy () {
		itemGymObjs.Clear();
	}
	//
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

		set {
			if (instance) {
				if (value)
					instance.Show (value);
				else {
					MailSubPageHtml tmp = (MailSubPageHtml)(instance.subPages [0]);
					if (tmp.webViewGameObject)
						Destroy(tmp.webViewGameObject);
					RemoveUI (instance.gameObject);
				}
			} else
				if (value)
					Get.Show(value);
		}
	}

	public static UIMail Get
	{
		
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMail;

			return instance;
		}
	}

	public static UIMail dynamicGet
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMail;

			return instance;
		}
	}

	protected override void InitData() {

	}

	protected override void InitCom() {
		window = GameObject.Find(UIName + "/Window");

		// group 0
		subPages [0] = new MailSubPageHtml (UIName, 0);
		subPages [1] = new MailSubPagePrize (UIName, 1);
		subPages [2] = new MailSubPageSocial (UIName, 2);

		SetBtnFun(UIName + "/Window/Center/Group0/Tabs/DailyLoginBtn", OnOpenDailyLogin);
		changeBtn = GameObject.Find(UIName + "/Window/Center/Group0/Tabs/ChangeBtn").GetComponent<UIButton>();
		SetBtnFun(UIName + "/Window/Center/Group0/Tabs/ChangeBtn", OnGotoGroup1);

		// group 1
		itemBuild = Resources.Load(UIPrefabPath.ItemGymEngage) as GameObject;
		decoScrollView = GameObject.Find (UIName + "/Window/Center/Group1/BuildingView/ScrollView").GetComponent<UIScrollView>();

		// BottomLeft
		SetBtnFun(UIName + "/Window/BottomLeft/BackBtn", OnClose);


	}

	private void OnOpenDailyLogin()
	{
		UIDailyLogin.Get.Show ();
	}
		
	private void OnGotoGroup1()
	{
		// 公告藏起來
		if (subPages [0].isActive)
			subPages [0].SetActive (false);
		//
		if (UI3DMainLobby.Visible)
			UI3DMainLobby.Get.Impl.OnSelect (8, true);
		changeBtn.gameObject.SetActive (false);
		nowGroup = 1;
		GetComponent<Animator>().SetTrigger("Group1");
	}

	private void setDecoScrollView () {
		int kind = 51 + 8;
		int index = 0;
		for(int i=0; i<GameData.DBuildData.Count; i++) {
			if(GameData.DBuildData[i].Kind == kind) {
				itemGymObjs.Add(addItems(index, GameData.DBuildData[i]));
				index ++;
			}
		}

	}

	private TITemGymObj addItems (int index, TItemData data) {
		bool isSelect = false;
		if(8 >= 0 && 8 < GameData.Team.GymBuild.Length)
			isSelect = (GameData.Team.GymBuild[8].ItemID == data.ID);

		if(isSelect)
			mAvatarIndex = data.Avatar;

		GameObject go = Instantiate(itemBuild) as GameObject;
		go.transform.parent = decoScrollView.gameObject.transform;
		go.transform.localPosition = new Vector3(170 * index, 0, 0);
		TITemGymObj obj = new TITemGymObj();
		obj.Init(go, GameFunction.GetBuildEnName(8));
		obj.UpdateUI(index, data, isSelect, GameData.Team.IsGymOwn(data.ID));
		UIEventListener.Get(go).onClick = OnChooseBuildType;
		UIEventListener.Get(obj.Buy).onClick = OnBuyBuildType;
		return obj;
	}

	public void OnChooseBuildType (GameObject go) {
		int result = 0;
		//itemObj index
		if(int.TryParse(go.name, out result)) {
			if(result >= 0 && result < itemGymObjs.Count) {
				if(!itemGymObjs[result].IsSelect) {
					mItemGymObjIndex = result;
					if(GameData.DItemData.ContainsKey(itemGymObjs[result].ItemID)) {
						if(itemGymObjs[result].IsBuy) {
							int itemIndex = GameData.GetBuildItemIndex(itemGymObjs[mItemGymObjIndex].ItemID);
							if(itemIndex != -1) {
								isRealChange = true;
								SendChangeBuildType(itemIndex);
							}
						} else {
							isRealChange = false;
						}
						UI3DMainLobby.Get.Impl.ReplaceObj(8, GameData.DItemData[itemGymObjs[result].ItemID].Avatar);
						refreshSelectBuild(mItemGymObjIndex);
					}
				}
			}
		}
	}

	public void OnBuyBuildType (GameObject go) {
		int result = 0;
		//itemObj index
		if(int.TryParse(go.name, out result)) {
			if(result >=0 && result < itemGymObjs.Count && GameData.DItemData.ContainsKey(itemGymObjs[result].ItemID)) {
				mItemGymObjIndex = result;
				CheckDiamond(GameData.DItemData[itemGymObjs[result].ItemID].Buy, true, string.Format(TextConst.S(11016),GameData.DItemData[itemGymObjs[result].ItemID].Buy, GameData.DItemData[itemGymObjs[result].ItemID].Name), ConfirmBuyBuildType, refreshLabelColor);
			}
		}
	}

	public void ConfirmBuyBuildType () {
		int itemIndex = GameData.GetBuildItemIndex(itemGymObjs[mItemGymObjIndex].ItemID);
		if(itemIndex != -1) 
			SendBuyBuildType(itemIndex);
	}

	private void refreshSelectBuild (int gymObjIndex) {
		for(int i=0; i<itemGymObjs.Count; i++) 
			itemGymObjs[i].SelectBuild((i == gymObjIndex));

	}


	private void refreshLabelColor () {
		//labelUpgradePrice.color = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(architectureValue.SpendKind, architectureValue.Cost), architectureValue.SpendKind);
		for(int i=0; i<itemGymObjs.Count; i++) {
			itemGymObjs[i].RefreshColor ();
		}
	}

	private void OnGotoGroup0()
	{
		if (UI3DMainLobby.Visible) {
			UI3DMainLobby.Get.Impl.OnSelect (8, true);
			//UIMainLobby.Get.View.PlayExitAnimation();
		}
		changeBtn.gameObject.SetActive (true);
		nowGroup = 0;	
		subPages [nowPage].OnPage ();
		GetComponent<Animator>().SetTrigger("Group0");
	}

	private void OnExitGroup0()
	{
		window.SetActive (false);
		UIMainLobby.Get.View.PlayEnterAnimation();
		UIGym.Get.CenterVisible = true;
		Visible = false;
	}

	private void OnExitGroup1()
	{
		if(!isRealChange) 
			UI3DMainLobby.Get.Impl.ReplaceObj(8, mAvatarIndex);
		
	}

	private void OnClose()
	{

		switch(nowGroup) {
		case 0:
			OnExitGroup0 ();

			break;

		case 1:
			OnExitGroup1 ();
			OnGotoGroup0 ();
			break;

		default:
			break;
		}

	}

	public void HideAllPage(){
		for (int i = 0; i < subPages.Length; i++) {
			subPages[i].SetActive(false);

		}
	}
	protected override void OnShow(bool isShow) {
		base.OnShow(isShow);
		if (isShow) {
			//initRedPoint(i);
			subPages [nowPage].OnPage ();
			setDecoScrollView ();

		}
	}

	private int getIdleIndex {
		get {
			for(int i=0; i<GameData.Team.GymQueue.Length; i++) 
				if(GameData.Team.GymQueue[i].IsOpen && GameData.Team.GymQueue[i].BuildIndex == -1)
					return i;

			return -1;
		}
	}

	private int getNowCDIndex (int buildIndex) {
		for(int i=0; i<GameData.Team.GymQueue.Length; i++) 
			if(GameData.Team.GymQueue[i].IsOpen && GameData.Team.GymQueue[i].BuildIndex == buildIndex)
				return i;

		return -1;
	}

	private void SendUpdateBuild () {
		WWWForm form = new WWWForm();
		form.AddField("Index", getIdleIndex);
		form.AddField("BuildIndex", 8);
		SendHttp.Get.Command(URLConst.GymUpdateBuild, waitUpdateBuild, form);
	}

	private void waitUpdateBuild(bool ok, WWW www) {
		if (ok) {
            TGymResult result = JsonConvertWrapper.DeserializeObject <TGymResult>(www.text); 
			GameData.Team.Money = result.Money;
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.GymBuild = result.GymBuild;
			GameData.Team.GymQueue = result.GymQueue;

			UIMainLobby.Get.UpdateUI();
			UIMainLobby.Get.RefreshQueue();
			//RefreshUI();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}

//	private void SendBuyBuildCD () {
//		WWWForm form = new WWWForm();
//		form.AddField("Index", getNowCDIndex(8));
//		form.AddField("BuildIndex", 8);
//		SendHttp.Get.Command(URLConst.GymBuyCD, waitBuyBuildCD, form);
//	}
//
//	private void waitBuyBuildCD(bool ok, WWW www) {
//		if (ok) {
//			TGymResult result = JsonConvert.DeserializeObject <TGymResult>(www.text, SendHttp.Get.JsonSetting); 
//			GameData.Team.Diamond = result.Diamond;
//			GameData.Team.GymBuild = result.GymBuild;
//			GameData.Team.GymQueue = result.GymQueue;
//
//			UIMainLobby.Get.UpdateUI();
//			UIMainLobby.Get.RefreshQueue();
//			RefreshUI();
//		} else {
//			Debug.LogError("text:"+www.text);
//		} 
//	}

	private void SendBuyBuildType (int itemIndex) {
		WWWForm form = new WWWForm();
		form.AddField("Index", itemIndex);//itemData Array index
		SendHttp.Get.Command(URLConst.GymBuyType, waitBuyBuildType, form);
	}

	private void waitBuyBuildType(bool ok, WWW www) {
		if (ok) {
            TGymBuildResult result = JsonConvertWrapper.DeserializeObject <TGymBuildResult>(www.text); 
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.GymOwn = result.GymOwn;

			UIMainLobby.Get.UpdateUI();
			if(mItemGymObjIndex >= 0 && mItemGymObjIndex < itemGymObjs.Count)
				itemGymObjs[mItemGymObjIndex].RefreshUI();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}

	private void SendChangeBuildType (int itemIndex) {
		WWWForm form = new WWWForm();
		form.AddField("Index", itemIndex);//itemData Array index
		form.AddField("BuildIndex", 8);
		SendHttp.Get.Command(URLConst.GymChangeBuildType, waitChangeBuildType, form);
	}

	private void waitChangeBuildType(bool ok, WWW www) {
		if (ok) {
            TGymBuildResult result = JsonConvertWrapper.DeserializeObject <TGymBuildResult>(www.text); 
			GameData.Team.GymBuild = result.GymBuild;
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}

}
