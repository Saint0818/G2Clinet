using System.Collections;
using GameEnum;
using UnityEngine;

public enum ESceneName
{
	Null = 0,
	Main = 1,
	Lobby = 2,
	SelectRole = 3,
	Court_0 = 4,
	Court_1 = 5,
}

public class SceneMgr : KnightSingleton<SceneMgr>
{
	public ESceneName CurrentScene = ESceneName.Main;
	public ESceneName LoadScene = ESceneName.Main;

	public delegate void LevelWillBeLoaded();
	public delegate void LevelWasLoaded();
	
	public static event LevelWillBeLoaded OnLevelWillBeLoaded;
	public static event LevelWasLoaded OnLevelWasLoaded;
	
	IEnumerator LoadLevelCoroutine(ESceneName levelToLoad)
	{
		if (OnLevelWillBeLoaded != null)
			OnLevelWillBeLoaded();

		yield return Application.LoadLevelAsync(levelToLoad.ToString());
			CurrentScene = levelToLoad;

		switch (levelToLoad) {
			case ESceneName.Lobby:
				GameData.StageID = -1;
				UILoading.UIShow(true, ELoadingGamePic.Lobby);

				break;
            case ESceneName.Court_0:
			case ESceneName.Court_1:
				UILoading.UIShow(true, ELoadingGamePic.Game);
				break;

			case ESceneName.SelectRole:
				if (GameData.StageID >= 0)
					UILoading.UIShow(true, ELoadingGamePic.Stage); 
				else {
					GameData.StageID = -1;
	                UILoading.UIShow(true, ELoadingGamePic.SelectRole);
				}

				break;

			case ESceneName.Null:
				break;
		}

		if (OnLevelWasLoaded != null) {
			OnLevelWasLoaded ();
		}

		Resources.UnloadUnusedAssets();
	}

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

    public void ChangeLevel(ESceneName scene, bool isNeedLoading = true)
    {
		if (CurrentScene == ESceneName.Main) {
			StartCoroutine (LoadLevelCoroutine (scene));
			LoadScene = scene;
		}
		else {
			if (CurrentScene != scene) {
				if(isNeedLoading)
					StartCoroutine(LoadLevelCoroutine(ESceneName.Null));
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

