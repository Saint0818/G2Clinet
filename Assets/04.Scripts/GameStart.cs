//#define En
#define zh_TW
using UnityEngine;
using System;
using System.Collections;

public class GameStart : MonoBehaviour {
	public static GameStart instance;

	public GameTest TestMode = GameTest.None;
	public CameraTest TestCameraMode = CameraTest.None;
//	public bool IsOpenIKSystem = true;
	public bool IsSplitScreen = false;
	public float CrossTimeX = 0.5f;
	public float CrossTimeZ = 1;
	public TScoreRate ScoreRate = new TScoreRate(1);

	public static GameStart Get {
		get {
			if (!instance) {
				GameObject obj2 = GameObject.Find("GameStart");
				if (!obj2) {
					GameObject obj = new GameObject("GameStart");
					instance = obj.AddComponent<GameStart>();
				} else
					instance = obj2.GetComponent<GameStart>();
            }
            
            return instance;
        }
    }

    void Start(){
		GameData.Init();
		TextConst.Init();
		UIGame.UIShow (true);
	}

	private void InitCom(){
		GameData.Init ();
	}
}
