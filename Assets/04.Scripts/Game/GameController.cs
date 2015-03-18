﻿using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public enum GameSituation{
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

public enum GameAction{
	Def = 0,
	Attack = 1
}

public enum GamePostion{
	C = 0,
	F = 1,
	G = 2
}

public enum GameTest{
	None,
	AttackA,
	AttackB,
	Edit
}

public struct TTactical
{
	public TActionPosition [] PosAy1;
	public TActionPosition [] PosAy2;
	public TActionPosition [] PosAy3;
}

public struct TActionPosition{
	public Vector3 Position;
	public bool Speedup;
}


public class GameController : MonoBehaviour {
	private static GameController instance;
	
	public float PickBallDis = 2.5f;
	private const float StealBallDis = 2;
	private const float PushPlayerDis = 1;
	private const float BlockDis = 5;
	private bool IsStart = true;

	private List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
	private PlayerBehaviour BallOwner;
	private PlayerBehaviour Joysticker;
	public PlayerBehaviour Catcher;
	public PlayerBehaviour Shooter;

	private GameSituation situation = GameSituation.None;
	private float CoolDownPass = 0;
	private float CoolDownCrossover = 0;
	private float ShootDis = 0;

	private Vector2 [] BaseRunAy_A = new Vector2[4];
	private Vector2 [] BaseRunAy_B = new Vector2[11];
	private Vector2 [] BaseRunAy_C = new Vector2[11];
	private Vector2 [] TeePosAy = new Vector2[3];
	private Vector2 [] TeeBackPosAy = new Vector2[3];
	private List<TTactical> MovePositionList = new List<TTactical>();

	public static GameController Get
	{
		get {
			if (!instance) {
				GameObject obj = GameObject.Find("UI2D/UIGame");
				if (!obj) {
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
			if(instance)
				return instance.gameObject.activeInHierarchy;
            else
                return false;
        }
    }

	void Start () {
		InitPos ();
		InitGame ();
	}

	private void InitPos(){
		BaseRunAy_A [0] = new Vector2 (0, 4.5f);
		BaseRunAy_A [1] = new Vector2 (4, 5.9f);
		BaseRunAy_A [2] = new Vector2 (0, 8);
		BaseRunAy_A [3] = new Vector2 (-4.9f, 6.2f);

		BaseRunAy_B [0] = new Vector2 (5.3f, 10);//5.3, 10
		BaseRunAy_B [1] = new Vector2 (1.8f, 13);//1.8, 13
		BaseRunAy_B [2] = new Vector2 (1.8f, 8.9f);//1.8, 8.9
		BaseRunAy_B [3] = new Vector2 (5.3f, 13);//5.3, 13
		BaseRunAy_B [4] = new Vector2 (3, 14);//3, 14
		BaseRunAy_B [5] = new Vector2 (-5.3f, 10);//-5.3, 10
		BaseRunAy_B [6] = new Vector2 (-1.8f, 13);//-1.8, 13
		BaseRunAy_B [7] = new Vector2 (-1.8f, 8.9f);//-1.8, 8.9
		BaseRunAy_B [8] = new Vector2 (-5.3f, 13);//-5.3, 13
		BaseRunAy_B [9] = new Vector2 (-5.3f, 10);//-5.3, 10
		BaseRunAy_B [10] = new Vector2 (-2.6f, 13);//-2.6, 13

		BaseRunAy_C [0] = new Vector2 (-5.3f, 10);//-5.3, 10
		BaseRunAy_C [1] = new Vector2 (-1.8f, 13);//-1.8, 13
		BaseRunAy_C [2] = new Vector2 (-1.8f, 8.9f);//-1.8, 8.9
		BaseRunAy_C [3] = new Vector2 (-5.3f, 13);//-5.3, 13
		BaseRunAy_C [4] = new Vector2 (-5.3f, 10);//-5.3, 10
		BaseRunAy_C [5] = new Vector2 (-2.6f, 13);//-2.6, 13
		BaseRunAy_C [6] = new Vector2 (5.3f, 10);//5.3, 10
		BaseRunAy_C [7] = new Vector2 (1.8f, 13);//1.8, 13
		BaseRunAy_C [8] = new Vector2 (1.8f, 8.9f);//1.8, 8.9
		BaseRunAy_C [9] = new Vector2 (5.3f, 13);//5.3, 13
		BaseRunAy_C [10] = new Vector2 (3, 14);//3, 14

		TeePosAy [0] = new Vector2 (5.6f, -13);
		TeePosAy [1] = new Vector2 (6, -19);
		TeePosAy [2] = new Vector2 (4, 10);

		TeeBackPosAy[0] = new Vector2 (0, 8);
		TeeBackPosAy[1] = new Vector2 (5.3f, 10);
		TeeBackPosAy[2] = new Vector2 (-5.3f, 10);

		try{
			DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/Run");
			FileInfo[] info = dir.GetFiles("*.gangrun");
			if(info.Length > 0){
				MovePositionList.Clear();
				
				foreach (FileInfo f in info){
					string filedata = GameFunction.StringRead(Application.dataPath + "/Resources/Run/" + f.Name);
					TTactical saveData = new TTactical();
					GameFunction.GetJsonData(filedata, ref saveData);
					MovePositionList.Add(saveData);
				}
			}
		}catch{}
	}

	public void InitGame(){
		EffectManager.Get.LoadGameEffect();
		PlayerList.Clear ();
		CreateTeam ();
		BallOwner = null;
		Shooter = null;
		Catcher = null;
		situation = GameSituation.Opening;
	}

	public void ChangePlayer(GameStruct.TAvatar attr, GameStruct.TAvatarTexture attrTexture) {
		PlayerList.Add (ModelManager.Get.CreatePlayer (PlayerList[0].gameObject, attr, attrTexture, 0, TeamKind.Self, new Vector3(0, 0, 0), BaseRunAy_A, MoveType.PingPong, GamePostion.G, false));
		PlayerList.RemoveAt (0);
	}

	private PlayerBehaviour FindDefMen(PlayerBehaviour npc){
		PlayerBehaviour Result = null;
		
		if (npc != null && PlayerList.Count > 1) {
			for(int i = 0; i < PlayerList.Count; i++){
				if(PlayerList[i] != npc && PlayerList[i].Team != npc.Team && PlayerList[i].Postion == npc.Postion){
					Result = PlayerList[i];
					break;
				}
			}
		}
		
		return Result;
	}

	public void ChangeTexture(GameStruct.TAvatar attr, int BodyPart, int ModelPart, int TexturePart) {
		ModelManager.Get.SetAvatarTexture (PlayerList [0].gameObject, attr, BodyPart, ModelPart, TexturePart);
	}

	public void CreateTeam(){
		GameStruct.TAvatar attr = new GameStruct.TAvatar(1);
		GameStruct.TAvatarTexture attrTexture = new GameStruct.TAvatarTexture (1);

		switch (GameStart.Get.TestMode) {				
			case GameTest.None:
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 0, TeamKind.Self, new Vector3(0, 0, 0), BaseRunAy_A, MoveType.PingPong, GamePostion.G, true));
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 1, TeamKind.Self, new Vector3 (5, 0, -2), BaseRunAy_B, MoveType.PingPong, GamePostion.F, true));
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 2, TeamKind.Self, new Vector3 (-5, 0, -2), BaseRunAy_C, MoveType.PingPong, GamePostion.C, true));
				
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 3, TeamKind.Npc, new Vector3 (0, 0, 5), BaseRunAy_A, MoveType.PingPong, GamePostion.G, true));	
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 4, TeamKind.Npc, new Vector3 (5, 0, 2), BaseRunAy_B, MoveType.PingPong, GamePostion.F, true));
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 5, TeamKind.Npc, new Vector3 (-5, 0, 2), BaseRunAy_C, MoveType.PingPong, GamePostion.C, true));

				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].DefPlaeyr = FindDefMen(PlayerList[i]);
				break;
			case GameTest.AttackA:
				attr.Body = 2;
				attr.Hair = 2;
				attr.AHead = 1;
				attr.Cloth = 5;
				attr.Pants = 6;
				attr.Shoes = 1;
				attr.MHandDress = 2;
				attr.ZBackEquip = 1;
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 0, TeamKind.Self, new Vector3(0, 0, 0), BaseRunAy_A, MoveType.PingPong, GamePostion.G, true));
				break;
			case GameTest.AttackB:
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 0, TeamKind.Npc, new Vector3(0, 0, 0), BaseRunAy_A, MoveType.PingPong, GamePostion.G, true));
				break;
			case GameTest.Edit:
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 0, TeamKind.Self, new Vector3(0, 0, 0), BaseRunAy_A, MoveType.PingPong, GamePostion.G, true));
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 1, TeamKind.Self, new Vector3 (5, 0, -2), BaseRunAy_B, MoveType.PingPong, GamePostion.F, true));
				PlayerList.Add (ModelManager.Get.CreatePlayer (null, attr, attrTexture, 2, TeamKind.Self, new Vector3 (-5, 0, -2), BaseRunAy_C, MoveType.PingPong, GamePostion.C, true));
				break;
		}

		Joysticker = PlayerList [0];
		EffectManager.Get.SelectEffectScript.SetTarget(Joysticker.gameObject);

		if(PlayerList[1].Team == Joysticker.Team)
			EffectManager.Get.SelectEffectScriptA.SetTarget(PlayerList[1].gameObject);

		if(PlayerList[2].Team == Joysticker.Team)
			EffectManager.Get.SelectEffectScriptB.SetTarget(PlayerList[2].gameObject);

		for (int i = 0; i < PlayerList.Count; i ++) {
			PlayerList[i].OnShoot = OnShoot;
			PlayerList[i].OnPass = OnPass;
			PlayerList[i].OnBlock = OnBlock;
			PlayerList[i].OnDunkJump = OnDunkJump;
			PlayerList[i].OnDunkBasket = OnDunkBasket;
		}
	}

	void FixedUpdate () {
		if(Time.time >= CoolDownPass)
			CoolDownPass = 0;

		if(Time.time >= CoolDownCrossover)
			CoolDownCrossover = 0;
		
		handleSituation();
	}

	private void handleSituation() {
		if (PlayerList.Count > 0) {
			//Action
			for(int i = 0 ; i < PlayerList.Count; i++){
				PlayerBehaviour Npc = PlayerList[i];
				
				if(Npc.isJoystick && Npc.Team == TeamKind.Self && Npc == Joysticker){
					
				}else{
					if(GameStart.Get.TestMode != GameTest.None)
						return;
					//AI
					switch(situation){
					case GameSituation.None:
						
						break;
					case GameSituation.Opening:
						
						break;
					case GameSituation.AttackA:
						if(Npc.Team == TeamKind.Self){
							if(!IsPassing){
								if(!IsShooting){
									Attack(ref Npc);
									AIMove(ref Npc);
								}else if(!Npc.IsShooting){
									Attack(ref Npc);
									AIMove(ref Npc);
								}								
							}
						}else
							Defend(ref Npc);			
						break;
					case GameSituation.AttackB:
						if(Npc.Team == TeamKind.Self){
							Defend(ref Npc);
						}else{
							if(!IsPassing){
								if(!IsShooting){
									Attack(ref Npc);
									AIMove(ref Npc);
								}else if(!Npc.IsShooting){
									Attack(ref Npc);
									AIMove(ref Npc);
								}
							}
						}
						break;
					case GameSituation.TeeAPicking:
						if(BallOwner == null){
							//Picking ball
							if(Npc.Team == TeamKind.Self && Npc.Postion == GamePostion.F){
								PickBall(ref Npc);								
							}else if(Npc.Team == TeamKind.Self){
								TeeBall(ref Npc, TeamKind.Self);
							}else if(Npc.Team == TeamKind.Npc){
								BackToDef(ref Npc, TeamKind.Npc);
							}
						}
						break;
					case GameSituation.TeeA:
						//Tee ball
						if(!IsPassing)
							TeeBall(ref Npc, TeamKind.Self);
						else if(Npc.Team == TeamKind.Npc)
							BackToDef(ref Npc, Npc.Team);
                            break;	
                	case GameSituation.TeeBPicking:
	                    if(BallOwner == null){
	                        //Pick up ball
	                        if(Npc.Team == TeamKind.Npc && Npc.Postion == GamePostion.F){                                    
									PickBall(ref Npc);	                                    
	                        }else if(Npc.Team == TeamKind.Npc){
	                            TeeBall(ref Npc, TeamKind.Npc);
	                        }else if(Npc.Team == TeamKind.Self){
	                            BackToDef(ref Npc, TeamKind.Self);
	                        }
	                    }
	                    break;
	                case GameSituation.TeeB:
	                    //Tee ball
	                    if(!IsPassing)
	                        TeeBall(ref Npc, TeamKind.Npc);
	                    else if(Npc.Team == TeamKind.Self)
	                        BackToDef(ref Npc, Npc.Team);
	                    break;					
	                case GameSituation.End:
	                    
	                    break;
                    }
                }
            }
        }
    }

	private void Shoot() {
		if (BallOwner) {
			SceneMgr.Get.ResetBasketEntra();
			BallOwner.AniState(PlayerState.Shooting, true, 
			                   SceneMgr.Get.Hood[BallOwner.Team.GetHashCode()].transform.position.x, 
			                   SceneMgr.Get.Hood[BallOwner.Team.GetHashCode()].transform.position.z);
        }
	}
        
    public bool OnShoot(PlayerBehaviour player) {
		if (BallOwner && BallOwner == player) {					
			Shooter = player;
			SetBall();
			SceneMgr.Get.RealBall.transform.localEulerAngles = Vector3.zero;
			SceneMgr.Get.SetBallState(PlayerState.Shooting);
			SceneMgr.Get.RealBall.GetComponent<Rigidbody>().velocity = 
				GameFunction.GetVelocity(SceneMgr.Get.RealBall.transform.position, 
				                         SceneMgr.Get.ShootPoint[player.Team.GetHashCode()].transform.position, 60);

			ShootDis = getDis(ref Shooter, SceneMgr.Get.ShootPoint[Shooter.Team.GetHashCode()].transform.position);
			DefBlock(ref Shooter);
			return true;
		} else
			return false;
	}

	public bool OnDunkInto(PlayerBehaviour player)
	{
		player.OnDunkInto();
		return true;
	}

	public bool OnDunkBasket(PlayerBehaviour player)
	{
		if(player == BallOwner)
		{

			SceneMgr.Get.SetBallState(PlayerState.DunkBasket);
			SetBall();
			return true;
		}
		else
			return false;
	}

	public bool OnDunkJump(PlayerBehaviour player)
	{
		if(player == BallOwner)
		{
			Shooter = player;
		
			return true;
		}
		else
			return false;
	}

	public void DoShoot()
	{
		if (IsStart && Joysticker && Joysticker == BallOwner) {
			Shoot();
			Joysticker.SetNoAiTime();		
		}			
    }
    
    private void Pass(PlayerBehaviour player) {
		if (BallOwner && BallOwner.IsDribble) {
			Catcher = player;
			Catcher.AniState(PlayerState.Catcher, true, BallOwner.transform.position.x, BallOwner.transform.position.z);
			BallOwner.AniState(PlayerState.Pass, true, Catcher.transform.position.x, Catcher.transform.position.z);
		}
	}
    
    public bool OnPass(PlayerBehaviour player) {
		if (Catcher) {
			SceneMgr.Get.SetBallState(PlayerState.Pass);
			SceneMgr.Get.RealBall.GetComponent<Rigidbody>().velocity = GameFunction.GetVelocity(SceneMgr.Get.RealBall.transform.position, Catcher.DummyBall.transform.position, Random.Range(40, 60));	
			if(Vector3.Distance(SceneMgr.Get.RealBall.transform.position, Catcher.DummyBall.transform.position) > 15f)
				CameraMgr.Get.IsLongPass = true;
            
            return true;
        }else
            return false;
    }

	public void DoPass()
	{
		if (IsStart && BallOwner && !Shooter && Joysticker && BallOwner.Team == 0) {
			if(BallOwner == Joysticker)
				Pass(PlayerList[1]);
			else
				Pass(Joysticker);

			Joysticker.SetNoAiTime();
		}
	}

    private void Steal(PlayerBehaviour player) {
        
    }
    
    public bool OnSteal(PlayerBehaviour player) {
		return true;
    }

	public void DoSteal()
	{
		if (IsStart && BallOwner && BallOwner != Joysticker) {
			BallOwner.AniState (PlayerState.Steal, true, BallOwner.transform.position.x, BallOwner.transform.position.z);
			Joysticker.SetNoAiTime ();		
		}			
	}

    private void Block(PlayerBehaviour player) {
		
	}
    
    public bool OnBlock(PlayerBehaviour player) {
		Rigidbody playerrigidbody = player.GetComponent<Rigidbody>();

		if (playerrigidbody != null) {
			if (BallOwner) {
				if (Vector3.Distance(Joysticker.transform.position, BallOwner.transform.position) < 5f)
					playerrigidbody.AddForce (player.JumpHight * transform.up + playerrigidbody.velocity.normalized /2.5f, ForceMode.VelocityChange);
				else
					playerrigidbody.velocity = GameFunction.GetVelocity (player.transform.position, 
					                                                     new Vector3(BallOwner.transform.position.x, 7, BallOwner.transform.position.z), 70);

				return true;
			} else  {
				if (Shooter && Vector3.Distance(player.transform.position, SceneMgr.Get.RealBall.transform.position) < 5)
					playerrigidbody.velocity = GameFunction.GetVelocity (player.transform.position, 
					                                                      new Vector3(SceneMgr.Get.RealBall.transform.position.x, 5, 
					            SceneMgr.Get.RealBall.transform.position.z), 70);
			}		
		}

		return false;
    }

	public void DoBlock()
	{
		if (IsStart && BallOwner) {
			BallOwner.AniState (PlayerState.Block, true, BallOwner.transform.position.x, BallOwner.transform.position.z);
			Joysticker.SetNoAiTime ();		
		}			
	}

    private void Rebound(PlayerBehaviour player) {
        
    }
	
	public bool OnRebound(PlayerBehaviour player) {
		return true;
    }

    public void DoSkill()
    {
		Joysticker.AniState (PlayerState.Dunk);
		Joysticker.SetNoAiTime ();
    }

	private bool CanMove
	{
		get{
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
		if (Joysticker && CanMove) {
			if (Mathf.Abs (move.joystickAxis.y) > 0 || Mathf.Abs (move.joystickAxis.x) > 0)
			{
				Joysticker.ClearMoveQueue();
				PlayerState ps = PlayerState.Run;
				if (BallOwner == Joysticker)
					ps = PlayerState.RunAndDrible;

				Joysticker.OnJoystickMove(move, ps);
            }
        }
    }
    
    public void OnJoystickMoveEnd(MovingJoystick move)
    {
		if (Joysticker) {
			PlayerState ps = PlayerState.Idle;
			if (BallOwner == Joysticker)
				ps = PlayerState.Dribble;
			
			Joysticker.OnJoystickMoveEnd(move, ps);
        }
	}

	private void Attack(ref PlayerBehaviour Npc){
		if (BallOwner != null) {
			int dunkRate = Random.Range(0, 100) + 1;
			int shootRate = Random.Range(0, 100) + 1;
			int shoot3Rate = Random.Range(0, 100) + 1;
			int passRate = Random.Range(0, 100) + 1;
			int pushRate = Random.Range(0, 100) + 1;
			int supRate = Random.Range(0, 100) + 1;
			int ALLYOOP = Random.Range(0, 100) + 1;
			float Dis = 0;
			float ShootPointDis = 0;
			Vector3 pos = SceneMgr.Get.ShootPoint[Npc.Team.GetHashCode()].transform.position;
			
			if(Npc.Team == TeamKind.Self)
				ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));
			else
				ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));
			
			if(Npc == BallOwner){
				//Dunk shoot shoot3 pass
				int Dir = HaveDefPlayer(ref Npc, 1.5f, 50);
				if(ShootPointDis <= 2f && dunkRate < 0 && CheckAttack(ref Npc)){
					Shoot();
				}else if(ShootPointDis <= 6f && (HaveDefPlayer(ref Npc, 1.5f, 40) == 0 || shootRate < 10) && CheckAttack(ref Npc)){
					Shoot();
				}else if(ShootPointDis <= 10.5f && (HaveDefPlayer(ref Npc, 1.5f, 40) == 0 || shoot3Rate < 3) && CheckAttack(ref Npc)){
					Shoot();
				}else if(passRate < 0 && CoolDownPass == 0){
					PlayerBehaviour partner = HavePartner(ref Npc, 20, 90);

					if(partner != null && HaveDefPlayer(ref partner, 1.5f, 40) == 0){
						Pass(partner);
						CoolDownPass = Time.time + 3;
					}else{
						int Who = Random.Range(0, 2);
						int find = 0;

						for(int j = 0;  j < PlayerList.Count; j++){
							if(PlayerList[j].Team == Npc.Team && PlayerList[j] != Npc){
								PlayerBehaviour anpc = PlayerList[j];
								
								if(HaveDefPlayer(ref anpc, 1.5f, 40) == 0 || Who == find){
									Pass(PlayerList[j]);
									CoolDownPass = Time.time + 3;
									break;
								}
								find++;
							}
						}
					}
				}else if(Dir != 0 && CoolDownCrossover == 0){
					//Crossover				
					TMoveData data = new TMoveData(0);
					if(Dir == 1)
						data.Target = new Vector2(Npc.transform.position.x - 2, Npc.transform.position.z);
					else
						data.Target = new Vector2(Npc.transform.position.x + 2, Npc.transform.position.z);
					
					Npc.FirstTargetPos = data;
					CoolDownCrossover = Time.time + 1.5f;
				}
			}else{
				//sup push
				Dis = getDis(ref BallOwner, ref Npc); 
				PlayerBehaviour NearPlayer = HaveNearPlayer(Npc, PushPlayerDis, false);
				
				if(ShootPointDis <= 1.5f && ALLYOOP < 50){
					//Npc.AniState(PlayerState.Jumper);
				}else if(NearPlayer != null && pushRate < 50){
					//Push
					
				}else if(Dis >= 1.5f && Dis <= 3 && supRate < 50){
					//Sup
					
				}
			}	
		}
	}
	
	private void Defend(ref PlayerBehaviour Npc){
		if (BallOwner != null) {
			int stealRate = Random.Range(0, 100) + 1;
			int pushRate = Random.Range(0, 100) + 1;		
			float Dis = 0;

			//steal push Def
			if(!IsShooting){
				if(BallOwner != null){
					Dis = getDis(ref BallOwner, ref Npc);
					
					if(!Npc.IsSteal){
						if(Dis <= PushPlayerDis && pushRate < 50){
							
						}else if(Dis <= StealBallDis && stealRate < 0 && BallOwner.Invincible == 0 && Npc.CoolDownSteal == 0){
							Npc.CoolDownSteal = Time.time + 3;
							Npc.AniState(PlayerState.Steal, true, BallOwner.gameObject.transform.localPosition.x, BallOwner.gameObject.transform.localPosition.z);
//							if(stealRate < 5){
//								SetBall(Npc);
//								Npc.SetInvincible(7);
//							}
						}
					}
				}
			}else{
				if(Shooter && !Npc.IsJump && !Npc.IsBlock){
					Dis = getDis(ref Npc, ref Shooter);
					if(Dis <= StealBallDis){
						Npc.AniState(PlayerState.Block, true, Shooter.transform.localPosition.x, Shooter.transform.localPosition.z);
					}
				}
			}				
		}
	}

	private Vector2 GetTarget(Vector2 A, Vector2 B){
		return new Vector2 ((A.x + B.x) / 2, (A.y + B.y) / 2);
	}

	private void BackToDef(ref PlayerBehaviour Npc, TeamKind Team, bool WatchBallOwner = false){
		if(!Npc.IsMove && Npc.WaitMoveTime == 0){
			TMoveData data = new TMoveData(0);
			if(Team == TeamKind.Self)
				data.Target = new Vector2(TeeBackPosAy[Npc.Postion.GetHashCode()].x, -TeeBackPosAy[Npc.Postion.GetHashCode()].y);
			else
				data.Target = TeeBackPosAy[Npc.Postion.GetHashCode()];

			if(BallOwner != null)
				data.LookTarget = BallOwner.transform;
			else{
				if(Team == TeamKind.Self)
					data.LookTarget = SceneMgr.Get.Hood[1].transform;
				else
					data.LookTarget = SceneMgr.Get.Hood[0].transform;
			}

			if(!WatchBallOwner)
				data.Speedup = true;
			Npc.TargetPos = data;
		}
	}

	private void TeeBall(ref PlayerBehaviour Npc, TeamKind Team){
		if(Npc.Team == Team){
			TMoveData data = new TMoveData(0);
			if(!Npc.IsMove && Npc.WaitMoveTime == 0){
				if(Team == TeamKind.Self)
					data.Target = TeePosAy[Npc.Postion.GetHashCode()];
				else
					data.Target = new Vector2(-TeePosAy[Npc.Postion.GetHashCode()].x, -TeePosAy[Npc.Postion.GetHashCode()].y);

				if(Npc.Postion == GamePostion.F)
					data.MoveFinish = NpcAutoTee;
				Npc.TargetPos = data;
			}
			
			if (Npc.WaitMoveTime != 0 && Npc == BallOwner)
				Npc.AniState(PlayerState.Dribble);
		}else{
			BackToDef(ref Npc, Npc.Team);
		}
	}

	private bool NpcAutoTee(PlayerBehaviour player, bool speedup){
		TeamKind Team = TeamKind.Self;
		if(situation == GameSituation.TeeB || situation == GameSituation.TeeBPicking)
			Team = TeamKind.Npc;

		for(int i = 0; i < PlayerList.Count; i++){
			if(PlayerList[i].Team == Team && PlayerList[i].Postion == GamePostion.G && BallOwner != PlayerList[i]){
				Pass(PlayerList[i]);
				CoolDownPass = Time.time + 1;
				break;
			}
		}

		return true;
	}

	private PlayerBehaviour NearBall(ref PlayerBehaviour Npc){
		PlayerBehaviour NearPlayer = null;

		for (int i = 0; i < PlayerList.Count; i++) {
			PlayerBehaviour Npc1 = PlayerList[i];
			if(Npc1.Team == Npc.Team){
				if(NearPlayer == null)
					NearPlayer = Npc1;
				else if(getDis(ref NearPlayer, SceneMgr.Get.RealBall.transform.position) > getDis(ref Npc1, SceneMgr.Get.RealBall.transform.position))
					NearPlayer = Npc1;
			}
		}

		if(Npc != NearPlayer)
			NearPlayer = null;

		return NearPlayer;
	}

	private void DefBlock(ref PlayerBehaviour Npc){
		if (PlayerList.Count > 0) {
			for(int i = 0; i < PlayerList.Count; i++){
				PlayerBehaviour Npc2 = PlayerList[i];

				if (Npc2 != Npc && Npc2.Team != Npc.Team && !Npc2.IsBlock) {
					if (getDis(ref Npc, ref Npc2) <= BlockDis) {
						int Rate = Random.Range(0, 100) + 1;
						if(Npc.Postion == Npc2.Postion || Rate <= 50){
							Npc2.AniState(PlayerState.Block, true, Npc.transform.position.x, Npc.transform.position.z);
						}
					}
				}
			}
		}
	}

	private PlayerBehaviour PickBall(ref PlayerBehaviour Npc, bool findNear = false){
		PlayerBehaviour A = null;

		if (BallOwner == null) {
			if(findNear){
				A = NearBall(ref Npc);

				if(A != null && !A.IsMove && A.WaitMoveTime == 0){
					TMoveData data = new TMoveData(0);
					data.FollowTarget = SceneMgr.Get.RealBall.transform;
					A.TargetPos = data;
				}else
					Npc.rotateTo(SceneMgr.Get.RealBall.transform.position.x, SceneMgr.Get.RealBall.transform.position.z);
			}else if(!Npc.IsMove && Npc.WaitMoveTime == 0){
				TMoveData data = new TMoveData(0);
				data.FollowTarget = SceneMgr.Get.RealBall.transform;
				Npc.TargetPos = data;
			}
		}

		return A;
	}

	private void AIMove(ref PlayerBehaviour Npc){
		if (BallOwner == null) {
			PickBall(ref Npc, true);
		}else{
			if(!Npc.IsMove && Npc.WaitMoveTime == 0 && Npc.TargetPosNum == 0){
				TMoveData data;

				data = new TMoveData(0);
				if(!CheckAttack(ref Npc)){
					if(Npc.Team == TeamKind.Self)
						data.Target = new Vector2(Npc.transform.position.x, 14);
					else
						data.Target = new Vector2(Npc.transform.position.x, -14);
					
					if(BallOwner != null && BallOwner != Npc)
						data.LookTarget = BallOwner.transform;	
					
					data.MoveFinish = DefMove;
					Npc.FirstTargetPos = data;
					DefMove(Npc);
				}

				if(MovePositionList.Count > 0){
					int Rate = Random.Range(0, MovePositionList.Count);
					Vector3 [] MoveAy = new Vector3[0];

					switch(Npc.Postion){
					case GamePostion.G:
						if(MovePositionList[Rate].PosAy1.Length > 0){
							MoveAy = new Vector3[MovePositionList[Rate].PosAy1.Length];
							for(int i = 0; i < MoveAy.Length; i++)
								MoveAy[i] = MovePositionList[Rate].PosAy1[i].Position;
						}
						break;
					case GamePostion.F:
						if(MovePositionList[Rate].PosAy2.Length > 0){
							MoveAy = new Vector3[MovePositionList[Rate].PosAy2.Length];
							for(int i = 0; i < MoveAy.Length; i++)
								MoveAy[i] = MovePositionList[Rate].PosAy2[i].Position;
						}
						break;
					case GamePostion.C:
						if(MovePositionList[Rate].PosAy3.Length > 0){
							MoveAy = new Vector3[MovePositionList[Rate].PosAy3.Length];
							for(int i = 0; i < MoveAy.Length; i++)
								MoveAy[i] = MovePositionList[Rate].PosAy3[i].Position;
						}
						break;
					}

					if(MoveAy.Length > 0){
						for(int i = 0; i < MoveAy.Length; i++){
							data = new TMoveData(0);
							if(Npc.Team == TeamKind.Self)
								data.Target = new Vector2(MoveAy[i].x, MoveAy[i].z);
							else
								data.Target = new Vector2(MoveAy[i].x, -MoveAy[i].z);
							
							if(BallOwner != null && BallOwner != Npc)
								data.LookTarget = BallOwner.transform;	
							
							data.MoveFinish = DefMove;
							Npc.TargetPos = data;		
						}
					}
				}
			}
				
			if(Npc.WaitMoveTime != 0 && BallOwner != null && Npc == BallOwner)
				Npc.AniState(PlayerState.Dribble);
		}
	}

	public bool DefMove(PlayerBehaviour player, bool speedup = false){
		for(int i = 0 ; i < PlayerList.Count; i++){
			if(player.Team != PlayerList[i].Team && player.Postion == PlayerList[i].Postion){
				if(!PlayerList[i].IsMove && PlayerList[i].WaitMoveTime == 0){
					PlayerBehaviour Npc2 = PlayerList[i];
					if(BallOwner != null){
						float dis = getDis(ref player, SceneMgr.Get.Hood[player.Team.GetHashCode()].transform.position);
						TMoveData data2 = new TMoveData(0);

						if(player == BallOwner){
							if(dis > 19 && Npc2.AutoFollow == false){// || !(player.IsBallOwner && Npc2.Postion == player.Postion
								Npc2.ResetMove();
								BackToDef(ref Npc2, Npc2.Team, true);
								Npc2.SetAutoFollowTime();
							}else{
								data2.DefPlayer = player;

								if(BallOwner != null)
									data2.LookTarget = BallOwner.transform;
								else
									data2.LookTarget = player.transform;

								data2.Speedup = speedup;
								Npc2.TargetPos = data2;
							}
						}else{
							float dis2;
							if(Npc2.Team == TeamKind.Self)
								dis2 = Vector2.Distance(new Vector2(TeeBackPosAy[Npc2.Postion.GetHashCode()].x, -TeeBackPosAy[Npc2.Postion.GetHashCode()].y), 
								                        new Vector2(Npc2.transform.position.x, Npc2.transform.position.z));
							else
								dis2 = Vector2.Distance(TeeBackPosAy[Npc2.Postion.GetHashCode()], 
								                        new Vector2(Npc2.transform.position.x, Npc2.transform.position.z));
		
							if(dis2 <= ParameterConst.AIlevelAy[Npc2.AILevel].DefDistance){
								PlayerBehaviour p = HaveNearPlayer(Npc2, ParameterConst.AIlevelAy[Npc2.AILevel].DefDistance, false, true);
								if(p != null)
									data2.DefPlayer = p;
								else if(getDis(ref player, ref Npc2) <= ParameterConst.AIlevelAy[Npc2.AILevel].DefDistance)
									data2.DefPlayer = player;

								if(data2.DefPlayer != null){
									if(BallOwner != null)
										data2.LookTarget = BallOwner.transform;
									else
										data2.LookTarget = player.transform;

									data2.Speedup = speedup;
									Npc2.TargetPos = data2;
								}
							}else{
								Npc2.ResetMove();
								BackToDef(ref Npc2, Npc2.Team, true);
							}
						}
					}else{
						PickBall(ref Npc2);
					}
					break;
				}
			}
		}
		return true;
	}

	private float getDis(ref PlayerBehaviour player1, ref PlayerBehaviour player2){
		if (player1 != null && player2 != null && player1 != player2){
			Vector3 V1 = player1.transform.position;
			Vector3 V2 = player2.transform.position;
			V1.y = V2.y;
			return Vector3.Distance(V1, V2);
		} else
			return -1;
	}

	private float getDis(ref PlayerBehaviour player1, Vector3 Target){
		if (player1 != null && Target != Vector3.zero){
			Vector3 V1 = player1.transform.position;
			return Vector3.Distance(V1, Target);
		} else
			return -1;
	}

	private float getDis(ref PlayerBehaviour player1, Vector2 Target){
		if (player1 != null && Target != Vector2.zero){
			Vector3 V1 = new Vector3(Target.x, 0, Target.y);
			Vector3 V2 = player1.transform.position;
			V1.y = V2.y;
			return Vector3.Distance(V1, V2);
		} else
			return -1;
	}

	public void SetBall(PlayerBehaviour p = null){
		if (PlayerList.Count > 0) {
			if(p != null && situation != GameSituation.End){
				if(BallOwner != null){
					if(BallOwner.Team != p.Team){
						if(situation == GameSituation.AttackA)
							ChangeSituation(GameSituation.AttackB);
						else if(situation == GameSituation.AttackB)
							ChangeSituation(GameSituation.AttackA);
					}else{
						if(situation == GameSituation.TeeA)
							ChangeSituation(GameSituation.AttackA);
						else if(situation == GameSituation.TeeB)
							ChangeSituation(GameSituation.AttackB);
						else
							BallOwner.ResetFlag();
					}
				}else{
					if(situation == GameSituation.TeeAPicking){
						ChangeSituation(GameSituation.TeeA);
					}else if(situation == GameSituation.TeeBPicking){
						ChangeSituation(GameSituation.TeeB);
					}else{
						if(p.Team == TeamKind.Self)
							ChangeSituation(GameSituation.AttackA);
						else if(p.Team == TeamKind.Npc)
							ChangeSituation(GameSituation.AttackB);
					}
				}

				PlayerBehaviour oldp = BallOwner;
				BallOwner = p;
				SceneMgr.Get.SetBallState(PlayerState.Dribble, p);
				p.ClearIsCatcher();

				if(p){
					p.WaitMoveTime = 0;
					if(p.IsJump){
						//ALLYOOP 
					}else{
						p.AniState (PlayerState.Dribble);
						p.MoveKind = MoveType.PingPong;
						if(oldp){
							int ran = UnityEngine.Random.Range(0, 3);
							if(ran == MoveType.PingPong.GetHashCode())
								oldp.MoveKind = MoveType.PingPong;
							else if(ran == MoveType.Cycle.GetHashCode())
								oldp.MoveKind = MoveType.Cycle;
							else if(ran == MoveType.Random.GetHashCode())
								oldp.MoveKind = MoveType.Random;
						}
					}

					for(int i = 0 ; i < PlayerList.Count; i++)
						if(PlayerList[i].Team != p.Team)
							PlayerList[i].ResetMove();
				}

				Shooter = null;
			}else{
				BallOwner = p;
				
				if(p)
					SceneMgr.Get.SetBallState(PlayerState.Dribble, p);
			}
		}
    }

	public void BallOnFloor() {
		SceneMgr.Get.ResetBasketEntra();
		GameController.Get.Shooter = null;
	}

	public void BallTouchPlayer(PlayerBehaviour player, int dir) {
		if (BallOwner || (Catcher && Catcher != player) || IsShooting)
			return;

		//rebound
		if (dir == 0) {
		} else {
			bool CanSetball = false;

			if (player && (player.IsCatcher || player.CanMove)) {
				if(situation == GameSituation.TeeAPicking){
					if(player.Team == TeamKind.Self && player.Postion == GamePostion.F)
						CanSetball = true;
				}else if(situation == GameSituation.TeeBPicking){
					if(player.Team == TeamKind.Npc && player.Postion == GamePostion.F)
						CanSetball = true;
				}else{
					CanSetball = true;
				}

				if(CanSetball){
					SetBall(player);
					
					switch (dir) {
					case 0: //top
						break;
					case 1: //FR
						break;
					}
				}
			}
		}
	}

	public void PlayerTouchPlayer(PlayerBehaviour player1, PlayerBehaviour player2, int dir) {
		switch (dir) {
		case 0: //top
			break;
		case 1: //FR
			break;
		}
	}

	public void PlayerTouchPlayer(GameObject player) {
		
	}

	private Vector2 SetMovePos(ref PlayerBehaviour Npc){
		Vector2 Result = Vector2.zero;
		int Index = 0;
		switch(Npc.MoveKind){
		case MoveType.PingPong:
			//0=>1=>2=>3=>2=>1=>0
			Npc.MoveIndex++;
			if(Npc.MoveIndex >= (Npc.RunPosAy.Length * 2) - 2)
				Npc.MoveIndex = 0;

			if(Npc.MoveIndex >= Npc.RunPosAy.Length){
				Index = (Npc.RunPosAy.Length - 1)* 2 - Npc.MoveIndex;
				if(Npc.Team == TeamKind.Self)
					Result = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
				else
					Result = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			}else{
				Index = Npc.MoveIndex;
				if(Npc.Team == TeamKind.Self)
					Result = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
				else
					Result = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			}
			break;
		case MoveType.Cycle:
			//0=>1=>2=>3=>0=>1=>2=>3
			Npc.MoveIndex++;
			if(Npc.MoveIndex >= Npc.RunPosAy.Length)
				Npc.MoveIndex = 0;

			Index = Npc.MoveIndex;
			if(Npc.Team == TeamKind.Self)
				Result = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
			else
				Result = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			break;
		case MoveType.Random:
			Index = Random.Range(0, Npc.RunPosAy.Length);
			if(Npc.Team == TeamKind.Self)
				Result = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
			else
				Result = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			break;
		case MoveType.Once:
			//0=>1=>2=>3
			if(Npc.MoveIndex < Npc.RunPosAy.Length){
				Npc.MoveIndex++;
				Index = Npc.MoveIndex;
				if(Npc.Team == TeamKind.Self)
					Result = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
				else
					Result = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			}else{
				Index = Npc.RunPosAy.Length - 1;
				if(Npc.Team == TeamKind.Self)
					Result = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
				else
					Result = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			}
			break;
		case MoveType.Idle:
			Result = new Vector2(Npc.transform.localPosition.x, Npc.transform.localPosition.z);
			break;
		}

		return Result;
	}

	private void GameResult(int team) {
		if(team == 0)
			UIHint.Get.ShowHint("You Win", Color.blue);
		else
			UIHint.Get.ShowHint("You Lost", Color.red);
		
		GameController.Get.ChangeSituation(GameSituation.End);
		UIGame.Get.Again.SetActive (true);
    }
    
    public void PlusScore(int team)
	{
		if (IsStart && GameStart.Get.TestMode == GameTest.None) {
			SceneMgr.Get.ResetBasketEntra();

			int score = 2;
			if(ShootDis != 0){
				if(ShootDis >= 10)
					score = 3;
			}else if(Shooter != null){
				if(getDis(ref Shooter, SceneMgr.Get.ShootPoint[Shooter.Team.GetHashCode()].transform.position) >= 10)
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

	public void ChangeSituation(GameSituation GS){
		if (situation != GameSituation.End) {
			situation = GS;

			for(int i = 0; i < PlayerList.Count; i++){
				PlayerList[i].ResetFlag();
				PlayerList[i].situation = GS;
			}				

			switch(GS){
			case GameSituation.Opening:
				
				break;
			case GameSituation.JumpBall:
				
				break;
			case GameSituation.AttackA:
				UIGame.Get.ChangeControl(true);
				CameraMgr.Get.SetTeamCamera(TeamKind.Self);
				break;
			case GameSituation.AttackB:
				UIGame.Get.ChangeControl(false);
				CameraMgr.Get.SetTeamCamera(TeamKind.Npc);
				break;
			case GameSituation.TeeAPicking:
				CameraMgr.Get.SetTeamCamera(TeamKind.Self);
				break;
			case GameSituation.TeeA:
				break;
			case GameSituation.TeeBPicking:
				CameraMgr.Get.SetTeamCamera(TeamKind.Npc);
				break;
			case GameSituation.TeeB:
				break;
			case GameSituation.End:
				break;
			}		
		}
	}

	public PlayerBehaviour HavePartner(ref PlayerBehaviour Npc, float dis, float angle){
		PlayerBehaviour Result = null;
		Vector3 lookAtPos;
		Vector3 relative;
		float mangle;
		
		if (PlayerList.Count > 0) {
			for (int i = 0; i < PlayerList.Count; i++) {
				if(PlayerList[i] != Npc && PlayerList[i].Team == Npc.Team){
					PlayerBehaviour TargetNpc = PlayerList[i];
					lookAtPos = TargetNpc.transform.position;
					relative = Npc.transform.InverseTransformPoint(lookAtPos);
					mangle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
					
					if(getDis(ref Npc, ref TargetNpc) <= dis){
						if(mangle >= 0 && mangle <= angle){
							Result = TargetNpc;
							break;
						}else if(mangle <= 0 && mangle >= -angle){
							Result = TargetNpc;
							break;
						}
					}
				}						
			}	
		}
		
		return Result;
	}


	public int HaveDefPlayer(ref PlayerBehaviour Npc, float dis, float angle){
		int Result = 0;
		Vector3 lookAtPos;
		Vector3 relative;
		float mangle;
		
		if (PlayerList.Count > 0) {
			for (int i = 0; i < PlayerList.Count; i++) {
				if(PlayerList[i].Team != Npc.Team){
					PlayerBehaviour TargetNpc = PlayerList[i];
					lookAtPos = TargetNpc.transform.position;
					relative = Npc.transform.InverseTransformPoint(lookAtPos);
					mangle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
					
					if(getDis(ref Npc, ref TargetNpc) <= dis){
						if(mangle >= 0 && mangle <= angle){
							Result = 1;
							break;
						}else if(mangle <= 0 && mangle >= -angle){
							Result = 2;
							break;
						}
					}
				}						
			}	
		}
		
		return Result;
	}
	
	private PlayerBehaviour HaveNearPlayer(PlayerBehaviour Self, float Dis, bool SameTeam, bool FindBallOwnerFirst = false){
		PlayerBehaviour Result = null;
		PlayerBehaviour Npc = null;
				
		if (PlayerList.Count > 1) {
				for(int i = 0 ; i < PlayerList.Count; i++){
				Npc = PlayerList[i];
				
				if(SameTeam){
					if(PlayerList[i] != Self && PlayerList[i].Team == Self.Team && getDis(ref Self, ref Npc) <= Dis){
						Result = Npc;
						break;
					}
				}else{
					if(FindBallOwnerFirst){
						if(Npc != Self && Npc.Team != Self.Team && Npc == BallOwner && getDis(ref Self, ref Npc) <= Dis){
							Result = Npc;
							break;
						}
					}else{
						if(Npc != Self && Npc.Team != Self.Team && getDis(ref Self, ref Npc) <= Dis){
							Result = Npc;
							break;
						}
					}
				}
			}
        }
        
        return Result;
    }

	public bool IsShooting {
		get{
			for(int i = 0; i < PlayerList.Count; i++){
				if(PlayerList[i].IsShooting)
					return true;
			}

			return false;
		}
	}

	public bool IsPassing {
		get{
			for(int i = 0; i < PlayerList.Count; i++){
				if(PlayerList[i].IsPass)
					return true;
			}
			
			return false;
		}
	}

	private bool CheckAttack(ref PlayerBehaviour Npc){
		if(Npc.Team == TeamKind.Self && Npc.transform.position.z > 16.4)
			return false;
		else if(Npc.Team == TeamKind.Npc && Npc.transform.position.z < -16.4)
			return false;
		else
			return true;
	}

	//Temp
	public Vector3 EditGetPosition(int index){
		if (PlayerList.Count > index) {
			return PlayerList[index].transform.position;		
		}else
			return Vector3.zero;
	}
	
	public void EditSetMove(TActionPosition ActionPosition, int index){
		if (PlayerList.Count > index) {
			TMoveData data = new TMoveData(0);
			data.Target = new Vector2(ActionPosition.Position.x, ActionPosition.Position.z);
			data.Speedup = ActionPosition.Speedup;
			PlayerList[index].TargetPos = data;
		}
	}

	public void EditSetJoysticker(int index){
		if (PlayerList.Count > index) {
			Joysticker = PlayerList[index];		
		}
	}

	public void Reset(){
		PlayerList [0].transform.position = new Vector3 (0, 0, 0);
		PlayerList [1].transform.position = new Vector3 (5, 0, -2);
		PlayerList [2].transform.position = new Vector3 (-5, 0, -2);
		
		PlayerList [3].transform.position = new Vector3 (0, 0, 5);
		PlayerList [4].transform.position = new Vector3 (5, 0, 2);
		PlayerList [5].transform.position = new Vector3 (-5, 0, 2);
		
		situation = GameSituation.Opening;
		BallOwner = null;
		SceneMgr.Get.RealBall.transform.parent = null;
		SceneMgr.Get.RealBall.transform.localPosition = new Vector3 (0, 5, 0);
		SceneMgr.Get.RealBall.GetComponent<Rigidbody>().isKinematic = false;
		SceneMgr.Get.RealBall.GetComponent<Rigidbody>().useGravity = true;
		SceneMgr.Get.RealBallTrigger.SetBoxColliderEnable(true);
	}
}
