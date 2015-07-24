using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;

public static class TextConst
{
	private static bool loaded = false;
	private static Dictionary<int, string[]> GameText = new Dictionary<int, string[]> ();

	private static void AddString(int key, string Text_TW, string Text_EN = ""){
		if (!GameText.ContainsKey (key)) {
			string [] Data = new string[2];
			Data[GameStruct.ELanguage.TW.GetHashCode()] = Text_TW;
			Data[GameStruct.ELanguage.EN.GetHashCode()] = Text_EN;
			GameText.Add(key, Data);
		}else
			Debug.Log("Repeat text key : " + key);
	}

	public static string S (int index){
		if(GameText.ContainsKey(index))
			return GameText[index][GameData.Setting.Language.GetHashCode()];
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
			AddString (12, "2PT-2分球命中率\n3PT-3分球命中率\nSPD-速度\nSTA-耐力", "2PT-2Point\n3PT-3Point\nSPD-Speed\nSTA-Stamina"); 
			AddString (13, "REB-籃板\nDNK-灌籃\nBLK-火鍋\nSTR-力量", "REB-Rebound\nDNK-Dunk\nBLK-Block\nSTR-Strength");
			AddString (14, "DEF-防守\nSTL-抄截\nDRB-控球\nPAS-傳球", "DEF-Defence\nSTL-Steal\nDRB-Dribble\nPAS-Pass");

		}
    }
}
