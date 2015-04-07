using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

public class GameData {

	public static TAIlevel[] AIlevelAy;
	public static TTactical[] TacticalData;

	public static void Init()
	{
		List<TDownloadData> DownloadList = new List<TDownloadData> ();
		TDownloadData data;
		//ailevel
		data = new TDownloadData ("ailevel", "0");
		DownloadList.Add (data);
		//tactical
		data = new TDownloadData ("tactical", "0");
		DownloadList.Add (data);

		FileManager.Get.LoadFileResource (DownloadList);
	}
}
