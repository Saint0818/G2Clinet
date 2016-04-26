using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TMailItem
{
	public int Index;
	public UILabel Head;
	public UILabel Body;
	public UILabel Data;

	public GameObject gameobject;
	public Transform DisablePool;
	public Transform EnablePool;

	private bool isEnable;

	public bool Enable
	{
		set{
			isEnable = value;

			if(gameobject){
				gameobject.SetActive(value);
				gameobject.transform.parent = gameobject.activeSelf? EnablePool : DisablePool;
			}
		}
		get{return isEnable;}
	}
}

public struct TMail
{
	public int Kind;
	public string Title;
	public string Contents;
	public DateTime Data;
	public bool isRead;
}

public class MailSubPage {
	public GameObject redPoint;
	public GameObject pageObject;

	public virtual void HookUI(string UIName, int i)
	{
			redPoint = GameObject.Find(UIName + "/Window/Center/Group0/Tabs/" + i.ToString() + "/RedPoint");
			pageObject = GameObject.Find(UIName + "/Window/Center/Group0/Pages/" + i.ToString());

			UIBase.SetBtnFun (UIName + "/Window/Center/Tabs/" + i.ToString (), OnPage);
			redPoint.SetActive(false);
			pageObject.SetActive(false);
	}

	public virtual void OnPage() {
	}

	public void SetActive(bool a){
		pageObject.SetActive (a);
	}
}

public class MailSubPageHtml : MailSubPage {

	public MailSubPageHtml(string UIName, int i){
		HookUI (UIName, i);
	}

	public override void HookUI(string UIName, int i)
	{
		base.HookUI (UIName, i);

	}

	public override void OnPage() {
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

	public override void OnPage() {
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

	public override void OnPage() {
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
	}
}

public class UIMail : UIBase {
	
	private static UIMail instance = null;
	private const string UIName = "UIMail";

	private int nowGroup = 0;
	//ui
	private GameObject window;
	private UIButton changeBtn;

	private const int pageNum = 3;
	private int nowPage = 1;
	private MailSubPage[] subPages = new MailSubPage[pageNum];

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
					instance.Show(value);
				else
					RemoveUI(instance.gameObject);
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

		// BottomLeft
		SetBtnFun(UIName + "/Window/BottomLeft/BackBtn", OnClose);


	}

	private void OnOpenDailyLogin()
	{
		UIDailyLogin.Get.Show ();
	}
		
	private void OnGotoGroup1()
	{
		if (UI3DMainLobby.Visible)
			UI3DMainLobby.Get.Impl.OnSelect (8);
		changeBtn.gameObject.SetActive (false);
		nowGroup = 1;
		
	}

	private void OnGotoGroup0()
	{
		if (UI3DMainLobby.Visible) {
			UI3DMainLobby.Get.Impl.OnSelect (8);
			UIMainLobby.Get.View.PlayExitAnimation();
		}
		changeBtn.gameObject.SetActive (true);
		nowGroup = 0;	
	}

	private void OnClose()
	{
		switch(nowGroup) {
		case 0:
			window.SetActive (false);
			UIMainLobby.Get.View.PlayEnterAnimation();
			UIGym.Get.CenterVisible = true;
			Visible = false;
			break;

		case 1:
			OnGotoGroup0 ();
			break;

		default:
			break;
		}

	}

	protected override void OnShow(bool isShow) {
		base.OnShow(isShow);
		if (isShow) {
			for (int i = 0; i < subPages.Length; i++) {
				subPages[i].SetActive(false);
				//initRedPoint(i);
			}

			subPages [nowPage].OnPage ();

		}
	}

}
