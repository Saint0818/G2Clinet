using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using RootMotion.FinalIK;
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

public enum GameTest
{
    None,
    AttackA,
    AttackB,
    Dunk,
    Block,
    Edit,
    OneByOne,
	Pass
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
	
    
    private GameSituation situation = GameSituation.None;
    private List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
    private List<TTactical> MovePositionList = new List<TTactical>();
    private Dictionary<int, int[]> situationPosition = new Dictionary<int, int[]>();
   
    public bool IsStart = false;
    public float CoolDownPass = 0;
    private float CoolDownCrossover = 0;
    private float ShootDis = 0;
    public float RealBallFxTime = 0;
	private float WaitTeeBallTime = 0;
	private float WaitStealTime = 0;
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
	private bool isCatchBall = false;

	//Score Animation Value
	public bool IsScore;
	public bool IsSwich;
	public bool IsAirBall;
	public string[] BasketScoreAnimationState;
	public string[] BasketScoreNoneAnimationState;


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
		InitBasket();
		CameraMgr.Get.setSplitScreen();
    }

	private void InitBasket(){
		Object[] objs = Resources.LoadAll("Stadiums/Bakset/Animation", typeof(AnimationClip));
		List<string> scoreName = new List<string>();
		List<string> noScoreName = new List<string>();
		for (int i=0; i<objs.Length; i++) {
			if(objs[i].name.Contains("BasketballAction_")){
				string[] nameSplit = objs[i].name.Split("_"[0]);
				int num = int.Parse(nameSplit[1]);
				if(num < 100) 
					scoreName.Add(objs[i].name);
				else
					noScoreName.Add(objs[i].name);
			}
		}
		BasketScoreAnimationState = new string[scoreName.Count];
		BasketScoreNoneAnimationState = new string[noScoreName.Count];
		for(int i=0; i<scoreName.Count; i++) {
			BasketScoreAnimationState[i] = scoreName[i];
		}
		for(int i=0; i<noScoreName.Count; i++) {
			BasketScoreNoneAnimationState[i] = noScoreName[i];
		}
	}

    private void InitPos()
    {
        TeeBackPosAy [0] = new Vector2(0, 8);
        TeeBackPosAy [1] = new Vector2(5.3f, 10);
        TeeBackPosAy [2] = new Vector2(-5.3f, 10);

		BornAy [0] = new Vector3 (0, 0, 0);
		BornAy [1] = new Vector3 (5, 0, -2);
		BornAy [2] = new Vector3 (-5, 0, -2);
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


    private TTactical GetMovePath(int index)
    {
        TTactical Result = new TTactical(false);

        if (index >= 0 && index < pathName.Length)
        {
            if (!situationPosition.ContainsKey(index))
            {
                List<int> ay = new List<int>();
                for (int i = 0; i < GameData.TacticalData.Length; i++)
                {
                    if (GameData.TacticalData [i].FileName.Contains(pathName [index]))
                        ay.Add(i);
                }
                
                situationPosition.Add(index, ay.ToArray());
            }   

            int len = situationPosition [index].Length;
            if (len > 0)
            {
                int r = Random.Range(0, len);
                int i = situationPosition [index] [r];
                Result = GameData.TacticalData [i];
            }
        }

        return Result;
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


	public void InitPlayer(){
		for (int i = 0; i < PlayerAy.Length; i++) {
			if(i >= (PlayerAy.Length / 2))
				PlayerAy[i] = new TPlayer(GameConst.NpcAILevel);
			else
				PlayerAy[i] = new TPlayer(GameConst.SelfAILevel);
			PlayerAy[i].ID = i;
			PlayerAy[i].Steal = UnityEngine.Random.Range(20, 100) + 1;

			if(i >= (PlayerAy.Length / 2)){
				PlayerAy[i].Body = 2001;
				PlayerAy[i].Cloth = 5002;
				PlayerAy[i].Hair = 2002;
				PlayerAy[i].MHandDress = 0;
				PlayerAy[i].Pants = 6002;
				PlayerAy[i].Shoes = 1002;
				PlayerAy[i].AHeadDress = 0;
				PlayerAy[i].ZBackEquip = 0;
			}
		}
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
                    PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
                break;
            case GameTest.AttackA:
            case GameTest.Dunk:
                PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, TeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
                break;
            case GameTest.AttackB:
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, TeamKind.Npc, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
				break;
            case GameTest.Block:
			case GameTest.OneByOne: 
				TPlayer Self = new TPlayer(0);
				Self.Steal = UnityEngine.Random.Range(20, 100) + 1;			

				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Self, new Vector3(0, 0, 0), Self));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Npc, new Vector3 (0, 0, 5), new GameStruct.TPlayer(0)));

                for (int i = 0; i < PlayerList.Count; i++)
                    PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
                break;
			case GameTest.Pass:
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (0, TeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
				PlayerList.Add (ModelManager.Get.CreateGamePlayer (1, TeamKind.Self, new Vector3 (0, 0, -2), new GameStruct.TPlayer(0)));
				for (int i = 0; i < PlayerList.Count; i++)
					PlayerList [i].DefPlayer = FindDefMen(PlayerList [i]);
				break;
            case GameTest.Edit:
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(0, TeamKind.Self, new Vector3(0, 0, 0), new GameStruct.TPlayer(0)));
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(1, TeamKind.Self, new Vector3(5, 0, -2), new GameStruct.TPlayer(0)));
				PlayerList.Add(ModelManager.Get.CreateGamePlayer(2, TeamKind.Self, new Vector3(-5, 0, -2), new GameStruct.TPlayer(0)));
				break;
        }

        Joysticker = PlayerList [0];
        EffectManager.Get.PlayEffect("SelectMe", Vector3.zero, null, Joysticker.gameObject);
        Joysticker.AIActiveHint = GameObject.Find("SelectMe/AI");

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
			PlayerList [i].OnFakeShootBlockMoment = OnFakeShootBlockMoment;
            PlayerList [i].OnBlockJump = OnBlockJump;
//            PlayerList [i].OnBlocking = OnBlocking;
            PlayerList [i].OnDunkJump = OnDunkJump;
            PlayerList [i].OnDunkBasket = OnDunkBasket;
			PlayerList [i].OnPickUpBall = OnPickUpBall;
			PlayerList [i].OnFall = OnFall;
        }
    }

	void Update(){
		if(isCatchBall && GameStart.Get.IsOpenIKSystem) {
			if(BallOwner) {
				Vector3 player = BallOwner.gameObject.transform.FindChild("DummyCatch").position;
				Vector3 pos = SceneMgr.Get.RealBall.transform.position;
				pos = Vector3.MoveTowards(pos, player, 0.25f);
				SceneMgr.Get.SetRealBallPosition(pos);
			}
		}

		if (Input.GetKeyDown (KeyCode.A))
			DoShoot (true);

		if (Input.GetKeyDown (KeyCode.R) && Joysticker != null)
			Joysticker.AniState (PlayerState.Rebound);

		if (Input.GetKeyDown (KeyCode.T) && Joysticker != null)
			Joysticker.AniState (PlayerState.ReboundCatch);

	}

    void FixedUpdate()
    {
        if (Time.time >= CoolDownPass)
            CoolDownPass = 0;

        if (Time.time >= CoolDownCrossover)
            CoolDownCrossover = 0;

        if (RealBallFxTime > 0)
        {
            RealBallFxTime -= Time.deltaTime;
            if (RealBallFxTime <= 0)
                SceneMgr.Get.RealBallFX.SetActive(false);
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
			
    }

    private void SituationAttack(TeamKind team)
    {
        if (PlayerList.Count > 0)
        {
            TTactical ap = GetMovePath(GetPosNameIndex(PosKind.Attack));

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
                                AIMove(ref Npc, ref ap);
                            } else 
                            if (!Npc.IsShoot)
                            {
                                Attack(ref Npc);
                                AIMove(ref Npc, ref ap);
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

                TTactical ap = new TTactical(false);
                TTactical defap = new TTactical(false);
				if (PickBallplayer != null)
				{
					ap = GetMovePath(GetPosNameIndex(PosKind.Tee, PickBallplayer.Index));
					defap = GetMovePath(GetPosNameIndex(PosKind.TeeDefence, PickBallplayer.Index));
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
                                TeeBall(ref Npc, team, ref ap);
                        } else 
                            BackToDef(ref Npc, TeamKind.Npc, ref defap);//SituationPickBall
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
                ap = GetMovePath(GetPosNameIndex(PosKind.Tee, BallOwner.Index));
                defap = GetMovePath(GetPosNameIndex(PosKind.TeeDefence, BallOwner.Index));
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
                SceneMgr.Get.RealBallFX.SetActive(false);
                for (int i = 0; i < PlayerList.Count; i++)
                {
					if(GS == GameSituation.TeeAPicking || GS == GameSituation.TeeBPicking)
					{
						if(PlayerList[i].NoAiTime > 0)
						{
							PlayerList[i].HaveNoAiTime = true;							
							PlayerList[i].SetAiTime();
						}
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
                TTactical pos = GetMovePath(GetPosNameIndex(PosKind.Fast, GetBall.Index));
                
				if(pos.FileName != string.Empty)
				{
					for (int i = 0; i < PlayerList.Count; i ++)
					{
						PlayerBehaviour npc = PlayerList [i];
						if (npc.Team == GetBall.Team)
						{
							TActionPosition [] ap = GetActionPosition(npc.Index, ref pos);
							
							if (ap != null)
							{
								for (int j = 0; j < ap.Length; j++)
								{
									TMoveData data = new TMoveData(0);
									data.Speedup = ap [j].Speedup;
									data.Catcher = ap [j].Catcher;
									data.Shooting = ap [j].Shooting;
									data.Target = new Vector2(ap [j].x, ap [j].z);
									if (BallOwner != null && BallOwner != npc)
										data.LookTarget = BallOwner.transform;  
									
									npc.TargetPos = data;
								}
							}
						}
					}
				}               
			}
			
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
				UIGame.Get.ChangeControl(true);
				CameraMgr.Get.SetTeamCamera(TeamKind.Self);
				PickBallplayer = null;
                break;
            case GameSituation.TeeA:
				EffectManager.Get.PlayEffect("ThrowInLineEffect", Vector3.zero, null, null, 0);
                break;
            case GameSituation.TeeBPicking:
           	 	UIGame.Get.ChangeControl(false);
           		CameraMgr.Get.SetTeamCamera(TeamKind.Npc);
				PickBallplayer = null;
                break;
			case GameSituation.TeeB:
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

	private void calculationScoreRate(ref bool isScore, PlayerBehaviour player, ScoreType type) {
//		Debug.Log("player name:"+player.name);
		if(player.name.Contains("Self")) {
			//0~90
			float angle = Mathf.Abs (GameFunction.GetPlayerToObjectAngle(SceneMgr.Get.Hood[0].transform, player.gameObject.transform)) - 90;
//			Debug.Log("self angle:"+angle);
		} else {
			float angle = Mathf.Abs (GameFunction.GetPlayerToObjectAngle(SceneMgr.Get.Hood[1].transform, player.gameObject.transform)) - 90;
//			Debug.Log("enemy angle:"+angle);
		}
		float originalRate = 0;
		if(ShootDis >= GameConst.TreePointDistance) {
			originalRate = player.ScoreRate.ThreeScoreRate * player.ScoreRate.ThreeScoreRateDeviation;
			EffectManager.Get.PlayEffect("ThreeLineEffect", Vector3.zero, null, null, 0);
		} else {
			originalRate = player.ScoreRate.TwoScoreRate * player.ScoreRate.TwoScoreRateDeviation;
		}
		if(type == ScoreType.DownHand) {
			isScore = (Random.Range(0.0f,100.0f) + 1) <= (originalRate - player.ScoreRate.DownHandScoreRate ) ? true : false;
			if(isScore) {
				IsSwich = (Random.Range(0.0f,100.0f) + 1) <= (originalRate - player.ScoreRate.DownHandSwishRate) ? true : false;
			} else {
				IsAirBall = (Random.Range(0, 100) + 1) <= player.ScoreRate.DownHandAirBallRate ? true : false;
			}
		} else 
		if(type == ScoreType.UpHand) {
			isScore = (Random.Range(0.0f,100.0f) + 1) <= (originalRate - player.ScoreRate.UpHandScoreRate) ? true : false;
			if(isScore) {
				IsSwich = (Random.Range(0.0f,100.0f) + 1) <= (originalRate - player.ScoreRate.UpHandSwishRate) ? true : false;
			} else {
				IsAirBall = (Random.Range(0, 100) + 1) <= player.ScoreRate.UpHandAirBallRate ? true : false;
			}
		} else 
		if(type == ScoreType.Normal) {
			isScore = (Random.Range(0.0f,100.0f) + 1) <= (originalRate - player.ScoreRate.NormalScoreRate) ? true : false;
			if(isScore) {
				IsSwich = (Random.Range(0.0f,100.0f) + 1) <= (originalRate - player.ScoreRate.NormalSwishRate) ? true : false;
			} else {
				IsAirBall = (Random.Range(0, 100) + 1) <= player.ScoreRate.NormalAirBallRate ? true : false;
			}
		} else 
		if(type == ScoreType.NearShot) {
			isScore = (Random.Range(0.0f,100.0f) + 1) <= (originalRate - player.ScoreRate.NearShotScoreRate ) ? true : false;
			if(isScore) {
				IsSwich = (Random.Range(0.0f,100.0f) + 1) <= (originalRate - player.ScoreRate.NearShotSwishRate) ? true : false;
			} else {
				IsAirBall = (Random.Range(0, 100) + 1) <= player.ScoreRate.NearShotAirBallRate ? true : false;
			}
		} else 
		if(type == ScoreType.LayUp) {
			isScore = (Random.Range(0.0f,100.0f) + 1) <= (originalRate - player.ScoreRate.LayUpScoreRate) ? true : false;
			if(isScore) {
				IsSwich = (Random.Range(0.0f,100.0f) + 1) <= (originalRate - player.ScoreRate.LayUpSwishRate) ? true : false;
			} else {
				IsAirBall = (Random.Range(0, 100) + 1) <= player.ScoreRate.LayUpAirBallRate ? true : false;
			}
		}
	}

	public void Shoot()
    {
        if (BallOwner)
        {
            SceneMgr.Get.ResetBasketEntra();

            int t = BallOwner.Team.GetHashCode();

            if (GameStart.Get.TestMode == GameTest.Dunk)
                BallOwner.AniState(PlayerState.Dunk, SceneMgr.Get.ShootPoint [t].transform.position);
            else if (Vector3.Distance(BallOwner.gameObject.transform.position, SceneMgr.Get.ShootPoint [t].transform.position) <= GameConst.DunkDistance)
			{
				float rate = Random.Range(0, 100);
				if(rate > 50)
                	BallOwner.AniState(PlayerState.Dunk, SceneMgr.Get.ShootPoint [t].transform.position);
				else
					BallOwner.AniState(PlayerState.Shoot1, SceneMgr.Get.Hood [t].transform.position);
			}
            else{

				float dis = Vector3.Distance(BallOwner.gameObject.transform.position, SceneMgr.Get.ShootPoint[BallOwner.Team.GetHashCode()].transform.position);

				if(BallOwner.IsMoving){
					if(dis > 15)
						BallOwner.AniState(PlayerState.Shoot3, SceneMgr.Get.Hood [t].transform.position);
					else if(dis > 9 && dis <= 15)
						BallOwner.AniState(PlayerState.Shoot2, SceneMgr.Get.Hood [t].transform.position);
					else
						BallOwner.AniState(PlayerState.Layup, SceneMgr.Get.Hood [t].transform.position);
				}
				else{
					if(dis > 15)
						BallOwner.AniState(PlayerState.Shoot3, SceneMgr.Get.Hood [t].transform.position);
					else if(dis > 9 && dis <= 15)
						BallOwner.AniState(PlayerState.Shoot0, SceneMgr.Get.Hood [t].transform.position);
					else
						BallOwner.AniState(PlayerState.Shoot6, SceneMgr.Get.Hood [t].transform.position);
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

			ShootDis = getDis(ref Shooter, SceneMgr.Get.ShootPoint [Shooter.Team.GetHashCode()].transform.position);
//			Debug.Log("player.crtState:"+player.crtState);
			if(player.crtState == PlayerState.Shoot0){
				calculationScoreRate(ref IsScore ,player, ScoreType.Normal);
			} else if(player.crtState == PlayerState.Shoot1 ||
			          player.crtState == PlayerState.Shoot6) {
				calculationScoreRate(ref IsScore ,player, ScoreType.NearShot);
			} else if(player.crtState == PlayerState.Shoot2) {
				calculationScoreRate(ref IsScore ,player, ScoreType.UpHand);
			} else if(player.crtState == PlayerState.Shoot3) {
				calculationScoreRate(ref IsScore ,player, ScoreType.DownHand);
			}
            
			SetBall();
            SceneMgr.Get.RealBall.transform.localEulerAngles = Vector3.zero;
			SceneMgr.Get.SetBallState(player.crtState);
			if(!IsScore && IsAirBall) {
				//AirBall
				Vector3 ori = SceneMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position - SceneMgr.Get.RealBall.transform.position;
				SceneMgr.Get.RealBallRigidbody.velocity = 
					GameFunction.GetVelocity(SceneMgr.Get.RealBall.transform.position, 
					                         SceneMgr.Get.RealBall.transform.position + (ori * 0.9f), 55);
			} else {
				SceneMgr.Get.RealBallRigidbody.velocity = 
					GameFunction.GetVelocity(SceneMgr.Get.RealBall.transform.position, 
					                         SceneMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position, 55);
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
		if (IsStart && BallOwner && CandoBtn)
        {
            PlayerBehaviour player = null;
            if (Joysticker == BallOwner)
            {
                player = Joysticker;
            } else 
            if (BallOwner.Team == TeamKind.Self) 
			{
                player = BallOwner;
				isshoot = true;
			}

            if (player)
            {
                int t = player.Team.GetHashCode();
                if (isshoot) 
					Shoot();
                else
                    player.AniState(PlayerState.FakeShoot, SceneMgr.Get.ShootPoint [t].transform.position);
            }
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
            SceneMgr.Get.SetBallState(PlayerState.DunkBasket);
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
            ShootDis = getDis(ref Shooter, SceneMgr.Get.ShootPoint [Shooter.Team.GetHashCode()].transform.position);
            return true;
        } else
            return false;
    }
    
    public bool Pass(PlayerBehaviour player, bool IsTee = false, bool IsBtn = false, bool MovePass = false)
    {
		bool Result = false;
		if (BallOwner != null && IsPassing == false && IsShooting == false && IsDunk == false && player != BallOwner)
        {
			if(!MovePass)
			{
				if(!IsBtn && CoolDownPass != 0)
					return Result;

				if(!IsBtn && BallOwner.NoAiTime > 0)
				   return Result;
			}
             
			if(IsTee)
			{
				if(BallOwner.AniState(PlayerState.Tee, player.transform.position))
				{
					Catcher = player;
				}												
			}else
			{
				float dis = Vector3.Distance(BallOwner.transform.position, player.transform.position);
				int disKind = GetEnemyDis(ref player);
				int rate = UnityEngine.Random.Range(0, 2);
				bool suc = false;
				int passkind = -1;

				if(dis <= GameConst.CloseDistance)
				{
					//Close
					if(disKind == 1)
					{
						if(rate == 1){
							suc = BallOwner.AniState(PlayerState.PassParabola, player.transform.position);
							passkind = 1;
						}else{ 
							suc = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
							passkind = 2;
						}
					} else 
					if(disKind == 2)
					{
						if(rate == 1){
							suc = BallOwner.AniState(PlayerState.PassFlat, player.transform.position);
							passkind = 0;
						}else{
							suc = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
							passkind = 2;
						}
					}						
					else
					{
						if(rate == 1){
							suc = BallOwner.AniState(PlayerState.PassFlat, player.transform.position);
							passkind = 0;
						}else{
							suc = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
							passkind = 2;
						}
					}
				}else if(dis <= GameConst.MiddleDistance)
				{
					//Middle
					if(disKind == 1)
					{
						if(rate == 1){
							suc = BallOwner.AniState(PlayerState.PassFlat, player.transform.position);
							passkind = 0;
						}else{
							suc = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
							passkind = 2;
						}
					} else 
					if(disKind == 2)
					{
						if(rate == 1){
							suc = BallOwner.AniState(PlayerState.PassParabola, player.transform.position);
							passkind = 1;
						}else{
							suc = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
							passkind = 2;
						}
					}						
					else
					{
						if(rate == 1){
							suc = BallOwner.AniState(PlayerState.PassFlat, player.transform.position);
							passkind = 0;
						}else{
							suc = BallOwner.AniState(PlayerState.PassFloor, player.transform.position);
							passkind = 2;
						}
					}
				}else{
					//Far
					suc = BallOwner.AniState(PlayerState.PassParabola, player.transform.position);
					passkind = 1;
				}

				if(suc){
					Catcher = player;

					float adis = Vector3.Distance(BallOwner.transform.position, Catcher.transform.position);
					if(adis <= 1){
						if(passkind == 0)
							Catcher.AniState(PlayerState.CatchFlat, BallOwner.transform.position);
						else if(passkind == 1)
							Catcher.AniState(PlayerState.CatchParabola, BallOwner.transform.position);
						else if(passkind == 2)
							Catcher.AniState(PlayerState.CatchFloor, BallOwner.transform.position);
					}
				}
			}            

			Result = true;
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

    public void DoPass(int playerid)
    {
		if (IsStart && BallOwner && !Shooter && Joysticker && BallOwner.Team == 0 && CandoBtn)
        {
            if (PlayerList.Count > 2)
            {
				float aiTime = BallOwner.NoAiTime;
				BallOwner.NoAiTime = 0;

                if (BallOwner == Joysticker)
                    Pass(PlayerList [playerid], false, true);
                else
					Pass(Joysticker, false, true);

				Joysticker.NoAiTime = aiTime;
            }
        }
    }

    private void Steal(PlayerBehaviour player)
    {
        
    }
	
	public bool OnStealMoment(PlayerBehaviour player)
    {
        if (BallOwner && BallOwner.Invincible == 0 && !IsShooting && !IsDunk){
			if(Vector3.Distance(BallOwner.transform.position, player.transform.position) <= GameConst.StealBallDistance)
			{
				int r = Mathf.RoundToInt(player.Attr.Steal - BallOwner.Attr.Control);
				int maxRate = 100;
				int minRate = 10;
				
				if (r > maxRate)
					r = maxRate;
				else if (r < minRate)
					r = minRate;
				
				int stealRate = Random.Range(0, 100) + 1;
				int AddRate = 0;
				if(SceneMgr.Get.RealBallFX.activeInHierarchy)
					AddRate = 30;
				
				Vector3 lookAtPos = player.transform.position;
				Vector3 relative = BallOwner.transform.InverseTransformPoint(lookAtPos);
				float mangle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
				
				if (stealRate <= (r + AddRate) && mangle <= 70 && mangle >= -70)
				{
					if(BallOwner)
						BallOwner.AniState(PlayerState.GotSteal);
					
//					setDropBall(player);
					return true;
				}else
					if(BallOwner != null && HaveStealPlayer(ref player, ref BallOwner, GameConst.StealBallDistance, 15) != 0)
				{
					stealRate = Random.Range(0, 100) + 1;
					
					if(stealRate <= r)
					{
						RealBallFxTime = 1f;
						SceneMgr.Get.RealBallFX.SetActive(true);
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
            if (Shooter && Vector3.Distance(player.transform.position, SceneMgr.Get.RealBall.transform.position) < 5)
            {
                player.PlayerRigidbody.velocity = GameFunction.GetVelocity(player.transform.position, 
                    new Vector3(SceneMgr.Get.RealBall.transform.position.x, 5, SceneMgr.Get.RealBall.transform.position.z), 70);
        
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
			if(BallOwner)
				Joysticker.rotateTo(BallOwner.gameObject.transform.position.x, BallOwner.gameObject.transform.position.z); 

            if (Shooter)
                Joysticker.AniState(PlayerState.Block, Shooter.transform.position);
            else
            if (BallOwner)
                Joysticker.AniState(PlayerState.Block, BallOwner.transform.position);
            else
                Joysticker.AniState(PlayerState.Block);
        }           
    }

    private void Rebound(PlayerBehaviour player)
    {
        
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
		if (Joysticker && (CanMove || Joysticker.IsFirstDribble))
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

		if(!Self.CheckAnimatorSate(PlayerState.HoldBall))
		{
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
							Self.AniState(PlayerState.FakeShoot, SceneMgr.Get.ShootPoint [Self.Team.GetHashCode()].transform.position);
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
			int shoot3Rate = Random.Range(0, 100) + 1;
			int passRate = Random.Range(0, 100) + 1;
			int pushRate = Random.Range(0, 100) + 1;
			int supRate = Random.Range(0, 100) + 1;
			int ALLYOOP = Random.Range(0, 100) + 1;
			int ElbowRate = Random.Range(0, 100) + 1;
			float Dis = 0;
			float ShootPointDis = 0;
            Vector3 pos = SceneMgr.Get.ShootPoint [Npc.Team.GetHashCode()].transform.position;
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
				if (ElbowRate < GameData.AIlevelAy[Npc.Attr.AILevel].ElbowingRate && CheckAttack(ref Npc) && (HaveDefPlayer(ref Npc, GameConst.StealBallDistance, 90, out man) != 0) && 
					Npc.CoolDownElbow ==0 && !Npc.CheckAnimatorSate(PlayerState.Elbow)){
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
                if (Npc.Attr.AILevel >= 3 && Dir != 0 && CoolDownCrossover == 0 && Npc.CanMove)
                {
                    //Crossover             
                    TMoveData data = new TMoveData(0);
                    if (Dir == 1)
                        data.Target = new Vector2(Npc.transform.position.x - 2, Npc.transform.position.z);
                    else
                        data.Target = new Vector2(Npc.transform.position.x + 2, Npc.transform.position.z);
                    
                    Npc.FirstTargetPos = data;


					data = new TMoveData(0);
					int AddZ = 3;
					if(Npc.Team == TeamKind.Npc)
						AddZ = -3;

					if (Dir == 1){
						if (Npc.Team == TeamKind.Self)
							data.Target = new Vector2(Npc.transform.position.x - 2, Mathf.Max(Npc.transform.position.z + AddZ, -17));
						else
							data.Target = new Vector2(Npc.transform.position.x - 2, Mathf.Min(Npc.transform.position.z + AddZ, 17));
					}else{		
						if (Npc.Team == TeamKind.Self)
							data.Target = new Vector2(Npc.transform.position.x + 2, Mathf.Max(Npc.transform.position.z + AddZ, -17));
						else
							data.Target = new Vector2(Npc.transform.position.x + 2, Mathf.Min(Npc.transform.position.z + AddZ, 17));					
					}
					
					Npc.FirstTargetPos = data;
					CoolDownCrossover = Time.time + 4;
				}
			} else
			{
				//sup push
				Dis = getDis(ref BallOwner, ref Npc); 
				PlayerBehaviour NearPlayer = HaveNearPlayer(Npc, GameConst.StealBallDistance, false);
                
                if (ShootPointDis <= 1.5f && ALLYOOP < 50)
                {
                    //Npc.AniState(PlayerState.Jumper);
                } else 
                if (NearPlayer != null && pushRate < GameData.AIlevelAy[Npc.Attr.AILevel].PushingRate && Npc.CoolDownPush == 0)
                {
                    //Push
					if(Npc.AniState(PlayerState.Push, NearPlayer.transform.position))
						Npc.CoolDownPush = Time.time + 3;                    
                } else 
                if (Dis >= 1.5f && Dis <= 3 && supRate < 50)
                {
                    //Sup
                    
                }
            }   
        }
    }

	private bool GetStealRate(ref PlayerBehaviour npc)
	{
		bool Result = false;

		int r = Mathf.RoundToInt(npc.Attr.Steal - BallOwner.Attr.Control);
		int maxRate = 100;
		int minRate = 10;
		int stealRate = Random.Range(0, 100) + 1;
		int AddRate = 0;
		
		if (r > maxRate)
			r = maxRate;
		else if (r < minRate)
			r = minRate;

		if(SceneMgr.Get.RealBallFX.activeInHierarchy)
			AddRate = 30;

		if (stealRate <= (GameData.AIlevelAy[npc.Attr.AILevel].StealRate + AddRate))
            Result = true;

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
							if (DisAy[i].Distance <= GameConst.StealBallDistance && DisAy[i].Player.crtState == PlayerState.Idle && pushRate <= GameData.AIlevelAy[Npc.Attr.AILevel].PushingRate && Npc.CoolDownPush == 0 && !IsPush)
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
							if(GetStealRate(ref Npc))
                            {
                                if (Npc.AniState(PlayerState.Steal, BallOwner.gameObject.transform.position)){
                                	Npc.CoolDownSteal = Time.time + 3;                              
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
				TActionPosition [] ap = GetActionPosition(Npc.Index, ref pos);
				
				if (ap != null)
				{
					for (int i = 0; i < ap.Length; i++)
					{
						if (Team == TeamKind.Self)
							data.Target = new Vector2(ap [i].x, -ap [i].z);
						else
							data.Target = new Vector2(ap [i].x, ap [i].z);
						
						if (BallOwner != null)
							data.LookTarget = BallOwner.transform;
						else
						{
							if (Team == TeamKind.Self)
								data.LookTarget = SceneMgr.Get.Hood [1].transform;
							else
								data.LookTarget = SceneMgr.Get.Hood [0].transform;
						}
						
						if (!WatchBallOwner)
							data.Speedup = true;
						Npc.TargetPos = data;
					}
				}
			}
		}
	}
	
	private void TeeBall(ref PlayerBehaviour Npc, TeamKind Team, ref TTactical pos)
	{
		TMoveData data = new TMoveData(0);

		if(Npc == BallOwner && Npc.TargetPosNum > 3)
			Npc.ResetMove();

		if (Npc.CanMove && !Npc.IsMoving && Npc.WaitMoveTime == 0 && Npc.TargetPosNum == 0)
		{
			if (Npc == BallOwner)
			{
				int TargetZ = 18;
				if(Team == TeamKind.Self)
					TargetZ = -18;

				if(Npc.Team == TeamKind.Self && Npc.transform.position.z <= -17 && Npc.transform.position.z >= -18)
					return;
				else if(Npc.Team == TeamKind.Npc && Npc.transform.position.z >= 17 && Npc.transform.position.z <= 18)
					return;

				data.Target = new Vector2(Npc.transform.position.x, TargetZ);
                data.MoveFinish = NpcAutoTee;
                Npc.TargetPos = data;
            } else 
			if(pos.FileName != string.Empty)
            {
				TActionPosition [] ap = GetActionPosition(Npc.Index, ref pos);
                
                if (ap != null)
                {
                    for (int j = 0; j < ap.Length; j++)
                    {
                        data = new TMoveData(0);
                        data.Speedup = ap [j].Speedup;
						data.Catcher = ap [j].Catcher;
						data.Shooting = ap [j].Shooting;
                        if (Team == TeamKind.Self) 
                            data.Target = new Vector2(ap [j].x, ap [j].z);
                        else
                            data.Target = new Vector2(ap [j].x, -ap [j].z);
                        data.LookTarget = SceneMgr.Get.RealBall.transform;
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
            if (Npc1.Team == Npc.Team && !Npc.IsFall && Npc.NoAiTime == 0)
            {
                if (NearPlayer == null)
                    NearPlayer = Npc1;
                else if (getDis(ref NearPlayer, SceneMgr.Get.RealBall.transform.position) > getDis(ref Npc1, SceneMgr.Get.RealBall.transform.position))
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
	                else if (getDis(ref NearPlayer, SceneMgr.Get.RealBall.transform.position) > getDis(ref Npc, SceneMgr.Get.RealBall.transform.position))
	                    NearPlayer = Npc;
				}
            }
        }
        
        return NearPlayer;
    }

    private void DefBlock(ref PlayerBehaviour Npc, int Kind = 0)
    {
        if (PlayerList.Count > 0 && !IsPassing)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerBehaviour Npc2 = PlayerList [i];

                if (Npc2 != Npc && Npc2.Team != Npc.Team && Npc2.NoAiTime == 0 && 
				    !Npc2.CheckAnimatorSate(PlayerState.Steal) && 
				    !Npc2.CheckAnimatorSate(PlayerState.Push))
                {
					if(!IsBlocking)
					{
						int Rate = Random.Range(0, 100) + 1;
						int BlockRate = GameData.AIlevelAy[Npc2.Attr.AILevel].BlockRate;
						
						if(Kind == 1)
							BlockRate = GameData.AIlevelAy[Npc2.Attr.AILevel].FaketBlockRate;	

						if (GameStart.Get.TestMode == GameTest.Block)
						{
							Npc2.AniState(PlayerState.Block, Npc.transform.position);
						} else
						if (getDis(ref Npc, ref Npc2) <= GameConst.BlockDistance)
						{
							if (Rate <= BlockRate)
							{
								Npc2.AniState(PlayerState.Block, Npc.transform.position);
							}
						}
					}else
						break;					
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
                    data.FollowTarget = SceneMgr.Get.RealBall.transform;
                    A.TargetPos = data;
				} else if(Npc.crtState != PlayerState.Block && Npc.NoAiTime == 0)
                    Npc.rotateTo(SceneMgr.Get.RealBall.transform.position.x, SceneMgr.Get.RealBall.transform.position.z);
            } else if (Npc.CanMove && Npc.WaitMoveTime == 0)
            {
                TMoveData data = new TMoveData(0);
                data.FollowTarget = SceneMgr.Get.RealBall.transform;
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
	            PickBall(ref npc, true);
	            PickBall(ref npc.DefPlayer, true);
			}
        } else
        {
			if (npc.CanMove && !npc.IsMoving && npc.WaitMoveTime == 0 && npc.TargetPosNum == 0)
	        {
	            TMoveData data;
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
					TActionPosition [] ap = GetActionPosition(npc.Index, ref pos);

	                if (ap != null)
	                {
	                    for (int i = 0; i < ap.Length; i++)
	                    {
	                        data = new TMoveData(0);
	                        data.Speedup = ap [i].Speedup;
							data.Catcher = ap [i].Catcher;
							data.Shooting = ap [i].Shooting;
	                        int z = 1;
	                        if (npc.Team != TeamKind.Self)
	                            z = -1;
	                        
	                        data.Target = new Vector2(ap [i].x, ap [i].z * z);
	                        if (BallOwner != null && BallOwner != npc)
	                            data.LookTarget = BallOwner.transform;  
	                        
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
        if (player.DefPlayer != null)
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
                        
						if (dis2 <= GameData.AIlevelAy [player.DefPlayer.Attr.AILevel].DefDistance)
                        {
							PlayerBehaviour p = HaveNearPlayer(player.DefPlayer, GameData.AIlevelAy [player.DefPlayer.Attr.AILevel].DefDistance, false, true);
                            if (p != null)
                                data2.DefPlayer = p;
							else if (getDis(ref player, ref player.DefPlayer) <= GameData.AIlevelAy [player.DefPlayer.Attr.AILevel].DefDistance)
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
                                        data.LookTarget = SceneMgr.Get.Hood [1].transform;
                                    else
                                        data.LookTarget = SceneMgr.Get.Hood [0].transform;
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
                                    data.LookTarget = SceneMgr.Get.Hood [1].transform;
                                else
                                    data.LookTarget = SceneMgr.Get.Hood [0].transform;
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
				if(SceneMgr.Get.RealBall.transform.position.y >= 2f ) {
					SceneMgr.Get.SetBallState(PlayerState.HoldBall, p);
				} else {
//					p.AniState(PlayerState.PickBall);
					if(GameFunction.GetPlayerToObjectAngle(BallOwner.gameObject.transform, SceneMgr.Get.RealBall.gameObject.transform) < 60 &&
					   GameFunction.GetPlayerToObjectAngle(BallOwner.gameObject.transform, SceneMgr.Get.RealBall.gameObject.transform) > -60 ) {
						StartCoroutine(catchBall(p));
					} else {
						SceneMgr.Get.SetBallState(PlayerState.HoldBall, p);
					}
				}
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
				SetBallOwnerNull();
//				if(BallOwner != null)
//					BallOwner.IsBallOwner = false;
//
//                BallOwner = p;
			}
        }

		return Result;
    }

	IEnumerator catchBall(PlayerBehaviour p) {
		if(!GameStart.Get.IsOpenIKSystem){
			yield return null;
			SceneMgr.Get.SetBallState(PlayerState.HoldBall, p);
		} else {
			p.isIKOpen = true;
			p.isIKCatchBall = true;
			yield return new WaitForSeconds(0.08f);
			isCatchBall = true;
			yield return new WaitForSeconds(0.17f);
			p.isIKOpen = false;
			p.isIKCatchBall = false;
			isCatchBall = false;
			SceneMgr.Get.SetBallState(PlayerState.HoldBall, p);
		}
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
        SceneMgr.Get.ResetBasketEntra();
        GameController.Get.Shooter = null;
    }

    public void BallTouchPlayer(PlayerBehaviour player, int dir)
    {
		if (BallOwner || IsShooting || !player.IsCanCatchBall || player.CheckAnimatorSate(PlayerState.GotSteal))
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

        //rebound
        if (dir == 0)
        {
        } else
        {
            bool CanSetball = false;

            if (player && (player.IsCatcher || player.CanMove))
            {
                if (situation == GameSituation.TeeAPicking)
                {
                    if (player.Team == TeamKind.Self)
                        CanSetball = true;
                } else if (situation == GameSituation.TeeBPicking)
                {
                    if (player.Team == TeamKind.Npc)
                        CanSetball = true;
                } else
                {
                    CanSetball = true;
                }

                if (CanSetball)
                {
					if(situation == GameSituation.TeeAPicking || situation == GameSituation.TeeBPicking){
						player.AniState(PlayerState.PickBall, SceneMgr.Get.RealBall.transform.position);
					}
					else{
	                    SetBall(player);
						if(player.NoAiTime == 0)
							player.AniState(PlayerState.Dribble);
						else if(player.CheckAnimatorSate(PlayerState.Run))
							player.AniState(PlayerState.RunAndDribble);
						else
							player.AniState(PlayerState.HoldBall);
					}

					switch (dir)
					{
					case 0: //top
						break;
					case 1: //FR
						break;
					}
                }
            }
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
            SceneMgr.Get.ResetBasketEntra();

            int score = 2;
            if (ShootDis >= GameConst.TreePointDistance)
                score = 3;
            else if (Shooter != null)
            {
                if (getDis(ref Shooter, SceneMgr.Get.ShootPoint [Shooter.Team.GetHashCode()].transform.position) >= 10)
                    score = 3;
            }

            ShootDis = 0;
            UIGame.Get.PlusScore(team, score);

            if (UIGame.Get.Scores [team] >= UIGame.Get.MaxScores [team])
                GameResult(team);
            else
            if (team == TeamKind.Self.GetHashCode())
                ChangeSituation(GameSituation.TeeBPicking);
            else
                ChangeSituation(GameSituation.TeeAPicking);

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
        if (Npc.Team == TeamKind.Self && Npc.transform.position.z > 16.4)
            return false;
        else 
        if (Npc.Team == TeamKind.Npc && Npc.transform.position.z < -16.4)
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
        if (Catcher != null && !Catcher.IsFall)
        {
            if(SetBall(Catcher))
				CoolDownPass = Time.time + 3;
			if(Catcher && Catcher.NeedShooting)
			{
				Shoot();
				Catcher.NeedShooting = false;
			}
            Catcher = null;
		}else{
			Catcher = null;
			SetBall();
			SceneMgr.Get.RealBall.transform.position = new Vector3(SceneMgr.Get.RealBall.transform.position.x, 0, SceneMgr.Get.RealBall.transform.position.z);
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
		SceneMgr.Get.SetBallState(PlayerState.Steal, player);
		Catcher = null;
	}
	
	public void Reset()
	{
		SetBallOwnerNull ();
		SceneMgr.Get.SetBallState (PlayerState.Reset);
		for(int i = 0; i < PlayerList.Count; i++){
			Destroy(PlayerList[i].gameObject);
		}
		GameObject selectMeObj = GameObject.Find("SelectMe");
		if(selectMeObj)
			Destroy(selectMeObj);
		GameObject selectAObj = GameObject.Find("SelectA");
		if(selectAObj)
			Destroy(selectAObj);
		GameObject selectBObj = GameObject.Find("SelectB");
		if(selectBObj)
			Destroy(selectBObj);
		InitGame();
    }

	public void SetPlayerLevel(){
		for(int i=0; i<PlayerList.Count; i++) {
			if(i >= 3)
				PlayerAy[i].AILevel = GameConst.NpcAILevel;
			else
				PlayerAy[i].AILevel = GameConst.SelfAILevel;
			
			PlayerList[i].Attr = PlayerAy[i];
		}
	}

	private TActionPosition [] GetActionPosition(int Index, ref TTactical pos)
	{
		TActionPosition [] Result = null;
		
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
				    PlayerList [i].CheckAnimatorSate(PlayerState.Shoot3))
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

	public void SetBallOwnerNull()
	{
		if (BallOwner != null) {
			BallOwner.IsBallOwner = false;
			BallOwner = null;
		}
	}
}
