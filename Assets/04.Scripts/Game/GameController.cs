using UnityEngine;
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
	PG = 0,
	PF = 1,
	C = 2
}

public class GameController : MonoBehaviour {
	private static GameController instance;

	private const int CountBackSecs = 4;
	public float PickBallDis = 2.5f;
	private const float StealBallDis = 2;
	private const float PushPlayerDis = 1;
	private const float NearEnemyDis = 2;
	
	public bool IsStart = true;
	public bool ShootInto0 = false;
	public bool ShootInto1 = false;

	public List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
	public PlayerBehaviour BallController;
	public PlayerBehaviour Catcher;
	public PlayerBehaviour ShootController;
	public PlayerBehaviour JoystickController;

	public GameSituation situation = GameSituation.None;
	private float Timer = 0;
	private float CoolDownPass = 0;
	private int NoAiTime = 0;

	public Vector2 [] BaseRunAy_A = new Vector2[4];
	public Vector2 [] BaseRunAy_B = new Vector2[11];
	public Vector2 [] BaseRunAy_C = new Vector2[11];
	public Vector2 [] TeePosAy = new Vector2[3];
	public Vector2 [] TeeBackPosAy = new Vector2[3];

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


	void Start () {
		EasyTouch.On_TouchDown += TouchDown;
		NoAiTime = 0;
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
	}

	private void TouchDown (Gesture gesture){
		if(UIGame.Get.Joystick.Visible && (situation == GameSituation.AttackA || situation == GameSituation.AttackB || situation == GameSituation.Opening))
			NoAiTime = CountBackSecs;
	}

	public void InitGame(){
		EffectManager.Get.LoadGameEffect();
		PlayerList.Clear ();
		CreateTeam ();
		BallController = null;
		ShootController = null;
		Catcher = null;
		NoAiTime = 0;
		situation = GameSituation.Opening;
	}

	public void CreateTeam(){
		PlayerList.Add (ModelManager.Get.CreatePlayer (0, TeamKind.Self, new Vector3(0, 0, 0), BaseRunAy_A, MoveType.PingPong, GamePostion.PG));
		PlayerList.Add (ModelManager.Get.CreatePlayer (1, TeamKind.Self, new Vector3(5, 0, -2), BaseRunAy_B, MoveType.PingPong, GamePostion.PF));
		PlayerList.Add (ModelManager.Get.CreatePlayer (2, TeamKind.Self, new Vector3(-5, 0, -2), BaseRunAy_C, MoveType.PingPong, GamePostion.C));

		PlayerList.Add (ModelManager.Get.CreatePlayer (3, TeamKind.Npc, new Vector3(0, 0, 5), BaseRunAy_A, MoveType.PingPong, GamePostion.PG));	
		PlayerList.Add (ModelManager.Get.CreatePlayer (4, TeamKind.Npc, new Vector3(5, 0, 2), BaseRunAy_B, MoveType.PingPong, GamePostion.PF));
		PlayerList.Add (ModelManager.Get.CreatePlayer (5, TeamKind.Npc, new Vector3(-5, 0, 2), BaseRunAy_C, MoveType.PingPong, GamePostion.C));

		JoystickController = PlayerList [0];
		EffectManager.Get.SelectEffectScript.SetTarget(JoystickController.gameObject);

		for (int i = 0; i < PlayerList.Count; i ++) {
			PlayerList[i].OnShoot = OnShoot;
			PlayerList[i].OnBlock = OnBlock;
		}
	}

	void FixedUpdate () {
		if (Time.time - Timer >= 1){
			Timer = Time.time;
			
			if(NoAiTime > 0){
				NoAiTime--;
				if(NoAiTime <= 0)
					EffectManager.Get.SelectEffectScript.SetParticleColor(true);
			}
		}
		
		if(Time.time >= CoolDownPass)
			CoolDownPass = 0;
		
		handleSituation();
	}

	private void handleSituation() {
		if (PlayerList.Count > 0) {
			//Action
			for(int i = 0 ; i < PlayerList.Count; i++){
				PlayerBehaviour Npc = PlayerList[i];
				
				if(NoAiTime > 0 && Npc.Team == TeamKind.Self && Npc == JoystickController){
					if(!IsPassing && (situation == GameSituation.AttackA || 
					                  situation == GameSituation.AttackB || 
					                  situation == GameSituation.Opening || 
					                  situation == GameSituation.TeeAPicking))
						if(SceneMgr.Get.RealBall.transform.position.y <= 0.5f && BallController == null && getDis(ref Npc, SceneMgr.Get.RealBall.transform.position) <= PickBallDis)
							SetBall(Npc);
				}else{
					//AI
					switch(situation){
					case GameSituation.None:
						
						break;
					case GameSituation.Opening:
						if(SceneMgr.Get.RealBall.transform.position.y <= 0.5f && !IsPassing && BallController == null && getDis(ref Npc, SceneMgr.Get.RealBall.transform.position) <= PickBallDis)
							SetBall(Npc);
						break;
					case GameSituation.AttackA:
						if(Npc.Team == TeamKind.Self){
							if(!IsPassing){
								if(!IsShooting){
									AttackAndDef(ref Npc, GameAction.Attack);
									AIMove(ref Npc, GameAction.Attack);
								}else{
									if(!Npc.IsShooting){
										AttackAndDef(ref Npc, GameAction.Attack);
										AIMove(ref Npc, GameAction.Attack);
									}
								}
							}
						}else{
							AttackAndDef(ref Npc, GameAction.Def);
							AIMove(ref Npc, GameAction.Def);
						}					
						
						if(SceneMgr.Get.RealBall.transform.position.y <= 0.5f && !IsShooting && !IsPassing && BallController == null && getDis(ref Npc, SceneMgr.Get.RealBall.transform.position) <= PickBallDis)
							SetBall(Npc);
						break;
					case GameSituation.AttackB:
						if(Npc.Team == TeamKind.Self){
							AttackAndDef(ref Npc, GameAction.Def);
							AIMove(ref Npc, GameAction.Def);
						}else{
							if(!IsPassing){
								if(!IsShooting){
									AttackAndDef(ref Npc, GameAction.Attack);
									AIMove(ref Npc, GameAction.Attack);
								}else{
									if(!Npc.IsShooting){
										AttackAndDef(ref Npc, GameAction.Attack);
										AIMove(ref Npc, GameAction.Attack);
									}
								}
							}
						}
						
						if(SceneMgr.Get.RealBall.transform.position.y <= 0.5f && !IsShooting && !IsPassing && BallController == null && getDis(ref Npc, SceneMgr.Get.RealBall.transform.position) <= PickBallDis)
							SetBall(Npc);
						break;
					case GameSituation.TeeAPicking:
						if(BallController == null){
							//Picking ball
							if(Npc.Team == TeamKind.Self && Npc.Postion == GamePostion.PF){
								AIPickupMove(ref Npc);
								if(getDis(ref Npc, SceneMgr.Get.RealBall.transform.position) <= PickBallDis)
									SetBall(Npc);
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
                            if(BallController == null){
                                //Pick up ball
                                if(Npc.Team == TeamKind.Npc && Npc.Postion == GamePostion.PF){
                                    AIPickupMove(ref Npc);
                                    
                                    if(getDis(ref Npc, SceneMgr.Get.RealBall.transform.position) <= PickBallDis)
                                        SetBall(Npc);
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
    
    public void DoPass()
    {
        if (BallController && !ShootController && JoystickController && BallController.Team == 0) {
            if(BallController.gameObject == JoystickController)
                Catcher = PlayerList[1];
            else
                Catcher = JoystickController;
            
            BallController.AniState(PlayerState.Pass);
        }
    }
    
    public void DoShoot()
    {
        if (JoystickController && JoystickController == BallController)
        {
            Vector3 pos = SceneMgr.Get.ShootPoint[BallController.Team.GetHashCode()].transform.position;
            JoystickController.AniState (PlayerState.Shooting, true, pos.x, pos.z);
			JoystickController.LookTarget = SceneMgr.Get.Hood[JoystickController.Team.GetHashCode()].transform;
        }
    }
    
    public void DoJump()
    {
        JoystickController.AniState (PlayerState.Jumper);
    }
    
    public void DoSteal()
    {
        if (BallController && BallController != JoystickController)
            BallController.AniState (PlayerState.Steal, true, BallController.transform.position.x, BallController.transform.position.z);
    }
    
    public void DoBlock()
    {
        if (BallController)
            BallController.AniState (PlayerState.Block, true, BallController.transform.position.x, BallController.transform.position.z);
    }
    
    public void DoSkill()
    {
        
    }
    
    public void OnShoot(PlayerBehaviour player) {
		if (BallController && BallController == player) {					
			ShootController = player;
			SetBall();
			//SceneMgr.Get.RealBall.transform.localEulerAngles = Vector3.zero;                                                                                                                        
			SceneMgr.Get.SetBallState(PlayerState.Shooting);
			SceneMgr.Get.RealBall.rigidbody.velocity = 
				GameFunction.GetVelocity(SceneMgr.Get.RealBall.transform.position, 
				                         SceneMgr.Get.ShootPoint[player.Team.GetHashCode()].transform.position, 60);
        }
    }
    
    public void OnBlock(PlayerBehaviour player) {
		if (BallController) {
			if (Vector3.Distance(JoystickController.transform.position, BallController.transform.position) < 5f)
				player.rigidbody.AddForce (player.JumpHight * transform.up + player.rigidbody.velocity.normalized /2.5f, ForceMode.VelocityChange);
			else
				player.rigidbody.velocity = 
					GameFunction.GetVelocity (player.transform.position, 
				                              new Vector3(BallController.transform.position.x, 7, 
					            			  BallController.transform.position.z), 70);
		} 
		else  {
			if (ShootController && Vector3.Distance(player.transform.position, SceneMgr.Get.RealBall.transform.position) < 5)
				player.rigidbody.velocity = 
					GameFunction.GetVelocity (player.transform.position, 
					                          new Vector3(SceneMgr.Get.RealBall.transform.position.x, 5, 
					            		 	  SceneMgr.Get.RealBall.transform.position.z), 70);
        }
    }

	private bool CheckCanUseControl()
	{
		if (situation != GameSituation.None &&
		    situation != GameSituation.TeeA &&
		    situation != GameSituation.TeeB && 
		    situation != GameSituation.TeeBPicking &&
		    situation != GameSituation.End)
			return true;
        else
            return false;
    }

	public void OnJoystickMove(MovingJoystick move)
	{
		if (JoystickController && CheckCanUseControl()) {
			if (Mathf.Abs (move.joystickAxis.y) > 0 || Mathf.Abs (move.joystickAxis.x) > 0)
			{
				PlayerState ps = PlayerState.Run;
				if (BallController == JoystickController)
					ps = PlayerState.RunAndDrible;

				JoystickController.OnJoystickMove(move, ps);
            }
        }
    }
    
    public void OnJoystickMoveEnd(MovingJoystick move)
    {
		if (JoystickController) {
			PlayerState ps = PlayerState.Run;
			if (BallController == JoystickController)
				ps = PlayerState.RunAndDrible;
			
			JoystickController.OnJoystickMoveEnd(move, ps);
        }
	}

	private void AttackAndDef(ref PlayerBehaviour Npc, GameAction Action){
		if (BallController != null) {
			int stealRate = Random.Range(0, 100) + 1;
			int pushRate = Random.Range(0, 100) + 1;
			int supRate = Random.Range(0, 100) + 1;
			int passRate = Random.Range(0, 100) + 1;
			int ALLYOOP = Random.Range(0, 100) + 1;
			int DunkRate = Random.Range(0, 100) + 1;
			int shootRate = Random.Range(0, 100) + 1;
			int shoot3Rate = Random.Range(0, 100) + 1;

			float Dis = 0;
			switch(Action){
			case GameAction.Def:
				//steal push Def
				if(!IsShooting){
					if(BallController != null){
						Dis = getDis(ref BallController, ref Npc);
						
						if(!Npc.IsSteal){
							if(Dis <= PushPlayerDis && pushRate < 50){
								
							}else if(Dis <= StealBallDis && stealRate < 50 && BallController.Invincible == 0 && Npc.CoolDownSteal == 0){
								Npc.CoolDownSteal = Time.time + 3;
								Npc.AniState(PlayerState.Steal, true, BallController.gameObject.transform.localPosition.x, BallController.gameObject.transform.localPosition.z);
								if(stealRate < 5){
									SetBall(Npc);
									Npc.SetInvincible(7);
								}
							}else
								Npc.AniState(PlayerState.Defence);
						}
					}
				}else{
					if(ShootController && !Npc.IsJump && !Npc.IsBlock){
						Dis = getDis(ref Npc, ref ShootController);
						if(Dis <= StealBallDis){
							Npc.AniState(PlayerState.Block, true, ShootController.transform.localPosition.x, ShootController.transform.localPosition.z);
						}
					}
				}
				break;
			case GameAction.Attack:
				float ShootPointDis = 0;
				Vector3 pos = SceneMgr.Get.ShootPoint[Npc.Team.GetHashCode()].transform.position;

				if(Npc.Team == TeamKind.Self)
					ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));
				else
					ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));

				if(Npc == BallController){
					//Dunk shoot shoot3 pass
					if(ShootPointDis <= 2f && DunkRate < 0){
						Npc.AniState(PlayerState.Dunk);
					}else if(ShootPointDis <= 6f && (!HaveDefPlayer(ref Npc, 1.5f, 40) || shootRate < 10)){
						Npc.AniState(PlayerState.Shooting, true, pos.x, pos.z);
					}else if(ShootPointDis <= 10.5f && (!HaveDefPlayer(ref Npc, 1.5f, 40) || shoot3Rate < 3)){
						Npc.AniState(PlayerState.Shooting, true, pos.x, pos.z);
					}else if(passRate < 5 && CoolDownPass == 0){
						int Who = Random.Range(0, 2);
						int find = 0;
						for(int j = 0;  j < PlayerList.Count; j++){
							if(PlayerList[j].Team == Npc.Team && PlayerList[j] != Npc){
								if(Who == find){
									Catcher = PlayerList[j];
									Catcher.AniState(PlayerState.Idle, true, BallController.transform.localPosition.x, BallController.transform.localPosition.z);
									BallController.AniState(PlayerState.Pass, true, Catcher.transform.localPosition.x, Catcher.transform.localPosition.z);
									CoolDownPass = Time.time + 3;
									break;
								}
								find++;
							}
						}
					}else if(HaveDefPlayer(ref Npc, 2, 50)){
						//Crossover
					}
				}else{
					//sup push
					Dis = getDis(ref BallController, ref Npc); 
					PlayerBehaviour NearPlayer = HaveNearPlayer(Npc, PushPlayerDis, false);
					
					if(ShootPointDis <= 1.5f && ALLYOOP < 50){
						//Npc.AniState(PlayerState.Jumper);
					}else if(NearPlayer != null && pushRate < 50){
						//Push
						
					}else if(Dis >= 1.5f && Dis <= 3 && supRate < 50){
						//Sup
						
					}
				}
				break;
			}		
		}
	}

	private Vector2 GetTarget(Vector2 A, Vector2 B){
		return new Vector2 ((A.x + B.x) / 2, (A.y + B.y) / 2);
	}

	private void AIPickupMove(ref PlayerBehaviour Npc){
		Npc.MoveTarget = new Vector2(SceneMgr.Get.RealBall.transform.position.x, SceneMgr.Get.RealBall.transform.position.z);
	}

	private void BackToDef(ref PlayerBehaviour Npc, TeamKind Team){
		if(!Npc.IsMove && Npc.WaitMoveTime == 0){
			if(Team == TeamKind.Self)
				Npc.MoveTarget = new Vector2(TeeBackPosAy[Npc.Postion.GetHashCode()].x, -TeeBackPosAy[Npc.Postion.GetHashCode()].y);
			else
				Npc.MoveTarget = TeeBackPosAy[Npc.Postion.GetHashCode()];
		}
	}

	private void TeeBall(ref PlayerBehaviour Npc, TeamKind Team){
		if(Npc.Team == Team){
			if(!Npc.AtMoveTarget){
				if(!Npc.IsMove && Npc.WaitMoveTime == 0){
					if(Team == TeamKind.Self)
						Npc.MoveTarget = TeePosAy[Npc.Postion.GetHashCode()];
					else
						Npc.MoveTarget = new Vector2(-TeePosAy[Npc.Postion.GetHashCode()].x, -TeePosAy[Npc.Postion.GetHashCode()].y);
				}
				
				if (Npc.WaitMoveTime != 0 && Npc == BallController)
					Npc.AniState(PlayerState.Dribble);
			}else{
				for(int i = 0; i < PlayerList.Count; i++){
					if(PlayerList[i].Team == Team && PlayerList[i].Postion == GamePostion.PG && BallController != PlayerList[i]){
						Catcher = PlayerList[i];
						if(BallController)
							BallController.AniState(PlayerState.Pass, true, Catcher.transform.localPosition.x, Catcher.transform.localPosition.z);

						CoolDownPass = Time.time + 1;
						break;
					}
				}
			}
		}else{
			BackToDef(ref Npc, Npc.Team);
		}
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

	private void AIMove(ref PlayerBehaviour Npc, GameAction Action){
		if (BallController == null) {
			PlayerBehaviour A = NearBall(ref Npc);
			if(A != null){
				A.MoveTarget = new Vector2(SceneMgr.Get.RealBall.transform.position.x, SceneMgr.Get.RealBall.transform.position.z);

			}
		}else{
			switch(Action){
			case GameAction.Def:
				//move
				for(int i = 0 ; i < PlayerList.Count; i++){
					if(Npc.Team != PlayerList[i].Team && Npc.Postion == PlayerList[i].Postion){
						Vector3 Target = PlayerList[i].gameObject.transform.position;
						Vector3 ShootPoint;
						PlayerBehaviour Npc2 = PlayerList[i];
						
						if(Npc.Team == TeamKind.Self)
							ShootPoint = SceneMgr.Get.ShootPoint[1].transform.position;
						else
							ShootPoint = SceneMgr.Get.ShootPoint[0].transform.position;

						Vector2 NewTarget = GetTarget(new Vector2(Target.x, Target.z), new Vector2(ShootPoint.x, ShootPoint.z));
						for(int j = 0 ; j < 10; j++){
							if(getDis(ref Npc2, new Vector2(NewTarget.x, NewTarget.y)) > NearEnemyDis)
								NewTarget = GetTarget(new Vector2(Target.x, Target.z), NewTarget);
							else
								break;
						}
						
						Npc.MoveTarget = NewTarget;
						Npc.LookTarget = PlayerList[i].transform;
						break;
					}
				}
				break;
			case GameAction.Attack:
				if(!Npc.IsMove && Npc.WaitMoveTime == 0)
					SetMovePos(ref Npc);

				if(Npc.WaitMoveTime == 0){
					if(BallController != null && BallController != Npc){
						Npc.MoveTarget = new Vector2(Npc.MoveTarget.x, Npc.MoveTarget.y);
						Npc.LookTarget = BallController.transform;
					}else {
						Npc.MoveTarget = new Vector2(Npc.MoveTarget.x, Npc.MoveTarget.y);
					}
				}else if(Npc == BallController)
					Npc.AniState(PlayerState.Dribble);
				break;
			}
		}
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
				if(BallController != null){
					if(BallController.Team != p.Team){
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
							BallController.ResetFlag();
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

				PlayerBehaviour oldp = BallController;
				SetBallController(p);
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
				}

				ShootController = null;
			}else
				SetBallController(p);
		}
    }

	private void SetBallController(PlayerBehaviour p = null){
		BallController = p;
		if(p)
			SceneMgr.Get.SetBallState(PlayerState.Dribble, p);
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
					Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
				else
					Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			}else{
				Index = Npc.MoveIndex;
				if(Npc.Team == TeamKind.Self)
					Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
				else
					Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			}
			break;
		case MoveType.Cycle:
			//0=>1=>2=>3=>0=>1=>2=>3
			Npc.MoveIndex++;
			if(Npc.MoveIndex >= Npc.RunPosAy.Length)
				Npc.MoveIndex = 0;

			Index = Npc.MoveIndex;
			if(Npc.Team == TeamKind.Self)
				Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
			else
				Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			break;
		case MoveType.Random:
			Index = Random.Range(0, Npc.RunPosAy.Length);
			if(Npc.Team == TeamKind.Self)
				Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
			else
				Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			break;
		case MoveType.Once:
			//0=>1=>2=>3
			if(Npc.MoveIndex < Npc.RunPosAy.Length){
				Npc.MoveIndex++;
				Index = Npc.MoveIndex;
				if(Npc.Team == TeamKind.Self)
					Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
				else
					Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			}else{
				Index = Npc.RunPosAy.Length - 1;
				if(Npc.Team == TeamKind.Self)
					Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
				else
					Npc.MoveTarget = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			}
			break;
		case MoveType.Idle:
			Npc.MoveTarget = new Vector2(Npc.transform.localPosition.x, Npc.transform.localPosition.z);
			break;
		}

		return Result;
	}

	public void PlusScore(int team)
	{
		ShootInto0 = false;
		ShootInto1 = false;
		CameraMgr.Get.AddScore (team);
		
		if (IsStart) {
			if(ShootController != null){
				if(getDis(ref ShootController, SceneMgr.Get.ShootPoint[ShootController.Team.GetHashCode()].transform.position) >= 9)
					UIGame.Get.PlusScore(team, 3);
				else
					UIGame.Get.PlusScore(team, 2);
			}else
				UIGame.Get.PlusScore(team, 2);

			if(team == TeamKind.Self.GetHashCode())
				ChangeSituation(GameSituation.TeeBPicking);
			else
				ChangeSituation(GameSituation.TeeAPicking);

			ShootController = null;
		}
	}

	public void ChangeSituation(GameSituation GS){
		if (situation != GameSituation.End) {
			situation = GS;
			switch(GS){
			case GameSituation.Opening:
				
				break;
			case GameSituation.JumpBall:
				
				break;
			case GameSituation.AttackA:
				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].ResetFlag();
				UIGame.Get.ChangeControl(true);

				CameraMgr.Get.SetTeamCamera(TeamKind.Self);
				break;
			case GameSituation.AttackB:
				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].ResetFlag();
				UIGame.Get.ChangeControl(false);

				CameraMgr.Get.SetTeamCamera(TeamKind.Npc);
				break;
			case GameSituation.TeeAPicking:
				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].ResetFlag();

				CameraMgr.Get.SetTeamCamera(TeamKind.Self);

				NoAiTime = 0;
				break;
			case GameSituation.TeeA:
				NoAiTime = 0;
				break;
			case GameSituation.TeeBPicking:
				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].ResetFlag();
				CameraMgr.Get.SetTeamCamera(TeamKind.Npc);
				NoAiTime = 0;
				break;
			case GameSituation.TeeB:
				NoAiTime = 0;
				break;
			case GameSituation.End:
				for(int i = 0; i < PlayerList.Count; i++)
					PlayerList[i].ResetFlag();
				break;
			}		
		}
	}

	public bool HaveDefPlayer(ref PlayerBehaviour Npc, float dis, float angle){
		bool Result = false;
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
					if(getDis(ref Npc, ref TargetNpc) <= dis && mangle <= angle && mangle >= -angle){
						Result = true;
						break;
					}
				}		
			}	
		}
		
		return Result;
	}
	
	private PlayerBehaviour HaveNearPlayer(PlayerBehaviour Self, float Dis, bool SameTeam){
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
                    if(PlayerList[i] != Self && PlayerList[i].Team != Self.Team && getDis(ref Self, ref Npc) <= Dis){
                        Result = Npc;
                        break;
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
}
