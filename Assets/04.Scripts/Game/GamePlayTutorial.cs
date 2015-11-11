using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GamePlayStruct;

public class GamePlayTutorial : KnightSingleton<GamePlayTutorial> {
	private List<TGamePlayEvent> eventList = new List<TGamePlayEvent>(0);
	public EventDelegate.Callback OnEventEnd = null;

	public void SetTutorialData(int id) {
		eventList.Clear();
		if (true) {//GameData.ServerVersion == BundleVersion.Version) {
			TGamePlayEvent[] temp = GameData.DStageTutorial[id].Events;
			for (int i = temp.Length-1; i >= 0; i--) {
				temp[i].ID = i;
				eventList.Add(temp[i]);
			}

			BegeingEvent();
		} else
			Debug.LogError("Client and server version are not the same. Do not play tutorial");
	}

	public void BegeingEvent() {
		for (int i = eventList.Count-1; i >= 0; i--)
			if (eventList[i].ConditionKind == 0) 
				if (HandleEvent(i))
					eventList.RemoveAt(i);
	}

	public bool HandleEvent(int i) {
		int nextEventID = 0;
		
		if (eventList[i].ConditionKind == 0 || eventList[i].ConditionKind == 1)
			nextEventID = eventList[i].ConditionValue;

		switch (eventList[i].Kind) {
		case 1: //change situation
			EGameSituation situation = (EGameSituation)(eventList[i].Value1);
			GameController.Get.InitIngameAnimator();
			GameController.Get.SetBornPositions();
			GameController.Get.ChangeSituation(situation);
			AIController.Get.ChangeState(situation);
			CameraMgr.Get.ShowPlayerInfoCamera (true);
			break;
		case 2: //set ball to player
			if (!GameController.Get.IsStart) {
				GameController.Get.IsStart = true;
				GameController.Get.StartGame(false);
			}

			UIGame.Get.CloseStartButton();
			GameController.Get.SetBall(eventList[i].Value1, eventList[i].Value2);

			break;
		case 3: //set state to player

			break;
		case 4: //set position to player
			for (int j = 0; j < eventList[i].Actions.Length; j++) {
				GameController.Get.SetPlayerAppear(ref eventList[i].Actions[j]); 
			}

			break;
		case 5: //show ui
			UIGame.Get.TutorialUI(eventList[i].Value1);
			break;

		case 6: //open ui tutorial
			UITutorial.Get.ShowTutorial(eventList[i].Value1, 1);
			UITutorial.Get.NextEventID = nextEventID;
			break;

		case 7: //turn on / off AI
			if (eventList[i].Value1 > 0)
				GameController.Get.SetPlayerAI(true);
			else {
				GameController.Get.SetPlayerAI(false);
				GameController.Get.ClearAutoFollowTime();
			}

			break;
		}

		return true;
	}

	public void CheckNextEvent(int eventID) {
		for (int i = 0; i < eventList.Count; i++)
			if (eventList[i].ID == eventID) {
				HandleEvent(i);
				return;
			}
	}
}
