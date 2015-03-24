using UnityEngine;
using System.Collections;

public class UIGame : UIBase {
	private static UIGame instance = null;
	private const string UIName = "UIGame";

	public float ButtonBTime = 0.003f;

	public int[] MaxScores = {13, 13};
	public int[] Scores = {0, 0};
	private bool IsUseKeyboard = false;
	public GameObject Again;
	public GameObject Continue;
	public GameObject Start;
	public GameJoystick Joystick = null;
	private GameObject myPlayer;
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
	private float shootBtnTime = 0;
	private bool shootBtnIsPress = false;
	
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
//		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonPass")).onPress = DoPassChoose;


		SetBtnFun (UIName + "/BottomRight/Attack/ButtonPass", DoPassChoose);
		SetBtnFun (UIName + "/BottomRight/Attack/ButtonShoot", GameController.Get.DoSkill);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonSteal", GameController.Get.DoSteal);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonBlock", GameController.Get.DoBlock);
		SetBtnFun (UIName + "/Center/ButtonAgain", ResetGame);
		SetBtnFun (UIName + "/Center/ButtonStart", StartGame);
		SetBtnFun (UIName + "/Center/ButtonContinue", ContinueGame);
		Again.SetActive (false);
		Continue.SetActive(false);

		drawLine = gameObject.AddComponent<DrawLine>();
		drawLine.UIs[0] = null;
		drawLine.UIs[1] = passObjectGroup[0];
		drawLine.UIs[2] = passObjectGroup[1];

		for(int i=0; i<homeHintSprite.Length; i++) {
			homeHintSprite[i].enabled = false;
		}
		for(int i=0; i<homeHintLabel.Length; i++) {
			homeHintLabel[i].enabled = false;
		}
	}

	public void SetPassObject(bool isShow){
		passObject.SetActive(isShow);
	}

	public void DoShoot(GameObject go, bool state) {
		shootBtnIsPress = state;
		if (state)
			shootBtnTime += Time.deltaTime;
		else {
			if(Time.deltaTime - shootBtnTime > ButtonBTime)
				GameController.Get.DoShoot(true);	
			else
				GameController.Get.DoShoot(false);

			shootBtnTime = 0;
		}	
	}
	public void DoPassChoose () {
		Debug.Log("DoPassChoose:"+UIButton.current.name);
		SetPassObject(true);
		drawLine.IsShow = true;
	}

	public void ContinueGame() {
		Debug.Log("Continue Game");
	}

	public void PauseGame(){
		Debug.Log("Pause Game");
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
		Debug.Log("Team:"+team);
		if(team == 0) {
			if(score == 2) 
				UIHint.Get.ShowHint(TextConst.S(1001), Color.blue);
			else 
			if(score == 3)
				UIHint.Get.ShowHint(TextConst.S(1002), Color.blue);
		}
	

		Scores [team] += score;
		TweenRotation.Begin(scoresLabel[team].gameObject, 0.5f, Quaternion.identity).to = new Vector3(0,720,0);
		scoresLabel[team].text = Scores [team].ToString ();
	}

	public void SetHomeHint(bool isShow, string homeHint = "") {
		homeHintLabel[0].text = homeHint;
		if(isShow) {
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

	public void setMyPlayer(GameObject obj){
		myPlayer = obj;
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

	void Update() {
		float playerX = CameraMgr.Get.CourtCamera.WorldToScreenPoint(myPlayer.transform.position).x;
		float playerY = CameraMgr.Get.CourtCamera.WorldToScreenPoint(myPlayer.transform.position).y;

//		playerY = Screen.height - playerY;
		Vector2 playerSreenPos = new Vector2(playerX - (Screen.width / 2), playerY - (Screen.height / 2));
		if(playerSreenPos.x < 0) {
			playerSreenPos.x = -920;   
		}

		if (playerSreenPos.y < 0){
			playerSreenPos.y = 500;
		}
		
		if (playerSreenPos.x > Screen.width){
			playerSreenPos.x = 920;
		}
		
		if (playerSreenPos.y > Screen.height){
			playerSreenPos.y = -500;
		}

		
		Vector2 sp1 = new Vector2(playerX, playerY);
		Vector2 sp2 = new Vector2(Screen.width/2, Screen.height/2);
		Vector3 cross = Vector3.Cross(sp2, sp1);
		float angle = Vector2.Angle(sp2, sp1);
		if (cross.z < 0)
			angle = 360 - angle;
		

		if(playerX < 0 ||
		   playerX > Screen.width ||
		   playerY < 0 ||
		   playerY > Screen.height) {
			screenLocation.SetActive(true);
		
			screenLocation.transform.localPosition = new Vector3(playerSreenPos.x, playerSreenPos.y, 0);
			screenLocation.transform.localEulerAngles = new Vector3(0, 0, angle);

		} else {
			screenLocation.SetActive(false);
		}

		if (shootBtnIsPress) {
			shootBtnTime += Time.deltaTime;
			if(shootBtnTime > 0.5f)
			{
				GameController.Get.DoShoot(true);
				shootBtnTime = 0;
				shootBtnIsPress = false;
			}
		}

		if (Input.GetKey(KeyCode.W)) {
			IsUseKeyboard = true;
			Move.joystickAxis.y = 1;
			Move.joystickValue.y = 10;
		} else if (Input.GetKey (KeyCode.D)) {
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

		if(IsUseKeyboard)
			GameController.Get.OnJoystickMove(Move);

		IsUseKeyboard = false;
	}
}
