using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;

public static class TextConst
{
	private static Dictionary<int, string[]> GameText = new Dictionary<int, string[]> ();

	private static void AddString(int key, string Text_TW, string Text_EN = ""){
		if (!GameText.ContainsKey (key)) {
			string [] Data = new string[2];
			Data[Language.zh_TW.GetHashCode()] = Text_TW;
			Data[Language.en.GetHashCode()] = Text_EN;
			GameText.Add(key, Data);
		}else
			Debug.Log("Repeat text key : " + key);
	}

	public static string S (int index){
		if(GameText.ContainsKey(index))
			return GameText[index][GameConst.GameLanguage.GetHashCode()];
		else
			return "";
	} 
	
	public static void Init(){
		AddString (0, "", "");
		AddString (1, "重新開始", "重新開始"); //1~1000 UI
		AddString (2, "點擊開始", "點擊開始");
		AddString (3, "點擊繼續", "點擊繼續");
    }
}
