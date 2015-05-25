using UnityEngine;
using System.Collections;

public class UIGame : UIBase {
	private static UIGame instance = null;
	private const string UIName = "UIGame";

	//Game const
	public float ButtonBTime = 0.14f; //Fake to shoot time
	private float showScoreBarInitTime = 2;
	public int[] MaxScores = {13, 13};

	private float shootBtnTime = 0;
	private float showScoreBarTime = 0;
	public int[] Scores = {0, 0};

	private bool isAttackState = true;
	private bool isPressElbowBtn = true;
	private bool isCanDefenceBtnPress = true;
	private bool isPressShootBtn = false;
	private bool isShowScoreBar = false;
	private bool isShootAvailable = true;

	//Stuff
	public GameObject Again;
	public GameObject Continue;
	public GameObject ScoreBar;
	public GameJoystick Joystick = null;
	private GameObject start;
	private GameObject restart;
	private GameObject mainMenu;
	private DrawLine drawLine;
	private MovingJoystick Move = new MovingJoystick();

	private GameObject[] attackGroup = new GameObject[2];
	private GameObject[] defenceGroup = new GameObject[2];
	private GameObject attackPush;
//	private GameObject[] coverAttack = new GameObject[3];
//	private GameObject[] coverDefence = new GameObject[3];
	private GameObject[] ControlButtonGroup= new GameObject[2];
	private GameObject passObject;
	private GameObject[] passObjectGroup = new GameObject[2];
	private GameObject screenLocation;
	private UILabel[] scoresLabel = new UILabel[2];
	private UIScrollBar[] aiLevelScrollBar = new UIScrollBar[3];
	private string[] aryHomeHintString = new string[3];

	//FX
	private float fxTime = 0.3f;
	private GameObject buttonPassFX;
	private float buttonPassFXTime;
	private GameObject buttonShootFX;
	private float buttonShootFXTime;
	private GameObject buttonObjectAFX;
	private float buttonObjectAFXTime;
	private GameObject buttonObjectBFX;
	private float buttonObjectBFXTime;
	private GameObject buttonBlockFX;
	private float buttonBlockFXTime;
	private GameObject buttonStealFX;
	private float buttonStealFXTime;
	private GameObject buttonAttackFX;
	private float buttonAttackFXTime;

	public static UIGame Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGame;
			
            return instance;
        }
    }
    
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

	void FixedUpdate()
	{
		if (Input.GetMouseButtonUp(0)) 
			isPressShootBtn = false;

		if (isPressShootBtn && shootBtnTime > 0) {
			shootBtnTime -= Time.deltaTime;
			if(shootBtnTime <= 0){
				isPressShootBtn = false;
				if (GameController.Get.BallOwner == GameController.Get.Joysticker) {
					GameController.Get.DoShoot(true);
					GameController.Get.Joysticker.SetNoAiTime();
//					showCoverAttack(true);
//					coverAttack[1].SetActive(false);
					attackPush.SetActive(false);
					attackGroup[0].SetActive(false);

					if(GameController.Get.IsCanPassAir) {
//						coverAttack[0].SetActive(false);
						attackGroup[0].SetActive(true);
						SetPassButton(3);
					}
				}
			}
		}
        
        if(isShowScoreBar && showScoreBarTime > 0) {
            showScoreBarTime -= Time.deltaTime;
            if(showScoreBarTime <= 0){
                isShowScoreBar = false;
                ScoreBar.SetActive(false);
            }
        }
        showButtonFX();
        judgePlayerScreenPosition();
	}

	protected override void InitCom() {
		ButtonBTime = GameStart.Get.ShootButtonTime;

		Joystick = GameObject.Find (UIName + "/GameJoystick").GetComponent<GameJoystick>();
		Joystick.Joystick = GameObject.Find (UIName + "/GameJoystick").GetComponent<EasyJoystick>();

		Again = GameObject.Find (UIName + "/Center/ButtonAgain");
		Continue = GameObject.Find (UIName + "/Center/ButtonContinue");
		start = GameObject.Find (UIName + "/Center/StartView");
		restart = GameObject.Find (UIName + "/Center/ButtonReset");
		mainMenu = GameObject.Find (UIName + "/Center/ButtonMainMenu");
		ScoreBar = GameObject.Find(UIName + "/Top/ScoreBar");
		scoresLabel [0] = GameObject.Find (UIName + "/Top/ScoreBar/LabelScore1").GetComponent<UILabel>();
		scoresLabel [1] = GameObject.Find (UIName + "/Top/ScoreBar/LabelScore2").GetComponent<UILabel>();

		ControlButtonGroup [0] = GameObject.Find (UIName + "/BottomRight/Attack");
		ControlButtonGroup [1] = GameObject.Find (UIName + "/BottomRight/Defance");

		attackGroup[0] = GameObject.Find(UIName + "/BottomRight/Attack/ButtonPass");
		attackGroup[1] = GameObject.Find(UIName + "/BottomRight/Attack/ButtonShoot");

		defenceGroup[0] = GameObject.Find(UIName + "/BottomRight/Defance/ButtonSteal");
		defenceGroup[1] = GameObject.Find(UIName + "/BottomRight/Defance/ButtonBlock");

		attackPush = GameObject.Find(UIName + "/BottomRight/ButtonAttack");

//		coverAttack[0] = GameObject.Find(UIName + "/BottomRight/Attack/CoverPass");
//		coverAttack[1] = GameObject.Find(UIName + "/BottomRight/Attack/CoverShoot");

//		coverDefence[0] = GameObject.Find(UIName + "/BottomRight/Defance/CoverBlock");
//		coverDefence[1] = GameObject.Find(UIName + "/BottomRight/Defance/CoverSteal");

		passObjectGroup [0] = GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectA");
		passObjectGroup [1] = GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectB");
		passObject = GameObject.Find (UIName + "/BottomRight/Attack/PassObject");

		aiLevelScrollBar [0] = GameObject.Find(UIName + "/Center/StartView/AISelect/HomeScrollBar").GetComponent<UIScrollBar>();
		aiLevelScrollBar [1] = GameObject.Find(UIName + "/Center/StartView/AISelect/AwayScrollBar").GetComponent<UIScrollBar>();
		aiLevelScrollBar [2] = GameObject.Find(UIName + "/Center/StartView/AISelect/AIControlScrollBar").GetComponent<UIScrollBar>();

		screenLocation = GameObject.Find (UIName + "/Right");
		screenLocation.SetActive(false);

		buttonPassFX = GameObject.Find(UIName + "/BottomRight/Attack/ButtonPass/UI_FX_A_21");
		buttonShootFX = GameObject.Find(UIName + "/BottomRight/Attack/ButtonShoot/UI_FX_A_21");
		buttonObjectAFX = GameObject.Find(UIName + "/BottomRight/Attack/PassObject/ButtonObjectA/UI_FX_A_21");
		buttonObjectBFX = GameObject.Find(UIName + "/BottomRight/Attack/PassObject/ButtonObjectB/UI_FX_A_21");
		buttonBlockFX = GameObject.Find(UIName + "/BottomRight/Defance/ButtonBlock/UI_FX_A_21");
		buttonStealFX = GameObject.Find(UIName + "/BottomRight/Defance/ButtonSteal/UI_FX_A_21");
		buttonAttackFX = GameObject.Find(UIName + "/BottomRight/ButtonAttack/UI_FX_A_21");
		buttonPassFX.SetActive(false);
		buttonShootFX.SetActive(false);
		buttonObjectAFX.SetActive(false);
		buttonObjectBFX.SetActive(false);
		buttonBlockFX.SetActive(false);
		buttonStealFX.SetActive(false);
		buttonAttackFX.SetActive(false);

		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonShoot")).onPress = DoShoot;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonPass")).onPress = DoPassChoose;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectA")).onPress = DoPassTeammateA;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectB")).onPress = DoPassTeammateB;


		aiLevelScrollBar[0].onChange.Add(new EventDelegate(changeSelfAILevel));
		aiLevelScrollBar[1].onChange.Add(new EventDelegate(changeNpcAILevel));
		aiLevelScrollBar[2].onChange.Add(new EventDelegate(changeAIChangeTime));

//		SetBtnFun (UIName + "/BottomRight/Attack/PassObject/ButtonObjectA", DoPassTeammateA);
//		SetBtnFun (UIName + "/BottomRight/Attack/PassObject/ButtonObjectB", DoPassTeammateB);
//		SetBtnFun (UIName + "/BottomRight/Attack/ButtonShoot", DoSkill);
		SetBtnFun (UIName + "/BottomRight/ButtonAttack", DoAttack);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonSteal", DoSteal);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonBlock", DoBlock);
		SetBtnFun (UIName + "/Center/ButtonAgain", ResetGame);
		SetBtnFun (UIName + "/Center/StartView/ButtonStart", StartGame);
		SetBtnFun (UIName + "/Center/ButtonContinue", ContinueGame);
		SetBtnFun (UIName + "/TopLeft/ButtonPause", PauseGame);
		SetBtnFun (UIName + "/Center/ButtonReset", RestartGame);
		SetBtnFun (UIName + "/Center/ButtonMainMenu", BackMainMenu);

		Again.SetActive (false);
		restart.SetActive(false);
		mainMenu.SetActive(false);
		Continue.SetActive(false);
		passObject.SetActive(false);
//		showCoverAttack(false);
//		showCoverDefence(false);

//		drawLine = gameObject.AddComponent<DrawLine>();
		ChangeControl(false);
		
		Joystick.gameObject.SetActive(false);
	}

	protected override void InitData() {
		MaxScores[0] = 13;
		MaxScores[1] = 13;
		Scores [0] = 0;
		Scores [1] = 0;
		scoresLabel[0].text = "0";
        scoresLabel[1].text = "0";
    }

	protected override void InitText(){
		SetLabel(UIName + "/Center/ButtonAgain/LabelReset" ,TextConst.S(1));
		SetLabel(UIName + "/Center/StartView/ButtonStart/LabelStart" ,TextConst.S(2));
		SetLabel(UIName + "/Center/ButtonContinue/LabelContinue" ,TextConst.S(3));
		SetLabel(UIName + "/Center/ButtonReset/LabelReset" ,TextConst.S(4));
		SetLabel(UIName + "/Center/StartView/AISelect/LabelAI" ,TextConst.S(5));
		SetLabel(UIName + "/Center/StartView/AISelect/AISecLabel" ,TextConst.S(6));
	}

//	private void initLine() {
//		drawLine.ClearTarget();
//		if (drawLine.UIs.Length == 0) {
//			for (int i = 0; i < 2; i ++) {
//				GameObject obj = GameObject.Find("PlayerInfoModel/Self" + (i+1).ToString());
//				if (obj)
//					drawLine.AddTarget(passObjectGroup[i], obj);
//			}
//		}
//	}

	IEnumerator WaitForVirtualScreen(){
		yield return new WaitForSeconds(1);
		
		Joystick.CheckVirtualScreen();
	}

//	private void showCoverAttack(bool isShow) {
//		for (int i=0; i<coverAttack.Length; i++) {
//			coverAttack[i].SetActive(isShow);
//		}
//	}

	private void showAttack(bool isShow) {
		for (int i=0; i<attackGroup.Length; i++) {
			attackGroup[i].SetActive(isShow);
		}
	}
	
//	private void showCoverDefence(bool isShow) {
//		for (int i=0; i<coverDefence.Length; i++) {
//			coverDefence[i].SetActive(isShow);
//		}
//	}

	private void showDefence(bool isShow) {
		for(int i=0; i<defenceGroup.Length; i++) {
			defenceGroup[i].SetActive(isShow);
		}
	}

	public void changeSelfAILevel(){
		GameConst.SelfAILevel = (int) Mathf.Round(aiLevelScrollBar[0].value * 5);
	}

	public void changeNpcAILevel(){
		GameConst.NpcAILevel = (int)  Mathf.Round(aiLevelScrollBar[1].value * 5);		
	}

	public void changeAIChangeTime(){
		int level = (int)  Mathf.Round(aiLevelScrollBar[2].value * 5);
		float time = 1;
		if(level == 0) {
			time = 1;
		} else if(level == 1) {
			time = 3;
		}else if(level == 2) {
			time = 5;
		}else if(level == 3) {
			time = 15;
		}else if(level == 4) {
			time = 30;
		}else if(level == 5) {
			time = 999999;
		}
		GameData.Setting.AIChangeTime = time;
	}
	// Effect
	public void BlockFX(){
		buttonBlockFXTime = fxTime;
		buttonBlockFX.SetActive(true);
	}

	public void StealFX(){
		buttonStealFXTime = fxTime;
		buttonStealFX.SetActive(true);
	}
	
	public void PassFX(){
		buttonPassFXTime = fxTime;
		buttonPassFX.SetActive(true);
	}
	
	public void ShootFX(){
		buttonShootFXTime = fxTime;
		buttonShootFX.SetActive(true);
	}

	public void AttackFX(){
		buttonAttackFXTime = fxTime;
		buttonAttackFX.SetActive(true);
	}

	
	public void DoAttack(){
		if(GameController.Get.Joysticker.IsBallOwner) {
			//Elbow
			if(isPressElbowBtn && 
			   !GameController.Get.Joysticker.IsFall && 
			   GameController.Get.situation == GameSituation.AttackA) {
//				coverDefence[2].SetActive(false);
//				coverAttack[2].SetActive(false);
//				showCoverAttack(true);
				showAttack(false);
				
				AttackFX();
				GameController.Get.DoElbow ();
				GameController.Get.Joysticker.SetNoAiTime();
			}
		} else {
			//Push
			if(isCanDefenceBtnPress && 
			   !GameController.Get.Joysticker.IsFall &&
			   (GameController.Get.situation == GameSituation.AttackB || GameController.Get.situation == GameSituation.AttackA)
			   ) {
//				showCoverDefence(true);
//				showCoverAttack(true);
//				coverAttack[2].SetActive(false);
//				coverDefence[2].SetActive(false);
				showDefence(false);
				
				AttackFX();
				GameController.Get.DoPush();
				GameController.Get.Joysticker.SetNoAiTime();
			}
		}
		
	}

	//Defence
	public void DoBlock() {
		if(isCanDefenceBtnPress && 
		   !GameController.Get.Joysticker.IsFall && 
		   GameController.Get.situation == GameSituation.AttackB
		   ) {
//			showCoverDefence(true);
//			coverDefence[0].SetActive(false);
//			showDefence(false);
			defenceGroup[0].SetActive(false);
			attackPush.SetActive(false);
//			defenceGroup[1].SetActive(true);

			BlockFX();
			GameController.Get.DoBlock();
			GameController.Get.Joysticker.SetNoAiTime();
		}
	}

	public void DoSteal(){
		if(isCanDefenceBtnPress && 
		   !GameController.Get.Joysticker.IsFall &&
		   GameController.Get.situation == GameSituation.AttackB && 
		   GameController.Get.StealBtnLiftTime <= 0
		   ) {
//			showCoverDefence(true);
//			coverDefence[1].SetActive(false);
//			showDefence(false);
			defenceGroup[1].SetActive(false);
			attackPush.SetActive(false);
//			defenceGroup[0].SetActive(true);

			StealFX();
			GameController.Get.DoSteal();
			GameController.Get.Joysticker.SetNoAiTime();
		}
	}

//	public void DoPush(){
//		if(isCanDefenceBtnPress && 
//		   !GameController.Get.Joysticker.IsFall &&
//		   (GameController.Get.situation == GameSituation.AttackB || GameController.Get.situation == GameSituation.AttackA)
//		   ) {
////			showCoverDefence(true);
////			showCoverAttack(true);
////			coverAttack[2].SetActive(false);
////			coverDefence[2].SetActive(false);
//
//
//			PushFX();
//			GameController.Get.DoPush();
//			GameController.Get.Joysticker.SetNoAiTime();
//		}
//	}
	
	//Attack
	public void DoSkill(){
		if(!GameController.Get.Joysticker.IsFall) {
			GameController.Get.DoSkill();
			GameController.Get.Joysticker.SetNoAiTime();
		}
	}
	
	public void DoShoot(GameObject go, bool state) {
		if(GameController.Get.IsShooting) {
			if(state && UIDoubleClick.Visible){
				UIDoubleClick.Get.ClickStop ();
			}
		} else {
			if(GameController.Get.Joysticker.IsBallOwner &&
				GameController.Get.situation == GameSituation.AttackA && 
			   !GameController.Get.Joysticker.IsFall && 
			   !GameController.Get.Joysticker.CheckAnimatorSate(PlayerState.MoveDodge0) && 
			   !GameController.Get.Joysticker.CheckAnimatorSate(PlayerState.MoveDodge1) && 
			   !GameController.Get.Joysticker.CheckAnimatorSate(PlayerState.Block)
			   ) {
				if(state && GameController.Get.Joysticker.IsFakeShoot && isShootAvailable) {
					isShootAvailable = false;
				}
				if(state){
					ShootFX();
				}else 
				if(!state && shootBtnTime > 0 && isShootAvailable){
					if(GameController.Get.BallOwner != null) {
						if(GameController.Get.Joysticker.IsBallOwner) {
//							showCoverAttack(true);
//							coverAttack[1].SetActive(false);
//							showAttack(false);
							attackPush.SetActive(false);
							attackGroup[0].SetActive(false);
						}
						shootBtnTime = ButtonBTime;
					}
					GameController.Get.DoShoot (false);
					GameController.Get.Joysticker.SetNoAiTime();
				}else
				if(!state && !isShootAvailable) {
					isShootAvailable = true;
				}
				
				isPressShootBtn = state;
			} else 
			if(!GameController.Get.Joysticker.IsBallOwner)
				GameController.Get.DoShoot(true);
		}
	}


	public void DoPassChoose (GameObject obj, bool state) {
		if(GameController.Get.Joysticker.IsBallOwner && 
		   !GameController.Get.Joysticker.IsFall && 
		   GameController.Get.situation == GameSituation.AttackA) {
			if(!GameController.Get.IsCanPassAir){
				passObject.SetActive(state);
				if (state) 
					SetPassButton(3);
			}
//			initLine();
//			drawLine.IsShow = state;
		} else {
			GameController.Get.DoPass(0);
			GameController.Get.Joysticker.SetNoAiTime();
		}
	}

	public void DoPassTeammateA(GameObject obj, bool state) {
//		showCoverAttack(true);
//		coverAttack[0].SetActive(false);
		PassFX();
		buttonObjectAFXTime = fxTime;
		buttonObjectAFX.SetActive(true);
		passObject.SetActive(false);
		attackGroup[1].SetActive(false);
		attackPush.SetActive(false);
		
		if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(1))
			GameController.Get.Joysticker.SetNoAiTime();
		else{
			showAttack(true);
//			attackPush.SetActive(true);
		}
//			showCoverAttack(false);

//		drawLine.IsShow = false;
	}

	public void DoPassTeammateB(GameObject obj, bool state) {
//		showCoverAttack(true);
//		coverAttack[0].SetActive(false);

		PassFX();
		buttonObjectBFXTime = fxTime;
		buttonObjectBFX.SetActive(true);
		passObject.SetActive(false);
		attackGroup[1].SetActive(false);
		attackPush.SetActive(false);
		if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(2))
			GameController.Get.Joysticker.SetNoAiTime();
		else {
			showAttack(true);
//			attackPush.SetActive(true);
		}
//			showCoverAttack(false);
	}

	public void DoPassNone() {
		passObject.SetActive(false);
//		drawLine.IsShow = false;
	}

	public void BackMainMenu() {
		Time.timeScale = 1;
		SceneMgr.Get.ChangeLevel(SceneName.Lobby);
	}

	public void ContinueGame() {
		Time.timeScale = 1;
		restart.SetActive(false);
		mainMenu.SetActive(false);
		Continue.SetActive(false);
		ScoreBar.SetActive(false);
		Joystick.gameObject.SetActive(true);
	}

	public void PauseGame(){
		Time.timeScale = 0;
		restart.SetActive(true);
		mainMenu.SetActive(true);
		Continue.SetActive(true);
		ScoreBar.SetActive(true);
		Joystick.gameObject.SetActive(false);
	}

	public void ResetGame() {
		GameController.Get.Reset ();
//		initLine();
		InitData ();
		Again.SetActive (false);
		isShowScoreBar = false;
		ScoreBar.SetActive(true);
		start.SetActive (true);
		Joystick.gameObject.SetActive(false);
	}

	public void StartGame() {
		start.SetActive (false);
		ScoreBar.SetActive (false);
		Joystick.gameObject.SetActive(true);

		CourtMgr.Get.SetBallState (PlayerState.Start);
		GameController.Get.StartGame();

		StartCoroutine("WaitForVirtualScreen");
	}

	public void RestartGame(){
		ResetGame();
		Time.timeScale = 1;
		restart.SetActive(false);
		mainMenu.SetActive(false);
		Continue.SetActive(false);
	}

	public void PlusScore(int team, int score) {
		Scores [team] += score;
		CourtMgr.Get.SetScoreboards (team, score);
		if(Scores [team] < MaxScores[team])
			showScoreBar();
		TweenRotation tweenRotation = TweenRotation.Begin(scoresLabel[team].gameObject, 0.5f, Quaternion.identity);
		tweenRotation.delay = 0.5f;
		tweenRotation.to = new Vector3(0,720,0);
		scoresLabel[team].text = Scores [team].ToString ();
	}

	public void SetPassButton(int kind) {
		switch (kind) {
		case 1:
			passObject.SetActive(true);
			passObjectGroup[0].SetActive(true);
			passObjectGroup[1].SetActive(false);
			break;
		case 2:
			passObject.SetActive(true);
			passObjectGroup[0].SetActive(false);
			passObjectGroup[1].SetActive(true);
            break;
		case 3:
			passObject.SetActive(true);
			passObjectGroup[0].SetActive(true);
			passObjectGroup[1].SetActive(true);
			break;
		default:
			passObject.SetActive(false);
			passObjectGroup[0].SetActive(true);
			passObjectGroup[1].SetActive(true);
			break;
        }
	}

	private void showScoreBar(){
		showScoreBarTime = showScoreBarInitTime;
		isShowScoreBar = true;
		ScoreBar.SetActive(true);
	}

	private void judgePlayerScreenPosition(){
		if(GameController.Get.IsStart && GameController.Get.Joysticker != null){
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
				   playerInCameraX < Screen.width + 100 &&
				   playerInCameraY > -50 &&
				   playerInCameraY < Screen.height + 100) {
					screenLocation.SetActive(false);
				} else {
					screenLocation.SetActive(true);
					screenLocation.transform.localPosition = new Vector3(playerScreenPos.x, playerScreenPos.y, 0);
					screenLocation.transform.localEulerAngles = new Vector3(0, 0, angle);
				}

		}
	}

	public void ChangeControl(bool IsAttack) {
		shootBtnTime = ButtonBTime;
		ControlButtonGroup[0].SetActive(IsAttack);
		showAttack(IsAttack);
		ControlButtonGroup[1].SetActive(!IsAttack);
		showDefence(!IsAttack);
		attackPush.SetActive(true);
		isAttackState = IsAttack;
		
//		if(!IsAttack)
//			drawLine.IsShow = false;
	}

	public bool OpenUIMask(PlayerBehaviour p = null){
		if(p == GameController.Get.Joysticker) {
			if(p.NoAiTime > 0)
				p.SetNoAiTime();

			shootBtnTime = ButtonBTime;
			ControlButtonGroup[0].SetActive(isAttackState);
			showAttack(isAttackState);
			ControlButtonGroup[1].SetActive(!isAttackState);
			showDefence(!isAttackState);
			attackPush.SetActive(true);

			isCanDefenceBtnPress = true;
			isPressElbowBtn = true;

			return true;
		} else {
			if (p.Team == GameController.Get.Joysticker.Team && p.crtState == PlayerState.Alleyoop)
				SetPassButton(0);

			return false;
		}
	}

	private void showButtonFX(){
		if(buttonPassFXTime > 0) {
			buttonPassFXTime -= Time.deltaTime;
			if(buttonPassFXTime <= 0) {
				buttonPassFXTime = 0;
				buttonPassFX.SetActive(false);
			}
		}

		if(buttonShootFXTime > 0) {
			buttonShootFXTime -= Time.deltaTime;
			if(buttonShootFXTime <= 0) {
				buttonShootFXTime = 0;
				buttonShootFX.SetActive(false);
			}
		}
		
		if(buttonObjectAFXTime > 0) {
			buttonObjectAFXTime -= Time.deltaTime;
			if(buttonObjectAFXTime <= 0) {
				buttonObjectAFXTime = 0;
				buttonObjectAFX.SetActive(false);
			}
		}
		
		if(buttonObjectBFXTime > 0) {
			buttonObjectBFXTime -= Time.deltaTime;
			if(buttonObjectBFXTime <= 0) {
				buttonObjectBFXTime = 0;
				buttonObjectBFX.SetActive(false);
			}
		}
		
		if(buttonBlockFXTime > 0) {
			buttonBlockFXTime -= Time.deltaTime;
			if(buttonBlockFXTime <= 0) {
				buttonBlockFXTime = 0;
				buttonBlockFX.SetActive(false);
			}
		}
		
		if(buttonStealFXTime > 0) {
			buttonStealFXTime -= Time.deltaTime;
			if(buttonStealFXTime <= 0) {
				buttonStealFXTime = 0;
				buttonStealFX.SetActive(false);
			}
		}
		
		if(buttonAttackFXTime > 0) {
			buttonAttackFXTime -= Time.deltaTime;
			if(buttonAttackFXTime <= 0) {
				buttonAttackFXTime = 0;
				buttonAttackFX.SetActive(false);
			}
		}

	}
}
