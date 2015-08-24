using UnityEngine;
using System;
using System.Collections;
using GameEnum;

public enum SceneName
{
	Main = 0,
	Null = 1,
	Lobby = 2,
	Court_0 = 3,
	Court_1 = 4,
	SelectRole = 5,
	Stage = 6
}

public class SceneMgr : KnightSingleton<SceneMgr>
{
	public SceneName CurrentScene = SceneName.Main;
	public SceneName LoadScene = SceneName.Main;

	public delegate void LevelWillBeLoaded();
	public delegate void LevelWasLoaded();
	
	public static event LevelWillBeLoaded OnLevelWillBeLoaded;
	public static event LevelWasLoaded OnLevelWasLoaded;
	
	IEnumerator LoadLevelCoroutine(SceneName levelToLoad)
	{
		if (OnLevelWillBeLoaded != null)
			OnLevelWillBeLoaded();

		yield return Application.LoadLevelAsync(levelToLoad.ToString());
			CurrentScene = levelToLoad;

		switch (levelToLoad) {
			case SceneName.Court_0:
			case SceneName.Court_1:
				UILoading.UIShow(true, ELoadingGamePic.Game); 
				CourtMgr.Get.InitCourtScene ();
				
				int rate = UnityEngine.Random.Range(0, 1);

				if(rate == 0)
					AudioMgr.Get.PlayMusic(EMusicType.MU_game0);
				else
					AudioMgr.Get.PlayMusic(EMusicType.MU_game1);
					
				break;
			case SceneName.SelectRole:
				AudioMgr.Get.StartGame();
				UILoading.UIShow(true, ELoadingGamePic.SelectRole);
				break;
			case SceneName.Stage:
				AudioMgr.Get.StartGame();
				UILoading.UIShow(true, ELoadingGamePic.Stage);
				break;
			case SceneName.Null:
				break;
		}

		if (OnLevelWasLoaded != null) {
			OnLevelWasLoaded ();
		}
	}

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

    public void ChangeLevel(SceneName scene, bool isNeedLoading = true)
    {
		if (CurrentScene == SceneName.Main) {
			StartCoroutine (LoadLevelCoroutine (scene));
		}
		else {
			if (CurrentScene != scene) {
				if(isNeedLoading)
					StartCoroutine(LoadLevelCoroutine(SceneName.Null));
				else 
					StartCoroutine(LoadLevelCoroutine(scene));

				LoadScene = scene;
				OnLevelWasLoaded += WaitLoadScene;
			}
		}
    }

	public void WaitLoadScene()
	{
		StartCoroutine(LoadLevelCoroutine(LoadScene));
		OnLevelWasLoaded -= WaitLoadScene;
	}

	public void SetDontDestory(GameObject obj){
		obj.transform.parent = gameObject.transform;
	}
}

