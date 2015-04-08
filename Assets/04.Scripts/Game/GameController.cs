using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using RootMotion.FinalIK;

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
    OneByOne
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
    private PlayerBehaviour BallOwner;
    private bool IsStart = true;
    private float CoolDownPass = 0;
    private float CoolDownCrossover = 0;
    private float ShootDis = 0;
    private float RealBallFxTime = 0;
    public PlayerBehaviour Joysticker;
    public PlayerBehaviour Catcher;
    public PlayerBehaviour Shooter;
    public Vector2[] TeeBackPosAy = new Vector2[3];
	public Vector3[] BornAy = new Vector3[6];
	private GameStruct.TPlayer [] PlayerAy = new TPlayer[6];

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
        BallOwner = null;
        Shooter = null;
        Catcher = null;
        situation = GameSituation.Opening;
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
			PlayerAy[i] = new TPlayer(0);
			PlayerAy[i].ID = i;
			PlayerAy[i].Steal = UnityEngine.Random.Range(20, 100) + 1;

			if(i >= 3){
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
					if(i >= 3)
						PlayerList.Add(ModelManager.Get.CreateGamePlayer(i % 3, TeamKind.Npc, BornAy[i], PlayerAy[i]));				
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
            PlayerList [i].OnPass = OnPass;
            PlayerList [i].OnSteal = OnSteal;
            PlayerList [i].OnBlockMoment = OnBlockMoment;
            PlayerList [i].OnBlockJump = OnBlockJump;
            PlayerList [i].OnBlocking = OnBlocking;
            PlayerList [i].OnDunkJump = OnDunkJump;
            PlayerList [i].OnDunkBasket = OnDunkBasket;
        }
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
                            if (!Npc.CheckAction(ActionFlag.IsShoot))
                            {
                                Attack(ref Npc);
                                AIMove(ref Npc, ref ap);
                            }                               
                        }
                    } else
                        Defend(ref Npc);
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
                PlayerBehaviour pickplayer = NearBall(team);
                TTactical ap = new TTactical(false);
                TTactical defap = new TTactical(false);
                if (pickplayer != null)
                {
                    ap = GetMovePath(GetPosNameIndex(PosKind.Tee, pickplayer.Index));
                    defap = GetMovePath(GetPosNameIndex(PosKind.TeeDefence, pickplayer.Index));
                }                   

                for (int i = 0; i < PlayerList.Count; i++)
                {
                    PlayerBehaviour Npc = PlayerList [i];
                    if (CandoAI(Npc))
                    {
                        if (Npc.Team == team)
                        {
                            if (Npc == pickplayer)
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
        if (npc.isJoystick && npc.Team == TeamKind.Self && npc == Joysticker && npc.NoAiTime != 0)
            return false;
        else
            return true;
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
					switch(PlayerList[i].Team)
					{
					case TeamKind.Self:
						if((GS == GameSituation.TeeB || (oldgs == GameSituation.TeeB && GS == GameSituation.AttackB)) == false)
							PlayerList[i].ResetFlag();
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
                    break;
                case GameSituation.TeeA:
                    break;
                case GameSituation.TeeBPicking:
                    UIGame.Get.ChangeControl(false);
                    CameraMgr.Get.SetTeamCamera(TeamKind.Npc);
                    break;
                case GameSituation.TeeB:
                    break;
                case GameSituation.End:
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

    public void Shoot()
    {
        if (BallOwner)
        {
            SceneMgr.Get.ResetBasketEntra();

            int t = BallOwner.Team.GetHashCode();

            if (GameStart.Get.TestMode == GameTest.Dunk)
                BallOwner.AniState(PlayerState.Dunk, SceneMgr.Get.ShootPoint [t].transform.position);
            else if (Vector3.Distance(BallOwner.gameObject.transform.position, SceneMgr.Get.ShootPoint [t].transform.position) <= GameConst.DunkDistance)
                BallOwner.AniState(PlayerState.Dunk, SceneMgr.Get.ShootPoint [t].transform.position);
            else
                BallOwner.AniState(PlayerState.Shooting, SceneMgr.Get.Hood [t].transform.position);
        }
    }
        
    public bool OnShooting(PlayerBehaviour player)
    {
        if (BallOwner && BallOwner == player)
        {                   
            Shooter = player;
            SetBall();
            SceneMgr.Get.RealBall.transform.localEulerAngles = Vector3.zero;
            SceneMgr.Get.SetBallState(PlayerState.Shooting);
            SceneMgr.Get.RealBallRigidbody.velocity = 
                GameFunction.GetVelocity(SceneMgr.Get.RealBall.transform.position, 
                                         SceneMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position, 60);

            ShootDis = getDis(ref Shooter, SceneMgr.Get.ShootPoint [Shooter.Team.GetHashCode()].transform.position);

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
        if (IsStart && BallOwner)
        {
            PlayerBehaviour player = null;
            if (Joysticker == BallOwner)
            {
                Joysticker.SetNoAiTime();
                player = Joysticker;
            } else 
            if (BallOwner.Team == TeamKind.Self) 
                player = BallOwner;

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
    
    public bool Pass(PlayerBehaviour player)
    {
		bool Result = false;
		if (BallOwner != null && Catcher == null && IsPassing == false && IsShooting == false && IsDunk == false)
        {
			if(BallOwner == Joysticker && BallOwner.NoAiTime > 0)
				return Result;

            Catcher = player;
            Catcher.AniState(PlayerState.Catch, BallOwner.transform.position);
            BallOwner.AniState(PlayerState.Pass, Catcher.transform.position);
			Result = true;
        }

		return Result;
    }
    
    public bool OnPass(PlayerBehaviour player)
    {
        if (Catcher)
        {
            SceneMgr.Get.SetBallState(PlayerState.Pass);
            SceneMgr.Get.RealBallRigidbody.velocity = GameFunction.GetVelocity(SceneMgr.Get.RealBall.transform.position, Catcher.DummyBall.transform.position, Random.Range(40, 60));   
            if (Vector3.Distance(SceneMgr.Get.RealBall.transform.position, Catcher.DummyBall.transform.position) > 15f)
                CameraMgr.Get.IsLongPass = true;
            
            return true;
        } else
            return false;
    }

    public void DoPass(int playerid)
    {
        if (IsStart && BallOwner && !Shooter && Joysticker && BallOwner.Team == 0)
        {
            if (PlayerList.Count > 2)
            {
                if (BallOwner == Joysticker)
                {
                    Pass(PlayerList [playerid]);
                } else
                    Pass(Joysticker);

                Joysticker.SetNoAiTime();
            }
        }
    }

    private void Steal(PlayerBehaviour player)
    {
        
    }
	
	public bool OnSteal(PlayerBehaviour player)
    {
        if (BallOwner){
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
							
			if (stealRate <= (r + AddRate))
			{
				setDropBall();
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
        
        return false;
    }

    public void DoSteal()
    {
        if (IsStart && Joysticker)
        {
            Joysticker.SetNoAiTime();   
            if (BallOwner && BallOwner.Team != Joysticker.Team)
            {
                Joysticker.AniState(PlayerState.Steal, BallOwner.transform.position);
            } else
                Joysticker.AniState(PlayerState.Steal);
        }
    }

    private void Block(PlayerBehaviour player)
    {
        
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

    public bool OnBlocking(PlayerBehaviour player)
    {
        int blockRate = UnityEngine.Random.Range(0, 100);
        if (blockRate < 30)
        {
            if (!BallOwner)
            {
                float dis = Vector3.Distance(player.transform.position, SceneMgr.Get.RealBall.transform.position);
                if (dis <= 4)
                {
                    SceneMgr.Get.SetBallState(PlayerState.Block);
                    return true;
                }
            } else 
            if (BallOwner.CheckAction(ActionFlag.IsShoot))
            {
                float dis = Vector3.Distance(player.transform.position, BallOwner.transform.position);
                if (dis <= 4)
                {
                    for (int i = 0; i < PlayerList.Count; i++)
                        if (PlayerList [i].Team == BallOwner.Team)
                            PlayerList [i].ResetMove();

                    SetBall(null);
                    SceneMgr.Get.SetBallState(PlayerState.Block);
					RealBallFxTime = 1f;
					SceneMgr.Get.RealBallFX.SetActive(true);
                    return true;
                }
            }
        }

        return false;
    }

    public void DoBlock()
    {
        if (IsStart)
        {
            Joysticker.SetNoAiTime();       
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
        if (Joysticker && CanMove)
        {
            if (Mathf.Abs(move.joystickAxis.y) > 0 || Mathf.Abs(move.joystickAxis.x) > 0)
            {
                Joysticker.ClearMoveQueue();
                PlayerState ps = PlayerState.Run;
                if (BallOwner == Joysticker)
                    ps = PlayerState.RunAndDribble;

                Joysticker.OnJoystickMove(move, ps);
            }
        }
    }
    
    public void OnJoystickMoveEnd(MovingJoystick move)
    {
        if (Joysticker)
        {
            PlayerState ps = PlayerState.Idle;
            if (BallOwner == Joysticker)
                ps = PlayerState.Dribble;
            
            Joysticker.OnJoystickMoveEnd(move, ps);
        }
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
            float Dis = 0;
            float ShootPointDis = 0;
            Vector3 pos = SceneMgr.Get.ShootPoint [Npc.Team.GetHashCode()].transform.position;
            
            if (Npc.Team == TeamKind.Self)
                ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));
            else
                ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));
            
            if (Npc == BallOwner)
            {
                //Dunk shoot shoot3 pass
                int Dir = HaveDefPlayer(ref Npc, 1.5f, 50);
                if (ShootPointDis <= GameConst.DunkDistance && dunkRate < 30 && CheckAttack(ref Npc))
                {
					Shoot();
                } else 
                if (ShootPointDis <= GameConst.TwoPointDistance && (HaveDefPlayer(ref Npc, 1.5f, 40) == 0 || shootRate < 10) && CheckAttack(ref Npc))
                {
                    Shoot();
                } else 
                if (ShootPointDis <= GameConst.TreePointDistance && (HaveDefPlayer(ref Npc, 1.5f, 40) == 0 || shoot3Rate < 3) && CheckAttack(ref Npc))
                {
                    Shoot();
                } else 
                if (passRate < 20 && CoolDownPass == 0 && !IsShooting && !IsDunk)
                {
                    PlayerBehaviour partner = HavePartner(ref Npc, 20, 90);

                    if (partner != null && HaveDefPlayer(ref partner, 1.5f, 40) == 0)
                    {
                        Pass(partner);
                        CoolDownPass = Time.time + 3;
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
                                    CoolDownPass = Time.time + 3;
                                    break;
                                }
                                find++;
                            }
                        }
                    }
                } else 
                if (Dir != 0 && CoolDownCrossover == 0)
                {
                    //Crossover             
                    TMoveData data = new TMoveData(0);
                    if (Dir == 1)
                        data.Target = new Vector2(Npc.transform.position.x - 2, Npc.transform.position.z);
                    else
                        data.Target = new Vector2(Npc.transform.position.x + 2, Npc.transform.position.z);
                    
                    Npc.FirstTargetPos = data;
                    CoolDownCrossover = Time.time + 1.5f;
                }
            } else
            {
                //sup push
                Dis = getDis(ref BallOwner, ref Npc); 
                PlayerBehaviour NearPlayer = HaveNearPlayer(Npc, GameConst.PushPlayerDistance, false);
                
                if (ShootPointDis <= 1.5f && ALLYOOP < 50)
                {
                    //Npc.AniState(PlayerState.Jumper);
                } else 
                if (NearPlayer != null && pushRate < 50)
                {
                    //Push
                    
                } else 
                if (Dis >= 1.5f && Dis <= 3 && supRate < 50)
                {
                    //Sup
                    
                }
            }   
        }
    }

	private void GetStealRate(ref PlayerBehaviour npc, out bool Val1, out bool Val2)
	{
		int r = Mathf.RoundToInt(npc.Attr.Steal - BallOwner.Attr.Control);
		int maxRate = 100;
		int minRate = 10;
		
		if (r > maxRate)
			r = maxRate;
		else if (r < minRate)
			r = minRate;

		int stealRate = Random.Range(0, 100) + 1;
		int AddRate = 0;
		Val1 = false;
        Val2 = false;
		if(SceneMgr.Get.RealBallFX.activeInHierarchy)
			AddRate = 30;

		if (stealRate <= (GameData.AIlevelAy[npc.Attr.AILevel].StealRate + AddRate))
        {
            Val1 = true;

            if (stealRate <= (r + AddRate))
            {
                Val2 = true;
			}else
			if(BallOwner != null && HaveStealPlayer(ref npc, ref BallOwner, GameConst.StealBallDistance, 15) != 0)
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
	
	private void Defend(ref PlayerBehaviour Npc)
	{
		if (BallOwner != null)
		{
			int pushRate = Random.Range(0, 100) + 1;        
            float Dis = 0;

            //steal push Def
            if (!IsShooting)
            {
                if (BallOwner != null)
                {
                    Dis = getDis(ref BallOwner, ref Npc);
                    
                    if (!Npc.CheckAction(ActionFlag.IsSteal))
                    {
                        if (Dis <= GameConst.PushPlayerDistance && pushRate < 50)
                        {
                            
                        } else 
                        if (Dis <= GameConst.StealBallDistance && BallOwner.Invincible == 0 && Npc.CoolDownSteal == 0)
                        {
                            bool IsDoSteal;
                            bool StealSuccess;
							GetStealRate(ref Npc, out IsDoSteal, out StealSuccess);

                            if(IsDoSteal)
                            {
                                if (Npc.AniState(PlayerState.Steal, BallOwner.gameObject.transform.position))
                                {
									if (StealSuccess)                                    
										setDropBall();

									Npc.CoolDownSteal = Time.time + 3;
                                }
                            }
                        }
                    }
                }
            } else
            {
                if (!Npc.IsJump && !IsBlocking)
                {
                    if (Shooter)
                    {
                        Dis = getDis(ref Npc, ref Shooter);
                        if ((Dis <= GameConst.StealBallDistance || Npc.DefPlayer == Shooter) && !IsBlocking && !IsPassing)
                        {
                            //Npc.AniState(PlayerState.Block, Shooter.transform.localPosition);
                        }
                    } else if (BallOwner && !IsPassing)
                    {
                        bool flag = false;
                        if (BallOwner.CheckAction(ActionFlag.IsFakeShoot))
                        {
                            int r = Random.Range(0, 3);
                            if (r <= 0)
                                flag = true;
                        } else if (BallOwner.CheckAction(ActionFlag.IsShoot))
                            flag = true;

                        if (flag)
                        {
                            Dis = getDis(ref Npc, ref BallOwner);
                            if (Dis <= 5)
                            {
                                //Npc.AniState(PlayerState.Block, BallOwner.transform.position);
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
			if (!Npc.CheckAction(ActionFlag.IsRun) && Npc.WaitMoveTime == 0 && Npc.TargetPosNum == 0)
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
		
		if (!Npc.CheckAction(ActionFlag.IsRun) && Npc.WaitMoveTime == 0 && Npc.TargetPosNum == 0)
		{
			if (Npc == BallOwner)
			{
				if (Team == TeamKind.Self)
					data.Target = new Vector2(Npc.transform.position.x, -18);
				else
					data.Target = new Vector2(Npc.transform.position.x, 18);

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
        PlayerBehaviour getball = HaveNearPlayer(player, 10, true);

        if (getball != null)
        {
            Pass(getball);
            CoolDownPass = Time.time + 1;
        } else
        {
            int ran = UnityEngine.Random.Range(0, 2);
            int count = 0;
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList [i].Team == player.Team && PlayerList [i] != player)
                {
                    if (count == ran)
                    {
                        Pass(PlayerList [i]);
                        CoolDownPass = Time.time + 1;
                        break;
                    }

                    count++;
                }
            }
        }

        return true;
    }

    private PlayerBehaviour NearBall(ref PlayerBehaviour Npc)
    {
        PlayerBehaviour NearPlayer = null;

        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerBehaviour Npc1 = PlayerList [i];
            if (Npc1.Team == Npc.Team)
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
                if (NearPlayer == null)
                    NearPlayer = Npc;
                else if (getDis(ref NearPlayer, SceneMgr.Get.RealBall.transform.position) > getDis(ref Npc, SceneMgr.Get.RealBall.transform.position))
                    NearPlayer = Npc;
            }
        }
        
        return NearPlayer;
    }

    private void DefBlock(ref PlayerBehaviour Npc)
    {
        if (PlayerList.Count > 0 && !IsPassing)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerBehaviour Npc2 = PlayerList [i];

                if (Npc2 != Npc && Npc2.Team != Npc.Team)
                {
					if(!IsBlocking)
					{
						if (GameStart.Get.TestMode == GameTest.Block)
							Npc2.AniState(PlayerState.Block, Npc.transform.position);
						else
							if (getDis(ref Npc, ref Npc2) <= GameConst.BlockDistance)
						{
							int Rate = Random.Range(0, 100) + 1;
							if (Npc.Index == Npc2.Index || Rate <= 50)
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

                if (A != null && !A.CheckAction(ActionFlag.IsRun) && A.WaitMoveTime == 0)
                {
                    TMoveData data = new TMoveData(0);
                    data.FollowTarget = SceneMgr.Get.RealBall.transform;
                    A.TargetPos = data;
                } else
                    Npc.rotateTo(SceneMgr.Get.RealBall.transform.position.x, SceneMgr.Get.RealBall.transform.position.z);
            } else if (!Npc.CheckAction(ActionFlag.IsRun) && Npc.WaitMoveTime == 0)
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
            PickBall(ref npc, true);
            PickBall(ref npc.DefPlayer, true);
        } else
        {
	        if (!npc.CheckAction(ActionFlag.IsRun) && npc.WaitMoveTime == 0 && npc.TargetPosNum == 0)
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
            if (!player.DefPlayer.CheckAction(ActionFlag.IsRun) && player.DefPlayer.WaitMoveTime == 0)
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

    public void SetBall(PlayerBehaviour p = null)
    {
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

                BallOwner = p;
                UIGame.Get.ChangeControl(p.Team == TeamKind.Self);
                SceneMgr.Get.SetBallState(PlayerState.Dribble, p);
                p.ClearIsCatcher();

                if (p)
                {
                    p.WaitMoveTime = 0;
                    if (p.IsJump)
                    {
                        //ALLYOOP 
                    } else
                    {
                        p.AniState(PlayerState.Dribble);
                    }

                    for (int i = 0; i < PlayerList.Count; i++)
                        if (PlayerList [i].Team != p.Team)
                            PlayerList [i].ResetMove();
                }

                Shooter = null;
            } else
            {
				if(BallOwner != null)
					BallOwner.AniState(PlayerState.Idle);

                BallOwner = p;
                
                if (p)
                    SceneMgr.Get.SetBallState(PlayerState.Dribble, p);
            }
        }
    }

    public void BallOnFloor()
    {
        SceneMgr.Get.ResetBasketEntra();
        GameController.Get.Shooter = null;
    }

    public void BallTouchPlayer(PlayerBehaviour player, int dir)
    {
        if (BallOwner || (Catcher && Catcher != player) || IsShooting)
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
                    SetBall(player);
                    
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
                        if (Npc != Self && Npc.Team != Self.Team && getDis(ref Self, ref Npc) <= Dis)
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
        if (BallOwner != null)
            BallOwner.DelActionFlag(ActionFlag.IsPass);
		else
		{
			for(int i = 0; i < PlayerList.Count; i++)
			{
				if(PlayerList[i].CheckAction(ActionFlag.IsPass))
				{
					PlayerList[i].DelActionFlag(ActionFlag.IsPass);
					break;
				}
			}
		}

        if (Catcher != null)
        {
            SetBall(Catcher);
            Catcher.DelActionFlag(ActionFlag.IsCatcher);
			if(Catcher.NeedShooting)
			{
				Shoot();
				Catcher.NeedShooting = false;
			}
            Catcher = null;
		}else{
			for(int i = 0; i < PlayerList.Count; i++)
			{
				if(PlayerList[i].CheckAction(ActionFlag.IsCatcher))
				{
					PlayerList[i].DelActionFlag(ActionFlag.IsCatcher);
					break;
				}
			}				
		}
    }

	private void setDropBall(){
		if(IsPassing)
		{
			if (BallOwner != null)
				BallOwner.DelActionFlag(ActionFlag.IsPass);
			else
			{
				for(int i = 0; i < PlayerList.Count; i++)
				{
					if(PlayerList[i].CheckAction(ActionFlag.IsPass))
					{
						PlayerList[i].DelActionFlag(ActionFlag.IsPass);
						break;
					}
				}
			}
			
			if (Catcher != null)
			{
				Catcher.DelActionFlag(ActionFlag.IsCatcher);
				if(Catcher.NeedShooting)
				{
					Shoot();
					Catcher.NeedShooting = false;
				}
				Catcher = null;
			}else{
				for(int i = 0; i < PlayerList.Count; i++)
				{
					if(PlayerList[i].CheckAction(ActionFlag.IsCatcher))
					{
						PlayerList[i].DelActionFlag(ActionFlag.IsCatcher);
						break;
					}
				}				
			}
		}
		
		SetBall(null);
		SceneMgr.Get.SetBallState(PlayerState.Steal);
		Catcher = null;
	}
	
	public void Reset()
	{
		for(int i = 0; i < PlayerList.Count; i++)
			PlayerList[i].transform.position = BornAy[i];
		
		situation = GameSituation.Opening;
		BallOwner = null;
		SceneMgr.Get.RealBall.transform.parent = null;
		SceneMgr.Get.RealBall.transform.localPosition = new Vector3(0, 5, 0);
		SceneMgr.Get.RealBallRigidbody.isKinematic = false;
		SceneMgr.Get.RealBallRigidbody.useGravity = true;
        SceneMgr.Get.RealBallTrigger.SetBoxColliderEnable(true);
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
                if (PlayerList [i].CheckAction(ActionFlag.IsShoot))
                    return true;            

            return false;
        }
    }

    public bool IsDunk
    {
        get
        {
            for (int i = 0; i < PlayerList.Count; i++)            
                if (PlayerList [i].CheckAction(ActionFlag.IsDunk))
                    return true;            
            
            return false;
        }
    }

    public bool IsPassing
    {
        get
        {
            for (int i = 0; i < PlayerList.Count; i++)
                if (PlayerList [i].CheckAction(ActionFlag.IsPass))
                    return true;
            
            return false;
        }
    }

    public bool IsBlocking
    {
        get
        {
            for (int i = 0; i < PlayerList.Count; i++)
                if (PlayerList [i].CheckAction(ActionFlag.IsBlock))
                    return true;
            
            return false;
        }
    }
}
