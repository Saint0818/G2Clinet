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
	private static Dictionary<int, TGreatPlayer> _DPlayers = new Dictionary<int, TGreatPlayer>();
	private static bool _isLoad_DPlayers = false;
    public static Dictionary<int, TGreatPlayer> DPlayers
	{
		get{ 
			if (!_isLoad_DPlayers)
				FileManager.Get.LoadFileResourceEx (FileManager.DataIndex.greatplayer, LoadGreatPlayerData);
			return _DPlayers;	
		}

		set{ _DPlayers = value; }
	}
	/// <summary>
    /// The D skill data.
    /// </summary>
	private static Dictionary<int, TSkillData> _DSkillData = new Dictionary<int, TSkillData>();
	private static bool _isLoad_DSkillData = false;
	public static Dictionary<int, TSkillData> DSkillData
	{
		get{ 
			if (!_isLoad_DSkillData)
				FileManager.Get.LoadFileResourceEx (FileManager.DataIndex.skill, LoadSkillData);
			return _DSkillData;	
		}

		set{ _DSkillData = value; }
		
	}
	/// <summary>
	/// The skill recommends.
	/// </summary>
    public static TSkillRecommend[] SkillRecommends;
    public static TMission[] MissionData;
    public static Dictionary<int, TMission> DMissionData = new Dictionary<int, TMission>();
    /// <summary>
    /// Key: ID.
    /// </summary>
	/// 
	private static Dictionary<int, TItemData> _DItemData = new Dictionary<int, TItemData>();
	private static bool _isLoad_DItemData = false;
    public static Dictionary<int, TItemData> DItemData 
	{
		get{ 
			if (!_isLoad_DItemData)
				FileManager.Get.LoadFileResourceEx (FileManager.DataIndex.item, LoadItemData);
			return _DItemData;	
		}

		set{ _DItemData = value; }
	}

	/// <summary>
	/// /
	/// </summary>
    public static Dictionary<int, TTutorial> DTutorial = new Dictionary<int, TTutorial>();
    public static Dictionary<string, int> DTutorialUI = new Dictionary<string, int>();
    public static Dictionary<int, int> DTutorialStageStart = new Dictionary<int, int>();
    public static Dictionary<int, int> DTutorialStageEnd = new Dictionary<int, int>();

    public static Dictionary<int, TPVPData> DPVPData = new Dictionary<int, TPVPData>();

    // Key: Lv.
    public static Dictionary<int, TExpData> DExpData = new Dictionary<int, TExpData>();
    // Key: TExpData.OpenIndex, Value: TExpData.Lv
//    public static Dictionary<EOpenUI, int> DOpenUILv = new Dictionary<EOpenUI, int>();
    public static Dictionary<int, TStageToturial> DStageTutorial = new Dictionary<int, TStageToturial>();
    public static TStageToturial[] StageTutorial = new TStageToturial[0];
    public static TPreloadEffect[] PreloadEffect;

	public static TPickCost[] DPickCost; // Order 排列順序
    /// <summary>
    /// The D shops.
    /// </summary>
	private static TShop[] _DShops;
	private static bool _isLoad_DShops = false;
	public static TShop[] DShops
	{
		get{ 
			if (!_isLoad_DShops)
				FileManager.Get.LoadFileResourceEx (FileManager.DataIndex.shop, LoadShopData);
			return _DShops;	
		}

		set{ _DShops = value; }
	}

	/// <summary>
	/// The D malls.
	/// </summary>
    public static TMall[] DMalls;
	public static Dictionary<int, TSuitCard> DSuitCard = new Dictionary<int, TSuitCard>();
	public static Dictionary<int, TSuitItem> DSuitItem = new Dictionary<int, TSuitItem>();

    public static Dictionary<int, TPotentital> DPotential = new Dictionary<int, TPotentital>();

	//Key: lv
	public static Dictionary<int, TArchitectureExp> DArchitectureExp = new Dictionary<int, TArchitectureExp>();

	//building key:Kind
	public static List<TItemData> DBuildData = new List<TItemData>();
	public static int[] DBuildHightestLvs = new int[9];

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
    public static bool TestStage = false;
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

	public static bool IsPVP
	{
		get
		{
			return StageTable.Ins.HasByID(StageID) && 
				   StageTable.Ins.GetByID(StageID).IDKind == TStageData.EKind.PVP;
		}			
	}

    public static bool OpenTutorial = false;
    public static bool IsUseFpsLimiter = true;

    public static float ExtraGreatRate = 5;
    public static float ExtraPerfectRate = 10;

    public static bool IsLoaded = false;

    public static bool Init()
    {
        if (!IsLoaded)
        {
            IsLoaded = true;
            initGameSetting();
            FileManager.Get.LoadFileResource();
            return true;
        } else
            return false;
    }

	public static int GetBuildItemIndex (int itemID) {
		for(int i=0; i<DBuildData.Count; i++) 
			if(DBuildData[i].ID == itemID)
				return DBuildData[i].Index;
			
		return  -1;
	}

	/// <summary>
	/// kind 0.orange1 1.green
	/// </summary>
	/// <returns>The enough sprite.</returns>
	/// <param name="enough">If set to <c>true</c> enough.</param>
	/// <param name="kind">Kind.</param>
    public static string CoinEnoughSprite(bool enough, int kind=0) {
        if (enough) {
            switch (kind) {
                case 1: return "button_green"; //buy
                default: return "button_orange1";
            }
        } else
            return "button_gray";
    }
	/// <summary>
	/// kind 0.diamond
	/// </summary>
	/// <returns>The enough text color.</returns>
	/// <param name="enough">If set to <c>true</c> enough.</param>
	/// <param name="kind">Kind.</param>
    public static Color CoinEnoughTextColor(bool enough, int kind=0) {
        if (enough) {
            if (kind == 0)
                return new Color(255, 0, 255, 255);
            else
                return Color.white;
        } else
            return Color.red;
    }

    public static void SetGameQuality(EQualityType lv)
    {
        Setting.Quality = lv.GetHashCode();
        int q = QualitySettings.GetQualityLevel();
        int foundIndex = 0;
        if (Setting.Quality == 1)
            foundIndex = 2;
        else
        if (Setting.Quality == 2)
            foundIndex = 4;
        
        if (q != foundIndex)
            QualitySettings.SetQualityLevel(foundIndex);
    }

	private static void initGameSetting()
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
                        //SetGameQuality((EQualityType)Setting.Quality);
                        break;
                    case ESave.AIChangeTimeLv:
                        Setting.AIChangeTimeLv = 0;
                        break;
                    case ESave.UserLanguage: 
                        Setting.Language = ELanguage.EN;
                        switch (Application.systemLanguage) {
                            case SystemLanguage.ChineseTraditional:
                            case SystemLanguage.Chinese:
                            case SystemLanguage.ChineseSimplified:
                                GameData.Setting.Language = ELanguage.TW;
                                break;
                        }
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
                        Setting.Quality = 1;//index;
                        //SetGameQuality((EQualityType)Setting.Quality);
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
			
            for (int i = 0; i < GameConst.PotentialCount; i++)
            {
                if (ownerlvPoint + ownerAvatarPoint >= GameFunction.GetPotentialRule(team.Player.BodyType, i))
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
            return FileManager.Company;
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

    public static bool IsOpenUIEnable(EOpenID openID)
    {
        if(!LimitTable.Ins.HasByOpenID(openID))
            return true;

        return Team.HighestLv >= LimitTable.Ins.GetByOpenID(openID).Lv;
	}

	public static bool IsOpenUIVisible(EOpenID openID)
	{
		if(!LimitTable.Ins.HasByOpenID(openID))
			return true;

		return Team.HighestLv >= LimitTable.Ins.GetVisibleLv(openID);
	}

	public static bool IsOpenUIEnableByPlayer(EOpenID openID)
	{
		if(!LimitTable.Ins.HasByOpenID(openID))
			return true;

		return Team.Player.Lv >= LimitTable.Ins.GetByOpenID(openID).Lv;
	}

	public static bool IsOpenUIVisibleByPlayer(EOpenID openID)
	{
		if(!LimitTable.Ins.HasByOpenID(openID))
			return true;

		return Team.Player.Lv >= LimitTable.Ins.GetByOpenID(openID).VisibleLv;
	}

	// data load extension
	public static void LoadItemData (string version, string text, bool isSaveVersion){

		try
		{
			_DItemData.Clear();

			TItemData[] data = JsonConvert.DeserializeObject<TItemData[]>(text);
			for (int i = 0; i < data.Length; i++) {
				if(!_DItemData.ContainsKey(data[i].ID) && data[i].ID > 0)
					_DItemData.Add(data[i].ID, data[i]);
				else 
					Debug.LogError("GameData.DItemData is ContainsKey:"+ data[i].ID);

				if(data[i].Kind >= 51 && data[i].Kind <= 59) {
					data[i].Index = i;
					GameData.DBuildData.Add(data[i]);
				}

				#if UNITY_EDITOR
				if (data[i].StageSource != null) {
					for (int j = 0; j < data[i].StageSource.Length; j++)
						if (StageTable.Ins.GetByID(data[i].StageSource[j]).ChallengeOnlyOnce)
							Debug.LogError(string.Format("Item source error stage item id {0} stage id {1}", data[i].ID, data[i].StageSource[j]));
				}
				#endif
			}
			_isLoad_DItemData = true;
			//if(isSaveVersion)
			//	SaveDataVersionAndJson(text, "item", version);

			Debug.Log ("[item parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("[item parsed error] " + ex.Message);
		}
	}

	public static void LoadGreatPlayerData (string version, string text, bool isSaveVersion){

		try {
			_DPlayers.Clear();

			TGreatPlayer[] data = (TGreatPlayer[])JsonConvert.DeserializeObject (text, typeof(TGreatPlayer[]));
			if (data != null) {
				for (int i = 0; i < data.Length; i++) 
					if (data[i].ID > 0 && !_DPlayers.ContainsKey(data[i].ID))
						_DPlayers.Add(data[i].ID, data[i]);
			}

			//if(isSaveVersion)
			//	SaveDataVersionAndJson(text, "greatplayer", version);
			_isLoad_DPlayers = true;
			Debug.Log ("[greatplayer parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("[greatplayer parsed error] " + ex.Message);
		}
	}

	public static void LoadSkillData (string version, string text, bool isSaveVersion){

		try {
			_DSkillData.Clear();

			TSkillData[] data = (TSkillData[])JsonConvert.DeserializeObject (text, typeof(TSkillData[]));
			for (int i = 0; i < data.Length; i++) {
				if(!_DSkillData.ContainsKey(data[i].ID)) {
					_DSkillData.Add(data[i].ID, data[i]);
					#if UNITY_EDITOR
					if(data[i].UpgradeExp != null && data[i].UpgradeExp.Length != data[i].UpgradeMoney.Length)
						Debug.LogError("UpgradeExp or UpgradeMoney is wrong:"+ data[i].ID);
					#endif
				} else
					Debug.LogError("GameData.DSkillData is ContainsKey:"+ data[i].ID);
			}

			//if(isSaveVersion)
			//	SaveDataVersionAndJson(text, "skill", version);
			_isLoad_DSkillData = true;

			Debug.Log ("[skill parsed finished.] ");
		} catch (System.Exception ex) {
			Debug.LogError ("[skill parsed error] " + ex.Message);
		}
	}

	public static void LoadShopData (string version, string text, bool isSaveVersion){

		try {
			_DShops = JsonConvert.DeserializeObject<TShop[]>(text);

			//if (isSaveVersion)
			//	SaveDataVersionAndJson(text, "shop", version);
			_isLoad_DShops = true;

			Debug.Log ("[Shop parsed finished.]");
		} catch (System.Exception ex) {
			Debug.LogError ("Shop parsed error : " + ex.Message);
		}
	}


}
