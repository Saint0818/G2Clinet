using System;
using System.Collections.Generic;
using GameEnum;
using GamePlayStruct;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public static class SettingText
{
    public const string TeamSave = "TeamSave";
    public const string Language = "UserLanguage";
    public const string AITime = "AIChangeTime";
    public const string Effect = "Effect";
    public const string GameRecord = "GameRecord";
    public const string GameRecordStart = "GameRecordStart";
    public const string GameRecordEnd = "GameRecordEnd";
}

public static class GameData
{
    public static TPlayerAttribute[] BaseAttr;
    public static TBasketShootPositionData[] BasketShootPosition;
    
    /// <summary>
    /// key: ID.
    /// </summary>
    public static Dictionary<int, TGreatPlayer> DPlayers = new Dictionary<int, TGreatPlayer>();
    public static Dictionary<int, TSkillData> DSkillData = new Dictionary<int, TSkillData>();
    public static TSkillRecommend[] SkillRecommends;
    public static TMission[] MissionData;
    public static Dictionary<int, TMission> DMissionData = new Dictionary<int, TMission>();
    /// <summary>
    /// Key: ID.
    /// </summary>
    public static Dictionary<int, TItemData> DItemData = new Dictionary<int, TItemData>();
    public static Dictionary<int, TTutorial> DTutorial = new Dictionary<int, TTutorial>();
    public static Dictionary<string, int> DTutorialUI = new Dictionary<string, int>();
    public static Dictionary<int, int> DTutorialStageStart = new Dictionary<int, int>();
    public static Dictionary<int, int> DTutorialStageEnd = new Dictionary<int, int>();
    public static Dictionary<string, UIAtlas> DItemAtlas = new Dictionary<string, UIAtlas>();

    public static Dictionary<int, TPVPData> DPVPData = new Dictionary<int, TPVPData>();

    // Key: Lv.
    public static Dictionary<int, TExpData> DExpData = new Dictionary<int, TExpData>();
    // Key: TExpData.OpenIndex, Value: TExpData.Lv
    public static Dictionary<EOpenUI, int> DOpenUILv = new Dictionary<EOpenUI, int>();
    public static Dictionary<int, TStageToturial> DStageTutorial = new Dictionary<int, TStageToturial>();
    public static TStageToturial[] StageTutorial = new TStageToturial[0];
    private static Dictionary<int, Texture2D> cardTextureCache = new Dictionary<int, Texture2D>();
    private static Dictionary<string, Texture2D> cardItemTextureCache = new Dictionary<string, Texture2D>();
    public static TPreloadEffect[] PreloadEffect;

	public static TPickCost[] DPickCost; // Order 排列順序
    public static TShop[] DShops;
    public static TMall[] DMalls;

    public static float ServerVersion;
    public static float SaveVersion;
    public static bool IsLoginRTS;
    public static int RoomIndex = -1;

    public static TScenePlayer ScenePlayer;
    public static TTeam Team;
    public const int Max_GamePlayer = 3;
    public static TTeam[] TeamMembers = new TTeam[Max_GamePlayer];
	public static TTeam[] EnemyMembers = new TTeam[Max_GamePlayer];
    public static TTeam[] PVPEnemyMembers = new TTeam[Max_GamePlayer];
    public static TGameSetting Setting;

    public static List<TSocialEvent> SocialEvents = new List<TSocialEvent>();

    /// <summary>
    /// 記錄玩家目前打的關卡.
    /// </summary>
    public static int StageID = -1;
    public static bool IsMainStage
    {
        get
        {
            return StageTable.Ins.HasByID(StageID) &&
                   StageTable.Ins.GetByID(StageID).IDKind == TStageData.EKind.MainStage;
        }
    }

    public static bool IsInstance
    {
        get
        {
            return StageTable.Ins.HasByID(StageID) &&
                   StageTable.Ins.GetByID(StageID).IDKind == TStageData.EKind.Instance;
        }
    }
    public static float ExtraGreatRate = 5;
    public static float ExtraPerfectRate = 10;

    private static bool isLoaded = false;

    public static void Init()
    {
        if (!isLoaded)
        {
            isLoaded = true;

            FileManager.Get.LoadFileResource();
            loadGameSetting();
            initAtlas();
        }
    }

    public static string AtlasName(int atlas)
    {
        return "AtlasItem_" + atlas.ToString();
    }

    private static void initAtlas()
    {
        UnityEngine.Object[] ats = Resources.LoadAll("UI/AtlasItem", typeof(UIAtlas));

        for (int i = 0; i < ats.Length; i++)
        {
            UIAtlas at = ats[i] as UIAtlas;
            DItemAtlas.Add(at.name, at);
        }
    }

    public static Texture2D CardTexture(int id)
    {
        if (GameData.DSkillData.ContainsKey(id))
        {
            if (GameData.DSkillData[id].PictureNo > 0)
                id = GameData.DSkillData[id].PictureNo;

            if (cardTextureCache.ContainsKey(id))
            {
                return cardTextureCache[id];
            }
            else
            {
                string path = "Textures/SkillCards/" + id.ToString();
                Texture2D obj = Resources.Load(path) as Texture2D;
                if (obj)
                {
                    cardTextureCache.Add(id, obj);
                    return obj;
                }
                else
                {
                    //download form server
                    return null;
                }
            }
        }
        else
            return null;
    }

    public static Texture2D CardItemTexture(int id)
    {
        if (GameData.DSkillData.ContainsKey(id))
        {
            if (GameData.DSkillData[id].PictureNo > 0)
                id = GameData.DSkillData[id].PictureNo;
			
            if (cardItemTextureCache.ContainsKey(id.ToString() + "t"))
            {
                return cardItemTextureCache[id.ToString() + "t"];
            }
            else
            {
                string path = "Textures/SkillCards/" + id.ToString() + "t";
                Texture2D obj = Resources.Load(path) as Texture2D;
                if (obj)
                {
                    cardItemTextureCache.Add(id.ToString() + "t", obj);
                    return obj;
                }
                else
                {
                    //download form server
                    return null;
                }
            }
        }
        else
            return null;
    }

    public static void SetGameQuality(QualityType lv)
    {
        string setting = lv.ToString();
        int foundIndex = -1;
        string[] names = QualitySettings.names;

        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == setting)
            {
                foundIndex = i;
                QualitySettings.SetQualityLevel(foundIndex);
                continue;
            }
        }
    }

    private static void loadGameSetting()
    {

        Setting.NewAvatar = new Dictionary<int, int>();
        for (int i = 1; i < 8; i++)
        {
            Setting.NewAvatar.Add(i, 0);
        }

        foreach (ESave item in Enum.GetValues(typeof(ESave)))
        {
            if (!PlayerPrefs.HasKey(item.ToString()))
            {
                //init
                switch (item)
                {
                    case ESave.MusicOn:
                        AudioMgr.Get.MusicOn(true);
                        Setting.Music = true;
                        break;
                    case ESave.SoundOn:
                        AudioMgr.Get.SoundOn(true);
                        Setting.Sound = true;
                        break;
                    case ESave.Quality:
                        Setting.Quality = 1;
                        SetGameQuality((QualityType)Setting.Quality);
                        break;
                    case ESave.AIChangeTimeLv:
                        Setting.AIChangeTimeLv = 0;
                        break;
                    case ESave.UserLanguage: 
                        Setting.Language = ELanguage.EN;
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
                        break;
                    case ESave.ShowEvent:
                        Setting.ShowEvent = false;
                        break;
                    case ESave.ShowWatchFriend:
                        Setting.ShowWatchFriend = false;
                        break;
                    case ESave.SocialEventTime:
                        Setting.SocialEventTime = DateTime.UtcNow;
                        break;
                    case ESave.WatchFriendTime:
                        Setting.WatchFriendTime = DateTime.UtcNow;
                        break;
                }
            }
            else
            {
                int index = PlayerPrefs.GetInt(item.ToString());
                string str = "";
                switch (item)
                {
                    case ESave.MusicOn:
                        Setting.Music = index == 1 ? true : false;
                        AudioMgr.Get.MusicOn(Setting.Music);
                        break;
                    case ESave.SoundOn:
                        Setting.Sound = index == 1 ? true : false;
                        AudioMgr.Get.SoundOn(Setting.Sound);
                        break;
                    case ESave.Quality:
                        Setting.Quality = index;
                        SetGameQuality((QualityType)Setting.Quality);
                        break;
                    case ESave.AIChangeTimeLv:
                        Setting.AIChangeTimeLv = index;
                        break;
                    case ESave.UserLanguage:
                        Setting.Language = (ELanguage)index; 
                        break;
                    case ESave.NewAvatar1:
                        Setting.NewAvatar[1] = index;
                        break;
                    case ESave.NewAvatar2:
                        Setting.NewAvatar[2] = index;
                        break;
                    case ESave.NewAvatar3:
                        Setting.NewAvatar[3] = index;
                        break;
                    case ESave.NewAvatar4:
                        Setting.NewAvatar[4] = index;
                        break;
                    case ESave.NewAvatar5:
                        Setting.NewAvatar[5] = index;
                        break;
                    case ESave.NewAvatar6:
                        Setting.NewAvatar[6] = index;
                        break;
                    case ESave.NewAvatar7:
                        Setting.NewAvatar[7] = index;
                        break;
                    case ESave.ShowEvent:
                        Setting.ShowEvent = index > 0 ? true : false;
                        break;
                    case ESave.ShowWatchFriend:
                        Setting.ShowWatchFriend = index > 0 ? true : false;
                        break;
                    case ESave.SocialEventTime:
                        str = PlayerPrefs.GetString(item.ToString());
                        if (!string.IsNullOrEmpty(str))
                        {
                            try
                            {
                                Setting.SocialEventTime = Convert.ToDateTime(str);
                            }
                            catch (Exception e)
                            {
                                Setting.SocialEventTime = DateTime.UtcNow;
                                Debug.Log(e.ToString());
                            }
                        }
                        else
                            Setting.SocialEventTime = DateTime.UtcNow;

                        break;
                    case ESave.WatchFriendTime:
                        str = PlayerPrefs.GetString(item.ToString());
                        if (!string.IsNullOrEmpty(str))
                        {
                            try
                            {
                                Setting.WatchFriendTime = Convert.ToDateTime(str);
                            }
                            catch (Exception e)
                            {
                                Setting.WatchFriendTime = DateTime.UtcNow;
                                Debug.Log(e.ToString());
                            }
                        }
                        else
                            Setting.WatchFriendTime = DateTime.UtcNow;
                        
                        break;
                }
            }
        }
    }

    public static bool LoadTeamSave()
    {
        if (PlayerPrefs.HasKey(SettingText.TeamSave))
        {
            string save = PlayerPrefs.GetString(SettingText.TeamSave, "");
            if (save != "")
            {
                try
                {
                    Team = JsonConvert.DeserializeObject <TTeam>(save);
                    Team.Init();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
        }
			
        return false;
    }

    public static bool PotentialNoticeEnable(ref TTeam team)
    {
        if (team.Identifier != GameData.Team.Identifier)
            return false;
        else
        {
            int ownerlvPoint = GameFunction.GetCurrentLvPotential(team.Player);
            int ownerAvatarPoint = GameFunction.GetAllPlayerTotalUseAvatarPotential();
			
            for (int i = 0; i < GameConst.PotentialRule.Length; i++)
            {
                if (ownerlvPoint + ownerAvatarPoint >= GameConst.PotentialRule[i])
                    return true;
            }
            return false;
        }

    }

    public static bool AvatarNoticeEnable()
    {
        foreach (KeyValuePair<int, int> item in Setting.NewAvatar)
        {
            if (item.Value > 0)
                return true;
        }
        return false;
    }

    public static bool AvatarNoticeEnable(int avatarKind)
    {
        //1: hair 
        if (Setting.NewAvatar.ContainsKey(avatarKind) && Setting.NewAvatar[avatarKind] > 0)
            return true;
        else
            return false;
    }

    public static void SetAvatarNotice(int avatarKind, int itemid)
    {
        //1: hair 
        if (Setting.NewAvatar.ContainsKey(avatarKind))
        {
            Setting.NewAvatar[avatarKind] = itemid;

            switch (avatarKind)
            {
                case 1:
                    PlayerPrefs.SetInt(ESave.NewAvatar1.ToString(), itemid);
                    break;
                case 2:
                    PlayerPrefs.SetInt(ESave.NewAvatar2.ToString(), itemid);
                    break;
                case 3:
                    PlayerPrefs.SetInt(ESave.NewAvatar3.ToString(), itemid);
                    break;
                case 4:
                    PlayerPrefs.SetInt(ESave.NewAvatar4.ToString(), itemid);
                    break;
                case 5:
                    PlayerPrefs.SetInt(ESave.NewAvatar5.ToString(), itemid);	
                    break;
                case 6:
                    PlayerPrefs.SetInt(ESave.NewAvatar6.ToString(), itemid);
                    break;
                case 7:
                    PlayerPrefs.SetInt(ESave.NewAvatar7.ToString(), itemid);
                    break;
            }

            PlayerPrefs.Save();
        }
    }

    public static void SaveTeam()
    {
        try
        {
            string save = JsonConvert.SerializeObject(Team);
            PlayerPrefs.SetString(SettingText.TeamSave, save);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public static string OS
    {
        get
        {
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

    public static string Company
    {
        get
        {
            return "NiceMarket";
        }
    }

    public static string UrlAnnouncement
    {
        get
        {
            if (Setting.Language == ELanguage.TW || Setting.Language == ELanguage.CN)
                return "http://nicemarket.com.tw/g2announcement1";
            else
                return "http://nicemarket.com.tw/g2announcement";
        }
    }

	public static bool IsPVP
	{
		get{ 
			foreach (KeyValuePair<int, TPVPData> item in GameData.DPVPData) 
			{
				if (StageID == item.Value.Stage) {
					return true;
				}
			}

			return false;
		}
	}

    public static bool IsOpenUIEnable(EOpenUI openUI)
    {
        if (!DOpenUILv.ContainsKey(openUI))
            return true;

        return Team.Player.Lv >= DOpenUILv[openUI];
    }
}
