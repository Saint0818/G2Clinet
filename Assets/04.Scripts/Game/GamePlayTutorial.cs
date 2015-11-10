using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GamePlayStruct;

public class GamePlayTutorial : KnightSingleton<GamePlayTutorial> {
	private List<TGamePlayEvent> eventList = new List<TGamePlayEvent>(0);

	public void SetTutorialData(int id) {
		eventList.Clear();
		TGamePlayEvent[] temp = GameData.DStageTutorial[id].Events;
		for (int i = temp.Length-1; i >= 0; i--)
			eventList.Add(temp[i]);

		BegeingEvent();
	}

	public void BegeingEvent() {
		for (int i = eventList.Count-1; i >= 0; i--)
			if (eventList[i].ConditionKind == 0) 
				if (HandleEvent(i))
					eventList.RemoveAt(i);
	}

	public bool HandleEvent(int i) {
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
			if (!GameController.Get.IsStart)
				GameController.Get.StartGame();

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


			break;
		}

		return true;
	}
}
