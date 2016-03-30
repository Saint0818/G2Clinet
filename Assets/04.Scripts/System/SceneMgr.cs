using System.Collections;
using GameEnum;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ESceneName
{
	public const string Null = "Null";
	public const string Lobby = "Lobby";
	public const string SelectRole = "SelectRole";
	public const string Court = "Court_";
}

public class SceneMgr : KnightSingleton<SceneMgr>
{
    public string CurrentScene = ESceneName.Lobby;
    public string LoadScene = ESceneName.Lobby;

    public delegate void LevelWillBeLoaded();
    public delegate void LevelWaitLoadNextScene();
	public delegate void LevelWasFinishedDoSomething();
    public int CurrentSceneNo = 0;

    IEnumerator LoadLevelCoroutine(string levelToLoad)
    {
        yield return SceneManager.LoadSceneAsync(levelToLoad);

        if(levelToLoad != ESceneName.Null.ToString())
            CurrentScene = levelToLoad;

        switch (levelToLoad)
        {
            case ESceneName.Null:
                SceneManager.UnloadScene(CurrentScene);
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

        Resources.UnloadUnusedAssets();
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeLevel(int courtNo, bool isNeedLoading = false)
    {
		string scene = ESceneName.Court + courtNo;
		ChangeLevel(scene, isNeedLoading);
        CurrentSceneNo = courtNo;
    }
        
    public bool IsCourt
    {
        get{  return CurrentScene == string.Format("{0}{1}", ESceneName.Court, CurrentSceneNo);}
    }

    public void ChangeLevel(string scene, bool isNeedLoading = false)
    {
        
        if (CurrentScene != scene)
        {
            if (isNeedLoading)
                StartCoroutine(LoadLevelCoroutine("Null"));
            else 
                StartCoroutine(LoadLevelCoroutine(scene));

            LoadScene = scene;
        }
    }
}

