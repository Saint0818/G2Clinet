using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public delegate void UIFunction();
public delegate void TBooleanWWWObj(bool val, WWW Result);
public delegate void TIndexObj(int Index);
public delegate IEnumerator OnWaitHttp(WWW www);

public static class URLConst {
	public const string GooglePlay = "https://play.google.com/store/apps/details?id=com.nicemarket.nbaa";
	public const string NiceMarketApk = "http://nicemarket.com.tw/assets/apk/BaskClub.apk";
	public const string Version = FileManager.URL + "version";
	public const string CheckSession = FileManager.URL + "checksession";
	public const string deviceLogin = FileManager.URL + "devicelogin";
	public const string Signup = FileManager.URL + "signup";
	public const string Login = FileManager.URL + "login";
	public const string FBLogin = FileManager.URL + "fblogin";
	public const string LinkFB = FileManager.URL + "linkfb";
	public const string CreateRole = FileManager.URL + "createrole";
	public const string TeamName = FileManager.URL + "teamname";
	public const string Conference = FileManager.URL + "conference";
	public const string PVPAward = FileManager.URL + "pvpaward";
	public const string IsSeePVPList = FileManager.URL + "isseepvplist";
	public const string Revenge = FileManager.URL + "revenge";
	public const string PVPStart = FileManager.URL + "pvpstart";
	public const string PVPEnd = FileManager.URL + "pvpend";
	public const string StageStart = FileManager.URL + "pvestart";
	public const string StageEnd = FileManager.URL + "pveend";
	public const string Tutorial = FileManager.URL + "tutorial";
	public const string PlayerKind = FileManager.URL + "playerkind";
	public const string AutoPower = FileManager.URL + "autopower";
	public const string Rank = FileManager.URL + "rank";
	public const string PVPRank = FileManager.URL + "pvprank";
	public const string MyPVPRank = FileManager.URL + "mypvprank";
	public const string MatchRank = FileManager.URL + "matchrank";
	public const string MatchAward = FileManager.URL + "matchaward";
	public const string MatchExchange = FileManager.URL + "matchexchange";
	public const string FBGift = FileManager.URL + "fbgift";
	public const string DayGift = FileManager.URL + "daygift";
	public const string ChangeLogo = FileManager.URL + "changelogo";
	public const string MaxLogo = FileManager.URL + "maxlogo";
	public const string FBRank = FileManager.URL + "fbrank";
	public const string UseItem = FileManager.URL + "useitem";
	public const string BuyMoney = FileManager.URL + "buymoney";
	public const string BuyDiamond = FileManager.URL + "buydiamond";
	public const string Polling = FileManager.URL + "polling";
	public const string TrainPlayer = FileManager.URL + "trainplayer";
	public const string trainplayerFinish = FileManager.URL + "trainplayerFinish";
	public const string trainplayerOk = FileManager.URL + "trainplayerOk";
	public const string openTrainBox = FileManager.URL + "openTrainBox";
	public const string ItemUp = FileManager.URL + "itemup";
	public const string SellItem = FileManager.URL + "sellitem";
	public const string fullPower = FileManager.URL + "fullPower";
	public const string BuyLuckBox = FileManager.URL + "buyluckbox";
	public const string BuyStoreItem = FileManager.URL + "buystoreitem";
	public const string PlayerEvo = FileManager.URL + "playerevo";
	public const string StageAward = FileManager.URL + "stageAward";
	public const string BingoItem = FileManager.URL + "bingoitem";
	public const string RecordGameStart = FileManager.URL + "recordgamestart";
	public const string RecordGameEnd = FileManager.URL + "recordgameend";
	public const string RecordGameRank = FileManager.URL + "recordgamerank";
	public const string CrusadeData = FileManager.URL + "crusadedata";
	public const string CrusadeStart = FileManager.URL + "crusadestart";
	public const string CrusadeEnd = FileManager.URL + "crusadeend";
	public const string ResetCrusade = FileManager.URL + "resetcrusade";
	public const string CrusadeAward = FileManager.URL + "crusadeaward";
	public const string SkillUpgrade = FileManager.URL + "skillupgrade";
	public const string ChangeSkillEffect = FileManager.URL + "changeskilleffect";
	public const string MissionAward = FileManager.URL + "missionaward";
	public const string CheckResetToday = FileManager.URL + "checkresettoday";
	public const string GetGymData = FileManager.URL + "getgymdata";
	public const string SearchGym = FileManager.URL + "searchgym";
	public const string GymStart = FileManager.URL + "gymstart";
	public const string GymEnd = FileManager.URL + "gymend";
	public const string GymDef = FileManager.URL + "gymdef";
	public const string GymAward = FileManager.URL + "gymaward";
	public const string PVEQuickFinish = FileManager.URL + "pvequickfinish";
	public const string ResetPVE = FileManager.URL + "resetpve";
	public const string RecordAward = FileManager.URL + "recordaward";
	public const string LiveTime = FileManager.URL + "livetime";
	public const string SaveTalk = FileManager.URL + "savetalk";
	public const string OpenLogoBox = FileManager.URL + "openlogobox";
	public const string BuildUpgrade = FileManager.URL + "buildupgrade";
	public const string QuickTrainPlayer = FileManager.URL + "quicktrainplayer";
	public const string ChangeItemEffect = FileManager.URL + "changeitemeffect";
}

public class ParameterConst
{
	public const int Min_ItemEffect = 10;
	public const int Max_ItemEffect = 20;
	public const int Max_LogoNum = 33;
	public const int Max_DiffStage = 5;
	public const int Max_ChargeNum = 3;
	public const int Max_StageNo = 320;
	public const int Min_EquipmentKind = 1;
	public const int Max_EquipmentKind = 6;
	public const int RecordGameTicketID = 2201;
	public const int PVPTicketID = 2202;
	public const int StageQuickFinishItemID = 2203;
	public const int LogoBoxItemID = 210;
	public const int OpenSkillALv = 10;
	public const int SkillLimitLv = 20;
	public const int OpenSkillBStar = 4;
	public const int OpenSkillCStar = 5;
	public const int Min_SpecialHeadItem = 51;
	public const int Max_SpecialHeadItem = 99;
	public const int Min_PracticeStage = 0;
	public const int Max_PracticeStage = 8;
	public const int Max_LogoBingo = 32;

	//OpenIndex
	public const int OpenIndex_TrainBox_1 = 1;
	public const int OpenIndex_TrainBox_2 = 2;
	public const int OpenIndex_TrainBox_3 = 3;
	public const int OpenIndex_TrainBox_4 = 4;
	public const int OpenIndex_TrainBox_5 = 5;

	public const int OpenIndex_DunkGame = 6;
	public const int OpenIndex_Shoot3Game = 7;
	public const int OpenIndex_PVP = 8;
	public const int OpenIndex_MatchGame = 9;
	public const int OpenIndex_Gym = 10;
	public const int OpenIndex_FreeLuckBox = 11;
	public const int OpenIndex_Gym2 = 12;

	public const int OpenIndex_Crusade_1 = 21;
	public const int OpenIndex_Crusade_2 = 22;
	public const int OpenIndex_Crusade_3 = 23;
	public const int OpenIndex_Crusade_4 = 24;
	public const int OpenIndex_Crusade_5 = 25;

	public const int ID_LeaderBook = 1600;

	public static int BattleStageNowChapter = -1;
	public static string [] IOSAchievementAy = {"Achievement_Lv_1", "Achievement_Lv_2", "Achievement_Lv_3", "Achievement_Lv_4", "Achievement_Lv_5"};
}
