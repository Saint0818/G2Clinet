using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private float coolDownPass = 0;
    public float CoolDownCrossover = 0;
    private float shootDistance = 0;
    public float RealBallFxTime = 0;
	public float StealBtnLiftTime = 1f;
	private float waitStealTime = 0;
	private float passingStealBallTime = 0;

    /// <summary>
    /// 玩家控制的球員.
    /// </summary>
	public PlayerBehaviour BallOwner;
	public PlayerBehaviour Joysticker;
	public PlayerBehaviour Shooter;
    public PlayerBehaviour Catcher;
	public PlayerBehaviour Passer;
	private PlayerBehaviour pickBallPlayer;
	private GameObject ballHolder = null;

    // 0:C, 1:F, 2:G
	private Vector2[] teeBackPosAy = new Vector2[3];

    // A Team, 0:G, 1:C, 2:F
    // B Team, 0:G, 1:C, 2:F
    private Vector3[] bornPosAy = new Vector3[6];

	private List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();

	public EGameSituation Situation = EGameSituation.None;
	public TGameRecord GameRecord = new TGameRecord();
	private TMoveData moveData = new TMoveData();
	private TTacticalData attackTactical;
	private TTacticalData defTactical;
	private TTacticalAction[] tacticalActions;

	//Shoot
	private int shootAngle = 55;
	private float extraScoreRate = 0;
	private float angleByPlayerHoop = 0;

	//Basket
	public EBasketSituation BasketSituation;
	public string BasketAnimationName = "BasketballAction_1";
	private EBasketDistanceAngle basketDistanceAngle = EBasketDistanceAngle.ShortCenter;
	private string[] basketanimationTest = new string[25]{"0","1","2","3","4","5","6","7","8","9","10","11","100","101","102","103","104","105","106","107","108","109","110","111","112"};
	
	//Skill
	private ESkillKind skillKind;
	private Dictionary<string, List<GameObject>> activeSkillTargets = new Dictionary<string, List<GameObject>>();
   
	//Effect
    public GameObject[] passIcon = new GameObject[3];
	private GameObject[] selectIcon = new GameObject[2];

	public EPlayerState testState = EPlayerState.Shoot0;
	public EPlayerState[] ShootStates = new EPlayerState[]{EPlayerState.Shoot0, EPlayerState.Shoot1, EPlayerState.Shoot2, EPlayerState.Shoot3, EPlayerState.Shoot6, EPlayerState.Layup0, EPlayerState.Layup1, EPlayerState.Layup2, EPlayerState.Layup3};
	public static Dictionary<EAnimatorState, bool> LoopStates = new Dictionary<EAnimatorState, bool>();

    void Start()
    {
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

		ChangeSituation (EGameSituation.JumpBall);
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
		PlayerList[aPosAy[1]].Postion = EPlayerPostion.C;
		PlayerList[aPosAy[1]].transform.position = bornPosAy[1];
		PlayerList[aPosAy[1]].ShowPos = 0;
		PlayerList[aPosAy[2]].Postion = EPlayerPostion.F;
		PlayerList[aPosAy[2]].transform.position = bornPosAy[2];
		PlayerList[aPosAy[2]].ShowPos = 2;
		
		//Team B
		PlayerList[bPosAy[0]].Postion = EPlayerPostion.G;
		PlayerList[bPosAy[0]].transform.position = bornPosAy[3];
		PlayerList[bPosAy[0]].ShowPos = 4;
		PlayerList[bPosAy[1]].Postion = EPlayerPostion.C;
		PlayerList[bPosAy[1]].transform.position = bornPosAy[4];
		PlayerList[bPosAy[1]].ShowPos = 3;
		PlayerList[bPosAy[2]].Postion = EPlayerPostion.F;
		PlayerList[bPosAy[2]].transform.position = bornPosAy[5];
		PlayerList[bPosAy[2]].ShowPos = 5;
	}

	private void PlayZreoPosition()
	{
		for(int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i])
				PlayerList[i].transform.position = Vector3.zero;
	}

	public void InitIngameAnimator()
	{
		for(int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i])
				ModelManager.Get.ChangeAnimator(PlayerList[i].AnimatorControl, PlayerList[i].Attribute.BodyType.ToString(), EanimatorType.AnimationControl);
		
		for(int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i].ShowPos != 0 || PlayerList[i].ShowPos != 3)
				PlayerList[i].AniState(EPlayerState.Idle);
	}
	
	public void CreateTeam()
    {
        switch (GameStart.Get.TestMode)
        {
    	case EGameTest.None:
			checkPlayerID();

			switch (GameStart.Get.FriendNumber)
            {
			    case 1:
//				    PlayerList[1].gameObject.SetActive(false);
//				    PlayerList[2].gameObject.SetActive(false);
//				    PlayerList[4].gameObject.SetActive(false);
//				    PlayerList[5].gameObject.SetActive(false);
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], GameData.Team.Player));
                    PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, bornPosAy[3], GameData.EnemyMembers[0].Player));
                    break;
			    case 2:
//				    PlayerList[2].gameObject.SetActive(false);
//				    PlayerList[5].gameObject.SetActive(false);
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
			PlayZreoPosition ();
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
        	PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Npc, new Vector3(0, 0, 11), new GameStruct.TPlayer(0)));
        	UIGame.Get.ChangeControl(true);

        	break;
		case EGameTest.AnimationUnit:
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
			break;
    	case EGameTest.AttackB:
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
			break;
    	case EGameTest.Block:
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, -8.4f), new GameStruct.TPlayer(0)));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Npc, new Vector3 (0, 0, -4.52f), new GameStruct.TPlayer(0)));

			break;
		case EGameTest.OneByOne: 
			TPlayer Self = new TPlayer(0);
			Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			

			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), Self));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Npc, new Vector3 (0, 0, 5), new GameStruct.TPlayer(0)));

        	break;
		case EGameTest.Alleyoop:
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, ETeamKind.Self, new Vector3 (0, 0, 3), new GameStruct.TPlayer(0)));

			break;
		case EGameTest.Pass:
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, ETeamKind.Self, new Vector3 (-5, 0, -2), new GameStruct.TPlayer(0)));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (2, ETeamKind.Self, new Vector3 (5, 0, -2), new GameStruct.TPlayer(0)));

			break;
    	case EGameTest.Edit:
			GameData.Team.Player.SetID(34);		
			GameData.TeamMembers[0].Player.SetID(24);			
			GameData.TeamMembers[1].Player.SetID(14);
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], GameData.Team.Player));	
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Self, bornPosAy[1], GameData.TeamMembers[0].Player));	
			PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Self, bornPosAy[2], GameData.TeamMembers[1].Player));

			break;
		case EGameTest.CrossOver:
			Self = new TPlayer(0);
			Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			
			
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), Self));
			PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Npc, new Vector3 (0, 0, 5), new TPlayer(0)));

			break;
		case EGameTest.Skill:
			if (GameData.Team.Player.ID == 0) 
				GameData.Team.Player.SetID(14);

			PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], GameData.Team.Player));	
			break;
        }

		for (int i = 0; i < PlayerList.Count; i++)
			PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);

        Joysticker = PlayerList [0];

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
			selectIcon[0] = EffectManager.Get.PlayEffect("SelectA", Vector3.zero, null, PlayerList [1].gameObject);
		}

        if (PlayerList.Count > 2 && PlayerList [2].Team == Joysticker.Team) {
			passIcon[2] = EffectManager.Get.PlayEffect("PassB", Joysticker.BodyHeight.transform.localPosition, PlayerList [2].gameObject);
			selectIcon[1] = EffectManager.Get.PlayEffect("SelectB", Vector3.zero, null, PlayerList [2].gameObject);
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
    }

	private void setPassIcon(bool isShow) {
		if(GameStart.Get.TestMode == EGameTest.None) {
			for(int i=0; i<3; i++) {
				//passIcon[i].SetActive(isShow);

				if (i < 2 && selectIcon[i])
					selectIcon[i].SetActive(isShow);
			}
		}
	}

	void FixedUpdate() {
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
				UIGame.Get.DoAttack();
			}

			if (Input.GetKeyDown(KeyCode.N))
			{
				TimerMgr.Get.PauseTime(true);
			}

			if (Input.GetKeyDown(KeyCode.T)){
				UIDoubleClick.Get.ClickStop();
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
					UIGame.Get.DoSteal();
				}

				if(Input.GetKeyDown (KeyCode.S)){
					UIGame.Get.DoBlock();
				}
			}

			if (Input.GetKeyDown (KeyCode.R) && Joysticker != null)
				DoPassiveSkill(ESkillSituation.Rebound, Joysticker);

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
				UIGame.Get.DoSkill();
			}

			if (Situation == EGameSituation.JumpBall || Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)
				jodgeSkillUI();
		}

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
	void OnGUI()
    {
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

//        GUI.Label(new Rect(100, 100, 300, 50), Situation.ToString());
	}
	#endif

	private void jumpBall()
	{
		if(BallOwner == null)
		{
			foreach(PlayerBehaviour someone in PlayerList)
			{
                doPickBall(someone);

//			    if(someone.Team == ETeamKind.Self)
//			    {
//			        doPickBall(someone);
//			        if(someone.DefPlayer != null)
//			            doPickBall(someone.DefPlayer);
//			    }
			}
		}
	}

    private void SituationAttack(ETeamKind team)
    {
        if (PlayerList.Count > 0)
        {
			if (BallOwner != null)
            {
				switch(BallOwner.Postion)
                {
				case EPlayerPostion.C:
                        AITools.RandomTactical(ETactical.Center, out attackTactical);
					break;
				case EPlayerPostion.F:
                        AITools.RandomTactical(ETactical.Forward, out attackTactical);
					break;
				case EPlayerPostion.G:
                        AITools.RandomTactical(ETactical.Guard, out attackTactical);
					break;
				default:
                        AITools.RandomTactical(ETactical.Attack, out attackTactical);
					break;
				}
			} else
                AITools.RandomTactical(ETactical.Attack, out attackTactical);

			bool isShooting = IsShooting;
			for (int i = 0; i < PlayerList.Count; i++) {
                PlayerBehaviour npc = PlayerList [i];
				if (npc.AIing && !DoSkill(npc)) {
					if (npc.Team == team) {                      
						if (!isShooting) {
                        	aiAttack(ref npc);
							aiMove(npc, ref attackTactical);
						} else 
						if (!npc.IsAllShoot) {
                        	aiAttack(ref npc);
							aiMove(npc, ref attackTactical);
						}                                                       
                    } else
                    	aiDefend(ref npc);
                }
            }   
        }
    }

    /// <summary>
    /// 某隊得分, 另一隊執行撿球.
    /// </summary>
    /// <param name="team"> 執行撿球的隊伍(玩家 or 電腦). </param>
    private void SituationPickBall(ETeamKind team)
    {
        if(pickBallPlayer || BallOwner || PlayerList.Count <= 0)
            return;

        pickBallPlayer = findNearBallPlayer(team);

        if(!pickBallPlayer)
            return;

        // 根據撿球員的位置(C,F,G) 選擇適當的進攻和防守戰術.
        if(GameStart.Get.CourtMode == ECourtMode.Full)
        {
//            AITools.RandomTactical(ETactical.Inbounds, pickBallPlayer.Index, out attackTactical);
//            AITools.RandomTactical(ETactical.InboundsDefence, pickBallPlayer.Index, out defTactical);
            AITools.RandomCorrespondingTactical(ETactical.Inbounds, ETactical.InboundsDefence, 
                                                pickBallPlayer.Index, out attackTactical, out defTactical);
        }
        else
        {
//            AITools.RandomTactical(ETactical.HalfInbounds, pickBallPlayer.Index, out attackTactical);
//            AITools.RandomTactical(ETactical.HalfInboundsDefence, pickBallPlayer.Index, out defTactical);
            AITools.RandomCorrespondingTactical(ETactical.HalfInbounds, ETactical.HalfInboundsDefence,
                                                pickBallPlayer.Index, out attackTactical, out defTactical);
        }

        foreach(PlayerBehaviour someone in PlayerList)
        {
            if(someone.Team == team)
            {
                if (someone == pickBallPlayer) 
                    doPickBall(someone);
                else 
                    InboundsBall(someone, team, ref attackTactical);
            }
            else 
                BackToDef(someone, ETeamKind.Npc, ref defTactical);
        }
    }

    private void SituationInbounds(ETeamKind team)
    {
		if(PlayerList.Count > 0 && BallOwner)
		{
		    if(GameStart.Get.CourtMode == ECourtMode.Full)
            {
//                AITools.RandomTactical(ETactical.Inbounds, BallOwner.Index, out attackTactical);
//                AITools.RandomTactical(ETactical.InboundsDefence, BallOwner.Index, out defTactical);
                AITools.RandomCorrespondingTactical(ETactical.Inbounds, ETactical.InboundsDefence,
                                    BallOwner.Index, out attackTactical, out defTactical);
            }
            else
            {
//                AITools.RandomTactical(ETactical.HalfInbounds, BallOwner.Index, out attackTactical);
//                AITools.RandomTactical(ETactical.HalfInboundsDefence, BallOwner.Index, out defTactical);
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

	private void aiShoot(ref PlayerBehaviour Self)
	{
		bool suc = false;
		
		if (Self.IsRebound || Self.IsUseSkill)
			suc = true;
		else
		if(!Self.CheckAnimatorSate(EPlayerState.HoldBall) && haveDefPlayer(ref Self, 5, 40) != 0) {
			int FakeRate = Random.Range (0, 100);
			
			if(FakeRate < GameConst.FakeShootRate) {
				if (PlayerList.Count > 1) {
					for (int i = 0; i < PlayerList.Count; i++) {
						PlayerBehaviour Npc = PlayerList [i];
						
						if (Npc != Self && Npc.Team != Self.Team && getDis(ref Self, ref Npc) <= GameConst.BlockDistance) {
							Self.AniState(EPlayerState.FakeShoot, CourtMgr.Get.ShootPoint [Self.Team.GetHashCode()].transform.position);
							suc = true;
							break;
						}
					}
				}
			}
		}
		
		if (!suc)
			Shoot();
		else
			coolDownPass = 0;
	}

	private void aiPass(ref PlayerBehaviour npc) {
		float angle = 90;
		if ((npc.Team == ETeamKind.Self && npc.transform.position.z > 0) ||
		    (npc.Team == ETeamKind.Npc && npc.transform.position.z < 0))
			angle = 180;

		PlayerBehaviour partner = havePartner(ref npc, 20, angle);
		
		if (partner != null)
			Pass(partner);
		else {
			int Who = Random.Range(0, 2);
			int find = 0;
			
			for (int j = 0; j < PlayerList.Count; j++) {
				if (PlayerList [j].gameObject.activeInHierarchy && PlayerList [j].Team == npc.Team && PlayerList [j] != npc) {
					PlayerBehaviour anpc = PlayerList [j];
					
					if (haveDefPlayer(ref anpc, 1.5f, 40) == 0 || Who == find) {
						Pass(PlayerList [j]);
						break;
					}
					
					find++;
				}
			}
		}
	}
	
	private void aiAttack(ref PlayerBehaviour npc) {
		if (BallOwner) {
			bool dunkRate = Random.Range(0, 100) < 30;
			bool shootRate = Random.Range(0, 100) < 10;
			bool shoot3Rate = Random.Range(0, 100) < 1;
			bool passRate = Random.Range(0, 100) < 20;
			bool pushRate = Random.Range(0, 100) < npc.Attr.PushingRate;
			bool ElbowRate = Random.Range(0, 100) < npc.Attr.ElbowingRate;
			float ShootPointDis = 0;
			Vector3 pos = CourtMgr.Get.ShootPoint [npc.Team.GetHashCode()].transform.position;
			PlayerBehaviour man = null;
			float Shoot3Dis = 2.5f;
			float Shoot3Angel = 40;
			
			if (npc.Attr.PointRate3 >= 0 && npc.Attr.PointRate3 <= 30) {
				Shoot3Dis = 3.5f;
				Shoot3Angel = 40;
			} else 
			if(npc.Attr.PointRate3 >= 31 && npc.Attr.PointRate3 <= 50) {
				Shoot3Dis = 3.5f;
				Shoot3Angel = 40;
			} else {
				Shoot3Dis = 3.5f;
				Shoot3Angel = 40;
			}
			
			if (npc.Team == ETeamKind.Self)
				ShootPointDis = getDis(ref npc, new Vector2(pos.x, pos.z));
			else
				ShootPointDis = getDis(ref npc, new Vector2(pos.x, pos.z));
			
			if (npc == BallOwner) {
				//Dunk shoot shoot3 pass                
				if (ShootPointDis <= GameConst.DunkDistance && (dunkRate || npc.CheckAnimatorSate(EPlayerState.HoldBall)) && CheckAttack(ref npc))
					aiShoot(ref npc);
				else 
				if (ShootPointDis <= GameConst.TwoPointDistance && (haveDefPlayer(ref npc.DefPlayer, 1.5f, 40) == 0 || shootRate || npc.CheckAnimatorSate(EPlayerState.HoldBall)) && CheckAttack(ref npc))
					aiShoot(ref npc);
				else 
				if (ShootPointDis <= GameConst.TreePointDistance + 1 && (haveDefPlayer(ref npc.DefPlayer, Shoot3Dis, Shoot3Angel) == 0 || shoot3Rate || npc.CheckAnimatorSate(EPlayerState.HoldBall)) && CheckAttack(ref npc))
					aiShoot(ref npc);
				else 
				if (ElbowRate && CheckAttack(ref npc) && (haveDefPlayer(ref npc, GameConst.StealBallDistance, 90, out man) != 0) && 
				   npc.CoolDownElbow ==0 && !npc.CheckAnimatorSate(EPlayerState.Elbow)) {
					if (DoPassiveSkill(ESkillSituation.Elbow, npc, man.transform.position)) {
						coolDownPass = 0;
						npc.CoolDownElbow = Time.time + 3;
						RealBallFxTime = 1f;
						CourtMgr.Get.RealBallFX.SetActive(true);
					}
				} else 
				if ((passRate || npc.CheckAnimatorSate(EPlayerState.HoldBall)) && coolDownPass == 0 && !IsShooting && !IsDunk && 
					 !npc.CheckAnimatorSate(EPlayerState.Elbow) && BallOwner.AIing)
					aiPass(ref npc);
				else
				if (npc.IsHaveMoveDodge && CoolDownCrossover == 0 && npc.CanMove) {
					if(Random.Range(0, 100) <=  npc.MoveDodgeRate)
						DoPassiveSkill(ESkillSituation.MoveDodge, npc);
				}
			} 
			else {
				//sup push 
				PlayerBehaviour NearPlayer = haveNearPlayer(npc, GameConst.StealBallDistance, false);
				
				if (NearPlayer && pushRate && npc.CoolDownPush == 0) { //Push
					if(DoPassiveSkill(ESkillSituation.Push0, npc, NearPlayer.transform.position))
						npc.CoolDownPush = Time.time + 3;                    
				} 
			}   
		}
	}

	private void aiDefend(ref PlayerBehaviour npc)
	{
		if (npc.AIing && !npc.IsSteal && !npc.CheckAnimatorSate(EPlayerState.Push0) && 
		    BallOwner && !IsDunk && !IsShooting) {
			bool pushRate = Random.Range(0, 100) < npc.Attr.PushingRate;        
			bool sucess = false;

			TPlayerDisData [] DisAy = GetPlayerDisAy(npc);
			
			for (int i = 0; i < DisAy.Length; i++) {
				if (DisAy[i].Distance <= GameConst.StealBallDistance && 
				    (DisAy[i].Player.crtState == EPlayerState.Idle && DisAy[i].Player.crtState == EPlayerState.Dribble0) && 
				    pushRate && npc.CoolDownPush == 0) {
					if(DoPassiveSkill(ESkillSituation.Push0, npc, DisAy[i].Player.transform.position)) {
						npc.CoolDownPush = Time.time + GameConst.CoolDownPushTime;
						sucess = true;
						
						break;
					}
				} 
			}
			
			if (!sucess && DisAy[0].Distance <= GameConst.StealBallDistance && waitStealTime == 0 && BallOwner.Invincible == 0 && npc.CoolDownSteal == 0) {
				if(Random.Range(0, 100) < npc.Attr.StealRate) {
					if(DoPassiveSkill(ESkillSituation.Steal0, npc, BallOwner.gameObject.transform.position)) {
						npc.CoolDownSteal = Time.time + GameConst.CoolDownSteal;                              
						waitStealTime = Time.time + GameConst.WaitStealTime;
					}
				}
			}           
		}
	}

	private void aiMove(PlayerBehaviour someone, ref TTacticalData tacticalData)
    {
		if (BallOwner == null)
        {
			if(!Passer)
            {
				if(Shooter == null)
				{
				    // doPickBall(npc, true);
				    nearestBallPlayerDoPickBall(someone);
				    // doPickBall(npc.DefPlayer, true);
                    nearestBallPlayerDoPickBall(someone.DefPlayer);
                }
				else
                {
					if((Situation == EGameSituation.AttackA && someone.Team == ETeamKind.Self) || (Situation == EGameSituation.AttackB && someone.Team == ETeamKind.Npc))
					    if(!someone.IsShoot)
					    {
                            // doPickBall(npc, true);
                            nearestBallPlayerDoPickBall(someone);
                        }
					
					if((Situation == EGameSituation.AttackA && someone.DefPlayer.Team == ETeamKind.Npc) || 
					   (Situation == EGameSituation.AttackB && someone.DefPlayer.Team == ETeamKind.Self)) {
						PlayerBehaviour fearPlayer = null;
						
						for (int i = 0; i < PlayerList.Count; i++) {
							PlayerBehaviour Npc1 = PlayerList [i];
							if (Npc1.Team == someone.DefPlayer.Team && !someone.DefPlayer.IsFall && someone.DefPlayer.AIing) {
								if (fearPlayer == null)
									fearPlayer = Npc1;
								else 
									if (getDis(ref fearPlayer, CourtMgr.Get.RealBall.transform.position) < getDis(ref Npc1, CourtMgr.Get.RealBall.transform.position))
										fearPlayer = Npc1;
							}
						}
						
						if (fearPlayer) {
							for (int i = 0; i < PlayerList.Count; i++) {
								if(fearPlayer.Team == PlayerList[i].Team) {
									if(PlayerList[i] != fearPlayer) {
										if (PlayerList[i] != null && PlayerList[i].CanMove && PlayerList[i].WaitMoveTime == 0) {
											moveData.Clear();
											moveData.FollowTarget = CourtMgr.Get.RealBall.transform;
											PlayerList[i].TargetPos = moveData;
										}
									}
								}
							}
						}
					}
				}
			}
		} else {
			if (someone.CanMove && someone.TargetPosNum == 0) {
				for(int i = 0; i < PlayerList.Count; i++) {
					if(PlayerList[i].Team == someone.Team && PlayerList[i] != someone && 
					   tacticalData.FileName != string.Empty && PlayerList[i].TargetPosName != tacticalData.FileName)
						PlayerList[i].ResetMove();
		}
				
				if(tacticalData.FileName != string.Empty) {
					tacticalActions = tacticalData.GetActions(someone.Postion.GetHashCode());
					
					if (tacticalActions != null) {
						for (int i = 0; i < tacticalActions.Length; i++) {
                        moveData.Clear();
							moveData.Speedup = tacticalActions [i].Speedup;
							moveData.Catcher = tacticalActions [i].Catcher;
							moveData.Shooting = tacticalActions [i].Shooting;
                        int z = 1;
							if (GameStart.Get.CourtMode == ECourtMode.Full && someone.Team != ETeamKind.Self)
                            z = -1;
							
							moveData.Target = new Vector2(tacticalActions [i].x, tacticalActions [i].z * z);
							if (BallOwner != null && BallOwner != someone)
								moveData.LookTarget = BallOwner.transform;  
							
							moveData.TacticalName = tacticalData.FileName;
							moveData.MoveFinish = DefMove;
							someone.TargetPos = moveData;
                    }
						
						DefMove(someone);
                }
            }
        }
			
			if (someone.WaitMoveTime != 0 && BallOwner != null && someone == BallOwner)
				someone.AniState(EPlayerState.Dribble0);
		}
    }

    public bool DefMove(PlayerBehaviour player, bool speedup = false)
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
							PlayerBehaviour p = haveNearPlayer(player.DefPlayer, player.DefPlayer.Attr.DefDistance, false, true);
							if (p != null)
								moveData.DefPlayer = p;
							else 
								if (getDis(ref player, ref player.DefPlayer) <= player.DefPlayer.Attr.DefDistance)
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
//                    doPickBall(player.DefPlayer, true);
                    nearestBallPlayerDoPickBall(player.DefPlayer);
                }
            }
        }
        
        return true;
    }
    
    public void ChangeSituation(EGameSituation newSituation, PlayerBehaviour player = null)
    {
		if(Situation != EGameSituation.End || newSituation == EGameSituation.Opening)
        {
            EGameSituation oldgs = Situation;
            if(Situation != newSituation)
            {
                RealBallFxTime = 0;
                waitStealTime = 0;
                CourtMgr.Get.RealBallFX.SetActive(false);
                for(int i = 0; i < PlayerList.Count; i++)
                {
                    if(newSituation == EGameSituation.APickBallAfterScore || newSituation == EGameSituation.BPickBallAfterScore)
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

//            Debug.LogFormat("{0} -> {1}", Situation, newSituation);

            Situation = newSituation;

            if(GameStart.Get.CourtMode == ECourtMode.Full && oldgs != newSituation && player &&
               (oldgs == EGameSituation.InboundsA || oldgs == EGameSituation.InboundsB))
            {
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
			case EGameSituation.Opening:
				setPassIcon(true);
				UIGame.UIShow (true);
				UIGame.Get.UIState(EUISituation.Opening);
				jodgeSkillUI ();

				break;
			case EGameSituation.JumpBall:
				IsStart = true;
				CourtMgr.Get.InitScoreboard (true);
				setPassIcon(true);

				PlayerBehaviour npc = findJumpBallPlayer(ETeamKind.Self);
				if (npc) {
//					npc.transform.position = bornPosAy[1];
					JumpBall(npc);
//					Rebound(npc);
				}

				npc = findJumpBallPlayer(ETeamKind.Npc);
				if (npc) {
//					npc.transform.position = bornPosAy[1];
					JumpBall(npc);
//					Rebound(npc);
				}

				break;
			case EGameSituation.AttackA:
				CameraMgr.Get.SetCameraSituation(ECameraSituation.Self);
				jodgeSkillUI ();

				if (Joysticker && GameData.Setting.AIChangeTime > 100)
					Joysticker.SetNoAI();

				break;
			case EGameSituation.AttackB:
				CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc);
				jodgeSkillUI ();

				if (Joysticker && GameData.Setting.AIChangeTime > 100)
					Joysticker.SetNoAI();

				break;
			case EGameSituation.APickBallAfterScore:
				CourtMgr.Get.Walls[1].SetActive(false);
				UIGame.Get.ChangeControl(true);
				CameraMgr.Get.SetCameraSituation(ECameraSituation.Self, true);
				pickBallPlayer = null;

                break;
            case EGameSituation.InboundsA:
				CourtMgr.Get.Walls[1].SetActive(false);
				EffectManager.Get.PlayEffect("ThrowInLineEffect", Vector3.zero);
                break;
            case EGameSituation.BPickBallAfterScore:
				CourtMgr.Get.Walls[0].SetActive(false);
           	 	UIGame.Get.ChangeControl(false);
				CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc, true);
				pickBallPlayer = null;

                break;
			case EGameSituation.InboundsB:
				CourtMgr.Get.Walls[0].SetActive(false);
				EffectManager.Get.PlayEffect("ThrowInLineEffect", Vector3.zero);

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
	                    break;
	                case EGameSituation.JumpBall:
						jumpBall();
	                    break;
	                case EGameSituation.AttackA:
	                    SituationAttack(ETeamKind.Self);
	                    break;
	                case EGameSituation.AttackB:
	                    SituationAttack(ETeamKind.Npc);
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

	private void jodgeSkillUI (){
		if (Joysticker && Joysticker.Attribute.ActiveSkill.ID > 0) {

			CourtMgr.Get.SkillAera((int)Joysticker.Team, Joysticker.IsAngerFull);

			List<GameObject> target = getActiveSkillTarget(Joysticker);
			bool result = false;
			for(int i=0; i<target.Count; i++) {
				if(CheckSkill(Joysticker, target[i])) {
					result = true;
				}
			}
			UIGame.Get.ShowSkillUI(IsStart, Joysticker.IsAngerFull, result);
		}
	}
	
	private void jodgeShootAngle(PlayerBehaviour player){
		//Angle
		float angle = 0;
		int distanceType = 0;
		if(player.name.Contains("Self")) {
			angleByPlayerHoop = GameFunction.GetPlayerToObjectAngle(CourtMgr.Get.Hood[0].transform, player.gameObject.transform);
			angle = Mathf.Abs(angleByPlayerHoop) - 90;
		} else {
			angleByPlayerHoop = GameFunction.GetPlayerToObjectAngle(CourtMgr.Get.Hood[1].transform, player.gameObject.transform);
			angle = Mathf.Abs(angleByPlayerHoop) - 90;
		}
		//Distance
		if(shootDistance >= 0 && shootDistance < 9) {
			distanceType = 0;
		} else 
		if(shootDistance >= 9 && shootDistance < 12) {
			distanceType = 1;
		} else 
		if(shootDistance >= 12) {
			distanceType = 2;
		}
		//Angle
		if(angle > 60) {// > 60 degree
			if(distanceType == 0){
				basketDistanceAngle = EBasketDistanceAngle.ShortCenter;
			}else if (distanceType == 1){
				basketDistanceAngle = EBasketDistanceAngle.MediumCenter;
			}else if (distanceType == 2){
				basketDistanceAngle = EBasketDistanceAngle.LongCenter;
			}
		} else 
		if(angle <= 60 && angle > 10){// > 10 degree <= 60 degree
			if(angleByPlayerHoop > 0) {//right
				if(distanceType == 0){
					basketDistanceAngle = EBasketDistanceAngle.ShortRight;
				}else if (distanceType == 1){
					basketDistanceAngle = EBasketDistanceAngle.MediumRight;
				}else if (distanceType == 2){
					basketDistanceAngle = EBasketDistanceAngle.LongRight;
				}
			} else {//left
				if(distanceType == 0){
					basketDistanceAngle = EBasketDistanceAngle.ShortLeft;
				}else if (distanceType == 1){
					basketDistanceAngle = EBasketDistanceAngle.MediumLeft;
				}else if (distanceType == 2){
					basketDistanceAngle = EBasketDistanceAngle.LongLeft;
				}
			}
		} else 
		if(angle <= 10 && angle >= -30){ // < 10 degree
			if(angleByPlayerHoop > 0) { // right
				if(distanceType == 0){
					basketDistanceAngle = EBasketDistanceAngle.ShortRightWing;
				}else if (distanceType == 1){
					basketDistanceAngle = EBasketDistanceAngle.MediumRightWing;
				}else if (distanceType == 2){
					basketDistanceAngle = EBasketDistanceAngle.LongRightWing;
				}
			} else { //left
				if(distanceType == 0){
					basketDistanceAngle = EBasketDistanceAngle.ShortLeftWing;
				}else if (distanceType == 1){
					basketDistanceAngle = EBasketDistanceAngle.MediumLeftWing;
				}else if (distanceType == 2){
					basketDistanceAngle = EBasketDistanceAngle.LongLeftWing;
				}
			}
		}
	}

	private void judgeBasketAnimationName (int basketDistanceAngleType) {
		if(BasketSituation == EBasketSituation.Score){
			if(CourtMgr.Get.DBasketAnimationName.Count > 0 && basketDistanceAngleType < CourtMgr.Get.DBasketAnimationName.Count){
				int random = Random.Range(0, CourtMgr.Get.DBasketAnimationName[basketDistanceAngleType].Count);
				if(CourtMgr.Get.DBasketAnimationName.Count > 0 && random < CourtMgr.Get.DBasketAnimationName.Count)
					BasketAnimationName = CourtMgr.Get.DBasketAnimationName[basketDistanceAngleType][random];
			}
		}else if(BasketSituation == EBasketSituation.NoScore){
			if(CourtMgr.Get.DBasketAnimationNoneState.Count > 0 && basketDistanceAngleType < CourtMgr.Get.DBasketAnimationName.Count) {
				int random = Random.Range(0, CourtMgr.Get.DBasketAnimationNoneState[basketDistanceAngleType].Count);
				if(CourtMgr.Get.DBasketAnimationNoneState.Count > 0 && random < CourtMgr.Get.DBasketAnimationName.Count)
					BasketAnimationName = CourtMgr.Get.DBasketAnimationNoneState[basketDistanceAngleType][random];
			}
		}
		if(string.IsNullOrEmpty(BasketAnimationName))
			judgeBasketAnimationName(basketDistanceAngleType);
	}

	private void calculationScoreRate(PlayerBehaviour player, EScoreType type) {
		//Score Rate
		float originalRate = 0;
		if(shootDistance >= GameConst.TreePointDistance) {
			originalRate = player.Attr.PointRate3;
			EffectManager.Get.PlayEffect("ThreeLineEffect", Vector3.zero, null, null, 0);
		} else {
			originalRate = player.Attr.PointRate2;
		}
		float rate = (Random.Range(0, 100) + 1);
		int airRate = (Random.Range(0, 100) + 1);
		bool isScore = false;
		bool isSwich = false;
		bool isAirBall = false;
		if(type == EScoreType.DownHand) {
			isScore = rate <= (originalRate - (originalRate * (player.ScoreRate.DownHandScoreRate / 100f)) + extraScoreRate) ? true : false;
			if(isScore) {
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.DownHandSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.DownHandAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.UpHand) {
			isScore = rate <= (originalRate - (originalRate * (player.ScoreRate.UpHandScoreRate / 100f)) + extraScoreRate) ? true : false;
			if(isScore) {
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.UpHandSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.UpHandAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.Normal) {
			isScore = rate <= (originalRate - (originalRate * (player.ScoreRate.NormalScoreRate / 100f)) + extraScoreRate) ? true : false;
			if(isScore) {
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.NormalSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.NormalAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.NearShot) {
			isScore = rate <= (originalRate + (originalRate * (player.ScoreRate.NearShotScoreRate / 100f)) + extraScoreRate) ? true : false;
			if(isScore) {
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.NearShotSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.NearShotAirBallRate ? true : false;
			}
		} else 
		if(type == EScoreType.LayUp) {
			isScore = rate <= (originalRate + (originalRate * (player.ScoreRate.LayUpScoreRate / 100f)) + extraScoreRate) ? true : false;
			if(isScore) {
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.LayUpSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.LayUpAirBallRate ? true : false;
			}
		}
	
		if(extraScoreRate == GameData.ExtraPerfectRate || shootDistance < 9)
			isAirBall = false;

		if(shootDistance > 15) 
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
			BasketSituation = EBasketSituation.Score;
			if(BasketSituation == EBasketSituation.Score || BasketSituation == EBasketSituation.NoScore){
				if((int)GameStart.Get.SelectBasketState > 100)
					BasketSituation = EBasketSituation.NoScore;
				BasketAnimationName = "BasketballAction_" + basketanimationTest[(int)GameStart.Get.SelectBasketState];
				UIHint.Get.ShowHint("BasketAnimationName: "+BasketAnimationName, Color.yellow);
			}
		}

		if (shootDistance >= GameConst.TreePointDistance)
			player.GameRecord.FG3++;
		else
			player.GameRecord.FG++;
		
		player.GameRecord.PushShot(new Vector2(player.transform.position.x, player.transform.position.z), BasketSituation.GetHashCode(), rate);
	}

	public void AddExtraScoreRate(float rate) {
		extraScoreRate = rate;
	}

	public bool Shoot() {
        if (BallOwner) {
			Vector3 v = CourtMgr.Get.ShootPoint [BallOwner.Team.GetHashCode()].transform.position;
			shootDistance = getDis(ref BallOwner, new Vector2(v.x, v.z));

			if(GameStart.Get.TestMode == EGameTest.Shoot) {
				BallOwner.AniState(testState, CourtMgr.Get.Hood[BallOwner.Team.GetHashCode()].transform.position);
				return true;
			} else 
			if (!BallOwner.IsDunk) {
				extraScoreRate = 0;
				UIGame.Get.DoPassNone();
				CourtMgr.Get.ResetBasketEntra();

				int t = BallOwner.Team.GetHashCode();
				if (GameStart.Get.TestMode == EGameTest.Dunk) {
					BallOwner.AniState(EPlayerState.Dunk20, CourtMgr.Get.ShootPoint [t].transform.position);
					return true;
				} else 
				if (BallOwner.IsRebound) {
					if (inTipinDistance(BallOwner)) {
						BallOwner.AniState(EPlayerState.TipIn, CourtMgr.Get.ShootPoint [t].transform.position);
						return true;
					}
				} else {
					if (BallOwner.IsMoving) {
						if (shootDistance > 15)
							DoPassiveSkill(ESkillSituation.Shoot3, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						else 
						if (shootDistance > 9 && shootDistance <= 15) {
							if (Random.Range(0, 2) == 0)
								DoPassiveSkill(ESkillSituation.Shoot2, BallOwner, CourtMgr.Get.Hood [t].transform.position);
							else
								DoPassiveSkill(ESkillSituation.Shoot0, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						} else 
						if (shootDistance > 7 && shootDistance <= 9) {
							float rate = Random.Range(0, 100);
							if(rate < BallOwner.Attr.DunkRate)
								DoPassiveSkill(ESkillSituation.Dunk0, BallOwner, CourtMgr.Get.ShootPoint [t].transform.position);
							else
								DoPassiveSkill(ESkillSituation.Layup0, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						} else {
							float rate = Random.Range(0, 100);
							if (rate < BallOwner.Attr.DunkRate)
								DoPassiveSkill(ESkillSituation.Dunk0, BallOwner, CourtMgr.Get.ShootPoint [t].transform.position);
							else {
								if(haveDefPlayer(ref BallOwner, 1.5f, 40) == 0)
									DoPassiveSkill(ESkillSituation.Layup0, BallOwner, CourtMgr.Get.Hood [t].transform.position);
								else
									DoPassiveSkill(ESkillSituation.Shoot1, BallOwner, CourtMgr.Get.Hood [t].transform.position);
							}
						}
					} else {
						if (shootDistance > 15)
							DoPassiveSkill(ESkillSituation.Shoot3, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						else 
						if (shootDistance > 9 && shootDistance <= 15)
							DoPassiveSkill(ESkillSituation.Shoot0, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						else
							DoPassiveSkill(ESkillSituation.Shoot1, BallOwner, CourtMgr.Get.Hood [t].transform.position);
					}

					return true;
				}
			}
        }

		return false;
	}
        
    public bool OnShooting(PlayerBehaviour player)
    {
        if (BallOwner && BallOwner == player)
		{                   
			Shooter = player;
			SetBallOwnerNull();
			UIGame.Get.SetPassButton();

			EScoreType st = EScoreType.Normal;
			if(player.name.Contains("Self")) 
				angleByPlayerHoop = GameFunction.GetPlayerToObjectAngle(CourtMgr.Get.Hood[0].transform, player.gameObject.transform);
			else 
				angleByPlayerHoop = GameFunction.GetPlayerToObjectAngle(CourtMgr.Get.Hood[1].transform, player.gameObject.transform);

			if(Mathf.Abs(angleByPlayerHoop) <= 85  && shootDistance < 5)
				shootAngle = 80;
			else
				shootAngle = 50;

			if(player.crtState == EPlayerState.TipIn){
				st = EScoreType.LayUp;
				player.GameRecord.TipinLaunch++;
			} else 
			if(skillKind == ESkillKind.NearShoot) 
				st = EScoreType.NearShot;
			else 
			if(skillKind == ESkillKind.UpHand) 
				st = EScoreType.UpHand;
			else 
			if(skillKind == ESkillKind.DownHand) 
				st = EScoreType.DownHand;
			else 
			if(skillKind == ESkillKind.Layup) {
				st = EScoreType.LayUp;
			}
			
			jodgeShootAngle(player);
			judgeBasketAnimationName ((int)basketDistanceAngle);
			calculationScoreRate(player, st);

			SetBall();
            CourtMgr.Get.RealBall.transform.localEulerAngles = Vector3.zero;
			CourtMgr.Get.SetBallState(player.crtState);

			if(BasketSituation == EBasketSituation.AirBall) {
				//AirBall
//				#if UNITY_EDITOR
//				UIHint.Get.ShowHint("AirBall", Color.yellow);
//				#endif
				Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Ignore Raycast"), LayerMask.NameToLayer ("RealBall"), true);
				Vector3 ori = CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position - CourtMgr.Get.RealBall.transform.position;
				CourtMgr.Get.RealBallVelocity = GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
					                         CourtMgr.Get.RealBall.transform.position + (ori * 0.8f), shootAngle);
			} else 
			if(player.crtState == EPlayerState.TipIn) {
				if(CourtMgr.Get.RealBall.transform.position.y > (CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.y + 0.2f)) {

					CourtMgr.Get.RealBallDoMove(new Vector3(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.x,
					                                       CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.y + 0.5f,
					                                       CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.z), 1 / TimerMgr.Get.CrtTime * 0.5f);
				} else {
					CourtMgr.Get.RealBallDoMove(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position, 1/ TimerMgr.Get.CrtTime * 0.2f); //0.2f	
				}
			} 
			else 
			if(BasketSituation == EBasketSituation.Swish) {
//				#if UNITY_EDITOR
//				UIHint.Get.ShowHint("Swish", Color.yellow);
//				#endif
				Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("BasketCollider"), LayerMask.NameToLayer ("RealBall"), true);
				CourtMgr.Get.RealBallVelocity = GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
					                         CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position , shootAngle);	
			} else {
				if(CourtMgr.Get.DBasketShootWorldPosition.ContainsKey (player.Team.GetHashCode().ToString() + "_" + BasketAnimationName)) {
					float dis = getDis(new Vector2(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z),
					                   new Vector2(CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].x, CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].z));
					if(dis>10)
						dis = 10;
					CourtMgr.Get.RealBallVelocity = GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
						                         							 CourtMgr.Get.DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName],
						                         							 shootAngle,
					                                                         dis * 0.05f);

				} else 
					Debug.LogError("No key:"+player.Team.GetHashCode().ToString() + "_" + BasketAnimationName);
			}

            for (int i = 0; i < PlayerList.Count; i++)
                if (PlayerList [i].Team == Shooter.Team)
                    PlayerList [i].ResetMove();

			return true;
        } else
            return false;
    }

	public bool DoShoot(bool isshoot)
    {
		if (IsStart && CandoBtn) {
			if (UIDoubleClick.Visible) {
				UIDoubleClick.Get.ClickStop ();
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
					return Shoot ();
				else
					return Joysticker.AniState(EPlayerState.FakeShoot, CourtMgr.Get.ShootPoint [Joysticker.Team.GetHashCode()].transform.position);
            } else //someone else shot
			if (BallOwner && BallOwner.Team == ETeamKind.Self) {
				Shoot();
			} else 
			if (!Joysticker.IsRebound)
				return Rebound(Joysticker);
        }

		return false;
    }

	public bool DoPush()
	{
		if (Joysticker) {
			PlayerBehaviour nearP = FindNearNpc();
			if(nearP)
				return DoPassiveSkill (ESkillSituation.Push0, Joysticker, nearP.transform.position);
			else
				return DoPassiveSkill (ESkillSituation.Push0, Joysticker);
		} else
			return false;
	}

	public bool DoElbow()
	{
		if (Joysticker)
			return DoPassiveSkill (ESkillSituation.Elbow, Joysticker);
		else
			return false;
	}

	public bool OnOnlyScore(PlayerBehaviour player) {
		if (player == BallOwner)
		{
			PlusScore(player.Team.GetHashCode(), true, false);

			player.GameRecord.Dunk++;
			if (shootDistance >= GameConst.TreePointDistance)
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

            CourtMgr.Get.SetBallState(EPlayerState.DunkBasket);
			PlusScore(player.Team.GetHashCode(), player.IsUseSkill, true);
            SetBall();

			player.GameRecord.ShotError++;
			player.GameRecord.Dunk++;
			if (shootDistance >= GameConst.TreePointDistance)
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
            shootDistance = getDis(ref Shooter, new Vector2(v.x, v.z));
			player.GameRecord.DunkLaunch++;
            return true;
        } else
            return false;
    }
    
    public bool Pass(PlayerBehaviour player, bool IsTee = false, bool IsBtn = false, bool MovePass = false) {
		bool Result = false;
		bool CanPass = true;

		if(BallOwner) {
			#if UNITY_EDITOR
			if(GameStart.Get.TestMode == EGameTest.Pass) {
				if(BallOwner.IsMoving) {
					float angle = GameFunction.GetPlayerToObjectAngleByVector(BallOwner.gameObject.transform, player.gameObject.transform.position);
					if (angle < 60f && angle > -60f){
						UIHint.Get.ShowHint("Direct Forward and Angle:" + angle, Color.yellow);
						Result = BallOwner.AniState(EPlayerState.Pass5);
					} else 
					if (angle <= -60f && angle > -120f){
						UIHint.Get.ShowHint("Direct Left and Angle:" + angle, Color.yellow);
						Result = BallOwner.AniState(EPlayerState.Pass7);
					} else 
					if (angle < 120f && angle >= 60f){
						UIHint.Get.ShowHint("Direct Right and Angle:" + angle, Color.yellow);
						Result = BallOwner.AniState(EPlayerState.Pass8);
					} else 
					if (angle >= 120f || angle <= -120f){
						UIHint.Get.ShowHint("Direct Back and Angle:" + angle, Color.yellow);
						if (Random.Range(0, 100) < 50)
							Result = BallOwner.AniState(EPlayerState.Pass9);
						else 
							Result = BallOwner.AniState(EPlayerState.Pass6);
					}
				} else 
					Result = BallOwner.AniState(EPlayerState.Pass0, player.gameObject.transform.position);
				
				if(Result){
					Catcher = player;
					UIGame.Get.DoPassNone();
				}
				
				return Result;
			}
			#endif

			if(IsShooting)
			{
				if(player.Team == ETeamKind.Self)
				{
					if(!IsBtn)
						CanPass = false;
					else 
					if(!IsCanPassAir)
						CanPass = false;
				}
				else if(player.Team == ETeamKind.Npc && !IsCanPassAir)
					CanPass = false;
			}

			if (!IsPassing && CanPass && !IsDunk && player != BallOwner)
			{
				if(!(IsBtn || MovePass) && coolDownPass != 0)
					return Result;
				
				if(!IsBtn && !BallOwner.AIing)
					return Result;
				
				if(IsTee)
				{
					if(BallOwner.AniState(EPlayerState.Pass50, player.transform.position))
					{
						Catcher = player;
						Result = true;
					}												
				}else if(IsCanPassAir && !IsTee)
				{
					if(BallOwner.AniState(EPlayerState.Pass4, player.transform.position))
					{
						Catcher = player;
						Result = true;
					}
				}
				else
				{
					float dis = Vector3.Distance(BallOwner.transform.position, player.transform.position);
					int disKind = GetEnemyDis(ref player);
					int rate = UnityEngine.Random.Range(0, 2);
					if(player.crtState == EPlayerState.Alleyoop)
						Result = BallOwner.AniState(EPlayerState.Pass3, player.transform.position);
					else
					if(dis <= GameConst.FastPassDistance)
					{
						Result = DoPassiveSkill(ESkillSituation.Pass4, BallOwner, player.transform.position);
					}
					else if(dis <= GameConst.CloseDistance)
					{
						//Close
						if(disKind == 1)
						{
							if(rate == 1){
								Result = DoPassiveSkill(ESkillSituation.Pass1, BallOwner, player.transform.position);
							}else{ 
								Result = DoPassiveSkill(ESkillSituation.Pass2, BallOwner, player.transform.position);
							}
						} else 
						if(disKind == 2)
						{
							if(rate == 1){
								Result = DoPassiveSkill(ESkillSituation.Pass0, BallOwner, player.transform.position);
							}else{
								Result = DoPassiveSkill(ESkillSituation.Pass2, BallOwner, player.transform.position);
							}
						}						
						else
						{
							if(rate == 1){
								Result = DoPassiveSkill(ESkillSituation.Pass0, BallOwner, player.transform.position);
							}else{
								Result = DoPassiveSkill(ESkillSituation.Pass2, BallOwner, player.transform.position);
							}
						}
					}else if(dis <= GameConst.MiddleDistance)
					{
						//Middle
						if(disKind == 1)
						{
							if(rate == 1){
								Result = DoPassiveSkill(ESkillSituation.Pass0, BallOwner, player.transform.position);
							}else{
								Result = DoPassiveSkill(ESkillSituation.Pass2, BallOwner, player.transform.position);
							}
						} else 
							if(disKind == 2)
						{
							if(rate == 1){
								Result = DoPassiveSkill(ESkillSituation.Pass1, BallOwner, player.transform.position);
							}else{
								Result = DoPassiveSkill(ESkillSituation.Pass2, BallOwner, player.transform.position);
							}
						}						
						else
						{
							if(rate == 1){
								Result = DoPassiveSkill(ESkillSituation.Pass0, BallOwner, player.transform.position);
							}else{
								Result = DoPassiveSkill(ESkillSituation.Pass2, BallOwner, player.transform.position);
							}
						}
					}else{
						//Far
						Result = DoPassiveSkill(ESkillSituation.Pass1, BallOwner, player.transform.position);
					}
					
					if(Result){
						Catcher = player;
						if (BallOwner && (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)) 
							BallOwner.GameRecord.Pass++;
						
						UIGame.Get.DoPassNone();
					}
				}
			}
		}

		return Result;
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
			if(Vector3.Distance(BallOwner.transform.position, player.transform.position) <= GameConst.StealBallDistance) {
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
				
				if (stealRate <= (r + AddRate) && Mathf.Abs(GetAngle(BallOwner.transform, player.transform)) <= 90 + AddAngle) {
					if(BallOwner && BallOwner.AniState(EPlayerState.GotSteal)) {
						BallOwner.SetAnger(GameConst.DelAnger_Stealed);
						return true;
					}
				} else 
				if(BallOwner != null && haveStealPlayer(ref player, ref BallOwner, GameConst.StealBallDistance, 15) != 0) {
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
				return DoPassiveSkill(ESkillSituation.Steal0, Joysticker, BallOwner.transform.position);
            } else
				return DoPassiveSkill(ESkillSituation.Steal0, Joysticker);
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
		if (player.Team == ETeamKind.Self && !UIDoubleClick.Visible && (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)) {
			GameRecord.DoubleClickLaunch++;

			switch (state) {
				case EPlayerState.Shoot0:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData( 1.3f, DoubleShoot);
					return true;
				case EPlayerState.Shoot1:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData( 1.23f, DoubleShoot);
					return true;
				case EPlayerState.Shoot2:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData( 1.3f, DoubleShoot);
					return true;
				case EPlayerState.Shoot3:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData( 1.3f, DoubleShoot);
					return true;
				case EPlayerState.Shoot6:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData( 1.3f, DoubleShoot);
					return true;
				case EPlayerState.Layup0:
				case EPlayerState.Layup1:
				case EPlayerState.Layup2:
				case EPlayerState.Layup3:
					UIDoubleClick.Get.SetData(1.3f, DoubleShoot);
					UIDoubleClick.UIShow(true);
					return true;

				case EPlayerState.Block:
				case EPlayerState.BlockCatch:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData(0.7f, null, DoubleBlock, player);
					return true;
				case EPlayerState.Rebound:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData(0.75f, DoubleRebound);
					return true;
			}
		}
		return false;
	}

	public void DoubleShoot(int lv)
	{
		switch (lv) {
			case 0: 
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
			break;
		case 1: 
			AddExtraScoreRate(GameData.ExtraGreatRate);
			Shoot();
			break;
		case 2: 
			AddExtraScoreRate(GameData.ExtraPerfectRate);
			Shoot();
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
				if(UIDoubleClick.Visible)
					UIDoubleClick.Get.ClickStop();

				return true;
			} else {
				if (Shooter)
					return DoPassiveSkill(ESkillSituation.Block, Joysticker, Shooter.transform.position);
	            else
	            if (BallOwner) {
					Joysticker.RotateTo(BallOwner.gameObject.transform.position.x, BallOwner.gameObject.transform.position.z); 
					return DoPassiveSkill(ESkillSituation.Block, Joysticker, BallOwner.transform.position);
				} else {
					if (!Shooter && inReboundDistance(Joysticker) && GameStart.Get.TestMode == EGameTest.None)
						return Rebound(Joysticker);
					else
						return DoPassiveSkill(ESkillSituation.Block, Joysticker);
				}
			}
        }

		return false;
    }

	private bool inReboundDistance(PlayerBehaviour player) {
		return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), 
		                        new Vector2(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z)) <= 6;
	}

	private bool inTipinDistance(PlayerBehaviour player) {
		return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), 
		                        new Vector2(CourtMgr.Get.ShootPoint[player.Team.GetHashCode()].transform.position.x, 
		            						CourtMgr.Get.ShootPoint[player.Team.GetHashCode()].transform.position.z)) <= 6;
	}

    private bool Rebound(PlayerBehaviour player)
    {
		return DoPassiveSkill(ESkillSituation.Rebound, player, CourtMgr.Get.RealBall.transform.position);
	}

	private bool JumpBall(PlayerBehaviour player)
	{
		return DoPassiveSkill(ESkillSituation.JumpBall, player, CourtMgr.Get.RealBall.transform.position);
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

	private bool DoSkill(PlayerBehaviour player) {
		bool result = false;
		if (player.CanUseSkill && player.Attribute.ActiveSkill.ID > 0) {
			List<GameObject> target = getActiveSkillTarget(player);
			if(target != null && target.Count > 0) {
				for(int i=0; i<target.Count; i++){
					if (CheckSkill(player, target[i])) 
						result = true;
				}
				if (result) {
					attackSkillEffect(player);
					result = player.ActiveSkill(player.gameObject);
				}
			}
		}
		return result;
	}

	private void attackSkillEffect(PlayerBehaviour player) {
		Vector3 v;
		TSkillData skill = GameData.DSkillData[player.Attribute.ActiveSkill.ID];
		switch (skill.Kind) {
		case 6://dunk
			v = CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position;
			shootDistance = getDis(ref player, new Vector2(v.x, v.z));
			break;
		case 7://double dunk
			v = CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position;
			shootDistance = getDis(ref player, new Vector2(v.x, v.z));
			break;
		case 15://steal
		case 17://push
			break;
		case 21://buffer
			switch (skill.TargetKind) {
			case 0:
			case 1:
			case 10:
				player.AddSkillAttribute(skill.ID, 
				                         skill.AttrKind, 
				                         skill.Value(player.Attribute.ActiveSkill.Lv), 
				                         skill.LifeTime(player.Attribute.ActiveSkill.Lv));
				break;
			case 3:
				for (int i = 0; i < PlayerList.Count; i++) {
					if (PlayerList[i].Team.GetHashCode() == player.Team.GetHashCode()) {
						if(CheckSkill(player, PlayerList[i].gameObject)) {
							PlayerList[i].AddSkillAttribute(skill.ID, 
							                                skill.AttrKind, 
							                                skill.Value(player.Attribute.ActiveSkill.Lv), 
							                                skill.LifeTime(player.Attribute.ActiveSkill.Lv));
						}
					}
				}
				break;
			}
			break;
		}
	}
	
	private List<GameObject> getActiveSkillTarget(PlayerBehaviour player) {
		if (GameData.DSkillData.ContainsKey(player.Attribute.ActiveSkill.ID)) {
			string key  = player.Team.ToString() + "_"+ player.Index.ToString() + "_" + GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind;
			if(activeSkillTargets.ContainsKey(key)) {
				return activeSkillTargets[key];
			} else {
				List<GameObject> objs = new List<GameObject>();
				switch (GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind) {
				case 0:// self
					objs.Add(player.gameObject);
					break;
				case 1://my basket
					objs.Add(CourtMgr.Get.BasketHoop[player.Team.GetHashCode()].gameObject);
					break;
				case 2:{//enemy basket
					int i = 1;
					if (player.Team == ETeamKind.Npc)
						i = 0;
					
					objs.Add(CourtMgr.Get.BasketHoop[i].gameObject);
					break;
				}
				case 3://my all teammates
					for (int i = 0; i < PlayerList.Count; i++) {
						if (PlayerList[i].Team == player.Team) {
							objs.Add(PlayerList[i].gameObject);
						}
					}
					break;
				case 10://ball
					objs.Add(CourtMgr.Get.RealBall);
					break;
				}
				activeSkillTargets.Add(key , objs);
				return activeSkillTargets[key];
			}
		}

		return null;
	}

	private bool checkSkillSituation(PlayerBehaviour player) {
		int kind = GameData.DSkillData[player.Attribute.ActiveSkill.ID].Kind;
		switch (Situation) {
		case EGameSituation.AttackA:
			if(player.Team == ETeamKind.Self) {
				if (kind >= 1 && kind <= 7 && player == BallOwner )
					return true;
				
				if ((kind == 11 || kind == 18) && player == BallOwner) 
					return true;
				
				if (kind == 17 && player != BallOwner) 
					return true;
				
				if (kind == 12 || kind == 13 || kind == 14 || kind == 19 || kind == 20 || kind == 21)
					return true;

			} else {
				if (kind == 16)
					return true;
				
				if (kind == 15 && player != BallOwner && CanUseStealSkill) 
					return true;
				
				if (kind == 17 && player != BallOwner) 
					return true;
				
				if (kind == 13 || kind == 14 || kind == 19 || kind == 20 || kind == 21)
					return true;
			}

			break;
		case EGameSituation.AttackB:
			if(player.Team == ETeamKind.Self) {
				if (kind == 16)
					return true;
				
				if (kind == 15 && player != BallOwner && CanUseStealSkill) 
					return true;
				
				if (kind == 17 && player != BallOwner) 
					return true;
				
				if (kind == 13 || kind == 14 || kind == 19 || kind == 20 || kind == 21)
					return true;

			} else  {
				if (kind >= 1 && kind <= 7 && player == BallOwner )
					return true;
				
				if ((kind == 11 || kind == 18) && player == BallOwner) 
					return true;
				
				if (kind == 17 && player != BallOwner) 
					return true;
				
				if (kind == 12 || kind == 13 || kind == 14 || kind == 19 || kind == 20 || kind == 21)
					return true;
			}

			break;
		}

		return false;
	}

	public bool CheckSkill(PlayerBehaviour player, GameObject target = null) {
		if (player.CanUseSkill) {
			if (target) {
				if(GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind != 1 && 
				   GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind != 2) {
					//Target(People)
					if (target == player.gameObject || getDis(ref player, new Vector2(target.transform.position.x, target.transform.position.z)) <= 
					    GameData.DSkillData[player.Attribute.ActiveSkill.ID].Distance(player.Attribute.ActiveSkill.Lv)) {
						if (checkSkillSituation(player))
							return true;
					}
				} else {
					//Basket
					if (target == player.gameObject || getDis(ref player, new Vector2(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.x, CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.z)) <= 
					    GameData.DSkillData[player.Attribute.ActiveSkill.ID].Distance(player.Attribute.ActiveSkill.Lv)) {
						if (checkSkillSituation(player))
							return true;
					}
				}
			} else
				return true;
		}

		return false;
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

	public bool DoPassiveSkill(ESkillSituation State, PlayerBehaviour player = null, Vector3 v = default(Vector3)) {
		bool Result = false;
		EPlayerState playerState = EPlayerState.Idle;
		try {
			playerState = (EPlayerState)System.Enum.Parse(typeof(EPlayerState), State.ToString());
		} catch {
			playerState = EPlayerState.Idle;
		}

		if(player) {
			switch(State) {
			case ESkillSituation.PickBall:
				skillKind = ESkillKind.Pick2;
				playerState = EPlayerState.PickBall2;
				player.PassiveID = 1310;
				player.PassiveLv = player.PickBall2Lv;
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.MoveDodge:
				if(player.IsBallOwner) {
					int Dir = haveDefPlayer(ref player, GameConst.CrossOverDistance, 50);
					if(Dir != 0 && player.IsHaveMoveDodge) {
						if(Random.Range(0, 100) <= player.MoveDodgeRate) {
							Vector3 pos = CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position;
							//Crossover     
							if(player.Team == ETeamKind.Self && player.transform.position.z >= 9.5)
								return Result;
							else 
							if(player.Team == ETeamKind.Npc && player.transform.position.z <= -9.5)
								return Result;
							
							int AddZ = 6;
							if(player.Team == ETeamKind.Npc)
								AddZ = -6;
							
							player.RotateTo(pos.x, pos.z);
							player.transform.DOMoveZ(player.transform.position.z + AddZ, GameStart.Get.CrossTimeZ).SetEase(Ease.Linear);
							if (Dir == 1) {
								player.transform.DOMoveX(player.transform.position.x - 1, GameStart.Get.CrossTimeX).SetEase(Ease.Linear);
								playerState = EPlayerState.MoveDodge0;
								player.PassiveID = 1100;
							} else {
								player.transform.DOMoveX(player.transform.position.x + 1, GameStart.Get.CrossTimeX).SetEase(Ease.Linear);
								playerState = EPlayerState.MoveDodge1;
								player.PassiveID = 1100;
							}			
							player.PassiveLv = player.MoveDodgeLv;
							
							CoolDownCrossover = Time.time + 4;
							Result = player.AniState(playerState);
						}
					} 
				}
				break;
			case ESkillSituation.Block:
				skillKind = ESkillKind.Block;
				playerState = player.PassiveSkill(ESkillSituation.Block, ESkillKind.Block, v);
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Dunk0:
				skillKind = ESkillKind.Dunk;
				playerState = player.PassiveSkill(ESkillSituation.Dunk0, ESkillKind.Dunk, v);
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Shoot0:
				skillKind = ESkillKind.Shoot;
				playerState = player.PassiveSkill(ESkillSituation.Shoot0, ESkillKind.Shoot, v, haveDefPlayer(ref player, 1.5f, 40));
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Shoot3:
				skillKind = ESkillKind.DownHand;
				playerState = player.PassiveSkill(ESkillSituation.Shoot3, ESkillKind.DownHand, Vector3.zero, haveDefPlayer(ref player, 1.5f, 40));
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Shoot2:
				skillKind = ESkillKind.UpHand;
				playerState = player.PassiveSkill(ESkillSituation.Shoot2, ESkillKind.UpHand, Vector3.zero, haveDefPlayer(ref player, 1.5f, 40));
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Shoot1:
				skillKind = ESkillKind.NearShoot;
				playerState = player.PassiveSkill(ESkillSituation.Shoot1, ESkillKind.NearShoot, Vector3.zero, haveDefPlayer(ref player, 1.5f, 40));
				Result = player.AniState(playerState, v );
				break;
			case ESkillSituation.Layup0:
				skillKind = ESkillKind.Layup;
				playerState = player.PassiveSkill(ESkillSituation.Layup0, ESkillKind.Layup);
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Elbow:
				skillKind = ESkillKind.Elbow;
				playerState = player.PassiveSkill(ESkillSituation.Elbow, ESkillKind.Elbow);
				Result = player.AniState (playerState);
				break;
			case ESkillSituation.Fall1:
				Result = true;
				break;
			case ESkillSituation.Fall2:
				Result = true;
				break;
			case ESkillSituation.Pass4:
				skillKind = ESkillKind.Pass;
				playerState = player.PassiveSkill(ESkillSituation.Pass4, ESkillKind.Pass, v);
				if(playerState != EPlayerState.Pass4)
					Result = player.AniState(playerState);
				else
					Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Pass0:
				skillKind = ESkillKind.Pass;
				playerState = player.PassiveSkill(ESkillSituation.Pass0, ESkillKind.Pass, v);
				if(playerState != EPlayerState.Pass0)
					Result = player.AniState(playerState);
				else
					Result = player.AniState(playerState, v);

				break;
			case ESkillSituation.Pass2:
				skillKind = ESkillKind.Pass;
				playerState = player.PassiveSkill(ESkillSituation.Pass2, ESkillKind.Pass, v);
				if(playerState != EPlayerState.Pass2)
					Result = player.AniState(playerState);
				else
					Result = player.AniState(playerState, v);

				break;
			case ESkillSituation.Pass1:
				skillKind = ESkillKind.Pass;
				playerState = player.PassiveSkill(ESkillSituation.Pass1, ESkillKind.Pass, v);
				if(playerState != EPlayerState.Pass1)
					Result = player.AniState(playerState);
				else
					Result = player.AniState(playerState, v);

				break;
			case ESkillSituation.Push0:
				skillKind = ESkillKind.Push;
				playerState = player.PassiveSkill(ESkillSituation.Push0, ESkillKind.Push);
				if(v == Vector3.zero)
					Result = player.AniState(playerState);
				else
					Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Rebound:
				skillKind = ESkillKind.Rebound;
				playerState = player.PassiveSkill(ESkillSituation.Rebound, ESkillKind.Rebound);
				Result = player.AniState (playerState);
				break;

			case ESkillSituation.JumpBall:
				skillKind = ESkillKind.JumpBall;
				playerState = player.PassiveSkill(ESkillSituation.JumpBall, ESkillKind.JumpBall);
				Result = player.AniState (playerState);
				break;

			case ESkillSituation.Steal0:	
				skillKind = ESkillKind.Steal;
				playerState = player.PassiveSkill(ESkillSituation.Steal0, ESkillKind.Steal);
				Result = player.AniState(playerState, v);

				break;
			}	
		}
		try {
			if(Result && !playerState.ToString().Equals(State.ToString())){
				if(GameData.DSkillData.ContainsKey(player.PassiveID)) {
					if(!player.IsUseSkill)
						UIPassiveEffect.Get.ShowCard(player, GameData.DSkillData[player.PassiveID].PictureNo, player.PassiveLv, GameData.DSkillData[player.PassiveID].Name);
					SkillEffectManager.Get.OnShowEffect(player, true);
					player.GameRecord.PassiveSkill++;
				}
			}
		} catch {
			Debug.Log(player.name  +" is no State: "+ State.ToString() +" or have no PassiveID:"+ player.PassiveID);
		}

		return Result;
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
				getball = haveNearPlayer(BallOwner, 10, true);
		
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
			if (Situation == EGameSituation.InboundsA)
				ChangeSituation(EGameSituation.AttackA);
			else
			if (Situation == EGameSituation.InboundsB)
				ChangeSituation(EGameSituation.AttackB);
		}
	}

//    private PlayerBehaviour findNearBallPlayer(PlayerBehaviour someone)
//    {
//        PlayerBehaviour nearPlayer = null;
//
//        foreach(PlayerBehaviour npc in PlayerList)
//        {
//            if(npc.Team == someone.Team && !someone.IsFall && someone.AIing && npc.CanMove)
//            {
//                if (!nearPlayer)
//                    nearPlayer = npc;
//                else if(getDis(nearPlayer, CourtMgr.Get.RealBall.transform.position) > getDis(npc, CourtMgr.Get.RealBall.transform.position))
//                    nearPlayer = npc;
//            }
//        }
//
//        if (someone != nearPlayer)
//            nearPlayer = null;
//
//        return nearPlayer;
//    }

    /// <summary>
    /// 找出某隊離球最近的球員.
    /// </summary>
    /// <param name="team"> 玩家 or 電腦. </param>
    /// <returns></returns>
    [CanBeNull]
    private PlayerBehaviour findNearBallPlayer(ETeamKind team)
    {
        float nearDis = float.MaxValue;
        PlayerBehaviour nearPlayer = null;

        Vector2 ballPos = Vector2.zero;
        ballPos.Set(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);

        foreach(PlayerBehaviour someone in PlayerList)
        {
            if (someone.Team != team)
                continue;

            Vector2 someonePos = Vector2.zero;
            someonePos.Set(someone.transform.position.x, someone.transform.position.z);

            var dis = Vector2.Distance(ballPos, someonePos);
            if(dis < nearDis)
            {
                nearDis = dis;
                nearPlayer = someone;
            }
        }
        
        return nearPlayer;
    }

	public float GetAngle(Transform t1, Transform t2)
	{
		Vector3 relative = t1.InverseTransformPoint(t2.position);
		return Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
	}

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
									DisAy[j].Distance = Mathf.Abs(GetAngle(Self.transform, anpc.transform));
								else
									DisAy[j].Distance = getDis(ref anpc, ref Self);
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
									DisAy[j].Distance = Mathf.Abs(GetAngle(Self.transform, anpc.transform));
								else
									DisAy[j].Distance = getDis(ref anpc, ref Self);
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

    private void DefBlock(ref PlayerBehaviour Npc, int Kind = 0)
    {
		if (PlayerList.Count > 0 && !IsPassing && !IsBlocking) {
			PlayerBehaviour Npc2;
			int Rate = Random.Range(0, 100);
			TPlayerDisData [] DisAy = GetPlayerDisAy(Npc, false, true);

			if(DisAy != null) {
				for (int i = 0; i < DisAy.Length; i++) {
					Npc2 = DisAy [i].Player;
					if (Npc2 && Npc2 != Npc && Npc2.Team != Npc.Team && Npc2.AIing && 
					    !Npc2.IsSteal && !Npc2.IsPush) {
						float BlockRate = Npc2.Attr.BlockRate;
						
						if(Kind == 1)
							BlockRate = Npc2.Attr.FaketBlockRate;	
						
						float mAngle = GetAngle(Npc.transform, PlayerList [i].transform);
						
						if (getDis(ref Npc, ref Npc2) <= GameConst.BlockDistance && Mathf.Abs(mAngle) <= 70) {
							if (Rate < BlockRate) {
								if(DoPassiveSkill(ESkillSituation.Block, Npc2, Npc.transform.position)) {
									if (Kind == 1)
										Npc2.GameRecord.BeFake++;

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
    private bool isNearestBall(PlayerBehaviour someone)
    {
        return findNearBallPlayer(someone.Team) == someone;
    }

    private void nearestBallPlayerDoPickBall(PlayerBehaviour someone)
    {
        if (isNearestBall(someone))
            doPickBall(someone);
        else
            doLookAtBall(someone);
    }

    //	private PlayerBehaviour PickBall(PlayerBehaviour someone/*, bool findNear = false*/)
    private void doPickBall(PlayerBehaviour someone/*, bool findNear = false*/)
	{
	    if(someone.CanMove && someone.WaitMoveTime == 0)
	    {
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
	
    private float getDis(ref PlayerBehaviour player1, ref PlayerBehaviour player2)
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

    private float getDis(PlayerBehaviour someone, Vector3 target)
    {
        if(someone != null && target != Vector3.zero)
            return Vector3.Distance(someone.transform.position, target);

        return -1;
    }

    private float getDis(ref PlayerBehaviour player1, Vector2 target)
    {
        if (player1 != null && target != Vector2.zero)
        {
            Vector3 v1 = new Vector3(target.x, 0, target.y);
            Vector3 v2 = player1.transform.position;
            v1.y = v2.y;
            return Vector3.Distance(v1, v2);
        } else
            return -1;
    }	

	public float getDis(Vector2 player1, Vector2 target)
	{
//		if(player1 != null && Target != Vector2.zero)
		if(target != Vector2.zero)
		{
			Vector3 v1 = new Vector3(target.x, 0, target.y);
			Vector3 v2 = new Vector3(player1.x, 0, player1.y);
			return Vector3.Distance(v1, v2);
		} else
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
		if(p != null && GameStart.Get.IsDebugAnimation)
			Debug.Log ("SetBall P : " + p.gameObject.name);

		bool result = false;
		IsPassing = false;
        if (p != null && Situation != EGameSituation.End) {
            if (BallOwner != null) {
                if (BallOwner.Team != p.Team) {
					if (GameStart.Get.CourtMode == ECourtMode.Full) {
	                    if (Situation == EGameSituation.AttackA)
	                        ChangeSituation(EGameSituation.AttackB);
	                    else 
						if (Situation == EGameSituation.AttackB)
	                        ChangeSituation(EGameSituation.AttackA);
					} else {
						if (p.Team == ETeamKind.Self)
							ChangeSituation(EGameSituation.InboundsA);
						else
							ChangeSituation(EGameSituation.InboundsB);
					}
                } else {
                   	if (Situation == EGameSituation.InboundsA)
                        ChangeSituation(EGameSituation.AttackA);
                    else
					if (Situation == EGameSituation.InboundsB)
                        ChangeSituation(EGameSituation.AttackB);
                    else
                        BallOwner.ResetFlag(false);
                }
            } else {
                if (Situation == EGameSituation.APickBallAfterScore)
					ChangeSituation(EGameSituation.InboundsA);
                else 
				if (Situation == EGameSituation.BPickBallAfterScore)
					ChangeSituation(EGameSituation.InboundsB);
				else
				if (Situation == EGameSituation.InboundsA)
					ChangeSituation(EGameSituation.AttackA);
				else
				if (Situation == EGameSituation.InboundsB)
					ChangeSituation(EGameSituation.AttackB);
                else {
					if (GameStart.Get.CourtMode == ECourtMode.Full || 
					   (p.Team == ETeamKind.Self && Situation == EGameSituation.AttackA) ||
					   (p.Team == ETeamKind.Npc && Situation == EGameSituation.AttackB)) {
						if (p.Team == ETeamKind.Self)
							ChangeSituation(EGameSituation.AttackA, p);
						else
							ChangeSituation(EGameSituation.AttackB, p);
					} else {
						if (p.Team == ETeamKind.Self)
							ChangeSituation(EGameSituation.InboundsA);
						else
							ChangeSituation(EGameSituation.InboundsB);
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

			if (BallOwner && BallOwner.DefPlayer)
				BallOwner.DefPlayer.SetAutoFollowTime();

			UIGame.Get.ChangeControl(p.Team == ETeamKind.Self);
			UIGame.Get.SetPassButton();
			CourtMgr.Get.SetBallState(EPlayerState.HoldBall, p);

        	if (p) {
				AudioMgr.Get.PlaySound(SoundType.SD_CatchBall);
				p.WaitMoveTime = 0;
				p.IsFirstDribble = true;

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
		int block = 0;
		PlayerBehaviour npc = null;
		for (int i = 0; i < PlayerList.Count; i++)
			if (PlayerList[i].gameObject.activeInHierarchy && PlayerList[i].Team == team && PlayerList[i].Attribute.Block > block)
				npc = PlayerList[i];

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
							player.AniState(EPlayerState.HoldBall);
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

	public void BallTouchPlayer(int index, int dir, bool isEnter) {
		if (index >= 0 && index < PlayerList.Count)
			BallTouchPlayer(PlayerList[index], dir, isEnter);
	}

    public void BallTouchPlayer(PlayerBehaviour player, int dir, bool isEnter) {
		if (BallOwner || 
		    IsShooting || 
		    !player.IsCanCatchBall || 
		    player.CheckAnimatorSate(EPlayerState.GotSteal) || 
		    player.IsPush || 
		    dir == 6)
            return;
		if(player.crtState == EPlayerState.Pass5 || player.crtState == EPlayerState.Pass6  ||player.crtState == EPlayerState.Pass7 || player.crtState == EPlayerState.Pass8  )
			Debug.LogError("name:" + player.name);

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
        
		switch (dir) {
		case 0: //top ,rebound
			if (Situation == EGameSituation.JumpBall) {	
				CourtMgr.Get.SetBallState(EPlayerState.JumpBall, player);
			} else
			if ((isEnter || GameStart.Get.TestMode == EGameTest.Rebound) && player != BallOwner && CourtMgr.Get.RealBall.transform.position.y >= 3 && (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)) {
				if (GameStart.Get.TestMode == EGameTest.Rebound || Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB) {
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
								Shoot();
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
					if(Mathf.Abs(GetAngle(player2.transform, player1.transform)) <= GameConst.SlowDownAngle)
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

	public void DefRangeTouchBall(PlayerBehaviour player1)
	{
		if(player1.IsHavePickBall2) {
			if (BallOwner == null && Shooter == null && Catcher == null && (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)) {
				int rate = Random.Range(0, 100);
				if(rate < player1.PickBall2Rate) {
					DoPassiveSkill(ESkillSituation.PickBall, player1, CourtMgr.Get.RealBall.transform.position);
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
						if (Random.Range(0, 100) < player.Attr.AlleyOopRate) {
							player.AniState(EPlayerState.Alleyoop, CourtMgr.Get.ShootPoint [team].transform.position);

							if ((BallOwner != Joysticker || (BallOwner == Joysticker && Joysticker.AIing)) && Random.Range(0, 100) < BallOwner.Attr.AlleyOopPassRate) {
								if (DoPassiveSkill(ESkillSituation.Pass0, BallOwner, player.transform.position))
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
	}

	public void SetGameRecordToUI() {
		UIGameResult.Get.SetGameRecord(ref GameRecord);
		for (int i = 0; i < PlayerList.Count; i ++)
			UIGameResult.Get.AddDetailString(ref PlayerList[i].Attr, i);
	}

    private void gameResult()
    {
        ChangeSituation(EGameSituation.End);
		UIGame.Get.GameOver();
		GameRecord.Done = true;
		SetGameRecord(true);
		StartCoroutine(playFinish());


		if (UIGame.Get.Scores [0] >= UIGame.Get.Scores [1]) {
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].Team == ETeamKind.Self)
					PlayerList [i].AniState (EPlayerState.Ending0);
				else
					PlayerList [i].AniState (EPlayerState.Ending10);
		}
		else
		{
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].Team == ETeamKind.Self)
					PlayerList [i].AniState (EPlayerState.Ending10);
			else
				PlayerList [i].AniState (EPlayerState.Ending0);
		}
    }

	private EPlayerState[] shootInState = new EPlayerState[4]{EPlayerState.Show101, EPlayerState.Show102, EPlayerState.Show103, EPlayerState.Show104};
	private EPlayerState[] shootOutState = new EPlayerState[2]{EPlayerState.Show201, EPlayerState.Show202};

	public void ShowShootSate(bool isIn, int team)
	{
		if (GameStart.Get.CourtMode == ECourtMode.Half && Shooter)
			team = Shooter.Team.GetHashCode();

		for (int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i].Team.GetHashCode() == team)
			{
				if(!PlayerList[i].IsDunk){
					if(isIn)
						PlayerList[i].AniState(shootInState[Random.Range(0, shootInState.Length -1)]);
					else
						PlayerList[i].AniState(shootOutState[Random.Range(0, shootOutState.Length -1)]);
				}
			}
	}
    
    public void PlusScore(int team, bool isSkill, bool isChangeSituation)
    {
		if (GameStart.Get.CourtMode == ECourtMode.Half && Shooter)
			team = Shooter.Team.GetHashCode();

		int score = 2;
		if (shootDistance >= GameConst.TreePointDistance)
			score = 3;

		if (GameStart.Get.TestMode == EGameTest.Skill)
			UIGame.Get.PlusScore(team, score);
		else
		if (IsStart && GameStart.Get.TestMode == EGameTest.None) {
			if (Shooter) {
				if (shootDistance >= GameConst.TreePointDistance)
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
		shootDistance = 0;
    }

	private PlayerBehaviour havePartner(ref PlayerBehaviour npc, float dis, float angle) {
        float mangle;
        
	    for (int i = 0; i < PlayerList.Count; i++) {
			PlayerBehaviour targetNpc = PlayerList [i];
			if (targetNpc.gameObject.activeSelf && targetNpc != npc && targetNpc.Team == npc.Team && 
			    getDis(ref npc, ref targetNpc) <= dis && haveDefPlayer(ref targetNpc, 1.5f, 40) == 0) {
				mangle = GetAngle(npc.transform, targetNpc.transform);
	            
                if (mangle >= 0 && mangle <= angle)
					return targetNpc;
                else 
				if (mangle <= 0 && mangle >= -angle) 
					return targetNpc;
	        }
        }
        
        return null;
    }

	private int haveDefPlayer(ref PlayerBehaviour npc, float dis, float angle) {
        int result = 0;
        float mangle;
        
	    for (int i = 0; i < PlayerList.Count; i++) {
			if (PlayerList [i].gameObject.activeInHierarchy && PlayerList [i].Team != npc.Team)  {
	            PlayerBehaviour targetNpc = PlayerList [i];
				mangle = GetAngle(npc.transform, targetNpc.transform);
	            
	            if (getDis(ref npc, ref targetNpc) <= dis) {
	                if (mangle >= 0 && mangle <= angle) {
	                    result = 1;
	                    break;
	                } else 
					if (mangle <= 0 && mangle >= -angle) {
	                    result = 2;
	                    break;
	                }
	            }
	        }  
        }
        
        return result;
    }

	private int haveDefPlayer(ref PlayerBehaviour npc, float dis, float angle, out PlayerBehaviour man) {
		int Result = 0;
		float mangle;
		man = null;

		for (int i = 0; i < PlayerList.Count; i++) {
			if (PlayerList [i].gameObject.activeInHierarchy && PlayerList [i].Team != npc.Team) {
				PlayerBehaviour TargetNpc = PlayerList [i];
				mangle = GetAngle(npc.transform, TargetNpc.transform);
				
				if (getDis(ref npc, ref TargetNpc) <= dis && TargetNpc.CheckAnimatorSate(EPlayerState.Idle)) {
					if (mangle >= 0 && mangle <= angle) {
						Result = 1;
						man = TargetNpc;
						break;
					} else 
					if (mangle <= 0 && mangle >= -angle) {
						Result = 2;
						man = TargetNpc;
						break;
					}
				}
			}
		}
		
		return Result;
	}

	private int haveStealPlayer(ref PlayerBehaviour P1, ref PlayerBehaviour P2, float dis, float angle) {
		int Result = 0;
		float mangle;

		if (P1 != null && P2 != null && P1 != P2) {
			mangle = GetAngle(P1.transform, P2.transform);
			
			if (getDis(ref P1, ref P2) <= dis) {
				if (mangle >= 0 && mangle <= angle)				
					Result = 1;
				else 
				if (mangle <= 0 && mangle >= -angle)				
					Result = 2;
			}
		}
		
		return Result;
	}
    
    private PlayerBehaviour haveNearPlayer(PlayerBehaviour Self, float Dis, bool SameTeam, bool FindBallOwnerFirst = false) {
        PlayerBehaviour Result = null;
        PlayerBehaviour Npc = null;
                
    	for (int i = 0; i < PlayerList.Count; i++) {
			if (PlayerList[i].gameObject.activeInHierarchy) {
		        Npc = PlayerList [i];
		        
		        if (SameTeam) {
		            if (PlayerList [i] != Self && PlayerList [i].Team == Self.Team && getDis(ref Self, ref Npc) <= Dis) {
		                Result = Npc;
		                break;
		            }
		        } else {
	            if (FindBallOwnerFirst) {
	                if (Npc != Self && Npc.Team != Self.Team && Npc == BallOwner && getDis(ref Self, ref Npc) <= Dis) {
	                    Result = Npc;
	                    break;
	                }
	            } else {
	                if (Npc != Self && Npc.Team != Self.Team && getDis(ref Self, ref Npc) <= Dis && Npc.crtState == EPlayerState.Idle) {
	                    Result = Npc;
	                    break;
	                }
	            }
			}
        }
    }
        
        return Result;
    }

    private bool CheckAttack(ref PlayerBehaviour Npc)
    {
		if (Npc.Team == ETeamKind.Self && (Npc.transform.position.z >= 15.5f && Npc.transform.position.x <= 1 && Npc.transform.position.x >= -1))
            return false;
        else if (Npc.Team == ETeamKind.Npc && Npc.transform.position.z <= -15.5f && Npc.transform.position.x <= 1 && Npc.transform.position.x >= -1)
            return false;
		else if(Npc.CheckAnimatorSate(EPlayerState.Elbow))
			return false;
        else
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
					Shoot();
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
					Shoot();
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

		ChangeSituation (EGameSituation.Opening);
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
            for (int i = 0; i < PlayerList.Count; i++)            
				if (PlayerList [i].CheckAnimatorSate(EPlayerState.Shoot0) || 
				    PlayerList [i].CheckAnimatorSate(EPlayerState.Shoot1) || 
				    PlayerList [i].CheckAnimatorSate(EPlayerState.Shoot2) || 
				    PlayerList [i].CheckAnimatorSate(EPlayerState.Shoot3) ||
				    PlayerList [i].CheckAnimatorSate(EPlayerState.Shoot6) ||
				    PlayerList [i].CheckAnimatorSate(EPlayerState.TipIn) ||
				    PlayerList [i].IsLayup)
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

	public bool IsOnceAnimation(EAnimatorState state)
	{
		if (LoopStates.ContainsKey (state))
			return false;
		else
			return true;
	}

	public List<PlayerBehaviour> GamePlayers {
		get {
			return PlayerList;
		}
	}
}
