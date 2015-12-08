using System;
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

public class UIMail : UIBase {
	/*
	private static UIMail instance = null;
	private static int mailKind = 0;
	private const string UIName = "UIMail";
	private Transform disableGroup;
	private Transform enablePool;
	private UIScrollView scrollView;
	private GameObject mailContents;
	private UILabel mailContentsHead;
	private UILabel mailContentsBody;
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UIMail Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMail;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow, int kind = 0){
		SetKind(kind);

		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else{
				instance.Show(isShow);
			}
		} else
			if (isShow)
				Get.Show(isShow);
	}

	protected override void InitCom() {

		SetBtnFun (UIName + "/MainView/BottomLeft/BackBtn", OnReturn);

		for (int i = 0; i < 3; i++) {
			string path = string.Format("UIMail/MainView/Center/TypeButton/MailBtn{0}", i + 1);
			SetBtnFun (path, OnMailKind);
			GameObject go = GameObject.Find(path);
			go.name = i.ToString();
		}

		scrollView = GameObject.Find (UIName + "/MainView/Center/MailList/ListView").GetComponent<UIScrollView>();
		enablePool = GameObject.Find (UIName + "/MainView/Left/ItemList").transform;
		GameObject disableObj = new GameObject();
		disableObj = new GameObject ();
		disableObj.name = "disableGroup";
		disableGroup = disableObj.transform;
		disableGroup.transform.parent = scrollView.transform;

		mailContents = GameObject.Find (UIName + "/MainView/Center/MailList/MailContents");
		SetBtnFun (UIName + "/MainView/Center/MailList/MailContents/Remove", OnRemove);
		SetBtnFun (UIName + "/MainView/Center/MailList/MailContents/Get", OnGet);


		if (mailContents) {
			mailContentsHead = mailContents.transform.FindChild("MailTitleLabel").gameObject.GetComponent<UILabel>();	
			mailContentsBody = mailContents.transform.FindChild("ContentsView/ContentsArea/ContentsLabel").gameObject.GetComponent<UILabel>();	
		}

		mailContents.SetActive (false);
	}

	private void OnReturn()
	{
	  UIShow (false);
	}

	private void OnRemove()
	{
		//select mail
	}

	private void OnGet()
	{
		//select mail
	}

	private void OnMailKind()
	{
		int index;
		if (int.TryParse (UIButton.current.name, out index)) {
			UpdateGroup(index);
		}
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		if (isShow) {
			UpdateGroup(mailKind);	
		}
	}

	public static void SetKind(int kind)
	{
		mailKind = kind;
	}

	public void UpdateGroup(int kind)
	{
		for (int i = 0; i < GameData.Team.Mails.Length; i++) {
		}
	}

	public void OnSeletMail()
	{
//		mailContents
	}
	*/
}
