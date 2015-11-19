//#define OutFile

using System.Collections.Generic;
using GameEnum;

public static class TextConst
{
	private static bool loaded = false;
	private static Dictionary<int, string[]> gameText = new Dictionary<int, string[]> ();

	#if OutFile
	private static StringBuilder sb = new StringBuilder();
	#endif
	
	private static void AddString(int key, string textTW, string textCN = "", string textEN = "", string textJP = ""){
		#if OutFile
		if (!textTW.Contains("\n") && !textEN.Contains("\n")) {
			sb.Append(key.ToString() + "\t" + textTW.ToString() + "\t" + textEN.ToString());
		} else {
			sb.Append(key.ToString() + "\t" + "-------------------");
		}
		
		sb.AppendLine();
		#endif

		if (textCN == "")
			textCN = textTW;

		if (textEN == "")
			textEN = textTW;

		if (textJP == "")
			textJP = textTW;

		if (gameText.ContainsKey (key)) {
			gameText[key][ELanguage.EN.GetHashCode()] = textEN;
			gameText[key][ELanguage.TW.GetHashCode()] = textTW;
			gameText[key][ELanguage.CN.GetHashCode()] = textCN;
			gameText[key][ELanguage.JP.GetHashCode()] = textJP;
		}else{
			string [] Data = new string[4];
			Data[ELanguage.EN.GetHashCode()] = textEN;
			Data[ELanguage.TW.GetHashCode()] = textTW;
			Data[ELanguage.CN.GetHashCode()] = textCN;
			Data[ELanguage.JP.GetHashCode()] = textJP;
			gameText.Add(key, Data);
		}
	}

	public static string S (int index){
		if(gameText.ContainsKey(index))
			return gameText[index][GameData.Setting.Language.GetHashCode()];
		else
			return "";
	} 
	
	public static void Init(){
		if (!loaded) {
			loaded = true;

			AddString (1, "重新開始", "重新開始"); //1~1000 UI
			AddString (2, "點擊開始", "點擊開始");
			AddString (3, "點擊繼續", "點擊繼續");
			AddString (4, "Reset", "Reset");
			AddString (5, "AI Level", "AI Level");
			AddString (6, "AI Control", "AI Control");
			AddString (7, "C", "C");
			AddString (8, "F", "F");
			AddString (9, "G", "G");
			AddString (10, "前往更新Apk", "");
			AddString (11, "前往更新IOS", "");
			AddString (12, "[33FFDD]2PT[-]-2分球命中率\n\n[33FFDD]3PT[-]-3分球命中率\n\n[33FFDD]SPD[-]-速度\n\n[33FFDD]STA[-]-耐力",
			           "[33FFDD]2PT[-]-2Point\n\n[33FFDD]3PT[-]-3Point\n\n[33FFDD]SPD[-]-Speed\n\n[33FFDD]STA[-]-Stamina"); 
			AddString (13, "[33FFDD]REB[-]-籃板\n\n[33FFDD]DNK[-]-灌籃\n\n[33FFDD]BLK[-]-火鍋\n\n[33FFDD]STR[-]-力量",
			           "[33FFDD]REB[-]-Rebound\n\n[33FFDD]DNK[-]-Dunk\n\n[33FFDD]BLK[-]-Block\n\n[33FFDD]STR[-]-Strength");
			AddString (14, "[33FFDD]DEF[-]-防守\n\n[33FFDD]STL[-]-抄截\n\n[33FFDD]DRB[-]-控球\n\n[33FFDD]PAS[-]-傳球",
			           "[33FFDD]DEF[-]-Defence\n\n[33FFDD]STL[-]-Steal\n\n[33FFDD]DRB[-]-Dribble\n\n[33FFDD]PAS[-]-Pass");
			AddString (15, "GUARD 後衛", "GUARD 後衛");
			AddString (16, "FORWARDS 前鋒", "FORWARDS 前鋒");
			AddString (17, "CENTER 中鋒", "CENTER 中鋒");
			AddString (18, "貧民窟長大的女孩，為了找尋失蹤的父親，跟照顧年幼的弟妹與母親，身兼多職，在酒吧練出花俏的手技。\n\n[33FFDD]擁有敏捷的動態視力"
			           , "");
			AddString (19, "因為階級鬥爭而喪失記憶與右臂的軍方臥底，因而被改造成半機械的生化人。\n\n為了尋找回憶中的女人，走上了復仇之路。\n\n[33FFDD]擁有高超的命中率"
			           , "");
			AddString (20, "摔角界的明日之星，但為了錢而一直打假比賽。直到某次比賽中失手打敗對方而欠下巨債，家庭破碎。\n\n[33FFDD]擁有強壯的體格"
			           , "");
			AddString (21, "GUARD", "GUARD");
			AddString (22, "FORWARD", "FORWARD");
			AddString (23, "CENTER", "CENTER");
			AddString (24, "GAME TARGET", "GAME TARGET");
			AddString (25, "HOME TEAM", "HOME TEAM");
			AddString (26, "AWAY TEAM", "AWAY TEAM");
			AddString (27, "有機率獲得", "");

			AddString (101, "Try Again", "Try Again");
			AddString (102, "Waiting for network.", "Waiting for network.");
			AddString (103, "Network no responds. Please try again.", "Network no responds. Please try again.");

			AddString (201, "AVATAR", "AVATAR");
			AddString (202, "CARDS", "CARDS");
			AddString (203, "確定售出？", "");
			AddString (204, "牌組變更，是否儲存？", "");
			AddString (205, "CUSTOMIZED PLAYER", "CUSTOMIZED PLAYER");
			AddString (206, "確定刪除此角色？", "");
			AddString (207, "確定購買角色欄位？", "");


		}
    }
}
