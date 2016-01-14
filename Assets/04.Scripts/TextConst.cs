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
	private static bool loaded = false;
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

			//UINamed
			AddString(9000000,"更改名稱");
			AddString(9000001,"請輸入名稱八字字");
			AddString(9000002,"本次修改需花費");
			AddString (9000003, "確定修改名稱？", "");
			AddString (9000004, "非法字元請重新輸入", "");
			AddString (9000005, "字元長度請介於1-12個字", "");
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

                    return TextConst.S(textNo) + "\n" + GameData.DItemData[e.Value].Name + " X " + e.Num.ToString();
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
                sResult = string.Format(TextConst.S(245), d, h);
            else 
            if (h > 0)
                sResult = string.Format(TextConst.S(244), h, m);
             else
                sResult = string.Format(TextConst.S(243), m, s);

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

    public static string DeadlineString(DateTime time) {
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
                sResult = string.Format("{0}:{1}:{2}", d, h, m);
            else 
            if (h > 0)
                sResult = string.Format("{0}:{1}:{2}", h, m, s);
            else
            if (m > 0)
                sResult = string.Format("{0}:{1}", m, s);
            else
            if (s > 0)
                sResult = s.ToString();
        } catch {
            sResult = string.Format("{0}s", sec.ToString());    
        }

        return sResult;
    }
}
