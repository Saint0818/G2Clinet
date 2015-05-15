using UnityEngine;
using System.Collections;
using System;
using RootMotion.FinalIK;

public enum SceneName
{
	Main = 0,
	Null = 1,
	Lobby = 2,
	Court_0 = 3,
	Court_1 = 4
}

public class SceneMgr : KnightSingleton<SceneMgr>
{
	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}


	
	public SceneName CurrentScene = SceneName.Main;
	public SceneName LoadScene = SceneName.Main;

	void OnLevelWasLoaded(int level) {
		if (level == 1) {
			Application.LoadLevel (LoadScene.ToString ());
		} else if (level == (int)LoadScene) {
			CurrentScene = LoadScene;
			switch(LoadScene){
				case SceneName.Court_0:
				case SceneName.Court_1:
					CourtMgr.Get.InitCourtScene();
				break;

				case SceneName.Lobby:
					
				break;
			}
		}
	}

    public void ChangeLevel(SceneName scene)
    {
		if (CurrentScene != scene) {
			Application.LoadLevel ("Null");
			LoadScene = scene;
		}
    }

	public void SetDontDestory(GameObject obj){
		obj.transform.parent = gameObject.transform;
	}
}

