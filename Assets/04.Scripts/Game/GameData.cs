using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

public class GameData {

	public static TAIlevel[] AIlevelAy;
	public static TTactical[] TacticalData;

	public static BasketShootPositionData[] BasketShootPosition;

	public static float AIChangeTime = 1;

	public static void Init()
	{
		List<TDownloadData> DownloadList = new List<TDownloadData> ();
		//ailevel
		DownloadList.Add (new TDownloadData ("ailevel", "0"));
		//tactical
		DownloadList.Add (new TDownloadData ("tactical", "0"));
		//basketShootPosition
		DownloadList.Add (new TDownloadData ("basketShootPosition", "0"));
		FileManager.Get.LoadFileResource (DownloadList);

		if(PlayerPrefs.HasKey("AIChangeTime"))
			AIChangeTime = PlayerPrefs.GetFloat("AIChangeTime", 1);
	}
}
