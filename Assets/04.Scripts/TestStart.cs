using UnityEngine;
using System.Collections;
using GameEnum;

public class TestStart : MonoBehaviour {
    //for game play
    public EGameTest TestMode = EGameTest.None;
    public EModelTest TestModel = EModelTest.None;
    public ECameraTest TestCameraMode = ECameraTest.None;
    public EPlayerState SelectAniState = EPlayerState.Dunk6;
    public ETestActive TestID = ETestActive.Dunk20;
    public int TestLv = 2;
    public bool IsDebugAnimation = false;
    public int StageID = 101;
    public int[] PlayerID = {31, 32, 33, 100, 101, 102};
	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
        GameObject obj = GameObject.Find("AudioMgr");
        if (!obj) {
            obj = Resources.Load("Prefab/AudioMgr") as GameObject;
            obj = Instantiate(obj) as GameObject;
            obj.name = "AudioMgr";
        }

        GameData.Init();
        GameData.TestStage = true;

        TStageData stageData = StageTable.Ins.GetByID(StageID);
        CourtMgr.Get.InitCourtScene(stageData.CourtNo);
        CourtMgr.Get.ShowEnd();
        CameraMgr.Get.TestCameraMode = TestCameraMode;

        GameController.Get.ChangeSituation(EGameSituation.None);

        //set test data
        GameController.Get.TestMode = TestMode;
        GameController.Get.TestModel = TestModel;
        GameController.Get.SelectAniState = SelectAniState;
        GameController.Get.TestID = TestID;
        GameController.Get.TestLv = TestLv;
        GameController.Get.IsDebugAnimation = IsDebugAnimation;
        GameController.Get.TestPlayerID = PlayerID;

        GameController.Get.LoadStage(stageData.ID);
        GameController.Get.InitIngameAnimator();
        GameController.Get.SetBornPositions();
        GameController.Get.ChangeSituation(EGameSituation.JumpBall);
        AIController.Get.ChangeState(EGameSituation.JumpBall);
        CameraMgr.Get.ShowPlayerInfoCamera(true);

        //set test data
        CameraMgr.Get.TestCameraMode = TestCameraMode;
        CourtMgr.Get.RealBall.TestMode = TestMode;
	}
}
