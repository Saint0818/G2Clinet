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
	private int conditionIndex = 0;
	private int conditionValue = 0;
	private int stageIndex = -1;
	private int actionTeam;
	private int actionIndex;
	private int actionMoveKind;

	private TGamePlayEvent[] eventList = new TGamePlayEvent[0];

	private string[] conditionExplain = {
		"0.進入球賽",
		"1.上個事件結束後",
		"2.時間達到",
		"3.我方分數達到",
		"4.對方分數達到"
	};

	private string[] eventExplain = {
		"0.None",
		"1.跳到某個比賽狀態",
		"2.設定球權給某球員",
		"3.設定球員狀態",
		"4.控制球員移動",
		"5.開啟操作介面",
		"6.開啟介面教學"
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

	void OnEnable () { 
		instance = this;
	}

	void OnDisable () {
		stageIndex = -1;
		instance = null; 
	}

    void OnGUI() {
		showManagerButton();
		showEvent();
    }

	private void saveTutorial() {
		TStageToturial st = GameData.StageTutorial[stageIndex];
		st.Events = eventList;
		GameData.StageTutorial[stageIndex] = st;
		GEStageTutorial.OnSave();
	}

	private void showManagerButton() {
		GUILayout.BeginHorizontal();
		GUILabel("Stage ID : " + GameData.StageTutorial[stageIndex].ID.ToString(), Color.yellow);
		eventIndex = GUIPopup(eventIndex, eventExplain, "Kind");
		conditionIndex = GUIPopup(conditionIndex, conditionExplain, "Condition");
		conditionValue = GUIIntEdit(conditionValue, "Value");

		if (GUIButton("New")) {
			TGamePlayEvent e = new TGamePlayEvent(0);
			Array.Resize(ref eventList, eventList.Length+1);

			e.Kind = eventIndex;
			e.ConditionKind = conditionIndex;
			e.ConditionValue = conditionValue;
			eventList[eventList.Length-1] = e;
		}

		if (GUIButton("Save", Color.blue)) {
			saveTutorial();
		}
		
		GUILayout.EndHorizontal();
		GUILayout.Space(spaceCount);
	}

	private void showEvent() {
		if (eventList.Length > 0 ) {
			mScroll = GUILayout.BeginScrollView(mScroll);
			for (int i = 0; i < eventList.Length; i++) {
				GUILayout.Space(spaceCount);
				GUILayout.BeginHorizontal();
				GUILabel(string.Format("Event{0}", i), Color.yellow);

				if (eventList[i].Kind > 0 && eventList[i].Kind <= eventExplain.Length)
					eventList[i].Kind = GUIPopup(eventList[i].Kind, eventExplain, "Kind");
				else 
					GUILabel("Kind " + eventList[i].Kind.ToString(), Color.red);

				if (eventList[i].ConditionKind >= 0 && eventList[i].ConditionKind < conditionExplain.Length)
					eventList[i].ConditionKind = GUIPopup(eventList[i].ConditionKind, conditionExplain, "Condition");
				else 
					GUILabel("ConditionKind " + eventList[i].ConditionKind.ToString(), Color.red);

				eventList[i].ConditionValue = GUIIntEdit(eventList[i].ConditionValue, "Value");

				if (GUIButton("Delete", Color.red)) {
					List<TGamePlayEvent> temp = new List<TGamePlayEvent>(eventList);
					temp.RemoveAt(i);
					eventList = temp.ToArray();
					return;
				}
				
				GUILayout.EndHorizontal();
				GUILayout.Space(spaceCount);

				switch (eventList[i].Kind) {
				case 1: //event kind
					GUILayout.BeginHorizontal();
					eventList[i].Value1 = GUIPopup(eventList[i].Value1, situationExplain, "Situation");
					GUILayout.EndHorizontal();
					break;
				case 2: //set ball
					GUILayout.BeginHorizontal();
					eventList[i].Value1 = GUIIntEdit(eventList[i].Value1, "Team");
					eventList[i].Value2 = GUIIntEdit(eventList[i].Value1, "Index");
					GUILayout.EndHorizontal();
					break;
				case 3: //set player state
					GUILayout.BeginHorizontal();
					eventList[i].Value1 = GUIIntEdit(eventList[i].Value1, "Team");
					eventList[i].Value2 = GUIIntEdit(eventList[i].Value1, "Index");
					GUILayout.EndHorizontal();
					break;
				case 4:
					showPlayerMove(i);
					break;
				case 5: //open ui
					GUILayout.BeginHorizontal();
					eventList[i].Value1 = GUIIntEdit(eventList[i].Value1, "UI Flag");
					GUILayout.EndHorizontal();
					break;
				case 6: //open ui tutorial
					GUILayout.BeginHorizontal();
					eventList[i].Value1 = GUIIntEdit(eventList[i].Value1, "Tutorial ID");
					GUILayout.EndHorizontal();
					break;
				}
			}
			
			GUILayout.EndScrollView();
		}
	}

	//event 3
	private void showPlayerMove(int i) {
		GUILayout.BeginHorizontal();

		actionTeam = GUIIntEdit(actionTeam, "Team");
		actionIndex = GUIIntEdit(actionIndex, "Index");
		actionMoveKind = GUIIntEdit(actionMoveKind, "Move");

		if (GUIButton("Add")) {
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

				GUILabel(string.Format("{0}.{1}-{2}", j+1, eventList[i].Actions[j].Team, eventList[i].Actions[j].Index));
				eventList[i].Actions[j].Action.x = GUIFloatEdit(eventList[i].Actions[j].Action.x, "X");
				eventList[i].Actions[j].Action.z = GUIFloatEdit(eventList[i].Actions[j].Action.z, "Z");
				
				if (GUIButton("Move To") && GameController.Visible)
					GameController.Get.SetPlayerMove(eventList[i].Actions[j].Action, 0);
				
				if (GUIButton("Get Pos") && GameController.Visible) {
					Vector3 Res = GameController.Get.EditGetPosition(0);
					eventList[i].Actions[j].Action.x = Convert.ToSingle(Math.Round(Res.x, 2));
					eventList[i].Actions[j].Action.z = Convert.ToSingle(Math.Round(Res.z, 2));
				}      

				eventList[i].Actions[j].MoveKind = Convert.ToInt32(GUIToggle(Convert.ToBoolean(eventList[i].Actions[j].MoveKind), "Appear"));
				eventList[i].Actions[j].Action.Speedup = GUIToggle(eventList[i].Actions[j].Action.Speedup, "Speed up");
				eventList[i].Actions[j].Action.Shooting = GUIToggle(eventList[i].Actions[j].Action.Shooting, "Shoot");
				//eventList[i].Actions[j].Action.Catcher = EditorGUILayout.Toggle("Catcher", eventList[i].Actions[j].Action.Catcher);

				if (GUIButton("Delete", Color.red)) {
					List<TToturialAction> temp = new List<TToturialAction>(eventList[i].Actions);
					temp.RemoveAt(j);
					eventList[i].Actions = temp.ToArray();
					return;
				}

				GUILayout.EndHorizontal();
			}
		}
	}

	public void SetStage(int index) {
		if (index >= 0 && index < GameData.StageTutorial.Length && stageIndex != index) {
			stageIndex = index;
			Array.Resize(ref eventList, 0);
			Array.Resize(ref eventList, GameData.StageTutorial[index].Events.Length);
			for (int i = 0; i < GameData.StageTutorial[index].Events.Length; i++)
				eventList[i] = GameData.StageTutorial[index].Events[i];

			instance.Focus();
		}
	}
}
