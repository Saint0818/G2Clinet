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

	private const int CountBackSecs = 4;
	public float PickBallDis = 2.5f;
	private const float StealBallDis = 2;
	private const float PushPlayerDis = 1;
	private const float NearEnemyDis = 2;

	public bool ShootInto0 = false;
	public bool ShootInto1 = false;
	public bool Passing = false;
	public bool Shooting = false;

	public List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
	public PlayerBehaviour BallController;
	public PlayerBehaviour Catcher;
	public PlayerBehaviour ShootController;
	public GameSituation situation = GameSituation.None;
	private float Timer = 0;
	private float CoolDownPass = 0;
	private int NoAiTime = 0;

	public Vector2 [] BaseRunAy_A = new Vector2[4];
	public Vector2 [] BaseRunAy_B = new Vector2[11];
	public Vector2 [] BaseRunAy_C = new Vector2[11];
	public Vector2 [] TeePosAy = new Vector2[3];
	public Vector2 [] TeeBackPosAy = new Vector2[3];

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

		TeePosAy [0] = new Vector2 (4.5f, -11);
		TeePosAy [1] = new Vector2 (5.6f, -16);
		TeePosAy [2] = new Vector2 (4, 10);

		TeeBackPosAy[0] = new Vector2 (0, 4.5f);
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
		UIGame.Get.targetPlayer = PlayerList [0];
	}

	void Update () {
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

		if (PlayerList.Count > 0) {
			//Action
			for(int i = 0 ; i < PlayerList.Count; i++){
				PlayerBehaviour Npc = PlayerList[i];

				if(NoAiTime > 0 && Npc.Team == TeamKind.Self && Npc == UIGame.Get.targetPlayer){
					if(!Passing && (situation == GameSituation.AttackA || 
					                situation == GameSituation.AttackB || 
					                situation == GameSituation.Opening || 
					                situation == GameSituation.TeeAPicking))
						if(SceneMgr.Inst.RealBall.transform.position.y <= 0.5f && BallController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
							SetBall(Npc);
					continue;
				}else{
					//AI
					switch(situation){
					case GameSituation.None:

						break;
					case GameSituation.Opening:
						if(SceneMgr.Inst.RealBall.transform.position.y <= 0.5f && !Passing && BallController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
							SetBall(Npc);
						break;
					case GameSituation.AttackA:
						if(Npc.Team == TeamKind.Self){
							if(!Passing && !Shooting){
								AttackAndDef(ref Npc, GameAction.Attack);
								AIMove(ref Npc, GameAction.Attack);
							}
						}else{
							AttackAndDef(ref Npc, GameAction.Def);
							if(!Shooting)
								AIMove(ref Npc, GameAction.Def);
						}					

						if(SceneMgr.Inst.RealBall.transform.position.y <= 0.5f && !Passing && BallController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
							SetBall(Npc);
						break;
					case GameSituation.AttackB:
						if(Npc.Team == TeamKind.Self){
							AttackAndDef(ref Npc, GameAction.Def);
							if(!Shooting)
								AIMove(ref Npc, GameAction.Def);
						}else{
							if(!Passing && !Shooting){
								AttackAndDef(ref Npc, GameAction.Attack);
								AIMove(ref Npc, GameAction.Attack);
							}
						}

						if(SceneMgr.Inst.RealBall.transform.position.y <= 0.5f && !Passing && BallController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
							SetBall(Npc);
						break;
					case GameSituation.TeeAPicking:
						if(BallController == null){
							//Picking ball
							if(Npc.Team == TeamKind.Self && Npc.Postion == GamePostion.PF){
								AIPickupMove(ref Npc);
								if(getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
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
						if(!Passing)
							TeeBall(ref Npc, TeamKind.Self);
						break;	
					case GameSituation.TeeBPicking:
						if(BallController == null){
							//Pick up ball
							if(Npc.Team == TeamKind.Npc && Npc.Postion == GamePostion.PF){
								AIPickupMove(ref Npc);
								
								if(getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
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
						if(!Passing)
							TeeBall(ref Npc, TeamKind.Npc);
						break;					
					case GameSituation.End:

						break;
					}
				}
			}
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
				if(!Shooting){
					if(BallController != null){
						Dis = getDis(ref BallController, ref Npc);
						
						if(!Npc.IsSteal){
							if(Dis <= PushPlayerDis && pushRate < 50){
								
							}else if(Dis <= StealBallDis && stealRate < 50 && BallController.Invincible == 0 && Npc.CoolDownSteal == 0){
								Npc.CoolDownSteal = Time.time + 3;
								Npc.AniState(PlayerState.Steal, true, BallController.gameObject.transform.localPosition.x, BallController.gameObject.transform.localPosition.z);
								if(stealRate < 20){
									SetBall(Npc);
									Npc.SetInvincible(5);
								}
							}else
								Npc.AniState(PlayerState.Defence);
						}
					}
				}else{
					if(ShootController){
						Dis = getDis(ref Npc, ref ShootController);
						if(Dis <= StealBallDis && !Npc.IsJump){
							Npc.AniState(PlayerState.Block, true, ShootController.transform.localPosition.x, ShootController.transform.localPosition.z);
						}
					}
				}
				break;
			case GameAction.Attack:
				float ShootPointDis = 0;
				Vector3 pos = SceneMgr.Inst.ShootPoint[Npc.Team.GetHashCode()].transform.position;

				if(Npc.Team == TeamKind.Self)
					ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));
				else
					ShootPointDis = getDis(ref Npc, new Vector2(pos.x, pos.z));

				if(Npc == BallController){
					//Dunk shoot shoot3 pass
					if(ShootPointDis <= 2f && DunkRate < 0){
						Npc.AniState(PlayerState.Dunk);
						Shooting = true;
					}else if(ShootPointDis <= 6f && shootRate < 50){
						Npc.AniState(PlayerState.Shooting, true, pos.x, pos.z);
						Shooting = true;
					}else if(ShootPointDis <= 7f && shoot3Rate < 50){
						Npc.AniState(PlayerState.Shooting, true, pos.x, pos.z);
						Shooting = true;
					}else if(passRate < 20 && CoolDownPass == 0){
						int Who = Random.Range(0, 2);
						int find = 0;
						for(int j = 0;  j < PlayerList.Count; j++){
							if(PlayerList[j].Team == Npc.Team && PlayerList[j] != Npc){
								if(Who == find){
									Catcher = PlayerList[j];
									BallController.AniState(PlayerState.Pass);
									CoolDownPass = Time.time + 3;
									break;
								}
								find++;
							}
						}
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
		Npc.TargetPos = new Vector2(SceneMgr.Inst.RealBall.transform.position.x, SceneMgr.Inst.RealBall.transform.position.z);
		Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, Npc.TargetPos.x, Npc.TargetPos.y);
	}

	private void BackToDef(ref PlayerBehaviour Npc, TeamKind Team){
		if(!Npc.IsMove && Npc.WaitMoveTime == 0){
			if(Team == TeamKind.Self)
				Npc.TargetPos = new Vector2(TeeBackPosAy[Npc.Postion.GetHashCode()].x, -TeeBackPosAy[Npc.Postion.GetHashCode()].y);
			else
				Npc.TargetPos = TeeBackPosAy[Npc.Postion.GetHashCode()];
		}
		
		if(Npc.WaitMoveTime == 0){
			if(BallController != null && BallController != Npc){
				Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, BallController.transform.position.x, BallController.transform.position.z);
			}else
				Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, Npc.TargetPos.x, Npc.TargetPos.y);
		}
	}

	private void TeeBall(ref PlayerBehaviour Npc, TeamKind Team){
		if(Npc.Team == Team){
			if(!Npc.ReadyTee){
				if(!Npc.IsMove && Npc.WaitMoveTime == 0){
					if(Team == TeamKind.Self)
						Npc.TargetPos = TeePosAy[Npc.Postion.GetHashCode()];
					else
						Npc.TargetPos = new Vector2(-TeePosAy[Npc.Postion.GetHashCode()].x, -TeePosAy[Npc.Postion.GetHashCode()].y);
				}
				
				if(Npc.WaitMoveTime == 0){
					if(BallController != null && BallController != Npc){
						Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, BallController.transform.position.x, BallController.transform.position.z);
					}else
						Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, Npc.TargetPos.x, Npc.TargetPos.y);
				}else if(Npc == BallController)
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

	private void AIMove(ref PlayerBehaviour Npc, GameAction Action){
		if (BallController == null) {
			Npc.TargetPos = new Vector2(SceneMgr.Inst.RealBall.transform.position.x, SceneMgr.Inst.RealBall.transform.position.z);
			Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, Npc.TargetPos.x, Npc.TargetPos.y);
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
							ShootPoint = SceneMgr.Inst.ShootPoint[1].transform.position;
						else
							ShootPoint = SceneMgr.Inst.ShootPoint[0].transform.position;

						Vector2 NewTarget = GetTarget(new Vector2(Target.x, Target.z), new Vector2(ShootPoint.x, ShootPoint.z));
						for(int j = 0 ; j < 10; j++){
							if(getDis(ref Npc2, new Vector2(NewTarget.x, NewTarget.y)) > NearEnemyDis)
								NewTarget = GetTarget(new Vector2(Target.x, Target.z), NewTarget);
							else
								break;
						}
						
						Npc.TargetPos = NewTarget;
						Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, PlayerList[i].transform.position.x, PlayerList[i].transform.position.z);
						break;
					}
				}
				break;
			case GameAction.Attack:
				if(!Npc.IsMove && Npc.WaitMoveTime == 0)
					SetMovePos(ref Npc);

				if(Npc.WaitMoveTime == 0){
					if(BallController != null && BallController != Npc){
						Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, BallController.transform.position.x, BallController.transform.position.z);
					}else
						Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, Npc.TargetPos.x, Npc.TargetPos.y);
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

	public void SetBall(PlayerBehaviour p = null){
		if (PlayerList.Count > 0) {
			if(p != null && situation != GameSituation.End){
				Passing = false;
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
				
				SetBallController(p);
				if(p){
					p.WaitMoveTime = 0;
					if(p.IsJump){
						//ALLYOOP 
					}else
						p.AniState (PlayerState.Dribble);
				}

				ShootController = null;
			}else
				SetBallController(p);
		}
    }

	public void SetBallState(PlayerState state)
	{
		switch(state)
		{
			case PlayerState.Dribble: 
				SceneMgr.Inst.RealBall.transform.parent = BallController.DummyBall.transform;
				SceneMgr.Inst.RealBall.rigidbody.useGravity = false;
				SceneMgr.Inst.RealBall.rigidbody.isKinematic = true;
				SceneMgr.Inst.RealBall.transform.localEulerAngles = Vector3.zero;
				SceneMgr.Inst.RealBall.transform.localPosition = Vector3.zero;
				SceneMgr.Inst.RealBallTrigger.SetBoxColliderEnable(false);
				break;
			case PlayerState.Shooting: 
				SceneMgr.Inst.RealBall.transform.parent = null;
				SceneMgr.Inst.RealBall.rigidbody.isKinematic = false;
				SceneMgr.Inst.RealBall.rigidbody.useGravity = true;
				SceneMgr.Inst.RealBallTrigger.SetBoxColliderEnable(true);
				break;
			case PlayerState.Pass: 
				SceneMgr.Inst.RealBall.transform.parent = null;
				SceneMgr.Inst.RealBall.rigidbody.isKinematic = false;
				SceneMgr.Inst.RealBall.rigidbody.useGravity = true;
				SceneMgr.Inst.RealBallTrigger.SetBoxColliderEnable(true);
				break;
			case PlayerState.Block: 
				SceneMgr.Inst.RealBall.transform.parent = null;
				SceneMgr.Inst.RealBall.rigidbody.isKinematic = false;
				SceneMgr.Inst.RealBall.rigidbody.useGravity = true;
				SceneMgr.Inst.RealBallTrigger.SetBoxColliderEnable(true);
				SceneMgr.Inst.RealBallTrigger.Falling();
				UIHint.Get.ShowHint("Blocking", Color.blue);
				break;
		}
	}

	private void SetBallController(PlayerBehaviour p = null){
		BallController = p;
		if(p)
			SetBallState(PlayerState.Dribble);
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
					Npc.TargetPos = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
				else
					Npc.TargetPos = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			}else{
				Index = Npc.MoveIndex;
				if(Npc.Team == TeamKind.Self)
					Npc.TargetPos = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
				else
					Npc.TargetPos = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			}
			break;
		case MoveType.Cycle:
			//0=>1=>2=>3=>0=>1=>2=>3
			Npc.MoveIndex++;
			if(Npc.MoveIndex >= Npc.RunPosAy.Length)
				Npc.MoveIndex = 0;

			Index = Npc.MoveIndex;
			if(Npc.Team == TeamKind.Self)
				Npc.TargetPos = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
			else
				Npc.TargetPos = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			break;
		case MoveType.Random:
			Index = Random.Range(0, Npc.RunPosAy.Length);
			if(Npc.Team == TeamKind.Self)
				Npc.TargetPos = new Vector2(Npc.RunPosAy[Index].x, Npc.RunPosAy[Index].y);
			else
				Npc.TargetPos = new Vector2(Npc.RunPosAy[Index].x, -Npc.RunPosAy[Index].y);
			break;
		}

		return Result;
	}

	public void PlusScore(int team)
	{
		ShootInto0 = false;
		ShootInto1 = false;
		
		if (UIGame.Get.IsStart) {
			UIGame.Get.PlusScore(team, 2);

			if(team == TeamKind.Self.GetHashCode())
				ChangeSituation(GameSituation.TeeBPicking);
			else
				ChangeSituation(GameSituation.TeeAPicking);
		}
	}

	public void ChangeSituation(GameSituation GS){
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
			break;
		case GameSituation.AttackB:
			for(int i = 0; i < PlayerList.Count; i++)
				PlayerList[i].ResetFlag();
			UIGame.Get.ChangeControl(false);
			break;
		case GameSituation.TeeAPicking:
			for(int i = 0; i < PlayerList.Count; i++)
				PlayerList[i].ResetFlag();
			break;
		case GameSituation.TeeA:

			break;
		case GameSituation.TeeBPicking:
			for(int i = 0; i < PlayerList.Count; i++)
				PlayerList[i].ResetFlag();
			break;
		case GameSituation.TeeB:

			break;
		case GameSituation.End:
			for(int i = 0; i < PlayerList.Count; i++)
				PlayerList[i].ResetFlag();
			break;
		}
	}
}
