﻿using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using DG.Tweening;
using Newtonsoft.Json;

public enum EGameSituation
{
	InitShowContorl   = -3,
	ShowOne           = -2,
	ShowTwo           = -1,
    None           = 0,
    Opening        = 1,
    JumpBall       = 2,
    AttackA        = 3,
    AttackB        = 4,
    TeeAPicking    = 5,
    TeeA           = 6,
    TeeBPicking    = 7,
    TeeB           = 8,
    End            = 9
}

public enum ELoadingGamePic
{
	SelectRole = -1,
	Game = 2
}
//public enum EGameAction
//{
//    Def = 0,
//    Attack = 1
//}
//
//public enum EBodyType
//{
//    Small = 0,
//    Middle = 1,
//    Big = 2
//}

public enum ESceneTest
{
	Single,
	Multi,
	Release
}

public enum EGameTest
{
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

public enum EModelTest
{
	None = -1,
	Center = 0,
	Forward = 1,
	Defender = 2
}

public enum ECameraTest
{
    None,
    RGB
}

public enum EPosKind
{
    None,
    Attack,
    Tee,
    TeeDefence,
    Fast,
	Center,
	Forward,
	Guard
}

public enum EScoreType {
	None,
	DownHand,
	UpHand,
	Normal,
	NearShot,
	LayUp
}

public enum EBasketSituation {
	Score = 0,
	Swish = 1,
	NoScore = 2,
	AirBall = 3
}

public enum EBasketDistanceAngle{
	ShortRightWing = 0,
	ShortRight = 1,
	ShortCenter = 2,
	ShortLeft = 3,
	ShortLeftWing = 4,
	MediumRightWing = 5,
	MediumRight = 6,
	MediumCenter = 7,
	MediumLeft = 8,
	MediumLeftWing = 9,
	LongRightWing = 10,
	LongRight = 11,
	LongCenter = 12,
	LongLeft = 13,
	LongLeftWing = 14,
}

public enum ESkillSituation{
	MoveDodge,
	Block,
	Dunk0,
	Fall1,
	Fall2,
	Layup0,
	Steal0,
	Pass0,
	Pass2,
	Pass1,
	Pass4,
	PickBall0,
	Push0,
	Rebound,
	Elbow,
	Shoot0,
	Shoot1,
	Shoot2,
	Shoot3
}

public enum ESkillKind{
	DownHand = 1,
	UpHand = 2,
	Shoot = 3,
	NearShoot = 4,
	Layup = 5,
	Dunk = 6,
	MoveDodge = 11,
	Pass = 12,
	Pick2 = 13,
	Rebound = 14,
	Steal = 15,
	Block = 16,
	Push = 17,
	Elbow = 18,
	Fall1 = 19,
	Fall2 = 20,
	Special1 = 101,
	Special2 = 102,
	Special3 = 103,
	Special4 = 104,
	Special5 = 105,
	Special6 = 106
}

public enum EPassDirectState {
	Forward = 1,
	Back = 2,
	Left = 3,
	Right = 4
}

public struct TTactical
{
    public string FileName;
    public TActionPosition[] PosAy1;
    public TActionPosition[] PosAy2;
    public TActionPosition[] PosAy3;

    public TTactical(bool flag)
    {
        FileName = "";
        PosAy1 = new TActionPosition[0];
        PosAy2 = new TActionPosition[1];
        PosAy3 = new TActionPosition[2];
    }
}

public struct TActionPosition
{
    public float x;
    public float z;
    public bool Speedup;
	public bool Catcher;
	public bool Shooting;
}

public struct TBasketShootPositionData {
	public string AnimationName;
	public float ShootPositionX;
	public float ShootPositionY;
	public float ShootPositionZ;
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

public class GameController : KnightSingleton<GameController> {
    private bool isSkip = false;
    public bool IsStart = false;
	public bool IsReset = false;
	public bool IsJumpBall = false;
	private bool isPassing = false;
	private bool isFirstScore = false;
    private float coolDownPass = 0;
    public float CoolDownCrossover = 0;
    private float shootDistance = 0;
    public float RealBallFxTime = 0;
	public float StealBtnLiftTime = 1f;
    private float waitTeeBallTime = 0;
	private float waitStealTime = 0;
	private float passingStealBallTime = 0;
	private int skillFirstScore = 2;
	private float angleByPlayerHoop = 0;
	private int shootAngle = 55;
	private float extraScoreRate = 0;
	public string BasketAnimationName = "BasketballAction_1";

    public PlayerBehaviour BallOwner;
	public PlayerBehaviour Joysticker;
	public PlayerBehaviour Shooter;
    public PlayerBehaviour Catcher;
	public PlayerBehaviour Passer;
	private GameObject ballHolder;
	private PlayerBehaviour pickBallPlayer = null;

	private Vector2[] teeBackPosAy = new Vector2[3];
	private Vector3[] bornPosAy = new Vector3[6];

	public EGameSituation Situation = EGameSituation.None;
    private ESkillKind skillKind;
    public EBasketSituation BasketSituation;
	private EBasketDistanceAngle basketDistanceAngle = EBasketDistanceAngle.ShortCenter;
	private List<int> TacticalDataList = new List<int>();
	private Dictionary<string, Shader> shaderCache = new Dictionary<string, Shader>();
	private List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
    
    public TGameRecord GameRecord = new TGameRecord();
	private TMoveData moveData = new TMoveData();
    private TTactical attackTactical;
	private TTactical defTactical;
	private TActionPosition [] tacticalData;
    
    public GameObject[] passIcon = new GameObject[3];
	private GameObject[] selectIcon = new GameObject[2];
	private List<GameObject> objsPassiveEffect =new List<GameObject>();

	public EPlayerState testState = EPlayerState.Shoot0;
	public EPlayerState[] ShootStates = new EPlayerState[]{EPlayerState.Shoot0, EPlayerState.Shoot1, EPlayerState.Shoot2, EPlayerState.Shoot3, EPlayerState.Shoot6, EPlayerState.Layup0, EPlayerState.Layup1, EPlayerState.Layup2, EPlayerState.Layup3};
	public static Dictionary<EAnimatorState, bool> LoopStates = new Dictionary<EAnimatorState, bool>();

    void Start() {
        EffectManager.Get.LoadGameEffect();
        InitPos();
        InitGame();
		InitAniState ();
    }

	void InitAniState()
	{
		if(!LoopStates.ContainsKey(EAnimatorState.Dribble))
			LoopStates.Add (EAnimatorState.Dribble, false);

		if(!LoopStates.ContainsKey(EAnimatorState.Defence))
			LoopStates.Add (EAnimatorState.Defence, false);  

		if(!LoopStates.ContainsKey(EAnimatorState.HoldBall))
			LoopStates.Add (EAnimatorState.HoldBall, false);

		if(!LoopStates.ContainsKey(EAnimatorState.Idle))
			LoopStates.Add (EAnimatorState.Idle, false);

		if(!LoopStates.ContainsKey(EAnimatorState.Run))
			LoopStates.Add (EAnimatorState.Run, false);        
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
        teeBackPosAy [0] = new Vector2(0, 11.5f);   //C
        teeBackPosAy [1] = new Vector2(5.3f, 8);    //F
        teeBackPosAy [2] = new Vector2(-5.3f, 8);   //G

		/*
		 *     0	 5
		 * 		 1 4
		 * 	   2     3
		 */

		bornPosAy [0] = new Vector3 (-3.5f, 0, -3);//G_A
		bornPosAy [1] = new Vector3 (0, 0, -1.5f);//C_A
		bornPosAy [2] = new Vector3 (3.5f, 0, -3);//F_A
		bornPosAy [3] = new Vector3 (3.5f, 0, 3);//G_B
		bornPosAy [4] = new Vector3 (0, 0, 1.5f);//C_B
		bornPosAy [5] = new Vector3 (-3.5f, 0, 3);//F_B
    }

    public void InitGame() {
		IsPassing = false;
		Shooter = null;
		for (var i = 0; i < PlayerList.Count; i ++)
		if (PlayerList[i]) {
			Destroy (PlayerList[i]);
			PlayerList[i] = null;
		}

        PlayerList.Clear();
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

	private void GetMovePath(int index, ref TTactical Result)
    {
		if (Result.PosAy1 == null)
			Result = new TTactical (false);

		Result.FileName = "";

		if (index >= 0 && index < GameConst.TacticalDataName.Length)
        {
			if (GameData.SituationPosition [index].Length > 0)
            {
				int r = Random.Range(0, GameData.SituationPosition [index].Length);
				int i = GameData.SituationPosition [index] [r];
                Result = GameData.TacticalData [i];
            }
        }
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

	private void InitPosition()
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

	private void InitIngameAnimator()
	{
		for(int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i])
				ModelManager.Get.ChangeAnimator(PlayerList[i].AnimatorControl, PlayerList[i].Attribute.BodyType.ToString(), EanimatorType.AnimationControl);
		
		for(int i = 0; i < PlayerList.Count; i++)
			if(PlayerList[i].ShowPos != 0 || PlayerList[i].ShowPos != 3)
				PlayerList[i].AniState(EPlayerState.Idle);
	}
	
	public void CreateTeam() {
        switch (GameStart.Get.TestMode)
        {               
            case EGameTest.None:
				checkPlayerID();
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], GameData.Team.Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Self, bornPosAy[1], GameData.TeamMembers[0].Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Self, bornPosAy[2], GameData.TeamMembers[1].Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, bornPosAy[3], GameData.EnemyMembers[0].Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Npc, bornPosAy[4], GameData.EnemyMembers[1].Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Npc, bornPosAy[5], GameData.EnemyMembers[2].Player));								

                for (int i = 0; i < PlayerList.Count; i++)
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);

				//1.G(Dribble) 2.C(Rebound) 3.F
				InitPosition();
                break;
			case EGameTest.All:
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], new GameStruct.TPlayer(0)));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Self, bornPosAy[1], new GameStruct.TPlayer(0)));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Self, bornPosAy[2], new GameStruct.TPlayer(0)));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, bornPosAy[3], new GameStruct.TPlayer(0)));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Npc, bornPosAy[4], new GameStruct.TPlayer(0)));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Npc, bornPosAy[5], new GameStruct.TPlayer(0)));								
				
				for (int i = 0; i < PlayerList.Count; i++)
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
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
				
				for (int i = 0; i < PlayerList.Count; i++)
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
				break;
			case EGameTest.OneByOne: 
				TPlayer Self = new TPlayer(0);
				Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			

				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), Self));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Npc, new Vector3 (0, 0, 5), new GameStruct.TPlayer(0)));

                for (int i = 0; i < PlayerList.Count; i++)
                    PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
                break;
			case EGameTest.Alleyoop:
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, ETeamKind.Self, new Vector3 (0, 0, 3), new GameStruct.TPlayer(0)));

				break;
			case EGameTest.Pass:
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, ETeamKind.Self, new Vector3 (-5, 0, -2), new GameStruct.TPlayer(0)));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (2, ETeamKind.Self, new Vector3 (5, 0, -2), new GameStruct.TPlayer(0)));
				for (int i = 0; i < PlayerList.Count; i++)
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
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
				
				for (int i = 0; i < PlayerList.Count; i++)
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
				break;
			case EGameTest.Skill:
				if (GameData.Team.Player.ID == 0) 
					GameData.Team.Player.SetID(14);

				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, bornPosAy[0], GameData.Team.Player));	
				break;
        }

        Joysticker = PlayerList [0];

		EffectManager.Get.PlayEffect("SelectMe", Vector3.zero, null, Joysticker.gameObject);
        Joysticker.AIActiveHint = GameObject.Find("SelectMe/AI");
		Joysticker.SpeedUpView = GameObject.Find("SelectMe/Speedup").GetComponent<UISprite>();

		passIcon[0] = EffectManager.Get.PlayEffect("PassMe", new Vector3(0, (4 - (Joysticker.Attribute.BodyType * 0.3f)), 0), Joysticker.gameObject);

		if (Joysticker.SpeedUpView)
			Joysticker.SpeedUpView.enabled = false;

        if (PlayerList.Count > 1 && PlayerList [1].Team == Joysticker.Team) {
			passIcon[1] = EffectManager.Get.PlayEffect("PassA", new Vector3(0, (4 - (PlayerList [1].Attribute.BodyType * 0.3f)), 0), PlayerList [1].gameObject);
			selectIcon[0] = EffectManager.Get.PlayEffect("SelectA", Vector3.zero, null, PlayerList [1].gameObject);
		}

        if (PlayerList.Count > 2 && PlayerList [2].Team == Joysticker.Team) {
			passIcon[2] = EffectManager.Get.PlayEffect("PassB", new Vector3(0, (4 - (PlayerList [2].Attribute.BodyType * 0.3f)), 0), PlayerList [2].gameObject);
			selectIcon[1] = EffectManager.Get.PlayEffect("SelectB", Vector3.zero, null, PlayerList [2].gameObject);
		}
		
		UIGame.Get.InitLine();
		setPassIcon(false);

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
				passIcon[i].SetActive(isShow);
				if(i<2)
					selectIcon[i].SetActive(isShow);
			}
		}
	}

	private Shader loadShader(string path) {
		if (shaderCache.ContainsKey(path)) {
			return shaderCache [path];
		} else {
			Shader obj = Resources.Load(path) as Shader;
			if (obj) {
				shaderCache.Add(path, obj);
				return obj;
			} else {
				//download form server
				return null;
			}
		}
	}

	void FixedUpdate() {
		if (Joysticker) {
			if (Input.GetKeyUp (KeyCode.K))
				gameResult(0);

			if (Input.GetKeyUp (KeyCode.D))
			{
				UIGame.Get.DoAttack();
			}

			if (Input.GetKeyDown(KeyCode.N))
			{
				isSkip = true;
				CourtMgr.Get.ShowEnd (true);
				InitIngameAnimator();
				InitPosition();
			}

			if (Input.GetKeyDown(KeyCode.T)){
				UIDoubleClick.Get.ClickStop();
			}

			if (Input.GetKeyUp (KeyCode.N))
			{
				UIDoubleClick.Get.Init();
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
					if(GameStart.Get.TestMode == EGameTest.AnimationUnit)
						Joysticker.AniState(GameStart.Get.SelectAniState);
					else
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
				for (int i = 0; i < PlayerList.Count; i ++)
					PlayerList[i].SetAnger(PlayerList[i].Attribute.MaxAnger);
			}

			if(Input.GetKeyDown(KeyCode.P) && Joysticker != null) 
				Joysticker.SetAnger(Joysticker.Attribute.MaxAnger);

			if(Input.GetKeyDown(KeyCode.O) && Joysticker != null) 
				UIGame.Get.DoSkill();

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

		if(waitTeeBallTime > 0 && Time.time >= waitTeeBallTime)
		{
			if(BallOwner != null)
			{
				if(AutoTee())
					waitTeeBallTime = 0;
			}else
				waitTeeBallTime = 0;
		}

		if(waitStealTime > 0 && Time.time >= waitStealTime)		
			waitStealTime = 0;

		if(passingStealBallTime > 0 && Time.time >= passingStealBallTime)		
			passingStealBallTime = 0;
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
		GameRecord.Version = BundleVersion.version;
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

	private int GetPosNameIndex(EPosKind Kind, int Index = -1)
	{
		switch (Kind)
		{
		case EPosKind.Attack:
			return 2;
		case EPosKind.Tee:
			if (Index == 0)
				return 3;
			else if (Index == 1)
				return 4;
			else if (Index == 2)
				return 5;
			else
				return -1;
		case EPosKind.TeeDefence:
			if (Index == 0)
				return 6;
			else if (Index == 1)
				return 7;
			else if (Index == 2)
				return 8;
			else
				return -1;
		case EPosKind.Fast:
			if (Index == 0)
				return 9;
			else if (Index == 1)
				return 10;
			else if (Index == 2)
				return 11;
			else
				return -1;
		case EPosKind.Center:
			return 12;
		case EPosKind.Forward:
			return 13;
		case EPosKind.Guard:
			return 14;
		default:
			return -1;
		}
	}

    
    #if UNITY_EDITOR
	void OnGUI() {
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
	}
	#endif

	private void JumpBall()
	{
		if (BallOwner == null)
		{
			for(int i = 0; i < PlayerList.Count; i++)
			{
				PlayerBehaviour npc = PlayerList[i];
				if(npc.Team == ETeamKind.Self)
				{
					PickBall(ref npc, false);
					if(npc.DefPlayer != null)
						PickBall(ref npc.DefPlayer, false);
				}
			}
		}
	}

    private void SituationAttack(ETeamKind team)
    {
        if (PlayerList.Count > 0)
        {
			if (BallOwner != null) {
				switch (BallOwner.Postion) {
				case EPlayerPostion.C:
					GetMovePath(GetPosNameIndex(EPosKind.Center), ref attackTactical);
					break;
				case EPlayerPostion.F:
					GetMovePath(GetPosNameIndex(EPosKind.Forward), ref attackTactical);
					break;
				case EPlayerPostion.G:
					GetMovePath(GetPosNameIndex(EPosKind.Guard), ref attackTactical);
					break;
				default:
					GetMovePath(GetPosNameIndex(EPosKind.Attack), ref attackTactical);
					break;
				}
			} else
				GetMovePath(GetPosNameIndex(EPosKind.Attack), ref attackTactical);

			bool isShooting = IsShooting;
			for (int i = 0; i < PlayerList.Count; i++) {
                PlayerBehaviour npc = PlayerList [i];
				if (CandoAI(npc) && !DoSkill(npc)) {
					if (npc.Team == team) {                      
						if (!isShooting) {
                        	aiAttack(ref npc);
							AIMove(ref npc, ref attackTactical);
						} else 
						if (!npc.IsAllShoot) {
                        	aiAttack(ref npc);
							AIMove(ref npc, ref attackTactical);
						}                                                       
                    } else
                    	aiDefend(ref npc);
                }
            }   
        }
    }

    private void SituationPickBall(ETeamKind team)
    {
        if (BallOwner == null)
        {
            if (PlayerList.Count > 0)
            {
				if(pickBallPlayer == null)
					pickBallPlayer = NearBall(team);

				if (pickBallPlayer != null)
				{
					GetMovePath(GetPosNameIndex(EPosKind.Tee, pickBallPlayer.Index), ref attackTactical);
					GetMovePath(GetPosNameIndex(EPosKind.TeeDefence, pickBallPlayer.Index), ref defTactical);
                }                   

                for (int i = 0; i < PlayerList.Count; i++)
                {
                    PlayerBehaviour Npc = PlayerList [i];
                    if (CandoAI(Npc))
                    {
                        if (Npc.Team == team)
                        {
							if (Npc == pickBallPlayer)
								PickBall(ref Npc);
                            else 
								TeeBall(ref Npc, team, ref attackTactical);
						} else 
							BackToDef(ref Npc, ETeamKind.Npc, ref defTactical);
                    }
                }
            }
        }
    }

    private void SituationTeeBall(ETeamKind team)
    {
        if (PlayerList.Count > 0)
        {
            if (BallOwner != null)
            {
				bool isPassing = IsPassing;
				GetMovePath(GetPosNameIndex(EPosKind.Tee, BallOwner.Index), ref attackTactical);
				GetMovePath(GetPosNameIndex(EPosKind.TeeDefence, BallOwner.Index), ref defTactical);

				for (int i = 0; i < PlayerList.Count; i++)
				{
					PlayerBehaviour Npc = PlayerList [i];
					if (CandoAI(Npc))
					{
						if (!isPassing && Npc.Team == team)
							TeeBall(ref Npc, team, ref attackTactical);
						else                    
							BackToDef(ref Npc, Npc.Team, ref defTactical);
					}
				}
            }            
        }
    }

	private void aiShoot(ref PlayerBehaviour Self)
	{
		bool suc = false;
		
		if (Self.IsRebound)
			suc = true;
		else
		if(!Self.CheckAnimatorSate(EPlayerState.HoldBall) && haveDefPlayer(ref Self, 5, 40) != 0) {
			int FakeRate = Random.Range (0, 100);
			
			if(FakeRate < GameConst.FakeShootRate)
			{
				if (PlayerList.Count > 1)
				{
					for (int i = 0; i < PlayerList.Count; i++)
					{
						PlayerBehaviour Npc = PlayerList [i];
						
						if (Npc != Self && Npc.Team != Self.Team && getDis(ref Self, ref Npc) <= GameConst.BlockDistance)
						{
							Self.AniState(EPlayerState.FakeShoot, CourtMgr.Get.ShootPoint [Self.Team.GetHashCode()].transform.position);
							suc = true;
							break;
						}
					}
				}
			}
		}
		
		if(!suc)
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
				if (PlayerList [j].Team == npc.Team && PlayerList [j] != npc) {
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
					 !npc.CheckAnimatorSate(EPlayerState.Elbow) && BallOwner.NoAiTime == 0)
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
		if (npc.NoAiTime == 0 && !npc.IsSteal && !npc.CheckAnimatorSate(EPlayerState.Push0) && 
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

    private bool CandoAI(PlayerBehaviour npc)
    {
        if (npc.NoAiTime == 0)
            return true;
        else
            return false;
    }
    
    public void ChangeSituation(EGameSituation gs, PlayerBehaviour player = null)
    {
        if (Situation != EGameSituation.End || gs == EGameSituation.ShowOne) {
            EGameSituation oldgs = Situation;
            if (Situation != gs) {
                RealBallFxTime = 0;
				waitStealTime = 0;
                CourtMgr.Get.RealBallFX.SetActive(false);
                for (int i = 0; i < PlayerList.Count; i++) {
					if(gs == EGameSituation.TeeAPicking || gs == EGameSituation.TeeBPicking) {
						if(PlayerList[i].NoAiTime > 0) {
							PlayerList[i].HaveNoAiTime = true;							
							PlayerList[i].SetAiTime();
						}

						PlayerList[i].ResetMove();
					}												

					switch(PlayerList[i].Team) {
					case ETeamKind.Self:
						if((gs == EGameSituation.TeeB || (oldgs == EGameSituation.TeeB && gs == EGameSituation.AttackB)) == false) {
							if(PlayerList[i].NoAiTime > 0) {
								if(!(gs == EGameSituation.AttackA || gs == EGameSituation.AttackB))
									PlayerList[i].ResetFlag();
							} else
								PlayerList[i].ResetFlag();
						}

						break;
					case ETeamKind.Npc:
						if((gs == EGameSituation.TeeA || (oldgs == EGameSituation.TeeA && gs == EGameSituation.AttackA)) == false)
							PlayerList[i].ResetFlag();

						break;
					}

                    PlayerList [i].situation = gs;
                }
            }
            
            Situation = gs;
            
            if ((oldgs == EGameSituation.TeeA || oldgs == EGameSituation.TeeB) && oldgs != gs && player) {
				GetMovePath(GetPosNameIndex(EPosKind.Fast, player.Index), ref attackTactical);
                
				if (attackTactical.FileName != string.Empty) {
					for (int i = 0; i < PlayerList.Count; i ++) {
						PlayerBehaviour npc = PlayerList [i];
						if (npc.Team == player.Team) {
							tacticalData = GetActionPosition(npc.Index, ref attackTactical);
							
							if (tacticalData != null) {
								for (int j = 0; j < tacticalData.Length; j++) {
									moveData.Clear();
									moveData.Speedup = tacticalData [j].Speedup;
									moveData.Catcher = tacticalData [j].Catcher;
									moveData.Shooting = tacticalData [j].Shooting;
									moveData.FileName = attackTactical.FileName;
									if(npc.Team == ETeamKind.Self)
										moveData.Target = new Vector2(tacticalData [j].x, tacticalData [j].z);
									else
										moveData.Target = new Vector2(tacticalData [j].x, -tacticalData [j].z);

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

			switch (gs) {
			case EGameSituation.InitShowContorl:
					isSkip = false;
					for(int i = 0; i < PlayerList.Count; i++)
						if(PlayerList[i]){
							ModelManager.Get.ChangeAnimator(PlayerList[i].AnimatorControl, PlayerList[i].Attribute.BodyType.ToString(), EanimatorType.ShowControl);
				}
				break;
			case EGameSituation.ShowOne:
				CourtMgr.Get.ShowEnd ();
				UIGame.UIShow (true);
				UIGame.Get.UIState(EUISituation.ShowTwo);
				if(!isSkip){
					InitPosition();
					InitIngameAnimator();
				}
				break;
			case EGameSituation.ShowTwo:
				if(IsStart == false)
				{
					UIGame.Get.UIState(EUISituation.ShowTwo);
					Situation = EGameSituation.Opening;
					ChangeSituation (EGameSituation.Opening);
					CourtMgr.Get.InitScoreboard (true);
				}
				break;
			case EGameSituation.Opening:
				setPassIcon(true);
				UIGame.UIShow (true);
				UIGame.Get.UIState(EUISituation.Opening);
				jodgeSkillUI ();
				CourtMgr.Get.InitBallShadow();
				break;
			case EGameSituation.JumpBall:
				IsStart = true;
				CourtMgr.Get.InitScoreboard (true);
				setPassIcon(true);

				for(int i = 0; i < PlayerList.Count; i++)
					if(PlayerList[i].Postion == EPlayerPostion.C)
						Rebound(PlayerList[i]);
				break;
			case EGameSituation.AttackA:
				CameraMgr.Get.SetCameraSituation(ECameraSituation.Self);
				jodgeSkillUI ();

				break;
			case EGameSituation.AttackB:
				CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc);
				jodgeSkillUI ();

				break;
			case EGameSituation.TeeAPicking:
				CourtMgr.Get.Walls[1].SetActive(false);
				UIGame.Get.ChangeControl(true);
				CameraMgr.Get.SetCameraSituation(ECameraSituation.Npc, true);
				pickBallPlayer = null;

                break;
            case EGameSituation.TeeA:
				CourtMgr.Get.Walls[1].SetActive(false);
				EffectManager.Get.PlayEffect("ThrowInLineEffect", Vector3.zero);

                break;
            case EGameSituation.TeeBPicking:
				CourtMgr.Get.Walls[0].SetActive(false);
           	 	UIGame.Get.ChangeControl(false);
				CameraMgr.Get.SetCameraSituation(ECameraSituation.Self, true);
				pickBallPlayer = null;

                break;
			case EGameSituation.TeeB:
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
            
	            switch (Situation)
	            {
					case EGameSituation.InitShowContorl:
						for(int i = 0; i < PlayerList.Count; i++)
						{
							if(PlayerList[i].ShowPos != -1 && isSkip == false){
								PlayerList[i].gameObject.transform.position = CameraMgr.Get.CharacterPos[PlayerList[i].ShowPos].transform.position;
								PlayerList[i].gameObject.transform.eulerAngles = CameraMgr.Get.CharacterPos[PlayerList[i].ShowPos].transform.eulerAngles;
							}
						}
						break;
					case EGameSituation.ShowOne:
						
						break;

					case EGameSituation.ShowTwo:

						break;

	                case EGameSituation.None:

	                    break;
	                case EGameSituation.Opening:
                
	                    break;
	                case EGameSituation.JumpBall:
						JumpBall();
	                    break;
	                case EGameSituation.AttackA:
	                    SituationAttack(ETeamKind.Self);
	                    break;
	                case EGameSituation.AttackB:
	                    SituationAttack(ETeamKind.Npc);
	                    break;
	                case EGameSituation.TeeAPicking:
	                    SituationPickBall(ETeamKind.Self);
	                    break;
	                case EGameSituation.TeeA:
	                    SituationTeeBall(ETeamKind.Self);
	                    break;
	                case EGameSituation.TeeBPicking:
	                    SituationPickBall(ETeamKind.Npc);
	                    break;
	                case EGameSituation.TeeB:
	                    SituationTeeBall(ETeamKind.Npc);
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

			GameObject target = getSkillTarget(Joysticker);
			if (CheckSkill(Joysticker, target)) {
				UIGame.Get.ShowSkillUI(IsStart, Joysticker.IsAngerFull, true);
			} else
				UIGame.Get.ShowSkillUI(IsStart, Joysticker.IsAngerFull);
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
			if(CourtMgr.Get.BasketAnimationName.Count > 0 && basketDistanceAngleType < CourtMgr.Get.BasketAnimationName.Count){
				int random = Random.Range(0, CourtMgr.Get.BasketAnimationName[basketDistanceAngleType].Count);
				if(CourtMgr.Get.BasketAnimationName.Count > 0 && random < CourtMgr.Get.BasketAnimationName.Count)
					BasketAnimationName = CourtMgr.Get.BasketAnimationName[basketDistanceAngleType][random];
			}
		}else if(BasketSituation == EBasketSituation.NoScore){
			if(CourtMgr.Get.BasketAnimationNoneState.Count > 0 && basketDistanceAngleType < CourtMgr.Get.BasketAnimationName.Count) {
				int random = Random.Range(0, CourtMgr.Get.BasketAnimationNoneState[basketDistanceAngleType].Count);
				if(CourtMgr.Get.BasketAnimationNoneState.Count > 0 && random < CourtMgr.Get.BasketAnimationName.Count)
					BasketAnimationName = CourtMgr.Get.BasketAnimationNoneState[basketDistanceAngleType][random];
			}
		}
	}

	private void calculationScoreRate(PlayerBehaviour player, EScoreType type) {
		jodgeShootAngle(player);
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
	
		if(extraScoreRate == GameData.ExtraPerfectRate || shootDistance < 7)
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

		if (shootDistance >= GameConst.TreePointDistance)
			player.GameRecord.FG3++;
		else
			player.GameRecord.FG++;
		
		player.GameRecord.PushShot(new Vector2(player.transform.position.x, player.transform.position.z), BasketSituation.GetHashCode(), rate);
	}

	public void AddExtraScoreRate(float rate) {
		extraScoreRate = rate;
	}

	public void Shoot()
    {
        if (BallOwner)
        {
			Vector3 v = CourtMgr.Get.ShootPoint [BallOwner.Team.GetHashCode()].transform.position;
			shootDistance = getDis(ref BallOwner, new Vector2(v.x, v.z));

			if(GameStart.Get.TestMode == EGameTest.Shoot) {
				BallOwner.AniState(testState, CourtMgr.Get.Hood[BallOwner.Team.GetHashCode()].transform.position);
			} else 
			if (!BallOwner.IsDunk) {
				extraScoreRate = 0;
				UIGame.Get.DoPassNone();
				CourtMgr.Get.ResetBasketEntra();

				int t = BallOwner.Team.GetHashCode();
				if (GameStart.Get.TestMode == EGameTest.Dunk)
					BallOwner.AniState(EPlayerState.Dunk0, CourtMgr.Get.ShootPoint [t].transform.position);
				else 
				if (BallOwner.IsRebound) {
					if (inTipinDistance(BallOwner))
						BallOwner.AniState(EPlayerState.TipIn, CourtMgr.Get.ShootPoint [t].transform.position);
				} else {
					if(BallOwner.IsMoving){
						if(shootDistance > 15)
							DoPassiveSkill(ESkillSituation.Shoot3, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						else 
						if(shootDistance > 9 && shootDistance <= 15) {
							if (Random.Range(0, 2) == 0)
								DoPassiveSkill(ESkillSituation.Shoot2, BallOwner, CourtMgr.Get.Hood [t].transform.position);
							else
								DoPassiveSkill(ESkillSituation.Shoot0, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						} else 
						if(shootDistance > 7 && shootDistance <= 9) {
							float rate = Random.Range(0, 100);
							if(rate < BallOwner.Attr.DunkRate)
								DoPassiveSkill(ESkillSituation.Dunk0, BallOwner, CourtMgr.Get.ShootPoint [t].transform.position);
							else
								DoPassiveSkill(ESkillSituation.Layup0, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						} else {
							float rate = Random.Range(0, 100);
							if (rate < BallOwner.Attr.DunkRate)
								DoPassiveSkill(ESkillSituation.Dunk0, BallOwner, CourtMgr.Get.ShootPoint [t].transform.position);
							else
								DoPassiveSkill(ESkillSituation.Shoot1, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						}
					} else {
						if(shootDistance > 15)
							DoPassiveSkill(ESkillSituation.Shoot3, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						else 
						if (shootDistance > 9 && shootDistance <= 15)
							DoPassiveSkill(ESkillSituation.Shoot0, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						else
							DoPassiveSkill(ESkillSituation.Shoot1, BallOwner, CourtMgr.Get.Hood [t].transform.position);
					}
				}
			}
        }
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

			if(Mathf.Abs(angleByPlayerHoop) >= 85)
				shootAngle = 55;
			else
				shootAngle = 80;

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

			calculationScoreRate(player, st);
			judgeBasketAnimationName ((int)basketDistanceAngle);

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
				CourtMgr.Get.RealBallRigidbody.velocity = 
					GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
					                         CourtMgr.Get.RealBall.transform.position + (ori * 0.8f), shootAngle);
			} else 
			if(player.crtState == EPlayerState.TipIn) {
				if(CourtMgr.Get.RealBall.transform.position.y > (CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.y + 0.2f)) {
//					CourtMgr.Get.RealBall.transform.DOMove(new Vector3(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.x,
//					                                                   CourtMgr.Get.RealBall.transform.position.y - 0.1f,
//					                                                   CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.z), 0.2f);
					CourtMgr.Get.RealBall.transform.DOMove(new Vector3(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.x,
					                                                   CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.y + GameStart.Get.TipInHeightAdd,
					                                                   CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.z), GameStart.Get.TipInTime);
				} else {
					CourtMgr.Get.RealBall.transform.DOMove(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position, 0.2f);
				}


			} else 
			if(BasketSituation == EBasketSituation.Swish) {
//				#if UNITY_EDITOR
//				UIHint.Get.ShowHint("Swish", Color.yellow);
//				#endif
				Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("BasketCollider"), LayerMask.NameToLayer ("RealBall"), true);
//				distanceBallToBasket = getDis(new Vector2(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z), new Vector2(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.x, CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.z));
//				Debug.Log("distanceBallToBasket : "+distanceBallToBasket);
				CourtMgr.Get.RealBallRigidbody.velocity = 
					GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
					                         CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position , shootAngle);	
			} else {
				CourtMgr.Get.RealBallRigidbody.velocity = 
					GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
					                         CourtMgr.Get.BasketHoop [player.Team.GetHashCode()].position + CourtMgr.Get.BasketShootPosition[BasketAnimationName], shootAngle);
			}

            for (int i = 0; i < PlayerList.Count; i++)
                if (PlayerList [i].Team == Shooter.Team)
                    PlayerList [i].ResetMove();

			return true;
        } else
            return false;
    }

	public void DoShoot(bool isshoot)
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
					Shoot ();
				else
					Joysticker.AniState(EPlayerState.FakeShoot, CourtMgr.Get.ShootPoint [Joysticker.Team.GetHashCode()].transform.position);
            } else 
			if (BallOwner && BallOwner.Team == ETeamKind.Self) 
				Shoot();
			else 
			if (!Joysticker.IsRebound)
				Rebound(Joysticker);
        }
    }

	public void DoPush()
	{
		if (Joysticker) {
			PlayerBehaviour nearP = FindNearNpc();
			if(nearP)
				DoPassiveSkill (ESkillSituation.Push0, Joysticker, nearP.transform.position);
			else
				DoPassiveSkill (ESkillSituation.Push0, Joysticker);
		}
	}

	public void DoElbow()
	{
		if (Joysticker)
			DoPassiveSkill (ESkillSituation.Elbow, Joysticker);
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
    
    public bool Pass(PlayerBehaviour player, bool IsTee = false, bool IsBtn = false, bool MovePass = false)
    {
		bool Result = false;
		bool CanPass = true;

		if(BallOwner != null && BallOwner.NoAiTime == 0)
		{
			if(GameStart.Get.TestMode == EGameTest.Pass) {
				if(BallOwner.IsMoving) {
					float angle = GameFunction.GetPlayerToObjectAngleByVector(BallOwner.gameObject.transform, player.gameObject.transform.position);
					if (angle < 60f && angle > -60f){
						UIHint.Get.ShowHint("Direct Forward and Angle:" + angle, Color.yellow);
						Result = BallOwner.AniState(EPlayerState.Pass5);
					}else 
					if (angle <= -60f && angle > -120f){
						UIHint.Get.ShowHint("Direct Left and Angle:" + angle, Color.yellow);
						Result = BallOwner.AniState(EPlayerState.Pass7);
					}else 
					if (angle < 120f && angle >= 60f){
						UIHint.Get.ShowHint("Direct Right and Angle:" + angle, Color.yellow);
						Result = BallOwner.AniState(EPlayerState.Pass8);
					}else 
					if (angle >= 120f || angle <= -120f){
						UIHint.Get.ShowHint("Direct Back and Angle:" + angle, Color.yellow);
						if(Random.Range(0, 100) < 50)
							Result = BallOwner.AniState(EPlayerState.Pass9);
						else 
							Result = BallOwner.AniState(EPlayerState.Pass6);
					}
				} else {
					Result = BallOwner.AniState(EPlayerState.Pass0, player.gameObject.transform.position);
				}

				if(Result){
					Catcher = player;
					UIGame.Get.DoPassNone();
				}
				return Result;
			}

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
				
				if(!IsBtn && BallOwner.NoAiTime > 0)
					return Result;
				
				if(IsTee)
				{
					if(BallOwner.AniState(EPlayerState.Pass1, player.transform.position))
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

	public int GetEnemyDis(ref PlayerBehaviour npc){
		float [] DisAy = new float[3];
		int Index = 0;
		for (int i = 0; i < PlayerList.Count; i++) 
		{
			if(PlayerList[i].Team != npc.Team)
			{
				DisAy[Index] = Vector3.Distance(npc.transform.position, PlayerList[i].transform.position);
				Index++;
			}		
		}

		for(int i = 0; i < DisAy.Length; i++)
		{
			if(DisAy[i] > 0)
			{
				if(DisAy[i] <= GameConst.StealBallDistance)
					return 2;
				else if(DisAy[i] <= GameConst.DefDistance)
					return 1;
			}
		}

		return 0;
	}
    
//    public bool OnPass(PlayerBehaviour player)
//    {
//        if (Catcher)
//        {
//            SceneMgr.Get.SetBallState(EPlayerState.PassFlat);
//            SceneMgr.Get.RealBallRigidbody.velocity = GameFunction.GetVelocity(SceneMgr.Get.RealBall.transform.position, Catcher.DummyBall.transform.position, Random.Range(40, 60));   
//            if (Vector3.Distance(SceneMgr.Get.RealBall.transform.position, Catcher.DummyBall.transform.position) > 15f)
//                CameraMgr.Get.IsLongPass = true;
//            
//            return true;
//        } else
//            return false;
//    }

    public bool DoPass(int playerid)
    {
		if (IsStart && BallOwner && (!Shooter  || IsCanPassAir) && Joysticker && BallOwner.Team == 0 && CandoBtn)
        {
            if (PlayerList.Count > 1)
            {
				BallOwner.NoAiTime = 0;

                return Pass(PlayerList [playerid], false, true);
            }
        }

		return false;
    }

    private void Steal(PlayerBehaviour player)
    {
        
    }
	
	public bool OnStealMoment(PlayerBehaviour player)
    {
        if (BallOwner && BallOwner.Invincible == 0 && !IsShooting && !IsDunk){
			if(Vector3.Distance(BallOwner.transform.position, player.transform.position) <= GameConst.StealBallDistance)
			{
				int r = Mathf.RoundToInt(player.Attribute.Steal - BallOwner.Attribute.Dribble);
				int maxRate = 100;
				int minRate = 10;
				
				if (r > maxRate)
					r = maxRate;
				else if (r < minRate)
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
				
				if (stealRate <= (r + AddRate) && Mathf.Abs(GetAngle(BallOwner.transform, player.transform)) <= 90 + AddAngle)
				{
					if(BallOwner && BallOwner.AniState(EPlayerState.GotSteal))
					{
						BallOwner.SetAnger(GameConst.DelAnger_Stealed);
						return true;
					}
				} else 
				if(BallOwner != null && haveStealPlayer(ref player, ref BallOwner, GameConst.StealBallDistance, 15) != 0)
				{
					stealRate = Random.Range(0, 100) + 1;
					
					if(stealRate <= r)
					{
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

    public void DoSteal()
    {
		if (StealBtnLiftTime <= 0 && IsStart && Joysticker && CandoBtn) {
			StealBtnLiftTime = 1f;
            if (BallOwner && BallOwner.Team != Joysticker.Team) {
				Joysticker.rotateTo(BallOwner.gameObject.transform.position.x, BallOwner.gameObject.transform.position.z);
				DoPassiveSkill(ESkillSituation.Steal0, Joysticker, BallOwner.transform.position);
            } else
				DoPassiveSkill(ESkillSituation.Steal0, Joysticker);
        }
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
				Joysticker.SetAnger(GameConst.AddAnger_Perfect);				
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

//    public bool OnBlocking(PlayerBehaviour player)
//    {
//        int blockRate = UnityEngine.Random.Range(0, 100);
//        if (blockRate < 30)
//        {
//            if (!BallOwner)
//            {
//                float dis = Vector3.Distance(player.transform.position, SceneMgr.Get.RealBall.transform.position);
//                if (dis <= 4)
//                {
//                    SceneMgr.Get.SetBallState(EPlayerState.Block);
//                    return true;
//                }
//            } else 
//            if (BallOwner.CheckAnimatorSate(EPlayerState.Shooting))
//            {
//                float dis = Vector3.Distance(player.transform.position, BallOwner.transform.position);
//                if (dis <= 4)
//                {
//                    for (int i = 0; i < PlayerList.Count; i++)
//                        if (PlayerList [i].Team == BallOwner.Team)
//                            PlayerList [i].ResetMove();
//
//                    SetBall(null);
//                    SceneMgr.Get.SetBallState(EPlayerState.Block);
//                    return true;
//                }
//            }
//        }
//
//        return false;
//    }

    public void DoBlock()
    {
		if (IsStart && CandoBtn)
		{
			if(Joysticker.crtState == EPlayerState.Block && Joysticker.IsPerfectBlockCatch){
				Joysticker.AniState(EPlayerState.BlockCatch);
				if(UIDoubleClick.Visible)
					UIDoubleClick.Get.ClickStop();
			}else{
				if (Shooter)
					DoPassiveSkill(ESkillSituation.Block, Joysticker, Shooter.transform.position);
	            else
	            if (BallOwner) {
					Joysticker.rotateTo(BallOwner.gameObject.transform.position.x, BallOwner.gameObject.transform.position.z); 
					DoPassiveSkill(ESkillSituation.Block, Joysticker, BallOwner.transform.position);
				} else {
				if (!Shooter && inReboundDistance(Joysticker) && GameStart.Get.TestMode == EGameTest.None)
					Rebound(Joysticker);
				else
					DoPassiveSkill(ESkillSituation.Block, Joysticker);
				}
			}
        }           
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

    private void Rebound(PlayerBehaviour player)
    {
		/*bool flag = true;
		for (int i = 0; i < PlayerList.Count; i ++)
		if (player.Index != i && PlayerList[i].Team == player.Team && player.IsRebound) {
			flag = false;
			break;
		}

		if (flag) {*/
//			player.AniState(EPlayerState.Rebound, CourtMgr.Get.RealBall.transform.position);
			DoPassiveSkill(ESkillSituation.Rebound, player, CourtMgr.Get.RealBall.transform.position);
		//}
	}
	
	public bool OnRebound(PlayerBehaviour player)
    {
        return true;
    }
    
    public void OnSkill() {
		if (CandoBtn && DoSkill(Joysticker)) {
			Joysticker.SetNoAiTime();
			EffectManager.Get.PlayEffect("SkillSign", new Vector3(0, (4 - (Joysticker.Attribute.BodyType * 0.5f)), 0), Joysticker.gameObject, null, 0.5f);
		}
    }

	private bool DoSkill(PlayerBehaviour player) {
		if (player.Attribute.ActiveSkill.ID > 0) {
			Vector3 v;
			GameObject target = getSkillTarget(player);
			if (CheckSkill(player, target)) {
				TSkillData skill = GameData.SkillData[player.Attribute.ActiveSkill.ID];
				switch (skill.Kind) {
				case 6:
					v = CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position;
					shootDistance = getDis(ref player, new Vector2(v.x, v.z));
					break;
				case 7:
					v = CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position;
					shootDistance = getDis(ref player, new Vector2(v.x, v.z));
					isFirstScore = true;

					break;
				case 21://buffer
					switch (skill.TargetKind1) {
					case 0:
						player.AddSkillAttribute(skill.ID, skill.AttrKind, 
						    skill.Value(player.Attribute.ActiveSkill.Lv), skill.LifeTime(player.Attribute.ActiveSkill.Lv));
						break;
					case 3:
						for (int i = 0; i < PlayerList.Count; i++)
							if (PlayerList[i].Team == player.Team)
								PlayerList[i].AddSkillAttribute(skill.ID, skill.AttrKind, 
						            skill.Value(player.Attribute.ActiveSkill.Lv), skill.LifeTime(player.Attribute.ActiveSkill.Lv));
						break;
					}

					break;
				}
				
				player.ActiveSkill(target);
				return true;
			}
		}

		return false;
	}

	private GameObject getSkillTarget(PlayerBehaviour player) {
		if (GameData.SkillData.ContainsKey(player.Attribute.ActiveSkill.ID)) {
			switch (GameData.SkillData[player.Attribute.ActiveSkill.ID].TargetKind1) {
			case 0:
				return player.gameObject;
			case 1:
				return CourtMgr.Get.BasketHoop[player.Team.GetHashCode()].gameObject;
			case 2:
				int i = 1;
				if (player.Team == ETeamKind.Npc)
					i = 0;

				return CourtMgr.Get.BasketHoop[i].gameObject;
			}
		}

		return null;
	}

	private bool checkSkillSituation(PlayerBehaviour player) {
		int kind = GameData.SkillData[player.Attribute.ActiveSkill.ID].Kind;
		switch (Situation) {
		case EGameSituation.AttackA:
			if (kind >= 1 && kind <= 7 && player == BallOwner)
				return true;

			break;
		case EGameSituation.AttackB:
			break;
		default:
			if (kind == 21)
				return true;

			break;
		}

		return false;
	}

	public bool CheckSkill(PlayerBehaviour player, GameObject target = null) {
		if (player.IsAngerFull && player.CanUseSkill) {
			if (target) {
				if (target == player.gameObject || getDis(ref player, new Vector2(target.transform.position.x, target.transform.position.z)) <= 
				    GameData.SkillData[player.Attribute.ActiveSkill.ID].Distance(player.Attribute.ActiveSkill.Lv)) {
					if (checkSkillSituation(player))
						return true;
				}
			} else
				return true;
		}

		return false;
	}
	
	public void OnShowEffect (PlayerBehaviour player = null) {
		List<GameObject> objs = new List<GameObject>();
		
		if(player != null)
			objs = getPassiveSkillTarget(player);
		if(objs.Count > 0){
			if(player.PassiveID != -1) {
				if(GameData.SkillData[player.PassiveID].TargetEffect1 != 0) {
					for(int i=0; i<objs.Count; i++) {
						GameObject parent = null;
						if(GameData.SkillData[player.PassiveID].EffectParent1 == 1) {
							parent = objs[i].gameObject;
						}
						EffectManager.Get.PlayEffect("SkillEffect" + GameData.SkillData[player.PassiveID].TargetEffect1, 
//						                             new Vector3 (0, (4 - (player.Attribute.BodyType * 0.5f)),0),
						                             Vector3.zero,
						                             parent,
						                             null,
						                             GameData.SkillData[player.PassiveID].Duration1);
					}
				}
			}
		}
	}

	private List<GameObject> getPassiveSkillTarget (PlayerBehaviour player) {
		int targetKind = -1;
		objsPassiveEffect.Clear();
		GameObject obj = new GameObject();
		if (GameData.SkillData.ContainsKey(player.PassiveID)) {

			targetKind = GameData.SkillData[player.PassiveID].TargetKind1;
			switch(targetKind) {
			case 0:// Feet
				objsPassiveEffect.Add(player.gameObject); 
				break;
			case 1://Body (Chest)
				Transform tBody = player.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1");
				if(tBody != null)
					objsPassiveEffect.Add(tBody.gameObject);
				break;
			case 2://Head
				obj.transform.position = new Vector3(player.transform.position.x, (4 - (player.Attribute.BodyType * 0.3f)), player.transform.position.z);
				objsPassiveEffect.Add(obj);
				break;
			case 3://Hand
				Transform tHand = player.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/DummyHand_R");
				if(tHand != null)
					objsPassiveEffect.Add(tHand.gameObject);
//				for(int i=0; i<PlayerList.Count; i++)
//					if(PlayerList[i].Team == player.Team) 
//						objs.Add(PlayerList[i].gameObject);
				break;
			case 4://
//				for(int i=0; i<PlayerList.Count; i++){
//					if(PlayerList[i].Team == player.Team) {
//						if(Vector3.Distance(PlayerList[i].gameObject.transform.position, Joysticker.gameObject.transform.position) <= GameData.SkillData[player.PassiveID].Distance(player.PassiveLv))
//							objsPassiveEffect.Add(PlayerList[i].gameObject);
//					}
//				}
				break;
			case 5://
//				for(int i=0; i<PlayerList.Count; i++)
//					if(PlayerList[i].Team != player.Team) 
//						objsPassiveEffect.Add(PlayerList[i].gameObject);
				break;
			case 6://
//				for(int i=0; i<PlayerList.Count; i++){
//					if(PlayerList[i].Team != player.Team) {
//						if(Vector3.Distance(PlayerList[i].gameObject.transform.position, Joysticker.gameObject.transform.position) <= GameData.SkillData[player.PassiveID].Distance(player.PassiveLv))
//							objsPassiveEffect.Add(PlayerList[i].gameObject);
//					}
//				}
				break;
			case 7://
				break;
			case 8://
				break;
			case 9://
				break;
			case 10://
				break;
			}
			return objsPassiveEffect;
		}
		return null;
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
			case ESkillSituation.MoveDodge:
				if(player.IsBallOwner) {
					int Dir = haveDefPlayer(ref player, GameConst.CrossOverDistance, 50);
					if(Dir != 0 && player.IsHaveMoveDodge) {
						if(Random.Range(0, 100) <= player.MoveDodgeRate) {
							Vector3 pos = CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position;
							//Crossover     
							if(player.Team == ETeamKind.Self && player.transform.position.z >= 9.5)
								return Result;
							else if(player.Team == ETeamKind.Npc && player.transform.position.z <= -9.5)
								return Result;
							
							int AddZ = 6;
							if(player.Team == ETeamKind.Npc)
								AddZ = -6;
							
							player.rotateTo(pos.x, pos.z);
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
							player.AniState(playerState);
							
							CoolDownCrossover = Time.time + 4;
							Result = true;
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
				playerState = player.PassiveSkill(ESkillSituation.Shoot0, ESkillKind.Shoot, v);
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Shoot3:
				skillKind = ESkillKind.DownHand;
				playerState = player.PassiveSkill(ESkillSituation.Shoot3, ESkillKind.DownHand);
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Shoot2:
				skillKind = ESkillKind.UpHand;
				playerState = player.PassiveSkill(ESkillSituation.Shoot2, ESkillKind.UpHand);
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.Shoot1:
				skillKind = ESkillKind.NearShoot;
				playerState = player.PassiveSkill(ESkillSituation.Shoot1, ESkillKind.NearShoot);
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
			case ESkillSituation.Steal0:	
				skillKind = ESkillKind.Steal;
				playerState = player.PassiveSkill(ESkillSituation.Steal0, ESkillKind.Steal);
				Result = player.AniState(playerState, v);
				break;
			case ESkillSituation.PickBall0:
				skillKind = ESkillKind.Pick2;
				playerState = player.PassiveSkill(ESkillSituation.PickBall0, ESkillKind.Pick2);
				Result = player.AniState(playerState, v);
				break;
			}	
		}
		try {
//			Debug.Log("Player : "+ player.name +"   ESkillSituation State : "+ State.ToString() + "  PlayerState: "+playerState.ToString() );
			if(Result && !playerState.ToString().Equals(State.ToString())){
				if(GameData.SkillData.ContainsKey(player.PassiveID)) {
//					Debug.Log("Player : "+ player.name +"   ESkillSituation State : "+ State.ToString() + "   PassiveID:"+ player.PassiveID + "   EffectID:"+ GameData.SkillData[player.PassiveID].Effect);
					OnShowEffect(player);
					player.GameRecord.PassiveSkill++;
				}
			}
		} catch {
			Debug.Log(player.name  +" is no State: "+ State.ToString() +" or have no PassiveID:"+ player.PassiveID);
		}

		return Result;
	}
	
	private void BackToDef(ref PlayerBehaviour Npc, ETeamKind Team, ref TTactical pos, bool WatchBallOwner = false)
    {
		if (pos.FileName != string.Empty) {
			if (Npc.CanMove && Npc.WaitMoveTime == 0 && Npc.TargetPosNum == 0) {			
				tacticalData = GetActionPosition(Npc.Postion.GetHashCode(), ref pos);
				
				if (tacticalData != null) {
					for (int i = 0; i < tacticalData.Length; i++) {
						if (Team == ETeamKind.Self)
							moveData.Target = new Vector2(tacticalData [i].x, -tacticalData [i].z);
						else
							moveData.Target = new Vector2(tacticalData [i].x, tacticalData [i].z);
						
						if (BallOwner != null)
							moveData.LookTarget = BallOwner.transform;
						else {
							if (Team == ETeamKind.Self)
								moveData.LookTarget = CourtMgr.Get.Hood [1].transform;
							else
								moveData.LookTarget = CourtMgr.Get.Hood [0].transform;
						}
						
						if (!WatchBallOwner)
							moveData.Speedup = true;

						moveData.FileName = pos.FileName;
						Npc.TargetPos = moveData;
					}
				}
			}
		}
	}
	
	private void TeeBall(ref PlayerBehaviour Npc, ETeamKind Team, ref TTactical pos)
	{
		moveData.Clear();
		if (Npc == BallOwner && Npc.TargetPosNum > 1) {
			if (Npc == BallOwner)
				if (!(Npc.MoveQueue.Peek().Target.y == 18 || Npc.MoveQueue.Peek().Target.y == -18))
					Npc.ResetMove();
		}
			

		if ((Npc.CanMove || Npc.CanMoveFirstDribble) && !Npc.IsMoving && Npc.WaitMoveTime == 0 && Npc.TargetPosNum == 0) {
			if (Npc == BallOwner) {
				int TargetZ = 18;
				if(Team == ETeamKind.Self)
					TargetZ = -18;

				if(Npc.Team == ETeamKind.Self && Npc.transform.position.z <= -17 && Npc.transform.position.z >= -18){
					if(waitTeeBallTime == 0)
						waitTeeBallTime = Time.time + 1;

					return;
				}
				else 
				if(Npc.Team == ETeamKind.Npc && Npc.transform.position.z >= 17 && Npc.transform.position.z <= 18){
					if(waitTeeBallTime == 0)
						waitTeeBallTime = Time.time + 1;

					return;
				}

				moveData.FileName = pos.FileName;
				moveData.Target = new Vector2(Npc.transform.position.x, TargetZ);
				moveData.MoveFinish = NpcAutoTee;
				Npc.TargetPos = moveData;
            } else 
			if (pos.FileName != string.Empty) {
				tacticalData = GetActionPosition(Npc.Index, ref pos);
                
				if (tacticalData != null) {
					for (int j = 0; j < tacticalData.Length; j++) {
						moveData.Clear();
						moveData.Speedup = tacticalData [j].Speedup;
						moveData.Catcher = tacticalData [j].Catcher;
						moveData.Shooting = tacticalData [j].Shooting;
                        if (Team == ETeamKind.Self) 
							moveData.Target = new Vector2(tacticalData [j].x, tacticalData [j].z);
                        else
							moveData.Target = new Vector2(tacticalData [j].x, -tacticalData [j].z);

						moveData.FileName = pos.FileName;
						moveData.LookTarget = CourtMgr.Get.RealBall.transform;
						Npc.TargetPos = moveData;
                    }
                }
            }
        }
        
        if (Npc.WaitMoveTime != 0 && Npc == BallOwner)
            Npc.AniState(EPlayerState.Dribble0);
    }
	
    private bool NpcAutoTee(PlayerBehaviour player, bool speedup)
    {
		if(waitTeeBallTime == 0)
		{
			waitTeeBallTime = Time.time + 1;
//			player.AniState (EPlayerState.Dribble);
		}

		return true;
    }

	public bool AutoTee()
	{
		bool Result = false;
		PlayerBehaviour getball = null;

		if (BallOwner.Team == ETeamKind.Self)
			getball = Joysticker;
		else
			getball = haveNearPlayer(BallOwner, 10, true);

		
		if (getball != null)
		{
			Result = Pass(getball, true);
		} 
		else
		{
			int ran = UnityEngine.Random.Range(0, 2);
			int count = 0;
			for (int i = 0; i < PlayerList.Count; i++)
			{
				if (PlayerList [i].Team == BallOwner.Team && PlayerList [i] != BallOwner)
				{
					if (count == ran)
					{
						Result = Pass(PlayerList [i], true);
						break;
					}
					
					count++;
				}
			}
		}

		return Result;
	}

    private PlayerBehaviour NearBall(ref PlayerBehaviour Npc)
    {
        PlayerBehaviour NearPlayer = null;

        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerBehaviour Npc1 = PlayerList [i];
            if (Npc1.Team == Npc.Team && !Npc.IsFall && Npc.NoAiTime == 0 && Npc1.CanMove)
            {
                if (NearPlayer == null)
                    NearPlayer = Npc1;
                else if (getDis(ref NearPlayer, CourtMgr.Get.RealBall.transform.position) > getDis(ref Npc1, CourtMgr.Get.RealBall.transform.position))
                    NearPlayer = Npc1;
            }
        }

        if (Npc != NearPlayer)
            NearPlayer = null;

        return NearPlayer;
    }

    private PlayerBehaviour NearBall(ETeamKind team)
    {
        PlayerBehaviour NearPlayer = null;
        
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerBehaviour Npc = PlayerList [i];
            if (Npc.Team == team)
            {
				if(team == ETeamKind.Self && Npc == Joysticker)
					continue;
				else if(Npc.NoAiTime == 0){
	                if (NearPlayer == null)
	                    NearPlayer = Npc;
	                else if (getDis(ref NearPlayer, CourtMgr.Get.RealBall.transform.position) > getDis(ref Npc, CourtMgr.Get.RealBall.transform.position))
	                    NearPlayer = Npc;
				}
            }
        }
        
        return NearPlayer;
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
		if (PlayerList.Count > 0 && !IsPassing && !IsBlocking)
        {
//			bool Suc = false;
			PlayerBehaviour Npc2;
			int Rate = Random.Range(0, 100);
			TPlayerDisData [] DisAy = GetPlayerDisAy(Npc, false, true);

			if(DisAy != null)
			{
				for (int i = 0; i < DisAy.Length; i++)
				{
					Npc2 = DisAy [i].Player;
					if (Npc2 && Npc2 != Npc && Npc2.Team != Npc.Team && Npc2.NoAiTime == 0 && 
					    !Npc2.IsSteal && 
					    !Npc2.IsPush)
					{
						float BlockRate = Npc2.Attr.BlockRate;
						
						if(Kind == 1)
							BlockRate = Npc2.Attr.FaketBlockRate;	
						
						float mAngle = GetAngle(Npc.transform, PlayerList [i].transform);
						
						if (getDis(ref Npc, ref Npc2) <= GameConst.BlockDistance && Mathf.Abs(mAngle) <= 70)
						{
							if (Rate < BlockRate)
							{
								if(DoPassiveSkill(ESkillSituation.Block, Npc2, Npc.transform.position))
								{
									//Suc = true;
									if (Kind == 1)
										Npc2.GameRecord.BeFake++;

									break;
								}
							}
						}
					}
				}
			}

//			if(!Suc)
//			{
//				for (int i = 0; i < DisAy.Length; i++)
//				{               
//					Npc2 = DisAy [i].Player;
//					
//					if (Npc2 && Npc2 != Npc && Npc2.Team != Npc.Team && Npc2.NoAiTime == 0 && 
//					    !Npc2.CheckAnimatorSate(EPlayerState.Steal) && 
//					    !Npc2.CheckAnimatorSate(EPlayerState.Push))
//					{
//						if(!IsBlocking)
//						{
//							float BlockRate = Npc2.Attr.BlockRate;
//							
//							if(Kind == 1)
//								BlockRate = Npc2.Attr.FaketBlockRate;	
//							
//							if (GameStart.Get.TestMode == GameTest.Block)
//							{
////								Npc2.AniState(EPlayerState.Block, Npc.transform.position);
//								DoPassiveSkill(TSkillSituation.Block, Npc2, Npc.transform.position);
//							} 
//							else if (getDis(ref Npc, ref Npc2) <= GameConst.BlockDistance)
//							{
//								if (Rate < BlockRate)
//								{
////									Npc2.AniState(EPlayerState.Block, Npc.transform.position);
//									DoPassiveSkill(TSkillSituation.Block, Npc2, Npc.transform.position);
//									break;
//								}
//							}
//						}else
//							break;					
//					}
//				}
//			}
		}
	}
	
	private PlayerBehaviour PickBall(ref PlayerBehaviour npc, bool findNear = false)
	{
		PlayerBehaviour player = null;
		
		if (BallOwner == null) {
            if (findNear) {
                player = NearBall(ref npc);

				if (player != null && player.CanMove && player.WaitMoveTime == 0) {
					moveData.Clear();
					moveData.FollowTarget = CourtMgr.Get.RealBall.transform;
					player.TargetPos = moveData;
				} else 
				if(npc.crtState != EPlayerState.Block && npc.NoAiTime == 0)
                    npc.rotateTo(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);
            } else 
			if (npc.CanMove && npc.WaitMoveTime == 0) {
				moveData.Clear();
				moveData.FollowTarget = CourtMgr.Get.RealBall.transform;
				npc.TargetPos = moveData;
            }
        }

        return player;
    }

    private void AIMove(ref PlayerBehaviour npc, ref TTactical pos)
    {
        if (BallOwner == null) {
			if(!Passer) {
				if(Shooter == null) {
					PickBall(ref npc, true);
					PickBall(ref npc.DefPlayer, true);
				} else {
					if((Situation == EGameSituation.AttackA && npc.Team == ETeamKind.Self) || (Situation == EGameSituation.AttackB && npc.Team == ETeamKind.Npc))
						if(!npc.IsShoot)
							PickBall(ref npc, true);

					if((Situation == EGameSituation.AttackA && npc.DefPlayer.Team == ETeamKind.Npc) || 
					   (Situation == EGameSituation.AttackB && npc.DefPlayer.Team == ETeamKind.Self)) {
						PlayerBehaviour FearPlayer = null;
						
						for (int i = 0; i < PlayerList.Count; i++) {
							PlayerBehaviour Npc1 = PlayerList [i];
							if (Npc1.Team == npc.DefPlayer.Team && !npc.DefPlayer.IsFall && npc.DefPlayer.NoAiTime == 0) {
								if (FearPlayer == null)
									FearPlayer = Npc1;
								else 
								if (getDis(ref FearPlayer, CourtMgr.Get.RealBall.transform.position) < getDis(ref Npc1, CourtMgr.Get.RealBall.transform.position))
									FearPlayer = Npc1;
							}
						}

						if (FearPlayer) {
							for (int i = 0; i < PlayerList.Count; i++) {
								if(FearPlayer.Team == PlayerList[i].Team) {
									if(PlayerList[i] != FearPlayer) {
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
			if (npc.CanMove && npc.TargetPosNum == 0) {
				for(int i = 0; i < PlayerList.Count; i++) {
					if(PlayerList[i].Team == npc.Team && PlayerList[i] != npc && 
					   pos.FileName != string.Empty && PlayerList[i].TargetPosName != pos.FileName)
						PlayerList[i].ResetMove();
				}

				if(pos.FileName != string.Empty) {
					tacticalData = GetActionPosition(npc.Postion.GetHashCode(), ref pos);

					if (tacticalData != null) {
						for (int i = 0; i < tacticalData.Length; i++) {
							moveData.Clear();
							moveData.Speedup = tacticalData [i].Speedup;
							moveData.Catcher = tacticalData [i].Catcher;
							moveData.Shooting = tacticalData [i].Shooting;
	                        int z = 1;
	                        if (npc.Team != ETeamKind.Self)
	                            z = -1;
	                        
							moveData.Target = new Vector2(tacticalData [i].x, tacticalData [i].z * z);
	                        if (BallOwner != null && BallOwner != npc)
								moveData.LookTarget = BallOwner.transform;  
	                        
							moveData.FileName = pos.FileName;
							moveData.MoveFinish = DefMove;
							npc.TargetPos = moveData;
	                    }

	                    DefMove(npc);
	                }
	            }
	        }
	        
	        if (npc.WaitMoveTime != 0 && BallOwner != null && npc == BallOwner)
	            npc.AniState(EPlayerState.Dribble0);
        }
    }

    public bool DefMove(PlayerBehaviour player, bool speedup = false)
    {
		if (player && player.DefPlayer && !player.CheckAnimatorSate(EPlayerState.MoveDodge1) && 
		   !player.CheckAnimatorSate(EPlayerState.MoveDodge0) && 
		   (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB)) {
			if (player.DefPlayer.CanMove && player.DefPlayer.WaitMoveTime == 0) {
                if (BallOwner != null) {
					int index = player.DefPlayer.Postion.GetHashCode();
					moveData.Clear();
                    if (player == BallOwner) {
						moveData.DefPlayer = player;
                        
                        if (BallOwner != null)
							moveData.LookTarget = BallOwner.transform;
                        else
							moveData.LookTarget = player.transform;
                        
						moveData.Speedup = speedup;
						player.DefPlayer.TargetPos = moveData;
                    } else {
                        float dis2;
						float z = player.DefPlayer.Team == ETeamKind.Self ? -1 : 1;
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
								z = player.DefPlayer.Team == ETeamKind.Self ? -1 : 1;
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
                        } else {
                            player.DefPlayer.ResetMove();
							z = player.DefPlayer.Team == ETeamKind.Self ? -1 : 1;
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
                } else {
                    player.DefPlayer.ResetMove();
                    PickBall(ref player.DefPlayer, true);
                }
            }
        }

        return true;
    }

    private float getDis(ref PlayerBehaviour player1, ref PlayerBehaviour player2)
    {
        if (player1 != null && player2 != null && player1 != player2)
        {
            Vector3 V1 = player1.transform.position;
            Vector3 V2 = player2.transform.position;
            V1.y = V2.y;
            return Vector3.Distance(V1, V2);
        } else
            return -1;
    }

    private float getDis(ref PlayerBehaviour player1, Vector3 Target)
    {
        if (player1 != null && Target != Vector3.zero)
        {
            Vector3 V1 = player1.transform.position;
            return Vector3.Distance(V1, Target);
        } else
            return -1;
    }

    private float getDis(ref PlayerBehaviour player1, Vector2 Target)
    {
        if (player1 != null && Target != Vector2.zero)
        {
            Vector3 V1 = new Vector3(Target.x, 0, Target.y);
            Vector3 V2 = player1.transform.position;
            V1.y = V2.y;
            return Vector3.Distance(V1, V2);
        } else
            return -1;
    }	

	private float getDis(Vector2 player1, Vector2 Target)
	{
		if (player1 != null && Target != Vector2.zero)
		{
			Vector3 V1 = new Vector3(Target.x, 0, Target.y);
			Vector3 V2 = new Vector3(player1.x, 0, player1.y);
			return Vector3.Distance(V1, V2);
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

		bool Result = false;
		IsPassing = false;
        if (p != null && Situation != EGameSituation.End) {
            if (BallOwner != null) {
                if (BallOwner.Team != p.Team) {
                    if (Situation == EGameSituation.AttackA)
                        ChangeSituation(EGameSituation.AttackB);
                    else 
					if (Situation == EGameSituation.AttackB)
                        ChangeSituation(EGameSituation.AttackA);
                } else {
                   if (Situation == EGameSituation.TeeA)
                        ChangeSituation(EGameSituation.AttackA);
                    else
					if (Situation == EGameSituation.TeeB)
                        ChangeSituation(EGameSituation.AttackB);
                    else
                        BallOwner.ResetFlag(false);
                }
            } else {
                if (Situation == EGameSituation.TeeAPicking)
					ChangeSituation(EGameSituation.TeeA);
                else 
				if (Situation == EGameSituation.TeeBPicking)
					ChangeSituation(EGameSituation.TeeB);
                else {
                    if (p.Team == ETeamKind.Self)
                        ChangeSituation(EGameSituation.AttackA, p);
                    else 
					if (p.Team == ETeamKind.Npc)
                        ChangeSituation(EGameSituation.AttackB, p);
                }
            }

			if(BallOwner != null)
				BallOwner.IsBallOwner = false;

        	BallOwner = p;
			BallOwner.WaitMoveTime = 0;
			BallOwner.IsBallOwner = true;
			Result = true;
			Shooter = null;

			if(ballHolder != null) {
				ballHolder.SetActive(true);
				ballHolder.transform.parent = BallOwner.transform;
				ballHolder.transform.localEulerAngles = Vector3.zero;
				ballHolder.transform.localScale = Vector3.one;ballHolder.transform.localPosition = new Vector3(0, 4-BallOwner.Attribute.BodyType.GetHashCode()*0.5f, 0);
			}
			
			for(int i = 0 ; i < PlayerList.Count; i++)
				PlayerList[i].ClearAutoFollowTime();

			if (BallOwner && BallOwner.DefPlayer)
				BallOwner.DefPlayer.SetAutoFollowTime();

			if (Situation == EGameSituation.AttackA || Situation == EGameSituation.AttackB) {
				for (int i = 0; i < PlayerList.Count; i++) {
					if(PlayerList[i].HaveNoAiTime) {
						PlayerList[i].HaveNoAiTime = false;
						PlayerList[i].SetNoAiTime();
					}
				}
			}

        	UIGame.Get.ChangeControl(p.Team == ETeamKind.Self);
			UIGame.Get.SetPassButton();
			CourtMgr.Get.SetBallState(EPlayerState.HoldBall, p);

        	if (p) {
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

		return Result;
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

			int Rate = UnityEngine.Random.Range(0, 100);

			if(CourtMgr.Get.RealBallState == EPlayerState.Pass0 || 
			   CourtMgr.Get.RealBallState == EPlayerState.Pass2 ||
			   CourtMgr.Get.RealBallState == EPlayerState.Pass1 || 
			   CourtMgr.Get.RealBallState == EPlayerState.Pass3)
			{
				if(BallOwner == null && (Rate > Passer.Attr.PassRate || dir == 5) && !player.IsPush)
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
			if(Situation == EGameSituation.TeeAPicking || Situation == EGameSituation.TeeBPicking)
				IsPassing = false;
			else
				return;
		}			

		if(Situation == EGameSituation.TeeAPicking && player == Joysticker)
			return;
        
		switch (dir) {
		case 0: //top ,rebound
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
						player.SetAnger(GameConst.AddAnger_Rebound);

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
			} else 
			if (Situation == EGameSituation.JumpBall) {	
				CourtMgr.Get.SetBallState(EPlayerState.JumpBall, player);
			}

			break;
		default :
			bool CanSetball = false;
			
			if (!player.IsRebound && (player.IsCatcher || player.CanMove))
			{
				if (Situation == EGameSituation.TeeAPicking)
				{
					if (player.Team == ETeamKind.Self)
						CanSetball = true;
				} else 
				if (Situation == EGameSituation.TeeBPicking)
				{
					if (player.Team == ETeamKind.Npc)
						CanSetball = true;
				} else
					CanSetball = true;
				
				if (CanSetball && !IsPickBall)
				{
					if (Situation == EGameSituation.TeeAPicking || Situation == EGameSituation.TeeBPicking){
						if(CourtMgr.Get.RealBall.transform.position.y > 1.7f)
							player.AniState(EPlayerState.CatchFlat, CourtMgr.Get.RealBall.transform.position);
//						else if(CourtMgr.Get.RealBall.transform.position.y > 1f && CourtMgr.Get.RealBall.transform.position.y <= 2f)
//							player.AniState(EPlayerState.CatchFlat, CourtMgr.Get.RealBall.transform.position);
//						else if(CourtMgr.Get.RealBall.transform.position.y > 0.5f && CourtMgr.Get.RealBall.transform.position.y <= 1f)
//							player.AniState(EPlayerState.CatchFloor, CourtMgr.Get.RealBall.transform.position);
						else
							player.AniState(EPlayerState.PickBall0, CourtMgr.Get.RealBall.transform.position);
					} else 
					if (SetBall(player)) {
						if(player.NoAiTime == 0 || player.IsIdle)
							player.AniState(EPlayerState.Dribble0);
						else if(player.IsRun || player.IsDribble)
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
				if(rate < player1.PickBall2Rate) 
					player1.AniState(EPlayerState.PickBall2, CourtMgr.Get.RealBall.transform.position);
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

							if ((BallOwner != Joysticker || (BallOwner == Joysticker && Joysticker.NoAiTime == 0)) && Random.Range(0, 100) < BallOwner.Attr.AlleyOopPassRate) {
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

		UIGameResult.Get.SetGameRecord(ref GameRecord);
	}

    private void gameResult(int team)
    {
        ChangeSituation(EGameSituation.End);
		UIGame.Get.GameOver();
		GameRecord.Done = true;
		SetGameRecord(true);
		StartCoroutine(playFinish());
    }
    
    public void PlusScore(int team, bool isSkill, bool isChangeSituation)
    {
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

				if (Passer && Passer.DribbleTime <= 1)
					Passer.GameRecord.Assist++;
			}
            
			AudioMgr.Get.PlaySound(SoundType.Net);
            UIGame.Get.PlusScore(team, score);

			if(isChangeSituation) {
				if (UIGame.Get.Scores [team] >= UIGame.Get.MaxScores [team])
					gameResult(team);
				else
				if (team == ETeamKind.Self.GetHashCode())
					ChangeSituation(EGameSituation.TeeBPicking);
				else
					ChangeSituation(EGameSituation.TeeAPicking);

				if(!isSkill && Shooter)
					Shooter.SetAnger(GameConst.AddAnger_PlusScore);
			}
		}

		Shooter = null;
		IsPassing = false;
		isFirstScore = false;
		shootDistance = 0;
    }

	private PlayerBehaviour havePartner(ref PlayerBehaviour npc, float dis, float angle)
    {
        float mangle;
        
	    for (int i = 0; i < PlayerList.Count; i++) {
			PlayerBehaviour TargetNpc = PlayerList [i];
			if (PlayerList [i] != npc && PlayerList [i].Team == npc.Team && getDis(ref npc, ref TargetNpc) <= dis &&
			    haveDefPlayer(ref TargetNpc, 1.5f, 40) == 0) {
				mangle = GetAngle(npc.transform, TargetNpc.transform);
	            
                if (mangle >= 0 && mangle <= angle)
					return TargetNpc;
                else 
				if (mangle <= 0 && mangle >= -angle) 
					return TargetNpc;
	        }
        }
        
        return null;
    }

	private int haveDefPlayer(ref PlayerBehaviour Npc, float dis, float angle) {
        int Result = 0;
        float mangle;
        
        if (PlayerList.Count > 0) {
            for (int i = 0; i < PlayerList.Count; i++) {
                if (PlayerList [i].Team != Npc.Team)  {
                    PlayerBehaviour TargetNpc = PlayerList [i];
					mangle = GetAngle(Npc.transform, TargetNpc.transform);
                    
                    if (getDis(ref Npc, ref TargetNpc) <= dis) {
                        if (mangle >= 0 && mangle <= angle) {
                            Result = 1;
                            break;
                        } else 
						if (mangle <= 0 && mangle >= -angle) {
                            Result = 2;
                            break;
                        }
                    }
                }                       
            }   
        }
        
        return Result;
    }

	private int haveDefPlayer(ref PlayerBehaviour Npc, float dis, float angle, out PlayerBehaviour man) {
		int Result = 0;
		float mangle;
		man = null;
		
		if (PlayerList.Count > 0) {
			for (int i = 0; i < PlayerList.Count; i++) {
				if (PlayerList [i].Team != Npc.Team) {
					PlayerBehaviour TargetNpc = PlayerList [i];
					mangle = GetAngle(Npc.transform, TargetNpc.transform);
					
					if (getDis(ref Npc, ref TargetNpc) <= dis && TargetNpc.CheckAnimatorSate(EPlayerState.Idle))
					{
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
                
        if (PlayerList.Count > 1) {
            for (int i = 0; i < PlayerList.Count; i++)
            {
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

    public void EditSetMove(TActionPosition ActionPosition, int index)
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
		if (Catcher != null && !Catcher.IsFall && !Catcher.IsPush && !Catcher.IsBlock && !Catcher.IsPass)
        {
            if(SetBall(Catcher))
				coolDownPass = Time.time + 3;

			if(Catcher && Catcher.NeedShooting)
			{
				Shoot();
				Catcher.NeedShooting = false;
			}
		}else{
            setDropBall(Passer);
		}

		IsPassing = false;
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

		CourtMgr.Get.SetBallState (EPlayerState.Reset);

		for (int i = 0; i < PlayerList.Count; i++) 
		{
			PlayerList [i].crtState = EPlayerState.Idle;
			PlayerList [i].ResetFlag();
			PlayerList [i].ResetCurveFlag();

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

		Joysticker.SetAnger (-100);
		Situation = EGameSituation.ShowOne;
		ChangeSituation (EGameSituation.ShowOne);
		setPassIcon(false);
    }

	public void SetPlayerLevel(){
		PlayerPrefs.SetFloat(SettingText.AITime, GameData.Setting.AIChangeTime);
		if (GameData.Setting.AIChangeTime > 100)
			Joysticker.SetNoAiTime();

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

	private TActionPosition [] GetActionPosition(int Index, ref TTactical pos)
	{
		if (Index == 0)
			return pos.PosAy1;
		else 
		if (Index == 1)
			return pos.PosAy2;
		else 
		if (Index == 2)
			return pos.PosAy3;
		else
			return null;
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
			if(Situation == EGameSituation.TeeA || Situation == EGameSituation.TeeB || Situation == EGameSituation.TeeAPicking || Situation == EGameSituation.TeeBPicking)
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
}
