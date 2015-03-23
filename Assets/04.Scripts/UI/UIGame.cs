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
	public GameObject Start;
	public GameJoystick Joystick = null;
	private MovingJoystick Move = new MovingJoystick();

	private GameObject[] ControlButtonGroup= new GameObject[2];
	private UILabel[] scoresLabel = new UILabel[2];
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
		Again = GameObject.Find (UIName + "/Again");
		Start = GameObject.Find (UIName + "/Start");
		//scoresLabel[0] = GameObject.Find (UIName + "/ScoreBar/Score1").GetComponent<UILabel>();
		//scoresLabel[1] = GameObject.Find (UIName + "/ScoreBar/Score2").GetComponent<UILabel>();

		ControlButtonGroup [0] = GameObject.Find (UIName + "/Attack");
		ControlButtonGroup [1] = GameObject.Find (UIName + "/Defance");

		//UIEventListener.Get (GameObject.Find (UIName + "Attack/ButtonB")).onPress = DoShoot;

		SetBtnFun (UIName + "Attack/ButtonA", GameController.Get.DoPass);
		SetBtnFun (UIName + "Attack/ButtonC", GameController.Get.DoSkill);
		SetBtnFun (UIName + "Defance/ButtonA", GameController.Get.DoSteal);
		SetBtnFun (UIName + "Defance/ButtonB", GameController.Get.DoBlock);
		SetBtnFun (UIName + "Defance/ButtonC", GameController.Get.DoSkill);
		SetBtnFun (UIName + "/Again/AgainBt", ResetGame);
		SetBtnFun (UIName + "/Start/StartBt", StartGame);
		//Again.SetActive (false);
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
	}

	public void PlusScore(int team, int score)
	{
		Scores [team] += score;
		scoresLabel[team].text = Scores [team].ToString ();
	}
	
	protected override void InitData() {
		MaxScores[0] = 13;
		MaxScores[1] = 13;
		Scores [0] = 0;
		Scores [1] = 0;
		//scoresLabel[0].text = "0";
		//scoresLabel[1].text = "0";
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
