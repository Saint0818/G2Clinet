//#define En
#define zh_TW
using UnityEngine;
using System;
using System.Collections;

public class GameStart : KnightSingleton<GameStart> {
	public static GameStart instance;
	public SceneTest  SceneMode = SceneTest.Single;
	public GameTest TestMode = GameTest.None;
	public CameraTest TestCameraMode = CameraTest.None;
	public bool IsSplitScreen = false;
	public float CrossTimeX = 0.5f;
	public float CrossTimeZ = 1;
	public TScoreRate ScoreRate = new TScoreRate(1);

    void Start(){
		SceneMgr.Get.SetDontDestory (gameObject);

		if (SceneMode == SceneTest.Single)
			SceneMgr.Get.ChangeLevel (SceneName.Court_0);

		GameData.Init();
		TextConst.Init();
	}

	void OnGUI()
	{
		if (SceneMode == SceneTest.Multi){
			if (GUI.Button (new Rect (100, 0, 100, 50), "Lobby"))
				{
					SceneMgr.Get.ChangeLevel(SceneName.Lobby);
				}
				
				if (GUI.Button (new Rect (200 , 0, 100, 50), "InGame"))
				{
					SceneMgr.Get.ChangeLevel(SceneName.Court_0);
				}
		}
	}

	private void InitCom(){
		GameData.Init ();
	}
}
