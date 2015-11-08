using G2;
using GamePlayEnum;
using UnityEngine;

public enum EUISituation{
	ShowTwo,
	Opening,
	Start, 
	Pause, 
	Continue, 
	Finish, 
	Reset,
	ReSelect,
	MainMenu,
	EffectSwitch,
	OptionSelect,
	MusicSwitch, 
	AITimeChange
}

public enum EUIControl {
	Attack,
	Block,
	Steal,
	Skill,
	Shoot,
	Push,
	Pass,
	PassA,
	PassB,
	AttackA,
	AttackB
}

public enum EUIPassType {
	MeBallOwner = 0,
	ABallOwner = 1,
	BBallOwner = 2
}

public enum EUIRangeType {
	Skill,
	Push,
	Elbow,
	Steal
}

public class UIGame : UIBase {
	private static UIGame instance = null;
	private const string UIName = "UIGame";

	private PlayerBehaviour PlayerMe;
	//Game const
	public float ButtonBTime = 0.08f; //Fake to shoot time
	private float showScoreBarInitTime = 3.15f;
	public int[] MaxScores = {13, 13};
	public int[] Scores = {0, 0};

	private float shootBtnTime = 0;
	private float showScoreBarTime = 0;

	private bool isPressElbowBtn = true;
	private bool isCanDefenceBtnPress = true;
	private bool isPressShootBtn = false;
	private bool isShowScoreBar = false;
	private bool isShootAvailable = true;

	// GoldFinger
	private bool isPressA = false;

	//GameJoystick
	private GameJoystick uiJoystick = null;
	private JoystickController joystickController;

	//Center
	private GameObject viewStart;

	//BottomRight
	private GameObject viewBottomRight;
	private GameObject uiShoot;
	private GameObject viewPass;
	private GameObject uiPassA;
	private GameObject uiPassB;
	private GameObject uiAlleyoopA;
	private GameObject uiAlleyoopB;
	private UISprite spriteAttack;
	private GameObject[] uiDefenceGroup = new GameObject[2]; // 0:ButtonSteal 1:ButtonBlock
	private GameObject[] controlButtonGroup= new GameObject[2];// 0:ViewAttack 1:ViewDefence
	private GameObject[] uiPassObjectGroup = new GameObject[3];// 0:SpriteMe 1:SpriteA 2:SpriteB
	
	//TopLeft
	private GameObject viewTopLeft;
	private GameObject uiLimitTime;
	private UILabel labelLimitTime;

	//Right
	private GameObject uiPlayerLocation;

	//Bottom
	private GameObject uiScoreBar;
	private GameObject uiLimitScore;
	private UILabel labelLimiteScore;
	private UILabel[] labelScores = new UILabel[2];
	private TweenRotation[] rotate = new TweenRotation[2];

	//TopRight
	private GameObject viewForceBar;
	private GameObject[] uiButtonSkill = new GameObject[3];
	private GameObject[] uiSkillEnables = new GameObject[3];
	private GameObject[] uiDCs = new GameObject[3];
	private UISprite[] spriteSkills = new UISprite[3];
	private UISprite[] spriteEmptys = new UISprite[3];
	private UISprite spriteForce;
	private UISprite spriteForceFirst;
	private GameObject uiSpriteFull;

	//DC
	private int dcCount = 0;
	private float baseForceValue;
	private float oldForceValue;
	private float newForceValue;
	private float timeForce;

	private DrawLine drawLine;
	
	private bool isShowSkillRange;
	private bool isShowPushRange;
	private bool isShowElbowRange;
	private bool isShowStealRange;
	private Transform skillRangeTarget;
	private PlayerBehaviour nearP;
	private float eulor;

	//FX
	private float fxTime = 0.3f;
	private GameObject buttonShootFX;
	private float buttonShootFXTime;
	private GameObject buttonBlockFX;
	private float buttonBlockFXTime;
	private GameObject buttonStealFX;
	private float buttonStealFXTime;
	private GameObject buttonAttackFX;
	private float buttonAttackFXTime;
	private GameObject buttonPassFX;
	private float buttonPassFXTime;
	private GameObject buttonPassAFX;
	private float buttonPassAFXTime;
	private GameObject buttonPassBFX;
	private float buttonPassBFXTime;

	//Location
	float playerInCameraX;
	float playerInCameraY;
	float playerInBoardX;
	float playerInBoardY;
	float baseValueX = 37.65f; 
	float baseValueY = 13.37f;
	float playerX;
	float playerY;
	Vector2 playerScreenPos;

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
		if(isShow)
			Get.drawLine.IsShow = isShow;

		if(instance)
			instance.Show(isShow);
		else
		if(isShow)
			Get.Show(isShow);
	}

	void FixedUpdate()
	{
		if(PlayerMe) {
			if(isShowSkillRange || isShowElbowRange || isShowPushRange || isShowStealRange) {
				if(isShowPushRange) {
					nearP = GameController.Get.FindNearNpc();
					CourtMgr.Get.RangeOfActionEuler( MathUtils.FindAngle(PlayerMe.PlayerRefGameObject.transform, nearP.PlayerRefGameObject.transform.position));
				} else if(isShowStealRange) {
					if(GameController.Get.BallOwner != null)
						CourtMgr.Get.RangeOfActionEuler( MathUtils.FindAngle(PlayerMe.PlayerRefGameObject.transform, GameController.Get.BallOwner.transform.position));
				}
				if(skillRangeTarget != null)
					CourtMgr.Get.RangeOfActionPosition(skillRangeTarget.position);
			}
		}

//		if(uiDC != null && uiDC.activeInHierarchy) {
//			if(dcLifeTime > 0) {
//				dcLifeTime -= Time.deltaTime;
//				if(dcLifeTime <= 0) {
//					uiDC.SetActive(false);
//				}
//			}
//		}

		runForceValue ();
		if (isPressShootBtn && shootBtnTime > 0) {
			shootBtnTime -= Time.deltaTime;
			if(shootBtnTime <= 0){
				isPressShootBtn = false;
				if (PlayerMe && GameController.Get.BallOwner == PlayerMe) {
					if (GameController.Get.DoShoot(true)) {
						PlayerMe.SetManually();
						spriteAttack.gameObject.SetActive(false);
						ShowSkillEnableUI(false);
					}

					if(GameController.Get.IsCanPassAir) {
						SetPassButton();
					}
				}
			}
		}
        
        if(isShowScoreBar && showScoreBarTime > 0) {
            showScoreBarTime -= Time.deltaTime;
            if(showScoreBarTime <= 0){
                isShowScoreBar = false;
                uiScoreBar.SetActive(false);
            }
        }
        showButtonFX();
        judgePlayerScreenPosition();
		setGameTime();
	}

	protected override void InitCom() {
		GameController.Get.onSkillDCComplete += AddForceValue;
		SetBtnFun (UIName + "/TopLeft/ButtonSpeed", OnSpeed);

		/*
		#if !UNITY_EDITOR
		GameObject obj;
		obj = GameObject.Find (UIName + "/TopLeft/ButtonSpeed");
		if (obj)
			obj.SetActive(false);
		#endif
		*/
		//GameJoystick
		uiJoystick = GameObject.Find (UIName + "/GameJoystick").GetComponent<GameJoystick>();
		if (uiJoystick)
			joystickController = uiJoystick.gameObject.AddComponent<JoystickController> ();
		
		uiJoystick.Joystick = GameObject.Find (UIName + "/GameJoystick").GetComponent<EasyJoystick>();

		//Center
		viewStart = GameObject.Find (UIName + "/Center/ViewStart");
		
		//BottomRight
		viewBottomRight = GameObject.Find(UIName + "/BottomRight");
		uiShoot = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonShoot/SpriteShoot");
		viewPass = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass");
		uiPassA = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA");
		uiPassB = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB");
		uiAlleyoopA = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/AlleyoopA");
		uiAlleyoopB = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/AlleyoopB");
		spriteAttack = GameObject.Find (UIName + "/BottomRight/ButtonAttack/SpriteAttack").GetComponent<UISprite>();
		uiDefenceGroup[0] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal/SpriteSteal");
		uiDefenceGroup[1] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonBlock/SpriteBlock");
		controlButtonGroup [0] = GameObject.Find (UIName + "/BottomRight/ViewAttack");
		controlButtonGroup [1] = GameObject.Find (UIName + "/BottomRight/ViewDefance");
		uiPassObjectGroup [0] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonPass/SpriteMe");
		uiPassObjectGroup [1] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA/SpriteA");
		uiPassObjectGroup [2] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB/SpriteB");
		
		//TopLeft
		viewTopLeft = GameObject.Find(UIName + "TopLeft");
		uiLimitTime = GameObject.Find (UIName + "/TopLeft/Countdown");
		labelLimitTime = GameObject.Find (UIName + "/TopLeft/Countdown/TimeLabel").GetComponent<UILabel>();
		SetBtnFun (UIName + "/TopLeft/ButtonPause", OnPause);
		
		//Right
		uiPlayerLocation = GameObject.Find (UIName + "/Right");
		
		//Bottom
		uiScoreBar = GameObject.Find (UIName + "/Bottom/UIScoreBar");
		uiLimitScore = GameObject.Find (UIName + "/Bottom/UIScoreBar/LimitScore");
		labelLimiteScore = GameObject.Find (UIName + "/Bottom/UIScoreBar/LimitScore/TargetScore").GetComponent<UILabel>();
		labelScores [0] = GameObject.Find (UIName + "/Bottom/UIScoreBar/LabelScore1").GetComponent<UILabel>();
		rotate[0] = GameObject.Find (UIName + "/Bottom/UIScoreBar/LabelScore1").GetComponent<TweenRotation>();
		labelScores [1] = GameObject.Find (UIName + "/Bottom/UIScoreBar/LabelScore2").GetComponent<UILabel>();
		rotate[1] = GameObject.Find (UIName + "/Bottom/UIScoreBar/LabelScore2").GetComponent<TweenRotation>();
		
		//TopRight
		viewForceBar = GameObject.Find(UIName + "/TopRight/ViewForceBar");
		spriteForce = GameObject.Find (UIName + "/TopRight/ViewForceBar/Forcebar/SpriteForce").GetComponent<UISprite>();
		spriteForceFirst = GameObject.Find (UIName + "/TopRight/ViewForceBar/Forcebar/SpriteForceFrist").GetComponent<UISprite>();
		uiSpriteFull = GameObject.Find (UIName + "/TopRight/ViewForceBar/ForcebarFull");
		for(int i=0; i<uiSkillEnables.Length; i++) {
			uiButtonSkill[i] = GameObject.Find(UIName + "/TopRight/ButtonSkill" + i.ToString());
			uiButtonSkill[i].name = i.ToString();
			uiSkillEnables[i] = GameObject.Find(uiButtonSkill[i].name + "/SpriteFull");
			uiDCs[i] = GameObject.Find (uiButtonSkill[i].name  + "/GetDCSoul");
			uiDCs[i].SetActive(false);
			spriteSkills[i] = GameObject.Find(uiButtonSkill[i].name  + "/SpriteSkill").GetComponent<UISprite>();
			spriteEmptys[i] = GameObject.Find(uiButtonSkill[i].name  + "/SkillEmpty").GetComponent<UISprite>();
			UIEventListener.Get (uiButtonSkill[i]).onPress = DoSkill;
			UIEventListener.Get (uiButtonSkill[i]).onDragOver = DoSkillOut;
			uiButtonSkill[i].SetActive(false);
		}

		buttonShootFX = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonShoot/UI_FX_A_21");
		buttonBlockFX = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonBlock/UI_FX_A_21");
		buttonStealFX = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal/UI_FX_A_21");
		buttonAttackFX = GameObject.Find(UIName + "/BottomRight/ButtonAttack/UI_FX_A_21");
		buttonPassFX = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonPass/UI_FX_A_21");
		buttonPassAFX = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA/UI_FX_A_21");
		buttonPassBFX = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB/UI_FX_A_21");

		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewAttack/ButtonShoot")).onPress = DoShoot;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonPass")).onPress = DoPassChoose;
		UIEventListener.Get (uiPassA).onPress = DoPassTeammateA;
		UIEventListener.Get (uiPassB).onPress = DoPassTeammateB;
		UIEventListener.Get (uiAlleyoopA).onPress = DoPassTeammateA;
		UIEventListener.Get (uiAlleyoopB).onPress = DoPassTeammateB;

		SetBtnFun (UIName + "/Center/ViewStart/ButtonStart", StartGame);
		SetBtnFun (UIName + "/BottomRight/ViewDefance/ButtonBlock", DoBlock);
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ButtonAttack")).onPress = DoAttack;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ButtonAttack")).onDragOver = DoAttackOut;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewDefance/ButtonSteal")).onPress = DoSteal;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewDefance/ButtonSteal")).onDragOver = DoStealOut;
		
		drawLine = gameObject.AddComponent<DrawLine>();
	}

	protected override void InitData() {
		isShowScoreBar = false;
		Scores [0] = 0;
		Scores [1] = 0;
		labelScores[0].text = "0";
		labelScores[1].text = "0";
		spriteForce.fillAmount = 0;
		spriteForceFirst.fillAmount = 0;
		
		if(GameController.Get.StageHintBit[0] == 0) 
			uiLimitTime.SetActive(false);
		else 
			labelLimitTime.text = GameController.Get.GameTime.ToString();
		
		if(GameController.Get.StageHintBit[1] == 2)
			labelLimiteScore.text = GameController.Get.GameWinValue.ToString();
		else
			uiLimitScore.SetActive(false);
		
		uiPlayerLocation.SetActive(false);
		uiScoreBar.SetActive(false);
		uiAlleyoopA.SetActive(false);
		uiAlleyoopB.SetActive(false);
		viewTopLeft.SetActive(false);
		uiSpriteFull.SetActive(false);
		buttonShootFX.SetActive(false);
		buttonBlockFX.SetActive(false);
		buttonStealFX.SetActive(false);
		buttonAttackFX.SetActive(false);
		buttonPassFX.SetActive(false);
		buttonPassAFX.SetActive(false);
		buttonPassBFX.SetActive(false);

		ChangeControl(true);
		showViewForceBar(false);
		ShowSkillEnableUI(false);
		uiJoystick.Joystick.isActivated = false;
		uiJoystick.Joystick.JoystickPositionOffset = new Vector2(200, UI2D.Get.RootHeight - 145);
		drawLine.IsShow = false;
    }

	protected override void InitText(){

	}

	private void setGameTime () {
		int minute = (int) (GameController.Get.GameTime / 60f);
		int second = (int) (GameController.Get.GameTime % 60f);
		if(second < 10)
			labelLimitTime.text = minute.ToString() + ":0" + second.ToString();
		else 
			labelLimitTime.text = minute.ToString() + ":" + second.ToString();
	}

	private void resetRange () {
		isShowElbowRange = false;
		isShowPushRange = false;
		isShowSkillRange = false;
		isShowStealRange = false;
		CourtMgr.Get.ShowRangeOfAction(false);
		CourtMgr.Get.ShowArrowOfAction(false);
	}

	public void InitGame (PlayerBehaviour p) {
		PlayerMe = p;
		for(int i=0; i<PlayerMe.Attribute.ActiveSkills.Count; i++) {
			if(IsPlayerMe && PlayerMe.Attribute.ActiveSkills.Count > 0) {
				spriteSkills[i].spriteName = GameData.DSkillData[PlayerMe.Attribute.ActiveSkills[i].ID].PictureNo + "s";
				spriteEmptys[i].spriteName = GameData.DSkillData[PlayerMe.Attribute.ActiveSkills[i].ID].PictureNo + "s";
			}
		}
		initLine();
	}
	
	private void initLine() {
		drawLine.ClearTarget();
		if (drawLine.UIs.Length == 0) {
			GameObject obj = GameObject.Find("PlayerInfoModel/Self0/PassMe");
			if (obj)
				drawLine.AddTarget(uiPassObjectGroup[0], obj);
		}
		drawLine.Show(true);
	}

	private GameObject getSkillRangeTarget (){
		if(GameStart.Get.TestMode == EGameTest.AttackA) 
			return PlayerMe.PlayerRefGameObject;
		if (IsPlayerMe && GameData.DSkillData.ContainsKey(PlayerMe.ActiveSkillUsed.ID)) {
			switch (GameData.DSkillData[PlayerMe.ActiveSkillUsed.ID].TargetKind) {
			case 0:
			case 4:
			case 6:
			case 10:
				return PlayerMe.PlayerRefGameObject;
			case 1:
				return CourtMgr.Get.BasketRangeCenter[0];
			case 2:
				return CourtMgr.Get.BasketRangeCenter[1];
			default:
				return null;
			}
		}
		return null;
	}

	private bool isCircleRange{
		get {
			if(IsPlayerMe && GameData.DSkillData.ContainsKey(PlayerMe.ActiveSkillUsed.ID)) {
				if(GameData.DSkillData[PlayerMe.ActiveSkillUsed.ID].Kind == 171)
					return false;
			}
			return true;
		}
	}

	private void showRange (EUIRangeType type, bool state) {
		skillRangeTarget = null;

		if(state && IsPlayerMe) {
			switch (type){
			case EUIRangeType.Skill:
				isShowSkillRange = state;
				isShowElbowRange = !state;
				isShowPushRange = !state;
				isShowStealRange = !state;

				if(getSkillRangeTarget() != null){
					skillRangeTarget = getSkillRangeTarget().transform;
					if (GameData.DSkillData.ContainsKey(PlayerMe.ActiveSkillUsed.ID)) {
						if(isCircleRange) {
							CourtMgr.Get.ShowRangeOfAction(state, 
							                               skillRangeTarget, 
							                               360, 
							                               GameData.DSkillData[PlayerMe.ActiveSkillUsed.ID].Distance(PlayerMe.ActiveSkillUsed.Lv)); 
						} else {
							//Draw Arrow
							CourtMgr.Get.ShowArrowOfAction(state,
							                               skillRangeTarget,
							                               GameData.DSkillData[PlayerMe.ActiveSkillUsed.ID].Distance(PlayerMe.ActiveSkillUsed.Lv));
						}
					}
				}
				break;
			case EUIRangeType.Elbow:
				isShowSkillRange = !state;
				isShowElbowRange = state;
				isShowPushRange = !state;
				isShowStealRange = !state;
				skillRangeTarget = PlayerMe.PlayerRefGameObject.transform;
				CourtMgr.Get.ShowRangeOfAction(state, 
				                               PlayerMe.PlayerRefGameObject.transform, 
				                               270, 
				                               GameConst.StealPushDistance); 
				break;
			case EUIRangeType.Push:
				isShowSkillRange = !state;
				isShowElbowRange = !state;
				isShowPushRange = state;
				isShowStealRange = !state;
				skillRangeTarget = PlayerMe.PlayerRefGameObject.transform;
				eulor = 0;
				nearP = GameController.Get.FindNearNpc();
				if(nearP)
					eulor = MathUtils.FindAngle(PlayerMe.PlayerRefGameObject.transform, nearP.PlayerRefGameObject.transform.position);
				CourtMgr.Get.ShowRangeOfAction(state, 
				                               PlayerMe.PlayerRefGameObject.transform, 
				                               30, 
				                               GameConst.StealPushDistance,
				                               eulor); 
				break;
			case EUIRangeType.Steal:
				isShowSkillRange = !state;
				isShowElbowRange = !state;
				isShowPushRange = !state;
				isShowStealRange = state;
				skillRangeTarget = PlayerMe.PlayerRefGameObject.transform;
				eulor = 0;
				if (GameController.Get.BallOwner)
					eulor = MathUtils.FindAngle(PlayerMe.PlayerRefGameObject.transform, GameController.Get.BallOwner.PlayerRefGameObject.transform.position);
				CourtMgr.Get.ShowRangeOfAction(state, 
				                               PlayerMe.PlayerRefGameObject.transform, 
				                               30, 
				                               GameConst.StealPushDistance,
				                               eulor); 
				break;
			}
		} else {
			CourtMgr.Get.ShowRangeOfAction(state);
			CourtMgr.Get.ShowArrowOfAction(state);
		}
	}

	public void DoAttackOut (GameObject go) {
		if(IsPlayerMe) {
			if(PlayerMe.IsBallOwner) {
				//Elbow
				if(isShowElbowRange)
					resetRange();
			} else {
				//Push
				if(isShowPushRange)
					resetRange();
			}
		}
	}
	
	public void DoAttack(GameObject go, bool state){
		if(IsPlayerMe) {
			if(PlayerMe.IsBallOwner) {
				//Elbow
				if(!state) 
					if(isShowElbowRange)
						UIControllerState(EUIControl.Attack);
				else
					resetRange();
				showRange(EUIRangeType.Elbow, state);
			} else {
				//Push
				if(!state) 
					if(isShowPushRange)
						UIControllerState(EUIControl.Attack);
				else
					resetRange();
				showRange(EUIRangeType.Push, state);
			}
		}
	}

	//Defence
	public void DoBlock() {UIControllerState(EUIControl.Block);}

	public void DoStealOut (GameObject go) {if(isShowStealRange) resetRange ();}

	public void DoSteal(GameObject go, bool state){
		if(!state) 
			if(isShowStealRange)
				UIControllerState(EUIControl.Steal);
			else
				resetRange();
		
		showRange(EUIRangeType.Steal, state);
	}
	
	//Attack
	public void DoSkillOut (GameObject go) {
		if(isShowSkillRange) 
			resetRange ();
	}
	public void DoSkill(GameObject go, bool state){
		if(PlayerMe.Attribute.ActiveSkills.Count > 0 && go && IsPlayerMe) {
			int id = 0;
			if(int.TryParse(go.name, out id)) {
				PlayerMe.ActiveSkillUsed = PlayerMe.Attribute.ActiveSkills[id];
				if(!state) 
					if(isShowSkillRange)
						UIControllerState(EUIControl.Skill);
					else
						resetRange();
				
				showRange(EUIRangeType.Skill, state);
			}
		} else
			resetRange ();
	}

	public void OnSpeed(){
		if (Time.timeScale == 1) 
			Time.timeScale = 2;
		else
		if (Time.timeScale == 2) 
			Time.timeScale = 0.5f;
		else
		if (Time.timeScale == 0.5f) 
			Time.timeScale = 1;

		GameController.Get.RecordTimeScale = Time.timeScale;
	}
	
	public void DoShoot(GameObject go, bool state) {UIControllerState(EUIControl.Shoot, go, state);}
	
	public void DoPassChoose (GameObject obj, bool state) {UIControllerState(EUIControl.Pass, obj, state);}
	
	public void DoPassNone() {SetPassButton();}
	
	public void DoPassTeammateA(GameObject obj, bool state) {
		isPressA = state;
		UIControllerState(EUIControl.PassA, obj, state);
	}
	
	public void DoPassTeammateB(GameObject obj, bool state) {
		if(IsPlayerMe && isPressA) {
			PlayerMe.SetAnger(PlayerMe.Attribute.MaxAnger);
			AddAllForce();
		}
		UIControllerState(EUIControl.PassB, obj, state);
	}
	
	public void OnReselect() {UIState(EUISituation.ReSelect);}

	public void OnPause(){if(Time.timeScale == 0) UIState(EUISituation.Continue);else UIState(EUISituation.Pause);}

	public void ResetGame() {UIState(EUISituation.Reset);}

	public void StartGame() {UIState(EUISituation.Start);}

	public void GameOver(){UIState(EUISituation.Finish);}

	public void ShowSkillEnableUI (bool isShow, int index = 0, bool isAngerFull = false, bool canUse = false){
		if(IsPlayerMe) {
			if(PlayerMe.Attribute.ActiveSkills.Count > 0 && index < PlayerMe.Attribute.ActiveSkills.Count) {
				if (isShow) {
					if(GameController.Get.IsStart)
						uiSkillEnables[index].SetActive((canUse && isAngerFull));
					else
						uiSkillEnables[index].SetActive(false);
				} else {
					for(int i=0; i<uiSkillEnables.Length; i++) {
						uiSkillEnables[i].SetActive(false);
					}
				}
			}
		}
	}
	
	public void ShowAlleyoop(bool isShow, int teammate = 1) {
		if(isShow && GameController.Get.Situation == EGameSituation.AttackGamer) {
			if(teammate == 1) {
				uiPassA.SetActive(!isShow);
				uiAlleyoopA.SetActive(isShow);
			} else if(teammate == 2) {
				uiPassB.SetActive(!isShow);
				uiAlleyoopB.SetActive(isShow);
			}
		} else {
			uiPassA.SetActive(true);
			uiAlleyoopA.SetActive(false);
			uiPassB.SetActive(true);
			uiAlleyoopB.SetActive(false);
		}
	}

	public void SetAngerUI (float max, float anger, int count){
		timeForce = 0;
		uiSpriteFull.SetActive (false);
		showViewForceBar(GameController.Get.IsStart);

		if (max > 0) {
			oldForceValue = spriteForce.fillAmount;
			newForceValue = anger / max;
			dcCount += count;
			baseForceValue = (newForceValue - oldForceValue) / dcCount;
//			spriteForceFirst.fillAmount = newForceValue;
			if (newForceValue >= 1) {
				uiSpriteFull.SetActive (true);
			} else if (newForceValue <=0 ) {
				oldForceValue = 0;
				newForceValue = 0;
				spriteForce.fillAmount = 0;
				spriteForceFirst.fillAmount = 0;
			}
			spriteForce.gameObject.SetActive(false);
			spriteForce.gameObject.SetActive(true);
			spriteForceFirst.gameObject.SetActive(false);
			spriteForceFirst.gameObject.SetActive(true);
		}
		if(IsPlayerMe) {
			if(PlayerMe.Attribute.ActiveSkills.Count > 0) {
				for(int i=0; i<PlayerMe.Attribute.ActiveSkills.Count; i++) {
					uiButtonSkill[i].SetActive((i < PlayerMe.Attribute.ActiveSkills.Count));
					if(uiButtonSkill[i].activeSelf) {
						spriteSkills[i].fillAmount = PlayerMe.Attribute.MaxAngerPercent(PlayerMe.Attribute.ActiveSkills[i].ID, anger);
						spriteSkills[i].gameObject.SetActive(false);
						spriteSkills[i].gameObject.SetActive(true);
					}
				}
			}
		}
	}
	
	public void PlusScore(int team, int score) {
		Scores [team] += score;
		CourtMgr.Get.SetScoreboards (team, Scores [team]);
		showScoreBar(GameController.Get.IsStart);
		resetScoreRotate();
		TweenRotation rotateScore = TweenRotation.Begin(labelScores[team].gameObject, 0.5f / Time.timeScale, Quaternion.identity);
		rotateScore.delay = 0.2f / Time.timeScale;
		rotateScore.from = Vector3.zero;
		rotateScore.to = new Vector3(0,720,0);
		labelScores[team].text = Scores [team].ToString ();
	}

	public void ChangeControl(bool IsAttack) {
		if(IsAttack) 
			UIMaskState(EUIControl.AttackA);
		else 
			UIMaskState(EUIControl.AttackB);
	}
	/// <summary>
	/// For GoldFinger
	/// </summary>
	public void AddAllForce (){
		spriteForce.fillAmount = 1;
		spriteForceFirst.fillAmount = 1;
	}

	public bool SetUIJoystick(PlayerBehaviour p = null, bool isShow = false){
		if(IsPlayerMe && p == PlayerMe) {
			if (GameController.Get.IsStart) {
				uiJoystick.gameObject.SetActive(isShow);
				return true;
			}
		}
		return false;
	}
	
	public bool AddForceValue(){
//		dcLifeTime = 0.1f;
		AudioMgr.Get.PlaySound(SoundType.SD_CatchMorale);

		oldForceValue += baseForceValue;
		if(dcCount >= 0)
			dcCount --;

		oldForceValue = Mathf.Clamp(oldForceValue, 0, newForceValue);
		spriteForce.fillAmount = oldForceValue;
		if(!SkillDCExplosion.Get.IsHaveDC) {
			dcCount = 0;
			spriteForce.fillAmount = newForceValue;
		}
		return true;
	}

	public bool UICantUse(PlayerBehaviour p = null) {
		if(IsPlayerMe) {
			if(p == PlayerMe) {
				if (GameController.Get.IsStart) {
					SetPassButton();
					ShowSkillEnableUI(false);
					resetRange ();
					spriteAttack.gameObject.SetActive(true);
				}
				return true;
			} else {
				if (p.Team == PlayerMe.Team && p.crtState == EPlayerState.Alleyoop)
					SetPassButton();
				
				return false;
			}
		}
		return false;
	}

	public bool OpenUIMask(PlayerBehaviour p = null){
		if(IsPlayerMe && p == PlayerMe) {
			if (GameController.Get.IsStart) { 
				ShowAlleyoop(false);
				
				if(GameController.Get.Situation == EGameSituation.AttackGamer) 
					UIMaskState(EUIControl.AttackA);
				else 
					UIMaskState(EUIControl.AttackB);
			}

			return true;
		} else {
			if (IsPlayerMe && p.Team == PlayerMe.Team && p.crtState == EPlayerState.Alleyoop)
				ShowAlleyoop(false);

			return false;
		}
	}

	public void SetPassButton() {
		if(GameStart.Get.TestMode != EGameTest.None && GameStart.Get.TestMode != EGameTest.Pass && GameStart.Get.TestMode != EGameTest.Alleyoop) 
			return;

		if (GameController.Get.IsShowSituation)
			return;

		int who = GameController.Get.GetBallOwner;
		switch (who) {
		case (int)EUIPassType.MeBallOwner:
			resetRange ();
			viewPass.SetActive(true);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(true);
			uiPassObjectGroup[2].SetActive(true);
			spriteAttack.spriteName = "B_elbow";
			break;
		case (int)EUIPassType.ABallOwner:
			viewPass.SetActive(true);
			uiPassObjectGroup[0].SetActive(true);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(true);
			spriteAttack.spriteName = "B_push";
			break;
		case (int)EUIPassType.BBallOwner:
			viewPass.SetActive(true);
			uiPassObjectGroup[0].SetActive(true);
			uiPassObjectGroup[1].SetActive(true);
			uiPassObjectGroup[2].SetActive(false);
			spriteAttack.spriteName = "B_push";
			break;
		default:
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			spriteAttack.spriteName = "B_push";
			if(GameController.Get.Situation == EGameSituation.AttackNPC || GameController.Get.Situation == EGameSituation.NPCPickBall) {
				viewPass.SetActive(false);
			} else {
				viewPass.SetActive(true);
			}
			break;
		}
	}

	public void UIMaskState (EUIControl controllerState) {
		joystickController.UIMaskState(controllerState);
		switch (controllerState) {
		case EUIControl.Skill:
			spriteAttack.gameObject.SetActive(false);
			uiDefenceGroup[0].SetActive(false);
			uiDefenceGroup[1].SetActive(false);
			uiShoot.SetActive(false);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case EUIControl.Attack:
			if(IsPlayerMe && PlayerMe.IsBallOwner) {
				//Elbow Attack
				UIEffectState(EUIControl.Attack);

				uiShoot.SetActive(false);
				spriteAttack.gameObject.SetActive(true);
				uiDefenceGroup[0].SetActive(false);
				uiDefenceGroup[1].SetActive(false);
				uiPassObjectGroup[0].SetActive(false);
				uiPassObjectGroup[1].SetActive(false);
				uiPassObjectGroup[2].SetActive(false);
			} else {
				//Push Deffence
				UIEffectState(EUIControl.Attack);

				ShowSkillEnableUI(false);
				uiShoot.SetActive(false);
				spriteAttack.gameObject.SetActive(true);
				uiDefenceGroup[0].SetActive(false);
				uiDefenceGroup[1].SetActive(false);
				uiPassObjectGroup[0].SetActive(false);
				uiPassObjectGroup[1].SetActive(false);
				uiPassObjectGroup[2].SetActive(false);
			}
			break;
		case EUIControl.Block:
			UIEffectState(EUIControl.Block);

			ShowSkillEnableUI(false);
			spriteAttack.gameObject.SetActive(false);
			uiDefenceGroup[0].SetActive(false);
			uiDefenceGroup[1].SetActive(true);
			break;
		case EUIControl.Steal:
			UIEffectState(EUIControl.Steal);

			ShowSkillEnableUI(false);
			spriteAttack.gameObject.SetActive(false);
			uiDefenceGroup[0].SetActive(true);
			uiDefenceGroup[1].SetActive(false);
			break;
		case EUIControl.Shoot:
			ShowSkillEnableUI(false);
			spriteAttack.gameObject.SetActive(false);
			uiShoot.SetActive(true);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case EUIControl.Pass:
		case EUIControl.PassA:
		case EUIControl.PassB:
			ShowSkillEnableUI(false);
			spriteAttack.gameObject.SetActive(false);
			uiShoot.SetActive(false);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case EUIControl.AttackA:
			shootBtnTime = ButtonBTime;
			isCanDefenceBtnPress = true;
			isPressElbowBtn = true;

			controlButtonGroup[0].SetActive(true);
			controlButtonGroup[1].SetActive(false);

			uiShoot.SetActive(true);
			spriteAttack.gameObject.SetActive(true);
			uiDefenceGroup[0].SetActive(false);
			uiDefenceGroup[1].SetActive(false);
			SetPassButton();

			break;
		case EUIControl.AttackB:
			shootBtnTime = ButtonBTime;
			isCanDefenceBtnPress = true;
			isPressElbowBtn = true;
			
			controlButtonGroup[0].SetActive(false);
			controlButtonGroup[1].SetActive(true);

			uiShoot.SetActive(false);
			spriteAttack.gameObject.SetActive(true);
			uiDefenceGroup[0].SetActive(true);
			uiDefenceGroup[1].SetActive(true);
			SetPassButton();

			break;
		}
	}

	public void UIControllerState (EUIControl controllerState, GameObject go = null, bool state = false) {
		if (GameController.Get.IsShowSituation || !PlayerMe.CanPressButton)
			return;
			
		if (IsPlayerMe && (GameController.Get.Situation == EGameSituation.AttackGamer || GameController.Get.Situation == EGameSituation.AttackNPC)) {
			bool noAI = false;
			switch(controllerState) {
			case EUIControl.Skill:
				if(PlayerMe.Attribute.ActiveSkills.Count > 0)
					noAI = GameController.Get.OnSkill(PlayerMe.ActiveSkillUsed);
				if (noAI)
					UIMaskState(EUIControl.Skill);
					
				break;

			case EUIControl.Attack:
				if(PlayerMe.IsBallOwner) {
					//Elbow
					if(isPressElbowBtn && 
					   !PlayerMe.IsFall && 
					   GameController.Get.Situation == EGameSituation.AttackGamer &&
					   PlayerMe.IsElbow) {
						noAI = GameController.Get.DoElbow ();
						if(noAI)
							UIMaskState(EUIControl.Attack);
					}
				} else {
					//Push
					if(isCanDefenceBtnPress && 
					   !PlayerMe.IsFall &&
					   (GameController.Get.Situation == EGameSituation.AttackNPC || GameController.Get.Situation == EGameSituation.AttackGamer) &&
					    PlayerMe.CanUseState(EPlayerState.Push0)) {
						noAI = GameController.Get.DoPush(nearP);
						if(noAI)
							UIMaskState(EUIControl.Attack);
					}
				}

				break;
			case EUIControl.Block:
				if(isCanDefenceBtnPress && 
				   !PlayerMe.IsFall && 
				   GameController.Get.Situation == EGameSituation.AttackNPC){
					noAI = GameController.Get.DoBlock();
					if(noAI)
						UIMaskState(EUIControl.Block);
				}

				break;
			case EUIControl.Steal:
				if(isCanDefenceBtnPress && 
				   !PlayerMe.IsFall &&
				   GameController.Get.Situation == EGameSituation.AttackNPC && 
				   GameController.Get.StealBtnLiftTime <= 0 && 
				   PlayerMe.CanUseState(EPlayerState.Steal0)) {
					noAI = GameController.Get.DoSteal();
					if(noAI)
						UIMaskState(EUIControl.Steal);
				}
				break;
			case EUIControl.Shoot:
				if(GameController.Get.IsShooting) {
					if(state){
						int index = GameController.Get.GetShootPlayerIndex();
						if(index!= -1 && index < UIDoubleClick.Get.DoubleClicks.Length && UIDoubleClick.Get.DoubleClicks[index].Enable)
							UIDoubleClick.Get.ClickStop (index);
					}
				} else {
					if(PlayerMe.IsBallOwner &&
					   GameController.Get.Situation == EGameSituation.AttackGamer && 
					   !PlayerMe.IsFall && 
					   !PlayerMe.CheckAnimatorSate(EPlayerState.MoveDodge0) && 
					   !PlayerMe.CheckAnimatorSate(EPlayerState.MoveDodge1) && 
					   !PlayerMe.IsBlock
					   ) {
						if(state && PlayerMe.IsFakeShoot && isShootAvailable) 
							isShootAvailable = false;

						if (state)
							UIEffectState(EUIControl.Shoot);
						else 
						if (!state && shootBtnTime > 0 && isShootAvailable){
							if(GameController.Get.BallOwner != null) {
								if(PlayerMe.IsBallOwner) 
									UIMaskState(EUIControl.Shoot);

								shootBtnTime = ButtonBTime;
							}

							noAI = GameController.Get.DoShoot (false);
						} else
						if (!state && !isShootAvailable) 
							isShootAvailable = true;
						
						isPressShootBtn = state;
					} else 
						if (!PlayerMe.IsBallOwner)
						noAI = GameController.Get.DoShoot(true);
				}

				break;
			case EUIControl.Pass:
				if(!PlayerMe.IsBallOwner) {
					if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(0)){
						UIMaskState(EUIControl.Pass);
						noAI = true;
					}
				}

				break;
			case EUIControl.PassA:
				if(GameController.Get.GetBallOwner != 1) 
					if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(1))
						UIMaskState(EUIControl.PassA);

				break;
			case EUIControl.PassB:
				if(GameController.Get.GetBallOwner != 2) 
					if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(2))
						UIMaskState(EUIControl.PassB);

				break;
			}

			if (noAI)
				PlayerMe.SetManually();
		}
	}

	public void UIState(EUISituation situation){
		resetRange ();
		switch(situation) {
		case EUISituation.ShowTwo:
			showViewForceBar(false);
			viewPass.SetActive(false);
			viewStart.SetActive (true);
			viewTopLeft.SetActive(false);
			viewBottomRight.SetActive(false);
			uiJoystick.gameObject.SetActive(false);
			controlButtonGroup[0].SetActive(false);
			controlButtonGroup[1].SetActive(false);
			uiJoystick.Joystick.isActivated = false;
			drawLine.IsShow = false;
			break;
		case EUISituation.Opening:
			viewPass.SetActive(true);
			viewTopLeft.SetActive(true);
			viewBottomRight.SetActive(true);
			uiJoystick.gameObject.SetActive(true);
			controlButtonGroup[0].SetActive(true);
			controlButtonGroup[1].SetActive(false);
			uiJoystick.Joystick.isActivated = false;
			drawLine.IsShow = true;
			break;
		case EUISituation.Start:
			GameController.Get.PlayCount ++;
			showViewForceBar(true);
			viewStart.SetActive (false);
			viewTopLeft.SetActive(true);
			uiSpriteFull.SetActive (false);
			viewBottomRight.SetActive(true);
			uiJoystick.gameObject.SetActive(true);
			uiJoystick.Joystick.isActivated = true;

			viewPass.SetActive(GameController.Get.Situation == EGameSituation.AttackGamer);
			controlButtonGroup[0].SetActive(GameController.Get.Situation == EGameSituation.AttackGamer);
			controlButtonGroup[1].SetActive(GameController.Get.Situation != EGameSituation.AttackGamer);
			SetPassButton();
			
			CourtMgr.Get.SetBallState (EPlayerState.Start);
			GameController.Get.StartGame();
			drawLine.IsShow = false;
			
			if(IsPlayerMe && PlayerMe.Attribute.ActiveSkills.Count > 0) {
				for (int i=0; i<PlayerMe.Attribute.ActiveSkills.Count; i++) {
					uiButtonSkill[i].SetActive(true);
				}
			}
			break;
		case EUISituation.Pause:
			if (!viewStart.activeInHierarchy) {
				Time.timeScale = 0;
				viewBottomRight.SetActive(false);
				showViewForceBar(false);

				uiScoreBar.SetActive(false);
				uiJoystick.gameObject.SetActive(false);
				ShowSkillEnableUI(false);

				if(UIPassiveEffect.Visible)UIPassiveEffect.UIShow(false);

				for (int i = 0; i < GameController.Get.GamePlayers.Count; i ++) {
					if (i < GameController.Get.GameRecord.PlayerRecords.Length) {
						GameController.Get.GameRecord.PlayerRecords[i] = GameController.Get.GamePlayers[i].GameRecord;
					}
				}
				UIGamePause.Get.SetGameRecord(ref GameController.Get.GameRecord);
			}
			break;
		case EUISituation.Continue:
			if (GameController.Get.IsStart) {
				Time.timeScale = 1;
				viewBottomRight.SetActive(true);
				showViewForceBar(true);

				uiJoystick.gameObject.SetActive(true);
				ShowSkillEnableUI(true);
			
				UIPassiveEffect.UIShow(!UIPassiveEffect.Visible);
				UIGamePause.UIShow(false);
			}
			break;
		case EUISituation.Finish:
			Time.timeScale = 1;
			viewBottomRight.SetActive(false);
			viewTopLeft.SetActive(false);
			uiScoreBar.SetActive(false);
			uiJoystick.Joystick.isActivated = false;
			uiJoystick.gameObject.SetActive(false);
			showViewForceBar(false);
			GameController.Get.IsStart = false;
			CameraMgr.Get.FinishGame();

			break;
		case EUISituation.Reset:
			UIShow(false);
			InitData ();
			GameController.Get.Reset();
			UIPassiveEffect.Get.Reset();
			CourtMgr.Get.SetScoreboards (0, Scores [0]);
			CourtMgr.Get.SetScoreboards (1, Scores [1]);
			
			SetPassButton();
			viewStart.SetActive (true);
			viewBottomRight.SetActive(false);
			uiJoystick.gameObject.SetActive(false);

			dcCount = 0;
			if(IsPlayerMe && PlayerMe.Attribute.ActiveSkills.Count > 0) {
				for (int i=0; i<PlayerMe.Attribute.ActiveSkills.Count; i++) {
					spriteSkills[i].fillAmount = 0;
					uiButtonSkill[i].SetActive(false);
					uiSkillEnables[i].SetActive(false);
				}
			}

			CameraMgr.Get.InitCamera(ECameraSituation.JumpBall);
			CameraMgr.Get.PlayGameStartCamera ();
			UIState(EUISituation.Opening);
			break;
		case EUISituation.ReSelect:
			Time.timeScale = 1;
			UIGameResult.UIShow(false);
			SceneMgr.Get.ChangeLevel (ESceneName.SelectRole);
			break;
		}
		AudioMgr.Get.PauseGame();
	}

	public void UIEffectState(EUIControl effect){
		switch(effect){
		case EUIControl.Attack:
			buttonAttackFXTime = fxTime;
			buttonAttackFX.SetActive(true);
			break;
		case EUIControl.Block:
			buttonBlockFXTime = fxTime;
			buttonBlockFX.SetActive(true);
			break;
		case EUIControl.Shoot:
			buttonShootFXTime = fxTime;
			buttonShootFX.SetActive(true);
			break;
		case EUIControl.Steal:
			buttonStealFXTime = fxTime;
			buttonStealFX.SetActive(true);
			break;
		}
	}
	
	private void runForceValue () {
		if(IsPlayerMe) {
			timeForce += Time.fixedDeltaTime;
			if(newForceValue > oldForceValue) 
				spriteForceFirst.fillAmount = Mathf.Lerp(oldForceValue, newForceValue, timeForce);
			else {
				spriteForce.fillAmount = newForceValue;
				spriteForceFirst.fillAmount = newForceValue;
			}
		}
	}

	private void showViewForceBar (bool isShow){
		if(!IsPlayerMe) {
			viewForceBar.SetActive(false);
			showUIButtonSkill(false);
		} else {
			if(PlayerMe.Attribute.ActiveSkills.Count > 0) {
				viewForceBar.SetActive(isShow);
				showUIButtonSkill(isShow);
			} else {
				viewForceBar.SetActive(false);
				showUIButtonSkill(false);
			}
		}
	}

	private void showUIButtonSkill (bool isShow) {
		if(IsPlayerMe)
			for(int i=0; i<PlayerMe.Attribute.ActiveSkills.Count; i++)
				uiButtonSkill[i].SetActive(((i<PlayerMe.Attribute.ActiveSkills.Count) && isShow));
	}
	
	private void showSkillRefUI (bool isShow) {
		if(IsPlayerMe) {
			if(PlayerMe.Attribute.ActiveSkills.Count > 0)
				viewForceBar.SetActive(isShow);
			else 
				viewForceBar.SetActive(false);
		}
	}
	
	private void resetScoreRotate() {
		for(int i=0; i<labelScores.Length; i++) {
			rotate[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
			rotate[i].enabled = false; 
		}
	}

	private void showScoreBar(bool isStart){
		if(isStart)
			showScoreBarTime = showScoreBarInitTime;
		isShowScoreBar = true;
		uiScoreBar.SetActive(false);
		uiScoreBar.SetActive(true);
	}
	
	private void judgePlayerScreenPosition(){
		if(GameController.Get.IsStart && IsPlayerMe && 
		   (GameController.Get.Situation == EGameSituation.AttackGamer || GameController.Get.Situation == EGameSituation.AttackNPC)){
			playerInCameraX = CameraMgr.Get.CourtCamera.WorldToScreenPoint(PlayerMe.PlayerRefGameObject.transform.position).x;
			playerInCameraY = CameraMgr.Get.CourtCamera.WorldToScreenPoint(PlayerMe.PlayerRefGameObject.transform.position).y;
			
			playerInBoardX = PlayerMe.PlayerRefGameObject.transform.position.z;
			playerInBoardY = PlayerMe.PlayerRefGameObject.transform.position.x;
			
			playerX = 15 - playerInBoardX;
			playerY = 11 - playerInBoardY;
			
			playerScreenPos = new Vector2((playerX * baseValueX) - 640 , (playerY * baseValueY) * (-1));
			if (playerScreenPos.y > -330 && playerScreenPos.y < 330 && playerInCameraX < 0) playerScreenPos.x = -610;
			else if (playerScreenPos.y > -330 && playerScreenPos.y < 330 && playerInCameraX >= Screen.width) playerScreenPos.x = 610;
			else if (playerScreenPos.x > 610) playerScreenPos.x = 610;
			else if (playerScreenPos.x < -610) playerScreenPos.x = -610;
			
			if (playerScreenPos.x > -610 && playerScreenPos.x < 610 && playerInCameraY < 0) playerScreenPos.y = -330;
			else if (playerScreenPos.y < -330) playerScreenPos.y = -330;
			
			float angle = 0f;
			
			if (playerScreenPos.x == -610 && playerScreenPos.y == -330) angle = -135;
			else if (playerScreenPos.x == 610 && playerScreenPos.y == -330) angle = -45;
			else if (playerScreenPos.x == 610) angle = 0;
			else if (playerScreenPos.x == -610) angle = 180;
			else if (playerScreenPos.y == -330) angle = -90;

			if(playerInCameraX > -50 &&
			   playerInCameraX < Screen.width + 100 &&
			   playerInCameraY > -90 &&
			   playerInCameraY < Screen.height + 100) {
				uiPlayerLocation.SetActive(false);
			} else {
				uiPlayerLocation.SetActive(true);
				uiPlayerLocation.transform.localPosition = new Vector3(playerScreenPos.x, playerScreenPos.y, 0);
				uiPlayerLocation.transform.localEulerAngles = new Vector3(0, 0, angle);
			}
		} else 
			uiPlayerLocation.SetActive(false);
	}

	private void showButtonFX(){
		if(buttonShootFXTime > 0) {
			buttonShootFXTime -= Time.deltaTime;
			if(buttonShootFXTime <= 0) {
				buttonShootFXTime = 0;
				buttonShootFX.SetActive(false);
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

	public bool IsPlayerMe {
		get {return (PlayerMe != null);}
	}

	public bool isStage
    {
//		get {return GameData.DStageData.ContainsKey(GameData.StageID); }
		get {return StageTable.Ins.HasByID(GameData.StageID); }
	}
}
