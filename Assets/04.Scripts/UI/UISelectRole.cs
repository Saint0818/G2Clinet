﻿using UnityEngine;
using System.Collections;
using GameStruct;
using System;
using System.Collections.Generic;
using DG.Tweening; 

public class UISelectRole : UIBase {
	private static UISelectRole instance = null;
	private const string UIName = "UISelectRole";
	public GameObject PlayerInfoModel = null;
	public float forGoTime = 1;
	private float GoTime = 0;
	private TAvatar[]  AvatarAy;
	private Vector3 [] Ay = new Vector3[3];
	private GameObject Select;
	private GameObject[] BtnAy = new GameObject[6];
	private GameObject RoleInfo;
	private TPlayer [] PlayerAy = new TPlayer[3];
	private GameObject [] PlayerObjAy = new GameObject[3];
	private int [] SelectIDAy = new int[3];
	private GameObject ViewLoading;
	private GameObject OkBtn;
	private GameObject InfoRange;
	private GameObject Left;
	private GameObject CharacterInfo;
	private UILabel CharacterInfoLabel;
	private int MaxValue = 100;
	private float Value = 0;
	private float axisX;
	private UILabel PlayerName;
	private GameObject PlayerNameObj;
	private UILabel PlayerBody;
	private GameObject PlayerBodyObj;
	private UILabel [] SelectABName = new UILabel[2];
	private UILabel [] SelectABBody = new UILabel[2];
	public static int [] RoleIDAy = new int[6]{14, 24, 34, 19, 29, 39};  // playerID

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

		SetBtnFun (UIName + "/Right/CharacterCheck", DoSelectRole);
		Left = GameObject.Find (UIName + "/Left");
		OkBtn = GameObject.Find (UIName + "/Right/CharacterCheck");
		RoleInfo = GameObject.Find (UIName + "/Center/TouchInfo");
		InfoRange = GameObject.Find (UIName + "/Right/InfoRange");
		ViewLoading = GameObject.Find (UIName + "/Center/ViewLoading");
		CharacterInfo = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo");
		CharacterInfoLabel = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/Label").GetComponent<UILabel>();
		CharacterInfoLabel.text = "";
		CharacterInfo.SetActive (false);

		UIEventListener.Get(GameObject.Find(UIName + "/Right/InfoRange/AttributeHexagon")).onClick = OnClickSixAttr;
		PlayerName = GameObject.Find (UIName + "/Right/InfoRange/PlayerName/Label").GetComponent<UILabel>();
		PlayerNameObj = GameObject.Find (UIName + "/Right/InfoRange/PlayerName");
		PlayerNameObj.SetActive (true);

		PlayerBody = GameObject.Find (UIName + "/Right/InfoRange/BodyType/Label").GetComponent<UILabel>();
		PlayerBodyObj = GameObject.Find (UIName + "/Right/InfoRange/BodyType");
		PlayerBodyObj.SetActive (true);

		SelectABName[0] = GameObject.Find(UIName + "/Center/ViewLoading/SelectA/PlayerNameA/Label").GetComponent<UILabel>();
		SelectABBody[0] = GameObject.Find(UIName + "/Center/ViewLoading/SelectA/BodyTypeA/Label").GetComponent<UILabel>();

		SelectABName[1] = GameObject.Find(UIName + "/Center/ViewLoading/SelectB/PlayerNameB/Label").GetComponent<UILabel>();
		SelectABBody[1] = GameObject.Find(UIName + "/Center/ViewLoading/SelectB/BodyTypeB/Label").GetComponent<UILabel>();
		UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 25.3f));
	}

	public void OnClickSixAttr(GameObject obj)
	{
		//CharacterInfo.SetActive (!CharacterInfo.activeInHierarchy);
	}

	public void DoSelectRole()
	{
		UITriangle.Get.Triangle.SetActive (false);
		InfoRange.SetActive (false);
		OkBtn.SetActive (false);
		Left.SetActive (false);
		PlayerNameObj.SetActive (false);
		PlayerBodyObj.SetActive (false);
		ViewLoading.SetActive (true);
		PlayerObjAy[0].transform.localEulerAngles = new Vector3(0, 180, 0);

		int RanID;
		int Count;
		for(int i = 1; i < Ay.Length; i++) 
		{
			PlayerObjAy[i].SetActive(true);

			RanID = UnityEngine.Random.Range(0, RoleIDAy.Length - i);
			Count = 0;

			for(int j = 0; j < RoleIDAy.Length; j++)
			{
				if(GameData.DPlayers.ContainsKey(RoleIDAy[j]))
				{
					if(i == 1)
					{
						if(RoleIDAy[j] != SelectIDAy[0])
						{
							if(Count == RanID)
							{
								SetPlayerAvatar(i, j);
								break;
							}
							else
								Count++;
						}
					}
					else
					{
						if(RoleIDAy[j] != SelectIDAy[0] && RoleIDAy[j] != SelectIDAy[1])
						{
							if(Count == RanID)
							{
								SetPlayerAvatar(i, j);
								break;
							}
							else
								Count++;
						}
					}
				}
			}
		}

		for(int i = 0; i < SelectIDAy.Length; i++) 
		{
			if(i == 0)
			{
				if(GameData.DPlayers.ContainsKey(SelectIDAy[i]))
				{
					GameData.Team.Player.ID = GameData.DPlayers[SelectIDAy[i]].ID;
					GameData.Team.Player.Name = GameData.DPlayers[SelectIDAy[i]].Name;
					GameData.Team.Player.AILevel = GameData.DPlayers[SelectIDAy[i]].AILevel;
				}
			}
			else
			{
				if(GameData.DPlayers.ContainsKey(SelectIDAy[i]))
				{
					GameData.TeamMembers[i - 1].Player.ID = GameData.DPlayers[SelectIDAy[i]].ID;
					GameData.TeamMembers[i - 1].Player.Name = GameData.DPlayers[SelectIDAy[i]].Name;
					GameData.TeamMembers[i - 1].Player.AILevel = GameData.DPlayers[SelectIDAy[i]].AILevel;
				}
			}
		}

		GoTime = Time.time + forGoTime;
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
			PlayerObjAy[0].transform.localEulerAngles = new Vector3(0, 180, 0);
			if(GameData.DPlayers.ContainsKey(SelectIDAy [0]))
			{
				TGreatPlayer data = GameData.DPlayers[SelectIDAy [0]];
				
				Value = data.Strength + data.Block;
				UITriangle.Get.ChangeValue (0, Value / 2 / MaxValue);
				
				Value = data.Defence + data.Steal;
				UITriangle.Get.ChangeValue (1, Value / 2 / MaxValue);
				
				Value = data.Dribble + data.Pass;
				UITriangle.Get.ChangeValue (2, Value / 2 / MaxValue);
				
				Value = data.Point2 + data.Point3;
				UITriangle.Get.ChangeValue (3, Value / 2 / MaxValue);
				
				Value = data.Speed + data.Stamina;
				UITriangle.Get.ChangeValue (4, Value / 2 / MaxValue);
				
				Value = data.Rebound + data.Dunk;
				UITriangle.Get.ChangeValue (5, Value / 2 / MaxValue);
			}
		}
	}

	private void SetPlayerAvatar(int RoleIndex, int Index)
	{
		PlayerAy[RoleIndex].ID = RoleIDAy[Index];
		PlayerAy [RoleIndex].AILevel = GameData.DPlayers [RoleIDAy [Index]].AILevel;
		SelectIDAy[RoleIndex] = RoleIDAy[Index];
		PlayerAy[RoleIndex].SetAvatar();
		AvatarAy[RoleIndex] = PlayerAy[RoleIndex].Avatar;
		ModelManager.Get.SetAvatar(ref PlayerObjAy[RoleIndex], PlayerAy[RoleIndex].Avatar, false, false);
		ChangeLayersRecursively(PlayerObjAy[RoleIndex].transform, "UI");

		switch(RoleIndex)
		{
		case 0:
			PlayerName.text = GameData.DPlayers [RoleIDAy [Index]].Name;
			PlayerBody.text = TextConst.S(GameData.DPlayers [RoleIDAy [Index]].BodyType + 7);
			break;
		case 1:
		case 2:
			SelectABName[RoleIndex - 1].text = GameData.DPlayers [RoleIDAy [Index]].Name;
			SelectABBody[RoleIndex - 1].text = TextConst.S(GameData.DPlayers [RoleIDAy [Index]].BodyType + 7);
			break;
		}

	}
	
	protected override void InitData() 
	{
		AvatarAy = new GameStruct.TAvatar[Ay.Length];
		SelectIDAy [0] = RoleIDAy[0];
		for(int i = 0; i < Ay.Length; i++) 
		{
			PlayerObjAy[i] = new GameObject();
			PlayerAy[i] = new TPlayer(0);
			PlayerAy[i].ID = RoleIDAy[i];
			PlayerAy[i].SetAvatar();
			PlayerObjAy[i].name = i.ToString();
			PlayerObjAy[i].transform.parent = PlayerInfoModel.transform;
			AvatarAy[i] = PlayerAy[i].Avatar;
			ModelManager.Get.SetAvatar(ref PlayerObjAy[i], PlayerAy[i].Avatar, false, false);
			PlayerObjAy[i].transform.localPosition = Ay[i];
			PlayerAy [i].AILevel = GameData.DPlayers [RoleIDAy [i]].AILevel;

			if(i == 0)
			{
				PlayerObjAy[i].transform.localPosition = new Vector3(0, -0.65f, 0);
				PlayerObjAy[i].transform.localEulerAngles = new Vector3(0, 180, 0);
				PlayerName.text = GameData.DPlayers [PlayerAy[i].ID].Name;
				PlayerBody.text = TextConst.S(GameData.DPlayers [PlayerAy[i].ID].BodyType + 7);
			}
			else if(i == 1)
			{
				PlayerObjAy[i].transform.localPosition = new Vector3(0.42f, -0.33f, 2.87f);
				PlayerObjAy[i].transform.localEulerAngles = new Vector3(0, 150, 0);
			}
			else if(i == 2)
			{
				PlayerObjAy[i].transform.localPosition = new Vector3(-0.4f, -0.35f, 2.58f);
				PlayerObjAy[i].transform.localEulerAngles = new Vector3(0, -150, 0);
			}

			if(i == 0)
				PlayerObjAy[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			else
				PlayerObjAy[i].transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

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

		ViewLoading.SetActive (false);
		for(int i = 1; i < Ay.Length; i++) 		
			PlayerObjAy[i].SetActive(false);

		if(GameData.DPlayers.ContainsKey(SelectIDAy [0]))
		{
			TGreatPlayer data = GameData.DPlayers[SelectIDAy [0]];
			
			Value = data.Strength + data.Block;
			UITriangle.Get.ChangeValue (0, Value / 2 / MaxValue);
			
			Value = data.Defence + data.Steal;
			UITriangle.Get.ChangeValue (1, Value / 2 / MaxValue);
			
			Value = data.Dribble + data.Pass;
			UITriangle.Get.ChangeValue (2, Value / 2 / MaxValue);
			
			Value = data.Point2 + data.Point3;
			UITriangle.Get.ChangeValue (3, Value / 2 / MaxValue);
			
			Value = data.Speed + data.Stamina;
			UITriangle.Get.ChangeValue (4, Value / 2 / MaxValue);
			
			Value = data.Rebound + data.Dunk;
			UITriangle.Get.ChangeValue (5, Value / 2 / MaxValue);
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
		if(GoTime > 0 && Time.time >= GoTime)
		{
			GoTime = 0;
			SceneMgr.Get.ChangeLevel (SceneName.Court_0);
		}

		if(OkBtn.activeInHierarchy)
		{
			if(Input.GetMouseButton(0)) 
			{
				axisX = 0;

				#if UNITY_EDITOR
				axisX = -Input.GetAxis ("Mouse X");
				#else
				#if UNITY_IOS
				if(Input.touchCount > 0)
					axisX = -Input.touches[0].deltaPosition.x;
				#endif
				#if UNITY_ANDROID
				if(Input.touchCount > 0)
					axisX = -Input.touches[0].deltaPosition.x;
				#endif
				#if (!UNITY_IOS && !UNITY_ANDROID)
				axisX = -Input.GetAxis ("Mouse X");
				#endif
				#endif
				PlayerObjAy[0].transform.Rotate(new Vector3(0, axisX, 0), Space.Self);
			} 
		}
	}
}

