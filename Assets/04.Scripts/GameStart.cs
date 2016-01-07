#define PUSHWOOSH
using GameEnum;
using UnityEngine;

public class GameStart : MonoBehaviour {
    private static GameStart instance;
    public EGameTest TestMode = EGameTest.None;
    public EModelTest TestModel = EModelTest.None;
    public ECameraTest TestCameraMode = ECameraTest.None;
    public int CourtMode = ECourtMode.Full;
    public bool ConnectToServer = false;
    public bool OpenTutorial = false;
    public bool IsDebugAnimation = false;
    public bool IsAutoReplay = false;
    public bool IsShowPlayerInfo = false;
    public bool IsShowShootRate = false;
    public bool IsOpenChronos = true;
	public bool IsUseFpsLimiter = true;
	public bool IsOpenColorfulFX = true;
    public int FriendNumber = 3;
    public int GameWinValue = 13;
    public int GameWinTimeValue = 0;
    public EPlayerState SelectAniState = EPlayerState.Dunk6;
    public EBasketAnimationTest SelectBasketState = EBasketAnimationTest.Basket0;
    public ETestActive TestID = ETestActive.Dunk20;
    public int TestLv = 2;

    public static GameStart Get {
        get {
            return instance;
        }
    }

	void Awake() {
        instance = gameObject.GetComponent<GameStart>();
        DontDestroyOnLoad(gameObject);
		#if UNITY_EDITOR
		Application.runInBackground = IsDebugAnimation;
		GameObject obj = GameObject.Find("FileManager");
		if (obj) {
			Destroy(obj);
			obj = null;
		}
		#else
		Application.runInBackground = false;
		#endif

		Time.timeScale = 1;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		SceneMgr.Get.SetDontDestory (gameObject);
		TextConst.Init();
		GameData.Init();
        AnimatorMgr.Get.InitAnimtorStatesType();
		SendHttp.Get.CheckServerData(ConnectToServer);
		#if PUSHWOOSH
        gameObject.AddComponent<PushNotificator>();
		#endif
	}
}
