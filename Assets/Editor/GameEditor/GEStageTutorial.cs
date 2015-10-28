using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using GamePlayStruct;

public class GEStageTutorial : GEBase {
	private string stageID;
	private static string FileName = "";
	private static string BackupFileName = "";
	private Vector2 mScroll = Vector2.zero;

	void OnEnable() {
		FileName = Application.dataPath + "/Resources/GameData/stagetutorial.json";
		BackupFileName = Application.dataPath + "/Resources/GameData/Backup/stagetutorial_" + DateTime.Now.ToString("MM-dd-yy") + ".json";
		OnLoad();
	}

    void OnGUI() {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Stage ID : ", StyleLabel, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line));
		stageID = EditorGUILayout.TextField(stageID, StyleEdit, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line));
		if (GUILayout.Button("Add", StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line))) {
			int id = -1;
			
			if (int.TryParse(stageID, out id) && id >= GameConst.Default_MainStageID)
				addTutorial(id);
			else
				Debug.Log("ID error.");
		}
		
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(2);

		if(GameData.StageTutorial.Count > 0 ){
			StyleLabel.normal.textColor = Color.yellow;
			GUILayout.Label("Stage ID : ", StyleLabel, GUILayout.Height(Height_Line));

			StyleLabel.normal.textColor = Color.white;
			mScroll = GUILayout.BeginScrollView(mScroll);
			for (int i = 0; i < GameData.StageTutorial.Count; i++) {
				GUILayout.Space(2);
				if (GUILayout.Button(GameData.StageTutorial[i].ID.ToString(), StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line))) {

				}
			}

			GUILayout.EndScrollView ();
		}
    }

	private void OnLoad() {
		string text = LoadFile(FileName);
		if (!string.IsNullOrEmpty(text))
			FileManager.Get.ParseStageTutorialData(BundleVersion.Version.ToString(), text, false);
	}

	public void OnSave() {
		if (GameData.StageTutorial != null && GameData.StageTutorial.Count > 0) {
			if (FileName != string.Empty) {
				SaveFile(FileName, JsonConvert.SerializeObject(GameData.StageTutorial.ToArray()));
				SaveFile(BackupFileName, JsonConvert.SerializeObject(GameData.StageTutorial.ToArray()));

				Debug.Log(FileName);
				Debug.Log(BackupFileName);
			} else
				Debug.LogError("FileName is empty");
		} else
			Debug.LogError("EditIndex error");
	}

	private bool addTutorial(int id) {
		for (int i = 0; i < GameData.StageTutorial.Count; i++)
			if (GameData.StageTutorial[i].ID == id) {
				Debug.LogError("Stage already exists.");
				return false;
			}

		int index = GameData.StageTutorial.Count;
		for (int i = 0; i < GameData.StageTutorial.Count; i++)
			if (id < GameData.StageTutorial[i].ID) {
				index = i;
				break;
			}

		TStageToturial data = new TStageToturial(0);
		data.ID = id;
		GameData.StageTutorial.Insert(index, data);
		OnSave();
		return true;
	}
}
