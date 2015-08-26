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

		}
    }
}
