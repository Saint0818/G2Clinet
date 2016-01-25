using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using GameEnum;
using GameStruct;
using UnityEngine;

public enum EUIRoleSituation {
	ListA = 1,
	ListB = 2,
	SelectRole = 3,
	ChooseRole = 5,
	BackToSelectMe = 6,
	Start = 7
}

public class UISelectRole : UIBase {
	private static UISelectRole instance = null;
	private const string UIName = "UISelectRole";

	private static int[] selectRoleID = new int[6]{11, 12, 13, 31, 32, 33};
	private const int MaxValue = 100;
	private const float X_Partner = 2.6f;
	private const float Y_Partner = 0;
	private const float Z_Partner = 0.75f;
	private const float Y_Player = 0;

	private float roleFallTime = 0;
	private int selectRoleIndex = 0;
	private float doubleClickTime = 3;
	private UIAttributes mUIAttributes;

	public GameObject playerInfoModel = null;
	public TPlayer [] arrayPlayerData = new TPlayer[3];
	private Vector3 [] arrayPlayerPosition = new Vector3[3];
	private GameObject [] arrayPlayer = new GameObject[3];
	private GameObject [] buttonSelectRole = new GameObject[6];
	private List<TPlayer> playerList = new List<TPlayer>();

	private Animator animatorLeft;
	private Animator animatorRight;
	private Animator animatorLoading;

	private GameObject uiSelect;
	private GameObject uiShowTime;
	private GameObject uiCharacterCheck;
	private GameObject uiInfoRange;
	private GameObject uiPartnerList;
	private GameObject uiChangPlayerA;
	private GameObject uiChangPlayerB;
	private GameObject uiRedPoint;

	private UILabel labelPlayerName;
    private UILabel labelStrategy;
	private UISprite spritePlayerBodyPic;

	private UILabel [] labelsSelectABName = new UILabel[2];
	private GameObject [] uiSelectAList = new GameObject[3];
	private GameObject [] uiSelectBList = new GameObject[3];
	private UILabel [] labelsSelectAListName = new UILabel[3];
	private UILabel [] labelsSelectBListName = new UILabel[3];
	private UISprite [] spritesSelectABBody = new UISprite[2];
	private UISprite [] spritesSelectAListPic = new UISprite[3];
	private UISprite [] spritesSelectAListBigPic = new UISprite[3];
	private UISprite [] spritesSelectBListPic = new UISprite[3];
	private UISprite [] spritesSelectBListBigPic = new UISprite[3];
	private UISprite [] spritesLine = new UISprite[6];
	private UISprite [] spritesBigHead = new UISprite[6]; 

	private Animator [] arrayAnimator = new Animator[3];
	private GameObject [] arrayNamePic = new GameObject[6]; 
	private float [] arrayOldNameValue = new float[6];
	private float [] arrayNewNameValue = new float[6];

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
			if (!instance) {
				instance = LoadUI(UIName) as UISelectRole;
			}

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

        if (!isShow)
            UI3DSelectRole.UIShow(false);
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
		if (selectRoleIndex >= 0 && selectRoleIndex < spritesLine.Length) {
			if(spritesLine[selectRoleIndex].fillAmount < 1)
				spritesLine[selectRoleIndex].fillAmount += 0.1f;		
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
	}
	
	public void InitPlayerList(ref int[] ids) {
		playerList.Clear();
        if (ids != null) {
    		for (int i = 0; i < ids.Length; i ++) {
    			if (GameData.DPlayers.ContainsKey(ids[i])) {
    				TPlayer player = new TPlayer();
    				player.SetID(ids[i]);
    				player.Name = GameData.DPlayers[ids[i]].Name;
    				player.RoleIndex = i;
    				playerList.Add(player);
    			}
    		}
    		randomPlayerList();
    		ModelManager.Get.LoadAllSelectPlayer(ref ids);
        }
	}

    public void InitPlayerList(ref Dictionary<string, TFriend> players) {
		playerList.Clear();
		if (players != null) {
            foreach (KeyValuePair<string, TFriend> item in players.ToList()) {
                if (item.Value.FightCount > 0 && item.Value.Identifier != GameData.Team.Identifier) {
                    if (item.Value.Kind == EFriendKind.Advice || item.Value.Kind == EFriendKind.Friend) {
                        TFriend friend = item.Value;
                        friend.Player.Identifier = item.Value.Identifier;
                        friend.Player.FriendKind = item.Value.Kind;
                        friend.Player.FightCount = item.Value.FightCount;
                        playerList.Add(friend.Player);
                    }
                }
            }
		}

        const int pNum = 3;
        if (playerList.Count < pNum) {
            int num = Mathf.Min(pNum, selectRoleID.Length);

			for (int i = 0; i < num; i ++) {
				if (GameData.DPlayers.ContainsKey(selectRoleID[i])) {
					TPlayer player = new TPlayer();
					player.SetID(selectRoleID[i]);
					player.Name = GameData.DPlayers[selectRoleID[i]].Name;
					playerList.Add(player);
                    if (playerList.Count >= pNum)
						break;
				}
			}
		}


        playerList = playerList.OrderBy(x => -x.CombatPower()).ToList();
        for (int i = 0; i < playerList.Count; i++) {
            TPlayer player = playerList[i];
            player.RoleIndex = i;
            playerList[i] = player;
        }
		//randomPlayerList();
	}

	private void randomPlayerList() {
		if (playerList.Count > 1) {
			for (int i = 0; i < playerList.Count; i++) {
				int j = UnityEngine.Random.Range(0, playerList.Count-1);
				if (i != j) {
					TPlayer player = playerList[i];
					playerList[i] = playerList[j];
					playerList[j] = player;
				}
			}

			for (int i = 0; i < playerList.Count; i++) {
				TPlayer player = playerList[i];
				player.RoleIndex = i;
				playerList[i] = player;
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
			arrayNamePic[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString() + "/Sprite");
			buttonSelectRole[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString());
			spritesLine[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString() + "/SpriteLine").GetComponent<UISprite>();
			spritesLine[i].fillAmount = 0;
			if (i < playerList.Count)
				spritesBigHead[i].spriteName = playerList[i].FacePicture;
			else
				buttonSelectRole[i].SetActive(false);
		}

		SetBtnFun (UIName + "/Right/CharacterCheck", DoChooseRole);
		SetBtnFun (UIName + "/Left/Back", DoBackToSelectMe);
		SetBtnFun (UIName + "/Right/GameStart", DoStart);
		SetBtnFun (UIName + "/Top/SelectA/PlayerNameA", OnChangePlayer);
        SetBtnFun (UIName + "/Top/SelectB/PlayerNameB", OnChangePlayer);
		SetBtnFun (UIName + "/Bottom/SkillCard", OnSkillCard);
        SetBtnFun (UIName + "/Bottom/StrategyBtn/", OnStrategy);

		GameObject.Find (UIName + "/Left/SelectCharacter/Back").SetActive(false);

		animatorLeft = GameObject.Find (UIName + "/Left").GetComponent<Animator>();
		animatorRight = GameObject.Find (UIName + "/Right").GetComponent<Animator>();
		animatorLoading = GameObject.Find (UIName + "/Center/ViewLoading").GetComponent<Animator>();

		uiSelect = GameObject.Find (UIName + "/Left/Select");
		uiSelect.SetActive(false);
		uiShowTime = GameObject.Find(UIName + "/Center/ShowTimeCollider");
		uiCharacterCheck = GameObject.Find(UIName + "/Right/CharacterCheck");
		uiInfoRange = GameObject.Find(UIName + "/Right/InfoRange");
        labelStrategy = GameObject.Find (UIName + "/Bottom/StrategyBtn/StrategyLabel").GetComponent<UILabel>();
		labelPlayerName = GameObject.Find (UIName + "/Right/InfoRange/PlayerName/Label").GetComponent<UILabel>();
		spritePlayerBodyPic = GameObject.Find (UIName + "/Right/InfoRange/BodyType/SpriteType").GetComponent<UISprite>();
        labelsSelectABName[0] = GameObject.Find(UIName + "/Top/SelectA/PlayerNameA/Label").GetComponent<UILabel>();
        spritesSelectABBody[0] = GameObject.Find(UIName + "/Top/SelectA/PlayerNameA/SpriteType").GetComponent<UISprite>();
        labelsSelectABName[1] = GameObject.Find(UIName + "/Top/SelectB/PlayerNameB/Label").GetComponent<UILabel>();
        spritesSelectABBody[1] = GameObject.Find(UIName + "/Top/SelectB/PlayerNameB/SpriteType").GetComponent<UISprite>();

		UIEventListener.Get(GameObject.Find(UIName + "/Right/InfoRange/UIAttributeHexagon")).onClick = OnClickSixAttr;
		UIEventListener.Get(GameObject.Find(UIName + "/Center/ShowTimeCollider")).onClick = DoPlayerAnimator;

		for(int i = 0; i < labelsSelectAListName.Length; i++) {
			uiSelectAList[i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString());
			labelsSelectAListName [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString() + "/PlayerName").GetComponent<UILabel>();
			spritesSelectAListPic [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString() + "/BodyType").GetComponent<UISprite>();
			spritesSelectAListBigPic [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString() + "/SpriteFace").GetComponent<UISprite>();
			SetBtnFun(UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString(), DoListA);
		}

		for(int i = 0; i < labelsSelectBListName.Length; i++) {
			uiSelectBList[i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString());
			labelsSelectBListName [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString() + "/PlayerName").GetComponent<UILabel>();
			spritesSelectBListBigPic [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString() + "/SpriteFace").GetComponent<UISprite>();
			spritesSelectBListPic [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString() + "/BodyType").GetComponent<UISprite>();
			SetBtnFun(UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString(), DoListB);
		}

        GameObject obj = GameObject.Find("UISelectRole/Right/InfoRange/UIAttributeHexagon");
        mUIAttributes = obj.GetComponent<UIAttributes>();
        mUIAttributes.PlayScale(1.5f); // 1.5 是 try and error 的數值, 看起來效果比較順暢.

		uiPartnerList = GameObject.Find(UIName + "/Center/ViewLoading/PartnerList");
		uiChangPlayerA = GameObject.Find(UIName + "/Top/SelectA");
		uiChangPlayerB = GameObject.Find(UIName + "/Top/SelectB");
		uiRedPoint = GameObject.Find(UIName + "/Bottom/SkillCard/RedPoint");

		uiRedPoint.SetActive(false);
		uiPartnerList.SetActive(false);
		uiChangPlayerA.SetActive(false);
		uiChangPlayerB.SetActive(false);
	}

	protected override void InitData() {
        InitPlayerList(ref selectRoleID);
		for(int i = 0; i < arrayPlayerPosition.Length; i++) {
			if (i < playerList.Count) {
				arrayPlayerData[i] = playerList[i];
				arrayPlayer[i] = new GameObject();
				arrayPlayer[i].name = i.ToString();
				arrayPlayer[i].transform.parent = playerInfoModel.transform;
				GameObject obj = ModelManager.Get.SetAvatar(ref arrayPlayer[i], arrayPlayerData[i].Avatar, arrayPlayerData[i].BodyType, EAnimatorType.AvatarControl, false);
				arrayAnimator[i] = arrayPlayer[i].GetComponent<Animator>();
				arrayPlayer[i].transform.localPosition = arrayPlayerPosition[i];

				if(i == 0) {
					arrayPlayer[i].transform.localPosition = Vector3.zero;
					arrayPlayer[i].transform.localEulerAngles = new Vector3(0, 180, 0);
					labelPlayerName.text = arrayPlayerData[i].Name;
					SetBodyPic(ref spritePlayerBodyPic, arrayPlayerData[i].BodyType);
				}else 
				if(i == 1) {
					arrayPlayer[i].transform.localPosition = new Vector3(2.6f, 0, 0.75f);
					arrayPlayer[i].transform.localEulerAngles = new Vector3(0, -150, 0);
				}else 
				if(i == 2) {
					arrayPlayer[i].transform.localPosition = new Vector3(-2.6f, 0, 0.75f);
					arrayPlayer[i].transform.localEulerAngles = new Vector3(0, 150, 0);
				}
				
				if(i == 0)
					arrayPlayer[i].transform.localScale = new Vector3(1, 1, 1);
				else
					arrayPlayer[i].transform.localScale = new Vector3(1, 1, 1);
				
                LayerMgr.Get.SetLayer(obj, ELayer.Default);
                obj.transform.localScale = Vector3.one;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localPosition = Vector3.zero;
			}
		}

		arrayPlayerData[1] = new TPlayer();
		arrayPlayerData[2] = new TPlayer();
		arrayPlayerData[1].RoleIndex = -1;
		arrayPlayerData[2].RoleIndex = -1;
		//if (playerList.Count > 0)
		//	playerList.RemoveAt(0);

		for(int i = 0; i < arrayPlayerPosition.Length; i++) 		
			arrayPlayer[i].SetActive(false);

        labelStrategy.text = TextConst.S(15002 + GameData.Team.Player.Strategy);
		arrayPlayer[0].transform.localPosition = new Vector3(0, 4, 0);
		Invoke("playerDoAnimator", 0.95f);
		Invoke("playerShowTime", 1.1f);
		Invoke("hideSelectRoleAnimator", 2.5f);

		setTriangleData();
	}

	private void UIState(EUIRoleSituation state) {
		switch (state) {
		case EUIRoleSituation.SelectRole:
			uiPartnerList.SetActive(false);
			uiChangPlayerA.SetActive(false);
			uiChangPlayerB.SetActive(false);
			
			int roleIndex;
			if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out roleIndex)) {
				if(selectRoleIndex != roleIndex) {
					changeBigHead(roleIndex);
					selectRoleIndex = roleIndex;
					for(int i = 0; i < spritesLine.Length; i++)
						spritesLine[i].fillAmount = 0;
					
					SetPlayerAvatar(0, roleIndex);
					arrayPlayer[0].transform.localEulerAngles = new Vector3(0, 180, 0);
					setTriangleData();
				}
			}
			
			break;
		case EUIRoleSituation.ChooseRole:
			uiPartnerList.SetActive(false);
			uiChangPlayerA.SetActive(true);
			uiChangPlayerB.SetActive(true);
			uiRedPoint.SetActive(PlayerPrefs.HasKey(ESave.NewCardFlag.ToString()));
			uiSelect.SetActive(false);
			arrayAnimator[0].SetTrigger("Idle");
			mUIAttributes.SetVisible(false);
			uiShowTime.SetActive(false);
			
			for(int i = 1; i < arrayPlayerPosition.Length; i++) {
				int index = getFreeListIndex(i);
				SetPlayerAvatar(i, index);
				arrayPlayer[i].SetActive(false);
			}
			
			changeRoleInfo();
			animatorLeft.SetTrigger("Close");
			animatorRight.SetTrigger("Close");
			Invoke("otherPlayerDoAnimator", 0.5f);
			Invoke("otherPlayerShowTime", 0.65f);
			Invoke("loadingShow", 1f);
			arrayPlayer[0].transform.localEulerAngles = new Vector3(0, 180, 0);
			
			break;
		case EUIRoleSituation.BackToSelectMe:
			if (isStage) {
				UIShow(false);
                if (SceneMgr.Get.CurrentScene != ESceneName.Lobby) {
                    UILoading.OpenUI = UILoading.OpenStageUI;
				    SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
                } else {
                    UIShow(false);
					LobbyStart.Get.EnterLobby();
                }
			} else {
				Invoke("showUITriangle", 1.25f);
				Invoke("leftRightShow", 0.5f);
				animatorLoading.SetTrigger("Close");
				for(int i = 1; i < arrayPlayerPosition.Length; i++) 	
					arrayPlayer[i].SetActive(false);
				
				uiShowTime.SetActive(true);
			}
			
			break;
            case EUIRoleSituation.Start:
                if (GameData.StageID == 10)
                {
                    enterPVP();
                }
                else
                {
                    if (GameStart.Get.ConnectToServer)
                        mainStageStart(GameData.StageID);
                    else
                        enterGame();
                }
			break;
		case EUIRoleSituation.ListA: // 1
		case EUIRoleSituation.ListB: // 2
			int mIndex;
			if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out mIndex)) {
				SetPlayerAvatar((int)state, mIndex);
				changeRoleInfo ();
			}
			
			break;
		}
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
		if(doubleClickTime == 0) {
			doubleClickTime = 1;
			UIState(EUIRoleSituation.ChooseRole);
		}
	}

    public void OnStrategy() {
        UIStrategy.Visible = true;
        UIStrategy.Get.LabelStrategy = labelStrategy;
    }

    public void OnClickPlayer(GameObject go){
        int index = 1;
        int.TryParse(go.name, out index);
        openChangePlayer(index);
    }

	public void OnChangePlayer(){
		int index = 1;
		if (UIButton.current.name == "PlayerNameB")
			index = 2;

        openChangePlayer(index);
	}

    private void openChangePlayer(int index) {
        if (UISelectPartner.Visible)
            return;

        for (int i = 0; i < arrayPlayer.Length; i++)
            if (arrayPlayer[i])
                arrayPlayer[i].SetActive(false);
        
        if (index == 1) {
            uiChangPlayerA.transform.localPosition = new Vector3(-400, -360, 0);
            uiChangPlayerB.SetActive(false);
        }

        arrayPlayer[index].SetActive(true);
        arrayPlayer[index].transform.localPosition = new Vector3(-X_Partner, 0, Z_Partner);
        arrayPlayer[index].transform.localEulerAngles = new Vector3(0, 150, 0);
        UISkillFormation.UIShow(false);
        UISelectPartner.UIShow(true);
        UISelectPartner.Get.InitMemberList(ref playerList, ref arrayPlayerData, index);
    }

	public void OnSkillCard() {
		UISelectPartner.UIShow(false);
		UISkillFormation.UIShow(true);
	}

	public void SelectRole() {
		UIState(EUIRoleSituation.SelectRole);
	}

	public void SetBodyPic(ref UISprite Pic, int Type) {
		switch(Type) {
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

	public void SetPlayerAvatar(int roleIndex, int index) {
		if (index >= 0 && index < playerList.Count) {
			arrayPlayerData[roleIndex] = playerList[index];
			GameObject temp = arrayPlayer [roleIndex];
			GameObject obj = ModelManager.Get.SetAvatar(ref arrayPlayer[roleIndex], arrayPlayerData[roleIndex].Avatar, arrayPlayerData[roleIndex].BodyType, EAnimatorType.AvatarControl, false, true);

			arrayPlayer[roleIndex].name = roleIndex.ToString();
			arrayPlayer[roleIndex].transform.parent = playerInfoModel.transform;
			arrayPlayer[roleIndex].transform.localPosition = arrayPlayerPosition[roleIndex];
            arrayPlayer[roleIndex].AddComponent<SelectEvent>();
            arrayPlayer[roleIndex].AddComponent<SpinWithMouse>();
            UIEventListener.Get (arrayPlayer[roleIndex]).onClick = OnClickPlayer;
			arrayPlayer[roleIndex].transform.localPosition = temp.transform.localPosition;
			arrayPlayer[roleIndex].transform.localEulerAngles = temp.transform.localEulerAngles;
			arrayPlayer[roleIndex].transform.localScale = temp.transform.localScale;
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localPosition = Vector3.zero;
            LayerMgr.Get.SetLayer(obj, ELayer.Default);
			arrayAnimator[roleIndex] = arrayPlayer[roleIndex].GetComponent<Animator>();

			switch(roleIndex) {
			case 0:
				labelPlayerName.text = arrayPlayerData[roleIndex].Name;
				SetBodyPic(ref spritePlayerBodyPic, arrayPlayerData[roleIndex].BodyType);
				break;
			case 1:
			case 2:
				labelsSelectABName[roleIndex - 1].text = arrayPlayerData[roleIndex].Name;
				SetBodyPic(ref spritesSelectABBody[roleIndex - 1], arrayPlayerData[roleIndex].BodyType);
				break;
			}
		}
	}

	public void SetEnemyMembers() {
		if (isStage) {
			int[] ids = StageTable.Ins.GetByID(GameData.StageID).PlayerID;
			int num = Mathf.Min(GameData.EnemyMembers.Length, ids.Length);
			for (int i = 0; i < num; i ++) {
				GameData.EnemyMembers[i].Player.SetID(ids[i]);
				if (GameData.DPlayers.ContainsKey(ids[i])) 
					GameData.EnemyMembers[i].Player.Name = GameData.DPlayers[ids[i]].Name;
			}
        } else {
			int num = Mathf.Min(GameData.EnemyMembers.Length, arrayPlayerData.Length);
			for(int i = 0; i < num; i++) 
				GameData.EnemyMembers[i].Player = playerList[i];
		}
	}

	private void changeRoleInfo () {
		for(int i = 0; i < labelsSelectAListName.Length; i++) {
			if (i < playerList.Count) {
				uiSelectAList[i].SetActive(true);
				labelsSelectAListName [i].text = playerList[i].Name;
				spritesSelectAListBigPic[i].spriteName = playerList[i].FacePicture;
				SetBodyPic(ref spritesSelectAListPic[i], playerList[i].BodyType);

				uiSelectBList[i].SetActive(true);
				labelsSelectBListName [i].text = playerList[i].Name;
				spritesSelectBListBigPic[i].spriteName = playerList[i].FacePicture;
				SetBodyPic(ref spritesSelectBListPic[i], playerList[i].BodyType);
			} else {
				uiSelectAList[i].SetActive(false);
				labelsSelectAListName [i].text = "";
				spritesSelectAListBigPic[i].spriteName = "";
				spritesSelectAListPic[i].spriteName = "";

				uiSelectBList[i].SetActive(false);
				labelsSelectBListName [i].text = "";
				spritesSelectBListBigPic[i].spriteName = "";
				spritesSelectBListPic[i].spriteName = "";
			}
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

	private void setTriangleData() {
		mUIAttributes.SetValue(UIAttributes.EGroup.Block, arrayPlayerData[0].Block / MaxValue);
		mUIAttributes.SetValue(UIAttributes.EGroup.Steal, arrayPlayerData[0].Steal / MaxValue);
		mUIAttributes.SetValue(UIAttributes.EGroup.Point2, arrayPlayerData[0].Point2 / MaxValue);
		mUIAttributes.SetValue(UIAttributes.EGroup.Dunk, arrayPlayerData[0].Dunk / MaxValue);
		mUIAttributes.SetValue(UIAttributes.EGroup.Point3, arrayPlayerData[0].Point3 / MaxValue);
		mUIAttributes.SetValue(UIAttributes.EGroup.Rebound, arrayPlayerData[0].Rebound / MaxValue);
	}
	
	private void hideSelectRoleAnimator(){
		if (!isStage) {
			changeBigHead(selectRoleIndex);
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
		arrayPlayer[0].transform.localPosition = new Vector3(0, Y_Player, 0);
	}

	private void otherPlayerDoAnimator(){
		arrayPlayer[1].transform.localPosition = new Vector3(X_Partner, 3f, Z_Partner);
		arrayPlayer[2].transform.localPosition = new Vector3(-X_Partner, 3f, Z_Partner);
		arrayPlayer[1].SetActive(true);
		arrayPlayer[2].SetActive(true);
		arrayAnimator[1].SetTrigger("SelectDown");
		arrayAnimator[2].SetTrigger("SelectDown");
		EffectManager.Get.PlayEffect("FX_SelectDown", new Vector3(1,0,1.7f), null, null, 1f);
		EffectManager.Get.PlayEffect("FX_SelectDown", new Vector3(-1.7f,0,1.7f), null, null, 1f);
	}

	private void otherPlayerShowTime(){
		arrayPlayer[1].transform.localPosition = new Vector3(X_Partner , Y_Partner, Z_Partner);
		arrayPlayer[2].transform.localPosition = new Vector3(-X_Partner, Y_Partner, Z_Partner);
	}

	private void loadingShow(){
		animatorLoading.SetTrigger("Open");
	}

	private void leftRightShow(){
		animatorLeft.SetTrigger("Open");
		animatorRight.SetTrigger("Open");
	}
	
	private void showUITriangle(){
		mUIAttributes.SetVisible(true);
		mUIAttributes.Play();
		uiSelect.SetActive(true);
	}

	private void waitLookFriends() {
        GameData.Team.InitFriends();
	    InitPlayerList(ref GameData.Team.Friends);
		selectFriendMode();
		UIState(EUIRoleSituation.ChooseRole);
	}

    public void InitPlayer() {
        uiChangPlayerB.SetActive(true);
        uiChangPlayerA.transform.localPosition = new Vector3(400, -360, 0);
        for (int i = 0; i < arrayPlayer.Length; i++)
            if (arrayPlayer[i]) {
                arrayPlayer[i].SetActive(true);
            }
        
        if (arrayPlayer[1]) {
            arrayPlayer[1].transform.localPosition = new Vector3(X_Partner, 0, Z_Partner);
            arrayPlayer[1].transform.localEulerAngles = new Vector3(0, -150, 0);
        }
    }

    public void LoadStage(int stageID) {
        GameData.StageID = stageID;
        UIMainLobby.Get.HideAll();
        UI3DSelectRole.UIShow(true);
        UIShow(true);

		if (StageTable.Ins.GetByID(GameData.StageID).IsOnlineFriend) {
            if (DateTime.UtcNow > GameData.Team.FreshFriendTime.ToUniversalTime()) {
                SendHttp.Get.FreshFriends(waitLookFriends, true);
				if (UILoading.Visible)
					UILoading.Get.ProgressValue = 0.7f;
			} else 
                waitLookFriends();
		} else {
			InitPlayerList(ref StageTable.Ins.GetByID(GameData.StageID).FriendID);
			selectFriendMode();
			UIState(EUIRoleSituation.ChooseRole);
		}
	}

	private void selectFriendMode() {
		//UILoading.UIShow(false);
		//UIShow(true);
		//UI3DSelectRole.UIShow(true);

		uiCharacterCheck.SetActive(false);
		uiInfoRange.SetActive(false);
		uiSelect.SetActive(false);
		doubleClickTime = 1;

		arrayPlayerData[0] = GameData.Team.Player;
		arrayPlayerData[0].RoleIndex = -1;
		GameObject temp = arrayPlayer [0];
		GameObject obj = ModelManager.Get.SetAvatar(ref arrayPlayer[0], GameData.Team.Player.Avatar, GameData.DPlayers [GameData.Team.Player.ID].BodyType, EAnimatorType.AvatarControl, false, true);

		arrayPlayer[0].name = 0.ToString();
		arrayPlayer[0].transform.parent = playerInfoModel.transform;
		arrayPlayer[0].transform.localPosition = arrayPlayerPosition[0];
        arrayPlayer[0].AddComponent<SelectEvent>();
        arrayPlayer[0].AddComponent<SpinWithMouse>();
		arrayPlayer[0].transform.localPosition = temp.transform.localPosition;
		arrayPlayer[0].transform.localEulerAngles = temp.transform.localEulerAngles;
		arrayPlayer[0].transform.localScale = temp.transform.localScale;
        obj.transform.localScale = Vector3.one;
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localPosition = Vector3.zero;
        LayerMgr.Get.SetLayer(obj, ELayer.Default);
		arrayAnimator[0] = arrayPlayer[0].GetComponent<Animator>();

		labelPlayerName.text = arrayPlayerData[0].Name;
		SetBodyPic(ref spritePlayerBodyPic, arrayPlayerData[0].BodyType);
	}

	private int getFreeListIndex(int roleIndex) {
		for (int i = 0; i < playerList.Count; i++) {
			bool flag = true;
			for (int j = 0; j < arrayPlayerData.Length; j++) {
				if (j != roleIndex) {
					if (arrayPlayerData[j].RoleIndex == i) {
						flag = false;
						break;
					}
				}
			}

			if (flag)
				return i;
		}

		return playerList.Count-1;
	}

	public int GetSelectedListIndex(int roleIndex) {
		if (roleIndex >= 0 && roleIndex < arrayPlayerData.Length) 
			return arrayPlayerData[roleIndex].RoleIndex;
		else
			return -1;
	}

	public void DisableRedPoint() {
		uiRedPoint.SetActive(false);
	}

    private void mainStageStart(int stageID)
    {
        var protocol = new MainStageStartProtocol();
        protocol.Send(stageID, waitMainStageStart);
    }

    private void enterGame() {
        for (int i = 0; i < arrayPlayerData.Length; i++)
            GameData.TeamMembers[i].Player = arrayPlayerData[i];

        SetEnemyMembers ();

        int courtNo = StageTable.Ins.GetByID(GameData.StageID).CourtNo;
        if (SceneMgr.Get.CurrentScene == ESceneName.Court + courtNo.ToString())
            UILoading.UIShow(true, ELoading.Game);
        else
            SceneMgr.Get.ChangeLevel (courtNo);
    }

    private void enterPVP()
    {
        for (int i = 0; i < arrayPlayerData.Length; i++)
            GameData.TeamMembers[i].Player = arrayPlayerData[i];
        
        int courtNo = StageTable.Ins.GetByID(GameData.StageID).CourtNo;

        if (SceneMgr.Get.CurrentScene == ESceneName.Court + courtNo.ToString())
            UILoading.UIShow(true, ELoading.Game);
        else
            SceneMgr.Get.ChangeLevel (courtNo);
    }

    private void waitMainStageStart(bool ok, MainStageStartProtocol.Data data)
    {
        if(ok)
            enterGame();
        else
            UIHint.Get.ShowHint(TextConst.S(9514), Color.red);
    }

	private bool isStage {
		get {return StageTable.Ins.HasByID(GameData.StageID);}
	}
}
