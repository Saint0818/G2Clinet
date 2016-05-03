using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using GameEnum;
using GameStruct;
using UnityEngine;

public class UISelectRole : UIBase {
	private static UISelectRole instance = null;
	private const string UIName = "UISelectRole";

	private static int[] selectRoleID = new int[6]{101, 102, 103, 104, 105, 106};
    private const int kind_item = 17;
	private const int MaxValue = 100;
	private const float X_Partner = 2.6f;
	private const float Y_Partner = 0;
	private const float Z_Partner = 0.1f;
    private const float Y_Down = 4.5f;
	private const float Y_Player = 0;
	private const int playerNum = 3;
	private const int itemNum = 2;

	private int tempIndex = -1;
	private Vector3 tempPosition;
	private Vector3 tempRotation;

	private TStageData stageData;
	private List<TPlayer> playerList = new List<TPlayer>();
	public TPlayer [] playerData = new TPlayer[playerNum];
	private GameObject [] playerObjects = new GameObject[playerNum*2];
	private GameObject uiBottom;
	private GameObject uiTopPVE;
    private GameObject uiTopPVP;
    private GameObject uiCenterPVP;
	private GameObject playerInfoModel = null;
	private GameObject uiRedPoint;
    private GameObject uiPVPWin;
    private GameObject uiPVPLose;
    private GameObject uiPVPFresh;
	private GameObject uiEquipement;
    private UILabel labelStrategy;
	private UILabel labelPVPFresh;
	private UILabel labelMyPower;
	private UILabel labelOpponentPower;
	private UILabel labelPVPWin;
	private UILabel labelPVPLose;
    private UILabel [] labelPlayerName = new UILabel[2];
    private UILabel [] labelCombatPower = new UILabel[playerNum];
    private UILabel [] labelPVPPlayerName = new UILabel[playerNum*2];
    private UILabel [] labelPVPCombatPower = new UILabel[playerNum*2];
    private TAvatarLoader [] avatarLoaders = new TAvatarLoader[playerNum*2];
	private UISprite [] spriteEquipEffect = new UISprite[itemNum];
	private UIEquipPartSlot [] equipSlot = new UIEquipPartSlot[itemNum];
	private UIValueItemData [] equipItemData = new UIValueItemData[itemNum];

	public static bool Visible {
		get {
			if (instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

        set {
            if (instance) {
                if (!value)
                    RemoveUI(instance.gameObject);
                else
                    instance.Show(value);
            } else
            if (value)
                Get.Show(value);

            if (!value) {
                UI3DSelectRole.UIShow(false);
                UI3DPVP.Visible = false;
                UISelectPartner.Visible = false;
                UISkillFormation.Visible = false;
				UIStrategy.Visible = false;
				UIEquipList.Visible = false;
            }

            if(value)
                Statistic.Ins.LogScreen(5);
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
        avatarLoaders = new TAvatarLoader[0];

		for (int i = 0; i < playerObjects.Length; i ++) {
			if (playerObjects[i]) {
				Destroy(playerObjects[i]);
			}
		}

		playerObjects = new GameObject[0];
	}

    protected override void InitCom() {
        GameObject obj = GameObject.Find("TeammatesPool");
        if (!obj) {
            playerInfoModel = new GameObject();
            playerInfoModel.name = "TeammatesPool";
        } else
            playerInfoModel = obj;

        SetBtnFun (UIName + "/Left/Back", OnExit);
        SetBtnFun (UIName + "/Right/GameStart", OnStart);
		SetBtnFun (UIName + "/Top/PVE/SelectA/1", OnChangePlayer);
		SetBtnFun (UIName + "/Top/PVE/SelectB/2", OnChangePlayer);
		SetBtnFun (UIName + "/Top/PVP/Player1/1", OnChangePlayer);
		SetBtnFun (UIName + "/Top/PVP/Player2/2", OnChangePlayer);
        SetBtnFun (UIName + "/Bottom/SkillCard", onSkillCard);
        SetBtnFun (UIName + "/Bottom/StrategyBtn/", OnStrategy);
		SetBtnFun (UIName + "/Left/ResteBtn/", OnRefreshOpponent);

		uiBottom = GameObject.Find (UIName + "/Bottom");
		uiEquipement = GameObject.Find (UIName + "/Bottom/EquipItemView");
        uiPVPWin = GameObject.Find (UIName + "/Right/Win");
        uiPVPLose = GameObject.Find (UIName + "/Right/Lose");
        uiPVPFresh = GameObject.Find (UIName + "/Left/ResteBtn");
		labelPVPWin = GameObject.Find (UIName + "/Right/Win/Label").GetComponent<UILabel>();
		labelPVPLose = GameObject.Find (UIName + "/Right/Lose/Label").GetComponent<UILabel>();
        labelPVPFresh = GameObject.Find (UIName + "/Left/ResteBtn/PriceLabel").GetComponent<UILabel>();
        labelMyPower = GameObject.Find (UIName + "/Center/PVP/CombatGroup/CombatLabel1").GetComponent<UILabel>();
		labelOpponentPower = GameObject.Find (UIName + "/Center/PVP/CombatGroup/CombatLabel0").GetComponent<UILabel>();
        labelStrategy = GameObject.Find (UIName + "/Bottom/StrategyBtn/StrategyLabel").GetComponent<UILabel>();
        labelCombatPower[0] = GameObject.Find(UIName + "/Top/PVE/SelectMe/CombatPower/Label").GetComponent<UILabel>();
        labelCombatPower[1] = GameObject.Find(UIName + "/Top/PVE/SelectA/CombatPower/Label").GetComponent<UILabel>();
        labelCombatPower[2] = GameObject.Find(UIName + "/Top/PVE/SelectB/CombatPower/Label").GetComponent<UILabel>();
        labelPlayerName[0] = GameObject.Find(UIName + "/Top/PVE/SelectA/PlayerNameA/Label").GetComponent<UILabel>();
        labelPlayerName[1] = GameObject.Find(UIName + "/Top/PVE/SelectB/PlayerNameB/Label").GetComponent<UILabel>();
        uiRedPoint = GameObject.Find(UIName + "/Bottom/SkillCard/RedPoint");

        for (int i = 0; i < playerNum * 2; i++) {
            labelPVPCombatPower[i] = GameObject.Find(UIName + string.Format("/Top/PVP/Player{0}/CombatPower/Label", i)).GetComponent<UILabel>();
            labelPVPPlayerName[i] = GameObject.Find(UIName + string.Format("/Top/PVP/Player{0}/PlayerName/Label", i)).GetComponent<UILabel>();
        }

		for (int i = 0; i < 2; i++) {
			string path = UIName + string.Format ("/Bottom/EquipItemView/Slot{0}/{0}", i + 6);
			equipSlot[i] = GameObject.Find (path).GetComponent<UIEquipPartSlot>();
			equipSlot [i].Index = i + 6;
			equipSlot [i].GetComponentInChildren<UIEquipItem> ().OnClickListener += OnEquip;
			equipSlot [i].GetComponentInChildren<UIEquipItem> ().name = (i+6).ToString ();
			spriteEquipEffect [i] = GameObject.Find (path + "/Effect").GetComponent<UISprite>();
			spriteEquipEffect [i].gameObject.SetActive (false);
		}

        uiTopPVE = GameObject.Find (UIName + "/Top/PVE");
        uiTopPVP = GameObject.Find (UIName + "/Top/PVP");
        uiCenterPVP = GameObject.Find (UIName + "/Center/PVP");

        showPVPUI(false);
        uiCenterPVP.SetActive(false);
        uiTopPVE.SetActive(false);
        uiRedPoint.SetActive(false);
    }

    protected override void InitData() {
        labelStrategy.text = GameData.Team.Player.StrategyText;
        uiRedPoint.SetActive(PlayerPrefs.HasKey(ESave.NewCardFlag.ToString()));
        for (int i = 0; i < GameData.TeamMembers.Length; i++) {
            GameData.TeamMembers[i] = new TTeam();
            GameData.EnemyMembers[i] = new TTeam();
        }

		initItem ();
    }

	private void initItem() {
		for (int i = 0; i < equipSlot.Length; i++) {
            if (GameData.Team.Player.ValueItems.ContainsKey (i+kind_item)) {
                int id = GameData.Team.Player.ValueItems[i+kind_item].ID;
                if (GameData.DItemData.ContainsKey(id))
    				setEquiptItem(i, UIValueItemDataBuilder.Build(
                        GameData.DItemData[GameData.Team.Player.ValueItems[i+kind_item].ID],
                        GameData.Team.Player.ValueItems[i+kind_item].InlayItemIDs,
                        GameData.Team.Player.ValueItems[i+kind_item].Num));
            } else {
                equipItemData[i] = UIValueItemDataBuilder.BuildEmpty();
                equipSlot [i].Set (equipItemData[i], GameData.Team.getStorageBestValueItemTotalPoints (i+kind_item) > 0);
            }       
		}
	}

	private void setEquiptItem(int index, UIValueItemData itemData) {
        bool redFlag = !itemData.IsValid() && GameData.Team.getStorageBestValueItemTotalPoints (index+kind_item) > 0;
		equipItemData [index] = itemData;
        equipSlot [index].Set (equipItemData[index], redFlag);
		spriteEquipEffect [index].gameObject.SetActive (false);
		foreach (KeyValuePair<EAttribute, UIValueItemData.BonusData> item in itemData.Values) {
			spriteEquipEffect [index].spriteName = item.Value.Icon;
			spriteEquipEffect [index].gameObject.SetActive (true);
		}
	}

    private void showPVPUI(bool flag) {
        uiPVPWin.SetActive(flag);
        uiPVPLose.SetActive(flag);
        uiPVPFresh.SetActive(flag);
        uiTopPVP.SetActive(flag);
    }

    private int[] addMercenary(int[] ids) {
        int len = ids.Length;
        if (stageData.MercenaryID != null && stageData.MercenaryID.Length > 0) {
            len += stageData.MercenaryID.Length;
            int [] ay = new int[len];
            for (int i = 0; i < ids.Length; i++)
                ay[i] = ids[i];

            for (int i = ids.Length; i < len; i++)
                ay[i] = stageData.MercenaryID[i-ids.Length];

            return ay;
        } else 
            return ids;
    }

    private void addMercenary() {
        if (stageData.MercenaryID != null && stageData.MercenaryID.Length > 0) {
            for (int i = 0; i < stageData.MercenaryID.Length; i ++) {
                if (GameData.DPlayers.ContainsKey(stageData.MercenaryID[i])) {
                    TPlayer player = new TPlayer();
                    player.SetID(stageData.MercenaryID[i]);
                    player.Name = GameData.DPlayers[stageData.MercenaryID[i]].Name;
                    player.RoleIndex = playerList.Count;
                    player.FriendKind = EFriendKind.Mercenary;
                    playerList.Add(player);
                }
            }
        }
    }

	private void initPlayerList(int[] ids) {
		playerList.Clear();
        if (ids != null) {
            if (stageData.IDKind != TStageData.EKind.PVP && !GameData.Team.RentExpire)
                addMercenary();

            for (int i = 0; i < ids.Length; i ++) {
                if (GameData.DPlayers.ContainsKey(ids[i])) {
                    TPlayer player = new TPlayer();
                    player.SetID(ids[i]);
                    player.Name = GameData.DPlayers[ids[i]].Name;
                    player.RoleIndex = playerList.Count;
                    playerList.Add(player);
                }
            }

            if (stageData.IDKind == TStageData.EKind.PVP)
                initPVPTeammate();
            else {
                if (GameData.Team.RentExpire)
                    addMercenary();

                initPVETeammate();
            }
        }
	}

    public void initPlayerList(ref Dictionary<string, TFriend> players) {
		playerList.Clear();
		if (players != null) {
            foreach (KeyValuePair<string, TFriend> item in players.ToList()) {
                if (item.Value.FightCount > 0 && item.Value.Identifier != GameData.Team.Identifier) {
                    if (item.Value.Kind == EFriendKind.Advice || item.Value.Kind == EFriendKind.Friend) {
                        bool flag = true;
                        if (stageData.IDKind == TStageData.EKind.PVP) {
                            for (int i = 0; i < GameData.PVPEnemyMembers.Length; i++)
                                if (item.Value.Identifier == GameData.PVPEnemyMembers[i].Identifier) {
                                    flag = false;
                                    break;
                                }

                        }
                            
                        if (flag) {
                            TFriend friend = item.Value;
                            friend.Player.Identifier = item.Value.Identifier;
                            friend.Player.FriendKind = item.Value.Kind;
                            friend.Player.FightCount = item.Value.FightCount;
                            playerList.Add(friend.Player);
                        }
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

        if (stageData.IDKind == TStageData.EKind.PVP)
            initPVPTeammate();
        else
            initPVETeammate();
	}

    private void initPlayerAvatar(int roleIndex, ref TPlayer player, GameObject anchorObj) {
        TLoadParameter p = new TLoadParameter(ELayer.Default, roleIndex.ToString());
        p.AddSpin = true;
        p.AddEvent = roleIndex > 0 && roleIndex < 3;
        avatarLoaders[roleIndex] = TAvatarLoader.Load(
            player.BodyType, player.Avatar, ref playerObjects [roleIndex], anchorObj, p);

        if (stageData.IDKind == TStageData.EKind.PVP) {
            labelPVPCombatPower[roleIndex].text = ((int)player.CombatPower()).ToString();
			if (roleIndex > 0) {
				labelPVPPlayerName[roleIndex].text = player.Name;
				if (roleIndex < 3)
                	UIEventListener.Get (playerObjects[roleIndex]).onClick = OnClickPlayer;
            }
        } else {
            labelCombatPower[roleIndex].text = ((int)player.CombatPower()).ToString();
            if (roleIndex > 0) {
				labelPlayerName[roleIndex - 1].text = player.Name;
				UIEventListener.Get (playerObjects[roleIndex]).onClick = OnClickPlayer;
            }
        }
    }

    private void initPVPTeammate() {
		bool init = playerObjects [0] == null;

		if (init) {
			GameData.Team.PlayerInit ();
			playerData [0] = GameData.Team.Player;
			playerData [0].RoleIndex = -1;

            if (string.IsNullOrEmpty(playerData[0].Name))
                playerData[0].Name = TextConst.S(3404);

	        int num = Mathf.Min(2, playerList.Count);
	        for (int i = 0; i < num; i++) {
	            playerData[i+1] = playerList[i];
	            labelPlayerName[i].text = playerList[i].Name;
	        }

	        for (int i = 0; i < playerData.Length; i++) 
	            initPlayerAvatar(i, ref playerData[i], UI3DPVP.Get.PlayerAnchor[i]);
		}

        for (int i = 0; i < GameData.PVPEnemyMembers.Length; i++) 
            initPlayerAvatar(i+3, ref GameData.PVPEnemyMembers[i].Player, UI3DPVP.Get.PlayerAnchor[i+3]);
      
        if (init) {
            for (int i = 0; i < avatarLoaders.Length; i++)
                avatarLoaders[i].SetTrigger ("Walk");

            Invoke("playerPVPShow", 1.1f);
        } else {
            for (int i = 3; i < avatarLoaders.Length; i++)
                avatarLoaders[i].SetTrigger ("SelectDown");
        }

		refreshOpponetUI ();
		computePower ();
    }

	private void refreshOpponetUI() {
		int pvpLv = GameData.Team.PVPLv;
		if (GameData.DPVPData.ContainsKey (pvpLv)) {
			labelPVPFresh.text = GameData.DPVPData [pvpLv].SearchCost.ToString();
			labelPVPFresh.color = GameData.CoinEnoughTextColor (GameData.Team.Money >= GameData.DPVPData [pvpLv].SearchCost, 1);
		}
	}

	private void computePower() {
		float power = 0;
		/*for (int i = 0; i < playerData.Length; i++) 
			power += playerData [i].CombatPower ();*/
		
		labelMyPower.text = string.Format("{0:F0}", GameData.Team.PVPIntegral);

		power = 0;
		for (int i = 0; i < GameData.PVPEnemyMembers.Length; i++)
			power += GameData.PVPEnemyMembers [i].PVPIntegral; //.Player.CombatPower ();

		labelOpponentPower.text = string.Format("{0:F0}", power / 3);

		int winpoint = 0;
		int lostpoint = 0;
		int pvpLv = GameData.Team.PVPLv;
		int calculate = (int)(Mathf.Abs(GameData.Team.PVPIntegral - GameData.Team.PVPEnemyIntegral) / GameData.DPVPData[GameData.Team.PVPLv].Calculate);
		if (GameData.Team.PVPIntegral > GameData.Team.PVPEnemyIntegral) {
			winpoint = (GameData.DPVPData[pvpLv].BasicScore - calculate);
			lostpoint = (GameData.DPVPData[pvpLv].BasicScore + calculate);
		} else 
		if (GameData.Team.PVPIntegral < GameData.Team.PVPEnemyIntegral) {
			winpoint = GameData.DPVPData[pvpLv].BasicScore + calculate;
			lostpoint = GameData.DPVPData[pvpLv].BasicScore - calculate;
		}
		else {
			winpoint = GameData.DPVPData[pvpLv].BasicScore;
			lostpoint = winpoint;
		}

		labelPVPWin.text = TextConst.S(9725) + "+" + winpoint.ToString();
		labelPVPLose.text = TextConst.S(9726) + "-" + lostpoint.ToString();
	}

    private void initPVETeammate() {
        GameData.Team.PlayerInit();
    	playerData[0] = GameData.Team.Player;
    	playerData[0].RoleIndex = -1;

        if (string.IsNullOrEmpty(playerData[0].Name))
            playerData[0].Name = TextConst.S(3404);

		int num = Mathf.Min(2, playerList.Count);
        for (int i = 0; i < num; i++) {
			playerData[i+1] = playerList[i];
            labelPlayerName[i].text = playerList[i].Name;
        }

		for (int i = 0; i < playerData.Length; i++) {
            initPlayerAvatar(i, ref playerData[i], playerInfoModel);

			switch (i) {
			    case 0:
                    playerObjects[0].transform.localPosition = new Vector3(0, Y_Down, 0);
                    playerObjects[0].transform.localEulerAngles = new Vector3(0, 180, 0);
				    break;
			    case 1:
                    playerObjects[i].transform.localPosition = new Vector3(2.6f, Y_Down, Z_Partner);
                    playerObjects[i].transform.localEulerAngles = new Vector3(0, -150, 0);
    				break;
    			case 2:
                    playerObjects[i].transform.localPosition = new Vector3(-2.6f, Y_Down, Z_Partner);
                    playerObjects[i].transform.localEulerAngles = new Vector3(0, 150, 0);
    				break;
			}
        }

        Invoke("otherPlayerDoAnimator", 0.5f);
        Invoke("otherPlayerShowTime", 0.65f);
        Invoke("playerDoAnimator", 0.95f);
        Invoke("playerShowTime", 1.1f);
    }

	public void InitPartnerPosition() {
		uiBottom.SetActive (true);
		if (stageData.IDKind == TStageData.EKind.PVP) {
            showPVPUI(true);
		} else
			uiTopPVE.SetActive (true);

		for (int i = 0; i < playerObjects.Length; i++)
			if (playerObjects[i])
				playerObjects[i].SetActive(true);

		if (tempIndex > 0 && playerObjects [tempIndex]) {
			playerObjects [tempIndex].transform.localPosition = tempPosition;
			playerObjects [tempIndex].transform.localEulerAngles = tempRotation;
		}
	}

    private void initEnemy() {
        if (stageData.IDKind == TStageData.EKind.PVP) {
            int num = Mathf.Min(GameData.EnemyMembers.Length, GameData.PVPEnemyMembers.Length);
            for (int i = 0; i < num; i ++) {
                GameData.EnemyMembers[i] = GameData.PVPEnemyMembers[i];
            }
        } else
        if (stageData.FriendKind == 1) {
            int count = 0;
            foreach (KeyValuePair<string, TFriend> item in GameData.Team.Friends) {
                if (item.Value.Kind == EFriendKind.Advice && item.Value.Identifier != playerData[1].Identifier && item.Value.Identifier != playerData[2].Identifier) {
                    GameData.EnemyMembers[count].Player = item.Value.Player;
                    count++;
                    if (count >= GameData.EnemyMembers.Length)
                        break;
                }
            }

            if (count < 3) {
                for (int i = 0; i < stageData.PlayerID.Length; i ++) {
                    if (GameData.DPlayers.ContainsKey(stageData.PlayerID[i])) {
                        GameData.EnemyMembers[count].Player.SetID(stageData.PlayerID[i]);
                        GameData.EnemyMembers[count].Player.Name = GameData.DPlayers[stageData.PlayerID[i]].Name;
                        count++;
                        if (count >= GameData.EnemyMembers.Length)
                            break;
                    }
                }
            }
        } else {
            int num = Mathf.Min(GameData.EnemyMembers.Length, stageData.PlayerID.Length);
            for (int i = 0; i < num; i ++) {
                if (GameData.DPlayers.ContainsKey(stageData.PlayerID[i])) {
                    GameData.EnemyMembers[i].Player.SetID(stageData.PlayerID[i]);
                    GameData.EnemyMembers[i].Player.Name = GameData.DPlayers[stageData.PlayerID[i]].Name;
                }
            }
        }
    }

	public void LoadStage(int stageID) {
		GameData.StageID = stageID;
		stageData = StageTable.Ins.GetByID(GameData.StageID);

        UIMainLobby.Get.Hide(false);
        UIResource.Get.Hide();

        Visible = true;

        if (stageData.IDKind == TStageData.EKind.PVP) {
            UI3DPVP.Visible = true;
            uiCenterPVP.SetActive(true);
            showPVPUI(true);
        } else {
            UI3DSelectRole.UIShow(true);
            uiTopPVE.SetActive(true);
        }

		GameData.Team.PlayerInit();
		playerData[0] = GameData.Team.Player;
        if (stageData.IsOnlineFriend) {
            if (GameData.Team.FreshFriendTime.ToUniversalTime() <= DateTime.UtcNow) {
				SendHttp.Get.FreshFriends(waitLookFriends, true);
				if (UILoading.Visible)
					UILoading.Get.ProgressValue = 0.7f;
			} else 
				waitLookFriends(true);
		} else
        if (stageData.FriendID != null) {
            initPlayerList(stageData.FriendID);
        } else
			initPlayerList(selectRoleID);
	}

	public void OnEquip() {
		int index = -1;
		if (int.TryParse (UIButton.current.name, out index)) {
			UIEquipList.Visible = true;
			UIEquipList.Get.InitItemData (index-6, onItemChange);
		}
	}

	private void onItemChange(int index, UIValueItemData item) {
		setEquiptItem (index, item);
	}

	public void OnStart(){
        if (GameData.IsPVP)
            SendPVPStart();
        else 
            mainStageStart(GameData.StageID);
	}

    public void OnExit() {
        Visible = false;

        if(GameData.IsInstance) {
            if (SceneMgr.Get.CurrentScene != ESceneName.Lobby) {
                UILoading.OpenUI = UILoading.OpenInstanceUI;
                SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            } else 
                UIInstance.Get.ShowByStageID(GameData.StageID);
        } else
        if(GameData.IsPVP) {
            if (SceneMgr.Get.CurrentScene != ESceneName.Lobby) {
                UILoading.OpenUI = UILoading.OpenPVPUI;
                SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            } else 
                UIPVP.UIShow(true);
        } else {
            if (SceneMgr.Get.CurrentScene != ESceneName.Lobby) {
                UILoading.OpenUI = UILoading.OpenStageUI;
                SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            } else 
                UIMainStage.Get.Show(GameData.StageID);
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
		int index = -1;
		if (int.TryParse(UIButton.current.name, out index))
        	openChangePlayer(index);
	}

    private void openChangePlayer(int index) {
        if (UISelectPartner.Visible)
            return;

		uiBottom.SetActive (false);
		uiTopPVE.SetActive (false);
        showPVPUI(false);

        for (int i = 0; i < playerObjects.Length; i++)
            if (playerObjects[i])
                playerObjects[i].SetActive(false);

		tempIndex = index;
		playerObjects[index].SetActive(true);
		tempPosition = playerObjects [index].transform.localPosition;
		tempRotation = playerObjects [index].transform.localEulerAngles;

		if (stageData.IDKind == TStageData.EKind.PVP) {
			playerObjects [index].transform.localEulerAngles = new Vector3 (0, 270, 0);
			if (index == 1)
				playerObjects [index].transform.localPosition = new Vector3 (-1f, 0.9f, 4.8f);
			else
				playerObjects [index].transform.localPosition = new Vector3 (-2f, 0.8f, 5.4f);
		} else {
			playerObjects [index].transform.localEulerAngles = new Vector3 (0, 150, 0);
			playerObjects [index].transform.localPosition = new Vector3 (-X_Partner, 0, Z_Partner);
		}

        UISkillFormation.Visible = false;
        UISelectPartner.Visible = true;
        UISelectPartner.Get.InitMemberList(ref playerList, ref playerData, index, stageData.FriendKind == 3);
    }

    private void onSkillCard()
    {
        UISelectPartner.Visible = false;
        UIStrategy.Visible = false;
        UISkillFormation.Visible = true;

        if(GameData.IsPVP)
            Statistic.Ins.LogEvent(15);
        else if(GameData.IsMainStage)
            Statistic.Ins.LogEvent(54);
        else if(GameData.IsInstance)
            Statistic.Ins.LogEvent(103);
    }

	private void waitPVPGetEnemy(bool ok, WWW www)
	{
	    if(ok)
	    {
	        TPVPEnemyTeams data = JsonConvertWrapper.DeserializeObject<TPVPEnemyTeams>(www.text);

	        Statistic.Ins.LogEvent(13, GameData.Team.Money - data.Money);

	        GameData.Team.Money = data.Money;
	        GameData.Team.PVPEnemyIntegral = data.PVPEnemyIntegral;

	        if(data.Teams != null)
	        {
	            int num = Mathf.Min(data.Teams.Length, GameData.EnemyMembers.Length);
	            for(int i = 0; i < num; i++)
	            {
	                data.Teams[i].PlayerInit();
	                GameData.PVPEnemyMembers[i] = data.Teams[i];
	                GameData.EnemyMembers[i] = data.Teams[i];
	            }

	            initPVPTeammate();
	        }
	    }
	    else
	        UIHint.Get.ShowHint(TextConst.S(255), Color.red);
	}

	private void sendRefreshOpponent() {
		WWWForm form = new WWWForm ();
		form.AddField ("Identifier", GameData.Team.Identifier);	
		form.AddField ("Kind", 1);
		SendHttp.Get.Command (URLConst.PVPGetEnemy, waitPVPGetEnemy, form, true);
	}

	public void OnRefreshOpponent() {
		int pvpLv = GameData.Team.PVPLv;
		if (GameData.DPVPData.ContainsKey (pvpLv))
			CheckMoney (GameData.DPVPData [pvpLv].SearchCost, true, string.Format(TextConst.S(9740), GameData.DPVPData [pvpLv].SearchCost), sendRefreshOpponent, refreshOpponetUI);
	}
		
	public void SelectPartner(int roleIndex, int index) {
		if (index >= 0 && index < playerList.Count) {
			playerData[roleIndex] = playerList[index];

			GameObject anchor = playerInfoModel;
			if (stageData.IDKind == TStageData.EKind.PVP)
				anchor = UI3DPVP.Get.PlayerAnchor [roleIndex];
			
			initPlayerAvatar(roleIndex, ref playerData[roleIndex], anchor);
            avatarLoaders[roleIndex].SetTrigger("SelectDown");
		}
	}

	private void playerPVPShow() {
        for (int i = 0; i < playerObjects.Length; i++)
            avatarLoaders[i].SetTrigger("Show");
	}

	private void playerDoAnimator() {
		playerObjects[0].SetActive(true);
        avatarLoaders[0].SetTrigger("SelectDown");
		EffectManager.Get.PlayEffect("FX_SelectDown", Vector3.zero, null, null, 1f);
	}
	
	private void playerShowTime () {
		playerObjects[0].transform.localPosition = new Vector3(0, Y_Player, 0);
	}

	private void otherPlayerDoAnimator() {
        playerObjects[1].SetActive(true);
		playerObjects[2].SetActive(true);
        avatarLoaders[1].SetTrigger("SelectDown");
        avatarLoaders[2].SetTrigger("SelectDown");
		EffectManager.Get.PlayEffect("FX_SelectDown", new Vector3(2.6f,0,0.1f), null, null, 1f);
		EffectManager.Get.PlayEffect("FX_SelectDown", new Vector3(-2.6f,0,0.1f), null, null, 1f);
	}

	private void otherPlayerShowTime(){
		playerObjects[1].transform.localPosition = new Vector3(X_Partner , Y_Partner, Z_Partner);
		playerObjects[2].transform.localPosition = new Vector3(-X_Partner, Y_Partner, Z_Partner);
	}

    private void waitLookFriends(bool ok) {
        if (ok) {
            GameData.Team.InitFriends();
    	    initPlayerList(ref GameData.Team.Friends);
        } else
        if (stageData.FriendID != null) 
            initPlayerList(stageData.FriendID);
        else
            initPlayerList(selectRoleID);
	}
    
	private int getFreeListIndex(int roleIndex) {
		for (int i = 0; i < playerList.Count; i++) {
			bool flag = true;
			for (int j = 0; j < playerData.Length; j++) {
				if (j != roleIndex) {
					if (playerData[j].RoleIndex == i) {
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
		if (roleIndex >= 0 && roleIndex < playerData.Length) 
			return playerData[roleIndex].RoleIndex;
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
        Visible = false;
        for (int i = 0; i < playerData.Length; i++)
            GameData.TeamMembers[i].Player = playerData[i];

        initEnemy ();

        int eventID = 0;
        if (stageData.IDKind == TStageData.EKind.MainStage)
            eventID = 57;
        else
        if (stageData.IDKind == TStageData.EKind.Instance)
            eventID = 106;
        else
        if (stageData.IDKind == TStageData.EKind.PVP)
            eventID = 18;

        if (eventID > 0) {
            for (int i = 1; i < playerData.Length; i++) {
                int id = eventID;
                if (playerData[i].FriendKind != EFriendKind.Friend && playerData[i].FriendKind != EFriendKind.Mercenary)
                    id++;
                
                Statistic.Ins.LogEvent(id, playerData[i].ID.ToString());
            }
        }

		int courtNo = stageData.CourtNo;
        SceneMgr.Get.ChangeLevel (courtNo);
    }

    private void SendPVPStart()
    {
        WWWForm form = new WWWForm();
        SendHttp.Get.Command(URLConst.PVPStart, WaitPVPStart, form, true);  
    }
   
    private void WaitPVPStart(bool ok, WWW www)
    {
        if (ok)
        {
            TPVPStart data = JsonConvertWrapper.DeserializeObject <TPVPStart>(www.text);
            GameData.Team.PVPTicket = data.PVPTicket;
            GameData.Team.PVPCD = data.PVPCD;

            Statistic.Ins.LogEvent(17, GameData.Team.Player.Lv.ToString());

            enterGame();
        }
    }

    private void waitMainStageStart(bool ok, MainStageStartProtocol.Data data)
    {
        if(ok)
            enterGame();
        else
            UIHint.Get.ShowHint(TextConst.S(9514), Color.red);
    }
}
