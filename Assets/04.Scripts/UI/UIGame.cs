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
	private DrawLine drawLine;
	private MovingJoystick Move = new MovingJoystick();
	
	private GameObject[] ControlButtonGroup= new GameObject[2];
	private GameObject passObject;
	private GameObject[] passObjectGroup = new GameObject[2];
	private UILabel[] scoresLabel = new UILabel[2];
	private UISprite[] homeHintSprite = new UISprite[3];
	private UILabel[] homeHintLabel = new UILabel[3];
	private string[] aryHomeHintString = new string[3];
	private float shootBtnTime = 0;
	private bool shootBtnIsPress = false;
	
	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if(instance)
			instance.Show(isShow);
		else
		if(isShow)
			Get.Show(isShow);
	}
	
	public static UIGame Get
	{
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

		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonShoot")).onPress = DoShoot;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonPass")).onPress = DoPassChoose;

//		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonPass")).onDragEnd = DoPassChooseEnd;
//		SetBtnFun (UIName + "/BottomRight/Attack/ButtonPass", GameController.Get.DoPass());

		SetBtnFun (UIName + "/BottomRight/Attack/ButtonShoot", GameController.Get.DoSkill);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonSteal", GameController.Get.DoSteal);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonBlock", GameController.Get.DoBlock);
//		SetBtnFun (UIName + "/BottomRight/Defance/ButtonC", GameController.Get.DoSkill);
		SetBtnFun (UIName + "/Center/ButtonAgain", ResetGame);
		SetBtnFun (UIName + "/Center/ButtonStart", StartGame);
		SetBtnFun (UIName + "/Center/ButtonContinue", ContinueGame);
		Again.SetActive (false);
		Continue.SetActive(false);	

//		drawLine = gameObject.AddComponent(Types.GetType("DrawLine"));
		drawLine = gameObject.AddComponent<DrawLine>();
		drawLine.UIs[0] = null;
		drawLine.UIs[1] = passObjectGroup[0];
		drawLine.UIs[2] = passObjectGroup[1];
//		passObject.SetActive(false);

		for(int i=0; i<homeHintSprite.Length; i++) {
			homeHintSprite[i].enabled = false;
		}
		for(int i=0; i<homeHintLabel.Length; i++) {
			homeHintLabel[i].enabled = false;
		}
	}

	public void DoShoot(GameObject go, bool state)
	{
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
	public void DoPassChoose (GameObject go, bool state){
		if(state){
			Debug.Log("Start:"+Input.mousePosition);
		} else {
			Debug.Log("End:"+Input.mousePosition);
		}
		passObject.SetActive(state);
		drawLine.IsShow = state;
	}

	public void DoPassChooseEnd(GameObject go, bool state) {
		Debug.Log("name:"+go.name);
	}

	public void ContinueGame() {
		Debug.Log("Continue Game");
	}

	public void PauseGame(){
		Debug.Log("Pause Game");
	}

	public void ResetGame(){
		GameController.Get.Reset ();
		InitData ();
		Again.SetActive (false);
	}

	public void StartGame(){
		Start.SetActive (false);

		SceneMgr.Get.RealBall.transform.localPosition = new Vector3 (0, 5, 0);
		SceneMgr.Get.RealBall.GetComponent<Rigidbody>().isKinematic = false;
		SceneMgr.Get.RealBall.GetComponent<Rigidbody>().useGravity = true;
	}

	public void ChangeControl(bool IsAttack)
	{
		ControlButtonGroup [0].SetActive (IsAttack);
		ControlButtonGroup [1].SetActive (!IsAttack);
		if(!IsAttack)
			drawLine.IsShow = false;
	}

	public void PlusScore(int team, int score)
	{
		Scores [team] += score;
		scoresLabel[team].text = Scores [team].ToString ();
	}

	private void addHomeHint(string homeHint){
		aryHomeHintString[2] = aryHomeHintString[1];
		aryHomeHintString[1] = aryHomeHintString[0];
		aryHomeHintString[0] = homeHint;
		for (int i=0; i< aryHomeHintString.Length; i++) {
			if(string.IsNullOrEmpty(aryHomeHintString[i])){
				homeHintLabel[i].enabled = false;
				homeHintSprite[i].enabled = false;
			} else {
				homeHintLabel[i].enabled = true;
				homeHintSprite[i].enabled = true;
				homeHintLabel[i].text = aryHomeHintString[i];
			}
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

	void Update()
	{
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
