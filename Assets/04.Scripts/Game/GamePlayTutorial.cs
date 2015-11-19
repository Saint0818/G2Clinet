using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GamePlayStruct;

public class GamePlayTutorial : KnightSingleton<GamePlayTutorial> {
	private List<TGamePlayEvent> eventList = new List<TGamePlayEvent>(0);
	public EventDelegate.Callback OnEventEnd = null;
	private CircularSectorMeshRenderer hintArea = null;

	public int NextEventID = 0;
	public int EventSituation = 0;

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
			if (eventList[i].ConditionKind == 0) {
				HandleEvent(i);
				removeEvent(eventList[i].ID);
			}
	}

	public void HandleEvent(int i, GameObject player=null) {
		int otherEventID = 0;

		switch (eventList[i].ConditionKind) {
		case 0:
		case 1:
			otherEventID = eventList[i].OtherEventID;
			break;
		case 2:
			if (GameController.Visible) {
				NextEventID = eventList[i].NextEventID;
				EventSituation = eventList[i].ConditionValue;
			}

			break;
		}

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
				CourtMgr.Get.InitScoreboard(true);
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
			UITutorial.Get.NextEventID = eventList[i].NextEventID;
			break;

		case 7: //turn on / off AI
			if (eventList[i].Value1 > 0)
				GameController.Get.SetPlayerAI(true);
			else {
				GameController.Get.SetPlayerAI(false);
				GameController.Get.ClearAutoFollowTime();
			}

			break;
		case 8:
			if (hintArea == null) {
				GameObject obj = Instantiate(Resources.Load("Effect/RangeOfAction") as GameObject);
				obj.name = "HintArea";
				obj.transform.parent = gameObject.transform;
				EventTrigger et = obj.AddComponent<EventTrigger>();
				et.NextEventID = eventList[i].NextEventID;
				SphereCollider sc = obj.AddComponent<SphereCollider>();
				sc.radius = eventList[i].Value3;
				sc.isTrigger = true;
				Rigidbody rb = obj.AddComponent<Rigidbody>();
				rb.isKinematic = true;
				rb.useGravity = false;
				TweenScale ts = obj.AddComponent<TweenScale>();
				ts.style = UITweener.Style.PingPong;
				ts.from = Vector3.one;
				ts.to = new Vector3(1.2f, 1.2f, 1.2f);
				ts.duration = 0.2f;
				hintArea = obj.GetComponent<CircularSectorMeshRenderer>();
			} else
				hintArea.gameObject.SetActive(true);

			hintArea.transform.position = new Vector3(eventList[i].Value1, 0.1f, eventList[i].Value2);
			hintArea.ChangeValue(360, eventList[i].Value3);

			break;
		case 9:
			if (player != null) {
				PlayerBehaviour p = player.GetComponent<PlayerBehaviour>();
				if (p != null)
					p.SetAnger(eventList[i].Value1, hintArea.gameObject);
			}

			hintArea.gameObject.SetActive(false);

			break;
		}

		if (otherEventID > 0 && otherEventID != eventList[i].ID)
			CheckNextEvent(otherEventID);
	}

	private void removeEvent(int eventID) {
		for (int j = 0; j < eventList.Count; j++)
			if (eventList[j].ID == eventID)
				eventList.RemoveAt(j);
				return;
	}

	public bool CheckNextEvent(int eventID, GameObject player=null) {
		if (eventID > 0) {
			for (int i = 0; i < eventList.Count; i++) {
				if (eventList[i].ID == eventID) {
					HandleEvent(i, player);
					removeEvent(eventID);
					return true;
				}
			}
		}

		return false;
	}

	public bool CheckSituationEvent(int situation) {
		if (NextEventID > 0) {
			for (int i = 0; i < eventList.Count; i++) {
				if (eventList[i].ID == NextEventID && situation == EventSituation) {
					HandleEvent(i, null);
					removeEvent(NextEventID);
					NextEventID = 0;
					EventSituation = 0;
					return true;
				}
			}
		}
		
		return false;
	}
}
