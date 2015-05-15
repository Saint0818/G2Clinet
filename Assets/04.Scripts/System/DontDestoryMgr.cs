using UnityEngine;
using System.Collections;
using System;

public class DontDestoryMgr : KnightSingleton<DontDestoryMgr>
{
	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

	void OnGUI()
	{
		if (GUI.Button (new Rect (0, 0, Screen.width /2, 100), "Lobby"))
		{
			SceneMgr.Get.ChangeLevel(SceneName.Lobby);	
		}

		if (GUI.Button (new Rect (Screen.width + 5  - Screen.width /2 , 0, Screen.width /2, 100), "InGame"))
		{
			SceneMgr.Get.ChangeLevel(SceneName.Court_0);	
		}
	}
}

