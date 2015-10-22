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

			AddString (101, "Try Again", "Try Again");
			AddString (102, "Waiting for network.", "Waiting for network.");
			AddString (103, "Network no responds. Please try again.", "Network no responds. Please try again.");

			AddString (201, "AVATAR", "AVATAR");
			AddString (202, "CARDS", "CARDS");
			AddString (203, "確定售出？", "");
			AddString (204, "牌組變更，是否儲存？？", "");

			AddString (2000011, "競賽進入下半場，請兩隊把握後半[FF3355]{0}秒[-]的時間。", 
			           "Competition began entering the second half, half-[FF3355] {0} seconds after both teams hold [-] time.");
			AddString (2000012, "競賽剩下倒數[FF3355]{0}秒[-]！！！", 
			           "Game time last remaining [FF3355] {0} seconds [-] will end the game! ! !");
			AddString (2000018, "[FFDD33]時間[-][33FFDD]限制[-]", 
			           "[FD33] Time [-] [33FFFF] limit [-]");
			AddString (2000019, "[33FFDD]時間[-]", 
			           "[33FFDD]TIME[-]");
			AddString (2000100, "[FFDD33]{1}[-][BBFF33]已達成得分目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] have reached the target score! ! ! [-]");
			AddString (2000101, "[FFDD33]{1}[-]已達成一半的得分目標。", 
			           "[FFDD33]{1}[-] have reached half scoring goals.");
			AddString (2000102, "[FFDD33]{1}[-]只要再得[FF3355]{0}分[-]即可獲得勝利！！！", 
			           "[FFDD33]{1}[-] as long again to get [FF3355] {0} minutes [-] to win! ! !");
			AddString (2000108, "[FFDD33]得分[-][33FFDD]須達到[-]", 
			           "[FD33] score [-] [33FFFF] required[-]");
			AddString (2000109, "[33FFDD]得分[-]", 
			           "[33FFDD]SCORE[-]");
			AddString (2000201, "[FFDD33]{1}[-]只要再失[FF3355]{0}分[-]就會輸掉這場競賽。", 
			           "[FFDD33] {1} [-] as long to lose [FF3355] {0} minutes [-] will lose the race.");
			AddString (2000202, "[FFDD33]{1}[-]要守住最後的[FF3355]{0}分[-]啊！！！", 
			           "[FFDD33] {1} [-] to hold the final [FF3355] {0} minutes [-] ah! ! !");
			AddString (2000208, "[FFDD33]失分[-][33FFDD]不超過[-]", 
			           "[FD33] lose points [-] [33FFFF] does not exceed [-]");
			AddString (2000209, "[33FFDD]失分[-]", 
			           "[33FFDD]LOSE POINTS[-]");	
			AddString (2000301, "[FFDD33]{1}[-]只要再得[FF3355]{0}分[-]就可獲得勝利。", 
			           "FFDD33] {1} [-] as long again to get [FF3355] {0} minutes [-] can win.");
			AddString (2000302, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}分[-]！！！", 
			           "[FFDD33] {1} [-] distance wins're just [FF3355] {0} minutes [-]! ! !");
			AddString (2000308, "[33FFDD]需贏過對手[-]", 
			           "[33FFFF]Need to win opponents[-]");
			AddString (2000309, "[33FFDD]分數[-]", 
			           "[33FFDD]SCORE[-]");
			AddString (2001000, "[FFDD33]{1}[-][BBFF33]已達成2分球目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] has reached a 2-point goal! ! ! [-]");
			AddString (2001001, "[FFDD33]{1}[-]只要再投進[FF3355]{0}顆[-]2分球即可獲得勝利。", 
			           "[FFDD33] {1} [-] Just then dropped [FF3355] {0} teeth [-] 2-point shot can win.");
			AddString (2001002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}顆[-]2分球！！！", 
			           "[FFDD33] {1} [-] distance wins're just [FF3355] {0} teeth [-] 2-point shot! ! !");
			AddString (2001008, "[FFDD33]2分球[-][33FFDD]數量[-]", 
			           "[33FFDD]The number of[-][FFDD33]2-POINTS[-]");
			AddString (2001009, "[FFDD33]2分球[-]", 
			           "[FFDD33]2-POINTS[-]");	
			AddString (2002000, "[FFDD33]{1}[-][BBFF33]已達成3分球目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] has reached a 3-point goal! ! ! [-]");
			AddString (2002001, "[FFDD33]{1}[-]只要再投進[FF3355]{0}顆[-]3分球即可獲得勝利。", 
			           "[FFDD33]{1}[-] Just then dropped [FF3355] {0} teeth [-] 3-point shot can win.");
			AddString (2002002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}顆[-]3分球！！！", 
			           "[FFDD33]{1}[-] distance wins're just [FF3355] {0} teeth [-] 3-point shot! ! !");
			AddString (2002008, "[FFDD33]3分球[-][33FFDD]數量[-]", 
			           "[33FFDD]The number of[-][FFDD33]3-POINTS[-]");
			AddString (2002009, "[FFDD33]3分球[-]", 
			           "[FFDD33]3-POINTS[-]");
			AddString (2003000, "[FFDD33]{1}[-][BBFF33]已達成灌籃目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] dunk goal has been reached! ! ! [-]");
			AddString (2003001, "[FFDD33]{1}[-]只要再[FF3355]{0}次[-]灌籃即可獲得勝利。", 
			           "[FFDD33]{1}[-] Just then [FF3355] {0} times [-] dunk you can win.");
			AddString (2003002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}次[-]灌籃！！！", 
			           "[FFDD33]{1}[-] distance wins're just [FF3355] {0} times [-] dunk! ! !");
			AddString (2003008, "[FFDD33]灌籃[-][33FFDD]成功次數[-]", 
			           "[33FFDD]The number of[-][FFDD33]DUNKS[-]");
			AddString (2003009, "[FFDD33]灌籃[-]", 
			           "[FFDD33]DUNKS[-]");
			AddString (2004000, "[FFDD33]{1}[-][BBFF33]已達成推倒目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] tear down the goal has been reached! ! ! [-]");
			AddString (2004001, "[FFDD33]{1}[-]只要再[FF3355]{0}次[-]推倒即可獲得勝利。", 
			           "[FFDD33]{1}[-] Just then [FF3355] {0} times [-] down you can win.");
			AddString (2004002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}次[-]推倒！！！", 
			           "[FFDD33]{1}[-] distance wins're just [FF3355] {0} times [-] down! ! !");
			AddString (2004008, "[FFDD33]推倒[-][33FFDD]成功次數[-]", 
			           "[33FFDD]The number of[-][FFDD33]PUSHES[-]");
			AddString (2004009, "[FFDD33]推倒[-]", 
			           "[FFDD33]PUSHES[-]");
			AddString (2005000, "[FFDD33]{1}[-][BBFF33]已達成抄截目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] steals the goal has been reached! ! ! [-]");
			AddString (2005001, "[FFDD33]{1}[-]只要再[FF3355]{0}次[-]抄截即獲得勝利。", 
			           "[FFDD33]{1}[-] Just then [FF3355] {0} times [-] steals you can win.");
			AddString (2005002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}次[-]抄截！！！", 
			           "[FFDD33]{1}[-] distance wins're just [FF3355] {0} times [-] steals! ! !");
			AddString (2005008, "[FFDD33]抄截[-][33FFDD]成功次數[-]", 
			           "[33FFDD]The number of[-][FFDD33]STEALS[-]");
			AddString (2005009, "[FFDD33]抄截[-]", 
			           "[FFDD33]STEALS[-]");
			AddString (2006000, "[FFDD33]{1}[-][BBFF33]已達成火鍋目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] have reached a hot target! ! ! [-]");
			AddString (2006001, "[FFDD33]{1}[-]只要再[FF3355]{0}次[-]火鍋即可獲得勝利。", 
			           "[FFDD33]{1}[-] Just then [FF3355] {0} times [-] BLOCKS you can win.");
			AddString (2006002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}次[-]火鍋！！！", 
			           "[FFDD33]{1}[-] distance wins're just [FF3355] {0} times [-] BLOCKS! ! !");
			AddString (2006008, "[FFDD33]火鍋[-][33FFDD]成功次數[-]", 
			           "[33FFDD]The number of[-][FFDD33]BLOCKS[-]");
			AddString (2006009, "[FFDD33]火鍋[-]", 
			           "[FFDD33]BLOCKS[-]");
		}
    }
}
