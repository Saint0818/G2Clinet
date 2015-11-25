using System.Collections;
using GameEnum;
using UnityEngine;

public static class ESceneName
{
	public const string Null = "Null";
	public const string Main = "Main";
	public const string Lobby = "Lobby";
	public const string SelectRole = "SelectRole";
	public const string Court = "Court_";
}

public class SceneMgr : KnightSingleton<SceneMgr>
{
	public string CurrentScene = ESceneName.Main;
	public string LoadScene = ESceneName.Main;

    public delegate void LevelWillBeLoaded();
    public delegate void LevelWaitLoadNextScene();
	public delegate void LevelWasFinishedDoSomething();
	
    public static event LevelWillBeLoaded OnLevelWillBeLoaded;
    public static event LevelWaitLoadNextScene OnLevelWaitLoadNext;
	public LevelWasFinishedDoSomething OnOpenStae = null;
    
    IEnumerator LoadLevelCoroutine(string levelToLoad)
    {
        if (OnLevelWillBeLoaded != null)
            OnLevelWillBeLoaded();

        yield return Application.LoadLevelAsync(levelToLoad.ToString());
        CurrentScene = levelToLoad;

        switch (levelToLoad)
        {
			case ESceneName.Null:
			break;
			case ESceneName.Lobby:
                GameData.StageID = -1;
                UILoading.UIShow(true, ELoadingGamePic.Lobby);
                break;

			case ESceneName.SelectRole:
                if (GameData.StageID >= 0)
                    UILoading.UIShow(true, ELoadingGamePic.Stage);
                else
                {
                    GameData.StageID = -1;
                    UILoading.UIShow(true, ELoadingGamePic.SelectRole);
                }
                break;
            default:
                //Court:
                UILoading.UIShow(true, ELoadingGamePic.Game);
                break;
        }

        if (OnLevelWaitLoadNext != null) 
			OnLevelWaitLoadNext ();

        Resources.UnloadUnusedAssets();
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void ChangeLevel(int courtNo, bool isNeedLoading = true)
    {
		string scene = ESceneName.Court + courtNo;
		ChangeLevel(scene, isNeedLoading);
    }

    public void ChangeLevel(string scene, bool isNeedLoading = true)
    {
        if (CurrentScene == "Main")
        {
            StartCoroutine(LoadLevelCoroutine(scene));
            LoadScene = scene;
        } else
        {
            if (CurrentScene != scene)
            {
                if (isNeedLoading)
                    StartCoroutine(LoadLevelCoroutine("Null"));
                else 
                    StartCoroutine(LoadLevelCoroutine(scene));

                LoadScene = scene;
                OnLevelWaitLoadNext += WaitLoadScene;
            }
        }
    }

	public void ChangeLevel(string scene, bool openstage, bool isNeedLoading)
	{
		if (openstage)
			OnOpenStae = OpenStageUI;
		ChangeLevel (scene, isNeedLoading);
	}

	private void OpenStageUI()
	{
		UIMainStage.Get.Show ();
		UIMainLobby.Get.Hide ();
	}

	public bool CheckNeedOpenStageUI()
	{
		if(OnOpenStae != null)
		{
			OnOpenStae();
			OnOpenStae = null;
			return true;
		}

		return false;
	}

    public void WaitLoadScene()
    {
        StartCoroutine(LoadLevelCoroutine(LoadScene));
        OnLevelWaitLoadNext -= WaitLoadScene;
    }

    public void SetDontDestory(GameObject obj)
    {
        obj.transform.parent = gameObject.transform;
    }
}

