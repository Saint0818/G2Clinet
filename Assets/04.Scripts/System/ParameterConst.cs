using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

public delegate void TBooleanWWWObj(bool val, WWW Result);

public enum Language
{
	zh_TW = 0,
	en = 1
}

public static class URLConst {
	public const string GooglePlay = "https://play.google.com/store/apps/details?id=com.nicemarket.nbaa";
	public const string NiceMarketApk = "http://nicemarket.com.tw/assets/apk/BaskClub.apk";
	public const string Version = FileManager.URL + "version";
	public const string CheckSession = FileManager.URL + "checksession";
	public const string deviceLogin = FileManager.URL + "devicelogin";
}

public class ParameterConst
{
	public static Language GameLanguage = Language.zh_TW;
	public static TAIlevel [] AIlevelAy = new TAIlevel[10];

	public static void Init(){
		AIlevelAy [0].ProactiveRate = 5;
		AIlevelAy [0].AutoFollowTime = 3;
		AIlevelAy [0].DefDistance = 3;

		AIlevelAy [1].ProactiveRate = 10;
		AIlevelAy [1].AutoFollowTime = 3;
		AIlevelAy [1].DefDistance = 3;

		AIlevelAy [2].ProactiveRate = 15;
		AIlevelAy [2].AutoFollowTime = 3;
		AIlevelAy [2].DefDistance = 3;

		AIlevelAy [3].ProactiveRate = 20;
		AIlevelAy [3].AutoFollowTime = 3;
		AIlevelAy [3].DefDistance = 3;

		AIlevelAy [4].ProactiveRate = 25;
		AIlevelAy [4].AutoFollowTime = 3;
		AIlevelAy [4].DefDistance = 3;

		AIlevelAy [5].ProactiveRate = 30;
		AIlevelAy [5].AutoFollowTime = 3;

		AIlevelAy [6].ProactiveRate = 35;
		AIlevelAy [6].AutoFollowTime = 3;
		AIlevelAy [6].DefDistance = 3;

		AIlevelAy [7].ProactiveRate = 40;
		AIlevelAy [7].AutoFollowTime = 3;
		AIlevelAy [7].DefDistance = 3;

		AIlevelAy [8].ProactiveRate = 45;
		AIlevelAy [8].AutoFollowTime = 3;
		AIlevelAy [8].DefDistance = 3;

		AIlevelAy [9].ProactiveRate = 50;
		AIlevelAy [9].AutoFollowTime = 3;
		AIlevelAy [9].DefDistance = 3;
	}
}
