using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Newtonsoft.Json;
using GamePlayStruct;

public class GMToturial : EditorWindow {
	[MenuItem ("GameEditor/GMToturial")]
    private static void PositionEdit() {
		EditorWindow.GetWindowWithRect(typeof(GMToturial), new Rect(0, 0, 1200, 800), true, "GMToturial").Show();
    }

	private string flagID;
	private float lineHeight = 36;
	private float buttonWeight = 160;
	private GUIStyle lebalStyle = new GUIStyle(EditorStyles.textField);
	private GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);

    void OnGUI() {
		lebalStyle.fontSize = 24;
		lebalStyle.normal.textColor = Color.white;
		buttonStyle.fontSize = 18;

		if (GameData.DTutorial.Count > 0) {
			EditorGUILayout.BeginHorizontal();
			buttonStyle.normal.textColor = Color.white;
			EditorGUILayout.LabelField("Flag ID : ", lebalStyle, GUILayout.Width(buttonWeight),GUILayout.Height(lineHeight));
			flagID = EditorGUILayout.TextField(flagID, lebalStyle, GUILayout.Width(buttonWeight),GUILayout.Height(lineHeight));
			if (GUILayout.Button("Test", buttonStyle, GUILayout.Width(buttonWeight), GUILayout.Height(lineHeight))) {
				int id = -1;

				if (int.TryParse(flagID, out id) && GameData.DTutorial.ContainsKey(id * 100 + 1)) 
					UITutorial.Get.ShowTutorial(id, 1);
				else
					Debug.Log("No this flag id.");
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
		}

        EditorGUILayout.BeginHorizontal();
		buttonStyle.normal.textColor = Color.red;
		if (GameData.Team.TutorialFlags != null && GameData.Team.TutorialFlags.Length > 0) {
			if (GUILayout.Button("Clear Tuturial", buttonStyle, GUILayout.Width(buttonWeight), GUILayout.Height(lineHeight))) {
				WWWForm form = new WWWForm();
				SendHttp.Get.Command(URLConst.ClearTutorialFlag, null, form, false);
				Array.Resize(ref GameData.Team.TutorialFlags, 0);
			}
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();

        if (GameData.Team.TutorialFlags != null) {
			for (int i = 0; i < GameData.Team.TutorialFlags.Length; i++) {
				EditorGUILayout.BeginHorizontal();

				int id = GameData.Team.TutorialFlags[i] * 100 + 1;
				if (GameData.DTutorial.ContainsKey(id)) {
					if (GUILayout.Button("Delete", buttonStyle, GUILayout.Width(buttonWeight), GUILayout.Height(lineHeight))) {
						GameData.Team.RemoveTutorialFlag(i);
					}

					EditorGUILayout.LabelField(string.Format("ID.{0} : {1} ", 
					    GameData.DTutorial[id].ID, GameData.DTutorial[id].UIName), lebalStyle, GUILayout.Height(lineHeight));
				}

				EditorGUILayout.EndHorizontal();
			}
        }

        EditorGUILayout.EndHorizontal();
    }
}
