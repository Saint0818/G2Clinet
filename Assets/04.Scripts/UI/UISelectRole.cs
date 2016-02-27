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

	private static int[] selectRoleID = new int[6]{20, 21, 22, 31, 32, 33};
	private const int MaxValue = 100;
	private const float X_Partner = 2.6f;
	private const float Y_Partner = 0;
	private const float Z_Partner = 0.1f;
    private const float Y_Down = 4.5f;
	private const float Y_Player = 0.25f;
	private const int playerNum = 3;

	private TStageData stageData;
	private List<TPlayer> playerList = new List<TPlayer>();
	public TPlayer [] playerData = new TPlayer[playerNum];
	private GameObject [] playerObjects = new GameObject[playerNum];
	private GameObject playerInfoModel = null;
	private GameObject uiChangPlayerA;
	private GameObject uiChangPlayerB;
	private GameObject uiRedPoint;
    private UILabel labelStrategy;
	private UILabel [] labelsSelectABName = new UILabel[2];
	private Animator [] arrayAnimator = new Animator[playerNum];

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
                    RemoveUI(UIName);
                else
                    instance.Show(value);
            } else
            if (value)
                Get.Show(value);

            UI3DSelectRole.UIShow(value);
            if (!value) {
                UISelectPartner.Visible = false;
                UISkillFormation.Visible = false;
				UIStrategy.Visible = false;
            }
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
		for (int i = 0; i < arrayAnimator.Length; i++)
			Destroy(arrayAnimator[i]);

		arrayAnimator = new Animator[0];

		for (int i = 0; i < playerObjects.Length; i ++) {
			if (playerObjects[i]) {
				SkinnedMeshRenderer smr = playerObjects[i].GetComponent<SkinnedMeshRenderer>();
				if (smr) {
					Material[] mats = smr.materials;
					for (int j = 0; j < mats.Length; j++) {
						Destroy(mats[j]);
						mats[j] = null;
					}

					smr.materials = new Material[0];
				}

				Destroy(playerObjects[i]);
			}
		}

		playerObjects = new GameObject[0];
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

    		ModelManager.Get.LoadAllSelectPlayer(ref ids);
            initTeammateList();
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

        initTeammateList();
	}

	protected override void InitCom() {
        GameObject obj = GameObject.Find("PlayerModel");
        if (!obj) {
		    playerInfoModel = new GameObject();
            playerInfoModel.name = "PlayerModel";
        } else
            playerInfoModel = obj;

		SetBtnFun (UIName + "/Left/Back", OnExit);
		SetBtnFun (UIName + "/Right/GameStart", OnStart);
		SetBtnFun (UIName + "/Top/SelectA/PlayerNameA", OnChangePlayer);
        SetBtnFun (UIName + "/Top/SelectB/PlayerNameB", OnChangePlayer);
		SetBtnFun (UIName + "/Bottom/SkillCard", OnSkillCard);
        SetBtnFun (UIName + "/Bottom/StrategyBtn/", OnStrategy);

        labelStrategy = GameObject.Find (UIName + "/Bottom/StrategyBtn/StrategyLabel").GetComponent<UILabel>();
        labelsSelectABName[0] = GameObject.Find(UIName + "/Top/SelectA/PlayerNameA/Label").GetComponent<UILabel>();
        labelsSelectABName[1] = GameObject.Find(UIName + "/Top/SelectB/PlayerNameB/Label").GetComponent<UILabel>();
		uiChangPlayerA = GameObject.Find(UIName + "/Top/SelectA");
		uiChangPlayerB = GameObject.Find(UIName + "/Top/SelectB");
		uiRedPoint = GameObject.Find(UIName + "/Bottom/SkillCard/RedPoint");

		uiRedPoint.SetActive(false);
	}

    private void initTeammateList() {
		GameData.Team.Player.Init();
		playerData[0] = GameData.Team.Player;
		playerData[0].RoleIndex = -1;

		int num = Mathf.Min(2, playerList.Count);
        for (int i = 0; i < num; i++) {
			playerData[i+1] = playerList[i];
            labelsSelectABName[i].text = playerData[i].Name;
        }

		for (int i = 0; i < playerData.Length; i++) {
            playerObjects[i] = new GameObject();
            playerObjects[i].name = i.ToString();
            playerObjects[i].transform.parent = playerInfoModel.transform;
            GameObject obj = ModelManager.Get.SetAvatar(ref playerObjects[i], playerData[i].Avatar, playerData[i].BodyType, EAnimatorType.AvatarControl, false);
            arrayAnimator[i] = playerObjects[i].GetComponent<Animator>();

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

            LayerMgr.Get.SetLayer(obj, ELayer.Default);
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localPosition = Vector3.zero;
        }

        labelStrategy.text = GameData.Team.Player.StrategyText;
        uiRedPoint.SetActive(PlayerPrefs.HasKey(ESave.NewCardFlag.ToString()));
        Invoke("otherPlayerDoAnimator", 0.5f);
        Invoke("otherPlayerShowTime", 0.65f);
        Invoke("playerDoAnimator", 0.95f);
        Invoke("playerShowTime", 1.1f);
    }

	public void InitPartnerPosition() {
		uiChangPlayerB.SetActive(true);
		uiChangPlayerA.transform.localPosition = new Vector3(400, -360, 0);
		for (int i = 0; i < playerObjects.Length; i++)
			if (playerObjects[i])
				playerObjects[i].SetActive(true);

		if (playerObjects[1])
			playerObjects[1].transform.localPosition = new Vector3(X_Partner, 0, Z_Partner);
	}

	public void LoadStage(int stageID) {
		GameData.StageID = stageID;
		stageData = StageTable.Ins.GetByID(GameData.StageID);
		UIMainLobby.Get.HideAll(false);
		Visible = true;

		GameData.Team.Player.Init();
		playerData[0] = GameData.Team.Player;
		if (stageData.IsOnlineFriend) {
			if (DateTime.UtcNow > GameData.Team.FreshFriendTime.ToUniversalTime() && GameData.Team.Friends == null) {
				SendHttp.Get.FreshFriends(waitLookFriends, true);
				if (UILoading.Visible)
					UILoading.Get.ProgressValue = 0.7f;
			} else 
				waitLookFriends(true);
		} else
		if (stageData.FriendID != null)
			InitPlayerList(ref stageData.FriendID);
		else
			InitPlayerList(ref selectRoleID);
	}

	public void OnStart(){
        Visible = false;
        if (GameData.IsPVP)
            SendPVPStart();
        else {
            if (GameStart.Get.ConnectToServer)
                mainStageStart(GameData.StageID);
            else
                enterGame();
        }
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
		int index = 1;
		if (UIButton.current.name == "PlayerNameB")
			index = 2;

        openChangePlayer(index);
	}

    private void openChangePlayer(int index) {
        if (UISelectPartner.Visible)
            return;

        for (int i = 0; i < playerObjects.Length; i++)
            if (playerObjects[i])
                playerObjects[i].SetActive(false);
        
        if (index == 1) {
            uiChangPlayerA.transform.localPosition = new Vector3(-400, -360, 0);
            uiChangPlayerB.SetActive(false);
        }

        playerObjects[index].SetActive(true);
        playerObjects[index].transform.localPosition = new Vector3(-X_Partner, 0, Z_Partner);
        UISkillFormation.Visible = false;
        UISelectPartner.Visible = true;
        UISelectPartner.Get.InitMemberList(ref playerList, ref playerData, index);
    }

	public void OnSkillCard() {
        UISelectPartner.Visible = false;
		UIStrategy.Visible = false;
		UISkillFormation.Visible = true;
	}

	public void SetPlayerAvatar(int roleIndex, int index) {
		if (index >= 0 && index < playerList.Count) {
			playerData[roleIndex] = playerList[index];
			GameObject temp = playerObjects [roleIndex];
			GameObject obj = ModelManager.Get.SetAvatar(ref playerObjects[roleIndex], playerData[roleIndex].Avatar, playerData[roleIndex].BodyType, EAnimatorType.AvatarControl, false, true);

			playerObjects[roleIndex].name = roleIndex.ToString();
			playerObjects[roleIndex].transform.parent = playerInfoModel.transform;
            playerObjects[roleIndex].AddComponent<SelectEvent>();
            playerObjects[roleIndex].AddComponent<SpinWithMouse>();
            UIEventListener.Get (playerObjects[roleIndex]).onClick = OnClickPlayer;
			playerObjects[roleIndex].transform.localPosition = temp.transform.localPosition;
			playerObjects[roleIndex].transform.localEulerAngles = temp.transform.localEulerAngles;
			playerObjects[roleIndex].transform.localScale = temp.transform.localScale;
            obj.transform.localScale = Vector3.one;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localPosition = Vector3.zero;
			LayerMgr.Get.SetLayer(obj, ELayer.Default);
			arrayAnimator[roleIndex] = playerObjects[roleIndex].GetComponent<Animator>();

            if (roleIndex > 0)
				labelsSelectABName[roleIndex - 1].text = playerData[roleIndex].Name;
		}
	}

	public void SetEnemyMembers() {
        if (!GameData.IsPVP) {
			int[] ids = stageData.PlayerID;
			int num = Mathf.Min(GameData.EnemyMembers.Length, ids.Length);
			for (int i = 0; i < num; i ++) {
				GameData.EnemyMembers[i].Player.SetID(ids[i]);
				if (GameData.DPlayers.ContainsKey(ids[i])) 
					GameData.EnemyMembers[i].Player.Name = GameData.DPlayers[ids[i]].Name;
			}
        } else {
			int num = Mathf.Min(GameData.EnemyMembers.Length, playerList.Count);
			for(int i = 0; i < num; i++) 
				GameData.EnemyMembers[i].Player = playerList[i];
		}
	}

	private void playerDoAnimator() {
		playerObjects[0].SetActive(true);
		arrayAnimator[0].SetTrigger("SelectDown");
		EffectManager.Get.PlayEffect("FX_SelectDown", Vector3.zero, null, null, 1f);
	}
	
	private void playerShowTime () {
		playerObjects[0].transform.localPosition = new Vector3(0, Y_Player, 0);
	}

	private void otherPlayerDoAnimator() {
        //playerObjects[1].transform.localPosition = new Vector3(X_Partner, Y_Down, Z_Partner);
        //playerObjects[2].transform.localPosition = new Vector3(-X_Partner, Y_Down, Z_Partner);
		playerObjects[1].SetActive(true);
		playerObjects[2].SetActive(true);
		arrayAnimator[1].SetTrigger("SelectDown");
		arrayAnimator[2].SetTrigger("SelectDown");
		EffectManager.Get.PlayEffect("FX_SelectDown", new Vector3(1,0,1.7f), null, null, 1f);
		EffectManager.Get.PlayEffect("FX_SelectDown", new Vector3(-1.7f,0,1.7f), null, null, 1f);
	}

	private void otherPlayerShowTime(){
		playerObjects[1].transform.localPosition = new Vector3(X_Partner , Y_Partner, Z_Partner);
		playerObjects[2].transform.localPosition = new Vector3(-X_Partner, Y_Partner, Z_Partner);
	}

    private void waitLookFriends(bool ok) {
        if (ok) {
            GameData.Team.InitFriends();
    	    InitPlayerList(ref GameData.Team.Friends);
        } else
            InitPlayerList(ref selectRoleID);
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
        for (int i = 0; i < playerData.Length; i++)
            GameData.TeamMembers[i].Player = playerData[i];

        SetEnemyMembers ();

		int courtNo = stageData.CourtNo;
        if (SceneMgr.Get.CurrentScene == ESceneName.Court + courtNo.ToString())
            UILoading.UIShow(true, ELoading.Game);
        else
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
            TPVPStart data = JsonConvert.DeserializeObject <TPVPStart>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.PVPTicket = data.PVPTicket;
            GameData.Team.PVPCD = data.PVPCD;
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
