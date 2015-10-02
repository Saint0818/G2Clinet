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
			AddString (0, "", "");
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
			AddString (18, "貧民窟長大的女孩，父親失蹤，為了養活12個弟妹和沒有生活能力的母親，身兼多職，除了在充滿醉漢的碼頭酒吧工作練出一身靈活身法，還在複雜的新舊城區街道中擔任極速送報員，而擁有敏捷速度。"
			           , "");
			AddString (19, "原本是前途看好的中階軍官，因為一次無聊的閱兵發生意外，導致右半邊身體被炸毀，沒有背景的他被軍方作為新式機械療法的實驗品，變成半個機器人，但成果不如預期而遭到棄置。論及婚嫁的女友因他不再是個真男人而拋棄了他。"
			           , "");
			AddString (20, "大塊頭，因為巨大的身形與無匹的力量而成為地下摔角選手，其實是個實力相當高強的選手，但因為後台不夠硬始終無法成為明星。在某次講好的比賽中失手打敗對方，害組頭損失慘重而欠下巨債，妻小因此離開他。"
			           , "");
			AddString (21, "GUARD", "GUARD");
			AddString (22, "FORWARDS", "FORWARDS");
			AddString (23, "CENTER", "CENTER");


			AddString (2000011, "競賽開始邁入下半場，請兩隊把握後半[FF3355]{0}秒[-]的時間。", 
			           "Competition began entering the second half, half-[FF3355] {0} seconds after both teams hold [-] time.");
			AddString (2000012, "競賽時間剩下最後[FF3355]{0}秒[-]就要結束比賽！！！", 
			           "Game time last remaining [FF3355] {0} seconds [-] will end the game! ! !");
			AddString (2000100, "[FFDD33]{1}[-][BBFF33]已達成得分目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] have reached the target score! ! ! [-]");
			AddString (2000101, "[FFDD33]{1}[-]已達成一半的得分目標。", 
			           "[FFDD33]{1}[-] have reached half scoring goals.");
			AddString (2000102, "[FFDD33]{1}[-]只要再得到[FF3355]{0}分[-]即可獲得勝利！！！", 
			           "[FFDD33]{1}[-] as long again to get [FF3355] {0} minutes [-] to win! ! !");
			AddString (2000201, "[FFDD33]{1}[-]只要再失去[FF3355]{0}分[-]就會輸掉這場競賽。", 
			           "[FFDD33] {1} [-] as long to lose [FF3355] {0} minutes [-] will lose the race.");
			AddString (2000202, "[FFDD33]{1}[-]要守住最後的[FF3355]{0}分[-]啊！！！", 
			           "[FFDD33] {1} [-] to hold the final [FF3355] {0} minutes [-] ah! ! !");
			AddString (2000301, "[FFDD33]{1}[-]只要再得到[FF3355]{0}分[-]就可以獲得勝利。", 
			           "FFDD33] {1} [-] as long again to get [FF3355] {0} minutes [-] can win.");
			AddString (2000302, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}分[-]！！！", 
			           "[FFDD33] {1} [-] distance wins're just [FF3355] {0} minutes [-]! ! !");
			AddString (2001000, "[FFDD33]{1}[-][BBFF33]已達成2分球目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] has reached a 2-point goal! ! ! [-]");
			AddString (2001001, "[FFDD33]{1}[-]只要再投進[FF3355]{0}顆[-]2分球就可以獲得勝利。", 
			           "[FFDD33] {1} [-] Just then dropped [FF3355] {0} teeth [-] 2-point shot can win.");
			AddString (2001002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}顆[-]2分球！！！", 
			           "[FFDD33] {1} [-] distance wins're just [FF3355] {0} teeth [-] 2-point shot! ! !");
			AddString (2002000, "[FFDD33]{1}[-][BBFF33]已達成3分球目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] has reached a 3-point goal! ! ! [-]");
			AddString (2002001, "[FFDD33]{1}[-]只要再投進[FF3355]{0}顆[-]3分球就可以獲得勝利。", 
			           "[FFDD33]{1}[-] Just then dropped [FF3355] {0} teeth [-] 3-point shot can win.");
			AddString (2002002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}顆[-]3分球！！！", 
			           "[FFDD33]{1}[-] distance wins're just [FF3355] {0} teeth [-] 3-point shot! ! !");
			AddString (2003000, "[FFDD33]{1}[-][BBFF33]已達成灌籃目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] dunk goal has been reached! ! ! [-]");
			AddString (2003001, "[FFDD33]{1}[-]只要再進行[FF3355]{0}次[-]灌籃就可以獲得勝利。", 
			           "[FFDD33]{1}[-] Just then [FF3355] {0} times [-] dunk you can win.");
			AddString (2003002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}次[-]灌籃！！！", 
			           "[FFDD33]{1}[-] distance wins're just [FF3355] {0} times [-] dunk! ! !");
			AddString (2004000, "[FFDD33]{1}[-][BBFF33]已達成推倒目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] tear down the goal has been reached! ! ! [-]");
			AddString (2004001, "[FFDD33]{1}[-]只要再進行[FF3355]{0}次[-]推倒就可以獲得勝利。", 
			           "[FFDD33]{1}[-] Just then [FF3355] {0} times [-] down you can win.");
			AddString (2004002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}次[-]推倒！！！", 
			           "[FFDD33]{1}[-] distance wins're just [FF3355] {0} times [-] down! ! !");
			AddString (2005000, "[FFDD33]{1}[-][BBFF33]已達成抄截目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] steals the goal has been reached! ! ! [-]");
			AddString (2005001, "[FFDD33]{1}[-]只要再進行[FF3355]{0}次[-]抄截就可以獲得勝利。", 
			           "[FFDD33]{1}[-] Just then [FF3355] {0} times [-] steals you can win.");
			AddString (2005002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}次[-]抄截！！！", 
			           "[FFDD33]{1}[-] distance wins're just [FF3355] {0} times [-] steals! ! !");
			AddString (2006000, "[FFDD33]{1}[-][BBFF33]已達成火鍋目標！！！[-]", 
			           "[FFDD33] {1} [-] [BBFF33] have reached a hot target! ! ! [-]");
			AddString (2006001, "[FFDD33]{1}[-]只要再進行[FF3355]{0}次[-]火鍋就可以獲得勝利。", 
			           "[FFDD33]{1}[-] Just then [FF3355] {0} times [-] pot you can win.");
			AddString (2006002, "[FFDD33]{1}[-]距離獲勝只差[FF3355]{0}次[-]火鍋！！！", 
			           "[FFDD33]{1}[-] distance wins're just [FF3355] {0} times [-] hot! ! !");
		}
    }
}
