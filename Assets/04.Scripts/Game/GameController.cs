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

	private const int CountBackSecs = 3;
	private const int MaxPos = 6;

	public List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
	public PlayerBehaviour ballController;
	public GameSituation situation = GameSituation.None;
	private float Timer = 0;
	private int NoAiTime = 0;
	public Vector2 [] ShortRunAy = new Vector2[MaxPos];
	public Vector2 [] MidRunAy = new Vector2[MaxPos];
	public Vector2 [] LongRunAy = new Vector2[MaxPos];

	void Start () {
		EasyTouch.On_TouchDown += TouchDown;
		NoAiTime = 0;
		InitPos ();
		InitGame ();
	}

	private void InitPos(){
		ShortRunAy [0] = new Vector2 (0, 15);
		ShortRunAy [1] = new Vector2 (5.2f, 15);
		ShortRunAy [2] = new Vector2 (-4.1f, 15);
		ShortRunAy [3] = new Vector2 (0, 12);
		ShortRunAy [4] = new Vector2 (3.9f, 12);
		ShortRunAy [5] = new Vector2 (-4.1f, 12);

		MidRunAy [0] = new Vector2 (-3.6f, 10);
		MidRunAy [1] = new Vector2 (4.6f, 10);
		MidRunAy [2] = new Vector2 (5.5f, 14);
		MidRunAy [3] = new Vector2 (-4.8f, 14);
		MidRunAy [4] = new Vector2 (0, 9);
		MidRunAy [5] = new Vector2 (4.4f, 9);

		LongRunAy [0] = new Vector2 (0, 5.7f);
		LongRunAy [1] = new Vector2 (-5.4f, 8);
		LongRunAy [2] = new Vector2 (6.7f, 8.9f);
		LongRunAy [3] = new Vector2 (8.2f, 14);
		LongRunAy [4] = new Vector2 (-8.2f, 14);
		LongRunAy [5] = new Vector2 (4.82f, 4.26f);
	}

	private void TouchDown (Gesture gesture){
		NoAiTime = CountBackSecs;
	}

	public void InitGame(){
		PlayerList.Clear ();
		CreateTeam ();
	}

	public void CreateTeam(){
		PlayerList.Add (ModelManager.Get.CreatePlayer (0, TeamKind.Self, RunDistanceType.Short, new Vector3(0, 0, 0), MoveType.BackAndForth, 0));
//		PlayerList.Add (ModelManager.Get.CreatePlayer (1, TeamKind.Self, RunDistanceType.Mid, new Vector3(5, 0, -2), MoveType.BackAndForth, 1));
//		PlayerList.Add (ModelManager.Get.CreatePlayer (2, TeamKind.Self, RunDistanceType.Long, new Vector3(-5, 0, -2), MoveType.Cycle, 2));
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

				if(NoAiTime > 0 && Npc.Team == TeamKind.Self && Npc == ballController){
					continue;
				}else{
					//AI
					switch(situation){
					case GameSituation.Opening:

						break;
					case GameSituation.AttackA:
						if(Npc.Team == TeamKind.Self)
							AttackAndDef(Npc, GameAction.Attack);
						else 
							AttackAndDef(Npc, GameAction.Def);				
						break;
					case GameSituation.AttackB:
						if(Npc.Team == TeamKind.Self)
							AttackAndDef(Npc, GameAction.Def);
						else 
							AttackAndDef(Npc, GameAction.Attack);
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

			//Move
			for(int i = 0 ; i < PlayerList.Count; i++){
				PlayerBehaviour Npc = PlayerList[i];

				if(NoAiTime > 0 && Npc.Team == TeamKind.Self && Npc == ballController){
					continue;
				}else{
					//AI
					switch(situation){
					case GameSituation.Opening:
						
						break;
					case GameSituation.AttackA:
						if(Npc.Team == TeamKind.Self)
							AIMove(Npc, GameAction.Attack);
						else 
							AIMove(Npc, GameAction.Def);				
						break;
					case GameSituation.AttackB:
						if(Npc.Team == TeamKind.Self)
							AIMove(Npc, GameAction.Def);
						else 
							AIMove(Npc, GameAction.Attack);
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

	private void AttackAndDef(PlayerBehaviour Npc, GameAction Action){
		int stealRate = Random.Range(0, 100) + 1;
		int pushRate = Random.Range(0, 100) + 1;
		int defRate = Random.Range(0, 100) + 1;
		int supRate = Random.Range(0, 100) + 1;
		int ALLYOOP = Random.Range(0, 100) + 1;

		switch(Action){
		case GameAction.Def:
			//steal push Def



			Npc.SetDef();
			break;
		case GameAction.Attack:
			if(Npc == ballController){
				//Dunk shoot shoot3 pass


			}else{
				//sup push
				float Dis = getDis(ballController, Npc); 
				PlayerBehaviour NearPlayer = HaveNearPlayer(Npc, 1.5f, false);

				if(NearPlayer != null && pushRate <= 50){
					//Push

				}else if(Dis >= 1.5f && Dis <= 3 && supRate <= 50){
					//Sup

				}
			}
			break;
		}
	}

	private void AIMove(PlayerBehaviour Npc, GameAction Action){
		switch(Action){
		case GameAction.Def:
			//move
			for(int i = 0 ; i < PlayerList.Count; i++){
				if(Npc.Team != PlayerList[i].Team && Npc.Postion == PlayerList[i].Postion){
					Npc.TargetPos = new Vector2(PlayerList[i].gameObject.transform.localPosition.x, PlayerList[i].gameObject.transform.localPosition.z);
					break;
				}
			}

			Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y);
			break;
		case GameAction.Attack:
			if(!Npc.Move){
				if(Npc.WaitMoveTime == 0)
					SetMovePos(Npc);
			}

			if(Npc.WaitMoveTime == 0)
				Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y);
			break;
		}
	}

	private float getDis(PlayerBehaviour player1, PlayerBehaviour player2)
	{
		if (player1 != null && player2 != null && player1 != player2){
			Vector3 V1 = player1.transform.position;
			Vector3 V2 = player2.transform.position;
			V1.y = V2.y;
			return Vector3.Distance(V1, V2);
		} else
			return -1;
	}

	private PlayerBehaviour HaveNearPlayer(PlayerBehaviour Self, float Dis, bool SameTeam){
		PlayerBehaviour Result = null;

		if (PlayerList.Count > 1) {
			for(int i = 0 ; i < PlayerList.Count; i++){
				if(SameTeam){
					if(PlayerList[i] != Self && PlayerList[i].Team == Self.Team && getDis(Self, PlayerList[i]) <= Dis){
						Result = PlayerList[i];
						break;
					}
				}else{
					if(PlayerList[i] != Self && PlayerList[i].Team != Self.Team && getDis(Self, PlayerList[i]) <= Dis){
						Result = PlayerList[i];
						break;
					}
				}
			}
		}

		return Result;
	}

	public void SetBall(GameObject player){
    	PlayerBehaviour p = (PlayerBehaviour)player.GetComponent<PlayerBehaviour>();
		if (p) {
			SceneMgr.Inst.RealBall.rigidbody.velocity = Vector3.zero;
			SceneMgr.Inst.RealBall.rigidbody.useGravity = false;
			SceneMgr.Inst.RealBall.rigidbody.isKinematic = false;
			SceneMgr.Inst.RealBall.transform.parent = p.DummyBall.transform;

			p.AniState(PlayerState.Dribble);
			SceneMgr.Inst.RealBall.transform.localEulerAngles = Vector3.zero;
			SceneMgr.Inst.RealBall.transform.localPosition = Vector3.zero;
		}
    }

	private Vector2 SetMovePos(PlayerBehaviour Npc){
		Vector2 Result = Vector2.zero;
		int Index = 0;
		switch(Npc.MoveKind){
		case MoveType.BackAndForth:
			//0=>1=>2=>3=>2=>1=>0
			Npc.MoveIndex++;
			if(Npc.MoveIndex >= (MaxPos * 2) - 2)
				Npc.MoveIndex = 0;

			if(Npc.MoveIndex >= MaxPos){
				Index = (MaxPos - 1)* 2 - Npc.MoveIndex;

				if(Npc.RunArea == RunDistanceType.Short)
					Npc.TargetPos = new Vector2(ShortRunAy[Index].x, ShortRunAy[Index].y);
				else if(Npc.RunArea == RunDistanceType.Mid)
					Npc.TargetPos = new Vector2(MidRunAy[Index].x, MidRunAy[Index].y);
				else if(Npc.RunArea == RunDistanceType.Long)
					Npc.TargetPos = new Vector2(LongRunAy[Index].x, LongRunAy[Index].y);
			}else{
				Index = Npc.MoveIndex;

				if(Npc.RunArea == RunDistanceType.Short)
					Npc.TargetPos = new Vector2(ShortRunAy[Index].x, ShortRunAy[Index].y);
				else if(Npc.RunArea == RunDistanceType.Mid)
					Npc.TargetPos = new Vector2(MidRunAy[Index].x, MidRunAy[Index].y);
				else if(Npc.RunArea == RunDistanceType.Long)
					Npc.TargetPos = new Vector2(LongRunAy[Index].x, LongRunAy[Index].y);
			}
			break;
		case MoveType.Cycle:
			//0=>1=>2=>3=>0=>1=>2=>3
			Npc.MoveIndex++;
			if(Npc.MoveIndex >= MaxPos)
				Npc.MoveIndex = 0;

			Index = Npc.MoveIndex;
			if(Npc.RunArea == RunDistanceType.Short)
				Npc.TargetPos = new Vector2(ShortRunAy[Index].x, ShortRunAy[Index].y);
			else if(Npc.RunArea == RunDistanceType.Mid)
				Npc.TargetPos = new Vector2(MidRunAy[Index].x, MidRunAy[Index].y);
			else if(Npc.RunArea == RunDistanceType.Long)
				Npc.TargetPos = new Vector2(LongRunAy[Index].x, LongRunAy[Index].y);
			break;
		case MoveType.Random:
			Index = Random.Range(0, MaxPos);
			
			if(Npc.RunArea == RunDistanceType.Short)
				Npc.TargetPos = new Vector2(ShortRunAy[Index].x, ShortRunAy[Index].y);
			else if(Npc.RunArea == RunDistanceType.Mid)
				Npc.TargetPos = new Vector2(MidRunAy[Index].x, MidRunAy[Index].y);
			else if(Npc.RunArea == RunDistanceType.Long)
				Npc.TargetPos = new Vector2(LongRunAy[Index].x, LongRunAy[Index].y);
			break;
		}

		return Result;
	}
}
