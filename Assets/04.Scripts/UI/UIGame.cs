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

	private bool isPressElbowBtn = true;
	private bool isCanDefenceBtnPress = true;
	private bool isPressShootBtn = false;
	private bool isShowScoreBar = false;
	public GameObject Again;
	public GameObject Continue;
	public GameObject Start;
	public GameObject ScoreBar;
	public GameObject Restart;
	public GameJoystick Joystick = null;
	public bool OpenMask = false;
	private DrawLine drawLine;
	private MovingJoystick Move = new MovingJoystick();

	private UIButton buttonPass;
	private UIButton buttonPush;
	private UIButton buttonElbow;
	private UIButton buttonSteal;
	private GameObject[] coverAttack = new GameObject[3];
	private UISprite[] coverAttackSprite = new UISprite[3];
	private GameObject[] coverDefence = new GameObject[3];
	private UISprite[] coverDefenceSprite = new UISprite[3];
	private GameObject[] ControlButtonGroup= new GameObject[2];
	private GameObject pushObject;
	private GameObject attackObject;
	private GameObject passObject;
	private GameObject passA;
	private GameObject passB;
	private GameObject[] passObjectGroup = new GameObject[2];
	private GameObject screenLocation;
	private UILabel[] scoresLabel = new UILabel[2];
	private UIScrollBar[] aiLevelScrollBar = new UIScrollBar[2];
	private string[] aryHomeHintString = new string[3];
	private GameObject playerTexture;

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
	private GameObject buttonPushFX;
	private float buttonPushFXTime;
	private GameObject buttonAttackFX;
	private float buttonAttackFXTime;

	private bool isSplit;

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
					showCoverAttack(true);
					coverAttack[1].SetActive(false);
					//coverAttackSprite[1].color = Color.green;
				}
			}
		}
		
		if(GameController.Get.BallOwner == GameController.Get.Joysticker) {
			buttonPush.gameObject.SetActive(false);
			buttonElbow.gameObject.SetActive(true);
		} else {
			buttonPush.gameObject.SetActive(true);
			buttonElbow.gameObject.SetActive(false);
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

		coverAttack[0] = GameObject.Find(UIName + "/BottomRight/Attack/CoverPass");
		coverAttack[1] = GameObject.Find(UIName + "/BottomRight/Attack/CoverShoot");
		coverAttack[2] = GameObject.Find(UIName + "/BottomRight/CoverAttack");

		coverAttackSprite[0] = GameObject.Find(UIName + "/BottomRight/Attack/CoverPass").GetComponent<UISprite>();
		coverAttackSprite[1] = GameObject.Find(UIName + "/BottomRight/Attack/CoverShoot").GetComponent<UISprite>();
		coverAttackSprite[2] = GameObject.Find(UIName + "/BottomRight/CoverAttack").GetComponent<UISprite>();

		coverDefence[0] = GameObject.Find(UIName + "/BottomRight/Defance/CoverBlock");
		coverDefence[1] = GameObject.Find(UIName + "/BottomRight/Defance/CoverSteal");
		coverDefence[2] = GameObject.Find(UIName + "/BottomRight/CoverPush");
		
		coverDefenceSprite[0] = GameObject.Find(UIName + "/BottomRight/Defance/CoverBlock").GetComponent<UISprite>();
		coverDefenceSprite[1] = GameObject.Find(UIName + "/BottomRight/Defance/CoverSteal").GetComponent<UISprite>();
		coverDefenceSprite[2] = GameObject.Find(UIName + "/BottomRight/CoverPush").GetComponent<UISprite>();

		passObjectGroup [0] = GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectA");
		passObjectGroup [1] = GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectB");
		passObject = GameObject.Find (UIName + "/BottomRight/Attack/PassObject");
		passA = GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectA");
		passB = GameObject.Find (UIName + "/BottomRight/Attack/PassObject/ButtonObjectB");

		aiLevelScrollBar [0] = GameObject.Find(UIName + "/Center/StartView/AISelect/HomeScrollBar").GetComponent<UIScrollBar>();
		aiLevelScrollBar [1] = GameObject.Find(UIName + "/Center/StartView/AISelect/AwayScrollBar").GetComponent<UIScrollBar>();

		screenLocation = GameObject.Find (UIName + "/Right");
		screenLocation.SetActive(false);

		playerTexture = GameObject.Find (UIName + "/Top/Texture");

		buttonPass = GameObject.Find(UIName + "/BottomRight/Attack/ButtonPass").GetComponent<UIButton>();
		buttonPush = GameObject.Find(UIName + "/BottomRight/ButtonPush").GetComponent<UIButton>();
		buttonElbow = GameObject.Find(UIName + "/BottomRight/ButtonAttack").GetComponent<UIButton>();
		buttonSteal = GameObject.Find(UIName + "/BottomRight/Defance/ButtonSteal").GetComponent<UIButton>();

		pushObject = GameObject.Find(UIName + "/BottomRight/ButtonPush");
		attackObject = GameObject.Find(UIName + "/BottomRight/ButtonAttack");

		buttonPassFX = GameObject.Find(UIName + "/BottomRight/Attack/ButtonPass/UI_FX_A_21");
		buttonShootFX = GameObject.Find(UIName + "/BottomRight/Attack/ButtonShoot/UI_FX_A_21");
		buttonObjectAFX = GameObject.Find(UIName + "/BottomRight/Attack/PassObject/ButtonObjectA/UI_FX_A_21");
		buttonObjectBFX = GameObject.Find(UIName + "/BottomRight/Attack/PassObject/ButtonObjectB/UI_FX_A_21");
		buttonBlockFX = GameObject.Find(UIName + "/BottomRight/Defance/ButtonBlock/UI_FX_A_21");
		buttonStealFX = GameObject.Find(UIName + "/BottomRight/Defance/ButtonSteal/UI_FX_A_21");
		buttonPushFX = GameObject.Find(UIName + "/BottomRight/ButtonPush/UI_FX_A_21");
		buttonAttackFX = GameObject.Find(UIName + "/BottomRight/ButtonAttack/UI_FX_A_21");
		buttonPassFX.SetActive(false);
		buttonShootFX.SetActive(false);
		buttonObjectAFX.SetActive(false);
		buttonObjectBFX.SetActive(false);
		buttonBlockFX.SetActive(false);
		buttonStealFX.SetActive(false);
		buttonPushFX.SetActive(false);
		buttonAttackFX.SetActive(false);

		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonShoot")).onPress = DoShoot;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/Attack/ButtonPass")).onPress = DoPassChoose;

		aiLevelScrollBar[0].onChange.Add(new EventDelegate(changeSelfAILevel));
		aiLevelScrollBar[1].onChange.Add(new EventDelegate(changeNpcAILevel));

		SetBtnFun (UIName + "/BottomRight/Attack/PassObject/ButtonObjectA", DoPassTeammateA);
		SetBtnFun (UIName + "/BottomRight/Attack/PassObject/ButtonObjectB", DoPassTeammateB);
		SetBtnFun (UIName + "/BottomRight/Attack/ButtonShoot", DoSkill);
		SetBtnFun (UIName + "/BottomRight/ButtonAttack", DoElbow);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonSteal", DoSteal);
		SetBtnFun (UIName + "/BottomRight/ButtonPush", DoPush);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonBlock", DoBlock);
		SetBtnFun (UIName + "/BottomRight/Defance/ButtonBlock", BlockFX);
		SetBtnFun (UIName + "/Center/ButtonAgain", ResetGame);
		SetBtnFun (UIName + "/Center/StartView/ButtonStart", StartGame);
		SetBtnFun (UIName + "/Center/ButtonContinue", ContinueGame);
		SetBtnFun (UIName + "/TopLeft/ButtonPause", PauseGame);
		SetBtnFun (UIName + "/Center/ButtonReset", RestartGame);
		Again.SetActive (false);
		Restart.SetActive(false);
		Continue.SetActive(false);
		passObject.SetActive(false);
		showCoverAttack(false);
		showCoverDefence(false);

		drawLine = gameObject.AddComponent<DrawLine>();
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

	public void showCoverAttack(bool isShow) {
		OpenMask = isShow;
		for (int i=0; i<coverAttack.Length; i++) {
			if(isShow)
				coverAttack[i].SetActive(true);
			else {
				coverAttack[i].SetActive(false);
				coverAttackSprite[i].color = Color.red;
			}
		}
	}
	
	public void showCoverDefence(bool isShow) {
		OpenMask = isShow;
		for (int i=0; i<coverDefence.Length; i++) {
			if(isShow)
				coverDefence[i].SetActive(true);
			else {
				coverDefence[i].SetActive(false);
				coverDefenceSprite[i].color = Color.red;
			}
		}
	}

	public void changeSelfAILevel(){
		GameConst.SelfAILevel = (int) Mathf.Round(aiLevelScrollBar[0].value * 5);
	}

	public void changeNpcAILevel(){
		GameConst.NpcAILevel = (int)  Mathf.Round(aiLevelScrollBar[1].value * 5);		
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

	public void PushFX(){
		buttonPushFXTime = fxTime;
		buttonPushFX.SetActive(true);
	}

	public void AttackFX(){
		buttonAttackFXTime = fxTime;
		buttonAttackFX.SetActive(true);
	}

	//Defence
	public void DoBlock() {
		if(isCanDefenceBtnPress && !GameController.Get.Joysticker.IsFall) {
			showCoverDefence(true);
			coverDefence[0].SetActive(false);
//			coverDefenceSprite[0].color = Color.green;
			GameController.Get.DoBlock();
			GameController.Get.Joysticker.SetNoAiTime();
		}
	}

	public void DoSteal(){
		if(isCanDefenceBtnPress && !GameController.Get.Joysticker.IsFall && GameController.Get.StealBtnLiftTime <= 0) {
			showCoverDefence(true);
			coverDefence[1].SetActive(false);
//			coverDefenceSprite[1].color = Color.green;
			StealFX();
			GameController.Get.DoSteal();
			GameController.Get.Joysticker.SetNoAiTime();
		}
	}

	public void DoPush(){
		if(isCanDefenceBtnPress && !GameController.Get.Joysticker.IsFall) {
			showCoverDefence(true);
			showCoverAttack(true);
			coverAttack[2].SetActive(false);
			coverDefence[2].SetActive(false);
//			coverDefenceSprite[2].color = Color.green;
			PushFX();
			GameController.Get.DoPush();
			GameController.Get.Joysticker.SetNoAiTime();
		}
	}
	
	//Attack
	public void DoSkill(){
		if(!GameController.Get.Joysticker.IsFall) {
			GameController.Get.DoSkill();
			GameController.Get.Joysticker.SetNoAiTime();
		}
	}
	
	public void DoShoot(GameObject go, bool state) {
		if(GameController.Get.IsShooting) {
			if(state && GameController.Get.IsExtraScoreRate)
				GameController.Get.AddExtraScoreRate(10);
		} else {
			if(!GameController.Get.Joysticker.IsFall) {
				if(state){
					ShootFX();
				}else 
				if(!state && shootBtnTime > 0){
					if(GameController.Get.BallOwner != null) {
						if(GameController.Get.Joysticker == GameController.Get.BallOwner) {
							showCoverAttack(true);
							coverAttack[1].SetActive(false);
							//						coverAttackSprite[1].color = Color.green;
						}
						shootBtnTime = ButtonBTime;
					}
					GameController.Get.DoShoot (false);
					GameController.Get.Joysticker.SetNoAiTime();
				}
				isPressShootBtn = state;
			}
		}
	}

	public void DoElbow(){
		if(isPressElbowBtn && !GameController.Get.Joysticker.IsFall) {
			coverDefence[2].SetActive(false);
			coverAttack[2].SetActive(false);
//			coverAttackSprite[2].color = Color.green;
			showCoverAttack(true);
			AttackFX();
			GameController.Get.DoElbow ();
			GameController.Get.Joysticker.SetNoAiTime();
		}
	}

	public void DoPassChoose (GameObject obj, bool state) {
		if(GameController.Get.Joysticker.IsBallOwner && !GameController.Get.Joysticker.IsFall) {
			initLine();
			passObject.SetActive(state);
			if (state) {
				passA.SetActive(true);
				passB.SetActive(true);
			}
//			drawLine.IsShow = state;
		} else {
			if(!GameController.Get.IsShooting){
				GameController.Get.DoPass(0);
				GameController.Get.Joysticker.SetNoAiTime();
			}
		}
	}

	public void DoPassTeammateA() {
		showCoverAttack(true);
		coverAttack[0].SetActive(false);
//			coverAttackSprite[0].color = Color.green;
		PassFX();
		buttonObjectAFXTime = fxTime;
		buttonObjectAFX.SetActive(true);
		passObject.SetActive(false);
		if(!GameController.Get.IsShooting){
			GameController.Get.DoPass(1);
			GameController.Get.Joysticker.SetNoAiTime();
		}
//			drawLine.IsShow = false;
	}

	public void DoPassTeammateB() {
		showCoverAttack(true);
		coverAttack[0].SetActive(false);
//			coverAttackSprite[0].color = Color.green;
		PassFX();
		buttonObjectBFXTime = fxTime;
		buttonObjectBFX.SetActive(true);
		passObject.SetActive(false);
		if(!GameController.Get.IsShooting){
			GameController.Get.DoPass(2);
			GameController.Get.Joysticker.SetNoAiTime();
		}
	}

	public void DoPassNone() {
		passObject.SetActive(false);
//		drawLine.IsShow = false;
	}

	public void ContinueGame() {
		Time.timeScale = 1;
		Restart.SetActive(false);
		Continue.SetActive(false);
		ScoreBar.SetActive(false);
		Joystick.gameObject.SetActive(true);
	}

	public void PauseGame(){
		Time.timeScale = 0;
		Restart.SetActive(true);
		Continue.SetActive(true);
		ScoreBar.SetActive(true);
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
		Joystick.gameObject.SetActive(false);
	}

	public void StartGame() {
		Start.SetActive (false);
		ScoreBar.SetActive (false);
		Joystick.gameObject.SetActive(true);

		SceneMgr.Get.SetBallState (PlayerState.Start);
		GameController.Get.StartGame();
	}

	public void RestartGame(){
		ResetGame();
		Time.timeScale = 1;
		Restart.SetActive(false);
		Continue.SetActive(false);
	}

	public void ChangeControl(bool IsAttack) {
		shootBtnTime = ButtonBTime;
		showCoverAttack(false);
		showCoverDefence(false);
		ControlButtonGroup [0].SetActive (IsAttack);
		attackObject.SetActive(IsAttack);
		ControlButtonGroup [1].SetActive (!IsAttack);
		pushObject.SetActive(!IsAttack);
//		if(!IsAttack)
//			drawLine.IsShow = false;
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

	public void SetPassButton(int kind) {
		switch (kind) {
		case 1:
			passObject.SetActive(true);
			passA.SetActive(true);
			passB.SetActive(false);
			break;
		case 2:
			passObject.SetActive(true);
			passA.SetActive(false);
			passB.SetActive(true);
            break;
		default:
			passObject.SetActive(false);
			passA.SetActive(true);
			passA.SetActive(true);
			break;
        }
	}

	private void showScoreBar(){
		showScoreBarTime = showScoreBarInitTime;
		isShowScoreBar = true;
		ScoreBar.SetActive(true);
	}

	private void judgePlayerScreenPosition(){
		if(GameController.Get.Joysticker != null){
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

			if(GameStart.Get.IsSplitScreen) {
				screenLocation.SetActive(false);
//				if(playerInCameraX > 0 &&
//				   playerInCameraX < Screen.width &&
//				   playerInCameraY > 0 &&
//				   playerInCameraY < Screen.height) {
//					playerTexture.SetActive(false);
//				}
//				
//				if(playerInCameraX < -100 ||
//				   playerInCameraX >Screen.width + 100 ||
//				   playerInCameraY < - 100 ||
//				   playerInCameraY > Screen.height + 100){
//					playerTexture.SetActive(true);
//				}
				if(playerInCameraX > -50 &&
				   playerInCameraX < Screen.width + 100 &&
				   playerInCameraY > -50 &&
				   playerInCameraY < Screen.height + 100 ) {
					playerTexture.SetActive(false);
				} else {
					playerTexture.SetActive(true);
				}
			}
			else 
			{
				playerTexture.SetActive(false);
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
	}

	public bool OpenUIMask(PlayerBehaviour p = null){
		if(p == GameController.Get.Joysticker) {
			if(p.NoAiTime > 0)
				p.SetNoAiTime();

			shootBtnTime = ButtonBTime;
			showCoverAttack(false);
			showCoverDefence(false);
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
		
		if(buttonPushFXTime > 0) {
			buttonPushFXTime -= Time.deltaTime;
			if(buttonPushFXTime <= 0) {
				buttonPushFXTime = 0;
				buttonPushFX.SetActive(false);
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
