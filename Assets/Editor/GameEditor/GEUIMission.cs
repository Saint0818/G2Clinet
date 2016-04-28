using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using GamePlayStruct;
using GameStruct;

public class GEUIMission : GEBase {
	private int flagID;
    private Vector2 mScroll = Vector2.zero;
    private string[] timeKindText = {"Achievement", "Daily", "Weekly", "Monthly"};

    void OnGUI() {
        EditorGUILayout.BeginHorizontal();
		StyleButton.normal.textColor = Color.red;
		if (GUIButton("Clear All", Color.red))
            sendClearMission(0);

		EditorGUILayout.EndHorizontal();
		GUILayout.Space(2);

        showMission(ref GameData.Team.MissionLv, timeKindText[0]);
        showMission(ref GameData.Team.DailyRecord.MissionLv, timeKindText[1]);
        showMission(ref GameData.Team.WeeklyRecord.MissionLv, timeKindText[2]);
        showMission(ref GameData.Team.MonthlyRecord.MissionLv, timeKindText[3]);
    }

    private void showMission(ref Dictionary<int, int> missionLv, string title) {
        if (missionLv != null) {
            EditorGUILayout.BeginHorizontal();
            GUILabel(title);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            mScroll = GUILayout.BeginScrollView(mScroll);
            foreach (KeyValuePair<int, int> item in missionLv) {
                EditorGUILayout.BeginHorizontal();

                int id = item.Key;
                int lv = item.Value;
                if (GUIButton("Delete"))
                    sendClearMission(id);

                string name = "";
                string timekind = "";
                if (GameData.DMissionData.ContainsKey(id)) {
                    name = GameData.DMissionData[id].Name;
                    if (GameData.DMissionData[id].TimeKind > 0 && GameData.DMissionData[id].TimeKind < timeKindText.Length)
                        timekind = timeKindText[GameData.DMissionData[id].TimeKind];
                }

                GUILabel(string.Format("ID:{0}, Lv:{1}, Name:{3}", id, lv, timekind, name));
                EditorGUILayout.EndHorizontal();
            } 

            GUILayout.EndScrollView ();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(1);
        }
    }

    private void sendClearMission(int id) {
        WWWForm form = new WWWForm();
        form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
        form.AddField("MissionID", id);
        SendHttp.Get.Command(URLConst.ClearMission, waitClearMission, form, true);
    }

    private void waitClearMission(bool ok, WWW www) {
        if (ok) {
            if (!string.IsNullOrEmpty(www.text)) {
                TTeam result = JsonConvertWrapper.DeserializeObject <TTeam>(www.text);
                if (result.MissionLv != null)
                    GameData.Team.MissionLv = result.MissionLv;

                if (result.DailyRecord.MissionLv != null)
                    GameData.Team.DailyRecord.MissionLv = result.DailyRecord.MissionLv;

                if (result.WeeklyRecord.MissionLv != null)
                    GameData.Team.WeeklyRecord.MissionLv = result.WeeklyRecord.MissionLv;

                if (result.MonthlyRecord.MissionLv != null)
                    GameData.Team.MonthlyRecord.MissionLv = result.MonthlyRecord.MissionLv;
            } else {
                if (GameData.Team.MissionLv != null)
                    GameData.Team.MissionLv.Clear();

                if (GameData.Team.DailyRecord.MissionLv != null)
                    GameData.Team.DailyRecord.MissionLv.Clear();

                if (GameData.Team.WeeklyRecord.MissionLv != null)
                    GameData.Team.WeeklyRecord.MissionLv.Clear();

                if (GameData.Team.MonthlyRecord.MissionLv != null)
                    GameData.Team.MonthlyRecord.MissionLv.Clear();
            }
        }
    }
}
