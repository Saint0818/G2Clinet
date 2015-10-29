using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using GamePlayStruct;

public class GEGamePlayTutorial : GEBase {
	private static GEGamePlayTutorial instance = null;
	private int eventIndex = 0;
	private int stageID;
	private int actionTeam;
	private int actionIndex;
	private int actionMoveKind;

	private List<TGamePlayEvent> eventList = new List<TGamePlayEvent>();

	private string[] eventExplain = {"1.掠過開頭動畫，跳到某個比賽狀態",
		"2.設定球權給某球員",
		"3.設定球員狀態",
		"4.控制球員移動",
		"5.開啟操作介面",
		"6.開啟介面教學"};

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

	void OnEnable () { 
		instance = this;
	}

	void OnDisable () { 
		instance = null; 
	}

    void OnGUI() {
		showManagerButton();
		showEvent();
    }

	private void showManagerButton() {
		StyleButton.normal.textColor = Color.white;
		GUILayout.BeginHorizontal();
		GUILayout.Label("Stage ID : " + stageID.ToString(), StyleLabel, GUILayout.Height(Height_Line));
		eventIndex = EditorGUILayout.Popup(eventIndex, eventExplain, StyleButton, GUILayout.Width(Weight_Button * 3), GUILayout.Height(Height_Line));
		if (GUILayout.Button("New Event", StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line))) {
			TGamePlayEvent e = new TGamePlayEvent(0);
			e.Kind = eventIndex+1;
			eventList.Add(e);
		}
		
		if (GUILayout.Button("Save", StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line))) {
		}
		
		GUILayout.EndHorizontal();
		GUILayout.Space(2);
	}

	private void showEvent() {
		if (eventList.Count > 0 ) {
			StyleLabel.normal.textColor = Color.white;
			mScroll = GUILayout.BeginScrollView(mScroll);
			for (int i = 0; i < eventList.Count; i++) {
				GUILayout.Space(2);
				GUILayout.BeginHorizontal();
				StyleLabel.normal.textColor = Color.yellow;
				GUILayout.Label("Event : " + i.ToString(), StyleLabel, GUILayout.Height(Height_Line));
				StyleButton.normal.textColor = Color.red;
				if (GUILayout.Button("Delete", StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line))) {
					eventList.RemoveAt(i);
				}
				
				GUILayout.EndHorizontal();

				if (eventList[i].Kind == 3) {
					GUILayout.Space(2);
					showPlayerMove(i);
				}
			}
			
			GUILayout.EndScrollView();
		}
	}

	private void showPlayerMove(int i) {
		StyleButton.normal.textColor = Color.white;
		GUILayout.BeginHorizontal();

		actionTeam = EditorGUILayout.IntField("Team", actionTeam);
		actionIndex = EditorGUILayout.IntField("Index", actionIndex);
		actionMoveKind = EditorGUILayout.IntField("Move Kind", actionMoveKind);

		if (GUILayout.Button("Add", StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line))) {
			TGamePlayEvent e = eventList[i];
			Array.Resize(ref e.Actions, e.Actions.Length+1);
			e.Actions[e.Actions.Length-1].Team = actionTeam;
			e.Actions[e.Actions.Length-1].Index = actionIndex;
			e.Actions[e.Actions.Length-1].MoveKind = actionMoveKind;
			eventList[i] = e;
		}
		
		GUILayout.EndHorizontal();

		if (eventList[i].Actions != null) {
			for (int j = 0; j < eventList[i].Actions.Length; j++) {
				GUILayout.BeginHorizontal();
				Vector2 v = EditorGUILayout.Vector2Field("(" + (j + 1).ToString() + ")", new Vector2(eventList[i].Actions[j].Action.x, eventList[i].Actions[j].Action.z));
				eventList[i].Actions[j].Action.x = v.x;
				eventList[i].Actions[j].Action.z = v.y;
				
				if (GUILayout.Button("MoveTo_" + (j + 1).ToString(), GUILayout.Height(32)))
					GameController.Get.EditSetMove(eventList[i].Actions[j].Action, 0);
				
				if (GUILayout.Button("Capture Position_" + (j + 1).ToString(), GUILayout.Height(32))) {
					Vector3 Res = GameController.Get.EditGetPosition(0);
					eventList[i].Actions[j].Action.x = Convert.ToSingle(Math.Round(Res.x, 2));
					eventList[i].Actions[j].Action.z = Convert.ToSingle(Math.Round(Res.z, 2));
				}      

				eventList[i].Actions[j].Action.Speedup = EditorGUILayout.Toggle("Speedup", eventList[i].Actions[j].Action.Speedup);
				eventList[i].Actions[j].Action.Shooting = EditorGUILayout.Toggle("Shooting", eventList[i].Actions[j].Action.Shooting);
				//eventList[i].Actions[j].Action.Catcher = EditorGUILayout.Toggle("Catcher", eventList[i].Actions[j].Action.Catcher);

				GUILayout.EndHorizontal();
			}
		}
	}

	public void SetStage(int id) {
		if (id >= 0 && GameData.DStageTutorial.ContainsKey(id)) {
			stageID = id;
			eventList.Clear();
			for (int i = 0; i < GameData.DStageTutorial[id].Events.Length; i++)
				eventList.Add(GameData.DStageTutorial[id].Events[i]);
		}
	}
}
