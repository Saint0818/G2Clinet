using UnityEngine;
using System;
using System.Collections;
using Newtonsoft.Json;
using GameStruct;

//public delegate void CallBack();

public struct TPlayerObject {
	public GameObject PlayerObject;
	public TTeam PlayerData;
	public TScenePlayer ScenePlayer;
}

public class LobbyStart : MonoBehaviour {
    private static LobbyStart instance;

    public static LobbyStart Get {
        get {
            return instance;
        }
    }

    public static bool Visible {
        get {
            return instance && instance.gameObject.activeInHierarchy;
        }
    }

	void Awake ()
    {
        instance = gameObject.GetComponent<LobbyStart>();
        //DontDestroyOnLoad(gameObject);
		Time.timeScale = 1;
    }

    public void EnterLobby()
    {
		try
        {
			UILoading.UIShow(false);
			UIMainLobby.Get.Show();
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
}
