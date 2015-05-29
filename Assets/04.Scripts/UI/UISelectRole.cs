using UnityEngine;
using System.Collections;
using GameStruct;
using System;
using System.Collections.Generic;

public class UISelectRole : UIBase {
	private static UISelectRole instance = null;
	private const string UIName = "UISelectRole";
	public GameObject PlayerInfoModel = null;

	private TAvatar[]  AvatarAy;
	private Vector3 [] Ay = new Vector3[3];
	private GameObject Select;
	private GameObject[] BtnAy = new GameObject[6];
	private GameObject RoleInfo;
	private TPlayer [] PlayerAy = new TPlayer[3];
	private GameObject [] PlayerObjAy = new GameObject[3];
	private int [] SelectIDAy = new int[3];
	private GameObject [] SelectLogoAy = new GameObject[3];

	private GameObject InfoRange;

	public static bool Visible
	{
		get
		{
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
		}
		else
			if (isShow)
				Get.Show(isShow);
	}
	
	public static UISelectRole Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISelectRole;
			
			return instance;
		}
	}

	protected override void InitCom() 
	{
		PlayerInfoModel = new GameObject();
		PlayerInfoModel.name = "PlayerInfoModel";
		Ay [0] = new Vector3 (0, 0, 0);
		Ay [1] = new Vector3 (3, 0, 0);
		Ay [2] = new Vector3 (-3, 0, 0);
		Select = GameObject.Find (UIName + "/Left/Select");

		for (int i = 0; i < 6; i++) 
		{
			SetBtnFun(UIName + "/Left/SelectCharacter/Button" + i.ToString(), SelectRole);
			BtnAy[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString());
		}

		RoleInfo = GameObject.Find (UIName + "/Center/TouchInfo");
		InfoRange = GameObject.Find (UIName + "/Right/InfoRange");
		SelectLogoAy [1] = GameObject.Find (UIName + "/Center/SelectA");
		SelectLogoAy [2] = GameObject.Find (UIName + "/Center/SelectB");

		UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 25.3f));
		UITriangle.Get.ChangeValue (0, 0.8f);
		UITriangle.Get.ChangeValue (1, 0.7f);
		UITriangle.Get.ChangeValue (2, 0.6f);
		UITriangle.Get.ChangeValue (3, 0.5f);
		UITriangle.Get.ChangeValue (4, 0.4f);
		UITriangle.Get.ChangeValue (5, 0.3f);
//		UITriangle.Get.Triangle.SetActive (false);
//		InfoRange.SetActive (false);
	}
	
	public void SelectRole()
	{
		int Index;
		if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out Index))
		{
			Select.transform.localPosition = new Vector3(BtnAy[Index].transform.localPosition.x, 
			                                             BtnAy[Index].transform.localPosition.y,
			                                             BtnAy[Index].transform.localPosition.z);

			SetPlayerAvatar(0, Index);
		}
	}

	private void SetPlayerAvatar(int RoleIndex, int Index)
	{
		PlayerAy[0].ID = Index + 1;
		SelectIDAy[0] = Index + 1;
		PlayerAy[0].SetAvatar();
		AvatarAy[0] = PlayerAy[0].Avatar;
		ModelManager.Get.SetAvatar(ref PlayerObjAy[0], PlayerAy[0].Avatar, false);
		ChangeLayersRecursively(PlayerObjAy[0].transform, "UI");
	}

	private void CreateAvatar(int Index)
	{
		if(Index >= 0 && Index < Ay.Length)
		{
			if(PlayerObjAy[Index] == null)
			{
				PlayerObjAy[Index] = new GameObject();

			}
		}
	}
	
	protected override void InitData() 
	{
		AvatarAy = new GameStruct.TAvatar[Ay.Length];
		SelectIDAy [0] = 1;
		for(int i = 0; i < Ay.Length; i++) 
		{
			PlayerObjAy[i] = new GameObject();
			PlayerAy[i] = new TPlayer();
			PlayerAy[i].ID = i + 1;
			PlayerAy[i].SetAvatar();
			PlayerObjAy[i].name = i.ToString();
			PlayerObjAy[i].transform.parent = PlayerInfoModel.transform;
			AvatarAy[i] = PlayerAy[i].Avatar;
			ModelManager.Get.SetAvatar(ref PlayerObjAy[i], PlayerAy[i].Avatar, false);
			PlayerObjAy[i].transform.localPosition = Ay[i];
			if(i == 0)
				PlayerObjAy[i].transform.localPosition = new Vector3(0, -0.65f, 0);
			PlayerObjAy[i].transform.localEulerAngles = new Vector3(0, 180, 0);
			PlayerObjAy[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

			ChangeLayersRecursively(PlayerObjAy[i].transform, "UI");
			for (int j = 0; j <PlayerObjAy[i].transform.childCount; j++) 
			{ 
				if(PlayerObjAy[i].transform.GetChild(j).name.Contains("PlayerMode")) 
				{
					PlayerObjAy[i].transform.GetChild(j).localScale = Vector3.one;
					PlayerObjAy[i].transform.GetChild(j).localEulerAngles = Vector3.zero;
					PlayerObjAy[i].transform.GetChild(j).localPosition = Vector3.zero;
				}
			}
		}

		for(int i = 1; i < Ay.Length; i++) 
		{
			PlayerObjAy[i].SetActive(false);
			SelectLogoAy[i].SetActive(false);
		}
	}

	private void ChangeLayersRecursively(Transform trans, string name)
	{
		trans.gameObject.layer = LayerMask.NameToLayer(name);
		foreach(Transform child in trans)
		{            
			ChangeLayersRecursively(child, name);
		}
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	void FixedUpdate()
	{
	
	}
}

