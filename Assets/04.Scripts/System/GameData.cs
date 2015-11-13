using System;
using System.Collections.Generic;
using GameEnum;
using GamePlayStruct;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public static class SettingText {
	public const string TeamSave = "TeamSave";
	public const string Language = "UserLanguage";
	public const string AITime = "AIChangeTime";
	public const string Effect = "Effect";
	public const string GameRecord = "GameRecord";
	public const string GameRecordStart = "GameRecordStart";
	public const string GameRecordEnd = "GameRecordEnd";
}

public static class GameData {
	public static TPlayerAttribute[] BaseAttr;
    public static TBasketShootPositionData[] BasketShootPosition;
    
    /// <summary>
    /// key: ID.
    /// </summary>
	public static Dictionary<int, TGreatPlayer> DPlayers = new Dictionary<int, TGreatPlayer> ();
	public static Dictionary<int, TSkillData> DSkillData = new Dictionary<int, TSkillData>();

    /// <summary>
    /// Key: ID.
    /// </summary>
	public static Dictionary<int, TItemData> DItemData = new Dictionary<int, TItemData>();
	public static Dictionary<int, TTutorial> DTutorial = new Dictionary<int, TTutorial>();
	public static Dictionary<string, int> DTutorialUI = new Dictionary<string, int>();
	public static Dictionary<int, int> DTutorialStageStart = new Dictionary<int, int>();
	public static Dictionary<int, int> DTutorialStageEnd = new Dictionary<int, int>();
	public static Dictionary<int, TExpData> DExpData = new Dictionary<int, TExpData>();
	public static Dictionary<int, TStageToturial> DStageTutorial = new Dictionary<int, TStageToturial>();
	public static TStageToturial[] StageTutorial = new TStageToturial[0];
	private static Dictionary<int, Texture2D> cardTextureCache = new Dictionary<int, Texture2D>();
	private static Dictionary<string, Texture2D> cardItemTextureCache = new Dictionary<string, Texture2D>();
	public static TPreloadEffect[] PreloadEffect;

	public static float ServerVersion;
	public static float SaveVersion;
	public static bool IsLoginRTS;
	public static int RoomIndex = -1;

	public static TScenePlayer ScenePlayer;
	public static TTeam Team;
	public static TTeam[] TeamMembers = new TTeam[3];
	public static TTeam[] EnemyMembers = new TTeam[3];
	public static TGameSetting Setting;

    /// <summary>
    /// 記錄玩家目前打的關卡.
    /// </summary>
	public static int StageID = -1;
	public static float ExtraGreatRate = 5;
	public static float ExtraPerfectRate = 10;

	private static bool isLoaded = false;
	public static void Init()
	{
		if (!isLoaded) {
			isLoaded = true;

			FileManager.Get.LoadFileResource ();
			loadGameSetting();
		}
	}

	public static Texture2D CardTexture(int id) {
		if (GameData.DSkillData.ContainsKey(id)) {
			if (GameData.DSkillData[id].PictureNo > 0)
				id = GameData.DSkillData[id].PictureNo;

			if (cardTextureCache.ContainsKey(id)) {
				return cardTextureCache [id];
			}else {
				string path = "Textures/SkillCards/" + id.ToString();
	            Texture2D obj = Resources.Load(path) as Texture2D;
				if (obj) {
					cardTextureCache.Add(id, obj);
					return obj;
				} else {
					//download form server
	                return null;
	            }
	        }
		} else
		return null;
    }

	public static Texture2D CardItemTexture(int id) {
		if (GameData.DSkillData.ContainsKey(id)) {
			if (GameData.DSkillData[id].PictureNo > 0)
				id = GameData.DSkillData[id].PictureNo;
			
			if (cardItemTextureCache.ContainsKey(id.ToString() + "t")) {
				return cardItemTextureCache [id.ToString() + "t"];
			}else {
				string path = "Textures/SkillCards/" + id.ToString() + "t";
				Texture2D obj = Resources.Load(path) as Texture2D;
				if (obj) {
					cardItemTextureCache.Add(id.ToString() + "t", obj);
					return obj;
				} else {
					//download form server
					return null;
				}
			}
		} else
			return null;
	}

	private static void loadGameSetting() {
		Setting.Language = ELanguage.EN;
		if (PlayerPrefs.HasKey (SettingText.Language)) {
			int temp = Convert.ToInt16(PlayerPrefs.GetString(SettingText.Language));

			switch (temp) {
			case 0:
				Setting.Language = ELanguage.EN;
				break;
			case 1:
				Setting.Language = ELanguage.CN;
				break;
			case 3:
				Setting.Language = ELanguage.JP;
				break;
			}
		} else {
			#if UNITY_EDITOR
				#if TW
				GameData.Setting.Language = ELanguage.TW;
				#endif

				#if CN
				GameData.Setting.Language = ELanguage.CN;
				#endif
				
				#if EN
				GameData.Setting.Language = ELanguage.EN;
				#endif

				#if JP
				GameData.Setting.Language = ELanguage.JP;
				#endif

			#else
			switch (Application.systemLanguage) {
			case SystemLanguage.ChineseTraditional:
			case SystemLanguage.Chinese:
				GameData.Setting.Language = ELanguage.TW;
				break;
			case SystemLanguage.ChineseSimplified:
				GameData.Setting.Language = ELanguage.CN;
				break;
			case SystemLanguage.Japanese:
				GameData.Setting.Language = ELanguage.JP;
				break;
			}
			#endif
		}

		GameData.Setting.Language = ELanguage.TW;
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

	public static string OS {
		get {
			string os = "0";
			#if UNITY_EDITOR
			
			#else
				#if UNITY_IOS
				os = "1";
				#endif
				#if UNITY_ANDROID
				os = "2";
				#endif
				#if (!UNITY_IOS && !UNITY_ANDROID)
				os = "3";
				#endif
			#endif
			
			return os;
		}
	}

	public static string Company {
		get {
			return "NiceMarket";
		}
	}
}
