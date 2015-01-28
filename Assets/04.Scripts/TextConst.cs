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

	public static string S (int index)
	{
		if(GameText.ContainsKey(index))
			return GameText[index][ParameterConst.GameLanguage.GetHashCode()];
		else
			return "";
	} 
	
	public static void Init(){
		AddString (0, "", "");
    }
}
