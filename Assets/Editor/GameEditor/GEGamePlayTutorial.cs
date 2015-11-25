using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using GamePlayStruct;

public class GEGamePlayTutorial : GEBase {
	private static GEGamePlayTutorial instance = null;
	private const int spaceCount = 5;

	private int eventIndex = 0;
	private int conditionKind = 0;
	private int conditionValue = 0;
	private int nextEventID = 0;
	private int stageIndex = -1;
	private int actionTeam;
	private int actionIndex;
	private int actionMoveKind;

	private TGamePlayEvent[] events = new TGamePlayEvent[0];

	private string[] conditionExplain = {
		"0.進入球賽",
		"1.事件連結",
		"2.比賽階段變化",
		"3.時間達到",
		"4.我方分數達到",
		"5.對方分數達到"
	};

	private string[] eventExplain = {
		"0.None",
		"1.跳到某個比賽狀態",
		"2.設定球權給某球員",
		"3.設定球員狀態",
		"4.控制球員移動",
		"5.開啟按鈕",
		"6.開啟介面教學",
		"7.開關AI",
		"8.提示場上目的座標",
		"9.增加士氣"
	};

	private string[] situationExplain = {
		"0.None",
		"1.開球",
		"2.跳球",
		"3.玩家進攻",
		"4.電腦進攻",
		"5.玩家撿球",
		"6.玩家發球",
		"7.電腦撿球",
		"8.電腦發球",
		"9.比賽結束",
		"10.球員得分後特殊演出",
		"11.比賽中觸發事件"
	};

	private Vector2 mScroll = Vector2.zero;
	
	public static GEGamePlayTutorial Get
	{
		get {
			if (instance == null) {
				instance = GEGamePlayTutorial.GetWindow<GEGamePlayTutorial>(true, "Game Play Tutorial");
				instance.SetStyle();
			}

			return instance;
		}
	}
	public static bool Visible {
		get {
			return instance != null;
		}
	}

	void OnEnable () { 
		instance = this;
	}

	void OnDisable () {
		if (!Application.isPlaying && GEStageTutorial.Get == null) {
			GameObject obj = GameObject.Find("FileManager");
			if (obj) {
				DestroyImmediate(obj);
				obj = null;
			}
		}

		stageIndex = -1;
		instance = null; 
	}

    void OnGUI() {
		showManagerButton();
		showEvent();
    }

	private void saveTutorial() {
		TStageToturial st = GameData.StageTutorial[stageIndex];
		st.Events = events;
		GameData.StageTutorial[stageIndex] = st;
		GEStageTutorial.OnSave();
	}

	private void showManagerButton() {
		GUILayout.BeginHorizontal();
		if (stageIndex >= 0 && stageIndex < GameData.StageTutorial.Length)
			GUILabel("Stage ID : " + GameData.StageTutorial[stageIndex].ID.ToString(), Color.yellow);

		eventIndex = GUIPopup(eventIndex, eventExplain, "Kind");
		conditionKind = GUIPopup(conditionKind, conditionExplain, "Condition");
		if (GUIButton("New")) {
			TGamePlayEvent e = new TGamePlayEvent(0);
			Array.Resize(ref events, events.Length+1);

			e.Kind = eventIndex;
			e.ConditionKind = conditionKind;
			e.OtherEventID = conditionValue;
			events[events.Length-1] = e;
		}

		if (GUIButton("Save", Color.blue)) {
			saveTutorial();
		}
		
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceCount);
	}

	private void exchangeItem(int s, int t) {
		if (s >= 0 && s < events.Length && t >= 0 && t < events.Length && s != t) {
			TGamePlayEvent e = events[s];
			events[s] = events[t];
			events[t] = e;
		}
	}

	private void showEvent() {
		if (events.Length > 0 ) {
			mScroll = GUILayout.BeginScrollView(mScroll);
			for (int i = 0; i < events.Length; i++) {
				GUILayout.Space(spaceCount);
				GUILayout.BeginHorizontal();
				GUILabel(string.Format("Event{0}", i), Color.yellow);

				if (GUIButton("↑", Width_Button / 3)) {
					exchangeItem(i, i-1);
					return;
				}

				if (GUIButton("↓", Width_Button / 3)) {
					exchangeItem(i, i+1);
					return;
				}

				if (events[i].Kind >= 0 && events[i].Kind <= eventExplain.Length)
					events[i].Kind = GUIPopup(events[i].Kind, eventExplain, "Kind");
				else 
					GUILabel("Kind " + events[i].Kind.ToString(), Color.red);

				if (events[i].ConditionKind >= 0 && events[i].ConditionKind < conditionExplain.Length)
					events[i].ConditionKind = GUIPopup(events[i].ConditionKind, conditionExplain, "Condition");
				else 
					GUILabel("Condition " + events[i].ConditionKind.ToString(), Color.red);

				if (events[i].ConditionKind > 0)
					events[i].OtherEventID = GUIIntEdit(events[i].OtherEventID, "OtherID");

				if (events[i].ConditionKind == 2)
					events[i].ConditionValue = GUIPopup(events[i].ConditionValue, situationExplain, "Situation");

				if (events[i].ConditionKind == 2 || events[i].Kind == 6 || events[i].Kind == 8)
					events[i].NextEventID = GUIIntEdit(events[i].NextEventID, "NextID");

				if (GUIButton("Delete", Color.red)) {
					List<TGamePlayEvent> temp = new List<TGamePlayEvent>(events);
					temp.RemoveAt(i);
					events = temp.ToArray();
					return;
				}
				
				GUILayout.EndHorizontal();
				GUILayout.Space(spaceCount);


				if (events[i].Kind != 4)
					GUILayout.BeginHorizontal(StyleLayout);

				switch (events[i].Kind) {
				case 1: //event kind
					events[i].Value1 = GUIPopup(events[i].Value1, situationExplain, "Situation");
					break;
				case 2: //set ball
					events[i].Value1 = GUIIntEdit(events[i].Value1, "Team");
					events[i].Value2 = GUIIntEdit(events[i].Value1, "Index");
					break;
				case 3: //set player state
					events[i].Value1 = GUIIntEdit(events[i].Value1, "Team");
					events[i].Value2 = GUIIntEdit(events[i].Value1, "Index");
					break;
				case 4:
					showPlayerMove(i);
					break;
				case 5: //open ui
					events[i].Value1 = GUIIntEdit(events[i].Value1, "UI Flag", Width_Button * 1.5f);
					break;
				case 6: //open ui tutorial
					events[i].Value1 = GUIIntEdit(events[i].Value1, "Tutorial ID", Width_Button);
					break;
				case 7: //switch AI
					events[i].Value1 = GUIIntEdit(events[i].Value1, "Turn on AI");
					break;
				case 8: //hint area
					events[i].Value1 = GUIIntEdit(events[i].Value1, "X");
					events[i].Value2 = GUIIntEdit(events[i].Value2, "X");
					events[i].Value3 = GUIIntEdit(events[i].Value3, "Distance");
					events[i].ConditionOperator = GUIIntEdit(events[i].ConditionOperator, "Target");
					break;
				case 9: //power
					events[i].Value1 = GUIIntEdit(events[i].Value1, "Power");
					break;
				}

				if (events[i].Kind != 4)
					GUILayout.EndHorizontal();
			}

			GUILayout.EndScrollView();
		}
	}

	//event 3
	private void showPlayerMove(int i) {
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Move", GUILayout.Width(200)) && GameController.Visible) {
			for (int j = 0; j < events[i].Actions.Length; j++)
			{
				GameController.Get.SetPlayerMove(events[i].Actions[j].Action, events[i].Actions[j].Team * 3 + events[i].Actions[j].Index);
			}
		}

		actionTeam = GUIIntEdit(actionTeam, "Team");
		actionIndex = GUIIntEdit(actionIndex, "Index");
		actionMoveKind = GUIIntEdit(actionMoveKind, "Move");

		if (GUIButton("Add")) {
			TGamePlayEvent e = events[i];
			Array.Resize(ref e.Actions, e.Actions.Length+1);
			e.Actions[e.Actions.Length-1].Team = actionTeam;
			e.Actions[e.Actions.Length-1].Index = actionIndex;
			e.Actions[e.Actions.Length-1].MoveKind = actionMoveKind;
			events[i] = e;
		}
		
		GUILayout.EndHorizontal();

		if (events[i].Actions != null) {
			for (int j = 0; j < events[i].Actions.Length; j++) {
				GUILayout.BeginHorizontal();

				GUILabel(string.Format("{0}.{1}-{2}", j+1, events[i].Actions[j].Team, events[i].Actions[j].Index));
				events[i].Actions[j].Action.x = GUIFloatEdit(events[i].Actions[j].Action.x, "X");
				events[i].Actions[j].Action.z = GUIFloatEdit(events[i].Actions[j].Action.z, "Z");
				
				if (GUIButton("Move To") && GameController.Visible)
					GameController.Get.SetPlayerMove(events[i].Actions[j].Action, 0, false);
				
				if (GUIButton("Get Pos") && GameController.Visible) {
					Vector3 Res = GameController.Get.EditGetPosition(0);
					events[i].Actions[j].Action.x = Convert.ToSingle(Math.Round(Res.x, 2));
					events[i].Actions[j].Action.z = Convert.ToSingle(Math.Round(Res.z, 2));
				}      

				events[i].Actions[j].MoveKind = Convert.ToInt32(GUIToggle(Convert.ToBoolean(events[i].Actions[j].MoveKind), "Appear"));
				events[i].Actions[j].Action.Speedup = GUIToggle(events[i].Actions[j].Action.Speedup, "Speed up");
				events[i].Actions[j].Action.Shooting = GUIToggle(events[i].Actions[j].Action.Shooting, "Shoot");
				//eventList[i].Actions[j].Action.Catcher = EditorGUILayout.Toggle("Catcher", eventList[i].Actions[j].Action.Catcher);

				if (GUIButton("Delete", Color.red)) {
					List<TToturialAction> temp = new List<TToturialAction>(events[i].Actions);
					temp.RemoveAt(j);
					events[i].Actions = temp.ToArray();
					return;
				}

				GUILayout.EndHorizontal();
			}
		}
	}

	public void SetStage(int index) {
		if (index >= 0 && index < GameData.StageTutorial.Length && stageIndex != index) {
			stageIndex = index;
			Array.Resize(ref events, 0);
			Array.Resize(ref events, GameData.StageTutorial[index].Events.Length);
			for (int i = 0; i < GameData.StageTutorial[index].Events.Length; i++)
				events[i] = GameData.StageTutorial[index].Events[i];

			instance.Focus();
		}
	}
}
