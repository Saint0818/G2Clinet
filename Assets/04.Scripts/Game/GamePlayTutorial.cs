using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GamePlayStruct;

public class TEventTrigger {
	public GameObject Item;
	public EventTrigger eventTrigger;
	public SphereCollider sphereCollider;
	public Rigidbody rigidbody;
	public TweenScale tweenScale;
	public CircularSectorMeshRenderer circularSectorMeshRenderer;
}

public class GamePlayTutorial : KnightSingleton<GamePlayTutorial> {
	private List<TGamePlayEvent> eventList = new List<TGamePlayEvent>(0);
	public EventDelegate.Callback OnEventEnd = null;
	private TEventTrigger eventTrigger = new TEventTrigger();

	public int CurrentEventID = 0;
	public int NextEventID = 0;
	public int EventValue = 0;
	public int EventPlayer = -2;
	public int BallOwnerTeam = -1;
	public int BallOwnerIndex = -1;
	private int[] talkManID = new int[2];
	private TToturialAction[] moveActions;
	private List<int> talkManList = new List<int>();

	void OnDestroy() {
		if (eventTrigger.Item)
			Destroy(eventTrigger.Item);
	}

	public void SetTutorialData(int id) {
		CurrentEventID = 0;
		NextEventID = 0;
		EventValue = 0;
		EventPlayer = -2;
		BallOwnerTeam = -1;
		BallOwnerIndex = -1;

		bool hasUIToturial = false;
		talkManList.Clear();
		eventList.Clear();
		if (true) {//GameData.ServerVersion == BundleVersion.Version) {
			TGamePlayEvent[] temp = GameData.DStageTutorial[id].Events;
			for (int i = temp.Length-1; i >= 0; i--) {
				temp[i].ID = i;
				eventList.Add(temp[i]);

				if (temp[i].Kind == 5) {
					hasUIToturial = true;
					GameFunction.FindTalkManID(temp[i].Value1, ref talkManID);
					for (int j = 0; j < talkManID.Length; j++)
						if (GameData.DPlayers.ContainsKey(talkManID[j]))
							talkManList.Add(talkManID[j]);
				}
			}

			if (hasUIToturial) {
				UI3DTutorial.UIShow(true);
				UI3DTutorial.UIShow(false);
				UITutorial.UIShow(true);
				UITutorial.UIShow(false);
				int[] ay = talkManList.ToArray();
				ModelManager.Get.LoadAllSelectPlayer(ref ay);
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
		CurrentEventID = eventList[i].ID;
		int otherEventID = 0;

		switch (eventList[i].ConditionKind) {
		case 1:
			otherEventID = eventList[i].OtherEventID;
			break;
		case 2:
			if (GameController.Visible) {
				NextEventID = eventList[i].NextEventID;
				EventValue = eventList[i].ConditionValue;
			}

			break;
		case 3:
			if (GameController.Visible) {
				NextEventID = eventList[i].NextEventID;
				EventPlayer = eventList[i].ConditionValue * GameData.Max_GamePlayer + eventList[i].ConditionValue2;
			}

			break;
		}

		switch (eventList[i].Kind) {
		case 1: //change situation
			EGameSituation situation = (EGameSituation)(eventList[i].Value1);
			GameController.Get.InitIngameAnimator();
			GameController.Get.SetBornPositions();
			GameController.Get.ChangeSituation(situation);

			if (GameController.Get.IsStart)
				AIController.Get.ChangeState(situation);

			CameraMgr.Get.ShowPlayerInfoCamera (true);
			break;
		case 2: //set ball to player
			BallOwnerTeam = eventList[i].Value1;
			BallOwnerIndex = eventList[i].Value2;
			StartCoroutine(setBall(i));

			break;
		case 3: //set state to player

			break;
		case 4: //set position to player
			moveActions = eventList[i].Actions;
			StartCoroutine(setPlayerMove(i));

			break;
		case 5: //show ui
			UIGame.Get.TutorialUI(eventList[i].Value1);

			break;

		case 6: //open ui tutorial
			//StartCoroutine(openTutorial(eventList[i].Value1, eventList[i].NextEventID));
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
			if (eventTrigger.Item == null) {
				eventTrigger.Item = Instantiate(Resources.Load("Effect/RangeOfAction") as GameObject);
				eventTrigger.Item.name = "HintArea";
				eventTrigger.Item.transform.parent = gameObject.transform;

				eventTrigger.eventTrigger = eventTrigger.Item.AddComponent<EventTrigger>();
				eventTrigger.sphereCollider = eventTrigger.Item.AddComponent<SphereCollider>();
				eventTrigger.rigidbody = eventTrigger.Item.AddComponent<Rigidbody>();
				eventTrigger.tweenScale = eventTrigger.Item.AddComponent<TweenScale>();
				eventTrigger.circularSectorMeshRenderer = eventTrigger.Item.GetComponent<CircularSectorMeshRenderer>();
			} else
				eventTrigger.Item.gameObject.SetActive(true);

			eventTrigger.eventTrigger.NextEventID = eventList[i].NextEventID;
			eventTrigger.sphereCollider.radius = eventList[i].Value3;
			eventTrigger.sphereCollider.isTrigger = true;
			eventTrigger.rigidbody.isKinematic = true;
			eventTrigger.rigidbody.useGravity = false;
			eventTrigger.tweenScale.style = UITweener.Style.PingPong;
			eventTrigger.tweenScale.from = Vector3.one;
			eventTrigger.tweenScale.to = new Vector3(1.2f, 1.2f, 1.2f);
			eventTrigger.tweenScale.duration = 0.2f;

			eventTrigger.circularSectorMeshRenderer.transform.position = new Vector3(eventList[i].Value1, 0.1f, eventList[i].Value2);
			eventTrigger.circularSectorMeshRenderer.ChangeValue(360, eventList[i].Value3);
			EventValue = eventList[i].ConditionValue * GameData.Max_GamePlayer + eventList[i].ConditionValue2;
			break;
		case 9:
			if (player != null) {
				PlayerBehaviour p = player.GetComponent<PlayerBehaviour>();
				if (p != null)
					p.SetAnger(eventList[i].Value1, eventTrigger.Item);
			}

			eventTrigger.Item.SetActive(false);

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

	IEnumerator setBall(int i) {
		yield return new WaitForEndOfFrame();

		if (!GameController.Get.IsStart) {
			GameController.Get.IsStart = true;
			GameController.Get.StartGame(false);
			CourtMgr.Get.InitScoreboard(true);
		}
		
		UIGame.Get.CloseStartButton();
		GameController.Get.SetBall(BallOwnerTeam, (EPlayerPostion)BallOwnerIndex);
	}

	IEnumerator setPlayerMove(int i) {
		yield return new WaitForEndOfFrame();

		if (moveActions != null) {
			for (int j = 0; j < moveActions.Length; j++) {
				GameController.Get.SetPlayerAppear(ref moveActions[j]); 
			}
		}

		moveActions = null;
	}

	IEnumerator openTutorial(int id, int nextID) {
		yield return new WaitForSeconds(1);
		UITutorial.Get.ShowTutorial(id, 1);
		UITutorial.Get.NextEventID = nextID;
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

	public bool CheckTriggerEvent(int eventID, GameObject player=null) {
		if (eventID > 0 && GameController.Visible && GameController.Get.IsStart) {
			for (int i = 0; i < eventList.Count; i++) {
				if (eventList[i].ID == eventID) {
					bool flag = true;
					
					//Moving to specific position. Have to check target index.
					if (EventValue >= 0) {
						PlayerBehaviour p = player.GetComponent<PlayerBehaviour>();
						if (p) {
							int team = EventValue / GameData.Max_GamePlayer;
							int index = EventValue % GameData.Max_GamePlayer;
							if (p.Team.GetHashCode() != team || p.Index.GetHashCode() != index)
								flag = false;
						}
					}
					
					if (flag) {
						HandleEvent(i, player);
						removeEvent(eventID);
						
						return true;
					}
				}
			}
		}
		
		return false;
	}

	public bool CheckSituationEvent(int situation) {
		if (NextEventID > 0) {
			for (int i = 0; i < eventList.Count; i++) {
				if (eventList[i].ID == NextEventID && situation == EventValue) {
					HandleEvent(i, null);
					removeEvent(NextEventID);
					NextEventID = 0;
					EventValue = 0;
					return true;
				}
			}
		}
		
		return false;
	}

	public bool CheckSetBallEvent(PlayerBehaviour player=null) {
		if (EventPlayer >= -1 && NextEventID > 0 && player && GameController.Visible && GameController.Get.IsStart) {
			for (int i = 0; i < eventList.Count; i++) {
				if (eventList[i].ID == NextEventID) {
					bool flag = true;
					
					//Moving to specific position. Have to check target index.
					if (EventPlayer >= 0) {
						int team = EventPlayer / GameData.Max_GamePlayer;
						int index = EventPlayer % GameData.Max_GamePlayer;
						if (player.Team.GetHashCode() != team || player.Index.GetHashCode() != index)
							flag = false;
					}
					
					if (flag) {
						EventPlayer = -2;
						HandleEvent(i, player.PlayerRefGameObject);
						removeEvent(NextEventID);
						
						return true;
					}
				}
			}
		}
		
		return false;
	}
}
