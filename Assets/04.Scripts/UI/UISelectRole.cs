using UnityEngine;
using System.Collections;
using GameStruct;
using GameEnum;
using System;
using System.Collections.Generic;
using DG.Tweening; 

public enum EUIRoleSituation {
	ListA = 1,
	ListB = 2,
	SelectRole = 3,
	ControlMusic = 4,
	ChooseRole = 5,
	BackToSelectMe = 6,
	Start = 7,
	BackToMode = 8
}


public class UISelectRole : UIBase {
	private static UISelectRole instance = null;
	private const string UIName = "UISelectRole";
	private static string[] arrayRoleAnimation = new string[9]{"FallQuickStand","Idle","Idle1","DefenceStay","Stop0","Stop1","Stop3","StayDribble","StayDodge0"};
	private float roleFallTime = 0;

	private TGreatPlayer data ;
	public GameObject playerInfoModel = null;
	private int [] arraySelectID = new int[3];
	private TPlayer [] arrayPlayerData = new TPlayer[3];
	private Vector3 [] arrayPlayerPosition = new Vector3[3];
	private GameObject [] arrayPlayer = new GameObject[3];
	private GameObject [] buttonSelectRole = new GameObject[6];

	private Animator animatorLeft;
	private Animator animatorRight;
	private Animator animatorLoading;

	private GameObject uiSelect;
	private GameObject uiShowTime;
	private GameObject uiBack;

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
	private GameObject [] arrayNamePic = new GameObject[6]; 
	private float [] arrayOldNameValue = new float[6];
	private float [] arrayNewNameValue = new float[6];

    private UIAttributes mUIAttributes;

	private const int MaxValue = 200;
//	private float value = 0;
	private float axisX;
	
	private int SelectRoleIndex = 0;

	private float doubleClickTime = 3;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UISelectRole Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISelectRole;
			
			return instance;
		}
	}

	void OnDestroy() {
		if (animatorLeft)
			Destroy (animatorLeft);

		if (animatorRight)
			Destroy (animatorRight);

		if (animatorLoading)
			Destroy (animatorLoading);

		animatorLeft = null;
		animatorRight = null;
		animatorLoading = null;

		for (int i = 0; i < arrayAnimator.Length; i++)
			Destroy(arrayAnimator[i]);

		arrayAnimator = new Animator[0];

		for (int i = 0; i < arrayPlayer.Length; i ++) {
			if (arrayPlayer[i]) {
				SkinnedMeshRenderer smr = arrayPlayer[i].GetComponent<SkinnedMeshRenderer>();
				if (smr) {
					Material[] mats = smr.materials;
					for (int j = 0; j < mats.Length; j++) {
						Destroy(mats[j]);
						mats[j] = null;
					}

					smr.materials = new Material[0];
				}

				Destroy(arrayPlayer[i]);
			}
		}

		arrayPlayer = new GameObject[0];
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
	
	void FixedUpdate(){
		if(doubleClickTime > 0) {
			doubleClickTime -= Time.deltaTime;
			if(doubleClickTime <= 0)
				doubleClickTime = 0;
		}
		if(roleFallTime > 0) {
			roleFallTime -= Time.deltaTime;
			if(roleFallTime <= 0) 
				roleFallTime = 0;
		}
		if (SelectRoleIndex >= 0 && SelectRoleIndex < spritesLine.Length) {
			if(spritesLine[SelectRoleIndex].fillAmount < 1)
				spritesLine[SelectRoleIndex].fillAmount += 0.1f;		
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
		if(uiSelect.activeInHierarchy) {
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
//				if(!UICharacterInfo.Visible)
//					arrayPlayer[0].transform.Rotate(new Vector3(0, axisX, 0), Space.Self);
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
			spritesBigHead[i].spriteName = GameData.PlayerName(GameConst.SelectRoleID[i]);
			arrayNamePic[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString() + "/Sprite");
			buttonSelectRole[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString());
			spritesLine[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString() + "/SpriteLine").GetComponent<UISprite>();
			spritesLine[i].fillAmount = 0;
		}

		SetBtnFun (UIName + "/Right/MusicSwitch/ButtonMusic", DoControlMusic);
		SetBtnFun (UIName + "/Right/CharacterCheck", DoChooseRole);
		SetBtnFun (UIName + "/Left/Back", DoBackToSelectMe);
		SetBtnFun (UIName + "/Right/GameStart", DoStart);
		SetBtnFun (UIName + "/Left/SelectCharacter/Back", DoBackToMode);
		GameObject.Find (UIName + "/Left/SelectCharacter/Back").SetActive(false);

		animatorLeft = GameObject.Find (UIName + "/Left").GetComponent<Animator>();
		animatorRight = GameObject.Find (UIName + "/Right").GetComponent<Animator>();
		animatorLoading = GameObject.Find (UIName + "/Center/ViewLoading").GetComponent<Animator>();

		uiSelect = GameObject.Find (UIName + "/Left/Select");
		uiSelect.SetActive(false);
		uiShowTime = GameObject.Find(UIName + "/Center/ShowTimeCollider");
		uiBack = GameObject.Find(UIName + "/Left/Back");

		spriteMusicOn = GameObject.Find (UIName + "/Right/MusicSwitch/ButtonMusic/On").GetComponent<UISprite>();
		spriteMusicOn.enabled = AudioMgr.Get.IsMusicOn;
		labelPlayerName = GameObject.Find (UIName + "/Right/InfoRange/PlayerName/Label").GetComponent<UILabel>();
		spritePlayerBodyPic = GameObject.Find (UIName + "/Right/InfoRange/BodyType/SpriteType").GetComponent<UISprite>();
		labelsSelectABName[0] = GameObject.Find(UIName + "/Center/ViewLoading/SelectA/PlayerNameA/Label").GetComponent<UILabel>();
		spritesSelectABBody[0] = GameObject.Find(UIName + "/Center/ViewLoading/SelectA/PlayerNameA/SpriteTypeA").GetComponent<UISprite>();
		labelsSelectABName[1] = GameObject.Find(UIName + "/Center/ViewLoading/SelectB/PlayerNameB/Label").GetComponent<UILabel>();
		spritesSelectABBody[1] = GameObject.Find(UIName + "/Center/ViewLoading/SelectB/PlayerNameB/SpriteTypeB").GetComponent<UISprite>();

		UIEventListener.Get(GameObject.Find(UIName + "/Right/InfoRange/UIAttributeHexagon")).onClick = OnClickSixAttr;
		UIEventListener.Get(GameObject.Find(UIName + "/Center/ShowTimeCollider")).onClick = DoPlayerAnimator;

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


        //		float WH = (float)Screen.width / (float)Screen.height;
        //		if(WH >= 1.33f && WH <= 1.34f)
        //			UITriangle.Get.CreateSixAttr(new Vector3(7, -0.9f, 34));
        //			UIAttributes.Get.CreateSixAttr();
        //		else if(WH >= 1.59f && WH <= 1.61f)
        //			UITriangle.Get.CreateSixAttr(new Vector3(7, -0.9f, 28.3f));
        //			UIAttributes.Get.CreateSixAttr();
        //		else if(WH >= 1.66f && WH <= 1.67f)
        //			UITriangle.Get.CreateSixAttr(new Vector3(7, -0.9f, 27f));
        //			UIAttributes.Get.CreateSixAttr();
        //		else if(WH >= 1.7f && WH <= 1.71f)
        //			UITriangle.Get.CreateSixAttr(new Vector3(7, -0.9f, 26.5f));
        //			UIAttributes.Get.CreateSixAttr();
        //		else
        //			UITriangle.Get.CreateSixAttr(new Vector3(7, -0.9f, 25.3f));

        GameObject obj = GameObject.Find("UISelectRole/Right/InfoRange/UIAttributeHexagon");
//        obj.GetComponent<UIAttributes>().Initialize(obj.transform, new Vector3(0, 0, 50), new Vector3(70, 70, 1));
        mUIAttributes = obj.GetComponent<UIAttributes>();
        mUIAttributes.PlayScale(1.5f); // 1.5 是 try and error 的數值, 看起來效果比較順暢.
	}
	
	protected override void OnShow(bool isShow) {
		
	}
	
	protected override void InitData() {
		SelectRoleIndex = UnityEngine.Random.Range (0, GameConst.SelectRoleID.Length);
		arraySelectID [0] = GameConst.SelectRoleID[SelectRoleIndex];
		for(int i = 0; i < arrayPlayerPosition.Length; i++) {
			arrayPlayer[i] = new GameObject();
			arrayPlayerData[i] = new TPlayer(0);
			arrayPlayerData[i].SetID(arraySelectID[0]);
			arrayPlayer[i].name = i.ToString();
			arrayPlayer[i].transform.parent = playerInfoModel.transform;
			ModelManager.Get.SetAvatar(ref arrayPlayer[i], arrayPlayerData[i].Avatar, GameData.DPlayers[arraySelectID[0]].BodyType, EAnimatorType.AvatarControl, false);
			arrayAnimator[i] = arrayPlayer[i].GetComponent<Animator>();
			arrayPlayer[i].GetComponent<CapsuleCollider>().enabled = false;
			arrayPlayer[i].transform.localPosition = arrayPlayerPosition[i];
			arrayPlayerData [i].AILevel = GameData.DPlayers [arraySelectID[0]].AILevel;
			arrayPlayer[i].AddComponent<SelectEvent>();

			if(i == 0) {
				arrayPlayer[i].transform.localPosition = new Vector3(0, -0.9f, 0);
				arrayPlayer[i].transform.localEulerAngles = new Vector3(0, 180, 0);
				labelPlayerName.text = GameData.PlayerName (arrayPlayerData[i].ID);
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

		for(int i = 0; i < arrayPlayerPosition.Length; i++) 		
			arrayPlayer[i].SetActive(false);

		arrayPlayer[0].transform.localPosition = new Vector3(0, 2, 0);
		Invoke("playerDoAnimator", 0.95f);
		Invoke("playerShowTime", 1.1f);
		Invoke("hideSelectRoleAnimator", 2.5f);

		setTriangleData();
	}

	public void DoPlayerAnimator(GameObject obj){
//		UICharacterInfo.Get.SetAttribute(data, arrayPlayerData[0]);
//		UICharacterInfo.Get.transform.localPosition = new Vector3(0, 0, -200);
//		UICharacterInfo.UIShow(!UICharacterInfo.Visible);
//		if(roleFallTime == 0 && uiSelect.activeInHierarchy) {
//			int ranAnimation = UnityEngine.Random.Range(0,9);
//			if(ranAnimation == 0)
//				roleFallTime = 3;
//			arrayAnimator[0].SetTrigger(arrayRoleAnimation[ranAnimation]);
//		}
	}

	public void OnClickSixAttr(GameObject obj) {
//		UICharacterInfo.Get.SetAttribute(data, arrayPlayerData[0]);
//		UICharacterInfo.Get.transform.localPosition = new Vector3(0, 0, -200);
//		UICharacterInfo.UIShow(!UICharacterInfo.Visible);
	}
	
	public void DoBackToSelectMe() {
		if(doubleClickTime == 0) {
			doubleClickTime = 1;
			UIState(EUIRoleSituation.BackToSelectMe);
		}
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

	public void DoBackToMode(){
		UIState(EUIRoleSituation.BackToMode);
	}

	public void DoChooseRole() {
		if(doubleClickTime == 0) {
			doubleClickTime = 1;
			UIState(EUIRoleSituation.ChooseRole);
		}
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
		int id = GameConst.SelectRoleID[Index];
		arrayPlayerData[RoleIndex].ID = id;
		arrayPlayerData [RoleIndex].AILevel = GameData.DPlayers [id].AILevel;
		arraySelectID[RoleIndex] = GameConst.SelectRoleID[Index];
		arrayPlayerData[RoleIndex].SetAvatar();
		GameObject temp = arrayPlayer [RoleIndex];

		ModelManager.Get.SetAvatar(ref arrayPlayer[RoleIndex], arrayPlayerData[RoleIndex].Avatar, GameData.DPlayers [id].BodyType, EAnimatorType.AvatarControl, false, true);

		arrayPlayer[RoleIndex].name = RoleIndex.ToString();
		arrayPlayer[RoleIndex].transform.parent = playerInfoModel.transform;
		arrayPlayer[RoleIndex].transform.localPosition = arrayPlayerPosition[RoleIndex];
		arrayPlayer[RoleIndex].GetComponent<CapsuleCollider>().enabled = false;
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
			labelPlayerName.text = GameData.PlayerName(id);
			SetBodyPic(ref spritePlayerBodyPic, GameData.DPlayers [id].BodyType);
			break;
		case 1:
		case 2:
			labelsSelectABName[RoleIndex - 1].text = GameData.PlayerName(id);
			SetBodyPic(ref spritesSelectABBody[RoleIndex - 1], GameData.DPlayers [id].BodyType);
			break;
		}

	}

	private void changeLayersRecursively(Transform trans, string name){
		trans.gameObject.layer = LayerMask.NameToLayer(name);
		foreach(Transform child in trans)
		{            
			changeLayersRecursively(child, name);
		}
	}

	public void SetEnemyMembers(){
		if (isStage) {
			GameData.EnemyMembers[0].Player.SetID(GameData.DStageData[GameData.StageID].PlayerID1);
			GameData.EnemyMembers[1].Player.SetID(GameData.DStageData[GameData.StageID].PlayerID2);
			GameData.EnemyMembers[2].Player.SetID(GameData.DStageData[GameData.StageID].PlayerID3);
		} else {
			int index = 0;

			for(int j = 0; j < GameConst.SelectRoleID.Length; j++) {
				if(GameConst.SelectRoleID[j] != GameData.Team.Player.ID &&
				   GameConst.SelectRoleID[j] != GameData.TeamMembers[0].Player.ID &&
				   GameConst.SelectRoleID[j] != GameData.TeamMembers[1].Player.ID) {
					if(GameData.DPlayers.ContainsKey(GameConst.SelectRoleID[j]))
					{
						GameData.EnemyMembers[index].Player.Name = GameData.PlayerName(GameConst.SelectRoleID[j]);
						GameData.EnemyMembers[index].Player.SetID(GameConst.SelectRoleID[j]);
						index++;
						if (index >= GameData.EnemyMembers.Length)
							break;
					}
				}
			}
		}
	}

	private void changeRoleInfo () {
		int index = 0;
		for(int j = 0; j < GameConst.SelectRoleID.Length; j++){
			if(GameConst.SelectRoleID[j] != GameData.Team.Player.ID &&
			   GameConst.SelectRoleID[j] != GameData.TeamMembers[0].Player.ID &&
			   GameConst.SelectRoleID[j] != GameData.TeamMembers[1].Player.ID) {
				if(GameData.DPlayers.ContainsKey(GameConst.SelectRoleID[j])) {
					arrayUnSelectID[index] = GameConst.SelectRoleID[j];
					index++;
					if (index >= arrayUnSelectID.Length)
						break;
				}
			}
		}
		
		for(int i = 0; i < labelsSelectAListName.Length; i++) {
			labelsSelectAListName [i].text = "";//GameData.DPlayers[UnSelectIDAy[i]].Name;
			spritesSelectAListBigPic[i].spriteName = GameData.PlayerName (arrayUnSelectID[i]);
			SetBodyPic(ref spritesSelectAListPic[i], GameData.DPlayers[arrayUnSelectID[i]].BodyType);
			
			labelsSelectBListName [i].text = "";//GameData.DPlayers[UnSelectIDAy[i]].Name;
			spritesSelectBListBigPic[i].spriteName = GameData.PlayerName (arrayUnSelectID[i]);
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

	private void setTriangleData()
    {
		if(GameData.DPlayers.ContainsKey(arraySelectID[0]))
        {
			data = GameData.DPlayers[arraySelectID[0]];
			
			var value = data.Strength + data.Block;
			mUIAttributes.SetValue(UIAttributes.EAttribute.StrBlk, value / MaxValue);
			
			value = data.Defence + data.Steal;
            mUIAttributes.SetValue(UIAttributes.EAttribute.DefStl, value / MaxValue);
			
			value = data.Dribble + data.Pass;
            mUIAttributes.SetValue(UIAttributes.EAttribute.DrbPass, value / MaxValue);
			
			value = data.Speed + data.Stamina;
            mUIAttributes.SetValue(UIAttributes.EAttribute.SpdSta, value / MaxValue);
			
			value = data.Point2 + data.Point3;
            mUIAttributes.SetValue(UIAttributes.EAttribute.Pt2Pt3, value / MaxValue);
			
			value = data.Rebound + data.Dunk;
            mUIAttributes.SetValue(UIAttributes.EAttribute.RebDnk, value / MaxValue);
		}
        else
		    Debug.LogErrorFormat("Can't find Player by ID:{0}", arraySelectID[0]);
	}

	private void UIState(EUIRoleSituation state) {
		switch (state) {
		case EUIRoleSituation.SelectRole:{
			int index;
			if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out index)) {
				if(SelectRoleIndex != index) {
					changeBigHead(index);
					SelectRoleIndex = index;
					for(int i = 0; i < spritesLine.Length; i++)
						spritesLine[i].fillAmount = 0;

					setPlayerAvatar(0, index);
					arrayPlayer[0].transform.localEulerAngles = new Vector3(0, 180, 0);
					
					setTriangleData();
					arrayPlayerData[0].SetAttribute(GameEnum.ESkillType.NPC);
//					if(UICharacterInfo.Visible)
//						UICharacterInfo.Get.SetAttribute(data, arrayPlayerData[0]);
				}
			}
		}
			break;
		case EUIRoleSituation.ControlMusic:
			spriteMusicOn.enabled = !spriteMusicOn.enabled;
			AudioMgr.Get.MusicOn(spriteMusicOn.enabled);	
			break;
		case EUIRoleSituation.ChooseRole:{
			uiSelect.SetActive(false);
			arrayAnimator[0].SetTrigger(arrayRoleAnimation[1]);
//			UIAttributes.Get.Triangle.SetActive (false);
            mUIAttributes.SetVisible(false);
			uiShowTime.SetActive(false);

			int RanID;
			int Count;
			for(int i = 1; i < arrayPlayerPosition.Length; i++) {
				
				RanID = UnityEngine.Random.Range(0, GameConst.SelectRoleID.Length - i);
				Count = 0;
				
				for(int j = 0; j < GameConst.SelectRoleID.Length; j++) {
					if(GameData.DPlayers.ContainsKey(GameConst.SelectRoleID[j])) {
						if(i == 1) {
							if(GameConst.SelectRoleID[j] != arraySelectID[0]) {
								if(Count == RanID) {
									setPlayerAvatar(i, j);
									break;
								}
								else
									Count++; 
							}
						} else {
							if(GameConst.SelectRoleID[j] != arraySelectID[0] && GameConst.SelectRoleID[j] != arraySelectID[1]) {
								if(Count == RanID) {
									setPlayerAvatar(i, j);
									break;
								} else
									Count++;
							}
						}
					}
				}
				arrayPlayer[i].SetActive(false);
			}
			
			for(int i = 0; i < arraySelectID.Length; i++) {
				if(i == 0) {
					if(GameData.DPlayers.ContainsKey(arraySelectID[i])) {
						GameData.Team.Player.ID = GameData.DPlayers[arraySelectID[i]].ID;
						GameData.Team.Player.Name = GameData.PlayerName (arraySelectID[i]);
						GameData.Team.Player.AILevel = GameData.DPlayers[arraySelectID[i]].AILevel;
					}
				} else {
					if(GameData.DPlayers.ContainsKey(arraySelectID[i])) {
						GameData.TeamMembers[i - 1].Player.ID = GameData.DPlayers[arraySelectID[i]].ID;
						GameData.TeamMembers[i - 1].Player.Name = GameData.PlayerName (arraySelectID[i]);
						GameData.TeamMembers[i - 1].Player.AILevel = GameData.DPlayers[arraySelectID[i]].AILevel;
					}
				}
			}
			changeRoleInfo();
			animatorLeft.SetTrigger("Close");
			animatorRight.SetTrigger("Close");
			Invoke("otherPlayerDoAnimator", 0.5f);
			Invoke("otherPlayerShowTime", 0.65f);
			Invoke("loadingShow", 1f);
			arrayPlayer[0].transform.localEulerAngles = new Vector3(0, 180, 0);
		}
			break;
		case EUIRoleSituation.BackToSelectMe:
			Invoke("showUITriangle", 1.25f);
			Invoke("leftRightShow", 0.5f);
			animatorLoading.SetTrigger("Close");
			
			for(int i = 1; i < arrayPlayerPosition.Length; i++) 	
				arrayPlayer[i].SetActive(false);
			uiShowTime.SetActive(true);
			break;
		case EUIRoleSituation.Start:
			SetEnemyMembers ();
			GameData.Team.Player.SetAttribute(GameEnum.ESkillType.Player);
			GameData.Team.Player.SetAvatar();
			GameData.TeamMembers [0].Player.SetAttribute (GameEnum.ESkillType.NPC);
			GameData.TeamMembers [0].Player.SetAvatar ();
			GameData.TeamMembers [1].Player.SetAttribute (GameEnum.ESkillType.NPC);
			GameData.TeamMembers [1].Player.SetAvatar ();

			if (SceneMgr.Get.CurrentScene == ESceneName.Court_0)
				UILoading.UIShow(true, ELoadingGamePic.Game);
			else
				SceneMgr.Get.ChangeLevel (ESceneName.Court_0);

			break;
		case EUIRoleSituation.ListA: // 1
		case EUIRoleSituation.ListB: // 2
			int mIndex;
			if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out mIndex)) {
				int SelectIndex = 0;
				for(int i = 0; i < GameConst.SelectRoleID.Length; i++) {
					if(GameConst.SelectRoleID[i] == arrayUnSelectID[mIndex]) {
						SelectIndex = i;
						break;
					}
				}
				
				setPlayerAvatar((int)state, SelectIndex);
				if(GameData.DPlayers.ContainsKey(arraySelectID[(int)state])) {
					GameData.TeamMembers[(int)state - 1].Player.ID = GameData.DPlayers[arraySelectID[(int)state]].ID;
					GameData.TeamMembers[(int)state - 1].Player.Name = GameData.PlayerName (arraySelectID[(int)state]);
					GameData.TeamMembers[(int)state - 1].Player.AILevel = GameData.DPlayers[arraySelectID[(int)state]].AILevel;
				}
				
				changeRoleInfo ();
			}
			break;
		case EUIRoleSituation.BackToMode:
			Destroy(playerInfoModel);
			UIShow(false);
			UI3D.Get.ShowCamera(false);
			UIGameMode.UIShow (true);
			break;
		}
	}

	private void hideSelectRoleAnimator(){
		if (!isStage) {
			changeBigHead(SelectRoleIndex);
			uiSelect.SetActive(true);
			this.GetComponent<Animator>().enabled = false;
		}
	}

	private void playerDoAnimator(){
		arrayPlayer[0].SetActive(true);
		arrayAnimator[0].SetTrigger("SelectDown");
		EffectManager.Get.PlayEffect("FX_SelectDown", Vector3.zero, null, null, 1f);
	}
	
	private void playerShowTime (){
		arrayPlayer[0].transform.localPosition = new Vector3(0, -0.9f, 0);
	}

	private void otherPlayerDoAnimator(){
		arrayPlayer[1].transform.localPosition = new Vector3(0.5f , 3f, 2.87f);
		arrayPlayer[2].transform.localPosition = new Vector3(-0.5f, 3f, 2.58f);
		arrayPlayer[1].SetActive(true);
		arrayPlayer[2].SetActive(true);
		arrayAnimator[1].SetTrigger("SelectDown");
		arrayAnimator[2].SetTrigger("SelectDown");
		EffectManager.Get.PlayEffect("FX_SelectDown", new Vector3(1,0,1.7f), null, null, 1f);
		EffectManager.Get.PlayEffect("FX_SelectDown", new Vector3(-1.7f,0,1.7f), null, null, 1f);
	}

	private void otherPlayerShowTime(){
		arrayPlayer[1].transform.localPosition = new Vector3(0.5f , -0.6f, 2.87f);
		arrayPlayer[2].transform.localPosition = new Vector3(-0.5f, -0.6f, 2.58f);
	}

	private void loadingShow(){
		animatorLoading.SetTrigger("Open");
	}

	private void leftRightShow(){
		animatorLeft.SetTrigger("Open");
		animatorRight.SetTrigger("Open");
	}
	
	private void showUITriangle(){
//		UIAttributes.Get.Triangle.SetActive (true);
		mUIAttributes.SetVisible(true);
		mUIAttributes.Play();
		uiSelect.SetActive(true);
	}

	public void SelectFriendMode() {
		arraySelectID[0] = GameData.Team.Player.ID;
		arrayPlayerData[0] = GameData.Team.Player;

		GameObject temp = arrayPlayer [0];
		ModelManager.Get.SetAvatar(ref arrayPlayer[0], GameData.Team.Player.Avatar, GameData.DPlayers [GameData.Team.Player.ID].BodyType, EAnimatorType.AvatarControl, false, true);

		arrayPlayer[0].name = 0.ToString();
		arrayPlayer[0].transform.parent = playerInfoModel.transform;
		arrayPlayer[0].transform.localPosition = arrayPlayerPosition[0];
		arrayPlayer[0].GetComponent<CapsuleCollider>().enabled = false;
		arrayPlayer[0].AddComponent<SelectEvent>();
		arrayPlayer[0].transform.localPosition = temp.transform.localPosition;
		arrayPlayer[0].transform.localEulerAngles = temp.transform.localEulerAngles;
		arrayPlayer[0].transform.localScale = temp.transform.localScale;
		for (int j = 0; j <arrayPlayer[0].transform.childCount; j++)  { 
			if(arrayPlayer[0].transform.GetChild(j).name.Contains("PlayerMode")) {
				arrayPlayer[0].transform.GetChild(j).localScale = Vector3.one;
				arrayPlayer[0].transform.GetChild(j).localEulerAngles = Vector3.zero;
				arrayPlayer[0].transform.GetChild(j).localPosition = Vector3.zero;
			}
		}
		
		arrayAnimator[0] = arrayPlayer[0].GetComponent<Animator>();
		changeLayersRecursively(arrayPlayer[0].transform, "UI");

		labelPlayerName.text = GameData.PlayerName (GameConst.SelectRoleID [0]);
		SetBodyPic(ref spritePlayerBodyPic, GameData.DPlayers [GameConst.SelectRoleID [0]].BodyType);
		uiBack.SetActive(false);

		doubleClickTime = 1;
		UIState(EUIRoleSituation.ChooseRole);

		uiSelect.SetActive(false);
	}

	private bool isStage {
		get {return GameData.DStageData.ContainsKey(GameData.StageID); }
	}
}

