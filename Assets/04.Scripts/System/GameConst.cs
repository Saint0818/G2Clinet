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

public class GameConst
{
	public static Language GameLanguage = Language.zh_TW;

	//Game play move speed
	public const float BasicMoveSpeed = 1;
	public const float DefSpeedup = 8.2f;
	public const float DefSpeedNormal = 7.2f;
	public const float BallOwnerSpeedup = 7.5f;
	public const float BallOwnerSpeedNormal = 6.5f;
	public const float AttackSpeedup = 8;
	public const float AttackSpeedNormal = 7;
	public const float TreePointDistance = 12;
	public const float TwoPointDistance = 3;
	public const float DunkDistance = 7;
	public const float PickBallDistance = 2.5f;
	public const float StealBallDistance = 2;
	public const float BlockDistance = 5;
    public const int StealReate = 30;
    public const int StealReate_Success = 10;
	public const int FakeShootRate = 40;
	public const float DefMoveTime = 0.2f;
	public static int SelfAILevel = 0;
	public static int NpcAILevel = 0;
	public static float MiddleDistance = 12;
	public static float CloseDistance = 4;
	public static float DefDistance = 3.5f;
	public static string[] Angle90 = {"0","1","2","3","4"};
	public static string[] Angle0 = {"0","1"};
	public static string[] AngleRight45 = {"0","1","2","3","4"};
	public static string[] AngleLeft45 = {"0","1","2","3","4"}; 
}
