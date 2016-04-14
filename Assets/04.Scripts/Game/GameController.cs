﻿using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using DG.Tweening;
using G2;
using GameEnum;
using GamePlayStruct;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EGameTest {
	None,
	All,
    AttackA,
    AttackB,
    Dunk,
    Block,
	Rebound,
    Edit,
    OneByOne,
	Pass,
	Alleyoop,
	CrossOver,
	Shoot,
	AnimationUnit,
	Skill,
	PassiveSkill
}

public struct TPlayerSkillLV {
	public int SkillID;
	public int SkillLV;
	public TSkillData Skill;
}

struct TPlayerDisData {
	public PlayerBehaviour Player;
	public float Distance;
}



public class GameController : KnightSingleton<GameController>
{
	public OnSkillDCComplete onSkillDCComplete = null;
    public bool IsStart = false;
	public bool IsFinish = false;
	public bool IsReset = false;
	public bool IsJumpBall = false;
	private bool isPassing = false;
//    public float CoolDownPass = 0;
    public readonly CountDownTimer PassCD = new CountDownTimer(GameConst.CoolDownPassTime);
    public float CoolDownCrossover = 0;
    public float ShootDistance = 0;
	public float StealBtnLiftTime = 1f;

	public float GameTime = 0;

	private float passingStealBallTime = 0;

    private GameObject defPointObject = null;
    public GameObject playerInfoModel = null;

    public bool HasBallOwner { get { return BallOwner != null; } }
	public PlayerBehaviour BallOwner; // 持球的球員.
	public PlayerBehaviour Joysticker; // 玩家控制的球員.

    /// <summary>
    /// 投籃出手的人. OnShooting 會有值, 得分後才會設定為 null.
    /// </summary>
    [CanBeNull]public PlayerBehaviour Shooter;

    /// <summary>
    /// 傳球時, 準備接球的人.
    /// </summary>
    public PlayerBehaviour Catcher;

    /// <summary>
    /// 傳球的人.
    /// </summary>
	public PlayerBehaviour Passer;
    //助攻者
    public PlayerBehaviour Assistant;

    /// <summary>
    /// 撿球的人.
    /// </summary>
	public PlayerBehaviour PickBallPlayer;
//	private GameObject ballHolder = null; // 這到底是什麼? 好像也從來沒有人設定過...

    // 0:C, 1:F, 2:G
    private readonly Vector2[] mHomePositions =
    {
        new Vector2(0, 14.5f), // C
        new Vector2(5.3f, 11), // F
        new Vector2(-5.3f, 11) // G
    };

    /*
	 *     0	 5
	 * 		 1 4
	 * 	   2     3
	 */
    private readonly Vector3[] mJumpBallPos = //new Vector3[6]
    {
        new Vector3(-3.5f, 0, -3), // G
        new Vector3(0, 0, -1.2f), // C
        new Vector3(3.5f, 0, -3), // F
        new Vector3(3.5f, 0, 3), // G
        new Vector3(0, 0, 1.2f), // C
        new Vector3(-3.5f, 0, 3) // F
    };

	private readonly List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();

	//Alleyoop
	public bool IsCatcherAlleyoop = false;

	public EGameSituation Situation = EGameSituation.None;
	public TGameRecord GameRecord = new TGameRecord();
	private TMoveData moveData = new TMoveData();
	private TTacticalData attackTactical;
	private TTacticalData defTactical;
	private TTacticalAction[] tacticalActions;

	//Shoot
	private float shootAngle = 55;
	private float extraScoreRate = 0;
	private float angleByPlayerHoop = 0;
	private EDoubleType doubleType = EDoubleType.None;
	private WeightedRandomizer<ESkillSituation> shootRandomizer = new WeightedRandomizer<ESkillSituation>();

	//Rebound
	public bool IsReboundTime = false;
	private EBallState ballState = EBallState.None;

	private TPVPResult beforeTeam = new TPVPResult();
	private TPVPResult afterTeam = new TPVPResult();
	private bool isEndShowScene = false;

    public EBallState BallState
    {
        set{
            ballState = value;

            if (CourtMgr.Get.RealBallCompoment){
                if (ballState != EBallState.None)
                    CourtMgr.Get.RealBallCompoment.ShowBallSFX();
                else
                    CourtMgr.Get.RealBallCompoment.HideBallSFX();
            }
        }

        get{ 
            return ballState;
        }
    }

	//Basket
	public EBasketSituation BasketSituation;
   
	//Effect
	public GameObject[] passIcon = new GameObject[3];

	//Player Anger 每秒回復的士氣值（浮動值會依據套卡而變化）
	private float recoverTime = 1;
	private float recoverAngerBase = 0.5f;

	//SelectMe
	private GameObject playerSelectMe;
	private Transform PlayerSelectArrow;
	private Vector3 playerTarget;
	private Vector3 enemyTarget;
	public PlayerBehaviour NpcSelectMe;

	public EPlayerState testState = EPlayerState.Shoot0;
	public EPlayerState[] ShootStates = {EPlayerState.Shoot0, EPlayerState.Shoot1, EPlayerState.Shoot2, EPlayerState.Shoot3, EPlayerState.Shoot6, EPlayerState.Layup0, EPlayerState.Layup1, EPlayerState.Layup2, EPlayerState.Layup3};
	public static Dictionary<EAnimatorState, bool> LoopStates = new Dictionary<EAnimatorState, bool>();

	//Instant
	public TStageData StageData = new TStageData();

	public float RecordTimeScale = 1;

	private float finishWaitTime = 2;

	//debug value
	[HideInInspector]public int PlayCount = 0;
	[HideInInspector]public int SelfWin = 0;
	[HideInInspector]public int NpcWin = 0;
	[HideInInspector]public int shootSwishTimes = 0;
	[HideInInspector]public int shootScoreSwishTimes = 0;
	[HideInInspector]public int shootTimes = 0;
	[HideInInspector]public int shootScoreTimes = 0;

	[HideInInspector]public float randomrate = 0;
	[HideInInspector]public float normalRate = 0;
	[HideInInspector]public float uphandRate = 0;
	[HideInInspector]public float downhandRate = 0;
	[HideInInspector]public float layupRate = 0;
	[HideInInspector]public float nearshotRate = 0;
	
	void OnDestroy() {
        UIGame.Visible = false;

		for (int i = 0; i < PlayerList.Count; i++) 
			Destroy(PlayerList[i]);

		PlayerList.Clear();

        DestroyImmediate(ModelManager.Get.gameObject);
	}

    [UsedImplicitly]
    private void Awake()
    {
        // 這是 AI 整個框架初始化的起點.
        AIController.Get.ChangeState(EGameSituation.None);
        AIController.Get.PlayerAttackTactical = GameData.Team.AttackTactical;
		UITransition.Visible = true;
		EffectManager.Get.LoadGameEffect();
		//ModelManager.Get.PreloadAnimator();
        initModel();
		StageData.Clear();
		InitAniState();
    }

    private void initModel() {
        defPointObject = Resources.Load("Character/Component/DefPoint") as GameObject;

        playerInfoModel = GameObject.Find("PlayerInfoModel");
        if (!playerInfoModel) {
            playerInfoModel = new GameObject();
            playerInfoModel.name = "PlayerInfoModel";
        }
    }

    private void initRigbody(GameObject obj)
    {
        Rigidbody rig = obj.GetComponent<Rigidbody> ();
        if (rig == null)
            rig = obj.AddComponent<Rigidbody>();

        rig.mass = 0.1f;
        rig.drag = 10f;
        rig.freezeRotation = true;
    }

    void InitAniState()
	{
		if(!LoopStates.ContainsKey(EAnimatorState.Dribble))
			LoopStates.Add(EAnimatorState.Dribble, false);

		if(!LoopStates.ContainsKey(EAnimatorState.Defence))
			LoopStates.Add(EAnimatorState.Defence, false);  

		if(!LoopStates.ContainsKey(EAnimatorState.HoldBall))
			LoopStates.Add(EAnimatorState.HoldBall, false);

		if(!LoopStates.ContainsKey(EAnimatorState.Idle))
			LoopStates.Add(EAnimatorState.Idle, false);

		if(!LoopStates.ContainsKey(EAnimatorState.Run))
			LoopStates.Add(EAnimatorState.Run, false);        
	}

	void OnApplicationFocus(bool focusStatus)
	{
		if (!focusStatus) {

		} else {
			#if UNITY_EDITOR
			#else
			GameRecord.ExitCount++;
			#endif
		}
	}

    public void InitGame()
    {
		IsPassing = false;
		Shooter = null;
		BallOwner = null; 
		Joysticker = null;
		Catcher = null;
		Passer = null;
		PickBallPlayer  = null;

		for (var i = 0; i < PlayerList.Count; i ++)
			if (PlayerList[i].PlayerRefGameObject) 
				Destroy (PlayerList[i].PlayerRefGameObject);

        PlayerList.Clear();

		StateChecker.InitState();
        CreateTeam();
		SetBallOwnerNull (); 
		MissionChecker.Get.SetPlayer(Joysticker);
		Joysticker.ReviveAnger(GameData.Team.InitGetAP());
    }

	public void LoadStage(int id) {
		if (StageTable.Ins.HasByID(id)) {
            StageData = StageTable.Ins.GetByID(id);
			GameTime = StageData.BitNum[0];
			StageData.WinValue = StageData.BitNum[1];
			UIGame.Get.MaxScores[0] = StageData.WinValue;
			UIGame.Get.MaxScores[1] = StageData.WinValue;
			CourtMgr.Get.ChangeBasket(StageData.CourtNo);
			MissionChecker.Get.Init(StageData);

			if (GameData.Team.Player.Lv == 0 && StageData.IsTutorial) {
				GameData.Team.StageTutorial = StageData.ID + 1;
				WWWForm form = new WWWForm();
				form.AddField("StageID", StageData.ID);
				form.AddField("Cause", 0);
                form.AddField("Company", GameData.Company);
				SendHttp.Get.Command(URLConst.AddStageTutorial, null, form, false);
			}
		} else {
			StageData.Clear();
			int a = LobbyStart.Get.GameWinTimeValue > 0 ? 1 : 0;
			int b = LobbyStart.Get.GameWinValue > 0 ? 2 : 0;
			StageData.Hint = b.ToString() + a.ToString();
			StageData.Bit0Num = LobbyStart.Get.GameWinTimeValue;
			StageData.Bit1Num = LobbyStart.Get.GameWinValue;
			StageData.WinValue = LobbyStart.Get.GameWinValue;
			GameTime = LobbyStart.Get.GameWinTimeValue;
			UIGame.Get.MaxScores[0] = LobbyStart.Get.GameWinValue;
			UIGame.Get.MaxScores[1] = LobbyStart.Get.GameWinValue;
			CourtMgr.Get.ChangeBasket(0);
			MissionChecker.Get.Init(StageData);
		}

		UIGame.Get.InitUI();
		UIInGameMission.Get.InitView(id);
		UIInGameMission.UIShow(false);
		#if !UNITY_EDITOR
		if (StageData.IsTutorial)
			UIGame.Get.InitTutorialUI();
		#endif

		CameraMgr.Get.SetCourtCamera (ESceneName.Court + StageData.CourtNo.ToString());
		InitGame();	
	}

    public void StageStart() {
        if (LobbyStart.Get.TestMode == EGameTest.None && LobbyStart.Get.OpenTutorial && GameData.DStageTutorial.ContainsKey(StageData.ID)) 
            GamePlayTutorial.Get.SetTutorialData(StageData.ID);

        if (Situation == EGameSituation.None)
            CameraMgr.Get.SetCameraSituation(ECameraSituation.Show); 
    }

	public void StartGame(bool jumpBall=true) {
		IsReset = false;
		IsJumpBall = false;
		SetPlayerLevel();
        /*
		if (GameStart.Get.TestMode == EGameTest.None && SendHttp.Get.CheckNetwork(false)) {
			string str = PlayerPrefs.GetString(SettingText.GameRecord);
			if (str != "") {
				WWWForm form = new WWWForm();
				form.AddField("GameRecord", str);
				form.AddField("Start", PlayerPrefs.GetString(SettingText.GameRecordStart));
				form.AddField("End", PlayerPrefs.GetString(SettingText.GameRecordEnd));
                SendHttp.Get.Command(URLConst.GameRecord, waitGameRecord, form, false);
			}
		}*/

		switch (LobbyStart.Get.TestMode) {
            case EGameTest.Rebound:
        	CourtMgr.Get.RealBallCompoment.Gravity = false; 
			CourtMgr.Get.RealBallObj.transform.position = new Vector3(0, 5, 13);
			break;
		case EGameTest.Edit:
			SetBall(PlayerList[0]);
			break;
		}

//		CourtMgr.Get.RealBallCompoment.SetBallState (EPlayerState.Start);
		ChangeSituation(EGameSituation.JumpBall);
		StartCoroutine(playerJumpBall());

		if (jumpBall)
	        AIController.Get.ChangeState(EGameSituation.JumpBall);
	}

	IEnumerator playerJumpBall () {
		CourtMgr.Get.RealBallCompoment.SetJumpBallPathUp();
		yield return new WaitForSeconds(0.55f);
		GameMsgDispatcher.Ins.SendMesssage(EGameMsg.PlayerTouchBallWhenJumpBall, getJumpTeam);
	}

    private PlayerBehaviour FindDefMen(PlayerBehaviour npc)
    {
        PlayerBehaviour Result = null;
        
        if (npc != null && PlayerList.Count > 1)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList [i] != npc && PlayerList [i].Team != npc.Team && PlayerList [i].Index == npc.Index)
                {
                    Result = PlayerList [i];
                    break;
                }
            }
        }
        
        return Result;
    }

	private void checkPlayerID() {
		for (int i = 0; i < GameData.TeamMembers.Length; i ++)
			if (!GameData.DPlayers.ContainsKey(GameData.TeamMembers[i].Player.ID))
				GameData.TeamMembers[i].Player.SetID(20 + i);

		for (int i = 0; i < GameData.EnemyMembers.Length; i ++)
			if (!GameData.DPlayers.ContainsKey(GameData.EnemyMembers[i].Player.ID))
				GameData.EnemyMembers[i].Player.SetID(31 + i);
	}

	public void SetBornPositions()
	{
		float v1 = 0;
		float v2 = 0;
		int [] aPosAy = new int[3];
		int [] bPosAy = new int[3];
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].Team == ETeamKind.Self)
			{
				if(PlayerList[i].Attribute.Dribble > v1)
				{
					v1 = PlayerList[i].Attribute.Dribble;
					aPosAy[0] = i;
				}
			}
			else
			{
				if(PlayerList[i].Attribute.Dribble > v2)
				{
					v2 = PlayerList[i].Attribute.Dribble;
					bPosAy[0] = i;
				}
			}
		}
		
		v1 = 0;
		v2 = 0;
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].Team == ETeamKind.Self)
			{
				if(PlayerList[i].Attribute.Rebound > v1 && aPosAy[0] != i)
				{
					v1 = PlayerList[i].Attribute.Rebound;
					aPosAy[1] = i;
				}
			}
			else
			{
				if(PlayerList[i].Attribute.Rebound > v2 && bPosAy[0] != i)
				{
					v2 = PlayerList[i].Attribute.Rebound;
					bPosAy[1] = i;
				}
			}
		}
		
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if(PlayerList[i].Team == ETeamKind.Self)
			{
				if(aPosAy[0] != i && aPosAy[1] != i)									
					aPosAy[2] = i;
			}
			else
			{
				if(bPosAy[0] != i && bPosAy[1] != i)
					bPosAy[2] = i;
			}
		}
		
		//Team A
		PlayerList[aPosAy[0]].Postion = EPlayerPostion.G;
		PlayerList[aPosAy[0]].transform.position = mJumpBallPos[0];
		PlayerList[aPosAy[0]].ShowPos = 1;
		PlayerList[aPosAy[0]].IsJumpBallPlayer = false;
		PlayerList[aPosAy[1]].Postion = EPlayerPostion.C;
		PlayerList[aPosAy[1]].transform.position = mJumpBallPos[1];
		PlayerList[aPosAy[1]].ShowPos = 0;
		PlayerList[aPosAy[1]].IsJumpBallPlayer = true;
		PlayerList[aPosAy[2]].Postion = EPlayerPostion.F;
		PlayerList[aPosAy[2]].transform.position = mJumpBallPos[2];
		PlayerList[aPosAy[2]].ShowPos = 2;
		PlayerList[aPosAy[2]].IsJumpBallPlayer = false;
		
		//Team B
		PlayerList[bPosAy[0]].Postion = EPlayerPostion.G;
		PlayerList[bPosAy[0]].transform.position = mJumpBallPos[3];
		PlayerList[bPosAy[0]].ShowPos = 4;
		PlayerList[bPosAy[0]].IsJumpBallPlayer = false;
		PlayerList[bPosAy[1]].Postion = EPlayerPostion.C;
		PlayerList[bPosAy[1]].transform.position = mJumpBallPos[4];
		PlayerList[bPosAy[1]].ShowPos = 3;
		PlayerList[bPosAy[1]].IsJumpBallPlayer = true;
		PlayerList[bPosAy[2]].Postion = EPlayerPostion.F;
		PlayerList[bPosAy[2]].transform.position = mJumpBallPos[5];
		PlayerList[bPosAy[2]].ShowPos = 5;
		PlayerList[bPosAy[2]].IsJumpBallPlayer = false;
	}

	/// <summary>
	/// Sets the player born target.
	/// Player is forward, stand JumpballPos[2]
	/// </summary>
	private void setPlayerBornTarget () {
		PlayerList[0].Postion = EPlayerPostion.F;
		PlayerList[0].transform.position = mJumpBallPos[2];
		PlayerList[0].ShowPos = 2;
		PlayerList[0].IsJumpBallPlayer = false;
		PlayerList[1].Postion = EPlayerPostion.C;
		PlayerList[1].transform.position = mJumpBallPos[1];
		PlayerList[1].ShowPos = 0;
		PlayerList[1].IsJumpBallPlayer = true;
		PlayerList[2].Postion = EPlayerPostion.G;
		PlayerList[2].transform.position = mJumpBallPos[0];
		PlayerList[2].ShowPos = 1;
		PlayerList[2].IsJumpBallPlayer = false;
	}

	private PlayerBehaviour getJumpTeam {
		get {
			float vSelf = 0;
			float vNPC = 0;
			for(int i=0; i<PlayerList.Count; i++) {
				if(PlayerList[i].IsJumpBallPlayer && PlayerList[i].Team == ETeamKind.Self)
					vSelf = PlayerList[i].Attribute.Rebound;
				else if(PlayerList[i].IsJumpBallPlayer && PlayerList[i].Team == ETeamKind.Npc)
					vNPC = PlayerList[i].Attribute.Rebound;
			}
			if(vSelf >= vNPC) {
				for(int i=0; i<PlayerList.Count; i++) 
					if(PlayerList[i].IsJumpBallPlayer && PlayerList[i].Team == ETeamKind.Self)
						return PlayerList[i];
			} else {
				for(int i=0; i<PlayerList.Count; i++) 
					if(PlayerList[i].IsJumpBallPlayer && PlayerList[i].Team == ETeamKind.Npc)
						return PlayerList[i];
			}
			return PlayerList[0];
		}
	}

	public void InitIngameAnimator() {
        //skip for smooth
        playerSelectMe.SetActive(true);
        for (int i = 0; i < PlayerList.Count; i++)
            PlayerList[i].PlayerRefGameObject.SetActive(true);
         
        /*
        if (PlayerList.Count > 0 && PlayerList[0].AnimatorControl && PlayerList[0].AnimatorControl.runtimeAnimatorController &&
            PlayerList[0].AnimatorControl.runtimeAnimatorController.name == EAnimatorType.ShowControl.ToString()) {
            for(int i = 0; i < PlayerList.Count; i++)
                if(PlayerList[i])
                    ModelManager.Get.ChangeAnimator(ref PlayerList[i].AnimatorControl, PlayerList[i].Attribute.BodyType, EAnimatorType.AnimationControl);

            for(int i = 0; i < PlayerList.Count; i++)
                if(PlayerList[i].ShowPos != 0 || PlayerList[i].ShowPos != 3)
                    PlayerList[i].AniState(EPlayerState.Idle);
        }*/
	}

    public PlayerBehaviour CreateGamePlayer(int teamIndex, ETeamKind team, Vector3 bornPos, TPlayer player, GameObject res = null)
    {
        if (GameData.DPlayers.ContainsKey(player.ID))
        {
            if (LobbyStart.Get.TestModel != EModelTest.None && LobbyStart.Get.TestMode != EGameTest.None)
                player.BodyType = (int)LobbyStart.Get.TestModel;

            TLoadParameter p = new TLoadParameter(ELayer.Player, team.ToString() + teamIndex.ToString(), true, false, false, true, false, EAnimatorType.AnimationControl);
            TAvatarLoader.Load(player.BodyType, player.Avatar, ref res, playerInfoModel, p);
            initRigbody(res);
            ETimerKind timeKey;
            if (team == ETeamKind.Self)
                timeKey = (ETimerKind)Enum.Parse(typeof(ETimerKind), string.Format("Self{0}", teamIndex));
            else
                timeKey = (ETimerKind)Enum.Parse(typeof(ETimerKind), string.Format("Npc{0}", teamIndex));

            res.transform.localPosition = bornPos;
            PlayerBehaviour playerBehaviour = res.AddComponent<PlayerBehaviour>();

            playerBehaviour.Team = team;
            playerBehaviour.MoveIndex = -1;
            playerBehaviour.Attribute = player;
            playerBehaviour.Index = (EPlayerPostion)teamIndex;
            playerBehaviour.Postion = playerBehaviour.Index;
            playerBehaviour.TimerKind = timeKey;

            playerBehaviour.InitTrigger(defPointObject);
            playerBehaviour.InitDoubleClick();
            playerBehaviour.InitAttr();

            if (team == ETeamKind.Npc)
                res.transform.localEulerAngles = new Vector3(0, 180, 0);

            playerBehaviour.AI = res.AddComponent<PlayerAI>();

            return playerBehaviour;
        }
        else
        {
            Debug.LogError("Error : playerId is not exist in great player");
            return null;
        }
    }
	
	public void CreateTeam()
    {
        int num = 0;
        switch (LobbyStart.Get.TestMode)
        {
            case EGameTest.None:
                if (StageData.FriendKind == 4)
                {
                    num = Mathf.Min(StageData.FriendID.Length, GameData.TeamMembers.Length);
                    for (int i = 0; i < num; i++)
                    {
                        if (GameData.TeamMembers[i].Player.SetID(StageData.FriendID[i]))
                        {
                            GameData.TeamMembers[i].Player.Name = GameData.DPlayers[StageData.FriendID[i]].Name;
                            PlayerList.Add(CreateGamePlayer(i, ETeamKind.Self, mJumpBallPos[i], GameData.TeamMembers[i].Player));
                        }
                    }

                    num = Mathf.Min(StageData.PlayerID.Length, GameData.EnemyMembers.Length);
                    for (int i = 0; i < num; i++)
                    {
                        if (GameData.EnemyMembers[i].Player.SetID(StageData.PlayerID[i]))
                        {
                            GameData.EnemyMembers[i].Player.Name = GameData.DPlayers[StageData.PlayerID[i]].Name;
                            PlayerList.Add(CreateGamePlayer(i, ETeamKind.Npc, mJumpBallPos[3 + i], GameData.EnemyMembers[i].Player));
                        }
                    }
                }
                else
                {
                    checkPlayerID();
                    num = Mathf.Min(LobbyStart.Get.FriendNumber, GameData.Max_GamePlayer);
                    for (int i = 0; i < num; i++)
                        PlayerList.Add(CreateGamePlayer(i, ETeamKind.Self, mJumpBallPos[i], GameData.TeamMembers[i].Player));

                    for (int i = 0; i < num; i++)
                        PlayerList.Add(CreateGamePlayer(i, ETeamKind.Npc, mJumpBallPos[i + 3], GameData.EnemyMembers[i].Player));
                }

                GameRecord.Init(PlayerList.Count);
                for (int i = 0; i < PlayerList.Count; i++)
                    if (PlayerList[i])
                        PlayerList[i].GameRecord.Init();
                
			    //1.G(Dribble) 2.C(Rebound) 3.F
				SetBornPositions();
				setPlayerBornTarget ();
                break;
            case EGameTest.All:
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, mJumpBallPos[0], new GameStruct.TPlayer(0)));	
                PlayerList.Add(CreateGamePlayer(1, ETeamKind.Self, mJumpBallPos[1], new GameStruct.TPlayer(0)));	
                PlayerList.Add(CreateGamePlayer(2, ETeamKind.Self, mJumpBallPos[2], new GameStruct.TPlayer(0)));	
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Npc, mJumpBallPos[3], new GameStruct.TPlayer(0)));	
                PlayerList.Add(CreateGamePlayer(1, ETeamKind.Npc, mJumpBallPos[4], new GameStruct.TPlayer(0)));	
                PlayerList.Add(CreateGamePlayer(2, ETeamKind.Npc, mJumpBallPos[5], new GameStruct.TPlayer(0)));

                break;
            case EGameTest.AttackA:
            case EGameTest.Shoot:
            case EGameTest.Dunk:
            case EGameTest.Rebound:
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, mJumpBallPos[0], GameData.TeamMembers[0].Player));
                PlayerList.Add(CreateGamePlayer(1, ETeamKind.Npc, mJumpBallPos[4], new TPlayer(0)));	
                SetBornPositions();
                UIGame.Get.ChangeControl(true);
                SetPlayerAI(false);
                break;
            case EGameTest.AnimationUnit:
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, Vector3.zero, new TPlayer(0)));
                PlayerList[0].IsJumpBallPlayer = true;
                SetPlayerAI(false);
                break;
            case EGameTest.AttackB:
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Npc, Vector3.zero, new TPlayer(0)));
                PlayerList[0].IsJumpBallPlayer = true;
                SetPlayerAI(false);
                break;
            case EGameTest.Block:
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, new Vector3(0, 0, -8.4f), new TPlayer(0)));
                PlayerList.Add(CreateGamePlayer(1, ETeamKind.Npc, new Vector3(0, 0, -4.52f), new TPlayer(0)));
                PlayerList[0].IsJumpBallPlayer = true;
                PlayerList[1].IsJumpBallPlayer = true;
                SetPlayerAI(false);
                break;
            case EGameTest.OneByOne: 
                TPlayer Self = new TPlayer(0);
                Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			

                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, Vector3.zero, Self));
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Npc, new Vector3(0, 0, 5), new TPlayer(0)));
                PlayerList[0].IsJumpBallPlayer = true;
                PlayerList[1].IsJumpBallPlayer = true;
                break;
            case EGameTest.Alleyoop:
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, Vector3.zero, new TPlayer(0)));
                PlayerList.Add(CreateGamePlayer(1, ETeamKind.Self, new Vector3(0, 0, 3), new TPlayer(0)));
                PlayerList[0].IsJumpBallPlayer = true;
                PlayerList[1].IsJumpBallPlayer = true;
                break;
            case EGameTest.Pass:
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, Vector3.zero, new TPlayer(0)));
                PlayerList.Add(CreateGamePlayer(1, ETeamKind.Self, new Vector3(-5, 0, -2), new TPlayer(0)));
                PlayerList.Add(CreateGamePlayer(2, ETeamKind.Self, new Vector3(5, 0, -2), new TPlayer(0)));
                PlayerList[0].IsJumpBallPlayer = true;
                SetPlayerAI(false);
                break;
            case EGameTest.Edit:
                createEditTeam();
                break;
            case EGameTest.CrossOver:
                Self = new TPlayer(0);
                Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			
			
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, Vector3.zero, Self));
                PlayerList.Add(CreateGamePlayer(1, ETeamKind.Npc, new Vector3(0, 0, 5), new TPlayer(0)));
                PlayerList[0].IsJumpBallPlayer = true;
                PlayerList[1].IsJumpBallPlayer = true;
                break;
            case EGameTest.Skill:
                if (GameData.Team.Player.ID == 0)
                    GameData.Team.Player.SetID(14);

                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, Vector3.zero, new TPlayer(0)));
                PlayerList.Add(CreateGamePlayer(1, ETeamKind.Npc, new Vector3(0, 0, 5), new TPlayer(0)));
                SetPlayerAI(false);
                break;
            case EGameTest.PassiveSkill:
                if (GameData.Team.Player.ID == 0)
                    GameData.Team.Player.SetID(14);
			
                PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, Vector3.zero, new TPlayer(0)));
                PlayerList.Add(CreateGamePlayer(1, ETeamKind.Npc, new Vector3(0, 0, 5), new TPlayer(0)));
                SetPlayerAI(false);
                break;
        }

        for (int i = 0; i < PlayerList.Count; i++)
            PlayerList[i].DefPlayer = FindDefMen(PlayerList[i]);

        Joysticker = PlayerList[0];
        UIGame.Get.SetJoystick(Joysticker);
        AddValueItemAttributes();

        playerSelectMe = EffectManager.Get.PlayEffect("SelectMe", Vector3.zero, null, Joysticker.PlayerRefGameObject);
        if (playerSelectMe) {
            PlayerSelectArrow = playerSelectMe.transform.FindChild("SelectArrow");
            Joysticker.AIActiveHint = GameObject.Find("SelectMe/AI");
            Joysticker.SpeedUpView = GameObject.Find("SelectMe/Speedup").GetComponent<UISprite>();
            Joysticker.SpeedAnimator = GameObject.Find("SelectMe").GetComponent<Animator>();
        }

        passIcon[0] = EffectManager.Get.PlayEffect("PassMe", Joysticker.BodyHeight.transform.localPosition, Joysticker.PlayerRefGameObject);

        if (PlayerList.Count > 1 && PlayerList[1].Team == Joysticker.Team)
            passIcon[1] = EffectManager.Get.PlayEffect("PassA", Joysticker.BodyHeight.transform.localPosition, PlayerList[1].PlayerRefGameObject);

        if (PlayerList.Count > 2 && PlayerList[2].Team == Joysticker.Team)
            passIcon[2] = EffectManager.Get.PlayEffect("PassB", Joysticker.BodyHeight.transform.localPosition, PlayerList[2].PlayerRefGameObject);

        for (int i = 0; i < PlayerList.Count; i++)
        {
            if (PlayerList[i].Team != Joysticker.Team)
            {
                PlayerList[i].SelectMe = EffectManager.Get.PlayEffect("SelectTarget", Vector3.zero, null, PlayerList[i].PlayerRefGameObject, 0, true, false);
                PlayerList[i].SelectMe.SetActive(false);
            }
        }
		UIGame.Get.InitPlayerSkillUI(Joysticker);
//		UIGame.Get.RefreshSkillUI();

        Joysticker.OnUIJoystick = UIGame.Get.SetUIJoystick;
        for (int i = 0; i < PlayerList.Count; i++)
        {
            UIDoubleClick.Get.InitDoubleClick(PlayerList[i], i);
            PlayerList[i].OnShooting = OnShooting;
//            PlayerList [i].OnStealMoment = OnStealMoment;
            PlayerList[i].OnGotSteal = OnGotSteal;
            PlayerList[i].OnBlockMoment = OnBlockMoment;
            PlayerList[i].OnDoubleClickMoment = OnDoubleClickMoment;
            PlayerList[i].OnFakeShootBlockMoment = OnFakeShootBlockMoment;
            PlayerList[i].OnBlockJump = OnBlockJump;
            PlayerList[i].OnDunkJump = OnDunkJump;
            PlayerList[i].OnDunkBasket = OnDunkBasket;
            PlayerList[i].OnOnlyScore = OnOnlyScore;
            PlayerList[i].OnPickUpBall = OnPickUpBall;
            PlayerList[i].OnFall = OnFall;
            PlayerList[i].OnUI = UIGame.Get.OpenUIMask;
            PlayerList[i].OnUICantUse = UIGame.Get.UICantUse;
            PlayerList[i].OnUIAnger = UIGame.Get.SetAngerUI;
			PlayerList[i].OnReviveAnger = UIGame.Get.AddForceReviveValue;
            PlayerList[i].PlayerRefGameObject.SetActive(false); //hide player for smooth 
        }

        playerSelectMe.SetActive(false);
        preLoadSkillEffect();
        GameMsgDispatcher.Ins.SendMesssage(EGameMsg.GamePlayersCreated, PlayerList.ToArray());
    }

    private void createEditTeam()
    {
        var playerC = new TPlayer(0) {ID = 3};
        playerC.SetID(3);
        PlayerList.Add(CreateGamePlayer(0, ETeamKind.Self, Vector3.zero, playerC));

        var playerF = new TPlayer(0) {ID = 2};
        playerF.SetID(2);
        PlayerList.Add(CreateGamePlayer(1, ETeamKind.Self, new Vector3(-5, 0, -2), playerF));

        var playerG = new TPlayer(0) {ID = 1};
        playerG.SetID(1);
        PlayerList.Add(CreateGamePlayer(2, ETeamKind.Self, new Vector3(5, 0, -2), playerG));

        PlayerList[0].IsJumpBallPlayer = true;
        SetPlayerAI(false);

        PlayerList[0].IsJumpBallPlayer = true;
        SetPlayerAI(false);
    }

	//加數值裝的數值
    private void AddValueItemAttributes()
    {
        Action<TItemData> addAttributes = item =>
        {
            for(var i = 0; i < item.Bonus.Length; i++)
            {
                if(item.Bonus[i] != EBonus.None)
                    Joysticker.SetAttribute((int)item.Bonus[i], item.BonusValues[i]);
            }
        };

        if(GameData.Team.Player.ConsumeValueItems == null)
            return;

        foreach(int itemID in GameData.Team.Player.ConsumeValueItems)
        {
            if(!GameData.DItemData.ContainsKey(itemID))
                continue;

            var item = GameData.DItemData[itemID];
            addAttributes(item);
        }
    }

    private void preLoadSkillEffect() {
		for (int i = 0; i < PlayerList.Count; i++)
			for (int j = 0; j < PlayerList[i].Attribute.SkillCards.Length; j++)
				EffectManager.Get.PreLoadSkillEffect(PlayerList[i].Attribute.SkillCards[j].ID);
	}

	public void SetPlayerAI(bool enable){
		for(int i = 0; i < PlayerList.Count; i++)
			PlayerList[i].AI.enabled = enable;
	}

	public void ClearAutoFollowTime(){
		for(int i = 0; i < PlayerList.Count; i++)
			PlayerList[i].ClearAutoFollowTime();
	}

	void FixedUpdate() {
		#if UNITY_EDITOR
		if (Joysticker) {
			switch(LobbyStart.Get.TestMode){
				case EGameTest.Rebound:
					if (Input.GetKeyDown (KeyCode.Z)) 
						resetTestMode();

					break;

				case EGameTest.AnimationUnit:
                case EGameTest.Block:
				case EGameTest.PassiveSkill:
					if (Input.GetKeyDown (KeyCode.S)){
                        Joysticker.AniState(LobbyStart.Get.SelectAniState);
						TSkill skill = new TSkill();
                        skill.ID = (int )LobbyStart.Get.SelectAniState;
						Joysticker.PassiveSkillUsed = skill;
                        if((int)LobbyStart.Get.SelectAniState > 100 && LobbyStart.Get.TestMode == EGameTest.AnimationUnit) 
							SkillEffectManager.Get.OnShowEffect(Joysticker, true);
					}

                    if (Input.GetKeyDown (KeyCode.T)){
                        Joysticker.AniState(EPlayerState.Dribble1);
                        Joysticker.AniState(EPlayerState.Dribble2);
                        Joysticker.AniState(EPlayerState.Dunk20);
                    }
                    if (Input.GetKeyDown (KeyCode.K)){
						Joysticker.PlayerSkillController.DoPassiveSkill(ESkillSituation.Fall1);
                    }

                    if(Input.GetKeyDown (KeyCode.R))
                    {
                        PlayerList[1].transform.position = Vector3.zero;
                        SetBall(PlayerList [1]);
                        PlayerList [1].AniState(EPlayerState.Dribble0);
                        PlayerList [1].AniState(EPlayerState.Shoot0);
                    }
					break;
				default:
					KeyboardControl();
					break;
			}
		}
		#endif
		selectMeEvent();
		angerRecoveryUpdate ();
        PassCD.Update(Time.deltaTime);

		if (CoolDownCrossover > 0 && Time.time >= CoolDownCrossover)
            CoolDownCrossover = 0;

        handleSituation();

		if(StealBtnLiftTime > 0)
			StealBtnLiftTime -= Time.deltaTime;

		if(passingStealBallTime > 0 && Time.time >= passingStealBallTime)		
			passingStealBallTime = 0;

		if (IsTimePass())
			gameResult();

		if (IsFinish)
		{
			if (!GameRecord.Done && !CourtMgr.Get.IsBallOffensive) 
			{
				UIEndGame.Get.ShowView();
				GameRecord.Done = true;
				SetGameRecord ();
				StartCoroutine (playFinish ());
			}
		}
	}

	private void angerRecoveryUpdate () {
		if(IsStart) {
			if(Joysticker != null && Joysticker.Attribute.IsHaveActiveSkill && Joysticker.AngerPower < Joysticker.Attribute.MaxAnger) {
				if(recoverTime > 0) {
					recoverTime -= Time.deltaTime;	
					if(recoverTime <= 0) {
						recoverTime = GameConst.AngerReviveTime;
						Joysticker.ReviveAnger(recoverAngerBase);
					}
				}
			}
		}
			
	}

	private void selectMeEvent() {
		if(PlayerSelectArrow != null) {
			if(Situation == EGameSituation.Presentation)
				playerSelectMe.SetActive(false);
			else {
				if(!playerSelectMe.activeInHierarchy)
					playerSelectMe.SetActive(true);
				if(Situation == EGameSituation.GamerAttack) {
					NpcSelectMe = FindNearNpc();
					PlayerSelectArrow.transform.localEulerAngles = new Vector3(0, MathUtils.FindAngle(Joysticker.PlayerRefGameObject.transform.position, CourtMgr.Get.Hood[ETeamKind.Self.GetHashCode()].transform.position), 0);
				} else if(Situation == EGameSituation.NPCAttack) {
					NpcSelectMe = FindNearNpc();
					showEnemySelect(NpcSelectMe);
					PlayerSelectArrow.transform.localEulerAngles = new Vector3(0, MathUtils.FindAngle(Joysticker.PlayerRefGameObject.transform.position, NpcSelectMe.transform.position), 0);
					NpcSelectMe.SelectMe.transform.localEulerAngles = new Vector3(0, MathUtils.FindAngle(Joysticker.PlayerRefGameObject.transform.position, NpcSelectMe.PlayerRefGameObject.transform.position) + 180, 0);
				} else
					PlayerSelectArrow.transform.localEulerAngles = new Vector3(0, MathUtils.FindAngle(Joysticker.PlayerRefGameObject.transform.position, CourtMgr.Get.RealBallObj.transform.position), 0);
			}
		}
	}

	private void showEnemySelect (PlayerBehaviour p) {
		hideAllEnemySelect (p);
		p.SelectMe.SetActive(true);
	}

	private void hideAllEnemySelect (PlayerBehaviour p = null) {
		for(int i=0; i<PlayerList.Count; i++) {
			if(p == null) {
				if(PlayerList[i].Team != Joysticker.Team && PlayerList[i] != null)
					PlayerList[i].SelectMe.SetActive(false);
			} else {
				if(PlayerList[i].Team != Joysticker.Team && PlayerList[i] != null && PlayerList[i] != p)
					PlayerList[i].SelectMe.SetActive(false);
			}
		}
	}

	private void KeyboardControl()
	{
		if (Input.GetKeyUp (KeyCode.D))
		{
			UIGame.Get.DoAttack(null, true);
			UIGame.Get.DoAttack(null, false);
		}
		
		if (Situation == EGameSituation.GamerAttack) {
			if (Input.GetKeyUp (KeyCode.A))
			{
				UIGame.Get.DoPassChoose(null, false);
			}
			
			if(Input.GetKeyDown (KeyCode.W))
				UIGame.Get.DoPassTeammateA(null, true);
			
			if(Input.GetKeyDown (KeyCode.E))
				UIGame.Get.DoPassTeammateB(null, true);
			
			if (Input.GetKeyDown (KeyCode.S))
			{
				if(LobbyStart.Get.TestMode != EGameTest.Skill) 
					UIGame.Get.DoShoot(null, true);
			}
			
			if (Input.GetKeyUp (KeyCode.S))
			{
				if(LobbyStart.Get.TestMode == EGameTest.Skill) {
					TSkill skill = new TSkill();
					skill.ID = LobbyStart.Get.TestID.GetHashCode();
					skill.Lv = LobbyStart.Get.TestLv;
					if (GameData.DSkillData[skill.ID].Kind == 171){
						CourtMgr.Get.ShowArrowOfAction(true,
							Joysticker.PlayerRefGameObject.transform,
							GameData.DSkillData[skill.ID].Distance(skill.Lv));
					}
				 	DoSkill(Joysticker, skill);
					CourtMgr.Get.ShowArrowOfAction(false);
				} else
				if(LobbyStart.Get.TestMode != EGameTest.AnimationUnit)
					UIGame.Get.DoShoot(null, false);

			}
		}
		else if(Situation == EGameSituation.NPCAttack){
			if(Input.GetKeyDown (KeyCode.A)){
				UIGame.Get.DoSteal(null, true);
				UIGame.Get.DoSteal(null, false);
			}
			
			if(Input.GetKeyDown (KeyCode.S)){
				if(LobbyStart.Get.TestMode == EGameTest.Skill) {
					TSkill skill = new TSkill();
					skill.ID = LobbyStart.Get.TestID.GetHashCode();
					skill.Lv = LobbyStart.Get.TestLv;
					if (GameData.DSkillData[skill.ID].Kind == 171){
						CourtMgr.Get.ShowArrowOfAction(true,
							Joysticker.PlayerRefGameObject.transform,
							GameData.DSkillData[skill.ID].Distance(skill.Lv));
					}
					DoSkill(Joysticker, skill);
					CourtMgr.Get.ShowArrowOfAction(false);
				} else
					UIGame.Get.DoBlock();
			}
		}
		
		if(Input.GetKeyDown(KeyCode.L)) {
			for (int i = 0; i < PlayerList.Count; i ++){
				PlayerList[i].SetAnger(PlayerList[i].Attribute.MaxAnger);
				UIGame.Get.RefreshSkillUI();
			}
		}
		
		if(Input.GetKeyDown(KeyCode.P) && Joysticker != null) { 
			Joysticker.SetAnger(Joysticker.Attribute.MaxAnger);
			UIGame.Get.RefreshSkillUI();
		}
	}
	
	private void resetTestMode() {
		SetBallOwnerNull();
		SetBall();
		CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.Shoot0);
		UIGame.Get.ChangeControl(false);

		ChangeSituation(EGameSituation.GamerAttack);

		PlayerList[0].transform.position = new Vector3(CourtMgr.Get.RealBallObj.transform.position.x, 0, CourtMgr.Get.RealBallObj.transform.position.z-1);
		PlayerList[0].AniState(EPlayerState.Idle);
    }

	public void SetGameRecord() {
        GameRecord.Identifier = GameData.Team.Identifier;
		GameRecord.Version = BundleVersion.Version;
		GameRecord.End = System.DateTime.UtcNow;

        double dt = new System.TimeSpan(GameRecord.End.Ticks - GameRecord.Start.Ticks).TotalSeconds;
        GameRecord.GamePlayTime = Mathf.Min(60*10, (int)dt);
		GameRecord.PauseCount++;
        GameRecord.StageID = StageData.ID;
        GameRecord.IsWin = IsWinner;
		GameRecord.Score1 = UIGame.Get.Scores [0];
		GameRecord.Score2 = UIGame.Get.Scores [1];
		for (int i = 0; i < PlayerList.Count; i ++) {
			if (i < GameRecord.PlayerRecords.Length) {
				PlayerList[i].GameRecord.ShotError = Mathf.Max(0, 
			        PlayerList[i].GameRecord.ShotError - PlayerList[i].GameRecord.BeBlock);

                PlayerList[i].GameRecord.GamePlayTime = GameRecord.GamePlayTime;
                PlayerList[i].GameRecord.Score = PlayerList[i].GameRecord.FGIn * 2 + PlayerList[i].GameRecord.FG3In * 3;
                if (!string.IsNullOrEmpty(PlayerList[i].Attribute.Identifier)) {
                    PlayerList[i].GameRecord.Identifier = PlayerList[i].Attribute.Identifier;
                    if (i > 0 && i < 3)
                        GameRecord.HaveFriend = true;
                }

                GameRecord.PlayerRecords[i] = PlayerList[i].GameRecord;
			}
		}
	}
        
    public void SendGameRecord() {
        if (!string.IsNullOrEmpty(GameRecord.Identifier) && LobbyStart.Get.TestMode == EGameTest.None && (!StageData.IsTutorial)) {
            string str = JsonConvert.SerializeObject(GameRecord);
            if (SendHttp.Get.CheckNetwork(false)) {
                WWWForm form = new WWWForm();
                form.AddField("GameRecord", str);
                form.AddField("Start", GameRecord.Start.ToString());
                form.AddField("End", GameRecord.End.ToString());
                SendHttp.Get.Command(URLConst.GameRecord, waitGameRecord, form, true);
                GameRecord.Identifier = "";
            } else {
                //PlayerPrefs.SetString(SettingText.GameRecord, str);
                //PlayerPrefs.SetString(SettingText.GameRecordStart, GameRecord.Start.ToString());
                //PlayerPrefs.SetString(SettingText.GameRecordEnd, GameRecord.End.ToString());
            }
        }
    }

    private void waitGameRecord(bool ok, WWW www) {
        if (ok) {
            TTeam result = JsonConvert.DeserializeObject <TTeam>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.TeamRecord = result.TeamRecord;
            GameData.Team.Player.PlayerRecord = result.Player.PlayerRecord;
			GameData.Team.LifetimeRecord = result.LifetimeRecord;
            if (result.Friends != null)
                GameData.Team.Friends = result.Friends;
        }
    }

    #if UNITY_EDITOR
	private bool isOpen = true;
	void OnGUI()
    {
        if (LobbyStart.Get.IsShowPlayerInfo) {
			if(isOpen){
				if(GUILayout.Button("Close"))
					isOpen = false;
                
            	if(Joysticker != null) {
                	GUILayout.Label("PointRate2:"+ Joysticker.Attr.PointRate2);
                	GUILayout.Label("PointRate3:"+ Joysticker.Attr.PointRate3);
                	GUILayout.Label("StealRate:"+ Joysticker.Attr.StealRate);
                	GUILayout.Label("DunkRate:"+ Joysticker.Attr.DunkRate);
                	GUILayout.Label("TipInRate:"+ Joysticker.Attr.TipInRate);
                	GUILayout.Label("AlleyOopRate:"+ Joysticker.Attr.AlleyOopRate);
                	GUILayout.Label("StrengthRate:"+ Joysticker.Attr.StrengthRate);
                	GUILayout.Label("BlockPushRate:"+ Joysticker.Attr.BlockPushRate);
                	GUILayout.Label("ElbowingRate:"+ Joysticker.Attr.ElbowingRate);
                	GUILayout.Label("ReboundRate:"+ Joysticker.Attr.ReboundRate);
                	GUILayout.Label("BlockRate:"+ Joysticker.Attr.BlockRate);
                	GUILayout.Label("PushingRate:"+ Joysticker.Attr.PushingRate);
                	GUILayout.Label("PassRate:"+ Joysticker.Attr.PassRate);
                	GUILayout.Label("SpeedValue:"+ Joysticker.Attr.SpeedValue);
                	GUILayout.Label("StaminaValue:"+ Joysticker.Attr.StaminaValue);
            	}
			} else {
				if(GUILayout.Button("Open"))
					isOpen = true;
			}
		}

        if (LobbyStart.Get.TestMode == EGameTest.Rebound) {
			if (GUI.Button(new Rect(100, 100, 100, 100), "Reset")) {
				resetTestMode();
			}
		}

        if (LobbyStart.Get.TestMode == EGameTest.CrossOver) {
			if (GUI.Button(new Rect(20, 50, 100, 100), "Left")) {
				PlayerList[0].transform.DOMoveX(PlayerList[0].transform.position.x - 1, GameConst.CrossTimeX).SetEase(Ease.Linear);
				PlayerList[0].transform.DOMoveZ(PlayerList[0].transform.position.z + 6, GameConst.CrossTimeZ).SetEase(Ease.Linear);
				PlayerList[0].AniState(EPlayerState.MoveDodge0);
			}

			if (GUI.Button(new Rect(120, 50, 100, 100), "Right")) {
				PlayerList[0].transform.DOMoveX(PlayerList[0].transform.position.x + 1, GameConst.CrossTimeX).SetEase(Ease.Linear);
				PlayerList[0].transform.DOMoveZ(PlayerList[0].transform.position.z + 6, GameConst.CrossTimeZ).SetEase(Ease.Linear);
				PlayerList[0].AniState(EPlayerState.MoveDodge1);
			}
		}

        if (LobbyStart.Get.TestMode == EGameTest.Shoot) {
			for(int i = 0 ; i < ShootStates.Length; i++){
				if (GUI.Button(new Rect(Screen.width / 2, 50 + i * 50, 100, 50), ShootStates[i].ToString())) {	
					testState = ShootStates[i];
				}
			}
		}
        if(LobbyStart.Get.IsDebugAnimation){
			GUI.Label(new Rect(Screen.width * 0.5f - 25, 100, 300, 50), "Play Counts:" + PlayCount.ToString());
			GUI.Label(new Rect(Screen.width * 0.25f - 25, 100, 300, 50), "Self Wins:" + SelfWin.ToString());
			GUI.Label(new Rect(Screen.width * 0.75f - 25, 100, 300, 50), "Npc Wins:" + NpcWin.ToString());
			GUI.Label(new Rect(Screen.width * 0.25f - 25, 150, 300, 50), "Shoot Swish Times:" + shootSwishTimes.ToString());
			GUI.Label(new Rect(Screen.width * 0.75f - 25, 150, 300, 50), "Shoot Score Swish Times:" + shootScoreSwishTimes.ToString());
			GUI.Label(new Rect(Screen.width * 0.25f - 25, 200, 300, 50), "Shoot Times:" + shootTimes.ToString());
			GUI.Label(new Rect(Screen.width * 0.75f - 25, 200, 300, 50), "Shoot Score Times:" + shootScoreTimes.ToString());
		}


        if(LobbyStart.Get.IsShowShootRate) {
			GUILayout.Label("random rate:"+ randomrate);
			GUILayout.Label("normal rate:"+ normalRate);
			GUILayout.Label("uphand rate:"+ uphandRate);
			GUILayout.Label("downhand rate:"+ downhandRate);
			GUILayout.Label("nearshot rate:"+ nearshotRate);
			GUILayout.Label("layup rate:"+ layupRate);
		}

        if(LobbyStart.Get.TestMode == EGameTest.Skill || LobbyStart.Get.TestMode == EGameTest.PassiveSkill) {
			if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 50), "player get Ball")) {
				SetBall(PlayerList[0]);
				PlayerList[1].AniState(EPlayerState.Idle);
			}
			if (GUI.Button(new Rect(Screen.width - 100, 150, 100, 50), "enemy get Ball")) {
				SetBall(PlayerList[1]);
				PlayerList[0].AniState(EPlayerState.Idle);
			}
			if (GUI.Button(new Rect(Screen.width - 100, 200, 100, 50), "shoot")) {
//				DoShoot();
				TSkill skill = new TSkill();
				skill.ID = 10700;
				skill.Lv = 2;
				DoSkill(BallOwner, skill);
			}
			if (GUI.Button(new Rect(Screen.width - 100, 270, 100, 50), "dunk")) {
				BallOwner.AniState(EPlayerState.Dunk0, CourtMgr.Get.GetShootPointPosition(BallOwner.Team));
			}
		}
	}
	#endif

    /// <summary>
    /// 某隊得分, 另一隊執行撿球.
    /// </summary>
    /// <param name="team"> 執行撿球的隊伍(玩家 or 電腦). </param>
    private void SituationPickBall(ETeamKind team)
    {
        // 撿球球員的移動位置會被目前系統的機制清掉(詳細運作流程目前不太清楚),
        // 所以現在用不佳的作法, 如果撿球者的移動位置被清掉, 那就重新設定.
        if((PickBallPlayer && PickBallPlayer.TargetPosNum > 0) || 
            BallOwner || PlayerList.Count <= 0)
            return;

        var player = AIController.Get.GetTeam(team).FindNearBallPlayer();
        if(player != null)
            PickBallPlayer = player.GetComponent<PlayerBehaviour>();

        if(PickBallPlayer == null)
            return;

        // 根據撿球員的位置(C,F,G) 選擇適當的進攻和防守戰術.
        if(LobbyStart.Get.CourtMode == ECourtMode.Full)
        {
            AITools.RandomCorrespondingTactical(
                ETacticalAuto.Inbounds, ETacticalAuto.InboundsDef, 
                PickBallPlayer.Index, out attackTactical, out defTactical);
        }
        else
        {
            AITools.RandomCorrespondingTactical(
                ETacticalAuto.HalfInbounds, ETacticalAuto.HalfInboundsDef, 
                PickBallPlayer.Index, out attackTactical, out defTactical);
        }

        for(int i = 0; i < PlayerList.Count; i++)
        {
            if(PlayerList[i].Team == team)
            {
                if(PlayerList[i] == PickBallPlayer)
                    MoveToBall(PlayerList[i]);
                else 
                    // InboundsBall 其實是混和的行為, 跑到這, 表示沒有持球者.
                    // 所以這行的意思其實是叫進攻方, 撿球以外的人執行戰術跑位.
                    inboundsBall(PlayerList[i], team, ref attackTactical);
            }
            else 
                backToDef(PlayerList[i], ETeamKind.Npc, ref defTactical);
        }
    }

    private void SituationInbounds(ETeamKind team)
    {
		if(PlayerList.Count > 0 && BallOwner)
		{
		    if(LobbyStart.Get.CourtMode == ECourtMode.Full)
            {
                AITools.RandomCorrespondingTactical(
                    ETacticalAuto.Inbounds, ETacticalAuto.InboundsDef, BallOwner.Index, 
                    out attackTactical, out defTactical);
            }
            else
            {
                AITools.RandomCorrespondingTactical(
                    ETacticalAuto.HalfInbounds, ETacticalAuto.HalfInboundsDef, BallOwner.Index, 
                    out attackTactical, out defTactical);
            }

//		    Debug.LogFormat("Attack:{0}, Defence:{1}", attackTactical, defTactical);

		    for(int i = 0; i < PlayerList.Count; i++)
		    {
		        if(PlayerList[i].Team == team)
		        {
		            if(!IsPassing)
		                inboundsBall(PlayerList[i], team, ref attackTactical);
		        }
		        else
		            backToDef(PlayerList[i], PlayerList[i].Team, ref defTactical);
		    }
		}
    }

//    /// <summary>
//    /// 不見得真的會傳球.
//    /// </summary>
//    /// <param name="player"> 持球者, 嘗試要傳球的人. </param>
//	public void AIPass([NotNull]PlayerBehaviour player)
//    {
//		float angle = 90;
//		if ((player.Team == ETeamKind.Self && player.transform.position.z > 0) ||
//		    (player.Team == ETeamKind.Npc && player.transform.position.z < 0))
//			angle = 180;
//
//		PlayerBehaviour teammate = findTeammate(player, 20, angle);
//
//		if(teammate != null)
//			TryPass(teammate);
//		else
//        {
//			int who = Random.Range(0, 2);
//			int find = 0;
//			
//			for(int i = 0; i < PlayerList.Count; i++)
//            {
//				if(PlayerList[i].gameObject.activeInHierarchy && 
//                   PlayerList[i].Team == player.Team && PlayerList[i] != player)
//                {
//					PlayerBehaviour someone = PlayerList[i];
//					
//					if(HasDefPlayer(someone, 1.5f, 40) == 0 || who == find)
//                    {
//						TryPass(PlayerList[i]);
//						break;
//					}
//					
//					find++;
//				}
//			}
//		}
//	}

    /// <summary>
    /// 僅用在玩家 or NPC 進攻時使用. 這部份都是對 player.DefPlayer 做移動邏輯.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="speedup"></param>
    public void MoveDefPlayer([NotNull] PlayerBehaviour player, bool speedup = false)
	{
		if(player && player.DefPlayer && !player.CheckAnimatorSate(EPlayerState.MoveDodge1) && 
		    !player.CheckAnimatorSate(EPlayerState.MoveDodge0) && 
		    (Situation == EGameSituation.GamerAttack || Situation == EGameSituation.NPCAttack))
        {
			if(player.DefPlayer.CanMove && player.DefPlayer.CantMoveTimer.IsOff())
            {
                // 防守球員可以移動.
				if(BallOwner != null)
                {
					moveData.Clear();
					if(player == BallOwner)
                    {
                        // 我是持球人, 我要防守球員看著我.
						moveData.DefPlayer = player;
						
						if (BallOwner != null)
							moveData.LookTarget = BallOwner.transform;
						else
							moveData.LookTarget = player.transform;
						
						moveData.Speedup = speedup;
						player.DefPlayer.TargetPos = moveData;
					}
                    else
                    {
                        // 我不是持球人.
                        int index = player.DefPlayer.Postion.GetHashCode();
                        float sign = LobbyStart.Get.CourtMode == ECourtMode.Full && player.DefPlayer.Team == ETeamKind.Self ? -1 : 1;

                        // HomePosition 和 DefPlayer 的距離.
                        float distance = Vector2.Distance(
                            new Vector2(mHomePositions[index].x, mHomePositions[index].y * sign), 
							new Vector2(player.DefPlayer.transform.position.x, player.DefPlayer.transform.position.z));
						
						if(distance <= player.DefPlayer.Attr.DefDistance)
                        {
                            // 防守者離 HomePosition 很接近了.(也就是靠近防守籃框)

                            // 如果有接近的球員, 要靠近他; 沒有接近的球員, 繼續往 Home Region 移動.
							PlayerBehaviour p = tryFindNearPlayer(player.DefPlayer, player.DefPlayer.Attr.DefDistance, false, true);
                            if (p != null)
                                moveData.DefPlayer = p;
                            else if (GetDis(player, player.DefPlayer) <= player.DefPlayer.Attr.DefDistance)
                                moveData.DefPlayer = player;
							
							if(moveData.DefPlayer != null)
                            {
								if(BallOwner != null)
									moveData.LookTarget = BallOwner.transform;
								else
									moveData.LookTarget = player.transform;
								
								moveData.Speedup = speedup;
								player.DefPlayer.TargetPos = moveData;
							}
                            else
                            {
								player.DefPlayer.ResetMove();
								sign = LobbyStart.Get.CourtMode == ECourtMode.Full && player.DefPlayer.Team == ETeamKind.Self ? -1 : 1;
                                moveData.SetTarget(mHomePositions[index].x, mHomePositions[index].y * sign);
                                
                                if (BallOwner != null)
									moveData.LookTarget = BallOwner.PlayerRefGameObject.transform;
                                else
                                {
                                    if (player.Team == ETeamKind.Self)
                                        moveData.LookTarget = CourtMgr.Get.Hood[1].transform;
                                    else
                                        moveData.LookTarget = CourtMgr.Get.Hood[0].transform;
                                }                                   
                                
                                player.DefPlayer.TargetPos = moveData;
                            }
                        }
                    }
                }
                else
                {
                    // 沒有人持球, 所以要叫附近的人去撿球.
                    player.DefPlayer.ResetMove();

                    if(player.DefPlayer)
                        NearestBallPlayerDoPickBall(player.DefPlayer);
                }
            }
        }
    }
    
    public void ChangeSituation(EGameSituation newSituation)
    {
        // 不能切換狀態的條件是
        // 目前狀態是 End, 新狀態不是 None, 也不是 Opening.
        // 當目前是 End 的時候, 只能切換到 None or Opening 狀態.
		if(Situation != EGameSituation.End || newSituation == EGameSituation.None || 
           newSituation == EGameSituation.Opening)
        {
            EGameSituation oldgs = Situation;
            if(Situation != newSituation)
            {
                // 當要切換不同狀態的時候, 會將某些東西重置.
                // todo 這段邏輯應該要打掉, 改寫在 PlayerAI 內.
				CourtMgr.Get.RealBallCompoment.HideBallSFX();
                for(int i = 0; i < PlayerList.Count; i++)
                {
                    if(newSituation == EGameSituation.GamerPickBall || 
                       newSituation == EGameSituation.NPCPickBall)
                    {
                        PlayerList[i].SetToAI();
                        PlayerList[i].ResetMove();
                    }												
                    
                    switch(PlayerList[i].Team)
                    {
                        case ETeamKind.Self:
                            if(newSituation == EGameSituation.NPCInbounds || (oldgs == EGameSituation.NPCInbounds && newSituation == EGameSituation.NPCAttack) == false)
                            {
                                if(!PlayerList[i].AIing)
                                {
                                    if(!(newSituation == EGameSituation.GamerAttack || newSituation == EGameSituation.NPCAttack))
                                        PlayerList[i].ResetFlag();
                                } else
                                    PlayerList[i].ResetFlag();
						    }

						break;
					case ETeamKind.Npc:
						if((newSituation == EGameSituation.GamerInbounds || (oldgs == EGameSituation.GamerInbounds && newSituation == EGameSituation.GamerAttack)) == false)
							PlayerList[i].ResetFlag();

						break;
					}

					PlayerList[i].situation = newSituation;
                }
            }

            Situation = newSituation;

            // todo 這兩行應該要拉到 StateExit.(目前看到是邊界發球時, 會把牆壁關掉, 所以才會有這個重新開啟的行為)
			CourtMgr.Get.Walls[0].SetActive(true);
			CourtMgr.Get.Walls[1].SetActive(true);

			switch(newSituation)
            {
			case EGameSituation.Presentation:
			case EGameSituation.InitCourt:
				break;
			case EGameSituation.CameraMovement:
				if (oldgs != newSituation) {
					InitIngameAnimator();
					CameraMgr.Get.PlayGameStartCamera();
					CameraMgr.Get.ShowEnd();
				}

				break;
			case EGameSituation.None:
				UIGame.UIShow(true);
				UIGame.UIShow(false);
				UIGameResult.UIShow(true);
				UIGameResult.UIShow(false);
				UIGameLoseResult.UIShow(true);
				UIGameLoseResult.UIShow(false);
				UIDoubleClick.UIShow(true);
				UIPassiveEffect.UIShow(true);
				UITransition.UIShow(true);
				UITransition.UIShow(false);
				UICourtInstant.UIShow(true);
				UICourtInstant.UIShow(false);
				UIInGameMission.UIShow(true);
				UIInGameMission.UIShow(false);
				break;
			case EGameSituation.Opening:
			case EGameSituation.JumpBall:
				UIGame.UIShow(true);
				UIInGameMission.UIShow(true);

				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].IsCanCatchBall = true;
				break;
			case EGameSituation.GamerAttack:
			case EGameSituation.NPCAttack:
				break;
			case EGameSituation.GamerPickBall:
				hideAllEnemySelect();
                break;
            case EGameSituation.GamerInbounds:
				hideAllEnemySelect();
				CourtMgr.Get.Walls[1].SetActive(false);
				EffectManager.Get.PlayEffect("ThrowInLineEffect", Vector3.zero);
				AudioMgr.Get.PlaySound(SoundType.SD_Line);
				UITransition.Get.SelfAttack();
                mInboundsBallOnlyOnce = true;
                break;
            case EGameSituation.NPCPickBall:
				PickBallPlayer = null;
				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].IsCanCatchBall = true;
                break;
			case EGameSituation.NPCInbounds:
				CourtMgr.Get.Walls[0].SetActive(false);
				EffectManager.Get.PlayEffect("ThrowInLineEffect", Vector3.zero);
				AudioMgr.Get.PlaySound(SoundType.SD_Line);
				UITransition.Get.SelfOffense();
                mInboundsBallOnlyOnce = true;
                break;
			case EGameSituation.End:
				
				IsFinish = true;
				UIGame.Get.GameOver();

				if (GameData.IsPVP) {
					WWWForm form = new WWWForm();
					form.AddField("Score1", UIGame.Get.Scores [0]);
					form.AddField("Score2", UIGame.Get.Scores [1]);
					SendHttp.Get.Command(URLConst.PVPEnd, waitPVPEnd, form, false);
					GameData.PVPEnemyMembers[0].Identifier = string.Empty;
				}
					
//				CameraMgr.Get.SetCameraSituation(ECameraSituation.Finish);
            	break;
            }

			if (GamePlayTutorial.Visible)
				GamePlayTutorial.Get.CheckSituationEvent(newSituation.GetHashCode());
        }
	}

	private void waitPVPEnd(bool ok, WWW www)
	{
		if (ok) {
			beforeTeam.PVPLv = GameData.Team.PVPLv;
			beforeTeam.PVPIntegral = GameData.Team.PVPIntegral;
			beforeTeam.PVPCoin = GameData.Team.PVPCoin;
			TPVPResult reslut = JsonConvert.DeserializeObject <TPVPResult>(www.text, SendHttp.Get.JsonSetting); 
			afterTeam.PVPLv = reslut.PVPLv;
			afterTeam.PVPIntegral = reslut.PVPIntegral;
			afterTeam.PVPCoin = reslut.PVPCoin;

            GameData.Team.PVPLv = reslut.PVPLv;
			GameData.Team.PVPIntegral = reslut.PVPIntegral;
			GameData.Team.PVPCoin = reslut.PVPCoin;
			GameData.Team.LifetimeRecord = reslut.LifetimeRecord;
			if(isEndShowScene) { //進去的話就表示還沒回傳就跑完End Game
				if(IsWinner) {
					UIGameResult.UIShow(true);
					UIGameResult.Get.SetGameRecord(ref GameRecord);
					UIGameResult.Get.SetPVPData(beforeTeam, afterTeam);
				} else {
					UIGameLoseResult.UIShow(true);
					UIGameLoseResult.Get.SetPVPData(beforeTeam, afterTeam);
				}
			}
		}
	}

    private void setMoveFrontCourtTactical(PlayerBehaviour player)
    {
        // 狀態是邊界發球變成任何其它狀態時, 會設定球員的戰術路徑.
        // 但我認為這應該只是攻守轉換的第一次, 要指定 Fast 戰術.
        // todo 這段程式碼應該要移動到 PlayerAI.
        AITools.RandomTactical(ETacticalAuto.MoveFrontCourt, player.Index, out attackTactical);

        if(attackTactical.Name != string.Empty)
        {
            for(int i = 0; i < PlayerList.Count; i ++)
            {
                PlayerBehaviour sameTeamPlayer = PlayerList[i];
                if(sameTeamPlayer.Team != player.Team)
                    continue;

                tacticalActions = attackTactical.GetActions(sameTeamPlayer.Index);
                if(tacticalActions == null)
                    continue;

                for(int j = 0; j < tacticalActions.Length; j++)
                {
                    moveData.Clear();
                    moveData.Speedup = tacticalActions[j].Speedup;
                    moveData.Catcher = tacticalActions[j].Catcher;
                    moveData.Shooting = tacticalActions[j].Shooting;
                    moveData.TacticalName = attackTactical.Name;
                    if(sameTeamPlayer.Team == ETeamKind.Self)
                        moveData.SetTarget(tacticalActions[j].X, tacticalActions[j].Z);
                    else
                        moveData.SetTarget(tacticalActions[j].X, -tacticalActions[j].Z);

                    if(BallOwner != null && BallOwner != sameTeamPlayer)
                        moveData.LookTarget = BallOwner.PlayerRefGameObject.transform;

                    sameTeamPlayer.TargetPos = moveData;
                }
            }
        }
    }

    private void handleSituation()
    {
        if (PlayerList.Count > 0)
        {
            //Action
			if(LobbyStart.Get.TestMode == EGameTest.All || LobbyStart.Get.TestMode == EGameTest.None) 
            {
	            switch(Situation)
	            {
					case EGameSituation.Presentation:
					case EGameSituation.CameraMovement:
					case EGameSituation.InitCourt:
	                case EGameSituation.None:
	                case EGameSituation.Opening:
	                case EGameSituation.JumpBall:
	                case EGameSituation.GamerAttack:
	                case EGameSituation.NPCAttack:
                        break;
	                case EGameSituation.GamerPickBall:
	                    SituationPickBall(ETeamKind.Self);
	                    break;
	                case EGameSituation.GamerInbounds:
	                    SituationInbounds(ETeamKind.Self);
	                    break;
	                case EGameSituation.NPCPickBall:
	                    SituationPickBall(ETeamKind.Npc);
	                    break;
	                case EGameSituation.NPCInbounds:
	                    SituationInbounds(ETeamKind.Npc);
	                    break;
	                case EGameSituation.End:
	                    break;
	            }
			}
        }
    }
	
	private int judgeShootAngle(PlayerBehaviour player){
		float angle = 0;
		int distanceType = 0;
		if(player.name.Contains("Self")) {
			angleByPlayerHoop = MathUtils.FindAngle(CourtMgr.Get.Hood[0].transform, player.PlayerRefGameObject.transform.position);
			angle = Mathf.Abs(angleByPlayerHoop) - 90;
		} else {
			angleByPlayerHoop = MathUtils.FindAngle(CourtMgr.Get.Hood[1].transform, player.PlayerRefGameObject.transform.position);
			angle = Mathf.Abs(angleByPlayerHoop) - 90;
		}
		//Distance
		if(ShootDistance >= 0 && ShootDistance < 9)
			distanceType = 0;
		else 
		if(ShootDistance >= 9 && ShootDistance < 12) 
			distanceType = 1;
		else 
		if(ShootDistance >= 12) 
			distanceType = 2;
		
		//Angle
		if(angle > 60) {// > 60 degree
			if(distanceType == 0)
				return (int)EBasketDistanceAngle.ShortCenter;
			else if (distanceType == 1)
				return (int)EBasketDistanceAngle.MediumCenter;
			else if (distanceType == 2)
				return (int)EBasketDistanceAngle.LongCenter;
		} else 
		if(angle <= 60 && angle > 10){// > 10 degree <= 60 degree
			if(angleByPlayerHoop > 0) {//right
				if(distanceType == 0)
					return (int)EBasketDistanceAngle.ShortRight;
				else if (distanceType == 1)
					return (int)EBasketDistanceAngle.MediumRight;
				else if (distanceType == 2)
					return (int)EBasketDistanceAngle.LongRight;
			} else {//left
				if(distanceType == 0)
					return (int)EBasketDistanceAngle.ShortLeft;
				else if (distanceType == 1)
					return (int)EBasketDistanceAngle.MediumLeft;
				else if (distanceType == 2)
					return (int)EBasketDistanceAngle.LongLeft;
			}
		} else 
		if(angle <= 10 && angle >= -30){ // < 10 degree
			if(angleByPlayerHoop > 0){ // right
				if(distanceType == 0)
					return (int)EBasketDistanceAngle.ShortRightWing;
				else if (distanceType == 1)
					return (int)EBasketDistanceAngle.MediumRightWing;
				else if (distanceType == 2)
					return (int)EBasketDistanceAngle.LongRightWing;
			} else {//left
				if(distanceType == 0)
					return (int)EBasketDistanceAngle.ShortLeftWing;
				else if (distanceType == 1)
					return (int)EBasketDistanceAngle.MediumLeftWing;
				else if (distanceType == 2)
					return (int)EBasketDistanceAngle.LongLeftWing;
			}
		}
		return (int)EBasketDistanceAngle.ShortCenter;
	}

	private void calculationScoreRate(PlayerBehaviour player, EScoreType type, bool isActive = false) {
		//Score Rate
		float originalRate = 0;
		if(ShootDistance >= GameConst.Point3Distance) {
			EffectManager.Get.PlayEffect("ThreeLineEffect", Vector3.zero, null, null, 0);
		} 
			originalRate = player.Attr.PointRate2;

		randomrate = 0;
		normalRate = 0;
		uphandRate = 0;
		downhandRate = 0;
		nearshotRate = 0;
		layupRate = 0;


		float rate = (Random.Range(0f, 100f) + 1);
		randomrate = rate;
		int airRate = (Random.Range(0, 100) + 1);
		bool isScore = false;
		bool isSwich = false;
		bool isAirBall = false;
		if(type == EScoreType.DownHand) {
			downhandRate = GameFunction.ShootingCalculate(player, originalRate, player.ScoreRate.DownHandScoreRate, extraScoreRate, ShootDistance);
			isScore = (rate <= downhandRate)? true : false;
			if(isScore) {
				rate = (Random.Range(0, 100) + 1);
				isSwich = (rate <= GameFunction.ShootingCalculate(player, originalRate, player.ScoreRate.DownHandSwishRate, extraScoreRate, -1)) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.DownHandAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.UpHand) {
			uphandRate = GameFunction.ShootingCalculate(player, originalRate, player.ScoreRate.UpHandScoreRate, extraScoreRate, ShootDistance);
			isScore = (rate <= uphandRate) ? true : false;
			if(isScore) {
				rate = (Random.Range(0, 100) + 1);
				isSwich = (rate <= GameFunction.ShootingCalculate(player, originalRate, player.ScoreRate.UpHandSwishRate, extraScoreRate, -1)) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.UpHandAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.Normal) {
			normalRate = GameFunction.ShootingCalculate(player, originalRate, player.ScoreRate.NormalScoreRate, extraScoreRate, ShootDistance);
			isScore = (rate <= normalRate) ? true : false;
			if(isScore) {
				rate = (Random.Range(0, 100) + 1);
				isSwich = (rate <= GameFunction.ShootingCalculate(player, originalRate, player.ScoreRate.NormalSwishRate, extraScoreRate, -1)) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.NormalAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.NearShot) {
			nearshotRate = GameFunction.ShootingCalculate(player, originalRate, player.ScoreRate.NearShotScoreRate, extraScoreRate, ShootDistance);	
			isScore = (rate <= nearshotRate) ? true : false;
			if(isScore) {
				rate = (Random.Range(0, 100) + 1);
				isSwich = (rate <=  GameFunction.ShootingCalculate(player, originalRate, player.ScoreRate.NearShotSwishRate, extraScoreRate, -1)) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.NearShotAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.LayUp) {
			layupRate = GameFunction.ShootingCalculate(player, originalRate, player.ScoreRate.LayUpScoreRate, extraScoreRate, ShootDistance);
			isScore = (rate <= layupRate) ? true : false;
			if(isScore) {
				rate = (Random.Range(0, 100) + 1);
				isSwich = (rate <= GameFunction.ShootingCalculate(player, originalRate, player.ScoreRate.LayUpSwishRate, extraScoreRate, -1)) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.LayUpAirBallRate ? true : false;
			}
		}
		if(DoubleClickType == EDoubleType.Perfect || DoubleClickType == EDoubleType.Good || ShootDistance < 9)
			isAirBall = false;

		if(DoubleClickType == EDoubleType.Weak || ShootDistance > 15) 
			isSwich = false;

		if(isScore)
        {
			if(isSwich)
				BasketSituation = EBasketSituation.Swish;
			else 
				BasketSituation = EBasketSituation.Score;

			player.GameRecord.ShotError++;
        } else {
			if(isAirBall)
				BasketSituation = EBasketSituation.AirBall;
			else 
				BasketSituation = EBasketSituation.NoScore;
		}

		if(isActive || LobbyStart.Get.TestMode == EGameTest.AttackA)
        {
			BasketSituation = EBasketSituation.Swish;
		}
		
		CourtMgr.Get.JudgeBasketAnimationName (judgeShootAngle(player));
		
		if(LobbyStart.Get.TestMode == EGameTest.PassiveSkill) {
			BasketSituation = EBasketSituation.Score;
			CourtMgr.Get.BasketAnimationName = LobbyStart.Get.SelectBasketState.ToString();	
		}

		if (ShootDistance >= GameConst.Point3Distance)
			player.GameRecord.FG3++;
		else
			player.GameRecord.FG++;
		
		player.GameRecord.PushShot(new Vector2(player.PlayerRefGameObject.transform.position.x, player.PlayerRefGameObject.transform.position.z), BasketSituation.GetHashCode(), rate);
	}

	public void AddExtraScoreRate(float rate) {
		extraScoreRate = rate;
	}

    /// <summary>
	/// 做真正的投籃行為.
	//Call From UIGame
    /// </summary>
    /// <returns></returns>
	public bool DoShoot()
    {
        if(BallOwner)
        {
            // 有持球者才可以投籃.
			Vector3 v = CourtMgr.Get.GetShootPointPosition(BallOwner.Team);

			ShootDistance = GetDis(BallOwner, new Vector2(v.x, v.z));

			if(LobbyStart.Get.TestMode == EGameTest.Shoot)
            {
				BallOwner.AniState(testState, CourtMgr.Get.Hood[BallOwner.Team.GetHashCode()].transform.position);
				return true;
			} 
             
			if(!BallOwner.IsDunk)
            {
                // 持球者不在灌籃中...
				UIGame.Get.DoPassNone();
				CourtMgr.Get.ResetBasketEntra();

                if(LobbyStart.Get.TestMode == EGameTest.Dunk)
                {
                    BallOwner.AniState(EPlayerState.Dunk20);
					return true;
				}

                if(BallOwner.IsRebound)
                {
                    // 持球者不在灌籃中, 但是搶籃板中 ...
//					if(inTipinDistance(BallOwner)  && BallOwner.CanUseTipIn)
					if(BallOwner.CanUseTipIn)
                    {
						BallOwner.AniState(EPlayerState.TipIn);
						return true;
					}
				}
                else
                {
					if(!BallOwner.IsTipIn)
					{
                    // 持球者不在灌籃和搶籃板狀態.
					shootRandomizer.Clear();
					if(BallOwner.IsMoving)
                    {
						if(ShootDistance > GameConst.LongShootDistance)
							shootRandomizer.AddOrUpdate(ESkillSituation.Shoot3, 50);
					
						if(ShootDistance > GameConst.Point2Distance && ShootDistance <= GameConst.LongShootDistance)
							shootRandomizer.AddOrUpdate(ESkillSituation.Shoot2, 50);
						
						if(ShootDistance > GameConst.ShortShootDistance && ShootDistance <= GameConst.LayupDistance)
							shootRandomizer.AddOrUpdate(ESkillSituation.Layup0, 50);
						else if(ShootDistance > GameConst.LayupDistance){
							if(BallOwner.GetPassiveAniRate(50, ShootDistance, GameConst.LayupDistance) > 0)
								shootRandomizer.AddOrUpdate(ESkillSituation.Layup0, BallOwner.GetPassiveAniRate(50, ShootDistance, GameConst.LayupDistance));
						}
						
						if(ShootDistance <= GameConst.DunkDistance) {
							if(BallOwner.Attr.DunkRate > 0)
								shootRandomizer.AddOrUpdate(ESkillSituation.Dunk0, BallOwner.Attr.DunkRate);
						} else {
							if(BallOwner.GetPassiveAniRate(60, ShootDistance, GameConst.DunkDistance) > 0)
								shootRandomizer.AddOrUpdate(ESkillSituation.Dunk0, BallOwner.GetPassiveAniRate(60, ShootDistance, GameConst.DunkDistance));
						}
					
						if(ShootDistance <= GameConst.ShortShootDistance)
							shootRandomizer.AddOrUpdate(ESkillSituation.Shoot1, 50);
						else {
							if(BallOwner.GetPassiveAniRate(40, ShootDistance, GameConst.ShortShootDistance) > 0)
								shootRandomizer.AddOrUpdate(ESkillSituation.Shoot1, BallOwner.GetPassiveAniRate(40, ShootDistance, GameConst.ShortShootDistance));
						}

						BallOwner.PlayerSkillController.DoPassiveSkill(shootRandomizer.GetNext(), CourtMgr.Get.GetHoodPosition(BallOwner.Team), ShootDistance);
					}
                    else
                    {
                        // 站在原地投籃.
						if(ShootDistance > GameConst.LongShootDistance)
							shootRandomizer.AddOrUpdate(ESkillSituation.Shoot3, 50);
					
						if(ShootDistance > GameConst.ShortShootDistance && ShootDistance <= GameConst.LongShootDistance)
							shootRandomizer.AddOrUpdate(ESkillSituation.Shoot0, 50);
						
						if(ShootDistance <= GameConst.DunkDistanceNoMove) {
							if(BallOwner.Attr.DunkRate > 0)
								shootRandomizer.AddOrUpdate(ESkillSituation.Dunk0, BallOwner.Attr.DunkRate);
						} else {
							if(BallOwner.GetPassiveAniRate(60, ShootDistance, GameConst.DunkDistanceNoMove) > 0)
								shootRandomizer.AddOrUpdate(ESkillSituation.Dunk0, BallOwner.GetPassiveAniRate(60, ShootDistance, GameConst.DunkDistanceNoMove));
						}
						
						if(ShootDistance <= GameConst.ShortShootDistance)
							shootRandomizer.AddOrUpdate(ESkillSituation.Shoot1, 50);
						else {
							if(BallOwner.GetPassiveAniRate(40, ShootDistance, GameConst.ShortShootDistance) > 0)
								shootRandomizer.AddOrUpdate(ESkillSituation.Shoot1, BallOwner.GetPassiveAniRate(40, ShootDistance, GameConst.ShortShootDistance));
						}
						
						BallOwner.PlayerSkillController.DoPassiveSkill(shootRandomizer.GetNext(), CourtMgr.Get.GetHoodPosition(BallOwner.Team), ShootDistance);
					}

					return true;
					}	
				}
			}
        }

		return false;
	}
        
    /// <summary>
    /// 呼叫時機: 撥投籃動作時, 由 Event 觸發.(通常會是投籃動作的中後段才會發出這個 event)
	/// Delegate
	/// Acitve = ActiveSkill
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public void OnShooting([NotNull]PlayerBehaviour player, bool isActive = false)
    {
        if(BallOwner && BallOwner == player)
		{     
            CourtMgr.Get.RealBallCompoment.Trigger.IsAutoRotate = true;
			CourtMgr.Get.IsBallOffensive = true;
			Shooter = player;
			UIGame.Get.SetPassButton();
            if (!isActive)
            {
                BallState = EBallState.CanBlock;
            }

			EScoreType scoreType = EScoreType.Normal;
			if(player.Team == ETeamKind.Self) 
				angleByPlayerHoop = MathUtils.FindAngle(CourtMgr.Get.Hood[0].transform, player.PlayerRefGameObject.transform.position);
			else 
				angleByPlayerHoop = MathUtils.FindAngle(CourtMgr.Get.Hood[1].transform, player.PlayerRefGameObject.transform.position);

			if(Mathf.Abs(angleByPlayerHoop) <= 85  && ShootDistance < 5)
				shootAngle = 80;
			else
				shootAngle = 50;

			if(isActive) {
				shootAngle = 66 - (ShootDistance * 0.6f);
				scoreType = EScoreType.UpHand;
			}

			if(player.crtState == EPlayerState.TipIn){
				scoreType = EScoreType.LayUp;
				player.GameRecord.TipinLaunch++;
			} else 
			if(player.GetSkillKind == ESkillKind.NearShoot) 
				scoreType = EScoreType.NearShot;
			else 
			if(player.GetSkillKind == ESkillKind.UpHand) 
				scoreType = EScoreType.UpHand;
			else 
			if(player.GetSkillKind == ESkillKind.DownHand) 
				scoreType = EScoreType.DownHand;
			else 
			if(player.GetSkillKind == ESkillKind.Layup) {
				scoreType = EScoreType.LayUp;
			}

			calculationScoreRate(player, scoreType, isActive);
			//確實把球放在手上
			SetBall();
			//再把球的持有者設成null
            CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.Shooting);
			CourtMgr.Get.RealBallShoot(player, shootAngle, ShootDistance);


            for (int i = 0; i < PlayerList.Count; i++)
				if(Shooter != null) {
					if (PlayerList [i].Team == Shooter.Team)
						PlayerList [i].ResetMove();
				}
        }
    }

	//Call From UIGame
	public int GetShootPlayerIndex()
	{
		int result = -1;

		for(int i = 0; i < PlayerList.Count; i++)
		{
			if(Shooter && PlayerList[i] == Shooter)
				result = i;
			else if(BallOwner && PlayerList[i] == BallOwner)
				result = i;
		}

		return result;
	}

	//Call From UIGame
	public bool DoShoot(bool isshoot)
    {
        if (StageData.IsTutorial)
            isshoot = true;

		if (IsStart && CandoBtn) {
            if (Joysticker == BallOwner) {
				if (isshoot)
					return DoShoot ();
				else
					return Joysticker.AniState(EPlayerState.FakeShoot, CourtMgr.Get.ShootPoint [Joysticker.Team.GetHashCode()].transform.position);
            } else //someone else shot
			if (BallOwner && BallOwner.Team == ETeamKind.Self)
            {
//                Debug.Log("UIGame Do Shoot.");

                // 如果是玩家命令自己的隊友投球, 要暫時將投球隊友的 AI 關閉.
                // 這麼做的原因是避免球員跳起來後, 在空中傳球. 
                // 所以當玩家命令自己隊友投球時, 就是要準確的投出.
                // 將 AI 停止 1.8 秒是我測試的結果, 讓球員真的準確投出球.
                StartCoroutine(disablePlayerAIForShortTime(BallOwner, 1.8f));
				DoShoot();
			} else 
			if (!Joysticker.IsRebound && IsReboundTime)
				return Rebound(Joysticker);
        }

		return false;
    }

    private IEnumerator disablePlayerAIForShortTime(PlayerBehaviour player, float delayTime)
    {
        var playerAI = player.GetComponent<PlayerAI>();
        if(playerAI)
            playerAI.enabled = false;

        yield return new WaitForSeconds(delayTime);

        if(playerAI)
            playerAI.enabled = true;
    }

	//Call From UIGame
	public bool DoPush(PlayerBehaviour nearP)
	{
		if(Joysticker)
        {
			if(nearP)
				return Joysticker.PlayerSkillController.DoPassiveSkill (ESkillSituation.Push0, nearP.PlayerRefGameObject.transform.position);

			return Joysticker.PlayerSkillController.DoPassiveSkill (ESkillSituation.Push0);
		}

        return false;
	}

	//Call From UIGame
	public bool DoElbow()
	{
		if(Joysticker && !Joysticker.IsPass && Joysticker.IsBallOwner)
			return Joysticker.PlayerSkillController.DoPassiveSkill(ESkillSituation.Elbow0);

        return false;
	}

	//Call Form UIGame
	public bool DoPass(int playerid) {
		if (IsStart && BallOwner && Joysticker && BallOwner.Team == 0 && CandoBtn && 
			playerid < PlayerList.Count && (!Shooter || IsCanPassAir) && 
			!BallOwner.IsElbow && !BallOwner.IsPass) {
			return TryPass(PlayerList [playerid], false, true);
		}

		return false;
	}

	//Call From UIGame
	public bool DoSteal()
	{
		if (StealBtnLiftTime <= 0 && IsStart && Joysticker && CandoBtn) {
			StealBtnLiftTime = 1f;
			if (BallOwner && BallOwner.Team != Joysticker.Team) {
				Joysticker.RotateTo(BallOwner.PlayerRefGameObject.transform.position.x, BallOwner.PlayerRefGameObject.transform.position.z);
				return Joysticker.PlayerSkillController.DoPassiveSkill(ESkillSituation.Steal0, BallOwner.PlayerRefGameObject.transform.position);
			} else
				return Joysticker.PlayerSkillController.DoPassiveSkill(ESkillSituation.Steal0);
		} else
			return false;
	}

	//Call From UIGame
	public bool DoBlock() {
		if (IsStart && CandoBtn && Joysticker) {
			if(Joysticker.IsBlock && Joysticker.IsPerfectBlockCatch) {
				Joysticker.AniState(EPlayerState.BlockCatch);
				if(UIDoubleClick.Get.DoubleClicks[0].Enable)
					UIDoubleClick.Get.ClickStop(0);

				return true;
			} else {
				if (Shooter) {
					if(IsReboundTime)
						return Rebound(Joysticker);
					else
						return Joysticker.PlayerSkillController.DoPassiveSkill(ESkillSituation.Block0, Shooter.PlayerRefGameObject.transform.position);
				} else {
					if (BallOwner) {
						Joysticker.RotateTo(BallOwner.PlayerRefGameObject.transform.position.x, BallOwner.PlayerRefGameObject.transform.position.z);
						return Joysticker.PlayerSkillController.DoPassiveSkill(ESkillSituation.Block0, BallOwner.PlayerRefGameObject.transform.position);
					} else {
						if (!Shooter && Joysticker.InReboundDistance && IsReboundTime && LobbyStart.Get.TestMode == EGameTest.None)
							return Rebound(Joysticker);
						else
							return Joysticker.PlayerSkillController.DoPassiveSkill(ESkillSituation.Block0);
					}
				}
			}
		}

		return false;
	}

	//Call From UIGame
	public bool OnSkill(TSkill tSkill) {
		if (CandoBtn && DoSkill(Joysticker, tSkill)) {			
			UIGame.Get.ResetRange();
			return true;
		} else
			return false;
	}

	public bool DoSkill(PlayerBehaviour player, TSkill tSkill)
	{
		bool result = false;
		if((player.CanUseActiveSkill(tSkill) && !CheckOthersUseSkill(player.TimerKind.GetHashCode())) || LobbyStart.Get.TestMode == EGameTest.Skill)
		{
			if ((player.CheckSkillDistance(tSkill) && player.PlayerSkillController.CheckSkillKind(tSkill)) || LobbyStart.Get.TestMode == EGameTest.Skill) {
//                TimerMgr.Get.SetTimeController(ref player);
				if(GameData.DSkillData.ContainsKey(tSkill.ID)) {
					if(GameData.DSkillData[tSkill.ID].Kind == 40){//鷹眼神射
						Vector3 v = CourtMgr.Get.GetShootPointPosition(BallOwner.Team);
						ShootDistance = GetDis(BallOwner, new Vector2(v.x, v.z));
					}
				}
				player.ActiveSkillUsed = tSkill;
				result = player.DoActiveSkill(player.PlayerRefGameObject);
				if(result){
					player.PlayerSkillController.CheckSkillValueAdd(tSkill);
					UIGame.Get.RefreshSkillUI();
				}
			}
		}
		return result;
	}

	//Call From Delegate
	public bool OnOnlyScore(PlayerBehaviour player) {
		if (player == BallOwner)
		{
			player.GameRecord.Dunk++;
			if (ShootDistance >= GameConst.Point3Distance)
				player.GameRecord.FG3++;
			else
				player.GameRecord.FG++;
			
			PlusScore(player.Team.GetHashCode(), true, false);
			ShowWord(EShowWordType.Dunk, player.Team.GetHashCode());

			return true;
		} else
			return false;
	}

	//Call From Delegate
    public bool OnDunkBasket(PlayerBehaviour player)
    {
        if (player == BallOwner)
        {
			if (player.crtState == EPlayerState.Alleyoop){
				player.GameRecord.Alleyoop++;
				ShootDistance = 0; //maybe score 3, so it sets 0.
			}else
				ShowWord(EShowWordType.Dunk, player.Team.GetHashCode());

			CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.DunkBasket);

			player.GameRecord.ShotError++;
			player.GameRecord.Dunk++;
			if (ShootDistance >= GameConst.Point3Distance)
				player.GameRecord.FG3++;
			else
				player.GameRecord.FG++;

			PlusScore(player.Team.GetHashCode(), player.IsUseActiveSkill, true);
            SetBall();


            return true;
        } else
            return false;
    }

	//Call From Delegate
    public bool OnDunkJump(PlayerBehaviour player)
    {
        if (player == BallOwner)
        {
            Shooter = player;
			Vector3 v = CourtMgr.Get.ShootPoint [Shooter.Team.GetHashCode()].transform.position;
            ShootDistance = GetDis(Shooter, new Vector2(v.x, v.z));
			player.GameRecord.DunkLaunch++;
            return true;
        } else
            return false;
    }

    /// <summary>
    /// 要求持球者將球傳給 catchPlayer.(不見得真的會傳出球)
    /// </summary>
    /// <param name="catchPlayer"></param>
    /// <param name="isInbounds"> true: 攻守轉換的邊界發球. </param>
    /// <param name="isPassUIPress"> true: 介面的傳球 UI 按下. </param>
    /// <param name="movePass"></param>
    /// <returns></returns>
    public bool TryPass(PlayerBehaviour catchPlayer, bool isInbounds = false, bool isPassUIPress = false, bool movePass = false)
    {
        Func<bool> isCanPass = () =>
        {
            if(IsShooting)
            {
                if(catchPlayer.Team == ETeamKind.Self)
                {
                    if(!isPassUIPress)
                        return false;
                    if(!IsCanPassAir)
                        return false;
                }
                else if(catchPlayer.Team == ETeamKind.Npc && !IsCanPassAir)
                    return false;
            }

            return true;
        };

        if(!BallOwner || BallOwner == catchPlayer || IsDunk || IsPassing || !isCanPass())
            return false;

        if(!(isPassUIPress || movePass) && !PassCD.IsTimeUp())
            return false;
				
        if(!isPassUIPress && !BallOwner.AIing)
            return false;

        bool result = false;

        if(isInbounds)
        {
            if(BallOwner.AniState(EPlayerState.Pass50, catchPlayer.PlayerRefGameObject.transform.position))
            {
                Catcher = catchPlayer;
                return true;
            }												
        }
        else if(IsCanPassAir)
        {
            if(BallOwner.AniState(EPlayerState.Pass4, catchPlayer.transform.position))
            {
                Catcher = catchPlayer;
                result = true;
            }
        }
        else
        {
            // 以下處理的是遊戲進行中的傳球.
            float dis = Vector3.Distance(BallOwner.transform.position, catchPlayer.transform.position);
            int disKind = getEnemyDis(catchPlayer); // 這控制選擇撥哪種傳球.
            int rate = Random.Range(0, 2); // 這控制選擇撥哪種傳球.
            if(catchPlayer.crtState == EPlayerState.Alleyoop)
            {
                IsCatcherAlleyoop = true;
                result = BallOwner.AniState(EPlayerState.Pass0, catchPlayer.PlayerRefGameObject.transform.position);
            }
            else if(dis <= GameConst.FastPassDistance)
            {
                result = BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass5, catchPlayer.PlayerRefGameObject.transform.position);
            }
            else if(dis <= GameConst.CloseDistance)
            {
                // 近距離傳球.
                if(disKind == 1)
                {
                    if(rate == 1)
                        result = BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass1, catchPlayer.PlayerRefGameObject.transform.position);
                    else 
                        result = BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass2, catchPlayer.PlayerRefGameObject.transform.position);
                }
                else
                {
                    // 這其實是重複的程式碼 ....
                    if(rate == 1)
                        result = BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass0, catchPlayer.PlayerRefGameObject.transform.position);
                    else
                        result = BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass2, catchPlayer.PlayerRefGameObject.transform.position);
                }
            }
            else if(dis <= GameConst.MiddleDistance)
            {
                // 中距離傳球.
                if(disKind == 2)
                {
                    if(rate == 1)
                        result = BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass1, catchPlayer.PlayerRefGameObject.transform.position);
                    else
                        result = BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass2, catchPlayer.PlayerRefGameObject.transform.position);
                }						
                else
                {
                    // 這也是重複的程式碼.
                    if(rate == 1)
                        result = BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass0, catchPlayer.PlayerRefGameObject.transform.position);
                    else
                        result = BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass2, catchPlayer.PlayerRefGameObject.transform.position);
                }
            }
            else
            {
                // 遠距離傳球.
                result = BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass1, catchPlayer.PlayerRefGameObject.transform.position);
            }
					
            if(result)
            {
                Catcher = catchPlayer;
                UIGame.Get.DoPassNone();
            }
        }

        return result;
    }

	private int getEnemyDis(PlayerBehaviour player)
	{
		float[] disAy = new float[3];
		int index = 0;
		for (int i = 0; i < PlayerList.Count; i++)
		{
			if (PlayerList[i].Team != player.Team)
			{
				disAy[index] = Vector3.Distance(player.transform.position, PlayerList[i].transform.position);
				index++;
			}		
		}

		for (int i = 0; i < disAy.Length; i++)
		{
			if (disAy[i] > 0)
			{
				if (disAy[i] <= GameConst.StealPushDistance)
					return 2;
				if (disAy[i] <= GameConst.DefDistance)
					return 1;
			}
		}

		return 0;
	}

	public void PlayShowAni(int playIndex, string aniName)
	{
		for (int i = 0; i < PlayerList.Count; i++) {
			if (IsShowSituation && PlayerList [i].ShowPos == playIndex)
				PlayerList [i].AnimatorControl.SetTrigger (aniName);
		}
	}

	//Call From Delegate
	public bool OnFall(PlayerBehaviour faller)
	{
		UIGame.Get.UICantUse(faller);
		if (faller && BallOwner == faller) {
			doDropBall();
			return true;
		}

		return false;
	}

    /// <summary>
    /// 呼叫時機: 球員撥抄截動作, 在動作撥大概 40% 左右時, 會發出的 event.
	/// Call From Delegate
    /// </summary>
    /// <param name="player"> 執行抄截的球員. </param>
    /// <returns> true: 抄截成功; false:抄截失敗. </returns>
	public bool OnStealMoment(PlayerBehaviour player, float dis, float angle)
    {
		if (StageData.IsTutorial) {
			dis = 30;
			angle = 360;
		}
        if(BallOwner && BallOwner.Invincible.IsOff() && !IsShooting && !IsDunk)
        {
			if(player.PlayerRefGameObject.transform.IsInFanArea(BallOwner.PlayerRefGameObject.transform.position, dis, angle))
			{
				player.IsStealCalculate = false;
				int probability = Mathf.RoundToInt(player.Attribute.Steal - BallOwner.Attribute.Dribble);
                probability = Mathf.Clamp(probability, 10, 100);

				int addRate = 0;
				int addAngle = 0;
				if(CourtMgr.Get.RealBallCompoment.IsBallSFXEnabled())
                    // 特效開啟, 就表示被懲罰的機率增加.
					addRate = 30;

				if(Vector3.Distance(BallOwner.PlayerRefGameObject.transform.position, CourtMgr.Get.Hood[BallOwner.Team.GetHashCode()].transform.position) <= GameConst.DunkDistance)
                {
                    // 持球者靠近籃下時, 被抄截的機率增加, 抄截判定的範圍也加大.
					addRate += 40;
					addAngle = 90;
				}

				if (StageData.IsTutorial) {
					addRate = 100;
					addAngle = 360;
					probability = 100;
				}

				if(Random.Range(0, 100) <= (probability + addRate) && 
				   Mathf.Abs(MathUtils.FindAngle(player.PlayerRefGameObject.transform, BallOwner.PlayerRefGameObject.transform.position)) <= 90 + addAngle)
                {
					if (StageData.IsTutorial)
						SetBall(player);

                    // 持球者嘗試撥被抄截的懲罰動作.
					if(BallOwner.AniState(EPlayerState.GotSteal))
                    {
                        // 抄截成功.
						BallOwner.SetAnger(GameConst.DelAnger_Stealed);
						//抄截成功直接嗆到手上（20160215）
						SetBall(player);
						ShowWord(EShowWordType.Steal, 0, player.ShowWord);

						player.GameRecord.Steal++;
						IsGameFinish ();
						return true;
					}
				}

                // 再 random 一次, 判斷要不要進入懲罰
                if (Random.Range(0, 100) <= probability)
                {
                    // 進入懲罰.
					CourtMgr.Get.RealBallCompoment.ShowBallSFX(player.Attr.PunishTime);
				}
			}
        }

        return false;
    }

    /// <summary>
	/// Call From Delegate
    /// </summary>
    /// <param name="player"></param>
    /// <returns> true:被抄截. </returns>
	public bool OnGotSteal(PlayerBehaviour player)
	{
		if(BallOwner == player)
        {
			doDropBall(player);
			return true;
		} 

		return false;
	}

	//Call From Delegate
	public bool OnFakeShootBlockMoment(PlayerBehaviour player)
	{
		if (player)
		{
			defBlock(ref player, true);
			return true;
		} else
			return false;
	}

	//Call From Delegate
    public bool OnBlockMoment(PlayerBehaviour player)
    {
        if (player)
        {
            defBlock(ref player, false);
            return true;
        } else
            return false;
    }

	//Call From Delegate
	public bool OnDoubleClickMoment(PlayerBehaviour player, EPlayerState state)
	{
		if (player.Team == ETeamKind.Self && (Situation == EGameSituation.GamerAttack || Situation == EGameSituation.NPCAttack)) {
            int playerindex = -1;
            if (Joysticker)
                Joysticker.GameRecord.DoubleClickLaunch++;

			for(int i = 0;i < PlayerList.Count;i++)
				if(PlayerList[i] == player)
					playerindex = i;

			switch (state) {
				case EPlayerState.Shoot0:
					UIDoubleClick.Get.SetData(EDoubleClick.Shoot, playerindex, 1.3f, DoubleShoot);
					return true;
				case EPlayerState.Shoot1:
					UIDoubleClick.Get.SetData(EDoubleClick.Shoot, playerindex, 1.23f, DoubleShoot);
					return true;
				case EPlayerState.Shoot2:
					UIDoubleClick.Get.SetData(EDoubleClick.Shoot, playerindex, 1.3f, DoubleShoot);
					return true;
				case EPlayerState.Shoot3:
					UIDoubleClick.Get.SetData(EDoubleClick.Shoot, playerindex, 1.3f, DoubleShoot);
					return true;
				case EPlayerState.Shoot6:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData(EDoubleClick.Shoot, playerindex, 1.3f, DoubleShoot);
					return true;
				case EPlayerState.Layup0:
				case EPlayerState.Layup1:
				case EPlayerState.Layup2:
				case EPlayerState.Layup3:
					UIDoubleClick.Get.SetData(EDoubleClick.Shoot, playerindex, 1.3f, DoubleShoot);
					return true;

				case EPlayerState.Block0:
				case EPlayerState.Block1:
				case EPlayerState.Block2:
				case EPlayerState.Block20:
				case EPlayerState.BlockCatch:
					UIDoubleClick.Get.SetData(EDoubleClick.Block, playerindex, 0.7f, null, DoubleBlock, player);
					return true;
				case EPlayerState.Rebound0:
				case EPlayerState.Rebound20:
					if(!player.PlayerSkillController.IsActiveUse)
						UIDoubleClick.Get.SetData(EDoubleClick.Rebound, playerindex, 0.75f, DoubleRebound);
					return true;
			}
		}
		return false;
	}

	public void DoubleShoot(int lv)
	{
		switch (lv) {
			case 0: 
				AddExtraScoreRate(0);
				break;
			case 1: 
				AddExtraScoreRate(GameData.ExtraGreatRate + Mathf.Min(UIDoubleClick.Get.Combo * 4, 20));
				doubleClickWord(UIDoubleClick.Get.Combo);
				break;
			case 2: 
				AddExtraScoreRate(GameData.ExtraPerfectRate + Mathf.Min(UIDoubleClick.Get.Combo * 4, 20));
				doubleClickWord(UIDoubleClick.Get.Combo);
				Joysticker.SetAnger(GameConst.AddAnger_Perfect, CameraMgr.Get.DoubleClickDCBorn, CameraMgr.Get.DoubleClickDCBorn);				
				break;
		}

	}

	private void doubleClickWord (int combo) {
		if (combo < 2)
			ShowWord(EShowWordType.DC5, 0, Joysticker.ShowWord);
		else
			if (combo < 3)
				ShowWord(EShowWordType.DC10, 0, Joysticker.ShowWord);
			else
				if (combo < 4)
					ShowWord(EShowWordType.DC15, 0, Joysticker.ShowWord);
				else
					if (combo < 5)
						ShowWord(EShowWordType.DC20, 0, Joysticker.ShowWord);
					else
						ShowWord(EShowWordType.DC20, 0, Joysticker.ShowWord);
		
	}

			

	public void DoubleBlock(int lv, PlayerBehaviour player){
		switch (lv) {
		case 0: 
			break;
		case 1: 
			if(Shooter)
				CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.Block0, Shooter);
			else
				CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.Block0, player);
			break;
		case 2: 
			SetBall(player);
			ShowWord(EShowWordType.Catch, 0, player.ShowWord);
			break;
		}
	}

	public void DoubleRebound(int lv)
	{
        if (lv > 0)
        {
			AddExtraScoreRate(GameData.ExtraGreatRate + Mathf.Min(UIDoubleClick.Get.Combo * 4, 20));
			DoShoot();
		}
        else
            AddExtraScoreRate(0);
	}

	//Call From Delegate
	public bool OnBlockJump(PlayerBehaviour player)
    {
        if (player.PlayerRigidbody != null)
        {
            if (BallOwner)
            {
				if (Vector3.Distance(Joysticker.PlayerRefGameObject.transform.position, BallOwner.PlayerRefGameObject.transform.position) < 5f)
                    player.PlayerRigidbody.AddForce(player.JumpHight * transform.up + player.PlayerRigidbody.velocity.normalized / 2.5f, ForceMode.Force);
                else
					player.PlayerRigidbody.velocity = GameFunction.GetVelocity(player.PlayerRefGameObject.transform.position, 
					                                                           new Vector3(BallOwner.PlayerRefGameObject.transform.position.x, 3, BallOwner.PlayerRefGameObject.transform.position.z), 70);

                return true;
            } else  
				if (Shooter && Vector3.Distance(player.PlayerRefGameObject.transform.position, CourtMgr.Get.RealBallObj.transform.position) < 5)
            {
				player.PlayerRigidbody.velocity = GameFunction.GetVelocity(player.PlayerRefGameObject.transform.position, 
                    new Vector3(CourtMgr.Get.RealBallObj.transform.position.x, 5, CourtMgr.Get.RealBallObj.transform.position.z), 70);
        
                return true;
            } else
            {
                player.PlayerRigidbody.AddForce(player.JumpHight * transform.up + player.PlayerRigidbody.velocity.normalized / 2.5f, ForceMode.Force);
                return true;
            }
        }

        return false;
    }


	private bool inTipinDistance(PlayerBehaviour player) {
		return Vector2.Distance(new Vector2(player.PlayerRefGameObject.transform.position.x, player.PlayerRefGameObject.transform.position.z), 
		                        new Vector2(CourtMgr.Get.ShootPoint[player.Team.GetHashCode()].transform.position.x, 
		            						CourtMgr.Get.ShootPoint[player.Team.GetHashCode()].transform.position.z)) <= 6;
	}

    private bool Rebound(PlayerBehaviour player)
    {
		return player.PlayerSkillController.DoPassiveSkill(ESkillSituation.Rebound0, CourtMgr.Get.RealBallObj.transform.position);
	}
	
    /// <summary>
    /// 只有攻守交換的時候才會被呼叫. 叫球員照著戰術路線跑.
    /// </summary>
    /// <param name="someone"></param>
    /// <param name="team"></param>
    /// <param name="tactical"></param>
    /// <param name="watchBallOwner"></param>
	private void backToDef(PlayerBehaviour someone, ETeamKind team, ref TTacticalData tactical, 
                           bool watchBallOwner = false)
	{
	    if(tactical.Name == string.Empty)
            return;

//        if(someone.CanMove && someone.WaitMoveTime == 0 && someone.TargetPosNum == 0) // 是否之前設定的戰術跑完.
        if(someone.CanMove && someone.CantMoveTimer.IsOff() && someone.TargetPosNum == 0) // 是否之前設定的戰術跑完.
        {
	        tacticalActions = tactical.GetActions(someone.Index);

            if(tacticalActions == null)
            {
                Debug.LogWarning("tacticalData is null!");
                return;
            }

            someone.ResetMove();
            for(int i = 0; i < tacticalActions.Length; i++)
            {
                moveData.Clear();
                if (LobbyStart.Get.CourtMode == ECourtMode.Full && team == ETeamKind.Self)
                    moveData.SetTarget(tacticalActions[i].X, -tacticalActions[i].Z);
                else
                    moveData.SetTarget(tacticalActions[i].X, tacticalActions[i].Z);
						
                if (BallOwner != null)
					moveData.LookTarget = BallOwner.PlayerRefGameObject.transform;
                else
                {
                    if (team == ETeamKind.Self || LobbyStart.Get.CourtMode == ECourtMode.Half)
                        moveData.LookTarget = CourtMgr.Get.Hood[1].transform;
                    else
                        moveData.LookTarget = CourtMgr.Get.Hood[0].transform;
                }
						
                if(!watchBallOwner)
                    moveData.Speedup = true;

                moveData.TacticalName = tactical.Name;
                someone.TargetPos = moveData;
            }
        }
	}

    /// <summary>
    /// 叫某人跑去球的位置(不見得真的會跑過去). 持球者會跑到界外區, 然後發球; 非持球者會跑企劃編輯的戰術.
    /// </summary>
    /// <param name="someone"></param>
    /// <param name="team"></param>
    /// <param name="data"></param>
    private void inboundsBall(PlayerBehaviour someone, ETeamKind team, ref TTacticalData data)
    {
		if(!IsPassing && (someone.CanMove || someone.CanMoveFirstDribble) && !someone.IsMoving && 
            someone.CantMoveTimer.IsOff() && someone.TargetPosNum == 0)
        {
            // 不是傳球中, 球員可移動, 球員留在原地(沒有移動), 球員並未移動中.
            // todo 我認為這種狀態的判斷應該是有多餘的檢查. 比如 CantMoveTimer 沒必要檢查.
            // Debug.LogFormat("InboundsBall, tactical:{0}", tacticalData);

            moveData.Clear();
			if(LobbyStart.Get.CourtMode == ECourtMode.Full)
			    inboundsFull(someone, team, data);
			else
			    inboundsHalf(someone, data);

            mInboundsBallOnlyOnce = false;
        }
        
        // 感覺這段是預防特殊狀況, 當特殊狀況發生時, 強迫持球員做運球.
        if(someone.CantMoveTimer.IsOn() && someone == BallOwner)
            someone.AniState(EPlayerState.Dribble0);
    }

    /// <summary>
    /// 這個變數的目的只是限制邊界發球時, 只可以跑一次戰術, 跑完後, 就停在原地, 不跑下一個戰術.
    /// 現在因為程式碼很亂, 我又沒有時間整理, 所以, 我就只好硬寫 ...
    /// </summary>
    private bool mInboundsBallOnlyOnce = true;
    /// <summary>
    /// 全場跑位.
    /// </summary>
    /// <param name="someone"></param>
    /// <param name="team"></param>
    /// <param name="data"></param>
    private void inboundsFull(PlayerBehaviour someone, ETeamKind team, TTacticalData data)
    {
        if(someone == BallOwner)
        {
            // 持球者跑到界外區, 然後發球.
            int targetZ = 18; // todo 魔術數字要拉出來 ...
            if(team == ETeamKind.Self)
                targetZ = -18;

            Vector2 v = new Vector2(someone.transform.position.x, targetZ);
            float dis = Vector2.Distance(new Vector2(someone.transform.position.x, someone.transform.position.z), v);
            if(dis <= 1.7f)
            {
                // 已經跑到界外區了, 要執行發球.
                StartCoroutine(AutoTee());
            }
            else
            {
                // 要求拿球的人要跑到界外區.

                // 這行就非常不合理, 我明明沒有用戰術的任何資料來指引持球者跑到某個位置,
                // 那這樣 TacticalName 還做設定, 是一件非常不合裡的設定.
                moveData.TacticalName = data.Name;
                moveData.SetTarget(someone.transform.position.x, targetZ);
                someone.TargetPos = moveData;
            }
        }
        else if(data.Name != string.Empty && mInboundsBallOnlyOnce)
        {
            // 沒有拿到球的人. 跑企劃編輯的位置.
            tacticalActions = data.GetActions(someone.Index);

            if(tacticalActions != null)
            {
                for(int i = 0; i < tacticalActions.Length; i++)
                {
                    moveData.Clear();
                    moveData.Speedup = tacticalActions[i].Speedup;
                    moveData.Catcher = tacticalActions[i].Catcher;
                    moveData.Shooting = tacticalActions[i].Shooting;
                    if(team == ETeamKind.Self)
                        moveData.SetTarget(tacticalActions[i].X, tacticalActions[i].Z);
                    else
                        moveData.SetTarget(tacticalActions[i].X, -tacticalActions[i].Z);

                    moveData.TacticalName = data.Name;
							moveData.LookTarget = CourtMgr.Get.RealBallObj.transform;
                    someone.TargetPos = moveData;
                }

                mInboundsBallOnlyOnce = false;
            }
        }
    }

    private void inboundsHalf(PlayerBehaviour someone, TTacticalData data)
    {
        // 這段是半場的跑位.
        // todo 這段幾乎都是重複的程式碼...
        if(someone == BallOwner)
        {
            Vector2 v = new Vector2(0, -0.2f);
            float dis = Vector2.Distance(new Vector2(someone.transform.position.x, someone.transform.position.z), v);
            if(dis <= 1.5f)
            {
                if(BallOwner)
                    StartCoroutine(AutoTee());
            }
            else
            {
                moveData.TacticalName = data.Name;
                //						moveData.Target = v;
                moveData.SetTarget(v.x, v.y);
                someone.TargetPos = moveData;
            }
        }
        else if(data.Name != string.Empty)
        {
            tacticalActions = data.GetActions(someone.Index);

            if(tacticalActions != null)
            {
                for(int j = 0; j < tacticalActions.Length; j++)
                {
                    moveData.Clear();
                    moveData.Speedup = tacticalActions[j].Speedup;
                    moveData.Catcher = tacticalActions[j].Catcher;
                    moveData.Shooting = tacticalActions[j].Shooting;
                    //							moveData.Target = new Vector2(tacticalActions [j].x, tacticalActions [j].z);
                    moveData.SetTarget(tacticalActions[j].X, tacticalActions[j].Z);

                    moveData.TacticalName = data.Name;
							moveData.LookTarget = CourtMgr.Get.RealBallObj.transform;
                    someone.TargetPos = moveData;
                }
            }
        }
    }

    /// <summary>
    /// tee: 準備發球. 用在對方得分後, 換場時的發球.
    /// </summary>
    /// <returns></returns>
	IEnumerator AutoTee()
    {
        if (BallOwner)
        {
            Vector3 lookatV3 = CourtMgr.Get.ShootPoint[BallOwner.Team == ETeamKind.Self ? 1 : 0].transform.position;
			BallOwner.RotateTo(lookatV3.x, lookatV3.z);
        }
		yield return new WaitForSeconds(1);

		bool flag = false;

	    if(BallOwner)
        {
		    PlayerBehaviour catchBallPlayer;
		    if(BallOwner.Team == ETeamKind.Self && BallOwner != Joysticker)
				catchBallPlayer = Joysticker; // 強迫玩家接球.
			else
				catchBallPlayer = tryFindNearPlayer(BallOwner, 10, true);
		
			if (catchBallPlayer != null)
				flag = TryPass(catchBallPlayer, true);
			else
            {
				int ran = Random.Range(0, 2);
				int count = 0;
				for (int i = 0; i < PlayerList.Count; i++)
                {
					if (PlayerList[i].gameObject.activeInHierarchy && PlayerList[i].Team == BallOwner.Team && 
					    PlayerList[i] != BallOwner)
                    {
						if (count == ran)
                        {
							flag = TryPass(PlayerList[i], true);
							break;
						}
						
						count++;
					}
				}
			}
		}

		if (!flag) {
		    if(Situation == EGameSituation.GamerInbounds)
		    {
		        ChangeSituation(EGameSituation.GamerAttack);
                AIController.Get.ChangeState(EGameSituation.GamerAttack);
		    }
			else if(Situation == EGameSituation.NPCInbounds)
			{
			    ChangeSituation(EGameSituation.NPCAttack);
                AIController.Get.ChangeState(EGameSituation.NPCAttack);
			}
		}
	}

    [CanBeNull]
    private TPlayerDisData[] findPlayerDisData([NotNull]PlayerBehaviour player, bool isSameTeam = false, 
                                               bool angle = false)
	{
		TPlayerDisData [] disData = null;

		if(isSameTeam)
		{
			if(PlayerList.Count > 2)
				disData = new TPlayerDisData[(PlayerList.Count / 2) - 1];
		}
		else
			disData = new TPlayerDisData[PlayerList.Count / 2];

        if(disData == null)
            return null;

        for(int i = 0; i < PlayerList.Count; i++) 
        {
            if(isSameTeam)
            {
                if(PlayerList[i].Team == player.Team && PlayerList[i] != player)
                {
                    PlayerBehaviour anpc = PlayerList[i];
                    for(int j = 0; j < disData.Length; j++)
                    {
                        if(disData[j].Distance == 0)
                        {
                            if(angle)
								disData[j].Distance = Mathf.Abs(MathUtils.FindAngle(player.PlayerRefGameObject.transform, anpc.PlayerRefGameObject.transform.position));
                            else
                                disData[j].Distance = GetDis(anpc, player);
                            disData[j].Player = anpc;
                            break;
                        }
                    }
                }
            }
            else
            {
                if(PlayerList[i].Team != player.Team)
                {
                    PlayerBehaviour anpc = PlayerList[i];
                    for(int j = 0; j < disData.Length; j++)
                    {
                        if(disData[j].Distance == 0)
                        {
                            if(angle)
								disData[j].Distance = Mathf.Abs(MathUtils.FindAngle(player.PlayerRefGameObject.transform, anpc.PlayerRefGameObject.transform.position));
                            else
                                disData[j].Distance = GetDis(anpc, player);
                            disData[j].Player = anpc;
                            break;
                        }
                    }
                }
            }
        }
			
        TPlayerDisData temp = new TPlayerDisData ();
			
        for(int i = 0; i < disData.Length - 1; i ++)
        {
            for(int j = 0; j < disData.Length - 1; j++)
            {
                if(disData[j].Distance > disData[j + 1].Distance)
                {
                    temp = disData[j];
                    disData[j] = disData[j + 1];
                    disData[j + 1] = temp;
                }
            }
        }

        return disData;
	}

    private void defBlock(ref PlayerBehaviour attacker, bool isFakeShoot)
    {
		if(PlayerList.Count > 0 && !IsPassing && !IsBlocking)
        {
		    TPlayerDisData[] playerDisData = findPlayerDisData(attacker, false, true);

            if(playerDisData == null)
                return;

            for(int i = 0; i < playerDisData.Length; i++)
            {
                var blocker = playerDisData[i].Player;
                if(blocker && blocker != attacker && blocker.Team != attacker.Team && blocker.AIing && 
                    !blocker.IsSteal && !blocker.IsPush)
                {
                    float angle = MathUtils.FindAngle(attacker.transform, PlayerList[i].transform.position);
						
                    if(GetDis(attacker, blocker) <= GameConst.BlockDistance && Mathf.Abs(angle) <= 70)
                    {
                        float blockRate = blocker.Attr.BlockRate;
                        if(isFakeShoot)
                            blockRate = blocker.Attr.FaketBlockRate;

                        if(Random.Range(0, 100) < blockRate)
                        {
                            if(blocker.PlayerSkillController.DoPassiveSkill(ESkillSituation.Block0, attacker.PlayerRefGameObject.transform.position))
                            {
                                if(isFakeShoot)
                                    blocker.GameRecord.BeFake++;

                                break;
                            }
                        }
                    }
                }
            }
        }
	}

    private void doLookAtBall(PlayerBehaviour someone)
    {
        if(!someone.IsBlock  && someone.AIing)
            someone.RotateTo(CourtMgr.Get.RealBallObj.transform.position.x, 
                             CourtMgr.Get.RealBallObj.transform.position.z);
    }

    /// <summary>
    /// 是否球員是該隊最接近球的球員?
    /// </summary>
    /// <param name="someone"></param>
    /// <returns></returns>
    private bool isNearestBall([NotNull]PlayerBehaviour someone)
    {
//        return findNearBallPlayer(someone.Team) == someone;

        return someone.GetComponent<PlayerAI>().isNearestBall();
    }

    public void NearestBallPlayerDoPickBall([NotNull]PlayerBehaviour someone)
    {
        if (isNearestBall(someone))
            MoveToBall(someone);
        else
            doLookAtBall(someone);
    }

    /// <summary>
    /// 叫球員撿球(不見得真的會去撿球).
    /// </summary>
    /// <param name="someone"></param>
    public void MoveToBall([NotNull] PlayerBehaviour someone)
	{
	    if(someone.CanMove && someone.CantMoveTimer.IsOff())
	    {
            // 球員移動到球的位置.
	        moveData.Clear();
	        moveData.FollowTarget = CourtMgr.Get.RealBallObj.transform;
	        someone.TargetPos = moveData;
	    }
    }
	
	public float GetDis(PlayerBehaviour player1, PlayerBehaviour player2)
    {
        if (player1 != null && player2 != null && player1 != player2)
        {
			Vector3 v1 = player1.PlayerRefGameObject.transform.position;
			Vector3 v2 = player2.PlayerRefGameObject.transform.position;
            v1.y = v2.y;
            return Vector3.Distance(v1, v2);
        } else
            return -1;
    }

	public float GetDis(PlayerBehaviour someone, Vector3 target)
    {
        if(someone != null && target != Vector3.zero)
			return Vector3.Distance(someone.PlayerRefGameObject.transform.position, target);

        return -1;
    }

	public float GetDis(PlayerBehaviour player1, Vector2 target)
    {
        if (player1 != null && target != Vector2.zero)
        {
            Vector3 v1 = new Vector3(target.x, 0, target.y);
			Vector3 v2 = player1.PlayerRefGameObject.transform.position;
            v1.y = v2.y;
            return Vector3.Distance(v1, v2);
        }

        return -1;
    }	

	public float GetDis(Vector2 player1, Vector2 target)
	{
		if(target != Vector2.zero)
		{
			Vector3 v1 = new Vector3(target.x, 0, target.y);
			Vector3 v2 = new Vector3(player1.x, 0, player1.y);
			return Vector3.Distance(v1, v2);
		}

        return -1;
	}
	
	public void SetBallOwnerNull()
	{
		if (BallOwner != null) {
			BallOwner.IsBallOwner = false;
			BallOwner = null;
			CourtMgr.Get.RealBallCompoment.SetBallOwnerNull();
		}
	}

	private int getPlayerIndex(int team, EPlayerPostion index)
    {
		for (int i = 0; i < PlayerList.Count; i++)
			if (PlayerList[i].PlayerRefGameObject.activeInHierarchy && PlayerList[i].Team.GetHashCode() == team && PlayerList[i].Index == index)
				return i;

		return -1;
	}

	//For Tutorial
	public bool SetBall(int team, EPlayerPostion index)
    {
		int p = getPlayerIndex(team, index);
		if(p > -1) 
			return SetBall(PlayerList[p]);

        return false;
	}

    /// <summary>
    /// 這應該是設定參數的 PlayerBehaviour 是持球者.
    /// </summary>
    /// <param name="newBallOwner"> null 表示遊戲沒有持球者.(這樣設計不好, 應該是直接設定持球者為空才對) </param>
    /// <returns> true: newBallOwner 變成持球者, false: 沒有持球者. </returns>
    public bool SetBall(PlayerBehaviour newBallOwner = null)
    {
		bool result = false;
		IsPassing = false;
		if(newBallOwner != null && Situation != EGameSituation.End)
        {
			IsReboundTime = false;
			CourtMgr.Get.IsBallOffensive = false;
			if (!newBallOwner.IsAlleyoopState) 
			{
				if(Situation == EGameSituation.GamerAttack || Situation == EGameSituation.NPCAttack)
					BallState = EBallState.CanSteal;	
				else
					BallState = EBallState.None;
			}

            if(BallOwner != null)
            {
                BallOwner.IsBallOwner = false;
                BallOwner.ResetFlag(false);
            }

            changeSituationForSetBall(newBallOwner);

            // 這部份才真正的是將 newBallOwner 設定為持球者.
            BallOwner = newBallOwner;
			BallOwner.CantMoveTimer.Clear();
			BallOwner.IsBallOwner = true;
			result = true;
			Shooter = null;

			for(int i = 0 ; i < PlayerList.Count; i++)
				PlayerList[i].ClearAutoFollowTime();

			if(BallOwner && BallOwner.DefPlayer != null)
				BallOwner.DefPlayer.SetAutoFollowTime();

			UIGame.Get.ChangeControl(newBallOwner.Team == ETeamKind.Self);
			UIGame.Get.SetPassButton();
			CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.HoldBall, newBallOwner);

			AudioMgr.Get.PlaySound(SoundType.SD_Catch);
			newBallOwner.CantMoveTimer.Clear();
			newBallOwner.IsFirstDribble = true;
            CourtMgr.Get.RealBallCompoment.Trigger.IsAutoRotate = false;

			for(int i = 0; i < PlayerList.Count; i++)
            {
				if(PlayerList [i].Team != newBallOwner.Team)
                {
					PlayerList [i].ResetMove();
					break;
				}
			} 
				 
			if(newBallOwner.IsIdle || newBallOwner.IsDef)
				newBallOwner.AniState(EPlayerState.Dribble0);
			else if(newBallOwner.IsRun)
				newBallOwner.AniState(EPlayerState.Dribble1);
			else
				newBallOwner.AniState(EPlayerState.HoldBall);

			if (GamePlayTutorial.Visible)
				GamePlayTutorial.Get.CheckSetBallEvent(newBallOwner);
    	}
        else
        {
			SetBallOwnerNull();
		}

		return result;
    }

    /// <summary>
    /// <para> 遊戲進行中時(GamerAttack or NPCAttack), 別的隊伍拿到球時, 要切換狀態. </para>
    /// <para>  </para>
    /// </summary>
    /// <param name="newBallOwner"></param>
    private void changeSituationForSetBall(PlayerBehaviour newBallOwner)
    {
        switch(Situation)
        {
            case EGameSituation.JumpBall:
                if(newBallOwner.Team == ETeamKind.Self)
                {
                    ChangeSituation(EGameSituation.GamerAttack);
                    AIController.Get.ChangeState(EGameSituation.GamerAttack);
                }
                else if(newBallOwner.Team == ETeamKind.Npc)
                {
                    ChangeSituation(EGameSituation.NPCAttack);
                    AIController.Get.ChangeState(EGameSituation.NPCAttack);
                }
                break;
            case EGameSituation.GamerAttack: // 遊戲進行中, 別的隊伍拿到球時, 要切換狀態.
                if(newBallOwner.Team == ETeamKind.Npc)
                {
                    ChangeSituation(EGameSituation.NPCAttack);
                    AIController.Get.ChangeState(EGameSituation.NPCAttack);
                }
                break;
            
            case EGameSituation.NPCAttack: // 遊戲進行中, 別的隊伍拿到球時, 要切換狀態.
                if(newBallOwner.Team == ETeamKind.Self)
                {
                    ChangeSituation(EGameSituation.GamerAttack);
                    AIController.Get.ChangeState(EGameSituation.GamerAttack);
                }
                break;
            case EGameSituation.GamerPickBall:
                ChangeSituation(EGameSituation.GamerInbounds);
                AIController.Get.ChangeState(EGameSituation.GamerInbounds);
                break;
            case EGameSituation.NPCPickBall:
                ChangeSituation(EGameSituation.NPCInbounds);
                AIController.Get.ChangeState(EGameSituation.NPCInbounds);
                break;
            case EGameSituation.GamerInbounds:
                ChangeSituation(EGameSituation.GamerAttack);
                AIController.Get.ChangeState(EGameSituation.GamerAttack);
                break;
            case EGameSituation.NPCInbounds:
                ChangeSituation(EGameSituation.NPCAttack);
                AIController.Get.ChangeState(EGameSituation.NPCAttack);
                break;
        }

        /*
        // 下面這一長串的 if else 是在做 GameController 的狀態切換.
        if(oldBallOwner != null)
        {
            if(oldBallOwner.Team != newBallOwner.Team)
            {
                // 別的隊伍拿到球(攻守轉換)
//                if(Situation == EGameSituation.GamerAttack)
//                {
//                    ChangeSituation(EGameSituation.NPCAttack);
//                    AIController.Ins.ChangeState(EGameSituation.NPCAttack);
//                }
//                else if(Situation == EGameSituation.NPCAttack)
//                {
//                    ChangeSituation(EGameSituation.GamerAttack);
//                    AIController.Ins.ChangeState(EGameSituation.GamerAttack);
//                }
            }
            else
            {
                // 同隊的人拿到球.
                if(Situation == EGameSituation.GamerInbounds)
                {
//                    ChangeSituation(EGameSituation.GamerAttack);
//                    AIController.Ins.ChangeState(EGameSituation.GamerAttack);
                }
                else if(Situation == EGameSituation.NPCInbounds)
                {
//                    ChangeSituation(EGameSituation.NPCAttack);
//                    AIController.Ins.ChangeState(EGameSituation.NPCAttack);
                }
                else
                    oldBallOwner.ResetFlag(false);
            }
        }
        else
        {
            // 目前沒有持球者.
            if(Situation == EGameSituation.GamerPickBall)
            {
//                ChangeSituation(EGameSituation.GamerInbounds);
//                AIController.Ins.ChangeState(EGameSituation.GamerInbounds);
            }
            else if(Situation == EGameSituation.NPCPickBall)
            {
//                ChangeSituation(EGameSituation.NPCInbounds);
//                AIController.Ins.ChangeState(EGameSituation.NPCInbounds);
            }
            else if(Situation == EGameSituation.GamerInbounds)
            {
//                ChangeSituation(EGameSituation.GamerAttack);
//                AIController.Ins.ChangeState(EGameSituation.GamerAttack);
            }
            else if(Situation == EGameSituation.NPCInbounds)
            {
//                ChangeSituation(EGameSituation.NPCAttack);
//                AIController.Ins.ChangeState(EGameSituation.NPCAttack);
            }
            else
            {
                // 我認為這是一個不好的判斷條件, 這是判斷是否為得分後的攻守轉換.
                if(LobbyStart.Get.CourtMode == ECourtMode.Full //||
             //      (newBallOwner.Team == ETeamKind.Self && Situation == EGameSituation.GamerAttack) ||
             //      (newBallOwner.Team == ETeamKind.Npc && Situation == EGameSituation.NPCAttack)
                   )
                {
//                    if(Situation == EGameSituation.GamerInbounds || Situation == EGameSituation.NPCInbounds)
//                        setMoveFrontCourtTactical(newBallOwner);

//                    if(newBallOwner.Team == ETeamKind.Self)
//                    {
//                        ChangeSituation(EGameSituation.GamerAttack);
//                        AIController.Ins.ChangeState(EGameSituation.GamerAttack);
//                    }
//                    else
//                    {
//                        ChangeSituation(EGameSituation.NPCAttack);
//                        AIController.Ins.ChangeState(EGameSituation.NPCAttack);
//                    }
                }
//                else
//                {
//                    // 半場狀態切換...
//                    if(newBallOwner.Team == ETeamKind.Self)
//                    {
//                        ChangeSituation(EGameSituation.GamerInbounds);
//                        AIController.Get.ChangeState(EGameSituation.GamerInbounds);
//                    }
//                    else
//                    {
//                        ChangeSituation(EGameSituation.NPCInbounds);
//                        AIController.Get.ChangeState(EGameSituation.NPCInbounds);
//                    }
//                }
            }
        }*/
    }

    public PlayerBehaviour FindNearNpc(){
		PlayerBehaviour p = null;
		float dis = 0;
		for (int i=0; i<PlayerList.Count; i++) {
			if (PlayerList [i].Team == ETeamKind.Npc){
				if(p == null){
					p = PlayerList[i];
					dis = Vector3.Distance(Joysticker.PlayerRefGameObject.transform.position, PlayerList[i].PlayerRefGameObject.transform.position);
				} else {
					float temp = Vector3.Distance(Joysticker.PlayerRefGameObject.transform.position, PlayerList[i].PlayerRefGameObject.transform.position);
					if(dis > temp) {
						dis = temp;
						p = PlayerList[i];
					}
				}
			}
		}
		return p;
	}
	
	public void BallOnFloor()
    {
        if (BallOwner == null)
        {
            CourtMgr.Get.ResetBasketEntra();
            CourtMgr.Get.IsBallOffensive = false;

            if(Situation == EGameSituation.GamerAttack || Situation == EGameSituation.NPCAttack) 
                BallState = EBallState.CanSteal;
            else 
                BallState = EBallState.None;

            Shooter = null;
        }

        // todo 這段是預防遊戲卡住, 但因為無法重現, 找不到原因, 所以只能根據別人的描述來寫預防措施.
        // 情況: 球員灌籃得分, 沒有人撿球.
	    if(Situation == EGameSituation.NPCPickBall || Situation == EGameSituation.GamerPickBall)
	    {
	        if(PickBallPlayer && 
               PickBallPlayer.crtState != EPlayerState.Idle &&
               PickBallPlayer.crtState != EPlayerState.Run0 &&
               PickBallPlayer.crtState != EPlayerState.Run1 &&
               PickBallPlayer.crtState != EPlayerState.Run2 &&
               PickBallPlayer.crtState != EPlayerState.Pick0 &&
               PickBallPlayer.crtState != EPlayerState.Pick1 &&
               PickBallPlayer.crtState != EPlayerState.Pick2)
	        {
	            PickBallPlayer.AniState(EPlayerState.Idle);
	        }
	        PickBallPlayer = null;
	    }

        if(LobbyStart.Get.TestMode == EGameTest.Shoot)
        {
            SetBall(Joysticker);    
            Joysticker.AniState(EPlayerState.HoldBall);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="dir"> -1: 沒碰到 trigger, 0: TriggerTop, 1:TriggerFR, 3:TriggerBR, 5:TriggerFinger, 6:TriggerSteal. </param>
    /// <returns> true: 抄球成功, false: 抄球失敗. </returns>
    public bool PassingStealBall(PlayerBehaviour player, int dir)
	{
		if(player.IsDefence && (player.IsMoving || player.IsRun || player.IsDef) && (Situation == EGameSituation.GamerAttack || Situation == EGameSituation.NPCAttack) && Passer && passingStealBallTime == 0)
		{
			if(Catcher == player || Catcher.Team == player.Team)
				return false;
            
			if(CourtMgr.Get.RealBallCompoment.RealBallState == EPlayerState.Pass0 || 
				CourtMgr.Get.RealBallCompoment.RealBallState == EPlayerState.Pass2 ||
				CourtMgr.Get.RealBallCompoment.RealBallState == EPlayerState.Pass1 || 
				CourtMgr.Get.RealBallCompoment.RealBallState == EPlayerState.Pass3)
			{
                if (dir == 5 || dir == 7 || dir == 6)
                {
					int rate = Random.Range(0, 100);
					passingStealBallTime = Time.time + 2;

                    if (BallOwner == null && (rate > Passer.Attr.PassRate) && !player.IsPush)
                    {
                        if (dir == 6)
                        {
                            player.AniState(EPlayerState.Intercept1, CourtMgr.Get.RealBallObj.transform.position);
                            return false;
                        }
                        else if (dir == 5)
                        {
							if ( player.AniState(EPlayerState.Intercept1, CourtMgr.Get.RealBallObj.transform.position))
                            {
                                if (BallTrigger.PassKind == 0 || BallTrigger.PassKind == 2)
                                    CourtMgr.Get.RealBallObj.transform.DOKill();

								if (Passer){
									ShowWord(EShowWordType.Turnover, Passer.Team.GetHashCode(), Passer.ShowWord);
									Passer.GameRecord.BeIntercept++;
								}
								player.GameRecord.Intercept++;

                                if (SetBall(player))
								{
                                    player.AniState(EPlayerState.HoldBall);
                                }

                                IsPassing = false;
                                return true;
                            }
                        }
                        else if(dir == 7)
                        {
                            player.AniState(EPlayerState.Intercept0);

                            if(BallTrigger.PassKind == 0 || BallTrigger.PassKind == 2)
                                CourtMgr.Get.RealBallObj.transform.DOKill();
							
							if (Passer){
								ShowWord(EShowWordType.Turnover, Passer.Team.GetHashCode(), Passer.ShowWord);
								Passer.GameRecord.BeIntercept++;
							}
							player.GameRecord.Intercept++;

                            if(SetBall(player))
							{
                                player.AniState(EPlayerState.HoldBall);
                            }

                            IsPassing = false;
                            return true;
                        }
                    }
                }
			}
		}

		return false;
	}

	public void BallTouchPlayer(int index, int dir, bool isEnter)
    {
	    if(index < 0 || index >= PlayerList.Count)
	    {
	        Debug.LogWarningFormat("Index({0}) out of range.", index);
            return;
	    }

//        if(index >= 0 && index < PlayerList.Count)
        BallTouchPlayer(PlayerList[index], dir, isEnter);
	}

    public void BallTouchPlayer(PlayerBehaviour player, int dir, bool isEnter)
    {
		if (Situation == EGameSituation.None || 
			BallOwner || 
		    !player.IsCanCatchBall || 
		    dir == 6)
            return;

		if (Catcher) {
			if(Situation == EGameSituation.GamerPickBall || Situation == EGameSituation.NPCPickBall)
				IsPassing = false;
		}			

//		if(Situation == EGameSituation.APickBallAfterScore && player == Joysticker)
//			return;

        // Special Action 要避免觸發任何的狀態切換, 狀態的切換應該要發生在 SpecialActionState.
        // Refactor 完畢後, 這就可以刪除了.(trigger 換成是送 message 的方式)
        if (Situation == EGameSituation.SpecialAction)
            return;
       
		switch (dir)
        {
		case 0: //top ,rebound
			if(Situation == EGameSituation.JumpBall && isEnter)
            {
//                GameMsgDispatcher.Ins.SendMesssage(EGameMsg.PlayerTouchBallWhenJumpBall, player);
			}
            else if((isEnter || LobbyStart.Get.TestMode == EGameTest.Rebound) &&
				   player != BallOwner &&
				   CourtMgr.Get.RealBallObj.transform.position.y >= 3 &&
				   (Situation == EGameSituation.GamerAttack || Situation == EGameSituation.NPCAttack))
            {

					if (LobbyStart.Get.TestMode == EGameTest.Rebound)
						Rebound(player);
//					else if(CourtMgr.Get.RealBallCompoment.RealBallState ==  EPlayerState.Steal0 || 
//						CourtMgr.Get.RealBallCompoment.RealBallState ==  EPlayerState.Rebound0)
					else if(GameController.Get.BallState == EBallState.CanRebound)
                    {
					    if(Random.Range(0, 100) < player.Attr.ReboundRate) 
					        Rebound(player);
					}
			}
            break;
		case 5: //finger
			if(Situation == EGameSituation.JumpBall && isEnter)
			{
//				GameMsgDispatcher.Ins.SendMesssage(EGameMsg.PlayerTouchBallWhenJumpBall, player);
			}
			else if (isEnter && !player.IsBallOwner && player.IsRebound && !IsTipin) {
				if (LobbyStart.Get.TestMode == EGameTest.Rebound || Situation == EGameSituation.GamerAttack || Situation == EGameSituation.NPCAttack || LobbyStart.Get.TestMode == EGameTest.Block) {
					if (SetBall(player)) {
						player.GameRecord.Rebound++;
						player.SetAnger(GameConst.AddAnger_Rebound, player.PlayerRefGameObject);

						if (player == BallOwner && inTipinDistance(player)) {
//							CoolDownPass = Time.time + GameConst.CoolDownPassTime;
                            PassCD.StartAgain();
							if (player == Joysticker)
								OnDoubleClickMoment(player, EPlayerState.Rebound0);
							else
							if (Random.Range(0, 100) < player.Attr.TipInRate)
								DoShoot();
							else
							if (player.Team == Joysticker.Team)
								OnDoubleClickMoment(player, EPlayerState.Rebound0);
						}
					}
				}
			}

			break;
		default :
			if(!player.IsRebound && (player.IsCatcher || player.CanMove))
            {
                bool canSetball = false; // 攻守交換時, 只有進攻方才可以撿球,
                if(Situation == EGameSituation.GamerPickBall)
                {
					if(player.Team == ETeamKind.Self)
						canSetball = true;
				}
                else if(Situation == EGameSituation.NPCPickBall)
				{
					if(player.Team == ETeamKind.Npc)
						canSetball = true;
				}
                else
					canSetball = true;
				
				if(canSetball && !IsPickBall)
				{
					if((Situation == EGameSituation.GamerPickBall || Situation == EGameSituation.NPCPickBall) &&
                        player == PickBallPlayer)
                    {
						if(CourtMgr.Get.RealBallObj.transform.position.y > 1.7f)
							player.AniState(EPlayerState.CatchFlat, CourtMgr.Get.RealBallObj.transform.position);
						else
							player.AniState(EPlayerState.Pick0, CourtMgr.Get.RealBallObj.transform.position);
					}
					else if(!IsPassing || isEnter)
					{
						if(SetBall(player)) {
							if(player.AIing || player.IsIdle)
								player.AniState(EPlayerState.Dribble0);
							else if(player.IsRun || player.IsDribble)
								player.AniState(EPlayerState.Dribble1);
							else
								player.AniState(EPlayerState.HoldBall);
						}
					}
                }
            }

            break;
        }
    }
    
    public bool OnPickUpBall(PlayerBehaviour player)
    {
        if (player && BallOwner == null) {
			SetBall (player);
            
            return true;
        }
        
        return false;
	}

    public void PlayerTouchPlayer(PlayerBehaviour player1, PlayerBehaviour player2, int dir)
    {
        switch (dir)
        {
            case 0: //top
                break;
            case 1: //FR
			case 2:
				if(player1.PlayerSkillController.IsHaveMoveDodge)
					player1.PlayerSkillController.DoPassiveSkill(ESkillSituation.MoveDodge);

				if(player1.IsSkillPushThrough)
					player2.PlayerSkillController.DoPassiveSkill(ESkillSituation.Fall1, player1.transform.position);
			
				if(player2.IsSkillPushThrough)
					player1.PlayerSkillController.DoPassiveSkill(ESkillSituation.Fall1, player2.transform.position);
				
				if(!player2.IsDefence && player1.IsDefence)
				{
					if(Mathf.Abs(MathUtils.FindAngle(player2.PlayerRefGameObject.transform, player1.PlayerRefGameObject.transform.position)) <= GameConst.SlowDownAngle)
						player2.SetSlowDown(GameConst.SlowDownTime);
				}
                break;
        }
    }

//    public void DefRangeTouch(PlayerBehaviour player1, PlayerBehaviour player2)
//    {
//        if(player1.IsDefence)
//        {
//            MoveDefPlayer(player1.DefPlayer);     
//        }
//    }

	public void DefRangeTouchBall(PlayerBehaviour player)
	{
		if(player.PlayerSkillController.IsHavePickBall2) {
			if (BallOwner == null && Shooter == null && Catcher == null && (Situation == EGameSituation.GamerAttack || Situation == EGameSituation.NPCAttack)) {
				player.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pick0, CourtMgr.Get.RealBallObj.transform.position);
			}
		}
	}

	private bool canPassToAlleyoop(EPlayerState state) {
		if (state == EPlayerState.Idle ||
		    state == EPlayerState.HoldBall ||
		    state == EPlayerState.Dribble0 ||
		    state == EPlayerState.Dribble1)
			return true;
		else
			return false;
    }
    
	public void PlayerEnterPaint(int team, GameObject obj) {
		if (BallOwner && canPassToAlleyoop(BallOwner.crtState) &&
		   (LobbyStart.Get.TestMode == EGameTest.Alleyoop || 
		 	Situation == EGameSituation.GamerAttack || Situation == EGameSituation.NPCAttack)) {
			bool flag = true;
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList[i].crtState == EPlayerState.Alleyoop) {
					flag = false;
					break;
				}

			if (flag) {
				PlayerBehaviour player = obj.GetComponent<PlayerBehaviour>();
				if (player && player.Team.GetHashCode() == team) {
					if (player != BallOwner && player.Team == BallOwner.Team) {
						if (Random.Range(0, 100) < player.Attr.AlleyOopRate || LobbyStart.Get.TestMode == EGameTest.Alleyoop) {
							player.AniState(EPlayerState.Alleyoop, CourtMgr.Get.ShootPoint [team].transform.position);

							if ((BallOwner != Joysticker || (BallOwner == Joysticker && Joysticker.AIing)) && Random.Range(0, 100) < BallOwner.Attr.AlleyOopPassRate) {
								if (BallOwner.PlayerSkillController.DoPassiveSkill(ESkillSituation.Pass0, player.PlayerRefGameObject.transform.position))
									Catcher = player;
							} else
								UIGame.Get.ShowAlleyoop(true, player.Index.GetHashCode());

							player.GameRecord.AlleyoopLaunch++;
						}
					}
				}
			}
		}
	}

	IEnumerator playFinish() {
		isEndShowScene = false;
		yield return new WaitForSeconds(2.5f);
	    IsStart = false;
		setEndShowScene();
		isEndShowScene = true;
		if(LobbyStart.Get.IsAutoReplay){
			UIGamePause.Get.OnAgain();
			Invoke("JumpBallForReplay", 2);
            Time.timeScale = LobbyStart.Get.GameSpeed;
			UIGameLoseResult.UIShow(false);
		}
	}

	public void JumpBallForReplay () {
		UIGame.Get.UIState(EUISituation.Start);
	}

	private void gameResult()
	{
		UITutorial.UIShow(false);
		UIInGameMission.UIShow(false);
		if(Situation != EGameSituation.End) {
			ChangeSituation(EGameSituation.End);
			AIController.Get.ChangeState(EGameSituation.End);
		}
	}

	private void pveEnd(int stageID)
	{
		if(LobbyStart.Get.ConnectToServer) {
			WWWForm form = new WWWForm();
			form.AddField("StageID", stageID);

			if(!StageData.IsTutorial)
			{
				UIGameResult.UIShow(true);
				UIGameResult.Get.SetGameRecord(ref GameRecord);
			}
			else {
                if (GameData.Team.Player.Lv == 0 && StageData.IsTutorial) {
    				form.AddField("Cause", 1);
                    form.AddField("Company", GameData.Company);
                    form.AddField("GameTime", GameRecord.GamePlayTime.ToString());
    				SendHttp.Get.Command(URLConst.AddStageTutorial, null, form, false);
                }

				UIGameResult.UIShow(true);
				UIGameResult.Get.SetGameRecord(ref GameRecord);
			}
		} else {
			if(!LobbyStart.Get.IsAutoReplay){
				UIGameResult.UIShow(true);
				UIGameResult.Get.SetGameRecord(ref GameRecord);
			}
		}
	}
	//GM Tools
	public void GMGameResult (bool isSelfWin) {
		GameTime = 0;
		if(isSelfWin) {
			UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] = 100;
			Joysticker.GameRecord.FGIn = 10;
			Joysticker.GameRecord.FG3In = 10;
			Joysticker.GameRecord.Dunk = 10;
			Joysticker.GameRecord.Push = 10;
			Joysticker.GameRecord.Steal = 10;
			Joysticker.GameRecord.Block = 10;
			Joysticker.GameRecord.Elbow = 10;
			Joysticker.GameRecord.Rebound = 10;
		} else {
			UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()] = 100;
			Joysticker.GameRecord.FGIn = 0;
			Joysticker.GameRecord.FG3In = 0;
			Joysticker.GameRecord.Dunk = 0;
			Joysticker.GameRecord.Push = 0;
			Joysticker.GameRecord.Steal = 0;
			Joysticker.GameRecord.Block = 0;
			Joysticker.GameRecord.Elbow = 0;
		}
		gameResult();
	}

	private void setEndShowScene () {
		UITutorial.UIShow(false);
		SetPlayerAI(false);
		AIController.Get.ChangeState(EGameSituation.End);

        SetBallOwnerNull();
        CourtMgr.Get.RealBallCompoment.Trigger.Reset();

		//Player
        int num = Mathf.Min(PlayerList.Count, CourtMgr.Get.EndPlayerPosition.Length);
		for (int i=0; i< num; i++) {
			if(!LobbyStart.Get.IsAutoReplay)
           		PlayerList[i].DefPlayer = null;
            PlayerList[i].Reset();
            PlayerList[i].ResetMove();
            PlayerList[i].AniState(EPlayerState.Idle);
			PlayerList[i].transform.position = CourtMgr.Get.EndPlayerPosition[i].position;
			PlayerList[i].transform.rotation = CourtMgr.Get.EndPlayerPosition[i].rotation;
		}

		if (IsWinner) {
            UILoading.StageID = GameData.StageID;
            AudioMgr.Get.PlaySound (SoundType.SD_ResultWin);
			SelfWin ++;
			for (int i = 0; i < PlayerList.Count; i++) {
				if (PlayerList [i].Team == ETeamKind.Self)
					PlayerList [i].AniState(EPlayerState.Ending0);
				else
					PlayerList [i].AniState(EPlayerState.Ending10);
			}
			if(!GameData.IsPVP)
				pveEnd(StageData.ID);
			else {
				if(beforeTeam.PVPLv != 0 && afterTeam.PVPLv != 0) {
					UIGameResult.UIShow(true);
					UIGameResult.Get.SetGameRecord(ref GameRecord);
					UIGameResult.Get.SetPVPData(beforeTeam, afterTeam);
				}
			}
		}
		else
		{
            UILoading.StageID = -1;
            AudioMgr.Get.PlaySound (SoundType.SD_ResultLose);
			NpcWin ++;
			for (int i = 0; i < PlayerList.Count; i++) {
				if (PlayerList [i].Team == ETeamKind.Self)
					PlayerList [i].AniState (EPlayerState.Ending10);
				else
					PlayerList [i].AniState (EPlayerState.Ending0);
			}
			UIGameLoseResult.UIShow(true);
			if(!GameData.IsPVP) 
				UIGameLoseResult.Get.Init();
			else 
				if(beforeTeam.PVPLv != 0 && afterTeam.PVPLv != 0) 
					UIGameLoseResult.Get.SetPVPData(beforeTeam, afterTeam);
		}
		SendGameRecord();
		CameraMgr.Get.SetEndShowSituation();
	}

	//投進的buff要從AI呼叫PlayerList[i].PlayerSkillController.DoPassiveSkill(ESkillSituation.ShowOwnIn);
	public void ShowShootSate(bool isIn, int team)
	{
		if (LobbyStart.Get.CourtMode == ECourtMode.Half && Shooter)
			team = Shooter.Team.GetHashCode();

		for (int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i].Team.GetHashCode() == team)
			{
				if(PlayerList[i] != PickBallPlayer && !PlayerList[i].IsDunk){
//					if(isIn)
//						PlayerList[i].PlayerSkillController.DoPassiveSkill(ESkillSituation.ShowOwnIn);
//					else
//                    {
					    if(!isIn && PlayerList[i].crtState == EPlayerState.Idle && 
					       GetDis(PlayerList[i], new Vector2(CourtMgr.Get.ShootPoint[PlayerList[i].Team.GetHashCode()].transform.position.x,
					              CourtMgr.Get.ShootPoint[PlayerList[i].Team.GetHashCode()].transform.position.z)) > 11)
							PlayerList[i].PlayerSkillController.DoPassiveSkill(ESkillSituation.ShowOwnOut);
//					}
				}
			}
	}

	private bool checkStageReasonable ()
    {
		if(StageTable.Ins.HasByID(StageData.ID))
        {
			if(StageData.BitNum[0] == 0 && StageData.BitNum[2] == 0 && StageData.BitNum[3] == 0 && 
			   (StageData.BitNum[1] == 2 || StageData.BitNum[1] == 3))
				return false;
			else 
				return true;
		} else 
			return false;
	}


	public bool IsTimePass() {
		if (LobbyStart.Get.TestMode == EGameTest.None && Situation != EGameSituation.End && IsStart && GameTime > 0) 
			return MissionChecker.Get.IsTimePass(ref GameTime);

		if(GameTime <= 0) {
			GameTime -= Time.deltaTime;
			if(GameTime < -2)
				CourtMgr.Get.IsBallOffensive = false;
		}
			
		return false;
	}
	public void CheckConditionText () {MissionChecker.Get.CheckConditionText(StageData.ID);}
	public bool IsScorePass(int team) {return MissionChecker.Get.IsScorePass(team);}
	public bool IsConditionPass  {get {return MissionChecker.Get.IsConditionPass;}}
	public bool IsWinner {get {return MissionChecker.Get.IsWinner(GameTime);}}

	public bool IsGameFinish (){
		CheckConditionText ();
		UIInGameMission.Get.CheckMisstion();
		bool flag = false;

		if(StageData.HintBit[1] == 3) 
		if (MissionChecker.Get.IsScoreFinish)
				flag = true;
		
		if (StageData.HintBit[0] == 0 || StageData.HintBit[0] == 1 ) 
			if (MissionChecker.Get.IsScoreFinish)
				if(IsConditionPass) 
					flag = true;

		if(flag)
			gameResult();

		return flag;
	}
    
    public void PlusScore(int team, bool isSkill, bool isChangeSituation)
    {
        if (LobbyStart.Get.CourtMode == ECourtMode.Half && Shooter != null)
			team = Shooter.Team.GetHashCode();

		BallState = EBallState.None;	
		CourtMgr.Get.IsRealBallActive = false;

		int score = 2;
		if (ShootDistance >= GameConst.Point3Distance) {
			score = 3;
			if(Shooter != null && !Shooter.IsDunk)
				ShowWord(EShowWordType.NiceShot, team);
		}

		if (LobbyStart.Get.TestMode == EGameTest.Skill)
			UIGame.Get.PlusScore(team, score);
		else
		if (IsStart && LobbyStart.Get.TestMode == EGameTest.None) {
			if (Shooter) {
				if (score == 3){
					Shooter.GameRecord.FG3In++;
					ShowWord(EShowWordType.GetThree, team);
				} else {
					Shooter.GameRecord.FGIn++;
					ShowWord(EShowWordType.GetTwo, team);
				}

				if (Shooter.crtState == EPlayerState.TipIn) {
					Shooter.GameRecord.Tipin++;
					if(Shooter != null)
						ShowWord(EShowWordType.TipShot, 0, Shooter.ShowWord);
				}

				if (IsShooting)
					Shooter.GameRecord.ShotError--;

				if (Assistant && Assistant.Team == Shooter.Team && Shooter.DribbleTime <= 2) {
                    Assistant.GameRecord.Assist++;
					ShowWord(EShowWordType.Assistant, team, Assistant.ShowWord);
				}
			}
            
			AudioMgr.Get.PlaySound(SoundType.SD_ShootNormal);
			UIGame.Get.PlusScore(team, score);
			CheckConditionText();

			if(isChangeSituation)
			{
				if(LobbyStart.Get.IsDebugAnimation) {
					Debug.LogWarning ("UIGame.Get.Scores [0] : " + UIGame.Get.Scores [0]);
					Debug.LogWarning ("UIGame.Get.MaxScores [0] : " + UIGame.Get.MaxScores [0]);
					Debug.LogWarning ("UIGame.Get.Scores [1] : " + UIGame.Get.Scores [1]);
					Debug.LogWarning ("UIGame.Get.MaxScores [1] : " + UIGame.Get.MaxScores [1]);
				}
				if(!IsGameFinish ()) {
					if(team == ETeamKind.Self.GetHashCode())
					{
						ChangeSituation(EGameSituation.SpecialAction);
						AIController.Get.ChangeState(EGameSituation.SpecialAction, EGameSituation.NPCPickBall);
					}
					else
					{
						ChangeSituation(EGameSituation.SpecialAction);
						AIController.Get.ChangeState(EGameSituation.SpecialAction, EGameSituation.GamerPickBall);
					}
					
					if (!isSkill && Shooter)
						Shooter.SetAnger(GameConst.AddAnger_PlusScore, CourtMgr.Get.ShootPoint[0]);
				}
			}
		}
		if(isChangeSituation)
			Shooter = null;

		IsPassing = false;
		ShootDistance = 0;

		if(LobbyStart.Get.IsDebugAnimation) {
			if(shootSwishTimes != shootScoreSwishTimes)
				Debug.LogWarning("shootSwishTimes != shootScoreSwishTimes");
			if(shootTimes != shootScoreTimes)
				Debug.LogWarning("shootTimes != shootScoreTimes");
		}
		if (LobbyStart.Get.TestMode == EGameTest.AttackA) {
			SetBall(Joysticker);
		}
    }

//    [CanBeNull]
//	private PlayerBehaviour findTeammate(PlayerBehaviour player, float dis, float angle)
//    {
//        for(int i = 0; i < PlayerList.Count; i++)
//        {
//			PlayerBehaviour someone = PlayerList[i];
//			if(someone.gameObject.activeSelf && 
//               someone != player && someone.Team == player.Team && 
//			   GetDis(player, someone) <= dis && HasDefPlayer(someone, 1.5f, 40) == 0)
//            {
//				float betweenAngle = MathUtils.FindAngle(player.transform, someone.transform.position);
//	            
//                if(0 <= betweenAngle && betweenAngle <= angle)
//					return someone;
//				if(-angle <= betweenAngle && betweenAngle <= 0)
//					return someone;
//	        }
//        }
//        
//        return null;
//    }

    /// <summary>
    /// 某位球員在某個距離和角度內, 是否有防守球員?
    /// </summary>
    /// <param name="player"></param>
    /// <param name="dis"></param>
    /// <param name="angle"></param>
    /// <returns> 0: 找不到防守球員; 1: 有找到, 防守球員在前方; 2: 有找到, 防守球員在後方. </returns>
	public int HasDefPlayer(PlayerBehaviour player, float dis, float angle)
    {
        if (player == null)
            return 0;

        int result = 0;

        for(int i = 0; i < PlayerList.Count; i++)
        {
			if(PlayerList[i].PlayerRefGameObject.activeInHierarchy && PlayerList[i].Team != player.Team)
            {
	            PlayerBehaviour targetNpc = PlayerList[i];
				float realAngle = MathUtils.FindAngle(player.PlayerRefGameObject.transform, targetNpc.PlayerRefGameObject.transform.position);
	            
//	            if(GetDis(npc, targetNpc) <= dis)
				if(MathUtils.Find2DDis(player.PlayerRefGameObject.transform.position, targetNpc.PlayerRefGameObject.transform.position) <= dis)
                {
	                if(realAngle >= 0 && realAngle <= angle)
                    {
	                    result = 1;
	                    break;
	                }
					if(realAngle <= 0 && realAngle >= -angle)
                    {
	                    result = 2;
	                    break;
	                }
	            }
	        }  
        }
        
        return result;
    }

	private int haveStealPlayer(PlayerBehaviour p1, PlayerBehaviour p2, float dis, float angle)
    {
		int result = 0;

	    if (p1 != null && p2 != null && p1 != p2)
		{
			float angleBetween = MathUtils.FindAngle(p1.PlayerRefGameObject.transform, p2.PlayerRefGameObject.transform.position);

		    if (GetDis(p1, p2) <= dis)
            {
				if(angleBetween >= 0 && angleBetween <= angle)				
					result = 1;
				else if(angleBetween <= 0 && angleBetween >= -angle)				
					result = 2;
			}
		}

	    return result;
	}
    
    [CanBeNull]
    private PlayerBehaviour tryFindNearPlayer(PlayerBehaviour self, float dis, bool isSameTeam, 
                                          bool findBallOwnerFirst = false)
    {
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerBehaviour someone = PlayerList[i];

            if(!someone.gameObject.activeInHierarchy)
                continue;
            
            if(isSameTeam)
            {
                if(someone != self && someone.Team == self.Team && 
                   MathUtils.Find2DDis(self.transform.position, someone.transform.position) <= dis)
                {
                    return someone;
                }
            }
            else
            {
                if(findBallOwnerFirst)
                {
                    if(someone != self && someone.Team != self.Team && someone == BallOwner &&
                       MathUtils.Find2DDis(self.transform.position, someone.transform.position) <= dis)
                        return someone;
                }
                else
                {
                    if(someone != self && someone.Team != self.Team &&
                       MathUtils.Find2DDis(self.transform.position, someone.transform.position) <= dis && 
                       someone.crtState == EPlayerState.Idle)
                        return someone;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 是否球員在前場.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool IsInUpfield(PlayerBehaviour player)
    {
		if(player.Team == ETeamKind.Self && (player.PlayerRefGameObject.transform.position.z >= 15.5f && player.PlayerRefGameObject.transform.position.x <= 1 && player.PlayerRefGameObject.transform.position.x >= -1))
            return false;
		if(player.Team == ETeamKind.Npc && player.PlayerRefGameObject.transform.position.z <= -15.5f && player.PlayerRefGameObject.transform.position.x <= 1 && player.PlayerRefGameObject.transform.position.x >= -1)
            return false;
		if(player.IsElbow)
			return false;
        return true;
    }
    
	#if UNITY_EDITOR
    public Vector3 EditGetPosition(int index)
    {
        if (PlayerList.Count > index)
			return PlayerList [index].PlayerRefGameObject.transform.position;       
        else
            return Vector3.zero;
    }
	#endif

	public void SetPlayerMove(TTacticalAction actionPosition, int index, bool clearPath = true) {
        if (index > -1 && index < PlayerList.Count) {
			moveData.Clear();
			moveData.SetTarget(actionPosition.X, actionPosition.Z);
			moveData.Speedup = actionPosition.Speedup;
			moveData.Catcher = actionPosition.Catcher;
			moveData.Shooting = actionPosition.Shooting;
			PlayerList [index].TargetPos = moveData;
        }
    }

	public void SetPlayerAppear(ref TToturialAction action) {
		int index = getPlayerIndex(action.Team, (EPlayerPostion)action.Index);
		if (index > -1 && index < PlayerList.Count) {
			if (action.MoveKind > 0)
				SetPlayerAppear(index, action.Action.X, action.Action.Z);
			else 
				SetPlayerMove(action.Action, index, false);

		}
	}

	public void SetPlayerAppear(int index, float x, float z) {
		PlayerList [index].ResetMove();
		PlayerList [index].transform.position = new Vector3(x, PlayerList [index].transform.position.y, z);
	}

	public void ResetAll()
	{
		for(int i = 0; i < PlayerList.Count; i++)
			PlayerList[i].ResetFlag();
	}

	public void SetPlayerMove(Vector2 ActionPosition, int index)
	{
		if (PlayerList.Count > index)
		{
			moveData.Clear();
			moveData.SetTarget(ActionPosition.x, ActionPosition.y); 
			PlayerList [index].TargetPos = moveData;
		}
	}
    
    public void EditSetJoysticker(int index)
    {
        if (PlayerList.Count > index)
        {
            Joysticker = PlayerList [index];
            UIGame.Get.SetJoystick(Joysticker);
        }
    }

    public void SetEndPass()
    {
		if(IsPassing){
			if (Catcher != null && !Catcher.IsFall && !Catcher.IsPush && !Catcher.IsBlock && !Catcher.IsPass)
	        {
	            if(SetBall(Catcher))
//					CoolDownPass = Time.time + GameConst.CoolDownPassTime;
                    PassCD.StartAgain();

				if(Catcher && Catcher.NeedShooting)
				{
					DoShoot();
					Catcher.NeedShooting = false;
				}

				Catcher = null;
			}else{
	            doDropBall(Passer);
			}
			IsPassing = false;
		}
    }

    /// <summary>
    /// 執行球員掉球.
    /// </summary>
    /// <param name="player"></param>
	private void doDropBall(PlayerBehaviour player = null)
    {
		if(IsPassing)
		{
			if (Catcher != null)
			{
				if(Catcher.NeedShooting)
				{
					DoShoot();
					Catcher.NeedShooting = false;
				}
			}
		}

		SetBall();
		CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.Steal0, player);
	}
	
	public void Reset()
	{
		IsReset = true;
		IsPassing = false;
		Shooter = null;
		IsStart = false;
		IsFinish = false;
		GameRecord.Init(PlayerList.Count);
		SetPlayerAI(true);
		SetBallOwnerNull();
		GameTime = MissionChecker.Get.MaxGameTime;
		MissionChecker.Get.Reset();
		CameraMgr.Get.ShowPlayerInfoCamera (false);
		CameraMgr.Get.InitCamera(ECameraSituation.JumpBall);
		CameraMgr.Get.ShowEnd();
		CameraMgr.Get.PlayGameStartCamera();
		UIPassiveEffect.Get.Reset();
		UIDoubleClick.Get.Reset();

		if (GameConst.AITime[GameData.Setting.AIChangeTimeLv] > 100)
			Joysticker.SetManually();
		else
			Joysticker.SetToAI();

		CourtMgr.Get.RealBallCompoment.SetBallState (EPlayerState.Reset);

		for(int i = 0; i < PlayerList.Count; i++) 
		{
			PlayerList [i].crtState = EPlayerState.Idle;
			PlayerList [i].AnimatorControl.Play("Idle");
			PlayerList [i].Reset();
			PlayerList [i].SetAnger (-PlayerList[i].Attribute.MaxAnger);

			if(PlayerList[i].Postion == EPlayerPostion.G)
			{
				if(PlayerList[i].Team == ETeamKind.Self)
					PlayerList[i].PlayerRefGameObject.transform.position = mJumpBallPos[0];
				else
					PlayerList[i].PlayerRefGameObject.transform.position = mJumpBallPos[3];
			}
			else if(PlayerList[i].Postion == EPlayerPostion.C)
			{
				if(PlayerList[i].Team == ETeamKind.Self)
					PlayerList[i].PlayerRefGameObject.transform.position = mJumpBallPos[1];
				else
					PlayerList[i].PlayerRefGameObject.transform.position = mJumpBallPos[4];
			}
			else
			{
				if(PlayerList[i].Team == ETeamKind.Self)
					PlayerList[i].PlayerRefGameObject.transform.position = mJumpBallPos[2];
				else
					PlayerList[i].PlayerRefGameObject.transform.position = mJumpBallPos[5];
			}

			PlayerList [i].AniState(EPlayerState.Idle);

			if(PlayerList[i].Team == ETeamKind.Npc)
				PlayerList[i].PlayerRefGameObject.transform.localEulerAngles = new Vector3(0, 180, 0);
			else
				PlayerList[i].PlayerRefGameObject.transform.localEulerAngles = Vector3.zero;
		}

		ChangeSituation(EGameSituation.Opening);
		AIController.Get.ChangeState(EGameSituation.Opening);
        TimerMgr.Get.ResetTime();
    }

	public void SetPlayerLevel(){
		if (GameConst.AITime[GameData.Setting.AIChangeTimeLv] > 100)
			Joysticker.SetManually();
		else
			Joysticker.SetToAI();

		//要讓玩家隨關卡變化AI等級，所以需要再重設一次
//		Joysticker.Attribute.AILevel = StageData.PlayerAI;
//		Joysticker.InitAttr();
		for(int i=0; i<PlayerList.Count; i++) {
			if(PlayerList[i].Team == ETeamKind.Self) {
				if(StageData.PlayerAI != 0) {
					PlayerList[i].Attribute.AILevel = StageData.PlayerAI;
					PlayerList[i].InitAttr();
				}
			} else {
				if(StageData.OppenentAI != 0) {
					PlayerList[i].Attribute.AILevel = StageData.OppenentAI;
					PlayerList[i].InitAttr();
				}
			}
		}
	}

    public void SetAllPlayerLayer (string layerName){
		for (int i = 0; i < PlayerList.Count; i++)
			LayerMgr.Get.ReSetLayerRecursively(PlayerList[i].PlayerRefGameObject, layerName, "PlayerModel", "(Clone)");
	}

	public void ShowWord (EShowWordType type, int team = 0, GameObject parent = null) {
		switch(type) {
		case EShowWordType.Block:
			EffectManager.Get.PlayEffect("ShowWord_Block", Vector3.zero, parent, null, 1, true);
			IsGameFinish();
			break;
		case EShowWordType.Dunk:
			EffectManager.Get.PlayEffect("ShowWord_Dunk", Vector3.zero, CourtMgr.Get.ShootPoint[team], null, 1, true);
			IsGameFinish();
			break;
		case EShowWordType.NiceShot:
			EffectManager.Get.PlayEffect("ShowWord_NiceShot", Vector3.zero, CourtMgr.Get.ShootPoint[team], null, 1, true);
			break;
		case EShowWordType.Punch:
			EffectManager.Get.PlayEffect("ShowWord_Punch", Vector3.zero, parent, null, 1, true);
			IsGameFinish();
			break;
		case EShowWordType.Steal:
			EffectManager.Get.PlayEffect("ShowWord_Steal", Vector3.zero, parent, null, 1, true);
			IsGameFinish();
			break;
		case EShowWordType.Turnover:
			EffectManager.Get.PlayEffect("ShowWord_Turnover", Vector3.zero, parent, null, 1, true);
			IsGameFinish();
			break;
		case EShowWordType.GetTwo:
			EffectManager.Get.PlayEffect("GetScoreTwo", CourtMgr.Get.ShootPoint[team].transform.position, null, null, 1.5f);
			break;
		case EShowWordType.GetThree:
			EffectManager.Get.PlayEffect("GetScoreThree", CourtMgr.Get.ShootPoint[team].transform.position, null, null, 1.5f);
			break;
		case EShowWordType.Assistant:
			EffectManager.Get.PlayEffect("ShowWord_Assist", Vector3.zero, parent, null, 1.5f);
			break;
		case EShowWordType.DC5:
			EffectManager.Get.PlayEffect("ShowWord_DC5", Vector3.zero, parent, null, 1.5f);
			break;
		case EShowWordType.DC10:
			EffectManager.Get.PlayEffect("ShowWord_DC10", Vector3.zero, parent, null, 1.5f);
			break;
		case EShowWordType.DC15:
			EffectManager.Get.PlayEffect("ShowWord_DC15", Vector3.zero, parent, null, 1.5f);
			break;
		case EShowWordType.DC20:
			EffectManager.Get.PlayEffect("ShowWord_DC20", Vector3.zero, parent, null, 1.5f);
			break;
		case EShowWordType.Catch:
			EffectManager.Get.PlayEffect("ShowWord_Catch", Vector3.zero, parent, null, 1.5f);
			break;
		case EShowWordType.TipShot:
			EffectManager.Get.PlayEffect("ShowWord_TipShot", Vector3.zero, parent, null, 1.5f);
			break;
		}
	}

	public void PushCalculate(PlayerBehaviour player, float dis, float angle)
	{
        //預防撿球時，被推倒卡住
        if (Situation == EGameSituation.GamerAttack || Situation == EGameSituation.NPCAttack)
        {
		for (int i = 0; i < PlayerList.Count; i++)
        {
			if(PlayerList[i] && PlayerList[i].Team != player.Team && !PlayerList[i].IsUseActiveSkill)
            {
				if(player.PlayerRefGameObject.transform.IsInFanArea(PlayerList[i].PlayerRefGameObject.transform.position, dis, angle))
                {
					int rate = Random.Range(0, 100);
					PlayerBehaviour faller = PlayerList[i];
					PlayerBehaviour pusher = player;

                    if (rate < faller.Attr.StrengthRate)
                    {
                        if (faller.AniState(EPlayerState.Fall2, pusher.PlayerRefGameObject.transform.position))
                        {
							faller.SetAnger(GameConst.DelAnger_Fall2);
							pusher.SetAnger(GameConst.AddAnger_Push, faller.PlayerRefGameObject);
							pusher.GameRecord.Knock++;
							faller.GameRecord.BeKnock++;
						}
					}
					else
                    {
						if(faller.PlayerSkillController.DoPassiveSkill(ESkillSituation.Fall1, pusher.transform.position))
                        {
							faller.SetAnger(GameConst.DelAnger_Fall1);
							pusher.SetAnger(GameConst.AddAnger_Push, faller.PlayerRefGameObject);
							ShowWord(EShowWordType.Punch, 0, pusher.ShowWord);
						}
					}

                        if (pusher.IsElbow)
                        {
						pusher.GameRecord.Elbow++;
						faller.GameRecord.BeElbow++;
                        }
                        else
                        {
						pusher.GameRecord.Push++;
						faller.GameRecord.BePush++;
					}
					pusher.IsElbowCalculate = false;
					pusher.IsPushCalculate = false;

					IsGameFinish();
				}

			}
		}
        }
	}
	
	public bool IsOnceAnimation(EAnimatorState state)
	{
		if (LoopStates.ContainsKey (state))
			return false;
		else
			return true;
	}

	public EDoubleType DoubleClickType {
		get {return doubleType;}
		set {
            doubleType = value;
            if (doubleType == EDoubleType.Good || doubleType == EDoubleType.Perfect) {
                if (Joysticker)
                    Joysticker.GameRecord.DoubleClickPerfact++;
            }
        }
	}

	public int GetBallOwner {
		get {
			for (int i = 0; i < PlayerList.Count; i++)            
				if (PlayerList [i].IsBallOwner)
					return i;
			
			return 99;
		}
	}

    public bool IsShooting
    {
        get
        {
            if (Shooter && (
                Shooter.CheckAnimatorSate(EPlayerState.Shoot0) || 
                Shooter.CheckAnimatorSate(EPlayerState.Shoot1) || 
                Shooter.CheckAnimatorSate(EPlayerState.Shoot2) || 
                Shooter.CheckAnimatorSate(EPlayerState.Shoot3) ||
                Shooter.CheckAnimatorSate(EPlayerState.Shoot6) ||
                Shooter.CheckAnimatorSate(EPlayerState.TipIn) ||
                Shooter.IsLayup))
                return true;
            else
                return false;
        }
    }

	public bool IsCanUseShootDoubleClick()
	{
		if(BallOwner && BallOwner.Team == ETeamKind.Self && (
			BallOwner.CheckAnimatorSate(EPlayerState.Shoot0) || 
			BallOwner.CheckAnimatorSate(EPlayerState.Shoot1) || 
			BallOwner.CheckAnimatorSate(EPlayerState.Shoot2) || 
			BallOwner.CheckAnimatorSate(EPlayerState.Shoot3) ||
			BallOwner.CheckAnimatorSate(EPlayerState.Shoot6) ||
			BallOwner.CheckAnimatorSate(EPlayerState.TipIn) ||
            BallOwner.CheckAnimatorSate(EPlayerState.Rebound0) ||
			BallOwner.IsLayup))
			return true;
		else
			return false;
	}

	public bool IsShowSituation
	{
		get{
			if(Situation <= EGameSituation.Opening)
				return true;
			else
				return false;
		}
	}

	public bool IsCanPassAir
	{
		get
		{
			for (int i = 0; i < PlayerList.Count; i++)
            {
                // Shoot0 and Shoot2 這 2 個 Animation, 是可以做空中傳球的特殊投籃動作.
                if(PlayerList[i].CheckAnimatorSate(EPlayerState.Shoot0) || 
                   PlayerList[i].CheckAnimatorSate(EPlayerState.Shoot2))
					return true;
            }
			
			return false;
		}
	}

    public bool IsDunk
    {
        get
        {
            for (int i = 0; i < PlayerList.Count; i++)            
				if (PlayerList [i].IsDunk)
                    return true;            
            
            return false;
        }
    }

	public bool IsAlleyoop
	{
		get
		{
			for (int i = 0; i < PlayerList.Count; i++)            
				if (PlayerList [i].CheckAnimatorSate(EPlayerState.Alleyoop))
					return true;            
			
			return false;
		}
	}

    public bool IsBlocking
    {
        get
        {
            for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].IsBlock)
					return true;
            
            return false;
        }
    }

	public bool IsPush
	{
		get
		{
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].IsPush)
					return true;
			
			return false;
		}
	}

	public bool IsTipin
	{
		get
		{
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].CheckAnimatorSate(EPlayerState.TipIn))
					return true;
            
            return false;
        }
    }

	public bool IsPickBall
	{
		get
		{
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].IsPickBall)
					return true;
			
			return false;
		}
	}

	public bool IsPassing {
		get {
			return isPassing;
		}
		set {
			isPassing = value;
			if (!value) {
				Catcher = null;
				Passer = null;
			}
		}
	}

	//true: 有人在使用 
	public bool CheckOthersUseSkill (int myTimerKind) {
		for (int i=0; i<PlayerList.Count; i++) 
			if(PlayerList[i].TimerKind.GetHashCode() != myTimerKind)
				if(PlayerList[i].IsUseActiveSkill)
					return true;
					return false;
	}

	public bool CheckAllPlayerIdle {
		get {
			bool isFlag = true;
			for (int i=0; i<PlayerList.Count; i++)
				if(!PlayerList[i].IsIdle)
					isFlag = false;

			return isFlag;
		}
	}

	public bool CanUseStealSkill {
		get {
			if(Passer == null && Catcher == null && BallOwner == null && Shooter == null)
				return false;

			if(Passer != null && Catcher == null && BallOwner == null && Shooter == null)
				return false;

			return true;
		}
	}

	public bool CandoBtn
	{
		get
		{
			if(Situation == EGameSituation.GamerInbounds || Situation == EGameSituation.NPCInbounds || Situation == EGameSituation.GamerPickBall || Situation == EGameSituation.NPCPickBall)
				return false;
			else
				return true;
		}
	}

	public List<PlayerBehaviour> GamePlayers {
		get {
			return PlayerList;
		}
	}
}
