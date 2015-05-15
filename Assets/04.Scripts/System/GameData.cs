using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameStruct;

public static class SettingText {
	public const string TeamSave = "TeamSave";
	public const string Language = "UserLanguage";
	public const string AITime = "AIChangeTime";
	public const string Effect = "Effect";
}

public class GameData {
	public static TAIlevel[] AIlevelAy;
	public static TTactical[] TacticalData;
    public static BasketShootPositionData[] BasketShootPosition;

	public static string ServerVersion;
	public static string SaveVersion;
	public static TTeam Team;
	public static TGameSetting Setting;

	private static bool isLoaded = false;
	public static void Init()
	{
		if (!isLoaded) {
			isLoaded = true;
			List<TDownloadData> DownloadList = new List<TDownloadData> ();
			//ailevel
			DownloadList.Add (new TDownloadData ("ailevel", "0"));
			//tactical
			DownloadList.Add (new TDownloadData ("tactical", "0"));
			//basketShootPosition
			DownloadList.Add (new TDownloadData ("ballposition", "0"));

			FileManager.Get.LoadFileResource (DownloadList);

			loadGameSetting();

			Team.Init();
		}
	}

	private static void loadGameSetting() {
		if (PlayerPrefs.HasKey (SettingText.Language)) {
			int temp = Convert.ToInt16(PlayerPrefs.GetString(SettingText.Language));

			if(temp == Language.TW.GetHashCode())
				Setting.Language = Language.TW;
			else
				Setting.Language = Language.EN;
		} else {
			#if UNITY_EDITOR
				#if En
				GameData.GameSetting.Language = Language.EN;
				#endif
				
				#if zh_TW
				GameData.Setting.Language = Language.TW;
				#endif
			#else
			switch (Application.systemLanguage) {
			case SystemLanguage.Chinese:
				GameData.GameSetting.Language = Language.TW;
				break;
			default:
				GameData.GameSetting.Language = Language.EN;
				break;
			}
			#endif
		}

		if(PlayerPrefs.HasKey(SettingText.AITime))
			Setting.AIChangeTime = PlayerPrefs.GetFloat(SettingText.AITime, 1);
		else
			Setting.AIChangeTime = 1;

		if(PlayerPrefs.HasKey(SettingText.Effect))
			Setting.Effect = Convert.ToBoolean(PlayerPrefs.GetInt(SettingText.Effect, 1));
		else
			Setting.Effect = true;
	}

	public static bool LoadTeamSave() {
		if(PlayerPrefs.HasKey(SettingText.TeamSave)) {
			string save = PlayerPrefs.GetString(SettingText.TeamSave, "");
			if (save != "") {
				try {
					Team = JsonConvert.DeserializeObject <TTeam>(save);
					return true;
				} catch (Exception e) {
					Debug.Log(e.ToString());
				}
			}
		}
			
		return false;
	}
}
