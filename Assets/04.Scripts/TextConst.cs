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
			return GameText[index][ParameterConst.GameLanguage.GetHashCode()];
		else
			return "";
	} 
	
	public static void Init(){
		AddString (0, "", "");
		AddString (1001, "2-Point", "2-Point");
		AddString (1002, "3-Point", "3-Point");
		AddString (1003, "Dunk", "Dunk");
		AddString (1004, "Steal", "Steal");
		AddString (1005, "Block", "Block");
		AddString (1006, "Pass A", "Pass A");
		AddString (1007, "Pass B", "Pass B");
    }
}
