using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Newtonsoft.Json;
using GamePlayStruct;

public class GEUIToturial : GEBase {
	private int flagID;
    private Vector2 mScroll = Vector2.zero;

    void OnGUI() {
		if (GameData.DTutorial.Count > 0) {
			EditorGUILayout.BeginHorizontal();
			flagID = GUIIntEdit(flagID, "Flag ID : ");
			if (GUIButton("Test")) {
				if (GameData.DTutorial.ContainsKey(flagID * 100 + 1)) 
					UITutorial.Get.ShowTutorial(flagID, 1);
				else
					Debug.Log("No this flag id.");
			}

			EditorGUILayout.EndHorizontal();
			GUILayout.Space(2);
		}

        EditorGUILayout.BeginHorizontal();
		if (GameData.Team.TutorialFlags != null && GameData.Team.TutorialFlags.Length > 0) {
			StyleButton.normal.textColor = Color.red;
			if (GUIButton("Clear All", Color.red)) {
				WWWForm form = new WWWForm();
				SendHttp.Get.Command(URLConst.ClearTutorialFlag, null, form, false);
				Array.Resize(ref GameData.Team.TutorialFlags, 0);
			}
		}

		EditorGUILayout.EndHorizontal();
		GUILayout.Space(2);
		EditorGUILayout.BeginHorizontal();

        if (GameData.Team.TutorialFlags != null) {
            mScroll = GUILayout.BeginScrollView(mScroll);
			for (int i = 0; i < GameData.Team.TutorialFlags.Length; i++) {
				EditorGUILayout.BeginHorizontal();

				int id = GameData.Team.TutorialFlags[i] * 100 + 1;
				if (GameData.DTutorial.ContainsKey(id)) {
					if (GUIButton("Delete")) {
						GameData.Team.RemoveTutorialFlag(i);
					}

					GUILabel(string.Format("ID.{0} : {1} ", GameData.DTutorial[id].ID, GameData.DTutorial[id].UIName));
				}

				EditorGUILayout.EndHorizontal();
			}

            GUILayout.EndScrollView ();
        }

        EditorGUILayout.EndHorizontal();
    }
}
