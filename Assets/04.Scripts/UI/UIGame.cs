﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum EUISituation{
	Start, 
	Pause, 
	Continue, 
	Finish, 
	Reset,
	ReSelect,
	MainMenu,
	EffectSwitch,
	OptionSelect,
	MusicSwitch
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

	private bool isPressElbowBtn = true;
	private bool isCanDefenceBtnPress = true;
	private bool isPressShootBtn = false;
	private bool isShowScoreBar = false;
	private bool isShootAvailable = true;
	private bool isShowOption = false;
	private bool isMusicOn = false;

	// GoldFinger
	private bool isPressA = false;
	private bool isPressB = false;

	private GameJoystick uiJoystick = null;
	//Stuff
	private GameObject viewStart;
	private GameObject viewTools;
	private GameObject viewOption;
	private GameObject viewPass;
	private GameObject viewPause;
	private GameObject viewBottomRight;

	private GameObject uiScoreBar;
	private GameObject buttonSkill;
	private GameObject uiSkillEnable;
	private GameObject uiSkill;
	private GameObject uiAttackPush;
	private GameObject uiPlayerLocation;
	private GameObject uiShoot;

	private GameObject uiPassA;
	private GameObject uiPassB;
	private GameObject uiAlleyoopA;
	private GameObject uiAlleyoopB;

	//Force
	private UISprite spriteForce;
	private UISprite spriteForceFirst;
	private float oldForceValue;
	private float newForceValue;
	private float timeForce;
	private GameObject uiSpriteFull;
	private GameObject uiSpriteAnimation;
	private UISpriteAnimation spriteAnimation;

	private GameObject[] uiDefenceGroup = new GameObject[2];
	private GameObject[] controlButtonGroup= new GameObject[2];
	private GameObject[] uiPassObjectGroup = new GameObject[3];
	private GameObject[] effectGroup = new GameObject[2];
	private GameObject[] musicGroup = new GameObject[2];

	private UILabel[] labelScores = new UILabel[2];
	private UIScrollBar aiLevelScrollBar;

	private DrawLine drawLine;

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
		
		if(isPressA && isPressB)
			GameController.Get.Joysticker.SetAnger(GameController.Get.Joysticker.Attribute.MaxAnger);

		runForceValue ();
		if (isPressShootBtn && shootBtnTime > 0) {
			shootBtnTime -= Time.deltaTime;
			if(shootBtnTime <= 0){
				isPressShootBtn = false;
				if (GameController.Get.BallOwner == GameController.Get.Joysticker) {
					GameController.Get.DoShoot(true);
					GameController.Get.Joysticker.SetNoAiTime();
					uiAttackPush.SetActive(false);
					ShowSkillUI(false);

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
	}

	protected override void InitCom() {
		uiJoystick = GameObject.Find (UIName + "/GameJoystick").GetComponent<GameJoystick>();
		uiJoystick.Joystick = GameObject.Find (UIName + "/GameJoystick").GetComponent<EasyJoystick>();

		viewStart = GameObject.Find (UIName + "/Center/ViewStart");
		viewTools = GameObject.Find (UIName + "/TopRight/ViewTools");
		viewOption = GameObject.Find (UIName + "TopRight/ViewTools/ViewOption");
		viewPause = GameObject.Find (UIName + "/Center/ViewPause");
		viewPass = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass");
		viewBottomRight = GameObject.Find(UIName + "/BottomRight");

		uiScoreBar = GameObject.Find (UIName + "/Bottom/UIScoreBar");
		buttonSkill = GameObject.Find(UIName + "/Bottom/ViewForceBar/ButtonSkill");
		uiSkillEnable = GameObject.Find(UIName + "/Bottom/ViewForceBar/ButtonSkill/SpriteFull");
		uiSkill = GameObject.Find(UIName + "/Bottom/ViewForceBar");
		uiPlayerLocation = GameObject.Find (UIName + "/Right");

		uiAttackPush = GameObject.Find(UIName + "/BottomRight/ButtonAttack/SpriteAttack");
		uiDefenceGroup[0] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal/SpriteSteal");
		uiDefenceGroup[1] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonBlock/SpriteBlock");

		uiShoot = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonShoot/SpriteShoot");
		uiPassObjectGroup [0] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonPass/SpriteMe");
		uiPassObjectGroup [1] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA/SpriteA");
		uiPassObjectGroup [2] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB/SpriteB");
		uiPassA = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA");
		uiPassB = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB");
		uiAlleyoopA = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/AlleyoopA");
		uiAlleyoopB = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/AlleyoopB");

		controlButtonGroup [0] = GameObject.Find (UIName + "/BottomRight/ViewAttack");
		controlButtonGroup [1] = GameObject.Find (UIName + "/BottomRight/ViewDefance");

		labelScores [0] = GameObject.Find (UIName + "/Bottom/UIScoreBar/LabelScore1").GetComponent<UILabel>();
		labelScores [1] = GameObject.Find (UIName + "/Bottom/UIScoreBar/LabelScore2").GetComponent<UILabel>();

		aiLevelScrollBar = GameObject.Find(UIName + "/Center/ViewStart/AISelect/AIControlScrollBar").GetComponent<UIScrollBar>();

		effectGroup[0] = GameObject.Find (UIName + "/TopRight/ViewTools/ViewOption/ButtonEffect/LabelON");
		effectGroup[1] = GameObject.Find (UIName + "/TopRight/ViewTools/ViewOption/ButtonEffect/LabelOff");
		effectGroup [0].SetActive (GameData.Setting.Effect);
		effectGroup [1].SetActive (!GameData.Setting.Effect);

		musicGroup[0] = GameObject.Find (UIName + "/TopRight/ViewTools/ViewOption/ButtonMusic/LabelON");
		musicGroup[1] = GameObject.Find (UIName + "/TopRight/ViewTools/ViewOption/ButtonMusic/LabelOff");
		musicGroup[0].SetActive(AudioMgr.Get.IsMusicOn);
		musicGroup[1].SetActive(!AudioMgr.Get.IsMusicOn);
		isMusicOn = AudioMgr.Get.IsMusicOn;

		spriteForce = GameObject.Find (UIName + "/Bottom/ViewForceBar/Forcebar/SpriteForce").GetComponent<UISprite>();
		spriteForceFirst = GameObject.Find (UIName + "/Bottom/ViewForceBar/Forcebar/SpriteForceFrist").GetComponent<UISprite>();
		uiSpriteFull = GameObject.Find (UIName + "/Bottom/ViewForceBar/Forcebar/SpriteFullTween");
		uiSpriteAnimation = GameObject.Find (UIName + "/Bottom/ViewForceBar/Forcebar/forcebar");
		spriteAnimation = GameObject.Find (UIName + "/Bottom/ViewForceBar/Forcebar/forcebar").GetComponent<UISpriteAnimation>();
		spriteAnimation.framesPerSecond = 25;
		uiSpriteFull.SetActive(false);
		spriteForce.fillAmount = 0;
		spriteForceFirst.fillAmount = 0;

		buttonShootFX = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonShoot/UI_FX_A_21");
		buttonBlockFX = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonBlock/UI_FX_A_21");
		buttonStealFX = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal/UI_FX_A_21");
		buttonAttackFX = GameObject.Find(UIName + "/BottomRight/ButtonAttack/UI_FX_A_21");
		buttonPassFX = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonPass/UI_FX_A_21");
		buttonPassAFX = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA/UI_FX_A_21");
		buttonPassBFX = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB/UI_FX_A_21");
		buttonShootFX.SetActive(false);
		buttonBlockFX.SetActive(false);
		buttonStealFX.SetActive(false);
		buttonAttackFX.SetActive(false);
		buttonPassFX.SetActive(false);
		buttonPassAFX.SetActive(false);
		buttonPassBFX.SetActive(false);

		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewAttack/ButtonShoot")).onPress = DoShoot;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonPass")).onPress = DoPassChoose;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA")).onPress = DoPassTeammateA;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB")).onPress = DoPassTeammateB;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/AlleyoopA")).onPress = DoPassTeammateA;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/AlleyoopB")).onPress = DoPassTeammateB;

		aiLevelScrollBar.onChange.Add(new EventDelegate(changeAIChangeTime));

		SetBtnFun (UIName + "/TopLeft/ButtonPause", PauseGame);
		SetBtnFun (UIName + "/Center/ViewStart/ButtonStart", StartGame);
		SetBtnFun (UIName + "/TopRight/ViewTools/ButtonOption", OptionSelect);
		SetBtnFun (UIName + "/TopRight/ViewTools/ViewOption/ButtonMusic", MusicSwitch);
		SetBtnFun (UIName + "/TopRight/ViewTools/ViewOption/ButtonMainMenu", BackMainMenu);
		SetBtnFun (UIName + "/TopRight/ViewTools/ViewOption/ButtonEffect", EffectSwitch);
		SetBtnFun (UIName + "/BottomRight/ViewDefance/ButtonSteal", DoSteal);
		SetBtnFun (UIName + "/BottomRight/ViewDefance/ButtonBlock", DoBlock);
		SetBtnFun (UIName + "/BottomRight/ButtonAttack", DoAttack);
		SetBtnFun (UIName + "/Bottom/ViewForceBar/ButtonSkill", DoSkill);

		viewTools.SetActive(false);
		viewOption.SetActive(false);
		viewPause.SetActive(false);

		uiAlleyoopA.SetActive(false);
		uiAlleyoopB.SetActive(false);
		uiPlayerLocation.SetActive(false);
		ShowSkillUI(false);

		ChangeControl(true);
		runForceBar ();

		uiJoystick.Joystick.isActivated = false;
		uiJoystick.Joystick.DynamicJoystick = false;
		uiJoystick.Joystick.JoystickPositionOffset = new Vector2(200, 545);

		drawLine = gameObject.AddComponent<DrawLine>();
	}

	protected override void InitData() {
		MaxScores[0] = 13;
		MaxScores[1] = 13;
		Scores [0] = 0;
		Scores [1] = 0;
		labelScores[0].text = "0";
        labelScores[1].text = "0";
    }

	protected override void InitText(){

	}

	public void InitLine() {
		drawLine.ClearTarget();
		if (drawLine.UIs.Length == 0) {
			GameObject obj = GameObject.Find("PlayerInfoModel/Self1/PassA");
			if (obj)
				drawLine.AddTarget(uiPassObjectGroup[1], obj);
			obj = GameObject.Find("PlayerInfoModel/Self2/PassB");
			if (obj)
				drawLine.AddTarget(uiPassObjectGroup[2], obj);
		}
		drawLine.Show(true);
	}

	public void changeAIChangeTime(){
		int level = (int)  Mathf.Round(aiLevelScrollBar.value * 5);
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
	
	public void DoAttack(){
		UIControllerState(EUIControl.Attack);
	}

	//Defence
	public void DoBlock() {
		UIControllerState(EUIControl.Block);
	}

	public void DoSteal(){
		UIControllerState(EUIControl.Steal);
	}
	
	//Attack
	public void DoSkill(){
		UIControllerState(EUIControl.Skill);
	}
	
	public void DoShoot(GameObject go, bool state) {
		UIControllerState(EUIControl.Shoot, go, state);
	}
	
	public void DoPassChoose (GameObject obj, bool state) {
		UIControllerState(EUIControl.Pass, obj, state);
	}

	public void DoPassTeammateA(GameObject obj, bool state) {
		isPressA = state;
		UIControllerState(EUIControl.PassA, obj, state);
	}

	public void DoPassTeammateB(GameObject obj, bool state) {
		isPressB = state;
		UIControllerState(EUIControl.PassB, obj, state);
	}

	public void DoPassNone() {
		SetPassButton();
	}
	
	public void BackMainMenu() {
		UIState(EUISituation.MainMenu);
	}

	public void OnReselect() {
		UIState(EUISituation.ReSelect);
	}

	public void PauseGame(){
		if(Time.timeScale == 0) 
			UIState(EUISituation.Continue);
		else
			UIState(EUISituation.Pause);
	}

	public void ResetGame() {
		UIState(EUISituation.Reset);
	}

	public void StartGame() {
		UIState(EUISituation.Start);
	}

	public void GameOver(){
		UIState(EUISituation.Finish);
	}

	public void EffectSwitch(){
		UIState(EUISituation.EffectSwitch);
	}

	public void OptionSelect(){
		UIState(EUISituation.OptionSelect);
	}

	public void MusicSwitch(){
		UIState(EUISituation.MusicSwitch);
	}

	public void ShowSkillUI (bool isShow, bool angerFull = false, bool canUse = false){
		buttonSkill.SetActive(isShow);
		if (isShow) {
			buttonSkill.SetActive(angerFull);
			uiSkillEnable.SetActive(canUse);
		}
	}
	
	public void ShowAlleyoop(bool isShow, int teammate = 1) {
		if(GameController.Get.Situation == EGameSituation.AttackA) {
			if(isShow) {
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
	}

	public void SetAngerUI (float max, float anger){
		timeForce = 0;
		uiSpriteFull.SetActive (false);
		uiSkill.SetActive(true);

		if (max > 0) {
			oldForceValue = spriteForce.fillAmount;
			newForceValue = anger / max;
			spriteForceFirst.fillAmount = newForceValue;
			if (newForceValue >= 1)
				uiSpriteFull.SetActive (true);
		} else
			buttonSkill.SetActive(false);
	}

	private void runForceValue () {
		timeForce += Time.fixedDeltaTime;
		if(oldForceValue != newForceValue) 
			spriteForce.fillAmount = Mathf.Lerp(oldForceValue, newForceValue, timeForce);
	}

	private void runForceBar () {
		float endValue = 0;
		if(spriteForce.fillAmount < 0.25f)
			uiSpriteAnimation.SetActive(false);
		else 
		if(spriteForce.fillAmount >=0.25f && spriteForce.fillAmount < 0.35f) 
			endValue = 0;
		else 
		if(spriteForce.fillAmount >= 0.35f && spriteForce.fillAmount < 0.45f) 
			endValue = 30;
		else 
		if(spriteForce.fillAmount >= 0.45f && spriteForce.fillAmount < 0.55f) 
			endValue = 60;
		else 
		if(spriteForce.fillAmount >= 0.55f && spriteForce.fillAmount < 0.65f) 
			endValue = 90;
		else 
		if(spriteForce.fillAmount >= 0.65f && spriteForce.fillAmount < 0.75f) 
			endValue = 120;
		else 
		if(spriteForce.fillAmount >= 0.75f && spriteForce.fillAmount < 0.85f) 
			endValue = 150;
		else 
		if(spriteForce.fillAmount >= 0.85f && spriteForce.fillAmount < 0.95f) 
			endValue = 180;
		else 
		if(spriteForce.fillAmount >= 0.95f) 
			endValue = 210;

		uiSpriteAnimation.transform.DOLocalMoveX(endValue, 1f).OnStepComplete(resetAnimation).SetEase(Ease.Linear);
	}

	private void resetAnimation (){
		uiSpriteAnimation.SetActive(true);
		uiSpriteAnimation.transform.localPosition = new Vector3(-25, 0, 0);
		spriteAnimation.ResetToBeginning();
		runForceBar ();
	}

	public void PlusScore(int team, int score) {
		Scores [team] += score;
		CourtMgr.Get.SetScoreboards (team, Scores [team]);
		showScoreBar(GameController.Get.IsStart);
		TweenRotation tweenRotation = TweenRotation.Begin(labelScores[team].gameObject, 0.5f, Quaternion.identity);
		tweenRotation.delay = 0.5f;
		tweenRotation.to = new Vector3(0,720,0);
		labelScores[team].text = Scores [team].ToString ();
	}

	public void ChangeControl(bool IsAttack) {
		if(IsAttack) 
			UIMaskState(EUIControl.AttackA);
		else 
			UIMaskState(EUIControl.AttackB);
	}

	public bool UICantUse(PlayerBehaviour p = null) {
		if(p == GameController.Get.Joysticker) {
			if (GameController.Get.IsStart) {
				SetPassButton();
				ShowSkillUI(false);
				uiAttackPush.SetActive(true);
			}
			return true;
		} else {
			if (p.Team == GameController.Get.Joysticker.Team && p.crtState == EPlayerState.Alleyoop)
				SetPassButton();
			
			return false;
		}
	}

	public bool OpenUIMask(PlayerBehaviour p = null){
		if(p == GameController.Get.Joysticker) {
			if(p.NoAiTime > 0)
				p.SetNoAiTime();

			if (GameController.Get.IsStart) { 
				ShowAlleyoop(false);
				
				if(GameController.Get.Situation == EGameSituation.AttackA) 
					UIMaskState(EUIControl.AttackA);
				else 
					UIMaskState(EUIControl.AttackB);
			}

			return true;
		} else {
			if (p.Team == GameController.Get.Joysticker.Team && p.crtState == EPlayerState.Alleyoop)
				ShowAlleyoop(false);

			return false;
		}
	}

	public void SetPassButton() {
		if(GameStart.Get.TestMode != EGameTest.None) 
			return;

		int who = GameController.Get.GetBallOwner;
		switch (who) {
		case (int)EUIPassType.MeBallOwner:
			viewPass.SetActive(true);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(true);
			uiPassObjectGroup[2].SetActive(true);
			GameController.Get.passIcon[0].SetActive(false);
			GameController.Get.passIcon[1].SetActive(true);
			GameController.Get.passIcon[2].SetActive(true);
			break;
		case (int)EUIPassType.ABallOwner:
			viewPass.SetActive(true);
			uiPassObjectGroup[0].SetActive(true);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(true);
			GameController.Get.passIcon[0].SetActive(true);
			GameController.Get.passIcon[1].SetActive(false);
			GameController.Get.passIcon[2].SetActive(true);
			break;
		case (int)EUIPassType.BBallOwner:
			viewPass.SetActive(true);
			uiPassObjectGroup[0].SetActive(true);
			uiPassObjectGroup[1].SetActive(true);
			uiPassObjectGroup[2].SetActive(false);
			GameController.Get.passIcon[0].SetActive(true);
			GameController.Get.passIcon[1].SetActive(true);
			GameController.Get.passIcon[2].SetActive(false);
			break;
		default:
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			if(GameController.Get.Situation == EGameSituation.AttackB || GameController.Get.Situation == EGameSituation.TeeBPicking) {
				viewPass.SetActive(false);
				GameController.Get.passIcon[0].SetActive(false);
				GameController.Get.passIcon[1].SetActive(false);
				GameController.Get.passIcon[2].SetActive(false);
			} else {
				viewPass.SetActive(true);
				if(GameController.Get.passIcon[0] != null) {
					GameController.Get.passIcon[0].SetActive(true);
					GameController.Get.passIcon[1].SetActive(true);
					GameController.Get.passIcon[2].SetActive(true);
				}
			}
			break;
		}
//			GameController.Get.SetBodyMaterial(kind);
	}

	public void UIMaskState (EUIControl controllerState) {
		switch (controllerState) {
		case EUIControl.Skill:
			ShowSkillUI(true);
			uiAttackPush.SetActive(false);
			uiDefenceGroup[0].SetActive(false);
			uiDefenceGroup[1].SetActive(false);
			uiShoot.SetActive(false);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case EUIControl.Attack:
			if(GameController.Get.Joysticker.IsBallOwner) {
				//Elbow Attack
				UIEffectState(EUIControl.Attack);

				uiShoot.SetActive(false);
				uiAttackPush.SetActive(true);
				uiDefenceGroup[0].SetActive(false);
				uiDefenceGroup[1].SetActive(false);
				uiPassObjectGroup[0].SetActive(false);
				uiPassObjectGroup[1].SetActive(false);
				uiPassObjectGroup[2].SetActive(false);
			} else {
				//Push Deffence
				UIEffectState(EUIControl.Attack);

				ShowSkillUI(false);
				uiShoot.SetActive(false);
				uiAttackPush.SetActive(true);
				uiDefenceGroup[0].SetActive(false);
				uiDefenceGroup[1].SetActive(false);
				uiPassObjectGroup[0].SetActive(false);
				uiPassObjectGroup[1].SetActive(false);
				uiPassObjectGroup[2].SetActive(false);
			}
			break;
		case EUIControl.Block:
			UIEffectState(EUIControl.Block);

			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiDefenceGroup[0].SetActive(false);
			uiDefenceGroup[1].SetActive(true);
			break;
		case EUIControl.Steal:
			UIEffectState(EUIControl.Steal);

			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiDefenceGroup[0].SetActive(true);
			uiDefenceGroup[1].SetActive(false);
			break;
		case EUIControl.Shoot:
			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiShoot.SetActive(true);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case EUIControl.Pass:
			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiShoot.SetActive(false);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case EUIControl.PassA:
			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiShoot.SetActive(false);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case EUIControl.PassB:
			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
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
			uiAttackPush.SetActive(true);
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
			uiAttackPush.SetActive(true);
			uiDefenceGroup[0].SetActive(true);
			uiDefenceGroup[1].SetActive(true);
			SetPassButton();
			break;
		}
	}

	public void UIControllerState (EUIControl controllerState, GameObject go = null, bool state = false) {
		switch(controllerState) {
		case EUIControl.Skill:
			UIMaskState(EUIControl.Skill);
			GameController.Get.OnSkill();
			break;

		case EUIControl.Attack:
			if(GameController.Get.Joysticker.IsBallOwner) {
				//Elbow
				if(isPressElbowBtn && 
				   !GameController.Get.Joysticker.IsFall && 
				   GameController.Get.Situation == EGameSituation.AttackA &&
				   GameController.Get.Joysticker.CanUseState(EPlayerState.Elbow)) {
					UIMaskState(EUIControl.Attack);
					GameController.Get.DoElbow ();
					GameController.Get.Joysticker.SetNoAiTime();
				}
			} else {
				//Push
				if(isCanDefenceBtnPress && 
				   !GameController.Get.Joysticker.IsFall &&
				   (GameController.Get.Situation == EGameSituation.AttackB || GameController.Get.Situation == EGameSituation.AttackA) &&
				   GameController.Get.Joysticker.CanUseState(EPlayerState.Push)) {
					UIMaskState(EUIControl.Attack);
					GameController.Get.DoPush();
					GameController.Get.Joysticker.SetNoAiTime();
				}
			}
			break;
		case EUIControl.Block:
			if(isCanDefenceBtnPress && 
			   !GameController.Get.Joysticker.IsFall && 
			   GameController.Get.Situation == EGameSituation.AttackB &&
			   GameController.Get.Joysticker.CanUseState(EPlayerState.Block)) {
				UIMaskState(EUIControl.Block);
				GameController.Get.DoBlock();
				GameController.Get.Joysticker.SetNoAiTime();
			}
			break;
		case EUIControl.Steal:
			if(isCanDefenceBtnPress && 
			   !GameController.Get.Joysticker.IsFall &&
			   GameController.Get.Situation == EGameSituation.AttackB && 
			   GameController.Get.StealBtnLiftTime <= 0 && 
			   GameController.Get.Joysticker.CanUseState(EPlayerState.Steal)) {
				UIMaskState(EUIControl.Steal);
				GameController.Get.DoSteal();
				GameController.Get.Joysticker.SetNoAiTime();
			}
			break;
		case EUIControl.Shoot:
			if(GameController.Get.IsShooting) {
				if(state && UIDoubleClick.Visible){
					UIDoubleClick.Get.ClickStop ();
				}
			} else {
				if(GameController.Get.Joysticker.IsBallOwner &&
				   GameController.Get.Situation == EGameSituation.AttackA && 
				   !GameController.Get.Joysticker.IsFall && 
				   !GameController.Get.Joysticker.CheckAnimatorSate(EPlayerState.MoveDodge0) && 
				   !GameController.Get.Joysticker.CheckAnimatorSate(EPlayerState.MoveDodge1) && 
				   !GameController.Get.Joysticker.CheckAnimatorSate(EPlayerState.Block)
				   ) {
					if(state && GameController.Get.Joysticker.IsFakeShoot && isShootAvailable) {
						isShootAvailable = false;
					}
					if(state)
						UIEffectState(EUIControl.Shoot);
					else 
					if(!state && shootBtnTime > 0 && isShootAvailable){
						if(GameController.Get.BallOwner != null) {
							if(GameController.Get.Joysticker.IsBallOwner) {
								UIMaskState(EUIControl.Shoot);
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
			break;
		case EUIControl.Pass:
//			if(GameController.Get.Joysticker.IsBallOwner && 
//			   !GameController.Get.Joysticker.IsFall && 
//			   GameController.Get.situation == GameSituation.AttackA) {
//				if(!GameController.Get.IsCanPassAir){
//					if (state) 
//						SetPassButton(4);
//				}
//			} else {
//				GameController.Get.DoPass(0);
//				GameController.Get.Joysticker.SetNoAiTime();
//			}
//			
//			if(!state)
//				SetPassButton(0);
			if(!GameController.Get.Joysticker.IsBallOwner) {
				UIMaskState(EUIControl.Pass);
				if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(0)){
					GameController.Get.Joysticker.SetNoAiTime();
				}
			}
			break;
		case EUIControl.PassA:
			if(GameController.Get.GetBallOwner != 1) {
				UIMaskState(EUIControl.PassA);
				if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(1)){
					GameController.Get.Joysticker.SetNoAiTime();
				}
			}
			break;
		case EUIControl.PassB:
			if(GameController.Get.GetBallOwner != 2) {
				UIMaskState(EUIControl.PassB);
				if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(2)){
					GameController.Get.Joysticker.SetNoAiTime();
				}
			}
			break;
		}
	}

	public void UIState(EUISituation situation){
		switch(situation) {
		case EUISituation.Start:
			viewStart.SetActive (false);
			uiScoreBar.SetActive (false);
			uiJoystick.Joystick.isActivated = true;

			uiJoystick.gameObject.SetActive(true);
			viewPass.SetActive(GameController.Get.Situation == EGameSituation.AttackA);
			controlButtonGroup[0].SetActive(GameController.Get.Situation == EGameSituation.AttackA);
			controlButtonGroup[1].SetActive(GameController.Get.Situation != EGameSituation.AttackA);
			
			CourtMgr.Get.SetBallState (EPlayerState.Start);
			GameController.Get.StartGame();
			drawLine.IsShow = false;
			break;
		case EUISituation.Pause:
			if (!viewStart.activeInHierarchy) {
				Time.timeScale = 0;
				viewTools.SetActive(true);
				viewPause.SetActive(true);
				viewBottomRight.SetActive(false);
				uiSkill.SetActive(false);

				uiScoreBar.SetActive(false);
				uiJoystick.gameObject.SetActive(false);

				GameController.Get.SetGameRecord(false);
				UIGameResult.Get.SetGameRecord(ref GameController.Get.GameRecord);
			}
			break;
		case EUISituation.Continue:
			if (GameController.Get.IsStart) {
				Time.timeScale = 1;
				isShowOption = false;
				viewTools.SetActive(false);
				viewPause.SetActive(false);
				viewOption.SetActive(false);
				viewBottomRight.SetActive(true);
				uiSkill.SetActive(true);
				uiScoreBar.SetActive(false);
				uiJoystick.gameObject.SetActive(true);
				UIGameResult.UIShow(false);
			}
			break;
		case EUISituation.Finish:
			viewBottomRight.SetActive(false);
			uiScoreBar.SetActive(false);
			uiJoystick.Joystick.isActivated = false;
			uiJoystick.gameObject.SetActive(false);
			uiSkill.SetActive(false);
			break;
		case EUISituation.Reset:
			GameController.Get.Reset ();
			InitData ();
			CourtMgr.Get.SetScoreboards (0, Scores [0]);
			CourtMgr.Get.SetScoreboards (1, Scores [1]);
			drawLine.IsShow = true;
			isShowOption = false;
			isShowScoreBar = false;
			
			viewStart.SetActive (true);
			viewTools.SetActive(false);
			viewPause.SetActive(false);
			viewBottomRight.SetActive(true);

			uiScoreBar.SetActive(true);
			uiJoystick.Joystick.isActivated = false;

			uiJoystick.gameObject.SetActive(true);
			buttonSkill.SetActive(true);
			ChangeControl(true);
			SetPassButton();

			CameraMgr.Get.InitCamera(ETeamKind.JumpBall);
			break;
		case EUISituation.ReSelect:
			Time.timeScale = 1;
			UIGameResult.UIShow(false);
			SceneMgr.Get.ChangeLevel (SceneName.SelectRole);
			break;
		case EUISituation.MainMenu:
			Time.timeScale = 1;
			UIGameResult.UIShow(false);
			SceneMgr.Get.ChangeLevel(SceneName.Lobby);
			break;
		case EUISituation.EffectSwitch:
			GameData.Setting.Effect = !GameData.Setting.Effect;
			effectGroup [0].SetActive (GameData.Setting.Effect);
			effectGroup [1].SetActive (!GameData.Setting.Effect);
			
			int index = 0;
			
			if (GameData.Setting.Effect)
				index = 1;
			
			CourtMgr.Get.EffectEnable (GameData.Setting.Effect);
			
			PlayerPrefs.SetInt (SettingText.Effect, index);
			PlayerPrefs.Save ();
			break;
		case EUISituation.OptionSelect:
			isShowOption = !isShowOption;
			viewOption.SetActive(isShowOption);
			break;
		case EUISituation.MusicSwitch:
			isMusicOn = !isMusicOn;
			AudioMgr.Get.MusicOn(isMusicOn);
			musicGroup[0].SetActive(isMusicOn);
			musicGroup[1].SetActive(!isMusicOn);
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

	private void showScoreBar(bool isFinish){
		if(!isFinish)
			showScoreBarTime = showScoreBarInitTime;
		isShowScoreBar = true;
		uiScoreBar.SetActive(true);
	}
	
	private void judgePlayerScreenPosition(){
		if(GameController.Get.IsStart && GameController.Get.Joysticker != null){
			float playerInCameraX = CameraMgr.Get.CourtCamera.WorldToScreenPoint(GameController.Get.Joysticker.gameObject.transform.position).x;
			float playerInCameraY = CameraMgr.Get.CourtCamera.WorldToScreenPoint(GameController.Get.Joysticker.gameObject.transform.position).y;
			
			float playerInBoardX = GameController.Get.Joysticker.gameObject.transform.position.z;
			float playerInBoardY = GameController.Get.Joysticker.gameObject.transform.position.x;
			
			float baseValueX = 37.65f; 
			float baseValueY = 13.37f;
			
			float playerX = 15 - playerInBoardX;
			float playerY = 11 - playerInBoardY;
			
			Vector2 playerScreenPos = new Vector2((playerX * baseValueX) - 640 , (playerY * baseValueY) * (-1));
			if(playerScreenPos.y > -330 && playerScreenPos.y < 330 && playerInCameraX < 0) {
				playerScreenPos.x = -610;
			} else 
			if(playerScreenPos.y > -330 && playerScreenPos.y < 330 && playerInCameraX >= Screen.width) {
				playerScreenPos.x = 610;
			} else 
			if(playerScreenPos.x > 610) {
				playerScreenPos.x = 610;
			} else 
			if(playerScreenPos.x < -610){
				playerScreenPos.x = -610;
			}
			
			if(playerScreenPos.x > -610 && playerScreenPos.x < 610 && playerInCameraY < 0) {
				playerScreenPos.y = -330;
			} else 
			if(playerScreenPos.y < -330){
				playerScreenPos.y = -330;
			}
			
			float angle = 0f;
			
			if(playerScreenPos.x == -610 && playerScreenPos.y == -330) {
				angle = -135;
			} else 
			if(playerScreenPos.x == 610 && playerScreenPos.y == -330) {
				angle = -45;
			} else 
			if(playerScreenPos.x == 610) {
				angle = 0;
			} else 
			if(playerScreenPos.x == -610) {
				angle = 180;
			} else
			if(playerScreenPos.y == -330) {
				angle = -90;
			}
			
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
		}
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
}
