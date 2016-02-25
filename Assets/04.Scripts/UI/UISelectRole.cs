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

	private static int[] selectRoleID = new int[6]{11, 12, 13, 31, 32, 33};
	private const int MaxValue = 100;
	private const float X_Partner = 2.6f;
	private const float Y_Partner = 0;
	private const float Z_Partner = 0.1f;
	private const float Y_Player = 0.25f;

	private float roleFallTime = 0;
	private int selectRoleIndex = 0;
	private float doubleClickTime = 3;

	public GameObject playerInfoModel = null;
	public TPlayer [] arrayPlayerData = new TPlayer[3];
	private Vector3 [] arrayPlayerPosition = new Vector3[3];
	private GameObject [] arrayPlayer = new GameObject[3];
	private List<TPlayer> playerList = new List<TPlayer>();

	private GameObject uiChangPlayerA;
	private GameObject uiChangPlayerB;
	private GameObject uiRedPoint;

    private UILabel labelStrategy;
	private UILabel [] labelsSelectABName = new UILabel[2];

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
	}

	protected override void InitCom() {
        GameObject obj = GameObject.Find("PlayerModel");
        if (!obj) {
		    playerInfoModel = new GameObject();
            playerInfoModel.name = "PlayerModel";
        } else
            playerInfoModel = obj;

		arrayPlayerPosition [0] = new Vector3 (0, 0, 0);
		arrayPlayerPosition [1] = new Vector3 (3, 0, 0);
		arrayPlayerPosition [2] = new Vector3 (-3, 0, 0);

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
					arrayPlayer [i].transform.localPosition = new Vector3 (0, 0.25f, 0);
					arrayPlayer[i].transform.localEulerAngles = new Vector3(0, 180, 0);
				}else 
				if(i == 1) {
					arrayPlayer[i].transform.localPosition = new Vector3(2.6f, 0, Z_Partner);
					arrayPlayer[i].transform.localEulerAngles = new Vector3(0, -150, 0);
				}else 
				if(i == 2) {
					arrayPlayer[i].transform.localPosition = new Vector3(-2.6f, 0, Z_Partner);
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

		for(int i = 0; i < arrayPlayerPosition.Length; i++) 		
			arrayPlayer[i].SetActive(false);

        labelStrategy.text = TextConst.S(15002 + GameData.Team.Player.Strategy);
		arrayPlayer[0].transform.localPosition = new Vector3(0, 4.5f, 0);
		Invoke("playerDoAnimator", 0.95f);
		Invoke("playerShowTime", 1.1f);

        selectFriendMode();
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
        if(GameData.IsInstance) {
            Visible = false;
            if (SceneMgr.Get.CurrentScene != ESceneName.Lobby) {
                UILoading.OpenUI = UILoading.OpenInstanceUI;
                SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            } else
                UIInstance.Get.ShowByStageID(GameData.StageID);
        } else {
            Visible = false;
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

        for (int i = 0; i < arrayPlayer.Length; i++)
            if (arrayPlayer[i])
                arrayPlayer[i].SetActive(false);
        
        if (index == 1) {
            uiChangPlayerA.transform.localPosition = new Vector3(-400, -360, 0);
            uiChangPlayerB.SetActive(false);
        }

        arrayPlayer[index].SetActive(true);
        arrayPlayer[index].transform.localPosition = new Vector3(-X_Partner, 0, Z_Partner);
        //arrayPlayer[index].transform.localEulerAngles = new Vector3(0, 150, 0);
        UISkillFormation.Visible = false;
        UISelectPartner.Visible = true;
        UISelectPartner.Get.InitMemberList(ref playerList, ref arrayPlayerData, index);
    }

	public void OnSkillCard() {
        UISelectPartner.Visible = false;
        UISkillFormation.Visible = true;
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

            if (roleIndex > 0)
				labelsSelectABName[roleIndex - 1].text = arrayPlayerData[roleIndex].Name;
		}
	}

	public void SetEnemyMembers() {
		if (GameData.IsMainStage) {
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

	private void waitLookFriends() {
        GameData.Team.InitFriends();
	    InitPlayerList(ref GameData.Team.Friends);
	}

    public void InitPlayer() {
        uiChangPlayerB.SetActive(true);
        uiChangPlayerA.transform.localPosition = new Vector3(400, -360, 0);
        for (int i = 0; i < arrayPlayer.Length; i++)
            if (arrayPlayer[i]) {
                arrayPlayer[i].SetActive(true);
            }
        
        if (arrayPlayer[1])
            arrayPlayer[1].transform.localPosition = new Vector3(X_Partner, 0, Z_Partner);
    }

    public void LoadStage(int stageID) {
        GameData.StageID = stageID;
        UIMainLobby.Get.HideAll(false);
        Visible = true;
		if (StageTable.Ins.GetByID(GameData.StageID).IsOnlineFriend) {
            if (DateTime.UtcNow > GameData.Team.FreshFriendTime.ToUniversalTime()) {
                SendHttp.Get.FreshFriends(waitLookFriends, true);
				if (UILoading.Visible)
					UILoading.Get.ProgressValue = 0.7f;
			} else 
                waitLookFriends();
		} else
			InitPlayerList(ref StageTable.Ins.GetByID(GameData.StageID).FriendID);
	}

	private void selectFriendMode() {
		doubleClickTime = 1;

        GameData.Team.Player.Init();
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

        uiChangPlayerA.SetActive(true);
        uiChangPlayerB.SetActive(true);
        uiRedPoint.SetActive(PlayerPrefs.HasKey(ESave.NewCardFlag.ToString()));
        arrayAnimator[0].SetTrigger("Idle");

        for(int i = 1; i < arrayPlayerPosition.Length; i++) {
            int index = getFreeListIndex(i);
            SetPlayerAvatar(i, index);
            arrayPlayer[i].SetActive(false);
        }

        Invoke("otherPlayerDoAnimator", 0.5f);
        Invoke("otherPlayerShowTime", 0.65f);
        arrayPlayer[0].transform.localEulerAngles = new Vector3(0, 180, 0);
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
            enterPVP();
        }
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
}
