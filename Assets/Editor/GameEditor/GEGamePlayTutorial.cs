﻿using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using GamePlayStruct;

public class GEGamePlayTutorial : GEBase {
	private string stageID;
	private static string FileName = "";
	private static string BackupFileName = "";
	private List<TStageToturial> toturialData = new List<TStageToturial>(0);

	private Vector2 mScroll = Vector2.zero;


	void OnEnable() {
		FileName = Application.dataPath + "/Resources/GameData/gameplaytutorial.json";
		BackupFileName = Application.dataPath + "/Resources/GameData/Backup/gameplaytutorial_" + DateTime.Now.ToString("MM-dd-yy") + ".json";
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

		if(toturialData.Count > 0 ){
			StyleLabel.normal.textColor = Color.yellow;
			GUILayout.Label("Stage ID : ", StyleLabel, GUILayout.Height(Height_Line));

			StyleLabel.normal.textColor = Color.white;
			mScroll = GUILayout.BeginScrollView(mScroll);
			for (int i = 0; i < toturialData.Count; i++) {
				GUILayout.Space(2);
				if (GUILayout.Button(toturialData[i].ID.ToString(), StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line))) {

				}
			}

			GUILayout.EndScrollView ();
		}
    }

	private void OnLoad() {
		string text = LoadFile(FileName);
		if (!string.IsNullOrEmpty(text)) {
			TStageToturial[] data = (TStageToturial[])JsonConvert.DeserializeObject(text, typeof(TStageToturial[]));
			toturialData.Clear();
			for (int i = 0; i < data.Length; i++)
				toturialData.Add(data[i]);
		}
		/*
		if (File.Exists(FileName)) {
			TextAsset tx = Resources.Load("GameData/gameplaytutorial") as TextAsset;
			if (tx) {
				TGamePlayToturial[] data = (TGamePlayToturial[])JsonConvert.DeserializeObject(tx.text, typeof(TGamePlayToturial[]));
				toturialData.Clear();
				for (int i = 0; i < data.Length; i++)
					toturialData.Add(data[i]);
			} 
		}*/
	}

	public void OnSave() {
		if (toturialData != null && toturialData.Count > 0) {
			if (FileName != string.Empty) {
				SaveFile(FileName, JsonConvert.SerializeObject(toturialData.ToArray()));
				SaveFile(BackupFileName, JsonConvert.SerializeObject(toturialData.ToArray()));

				Debug.Log(FileName);
				Debug.Log(BackupFileName);
			} else
				Debug.LogError("FileName is empty");
		} else
			Debug.LogError("EditIndex error");
	}

	private bool addTutorial(int id) {
		for (int i = 0; i < toturialData.Count; i++)
			if (toturialData[i].ID == id) {
				Debug.LogError("Stage already exists.");
				return false;
			}

		int index = toturialData.Count;
		for (int i = 0; i < toturialData.Count; i++)
			if (id < toturialData[i].ID) {
				index = i;
				break;
			}

		TStageToturial data = new TStageToturial(0);
		data.ID = id;
		toturialData.Insert(index, data);
		OnSave();
		return true;
	}

	public void SetStage(int id) {
		if (id >= 0 && GameData.DStageTutorial.ContainsKey(id)) {

		}
	}
}
