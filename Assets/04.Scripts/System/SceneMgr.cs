using UnityEngine;
using System;
using System.Collections;

public enum SceneName
{
	Main = 0,
	Null = 1,
	Lobby = 2,
	Court_0 = 3,
	Court_1 = 4,
	SelectRole = 5
}

public class SceneMgr : KnightSingleton<SceneMgr>
{
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
				CourtMgr.Get.InitCourtScene ();
				break;
			case SceneName.SelectRole:
			//				OnLevelWasLoaded += loadingShow;
				UILoading.UIShow(true, ELoadingGamePic.None);
				Invoke("loadSelectRole", 1);
				break;
			case SceneName.Null:
			//				UIWaitLoading.UIShow(true);
//				OnLevelWasLoaded -= loadingShow;
				UILoading.UIShow(true, ELoadingGamePic.Game); 
				break;
		}

		if (OnLevelWasLoaded != null) {
			OnLevelWasLoaded ();
		}
	}

	private void loadSelectRole(){
		CameraMgr.Get.SetSelectRoleCamera();
		UISelectRole.UIShow(true);
		UI3DSelectRole.UIShow(true);
	}

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

	public SceneName CurrentScene = SceneName.Main;
	public SceneName LoadScene = SceneName.Main;

    public void ChangeLevel(SceneName scene)
    {
		if (CurrentScene == SceneName.Main) {
			StartCoroutine (LoadLevelCoroutine (scene));
		}
		else {
			if (CurrentScene != scene) {
				StartCoroutine(LoadLevelCoroutine(SceneName.Null));
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

