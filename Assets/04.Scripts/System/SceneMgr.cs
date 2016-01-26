using System.Collections;
using GameEnum;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int CurrentSceneNo = 0;

    IEnumerator LoadLevelCoroutine(string levelToLoad)
    {
        if (OnLevelWillBeLoaded != null)
            OnLevelWillBeLoaded();

        yield return SceneManager.LoadSceneAsync(levelToLoad.ToString());

        if(levelToLoad != ESceneName.Null.ToString())
         CurrentScene = levelToLoad;

        switch (levelToLoad)
        {
            case ESceneName.Null:
                SceneManager.UnloadScene(CurrentScene);
                System.GC.Collect();
                CurrentScene = levelToLoad;
			break;
			case ESceneName.Lobby:
                GameData.StageID = -1;
                UILoading.UIShow(true, ELoading.Lobby);
                break;

			case ESceneName.SelectRole:
                if (GameData.StageID >= 0)
                    UILoading.UIShow(true, ELoading.Stage);
                else
                    UILoading.UIShow(true, ELoading.SelectRole);
                
                break;
            default:
                //Court:
                UILoading.UIShow(true, ELoading.Game);
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
        CurrentSceneNo = courtNo;
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

