﻿using UnityEngine;
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
	private const int MaxPos = 4;
	private const float PickBallDis = 2.5f;
	private const float StealBallDis = 2;
	private const float PushPlayerDis = 1;
	private const float NearEnemyDis = 1;

	public bool ShootInto0 = false;
	public bool ShootInto1 = false;

	public List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
	public PlayerBehaviour ballController;
	public GameSituation situation = GameSituation.None;
	private float Timer = 0;
	private int NoAiTime = 0;
	public Vector2 [] ShortRunAy_A = new Vector2[MaxPos];
	public Vector2 [] MidRunAy_A = new Vector2[MaxPos];
	public Vector2 [] LongRunAy_A = new Vector2[MaxPos];
	public Vector2 [] ShortRunAy_B = new Vector2[MaxPos];
	public Vector2 [] MidRunAy_B = new Vector2[MaxPos];
	public Vector2 [] LongRunAy_B = new Vector2[MaxPos];

	void Start () {
		EasyTouch.On_TouchDown += TouchDown;
		NoAiTime = 0;
		InitPos ();
		InitGame ();
	}

	private void InitPos(){
		ShortRunAy_A [0] = new Vector2 (0, 4.5f);
		ShortRunAy_A [1] = new Vector2 (4, 5.9f);
		ShortRunAy_A [2] = new Vector2 (0, 8);
		ShortRunAy_A [3] = new Vector2 (-4.9f, 6.2f);
		ShortRunAy_B [0] = new Vector2 (0, -4.5f);
		ShortRunAy_B [1] = new Vector2 (4, -5.9f);
		ShortRunAy_B [2] = new Vector2 (0, -8);
		ShortRunAy_B [3] = new Vector2 (-4.9f, -6.2f);

		MidRunAy_A [0] = new Vector2 (5.3f, 10);
		MidRunAy_A [1] = new Vector2 (1.8f, 13);
		MidRunAy_A [2] = new Vector2 (1.8f, 8.9f);
		MidRunAy_A [3] = new Vector2 (5.3f, 13);
		MidRunAy_B [0] = new Vector2 (5.3f, -10);
		MidRunAy_B [1] = new Vector2 (1.8f, -13);
		MidRunAy_B [2] = new Vector2 (1.8f, -8.9f);
		MidRunAy_B [3] = new Vector2 (5.3f, -13);

		LongRunAy_A [0] = new Vector2 (-5.3f, 10);
		LongRunAy_A [1] = new Vector2 (-1.8f, 13);
		LongRunAy_A [2] = new Vector2 (-1.8f, 8.9f);
		LongRunAy_A [3] = new Vector2 (-5.3f, 13);
		LongRunAy_B [0] = new Vector2 (-5.3f, -10);
		LongRunAy_B [1] = new Vector2 (-1.8f, -13);
		LongRunAy_B [2] = new Vector2 (-1.8f, -8.9f);
		LongRunAy_B [3] = new Vector2 (-5.3f, -13);
	}

	private void TouchDown (Gesture gesture){
		if(UIGame.Get.Joystick.Visible)
			NoAiTime = CountBackSecs;
	}

	public void InitGame(){
		EffectManager.Get.LoadGameEffect();
		PlayerList.Clear ();
		CreateTeam ();
	}

	public void CreateTeam(){
		PlayerList.Add (ModelManager.Get.CreatePlayer (0, TeamKind.Self, RunDistanceType.Short, new Vector3(0, 0, 0), MoveType.PingPong, 0));
		PlayerList.Add (ModelManager.Get.CreatePlayer (1, TeamKind.Self, RunDistanceType.Mid, new Vector3(5, 0, -2), MoveType.PingPong, 1));
		PlayerList.Add (ModelManager.Get.CreatePlayer (2, TeamKind.Self, RunDistanceType.Long, new Vector3(-5, 0, -2), MoveType.PingPong, 2));
//
//
//		PlayerList.Add (ModelManager.Get.CreatePlayer (3, TeamKind.Npc, RunDistanceType.Short, new Vector3(0, 0, 2), MoveType.BackAndForth, 0));
//		PlayerList.Add (ModelManager.Get.CreatePlayer (4, TeamKind.Npc, RunDistanceType.Mid, new Vector3(5, 0, 2), MoveType.Cycle, 1));
//		PlayerList.Add (ModelManager.Get.CreatePlayer (5, TeamKind.Npc, RunDistanceType.Long, new Vector3(-5, 0, 2), MoveType.Random, 2));
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
					if(ballController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
						SetBall(ref Npc);
					continue;
				}else{
					//AI
					switch(situation){
					case GameSituation.None:
						if(ballController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
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

						if(ballController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
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

						if(ballController == null && getDis(ref Npc, SceneMgr.Inst.RealBall.transform.position) <= PickBallDis)
							SetBall(ref Npc);
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
		}
	}

	private void AttackAndDef(ref PlayerBehaviour Npc, GameAction Action){
		if (ballController != null) {
			int stealRate = Random.Range(0, 100) + 1;
			int pushRate = Random.Range(0, 100) + 1;
			int supRate = Random.Range(0, 100) + 1;
			int ALLYOOP = Random.Range(0, 100) + 1;
			float Dis = 0;
			switch(Action){
			case GameAction.Def:
				//steal push Def
				if(ballController != null){
					Dis = getDis(ref ballController, ref Npc);
					
					if(!Npc.IsSteal){
						if(Dis <= PushPlayerDis && pushRate < 50){
							
						}else if(Dis <= StealBallDis && stealRate < 50 && ballController.Invincible == 0 && Npc.CoolDownSteal == 0){
							if(!Npc.IsSteal){
								Npc.CoolDownSteal = Time.time + 3;
								Npc.AniState(PlayerState.Steal, true, ballController.gameObject.transform.localPosition.x, ballController.gameObject.transform.localPosition.z);
								if(stealRate < 5){
									SetBall(ref Npc);
									Npc.Invincible = Time.time + 5;
								}
							}
						}else
							Npc.AniState(PlayerState.Defence);
					}
				}
				break;
			case GameAction.Attack:
				if(Npc == ballController){
					//Dunk shoot shoot3 pass

					
				}else{
					//sup push
					Dis = getDis(ref ballController, ref Npc); 
					PlayerBehaviour NearPlayer = HaveNearPlayer(Npc, PushPlayerDis, false);
					
					float ShootPointDis = 0;
					if(Npc.Team == TeamKind.Self)
						ShootPointDis = getDis(ref Npc, new Vector2(SceneMgr.Inst.ShootPoint[0].transform.position.x, SceneMgr.Inst.ShootPoint[0].transform.position.z));
					else
						ShootPointDis = getDis(ref Npc, new Vector2(SceneMgr.Inst.ShootPoint[1].transform.position.x, SceneMgr.Inst.ShootPoint[1].transform.position.z));
					
					if(ShootPointDis <= 1.5f && ALLYOOP < 50){
						Npc.AniState(PlayerState.Jumper);
						//Npc.Jump();
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

	private void AIMove(ref PlayerBehaviour Npc, GameAction Action){
		if (ballController == null) {
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
					if(ballController != null && ballController != Npc){
						Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, ballController.transform.position.x, ballController.transform.position.z);
					}else
						Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y, Npc.TargetPos.x, Npc.TargetPos.y);
				}else if(Npc == ballController)
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
				if(ballController != null){
					if(ballController.Team != p.Team){
						if(situation == GameSituation.AttackA)
							ChangeSituation(GameSituation.AttackB);
						else if(situation == GameSituation.AttackB)
							ChangeSituation(GameSituation.AttackA);
					}else
						ballController.ResetFlag();
				}else{
					if(p.Team == TeamKind.Self)
						ChangeSituation(GameSituation.AttackA);
					else if(p.Team == TeamKind.Npc)
						ChangeSituation(GameSituation.AttackB);
				}
				
				SetballController(p);
				if(p.IsJump){
					//ALLYOOP 

				}else
					p.AniState (PlayerState.Dribble);

				SceneMgr.Inst.RealBall.transform.parent = p.DummyBall.transform;
				SetBallState(PlayerState.Dribble);
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

	public void SetballController(PlayerBehaviour p = null){
		ballController = p;
	}

	private Vector2 SetMovePos(ref PlayerBehaviour Npc){
		Vector2 Result = Vector2.zero;
		int Index = 0;
		switch(Npc.MoveKind){
		case MoveType.PingPong:
			//0=>1=>2=>3=>2=>1=>0
			Npc.MoveIndex++;
			if(Npc.MoveIndex >= (MaxPos * 2) - 2)
				Npc.MoveIndex = 0;

			if(Npc.MoveIndex >= MaxPos){
				Index = (MaxPos - 1)* 2 - Npc.MoveIndex;

				if(Npc.Team == TeamKind.Self){
					if(Npc.RunArea == RunDistanceType.Short)
						Npc.TargetPos = new Vector2(ShortRunAy_A[Index].x, ShortRunAy_A[Index].y);
					else if(Npc.RunArea == RunDistanceType.Mid)
						Npc.TargetPos = new Vector2(MidRunAy_A[Index].x, MidRunAy_A[Index].y);
					else if(Npc.RunArea == RunDistanceType.Long)
						Npc.TargetPos = new Vector2(LongRunAy_A[Index].x, LongRunAy_A[Index].y);
				}else if(Npc.Team == TeamKind.Npc){
					if(Npc.RunArea == RunDistanceType.Short)
						Npc.TargetPos = new Vector2(ShortRunAy_B[Index].x, ShortRunAy_B[Index].y);
					else if(Npc.RunArea == RunDistanceType.Mid)
						Npc.TargetPos = new Vector2(MidRunAy_B[Index].x, MidRunAy_B[Index].y);
					else if(Npc.RunArea == RunDistanceType.Long)
						Npc.TargetPos = new Vector2(LongRunAy_B[Index].x, LongRunAy_B[Index].y);
				}
			}else{
				Index = Npc.MoveIndex;

				if(Npc.Team == TeamKind.Self){
					if(Npc.RunArea == RunDistanceType.Short)
						Npc.TargetPos = new Vector2(ShortRunAy_A[Index].x, ShortRunAy_A[Index].y);
					else if(Npc.RunArea == RunDistanceType.Mid)
						Npc.TargetPos = new Vector2(MidRunAy_A[Index].x, MidRunAy_A[Index].y);
					else if(Npc.RunArea == RunDistanceType.Long)
						Npc.TargetPos = new Vector2(LongRunAy_A[Index].x, LongRunAy_A[Index].y);
				}else if(Npc.Team == TeamKind.Npc){
					if(Npc.RunArea == RunDistanceType.Short)
						Npc.TargetPos = new Vector2(ShortRunAy_B[Index].x, ShortRunAy_B[Index].y);
					else if(Npc.RunArea == RunDistanceType.Mid)
						Npc.TargetPos = new Vector2(MidRunAy_B[Index].x, MidRunAy_B[Index].y);
					else if(Npc.RunArea == RunDistanceType.Long)
						Npc.TargetPos = new Vector2(LongRunAy_B[Index].x, LongRunAy_B[Index].y);
				}
			}
			break;
		case MoveType.Cycle:
			//0=>1=>2=>3=>0=>1=>2=>3
			Npc.MoveIndex++;
			if(Npc.MoveIndex >= MaxPos)
				Npc.MoveIndex = 0;

			Index = Npc.MoveIndex;

			if(Npc.Team == TeamKind.Self){
				if(Npc.RunArea == RunDistanceType.Short)
					Npc.TargetPos = new Vector2(ShortRunAy_A[Index].x, ShortRunAy_A[Index].y);
				else if(Npc.RunArea == RunDistanceType.Mid)
					Npc.TargetPos = new Vector2(MidRunAy_A[Index].x, MidRunAy_A[Index].y);
				else if(Npc.RunArea == RunDistanceType.Long)
					Npc.TargetPos = new Vector2(LongRunAy_A[Index].x, LongRunAy_A[Index].y);
			}else if(Npc.Team == TeamKind.Npc){
				if(Npc.RunArea == RunDistanceType.Short)
					Npc.TargetPos = new Vector2(ShortRunAy_B[Index].x, ShortRunAy_B[Index].y);
				else if(Npc.RunArea == RunDistanceType.Mid)
					Npc.TargetPos = new Vector2(MidRunAy_B[Index].x, MidRunAy_B[Index].y);
				else if(Npc.RunArea == RunDistanceType.Long)
					Npc.TargetPos = new Vector2(LongRunAy_B[Index].x, LongRunAy_B[Index].y);
			}
			break;
		case MoveType.Random:
			Index = Random.Range(0, MaxPos);

			if(Npc.Team == TeamKind.Self){
				if(Npc.RunArea == RunDistanceType.Short)
					Npc.TargetPos = new Vector2(ShortRunAy_A[Index].x, ShortRunAy_A[Index].y);
				else if(Npc.RunArea == RunDistanceType.Mid)
					Npc.TargetPos = new Vector2(MidRunAy_A[Index].x, MidRunAy_A[Index].y);
				else if(Npc.RunArea == RunDistanceType.Long)
					Npc.TargetPos = new Vector2(LongRunAy_A[Index].x, LongRunAy_A[Index].y);
			}else if(Npc.Team == TeamKind.Npc){
				if(Npc.RunArea == RunDistanceType.Short)
					Npc.TargetPos = new Vector2(ShortRunAy_B[Index].x, ShortRunAy_B[Index].y);
				else if(Npc.RunArea == RunDistanceType.Mid)
					Npc.TargetPos = new Vector2(MidRunAy_B[Index].x, MidRunAy_B[Index].y);
				else if(Npc.RunArea == RunDistanceType.Long)
					Npc.TargetPos = new Vector2(LongRunAy_B[Index].x, LongRunAy_B[Index].y);
			}
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
