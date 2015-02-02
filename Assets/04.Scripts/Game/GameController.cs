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

	private const int CountBackSecs = 3;
	private const int MaxPos = 3;

	public List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
	public PlayerBehaviour ballController;
	public GameSituation situation = GameSituation.None;
	private float Timer = 0;
	private int NoAiTime = 0;
	public Vector2 [] BigRunAy = new Vector2[MaxPos];
	public Vector2 [] MidRunAy = new Vector2[MaxPos];
	public Vector2 [] SmallRunAy = new Vector2[MaxPos];

	void Start () {
		EasyTouch.On_TouchDown += TouchDown;
		NoAiTime = 0;
		InitPos ();
		InitGame ();
	}

	private void InitPos(){
		BigRunAy [0] = new Vector2 (6, 6.6f);
		BigRunAy [1] = new Vector2 (0, 0);
		BigRunAy [2] = new Vector2 (-2.9f, 4.19f);
	}

	private void TouchDown (Gesture gesture){
		NoAiTime = CountBackSecs;
	}

	public void InitGame(){
		PlayerList.Clear ();
		CreateTeam ();
	}

	public void CreateTeam(){
		PlayerList.Add (ModelManager.Get.CreatePlayer (0, TeamKind.Self, BodyType.Big));
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
		}
	}

	private void AttackAndDef(PlayerBehaviour Npc, GameAction Action){
		switch(Action){
		case GameAction.Def:
			//steal block move

			break;
		case GameAction.Attack:
			if(Npc == ballController){
				//Dunk shoot shoot3 pass move

			}else{
				//move sup push
				float Dis = getDis(ballController, Npc); 
				int supRate = Random.Range(0, 100) + 1;
				int pushRate = Random.Range(0, 100) + 1;
				PlayerBehaviour NearPlayer = HaveNearPlayer(Npc, 1.5f, false);

				if(NearPlayer != null && pushRate <= 50){
					//Push

				}else if(Dis >= 1.5f && Dis <= 3 && supRate <= 50){
					//Sup

				}else{
					//Move
					if(!Npc.Move){
//						int Index = Random.Range(0, MaxPos);
						int Index = Npc.MoveIndex;

						if(Npc.Body == BodyType.Big)
							Npc.TargetPos = new Vector2(BigRunAy[Index].x, BigRunAy[Index].y);
						else if(Npc.Body == BodyType.Mid)
							Npc.TargetPos = new Vector2(MidRunAy[Index].x, MidRunAy[Index].y);
						else if(Npc.Body == BodyType.Small)
							Npc.TargetPos = new Vector2(SmallRunAy[Index].x, SmallRunAy[Index].y);
						Npc.MoveIndex++;
						if(Npc.MoveIndex >= MaxPos)
							Npc.MoveIndex = 0;
					}

					Npc.MoveTo(Npc.TargetPos.x, Npc.TargetPos.y);
				}
			}
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
}
