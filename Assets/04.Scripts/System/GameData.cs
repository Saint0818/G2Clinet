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
	public const string GameRecord = "GameRecord";
	public const string GameRecordStart = "GameRecordStart";
	public const string GameRecordEnd = "GameRecordEnd";
}

public class GameData {
	public static Dictionary<int, TGreatPlayer> DPlayers = new Dictionary<int, TGreatPlayer> ();
	public static TPlayerAttribute[] BaseAttr;
	public static TTactical[] TacticalData;

    /// <summary>
    /// <para> key: GameConst.TacticalDataName 的索引值. </para>
    /// <para> Value: GameData.TacticalData 的索引值. </para>
    /// <para> 原作者的用意是用 [a][b] 的方式取出 TacticalData 的某筆戰術. 這個意思就是亂數找出某個情況下的戰術. </para>
    /// <para> a: 會是遊戲中的某個情況, 比如跳球, 邊界發球等等. </para>
    /// <para> b 通常都是亂數. </para>
    /// </summary>
	public static Dictionary<int, int[]> SituationPosition = new Dictionary<int, int[]>();
    public static TBasketShootPositionData[] BasketShootPosition;
	public static Dictionary<int, TSkillData> SkillData = new Dictionary<int, TSkillData>();
	
	public static Dictionary<int, Texture> CardTextures = new Dictionary<int, Texture>();

	public static string ServerVersion;
	public static string SaveVersion;
	public static bool IsLoginRTS;
	public static int RoomIndex = -1;

	public static TScenePlayer ScenePlayer;
	public static TTeam Team;
	public static TTeam[] TeamMembers = new TTeam[2];
	public static TTeam[] EnemyMembers = new TTeam[3];
	public static TGameSetting Setting;
	public static float ExtraGreatRate = 5;
	public static float ExtraPerfectRate = 10;

	private static bool isLoaded = false;
	public static void Init()
	{
		if (!isLoaded) {
			isLoaded = true;
			List<TDownloadData> DownloadList = new List<TDownloadData> ();

			DownloadList.Add (new TDownloadData ("greatplayer", "0"));
			DownloadList.Add (new TDownloadData ("baseattr", "0"));
			DownloadList.Add (new TDownloadData ("tactical", "0"));
			DownloadList.Add (new TDownloadData ("ballposition", "0"));
			DownloadList.Add (new TDownloadData ("skill", "0"));

			FileManager.Get.LoadFileResource (DownloadList);

			loadGameSetting();
			loadCardTextures();
		}

		Team.Init();
	}

	private static void loadCardTextures(){
		UnityEngine.Object[] obj = Resources.LoadAll("Textures/SkillCards");
		if(obj.Length > 0) {
			for(int i=0; i<obj.Length; i++) {
				CardTextures.Add(int.Parse(obj[i].name), obj[i] as Texture);
			}
		}
	}

	private static void loadGameSetting() {
		if (PlayerPrefs.HasKey (SettingText.Language)) {
			int temp = Convert.ToInt16(PlayerPrefs.GetString(SettingText.Language));

			if(temp == ELanguage.TW.GetHashCode())
				Setting.Language = ELanguage.TW;
			else
				Setting.Language = ELanguage.EN;
		} else {
			#if UNITY_EDITOR
				#if En
				GameData.Setting.Language = ELanguage.EN;
				#endif
				
				#if zh_TW
				GameData.Setting.Language = ELanguage.TW;
				#endif
			#else
			switch (Application.systemLanguage) {
			case SystemLanguage.Chinese:
				GameData.Setting.Language = ELanguage.TW;
				break;
			default:
				GameData.Setting.Language = ELanguage.EN;
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
					Team.Init();
					return true;
				} catch (Exception e) {
					Debug.Log(e.ToString());
				}
			}
		}
			
		return false;
	}

	public static void SaveTeam() {
		try {
			string save = JsonConvert.SerializeObject(Team);
			PlayerPrefs.SetString(SettingText.TeamSave, save);
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}
}
