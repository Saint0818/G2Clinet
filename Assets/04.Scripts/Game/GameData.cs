using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

public class GameData {

	public static TAIlevel[] AIlevelAy;
	public static TTactical[] TacticalData;

	public static void Init()
	{
		List<TUpdateData> DownloadList = new List<TUpdateData> ();
		TUpdateData data = new TUpdateData ("ailevel", "0");
		DownloadList.Add (data);
		data = new TUpdateData ("tactical", "0");
		DownloadList.Add (data);

		FileManager.Get.LoadFileResource (DownloadList);
	}
}
