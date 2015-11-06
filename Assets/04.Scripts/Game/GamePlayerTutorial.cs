using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GamePlayStruct;

public class GamePlayerTutorial : KnightSingleton<GamePlayerTutorial> {
	private List<TGamePlayEvent> eventList = new List<TGamePlayEvent>(0);

	public void SetTutorialData(int id) {
		eventList.Clear();
		for (int i = 0; i < GameData.DStageTutorial[id].Events.Length; i++)
			eventList.Add(GameData.DStageTutorial[id].Events[i]);
	}

	public void BegeingEvent() {
		for (int i = 0; i < eventList.Count; i++)
			if (eventList[i].ConditionKind == 0) 
				HandleEvent(ref eventList[i]);
	}

	public void HandleEvent(ref TGamePlayEvent e) {

	}
}
