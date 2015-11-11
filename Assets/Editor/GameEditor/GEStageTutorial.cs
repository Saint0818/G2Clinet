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
	private int delID = 0;
	private int index = -1;
	private static string FileName = "";
	private static string BackupFileName = "";
	private Vector2 mScroll = Vector2.zero;

	void OnDisable () {
		if (!Application.isPlaying && !GEGamePlayTutorial.Visible) {
			GameObject obj = GameObject.Find("FileManager");
			if (obj) {
				DestroyImmediate(obj);
				obj = null;
			}
		}

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
		GUILabel("Open", Color.yellow);
		stageID = GUIIntEdit(stageID, "Stage ID : ");
		if (GUIButton("Add")) {
			if (stageID > 0)
				addTutorial(stageID);
			else
				Debug.LogError("ID error.");
		}

		delID = GUIIntEdit(delID, "DeleteID");
		if (GUIButton("Del", Color.red)) {
			if (!delTutorial(delID))
				Debug.LogError("index error.");
		}

		
		GUILayout.EndHorizontal();
		GUILayout.Space(2);

		if (GameData.StageTutorial.Length > 0 ) {
			mScroll = GUILayout.BeginScrollView(mScroll);
			for (int i = 0; i < GameData.StageTutorial.Length; i++) {
				GUILayout.Space(2);
				GUILayout.BeginHorizontal();
				if (GUIButton(GameData.StageTutorial[i].ID.ToString())) {
					index = i;
					EditorApplication.delayCall += openGamePlayTutorial; 
				}

				if (GUIButton("Test")) {
					index = i;
					EditorApplication.delayCall += onTestStage; 
				}

				GUILayout.EndHorizontal();
			}

			GUILayout.EndScrollView ();
		}
    }

	private void openGamePlayTutorial() {
		GEGamePlayTutorial.Get.SetStage(index);
	}

	private void onTestStage() {
		if (GameStart.Visible) {
			GameData.StageID = GameData.StageTutorial[index].ID;
			SceneMgr.Get.ChangeLevel(ESceneName.SelectRole);
			this.Close();
		} else 
			Debug.LogError("Please run game first.");
	}

	private void OnLoad() {
		string text = LoadFile(FileName);
		if (!string.IsNullOrEmpty(text))
			FileManager.Get.ParseStageTutorialData(BundleVersion.Version.ToString(), text, false);
		else {
			GameData.DStageTutorial.Clear();
			Array.Resize(ref GameData.StageTutorial, 0);
		}
	}

	public static void OnSave() {
		if (GameData.StageTutorial != null && GameData.StageTutorial.Length > 0) {
			if (FileName != string.Empty) {
				SaveFile(FileName, JsonConvert.SerializeObject(GameData.StageTutorial));
				SaveFile(BackupFileName, JsonConvert.SerializeObject(GameData.StageTutorial));

				GameData.DStageTutorial.Clear();
				
				for (int i = 0; i < GameData.StageTutorial.Length; i++) {
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
		for (int i = 0; i < GameData.StageTutorial.Length; i++)
			if (GameData.StageTutorial[i].ID == id) {
				Debug.LogError("Stage already exists.");
				return false;
			}

		int index = GameData.StageTutorial.Length;
		for (int i = 0; i < GameData.StageTutorial.Length; i++)
			if (id < GameData.StageTutorial[i].ID) {
				index = i;
				break;
			}

		TStageToturial data = new TStageToturial(0);
		data.ID = id;

		List<TStageToturial> temp = new List<TStageToturial>(GameData.StageTutorial);
		temp.Insert(index, data);
		GameData.StageTutorial = temp.ToArray();
		OnSave();
		return true;
	}

	private bool delTutorial(int id) {
		int index = -1;
		for (int i = 0; i < GameData.StageTutorial.Length; i++)
		if (id == GameData.StageTutorial[i].ID) {
			index = i;
			break;
		}

		if (index >= 0 && index < GameData.StageTutorial.Length) {
			List<TStageToturial> temp = new List<TStageToturial>(GameData.StageTutorial);
			temp.RemoveAt(index);
			GameData.StageTutorial = temp.ToArray();
			OnSave();
			return true;
		} else
			return false;
	}
}
