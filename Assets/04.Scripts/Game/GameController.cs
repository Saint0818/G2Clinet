using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	private const int CountBackSecs = 3;

	public List<PlayerBehaviour> PlayerList = new List<PlayerBehaviour>();
	public PlayerBehaviour ballController;
	public int situation = 0;
	private float Timer = 0;
	private int NoAiTime = 0;
	private float[] XAy = new float[3];
	private float[] ZAy = new float[3];

	// Use this for initialization
	void Start () {
		EasyTouch.On_TouchDown += TouchDown;
		NoAiTime = 0;

		PlayerList.Add (ModelManager.Get.CreatePlayer (0, TeamKind.Self));
		UIGame.Get.targetPlayer = PlayerList [0];
		XAy [0] = 6;
		XAy [1] = 0;
		XAy [2] = -2.9f;

		ZAy [0] = 6.6f;
		ZAy [1] = 0;
		ZAy [2] = 4.19f;
	}

	private void TouchDown (Gesture gesture){
		NoAiTime = CountBackSecs;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - Timer >= 1){
			Timer = Time.time;

			if(NoAiTime > 0){
				NoAiTime--;
				Debug.Log(NoAiTime);
			}
		}

		if (PlayerList.Count > 0) {
			for(int i = 0 ; i < PlayerList.Count; i++){
				PlayerBehaviour Npc = PlayerList[i];

				if(NoAiTime > 0 && Npc.Team == TeamKind.Self && Npc == ballController){
					continue;
				}else{
					//AI
					switch(Npc.crtState){
					case PlayerState.Idle:
//						int a = Random.Range(0, 3);
//						Npc.MoveTo(XAy[a], ZAy[a]);
						break;
					}
				}
			}		
		}
	}
}
