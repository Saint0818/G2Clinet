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
    public string CurrentScene = "Main";
    public string LoadScene = "Main";

    public delegate void LevelWillBeLoaded();
    public delegate void LevelWasLoaded();
    
    public static event LevelWillBeLoaded OnLevelWillBeLoaded;
    public static event LevelWasLoaded OnLevelWasLoaded;
    
    IEnumerator LoadLevelCoroutine(string levelToLoad)
    {
        if (OnLevelWillBeLoaded != null)
            OnLevelWillBeLoaded();

        yield return Application.LoadLevelAsync(levelToLoad.ToString());
        CurrentScene = levelToLoad;

        switch (levelToLoad)
        {
            case "Lobby":
                GameData.StageID = -1;
                UILoading.UIShow(true, ELoadingGamePic.Lobby);
                break;

            case "SelectRole":
                if (GameData.StageID >= 0)
                    UILoading.UIShow(true, ELoadingGamePic.Stage);
                else
                {
                    GameData.StageID = -1;
                    UILoading.UIShow(true, ELoadingGamePic.SelectRole);
                }
                break;

            case "Null":
                break;

            default:
                //Court:
                UILoading.UIShow(true, ELoadingGamePic.Game);
                break;
        }

        if (OnLevelWasLoaded != null)
        {
            OnLevelWasLoaded();
        }

        Resources.UnloadUnusedAssets();
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void ChangeLevel(int courtNo, bool isNeedLoading = true)
    {
        string scene = string.Format("Court_{0}", courtNo);
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
                OnLevelWasLoaded += WaitLoadScene;
            }
        }
    }

    public void WaitLoadScene()
    {
        StartCoroutine(LoadLevelCoroutine(LoadScene));
        OnLevelWasLoaded -= WaitLoadScene;
    }

    public void SetDontDestory(GameObject obj)
    {
        obj.transform.parent = gameObject.transform;
    }
}

