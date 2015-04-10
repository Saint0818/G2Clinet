using UnityEngine;
using System.Collections;

public class UIGame : UIBase {
	private static UIGame instance = null;
	private const string UIName = "UIGame";

	//Game const
	public float ButtonBTime = 0.2f; //Fake to shoot time
	private float showScoreBarInitTime = 2;
	
	private float shootBtnTime = 0;
	private float showScoreBarTime = 0;
	private float homeHintTime = -1;
	public int[] MaxScores = {13, 13};
	public int[] Scores = {0, 0};

	private bool isPressShootBtn = false;
	private bool isShowScoreBar = false;
	public GameObject Again;
	public GameObject Continue;
	public GameObject Start;
	public GameObject ScoreBar;
	public GameObject Restart;
	public GameJoystick Joystick = null;
	private DrawLine drawLine;
	private MovingJoystick Move = new MovingJoystick();
	
	private GameObject[] ControlButtonGroup= new GameObject[2];
	private GameObject passObject;
	private GameObject[] passObjectGroup = new GameObject[2];
	private GameObject screenLocation;
	private UILabel[] scoresLabel = new UILabel[2];
	private UIScrollBar[] aiLevelScrollBar = new UIScrollBar[2];
	private string[] aryHomeHintString = new string[3];

	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow) {
		if(instance)
			instance.Show(isShow);
		else
		if(isShow)
			Get.Show(isShow);
	}
	
	public static UIGame Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGame;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		Joystick = GameObject.Find (UIName + "/GameJoystick").GetComponent<GameJoystick>();
		Joystick.Joystick = GameObject.Find (UIName + "GameJoystick").GetComponent<EasyJoystick>();
		Again = GameObject.Find (UIName + "/Center/ButtonAgain");
		Continue = GameObject.Find (UIName + "/Center/ButtonContinue");
		Start = GameObject.Find (UIName + "/Center/StartView");
		Restart = GameObject.Find (UIName + "/Center/ButtonReset");
		ScoreBar = GameObject.Find(UIName + "/Top/ScoreBar");
		scoresLabel [0] = GameObject.Find (UIName + "/Top/ScoreBar/LabelScore1").GetComponent<UILabel>();
		scoresLabel [1] = GameObject.Find (UIName + "/Top/ScoreBar/LabelScore2").GetComponent<UILabel>();

		ControlButtonGroup [0] = GameObject.Find (UIName + "/BottomRight/Attack");
		ControlButtonGroup [1] = GameObject.Find (UIName + "/BottomRight/Defance");

		passObjectGroup [0] = GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectA");
		passObjectGroup [1] = GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectB");
		passObject = GameObject.Find (UIName + "/BottomRight/Attack/PassObject");

		aiLevelScrollBar [0] = GameObject.Find(UIName + "/Center/StartView/AISelect/HomeScrollBar").GetComponent<UIScrollBar>();
		aiLevelScrollBar [1] = GameObject.Find(UIName + "/Center/StartView/AISelect/AwayScrollBar").GetComponent<UIScrollBar>();

		screenLocation = GameObject.Find (UIName + "/Right");

		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonShoot")).onPress = DoShoot;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonPass")).onPress = DoPassChoose;;

//		UIEventListener.Get (GameObject.Find (UIName + "/Center/StartView/AISelect/HomeScrollBar")).onScroll = changeSelfAILevel;
//		UIEventListener.Get (GameObject.Find (UIName + "/Center/StartView/AISelect/AwayScrollBar")).onScroll = changeNpcAILevel;
		aiLevelScrollBar[0].onChange.Add(new EventDelegate(changeSelfAILevel));
		aiLevelScrollBar[1].onChange.Add(new EventDelegate(changeNpcAILevel));

		SetBtnFun (UIName + "/BottomRight/Attack/PassObject/ButtonObjectA", DoPassTeammateA);
		SetBtnFun (UIName + "/BottomRight/Attack/PassObject/ButtonObjectB", DoPassTeammateB);
		SetBtnFun (UIName + "/BottomRight/Attack/ButtonShoot", GameController.Get.DoSkill);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonSteal", GameController.Get.DoSteal);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonBlock", GameController.Get.DoBlock);
		SetBtnFun (UIName + "/Center/ButtonAgain", ResetGame);
		SetBtnFun (UIName + "/Center/StartView/ButtonStart", StartGame);
		SetBtnFun (UIName + "/Center/ButtonContinue", ContinueGame);
		SetBtnFun (UIName + "/TopLeft/ButtonPause", PauseGame);
		SetBtnFun (UIName + "/Center/ButtonReset", RestartGame);
		Again.SetActive (false);
		Restart.SetActive(false);
		Continue.SetActive(false);
		passObject.SetActive(false);

		drawLine = gameObject.AddComponent<DrawLine>();
		ChangeControl(false);
		
		Joystick.gameObject.SetActive(false);
	}

	protected override void InitText(){
		SetLabel(UIName + "/Center/ButtonAgain/LabelReset" ,TextConst.S(1));
		SetLabel(UIName + "/Center/StartView/ButtonStart/LabelStart" ,TextConst.S(2));
		SetLabel(UIName + "/Center/ButtonContinue/LabelContinue" ,TextConst.S(3));
		SetLabel(UIName + "/Center/ButtonReset/LabelReset" ,TextConst.S(4));
		SetLabel(UIName + "/Center/StartView/AISelect/LabelAI" ,TextConst.S(5));
		
	}

	private void initLine() {
		drawLine.ClearTarget();
		if (drawLine.UIs.Length == 0) {
			for (int i = 0; i < 2; i ++) {
				GameObject obj = GameObject.Find("PlayerInfoModel/Self" + (i+1).ToString());
				if (obj)
					drawLine.AddTarget(passObjectGroup[i], obj);
			}
		}
	}

	public void changeSelfAILevel(){
		GameStart.Get.SelfAILevel =(int) Mathf.Round(aiLevelScrollBar[0].value * 5);
	}

	public void changeNpcAILevel(){
		GameStart.Get.NpcAILevel =(int)  Mathf.Round(aiLevelScrollBar[1].value * 5);		
	}

	public void DoShoot(GameObject go, bool state) {
		if(state)
			shootBtnTime = ButtonBTime;
		else if(!state && shootBtnTime > 0){
			GameController.Get.DoShoot (false);
			shootBtnTime = ButtonBTime;
		}

		isPressShootBtn = state;
	}
	
	public void DoPassChoose (GameObject obj, bool state) {
		if(GameController.Get.Joysticker.IsBallOwner) {
			initLine();
			passObject.SetActive(state);
			drawLine.IsShow = state;
		} else {
			if(!GameController.Get.IsShooting)
				GameController.Get.DoPass(0);
		}
	}

	public void DoPassTeammateA() {
		if(!GameController.Get.IsShooting)
			GameController.Get.DoPass(1);
		passObject.SetActive(false);
		drawLine.IsShow = false;
	}
	public void DoPassTeammateB() {
		if(!GameController.Get.IsShooting)
			GameController.Get.DoPass(2);
		passObject.SetActive(false);
		drawLine.IsShow = false;
	}
	public void DoPassNone() {
		passObject.SetActive(false);
		drawLine.IsShow = false;
	}

	public void ContinueGame() {
		Time.timeScale = 1;
		Restart.SetActive(false);
		Continue.SetActive(false);
		Joystick.gameObject.SetActive(true);
	}

	public void PauseGame(){
		Time.timeScale = 0;
		Restart.SetActive(true);
		Continue.SetActive(true);
		Joystick.gameObject.SetActive(false);
	}

	public void ResetGame() {
		GameController.Get.Reset ();
		initLine();
		InitData ();
		Again.SetActive (false);
		isShowScoreBar = false;
		ScoreBar.SetActive(true);
		Start.SetActive (true);
		Joystick.gameObject.SetActive(true);
	}

	public void StartGame() {
		Start.SetActive (false);
		ScoreBar.SetActive (false);
		Joystick.gameObject.SetActive(true);
		GameController.Get.SetPlayerLevel();

		SceneMgr.Get.RealBall.transform.localPosition = new Vector3 (0, 5, 0);
		SceneMgr.Get.RealBall.GetComponent<Rigidbody>().isKinematic = false;
		SceneMgr.Get.RealBall.GetComponent<Rigidbody>().useGravity = true;
	}

	public void RestartGame(){
		Time.timeScale = 1;
		GameController.Get.Reset ();
		initLine();
		InitData ();
		isShowScoreBar = false;
		ScoreBar.SetActive(true);
		Restart.SetActive(false);
		Continue.SetActive(false);
		Start.SetActive (true);
		Joystick.gameObject.SetActive(false);
	}

	public void ChangeControl(bool IsAttack) {
		ControlButtonGroup [0].SetActive (IsAttack);
		ControlButtonGroup [1].SetActive (!IsAttack);
		if(!IsAttack)
			drawLine.IsShow = false;
	}

	public void PlusScore(int team, int score) {
		Scores [team] += score;
		if(Scores [team] < MaxScores[team])
			showScoreBar();
		TweenRotation tweenRotation = TweenRotation.Begin(scoresLabel[team].gameObject, 0.5f, Quaternion.identity);
		tweenRotation.delay = 0.5f;
		tweenRotation.to = new Vector3(0,720,0);
		scoresLabel[team].text = Scores [team].ToString ();
	}

	private void showScoreBar(){
		showScoreBarTime = showScoreBarInitTime;
		isShowScoreBar = true;
		ScoreBar.SetActive(true);
	}

	private void judgePlayerScreenPosition(){
		float playerInCameraX = CameraMgr.Get.CourtCamera.WorldToScreenPoint(GameController.Get.Joysticker.gameObject.transform.position).x;
		float playerInCameraY = CameraMgr.Get.CourtCamera.WorldToScreenPoint(GameController.Get.Joysticker.gameObject.transform.position).y;

		float playerInBoardX = GameController.Get.Joysticker.gameObject.transform.position.z;
		float playerInBoardY = GameController.Get.Joysticker.gameObject.transform.position.x;

		float baseValueX = 56.47f; 
		float baseValueY = 24.55f;
		
		float playerX = 15 - playerInBoardX;
		float playerY = 11 - playerInBoardY;

		Vector2 playerScreenPos = new Vector2((playerX * baseValueX) - 960 , (playerY * baseValueY) * (-1));
		if(playerScreenPos.y > -510 && playerScreenPos.y < 510 && playerInCameraX < 0) {
			playerScreenPos.x = -930;
		} else 
		if(playerScreenPos.y > -510 && playerScreenPos.y < 510 && playerInCameraX >= 0) {
			playerScreenPos.x = 930;
		} else 
		if(playerScreenPos.x > 930) {
			playerScreenPos.x = 930;
		} else 
		if(playerScreenPos.x < -930){
			playerScreenPos.x = -930;
		}

		if(playerScreenPos.x > -930 && playerScreenPos.x < 930 && playerInCameraY < 0) {
			playerScreenPos.y = -510;
		} else 
		if(playerScreenPos.y < -510){
			playerScreenPos.y = -510;
		}

		float angle = 0f;

		if(playerScreenPos.x == -930 && playerScreenPos.y == -510) {
			angle = -135;
		} else 
		if(playerScreenPos.x == 930 && playerScreenPos.y == -510) {
			angle = -45;
		} else 
		if(playerScreenPos.x == 930) {
			angle = 0;
		} else 
		if(playerScreenPos.x == -930) {
			angle = 180;
		} else
		if(playerScreenPos.y == -510) {
			angle = -90;
		}

		if(playerInCameraX > -50 &&
		   playerInCameraX < Screen.width + 50 &&
		   playerInCameraY > - 100 &&
		   playerInCameraY < Screen.height + 100) {
		    screenLocation.SetActive(false);
		} else {
			screenLocation.SetActive(true);
			screenLocation.transform.localPosition = new Vector3(playerScreenPos.x, playerScreenPos.y, 0);
			screenLocation.transform.localEulerAngles = new Vector3(0, 0, angle);
		}
	}

	protected override void InitData() {
		MaxScores[0] = 13;
		MaxScores[1] = 13;
		Scores [0] = 0;
		Scores [1] = 0;
		scoresLabel[0].text = "0";
		scoresLabel[1].text = "0";
	}

	void FixedUpdate()
	{
		if (Input.GetMouseButtonUp(0)) {
			isPressShootBtn = false;
			shootBtnTime = ButtonBTime;
		}

		if (isPressShootBtn && shootBtnTime > 0) {
			shootBtnTime -= Time.deltaTime;
			if(shootBtnTime <= 0)
				GameController.Get.DoShoot(true);
		}

		if(isShowScoreBar && showScoreBarTime > 0) {
			showScoreBarTime -= Time.deltaTime;
			if(showScoreBarTime <= 0){
				isShowScoreBar = false;
				ScoreBar.SetActive(false);
			}
		}

		judgePlayerScreenPosition();
	}
}
