//#define OutFile
using UnityEngine;
using System;
using System.Collections.Generic;
using GameEnum;

public struct TTextConst {
	public int ID;
	public string TW;
	public string CN;
	public string JP;
	public string EN;
	
	public string Text {
		get{
			switch(GameData.Setting.Language){
			case ELanguage.TW: return TW;
			case ELanguage.CN: return CN;
			case ELanguage.JP: return JP;
			default: return EN;
			}
		}
	}
}

public static class TextConst
{
	private static Dictionary<int, TTextConst> gameText = new Dictionary<int, TTextConst> ();
	public static TTeamName[] TeamNameAy;
	
	#if OutFile
	private static StringBuilder sb = new StringBuilder();
	#endif
	
	private static void AddString(int id, string textTW, string textCN = "", string textEN = "", string textJP = ""){
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

		TTextConst data = new TTextConst();
		data.ID = id;
		data.EN = textEN;
		data.TW = textTW;
		data.CN = textCN;
		data.JP = textJP;

		if (gameText.ContainsKey (id)) {
			gameText[id] = data;
			Debug.Log("Add the same text " + data.ID.ToString());
		} else
			gameText.Add(id, data);
	}

	public static bool HasText(int id) {
		return gameText.ContainsKey(id);
	}

	public static string S (int index){
		if(gameText.ContainsKey(index))
			return gameText[index].Text;
		else
            return index.ToString();
	}

	public static string StringFormat(int stringId, params object[] arguments)
	{
		string format = S (stringId);
		if (format == string.Empty || format == null) {
			return string.Empty;
		}else
			return string.Format(format, arguments);
	}

	public static void LoadText(ref TTextConst[] data) {
		for (int i = 0; i < data.Length; i ++) {
			if (gameText.ContainsKey(data[i].ID)) {
				gameText[data[i].ID] = data[i];
				Debug.Log("Repeat text " + data[i].ID.ToString());
			} else
				gameText.Add(data[i].ID, data[i]);
		}
	}

    public static string GetSocialText(GameStruct.TSocialEvent e) {
        int textNo = 0;
        switch (e.Kind) {
            case 1: //friend
                switch (e.Value) {
                    case 2: return "\n" + TextConst.S(5029);
                    case 3: return "\n" + TextConst.S(5024);
                    case 4: return "\n" + TextConst.S(5030);
                }

                break;
            case 3: //mission
                if (GameData.DMissionData.ContainsKey(e.Value)) {
                    switch (GameData.DMissionData[e.Value].TimeKind) {
                        case 1: textNo = 5039; break;
                        case 2: textNo = 5040; break;
                        case 3: textNo = 5041; break;
                        default : textNo = 5038; break;
                    }
                    return S(textNo) + "\n" + GameData.DMissionData[e.Value].Name;
                }

                break;
            case 4: //item
                if (GameData.DItemData.ContainsKey(e.Value)) {
                    textNo = 10327;
                    if (e.Cause == 11)
                        textNo = 5036;

					return TextConst.S(textNo) + "\n" + GameData.DItemData[e.Value].Name; // + " X " + e.Num.ToString();
                }

                break;
        }

        return "";
    }

    public static string SecondString(int sec) {
        int d = 0;
        int s = 0; 
        int m = 0; 
        int h = 0; 
        string sResult = "";

        try {
            s = sec % 60; 

            if (sec >= 60) {
                m = sec / 60; 
                if (m >= 60) {
                    h = m / 60;
                    m = m % 60;  

                    if (h >= 24) {
                        d = (h) / 24; 
                        h = h % 24;
                    }
                }
            }

            if (d > 0)
                sResult = string.Format(TextConst.S(243), d, h);
            else 
            if (h > 0)
                sResult = string.Format(TextConst.S(244), h, m);
            else
			if (m > 0)
				sResult = string.Format(TextConst.S(245), m, s);
			else
			if (s > 0)
				sResult = s.ToString();
        } catch {
            sResult = string.Format("{0}s", sec.ToString());    
        }

        return sResult;
    }

    public static string SecondString(DateTime time) {
        int sec = (int)(new System.TimeSpan(time.Ticks - DateTime.UtcNow.Ticks).TotalSeconds);
        int d = 0;
        int s = 0; 
        int m = 0; 
        int h = 0; 
        string sResult = "";

        try {
            s = sec % 60; 

            if (sec >= 60) {
                m = sec / 60; 
                if (m >= 60) {
                    h = m / 60;
                    m = m % 60;  

                    if (h >= 24) {
                        d = (h) / 24; 
                        h = h % 24;
                    }
                }
            }

            if (d > 0)
                sResult = string.Format(TextConst.S(243), d, h);
            else 
                if (h > 0)
                    sResult = string.Format(TextConst.S(244), h, m);
                else
                    if (m > 0)
                        sResult = string.Format(TextConst.S(245), m, s);
                    else
                        if (s > 0)
                            sResult = s.ToString();
        } catch {
            sResult = string.Format("{0}s", sec.ToString());    
        }

        return sResult;
    }

    public static string AfterTimeString(DateTime time) {
        int dt = (int)(new System.TimeSpan(DateTime.UtcNow.Ticks - time.Ticks).TotalSeconds / 60);
        if (dt <= 0)
            dt = 1;
        
        int t = 0;
        if (dt < 60)
            return dt.ToString()+S(246);
        else //after hour
        if (dt < 1440) {
            t = dt / 60;
            return t.ToString() + S(247);
        } else //after 7 days
        if (dt < 10080) {
            t = dt / 1440;
            return t.ToString() + S(248);
        } else
            return time.ToString();
    }
        
	public static float DeadlineStringPercent (DateTime oriTime, DateTime time) {
		float total = (float)(new System.TimeSpan(time.Ticks - oriTime.Ticks).TotalSeconds);
		float current = (float)(new System.TimeSpan(DateTime.UtcNow.Ticks - oriTime.Ticks).TotalSeconds);

		return current / total;
	}
	/// <summary>
	/// Color the specified quality.
	/// </summary>
	/// <param name="quality">Quality.</param>
    public static Color32 Color(int quality) {
        if (quality < 2)
            return new Color32(255, 255, 255, 255);
        else
        if (quality < 3)
            return new Color32(0, 179, 90, 255);
        else
        if (quality < 4)
            return new Color32(51, 187, 255, 255);
        else
        if (quality < 5)
            return new Color32(150, 101, 255, 255);
        else
            return new Color32(255, 128, 0, 255);
	}
	/// <summary>
	/// Colors the B.
	/// </summary>
	/// <returns>The B.</returns>
	/// <param name="quality">Quality.</param>
	public static Color32 ColorBG(int quality) {
		if (quality < 2)
			return new Color32(150, 150, 150, 255);
		else
			if (quality < 3)
				return new Color32(20, 125, 35, 255);
			else
				if (quality < 4)
					return new Color32(20, 100, 150, 255);
				else
					if (quality < 5)
						return new Color32(175, 40, 200, 255);
					else
						return new Color32(255, 145, 0, 255);
	}
}
