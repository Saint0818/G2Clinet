using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameSituation{
	None     = 0,
	Opening  = 1,
	JumpBall = 2,
	AttackA  = 3,
	AttackB  = 4,
	TeeA     = 5,
	TeeB     = 6,
	End      = 7
}

public enum GameAction{
	Def = 0,
	Attack = 1
}

public class GameController : MonoBehaviour {

	private const int CountBackSecs = 4;
	private const float PickBallDis = 2.5f;
	private const float StealBallDis = 2;
	private const float PushPlayerDis = 1;
	private const float NearEnemyDis = 1;

	public bool ShootInto0 = false;
	public bool ShootInto1 = false;
	public bool Passing = false;

	public List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
	public PlayerBehaviour BallController;
	public PlayerBehaviour Catcher;
	public PlayerBehaviour ShootController;
	public GameSituation situation = GameSituation.None;
	private float Timer = 0;
	private int NoAiTime = 0;
	public Vector2 [] BaseRunAy_A = new Vector2[4];
	public Vector2 [] BaseRunAy_B = new Vector2[11];
	public Vector2 [] BaseRunAy_C = new Vector2[11];

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
	}

	private void TouchDown (Gesture gesture){
		if(UIGame.Get.Joystick.Visible && (situation == GameSituation.AttackA || situation == GameSituation.AttackB))
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
	}

	public void CreateTeam(){
		PlayerList.Add (ModelManager.Get.CreatePlayer (0, TeamKind.Self, new Vector3(0, 0, 0), BaseRunAy_A, MoveType.PingPong, 0));
		PlayerList.Add (ModelManager.Get.CreatePlayer (1, TeamKind.Self, new Vector3(5, 0, -2), BaseRunAy_B, MoveType.PingPong, 1));
		PlayerList.Add (ModelManager.Get.CreatePlayer (2, TeamKind.Self, new Vector3(-5, 0, -2), BaseRunAy_C, MoveType.PingPong, 2));

		PlayerList.Add (ModelManager.Get.CreatePlayer (3, TeamKind.Npc, new Vector3(0, 0, 2), BaseRunAy_A, MoveType.PingPong, 0));	
		PlayerList.Add (ModelManager.Get.CreatePlayer (4, TeamKind.Npc, new Vector3(5, 0, 2), BaseRunAy_B, MoveType.PingPong, 1));
		PlayerList.Add (ModelManager.Get.CreatePlayer (5, TeamKind.Npc, new Vector3(-5, 0, 2), BaseRunAy_C, MoveType.PingPong, 2));
		UIGame.Get.targetPlayer = PlayerList [0];
	}

	void Update () {
		if (Time.time - Timer >= 1){
			Timer = Time.time;

			if(NoAiTime > 0){
				NoAiTime--;
			}
		}

		if (PlayerList.Count > 0) {
			//Action
			for(int i = 0 ; i < PlayerList.Count; i++){
				PlayerBehaviour Npc = PlayerList[i];

				if(NoAiTime > 0 && Npc.Team == TeamKind.Self && Npc == UIGame.Get.targetPlayer){
					if(!Passing && (situation == GameSituation.AttackA || situation == GameSituation.AttackB || situation == GameSituation.Opening || situation == GameSituation.TeeA))
						if(BallController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
							SetBall(ref Npc);
					continue;
				}else{
					//AI
					switch(situation){
					case GameSituation.None:
						if(BallController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
							SetBall(ref Npc);
						break;
					case GameSituation.Opening:

						break;
					case GameSituation.AttackA:
						if(Npc.Team == TeamKind.Self){
							AttackAndDef(ref Npc, GameAction.Attack);
							AIMove(ref Npc, GameAction.Attack);
						}else{
							AttackAndDef(ref Npc, GameAction.Def);
							AIMove(ref Npc, GameAction.Def);
						}					

						if(!Passing && BallController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
							SetBall(ref Npc);
						break;
					case GameSituation.AttackB:
						if(Npc.Team == TeamKind.Self){
							AttackAndDef(ref Npc, GameAction.Def);
							AIMove(ref Npc, GameAction.Def);
						}else{
							AttackAndDef(ref Npc, GameAction.Attack);
							AIMove(ref Npc, GameAction.Attack);
						}

						if(!Passing && BallController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
							SetBall(ref Npc);
						break;
					case GameSituation.TeeA:
						if(BallController == null){
							//Pick up ball
							if(Npc.Team == TeamKind.Self)
								AIPickupMove(ref Npc);

							if(!Passing && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
								SetBall(ref Npc);
						}else{
							//Tee ball

						}
						break;
					case GameSituation.TeeB:
						if(BallController == null){
							//Pick up ball
							if(Npc.Team == TeamKind.Npc)
								AIPickupMove(ref Npc);
							
							if(!Passing && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
								SetBall(ref Npc);
						}else{
							//Tee ball
							
						}
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
			int ALLYOOP = Random.Range(0, 100) + 1;
			float Dis = 0;
			switch(Action){
			case GameAction.Def:
				//steal push Def
				if(BallController != null){
					Dis = getDis(ref BallController, ref Npc);
					
					if(!Npc.IsSteal){
						if(Dis <= PushPlayerDis && pushRate < 50){
							
						}else if(Dis <= StealBallDis && stealRate < 50 && BallController.Invincible == 0 && Npc.CoolDownSteal == 0){
							Npc.CoolDownSteal = Time.time + 3;
							Npc.AniState(PlayerState.Steal, true, BallController.gameObject.transform.localPosition.x, BallController.gameObject.transform.localPosition.z);
							if(stealRate < 5){
								SetBall(ref Npc);
								Npc.Invincible = Time.time + 5;
							}
						}else{
							if(Dis <= 3)
								Npc.AniState(PlayerState.Defence);
							else
								Npc.AniState(PlayerState.Idle);
						}
							
					}
				}
				break;
			case GameAction.Attack:
				if(Npc == BallController){
					//Dunk shoot shoot3 pass

					
				}else{
					//sup push
					Dis = getDis(ref BallController, ref Npc); 
					PlayerBehaviour NearPlayer = HaveNearPlayer(Npc, PushPlayerDis, false);
					
					float ShootPointDis = 0;
					if(Npc.Team == TeamKind.Self)
						ShootPointDis = getDis(ref Npc, new Vector2(SceneMgr.Inst.ShootPoint[0].transform.position.x, SceneMgr.Inst.ShootPoint[0].transform.position.z));
					else
						ShootPointDis = getDis(ref Npc, new Vector2(SceneMgr.Inst.ShootPoint[1].transform.position.x, SceneMgr.Inst.ShootPoint[1].transform.position.z));
					
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

	public void SetBall(ref PlayerBehaviour p){
		if (PlayerList.Count > 0) {
			if(p != null && situation != GameSituation.End){
				if(BallController != null){
					if(BallController.Team != p.Team){
						if(situation == GameSituation.AttackA)
							ChangeSituation(GameSituation.AttackB);
						else if(situation == GameSituation.AttackB)
							ChangeSituation(GameSituation.AttackA);
					}else
						BallController.ResetFlag();
				}else{
					if(p.Team == TeamKind.Self)
						ChangeSituation(GameSituation.AttackA);
					else if(p.Team == TeamKind.Npc)
						ChangeSituation(GameSituation.AttackB);
				}
				
				SetBallController(p);
				if(p.IsJump){
					//ALLYOOP 

				}else
					p.AniState (PlayerState.Dribble);

				SceneMgr.Inst.RealBall.transform.parent = p.DummyBall.transform;
				SetBallState(PlayerState.Dribble);
				ShootController = null;
			}
		}
    }

	public void SetBallState(PlayerState state)
	{
		switch(state)
		{
			case PlayerState.Dribble: 
//				SceneMgr.Inst.RealBall.rigidbody.velocity = Vector3.zero;
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
		}
	}

	public void SetBallController(PlayerBehaviour p = null){
		BallController = p;
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
				ChangeSituation(GameSituation.TeeB);
			else
				ChangeSituation(GameSituation.TeeA);
		}
	}

	private void ChangeSituation(GameSituation GS){
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
		case GameSituation.TeeA:
			break;
		case GameSituation.TeeB:
			break;
		case GameSituation.End:
			break;
		}
	}
}
