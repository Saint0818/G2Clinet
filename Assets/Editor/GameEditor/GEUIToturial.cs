using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Newtonsoft.Json;
using GamePlayStruct;

public class GEUIToturial : GEBase {
	private string flagID;

	void OnEnable() {

	}

    void OnGUI() {
		if (GameData.DTutorial.Count > 0) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Flag ID : ", StyleLabel, GUILayout.Width(Weight_Button),GUILayout.Height(Height_Line));
			flagID = EditorGUILayout.TextField(flagID, StyleEdit, GUILayout.Width(Weight_Button),GUILayout.Height(Height_Line));
			if (GUILayout.Button("Test", StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line))) {
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
		if (GameData.Team.TutorialFlags != null && GameData.Team.TutorialFlags.Length > 0) {
			StyleButton.normal.textColor = Color.red;
			if (GUILayout.Button("Clear Tutorial", StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line))) {
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
					if (GUILayout.Button("Delete", StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line))) {
						GameData.Team.RemoveTutorialFlag(i);
					}

					EditorGUILayout.LabelField(string.Format("ID.{0} : {1} ", 
					    GameData.DTutorial[id].ID, GameData.DTutorial[id].UIName), StyleLabel, GUILayout.Height(Height_Line));
				}

				EditorGUILayout.EndHorizontal();
			}
        }

        EditorGUILayout.EndHorizontal();
    }
}
