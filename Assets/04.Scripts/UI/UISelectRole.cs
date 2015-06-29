using UnityEngine;
using System.Collections;
using GameStruct;
using System;
using System.Collections.Generic;
using DG.Tweening; 

public struct TSelectAttrData{
	public UISlider Slider;
	public UILabel Value;
}

public enum EUIRoleSituation {
	ListA = 1,
	ListB = 2,
	SelectRole = 3,
	ControlMusic = 4,
	ChooseRole = 5,
	BackToSelectMe = 6,
	Start = 7
}


public class UISelectRole : UIBase {
	private static UISelectRole instance = null;
	private const string UIName = "UISelectRole";
	//	private TAvatar[]  arrayAvatar;
	public static int [] arrayRoleID = new int[6]{14, 24, 34, 19, 29, 39};  // playerID
	public GameObject playerInfoModel = null;
	private int [] arraySelectID = new int[3];
	private TPlayer [] arrayPlayerData = new TPlayer[3];
	private Vector3 [] arrayPlayerPosition = new Vector3[3];
	private GameObject [] arrayPlayer = new GameObject[3];
	private GameObject [] buttonSelectRole = new GameObject[6];

	private GameObject viewLeft;
	private GameObject viewLoading;

	private GameObject uiOK;
	private GameObject uiSelect;
	private GameObject uiInfoRange;
	private GameObject uiCharacterInfo;
	private GameObject uiPlayerName;
	private GameObject uiPlayerBody;

	private UILabel labelPlayerName;
	private UISprite spritePlayerBodyPic;
	private UISprite spriteMusicOn;

	private UILabel [] labelsSelectABName = new UILabel[2];
	private UISprite [] spritesSelectABBody = new UISprite[2];
	private UILabel [] labelsSelectAListName = new UILabel[3];
	private UISprite [] spritesSelectAListPic = new UISprite[3];
	private UISprite [] spritesSelectAListBigPic = new UISprite[3];
	private UILabel [] labelsSelectBListName = new UILabel[3];
	private UISprite [] spritesSelectBListPic = new UISprite[3];
	private UISprite [] spritesSelectBListBigPic = new UISprite[3];
	private UISprite [] spritesLine = new UISprite[6];
	private UISprite [] spritesBigHead = new UISprite[6]; 

	private int [] arrayUnSelectID = new int[3];
	private Animator [] arrayAnimator = new Animator[3];
	private string [] arrayAnimatorName = new string[1]{""};
	private float [] arrayOldValue = new float[12];
	private float [] arrayNewValue = new float[12];
	private GameObject [] arrayNamePic = new GameObject[6]; 
	private float [] arrayOldNameValue = new float[6];
	private float [] arrayNewNameValue = new float[6];
	private TSelectAttrData [] arraySelectAttrData = new TSelectAttrData[12];

	private int maxValue = 100;
	private float value = 0;
	private float axisX;
	
	private int SelectRoleIndex = 0;

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
		}
		else
			if (isShow)
				Get.Show(isShow);
	}
	
	public static UISelectRole Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISelectRole;
			
			return instance;
		}
	}

	
	void FixedUpdate(){
		if (SelectRoleIndex >= 0 && SelectRoleIndex < spritesLine.Length) {
			if(spritesLine[SelectRoleIndex].fillAmount < 1)
				spritesLine[SelectRoleIndex].fillAmount += 0.1f;		
		}
		
		for(int i = 0; i < arrayOldValue.Length; i++) {
			if(arrayOldValue[i] != arrayNewValue[i]) {
				if(arrayOldValue[i] > arrayNewValue[i]) {
					setSubAttr(i, arrayOldValue[i] - 1);
					arrayOldValue[i] -= 1;
				} else {
					setSubAttr(i, arrayOldValue[i] + 1);
					arrayOldValue[i] += 1;
				}
			}
		}
		
		for(int i = 0; i < arrayOldNameValue.Length; i++) {
			if(arrayOldNameValue[i] != arrayNewNameValue[i]) {
				if(arrayOldNameValue[i] > arrayNewNameValue[i]) {
					arrayOldNameValue[i] -= 10;
					arrayNamePic[i].transform.localPosition = new Vector3(0, arrayOldNameValue[i], 0);
				} else {
					arrayOldNameValue[i] += 10;
					arrayNamePic[i].transform.localPosition = new Vector3(0, arrayOldNameValue[i], 0);
				}
			}
		}
		
		if(uiOK.activeInHierarchy) {
			if(Input.GetMouseButton(0)) {
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
				if(!uiCharacterInfo.activeInHierarchy)
					arrayPlayer[0].transform.Rotate(new Vector3(0, axisX, 0), Space.Self);
			} 
		}
	}


	protected override void InitCom() {
		playerInfoModel = new GameObject();
		playerInfoModel.name = "PlayerInfoModel";
		arrayPlayerPosition [0] = new Vector3 (0, 0, 0);
		arrayPlayerPosition [1] = new Vector3 (3, 0, 0);
		arrayPlayerPosition [2] = new Vector3 (-3, 0, 0);


		for (int i = 0; i < 6; i++) {
			SetBtnFun(UIName + "/Left/SelectCharacter/Button" + i.ToString(), SelectRole);
			spritesBigHead[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString() + "/SpriteFace").GetComponent<UISprite>();
			spritesBigHead[i].spriteName = GameData.DPlayers[arrayRoleID[i]].Name;
			arrayNamePic[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString() + "/Sprite");
			buttonSelectRole[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString());
			spritesLine[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString() + "/SpriteLine").GetComponent<UISprite>();
			spritesLine[i].fillAmount = 0;
		}

		SetBtnFun (UIName + "/Right/MusicSwitch/ButtonMusic", DoControlMusic);
		SetBtnFun (UIName + "/Right/CharacterCheck", DoChooseRole);
		SetBtnFun (UIName + "/Center/ViewLoading/Back", DoBackToSelectMe);
		SetBtnFun (UIName + "/Center/ViewLoading/GameStart", DoStart);

		viewLeft = GameObject.Find (UIName + "/Left");
		viewLoading = GameObject.Find (UIName + "/Center/ViewLoading");

		uiSelect = GameObject.Find (UIName + "/Left/Select");
		uiOK = GameObject.Find (UIName + "/Right/CharacterCheck");
		uiInfoRange = GameObject.Find (UIName + "/Right/InfoRange");
		uiCharacterInfo = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo");
		uiCharacterInfo.SetActive (false);
		uiPlayerName = GameObject.Find (UIName + "/Right/InfoRange/PlayerName");
		uiPlayerName.SetActive (true);
		uiPlayerBody = GameObject.Find (UIName + "/Right/InfoRange/BodyType");
		uiPlayerBody.SetActive (true);

		spriteMusicOn = GameObject.Find (UIName + "/Right/MusicSwitch/ButtonMusic/On").GetComponent<UISprite>();
		spriteMusicOn.enabled = AudioMgr.Get.IsMusicOn;
		labelPlayerName = GameObject.Find (UIName + "/Right/InfoRange/PlayerName/Label").GetComponent<UILabel>();
		spritePlayerBodyPic = GameObject.Find (UIName + "/Right/InfoRange/BodyType/SpriteType").GetComponent<UISprite>();
		labelsSelectABName[0] = GameObject.Find(UIName + "/Center/ViewLoading/SelectA/PlayerNameA/Label").GetComponent<UILabel>();
		spritesSelectABBody[0] = GameObject.Find(UIName + "/Center/ViewLoading/SelectA/PlayerNameA/SpriteTypeA").GetComponent<UISprite>();
		labelsSelectABName[1] = GameObject.Find(UIName + "/Center/ViewLoading/SelectB/PlayerNameB/Label").GetComponent<UILabel>();
		spritesSelectABBody[1] = GameObject.Find(UIName + "/Center/ViewLoading/SelectB/PlayerNameB/SpriteTypeB").GetComponent<UISprite>();

		arraySelectAttrData [0].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/2Point").GetComponent<UISlider>();
		arraySelectAttrData [0].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/2Point/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [1].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/3Point").GetComponent<UISlider>();
		arraySelectAttrData [1].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/3Point/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [2].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Speed").GetComponent<UISlider>();
		arraySelectAttrData [2].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Speed/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [3].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Stamina").GetComponent<UISlider>();
		arraySelectAttrData [3].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Stamina/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [4].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Rebound").GetComponent<UISlider>();
		arraySelectAttrData [4].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Rebound/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [5].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Dunk").GetComponent<UISlider>();
		arraySelectAttrData [5].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Dunk/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [6].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Block").GetComponent<UISlider>();
		arraySelectAttrData [6].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Block/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [7].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Strength").GetComponent<UISlider>();
		arraySelectAttrData [7].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Strength/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [8].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Defence").GetComponent<UISlider>();
		arraySelectAttrData [8].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Defence/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [9].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Steal").GetComponent<UISlider>();
		arraySelectAttrData [9].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Steal/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [10].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Dribble").GetComponent<UISlider>();
		arraySelectAttrData [10].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Dribble/LabelValue").GetComponent<UILabel>();
		arraySelectAttrData [11].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Pass").GetComponent<UISlider>();
		arraySelectAttrData [11].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Pass/LabelValue").GetComponent<UILabel>();

		UIEventListener.Get(GameObject.Find(UIName + "/Right/InfoRange/AttributeHexagon")).onClick = OnClickSixAttr;
		UIEventListener.Get(GameObject.Find(UIName + "/Right/InfoRange/CharacterInfo")).onClick = OnClickSixAttr;

		for(int i = 0; i < labelsSelectAListName.Length; i++) {
			labelsSelectAListName [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString() + "/PlayerName").GetComponent<UILabel>();
			spritesSelectAListPic [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString() + "/BodyType").GetComponent<UISprite>();
			spritesSelectAListBigPic [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString() + "/SpriteFace").GetComponent<UISprite>();
			SetBtnFun(UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString(), DoListA);
		}

		for(int i = 0; i < labelsSelectBListName.Length; i++) {
			labelsSelectBListName [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString() + "/PlayerName").GetComponent<UILabel>();
			spritesSelectBListBigPic [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString() + "/SpriteFace").GetComponent<UISprite>();
			spritesSelectBListPic [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString() + "/BodyType").GetComponent<UISprite>();
			SetBtnFun(UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString(), DoListB);
		}


		float WH = (float)Screen.width / (float)Screen.height;
		if(WH >= 1.33f && WH <= 1.34f)
			UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 34));
		else if(WH >= 1.59f && WH <= 1.61f)
			UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 28.3f));
		else if(WH >= 1.66f && WH <= 1.67f)
			UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 27f));
		else if(WH >= 1.7f && WH <= 1.71f)
			UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 26.5f));
		else
			UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 25.3f));
	}
	
	protected override void OnShow(bool isShow) {
		
	}
	
	protected override void InitData() {
//		arrayAvatar = new GameStruct.TAvatar[Ay.Length];
		SelectRoleIndex = UnityEngine.Random.Range (0, arrayRoleID.Length);
		arraySelectID [0] = arrayRoleID[SelectRoleIndex];
		for(int i = 0; i < arrayPlayerPosition.Length; i++) {
			arrayPlayer[i] = new GameObject();
			arrayPlayerData[i] = new TPlayer(0);
			arrayPlayerData[i].ID = arraySelectID[0];
			arrayPlayerData[i].SetAvatar();
			arrayPlayer[i].name = i.ToString();
			arrayPlayer[i].transform.parent = playerInfoModel.transform;
//			arrayAvatar[i] = PlayerAy[i].Avatar;
			ModelManager.Get.SetAvatar(ref arrayPlayer[i], arrayPlayerData[i].Avatar, GameData.DPlayers[arraySelectID[0]].BodyType, false, false);
			arrayAnimator[i] = arrayPlayer[i].GetComponent<Animator>();
			arrayPlayer[i].transform.localPosition = arrayPlayerPosition[i];
			arrayPlayerData [i].AILevel = GameData.DPlayers [arraySelectID[0]].AILevel;
			arrayPlayer[i].AddComponent<SelectEvent>();

			if(i == 0) {
				arrayPlayer[i].transform.localPosition = new Vector3(0, -0.8f, 0);
				arrayPlayer[i].transform.localEulerAngles = new Vector3(0, 180, 0);
				labelPlayerName.text = GameData.DPlayers [arrayPlayerData[i].ID].Name;
				SetBodyPic(ref spritePlayerBodyPic, GameData.DPlayers [arrayPlayerData[i].ID].BodyType);
			}else 
			if(i == 1) {
				arrayPlayer[i].transform.localPosition = new Vector3(0.5f, -0.6f, 2.87f);
				arrayPlayer[i].transform.localEulerAngles = new Vector3(0, 150, 0);
			}else 
			if(i == 2) {
				arrayPlayer[i].transform.localPosition = new Vector3(-0.5f, -0.6f, 2.58f);
				arrayPlayer[i].transform.localEulerAngles = new Vector3(0, -150, 0);
			}
			
			if(i == 0)
				arrayPlayer[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			else
				arrayPlayer[i].transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
			
			changeLayersRecursively(arrayPlayer[i].transform, "UI");
			for (int j = 0; j <arrayPlayer[i].transform.childCount; j++) 
			{ 
				if(arrayPlayer[i].transform.GetChild(j).name.Contains("PlayerMode")) 
				{
					arrayPlayer[i].transform.GetChild(j).localScale = Vector3.one;
					arrayPlayer[i].transform.GetChild(j).localEulerAngles = Vector3.zero;
					arrayPlayer[i].transform.GetChild(j).localPosition = Vector3.zero;
				}
			}
		}
		
		viewLoading.SetActive (false);
		for(int i = 1; i < arrayPlayerPosition.Length; i++) 		
			arrayPlayer[i].SetActive(false);

		changeBigHead(SelectRoleIndex);
		
		if(GameData.DPlayers.ContainsKey(arraySelectID [0])) {
			TGreatPlayer data = GameData.DPlayers[arraySelectID [0]];
			
			value = data.Strength + data.Block;
			UITriangle.Get.ChangeValue (0, value / 2 / maxValue);
			
			value = data.Defence + data.Steal;
			UITriangle.Get.ChangeValue (1, value / 2 / maxValue);
			
			value = data.Dribble + data.Pass;
			UITriangle.Get.ChangeValue (2, value / 2 / maxValue);
			
			value = data.Speed + data.Stamina;
			UITriangle.Get.ChangeValue (3, value / 2 / maxValue);
			
			value = data.Point2 + data.Point3;
			UITriangle.Get.ChangeValue (4, value / 2 / maxValue);
			
			value = data.Rebound + data.Dunk;
			UITriangle.Get.ChangeValue (5, value / 2 / maxValue);
			setAttr(data);
		}
	}

	private void setSubAttr(int Index, float Value) {
		arraySelectAttrData [Index].Slider.value = 0;//Value / 100;
		arraySelectAttrData [Index].Value.text = Value.ToString ();
	}

	private void setAttr(TGreatPlayer data) {
		if(arrayOldValue[0] == 0) {
			arrayOldValue[0] = data.Point2;
			arrayNewValue[0] = data.Point2;
			setSubAttr(0, data.Point2);
			arrayOldValue[1] = data.Point3;
			arrayNewValue[1] = data.Point3;
			setSubAttr(1, data.Point3);
			arrayOldValue[2] = data.Speed;
			arrayNewValue[2] = data.Speed;
			setSubAttr(2, data.Speed);
			arrayOldValue[3] = data.Stamina;
			arrayNewValue[3] = data.Stamina;
			setSubAttr(3, data.Stamina);
			arrayOldValue[4] = data.Rebound;
			arrayNewValue[4] = data.Rebound;
			setSubAttr(4, data.Rebound);
			arrayOldValue[5] = data.Dunk;
			arrayNewValue[5] = data.Dunk;
			setSubAttr(5, data.Dunk);
			arrayOldValue[6] = data.Block;
			arrayNewValue[6] = data.Block;
			setSubAttr(6, data.Block);
			arrayOldValue[7] = data.Strength;
			arrayNewValue[7] = data.Strength;
			setSubAttr(7, data.Strength);
			arrayOldValue[8] = data.Defence;
			arrayNewValue[8] = data.Defence;
			setSubAttr(8, data.Defence);
			arrayOldValue[9] = data.Steal;
			arrayNewValue[9] = data.Steal;
			setSubAttr(9, data.Steal);
			arrayOldValue[10] = data.Dribble;
			arrayNewValue[10] = data.Dribble;
			setSubAttr(10, data.Dribble);
			arrayOldValue[11] = data.Pass;
			arrayNewValue[11] = data.Pass;
			setSubAttr(11, data.Pass);
		} else {
			arrayNewValue[0] = data.Point2;
			arrayNewValue[1] = data.Point3;
			arrayNewValue[2] = data.Speed;
			arrayNewValue[3] = data.Stamina;
			arrayNewValue[4] = data.Rebound;
			arrayNewValue[5] = data.Dunk;
			arrayNewValue[6] = data.Block;
			arrayNewValue[7] = data.Strength;
			arrayNewValue[8] = data.Defence;
			arrayNewValue[9] = data.Steal;
			arrayNewValue[10] = data.Dribble;
			arrayNewValue[11] = data.Pass;
		}
	}

	public void OnClickSixAttr(GameObject obj) {
		uiCharacterInfo.SetActive (!uiCharacterInfo.activeInHierarchy);
	}
	
	public void DoBackToSelectMe() {
		UIState(EUIRoleSituation.BackToSelectMe);
	}
	
	public void DoControlMusic() {
		UIState(EUIRoleSituation.ControlMusic);
	}

	public void DoStart(){
		UIState(EUIRoleSituation.Start);
	}

	public void DoListA() {
		UIState(EUIRoleSituation.ListA);
	}

	public void DoListB(){
		UIState(EUIRoleSituation.ListB);
	}

	public void DoChooseRole() {
		UIState(EUIRoleSituation.ChooseRole);
	}

	public void SelectRole() {
		UIState(EUIRoleSituation.SelectRole);
	}

	public void SetBodyPic(ref UISprite Pic, int Type) {
		switch(Type) {
		case 0:
			Pic.spriteName = "L_namecard_CENTER";
			break;
		case 1:
			Pic.spriteName = "L_namecard_FORWARD";
			break;
		case 2:
			Pic.spriteName = "L_namecard_GUARD";
			break;
		default:
			Pic.spriteName = "L_namecard_CENTER";
			break;
		}
	}

	private void setPlayerAvatar(int RoleIndex, int Index) {
		arrayPlayerData[RoleIndex].ID = arrayRoleID[Index];
		arrayPlayerData [RoleIndex].AILevel = GameData.DPlayers [arrayRoleID [Index]].AILevel;
		arraySelectID[RoleIndex] = arrayRoleID[Index];
		arrayPlayerData[RoleIndex].SetAvatar();
//		arrayAvatar[RoleIndex] = PlayerAy[RoleIndex].Avatar;
		GameObject temp = arrayPlayer [RoleIndex];

		ModelManager.Get.SetAvatar(ref arrayPlayer[RoleIndex], arrayPlayerData[RoleIndex].Avatar, GameData.DPlayers [arrayRoleID [Index]].BodyType, false, false, true);

		arrayPlayer[RoleIndex].name = RoleIndex.ToString();
		arrayPlayer[RoleIndex].transform.parent = playerInfoModel.transform;
		arrayPlayer[RoleIndex].transform.localPosition = arrayPlayerPosition[RoleIndex];
		arrayPlayer[RoleIndex].AddComponent<SelectEvent>();
		arrayPlayer[RoleIndex].transform.localPosition = temp.transform.localPosition;
		arrayPlayer[RoleIndex].transform.localEulerAngles = temp.transform.localEulerAngles;
		arrayPlayer[RoleIndex].transform.localScale = temp.transform.localScale;
		for (int j = 0; j <arrayPlayer[RoleIndex].transform.childCount; j++)  { 
			if(arrayPlayer[RoleIndex].transform.GetChild(j).name.Contains("PlayerMode")) {
				arrayPlayer[RoleIndex].transform.GetChild(j).localScale = Vector3.one;
				arrayPlayer[RoleIndex].transform.GetChild(j).localEulerAngles = Vector3.zero;
				arrayPlayer[RoleIndex].transform.GetChild(j).localPosition = Vector3.zero;
			}
		}


		arrayAnimator[RoleIndex] = arrayPlayer[RoleIndex].GetComponent<Animator>();
		changeLayersRecursively(arrayPlayer[RoleIndex].transform, "UI");

		switch(RoleIndex) {
		case 0:
			labelPlayerName.text = GameData.DPlayers [arrayRoleID [Index]].Name;
			SetBodyPic(ref spritePlayerBodyPic, GameData.DPlayers [arrayRoleID [Index]].BodyType);
			break;
		case 1:
		case 2:
			labelsSelectABName[RoleIndex - 1].text = GameData.DPlayers [arrayRoleID [Index]].Name;
			SetBodyPic(ref spritesSelectABBody[RoleIndex - 1], GameData.DPlayers [arrayRoleID [Index]].BodyType);
			break;
		}

	}

	private void playAnimator(int Index, string name) {
		if(Index == 0 && Index < arrayAnimator.Length && name != "" && arrayAnimator [Index] != null)
			arrayAnimator [Index].SetTrigger (name);
	}

	private void changeLayersRecursively(Transform trans, string name){
		trans.gameObject.layer = LayerMask.NameToLayer(name);
		foreach(Transform child in trans)
		{            
			changeLayersRecursively(child, name);
		}
	}

	public void SetEnemyMembers(){
		int Index = 0;

		for(int j = 0; j < arrayRoleID.Length; j++) {
			if(arrayRoleID[j] != GameData.Team.Player.ID &&
			   arrayRoleID[j] != GameData.TeamMembers[0].Player.ID &&
			   arrayRoleID[j] != GameData.TeamMembers[1].Player.ID) {
				if(GameData.DPlayers.ContainsKey(arrayRoleID[j]))
				{
					GameData.EnemyMembers[Index].Player.ID = arrayRoleID[j];
					GameData.EnemyMembers[Index].Player.Name = GameData.DPlayers[arrayRoleID[j]].Name;
					GameData.EnemyMembers[Index].Player.SetAttribute();
					GameData.EnemyMembers[Index].Player.SetAvatar();
					Index++;
				}
			}
		}
	}

	private void changeRoleInfo () {
		int index = 0;
		for(int j = 0; j < arrayRoleID.Length; j++){
			if(arrayRoleID[j] != GameData.Team.Player.ID &&
			   arrayRoleID[j] != GameData.TeamMembers[0].Player.ID &&
			   arrayRoleID[j] != GameData.TeamMembers[1].Player.ID) {
				if(GameData.DPlayers.ContainsKey(arrayRoleID[j])) {
					arrayUnSelectID[index] = arrayRoleID[j];
					index++;
				}
			}
		}
		
		for(int i = 0; i < labelsSelectAListName.Length; i++) {
			labelsSelectAListName [i].text = "";//GameData.DPlayers[UnSelectIDAy[i]].Name;
			spritesSelectAListBigPic[i].spriteName = GameData.DPlayers[arrayUnSelectID[i]].Name;
			SetBodyPic(ref spritesSelectAListPic[i], GameData.DPlayers[arrayUnSelectID[i]].BodyType);
			
			labelsSelectBListName [i].text = "";//GameData.DPlayers[UnSelectIDAy[i]].Name;
			spritesSelectBListBigPic[i].spriteName = GameData.DPlayers[arrayUnSelectID[i]].Name;
			SetBodyPic(ref spritesSelectBListPic[i], GameData.DPlayers[arrayUnSelectID[i]].BodyType);
		}
	}

	private void changeBigHead (int index){
		uiSelect.transform.localPosition = buttonSelectRole[index].transform.localPosition;
		for (int i = 0; i < 6; i++) {
			if(index != i) {
				spritesBigHead[i].color = new Color32(107, 107, 107, 255);
				arrayNewNameValue[i] = 0;
			} else {
				spritesBigHead[i].color = new Color32(255, 255, 255, 255);
				arrayNewNameValue[i] = -80;
			}
		}
	}

	private void UIState(EUIRoleSituation state) {
		switch (state) {
		case EUIRoleSituation.SelectRole:{
			int Index;
			if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out Index)) {
				changeBigHead(Index);
				
				SelectRoleIndex = Index;
				for(int i = 0; i < spritesLine.Length; i++)
					spritesLine[i].fillAmount = 0;
				setPlayerAvatar(0, Index);
				arrayPlayer[0].transform.localEulerAngles = new Vector3(0, 180, 0);
				playAnimator(0, arrayAnimatorName[UnityEngine.Random.Range(0, arrayAnimatorName.Length)]);
				if(GameData.DPlayers.ContainsKey(arraySelectID [0])) {
					TGreatPlayer data = GameData.DPlayers[arraySelectID [0]];
					
					value = data.Strength + data.Block;
					UITriangle.Get.ChangeValue (0, value / 2 / maxValue);
					
					value = data.Defence + data.Steal;
					UITriangle.Get.ChangeValue (1, value / 2 / maxValue);
					
					value = data.Dribble + data.Pass;
					UITriangle.Get.ChangeValue (2, value / 2 / maxValue);
					
					value = data.Speed + data.Stamina;
					UITriangle.Get.ChangeValue (3, value / 2 / maxValue);
					
					value = data.Point2 + data.Point3;
					UITriangle.Get.ChangeValue (4, value / 2 / maxValue);
					
					value = data.Rebound + data.Dunk;
					UITriangle.Get.ChangeValue (5, value / 2 / maxValue);
					setAttr(data);
				}
			}
		}
			break;
		case EUIRoleSituation.ControlMusic:
			spriteMusicOn.enabled = !spriteMusicOn.enabled;
			AudioMgr.Get.MusicOn(spriteMusicOn.enabled);	
			break;
		case EUIRoleSituation.ChooseRole:{
			UITriangle.Get.Triangle.SetActive (false);
			viewLeft.SetActive (false);
			viewLoading.SetActive (true);

			uiOK.SetActive (false);
			uiInfoRange.SetActive (false);
			uiPlayerName.SetActive (false);
			uiPlayerBody.SetActive (false);
			arrayPlayer[0].transform.localEulerAngles = new Vector3(0, 180, 0);
			
			int RanID;
			int Count;
			for(int i = 1; i < arrayPlayerPosition.Length; i++) {
				arrayPlayer[i].SetActive(true);
				
				RanID = UnityEngine.Random.Range(0, arrayRoleID.Length - i);
				Count = 0;
				
				for(int j = 0; j < arrayRoleID.Length; j++) {
					if(GameData.DPlayers.ContainsKey(arrayRoleID[j])) {
						if(i == 1) {
							if(arrayRoleID[j] != arraySelectID[0]) {
								if(Count == RanID) {
									setPlayerAvatar(i, j);
									break;
								}
								else
									Count++; 
							}
						} else {
							if(arrayRoleID[j] != arraySelectID[0] && arrayRoleID[j] != arraySelectID[1]) {
								if(Count == RanID) {
									setPlayerAvatar(i, j);
									break;
								} else
									Count++;
							}
						}
					}
				}
			}
			
			for(int i = 0; i < arraySelectID.Length; i++) {
				if(i == 0) {
					if(GameData.DPlayers.ContainsKey(arraySelectID[i])) {
						GameData.Team.Player.ID = GameData.DPlayers[arraySelectID[i]].ID;
						GameData.Team.Player.Name = GameData.DPlayers[arraySelectID[i]].Name;
						GameData.Team.Player.AILevel = GameData.DPlayers[arraySelectID[i]].AILevel;
					}
				} else {
					if(GameData.DPlayers.ContainsKey(arraySelectID[i])) {
						GameData.TeamMembers[i - 1].Player.ID = GameData.DPlayers[arraySelectID[i]].ID;
						GameData.TeamMembers[i - 1].Player.Name = GameData.DPlayers[arraySelectID[i]].Name;
						GameData.TeamMembers[i - 1].Player.AILevel = GameData.DPlayers[arraySelectID[i]].AILevel;
					}
				}
			}
			changeRoleInfo();
		}
			break;
		case EUIRoleSituation.BackToSelectMe:
			UITriangle.Get.Triangle.SetActive (true);
			viewLeft.SetActive (true);
			viewLoading.SetActive (false);

			uiOK.SetActive (true);
			uiInfoRange.SetActive (true);
			uiPlayerName.SetActive (true);
			uiPlayerBody.SetActive (true);
			
			for(int i = 1; i < arrayPlayerPosition.Length; i++) 	
				arrayPlayer[i].SetActive(false);
			break;
		case EUIRoleSituation.Start:
			SetEnemyMembers ();
			GameData.Team.Player.SetAttribute();
			GameData.Team.Player.SetAvatar();
			GameData.TeamMembers [0].Player.SetAttribute ();
			GameData.TeamMembers [0].Player.SetAvatar ();
			GameData.TeamMembers [1].Player.SetAttribute ();
			GameData.TeamMembers [1].Player.SetAvatar ();		
			SceneMgr.Get.ChangeLevel (SceneName.Court_0);
			break;
		case EUIRoleSituation.ListA: // 1
		case EUIRoleSituation.ListB: // 2
			int mIndex;
			if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out mIndex)) {
				int SelectIndex = 0;
				for(int i = 0; i < arrayRoleID.Length; i++) {
					if(arrayRoleID[i] == arrayUnSelectID[mIndex]) {
						SelectIndex = i;
						break;
					}
				}
				
				setPlayerAvatar((int)state, SelectIndex);
				if(GameData.DPlayers.ContainsKey(arraySelectID[(int)state])) {
					GameData.TeamMembers[(int)state - 1].Player.ID = GameData.DPlayers[arraySelectID[(int)state]].ID;
					GameData.TeamMembers[(int)state - 1].Player.Name = GameData.DPlayers[arraySelectID[(int)state]].Name;
					GameData.TeamMembers[(int)state - 1].Player.AILevel = GameData.DPlayers[arraySelectID[(int)state]].AILevel;
				}
				
				changeRoleInfo ();
			}
			break;
		}
	}
}

