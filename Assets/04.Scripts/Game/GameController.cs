using System.Collections;
using System.Collections.Generic;
using AI;
using DG.Tweening;
using G2;
using GamePlayEnum;
using GamePlayStruct;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

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
	Skill
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
	public bool IsReset = false;
	public bool IsJumpBall = false;
	private bool isPassing = false;
	public float GameTime = 0;
    public float coolDownPass = 0;
    public float CoolDownCrossover = 0;
    public float ShootDistance = 0;
    public float RealBallFxTime = 0;
	public float StealBtnLiftTime = 1f;
	private float waitStealTime = 0;
	private float passingStealBallTime = 0;

	public PlayerBehaviour BallOwner; // 持球的球員.
	public PlayerBehaviour Joysticker; // 玩家控制的球員.

    /// <summary>
    /// 投籃出手的人. OnShooting 會有值, 得分後才會設定為 null.
    /// </summary>
    [CanBeNull]public PlayerBehaviour Shooter;
    public PlayerBehaviour Catcher;
	public PlayerBehaviour Passer;
	private PlayerBehaviour pickBallPlayer;
	private GameObject ballHolder = null;

    // 0:C, 1:F, 2:G
	private Vector2[] teeBackPosAy = new Vector2[3];

    // A Team, 0:G, 1:C, 2:F
    // B Team, 0:G, 1:C, 2:F
    private Vector3[] bornPosAy = new Vector3[6];

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
	public bool IsSwishIn = false;
	private int shootAngle = 55;
	private float extraScoreRate = 0;
	private float angleByPlayerHoop = 0;
	private EDoubleType doubleType = EDoubleType.None;

	//Rebound
	public bool IsReboundTime = false;

	//Basket
	public EBasketSituation BasketSituation;
	public string BasketAnimationName = "BasketballAction_1";
	private EBasketDistanceAngle basketDistanceAngle = EBasketDistanceAngle.ShortCenter;
	private string[] basketanimationTest = new string[25]{"0","1","2","3","4","5","6","7","8","9","10","11","100","101","102","103","104","105","106","107","108","109","110","111","112"};
   
	//Effect
    public GameObject[] passIcon = new GameObject[3];

	public EPlayerState testState = EPlayerState.Shoot0;
	public EPlayerState[] ShootStates = new EPlayerState[]{EPlayerState.Shoot0, EPlayerState.Shoot1, EPlayerState.Shoot2, EPlayerState.Shoot3, EPlayerState.Shoot6, EPlayerState.Layup0, EPlayerState.Layup1, EPlayerState.Layup2, EPlayerState.Layup3};
	public static Dictionary<EAnimatorState, bool> LoopStates = new Dictionary<EAnimatorState, bool>();


	//debug value
	public float RecordTimeScale = 1;
	public int PlayCount = 0;
	public int SelfWin = 0;
	public int NpcWin = 0;
	public int shootSwishTimes = 0;
	public int shootScoreSwishTimes = 0;
	public int shootTimes = 0;
	public int shootScoreTimes = 0;

	public float randomrate = 0;
	public float normalRate = 0;
	public float uphandRate = 0;
	public float downhandRate = 0;
	public float layupRate = 0;
	public float nearshotRate = 0;
	
	void OnDestroy() {
		for (int i = 0; i < PlayerList.Count; i++) 
			Destroy(PlayerList[i]);

		PlayerList.Clear();
	}

    [UsedImplicitly]
    private void Awake()
    {
        // 這是 AI 整個框架初始化的起點.
        AIController.Get.ChangeState(EGameSituation.None);
		UITransition.Visible = true;
		EffectManager.Get.LoadGameEffect();
		InitPos();
		InitGame();
		InitAniState();
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

    private void InitPos()
    {
        teeBackPosAy[0] = new Vector2(0, 14.5f);   // C
        teeBackPosAy[1] = new Vector2(5.3f, 11);   // F
        teeBackPosAy[2] = new Vector2(-5.3f, 11);  // G

		/*
		 *     0	 5
		 * 		 1 4
		 * 	   2     3
		 */

		bornPosAy[0] = new Vector3(-3.5f, 0, -3); // G_A
		bornPosAy[1] = new Vector3(0, 0, -1.5f);  // C_A
		bornPosAy[2] = new Vector3(3.5f, 0, -3);  // F_A
		bornPosAy[3] = new Vector3(3.5f, 0, 3);   // G_B
		bornPosAy[4] = new Vector3(0, 0, 1.5f);   // C_B
		bornPosAy[5] = new Vector3(-3.5f, 0, 3);  // F_B
    }

    public void InitGame()
    {
		IsPassing = false;
		Shooter = null;
		for (var i = 0; i < PlayerList.Count; i ++)
			if (PlayerList[i]) {
				Destroy (PlayerList[i]);
				PlayerList[i] = null;
			}

        PlayerList.Clear();

		GameTime = GameStart.Get.GameWinValue;
		UIGame.Get.MaxScores[0] = GameStart.Get.GameWinValue;
		UIGame.Get.MaxScores[1] = GameStart.Get.GameWinValue;

		StateChecker.InitState();
        CreateTeam();
		SetBallOwnerNull (); 
    }

	public void StartGame() {
		IsReset = false;
		IsJumpBall = false;
		SetPlayerLevel();

		if (SendHttp.Get.CheckNetwork()) {
			string str = PlayerPrefs.GetString(SettingText.GameRecord);
			if (str != "") {
				WWWForm form = new WWWForm();
				form.AddField("GameRecord", str);
				form.AddField("Start", PlayerPrefs.GetString(SettingText.GameRecordStart));
				form.AddField("End", PlayerPrefs.GetString(SettingText.GameRecordEnd));
				SendHttp.Get.Command(URLConst.GameRecord, null, form, false);
			}
		}

		GameRecord.Init(PlayerList.Count);
		for (var i = 0; i < PlayerList.Count; i ++)
			if (PlayerList[i]) 
				PlayerList[i].GameRecord.Init();

		if (GameStart.Get.TestMode == EGameTest.Rebound) {
			CourtMgr.Get.RealBallRigidbody.isKinematic = true;
			CourtMgr.Get.RealBall.transform.position = new Vector3(0, 5, 13);
		}

		ChangeSituation(EGameSituation.JumpBall);
        AIController.Get.ChangeState(EGameSituation.JumpBall);
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
		if (!GameData.DPlayers.ContainsKey(GameData.Team.Player.ID))
			GameData.Team.Player.SetID(14);

		if (!GameData.DPlayers.ContainsKey(GameData.TeamMembers[0].Player.ID))
			GameData.TeamMembers[0].Player.SetID(24);

		if (!GameData.DPlayers.ContainsKey(GameData.TeamMembers[1].Player.ID))
			GameData.TeamMembers[1].Player.SetID(34);

		for (int i = 0; i < GameData.EnemyMembers.Length; i ++)
			if (!GameData.DPlayers.ContainsKey(GameData.EnemyMembers[i].Player.ID))
				GameData.EnemyMembers[i].Player.SetID(19 + i*10);
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
		PlayerList[aPosAy[0]].transform.position = bornPosAy[0];
		PlayerList[aPosAy[0]].ShowPos = 1;
		PlayerList[aPosAy[0]].IsJumpBallPlayer = false;
		PlayerList[aPosAy[1]].Postion = EPlayerPostion.C;
		PlayerList[aPosAy[1]].transform.position = bornPosAy[1];
		PlayerList[aPosAy[1]].ShowPos = 0;
		PlayerList[aPosAy[1]].IsJumpBallPlayer = true;
		PlayerList[aPosAy[2]].Postion = EPlayerPostion.F;
		PlayerList[aPosAy[2]].transform.position = bornPosAy[2];
		PlayerList[aPosAy[2]].ShowPos = 2;
		PlayerList[aPosAy[2]].IsJumpBallPlayer = false;
		
		//Team B
		PlayerList[bPosAy[0]].Postion = EPlayerPostion.G;
		PlayerList[bPosAy[0]].transform.position = bornPosAy[3];
		PlayerList[bPosAy[0]].ShowPos = 4;
		PlayerList[bPosAy[0]].IsJumpBallPlayer = false;
		PlayerList[bPosAy[1]].Postion = EPlayerPostion.C;
		PlayerList[bPosAy[1]].transform.position = bornPosAy[4];
		PlayerList[bPosAy[1]].ShowPos = 3;
		PlayerList[bPosAy[1]].IsJumpBallPlayer = true;
		PlayerList[bPosAy[2]].Postion = EPlayerPostion.F;
		PlayerList[bPosAy[2]].transform.position = bornPosAy[5];
		PlayerList[bPosAy[2]].ShowPos = 5;
		PlayerList[bPosAy[2]].IsJumpBallPlayer = false;
	}

	private void PlayZeroPosition()
	{
		for(int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i])
				PlayerList[i].transform.position = Vector3.zero;
	}

	public void InitIngameAnimator()
	{
		for(int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i])
				ModelManager.Get.ChangeAnimator(ref PlayerList[i].AnimatorControl, PlayerList[i].Attribute.BodyType.ToString(), EanimatorType.AnimationControl);
		
		for(int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i].ShowPos != 0 || PlayerList[i].ShowPos != 3)
				PlayerList[i].AniState(EPlayerState.Idle);
	}
	
	public void CreateTeam()
    {
        switch(GameStart.Get.TestMode)
        {
    	case EGameTest.None:
			checkPlayerID();

			switch (GameStart.Get.FriendNumber)
            {
			    case 1:
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], GameData.Team.Player));
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, bornPosAy[3], GameData.EnemyMembers[0].Player));
                    break;
			    case 2:
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], GameData.Team.Player));
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Self, bornPosAy[1], GameData.TeamMembers[0].Player));
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, bornPosAy[3], GameData.EnemyMembers[0].Player));
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Npc, bornPosAy[4], GameData.EnemyMembers[1].Player));
                    break;

                case 3:
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], GameData.Team.Player));
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Self, bornPosAy[1], GameData.TeamMembers[0].Player));
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Self, bornPosAy[2], GameData.TeamMembers[1].Player));
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, bornPosAy[3], GameData.EnemyMembers[0].Player));
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Npc, bornPosAy[4], GameData.EnemyMembers[1].Player));
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Npc, bornPosAy[5], GameData.EnemyMembers[2].Player));
                    break;
            }

			//1.G(Dribble) 2.C(Rebound) 3.F
			SetBornPositions();
			PlayZeroPosition ();
        	break;
		case EGameTest.All:
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], new GameStruct.TPlayer(0)));	
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Self, bornPosAy[1], new GameStruct.TPlayer(0)));	
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Self, bornPosAy[2], new GameStruct.TPlayer(0)));	
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, bornPosAy[3], new GameStruct.TPlayer(0)));	
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Npc, bornPosAy[4], new GameStruct.TPlayer(0)));	
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Npc, bornPosAy[5], new GameStruct.TPlayer(0)));

			break;
    	case EGameTest.AttackA:
    	case EGameTest.Shoot:
    	case EGameTest.Dunk:
		case EGameTest.Rebound:
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[1], new GameStruct.TPlayer(0)));	
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Npc, bornPosAy[4], new GameStruct.TPlayer(0)));	
			SetBornPositions();
        	UIGame.Get.ChangeControl(true);
			SetPlayerAI(false);
        	break;
		case EGameTest.AnimationUnit:
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
			PlayerList[0].IsJumpBallPlayer = true;
			SetPlayerAI(false);
			break;
    	case EGameTest.AttackB:
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
			PlayerList[0].IsJumpBallPlayer = true;
			SetPlayerAI(false);
			break;
    	case EGameTest.Block:
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, -8.4f), new GameStruct.TPlayer(0)));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, ETeamKind.Npc, new Vector3 (0, 0, -4.52f), new GameStruct.TPlayer(0)));
			PlayerList[0].IsJumpBallPlayer = true;
			PlayerList[1].IsJumpBallPlayer = true;
			SetPlayerAI(false);
			break;
		case EGameTest.OneByOne: 
			TPlayer Self = new TPlayer(0);
			Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			

			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), Self));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Npc, new Vector3 (0, 0, 5), new GameStruct.TPlayer(0)));
			PlayerList[0].IsJumpBallPlayer = true;
			PlayerList[1].IsJumpBallPlayer = true;
        	break;
		case EGameTest.Alleyoop:
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, ETeamKind.Self, new Vector3 (0, 0, 3), new GameStruct.TPlayer(0)));
			PlayerList[0].IsJumpBallPlayer = true;
			PlayerList[1].IsJumpBallPlayer = true;
			break;
		case EGameTest.Pass:
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, ETeamKind.Self, new Vector3 (-5, 0, -2), new GameStruct.TPlayer(0)));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (2, ETeamKind.Self, new Vector3 (5, 0, -2), new GameStruct.TPlayer(0)));
			PlayerList[0].IsJumpBallPlayer = true;
			break;
    	case EGameTest.Edit:
			GameData.Team.Player.SetID(34);		
			GameData.TeamMembers[0].Player.SetID(24);			
			GameData.TeamMembers[1].Player.SetID(14);
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], GameData.Team.Player));	
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Self, bornPosAy[1], GameData.TeamMembers[0].Player));	
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Self, bornPosAy[2], GameData.TeamMembers[1].Player));
			PlayerList[0].IsJumpBallPlayer = true;
			break;
		case EGameTest.CrossOver:
			Self = new TPlayer(0);
			Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			
			
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), Self));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, ETeamKind.Npc, new Vector3 (0, 0, 5), new TPlayer(0)));
			PlayerList[0].IsJumpBallPlayer = true;
			PlayerList[1].IsJumpBallPlayer = true;
			break;
		case EGameTest.Skill:
			if (GameData.Team.Player.ID == 0) 
				GameData.Team.Player.SetID(14);

			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], GameData.Team.Player));
			SetPlayerAI(false);
			break;
        }

		for (int i = 0; i < PlayerList.Count; i++)
			PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);

        Joysticker = PlayerList[0];

		EffectManager.Get.PlayEffect("SelectMe", Vector3.zero, null, Joysticker.gameObject);
		#if UNITY_EDITOR
        Joysticker.AIActiveHint = GameObject.Find("SelectMe/AI");
		#else
		GameObject obj = GameObject.Find("SelectMe/AI");
		if (obj)
			obj.SetActive(false);
		#endif

		Joysticker.SpeedUpView = GameObject.Find("SelectMe/Speedup").GetComponent<UISprite>();

		passIcon[0] = EffectManager.Get.PlayEffect("PassMe", Joysticker.BodyHeight.transform.localPosition, Joysticker.gameObject);

		if (Joysticker.SpeedUpView)
			Joysticker.SpeedUpView.enabled = false;

        if (PlayerList.Count > 1 && PlayerList [1].Team == Joysticker.Team) {
			passIcon[1] = EffectManager.Get.PlayEffect("PassA", Joysticker.BodyHeight.transform.localPosition, PlayerList [1].gameObject);
//			selectIcon[0] = EffectManager.Get.PlayEffect("SelectA", Vector3.zero, null, PlayerList [1].gameObject);
		}

        if (PlayerList.Count > 2 && PlayerList [2].Team == Joysticker.Team) {
			passIcon[2] = EffectManager.Get.PlayEffect("PassB", Joysticker.BodyHeight.transform.localPosition, PlayerList [2].gameObject);
//			selectIcon[1] = EffectManager.Get.PlayEffect("SelectB", Vector3.zero, null, PlayerList [2].gameObject);
		}
		
		UIGame.Get.InitGameUI();
		setPassIcon(false);

		Joysticker.OnUIJoystick = UIGame.Get.SetUIJoystick;
        for (int i = 0; i < PlayerList.Count; i ++)
        {
            PlayerList [i].OnShooting = OnShooting;
//            PlayerList [i].OnPass = OnPass;
            PlayerList [i].OnStealMoment = OnStealMoment;
			PlayerList [i].OnGotSteal = OnGotSteal;
            PlayerList [i].OnBlockMoment = OnBlockMoment;
			PlayerList [i].OnDoubleClickMoment = OnDoubleClickMoment;
			PlayerList [i].OnFakeShootBlockMoment = OnFakeShootBlockMoment;
            PlayerList [i].OnBlockJump = OnBlockJump;
//			PlayerList [i].OnBlockCatching = OnBlockCatching;
//			PlayerList [i].OnBlocking = OnBlocking;
            PlayerList [i].OnDunkJump = OnDunkJump;
            PlayerList [i].OnDunkBasket = OnDunkBasket;
			PlayerList [i].OnOnlyScore = OnOnlyScore;
			PlayerList [i].OnPickUpBall = OnPickUpBall;
			PlayerList [i].OnFall = OnFall;
			PlayerList [i].OnUI = UIGame.Get.OpenUIMask;
//			PlayerList [i].OnUISkill = UIGame.Get.ShowSkill;
			PlayerList [i].OnUICantUse = UIGame.Get.UICantUse;
			PlayerList [i].OnUIAnger = UIGame.Get.SetAngerUI;
        }

        GameMsgDispatcher.Ins.SendMesssage(EGameMsg.GamePlayersCreated, PlayerList.ToArray());
    }

	public void SetPlayerAI(bool enable){
		for(int i = 0; i < PlayerList.Count; i++)
			PlayerList[i].GetComponent<PlayerAI>().enabled = enable;
	}

	private void setPassIcon(bool isShow) {
//		if(GameStart.Get.TestMode == EGameTest.None) {
//			for(int i=0; i<3; i++) {
//				if (i < 2 && selectIcon[i])
//					selectIcon[i].SetActive(isShow);
//			}
//		}
	}

	void FixedUpdate() {
		#if UNITY_EDITOR
		if (Joysticker) {
			if (Input.GetKeyUp (KeyCode.V))
				BallOwner.AniState(EPlayerState.KnockDown1);

			if (Input.GetKeyUp (KeyCode.K))
				gameResult();

			if (Input.GetKeyUp (KeyCode.I))
			{
				AnimationEvent e = new AnimationEvent();
				e.floatParameter = 1.5f;
				e.intParameter = 1;
				Joysticker.SkillEvent(e);
			}

			if (Input.GetKeyUp (KeyCode.U))
			{
				AnimationEvent e = new AnimationEvent();
				e.floatParameter = 1.5f;
				e.intParameter = 2;
				Joysticker.SkillEvent(e);
			}

			if (Input.GetKeyUp (KeyCode.D))
			{
				UIGame.Get.DoAttack(null, true);
				UIGame.Get.DoAttack(null, false);
			}

			if (Input.GetKeyDown(KeyCode.N))
			{
				TimerMgr.Get.PauseTime(true);
			}

			if (Input.GetKeyDown(KeyCode.T)){
				UIDoubleClick.Get.ClickStop(0);
			}

			if (Input.GetKeyUp (KeyCode.M))
			{
				TimerMgr.Get.PauseTime(false);
			}

			if (Situation == EGameSituation.AttackA) {
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
					if(GameStart.Get.TestMode == EGameTest.AnimationUnit){
						Joysticker.AniState(GameStart.Get.SelectAniState);
						Joysticker.PassiveID = (int )GameStart.Get.SelectAniState;
						if((int)GameStart.Get.SelectAniState > 100 && GameStart.Get.TestMode == EGameTest.AnimationUnit) 
							SkillEffectManager.Get.OnShowEffect(Joysticker, true);
					}else
						UIGame.Get.DoShoot(null, true);
				}
				
				if (Input.GetKeyUp (KeyCode.S))
				{
					if(GameStart.Get.TestMode != EGameTest.AnimationUnit)
						UIGame.Get.DoShoot(null, false);
				}
			}
			else if(Situation == EGameSituation.AttackB){
				if(Input.GetKeyDown (KeyCode.A)){
					UIGame.Get.DoSteal(null, true);
					UIGame.Get.DoSteal(null, false);
				}

				if(Input.GetKeyDown (KeyCode.S)){
					UIGame.Get.DoBlock();
				}
			}

			if (Input.GetKeyDown (KeyCode.R) && Joysticker != null)
				Joysticker.DoPassiveSkill(ESkillSituation.Rebound);

			if (Input.GetKeyDown (KeyCode.T) && Joysticker != null)
				Joysticker.AniState (EPlayerState.ReboundCatch);

			if (GameStart.Get.TestMode == EGameTest.Rebound && Input.GetKeyDown (KeyCode.Z)) {
				resetTestMode();
            }

			if(Input.GetKeyDown(KeyCode.L)) {
				for (int i = 0; i < PlayerList.Count; i ++){
					PlayerList[i].SetAnger(PlayerList[i].Attribute.MaxAnger);
                UIGame.Get.AddAllForce();
				}
			}

			if(Input.GetKeyDown(KeyCode.P) && Joysticker != null) { 
				Joysticker.SetAnger(Joysticker.Attribute.MaxAnger);
				UIGame.Get.AddAllForce();
			}

			if(Input.GetKeyDown(KeyCode.O) && Joysticker != null) {
				UIGame.Get.DoSkill(null, false);
			}

			if (Situation == EGameSituation.JumpBall || Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)
				judgeSkillUI();
		}
		#endif

		if (coolDownPass > 0 && Time.time >= coolDownPass)
            coolDownPass = 0;

		if (CoolDownCrossover > 0 && Time.time >= CoolDownCrossover)
            CoolDownCrossover = 0;

        if (RealBallFxTime > 0)
        {
            RealBallFxTime -= Time.deltaTime;
            if (RealBallFxTime <= 0)
                CourtMgr.Get.RealBallFX.SetActive(false);
        }
        
        handleSituation();

		if(StealBtnLiftTime > 0)
			StealBtnLiftTime -= Time.deltaTime;

		if(waitStealTime > 0 && Time.time >= waitStealTime)		
			waitStealTime = 0;

		if(passingStealBallTime > 0 && Time.time >= passingStealBallTime)		
			passingStealBallTime = 0;

		if (GameStart.Get.WinMode == EWinMode.Time && GameTime > 0) {
			if (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB) {
				GameTime -= Time.deltaTime;
				if (GameTime <= 0) {
					GameTime = 0;

					gameResult();
				}
			}
		}

		judgeSkillUI ();
	}
	
	private void resetTestMode() {
		SetBallOwnerNull();
		SetBall();
		CourtMgr.Get.RealBall.transform.localEulerAngles = Vector3.zero;
		CourtMgr.Get.SetBallState(EPlayerState.Shoot0);
		CourtMgr.Get.RealBall.transform.position = new Vector3(0, 5, 13);
		CourtMgr.Get.RealBallRigidbody.isKinematic = true;
		UIGame.Get.ChangeControl(true);
//		TMoveData md = new TMoveData(1);
		//md.Target = new Vector2(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);
		PlayerList[1].transform.position = new Vector3(CourtMgr.Get.RealBall.transform.position.x, 0, CourtMgr.Get.RealBall.transform.position.z);
		PlayerList[1].AniState(EPlayerState.Idle);
    }

	public void SetGameRecord(bool upload) {
		GameRecord.Identifier = SystemInfo.deviceUniqueIdentifier;
		GameRecord.Version = BundleVersion.Version;
		GameRecord.End = System.DateTime.UtcNow;
		GameRecord.PauseCount++;
		GameRecord.Score1 = UIGame.Get.Scores [0];
		GameRecord.Score2 = UIGame.Get.Scores [1];
		for (int i = 0; i < PlayerList.Count; i ++) {
			if (i < GameRecord.PlayerRecords.Length) {
				PlayerList[i].GameRecord.ShotError = Mathf.Max(0, 
			        PlayerList[i].GameRecord.ShotError - PlayerList[i].GameRecord.BeBlock);

                GameRecord.PlayerRecords[i] = PlayerList[i].GameRecord;
			}
		}

		if (upload) {
			string str = JsonConvert.SerializeObject(GameRecord);
			if (SendHttp.Get.CheckNetwork()) {
				WWWForm form = new WWWForm();
				form.AddField("GameRecord", str);
				form.AddField("Start", GameRecord.Start.ToString());
				form.AddField("End", GameRecord.End.ToString());
				SendHttp.Get.Command(URLConst.GameRecord, null, form, false);
			} else {
				PlayerPrefs.SetString(SettingText.GameRecord, str);
				PlayerPrefs.SetString(SettingText.GameRecordStart, GameRecord.Start.ToString());
				PlayerPrefs.SetString(SettingText.GameRecordEnd, GameRecord.End.ToString());
			}
		}
	}

    #if UNITY_EDITOR
	private bool isOpen = true;
	void OnGUI()
    {
		if (GameStart.Get.IsShowPlayerInfo) {
			if(isOpen){
				if(GUILayout.Button("Close"))
					isOpen = false;
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
			} else {
				if(GUILayout.Button("Open"))
					isOpen = true;
			}


		}

		if (GameStart.Get.TestMode == EGameTest.Rebound) {
			if (GUI.Button(new Rect(100, 100, 100, 100), "Reset")) {
				resetTestMode();
			}
		}

		if (GameStart.Get.TestMode == EGameTest.CrossOver) {
			if (GUI.Button(new Rect(20, 50, 100, 100), "Left")) {
				PlayerList[0].transform.DOMoveX(PlayerList[0].transform.position.x - 1, GameStart.Get.CrossTimeX).SetEase(Ease.Linear);
				PlayerList[0].transform.DOMoveZ(PlayerList[0].transform.position.z + 6, GameStart.Get.CrossTimeZ).SetEase(Ease.Linear);
				PlayerList[0].AniState(EPlayerState.MoveDodge0);
			}

			if (GUI.Button(new Rect(120, 50, 100, 100), "Right")) {
				PlayerList[0].transform.DOMoveX(PlayerList[0].transform.position.x + 1, GameStart.Get.CrossTimeX).SetEase(Ease.Linear);
				PlayerList[0].transform.DOMoveZ(PlayerList[0].transform.position.z + 6, GameStart.Get.CrossTimeZ).SetEase(Ease.Linear);
				PlayerList[0].AniState(EPlayerState.MoveDodge1);
			}
		}

		if (GameStart.Get.TestMode == EGameTest.Shoot) {
			for(int i = 0 ; i < ShootStates.Length; i++){
				if (GUI.Button(new Rect(Screen.width / 2, 50 + i * 50, 100, 50), ShootStates[i].ToString())) {	
					testState = ShootStates[i];
				}
			}
		}
		if(GameStart.Get.IsDebugAnimation){
			GUI.Label(new Rect(Screen.width * 0.5f - 25, 100, 300, 50), "Play Counts:" + PlayCount.ToString());
			GUI.Label(new Rect(Screen.width * 0.25f - 25, 100, 300, 50), "Self Wins:" + SelfWin.ToString());
			GUI.Label(new Rect(Screen.width * 0.75f - 25, 100, 300, 50), "Npc Wins:" + NpcWin.ToString());
			GUI.Label(new Rect(Screen.width * 0.25f - 25, 150, 300, 50), "Shoot Swish Times:" + shootSwishTimes.ToString());
			GUI.Label(new Rect(Screen.width * 0.75f - 25, 150, 300, 50), "Shoot Score Swish Times:" + shootScoreSwishTimes.ToString());
			GUI.Label(new Rect(Screen.width * 0.25f - 25, 200, 300, 50), "Shoot Times:" + shootTimes.ToString());
			GUI.Label(new Rect(Screen.width * 0.75f - 25, 200, 300, 50), "Shoot Score Times:" + shootScoreTimes.ToString());
		}

		if(GameStart.Get.TestMode == EGameTest.AnimationUnit){
			if (GUI.Button(new Rect(0, 0, 100, 100), "shine player")) {
				Joysticker.IsChangeColor = true;
			}
		}

		if(GameStart.Get.IsShowShootRate) {
			GUILayout.Label("random rate:"+ randomrate);
			GUILayout.Label("normal rate:"+ normalRate);
			GUILayout.Label("uphand rate:"+ uphandRate);
			GUILayout.Label("downhand rate:"+ downhandRate);
			GUILayout.Label("nearshot rate:"+ nearshotRate);
			GUILayout.Label("layup rate:"+ layupRate);
		}
	}
	#endif

//	private void jumpBall()
//	{
//		if(BallOwner == null)
//		{
//			foreach(PlayerBehaviour someone in PlayerList)
//			{
//                DoPickBall(someone);
//
////			    if(someone.Team == ETeamKind.Self)
////			    {
////			        doPickBall(someone);
////			        if(someone.DefPlayer != null)
////			            doPickBall(someone.DefPlayer);
////			    }
//			}
//		}
//	}

    /// <summary>
    /// 某隊得分, 另一隊執行撿球.
    /// </summary>
    /// <param name="team"> 執行撿球的隊伍(玩家 or 電腦). </param>
    private void SituationPickBall(ETeamKind team)
    {
        if(pickBallPlayer || BallOwner || PlayerList.Count <= 0)
            return;

//        if(team == ETeamKind.Self)
//        {
//            var player = AIController.Get.PlayerTeam.FindNearBallPlayer();
//            if(player != null)
//                pickBallPlayer = player.GetComponent<PlayerBehaviour>();
//        }
//        else if(team == ETeamKind.Npc)
//        {
//            var player = AIController.Get.NpcTeam.FindNearBallPlayer();
//            if(player != null)
//                pickBallPlayer = player.GetComponent<PlayerBehaviour>();
//        }

        var player = AIController.Get.GeTeam(team).FindNearBallPlayer();
        if (player != null)
            pickBallPlayer = player.GetComponent<PlayerBehaviour>();

        if (pickBallPlayer == null)
            return;

        // 根據撿球員的位置(C,F,G) 選擇適當的進攻和防守戰術.
        if(GameStart.Get.CourtMode == ECourtMode.Full)
        {
            AITools.RandomCorrespondingTactical(ETactical.Inbounds, ETactical.InboundsDefence, 
                                                pickBallPlayer.Index, out attackTactical, out defTactical);
        }
        else
        {
            AITools.RandomCorrespondingTactical(ETactical.HalfInbounds, ETactical.HalfInboundsDefence,
                                                pickBallPlayer.Index, out attackTactical, out defTactical);
        }

        for(int i = 0; i < PlayerList.Count; i++)
        {
            if(PlayerList[i].Team == team)
            {
                if (PlayerList[i] == pickBallPlayer) 
                    DoPickBall(PlayerList[i]);
                else 
                    InboundsBall(PlayerList[i], team, ref attackTactical);
            }
            else 
                BackToDef(PlayerList[i], ETeamKind.Npc, ref defTactical);
        }
    }

    private void SituationInbounds(ETeamKind team)
    {
		if(PlayerList.Count > 0 && BallOwner)
		{
		    if(GameStart.Get.CourtMode == ECourtMode.Full)
            {
                AITools.RandomCorrespondingTactical(ETactical.Inbounds, ETactical.InboundsDefence,
                                    BallOwner.Index, out attackTactical, out defTactical);
            }
            else
            {
                AITools.RandomCorrespondingTactical(ETactical.HalfInbounds, ETactical.HalfInboundsDefence,
                    BallOwner.Index, out attackTactical, out defTactical);
            }

//		    Debug.LogFormat("Attack:{0}, Defence:{1}", attackTactical, defTactical);

		    foreach(PlayerBehaviour someone in PlayerList)
		    {
		        if(someone.Team == team)
		        {
		            if(!IsPassing)
		                InboundsBall(someone, team, ref attackTactical);
		        }
		        else
		            BackToDef(someone, someone.Team, ref defTactical);
		    }
		}
    }

//	public void AIFakeShoot([NotNull] PlayerBehaviour player)
//	{
//		bool isDoShooting = true;
//		
//		if(player.IsRebound || player.IsUseSkill)
//            isDoShooting = false;
//		else if(!player.CheckAnimatorSate(EPlayerState.HoldBall) && HasDefPlayer(player, 5, 40) != 0)
//        {
//            // 判斷是否要做投籃假動作.
//			int fakeRate = Random.Range(0, 100);
//			
//			if(fakeRate < GameConst.FakeShootRate && PlayerList.Count > 1)
//            {
//				for(int i = 0; i < PlayerList.Count; i++)
//                {
//					PlayerBehaviour npc = PlayerList[i];
//
//                    float dis = AITools.Find2DDis(player.transform.position, npc.transform.position);
//						
//                    // 有靠近我的對手時(可以蓋我火鍋的距離內), 我才可能做投籃假動作.
//					if(npc != player && npc.Team != player.Team && 
////                           GetDis(player, npc) <= GameConst.BlockDistance)
//                        dis <= GameConst.BlockDistance)
//                    {
//						player.AniState(EPlayerState.FakeShoot, CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position);
//						isDoShooting = false;
//						break;
//					}
//				}
//			}
//		}
//		
//		if(isDoShooting)
//			DoShoot();
//		else
//			coolDownPass = 0;
//	}

    /// <summary>
    /// 不見得真的會傳球.
    /// </summary>
    /// <param name="player"></param>
	public void AIPass([NotNull]PlayerBehaviour player)
    {
		float angle = 90;
		if ((player.Team == ETeamKind.Self && player.transform.position.z > 0) ||
		    (player.Team == ETeamKind.Npc && player.transform.position.z < 0))
			angle = 180;

		PlayerBehaviour teammate = findTeammate(player, 20, angle);
		
		if(teammate != null)
			Pass(teammate);
		else
        {
			int who = Random.Range(0, 2);
			int find = 0;
			
			for(int i = 0; i < PlayerList.Count; i++)
            {
				if (PlayerList [i].gameObject.activeInHierarchy && 
                    PlayerList[i].Team == player.Team && PlayerList[i] != player)
                {
					PlayerBehaviour npc = PlayerList[i];
					
					if(HasDefPlayer(npc, 1.5f, 40) == 0 || who == find)
                    {
						Pass(PlayerList [i]);
						break;
					}
					
					find++;
				}
			}
		}
	}
	
    
//	public void AIAttack([NotNull]PlayerBehaviour player)
//    {
//	    if(!BallOwner)
//            return;
//
//	    bool dunkRate = Random.Range(0, 100) < 30;
//	    bool shootRate = Random.Range(0, 100) < 10;
//	    bool shoot3Rate = Random.Range(0, 100) < 1;
//	    bool passRate = Random.Range(0, 100) < 20;
//	    
//	    bool elbowRate = Random.Range(0, 100) < player.Attr.ElbowingRate;
//			
//	    if(player == BallOwner)
//	    {
//	        Vector3 pos = CourtMgr.Get.ShootPoint[player.Team.GetHashCode()].transform.position;
//	        float shootPointDis = GetDis(player, new Vector2(pos.x, pos.z));
//
//	        if(shootPointDis <= GameConst.DunkDistance && 
//	           (dunkRate || player.CheckAnimatorSate(EPlayerState.HoldBall)) && 
//	           IsInUpfield(player))
//	            AIFakeShoot(player);
//	        else if(shootPointDis <= GameConst.TwoPointDistance && 
//	                (HasDefPlayer(player.DefPlayer, 1.5f, 40) == 0 || shootRate || player.CheckAnimatorSate(EPlayerState.HoldBall)) && 
//	                IsInUpfield(player))
//	            AIFakeShoot(player);
//	        else if(shootPointDis <= GameConst.TreePointDistance + 1 && 
//	                (HasDefPlayer(player.DefPlayer, 3.5f, 40) == 0 || shoot3Rate || player.CheckAnimatorSate(EPlayerState.HoldBall)) && 
//	                IsInUpfield(player))
//	            AIFakeShoot(player);
//	        else
//	        {
//	            // 做 Elbow 攻擊對方.
//	            PlayerBehaviour man = null;
//	            if(elbowRate && IsInUpfield(player) && (HaveDefPlayer(player, GameConst.StealBallDistance, 90, out man) != 0) && 
//	               player.CoolDownElbow ==0 && !player.CheckAnimatorSate(EPlayerState.Elbow))
//	            {
//	                if (player.DoPassiveSkill(ESkillSituation.Elbow, man.transform.position))
//	                {
//	                    coolDownPass = 0;
//	                    player.CoolDownElbow = Time.time + 3;
//	                    RealBallFxTime = 1f;
//	                    CourtMgr.Get.RealBallFX.SetActive(true);
//	                }
//	            }
//	            else if((passRate || player.CheckAnimatorSate(EPlayerState.HoldBall)) && coolDownPass == 0 && !IsShooting && !IsDunk && 
//	                    !player.CheckAnimatorSate(EPlayerState.Elbow) && BallOwner.AIing)
//	                AIPass(player);
//	            else if(player.IsHaveMoveDodge && CoolDownCrossover == 0 && player.CanMove)
//	            {
//	                if(Random.Range(0, 100) <= player.MoveDodgeRate)
//	                    player.DoPassiveSkill(ESkillSituation.MoveDodge);
//	            }
//	        }
//	    }
//	    else
//	    {
//	        // 參數 player 並未持球, 所以只能做 Push 被動技.
//	        PlayerBehaviour nearPlayer = hasNearPlayer(player, GameConst.StealBallDistance, false);
//            bool pushRate = Random.Range(0, 100) < player.Attr.PushingRate;
//            if(nearPlayer && pushRate && player.CoolDownPush == 0)
//	        { 
//	            if(player.DoPassiveSkill(ESkillSituation.Push0, nearPlayer.transform.position))
//	                player.CoolDownPush = Time.time + 3;                    
//	        } 
//	    }
//    }

	public void AIDefend([NotNull] PlayerBehaviour player)
	{
		if (player.AIing && !player.IsSteal && !player.CheckAnimatorSate(EPlayerState.Push0) && 
		    BallOwner && !IsDunk && !IsShooting) {
			bool pushRate = Random.Range(0, 100) < player.Attr.PushingRate;        
			bool sucess = false;

			TPlayerDisData [] disAy = GetPlayerDisAy(player);
			
			for (int i = 0; i < disAy.Length; i++) {
				if (disAy[i].Distance <= GameConst.StealBallDistance && 
				    (disAy[i].Player.crtState == EPlayerState.Idle && disAy[i].Player.crtState == EPlayerState.Dribble0) && 
				    pushRate && player.CoolDownPush == 0) {
					if(player.DoPassiveSkill(ESkillSituation.Push0, disAy[i].Player.transform.position)) {
						player.CoolDownPush = Time.time + GameConst.CoolDownPushTime;
						sucess = true;
						
						break;
					}
				} 
			}
			
			if (!sucess && disAy[0].Distance <= GameConst.StealBallDistance && waitStealTime == 0 && BallOwner.Invincible == 0 && player.CoolDownSteal == 0) {
				if(Random.Range(0, 100) < player.Attr.StealRate) {
					if(player.DoPassiveSkill(ESkillSituation.Steal0, BallOwner.gameObject.transform.position)) {
						player.CoolDownSteal = Time.time + GameConst.CoolDownSteal;                              
						waitStealTime = Time.time + GameConst.WaitStealTime;
					}
				}
			}           
		}
	}

//	public void AIMove([NotNull] PlayerBehaviour someone, ref TTacticalData tacticalData)
//    {
//		if(BallOwner == null)
//        {
//            // 沒人有球, 所以要開始搶球. 最接近球的球員去撿球就好.
//			if(!Passer)
//            {
//				if(Shooter == null)
//				{
//				    NearestBallPlayerDoPickBall(someone);
//                    if(someone.DefPlayer != null)
//                        NearestBallPlayerDoPickBall(someone.DefPlayer);
//                }
//				else
//                {
//                    // 投籃者出手, 此時球在空中飛行.
//                    if((Situation == EGameSituation.AttackA && someone.Team == ETeamKind.Self) ||
//                       (Situation == EGameSituation.AttackB && someone.Team == ETeamKind.Npc))
//                    {
//                        if(!someone.IsShoot)
//                            NearestBallPlayerDoPickBall(someone);
//                        if(someone.DefPlayer != null)
//                            NearestBallPlayerDoPickBall(someone.DefPlayer);
//                    }
//				}
//			}
//		}
//        else
//        {
//			if(someone.CanMove && someone.TargetPosNum == 0)
//            {
//				for(int i = 0; i < PlayerList.Count; i++)
//                {
//					if(PlayerList[i].Team == someone.Team && PlayerList[i] != someone && 
//					   tacticalData.FileName != string.Empty && PlayerList[i].TargetPosName != tacticalData.FileName)
//						PlayerList[i].ResetMove();
//		        }
//				
//				if(tacticalData.FileName != string.Empty)
//                {
//					tacticalActions = tacticalData.GetActions(someone.Postion.GetHashCode());
//					
//					if (tacticalActions != null)
//                    {
//						for (int i = 0; i < tacticalActions.Length; i++)
//                        {
//                            moveData.Clear();
//							moveData.Speedup = tacticalActions [i].Speedup;
//							moveData.Catcher = tacticalActions [i].Catcher;
//							moveData.Shooting = tacticalActions [i].Shooting;
//                            int z = 1;
//
//                            if (GameStart.Get.CourtMode == ECourtMode.Full && someone.Team != ETeamKind.Self)
//                            z = -1;
//							
//							moveData.Target = new Vector2(tacticalActions [i].x, tacticalActions [i].z * z);
//							if(BallOwner != null && BallOwner != someone)
//								moveData.LookTarget = BallOwner.transform;  
//							
//							moveData.TacticalName = tacticalData.FileName;
//							moveData.MoveFinish = DefMove;
//							someone.TargetPos = moveData;
//                        }
//						
//						DefMove(someone);
//                    }
//                }
//            }
//			
//			if (someone.WaitMoveTime != 0 && BallOwner != null && someone == BallOwner)
//				someone.AniState(EPlayerState.Dribble0);
//		}
//    }

    public bool DefMove([NotNull] PlayerBehaviour player, bool speedup = false)
	{
		if(player && player.DefPlayer && !player.CheckAnimatorSate(EPlayerState.MoveDodge1) && 
		    !player.CheckAnimatorSate(EPlayerState.MoveDodge0) && 
		    (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB))
        {
			if(player.DefPlayer.CanMove && player.DefPlayer.WaitMoveTime == 0)
            {
				if(BallOwner != null)
                {
					int index = player.DefPlayer.Postion.GetHashCode();
					moveData.Clear();
					if (player == BallOwner)
                    {
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
						float dis2;
						float z = GameStart.Get.CourtMode == ECourtMode.Full && player.DefPlayer.Team == ETeamKind.Self ? -1 : 1;
						dis2 = Vector2.Distance(new Vector2(teeBackPosAy [index].x, teeBackPosAy [index].y * z), 
						                        new Vector2(player.DefPlayer.transform.position.x, player.DefPlayer.transform.position.z));
						
						if (dis2 <= player.DefPlayer.Attr.DefDistance) {
							PlayerBehaviour p = hasNearPlayer(player.DefPlayer, player.DefPlayer.Attr.DefDistance, false, true);
							if (p != null)
								moveData.DefPlayer = p;
							else 
								if (GetDis(player, player.DefPlayer) <= player.DefPlayer.Attr.DefDistance)
									moveData.DefPlayer = player;
							
							if (moveData.DefPlayer != null) {
								if (BallOwner != null)
									moveData.LookTarget = BallOwner.transform;
								else
									moveData.LookTarget = player.transform;
								
								moveData.Speedup = speedup;
								player.DefPlayer.TargetPos = moveData;
							} else {
								player.DefPlayer.ResetMove();
								z = GameStart.Get.CourtMode == ECourtMode.Full && player.DefPlayer.Team == ETeamKind.Self ? -1 : 1;
                                moveData.Target = new Vector2(teeBackPosAy [index].x, teeBackPosAy [index].y * z);
                                
                                if (BallOwner != null)
                                    moveData.LookTarget = BallOwner.transform;
                                else {
                                    if (player.Team == ETeamKind.Self)
                                        moveData.LookTarget = CourtMgr.Get.Hood [1].transform;
                                    else
                                        moveData.LookTarget = CourtMgr.Get.Hood [0].transform;
                                }                                   
                                
                                player.DefPlayer.TargetPos = moveData;
                            }
                        }
                        else
                        {
                            player.DefPlayer.ResetMove();
                            z = GameStart.Get.CourtMode == ECourtMode.Full && player.DefPlayer.Team == ETeamKind.Self ? -1 : 1;
                            moveData.Target = new Vector2(teeBackPosAy [index].x, teeBackPosAy [index].y * z);
                            
                            if (BallOwner != null)
                                moveData.LookTarget = BallOwner.transform;
                            else {
                                if (player.Team == ETeamKind.Self)
                                    moveData.LookTarget = CourtMgr.Get.Hood [1].transform;
                                else
                                    moveData.LookTarget = CourtMgr.Get.Hood [0].transform;
                            }                                   
                            
                            player.DefPlayer.TargetPos = moveData;                         
                        }
                    }
                }
                else
                {
                    player.DefPlayer.ResetMove();

                    if(player.DefPlayer)
                        NearestBallPlayerDoPickBall(player.DefPlayer);
                }
            }
        }
        
        return true;
    }
    
    public void ChangeSituation(EGameSituation newSituation, PlayerBehaviour player = null)
    {
		if(Situation != EGameSituation.End || newSituation == EGameSituation.None || 
           newSituation == EGameSituation.Opening)
        {
            EGameSituation oldgs = Situation;
            if(Situation != newSituation)
            {
                RealBallFxTime = 0;
                waitStealTime = 0;
                CourtMgr.Get.RealBallFX.SetActive(false);
                for(int i = 0; i < PlayerList.Count; i++)
                {
                    if(newSituation == EGameSituation.APickBallAfterScore || 
                       newSituation == EGameSituation.BPickBallAfterScore)
                    {
                        PlayerList[i].SetToAI();
                        PlayerList[i].ResetMove();
                    }												
                    
                    switch(PlayerList[i].Team)
                    {
                        case ETeamKind.Self:
                            if((newSituation == EGameSituation.InboundsB || (oldgs == EGameSituation.InboundsB && newSituation == EGameSituation.AttackB)) == false)
                            {
                                if(!PlayerList[i].AIing)
                                {
                                    if(!(newSituation == EGameSituation.AttackA || newSituation == EGameSituation.AttackB))
                                        PlayerList[i].ResetFlag();
                                } else
                                    PlayerList[i].ResetFlag();
						    }

						break;
					case ETeamKind.Npc:
						if((newSituation == EGameSituation.InboundsA || (oldgs == EGameSituation.InboundsA && newSituation == EGameSituation.AttackA)) == false)
							PlayerList[i].ResetFlag();

						break;
					}

                    PlayerList [i].situation = newSituation;
                }
            }

            Situation = newSituation;

            if(GameStart.Get.CourtMode == ECourtMode.Full && oldgs != newSituation && player &&
               (oldgs == EGameSituation.InboundsA || oldgs == EGameSituation.InboundsB)) {
                AITools.RandomTactical(ETactical.Fast, player.Index, out attackTactical);
                
				if(attackTactical.FileName != string.Empty)
                {
					for (int i = 0; i < PlayerList.Count; i ++)
                    {
						PlayerBehaviour npc = PlayerList [i];
						if (npc.Team == player.Team)
                        {
							tacticalActions = attackTactical.GetActions(npc.Index);
							
							if (tacticalActions != null) {
								for (int j = 0; j < tacticalActions.Length; j++) {
									moveData.Clear();
									moveData.Speedup = tacticalActions [j].Speedup;
									moveData.Catcher = tacticalActions [j].Catcher;
									moveData.Shooting = tacticalActions [j].Shooting;
									moveData.TacticalName = attackTactical.FileName;
									if(npc.Team == ETeamKind.Self)
										moveData.Target = new Vector2(tacticalActions [j].x, tacticalActions [j].z);
									else
										moveData.Target = new Vector2(tacticalActions [j].x, -tacticalActions [j].z);

									if (BallOwner != null && BallOwner != npc)
										moveData.LookTarget = BallOwner.transform;  
									
									npc.TargetPos = moveData;
								}
							}
						}
					}
				}               
			}

			CourtMgr.Get.Walls[0].SetActive(true);
			CourtMgr.Get.Walls[1].SetActive(true);

			switch(newSituation)
            {
			case EGameSituation.Presentation:
			case EGameSituation.CameraMovement:
			case EGameSituation.InitCourt:
				break;
			case EGameSituation.None:
				UIGame.UIShow(true);
				UIGame.UIShow(false);
				UIGameResult.UIShow(true);
				UIGameResult.UIShow(false);
				UIDoubleClick.UIShow(true);
				for(int i = 0; i < PlayerList.Count; i++)
					UIDoubleClick.Get.InitDoubleClick(PlayerList[i], i);
				UIPassiveEffect.UIShow(true);
				UITransition.UIShow(true);
				break;
			case EGameSituation.Opening:
//				setPassIcon(true);
//				UIGame.UIShow (true);
//				UIGame.Get.UIState(EUISituation.Opening);
				judgeSkillUI();

				break;
			case EGameSituation.JumpBall:
//				IsStart = true;
//				CourtMgr.Get.InitScoreboard (true);
//				setPassIcon(true);
//
//				PlayerBehaviour npc = findJumpBallPlayer(ETeamKind.Self);
//				if (npc) {
////					npc.transform.position = bornPosAy[1];
//					JumpBall(npc);
////					Rebound(npc);
//				}
//
//				npc = findJumpBallPlayer(ETeamKind.Npc);
//				if (npc) {
////					npc.transform.position = bornPosAy[1];
//					JumpBall(npc);
////					Rebound(npc);
//				}

				break;
			case EGameSituation.AttackA:
				judgeSkillUI();
				break;
			case EGameSituation.AttackB:
				judgeSkillUI ();
				break;
			case EGameSituation.APickBallAfterScore:
//				CourtMgr.Get.Walls[1].SetActive(false);
//				UIGame.Get.ChangeControl(true);
//				CameraMgr.Get.SetCameraSituation(ECameraSituation.Self, true);
				pickBallPlayer = null;

                break;
            case EGameSituation.InboundsA:
				CourtMgr.Get.Walls[1].SetActive(false);
				EffectManager.Get.PlayEffect("ThrowInLineEffect", Vector3.zero);
				UITransition.Get.SelfAttack();
                break;

            case EGameSituation.BPickBallAfterScore:
//				CourtMgr.Get.Walls[0].SetActive(false);
//         	 	UIGame.Get.ChangeControl(false);
//				CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc, true);
				pickBallPlayer = null;

                break;
			case EGameSituation.InboundsB:
				CourtMgr.Get.Walls[0].SetActive(false);
				EffectManager.Get.PlayEffect("ThrowInLineEffect", Vector3.zero);
				UITransition.Get.SelfOffense();
				break;
			case EGameSituation.End:
				IsStart = false;
				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].AniState(EPlayerState.Idle);			

				CameraMgr.Get.SetCameraSituation(ECameraSituation.Finish);
            	break;
            }       
        }
    }
    
    private void handleSituation()
    {
        if (PlayerList.Count > 0)
        {
            //Action
			if(GameStart.Get.TestMode == EGameTest.All || GameStart.Get.TestMode == EGameTest.None) {

				if (Situation != EGameSituation.None && Situation != EGameSituation.Opening)
					GameRecord.GameTime += Time.deltaTime;
            
	            switch(Situation)
	            {
					case EGameSituation.Presentation:
					case EGameSituation.CameraMovement:
					case EGameSituation.InitCourt:
	                case EGameSituation.None:
	                case EGameSituation.Opening:
	                case EGameSituation.JumpBall:
//						jumpBall();
	                    break;
	                case EGameSituation.AttackA:
	                case EGameSituation.AttackB:
	                    break;
	                case EGameSituation.APickBallAfterScore:
	                    SituationPickBall(ETeamKind.Self);
	                    break;
	                case EGameSituation.InboundsA:
	                    SituationInbounds(ETeamKind.Self);
	                    break;
	                case EGameSituation.BPickBallAfterScore:
	                    SituationPickBall(ETeamKind.Npc);
	                    break;
	                case EGameSituation.InboundsB:
	                    SituationInbounds(ETeamKind.Npc);
	                    break;
	                case EGameSituation.End:
	                    break;
	            }
			}
        }
    }

	private void judgeSkillUI()
    {
		if(Joysticker && Joysticker.Attribute.ActiveSkill.ID > 0 && IsStart)
        {
			CourtMgr.Get.SkillAera((int)Joysticker.Team, Joysticker.IsAngerFull);
			UIGame.Get.ShowSkillUI(IsStart, Joysticker.IsAngerFull, Joysticker.CheckSkill);
		}
	}
	
	private EBasketDistanceAngle judgeShootAngle(PlayerBehaviour player){
		float angle = 0;
		int distanceType = 0;
		if(player.name.Contains("Self")) {
			angleByPlayerHoop = MathUtils.GetAngle(CourtMgr.Get.Hood[0].transform, player.gameObject.transform);
			angle = Mathf.Abs(angleByPlayerHoop) - 90;
		} else {
			angleByPlayerHoop = MathUtils.GetAngle(CourtMgr.Get.Hood[1].transform, player.gameObject.transform);
			angle = Mathf.Abs(angleByPlayerHoop) - 90;
		}
		//Distance
		if(ShootDistance >= 0 && ShootDistance < 9) {
			distanceType = 0;
		} else 
		if(ShootDistance >= 9 && ShootDistance < 12) {
			distanceType = 1;
		} else 
		if(ShootDistance >= 12) {
			distanceType = 2;
		}
		//Angle
		if(angle > 60) {// > 60 degree
			if(distanceType == 0){
				return EBasketDistanceAngle.ShortCenter;
			}else if (distanceType == 1){
				return EBasketDistanceAngle.MediumCenter;
			}else if (distanceType == 2){
				return EBasketDistanceAngle.LongCenter;
			}
		} else 
		if(angle <= 60 && angle > 10){// > 10 degree <= 60 degree
			if(angleByPlayerHoop > 0) {//right
				if(distanceType == 0){
					return EBasketDistanceAngle.ShortRight;
				}else if (distanceType == 1){
					return EBasketDistanceAngle.MediumRight;
				}else if (distanceType == 2){
					return EBasketDistanceAngle.LongRight;
				}
			} else {//left
				if(distanceType == 0){
					return EBasketDistanceAngle.ShortLeft;
				}else if (distanceType == 1){
					return EBasketDistanceAngle.MediumLeft;
				}else if (distanceType == 2){
					return EBasketDistanceAngle.LongLeft;
				}
			}
		} else 
		if(angle <= 10 && angle >= -30){ // < 10 degree
			if(angleByPlayerHoop > 0) { // right
				if(distanceType == 0){
					return EBasketDistanceAngle.ShortRightWing;
				}else if (distanceType == 1){
					return EBasketDistanceAngle.MediumRightWing;
				}else if (distanceType == 2){
					return EBasketDistanceAngle.LongRightWing;
				}
			} else { //left
				if(distanceType == 0){
					return EBasketDistanceAngle.ShortLeftWing;
				}else if (distanceType == 1){
					return EBasketDistanceAngle.MediumLeftWing;
				}else if (distanceType == 2){
					return EBasketDistanceAngle.LongLeftWing;
				}
			}
		}
		return EBasketDistanceAngle.ShortCenter;
	}

	private void judgeBasketAnimationName (int basketDistanceAngleType) {
		int random = 0;
		if(BasketSituation == EBasketSituation.Score){
			if(CourtMgr.Get.DBasketAnimationName.Count > 0 && basketDistanceAngleType < CourtMgr.Get.DBasketAnimationName.Count){
				random = Random.Range(0, CourtMgr.Get.DBasketAnimationName[basketDistanceAngleType].Count);
				if(CourtMgr.Get.DBasketAnimationName.Count > 0 && random < CourtMgr.Get.DBasketAnimationName.Count)
					BasketAnimationName = CourtMgr.Get.DBasketAnimationName[basketDistanceAngleType][random];
			}
		}else if(BasketSituation == EBasketSituation.NoScore){
			if(CourtMgr.Get.DBasketAnimationNoneState.Count > 0 && basketDistanceAngleType < CourtMgr.Get.DBasketAnimationNoneState.Count) {
				random = Random.Range(0, CourtMgr.Get.DBasketAnimationNoneState[basketDistanceAngleType].Count);
				if(CourtMgr.Get.DBasketAnimationNoneState.Count > 0 && random < CourtMgr.Get.DBasketAnimationNoneState.Count)
					BasketAnimationName = CourtMgr.Get.DBasketAnimationNoneState[basketDistanceAngleType][random];
			}
		}

		if(BasketSituation == EBasketSituation.Score || BasketSituation == EBasketSituation.NoScore) {
			if(string.IsNullOrEmpty(BasketAnimationName))
				judgeBasketAnimationName(basketDistanceAngleType);
		}
	}

	private void calculationScoreRate(PlayerBehaviour player, EScoreType type) {
		//Score Rate
		float originalRate = 0;
		if(ShootDistance >= GameConst.TreePointDistance) {
			originalRate = player.Attr.PointRate3;
			EffectManager.Get.PlayEffect("ThreeLineEffect", Vector3.zero, null, null, 0);
		} else {
			originalRate = player.Attr.PointRate2;
		}

		
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
			isScore = (rate <= (originalRate - (originalRate * (player.ScoreRate.DownHandScoreRate / 100f)) + extraScoreRate)) ? true : false;
			downhandRate = (originalRate - (originalRate * (player.ScoreRate.DownHandScoreRate / 100f)) + extraScoreRate);
			if(isScore) {
				rate = (Random.Range(0, 100) + 1);
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.DownHandSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.DownHandAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.UpHand) {
			isScore = (rate <= (originalRate - (originalRate * (player.ScoreRate.UpHandScoreRate / 100f)) + extraScoreRate)) ? true : false;
			uphandRate = (originalRate - (originalRate * (player.ScoreRate.UpHandScoreRate / 100f)) + extraScoreRate);
			if(isScore) {
				rate = (Random.Range(0, 100) + 1);
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.UpHandSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.UpHandAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.Normal) {
			isScore = (rate <= (originalRate - (originalRate * (player.ScoreRate.NormalScoreRate / 100f)) + extraScoreRate)) ? true : false;
			normalRate = (originalRate - (originalRate * (player.ScoreRate.NormalScoreRate / 100f)) + extraScoreRate);
			if(isScore) {
				rate = (Random.Range(0, 100) + 1);
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.NormalSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.NormalAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.NearShot) {
			isScore = (rate <= (originalRate + (originalRate * (player.ScoreRate.NearShotScoreRate / 100f)) + extraScoreRate)) ? true : false;
			nearshotRate = (originalRate + (originalRate * (player.ScoreRate.NearShotScoreRate / 100f)) + extraScoreRate);
			if(isScore) {
				rate = (Random.Range(0, 100) + 1);
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.NearShotSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.NearShotAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.LayUp) {
			isScore = (rate <= (originalRate + (originalRate * (player.ScoreRate.LayUpScoreRate / 100f)) + extraScoreRate)) ? true : false;
			layupRate = (originalRate + (originalRate * (player.ScoreRate.LayUpScoreRate / 100f)) + extraScoreRate);
			if(isScore) {
				rate = (Random.Range(0, 100) + 1);
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.LayUpSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.LayUpAirBallRate ? true : false;
			}
		}
		if(DoubleClickType == EDoubleType.Perfect || ShootDistance < 9)
			isAirBall = false;

		if(DoubleClickType == EDoubleType.Weak || ShootDistance > 15) 
			isSwich = false;

		if(isScore) {
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

		
		if(GameStart.Get.TestMode == EGameTest.AttackA) {
			BasketSituation = EBasketSituation.Swish;
//			BasketSituation = EBasketSituation.Score;
//			if(BasketSituation == EBasketSituation.Score || BasketSituation == EBasketSituation.NoScore){
//				if((int)GameStart.Get.SelectBasketState > 100)
//					BasketSituation = EBasketSituation.NoScore;
//				BasketAnimationName = "BasketballAction_" + basketanimationTest[(int)GameStart.Get.SelectBasketState];
//				UIHint.Get.ShowHint("BasketAnimationName: "+BasketAnimationName, Color.yellow);
//			}
		}
		
		judgeBasketAnimationName ((int)basketDistanceAngle);

		if (ShootDistance >= GameConst.TreePointDistance)
			player.GameRecord.FG3++;
		else
			player.GameRecord.FG++;
		
		player.GameRecord.PushShot(new Vector2(player.transform.position.x, player.transform.position.z), BasketSituation.GetHashCode(), rate);
	}

	public void AddExtraScoreRate(float rate) {
		extraScoreRate = rate;
	}

    /// <summary>
    /// 做真正的投籃行為.
    /// </summary>
    /// <returns></returns>
	public bool DoShoot()
    {
        if(BallOwner)
        {
			Vector3 v = CourtMgr.Get.ShootPoint[BallOwner.Team.GetHashCode()].transform.position;
			ShootDistance = GetDis(BallOwner, new Vector2(v.x, v.z));

			if(GameStart.Get.TestMode == EGameTest.Shoot)
            {
				BallOwner.AniState(testState, CourtMgr.Get.Hood[BallOwner.Team.GetHashCode()].transform.position);
				return true;
			} 
             
			if(!BallOwner.IsDunk)
            {
				UIGame.Get.DoPassNone();
				CourtMgr.Get.ResetBasketEntra();

				int t = BallOwner.Team.GetHashCode();
				if (GameStart.Get.TestMode == EGameTest.Dunk)
                {
					BallOwner.AniState(EPlayerState.Dunk20, CourtMgr.Get.ShootPoint [t].transform.position);
					return true;
				}

                if(BallOwner.IsRebound)
                {
					if(inTipinDistance(BallOwner))
                    {
						BallOwner.AniState(EPlayerState.TipIn, CourtMgr.Get.ShootPoint [t].transform.position);
						return true;
					}
				}
                else
                {
					if(BallOwner.IsMoving)
                    {
						if (ShootDistance > 15)
							BallOwner.DoPassiveSkill(ESkillSituation.Shoot3, CourtMgr.Get.Hood [t].transform.position);
						else 
						if (ShootDistance > 9 && ShootDistance <= 15) {
							if (Random.Range(0, 2) == 0)
								BallOwner.DoPassiveSkill(ESkillSituation.Shoot2, CourtMgr.Get.Hood [t].transform.position);
							else
								BallOwner.DoPassiveSkill(ESkillSituation.Shoot0, CourtMgr.Get.Hood [t].transform.position);
						} else 
						if (ShootDistance > 7 && ShootDistance <= 9) {
							float rate = Random.Range(0, 100);
							if(rate < BallOwner.Attr.DunkRate)
								BallOwner.DoPassiveSkill(ESkillSituation.Dunk0, CourtMgr.Get.ShootPoint [t].transform.position);
							else
								BallOwner.DoPassiveSkill(ESkillSituation.Layup0, CourtMgr.Get.Hood [t].transform.position);
						} else {
							float rate = Random.Range(0, 100);
							if (rate < BallOwner.Attr.DunkRate)
								BallOwner.DoPassiveSkill(ESkillSituation.Dunk0, CourtMgr.Get.ShootPoint [t].transform.position);
							else {
								if(HasDefPlayer(BallOwner, 1.5f, 40) == 0)
									BallOwner.DoPassiveSkill(ESkillSituation.Layup0, CourtMgr.Get.Hood [t].transform.position);
								else
									BallOwner.DoPassiveSkill(ESkillSituation.Shoot1, CourtMgr.Get.Hood [t].transform.position);
							}
						}
					}
                    else
                    {
						if (ShootDistance > 15)
							BallOwner.DoPassiveSkill(ESkillSituation.Shoot3, CourtMgr.Get.Hood [t].transform.position);
						else if(ShootDistance > 9 && ShootDistance <= 15)
							BallOwner.DoPassiveSkill(ESkillSituation.Shoot0, CourtMgr.Get.Hood [t].transform.position);
						else
							BallOwner.DoPassiveSkill(ESkillSituation.Shoot1, CourtMgr.Get.Hood [t].transform.position);
					}

					return true;
				}
			}
        }

		return false;
	}
        
    /// <summary>
    /// 呼叫時機: 撥投籃動作時, 由 Event 觸發.(通常會是投籃動作的中後段才會發出這個 event)
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool OnShooting([NotNull]PlayerBehaviour player)
    {
        if (BallOwner && BallOwner == player)
		{     
			CourtMgr.Get.RealBallTrigger.IsAutoRotate = true;
			Shooter = player;
			SetBallOwnerNull();
			UIGame.Get.SetPassButton();

			EScoreType scoreType = EScoreType.Normal;
			if(player.Team == ETeamKind.Self) 
				angleByPlayerHoop = MathUtils.GetAngle(CourtMgr.Get.Hood[0].transform, player.gameObject.transform);
			else 
				angleByPlayerHoop = MathUtils.GetAngle(CourtMgr.Get.Hood[1].transform, player.gameObject.transform);

			if(Mathf.Abs(angleByPlayerHoop) <= 85  && ShootDistance < 5)
				shootAngle = 80;
			else
				shootAngle = 50;

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

			basketDistanceAngle = judgeShootAngle(player);
			calculationScoreRate(player, scoreType);

			SetBall();
			CourtMgr.Get.SetBallState(player.crtState);

			if(BasketSituation == EBasketSituation.AirBall) {
				//AirBall
//				#if UNITY_EDITOR
//				UIHint.Get.ShowHint("AirBall", Color.yellow);
//				#endif
				Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Ignore Raycast"), LayerMask.NameToLayer ("RealBall"), true);
				CourtMgr.Get.RealBallVelocity = GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
				                                                         CourtMgr.Get.BasketAirBall[player.Team.GetHashCode()].transform.position, shootAngle);

			} else 
			if(player.crtState == EPlayerState.TipIn) {
				if(BasketSituation == EBasketSituation.Swish) {
					if(CourtMgr.Get.RealBall.transform.position.y > (CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.y + 0.2f)) {
						
						CourtMgr.Get.RealBallDoMove(new Vector3(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.x,
						                                        CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.y + 0.5f,
						                                        CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.z), 1 / TimerMgr.Get.CrtTime * 0.5f);
					} else {
						CourtMgr.Get.RealBallDoMove(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position, 1/ TimerMgr.Get.CrtTime * 0.2f); //0.2f	
					}
				} else {
					if(CourtMgr.Get.RealBall.transform.position.y > (CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].y + 0.2f)) {
						
						CourtMgr.Get.RealBallDoMove(new Vector3(CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].x,
						                                        CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].y + 0.5f,
						                                        CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].z), 1 / TimerMgr.Get.CrtTime * 0.5f);
					} else
						CourtMgr.Get.RealBallDoMove(CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName], 1/ TimerMgr.Get.CrtTime * 0.2f); //0.2f	
				}
			}else 
			if(BasketSituation == EBasketSituation.Swish) {
//				#if UNITY_EDITOR
//				UIHint.Get.ShowHint("Swish", Color.yellow);
//				#endif
				Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("BasketCollider"), LayerMask.NameToLayer ("RealBall"), true);
				if(player.GetSkillKind == ESkillKind.LayupSpecial) {
					CourtMgr.Get.RealBallDoMove(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position, 1/ TimerMgr.Get.CrtTime * 0.4f); //0.2
				} else if(player.Attribute.BodyType == 0 && ShootDistance < 5) {
					CourtMgr.Get.RealBallVelocity = GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
					                                                         CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position , shootAngle, 1f);
				} else 
					CourtMgr.Get.RealBallVelocity = GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
				    	                                                     CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position , shootAngle);	
			} else {
				if(CourtMgr.Get.DBasketShootWorldPosition.ContainsKey (player.Team.GetHashCode().ToString() + "_" + BasketAnimationName)) {
					if(player.GetSkillKind == ESkillKind.LayupSpecial) {
						CourtMgr.Get.RealBallDoMove(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position, 1/ TimerMgr.Get.CrtTime * 0.4f); //0.2
					} else if(player.Attribute.BodyType == 0 && ShootDistance < 5) {
						CourtMgr.Get.RealBallVelocity = GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
						                                                         CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName] , shootAngle, 1f);
					}  else {
						float dis = GetDis(new Vector2(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z),
						                   new Vector2(CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].x, CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].z));
						if(dis>10)
							dis = 10;
						CourtMgr.Get.RealBallVelocity = GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
						                                                         CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName],
						                                                         shootAngle,
						                                                         dis * 0.05f);
					}

				} else 
					Debug.LogError("No key:"+player.Team.GetHashCode().ToString() + "_" + BasketAnimationName);
			}

            for (int i = 0; i < PlayerList.Count; i++)
				if(Shooter != null) {
					if (PlayerList [i].Team == Shooter.Team)
						PlayerList [i].ResetMove();
				}
			return true;
        } else
            return false;
    }

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

	public bool DoShoot(bool isshoot)
    {
		if (IsStart && CandoBtn) {
			int index = GetShootPlayerIndex();
			if(index >= 0 && UIDoubleClick.Get.DoubleClicks[index].Enable){

				UIDoubleClick.Get.ClickStop (index);
				switch (UIDoubleClick.Get.Lv) {
				case 0:
					GameRecord.DoubleClickLv1++;
					break;
				case 1:
					GameRecord.DoubleClickLv2++;
					break;
				case 2:
					GameRecord.DoubleClickLv3++;
					break;
                }
			} else
            if (Joysticker == BallOwner) {
				if (isshoot)
					return DoShoot ();
				else
					return Joysticker.AniState(EPlayerState.FakeShoot, CourtMgr.Get.ShootPoint [Joysticker.Team.GetHashCode()].transform.position);
            } else //someone else shot
			if (BallOwner && BallOwner.Team == ETeamKind.Self) {
				DoShoot();
			} else 
			if (!Joysticker.IsRebound && IsReboundTime)
				return Rebound(Joysticker);
        }

		return false;
    }

	public bool DoPush(PlayerBehaviour nearP)
	{
		if (Joysticker) {
//			PlayerBehaviour nearP = FindNearNpc();
//			if(nearP)
				return Joysticker.DoPassiveSkill (ESkillSituation.Push0, nearP.transform.position);
//			else
//				return Joysticker.DoPassiveSkill (ESkillSituation.Push0);
		} else
			return false;
	}

	public bool DoElbow()
	{
		if (Joysticker)
			return Joysticker.DoPassiveSkill (ESkillSituation.Elbow);
		else
			return false;
	}

	public bool OnOnlyScore(PlayerBehaviour player) {
		if (player == BallOwner)
		{
			PlusScore(player.Team.GetHashCode(), true, false);
			GameController.Get.ShowWord(EShowWordType.Dunk, player.Team.GetHashCode());
			
			player.GameRecord.Dunk++;
			if (ShootDistance >= GameConst.TreePointDistance)
				player.GameRecord.FG3++;
			else
				player.GameRecord.FG++;

			return true;
		} else
			return false;
	}

    public bool OnDunkBasket(PlayerBehaviour player)
    {
        if (player == BallOwner)
        {
			if (player.crtState == EPlayerState.Alleyoop)
				player.GameRecord.Alleyoop++;
			else
				GameController.Get.ShowWord(EShowWordType.Dunk, player.Team.GetHashCode());

            CourtMgr.Get.SetBallState(EPlayerState.DunkBasket);
			if(GameStart.Get.TestMode == EGameTest.Alleyoop) 
				UIHint.Get.ShowHint("Alleyoop Score.", Color.yellow);
			PlusScore(player.Team.GetHashCode(), player.IsUseSkill, true);
            SetBall();

			player.GameRecord.ShotError++;
			player.GameRecord.Dunk++;
			if (ShootDistance >= GameConst.TreePointDistance)
				player.GameRecord.FG3++;
			else
				player.GameRecord.FG++;

            return true;
        } else
            return false;
    }

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
    
    public bool Pass(PlayerBehaviour player, bool isTee = false, bool isBtn = false, bool movePass = false) {
		bool result = false;
		bool canPass = true;

		if(BallOwner) {
			#if UNITY_EDITOR
			if(GameStart.Get.TestMode == EGameTest.Pass) {
				if(BallOwner.IsMoving) {
					float angle = MathUtils.GetAngle(BallOwner.gameObject.transform, player.gameObject.transform);
					if (angle < 60f && angle > -60f){
						UIHint.Get.ShowHint("Direct Forward and Angle:" + angle, Color.yellow);
						result = BallOwner.AniState(EPlayerState.Pass5);
					} else 
					if (angle <= -60f && angle > -120f){
						UIHint.Get.ShowHint("Direct Left and Angle:" + angle, Color.yellow);
						result = BallOwner.AniState(EPlayerState.Pass7);
					} else 
					if (angle < 120f && angle >= 60f){
						UIHint.Get.ShowHint("Direct Right and Angle:" + angle, Color.yellow);
						result = BallOwner.AniState(EPlayerState.Pass8);
					} else 
					if (angle >= 120f || angle <= -120f){
						UIHint.Get.ShowHint("Direct Back and Angle:" + angle, Color.yellow);
						if (Random.Range(0, 100) < 50)
							result = BallOwner.AniState(EPlayerState.Pass9);
						else 
							result = BallOwner.AniState(EPlayerState.Pass6);
					}
				} else 
					result = BallOwner.AniState(EPlayerState.Pass0, player.gameObject.transform.position);
				
				if(result){
					Catcher = player;
					UIGame.Get.DoPassNone();
				}
				
				return result;
			}
			#endif

			if(IsShooting)
			{
				if(player.Team == ETeamKind.Self)
				{
					if(!isBtn)
						canPass = false;
					else 
					if(!IsCanPassAir)
						canPass = false;
				}
				else if(player.Team == ETeamKind.Npc && !IsCanPassAir)
					canPass = false;
			}

			if (!IsPassing && canPass && !IsDunk && player != BallOwner)
			{
				if(!(isBtn || movePass) && coolDownPass != 0)
					return result;
				
				if(!isBtn && !BallOwner.AIing)
					return result;
				
				if(isTee)
				{
					if(BallOwner.AniState(EPlayerState.Pass50, player.transform.position))
					{
						Catcher = player;
						result = true;
					}												
				}else if(IsCanPassAir && !isTee)
				{
					if(BallOwner.AniState(EPlayerState.Pass4, player.transform.position))
					{
						Catcher = player;
						result = true;
					}
				}
				else
				{
					float dis = Vector3.Distance(BallOwner.transform.position, player.transform.position);
					int disKind = GetEnemyDis(ref player);
					int rate = UnityEngine.Random.Range(0, 2);
					if(player.crtState == EPlayerState.Alleyoop) {
						IsCatcherAlleyoop = true;
						result = BallOwner.AniState(EPlayerState.Pass0, player.transform.position);
					} else
					if(dis <= GameConst.FastPassDistance)
					{
						result = BallOwner.DoPassiveSkill(ESkillSituation.Pass4, player.transform.position);
					}
					else if(dis <= GameConst.CloseDistance)
					{
						//Close
						if(disKind == 1)
						{
							if(rate == 1){
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass1, player.transform.position);
							}else{ 
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass2, player.transform.position);
							}
						} else 
						if(disKind == 2)
						{
							if(rate == 1){
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass0, player.transform.position);
							}else{
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass2, player.transform.position);
							}
						}						
						else
						{
							if(rate == 1){
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass0, player.transform.position);
							}else{
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass2, player.transform.position);
							}
						}
					}else if(dis <= GameConst.MiddleDistance)
					{
						//Middle
						if(disKind == 1)
						{
							if(rate == 1){
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass0, player.transform.position);
							}else{
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass2, player.transform.position);
							}
						} else 
							if(disKind == 2)
						{
							if(rate == 1){
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass1, player.transform.position);
							}else{
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass2, player.transform.position);
							}
						}						
						else
						{
							if(rate == 1){
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass0, player.transform.position);
							}else{
								result = BallOwner.DoPassiveSkill(ESkillSituation.Pass2, player.transform.position);
							}
						}
					}else{
						//Far
						result = BallOwner.DoPassiveSkill(ESkillSituation.Pass1, player.transform.position);
					}
					
					if(result){
						Catcher = player;
						if (BallOwner && (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)) 
							BallOwner.GameRecord.Pass++;
						
						UIGame.Get.DoPassNone();
					}
				}
			}
		}

		return result;
    }

	public void PlayShowAni(int playIndex, string aniName)
	{
		for (int i = 0; i < PlayerList.Count; i++) {
			if (IsShowSituation && PlayerList [i].ShowPos == playIndex)
				PlayerList [i].AnimatorControl.SetTrigger (aniName);
		}
	}

	public bool OnFall(PlayerBehaviour faller)
	{
		UIGame.Get.UICantUse(faller);
		if (faller && BallOwner == faller) {
			setDropBall ();
			return true;
		}

		return false;
	}

	public int GetEnemyDis(ref PlayerBehaviour npc) {
		float [] DisAy = new float[3];
		int Index = 0;
		for (int i = 0; i < PlayerList.Count; i++) {
			if (PlayerList[i].Team != npc.Team) {
				DisAy[Index] = Vector3.Distance(npc.transform.position, PlayerList[i].transform.position);
				Index++;
			}		
		}

		for (int i = 0; i < DisAy.Length; i++) {
			if (DisAy[i] > 0) {
				if (DisAy[i] <= GameConst.StealBallDistance)
					return 2;
				else 
				if (DisAy[i] <= GameConst.DefDistance)
					return 1;
			}
		}

		return 0;
	}
    
    public bool DoPass(int playerid) {
		if (IsStart && BallOwner && Joysticker && BallOwner.Team == 0 && CandoBtn && 
		    playerid < PlayerList.Count && (!Shooter || IsCanPassAir)) {
        	return Pass(PlayerList [playerid], false, true);
        }

		return false;
    }

    private void Steal(PlayerBehaviour player) {
        
    }
	
	public bool OnStealMoment(PlayerBehaviour player) {
        if (BallOwner && BallOwner.Invincible == 0 && !IsShooting && !IsDunk) {
			if(Vector3.Distance(BallOwner.transform.position, player.transform.position) <= GameConst.StealBallDistance && GameFunction.IsTouchPlayerArea(player.transform, BallOwner.transform.position, 30)) {
				int r = Mathf.RoundToInt(player.Attribute.Steal - BallOwner.Attribute.Dribble);
				int maxRate = 100;
				int minRate = 10;
				
				if (r > maxRate)
					r = maxRate;
				else 
				if (r < minRate)
					r = minRate;
				
				int stealRate = Random.Range(0, 100) + 1;
				int AddRate = 0;
				int AddAngle = 0;
				if(CourtMgr.Get.RealBallFX.activeInHierarchy)
					AddRate = 30;

				if(Vector3.Distance(BallOwner.transform.position, CourtMgr.Get.Hood[BallOwner.Team.GetHashCode()].transform.position) <= GameConst.DunkDistance){
					AddRate += 40;
					AddAngle = 90;
				}
				
				if (stealRate <= (r + AddRate) && Mathf.Abs(MathUtils.GetAngle(BallOwner.transform, player.transform)) <= 90 + AddAngle) {
					if(BallOwner && BallOwner.AniState(EPlayerState.GotSteal)) {
						BallOwner.SetAnger(GameConst.DelAnger_Stealed);
						if(player == Joysticker || BallOwner == Joysticker)
							ShowWord(EShowWordType.Steal, 0, player.ShowWord);
						return true;
					}
				} else 
				if(BallOwner != null && haveStealPlayer(player, BallOwner, GameConst.StealBallDistance, 15) != 0) {
					stealRate = Random.Range(0, 100) + 1;
					
					if(stealRate <= r) {
						RealBallFxTime = 1f;
						CourtMgr.Get.RealBallFX.SetActive(true);
					}
				}
			}
        }
        
        return false;
    }

	public bool OnGotSteal(PlayerBehaviour player)
	{
		if (BallOwner == player) {
			setDropBall (player);
			return true;
		} else
			return false;
	}

    public bool DoSteal()
    {
		if (StealBtnLiftTime <= 0 && IsStart && Joysticker && CandoBtn) {
			StealBtnLiftTime = 1f;
            if (BallOwner && BallOwner.Team != Joysticker.Team) {
				Joysticker.RotateTo(BallOwner.gameObject.transform.position.x, BallOwner.gameObject.transform.position.z);
				return Joysticker.DoPassiveSkill(ESkillSituation.Steal0, BallOwner.transform.position);
            } else
				return Joysticker.DoPassiveSkill(ESkillSituation.Steal0);
        } else
			return false;
    }

    private void Block(PlayerBehaviour player)
    {
        
    }

	public bool OnFakeShootBlockMoment(PlayerBehaviour player)
	{
		if (player)
		{
			DefBlock(ref player, 1);
			return true;
		} else
			return false;
	}

    public bool OnBlockMoment(PlayerBehaviour player)
    {
        if (player)
        {
            DefBlock(ref player);
            return true;
        } else
            return false;
    }

	public bool OnDoubleClickMoment(PlayerBehaviour player, EPlayerState state)
	{
		if (player.Team == ETeamKind.Self && (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)) {
			GameRecord.DoubleClickLaunch++;
			int playerindex = -1;

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

				case EPlayerState.Block:
				case EPlayerState.BlockCatch:
					UIDoubleClick.Get.SetData(EDoubleClick.Block, playerindex, 0.7f, null, DoubleBlock, player);
					return true;
				case EPlayerState.Rebound:
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
				AddExtraScoreRate(GameData.ExtraGreatRate);
				break;
			case 2: 
				AddExtraScoreRate(GameData.ExtraPerfectRate);
				Joysticker.SetAnger(GameConst.AddAnger_Perfect, CameraMgr.Get.DoubleClickDCBorn, CameraMgr.Get.DoubleClickDCBorn);				
				break;
		}

	}

	public void DoubleBlock(int lv, PlayerBehaviour player){
		switch (lv) {
		case 0: 
			break;
		case 1: 
			if(Shooter)
				CourtMgr.Get.SetBallState(EPlayerState.Block, Shooter);
			else
				CourtMgr.Get.SetBallState(EPlayerState.Block, player);
			break;
		case 2: 
			SetBall(player);
			break;
		}
	}

	public void DoubleRebound(int lv)
	{
		switch (lv) {
		case 0: 
			AddExtraScoreRate(0);
			break;
		case 1: 
			AddExtraScoreRate(GameData.ExtraGreatRate);
			DoShoot();
			break;
		case 2: 
			AddExtraScoreRate(GameData.ExtraPerfectRate);
			DoShoot();
			break;
		}
		
	}
	
	public bool OnBlockJump(PlayerBehaviour player)
    {
        if (player.PlayerRigidbody != null)
        {
            if (BallOwner)
            {
                if (Vector3.Distance(Joysticker.transform.position, BallOwner.transform.position) < 5f)
                    player.PlayerRigidbody.AddForce(player.JumpHight * transform.up + player.PlayerRigidbody.velocity.normalized / 2.5f, ForceMode.Force);
                else
                    player.PlayerRigidbody.velocity = GameFunction.GetVelocity(player.transform.position, 
                        new Vector3(BallOwner.transform.position.x, 3, BallOwner.transform.position.z), 70);

                return true;
            } else  
            if (Shooter && Vector3.Distance(player.transform.position, CourtMgr.Get.RealBall.transform.position) < 5)
            {
                player.PlayerRigidbody.velocity = GameFunction.GetVelocity(player.transform.position, 
                    new Vector3(CourtMgr.Get.RealBall.transform.position.x, 5, CourtMgr.Get.RealBall.transform.position.z), 70);
        
                return true;
            } else
            {
                player.PlayerRigidbody.AddForce(player.JumpHight * transform.up + player.PlayerRigidbody.velocity.normalized / 2.5f, ForceMode.Force);
                return true;
            }
        }

        return false;
    }

    public bool DoBlock() {
		if (IsStart && CandoBtn && Joysticker) {
			if(Joysticker.crtState == EPlayerState.Block && Joysticker.IsPerfectBlockCatch) {
				Joysticker.AniState(EPlayerState.BlockCatch);
				if(UIDoubleClick.Get.DoubleClicks[0].Enable)
					UIDoubleClick.Get.ClickStop(0);

				return true;
			} else {
				if (Shooter)
					if(IsReboundTime)
						return Rebound(Joysticker);
					else
						return Joysticker.DoPassiveSkill(ESkillSituation.Block, Shooter.transform.position);
	            else
	            if (BallOwner) {
					Joysticker.RotateTo(BallOwner.gameObject.transform.position.x, BallOwner.gameObject.transform.position.z); 
					return Joysticker.DoPassiveSkill(ESkillSituation.Block, BallOwner.transform.position);
				} else {
					if (!Shooter && Joysticker.InReboundDistance && IsReboundTime && GameStart.Get.TestMode == EGameTest.None)
						return Rebound(Joysticker);
					else
						return Joysticker.DoPassiveSkill(ESkillSituation.Block);
				}
			}
        }

		return false;
    }

	private bool inTipinDistance(PlayerBehaviour player) {
		return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), 
		                        new Vector2(CourtMgr.Get.ShootPoint[player.Team.GetHashCode()].transform.position.x, 
		            						CourtMgr.Get.ShootPoint[player.Team.GetHashCode()].transform.position.z)) <= 6;
	}

    private bool Rebound(PlayerBehaviour player)
    {
		return player.DoPassiveSkill(ESkillSituation.Rebound, CourtMgr.Get.RealBall.transform.position);
	}

	private bool JumpBall(PlayerBehaviour player)
	{
		return player.DoPassiveSkill(ESkillSituation.JumpBall, CourtMgr.Get.RealBall.transform.position);
	}
	
	public bool OnRebound(PlayerBehaviour player)
    {
        return true;
    }
    
    public bool OnSkill() {
		if (CandoBtn && DoSkill(Joysticker)) {
			return true;
		} else
			return false;
    }

	public bool DoSkill(PlayerBehaviour player)
    {
		bool result = false;
		if(player.CanUseActiveSkill && CheckOthersUseSkill)
        {
			if (player.CheckSkill) {
				player.AttackSkillEffect(player.Attribute.ActiveSkill.ID);
				result = player.ActiveSkill(player.gameObject);
			}
		}
		return result;
	}

	public void OnJoystickMoveStart(MovingJoystick move) {
		if (Joysticker)
			Joysticker.OnJoystickStart(move);
	}

	public void OnJoystickMove(MovingJoystick move)
    {
		if (Joysticker && (CanMoveSituation || Joysticker.CanMoveFirstDribble))
        {
            if (Mathf.Abs(move.joystickAxis.y) > 0 || Mathf.Abs(move.joystickAxis.x) > 0)
            {
                Joysticker.ClearMoveQueue();
				EPlayerState ps = Joysticker.crtState;

				if(!Joysticker.IsFall)
				{
					if(BallOwner == Joysticker)
						ps = EPlayerState.Dribble1;
					else
						ps = EPlayerState.Run0;
				}
                    
                Joysticker.OnJoystickMove(move, ps);
            }
        }
    }
    
    public void OnJoystickMoveEnd(MovingJoystick move)
    {
        if (Joysticker)
        {
			EPlayerState ps;

			if (BallOwner == Joysticker)
			{
				if(Joysticker.crtState == EPlayerState.Elbow)
					ps = EPlayerState.Elbow;
				else if(Joysticker.crtState == EPlayerState.HoldBall)
					ps = EPlayerState.HoldBall;
				else
					ps = EPlayerState.Dribble0;
			} else
				ps = EPlayerState.Idle;
            
            Joysticker.OnJoystickMoveEnd(move, ps);
        }
    }
	
	private void BackToDef(PlayerBehaviour someone, ETeamKind team, ref TTacticalData tactical, 
                           bool watchBallOwner = false)
	{
	    if(tactical.FileName == string.Empty)
            return;

        if(someone.CanMove && someone.WaitMoveTime == 0 && someone.TargetPosNum == 0) // 是否之前設定的戰術跑完.
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
                if (GameStart.Get.CourtMode == ECourtMode.Full && team == ETeamKind.Self)
                    moveData.Target = new Vector2(tacticalActions[i].x, -tacticalActions[i].z);
                else
                    moveData.Target = new Vector2(tacticalActions[i].x, tacticalActions[i].z);
						
                if (BallOwner != null)
                    moveData.LookTarget = BallOwner.transform;
                else
                {
                    if (team == ETeamKind.Self || GameStart.Get.CourtMode == ECourtMode.Half)
                        moveData.LookTarget = CourtMgr.Get.Hood[1].transform;
                    else
                        moveData.LookTarget = CourtMgr.Get.Hood[0].transform;
                }
						
                if(!watchBallOwner)
                    moveData.Speedup = true;

                moveData.TacticalName = tactical.FileName;
                someone.TargetPos = moveData;
            }
        }
	}

    private void InboundsBall(PlayerBehaviour someone, ETeamKind team, ref TTacticalData data)
    {
		if(!IsPassing && (someone.CanMove || someone.CanMoveFirstDribble) && !someone.IsMoving && someone.WaitMoveTime == 0 && someone.TargetPosNum == 0)
        {
            // Debug.LogFormat("InboundsBall, tactical:{0}", tacticalData);

            moveData.Clear();
			if(GameStart.Get.CourtMode == ECourtMode.Full)
            {
				if(someone == BallOwner)
                {
					int targetZ = 18;
					if (team == ETeamKind.Self)
						targetZ = -18;

					Vector2 v = new Vector2(someone.transform.position.x, targetZ);
					float dis = Vector2.Distance(new Vector2(someone.transform.position.x, someone.transform.position.z), v);
					if(dis <= 1.7f)
                    {
						if(BallOwner)
							StartCoroutine(AutoTee());
					}
                    else
                    {
						moveData.TacticalName = data.FileName;
						moveData.Target = new Vector2(someone.transform.position.x, targetZ);
						someone.TargetPos = moveData;
					}
	            }
                else if(data.FileName != string.Empty)
                {
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
								moveData.Target = new Vector2(tacticalActions[i].x, tacticalActions[i].z);
	                        else
								moveData.Target = new Vector2(tacticalActions[i].x, -tacticalActions[i].z);

							moveData.TacticalName = data.FileName;
							moveData.LookTarget = CourtMgr.Get.RealBall.transform;
							someone.TargetPos = moveData;
	                    }
	                }
	            }
			}
            else
            {
				if(someone == BallOwner)
                {
					Vector2 v = new Vector2(0, -0.2f);
					float dis = Vector2.Distance(new Vector2(someone.transform.position.x, someone.transform.position.z), v);
					if (dis <= 1.5f) {
						if (BallOwner)
							StartCoroutine(AutoTee());
					} else {
						moveData.TacticalName = data.FileName;
						moveData.Target = v;
						someone.TargetPos = moveData;
					}
				}
                else if (data.FileName != string.Empty)
                {
					tacticalActions = data.GetActions(someone.Index);
					
					if(tacticalActions != null)
                    {
						for(int j = 0; j < tacticalActions.Length; j++)
                        {
							moveData.Clear();
							moveData.Speedup = tacticalActions [j].Speedup;
							moveData.Catcher = tacticalActions [j].Catcher;
							moveData.Shooting = tacticalActions [j].Shooting;
							moveData.Target = new Vector2(tacticalActions [j].x, tacticalActions [j].z);
							
							moveData.TacticalName = data.FileName;
							moveData.LookTarget = CourtMgr.Get.RealBall.transform;
							someone.TargetPos = moveData;
						}
					}
				}
			}
		}
        
        if (someone.WaitMoveTime != 0 && someone == BallOwner)
            someone.AniState(EPlayerState.Dribble0);
    }

	IEnumerator AutoTee()
    {
		yield return new WaitForSeconds(1);

		bool flag = false;

	    if(BallOwner)
        {
		    PlayerBehaviour getball = null;
		    if (BallOwner.Team == ETeamKind.Self && BallOwner != Joysticker)
				getball = Joysticker;
			else
				getball = hasNearPlayer(BallOwner, 10, true);
		
			if (getball != null)
				flag = Pass(getball, true);
			else {
				int ran = UnityEngine.Random.Range(0, 2);
				int count = 0;
				for (int i = 0; i < PlayerList.Count; i++) {
					if (PlayerList [i].gameObject.activeInHierarchy && PlayerList [i].Team == BallOwner.Team && 
					    PlayerList [i] != BallOwner) {
						if (count == ran) {
							flag = Pass(PlayerList [i], true);
							break;
						}
						
						count++;
					}
				}
			}
		}

		if (!flag) {
		    if(Situation == EGameSituation.InboundsA)
		    {
		        ChangeSituation(EGameSituation.AttackA);
                AIController.Get.ChangeState(EGameSituation.AttackA, true);
		    }
			else if(Situation == EGameSituation.InboundsB)
			{
			    ChangeSituation(EGameSituation.AttackB);
                AIController.Get.ChangeState(EGameSituation.AttackB, true);
			}
		}
	}

//    /// <summary>
//    /// 找出某隊離球最近的球員.
//    /// </summary>
//    /// <param name="team"> 玩家 or 電腦. </param>
//    /// <returns></returns>
//    [CanBeNull]
//    private PlayerBehaviour findNearBallPlayer(ETeamKind team)
//    {
//        float nearDis = float.MaxValue;
//        PlayerBehaviour nearPlayer = null;
//
//        Vector2 ballPos = Vector2.zero;
//        ballPos.Set(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);
//
//        foreach(PlayerBehaviour someone in PlayerList)
//        {
//            if (someone.Team != team)
//                continue;
//
//            Vector2 someonePos = Vector2.zero;
//            someonePos.Set(someone.transform.position.x, someone.transform.position.z);
//
//            var dis = Vector2.Distance(ballPos, someonePos);
//            if(dis < nearDis)
//            {
//                nearDis = dis;
//                nearPlayer = someone;
//            }
//        }
//        
//        return nearPlayer;
//    }

    private TPlayerDisData [] GetPlayerDisAy(PlayerBehaviour Self, bool SameTeam = false, bool Angel = false)
	{
		TPlayerDisData [] DisAy = null;

		if(SameTeam)
		{
			if(PlayerList.Count > 2)
				DisAy = new TPlayerDisData[(PlayerList.Count / 2) - 1];
		}
		else
			DisAy = new TPlayerDisData[PlayerList.Count / 2];

		if (DisAy != null) 
		{
			for (int i = 0; i < PlayerList.Count; i++) 
			{
				if(SameTeam)
				{
					if(PlayerList[i].Team == Self.Team && PlayerList[i] != Self)
					{
						PlayerBehaviour anpc = PlayerList[i];
						for(int j = 0; j < DisAy.Length; j++)
						{
							if(DisAy[j].Distance == 0)
							{
								if(Angel)
									DisAy[j].Distance = Mathf.Abs(MathUtils.GetAngle(Self.transform, anpc.transform));
								else
									DisAy[j].Distance = GetDis(anpc, Self);
								DisAy[j].Player = anpc;
								break;
							}
						}
					}
				}
				else
				{
					if(PlayerList[i].Team != Self.Team)
					{
						PlayerBehaviour anpc = PlayerList[i];
						for(int j = 0; j < DisAy.Length; j++)
						{
							if(DisAy[j].Distance == 0)
							{
								if(Angel)
									DisAy[j].Distance = Mathf.Abs(MathUtils.GetAngle(Self.transform, anpc.transform));
								else
									DisAy[j].Distance = GetDis(anpc, Self);
								DisAy[j].Player = anpc;
								break;
							}
						}
					}
				}
			}
			
			TPlayerDisData temp = new TPlayerDisData ();
			
			for(int i = 0; i < DisAy.Length - 1; i ++)
			{
				for(int j = 0; j < DisAy.Length - 1; j++)
				{
					if(DisAy[j].Distance > DisAy[j + 1].Distance)
					{
						temp = DisAy[j];
						DisAy[j] = DisAy[j + 1];
						DisAy[j + 1] = temp;
					}
				}
			}	
		}

		return DisAy;
	}

    private void DefBlock(ref PlayerBehaviour npc, int kind = 0)
    {
		if (PlayerList.Count > 0 && !IsPassing && !IsBlocking) {
			PlayerBehaviour npc2;
			int rate = Random.Range(0, 100);
			TPlayerDisData [] DisAy = GetPlayerDisAy(npc, false, true);

			if(DisAy != null) {
				for (int i = 0; i < DisAy.Length; i++) {
					npc2 = DisAy [i].Player;
					if (npc2 && npc2 != npc && npc2.Team != npc.Team && npc2.AIing && 
					    !npc2.IsSteal && !npc2.IsPush) {
						float BlockRate = npc2.Attr.BlockRate;
						
						if(kind == 1)
							BlockRate = npc2.Attr.FaketBlockRate;	
						
						float mAngle = MathUtils.GetAngle(npc.transform, PlayerList [i].transform);
						
						if (GetDis(npc, npc2) <= GameConst.BlockDistance && Mathf.Abs(mAngle) <= 70) {
							if (rate < BlockRate) {
								if(npc2.DoPassiveSkill(ESkillSituation.Block, npc.transform.position)) {
									if (kind == 1)
										npc2.GameRecord.BeFake++;

									break;
								}
							}
						}
					}
				}
			}
		}
	}

    private void doLookAtBall(PlayerBehaviour someone)
    {
        if(someone.crtState != EPlayerState.Block && someone.AIing)
            someone.RotateTo(CourtMgr.Get.RealBall.transform.position.x, 
                             CourtMgr.Get.RealBall.transform.position.z);
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
            DoPickBall(someone);
        else
            doLookAtBall(someone);
    }

    public void DoPickBall([NotNull] PlayerBehaviour someone)
	{
	    if(someone.CanMove && someone.WaitMoveTime == 0)
	    {
            // 球員移動到球的位置.
	        moveData.Clear();
	        moveData.FollowTarget = CourtMgr.Get.RealBall.transform;
	        someone.TargetPos = moveData;
	    }

	    /*PlayerBehaviour player = null;
		
		if(BallOwner == null)
        {
            if(findNear)
            {
                player = findNearBallPlayer(someone);

				if(player != null && player.CanMove && player.WaitMoveTime == 0)
                {
					moveData.Clear();
					moveData.FollowTarget = CourtMgr.Get.RealBall.transform;
					player.TargetPos = moveData;
				}
                else if(someone.crtState != EPlayerState.Block && someone.AIing)
                    someone.RotateTo(CourtMgr.Get.RealBall.transform.position.x, 
                                 CourtMgr.Get.RealBall.transform.position.z);
            }
            else if(someone.CanMove && someone.WaitMoveTime == 0)
            {
				moveData.Clear();
				moveData.FollowTarget = CourtMgr.Get.RealBall.transform;
				someone.TargetPos = moveData;
            }
        }

        return player;*/
    }
	
	public float GetDis(PlayerBehaviour player1, PlayerBehaviour player2)
    {
        if (player1 != null && player2 != null && player1 != player2)
        {
            Vector3 v1 = player1.transform.position;
            Vector3 v2 = player2.transform.position;
            v1.y = v2.y;
            return Vector3.Distance(v1, v2);
        } else
            return -1;
    }

	public float GetDis(PlayerBehaviour someone, Vector3 target)
    {
        if(someone != null && target != Vector3.zero)
            return Vector3.Distance(someone.transform.position, target);

        return -1;
    }

	public float GetDis(PlayerBehaviour player1, Vector2 target)
    {
        if (player1 != null && target != Vector2.zero)
        {
            Vector3 v1 = new Vector3(target.x, 0, target.y);
            Vector3 v2 = player1.transform.position;
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
			CourtMgr.Get.RealBall.transform.parent = null;
		}
	}

    public bool SetBall(PlayerBehaviour p = null)
    {
		bool result = false;
		IsPassing = false;
		if (p != null && Situation != EGameSituation.End) {
			p.IsChangeColor = true;
			IsReboundTime = false;
			if (BallOwner != null) {
                if (BallOwner.Team != p.Team) {
					if (GameStart.Get.CourtMode == ECourtMode.Full) {
					    if(Situation == EGameSituation.AttackA)
					    {
					        ChangeSituation(EGameSituation.AttackB);
                            AIController.Get.ChangeState(EGameSituation.AttackB, false);
					    }
	                    else if(Situation == EGameSituation.AttackB)
	                    {
	                        ChangeSituation(EGameSituation.AttackA);
                            AIController.Get.ChangeState(EGameSituation.AttackA, false);
	                    }
					} else {
					    if(p.Team == ETeamKind.Self)
					    {
					        ChangeSituation(EGameSituation.InboundsA);
					        AIController.Get.ChangeState(EGameSituation.InboundsA);
					    }
					    else
					    {
					        ChangeSituation(EGameSituation.InboundsB);
                            AIController.Get.ChangeState(EGameSituation.InboundsB);
					    }
					}
                } else {
                    if(Situation == EGameSituation.InboundsA)
                    {
                        ChangeSituation(EGameSituation.AttackA);
                        AIController.Get.ChangeState(EGameSituation.AttackA, true);
                    }
                    else if(Situation == EGameSituation.InboundsB)
                    {
                        ChangeSituation(EGameSituation.AttackB);
                        AIController.Get.ChangeState(EGameSituation.AttackB, true);
                    }
                    else
                        BallOwner.ResetFlag(false);
                }
            } else {
                if(Situation == EGameSituation.APickBallAfterScore)
                {
                    ChangeSituation(EGameSituation.InboundsA);
                    AIController.Get.ChangeState(EGameSituation.InboundsA);
                }
                else if(Situation == EGameSituation.BPickBallAfterScore)
                {
                    ChangeSituation(EGameSituation.InboundsB);
                    AIController.Get.ChangeState(EGameSituation.InboundsB);
                }
				else if(Situation == EGameSituation.InboundsA)
				{
				    ChangeSituation(EGameSituation.AttackA);
                    AIController.Get.ChangeState(EGameSituation.AttackA, true);
				}
				else if(Situation == EGameSituation.InboundsB)
				{
				    ChangeSituation(EGameSituation.AttackB);
                    AIController.Get.ChangeState(EGameSituation.AttackB, true);
				}
                else {
					if (GameStart.Get.CourtMode == ECourtMode.Full || 
					   (p.Team == ETeamKind.Self && Situation == EGameSituation.AttackA) ||
					   (p.Team == ETeamKind.Npc && Situation == EGameSituation.AttackB)) {
					       if(p.Team == ETeamKind.Self)
					       {
					           ChangeSituation(EGameSituation.AttackA, p);
					           AIController.Get.ChangeState(EGameSituation.AttackA);
					       }
					       else
					       {
					           ChangeSituation(EGameSituation.AttackB, p);
                                AIController.Get.ChangeState(EGameSituation.AttackB);
					       }
					} else {
					    if(p.Team == ETeamKind.Self)
					    {
					        ChangeSituation(EGameSituation.InboundsA);
					        AIController.Get.ChangeState(EGameSituation.InboundsA);
					    }
					    else
					    {
					        ChangeSituation(EGameSituation.InboundsB);
                            AIController.Get.ChangeState(EGameSituation.InboundsB);
					    }
					}
                }
            }

			if(BallOwner != null)
				BallOwner.IsBallOwner = false;

        	BallOwner = p;
			BallOwner.WaitMoveTime = 0;
			BallOwner.IsBallOwner = true;
			result = true;
			Shooter = null;

			if(ballHolder != null) {
				ballHolder.SetActive(true);
				ballHolder.transform.parent = BallOwner.transform;
				ballHolder.transform.localEulerAngles = Vector3.zero;
				ballHolder.transform.localScale = Vector3.one;
				ballHolder.transform.localPosition = BallOwner.BodyHeight.transform.localPosition;
			}
			
			for(int i = 0 ; i < PlayerList.Count; i++)
				PlayerList[i].ClearAutoFollowTime();

			if (BallOwner && BallOwner.DefPlayer != null)
				BallOwner.DefPlayer.SetAutoFollowTime();

			UIGame.Get.ChangeControl(p.Team == ETeamKind.Self);
			UIGame.Get.SetPassButton();
			CourtMgr.Get.SetBallState(EPlayerState.HoldBall, p);

        	if (p) {
				AudioMgr.Get.PlaySound(SoundType.SD_CatchBall);
				p.WaitMoveTime = 0;
				p.IsFirstDribble = true;
				CourtMgr.Get.RealBallTrigger.IsAutoRotate = false;

				for (int i = 0; i < PlayerList.Count; i++){
					if (PlayerList [i].Team != p.Team) {
						PlayerList [i].ResetMove();
						break;
					}
				} 
				 
				if(p.IsIdle || p.IsDef)
					p.AniState(EPlayerState.Dribble0);
				else 
				if(p.IsRun)
					p.AniState(EPlayerState.Dribble1);        
        	}
    	} else {
			if(ballHolder != null)				
				ballHolder.SetActive(false);
        
			SetBallOwnerNull();
		}

		return result;
    }

	private PlayerBehaviour findJumpBallPlayer(ETeamKind team) {
		int findIndex = team == 0? 0 : 3;
		PlayerBehaviour npc = null;
		for (int i = 0; i < PlayerList.Count; i++)
			if (PlayerList [i].gameObject.activeInHierarchy && PlayerList [i].Team == team && PlayerList [i].IsJumpBallPlayer) {
				findIndex = i;
			}
			
		if(findIndex < PlayerList.Count)
			npc = PlayerList[findIndex];

		return npc;
	}

	public PlayerBehaviour FindNearNpc(){
		PlayerBehaviour p = null;
		float dis = 0;
		for (int i=0; i<PlayerList.Count; i++) {
			if (PlayerList [i].Team == ETeamKind.Npc){
				if(p == null){
					p = PlayerList[i];
					dis = Vector3.Distance(Joysticker.gameObject.transform.position, PlayerList[i].transform.position);
				} else {
					float temp = Vector3.Distance(Joysticker.gameObject.transform.position, PlayerList[i].transform.position);
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
		CourtMgr.Get.ResetBasketEntra();
        Shooter = null;

		if (GameStart.Get.TestMode == EGameTest.Shoot) {
			SetBall(Joysticker);	
			Joysticker.AniState(EPlayerState.HoldBall);
		}
		else if(GameStart.Get.TestMode == EGameTest.Block){
			SetBall(PlayerList [1]);
			PlayerList [1].AniState(EPlayerState.Dribble0);
			PlayerList [1].AniState(EPlayerState.Shoot0);
		}
    }
	
	public bool PassingStealBall(PlayerBehaviour player, int dir)
	{
		if(player.IsDefence && (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB) && Passer && passingStealBallTime == 0)
		{
			if(Catcher == player)
				return false;

			int rate = UnityEngine.Random.Range(0, 100);

			if(CourtMgr.Get.RealBallState == EPlayerState.Pass0 || 
			   CourtMgr.Get.RealBallState == EPlayerState.Pass2 ||
			   CourtMgr.Get.RealBallState == EPlayerState.Pass1 || 
			   CourtMgr.Get.RealBallState == EPlayerState.Pass3)
			{
				if(BallOwner == null && (rate > Passer.Attr.PassRate || dir == 5) && !player.IsPush)
				{
					if(dir == 6)
					{
						player.AniState(EPlayerState.Intercept1, CourtMgr.Get.RealBall.transform.position);
					}
					else if(dir == 5)
					{
						if(player.CheckAnimatorSate(EPlayerState.Intercept1))
						{
							if(BallTrigger.PassKind == 0 || BallTrigger.PassKind == 2)
								CourtMgr.Get.RealBall.transform.DOKill();
							
							if(SetBall(player))
							{
								player.AniState(EPlayerState.HoldBall);
								passingStealBallTime = Time.time + 2;
							}

							player.GameRecord.Intercept++;
							if (Passer)
								Passer.GameRecord.BeIntercept++;

							IsPassing = false;
						}
					}
					else if(dir != 0)
					{
						player.AniState(EPlayerState.Intercept0);
						
						if(BallTrigger.PassKind == 0 || BallTrigger.PassKind == 2)
							CourtMgr.Get.RealBall.transform.DOKill();
						
						if(SetBall(player))
						{
//							player.AniState(EPlayerState.HoldBall);
							passingStealBallTime = Time.time + 2;
						}

						player.GameRecord.Intercept++;
						if (Passer)
							Passer.GameRecord.BeIntercept++;

                        IsPassing = false;
					}
				}
			}

			return true;
		}else
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
		    IsShooting || 
		    !player.IsCanCatchBall || 
		    player.CheckAnimatorSate(EPlayerState.GotSteal) || 
		    player.IsPush || 
		    dir == 6)
            return;

		if (Catcher) {
			if(Situation == EGameSituation.APickBallAfterScore || Situation == EGameSituation.BPickBallAfterScore)
				IsPassing = false;
			else
				return;
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
			if(Situation == EGameSituation.JumpBall)
            {
//				CourtMgr.Get.SetBallState(EPlayerState.JumpBall, player);
                GameMsgDispatcher.Ins.SendMesssage(EGameMsg.PlayerTouchBallWhenJumpBall, player);
			}
            else if((isEnter || GameStart.Get.TestMode == EGameTest.Rebound) &&
				   player != BallOwner &&
				   CourtMgr.Get.RealBall.transform.position.y >= 3 &&
				   (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)) {

				if (GameStart.Get.TestMode == EGameTest.Rebound ||
				    Situation == EGameSituation.AttackA ||
				    Situation == EGameSituation.AttackB) {
					if (GameStart.Get.TestMode == EGameTest.Rebound || CourtMgr.Get.RealBallState ==  EPlayerState.Steal0 || CourtMgr.Get.RealBallState ==  EPlayerState.Rebound) {
						if (Random.Range(0, 100) < player.Attr.ReboundRate) {
							Rebound(player);
						}
					}
				}
			}
            break;
		case 5: //finger
			if (isEnter && !player.IsBallOwner && player.IsRebound && !IsTipin) {
				if (GameStart.Get.TestMode == EGameTest.Rebound || Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB) {
					if (SetBall(player)) {
						player.GameRecord.Rebound++;
						player.SetAnger(GameConst.AddAnger_Rebound, player.gameObject);

						if (player == BallOwner && inTipinDistance(player)) {
							coolDownPass = Time.time + 3;
							if (player == Joysticker)
								OnDoubleClickMoment(player, EPlayerState.Rebound);
							else
							if (Random.Range(0, 100) < player.Attr.TipInRate)
								DoShoot();
							else
							if (player.Team == Joysticker.Team)
								OnDoubleClickMoment(player, EPlayerState.Rebound);
						}
					}
				}
			}

			break;
		default :
			bool canSetball = false;
			
			if (!player.IsRebound && (player.IsCatcher || player.CanMove)) {
				if (Situation == EGameSituation.APickBallAfterScore) {
					if (player.Team == ETeamKind.Self)
						canSetball = true;
				} else 
				if (Situation == EGameSituation.BPickBallAfterScore)
				{
					if (player.Team == ETeamKind.Npc)
						canSetball = true;
				} else
					canSetball = true;
				
				if (canSetball && !IsPickBall)
				{
					if (Situation == EGameSituation.APickBallAfterScore || Situation == EGameSituation.BPickBallAfterScore){
						if(CourtMgr.Get.RealBall.transform.position.y > 1.7f)
							player.AniState(EPlayerState.CatchFlat, CourtMgr.Get.RealBall.transform.position);
						else
							player.AniState(EPlayerState.PickBall0, CourtMgr.Get.RealBall.transform.position);
					} else 
					if (SetBall(player)) {
						if(player.AIing || player.IsIdle)
							player.AniState(EPlayerState.Dribble0);
						else 
						if(player.IsRun || player.IsDribble)
                        	player.AniState(EPlayerState.Dribble1);
                    	else
                        	player.AniState(EPlayerState.HoldBall);
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
				if(!player2.IsDefence && player1.IsDefence)
				{
					if(Mathf.Abs(MathUtils.GetAngle(player2.transform, player1.transform)) <= GameConst.SlowDownAngle)
						player2.SetSlowDown(GameConst.SlowDownTime);
				}
                break;
        }
    }

    public void DefRangeTouch(PlayerBehaviour player1, PlayerBehaviour player2)
    {
        if (player1.IsDefence)
        {
            DefMove(player1.DefPlayer);     
        }
    }

	public void DefRangeTouchBall(PlayerBehaviour player)
	{
		if(player.IsHavePickBall2) {
			if (BallOwner == null && Shooter == null && Catcher == null && (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)) {
				int rate = Random.Range(0, 100);
				if(rate < player.PickBall2Rate) {
					player.DoPassiveSkill(ESkillSituation.PickBall, CourtMgr.Get.RealBall.transform.position);
				}
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
		   (GameStart.Get.TestMode == EGameTest.Alleyoop || 
		 	Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)) {
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
						if (Random.Range(0, 100) < player.Attr.AlleyOopRate || GameStart.Get.TestMode == EGameTest.Alleyoop) {
							player.AniState(EPlayerState.Alleyoop, CourtMgr.Get.ShootPoint [team].transform.position);

							if ((BallOwner != Joysticker || (BallOwner == Joysticker && Joysticker.AIing)) && Random.Range(0, 100) < BallOwner.Attr.AlleyOopPassRate) {
								if (BallOwner.DoPassiveSkill(ESkillSituation.Pass0, player.transform.position))
									Catcher = player;
							} else
								UIGame.Get.ShowAlleyoop(true, player.Index);

							player.GameRecord.AlleyoopLaunch++;
						}
					}
				}
			}
		}
	}

	IEnumerator playFinish() {
		yield return new WaitForSeconds(2);

		SetGameRecordToUI();
		if(GameStart.Get.IsAutoReplay){
			UIGameResult.Get.OnAgain();
			Invoke("JumpBallForReplay", 1);
		}
	}
	
	public void JumpBallForReplay () {
		UIGame.Get.UIState(EUISituation.Start);
	}
	
	public void SetGameRecordToUI() {
		UIGameResult.Get.SetGameRecord(ref GameRecord);
		for (int i = 0; i < PlayerList.Count; i ++)
			UIGameResult.Get.AddDetailString(ref PlayerList[i].Attr, i);
	}

    private void gameResult()
    {
        ChangeSituation(EGameSituation.End);
        AIController.Get.ChangeState(EGameSituation.End);

		UIGame.Get.GameOver();
		GameRecord.Done = true;
		SetGameRecord(true);
		StartCoroutine(playFinish());


		if (UIGame.Get.Scores [0] >= UIGame.Get.Scores [1]) {
			SelfWin ++;
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].Team == ETeamKind.Self)
					PlayerList [i].AniState (EPlayerState.Ending0);
				else
					PlayerList [i].AniState (EPlayerState.Ending10);
		}
		else
		{
			NpcWin ++;
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].Team == ETeamKind.Self)
					PlayerList [i].AniState (EPlayerState.Ending10);
			else
				PlayerList [i].AniState (EPlayerState.Ending0);
		}
    }

	private readonly EPlayerState[] shootInState = { EPlayerState.Show101, EPlayerState.Show102, EPlayerState.Show103, EPlayerState.Show104};
	private readonly EPlayerState[] shootOutState = {EPlayerState.Show201, EPlayerState.Show202};

	public void ShowShootSate(bool isIn, int team)
	{
		if (GameStart.Get.CourtMode == ECourtMode.Half && Shooter)
			team = Shooter.Team.GetHashCode();

		for (int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i].Team.GetHashCode() == team)
			{
				if(PlayerList[i] != pickBallPlayer && !PlayerList[i].IsDunk){
					if(isIn)
						PlayerList[i].AniState(shootInState[Random.Range(0, shootInState.Length -1)]);
					else {
					if(PlayerList[i].crtState == EPlayerState.Idle && 
					   GetDis(PlayerList[i], new Vector2(CourtMgr.Get.ShootPoint[PlayerList[i].Team.GetHashCode()].transform.position.x,
					                                     CourtMgr.Get.ShootPoint[PlayerList[i].Team.GetHashCode()].transform.position.z)) > 11
					   )
							PlayerList[i].AniState(shootOutState[Random.Range(0, shootOutState.Length -1)]);
					}
				}
			}
	}
    
    public void PlusScore(int team, bool isSkill, bool isChangeSituation)
    {
		if (GameStart.Get.CourtMode == ECourtMode.Half && Shooter)
			team = Shooter.Team.GetHashCode();

		int score = 2;
		if (ShootDistance >= GameConst.TreePointDistance) {
			score = 3;
			GameController.Get.ShowWord(EShowWordType.NiceShot, team);
		}

		if (GameStart.Get.TestMode == EGameTest.Skill)
			UIGame.Get.PlusScore(team, score);
		else
		if (IsStart && GameStart.Get.TestMode == EGameTest.None) {
			if (Shooter) {
				if (ShootDistance >= GameConst.TreePointDistance)
					Shooter.GameRecord.FG3In++;
				else
					Shooter.GameRecord.FGIn++;

				if (Shooter.crtState == EPlayerState.TipIn)
					Shooter.GameRecord.Tipin++;

				if (IsShooting)
					Shooter.GameRecord.ShotError--;

				if (Passer && Passer.DribbleTime <= 2)
					Passer.GameRecord.Assist++;
			}
            
			AudioMgr.Get.PlaySound(SoundType.SD_Net);
            UIGame.Get.PlusScore(team, score);

			if(isChangeSituation)
			{
				if(GameStart.Get.IsDebugAnimation) {
					Debug.LogWarning ("UIGame.Get.Scores [0] : " + UIGame.Get.Scores [0]);
					Debug.LogWarning ("UIGame.Get.MaxScores [0] : " + UIGame.Get.MaxScores [0]);
					Debug.LogWarning ("UIGame.Get.Scores [1] : " + UIGame.Get.Scores [1]);
					Debug.LogWarning ("UIGame.Get.MaxScores [1] : " + UIGame.Get.MaxScores [1]);
				}
				if (GameStart.Get.WinMode == EWinMode.Score && UIGame.Get.Scores[team] >= UIGame.Get.MaxScores[team])
                    gameResult();
				else if(team == ETeamKind.Self.GetHashCode())
				{
                    //ChangeSituation(EGameSituation.TeeBPicking);
                    ChangeSituation(EGameSituation.SpecialAction);
				    AIController.Get.ChangeState(EGameSituation.SpecialAction, EGameSituation.BPickBallAfterScore);
				}
				else
				{
                    //ChangeSituation(EGameSituation.TeeAPicking);
                    ChangeSituation(EGameSituation.SpecialAction);
                    AIController.Get.ChangeState(EGameSituation.SpecialAction, EGameSituation.APickBallAfterScore);
				}

                if (!isSkill && Shooter)
					Shooter.SetAnger(GameConst.AddAnger_PlusScore, CourtMgr.Get.ShootPoint[0].gameObject);
			}
		}

		Shooter = null;
		IsPassing = false;
		ShootDistance = 0;

		if(GameStart.Get.IsDebugAnimation) {
			if(shootSwishTimes != shootScoreSwishTimes)
				Debug.LogWarning("shootSwishTimes != shootScoreSwishTimes");
			if(shootTimes != shootScoreTimes)
				Debug.LogWarning("shootTimes != shootScoreTimes");
		}
		if (GameStart.Get.TestMode == EGameTest.AttackA) {
			SetBall(Joysticker);
		}
    }

    [CanBeNull]
	private PlayerBehaviour findTeammate(PlayerBehaviour player, float dis, float angle)
    {
        for (int i = 0; i < PlayerList.Count; i++)
        {
			PlayerBehaviour targetNpc = PlayerList[i];
			if(targetNpc.gameObject.activeSelf && targetNpc != player && targetNpc.Team == player.Team && 
			    GetDis(player, targetNpc) <= dis && HasDefPlayer(targetNpc, 1.5f, 40) == 0)
            {
				float mangle = MathUtils.GetAngle(player.transform, targetNpc.transform);
	            
                if (mangle >= 0 && mangle <= angle)
					return targetNpc;
				if (mangle <= 0 && mangle >= -angle) 
					return targetNpc;
	        }
        }
        
        return null;
    }

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
			if(PlayerList[i].gameObject.activeInHierarchy && PlayerList[i].Team != player.Team)
            {
	            PlayerBehaviour targetNpc = PlayerList[i];
				float realAngle = MathUtils.GetAngle(player.transform, targetNpc.transform);
	            
//	            if(GetDis(npc, targetNpc) <= dis)
	            if(MathUtils.Find2DDis(player.transform.position, targetNpc.transform.position) <= dis)
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

//	public int HaveDefPlayer(PlayerBehaviour player, float dis, float angle, out PlayerBehaviour defPlayer)
//    {
//		int result = 0;
//		float mangle;
//		defPlayer = null;
//
//		for (int i = 0; i < PlayerList.Count; i++) {
//			if (PlayerList [i].gameObject.activeInHierarchy && PlayerList [i].Team != player.Team) {
//				PlayerBehaviour TargetNpc = PlayerList [i];
//				mangle = MathUtils.GetAngle(player.transform, TargetNpc.transform);
//				
//				if (GetDis(player, TargetNpc) <= dis && TargetNpc.CheckAnimatorSate(EPlayerState.Idle)) {
//					if (mangle >= 0 && mangle <= angle) {
//						result = 1;
//						defPlayer = TargetNpc;
//						break;
//					} else 
//					if (mangle <= 0 && mangle >= -angle) {
//						result = 2;
//						defPlayer = TargetNpc;
//						break;
//					}
//				}
//			}
//		}
//		
//		return result;
//	}

	private int haveStealPlayer(PlayerBehaviour p1, PlayerBehaviour p2, float dis, float angle)
    {
		int result = 0;
		float mangle;

		if (p1 != null && p2 != null && p1 != p2) {
			mangle = MathUtils.GetAngle(p1.transform, p2.transform);
			
			if (GetDis(p1, p2) <= dis) {
				if (mangle >= 0 && mangle <= angle)				
					result = 1;
				else 
				if (mangle <= 0 && mangle >= -angle)				
					result = 2;
			}
		}
		
		return result;
	}
    
    private PlayerBehaviour hasNearPlayer(PlayerBehaviour self, float dis, bool isSameTeam, 
                                          bool findBallOwnerFirst = false)
    {
        PlayerBehaviour nearPlayer = null;

        for (int i = 0; i < PlayerList.Count; i++)
        {
			if (PlayerList[i].gameObject.activeInHierarchy)
			{
			    PlayerBehaviour npc = PlayerList [i];

			    if (isSameTeam)
                {
		            if(PlayerList[i] != self && PlayerList[i].Team == self.Team && 
//                       GetDis(self, npc) <= dis)
                       MathUtils.Find2DDis(self.transform.position, npc.transform.position) <= dis)
                    {
		                nearPlayer = npc;
		                break;
		            }
		        }
                else
                {
	                if (findBallOwnerFirst)
                    {
	                    if(npc != self && npc.Team != self.Team && npc == BallOwner && GetDis(self, npc) <= dis)
                        {
	                        nearPlayer = npc;
	                        break;
	                    }
	                }
                    else
                    {
	                    if(npc != self && npc.Team != self.Team && GetDis(self, npc) <= dis && npc.crtState == EPlayerState.Idle)
                        {
	                        nearPlayer = npc;
	                        break;
	                    }
	                }
			    }
			}
        }
        
        return nearPlayer;
    }

    /// <summary>
    /// 是否球員在前場.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool IsInUpfield(PlayerBehaviour player)
    {
		if(player.Team == ETeamKind.Self && (player.transform.position.z >= 15.5f && player.transform.position.x <= 1 && player.transform.position.x >= -1))
            return false;
        if(player.Team == ETeamKind.Npc && player.transform.position.z <= -15.5f && player.transform.position.x <= 1 && player.transform.position.x >= -1)
            return false;
		if(player.CheckAnimatorSate(EPlayerState.Elbow))
			return false;
        return true;
    }
    
    //Temp
    public Vector3 EditGetPosition(int index)
    {
        if (PlayerList.Count > index)
        {
            return PlayerList [index].transform.position;       
        } else
            return Vector3.zero;
    }
    
	public void ResetAll()
	{
		for(int i = 0; i < PlayerList.Count; i++)
			PlayerList[i].ResetFlag();
	}

    public void EditSetMove(TTacticalAction ActionPosition, int index)
    {
        if (PlayerList.Count > index)
        {
			moveData.Clear();
			moveData.Target = new Vector2(ActionPosition.x, ActionPosition.z);
			moveData.Speedup = ActionPosition.Speedup;
			moveData.Catcher = ActionPosition.Catcher;
			moveData.Shooting = ActionPosition.Shooting;
			PlayerList [index].TargetPos = moveData;
        }
    }

	public void EditSetMove(Vector2 ActionPosition, int index)
	{
		if (PlayerList.Count > index)
		{
			moveData.Clear();
			moveData.Target = ActionPosition;
			PlayerList [index].TargetPos = moveData;
		}
	}
    
    public void EditSetJoysticker(int index)
    {
        if (PlayerList.Count > index)
        {
            Joysticker = PlayerList [index];        
        }
    }

    public void SetEndPass()
    {
		if(IsPassing){
			if (Catcher != null && !Catcher.IsFall && !Catcher.IsPush && !Catcher.IsBlock && !Catcher.IsPass)
	        {
	            if(SetBall(Catcher))
					coolDownPass = Time.time + 3;

				if(Catcher && Catcher.NeedShooting)
				{
					DoShoot();
					Catcher.NeedShooting = false;
				}

				Catcher = null;
			}else{
	            setDropBall(Passer);
			}
			IsPassing = false;
		}
    }

	private void setDropBall(PlayerBehaviour player = null){
		if(IsPassing)
		{
//			if (BallOwner != null)
//				BallOwner.DelActionFlag(ActionFlag.IsPass);
//			else
//			{
//				for(int i = 0; i < PlayerList.Count; i++)
//				{
//					if(PlayerList[i].CheckAction(ActionFlag.IsPass))
//					{
//						PlayerList[i].DelActionFlag(ActionFlag.IsPass);
//						break;
//					}
//				}
//			}
			
			if (Catcher != null)
			{
//				Catcher.DelActionFlag(ActionFlag.IsCatcher);
				if(Catcher.NeedShooting)
				{
					DoShoot();
					Catcher.NeedShooting = false;
				}
			}
//			else{
//				for(int i = 0; i < PlayerList.Count; i++)
//				{
//					if(PlayerList[i].CheckAction(ActionFlag.IsCatcher))
//					{
//						PlayerList[i].DelActionFlag(ActionFlag.IsCatcher);
//						break;
//					}
//				}				
//			}
		}
		
		SetBall();
		CourtMgr.Get.SetBallState(EPlayerState.Steal0, player);
	}
	
	public void Reset()
	{
		IsReset = true;
		IsPassing = false;
		Shooter = null;
		IsStart = false;
		SetBallOwnerNull ();

		GameTime = GameStart.Get.GameWinValue;
		UIGame.Get.MaxScores[0] = GameStart.Get.GameWinValue;
		UIGame.Get.MaxScores[1] = GameStart.Get.GameWinValue;
		CameraMgr.Get.ShowPlayerInfoCamera (false);
		UIPassiveEffect.Get.Reset();

		if (GameData.Setting.AIChangeTime > 100)
			Joysticker.SetNoAI();
		else
			Joysticker.SetToAI();

		CourtMgr.Get.SetBallState (EPlayerState.Reset);

		for (int i = 0; i < PlayerList.Count; i++) 
		{
			PlayerList [i].crtState = EPlayerState.Idle;
			PlayerList [i].ResetFlag();
			PlayerList [i].ResetCurveFlag();
			PlayerList [i].ResetSkill();
			PlayerList [i].SetAnger (-PlayerList[i].Attribute.MaxAnger);

			if(PlayerList[i].Postion == EPlayerPostion.G)
			{
				if(PlayerList[i].Team == ETeamKind.Self)
					PlayerList[i].transform.position = bornPosAy[0];
				else
					PlayerList[i].transform.position = bornPosAy[3];
			}
			else if(PlayerList[i].Postion == EPlayerPostion.C)
			{
				if(PlayerList[i].Team == ETeamKind.Self)
					PlayerList[i].transform.position = bornPosAy[1];
				else
					PlayerList[i].transform.position = bornPosAy[4];
			}
			else
			{
				if(PlayerList[i].Team == ETeamKind.Self)
					PlayerList[i].transform.position = bornPosAy[2];
				else
					PlayerList[i].transform.position = bornPosAy[5];
			}

			PlayerList [i].AniState(EPlayerState.Idle);

			if(PlayerList[i].Team == ETeamKind.Npc)
				PlayerList[i].transform.localEulerAngles = new Vector3(0, 180, 0);
			else
				PlayerList[i].transform.localEulerAngles = Vector3.zero;
		}

		ChangeSituation(EGameSituation.Opening);
        AIController.Get.ChangeState(EGameSituation.Opening);
    }

	public void SetPlayerLevel(){
		GameData.Setting.AIChangeTime = PlayerPrefs.GetFloat(SettingText.AITime, 1);
		if (GameData.Setting.AIChangeTime > 100)
			Joysticker.SetNoAI();
		else
			Joysticker.SetToAI();

		for (int i = 0; i < PlayerList.Count; i++) {
			if (PlayerList[i].Team == ETeamKind.Npc)
				PlayerList[i].Attribute.AILevel = GameConst.NpcAILevel;
			else
				PlayerList[i].Attribute.AILevel = GameConst.SelfAILevel;

			if(PlayerList[i].Team == ETeamKind.Npc)
				PlayerList[i].transform.localEulerAngles = new Vector3(0, 180, 0);
			else
				PlayerList[i].transform.localEulerAngles = Vector3.zero;

			PlayerList[i].InitAttr();
		}
	}

    public void SetAllPlayerLayer (string layerName){
		for (int i = 0; i < PlayerList.Count; i++)
			GameFunction.ReSetLayerRecursively(PlayerList[i].gameObject, layerName,"PlayerModel", "(Clone)");
	}

	public void ShowWord (EShowWordType type, int team = 0, GameObject parent = null) {
		switch(type) {
		case EShowWordType.Block:
			EffectManager.Get.PlayEffect("ShowWord_Block", Vector3.zero, parent, null, 0.5f, true);
			break;
		case EShowWordType.Dunk:
			EffectManager.Get.PlayEffect("ShowWord_Dunk", Vector3.zero, CourtMgr.Get.ShootPoint[team], null, 0.5f, true);
			break;
		case EShowWordType.NiceShot:
			EffectManager.Get.PlayEffect("ShowWord_NiceShot", Vector3.zero, CourtMgr.Get.ShootPoint[team], null, 0.5f, true);
			break;
		case EShowWordType.Punch:
			EffectManager.Get.PlayEffect("ShowWord_Punch", Vector3.zero, parent, null, 0.5f, true);
			break;
		case EShowWordType.Steal:
			EffectManager.Get.PlayEffect("ShowWord_Steal", Vector3.zero, parent, null, 0.5f, true);
			break;
		}
	}

	public void PushCalculate(PlayerBehaviour player, float dis, float angle)
	{
		for (int i = 0; i < PlayerList.Count; i++) {
			if(PlayerList[i] && PlayerList[i].Team != player.Team){
				if((Vector3.Distance(PlayerList[i].transform.position, player.transform.position) < dis) && GameFunction.IsTouchPlayerArea(player.transform, PlayerList[i].transform.position, angle)){
					int rate = UnityEngine.Random.Range(0, 100);
					PlayerBehaviour faller = PlayerList[i];
					PlayerBehaviour pusher = player;

					if(rate < faller.Attr.StrengthRate){
						if(faller.AniState(EPlayerState.Fall2, pusher.transform.position)) {
							faller.SetAnger(GameConst.DelAnger_Fall2);
							pusher.SetAnger(GameConst.AddAnger_Push, faller.gameObject);
							pusher.GameRecord.Knock++;
							faller.GameRecord.BeKnock++;
						}
					}
					else{
						if(faller.AniState(EPlayerState.Fall1, pusher.transform.position)) {
							faller.SetAnger(GameConst.DelAnger_Fall1);
							pusher.SetAnger(GameConst.AddAnger_Push, faller.gameObject);
							if(faller == Joysticker || pusher == Joysticker)
								ShowWord(EShowWordType.Punch, 0, pusher.ShowWord);
						}
					}

					if (pusher.crtState == EPlayerState.Elbow) {
						pusher.GameRecord.Elbow++;
						faller.GameRecord.BeElbow++;
					} else {
						pusher.GameRecord.Push++;
						faller.GameRecord.BePush++;
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
		set {doubleType = value;}
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
            for(int i = 0; i < PlayerList.Count; i++)            
				if (PlayerList[i].CheckAnimatorSate(EPlayerState.Shoot0) || 
				    PlayerList[i].CheckAnimatorSate(EPlayerState.Shoot1) || 
				    PlayerList[i].CheckAnimatorSate(EPlayerState.Shoot2) || 
				    PlayerList[i].CheckAnimatorSate(EPlayerState.Shoot3) ||
				    PlayerList[i].CheckAnimatorSate(EPlayerState.Shoot6) ||
				    PlayerList[i].CheckAnimatorSate(EPlayerState.TipIn) ||
				    PlayerList[i].IsLayup)
                    return true;

            return false;
        }
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
				if (PlayerList [i].CheckAnimatorSate(EPlayerState.Shoot0) || PlayerList [i].CheckAnimatorSate(EPlayerState.Shoot2))
					return true;            
			
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
				if (PlayerList [i].CheckAnimatorSate(EPlayerState.Block))
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
				if (PlayerList [i].CheckAnimatorSate(EPlayerState.PickBall0))
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

	public bool CheckOthersUseSkill {
		get {
			for (int i=0; i<PlayerList.Count; i++) {
				if(PlayerList[i].IsUseSkill)
					return false;
			}
			return true;
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

	private bool CanMoveSituation
	{
		get
		{
			if (Situation == EGameSituation.AttackA ||
			    Situation == EGameSituation.AttackB ||
			    Situation == EGameSituation.Opening || 
			    Situation == EGameSituation.JumpBall)
				return true;
			else
				return false;
		}
	}

	public bool CandoBtn
	{
		get
		{
			if(Situation == EGameSituation.InboundsA || Situation == EGameSituation.InboundsB || Situation == EGameSituation.APickBallAfterScore || Situation == EGameSituation.BPickBallAfterScore)
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
