using UnityEngine;
using System.Collections;

public class UIGame : UIBase {
	private static UIGame instance = null;
	private const string UIName = "UIGame";

	private float ButtonBTime = 0.09f;
	private float shootBtnTime = 0;

	public int[] MaxScores = {13, 13};
	public int[] Scores = {0, 0};
	private bool IsUseKeyboard = false;
	public GameObject Again;
	public GameObject Continue;
	public GameObject Start;
	public GameJoystick Joystick = null;
	private DrawLine drawLine;
	private MovingJoystick Move = new MovingJoystick();
	
	private GameObject[] ControlButtonGroup= new GameObject[2];
	private GameObject passObject;
	private GameObject[] passObjectGroup = new GameObject[2];
	private GameObject screenLocation;
	private UILabel[] scoresLabel = new UILabel[2];
	private UISprite[] homeHintSprite = new UISprite[3];
	private UILabel[] homeHintLabel = new UILabel[3];
	private string[] aryHomeHintString = new string[3];
	private float homeHintTime = -1;
	
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
		Start = GameObject.Find (UIName + "/Center/ButtonStart");
		scoresLabel [0] = GameObject.Find (UIName + "/Top/ScoreBar/LabelScore1").GetComponent<UILabel>();
		scoresLabel [1] = GameObject.Find (UIName + "/Top/ScoreBar/LabelScore2").GetComponent<UILabel>();
		homeHintSprite [0] = GameObject.Find (UIName + "/Top/HomeHint/SpriteBottom0").GetComponent<UISprite>();
		homeHintSprite [1] = GameObject.Find (UIName + "/Top/HomeHint/SpriteBottom1").GetComponent<UISprite>();
		homeHintSprite [2] = GameObject.Find (UIName + "/Top/HomeHint/SpriteBottom2").GetComponent<UISprite>();
		homeHintLabel [0] = GameObject.Find (UIName + "/Top/HomeHint/LabelHint0").GetComponent<UILabel>();
		homeHintLabel [1] = GameObject.Find (UIName + "/Top/HomeHint/LabelHint1").GetComponent<UILabel>();
		homeHintLabel [2] = GameObject.Find (UIName + "/Top/HomeHint/LabelHint2").GetComponent<UILabel>();

		ControlButtonGroup [0] = GameObject.Find (UIName + "/BottomRight/Attack");
		ControlButtonGroup [1] = GameObject.Find (UIName + "/BottomRight/Defance");

		passObjectGroup [0] = GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectA");
		passObjectGroup [1] = GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectB");
		passObject = GameObject.Find (UIName + "/BottomRight/Attack/PassObject");

		screenLocation = GameObject.Find (UIName + "/Right");

		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonShoot")).onPress = DoShoot;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonPass")).onPress = DoPassChoose;
//		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonPass")).onPress = DoPassChoose;


//		SetBtnFun (UIName + "/BottomRight/Attack/ButtonPass", DoPassChoose);
		SetBtnFun (UIName + "/BottomRight/Attack/PassObject/ButtonObjectA", DoPassTeammateA);
		SetBtnFun (UIName + "/BottomRight/Attack/PassObject/ButtonObjectB", DoPassTeammateB);
		SetBtnFun (UIName + "/BottomRight/Attack/ButtonShoot", GameController.Get.DoSkill);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonSteal", GameController.Get.DoSteal);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonBlock", GameController.Get.DoBlock);
		SetBtnFun (UIName + "/Center/ButtonAgain", ResetGame);
		SetBtnFun (UIName + "/Center/ButtonStart", StartGame);
		SetBtnFun (UIName + "/Center/ButtonContinue", ContinueGame);
		SetBtnFun (UIName + "/TopLeft/ButtonPause", PauseGame);
		Again.SetActive (false);
		Continue.SetActive(false);
		passObject.SetActive(false);

		drawLine = gameObject.AddComponent<DrawLine>();
		ChangeControl(false);

<<<<<<< HEAD
		for(int i=0; i<homeHintSprite.Length; i++) {
=======
		for(int i=0; i<homeHintSprite.Length; i++) 
>>>>>>> origin/master
			homeHintSprite[i].enabled = false;

		for(int i=0; i<homeHintLabel.Length; i++) 
			homeHintLabel[i].enabled = false;
	}

	private void initLine() {
		if (drawLine.UIs.Length == 0) {
			for (int i = 0; i < 2; i ++) {
				GameObject obj = GameObject.Find("PlayerInfoModel/" + (i+1).ToString());
				if (obj)
					drawLine.AddTarget(passObjectGroup[i], obj);
			}
		}
	}

	private bool isPressShootBtn = false;

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
		Debug.Log("isballowner:"+GameController.Get.Joysticker.IsBallOwner); 
		if(GameController.Get.Joysticker.IsBallOwner) {
			initLine();
			passObject.SetActive(state);
			drawLine.IsShow = state;
		} else {
			GameController.Get.DoPass(0);
		}
	}

	public void DoPassTeammateA() {
		GameController.Get.DoPass(1);
	}
	public void DoPassTeammateB() {
		GameController.Get.DoPass(2);
	}

	public void ContinueGame() {
		Debug.Log("Continue Game");
		Time.timeScale = 1;
		Continue.SetActive(false);
		Joystick.enabled = true;
	}

	public void PauseGame(){
		Time.timeScale = 0;
		Continue.SetActive(true);
		Joystick.enabled = false;
	}

	public void ResetGame() {
		GameController.Get.Reset ();
		InitData ();
		Again.SetActive (false);
	}

	public void StartGame() {
		Start.SetActive (false);

		SceneMgr.Get.RealBall.transform.localPosition = new Vector3 (0, 5, 0);
		SceneMgr.Get.RealBall.GetComponent<Rigidbody>().isKinematic = false;
		SceneMgr.Get.RealBall.GetComponent<Rigidbody>().useGravity = true;
	}

	public void ChangeControl(bool IsAttack) {
		ControlButtonGroup [0].SetActive (IsAttack);
		ControlButtonGroup [1].SetActive (!IsAttack);
		if(!IsAttack)
			drawLine.IsShow = false;
	}

	public void PlusScore(int team, int score) {
//		if(team == 0) { 
//			if(score == 3)
//				UIGame.Get.SetHomeHint(true, TextConst.S(1002));
//		}
		Scores [team] += score;
		TweenRotation.Begin(scoresLabel[team].gameObject, 0.5f, Quaternion.identity).to = new Vector3(0,720,0);
		scoresLabel[team].text = Scores [team].ToString ();
	}

	public void SetHomeHint(bool isShow, string homeHint = "") {
		homeHintLabel[0].text = homeHint;
		if(isShow) {
			homeHintTime = 3;
			showHomeHint();
			homeHintLabel[0].enabled = true;
			homeHintSprite[0].enabled = true;
		} else {
			homeHintLabel[0].enabled = false;
			homeHintSprite[0].enabled = false;
		}

//		aryHomeHintString[2] = aryHomeHintString[1];
//		aryHomeHintString[1] = aryHomeHintString[0];
//		aryHomeHintString[0] = homeHint;
//		for (int i=0; i< aryHomeHintString.Length; i++) {
//			if(string.IsNullOrEmpty(aryHomeHintString[i])){
//				homeHintLabel[i].enabled = false;
//				homeHintSprite[i].enabled = false;
//			} else {
//				homeHintLabel[i].enabled = true;
//				homeHintSprite[i].enabled = true;
//				homeHintLabel[i].text = aryHomeHintString[i];
//			}
//		}
	}

	private void showHomeHint() {
		if(homeHintTime > 0) {
			homeHintTime -= Time.deltaTime;
			if(homeHintTime <=0 ) {
				SetHomeHint(false);
			}
		}
	}

	private void judgePlayerScreenPosition(){
		float playerX = CameraMgr.Get.CourtCamera.WorldToScreenPoint(GameController.Get.Joysticker.gameObject.transform.position).x;
		float playerY = CameraMgr.Get.CourtCamera.WorldToScreenPoint(GameController.Get.Joysticker.gameObject.transform.position).y;
		float baseValueX = 1920 / Screen.width; 
		float baseValueY = 1080 / Screen.height;
<<<<<<< HEAD
		Vector2 playerSreenPos = new Vector2((playerX - (Screen.width/2)) * baseValueX , (playerY- (Screen.height/2)) * baseValueY);
		if(playerSreenPos.x < - 940) {
			playerSreenPos.x = - 940;   
		}
		
		if (playerSreenPos.y < -520){
			playerSreenPos.y = -520;
		}
		
		if (playerSreenPos.x > 940){
			playerSreenPos.x = 940;
		}
		
		if (playerSreenPos.y > 520){
			playerSreenPos.y = 520;
=======
		Vector2 playerScreenPos = new Vector2((playerX - (Screen.width/2)) * baseValueX , (playerY- (Screen.height/2)) * baseValueY);
		if(playerScreenPos.y > -510 && playerScreenPos.y <= 0 && playerScreenPos.x < - 930) {
			playerScreenPos.x = -930;
		} else 
		if(playerScreenPos.y > -510 && playerScreenPos.y <= 0 && playerScreenPos.x > 930) {
			playerScreenPos.x = 930;
		} else 
		if(playerScreenPos.y > 0 && playerScreenPos.y < 510 && playerScreenPos.x > 930) {
			playerScreenPos.x = 930;
		} else 
		if(playerScreenPos.y > 0 && playerScreenPos.y < 510 && playerScreenPos.x < -930) {
			playerScreenPos.x = -930;
		} else 
		if(playerScreenPos.x > 930) {
			playerScreenPos.x = 930;
		} else 
		if(playerScreenPos.x < -930){
			playerScreenPos.x = -930;
		}

		if(playerScreenPos.x > -930 && playerScreenPos.x <= 0 && playerScreenPos.y < -510) {
			playerScreenPos.y = -510;
		} else 
		if(playerScreenPos.x > -930 && playerScreenPos.x <= 0 && playerScreenPos.y > 510) {
			playerScreenPos.y = 510;
		} else 
		if(playerScreenPos.x > 0 && playerScreenPos.x < 930 && playerScreenPos.y > 510) {
			playerScreenPos.y = 510;
		} else 
		if(playerScreenPos.x > 0 && playerScreenPos.x < 930 && playerScreenPos.y < -510) {
			playerScreenPos.y = -510;
		} else 
		if(playerScreenPos.y > 510) {
			playerScreenPos.y = 510;
		} else 
		if(playerScreenPos.y < -510){
			playerScreenPos.y = -510;
>>>>>>> origin/master
		}

		Vector2 from = new Vector2(Screen.width/2, Screen.height/2);
		Vector2 to = new Vector2(playerX - Screen.width/2, playerY - Screen.height/2);
		float angle = Vector2.Angle(from, to);
		Vector3 cross = Vector3.Cross(from, to);
		if (cross.z < 0)
			angle = 360 - angle;

		if(playerX > -50 &&
		   playerX < Screen.width + 50 &&
		   playerY > - 50 &&
		   playerY < Screen.height + 50) {
<<<<<<< HEAD
			screenLocation.SetActive(false);
=======
		    screenLocation.SetActive(false);
>>>>>>> origin/master
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
	
	protected override void OnShow(bool isShow) {
		
	}

	void FixedUpdate()
	{
		if (isPressShootBtn && shootBtnTime > 0) {
			shootBtnTime -= Time.deltaTime;
			if(shootBtnTime <= 0)
				GameController.Get.DoShoot(true);
		}
	}

	void Update() {
		showHomeHint();
		judgePlayerScreenPosition();


<<<<<<< HEAD
//		if (Input.GetKey(KeyCode.W)) {
//			IsUseKeyboard = true;
//			Move.joystickAxis.y = 1;
//			Move.joystickValue.y = 10;
//		} else if (Input.GetKey (KeyCode.D)) {
//			IsUseKeyboard = true;
//			Move.joystickAxis.y = -1;
//			Move.joystickValue.y = -10;
//		} else {
//			Move.joystickAxis.y = 0;
//			Move.joystickValue.y = 0;
//		}
//		
//		if (Input.GetKey (KeyCode.A)) {
//			IsUseKeyboard = true;
//			Move.joystickAxis.x = -1;
//			Move.joystickValue.x = -10;
//		} else
//		if (Input.GetKey (KeyCode.S)) {
//			IsUseKeyboard = true;
//			Move.joystickAxis.x = 1;
//			Move.joystickValue.x = 10;
//		} else {
//			Move.joystickValue.x = 0;
//			Move.joystickAxis.x = 0;
//		}
//
//		if(IsUseKeyboard)
//			GameController.Get.OnJoystickMove(Move);
=======
		if (Input.GetKey(KeyCode.W)) {
			IsUseKeyboard = true;
			Move.joystickAxis.y = 1;
			Move.joystickValue.y = 10;
		} else 
		if (Input.GetKey (KeyCode.D)) {
			IsUseKeyboard = true;
			Move.joystickAxis.y = -1;
			Move.joystickValue.y = -10;
		} else {
			Move.joystickAxis.y = 0;
			Move.joystickValue.y = 0;
		}
		
		if (Input.GetKey (KeyCode.A)) {
			IsUseKeyboard = true;
			Move.joystickAxis.x = -1;
			Move.joystickValue.x = -10;
		} else
		if (Input.GetKey (KeyCode.S)) {
			IsUseKeyboard = true;
			Move.joystickAxis.x = 1;
			Move.joystickValue.x = 10;
		} else {
			Move.joystickValue.x = 0;
			Move.joystickAxis.x = 0;
		}

		if (Input.GetMouseButtonUp(0))
			passObject.SetActive(false);

		if(IsUseKeyboard)
			GameController.Get.OnJoystickMove(Move);
>>>>>>> origin/master

//		IsUseKeyboard = false;
	}
}
