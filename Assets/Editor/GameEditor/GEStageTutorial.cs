using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using GamePlayStruct;

public class GEStageTutorial : GEBase {
	public static GEStageTutorial Get = null;
	private int stageID = 0;
	private static string FileName = "";
	private static string BackupFileName = "";
	private Vector2 mScroll = Vector2.zero;

	void OnDisable () { 
		Get = null; 
	}

	void OnEnable() {
		Get = this;
		FileName = Application.dataPath + "/Resources/GameData/stagetutorial.json";
		BackupFileName = Application.dataPath + "/Resources/GameData/Backup/stagetutorial_" + DateTime.Now.ToString("MM-dd-yy") + ".json";
		OnLoad();
	}

    void OnGUI() {
		GUILayout.BeginHorizontal();
		GUILabel("Stage ID : ", Color.yellow);
		stageID = GUIIntEdit(stageID, "");
		if (GUIButton("Add")) {
			int id = -1;
			
			if (stageID >= GameConst.Default_MainStageID)
				addTutorial(id);
			else
				Debug.Log("ID error.");
		}
		
		GUILayout.EndHorizontal();
		GUILayout.Space(2);

		if (GameData.StageTutorial.Count > 0 ) {
			GUILabel("Stage ID : ", Color.yellow);
			mScroll = GUILayout.BeginScrollView(mScroll);
			for (int i = 0; i < GameData.StageTutorial.Count; i++) {
				GUILayout.Space(2);
				if (GUIButton(GameData.StageTutorial[i].ID.ToString())) {
					GEGamePlayTutorial.Get.SetStage(i);
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

	public static void OnSave() {
		if (GameData.StageTutorial != null && GameData.StageTutorial.Count > 0) {
			if (FileName != string.Empty) {
				SaveFile(FileName, JsonConvert.SerializeObject(GameData.StageTutorial.ToArray()));
				SaveFile(BackupFileName, JsonConvert.SerializeObject(GameData.StageTutorial.ToArray()));

				GameData.DStageTutorial.Clear();
				
				for (int i = 0; i < GameData.StageTutorial.Count; i++) {
					int id = GameData.StageTutorial[i].ID;
					if (!GameData.DStageTutorial.ContainsKey(id)) 
						GameData.DStageTutorial.Add(id, GameData.StageTutorial[i]);
					else 
						Debug.LogError("Stage tutorial key error i : " + i.ToString());
				}

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
