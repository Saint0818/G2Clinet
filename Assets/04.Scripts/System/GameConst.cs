﻿using UnityEngine;
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
	public const float TreePointDistance = 10;
	public const float TwoPointDistance = 3;
	public const float DunkDistance = 7;
	public const float PickBallDistance = 2.5f;
	public const float StealBallDistance = 2;
	public const float PushPlayerDistance = 1;
	public const float BlockDistance = 5;
    public const int StealReate = 30;
    public const int StealReate_Success = 10;
}
