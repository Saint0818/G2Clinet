using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using DG.Tweening;
using Newtonsoft.Json;

public enum EGameSituation
{
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

public enum EGameAction
{
    Def = 0,
    Attack = 1
}

public enum EBodyType
{
    Small = 0,
    Middle = 1,
    Big = 2
}

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
	AnimationUnit
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
	Steal,
	Pass0,
	Pass2,
	Pass1,
	Pass4,
	PickBall0,
	Push,
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

public struct TPlayerSkillLV{
	public int SkillID;
	public int SkillLV;
	public TSkillData Skill;

}

public enum EPassDirectState {
	Forward = 1,
	Back = 2,
	Left = 3,
	Right = 4
}

public class GameController : KnightSingleton<GameController>
{
    public EGameSituation situation = EGameSituation.None;
    private List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();

    public bool IsStart = false;
	public bool IsReset = false;
	public bool IsJumpBall = false;
    public float CoolDownPass = 0;
    public float CoolDownCrossover = 0;
    private float shootDistance = 0;
    public float RealBallFxTime = 0;
	private float WaitTeeBallTime = 0;
	private float WaitStealTime = 0;
	private float PassingStealBallTime = 0;
	private bool isPassing = false;
	private ESkillKind skillKind;
	private bool IsSkillFirstScore = false;
	private int skillFirstScore = 2;
	
	public PlayerBehaviour BallOwner;
	public PlayerBehaviour Joysticker;
	public PlayerBehaviour Shooter;
    public PlayerBehaviour Catcher;
	public PlayerBehaviour Passer;
	private PlayerBehaviour PickBallplayer = null;

    public Vector2[] TeeBackPosAy = new Vector2[3];
	public Vector3[] BornAy = new Vector3[6];

	//Score Animation Value
	private float extraScoreRate = 0;
	public string BasketAnimationName = "BasketballAction_1";
	public EBasketSituation BasketSituationType;
	private EBasketDistanceAngle basketDistanceAngle = EBasketDistanceAngle.ShortCenter;
	private List<int> TacticalDataList = new List<int>();
	private Dictionary<string, Shader> shaderCache = new Dictionary<string, Shader>();

	private TActionPosition [] tacticalData;
	private TTactical attackTactical;
	private TTactical defTactical;
	public GameObject selectMe;
	public GameObject BallHolder;
	public GameObject[] passIcon = new GameObject[3];

	private int shootAngle = 55;
	public float StealBtnLiftTime = 1f;
	public EPlayerState testState = EPlayerState.Shoot0;
	public EPlayerState[] ShootStates = new EPlayerState[]{EPlayerState.Shoot0, EPlayerState.Shoot1, EPlayerState.Shoot2, EPlayerState.Shoot3, EPlayerState.Shoot6, EPlayerState.Layup0, EPlayerState.Layup1, EPlayerState.Layup2, EPlayerState.Layup3};
	
	public TGameRecord GameRecord = new TGameRecord();

	private float angleByPlayerHoop = 0;
	private float distanceBallToBasket = 0;

    void Start()
    {
        EffectManager.Get.LoadGameEffect();
        InitPos();
        InitGame();
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
        TeeBackPosAy [0] = new Vector2(0, 8);
        TeeBackPosAy [1] = new Vector2(5.3f, 10);
        TeeBackPosAy [2] = new Vector2(-5.3f, 10);

		BornAy [0] = new Vector3 (-5, 0, -2);//G_A
		BornAy [1] = new Vector3 (0, 0, -1);//C_A
		BornAy [2] = new Vector3 (5, 0, -2);//F_A
		BornAy [3] = new Vector3 (5, 0, 2);//G_B
		BornAy [4] = new Vector3 (0, 0, 1);//C_B
		BornAy [5] = new Vector3 (-5, 0, 2);//F_B
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
        CreateTeam();
		SetBallOwnerNull ();

        situation = EGameSituation.Opening;
		ChangeSituation (EGameSituation.Opening);
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
	
	public void CreateTeam() {
        switch (GameStart.Get.TestMode)
        {               
            case EGameTest.None:
				checkPlayerID();
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, BornAy[0], GameData.Team.Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Self, BornAy[1], GameData.TeamMembers[0].Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Self, BornAy[2], GameData.TeamMembers[1].Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, BornAy[3], GameData.EnemyMembers[0].Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Npc, BornAy[4], GameData.EnemyMembers[1].Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Npc, BornAy[5], GameData.EnemyMembers[2].Player));								

                for (int i = 0; i < PlayerList.Count; i++)
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);

				//1.G(Dribble) 2.C(Rebound) 3.F
				float v1 = 0;
				float v2 = 0;
				int [] aPosAy = new int[3];
				int [] bPosAy = new int[3];
				for (int i = 0; i < PlayerList.Count; i++)
				{
					if(PlayerList[i].Team == ETeamKind.Self)
					{
						if(PlayerList[i].Player.Dribble > v1)
						{
							v1 = PlayerList[i].Player.Dribble;
							aPosAy[0] = i;
						}
					}
					else
					{
						if(PlayerList[i].Player.Dribble > v2)
						{
							v2 = PlayerList[i].Player.Dribble;
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
						if(PlayerList[i].Player.Rebound > v1 && aPosAy[0] != i)
						{
							v1 = PlayerList[i].Player.Rebound;
							aPosAy[1] = i;
						}
					}
					else
					{
						if(PlayerList[i].Player.Rebound > v2 && bPosAy[0] != i)
						{
							v2 = PlayerList[i].Player.Rebound;
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
				PlayerList[aPosAy[0]].transform.position = BornAy[0];
				PlayerList[aPosAy[1]].Postion = EPlayerPostion.C;
				PlayerList[aPosAy[1]].transform.position = BornAy[1];
				PlayerList[aPosAy[2]].Postion = EPlayerPostion.F;
				PlayerList[aPosAy[2]].transform.position = BornAy[2];

				//Team B
				PlayerList[bPosAy[0]].Postion = EPlayerPostion.G;
				PlayerList[bPosAy[0]].transform.position = BornAy[3];
				PlayerList[bPosAy[1]].Postion = EPlayerPostion.C;
				PlayerList[bPosAy[1]].transform.position = BornAy[4];
				PlayerList[bPosAy[2]].Postion = EPlayerPostion.F;
				PlayerList[bPosAy[2]].transform.position = BornAy[5];
                break;
			case EGameTest.All:
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, BornAy[0], new GameStruct.TPlayer(0)));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Self, BornAy[1], new GameStruct.TPlayer(0)));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Self, BornAy[2], new GameStruct.TPlayer(0)));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Npc, BornAy[3], new GameStruct.TPlayer(0)));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Npc, BornAy[4], new GameStruct.TPlayer(0)));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Npc, BornAy[5], new GameStruct.TPlayer(0)));								
				
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
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, ETeamKind.Self, BornAy[0], GameData.Team.Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, ETeamKind.Self, BornAy[1], GameData.TeamMembers[0].Player));	
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, ETeamKind.Self, BornAy[2], GameData.TeamMembers[1].Player));
				break;
			case EGameTest.CrossOver:
				Self = new TPlayer(0);
				Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			
				
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Self, new Vector3(0, 0, 0), Self));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, ETeamKind.Npc, new Vector3 (0, 0, 5), new TPlayer(0)));
				
				for (int i = 0; i < PlayerList.Count; i++)
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
				break;
        }

        Joysticker = PlayerList [0];

		selectMe = setEffectMagager("SelectMe");
        Joysticker.AIActiveHint = GameObject.Find("SelectMe/AI");
		Joysticker.SpeedUpView = GameObject.Find("SelectMe/Speedup").GetComponent<UISprite>();

		passIcon[0] = setEffectMagager("PassMe");

		if (Joysticker.SpeedUpView)
			Joysticker.SpeedUpView.enabled = false;

        if (PlayerList.Count > 1 && PlayerList [1].Team == Joysticker.Team) {
			passIcon[1] = setEffectMagager("PassA");
			setEffectMagager("SelectA");
		}

        if (PlayerList.Count > 2 && PlayerList [2].Team == Joysticker.Team) {
			passIcon[2] = setEffectMagager("PassB");
			setEffectMagager("SelectB");
		}
		UIGame.Get.InitLine();

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
			PlayerList [i].OnUIAnger = UIGame.Get.SetAnger;
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
			if (Input.GetKeyUp (KeyCode.D))
			{
				UIGame.Get.DoAttack();
			}

			if (Input.GetKeyDown(KeyCode.B))
			{
				UIDoubleClick.UIShow(true);
			}

			if (Input.GetKeyDown(KeyCode.T)){
				UIDoubleClick.Get.ClickStop();
			}

			if (Input.GetKeyUp (KeyCode.N))
			{
				UIDoubleClick.Get.Init();
			}

			if (situation == EGameSituation.AttackA) {
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
			else if(situation == EGameSituation.AttackB){
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

			if(Input.GetKeyDown(KeyCode.P) && Joysticker != null) 
				Joysticker.SetAnger(100);

			if(Input.GetKeyDown(KeyCode.O) && Joysticker != null) 
				UIGame.Get.DoSkill();

			jodgeSkillType ();
		}

		if (CoolDownPass > 0 && Time.time >= CoolDownPass)
            CoolDownPass = 0;

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

		if(WaitTeeBallTime > 0 && Time.time >= WaitTeeBallTime)
		{
			if(BallOwner != null)
			{
				if(AutoTee())
					WaitTeeBallTime = 0;
			}else
				WaitTeeBallTime = 0;
		}

		if(WaitStealTime > 0 && Time.time >= WaitStealTime)		
			WaitStealTime = 0;

		if(PassingStealBallTime > 0 && Time.time >= PassingStealBallTime)		
			PassingStealBallTime = 0;
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
			if(BallOwner != null)
			{
				switch(BallOwner.Postion)
				{
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
			}else
				GetMovePath(GetPosNameIndex(EPosKind.Attack), ref attackTactical);
			bool isShooting = IsShooting;
			for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerBehaviour Npc = PlayerList [i];
				if (CandoAI(Npc))
				{
					if (Npc.Team == team)
                    {                      
						if (!isShooting)
						{
                        	Attack(ref Npc);
							AIMove(ref Npc, ref attackTactical);
						} 
						else if (!Npc.IsAllShoot)
                    	{
                        	Attack(ref Npc);
							AIMove(ref Npc, ref attackTactical);
						}                                                       
                    } 
					else
                    	Defend(ref Npc);
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
				if(PickBallplayer == null)
					PickBallplayer = NearBall(team);

				if (PickBallplayer != null)
				{
					GetMovePath(GetPosNameIndex(EPosKind.Tee, PickBallplayer.Index), ref attackTactical);
					GetMovePath(GetPosNameIndex(EPosKind.TeeDefence, PickBallplayer.Index), ref defTactical);
                }                   

                for (int i = 0; i < PlayerList.Count; i++)
                {
                    PlayerBehaviour Npc = PlayerList [i];
                    if (CandoAI(Npc))
                    {
                        if (Npc.Team == team)
                        {
							if (Npc == PickBallplayer)
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

    private bool CandoAI(PlayerBehaviour npc)
    {
        if (npc.NoAiTime == 0)
            return true;
        else
            return false;
    }
    
    public void ChangeSituation(EGameSituation GS, PlayerBehaviour GetBall = null)
    {
        if (situation != EGameSituation.End)
        {
            EGameSituation oldgs = situation;
            if (situation != GS)
            {
                RealBallFxTime = 0;
				WaitStealTime = 0;
                CourtMgr.Get.RealBallFX.SetActive(false);
                for (int i = 0; i < PlayerList.Count; i++)
                {
					if(GS == EGameSituation.TeeAPicking || GS == EGameSituation.TeeBPicking)
					{
						if(PlayerList[i].NoAiTime > 0)
						{
							PlayerList[i].HaveNoAiTime = true;							
							PlayerList[i].SetAiTime();
						}

						PlayerList[i].ResetMove();
					}												

					switch(PlayerList[i].Team)
					{
					case ETeamKind.Self:
						if((GS == EGameSituation.TeeB || (oldgs == EGameSituation.TeeB && GS == EGameSituation.AttackB)) == false){
							if(PlayerList[i].NoAiTime > 0){
								if(!(GS == EGameSituation.AttackA || GS == EGameSituation.AttackB))
									PlayerList[i].ResetFlag();
							}else
								PlayerList[i].ResetFlag();
						}
						break;
					case ETeamKind.Npc:
						if((GS == EGameSituation.TeeA || (oldgs == EGameSituation.TeeA && GS == EGameSituation.AttackA)) == false)
							PlayerList[i].ResetFlag();
						break;
					}

                    PlayerList [i].situation = GS;
                }
            }
            
            situation = GS;
            
            if ((oldgs == EGameSituation.TeeA || oldgs == EGameSituation.TeeB) && oldgs != GS && GetBall != null)
            {
				GetMovePath(GetPosNameIndex(EPosKind.Fast, GetBall.Index), ref attackTactical);
                
				if(attackTactical.FileName != string.Empty)
				{
					for (int i = 0; i < PlayerList.Count; i ++)
					{
						PlayerBehaviour npc = PlayerList [i];
						if (npc.Team == GetBall.Team)
						{
							GetActionPosition(npc.Index, ref attackTactical, ref tacticalData);
							
							if (tacticalData != null)
							{
								for (int j = 0; j < tacticalData.Length; j++)
								{
									TMoveData data = new TMoveData(0);
									data.Speedup = tacticalData [j].Speedup;
									data.Catcher = tacticalData [j].Catcher;
									data.Shooting = tacticalData [j].Shooting;
									data.FileName = attackTactical.FileName;
									if(npc.Team == ETeamKind.Self)
										data.Target = new Vector2(tacticalData [j].x, tacticalData [j].z);
									else
										data.Target = new Vector2(tacticalData [j].x, -tacticalData [j].z);
									if (BallOwner != null && BallOwner != npc)
										data.LookTarget = BallOwner.transform;  
									
									npc.TargetPos = data;
								}
							}
						}
					}
				}               
			}

			CourtMgr.Get.Walls[0].SetActive(true);
			CourtMgr.Get.Walls[1].SetActive(true);

			switch (GS)
			{
			case EGameSituation.Opening:
				IsStart = true;
				break;
			case EGameSituation.JumpBall:
				for(int i = 0; i < PlayerList.Count; i++)
					if(PlayerList[i].Postion == EPlayerPostion.C)
						Rebound(PlayerList[i]);
				break;
			case EGameSituation.AttackA:
				CameraMgr.Get.SetTeamCamera(ETeamKind.Self);
				break;
			case EGameSituation.AttackB:
				CameraMgr.Get.SetTeamCamera(ETeamKind.Npc);
				break;
			case EGameSituation.TeeAPicking:
				CourtMgr.Get.Walls[1].SetActive(false);
				UIGame.Get.ChangeControl(true);
				CameraMgr.Get.SetTeamCamera(ETeamKind.Npc, true);
				PickBallplayer = null;
                break;
            case EGameSituation.TeeA:
				CourtMgr.Get.Walls[1].SetActive(false);
				setEffectMagager("ThrowInLineEffect");
                break;
            case EGameSituation.TeeBPicking:
				CourtMgr.Get.Walls[0].SetActive(false);
           	 	UIGame.Get.ChangeControl(false);
           		CameraMgr.Get.SetTeamCamera(ETeamKind.Self, true);
				PickBallplayer = null;
                break;
			case EGameSituation.TeeB:
				CourtMgr.Get.Walls[0].SetActive(false);
				setEffectMagager("ThrowInLineEffect");
				break;
			case EGameSituation.End:
				IsStart = false;
				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].AniState(EPlayerState.Idle);					
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

				if (situation != EGameSituation.None && situation != EGameSituation.Opening)
					GameRecord.GameTime += Time.deltaTime;
            
	            switch (situation)
	            {
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

	//Attack <15   Deffence >15  All
	private void jodgeSkillType (){
		if(UIGame.Get.isAngerFull && Joysticker.IsBallOwner) {
//			Vector3 v = CourtMgr.Get.ShootPoint [Joysticker.Team.GetHashCode()].transform.position;
			if(situation == EGameSituation.AttackA && UIGame.Get.isCanShowSkill) {
				if (Joysticker.activeSkill.type == EActiveDistanceType.AttackHalfCount ) {
					if (Joysticker.transform.position.z > 0)
						UIGame.Get.ShowSkillUI(true);
					else 
						UIGame.Get.ShowSkillUI(false);
				} else 
				if (Joysticker.activeSkill.type == EActiveDistanceType.DeffenceHalfCount ) {
					if (Joysticker.transform.position.z <= 0)
						UIGame.Get.ShowSkillUI(true);
					else 
						UIGame.Get.ShowSkillUI(false);
				} else 
				if (Joysticker.activeSkill.type == EActiveDistanceType.AllCount ) 
					UIGame.Get.ShowSkillUI(true);
			}else
				UIGame.Get.ShowSkillUI(false);
		} else
			UIGame.Get.ShowSkillUI(false);
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
		if(BasketSituationType == EBasketSituation.Score){
			if(CourtMgr.Get.BasketAnimationName.Count > 0 && basketDistanceAngleType < CourtMgr.Get.BasketAnimationName.Count){
				int random = Random.Range(0, CourtMgr.Get.BasketAnimationName[basketDistanceAngleType].Count);
				if(CourtMgr.Get.BasketAnimationName.Count > 0 && random < CourtMgr.Get.BasketAnimationName.Count)
					BasketAnimationName = CourtMgr.Get.BasketAnimationName[basketDistanceAngleType][random];
			}
		}else if(BasketSituationType == EBasketSituation.NoScore){
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
			setEffectMagager("ThreeLineEffect");
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
				BasketSituationType = EBasketSituation.Swish;
			else 
				BasketSituationType = EBasketSituation.Score;

			player.GameRecord.ShotError++;
        } else {
			if(isAirBall)
				BasketSituationType = EBasketSituation.AirBall;
			else 
				BasketSituationType = EBasketSituation.NoScore;
		}

		if (shootDistance >= GameConst.TreePointDistance)
			player.GameRecord.FG3++;
		else
			player.GameRecord.FG++;
		
		player.GameRecord.PushShot(new Vector2(player.transform.position.x, player.transform.position.z), BasketSituationType.GetHashCode(), rate);
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
						if(shootDistance > 9 && shootDistance <= 15)
							DoPassiveSkill(ESkillSituation.Shoot2, BallOwner, CourtMgr.Get.Hood [t].transform.position);
						else 
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
				shootAngle = 75;

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

			if(BasketSituationType == EBasketSituation.AirBall) {
				//AirBall
				#if UNITY_EDITOR
				UIHint.Get.ShowHint("AirBall", Color.yellow);
				#endif
				Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Ignore Raycast"), LayerMask.NameToLayer ("RealBall"), true);
				Vector3 ori = CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position - CourtMgr.Get.RealBall.transform.position;
				CourtMgr.Get.RealBallRigidbody.velocity = 
					GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
					                         CourtMgr.Get.RealBall.transform.position + (ori * 0.8f), shootAngle);
			} else 
			if(player.crtState == EPlayerState.TipIn) {
				if(CourtMgr.Get.RealBall.transform.position.y > (CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.y + 0.2f)) {
					CourtMgr.Get.RealBall.transform.DOMove(new Vector3(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.x,
					                                                   CourtMgr.Get.RealBall.transform.position.y - 0.1f,
					                                                   CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.z), 0.2f);
				} else {
					CourtMgr.Get.RealBall.transform.DOMove(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position, 0.2f);
				}


			} else 
			if(BasketSituationType == EBasketSituation.Swish) {
				#if UNITY_EDITOR
				UIHint.Get.ShowHint("Swish", Color.yellow);
				#endif
				Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("BasketCollider"), LayerMask.NameToLayer ("RealBall"), true);
				distanceBallToBasket = getDis(new Vector2(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z), new Vector2(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.x, CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.z));
				if(distanceBallToBasket <= 1)
					shootAngle = 80;
				else if(distanceBallToBasket > 1 && distanceBallToBasket <= 2)
					shootAngle = 70;
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
				DoPassiveSkill (ESkillSituation.Push, Joysticker, nearP.transform.position);
			else
				DoPassiveSkill (ESkillSituation.Push, Joysticker);
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
				if(!(IsBtn || MovePass) && CoolDownPass != 0)
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
						if (BallOwner && (situation == EGameSituation.AttackA || situation == EGameSituation.AttackB)) 
							BallOwner.GameRecord.Pass++;
						
						UIGame.Get.DoPassNone();
					}
				}
			}
		}

		return Result;
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
				float aiTime = BallOwner.NoAiTime;
				BallOwner.NoAiTime = 0;

//                if (BallOwner == Joysticker)
                    return Pass(PlayerList [playerid], false, true);
//                else
//					return Pass(Joysticker, false, true);

				Joysticker.NoAiTime = aiTime;
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
				int r = Mathf.RoundToInt(player.Player.Steal - BallOwner.Player.Dribble);
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
				if(BallOwner != null && HaveStealPlayer(ref player, ref BallOwner, GameConst.StealBallDistance, 15) != 0)
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
		if (StealBtnLiftTime <= 0 && IsStart && Joysticker && CandoBtn)
        {
			StealBtnLiftTime = 1f;
            if (BallOwner && BallOwner.Team != Joysticker.Team)
            {
				Joysticker.rotateTo(BallOwner.gameObject.transform.position.x, BallOwner.gameObject.transform.position.z);
//				Joysticker.AniState(EPlayerState.Steal, BallOwner.transform.position);
				DoPassiveSkill(ESkillSituation.Steal, Joysticker, BallOwner.transform.position);
            } else
//              Joysticker.AniState(EPlayerState.Steal);
				DoPassiveSkill(ESkillSituation.Steal, Joysticker);
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
		if (player.Team == ETeamKind.Self && !UIDoubleClick.Visible) {
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
    
    public void DoSkill() {
		if(CandoBtn)
			Joysticker.SetNoAiTime();

		setEffectMagager("SkillSign");

		Vector3 v = CourtMgr.Get.ShootPoint [Joysticker.Team.GetHashCode()].transform.position;
		float dis = getDis(ref Joysticker, new Vector2(v.x, v.z));
		skillFirstScore = 2;
		if(dis > GameConst.TreePointDistance)
			skillFirstScore =  3;
			
		IsSkillFirstScore = true;
		Joysticker.SetAnger(-100);
		Joysticker.SetInvincible(Joysticker.GetActiveTime());
		Joysticker.AniState((EPlayerState)System.Enum.Parse(typeof(EPlayerState), Joysticker.activeSkill.Name), CourtMgr.Get.ShootPoint [0].transform.position);
		Joysticker.GameRecord.Skill++;
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

	private void AIShoot(ref PlayerBehaviour Self)
	{
		bool suc = false;

		if (Self.IsRebound)
			suc = true;
		else
		if(!Self.CheckAnimatorSate(EPlayerState.HoldBall) && HaveDefPlayer(ref Self, 5, 40) != 0) {
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
			CoolDownPass = 0;
	}
	
	private void Attack(ref PlayerBehaviour Npc)
	{
		if (BallOwner != null)
		{
			bool dunkRate = Random.Range(0, 100) < 30;
			bool shootRate = Random.Range(0, 100) < 10;
			bool shoot3Rate = Random.Range(0, 100) < 1;
			bool passRate = Random.Range(0, 100) < 20;
			bool pushRate = Random.Range(0, 100) < Npc.Attr.PushingRate;
			bool ElbowRate = Random.Range(0, 100) < Npc.Attr.ElbowingRate;
			float ShootPointDis = 0;
            Vector3 pos = CourtMgr.Get.ShootPoint [Npc.Team.GetHashCode()].transform.position;
			PlayerBehaviour man = null;
			float Shoot3Dis = 2.5f;
			float Shoot3Angel = 40;

			if(Npc.Attr.PointRate3 >= 0 && Npc.Attr.PointRate3 <= 30)
			{
				Shoot3Dis = 3.5f;
				Shoot3Angel = 40;
			}
			else if(Npc.Attr.PointRate3 >= 31 && Npc.Attr.PointRate3 <= 50)
			{
				Shoot3Dis = 3.5f;
				Shoot3Angel = 40;
			}
			else
			{
				Shoot3Dis = 3.5f;
				Shoot3Angel = 40;
			}
            
            if (Npc.Team == ETeamKind.Self)
                ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));
            else
                ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));
            
            if (Npc == BallOwner)
            {
                //Dunk shoot shoot3 pass                
				if (ShootPointDis <= GameConst.DunkDistance && (dunkRate || Npc.CheckAnimatorSate(EPlayerState.HoldBall)) && CheckAttack(ref Npc))
                {
					AIShoot(ref Npc);
                } 
				else if (ShootPointDis <= GameConst.TwoPointDistance && (HaveDefPlayer(ref Npc.DefPlayer, 1.5f, 40) == 0 || shootRate || Npc.CheckAnimatorSate(EPlayerState.HoldBall)) && CheckAttack(ref Npc))
                {
					AIShoot(ref Npc);
				} 
				else if (ShootPointDis <= GameConst.TreePointDistance + 1 && (HaveDefPlayer(ref Npc.DefPlayer, Shoot3Dis, Shoot3Angel) == 0 || shoot3Rate || Npc.CheckAnimatorSate(EPlayerState.HoldBall)) && CheckAttack(ref Npc))
                {
					AIShoot(ref Npc);				
				} 
				else if (ElbowRate && CheckAttack(ref Npc) && (HaveDefPlayer(ref Npc, GameConst.StealBallDistance, 90, out man) != 0) && Npc.CoolDownElbow ==0 && !Npc.CheckAnimatorSate(EPlayerState.Elbow))
				{
					if(DoPassiveSkill(ESkillSituation.Elbow, Npc, man.transform.position))
					{
						CoolDownPass = 0;
						Npc.CoolDownElbow = Time.time + 3;
						RealBallFxTime = 1f;
						CourtMgr.Get.RealBallFX.SetActive(true);
					}
				}
				else if ((passRate || Npc.CheckAnimatorSate(EPlayerState.HoldBall)) && CoolDownPass == 0 && !IsShooting && !IsDunk && !Npc.CheckAnimatorSate(EPlayerState.Elbow) && BallOwner.NoAiTime == 0)
                {
                    PlayerBehaviour partner = HavePartner(ref Npc, 20, 90);

                    if (partner != null && HaveDefPlayer(ref partner, 1.5f, 40) == 0)
                    {
                        Pass(partner);
                    } 
					else
                    {
                        int Who = Random.Range(0, 2);
                        int find = 0;

                        for (int j = 0; j < PlayerList.Count; j++)
                        {
                            if (PlayerList [j].Team == Npc.Team && PlayerList [j] != Npc)
                            {
                                PlayerBehaviour anpc = PlayerList [j];
                                
                                if (HaveDefPlayer(ref anpc, 1.5f, 40) == 0 || Who == find)
                                {
                                    Pass(PlayerList [j]);
                                    break;
                                }
                                find++;
                            }
                        }
                    }
                } 
				else if (Npc.IsHaveMoveDodge && CoolDownCrossover == 0 && Npc.CanMove)
                {
					DoPassiveSkill(ESkillSituation.MoveDodge, Npc);
				}
			} 
			else
			{
				//sup push 
				PlayerBehaviour NearPlayer = HaveNearPlayer(Npc, GameConst.StealBallDistance, false);

				if (NearPlayer != null && pushRate && Npc.CoolDownPush == 0)
                {
                    //Push
					if(DoPassiveSkill(ESkillSituation.Push, Npc, NearPlayer.transform.position))
						Npc.CoolDownPush = Time.time + 3;                    
                } 
            }   
        }
    }

	public void ShowPassiveEffect (){
		setEffectMagager("SkillSign01");
	}

	public bool DoPassiveSkill(ESkillSituation State, PlayerBehaviour player = null, Vector3 v = default(Vector3)) {
		bool Result = false;

		if(player) {
			EPlayerState p ;
			switch(State) {
			case ESkillSituation.MoveDodge:
				if(player.IsBallOwner) {
					int Dir = HaveDefPlayer(ref player, GameConst.CrossOverDistance, 50);
					if(Dir != 0 && player.IsHaveMoveDodge) {
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
							player.AniState(EPlayerState.MoveDodge0);
						} else {
							player.transform.DOMoveX(player.transform.position.x + 1, GameStart.Get.CrossTimeX).SetEase(Ease.Linear);
							player.AniState(EPlayerState.MoveDodge1);
						}			
						
						CoolDownCrossover = Time.time + 4;
						Result = true;
						if(player == Joysticker)
							ShowPassiveEffect ();
						return Result;
					} 
				}
				break;
			case ESkillSituation.Block:
				skillKind = ESkillKind.Block;
				Result = player.AniState(player.PassiveSkill(ESkillSituation.Block, ESkillKind.Block, v), v);
				break;
			case ESkillSituation.Dunk0:
				skillKind = ESkillKind.Dunk;
				Result = player.AniState(player.PassiveSkill(ESkillSituation.Dunk0, ESkillKind.Dunk, v), v);
				break;
			case ESkillSituation.Shoot0:
				skillKind = ESkillKind.Shoot;
				Result = player.AniState(player.PassiveSkill(ESkillSituation.Shoot0, ESkillKind.Shoot, v), v);
				break;
			case ESkillSituation.Shoot3:
				skillKind = ESkillKind.DownHand;
				Result = player.AniState(player.PassiveSkill(ESkillSituation.Shoot3, ESkillKind.DownHand), v);
				break;
			case ESkillSituation.Shoot2:
				skillKind = ESkillKind.UpHand;
				Result = player.AniState(player.PassiveSkill(ESkillSituation.Shoot2, ESkillKind.UpHand), v);
				break;
			case ESkillSituation.Shoot1:
				skillKind = ESkillKind.NearShoot;
				Result = player.AniState(player.PassiveSkill(ESkillSituation.Shoot1, ESkillKind.NearShoot), v );
				break;
			case ESkillSituation.Layup0:
				skillKind = ESkillKind.Layup;
				Result = player.AniState(player.PassiveSkill(ESkillSituation.Layup0, ESkillKind.Layup), v);
				break;
			case ESkillSituation.Elbow:
				skillKind = ESkillKind.Elbow;
				Result = player.AniState (player.PassiveSkill(ESkillSituation.Elbow, ESkillKind.Elbow));
				break;
			case ESkillSituation.Fall1:
				Result = true;
				break;
			case ESkillSituation.Fall2:
				Result = true;
				break;
			case ESkillSituation.Pass4:{
				skillKind = ESkillKind.Pass;
				p = player.PassiveSkill(ESkillSituation.Pass4, ESkillKind.Pass, v);
				if(p != EPlayerState.Pass4)
					Result = player.AniState(p);
				else
					Result = player.AniState(p, v);
			}
				break;
			case ESkillSituation.Pass0:
				skillKind = ESkillKind.Pass;
				 p = player.PassiveSkill(ESkillSituation.Pass0, ESkillKind.Pass, v);
				if(p != EPlayerState.Pass0)
					Result = player.AniState(p);
				else
					Result = player.AniState(p, v);
				break;
			case ESkillSituation.Pass2:
				skillKind = ESkillKind.Pass;
				p = player.PassiveSkill(ESkillSituation.Pass2, ESkillKind.Pass, v);
				if(p != EPlayerState.Pass2)
					Result = player.AniState(p);
				else
					Result = player.AniState(p, v);
				break;
			case ESkillSituation.Pass1:
				skillKind = ESkillKind.Pass;
				p = player.PassiveSkill(ESkillSituation.Pass1, ESkillKind.Pass, v);
				if(p != EPlayerState.Pass1)
					Result = player.AniState(p);
				else
					Result = player.AniState(p, v);
				break;
			case ESkillSituation.Push:
				skillKind = ESkillKind.Push;
				if(v == Vector3.zero)
					Result = player.AniState(player.PassiveSkill(ESkillSituation.Push, ESkillKind.Push));
				else
					Result = player.AniState(player.PassiveSkill(ESkillSituation.Push, ESkillKind.Push), v);
				break;
			case ESkillSituation.Rebound:
				skillKind = ESkillKind.Rebound;
				Result = player.AniState (player.PassiveSkill(ESkillSituation.Rebound, ESkillKind.Rebound));
				break;
			case ESkillSituation.Steal:	
				skillKind = ESkillKind.Steal;		
				Result = player.AniState(player.PassiveSkill(ESkillSituation.Steal, ESkillKind.Steal), v);
				break;
			case ESkillSituation.PickBall0:
				skillKind = ESkillKind.Pick2;
				Result = player.AniState(player.PassiveSkill(ESkillSituation.PickBall0, ESkillKind.Pick2), v);
				break;
			}	
		}

		return Result;
	}

	struct TPlayerDisData
	{
		public PlayerBehaviour Player;
		public float Distance;
	}

	private void Defend(ref PlayerBehaviour Npc)
	{
		if (BallOwner != null)
		{
			bool pushRate = Random.Range(0, 100) < Npc.Attr.PushingRate;        
			bool sucess = false;
			bool isPush =  IsPush;

            //steal push Def
			if (!IsShooting && Npc.NoAiTime == 0)
			{

				if (BallOwner != null && !Npc.CheckAnimatorSate(EPlayerState.Steal) && !Npc.CheckAnimatorSate(EPlayerState.Push) && !IsDunk && !IsShooting)
                {
					TPlayerDisData [] DisAy = GetPlayerDisAy(Npc);

					for(int i = 0; i < DisAy.Length; i++)
					{
						if (DisAy[i].Distance <= GameConst.StealBallDistance && 
						    (DisAy[i].Player.crtState == EPlayerState.Idle && DisAy[i].Player.crtState == EPlayerState.Dribble0) && 
						    pushRate && Npc.CoolDownPush == 0 && !isPush)
						{
							if(DoPassiveSkill(ESkillSituation.Push, Npc, DisAy[i].Player.transform.position))
							{
								Npc.CoolDownPush = Time.time + GameConst.CoolDownPushTime;
								sucess = true;
							}
							break;
						} 
					}

					if (!sucess && DisAy[0].Distance <= GameConst.StealBallDistance && WaitStealTime == 0 && BallOwner.Invincible == 0 && Npc.CoolDownSteal == 0)
                	{
						if(Random.Range(0, 100) < Npc.Attr.StealRate)
                    	{
							if(DoPassiveSkill(ESkillSituation.Steal, Npc, BallOwner.gameObject.transform.position))
							{
                        		Npc.CoolDownSteal = Time.time + GameConst.CoolDownSteal;                              
								WaitStealTime = Time.time + GameConst.WaitStealTime;
							}
                    	}
                	}                    
                }
            }               
        }
    }

	private void BackToDef(ref PlayerBehaviour Npc, ETeamKind Team, ref TTactical pos, bool WatchBallOwner = false)
    {
		if(pos.FileName != string.Empty)
		{
			if (Npc.CanMove && Npc.WaitMoveTime == 0 && Npc.TargetPosNum == 0)
			{
				TMoveData data = new TMoveData(0);				
				GetActionPosition(Npc.Index, ref pos, ref tacticalData);
				
				if (tacticalData != null)
				{
					for (int i = 0; i < tacticalData.Length; i++)
					{
						if (Team == ETeamKind.Self)
							data.Target = new Vector2(tacticalData [i].x, -tacticalData [i].z);
						else
							data.Target = new Vector2(tacticalData [i].x, tacticalData [i].z);
						
						if (BallOwner != null)
							data.LookTarget = BallOwner.transform;
						else
						{
							if (Team == ETeamKind.Self)
								data.LookTarget = CourtMgr.Get.Hood [1].transform;
							else
								data.LookTarget = CourtMgr.Get.Hood [0].transform;
						}
						
						if (!WatchBallOwner)
							data.Speedup = true;

						data.FileName = pos.FileName;
						Npc.TargetPos = data;
					}
				}
			}
		}
	}
	
	private void TeeBall(ref PlayerBehaviour Npc, ETeamKind Team, ref TTactical pos)
	{
		TMoveData data = new TMoveData(0);

		if(Npc == BallOwner && Npc.TargetPosNum > 1)
		{
			if (Npc == BallOwner)
				if(!(Npc.MoveQueue.Peek().Target.y == 18 || Npc.MoveQueue.Peek().Target.y == -18))
					Npc.ResetMove();
		}
			

		if ((Npc.CanMove || Npc.CanMoveFirstDribble) && !Npc.IsMoving && Npc.WaitMoveTime == 0 && Npc.TargetPosNum == 0)
		{
			if (Npc == BallOwner)
			{
				int TargetZ = 18;
				if(Team == ETeamKind.Self)
					TargetZ = -18;

				if(Npc.Team == ETeamKind.Self && Npc.transform.position.z <= -17 && Npc.transform.position.z >= -18){
					if(WaitTeeBallTime == 0)
						WaitTeeBallTime = Time.time + 1;
					return;
				}
				else if(Npc.Team == ETeamKind.Npc && Npc.transform.position.z >= 17 && Npc.transform.position.z <= 18){
					if(WaitTeeBallTime == 0)
						WaitTeeBallTime = Time.time + 1;
					return;
				}

				data.FileName = pos.FileName;
				data.Target = new Vector2(Npc.transform.position.x, TargetZ);
                data.MoveFinish = NpcAutoTee;
                Npc.TargetPos = data;
            } 
			else 
			if(pos.FileName != string.Empty)
            {
				GetActionPosition(Npc.Index, ref pos, ref tacticalData);
                
				if (tacticalData != null)
                {
					for (int j = 0; j < tacticalData.Length; j++)
                    {
                        data = new TMoveData(0);
						data.Speedup = tacticalData [j].Speedup;
						data.Catcher = tacticalData [j].Catcher;
						data.Shooting = tacticalData [j].Shooting;
                        if (Team == ETeamKind.Self) 
							data.Target = new Vector2(tacticalData [j].x, tacticalData [j].z);
                        else
							data.Target = new Vector2(tacticalData [j].x, -tacticalData [j].z);

						data.FileName = pos.FileName;
                        data.LookTarget = CourtMgr.Get.RealBall.transform;
                        Npc.TargetPos = data;
                    }
                }
            }
        }
        
        if (Npc.WaitMoveTime != 0 && Npc == BallOwner)
            Npc.AniState(EPlayerState.Dribble0);
    }
	
    private bool NpcAutoTee(PlayerBehaviour player, bool speedup)
    {
		if(WaitTeeBallTime == 0)
		{
			WaitTeeBallTime = Time.time + 1;
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
			getball = HaveNearPlayer(BallOwner, 10, true);

		
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
					    !Npc2.CheckAnimatorSate(EPlayerState.Steal) && 
					    !Npc2.CheckAnimatorSate(EPlayerState.Push))
					{
						float BlockRate = Npc2.Attr.BlockRate;
						
						if(Kind == 1)
							BlockRate = Npc2.Attr.FaketBlockRate;	
						
						float mAngle = GetAngle(Npc.transform, PlayerList [i].transform);
						
						if (getDis(ref Npc, ref Npc2) <= GameConst.BlockDistance && Mathf.Abs(mAngle) <= 90)
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
	
	private PlayerBehaviour PickBall(ref PlayerBehaviour Npc, bool findNear = false)
	{
		PlayerBehaviour A = null;
		
		if (BallOwner == null)
        {
            if (findNear)
            {
                A = NearBall(ref Npc);

				if (A != null && A.CanMove && A.WaitMoveTime == 0)
                {
                    TMoveData data = new TMoveData(0);
                    data.FollowTarget = CourtMgr.Get.RealBall.transform;
                    A.TargetPos = data;
				} else 
				if(Npc.crtState != EPlayerState.Block && Npc.NoAiTime == 0)
                    Npc.rotateTo(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);
            } else 
			if (Npc.CanMove && Npc.WaitMoveTime == 0) {
                TMoveData data = new TMoveData(0);
                data.FollowTarget = CourtMgr.Get.RealBall.transform;
                Npc.TargetPos = data;
            }
        }

        return A;
    }

    private void AIMove(ref PlayerBehaviour npc, ref TTactical pos)
    {
        if (BallOwner == null)
        {
			if(!Passer){
				if(Shooter == null)
				{
					PickBall(ref npc, true);
					PickBall(ref npc.DefPlayer, true);
				}
				else
				{
					if((situation == EGameSituation.AttackA && npc.Team == ETeamKind.Self) || (situation == EGameSituation.AttackB && npc.Team == ETeamKind.Npc))
						PickBall(ref npc, true);

					if((situation == EGameSituation.AttackA && npc.DefPlayer.Team == ETeamKind.Npc) || (situation == EGameSituation.AttackB && npc.DefPlayer.Team == ETeamKind.Self))
					{
						PlayerBehaviour FearPlayer = null;
						
						for (int i = 0; i < PlayerList.Count; i++)
						{
							PlayerBehaviour Npc1 = PlayerList [i];
							if (Npc1.Team == npc.DefPlayer.Team && !npc.DefPlayer.IsFall && npc.DefPlayer.NoAiTime == 0)
							{
								if (FearPlayer == null)
									FearPlayer = Npc1;
								else if (getDis(ref FearPlayer, CourtMgr.Get.RealBall.transform.position) < getDis(ref Npc1, CourtMgr.Get.RealBall.transform.position))
									FearPlayer = Npc1;
							}
						}

						if (FearPlayer) {
							for (int i = 0; i < PlayerList.Count; i++)
							{
								if(FearPlayer.Team == PlayerList[i].Team)
								{
									if(PlayerList[i] != FearPlayer)
									{
										if (PlayerList[i] != null && PlayerList[i].CanMove && PlayerList[i].WaitMoveTime == 0)
										{
											TMoveData data = new TMoveData(0);
											data.FollowTarget = CourtMgr.Get.RealBall.transform;
											PlayerList[i].TargetPos = data;
										}
									}
	//								else
	//								{
	//									if (PlayerList[i] != null && PlayerList[i].CanMove && PlayerList[i].WaitMoveTime == 0)
	//									{
	//										TMoveData data = new TMoveData(0);
	//										data.Target = new Vector2(SceneMgr.Get.Hood[PlayerList[i].Team.GetHashCode()].transform.position.x, SceneMgr.Get.Hood[PlayerList[i].Team.GetHashCode()].transform.position.z);
	//										PlayerList[i].TargetPos = data;
	//									}
	//								}
								}
							}
						}
					}
				}
			}
        } else
        {
			TMoveData data;

			if (npc.CanMove && npc.TargetPosNum == 0)
	        {
				for(int i = 0; i < PlayerList.Count; i++)
				{
					if(PlayerList[i].Team == npc.Team && PlayerList[i] != npc && pos.FileName != string.Empty && PlayerList[i].TargetPosName != pos.FileName)
					{
						PlayerList[i].ResetMove();
					}
				}

				if(pos.FileName != string.Empty)
	            {
					GetActionPosition(npc.Postion.GetHashCode(), ref pos, ref tacticalData);

					if (tacticalData != null)
	                {
						for (int i = 0; i < tacticalData.Length; i++)
	                    {
	                        data = new TMoveData(0);
							data.Speedup = tacticalData [i].Speedup;
							data.Catcher = tacticalData [i].Catcher;
							data.Shooting = tacticalData [i].Shooting;
	                        int z = 1;
	                        if (npc.Team != ETeamKind.Self)
	                            z = -1;
	                        
							data.Target = new Vector2(tacticalData [i].x, tacticalData [i].z * z);
	                        if (BallOwner != null && BallOwner != npc)
	                            data.LookTarget = BallOwner.transform;  
	                        
							data.FileName = pos.FileName;
	                        data.MoveFinish = DefMove;
	                        npc.TargetPos = data;
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
		if (player && player.DefPlayer && !player.CheckAnimatorSate(EPlayerState.MoveDodge1) && !player.CheckAnimatorSate(EPlayerState.MoveDodge0) && (situation == EGameSituation.AttackA || situation == EGameSituation.AttackB))
        {
			if (player.DefPlayer.CanMove && player.DefPlayer.WaitMoveTime == 0)
            {
                TMoveData data2 = new TMoveData(0);

                if (BallOwner != null)
                {
                    if (player == BallOwner)
                    {
                        data2.DefPlayer = player;
                        
                        if (BallOwner != null)
                            data2.LookTarget = BallOwner.transform;
                        else
                            data2.LookTarget = player.transform;
                        
                        data2.Speedup = speedup;
                        player.DefPlayer.TargetPos = data2;
                    } else
                    {
                        float dis2;
                        if (player.DefPlayer.Team == ETeamKind.Self)
                            dis2 = Vector2.Distance(new Vector2(TeeBackPosAy [player.DefPlayer.Index].x, -TeeBackPosAy [player.DefPlayer.Index].y), 
                                                    new Vector2(player.DefPlayer.transform.position.x, player.DefPlayer.transform.position.z));
                        else
                            dis2 = Vector2.Distance(TeeBackPosAy [player.DefPlayer.Index], 
                                                    new Vector2(player.DefPlayer.transform.position.x, player.DefPlayer.transform.position.z));
                        
						if (dis2 <= player.DefPlayer.Attr.DefDistance)
                        {
							PlayerBehaviour p = HaveNearPlayer(player.DefPlayer, player.DefPlayer.Attr.DefDistance, false, true);
                            if (p != null)
                                data2.DefPlayer = p;
							else if (getDis(ref player, ref player.DefPlayer) <= player.DefPlayer.Attr.DefDistance)
                                data2.DefPlayer = player;
                            
                            if (data2.DefPlayer != null)
                            {
                                if (BallOwner != null)
                                    data2.LookTarget = BallOwner.transform;
                                else
                                    data2.LookTarget = player.transform;
                                
                                data2.Speedup = speedup;
                                player.DefPlayer.TargetPos = data2;
                            } else
                            {
                                player.DefPlayer.ResetMove();

                                TMoveData data = new TMoveData(0);
                                if (player.DefPlayer.Team == ETeamKind.Self)
                                    data.Target = new Vector2(TeeBackPosAy [player.Index].x, -TeeBackPosAy [player.Index].y);
                                else
                                    data.Target = TeeBackPosAy [player.Index];
                                
                                if (BallOwner != null)
                                    data.LookTarget = BallOwner.transform;
                                else
                                {
                                    if (player.Team == ETeamKind.Self)
                                        data.LookTarget = CourtMgr.Get.Hood [1].transform;
                                    else
                                        data.LookTarget = CourtMgr.Get.Hood [0].transform;
                                }                                   

                                player.DefPlayer.TargetPos = data;
                            }
                        } else
                        {
                            player.DefPlayer.ResetMove();
                        
                            TMoveData data = new TMoveData(0);
                            if (player.DefPlayer.Team == ETeamKind.Self)
                                data.Target = new Vector2(TeeBackPosAy [player.Index].x, -TeeBackPosAy [player.Index].y);
                            else
                                data.Target = TeeBackPosAy [player.Index];
                            
                            if (BallOwner != null)
                                data.LookTarget = BallOwner.transform;
                            else
                            {
                                if (player.Team == ETeamKind.Self)
                                    data.LookTarget = CourtMgr.Get.Hood [1].transform;
                                else
                                    data.LookTarget = CourtMgr.Get.Hood [0].transform;
                            }                                   
                            
                            player.DefPlayer.TargetPos = data;                         
                        }
                    }
                } else
                {
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
		if (PlayerList.Count > 0) {
			IsPassing = false;
            if (p != null && situation != EGameSituation.End) {
                if (BallOwner != null) {
                    if (BallOwner.Team != p.Team) {
                        if (situation == EGameSituation.AttackA)
                            ChangeSituation(EGameSituation.AttackB);
                        else if (situation == EGameSituation.AttackB)
                            ChangeSituation(EGameSituation.AttackA);
                    } else {
                        if (situation == EGameSituation.TeeA)
                            ChangeSituation(EGameSituation.AttackA);
                        else if (situation == EGameSituation.TeeB)
                            ChangeSituation(EGameSituation.AttackB);
                        else
                            BallOwner.ResetFlag(false);
                    }
                } else {
                    if (situation == EGameSituation.TeeAPicking)
						ChangeSituation(EGameSituation.TeeA);
                    else 
					if (situation == EGameSituation.TeeBPicking)
						ChangeSituation(EGameSituation.TeeB);
                    else {
                        if (p.Team == ETeamKind.Self)
                            ChangeSituation(EGameSituation.AttackA, p);
                        else if (p.Team == ETeamKind.Npc)
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

				if(BallHolder != null) {
					BallHolder.SetActive(true);
					BallHolder.transform.parent = BallOwner.transform;
					BallHolder.transform.localEulerAngles = Vector3.zero;
					BallHolder.transform.localScale = Vector3.one;BallHolder.transform.localPosition = new Vector3(0, 4-BallOwner.Player.BodyType.GetHashCode()*0.5f, 0);
				}
				
				for(int i = 0 ; i < PlayerList.Count; i++)
					PlayerList[i].ClearAutoFollowTime();

				if (BallOwner && BallOwner.DefPlayer)
					BallOwner.DefPlayer.SetAutoFollowTime();

				if (situation == EGameSituation.AttackA || situation == EGameSituation.AttackB) {
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
					 
					if(p.IsIdle)
						p.AniState(EPlayerState.Dribble0);
					else if(p.IsRun)
						p.AniState(EPlayerState.Dribble1);        
                }
            } else {
				if(BallHolder != null)				
					BallHolder.SetActive(false);
                
				SetBallOwnerNull();
			}
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
		if(player.IsDefence && (situation == EGameSituation.AttackA || situation == EGameSituation.AttackB) && Passer && PassingStealBallTime == 0)
		{
			int Rate = UnityEngine.Random.Range(0, 100);

			if(CourtMgr.Get.RealBallState == EPlayerState.Pass0 || 
			   CourtMgr.Get.RealBallState == EPlayerState.Pass2 ||
			   CourtMgr.Get.RealBallState == EPlayerState.Pass1 || 
			   CourtMgr.Get.RealBallState == EPlayerState.Pass3)
			{
				if(BallOwner == null && (Rate > Passer.Attr.PassRate || dir == 5) && !player.CheckAnimatorSate(EPlayerState.Push))
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
								PassingStealBallTime = Time.time + 2;
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
							PassingStealBallTime = Time.time + 2;
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
		    player.CheckAnimatorSate(EPlayerState.Push) || 
		    dir == 6)
            return;

		if (Catcher) {
			if(situation == EGameSituation.TeeAPicking || situation == EGameSituation.TeeBPicking)
				IsPassing = false;
			else
				return;
		}			

		if(situation == EGameSituation.TeeAPicking && player == Joysticker)
			return;
        
		switch (dir) {
		case 0: //top ,rebound
			if ((isEnter || GameStart.Get.TestMode == EGameTest.Rebound) && player != BallOwner && CourtMgr.Get.RealBall.transform.position.y >= 3 && (situation == EGameSituation.AttackA || situation == EGameSituation.AttackB)) {
				if (GameStart.Get.TestMode == EGameTest.Rebound || situation == EGameSituation.AttackA || situation == EGameSituation.AttackB) {
					if (GameStart.Get.TestMode == EGameTest.Rebound || CourtMgr.Get.RealBallState ==  EPlayerState.Steal || CourtMgr.Get.RealBallState ==  EPlayerState.Rebound) {
						if (Random.Range(0, 100) < player.Attr.ReboundRate) {
							Rebound(player);
						}
					}
				}
			}
            break;
		case 5: //finger
			if (isEnter && !player.IsBallOwner && player.IsRebound && !IsTipin) {
				if (GameStart.Get.TestMode == EGameTest.Rebound || situation == EGameSituation.AttackA || situation == EGameSituation.AttackB) {
					if (SetBall(player)) {
						player.GameRecord.Rebound++;
						player.SetAnger(GameConst.AddAnger_Rebound);

						if (player == BallOwner && inTipinDistance(player)) {
							CoolDownPass = Time.time + 3;
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
			if (situation == EGameSituation.JumpBall) {	
				CourtMgr.Get.SetBallState(EPlayerState.JumpBall, player);
			}

			break;
		default :
			bool CanSetball = false;
			
			if (!player.IsRebound && (player.IsCatcher || player.CanMove))
			{
				if (situation == EGameSituation.TeeAPicking)
				{
					if (player.Team == ETeamKind.Self)
						CanSetball = true;
				} else 
				if (situation == EGameSituation.TeeBPicking)
				{
					if (player.Team == ETeamKind.Npc)
						CanSetball = true;
				} else
					CanSetball = true;
				
				if (CanSetball && !IsPickBall)
				{
					if (situation == EGameSituation.TeeAPicking || situation == EGameSituation.TeeBPicking){
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
			if (BallOwner == null && Shooter == null && Catcher == null && (situation == EGameSituation.AttackA || situation == EGameSituation.AttackB)) {
				int rate = Random.Range(0, 100);
				if(rate < player1.Attr.StaminaValue) 
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
		 	situation == EGameSituation.AttackA || situation == EGameSituation.AttackB)) {
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

    private void GameResult(int team)
    {
        GameController.Get.ChangeSituation(EGameSituation.End);
		UIGame.Get.GameOver();
		GameRecord.Done = true;
		SetGameRecord(true);
		UIGameResult.Get.SetGameRecord(ref GameRecord);
    }
    
    public void PlusScore(int team, bool isSkill, bool isChangeSituation)
    {
        if (IsStart && GameStart.Get.TestMode == EGameTest.None)
		{
            int score = 2;
			if(IsSkillFirstScore){
				IsSkillFirstScore = false;
				score = skillFirstScore;
			}

			if (shootDistance >= GameConst.TreePointDistance)
				score = 3;

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
					GameResult(team);
				else
				if (team == ETeamKind.Self.GetHashCode())
					ChangeSituation(EGameSituation.TeeBPicking);
				else
					ChangeSituation(EGameSituation.TeeAPicking);

				if(!isSkill) {
					if(Shooter != null)
						Shooter.SetAnger(GameConst.AddAnger_PlusScore);
				}
			}
		}

		Shooter = null;
		IsPassing = false;
		shootDistance = 0;
    }

    public PlayerBehaviour HavePartner(ref PlayerBehaviour Npc, float dis, float angle)
    {
        PlayerBehaviour Result = null;
        float mangle;
        
        if (PlayerList.Count > 0)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList [i] != Npc && PlayerList [i].Team == Npc.Team)
                {
                    PlayerBehaviour TargetNpc = PlayerList [i];
					mangle = GetAngle(Npc.transform, TargetNpc.transform);
                    
                    if (getDis(ref Npc, ref TargetNpc) <= dis)
                    {
                        if (mangle >= 0 && mangle <= angle)
                        {
                            Result = TargetNpc;
                            break;
                        } else if (mangle <= 0 && mangle >= -angle)
                        {
                            Result = TargetNpc;
                            break;
                        }
                    }
                }                       
            }   
        }
        
        return Result;
    }

    public int HaveDefPlayer(ref PlayerBehaviour Npc, float dis, float angle)
    {
        int Result = 0;
        float mangle;
        
        if (PlayerList.Count > 0)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList [i].Team != Npc.Team)
                {
                    PlayerBehaviour TargetNpc = PlayerList [i];
					mangle = GetAngle(Npc.transform, TargetNpc.transform);
                    
                    if (getDis(ref Npc, ref TargetNpc) <= dis)
                    {
                        if (mangle >= 0 && mangle <= angle)
                        {
                            Result = 1;
                            break;
                        } else if (mangle <= 0 && mangle >= -angle)
                        {
                            Result = 2;
                            break;
                        }
                    }
                }                       
            }   
        }
        
        return Result;
    }

	public int HaveDefPlayer(ref PlayerBehaviour Npc, float dis, float angle, out PlayerBehaviour man)
	{
		int Result = 0;
		float mangle;
		man = null;
		
		if (PlayerList.Count > 0)
		{
			for (int i = 0; i < PlayerList.Count; i++)
			{
				if (PlayerList [i].Team != Npc.Team)
				{
					PlayerBehaviour TargetNpc = PlayerList [i];
					mangle = GetAngle(Npc.transform, TargetNpc.transform);
					
					if (getDis(ref Npc, ref TargetNpc) <= dis && TargetNpc.CheckAnimatorSate(EPlayerState.Idle))
					{
						if (mangle >= 0 && mangle <= angle)
						{
							Result = 1;
							man = TargetNpc;
							break;
						} else if (mangle <= 0 && mangle >= -angle)
						{
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

	public int HaveStealPlayer(ref PlayerBehaviour P1, ref PlayerBehaviour P2, float dis, float angle)
	{
		int Result = 0;
		float mangle;

		if (P1 != null && P2 != null && P1 != P2) 
		{
			mangle = GetAngle(P1.transform, P2.transform);
			
			if (getDis(ref P1, ref P2) <= dis)
			{
				if (mangle >= 0 && mangle <= angle)				
					Result = 1;
				else if (mangle <= 0 && mangle >= -angle)				
					Result = 2;
			}
		}
		
		return Result;
	}
    
    private PlayerBehaviour HaveNearPlayer(PlayerBehaviour Self, float Dis, bool SameTeam, bool FindBallOwnerFirst = false)
    {
        PlayerBehaviour Result = null;
        PlayerBehaviour Npc = null;
                
        if (PlayerList.Count > 1)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                Npc = PlayerList [i];
                
                if (SameTeam)
                {
                    if (PlayerList [i] != Self && PlayerList [i].Team == Self.Team && getDis(ref Self, ref Npc) <= Dis)
                    {
                        Result = Npc;
                        break;
                    }
                } else
                {
                    if (FindBallOwnerFirst)
                    {
                        if (Npc != Self && Npc.Team != Self.Team && Npc == BallOwner && getDis(ref Self, ref Npc) <= Dis)
                        {
                            Result = Npc;
                            break;
                        }
                    } else
                    {
                        if (Npc != Self && Npc.Team != Self.Team && getDis(ref Self, ref Npc) <= Dis && Npc.crtState == EPlayerState.Idle)
                        {
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
            TMoveData data = new TMoveData(0);
            data.Target = new Vector2(ActionPosition.x, ActionPosition.z);
            data.Speedup = ActionPosition.Speedup;
			data.Catcher = ActionPosition.Catcher;
			data.Shooting = ActionPosition.Shooting;
            PlayerList [index].TargetPos = data;
        }
    }

	public void EditSetMove(Vector2 ActionPosition, int index)
	{
		if (PlayerList.Count > index)
		{
			TMoveData data = new TMoveData(0);
			data.Target = ActionPosition;
			PlayerList [index].TargetPos = data;
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
		if (Catcher != null && !Catcher.IsFall && !Catcher.CheckAnimatorSate(EPlayerState.Push) && !Catcher.IsBlock)
        {
            if(SetBall(Catcher))
				CoolDownPass = Time.time + 3;

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
		CourtMgr.Get.SetBallState(EPlayerState.Steal, player);
	}
	
	public void Reset()
	{
		IsReset = true;
		IsPassing = false;
		Shooter = null;
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
					PlayerList[i].transform.position = BornAy[0];
				else
					PlayerList[i].transform.position = BornAy[3];
			}
			else if(PlayerList[i].Postion == EPlayerPostion.C)
			{
				if(PlayerList[i].Team == ETeamKind.Self)
					PlayerList[i].transform.position = BornAy[1];
				else
					PlayerList[i].transform.position = BornAy[4];
			}
			else
			{
				if(PlayerList[i].Team == ETeamKind.Self)
					PlayerList[i].transform.position = BornAy[2];
				else
					PlayerList[i].transform.position = BornAy[5];
			}

			PlayerList [i].AniState(EPlayerState.Idle);

			if(PlayerList[i].Team == ETeamKind.Npc)
				PlayerList[i].transform.localEulerAngles = new Vector3(0, 180, 0);
			else
				PlayerList[i].transform.localEulerAngles = Vector3.zero;
		}

		Joysticker.SetAnger (-100);
		situation = EGameSituation.Opening;
		ChangeSituation (EGameSituation.Opening);
    }

	public void SetPlayerLevel(){
		PlayerPrefs.SetFloat(SettingText.AITime, GameData.Setting.AIChangeTime);
		if (GameData.Setting.AIChangeTime > 100)
			Joysticker.SetNoAiTime();

		for (int i = 0; i < PlayerList.Count; i++) {
			if (PlayerList[i].Team == ETeamKind.Npc)
				PlayerList[i].Player.AILevel = GameConst.NpcAILevel;
			else
				PlayerList[i].Player.AILevel = GameConst.SelfAILevel;

			if(PlayerList[i].Team == ETeamKind.Npc)
				PlayerList[i].transform.localEulerAngles = new Vector3(0, 180, 0);
			else
				PlayerList[i].transform.localEulerAngles = Vector3.zero;

			PlayerList[i].InitAttr();
		}
	}

	public void SetBodyMaterial(int kind){
		switch (kind) {
			case 1:
				SetBodyMaterial(true, 1);
				SetBodyMaterial(false, 2);
				break;
			case 2:
				SetBodyMaterial(false, 1);
				SetBodyMaterial(true, 2);
				break;
			case 3:
			case 4:
				SetBodyMaterial(true, 1);
				SetBodyMaterial(true, 2);
				break;
			 default:
				SetBodyMaterial(false, 1);
				SetBodyMaterial(false, 2);
				break;
		}
	}

	public void SetBodyMaterial(bool open, int index) {
		if (PlayerList.Count > 0 && index < PlayerList.Count) {
			string name = "Shaders/Toony-Transparent";
			if (open)
				name = "Shaders/Toony-BasicOutline";

			Shader shader = loadShader(name);
			if (shader) {
				switch(index) {
				case 0:
					PlayerList[index].BodyMaterial.shader = shader;
					break;
				case 1:
					PlayerList[index].BodyMaterial.shader = shader;
					if(open){
						PlayerList[index].BodyMaterial.SetColor("_OutlineColor", Color.yellow);
						PlayerList[index].BodyMaterial.SetFloat("_Outline", 0.002f);
					}
					break;
				case 2:
					PlayerList[index].BodyMaterial.shader = shader;
					if(open){
						PlayerList[index].BodyMaterial.SetColor("_OutlineColor", Color.blue);
						PlayerList[index].BodyMaterial.SetFloat("_Outline", 0.002f);
					}
					break;
				}
			}
		}
	}

	private GameObject setEffectMagager (string effectName){
		GameObject obj = null;
		switch (effectName) {
		case "SkillSign":
		case "SkillSign01" :
			obj = EffectManager.Get.PlayEffect(effectName, new Vector3(0, (4 - (Joysticker.Player.BodyType * 0.5f)), 0), Joysticker.gameObject, null, 0.5f);
			break;
		case "ThreeLineEffect":
		case "ThrowInLineEffect":
			obj = EffectManager.Get.PlayEffect(effectName, Vector3.zero, null, null, 0);
			break;
		case "PassMe":
			obj = EffectManager.Get.PlayEffect(effectName, new Vector3(0, (4 - (Joysticker.Player.BodyType * 0.3f)), 0), Joysticker.gameObject);
			break;
		case "PassA":
			obj = EffectManager.Get.PlayEffect(effectName, new Vector3(0, (4 - (PlayerList [1].Player.BodyType * 0.3f)), 0), PlayerList [1].gameObject);
			break;
		case "PassB":
			obj = EffectManager.Get.PlayEffect(effectName, new Vector3(0, (4 - (PlayerList [2].Player.BodyType * 0.3f)), 0), PlayerList [2].gameObject);
			break;
		case "SelectA":
			obj = EffectManager.Get.PlayEffect(effectName, Vector3.zero, null, PlayerList [1].gameObject);
			break;
		case "SelectB":
			obj = EffectManager.Get.PlayEffect(effectName, Vector3.zero, null, PlayerList [2].gameObject);
			break;
		case "SelectMe":
			obj = EffectManager.Get.PlayEffect(effectName, Vector3.zero, null, Joysticker.gameObject);
			break;
		}
		return obj;
	}	

	private TActionPosition [] GetActionPosition(int Index, ref TTactical pos, ref TActionPosition [] Result)
	{
		Result = null;
		
		if (Index == 0)
			Result = pos.PosAy1;
		else if (Index == 1)
			Result = pos.PosAy2;
		else if (Index == 2)
			Result = pos.PosAy3;
		
		return Result;
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
				if (PlayerList [i].CheckAnimatorSate(EPlayerState.Push))
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
			if (situation == EGameSituation.AttackA ||
			    situation == EGameSituation.AttackB ||
			    situation == EGameSituation.Opening || 
			    situation == EGameSituation.JumpBall)
				return true;
			else
				return false;
		}
	}

	public bool CandoBtn
	{
		get
		{
			if(situation == EGameSituation.TeeA || situation == EGameSituation.TeeB || situation == EGameSituation.TeeAPicking || situation == EGameSituation.TeeBPicking)
				return false;
			else
				return true;
		}
	}
}
