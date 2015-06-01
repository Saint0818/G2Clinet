using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using DG.Tweening;


public enum GameSituation
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

public enum GameAction
{
    Def = 0,
    Attack = 1
}

public enum BodyType
{
    Small = 0,
    Middle = 1,
    Big = 2
}

public enum SceneTest
{
	Single,
	Multi,
	Release,
	SelectRole
}

public enum GameTest
{
    None,
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
	Shoot
}

public enum CameraTest
{
    None,
    RGB
}

public enum PosKind
{
    None,
    Attack,
    Tee,
    TeeDefence,
    Fast
}

public enum ScoreType {
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

public struct BasketShootPositionData {
	public string AnimationName;
	public float ShootPositionX;
	public float ShootPositionY;
	public float ShootPositionZ;
}

public enum BasketSituation {
	Score = 0,
	Swich = 1,
	NoScore = 2,
	AirBall = 3
}

public enum BasketDistanceAngle{
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

public class GameController : MonoBehaviour
{
    private static GameController instance;
    private static string[] pathName = {"jumpball0",   //0
                                        "jumpball1",   //1
                                        "normal",      //2      
                                        "tee0",        //3
                                        "tee1",        //4
                                        "tee2",        //5
                                        "teedefence0", //6
                                        "teedefence1", //7
                                        "teedefence2", //8
                                        "fast0",       //9
                                        "fast1",       //10
                                        "fast2"};      //11
	
    
    public GameSituation situation = GameSituation.None;
    private List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
    private List<TTactical> MovePositionList = new List<TTactical>();

    private Dictionary<int, int[]> situationPosition = new Dictionary<int, int[]>();
   
    public bool IsStart = false;
	public bool IsReset = false;
    public float CoolDownPass = 0;
    private float CoolDownCrossover = 0;
    private float ShootDis = 0;
    public float RealBallFxTime = 0;
	private float WaitTeeBallTime = 0;
	private float WaitStealTime = 0;
	private float PassingStealBallTime = 0;
	public bool IsPassing = false;
	
	public PlayerBehaviour BallOwner;
	public PlayerBehaviour Joysticker;
    public PlayerBehaviour Catcher;
    public PlayerBehaviour Shooter;
	public PlayerBehaviour Passer;
	private PlayerBehaviour PickBallplayer = null;

    public Vector2[] TeeBackPosAy = new Vector2[3];
	public Vector3[] BornAy = new Vector3[6];
	private GameStruct.TPlayer [] PlayerAy = new TPlayer[6];

	//Score Animation Value
	private float extraScoreRate = 0;
	public string BasketAnimationName;
	public BasketSituation BasketSituationType;
	private BasketDistanceAngle basketDistanceAngle = BasketDistanceAngle.ShortCenter;
	 

	private TActionPosition [] tacticalData;
	private TTactical attackTactical;
	private TTactical defTactical;
	public GameObject selectMe;
	private List<int> TacticalDataList = new List<int>();

	private int shootAngle = 55;

    private int GetPosNameIndex(PosKind Kind, int Index = -1)
    {
        switch (Kind)
        {
            case PosKind.Attack:
                return 2;
            case PosKind.Tee:
                if (Index == 0)
                    return 3;
                else if (Index == 1)
                    return 4;
                else if (Index == 2)
                    return 5;
                else
                    return -1;
            case PosKind.TeeDefence:
                if (Index == 0)
                    return 6;
                else if (Index == 1)
                    return 7;
                else if (Index == 2)
                    return 8;
                else
                    return -1;
            case PosKind.Fast:
                if (Index == 0)
                    return 9;
                else if (Index == 1)
                    return 10;
                else if (Index == 2)
                    return 11;
                else
                    return -1;
            default:
                return -1;
        }
    }

    public static GameController Get
    {
        get
        {
            if (!instance)
            {
                GameObject obj = GameObject.Find("UI2D/UIGame");
                if (!obj)
                {
                    obj = new GameObject();
                    obj.name = "GameController";                
                }

                instance = obj.AddComponent<GameController>();
            }

            return instance;
        }
    }

    public static bool Visible
    {
        get
        {
            if (instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }
    }

    void Start()
    {
        EffectManager.Get.LoadGameEffect();
        InitPos();
        InitGame();
//		SceneMgr.Get.ChangeLevel (SceneName.Court_0);

		if (GameStart.Get.IsSplitScreen)
			CameraMgr.Get.setSplitScreen();
    }

	 

    private void InitPos()
    {
        TeeBackPosAy [0] = new Vector2(0, 8);
        TeeBackPosAy [1] = new Vector2(5.3f, 10);
        TeeBackPosAy [2] = new Vector2(-5.3f, 10);

		BornAy [0] = new Vector3 (0, 0, 0);
		BornAy [1] = new Vector3 (-5, 0, -2);
		BornAy [2] = new Vector3 (5, 0, -2);
		BornAy [3] = new Vector3 (0, 0, 5);
		BornAy [4] = new Vector3 (5, 0, 2);
		BornAy [5] = new Vector3 (-5, 0, 2);

        MovePositionList.Clear();
        for (int i = 0; i < GameData.TacticalData.Length; i++)
            if (!string.IsNullOrEmpty(GameData.TacticalData [i].FileName))
                MovePositionList.Add(GameData.TacticalData [i]);
    }

    public void InitGame()
    {
        PlayerList.Clear();
        CreateTeam();
		SetBallOwnerNull ();
        Shooter = null;
        Catcher = null;
        situation = GameSituation.Opening;
		ChangeSituation (GameSituation.Opening);
    }

	public void StartGame() {
		IsReset = false;
		SetPlayerLevel();
		if (GameStart.Get.TestMode == GameTest.Rebound) {
			CourtMgr.Get.RealBallRigidbody.isKinematic = true;
			CourtMgr.Get.RealBall.transform.position = new Vector3(0, 5, 13);
		}
	}

	private void GetMovePath(int index, ref TTactical Result)
    {
		if (Result.PosAy1 == null)
			Result = new TTactical (false);

		Result.FileName = "";

        if (index >= 0 && index < pathName.Length)
        {
            if (!situationPosition.ContainsKey(index))
            {
				TacticalDataList.Clear();
                for (int i = 0; i < GameData.TacticalData.Length; i++)
                {
                    if (GameData.TacticalData [i].FileName.Contains(pathName [index]))
						TacticalDataList.Add(i);
                }
                
				situationPosition.Add(index, TacticalDataList.ToArray());
            }   

            int len = situationPosition [index].Length;
            if (len > 0)
            {
                int r = Random.Range(0, len);
                int i = situationPosition [index] [r];
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

	private void setPlayerData(int index, ref TTeam team) {
		if (index >= 0 && index < PlayerAy.Length && GameData.DPlayers.ContainsKey(team.Player.ID)) {
			PlayerAy[index].ID = team.Player.ID;
			PlayerAy[index].Name = team.Player.Name;
			PlayerAy[index].SetAttribute();
			PlayerAy[index].SetAvatar();
        }
	}

	public void InitPlayer(){
		for (int i = 0; i < PlayerAy.Length; i++) {
			if(i >= (PlayerAy.Length / 2))
				PlayerAy[i] = new TPlayer(GameConst.NpcAILevel);
			else
				PlayerAy[i] = new TPlayer(GameConst.SelfAILevel);

			if(GameData.DPlayers.ContainsKey(i + 1))
			{
				PlayerAy[i].ID = GameData.DPlayers[i + 1].ID;
				PlayerAy[i].Name = GameData.DPlayers[i + 1].Name;
				PlayerAy[i].SetAttribute();
				PlayerAy[i].SetAvatar();
			}
		}

		setPlayerData(0, ref GameData.Team);
		for (int i = 0; i < GameData.TeamMembers.Length; i++)
			setPlayerData(i+1, ref GameData.TeamMembers[i]);
	}
	
	public void CreateTeam()
    {
        switch (GameStart.Get.TestMode)
        {               
            case GameTest.None:
				InitPlayer ();
				
				for (int i = 0; i < PlayerAy.Length; i++) {
					if(i >= (PlayerAy.Length / 2))
						PlayerList.Add(ModelManager.Get.CreateGamePlayer(i % (PlayerAy.Length / 2), TeamKind.Npc, BornAy[i], PlayerAy[i]));				
					else
						PlayerList.Add(ModelManager.Get.CreateGamePlayer(i, TeamKind.Self, BornAy[i], PlayerAy[i]));					
				}

                for (int i = 0; i < PlayerList.Count; i++)
				{    
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
					PlayerList [i].SetMovePower(100);
				}						
                break;
            case GameTest.AttackA:
            case GameTest.Shoot:
            case GameTest.Dunk:
			case GameTest.Rebound:
                PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, TeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
				PlayerList [0].SetMovePower(100);
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, TeamKind.Npc, new Vector3(0, 0, 11), new GameStruct.TPlayer(0)));
				
                UIGame.Get.ChangeControl(true);
                break;
            case GameTest.AttackB:
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, TeamKind.Npc, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
				break;
            case GameTest.Block:
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Self, new Vector3(0, 0, -8.4f), new GameStruct.TPlayer(0)));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Npc, new Vector3 (0, 0, -4.52f), new GameStruct.TPlayer(0)));
				
				for (int i = 0; i < PlayerList.Count; i++)
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
				break;
			case GameTest.OneByOne: 
				TPlayer Self = new TPlayer(0);
				Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			

				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Self, new Vector3(0, 0, 0), Self));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Npc, new Vector3 (0, 0, 5), new GameStruct.TPlayer(0)));

                for (int i = 0; i < PlayerList.Count; i++)
                    PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
                break;
			case GameTest.Alleyoop:
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, TeamKind.Self, new Vector3 (0, 0, 3), new GameStruct.TPlayer(0)));

				break;
			case GameTest.Pass:
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, TeamKind.Self, new Vector3 (-5, 0, -2), new GameStruct.TPlayer(0)));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (2, TeamKind.Self, new Vector3 (5, 0, -2), new GameStruct.TPlayer(0)));
			for (int i = 0; i < PlayerList.Count; i++)
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
				break;
            case GameTest.Edit:
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, TeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, TeamKind.Self, new Vector3(5, 0, -2), new GameStruct.TPlayer(0)));
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, TeamKind.Self, new Vector3(-5, 0, -2), new GameStruct.TPlayer(0)));
				break;
			case GameTest.CrossOver:
				Self = new TPlayer(0);
				Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			
				
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Self, new Vector3(0, 0, 0), Self));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Npc, new Vector3 (0, 0, 5), new GameStruct.TPlayer(0)));
				
				for (int i = 0; i < PlayerList.Count; i++)
				{
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
					PlayerList [i].SetMovePower(100);
				}
				break;
        }

        Joysticker = PlayerList [0];

		selectMe = EffectManager.Get.PlayEffect("SelectMe", Vector3.zero, null, Joysticker.gameObject);
        Joysticker.AIActiveHint = GameObject.Find("SelectMe/AI");
		Joysticker.SpeedUpView = GameObject.Find("SelectMe/Speedup").GetComponent<UISprite>();
		Joysticker.AngerView = GameObject.Find("SelectMe/Angry").GetComponent<UISprite>();
		Joysticker.AngerView.fillAmount = 0;

        if (PlayerList.Count > 1 && PlayerList [1].Team == Joysticker.Team) 
            EffectManager.Get.PlayEffect("SelectA", Vector3.zero, null, PlayerList [1].gameObject);

        if (PlayerList.Count > 2 && PlayerList [2].Team == Joysticker.Team) 
            EffectManager.Get.PlayEffect("SelectB", Vector3.zero, null, PlayerList [2].gameObject);

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
			PlayerList [i].OnPickUpBall = OnPickUpBall;
			PlayerList [i].OnFall = OnFall;
			PlayerList [i].OnUI = UIGame.Get.OpenUIMask;
        }
    }

	private bool isPressPassBtn = false;

	void FixedUpdate() {

		if (Joysticker) {
			if (Input.GetKeyUp (KeyCode.D))
			{
//				if(Joysticker.IsBallOwner)
				UIGame.Get.DoAttack();
//				else
//					UIGame.Get.DoPush();
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

			if (situation == GameSituation.AttackA) {
				if (Input.GetKeyDown (KeyCode.A))
				{
					isPressPassBtn = true;

					UIGame.Get.DoPassChoose(null, true);
				}

				if (Input.GetKeyUp (KeyCode.A))
				{
					isPressPassBtn = false;
					UIGame.Get.DoPassChoose(null, false);
				}

				if(isPressPassBtn){ 
					if(Input.GetKeyDown (KeyCode.W))
						UIGame.Get.DoPassTeammateA();
					
					if(Input.GetKeyDown (KeyCode.E))
						UIGame.Get.DoPassTeammateB();
				}

				if (Input.GetKeyDown (KeyCode.S))
				{
//					Joysticker.AniState(PlayerState.Shoot1);
					UIGame.Get.DoShoot(null, true);
				}
				
				if (Input.GetKeyUp (KeyCode.S))
				{
					UIGame.Get.DoShoot(null, false);
				}
			}
			else if(situation == GameSituation.AttackB){
				if(Input.GetKeyDown (KeyCode.A)){
					UIGame.Get.DoSteal();
				}

				if(Input.GetKeyDown (KeyCode.S)){
					UIGame.Get.DoBlock();
				}
			}

			if (Input.GetKeyDown (KeyCode.R) && Joysticker != null)
				Joysticker.AniState (PlayerState.Rebound);

			if (Input.GetKeyDown (KeyCode.T) && Joysticker != null)
				Joysticker.AniState (PlayerState.ReboundCatch);

			if (GameStart.Get.TestMode == GameTest.Rebound && Input.GetKeyDown (KeyCode.Z)) {
				resetTestMode();
            }
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
			WaitTeeBallTime = 0;
			if(BallOwner != null)
				AutoTee();
		}

		if(WaitStealTime > 0 && Time.time >= WaitStealTime)		
			WaitStealTime = 0;

		if(PassingStealBallTime > 0 && Time.time >= PassingStealBallTime)		
			PassingStealBallTime = 0;
	}

	public PlayerState testState = PlayerState.Shoot0;
	public PlayerState[] ShootStates = new PlayerState[6]{PlayerState.Shoot0, PlayerState.Shoot1,PlayerState.Shoot2,PlayerState.Shoot3,PlayerState.Shoot6,PlayerState.Layup};

	private void resetTestMode() {
		SetBallOwnerNull();
		SetBall();
		CourtMgr.Get.RealBall.transform.localEulerAngles = Vector3.zero;
		CourtMgr.Get.SetBallState(PlayerState.Shoot0);
		CourtMgr.Get.RealBall.transform.position = new Vector3(0, 5, 13);
		CourtMgr.Get.RealBallRigidbody.isKinematic = true;
		UIGame.Get.ChangeControl(true);
		TMoveData md = new TMoveData(1);
		//md.Target = new Vector2(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);
		PlayerList[1].transform.position = new Vector3(CourtMgr.Get.RealBall.transform.position.x, 0, CourtMgr.Get.RealBall.transform.position.z);
		PlayerList[1].AniState(PlayerState.Idle);
    }
    
    #if UNITY_EDITOR
	void OnGUI() {
		if (GameStart.Get.TestMode == GameTest.Rebound) {
			if (GUI.Button(new Rect(100, 100, 100, 100), "Reset")) {
				resetTestMode();
			}
		}

		if (GameStart.Get.TestMode == GameTest.CrossOver) {
			if (GUI.Button(new Rect(20, 50, 100, 100), "Left")) {
				PlayerList[0].transform.DOMoveX(PlayerList[0].transform.position.x - 1, GameStart.Get.CrossTimeX).SetEase(Ease.Linear);
				PlayerList[0].transform.DOMoveZ(PlayerList[0].transform.position.z + 6, GameStart.Get.CrossTimeZ).SetEase(Ease.Linear);
				PlayerList[0].AniState(PlayerState.MoveDodge0);
			}

			if (GUI.Button(new Rect(120, 50, 100, 100), "Right")) {
				PlayerList[0].transform.DOMoveX(PlayerList[0].transform.position.x + 1, GameStart.Get.CrossTimeX).SetEase(Ease.Linear);
				PlayerList[0].transform.DOMoveZ(PlayerList[0].transform.position.z + 6, GameStart.Get.CrossTimeZ).SetEase(Ease.Linear);
				PlayerList[0].AniState(PlayerState.MoveDodge1);
			}
		}

		if (GameStart.Get.TestMode == GameTest.Shoot) {
			for(int i = 0 ; i < ShootStates.Length; i++){
				if (GUI.Button(new Rect(Screen.width / 2, 50 + i * 50, 100, 50), ShootStates[i].ToString())) {	
					testState = ShootStates[i];
				}
			}
		}
	}
	#endif

    private void SituationAttack(TeamKind team)
    {
        if (PlayerList.Count > 0)
        {
			GetMovePath(GetPosNameIndex(PosKind.Attack), ref attackTactical);
			
			for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerBehaviour Npc = PlayerList [i];
				if (CandoAI(Npc))
				{
					if (Npc.Team == team)
                    {
                        if (!IsPassing)
                        {
                            if (!IsShooting)
                            {
                                Attack(ref Npc);
								AIMove(ref Npc, ref attackTactical);
							} 
							else if (!Npc.IsShoot)
                            {
                                Attack(ref Npc);
								AIMove(ref Npc, ref attackTactical);
							}                               
                        }
                    } else{
                        Defend(ref Npc);
//						DefMove(Npc.DefPlayer);
					}
                }
            }   
        }
    }

    private void SituationPickBall(TeamKind team)
    {
        if (BallOwner == null)
        {
            if (PlayerList.Count > 0)
            {
				if(PickBallplayer == null)
					PickBallplayer = NearBall(team);

				if (PickBallplayer != null)
				{
					GetMovePath(GetPosNameIndex(PosKind.Tee, PickBallplayer.Index), ref attackTactical);
					GetMovePath(GetPosNameIndex(PosKind.TeeDefence, PickBallplayer.Index), ref defTactical);
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
							BackToDef(ref Npc, TeamKind.Npc, ref defTactical);
                    }
                }
            }
        }
    }

    private void SituationTeeBall(TeamKind team)
    {
        if (PlayerList.Count > 0)
        {
            TTactical ap = new TTactical(false);
            TTactical defap = new TTactical(false);
            if (BallOwner != null)
            {
                GetMovePath(GetPosNameIndex(PosKind.Tee, BallOwner.Index), ref ap);
				GetMovePath(GetPosNameIndex(PosKind.TeeDefence, BallOwner.Index), ref defap);
            }

            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerBehaviour Npc = PlayerList [i];
                if (CandoAI(Npc))
                {
                    if (!IsPassing && Npc.Team == team)
                        TeeBall(ref Npc, team, ref ap);
                    else                    
                        BackToDef(ref Npc, Npc.Team, ref defap);//SituationTeeBall
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
    
    public void ChangeSituation(GameSituation GS, PlayerBehaviour GetBall = null)
    {
        if (situation != GameSituation.End)
        {
            GameSituation oldgs = situation;
            if (situation != GS)
            {
                RealBallFxTime = 0;
				WaitStealTime = 0;
                CourtMgr.Get.RealBallFX.SetActive(false);
                for (int i = 0; i < PlayerList.Count; i++)
                {
					if(GS == GameSituation.TeeAPicking || GS == GameSituation.TeeBPicking)
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
					case TeamKind.Self:
						if((GS == GameSituation.TeeB || (oldgs == GameSituation.TeeB && GS == GameSituation.AttackB)) == false){
							if(PlayerList[i].NoAiTime > 0){
								if(!(GS == GameSituation.AttackA || GS == GameSituation.AttackB))
									PlayerList[i].ResetFlag();
							}else
								PlayerList[i].ResetFlag();
						}
						break;
					case TeamKind.Npc:
						if((GS == GameSituation.TeeA || (oldgs == GameSituation.TeeA && GS == GameSituation.AttackA)) == false)
							PlayerList[i].ResetFlag();
						break;
					}

                    PlayerList [i].situation = GS;
                }
            }
            
            situation = GS;
            
            if ((oldgs == GameSituation.TeeA || oldgs == GameSituation.TeeB) && oldgs != GS && GetBall != null)
            {
				GetMovePath(GetPosNameIndex(PosKind.Fast, GetBall.Index), ref attackTactical);
                
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
									if(npc.Team == TeamKind.Self)
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
			case GameSituation.Opening:
				IsStart = true;
				break;
			case GameSituation.JumpBall:
				
				break;
			case GameSituation.AttackA:
				CameraMgr.Get.SetTeamCamera(TeamKind.Self);
				break;
			case GameSituation.AttackB:
				CameraMgr.Get.SetTeamCamera(TeamKind.Npc);
				break;
			case GameSituation.TeeAPicking:
				CourtMgr.Get.Walls[1].SetActive(false);
				UIGame.Get.ChangeControl(true);
				CameraMgr.Get.SetTeamCamera(TeamKind.Npc, true);
				PickBallplayer = null;
                break;
            case GameSituation.TeeA:
				CourtMgr.Get.Walls[1].SetActive(false);
				EffectManager.Get.PlayEffect("ThrowInLineEffect", Vector3.zero, null, null, 0);
                break;
            case GameSituation.TeeBPicking:
				CourtMgr.Get.Walls[0].SetActive(false);
           	 	UIGame.Get.ChangeControl(false);
           		CameraMgr.Get.SetTeamCamera(TeamKind.Self, true);
				PickBallplayer = null;
                break;
			case GameSituation.TeeB:
				CourtMgr.Get.Walls[0].SetActive(false);
				EffectManager.Get.PlayEffect("ThrowInLineEffect", Vector3.zero, null, null, 0);
				break;
			case GameSituation.End:
				IsStart = false;
				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].AniState(PlayerState.Idle);					
            	break;
            }       
        }
    }
    
    private void handleSituation()
    {
        if (PlayerList.Count > 0)
        {
            //Action
            if (GameStart.Get.TestMode != GameTest.None)
                return;
            
            switch (situation)
            {
                case GameSituation.None:
                
                    break;
                case GameSituation.Opening:
                
                    break;
                case GameSituation.JumpBall:
                
                    break;
                case GameSituation.AttackA:
                    SituationAttack(TeamKind.Self);
                    break;
                case GameSituation.AttackB:
                    SituationAttack(TeamKind.Npc);
                    break;
                case GameSituation.TeeAPicking:
                    SituationPickBall(TeamKind.Self);
                    break;
                case GameSituation.TeeA:
                    SituationTeeBall(TeamKind.Self);
                    break;
                case GameSituation.TeeBPicking:
                    SituationPickBall(TeamKind.Npc);
                    break;
                case GameSituation.TeeB:
                    SituationTeeBall(TeamKind.Npc);
                    break;
                case GameSituation.End:
                
                    break;
            }
        }
    }
	
	private void jodgeShootAngle(PlayerBehaviour player){
		//Angle
		float ang = 0;
		float angle = 0;
		int distanceType = 0;
		if(player.name.Contains("Self")) {
			ang = GameFunction.GetPlayerToObjectAngle(CourtMgr.Get.Hood[0].transform, player.gameObject.transform);
			angle = Mathf.Abs(ang) - 90;
		} else {
			ang = GameFunction.GetPlayerToObjectAngle(CourtMgr.Get.Hood[1].transform, player.gameObject.transform);
			angle = Mathf.Abs(ang) - 90;
		}
		//Distance
		if(ShootDis >= 0 && ShootDis < 9) {
			distanceType = 0;
		} else 
		if(ShootDis >= 9 && ShootDis < 12) {
			distanceType = 1;
		} else 
		if(ShootDis >= 12) {
			distanceType = 2;
		}
		//Angle
		if(angle > 60) {// > 60 degree
			if(distanceType == 0){
				basketDistanceAngle = BasketDistanceAngle.ShortCenter;
			}else if (distanceType == 1){
				basketDistanceAngle = BasketDistanceAngle.MediumCenter;
			}else if (distanceType == 2){
				basketDistanceAngle = BasketDistanceAngle.LongCenter;
			}
		} else 
		if(angle <= 60 && angle > 10){// > 10 degree <= 60 degree
			if(ang > 0) {//right
				if(distanceType == 0){
					basketDistanceAngle = BasketDistanceAngle.ShortRight;
				}else if (distanceType == 1){
					basketDistanceAngle = BasketDistanceAngle.MediumRight;
				}else if (distanceType == 2){
					basketDistanceAngle = BasketDistanceAngle.LongRight;
				}
			} else {//left
				if(distanceType == 0){
					basketDistanceAngle = BasketDistanceAngle.ShortLeft;
				}else if (distanceType == 1){
					basketDistanceAngle = BasketDistanceAngle.MediumLeft;
				}else if (distanceType == 2){
					basketDistanceAngle = BasketDistanceAngle.LongLeft;
				}
			}
		} else 
		if(angle <= 10 && angle >= -30){ // < 10 degree
			if(ang > 0) { // right
				if(distanceType == 0){
					basketDistanceAngle = BasketDistanceAngle.ShortRightWing;
				}else if (distanceType == 1){
					basketDistanceAngle = BasketDistanceAngle.MediumRightWing;
				}else if (distanceType == 2){
					basketDistanceAngle = BasketDistanceAngle.LongRightWing;
				}
			} else { //left
				if(distanceType == 0){
					basketDistanceAngle = BasketDistanceAngle.ShortLeftWing;
				}else if (distanceType == 1){
					basketDistanceAngle = BasketDistanceAngle.MediumLeftWing;
				}else if (distanceType == 2){
					basketDistanceAngle = BasketDistanceAngle.LongLeftWing;
				}
			}
		}
	}

	private void judgeBasketAnimationName (int basketDistanceAngleType) {
		if(BasketSituationType == BasketSituation.Score){
			if(CourtMgr.Get.BasketAnimationName.Count > 0 && basketDistanceAngleType < CourtMgr.Get.BasketAnimationName.Count){
				int random = Random.Range(0, CourtMgr.Get.BasketAnimationName[basketDistanceAngleType].Count);
				if(CourtMgr.Get.BasketAnimationName.Count > 0 && random < CourtMgr.Get.BasketAnimationName.Count)
					BasketAnimationName = CourtMgr.Get.BasketAnimationName[basketDistanceAngleType][random];
			}
		}else if(BasketSituationType == BasketSituation.NoScore){
			if(CourtMgr.Get.BasketAnimationNoneState.Count > 0 && basketDistanceAngleType < CourtMgr.Get.BasketAnimationName.Count) {
				int random = Random.Range(0, CourtMgr.Get.BasketAnimationNoneState[basketDistanceAngleType].Count);
				if(CourtMgr.Get.BasketAnimationNoneState.Count > 0 && random < CourtMgr.Get.BasketAnimationName.Count)
					BasketAnimationName = CourtMgr.Get.BasketAnimationNoneState[basketDistanceAngleType][random];
			}
		}
	}

	private void calculationScoreRate(PlayerBehaviour player, ScoreType type) {
		jodgeShootAngle(player);
		//Score Rate
		float originalRate = 0;
		if(ShootDis >= GameConst.TreePointDistance) {
			originalRate = player.Attr.PointRate3 * player.ScoreRate.ThreeScoreRateDeviation;
			EffectManager.Get.PlayEffect("ThreeLineEffect", Vector3.zero, null, null, 0);
		} else {
			originalRate = player.Attr.PointRate2 * player.ScoreRate.TwoScoreRateDeviation;
		}
		float rate = (Random.Range(0, 100) + 1);
		int airRate = (Random.Range(0, 100) + 1);
		bool isScore = false;
		bool isSwich = false;
		bool isAirBall = false;
		if(type == ScoreType.DownHand) {
			isScore = rate <= (originalRate - (originalRate * (player.ScoreRate.DownHandScoreRate / 100f)) + extraScoreRate) ? true : false;
			if(isScore) {
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.DownHandSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.DownHandAirBallRate ? true : false;
			}
		} else 
		if(type == ScoreType.UpHand) {
			isScore = rate <= (originalRate - (originalRate * (player.ScoreRate.UpHandScoreRate / 100f)) + extraScoreRate) ? true : false;
			if(isScore) {
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.UpHandSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.UpHandAirBallRate ? true : false;
			}
		} else 
		if(type == ScoreType.Normal) {
			isScore = rate <= (originalRate - (originalRate * (player.ScoreRate.NormalScoreRate / 100f)) + extraScoreRate) ? true : false;
			if(isScore) {
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.NormalSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.NormalAirBallRate ? true : false;
			}
		} else 
		if(type == ScoreType.NearShot) {
			isScore = rate <= (originalRate - (originalRate * (player.ScoreRate.NearShotScoreRate / 100f)) + extraScoreRate) ? true : false;
			if(isScore) {
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.NearShotSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.NearShotAirBallRate ? true : false;
			}
		} else 
		if(type == ScoreType.LayUp) {
			isScore = rate <= (originalRate - (originalRate * (player.ScoreRate.LayUpScoreRate / 100f)) + extraScoreRate) ? true : false;
			if(isScore) {
				isSwich = rate <= (originalRate - (originalRate * (player.ScoreRate.LayUpSwishRate / 100f))) ? true : false;
			} else {
				isAirBall = airRate <= player.ScoreRate.LayUpAirBallRate ? true : false;
			}
		}
			
		if(extraScoreRate == GameData.ExtraPerfectRate)
			isAirBall = false;
		
		if(isScore && isSwich) 
			BasketSituationType = BasketSituation.Swich;
		else if(isScore && !isSwich)
			BasketSituationType = BasketSituation.Score;
		else if(!isScore && isAirBall)
			BasketSituationType = BasketSituation.AirBall;
		else if(!isScore && !isAirBall)
			BasketSituationType = BasketSituation.NoScore;
	}

	public void AddExtraScoreRate(float rate) {
		extraScoreRate = rate;
	}

	public void Shoot()
    {
        if (BallOwner)
        {
			if(GameStart.Get.TestMode == GameTest.Shoot){
				BallOwner.AniState(testState, CourtMgr.Get.Hood [BallOwner.Team.GetHashCode()].transform.position);
			}
			else{
				extraScoreRate = 0;
				UIGame.Get.DoPassNone();
				CourtMgr.Get.ResetBasketEntra();
				Vector3 v = CourtMgr.Get.ShootPoint [BallOwner.Team.GetHashCode()].transform.position;
				ShootDis = getDis(ref BallOwner, new Vector2(v.x, v.z));
				int t = BallOwner.Team.GetHashCode();
	            if (GameStart.Get.TestMode == GameTest.Dunk)
	                BallOwner.AniState(PlayerState.Dunk, CourtMgr.Get.ShootPoint [t].transform.position);
	            else 
				if (BallOwner.IsRebound) {
					if (inTipinDistance(BallOwner))
						BallOwner.AniState(PlayerState.TipIn, CourtMgr.Get.ShootPoint [t].transform.position);
				} else
				if (Vector3.Distance(BallOwner.gameObject.transform.position, CourtMgr.Get.ShootPoint [t].transform.position) <= GameConst.DunkDistance)
				{
					float rate = Random.Range(0, 100);
					if(rate < BallOwner.Attr.DunkRate)
	                	BallOwner.AniState(PlayerState.Dunk, CourtMgr.Get.ShootPoint [t].transform.position);
					else
						BallOwner.AniState(PlayerState.Layup, CourtMgr.Get.Hood [t].transform.position);
				}
	            else {
					float dis = Vector3.Distance(BallOwner.gameObject.transform.position, CourtMgr.Get.ShootPoint[BallOwner.Team.GetHashCode()].transform.position);

					if(BallOwner.IsMoving){
						if(dis > 15)
							BallOwner.AniState(PlayerState.Shoot3, CourtMgr.Get.Hood [t].transform.position);
						else if(dis > 9 && dis <= 15)
							BallOwner.AniState(PlayerState.Shoot2, CourtMgr.Get.Hood [t].transform.position);
						else
							BallOwner.AniState(PlayerState.Layup, CourtMgr.Get.Hood [t].transform.position);
					}
					else{
						if(dis > 15)
							BallOwner.AniState(PlayerState.Shoot3, CourtMgr.Get.Hood [t].transform.position);
						else if(dis > 9 && dis <= 15)
							BallOwner.AniState(PlayerState.Shoot0, CourtMgr.Get.Hood [t].transform.position);
						else
							BallOwner.AniState(PlayerState.Shoot6, CourtMgr.Get.Hood [t].transform.position);
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
			UIGame.Get.SetPassButton(0);
			for(int i = 0; i < PlayerList.Count; i++)
				if(PlayerList[i] != Shooter)
					PlayerList[i].ResetMove();
			shootAngle = 55;
			if(player.crtState == PlayerState.Shoot0){
				calculationScoreRate(player, ScoreType.Normal);
			} else 
			if(player.crtState == PlayerState.Shoot1 || player.crtState == PlayerState.Shoot6) {
				calculationScoreRate(player, ScoreType.NearShot);
			} else 
			if(player.crtState == PlayerState.Shoot2) {
				calculationScoreRate(player, ScoreType.UpHand);
			} else 
			if(player.crtState == PlayerState.Shoot3) {
				calculationScoreRate(player, ScoreType.DownHand);
			} else 
			if(player.crtState == PlayerState.Layup|| player.crtState == PlayerState.TipIn){
				calculationScoreRate(player, ScoreType.LayUp);
				shootAngle = 75;
			}

			judgeBasketAnimationName ((int)basketDistanceAngle);

			SetBall();
            CourtMgr.Get.RealBall.transform.localEulerAngles = Vector3.zero;
			CourtMgr.Get.SetBallState(player.crtState);

			if(BasketSituationType == BasketSituation.AirBall) {
				//AirBall
				Vector3 ori = CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position - CourtMgr.Get.RealBall.transform.position;
				CourtMgr.Get.RealBallRigidbody.velocity = 
					GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
					                         CourtMgr.Get.RealBall.transform.position + (ori * 0.87f), shootAngle);
			} else 
			if(BasketSituationType == BasketSituation.Swich) {
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
            //DefBlock(ref Shooter);
            return true;
        } else
            return false;
    }

	public void DoShoot(bool isshoot)
    {
		if (IsStart && CandoBtn) {
			if (UIDoubleClick.Visible)
				UIDoubleClick.Get.ClickStop ();
			else
            if (Joysticker == BallOwner) {
				if (isshoot)
					Shoot ();
				else
					Joysticker.AniState(PlayerState.FakeShoot, CourtMgr.Get.ShootPoint [Joysticker.Team.GetHashCode()].transform.position);
            } else 
			if (BallOwner && BallOwner.Team == TeamKind.Self) 
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
				Joysticker.rotateTo(nearP.gameObject.transform.position.x, nearP.gameObject.transform.position.z);
			Joysticker.AniState (PlayerState.Push);
		}
	}

	public void DoElbow()
	{
		if (Joysticker)
			Joysticker.AniState (PlayerState.Elbow);
	}

    public bool OnDunkBasket(PlayerBehaviour player)
    {
        if (player == BallOwner)
        {
            CourtMgr.Get.SetBallState(PlayerState.DunkBasket);
            PlusScore(player.Team.GetHashCode());
            SetBall();
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
            ShootDis = getDis(ref Shooter, new Vector2(v.x, v.z));
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
				if(player.Team == TeamKind.Self)
				{
					if(!IsBtn)
						CanPass = false;
					else if(!IsCanPassAir)
						CanPass = false;
				}
				else if(player.Team == TeamKind.Npc && !IsCanPassAir)
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
					if(BallOwner.AniState(PlayerState.Tee, player.transform.position))
					{
						Catcher = player;
						Result = true;
					}												
				}else if(IsCanPassAir && !IsTee)
				{
					if(BallOwner.AniState(PlayerState.PassAir, player.transform.position))
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
					int passkind = -1;
					
					if(dis <= GameConst.FastPassDistance || player.crtState == PlayerState.Alleyoop)
					{
						Result = BallOwner.AniState(PlayerState.PassFast, player.transform.position);
					}
					else if(dis <= GameConst.CloseDistance)
					{
						//Close
						if(disKind == 1)
						{
							if(rate == 1){
								Result = BallOwner.AniState(PlayerState.PassParabola, player.transform.position);
								passkind = 1;
							}else{ 
								Result = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
								passkind = 2;
							}
						} else 
							if(disKind == 2)
						{
							if(rate == 1){
								Result = BallOwner.AniState(PlayerState.PassFlat, player.transform.position);
								passkind = 0;
							}else{
								Result = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
								passkind = 2;
							}
						}						
						else
						{
							if(rate == 1){
								Result = BallOwner.AniState(PlayerState.PassFlat, player.transform.position);
								passkind = 0;
							}else{
								Result = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
								passkind = 2;
							}
						}
					}else if(dis <= GameConst.MiddleDistance)
					{
						//Middle
						if(disKind == 1)
						{
							if(rate == 1){
								Result = BallOwner.AniState(PlayerState.PassFlat, player.transform.position);
								passkind = 0;
							}else{
								Result = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
								passkind = 2;
							}
						} else 
							if(disKind == 2)
						{
							if(rate == 1){
								Result = BallOwner.AniState(PlayerState.PassParabola, player.transform.position);
								passkind = 1;
							}else{
								Result = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
								passkind = 2;
							}
						}						
						else
						{
							if(rate == 1){
								Result = BallOwner.AniState(PlayerState.PassFlat, player.transform.position);
								passkind = 0;
							}else{
								Result = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
								passkind = 2;
							}
						}
					}else{
						//Far
						Result = BallOwner.AniState(PlayerState.PassParabola, player.transform.position);
						passkind = 1;
					}
					
					if(Result){
						Catcher = player;
						
						//					float adis = Vector3.Distance(BallOwner.transform.position, Catcher.transform.position);
						//					if(adis <= 1){
						//						if(passkind == 0)
						//							Catcher.AniState(PlayerState.CatchFlat, BallOwner.transform.position);
						//						else if(passkind == 1)
						//							Catcher.AniState(PlayerState.CatchParabola, BallOwner.transform.position);
						//						else if(passkind == 2)
						//							Catcher.AniState(PlayerState.CatchFloor, BallOwner.transform.position);
						//					}
						
						UIGame.Get.DoPassNone();
					}
				}
			}
		}

		return Result;
    }

	public bool OnFall(PlayerBehaviour faller)
	{
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
//            SceneMgr.Get.SetBallState(PlayerState.PassFlat);
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

                if (BallOwner == Joysticker)
                    return Pass(PlayerList [playerid], false, true);
                else
					return Pass(Joysticker, false, true);

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
				
				if (stealRate <= (r + AddRate) && Mathf.Abs(GetAngle(BallOwner, player)) <= 90 + AddAngle)
				{
					if(BallOwner && BallOwner.AniState(PlayerState.GotSteal))
					{
						BallOwner.SetAnger(GameConst.DelAnger_Stealed);
						return true;
					}
				}else if(BallOwner != null && HaveStealPlayer(ref player, ref BallOwner, GameConst.StealBallDistance, 15) != 0)
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
		}
		else
			return false;
	}

	public float StealBtnLiftTime = 1f;

    public void DoSteal()
    {
		if (StealBtnLiftTime <= 0 && IsStart && Joysticker && CandoBtn)
        {
			StealBtnLiftTime = 1f;
            if (BallOwner && BallOwner.Team != Joysticker.Team)
            {
				Joysticker.rotateTo(BallOwner.gameObject.transform.position.x, BallOwner.gameObject.transform.position.z);
                Joysticker.AniState(PlayerState.Steal, BallOwner.transform.position);
            } else
                Joysticker.AniState(PlayerState.Steal);
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

	public bool OnDoubleClickMoment(PlayerBehaviour player, PlayerState state)
	{
		if (player.Team == TeamKind.Self && !UIDoubleClick.Visible) {
			switch (state) {
				case PlayerState.Shoot0:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData( 1.3f, DoubleShoot);
					return true;
				case PlayerState.Shoot1:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData( 1.23f, DoubleShoot);
					return true;
				case PlayerState.Shoot2:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData( 1.3f, DoubleShoot);
					return true;
				case PlayerState.Shoot3:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData( 1.3f, DoubleShoot);
					return true;
				case PlayerState.Shoot6:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData( 1.3f, DoubleShoot);
					return true;
				case PlayerState.Layup:
					UIDoubleClick.Get.SetData(1.3f, DoubleShoot);
					UIDoubleClick.UIShow(true);
					return true;

				case PlayerState.Block:
				case PlayerState.BlockCatch:
					UIDoubleClick.UIShow(true);
					UIDoubleClick.Get.SetData(0.7f, null, DoubleBlock, player);
					return true;
				case PlayerState.Rebound:
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
				break;
		}

	}

	public void DoubleBlock(int lv, PlayerBehaviour player){
		switch (lv) {
		case 0: 
			break;
		case 1: 
			if(Shooter)
				CourtMgr.Get.SetBallState(PlayerState.Block, Shooter);
			else
				CourtMgr.Get.SetBallState(PlayerState.Block, player);
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
//                    SceneMgr.Get.SetBallState(PlayerState.Block);
//                    return true;
//                }
//            } else 
//            if (BallOwner.CheckAnimatorSate(PlayerState.Shooting))
//            {
//                float dis = Vector3.Distance(player.transform.position, BallOwner.transform.position);
//                if (dis <= 4)
//                {
//                    for (int i = 0; i < PlayerList.Count; i++)
//                        if (PlayerList [i].Team == BallOwner.Team)
//                            PlayerList [i].ResetMove();
//
//                    SetBall(null);
//                    SceneMgr.Get.SetBallState(PlayerState.Block);
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
			if(Joysticker.crtState == PlayerState.Block && Joysticker.IsPerfectBlockCatch){
				Joysticker.AniState(PlayerState.BlockCatch);
				if(UIDoubleClick.Visible)
					UIDoubleClick.Get.ClickStop();
			}else{
				if (Shooter)
	                Joysticker.AniState(PlayerState.Block, Shooter.transform.position);
	            else
	            if (BallOwner) {
					Joysticker.rotateTo(BallOwner.gameObject.transform.position.x, BallOwner.gameObject.transform.position.z); 
	                Joysticker.AniState(PlayerState.Block, BallOwner.transform.position);
				} else {
				if (!Shooter && inReboundDistance(Joysticker) && GameStart.Get.TestMode == GameTest.None)
					Rebound(Joysticker);
				else
                	Joysticker.AniState(PlayerState.Block);
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
			player.AniState(PlayerState.Rebound, CourtMgr.Get.RealBall.transform.position);
		//}
	}
	
	public bool OnRebound(PlayerBehaviour player)
    {
        return true;
    }
    
    public void DoSkill()
    {
		if(CandoBtn)
        	Joysticker.SetNoAiTime();
    }

    private bool CanMove
    {
        get
        {
            if (situation == GameSituation.AttackA ||
                situation == GameSituation.AttackB ||
                situation == GameSituation.Opening)
                return true;
            else
                return false;
        }
    }

    public void OnJoystickMove(MovingJoystick move)
    {
		if (Joysticker && (CanMove || Joysticker.CanMoveFirstDribble))
        {
            if (Mathf.Abs(move.joystickAxis.y) > 0 || Mathf.Abs(move.joystickAxis.x) > 0)
            {
                Joysticker.ClearMoveQueue();
				PlayerState ps = Joysticker.crtState;

				if(!Joysticker.IsFall)
				{
					if(BallOwner == Joysticker)
						ps = PlayerState.RunAndDribble;
					else
						ps = PlayerState.Run;
				}
                    
                Joysticker.OnJoystickMove(move, ps);
            }
        }
    }
    
    public void OnJoystickMoveEnd(MovingJoystick move)
    {
        if (Joysticker)
        {
			PlayerState ps;

			if (BallOwner == Joysticker)
			{
				if(Joysticker.crtState == PlayerState.Elbow)
					ps = PlayerState.Elbow;
				else if(Joysticker.crtState == PlayerState.HoldBall)
					ps = PlayerState.HoldBall;
				else
					ps = PlayerState.Dribble;
			}
			else
				ps = PlayerState.Idle;
            
            Joysticker.OnJoystickMoveEnd(move, ps);
        }
    }

	private void AIShoot(ref PlayerBehaviour Self)
	{
		bool suc = false;

		if (Self.IsRebound)
			suc = true;
		else
		if(!Self.CheckAnimatorSate(PlayerState.HoldBall) && HaveDefPlayer(ref Self, 5, 40) != 0) {
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
							Self.AniState(PlayerState.FakeShoot, CourtMgr.Get.ShootPoint [Self.Team.GetHashCode()].transform.position);
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
			int dunkRate = Random.Range(0, 100) + 1;
			int shootRate = Random.Range(0, 100) + 1;
			int passRate = Random.Range(0, 100) + 1;
			int pushRate = Random.Range(0, 100) + 1;
			int ElbowRate = Random.Range(0, 100) + 1;
			float ShootPointDis = 0;
            Vector3 pos = CourtMgr.Get.ShootPoint [Npc.Team.GetHashCode()].transform.position;
			PlayerBehaviour man = null;
            
            if (Npc.Team == TeamKind.Self)
                ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));
            else
                ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));
            
            if (Npc == BallOwner)
            {
                //Dunk shoot shoot3 pass
                int Dir = HaveDefPlayer(ref Npc, 1.5f, 50);
				if (ShootPointDis <= GameConst.DunkDistance && (dunkRate < 30 || Npc.CheckAnimatorSate(PlayerState.HoldBall)) && CheckAttack(ref Npc))
                {
					AIShoot(ref Npc);
                } else 
				if (ShootPointDis <= GameConst.TwoPointDistance && (HaveDefPlayer(ref Npc, 1.5f, 40) == 0 || shootRate < 10 || Npc.CheckAnimatorSate(PlayerState.HoldBall)) && CheckAttack(ref Npc))
                {
					AIShoot(ref Npc);
				} else 
				if (ShootPointDis <= GameConst.TreePointDistance && (HaveDefPlayer(ref Npc, 10, 90) == 0) && CheckAttack(ref Npc))//|| shoot3Rate < 3
                {
					AIShoot(ref Npc);				
				} else 
				if (ElbowRate < Npc.Attr.ElbowingRate && CheckAttack(ref Npc) && (HaveDefPlayer(ref Npc, GameConst.StealBallDistance, 90, out man) != 0) && 
					Npc.CoolDownElbow ==0 && !Npc.CheckAnimatorSate(PlayerState.Elbow))
				{
					if(Npc.AniState(PlayerState.Elbow, man.transform.position)){
						CoolDownPass = 0;
						Npc.CoolDownElbow = Time.time + 3;
					}
				}else 
				if ((passRate < 20 || Npc.CheckAnimatorSate(PlayerState.HoldBall)) && CoolDownPass == 0 && !IsShooting && !IsDunk && !Npc.CheckAnimatorSate(PlayerState.Elbow) && BallOwner.NoAiTime == 0)
                {
                    PlayerBehaviour partner = HavePartner(ref Npc, 20, 90);

                    if (partner != null && HaveDefPlayer(ref partner, 1.5f, 40) == 0)
                    {
                        Pass(partner);
                    } else
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
                } else 
                if (Npc.Player.AILevel >= 3 && Dir != 0 && CoolDownCrossover == 0 && Npc.CanMove)
                {
                    //Crossover     
					if(Npc.Team == TeamKind.Self && Npc.transform.position.z >= 9.5)
						return;
					else if(Npc.Team == TeamKind.Npc && Npc.transform.position.z <= -9.5)
						return;

					int AddZ = 6;
					if(Npc.Team == TeamKind.Npc)
						AddZ = -6;

					Npc.rotateTo(pos.x, pos.z);
					Npc.transform.DOMoveZ(Npc.transform.position.z + AddZ, GameStart.Get.CrossTimeZ).SetEase(Ease.Linear);
					if (Dir == 1)
					{
						Npc.transform.DOMoveX(Npc.transform.position.x - 1, GameStart.Get.CrossTimeX).SetEase(Ease.Linear);
						Npc.AniState(PlayerState.MoveDodge0);
					}
					else
					{
						Npc.transform.DOMoveX(Npc.transform.position.x + 1, GameStart.Get.CrossTimeX).SetEase(Ease.Linear);
						Npc.AniState(PlayerState.MoveDodge1);
					}
					CoolDownCrossover = Time.time + 4;
				}
			} 
			else
			{
				//sup push 
				PlayerBehaviour NearPlayer = HaveNearPlayer(Npc, GameConst.StealBallDistance, false);

				if (NearPlayer != null && pushRate < Npc.Attr.PushingRate && Npc.CoolDownPush == 0)
                {
                    //Push
					if(Npc.AniState(PlayerState.Push, NearPlayer.transform.position))
						Npc.CoolDownPush = Time.time + 3;                    
                } 
            }   
        }
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
			int pushRate = Random.Range(0, 100) + 1;        
			TPlayerDisData [] DisAy = new TPlayerDisData[PlayerList.Count / 2];
			bool sucess = false;

            //steal push Def
			if (!IsShooting && Npc.NoAiTime == 0)
			{
				if (BallOwner != null)
                {
					DisAy[0].Distance = getDis(ref BallOwner, ref Npc);
					DisAy[0].Player = BallOwner;

					for(int i = 0 ; i < PlayerList.Count; i++)
					{
						if(PlayerList[i].Team != Npc.Team && PlayerList[i] != BallOwner)
						{
							PlayerBehaviour anpc = PlayerList[i];
							if(DisAy[1].Distance == 0)
							{
								DisAy[1].Distance = getDis(ref anpc, ref Npc);
								DisAy[1].Player = anpc;
							}

							if(DisAy[2].Distance == 0)
							{
								DisAy[2].Distance = getDis(ref anpc, ref Npc);
								DisAy[2].Player = anpc;
							}
						}
					}
                    
					if (!Npc.CheckAnimatorSate(PlayerState.Steal) && !Npc.CheckAnimatorSate(PlayerState.Push) && !IsDunk && !IsShooting)
                    {
						for(int i = 0; i < DisAy.Length; i++){
							if (DisAy[i].Distance <= GameConst.StealBallDistance && (DisAy[i].Player.crtState == PlayerState.Idle && DisAy[i].Player.crtState == PlayerState.Dribble) && pushRate <= Npc.Attr.PushingRate && Npc.CoolDownPush == 0 && !IsPush)
							{
								if(Npc.AniState (PlayerState.Push, DisAy[i].Player.transform.position)){
									Npc.CoolDownPush = Time.time + 3;
									sucess = true;
								}
								break;
							} 
						}

						if (!sucess && DisAy[0].Distance <= GameConst.StealBallDistance && WaitStealTime == 0 && BallOwner.Invincible == 0 && Npc.CoolDownSteal == 0)
                        {
							if(Random.Range(0, 100) < Npc.Attr.StealRate)
                            {
                                if (Npc.AniState(PlayerState.Steal, BallOwner.gameObject.transform.position)){
                                	Npc.CoolDownSteal = Time.time + 2;                              
									WaitStealTime = Time.time + 2;
								}
                            }
                        }
                    }
                }
            }               
        }
    }

	private void BackToDef(ref PlayerBehaviour Npc, TeamKind Team, ref TTactical pos, bool WatchBallOwner = false)
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
						if (Team == TeamKind.Self)
							data.Target = new Vector2(tacticalData [i].x, -tacticalData [i].z);
						else
							data.Target = new Vector2(tacticalData [i].x, tacticalData [i].z);
						
						if (BallOwner != null)
							data.LookTarget = BallOwner.transform;
						else
						{
							if (Team == TeamKind.Self)
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
	
	private void TeeBall(ref PlayerBehaviour Npc, TeamKind Team, ref TTactical pos)
	{
		TMoveData data = new TMoveData(0);

		if(Npc == BallOwner && Npc.TargetPosNum > 1)
		{
			if (Npc == BallOwner)
				if(!(Npc.MoveQueue.Peek().Target.y == 18 || Npc.MoveQueue.Peek().Target.y == -18))
					Npc.ResetMove();
		}
			

		if (Npc.CanMove && !Npc.IsMoving && Npc.WaitMoveTime == 0 && Npc.TargetPosNum == 0)
		{
			if (Npc == BallOwner)
			{
				int TargetZ = 18;
				if(Team == TeamKind.Self)
					TargetZ = -18;

				if(Npc.Team == TeamKind.Self && Npc.transform.position.z <= -17 && Npc.transform.position.z >= -18){
					if(WaitTeeBallTime == 0)
						WaitTeeBallTime = Time.time + 1;
					return;
				}
				else if(Npc.Team == TeamKind.Npc && Npc.transform.position.z >= 17 && Npc.transform.position.z <= 18){
					if(WaitTeeBallTime == 0)
						WaitTeeBallTime = Time.time + 1;
					return;
				}

				data.FileName = pos.FileName;
				data.Target = new Vector2(Npc.transform.position.x, TargetZ);
                data.MoveFinish = NpcAutoTee;
                Npc.TargetPos = data;
            } else 
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
                        if (Team == TeamKind.Self) 
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
            Npc.AniState(PlayerState.Dribble);
    }
	
    private bool NpcAutoTee(PlayerBehaviour player, bool speedup)
    {
		if(WaitTeeBallTime == 0)
		{
			WaitTeeBallTime = Time.time + 1;
//			player.AniState (PlayerState.Dribble);
		}

		return true;
    }

	public void AutoTee()
	{
		PlayerBehaviour getball = null;

		if (BallOwner.Team == TeamKind.Self)
			getball = Joysticker;
		else
			getball = HaveNearPlayer(BallOwner, 10, true);

		
		if (getball != null)
		{
			Pass(getball, true);
		} else
		{
			int ran = UnityEngine.Random.Range(0, 2);
			int count = 0;
			for (int i = 0; i < PlayerList.Count; i++)
			{
				if (PlayerList [i].Team == BallOwner.Team && PlayerList [i] != BallOwner)
				{
					if (count == ran)
					{
						Pass(PlayerList [i], true);
						break;
					}
					
					count++;
				}
			}
		}
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

    private PlayerBehaviour NearBall(TeamKind team)
    {
        PlayerBehaviour NearPlayer = null;
        
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerBehaviour Npc = PlayerList [i];
            if (Npc.Team == team)
            {
				if(team == TeamKind.Self && Npc == Joysticker)
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

	public float GetAngle(PlayerBehaviour Self, PlayerBehaviour Enemy)
	{
		Vector3 relative = Self.transform.InverseTransformPoint(Enemy.transform.position);
		return Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
	}

	public float GetAngle(PlayerBehaviour Self, Vector3 Enemy)
	{
		Vector3 relative = Self.transform.InverseTransformPoint(Enemy);
		return Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
	}

	private TPlayerDisData [] GetPlayerDisAy(PlayerBehaviour Self, bool SameTeam = false)
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
        if (PlayerList.Count > 0 && !IsPassing)
        {
			bool Suc = false;
			PlayerBehaviour Npc2;
			int Rate = Random.Range(0, 100);
			TPlayerDisData [] DisAy = GetPlayerDisAy(Npc);

			if(DisAy != null)
			{
				for (int i = 0; i < DisAy.Length; i++)
				{
					Npc2 = DisAy [i].Player;
					if (Npc2 && Npc2 != Npc && Npc2.Team != Npc.Team && Npc2.NoAiTime == 0 && 
					    !Npc2.CheckAnimatorSate(PlayerState.Steal) && 
					    !Npc2.CheckAnimatorSate(PlayerState.Push))
					{
						if(!IsBlocking)
						{
							int BlockRate = Npc2.Attr.BlockRate;
							
							if(Kind == 1)
								BlockRate = Npc2.Attr.FaketBlockRate;	
							
							float mAngle = GetAngle(Npc, PlayerList [i]);
							
							if (getDis(ref Npc, ref Npc2) <= GameConst.BlockDistance && Mathf.Abs(mAngle) <= 50)
							{
								if (Rate < BlockRate)
								{
									if(Npc2.AniState(PlayerState.Block, Npc.transform.position))
									{
										Suc = true;
										break;
									}
								}
							}
						}
					}
				}
			}

			if(!Suc)
			{
				for (int i = 0; i < DisAy.Length; i++)
				{               
					Npc2 = DisAy [i].Player;
					
					if (Npc2 && Npc2 != Npc && Npc2.Team != Npc.Team && Npc2.NoAiTime == 0 && 
					    !Npc2.CheckAnimatorSate(PlayerState.Steal) && 
					    !Npc2.CheckAnimatorSate(PlayerState.Push))
					{
						if(!IsBlocking)
						{
							int BlockRate = Npc2.Attr.BlockRate;
							
							if(Kind == 1)
								BlockRate = Npc2.Attr.FaketBlockRate;	
							
							if (GameStart.Get.TestMode == GameTest.Block)
							{
								Npc2.AniState(PlayerState.Block, Npc.transform.position);
							} 
							else if (getDis(ref Npc, ref Npc2) <= GameConst.BlockDistance)
							{
								if (Rate < BlockRate)
								{
									Npc2.AniState(PlayerState.Block, Npc.transform.position);
									break;
								}
							}
						}else
							break;					
					}
				}
			}
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
				if(Npc.crtState != PlayerState.Block && Npc.NoAiTime == 0)
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
					if((situation == GameSituation.AttackA && npc.Team == TeamKind.Self) || (situation == GameSituation.AttackB && npc.Team == TeamKind.Npc))
						PickBall(ref npc, true);

					if((situation == GameSituation.AttackA && npc.DefPlayer.Team == TeamKind.Npc) || (situation == GameSituation.AttackB && npc.DefPlayer.Team == TeamKind.Self))
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
			bool suc = false;

			if(npc == BallOwner && HaveDefPlayer(ref npc, 45, 90) == 0 && npc.FirstTargetPosNum == 0 && !npc.IsCatch)
			{
				if(npc.Team == TeamKind.Self && npc.transform.localRotation.y <= 90 && npc.transform.localRotation.y >= -90)
				{
					suc = true;
					data = new TMoveData(0);
					data.Target = new Vector2(CourtMgr.Get.Hood[npc.Team.GetHashCode()].transform.position.x, CourtMgr.Get.Hood[npc.Team.GetHashCode()].transform.position.z);				
					data.MoveFinish = DefMove;
					npc.FirstTargetPos = data;
					DefMove(npc);

				}else if(npc.Team == TeamKind.Npc && npc.transform.localRotation.y > 90 ||npc.transform.localRotation.y < -90)
				{
					suc = true;
					data = new TMoveData(0);
					data.Target = new Vector2(CourtMgr.Get.Hood[npc.Team.GetHashCode()].transform.position.x, CourtMgr.Get.Hood[npc.Team.GetHashCode()].transform.position.z);				
					data.MoveFinish = DefMove;
					npc.FirstTargetPos = data;
					DefMove(npc);
				}

			}

			if (!suc && npc.CanMove && npc.TargetPosNum == 0)
	        {
				for(int i = 0; i < PlayerList.Count; i++)
				{
					if(PlayerList[i].Team == npc.Team && PlayerList[i] != npc && pos.FileName != string.Empty && PlayerList[i].TargetPosName != pos.FileName)
					{
						PlayerList[i].ResetMove();
					}
				}

	            if (!CheckAttack(ref npc))
	            {
	                data = new TMoveData(0);
	                if (npc.Team == TeamKind.Self)
	                    data.Target = new Vector2(npc.transform.position.x, 14);
	                else
	                    data.Target = new Vector2(npc.transform.position.x, -14);
	                
	                if (BallOwner != null && BallOwner != npc)
	                    data.LookTarget = BallOwner.transform;  
	                
	                data.MoveFinish = DefMove;
	                npc.FirstTargetPos = data;
	                DefMove(npc);
	            } else
				if(pos.FileName != string.Empty)
	            {
					GetActionPosition(npc.Index, ref pos, ref tacticalData);

					if (tacticalData != null)
	                {
						for (int i = 0; i < tacticalData.Length; i++)
	                    {
	                        data = new TMoveData(0);
							data.Speedup = tacticalData [i].Speedup;
							data.Catcher = tacticalData [i].Catcher;
							data.Shooting = tacticalData [i].Shooting;
	                        int z = 1;
	                        if (npc.Team != TeamKind.Self)
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
	            npc.AniState(PlayerState.Dribble);
        }
    }

    public bool DefMove(PlayerBehaviour player, bool speedup = false)
    {
		if (player && player.DefPlayer && !player.CheckAnimatorSate(PlayerState.MoveDodge1) && !player.CheckAnimatorSate(PlayerState.MoveDodge0) && (situation == GameSituation.AttackA || situation == GameSituation.AttackB))
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
                        if (player.DefPlayer.Team == TeamKind.Self)
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
                                if (player.DefPlayer.Team == TeamKind.Self)
                                    data.Target = new Vector2(TeeBackPosAy [player.Index].x, -TeeBackPosAy [player.Index].y);
                                else
                                    data.Target = TeeBackPosAy [player.Index];
                                
                                if (BallOwner != null)
                                    data.LookTarget = BallOwner.transform;
                                else
                                {
                                    if (player.Team == TeamKind.Self)
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
                            if (player.DefPlayer.Team == TeamKind.Self)
                                data.Target = new Vector2(TeeBackPosAy [player.Index].x, -TeeBackPosAy [player.Index].y);
                            else
                                data.Target = TeeBackPosAy [player.Index];
                            
                            if (BallOwner != null)
                                data.LookTarget = BallOwner.transform;
                            else
                            {
                                if (player.Team == TeamKind.Self)
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

	public void SetBallOwnerNull()
	{
		if (BallOwner != null) {
			BallOwner.IsBallOwner = false;
			BallOwner = null;
		}
	}

    public bool SetBall(PlayerBehaviour p = null)
    {
		bool Result = false;
		if (PlayerList.Count > 0)
        {
            if (p != null && situation != GameSituation.End)
            {
                if (BallOwner != null)
                {
                    if (BallOwner.Team != p.Team)
                    {
                        if (situation == GameSituation.AttackA)
                            ChangeSituation(GameSituation.AttackB);
                        else if (situation == GameSituation.AttackB)
                            ChangeSituation(GameSituation.AttackA);
                    } else
                    {
                        if (situation == GameSituation.TeeA)
                            ChangeSituation(GameSituation.AttackA);
                        else if (situation == GameSituation.TeeB)
                            ChangeSituation(GameSituation.AttackB);
                        else
                            BallOwner.ResetFlag(false);
                    }
                } else
                {
                    if (situation == GameSituation.TeeAPicking)
                    {
                        ChangeSituation(GameSituation.TeeA);
                    } else if (situation == GameSituation.TeeBPicking)
                    {
                        ChangeSituation(GameSituation.TeeB);
                    } else
                    {
                        if (p.Team == TeamKind.Self)
                            ChangeSituation(GameSituation.AttackA, p);
                        else if (p.Team == TeamKind.Npc)
                            ChangeSituation(GameSituation.AttackB, p);
                    }
                }

				if(BallOwner != null)
					BallOwner.IsBallOwner = false;

                BallOwner = p;
				BallOwner.WaitMoveTime = 0;
				BallOwner.IsBallOwner = true;
				Result = true;
				Passer = null;
				Shooter = null;
				Catcher = null;
				for(int i = 0 ; i < PlayerList.Count; i++)
					PlayerList[i].ClearAutoFollowTime();

				if (BallOwner && BallOwner.DefPlayer)
					BallOwner.DefPlayer.SetAutoFollowTime();

				if(situation == GameSituation.AttackA || situation == GameSituation.AttackB)
				{
					for(int i = 0; i < PlayerList.Count; i++)
					{
						if(PlayerList[i].HaveNoAiTime)
						{
							PlayerList[i].HaveNoAiTime = false;
							PlayerList[i].SetNoAiTime();
						}
					}
				}

                UIGame.Get.ChangeControl(p.Team == TeamKind.Self);
				UIGame.Get.SetPassButton(0);
				CourtMgr.Get.SetBallState(PlayerState.HoldBall, p);

				p.ClearIsCatcher();

                if (p)
				{
					p.WaitMoveTime = 0;
					p.IsFirstDribble = true;

					for (int i = 0; i < PlayerList.Count; i++){
						if (PlayerList [i].Team != p.Team){
							PlayerList [i].ResetMove();
							break;
						}
					}

					if (p.IsJump)
                    {
                        //ALLYOOP 
                    } else
                    {
//						if(p.NoAiTime == 0)
//							p.AniState(PlayerState.Dribble);
//						else
//                        	p.AniState(PlayerState.HoldBall);
                    }                    
                }

                Shooter = null;
				Catcher = null;
            } else
			{
				Catcher = null;
				SetBallOwnerNull();
//				if(BallOwner != null)
//					BallOwner.IsBallOwner = false;
//
//                BallOwner = p;
			}
        }

		return Result;
    }

	public PlayerBehaviour FindNearNpc(){
		PlayerBehaviour p = null;
		float dis = 0;
		for (int i=0; i<PlayerList.Count; i++) {
			if (PlayerList [i].Team == TeamKind.Npc){
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

		if (GameStart.Get.TestMode == GameTest.Shoot) {
			SetBall(Joysticker);		
		}
		else if(GameStart.Get.TestMode == GameTest.Block){
			SetBall(PlayerList [1]);
			PlayerList [1].AniState(PlayerState.Dribble);
			PlayerList [1].AniState(PlayerState.Shoot0);
		}
    }
	
	public bool PassingStealBall(PlayerBehaviour player, int dir)
	{
		if(player.IsDefence && (situation == GameSituation.AttackA || situation == GameSituation.AttackB) && Passer && PassingStealBallTime == 0)
		{
			int Rate = UnityEngine.Random.Range(0, 100);

			if(CourtMgr.Get.RealBallState == PlayerState.PassFlat || 
			   CourtMgr.Get.RealBallState == PlayerState.PassFloor ||
			   CourtMgr.Get.RealBallState == PlayerState.PassParabola || 
			   CourtMgr.Get.RealBallState == PlayerState.PassFast)
			{
				if(BallOwner == null && (Rate > Passer.Attr.PassRate || dir == 5) && !player.CheckAnimatorSate(PlayerState.Push))
				{
					if(dir == 6)
					{
						player.AniState(PlayerState.Intercept1, CourtMgr.Get.RealBall.transform.position);
					}
					else if(dir == 5)
					{
						if(player.CheckAnimatorSate(PlayerState.Intercept1))
						{
							if(BallTrigger.PassKind == 0 || BallTrigger.PassKind == 2)
								CourtMgr.Get.RealBall.transform.DOKill();
							
							if(SetBall(player))
							{
								player.AniState(PlayerState.HoldBall);
								PassingStealBallTime = Time.time + 2;
							}
							
							Catcher = null;
							IsPassing = false;
						}
					}
					else if(dir != 0)
					{
						player.AniState(PlayerState.Intercept0);
						
						if(BallTrigger.PassKind == 0 || BallTrigger.PassKind == 2)
							CourtMgr.Get.RealBall.transform.DOKill();
						
						if(SetBall(player))
						{
							player.AniState(PlayerState.HoldBall);
							PassingStealBallTime = Time.time + 2;
						}
						
						Catcher = null;
						IsPassing = false;
					}
				}
			}

			return true;
		}else
			return false;
	}

    public void BallTouchPlayer(PlayerBehaviour player, int dir, bool isEnter)
    {
		if (BallOwner || 
		    IsShooting || 
		    !player.IsCanCatchBall || 
		    player.CheckAnimatorSate(PlayerState.GotSteal) || 
		    player.CheckAnimatorSate(PlayerState.Push) || 
		    dir == 6)
            return;

		if (Catcher) 
		{
			if(situation == GameSituation.TeeAPicking || situation == GameSituation.TeeBPicking)
				Catcher = null;	
			else
				return;
		}			

		if(situation == GameSituation.TeeAPicking && player == Joysticker)
			return;
        
		switch (dir)
		{
		case 0: //top ,rebound
			if ((isEnter || GameStart.Get.TestMode == GameTest.Rebound) && player != BallOwner && CourtMgr.Get.RealBall.transform.position.y >= 3) {
				if (GameStart.Get.TestMode == GameTest.Rebound || situation == GameSituation.AttackA || situation == GameSituation.AttackB) {
					if (GameStart.Get.TestMode == GameTest.Rebound || CourtMgr.Get.RealBallState ==  PlayerState.Steal || CourtMgr.Get.RealBallState ==  PlayerState.Rebound) {
						if (Random.Range(0, 100) < player.Attr.ReboundRate) {
							Rebound(player);
						}
					}
				}
			}

            break;
		case 5: //finger
			if (isEnter && !player.IsBallOwner && player.IsRebound && !IsTipin) {
				if (GameStart.Get.TestMode == GameTest.Rebound || situation == GameSituation.AttackA || situation == GameSituation.AttackB) {
					if (SetBall(player)) {
						player.SetAnger(GameConst.AddAnger_Rebound);

						if (player == BallOwner && inTipinDistance(player)) {
							CoolDownPass = Time.time + 3;
							if (player == Joysticker)
								OnDoubleClickMoment(player, PlayerState.Rebound);
							else
							if (Random.Range(0, 100) < player.Attr.TipInRate)
								Shoot();
							else
							if (player.Team == Joysticker.Team)
								OnDoubleClickMoment(player, PlayerState.Rebound);
						}
					}
				}
			}

			break;
		default :
			bool CanSetball = false;
			
			if (!player.IsRebound && (player.IsCatcher || player.CanMove))
			{
				if (situation == GameSituation.TeeAPicking)
				{
					if (player.Team == TeamKind.Self)
						CanSetball = true;
				} else 
				if (situation == GameSituation.TeeBPicking)
				{
					if (player.Team == TeamKind.Npc)
						CanSetball = true;
				} else
					CanSetball = true;
				
				if (CanSetball && !IsPickBall)
				{
					if (situation == GameSituation.TeeAPicking || situation == GameSituation.TeeBPicking){
//						Debug.Log("height:"+CourtMgr.Get.RealBall.transform.position.y);
						if(CourtMgr.Get.RealBall.transform.position.y > 1.7f)
							player.AniState(PlayerState.CatchFlat, CourtMgr.Get.RealBall.transform.position);
//						else if(CourtMgr.Get.RealBall.transform.position.y > 1f && CourtMgr.Get.RealBall.transform.position.y <= 2f)
//							player.AniState(PlayerState.CatchFlat, CourtMgr.Get.RealBall.transform.position);
//						else if(CourtMgr.Get.RealBall.transform.position.y > 0.5f && CourtMgr.Get.RealBall.transform.position.y <= 1f)
//							player.AniState(PlayerState.CatchFloor, CourtMgr.Get.RealBall.transform.position);
						else
							player.AniState(PlayerState.PickBall, CourtMgr.Get.RealBall.transform.position);
					} else 
					if (SetBall(player)) {
						if(player.NoAiTime == 0)
							player.AniState(PlayerState.Dribble);
						else 
						if(player.CheckAnimatorSate(PlayerState.Run))
                        	player.AniState(PlayerState.RunAndDribble);
                    	else
                        	player.AniState(PlayerState.HoldBall);
					}
                }
            }

            break;
        }
    }
    
    public bool OnPickUpBall(PlayerBehaviour player)
    {
        if (player) {
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
					if(GetAngle(player2, player1) <= GameConst.SlowDownAngle)
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

    public void PlayerTouchPlayer(GameObject player)
    {
        
    }

	private bool canPassToAlleyoop(PlayerState state) {
		if (state == PlayerState.Idle ||
		    state == PlayerState.HoldBall ||
		    state == PlayerState.Dribble ||
		    state == PlayerState.RunAndDribble)
			return true;
		else
			return false;
    }
    
	public void PlayerEnterPaint(int team, GameObject obj) {
		if (BallOwner && canPassToAlleyoop(BallOwner.crtState) &&
		   (GameStart.Get.TestMode == GameTest.Alleyoop || 
		 	situation == GameSituation.AttackA || situation == GameSituation.AttackB)) {
			bool flag = true;
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList[i].crtState == PlayerState.Alleyoop) {
					flag = false;
					break;
				}

			if (flag) {
				PlayerBehaviour player = obj.GetComponent<PlayerBehaviour>();
				if (player && player.Team.GetHashCode() == team) {
					if (player != BallOwner && player.Team == BallOwner.Team) {
						if (Random.Range(0, 100) < player.Attr.AlleyOopRate) {
							player.AniState(PlayerState.Alleyoop, CourtMgr.Get.ShootPoint [team].transform.position);

							if (BallOwner != Joysticker) {
								if (BallOwner.AniState(PlayerState.PassFlat, player.transform.position))
									Catcher = player;
							} else
								UIGame.Get.SetPassButton(player.Index);
						}
					}
				}
			}
		}
	}

    private void GameResult(int team)
    {
        if (team == 0)
            UIHint.Get.ShowHint("You Win", Color.blue);
        else
            UIHint.Get.ShowHint("You Lost", Color.red);
        
        GameController.Get.ChangeSituation(GameSituation.End);
        UIGame.Get.Again.SetActive(true);
		UIGame.Get.ScoreBar.SetActive(true);
        UIGame.Get.Joystick.gameObject.SetActive(false);
    }
    
    public void PlusScore(int team)
    {
        if (IsStart && GameStart.Get.TestMode == GameTest.None)
		{
            int score = 2;
            if (ShootDis >= GameConst.TreePointDistance)
                score = 3;
//            else if (Shooter != null)
//            {
//                if (getDis(ref Shooter, SceneMgr.Get.ShootPoint [Shooter.Team.GetHashCode()].transform.position) >= 10)
//                    score = 3;
//            }

            ShootDis = 0;
            UIGame.Get.PlusScore(team, score);

            if (UIGame.Get.Scores [team] >= UIGame.Get.MaxScores [team])
                GameResult(team);
            else
            if (team == TeamKind.Self.GetHashCode())
                ChangeSituation(GameSituation.TeeBPicking);
            else
                ChangeSituation(GameSituation.TeeAPicking);

			if(Shooter != null)
				Shooter.SetAnger(GameConst.AddAnger_PlusScore);
            Shooter = null;

		}
    }

    public PlayerBehaviour HavePartner(ref PlayerBehaviour Npc, float dis, float angle)
    {
        PlayerBehaviour Result = null;
        Vector3 lookAtPos;
        Vector3 relative;
        float mangle;
        
        if (PlayerList.Count > 0)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList [i] != Npc && PlayerList [i].Team == Npc.Team)
                {
                    PlayerBehaviour TargetNpc = PlayerList [i];
                    lookAtPos = TargetNpc.transform.position;
                    relative = Npc.transform.InverseTransformPoint(lookAtPos);
                    mangle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                    
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
        Vector3 lookAtPos;
        Vector3 relative;
        float mangle;
        
        if (PlayerList.Count > 0)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList [i].Team != Npc.Team)
                {
                    PlayerBehaviour TargetNpc = PlayerList [i];
                    lookAtPos = TargetNpc.transform.position;
                    relative = Npc.transform.InverseTransformPoint(lookAtPos);
                    mangle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                    
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
		Vector3 lookAtPos;
		Vector3 relative;
		float mangle;
		man = null;
		
		if (PlayerList.Count > 0)
		{
			for (int i = 0; i < PlayerList.Count; i++)
			{
				if (PlayerList [i].Team != Npc.Team)
				{
					PlayerBehaviour TargetNpc = PlayerList [i];
					lookAtPos = TargetNpc.transform.position;
					relative = Npc.transform.InverseTransformPoint(lookAtPos);
					mangle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
					
					if (getDis(ref Npc, ref TargetNpc) <= dis && TargetNpc.CheckAnimatorSate(PlayerState.Idle))
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
		Vector3 lookAtPos;
		Vector3 relative;
		float mangle;

		if (P1 != null && P2 != null && P1 != P2) 
		{
			lookAtPos = P2.transform.position;
			relative = P1.transform.InverseTransformPoint(lookAtPos);
			mangle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
			
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
                        if (Npc != Self && Npc.Team != Self.Team && getDis(ref Self, ref Npc) <= Dis && Npc.crtState == PlayerState.Idle)
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
		if (Npc.Team == TeamKind.Self && (Npc.transform.position.z >= 15.5f && Npc.transform.position.x <= 1 && Npc.transform.position.x >= -1))
            return false;
        else if (Npc.Team == TeamKind.Npc && Npc.transform.position.z <= -15.5f && Npc.transform.position.x <= 1 && Npc.transform.position.x >= -1)
            return false;
		else if(Npc.CheckAnimatorSate(PlayerState.Elbow))
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
    
    public void EditSetJoysticker(int index)
    {
        if (PlayerList.Count > index)
        {
            Joysticker = PlayerList [index];        
        }
    }

    public void SetEndPass()
    {
		if (Catcher != null && !Catcher.IsFall && !Catcher.CheckAnimatorSate(PlayerState.Push))
        {
            if(SetBall(Catcher))
				CoolDownPass = Time.time + 3;

			if(Catcher && Catcher.NeedShooting)
			{
				Shoot();
				Catcher.NeedShooting = false;
			}
		}else{
			CourtMgr.Get.SetBallState(PlayerState.Steal, Passer);
			SetBall();
		}

		Catcher = null;
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
				Catcher = null;
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
		CourtMgr.Get.SetBallState(PlayerState.Steal, player);
		Catcher = null;
	}
	
	public void Reset()
	{
		IsReset = true;
		SetBallOwnerNull ();
		CourtMgr.Get.SetBallState (PlayerState.Reset);

		for (int i = 0; i < PlayerList.Count; i++) 
		{
			PlayerList [i].crtState = PlayerState.Idle;
			PlayerList [i].ResetFlag();
			PlayerList [i].transform.position = BornAy [i];								
		}

		Shooter = null;
		Catcher = null;
		Joysticker.AngerView.fillAmount = 0;
		situation = GameSituation.Opening;
		ChangeSituation (GameSituation.Opening);
    }

	public void SetPlayerLevel(){
		PlayerPrefs.SetFloat("AIChangeTime", GameData.Setting.AIChangeTime);
		if(GameData.Setting.AIChangeTime > 100) {
			Joysticker.SetNoAiTime();
		}
		for(int i=0; i<PlayerList.Count; i++) {
			if(i >= 3)
				PlayerAy[i].AILevel = GameConst.NpcAILevel;
			else
				PlayerAy[i].AILevel = GameConst.SelfAILevel;

			PlayerList[i].Player = PlayerAy[i];
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

	public void SetBodyMaterial(bool open,int index){

		if (PlayerList.Count > 0 && index < PlayerList.Count) {
			Shader loadshader;
			
			if(open)
				loadshader = Resources.Load("Shaders/Toony-BasicOutline", typeof(Shader)) as Shader; 
			else
				loadshader = Resources.Load("Shaders/Toony-Basic", typeof(Shader)) as Shader; 

			switch(index)
			{
				case 0:
					if(loadshader){
						PlayerList[index].BodyMaterial.shader = loadshader;
					}
						break;
				case 1:
					if(loadshader){
						PlayerList[index].BodyMaterial.shader = loadshader;
						if(open){
							PlayerList[index].BodyMaterial.SetColor("_OutlineColor", Color.yellow);
							PlayerList[index].BodyMaterial.SetFloat("_Outline", 0.002f);
						}
					}
					break;
				case 2:
					if(loadshader){
						PlayerList[index].BodyMaterial.shader = loadshader;
						if(open){
							PlayerList[index].BodyMaterial.SetColor("_OutlineColor", Color.blue);
							PlayerList[index].BodyMaterial.SetFloat("_Outline", 0.002f);
						}
					}
					break;
			}
		}

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

    public bool IsShooting
    {
        get
        {
            for (int i = 0; i < PlayerList.Count; i++)            
				if (PlayerList [i].CheckAnimatorSate(PlayerState.Shoot0) || 
				    PlayerList [i].CheckAnimatorSate(PlayerState.Shoot1) || 
				    PlayerList [i].CheckAnimatorSate(PlayerState.Shoot2) || 
				    PlayerList [i].CheckAnimatorSate(PlayerState.Shoot3) ||
				    PlayerList [i].CheckAnimatorSate(PlayerState.Shoot6) ||
				    PlayerList [i].CheckAnimatorSate(PlayerState.TipIn) ||
				    PlayerList [i].CheckAnimatorSate(PlayerState.Layup))
                    return true;            

            return false;
        }
    }

	public bool IsCanPassAir
	{
		get
		{
			for (int i = 0; i < PlayerList.Count; i++)            
				if (PlayerList [i].CheckAnimatorSate(PlayerState.Shoot0) || PlayerList [i].CheckAnimatorSate(PlayerState.Shoot2))
					return true;            
			
			return false;
		}
	}

    public bool IsDunk
    {
        get
        {
            for (int i = 0; i < PlayerList.Count; i++)            
				if (PlayerList [i].CheckAnimatorSate(PlayerState.Dunk) || PlayerList [i].CheckAnimatorSate(PlayerState.DunkBasket))
                    return true;            
            
            return false;
        }
    }

	public bool IsAlleyoop
	{
		get
		{
			for (int i = 0; i < PlayerList.Count; i++)            
				if (PlayerList [i].CheckAnimatorSate(PlayerState.Alleyoop))
					return true;            
			
			return false;
		}
	}

    public bool IsBlocking
    {
        get
        {
            for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].CheckAnimatorSate(PlayerState.Block))
					return true;
            
            return false;
        }
    }

	public bool IsPush
	{
		get
		{
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].CheckAnimatorSate(PlayerState.Push))
					return true;
			
			return false;
		}
	}

	public bool IsTipin
	{
		get
		{
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].CheckAnimatorSate(PlayerState.TipIn))
					return true;
            
            return false;
        }
    }

	public bool IsPickBall
	{
		get
		{
			for (int i = 0; i < PlayerList.Count; i++)
				if (PlayerList [i].CheckAnimatorSate(PlayerState.PickBall))
					return true;
			
			return false;
		}
	}


	public bool CandoBtn
	{
		get
		{
			if(situation == GameSituation.TeeA || situation == GameSituation.TeeB || situation == GameSituation.TeeAPicking || situation == GameSituation.TeeBPicking)
				return false;
			else
				return true;
		}
	}
}
