using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum UISituation{
	Start, 
	Pause, 
	Continue, 
	Finish, 
	Restart,
	Reset,
	ReSelect,
	MainMenu,
	EffectSwitch,
	OptionSelect
}

public enum UIController {
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

public enum UIPassType {
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

	private bool isAttackState = true;
	private bool isPressElbowBtn = true;
	private bool isCanDefenceBtnPress = true;
	private bool isPressShootBtn = false;
	private bool isShowScoreBar = false;
	private bool isShootAvailable = true;
	private bool isShowOption = false;
	public bool isCanShowSkill = false;

	private GameJoystick uiJoystick = null;
	//Stuff
	private GameObject viewStart;
	private GameObject viewFinish;
	private GameObject viewTools;
	private GameObject viewOption;
	private GameObject viewPass;
	private GameObject viewPause;

	private GameObject uiScoreBar;
	private GameObject uiReselect;
	private GameObject uiContinue;
	private GameObject uiSkill;
	private GameObject uiSkillFull;
	private GameObject uiAttackPush;
	private GameObject uiPlayerLocation;
	private GameObject uiShoot;

	//Force
	private UISprite spriteForce;
	private GameObject uiSpriteFull;
	private GameObject uiSpriteAnimation;
	private UISpriteAnimation spriteAnimation;
	public bool isAngerFull = false;

	private GameObject[] uiDefenceGroup = new GameObject[2];
	private GameObject[] controlButtonGroup= new GameObject[2];
	private GameObject[] uiPassObjectGroup = new GameObject[3];
	private GameObject[] effectGroup = new GameObject[2];

	private UILabel[] labelScores = new UILabel[2];
	private UIScrollBar[] aiLevelScrollBar = new UIScrollBar[3];

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
//		if (Input.GetMouseButtonUp(0)) {
//			isPressShootBtn = false;
//			if(UICamera.hoveredObject.name.Equals("ButtonObjectA")) {
//				DoPassTeammateA();
//			} else if (UICamera.hoveredObject.name.Equals("ButtonObjectB")) {
//				DoPassTeammateB();
//			}
//		}

		if (isPressShootBtn && shootBtnTime > 0) {
			shootBtnTime -= Time.deltaTime;
			if(shootBtnTime <= 0){
				isPressShootBtn = false;
				if (GameController.Get.BallOwner == GameController.Get.Joysticker) {
					GameController.Get.DoShoot(true);
					GameController.Get.Joysticker.SetNoAiTime();
					uiAttackPush.SetActive(false);
					ShowSkillUI(false);
					isCanShowSkill = false;

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
		viewFinish = GameObject.Find (UIName + "/Center/ViewFinish");
		viewTools = GameObject.Find (UIName + "/Center/ViewTools");
		viewOption = GameObject.Find (UIName + "Center/ViewTools/ViewOption");
		viewPause = GameObject.Find (UIName + "/Center/ViewPause");
		viewPass = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass");

		uiReselect = GameObject.Find (UIName + "/Center/ButtonReturnSelect");
		uiContinue = GameObject.Find (UIName + "/TopLeft/ButtonContinue");
		uiScoreBar = GameObject.Find (UIName + "/Top/UIScoreBar");
		uiSkill = GameObject.Find(UIName + "/BottomRight/ButtonSkill");
		uiSkillFull = GameObject.Find(UIName + "/BottomRight/ButtonSkill/SpriteFull");
		uiPlayerLocation = GameObject.Find (UIName + "/Right");
		
		uiAttackPush = GameObject.Find(UIName + "/BottomRight/ButtonAttack/SpriteAttack");
		uiDefenceGroup[0] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal/SpriteSteal");
		uiDefenceGroup[1] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonBlock/SpriteBlock");

		uiShoot = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonShoot/SpriteShoot");
		uiPassObjectGroup [0] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonPass/SpriteMe");
		uiPassObjectGroup [1] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA/SpriteA");
		uiPassObjectGroup [2] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB/SpriteB");

		controlButtonGroup [0] = GameObject.Find (UIName + "/BottomRight/ViewAttack");
		controlButtonGroup [1] = GameObject.Find (UIName + "/BottomRight/ViewDefance");

		labelScores [0] = GameObject.Find (UIName + "/Top/UIScoreBar/LabelScore1").GetComponent<UILabel>();
		labelScores [1] = GameObject.Find (UIName + "/Top/UIScoreBar/LabelScore2").GetComponent<UILabel>();

		aiLevelScrollBar [0] = GameObject.Find(UIName + "/Center/ViewStart/AISelect/HomeScrollBar").GetComponent<UIScrollBar>();
		aiLevelScrollBar [1] = GameObject.Find(UIName + "/Center/ViewStart/AISelect/AwayScrollBar").GetComponent<UIScrollBar>();
		aiLevelScrollBar [2] = GameObject.Find(UIName + "/Center/ViewStart/AISelect/AIControlScrollBar").GetComponent<UIScrollBar>();

		effectGroup[0] = GameObject.Find (UIName + "/Center/ViewTools/ViewOption/ButtonEffect/LabelON");
		effectGroup[1] = GameObject.Find (UIName + "/Center/ViewTools/ViewOption/ButtonEffect/LabelOff");
		effectGroup [0].SetActive (GameData.Setting.Effect);
		effectGroup [1].SetActive (!GameData.Setting.Effect);

		spriteForce = GameObject.Find (UIName + "/Forcebar/SpriteForce").GetComponent<UISprite>();
		uiSpriteFull = GameObject.Find (UIName + "/Forcebar/SpriteFullTween");
		uiSpriteAnimation = GameObject.Find (UIName + "Forcebar/forcebar");
		spriteAnimation = GameObject.Find (UIName + "Forcebar/forcebar").GetComponent<UISpriteAnimation>();
		spriteAnimation.framesPerSecond = 25;
		uiSpriteFull.SetActive(false);
		spriteForce.fillAmount = 0;

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

		aiLevelScrollBar[0].onChange.Add(new EventDelegate(changeSelfAILevel));
		aiLevelScrollBar[1].onChange.Add(new EventDelegate(changeNpcAILevel));
		aiLevelScrollBar[2].onChange.Add(new EventDelegate(changeAIChangeTime));

		SetBtnFun (UIName + "/TopLeft/ButtonPause", PauseGame);
		SetBtnFun (UIName + "/TopLeft/ButtonContinue", ContinueGame);
		SetBtnFun (UIName + "/Center/ViewStart/ButtonStart", StartGame);
		SetBtnFun (UIName + "/Center/ViewFinish/ButtonAgain", ResetGame);
		SetBtnFun (UIName + "/Center/ViewPause/ButtonReset", RestartGame);
		SetBtnFun (UIName + "/Center/ViewTools/ButtonOption", OptionSelect);
		SetBtnFun (UIName + "/Center/ButtonReturnSelect", OnReselect);
		SetBtnFun (UIName + "/Center/ViewTools/ViewOption/ButtonMainMenu", BackMainMenu);
		SetBtnFun (UIName + "/Center/ViewTools/ViewOption/ButtonEffect", EffectSwitch);
		SetBtnFun (UIName + "/BottomRight/ViewDefance/ButtonSteal", DoSteal);
		SetBtnFun (UIName + "/BottomRight/ViewDefance/ButtonBlock", DoBlock);
		SetBtnFun (UIName + "/BottomRight/ButtonAttack", DoAttack);
		SetBtnFun (UIName + "/BottomRight/ButtonSkill", DoSkill);

		viewFinish.SetActive(false);
		viewTools.SetActive(false);
		viewOption.SetActive(false);
		viewPause.SetActive(false);

		uiReselect.SetActive(false);
		uiContinue.SetActive(false);
		uiPlayerLocation.SetActive(false);
		ShowSkillUI(false);
//		SetPassButton();

		ChangeControl(false);
		runForceBar ();

		uiJoystick.Joystick.isActivated = false;
		uiJoystick.Joystick.DynamicJoystick = false;
		uiJoystick.Joystick.JoystickPositionOffset = new Vector2(200, 545);
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
//		SetLabel(UIName + "/Center/ViewFinish/ButtonAgain/LabelReset" ,TextConst.S(1));
//		SetLabel(UIName + "/Center/ViewStart/ButtonStart/LabelStart" ,TextConst.S(2));
//		SetLabel(UIName + "/Center/ViewPause/ButtonReset/LabelReset" ,TextConst.S(4));
		SetLabel(UIName + "/Center/ViewStart/AISelect/LabelAI" ,TextConst.S(5));
		SetLabel(UIName + "/Center/ViewStart/AISelect/AISecLabel" ,TextConst.S(6));
	}

//	IEnumerator WaitForVirtualScreen(){
//		yield return new WaitForSeconds(1);
//		
//		uiJoystick.CheckVirtualScreen();
//	}

//	private void showDefence(bool isShow) {
//		for(int i=0; i<uiDefenceGroup.Length; i++) {
//			uiDefenceGroup[i].SetActive(isShow);
//		}
//	}

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
	
	public void DoAttack(){
		UIControllerState(UIController.Attack);
	}

	//Defence
	public void DoBlock() {
		UIControllerState(UIController.Block);
	}

	public void DoSteal(){
		UIControllerState(UIController.Steal);
	}
	
	//Attack
	public void DoSkill(){
		UIControllerState(UIController.Skill);
	}
	
	public void DoShoot(GameObject go, bool state) {
		UIControllerState(UIController.Shoot, go, state);
	}
	
	public void DoPassChoose (GameObject obj, bool state) {
		UIControllerState(UIController.Pass, obj, state);
	}

	public void DoPassTeammateA(GameObject obj, bool state) {
		UIControllerState(UIController.PassA, obj, state);
	}

	public void DoPassTeammateB(GameObject obj, bool state) {
		UIControllerState(UIController.PassB, obj, state);
	}

	public void DoPassNone() {
		SetPassButton();
	}
	
	public void BackMainMenu() {
		UIState(UISituation.MainMenu);
	}

	public void OnReselect() {
		UIState(UISituation.ReSelect);
	}

	public void ContinueGame() {
		UIState(UISituation.Continue);
	}

	public void PauseGame(){
		UIState(UISituation.Pause);
	}

	public void ResetGame() {
		UIState(UISituation.Reset);
	}

	public void StartGame() {
		UIState(UISituation.Start);
	}

	public void RestartGame(){
		UIState(UISituation.Restart);
	}

	public void GameOver(){
		UIState(UISituation.Finish);
	}

	public void EffectSwitch(){
		UIState(UISituation.EffectSwitch);
	}

	public void OptionSelect(){
		UIState(UISituation.OptionSelect);
	}

	public void SetAnger (PlayerBehaviour p = null, float anger = 0){
		spriteForce.fillAmount = anger / 100;
		if (spriteForce.fillAmount == 1) {
			isAngerFull = true;
			uiSpriteFull.SetActive (true);
			ShowSkill();
		} else {
			isAngerFull = false;
			uiSpriteFull.SetActive (false);
		}
	}

	public void ShowSkill(){
		if(isAngerFull) {
			ShowSkillUI(true);
			isCanShowSkill = true;
		}
	}

	public void ShowSkillUI (bool isShow){
		if(isAngerFull) 
			uiSkill.SetActive(true);
		else 
			uiSkill.SetActive(false);

		if(GameController.Get.Joysticker != null && GameController.Get.situation == EGameSituation.AttackA) 
			uiSkillFull.SetActive(isShow);
		else 
			uiSkillFull.SetActive(false);
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
		if(Scores [team] < MaxScores[team])
			showScoreBar();
		TweenRotation tweenRotation = TweenRotation.Begin(labelScores[team].gameObject, 0.5f, Quaternion.identity);
		tweenRotation.delay = 0.5f;
		tweenRotation.to = new Vector3(0,720,0);
		labelScores[team].text = Scores [team].ToString ();
	}

	public void ChangeControl(bool IsAttack) {
		isAttackState = IsAttack;
		if(IsAttack) {
			UIMaskState(UIController.AttackA);
		} else {
			UIMaskState(UIController.AttackB);
		}
	}

	public bool UICantUse(PlayerBehaviour p = null) {
		if(p == GameController.Get.Joysticker) {
			isCanShowSkill = false;
			SetPassButton();
			ShowSkillUI(false);
			uiAttackPush.SetActive(true);
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
			if(isAttackState) UIMaskState(UIController.AttackA);
			else UIMaskState(UIController.AttackB);
			return true;
		} else {
			if (p.Team == GameController.Get.Joysticker.Team && p.crtState == EPlayerState.Alleyoop)
				SetPassButton();

			return false;
		}
	}

	public void SetPassButton() {
		if(GameStart.Get.TestMode != EGameTest.None) 
			return;

		int who = GameController.Get.GetBallOwner;
		switch (who) {
		case (int)UIPassType.MeBallOwner:
			viewPass.SetActive(true);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(true);
			uiPassObjectGroup[2].SetActive(true);
			GameController.Get.passIcon[0].SetActive(false);
			GameController.Get.passIcon[1].SetActive(true);
			GameController.Get.passIcon[2].SetActive(true);
			break;
		case (int)UIPassType.ABallOwner:
			viewPass.SetActive(true);
			uiPassObjectGroup[0].SetActive(true);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(true);
			GameController.Get.passIcon[0].SetActive(true);
			GameController.Get.passIcon[1].SetActive(false);
			GameController.Get.passIcon[2].SetActive(true);
			break;
		case (int)UIPassType.BBallOwner:
			viewPass.SetActive(true);
			uiPassObjectGroup[0].SetActive(true);
			uiPassObjectGroup[1].SetActive(true);
			uiPassObjectGroup[2].SetActive(false);
			GameController.Get.passIcon[0].SetActive(true);
			GameController.Get.passIcon[1].SetActive(true);
			GameController.Get.passIcon[2].SetActive(false);
			break;
		default:
			if(GameController.Get.situation == EGameSituation.AttackB) {
				viewPass.SetActive(false);
				GameController.Get.passIcon[0].SetActive(false);
				GameController.Get.passIcon[1].SetActive(false);
				GameController.Get.passIcon[2].SetActive(false);
			} else if(GameController.Get.situation == EGameSituation.AttackA) {
				viewPass.SetActive(true);
				GameController.Get.passIcon[0].SetActive(true);
				GameController.Get.passIcon[1].SetActive(true);
				GameController.Get.passIcon[2].SetActive(true);
			}
			break;
		}
//			GameController.Get.SetBodyMaterial(kind);
	}

	public void UIMaskState (UIController controllerState) {
		switch (controllerState) {
		case UIController.Skill:
			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiDefenceGroup[0].SetActive(false);
			uiDefenceGroup[1].SetActive(false);
			uiShoot.SetActive(false);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case UIController.Attack:
			if(GameController.Get.Joysticker.IsBallOwner) {
				//Elbow Attack
				isCanShowSkill = false;
				UIEffectState(UIController.Attack);

				uiShoot.SetActive(false);
				uiAttackPush.SetActive(true);
				uiDefenceGroup[0].SetActive(false);
				uiDefenceGroup[1].SetActive(false);
				uiPassObjectGroup[0].SetActive(false);
				uiPassObjectGroup[1].SetActive(false);
				uiPassObjectGroup[2].SetActive(false);
			} else {
				//Push Deffence
				isCanShowSkill = false;
				UIEffectState(UIController.Attack);

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
		case UIController.Block:
			isCanShowSkill = false;
			UIEffectState(UIController.Block);

			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiDefenceGroup[0].SetActive(false);
			uiDefenceGroup[1].SetActive(true);
			break;
		case UIController.Steal:
			isCanShowSkill = false;
			UIEffectState(UIController.Steal);

			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiDefenceGroup[0].SetActive(false);
			uiDefenceGroup[1].SetActive(false);
			break;
		case UIController.Shoot:
			isCanShowSkill = false;

			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiShoot.SetActive(true);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case UIController.Pass:
			isCanShowSkill = false;
	
			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiShoot.SetActive(false);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case UIController.PassA:
			isCanShowSkill = false;

			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiShoot.SetActive(false);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case UIController.PassB:
			isCanShowSkill = false;

			ShowSkillUI(false);
			uiAttackPush.SetActive(false);
			uiShoot.SetActive(false);
			uiPassObjectGroup[0].SetActive(false);
			uiPassObjectGroup[1].SetActive(false);
			uiPassObjectGroup[2].SetActive(false);
			break;
		case UIController.AttackA:
			shootBtnTime = ButtonBTime;
			isAttackState = true;
			isCanShowSkill = true;
			isCanDefenceBtnPress = true;
			isPressElbowBtn = true;

			controlButtonGroup[0].SetActive(true);
			controlButtonGroup[1].SetActive(false);

			ShowSkillUI(true);
			uiShoot.SetActive(true);
			uiAttackPush.SetActive(true);
			uiDefenceGroup[0].SetActive(false);
			uiDefenceGroup[1].SetActive(false);
			SetPassButton();
			break;
		case UIController.AttackB:
			shootBtnTime = ButtonBTime;
			isAttackState = false;
			isCanShowSkill = true;
			isCanDefenceBtnPress = true;
			isPressElbowBtn = true;
			
			controlButtonGroup[0].SetActive(false);
			controlButtonGroup[1].SetActive(true);
			
			ShowSkillUI(true);
			uiShoot.SetActive(false);
			uiAttackPush.SetActive(true);
			uiDefenceGroup[0].SetActive(true);
			uiDefenceGroup[1].SetActive(true);
			SetPassButton();
			break;
		}
	}

	public void UIControllerState (UIController controllerState, GameObject go = null, bool state = false) {
		switch(controllerState) {
		case UIController.Skill:
			if(!GameController.Get.Joysticker.IsFall && GameController.Get.Joysticker.IsBallOwner &&
			   uiSkillFull.activeInHierarchy && GameController.Get.Joysticker.CanUseState(EPlayerState.Dunk20)) {
				UIMaskState(UIController.Skill);
				GameController.Get.DoSkill();
			}
			break;
		case UIController.Attack:
			if(GameController.Get.Joysticker.IsBallOwner) {
				//Elbow
				if(isPressElbowBtn && 
				   !GameController.Get.Joysticker.IsFall && 
				   GameController.Get.situation == EGameSituation.AttackA &&
				   GameController.Get.Joysticker.CanUseState(EPlayerState.Elbow)) {
					UIMaskState(UIController.Attack);
					GameController.Get.DoElbow ();
					GameController.Get.Joysticker.SetNoAiTime();
				}
			} else {
				//Push
				if(isCanDefenceBtnPress && 
				   !GameController.Get.Joysticker.IsFall &&
				   (GameController.Get.situation == EGameSituation.AttackB || GameController.Get.situation == EGameSituation.AttackA) &&
				   GameController.Get.Joysticker.CanUseState(EPlayerState.Push)) {
					UIMaskState(UIController.Attack);
					GameController.Get.DoPush();
					GameController.Get.Joysticker.SetNoAiTime();
				}
			}
			break;
		case UIController.Block:
			if(isCanDefenceBtnPress && 
			   !GameController.Get.Joysticker.IsFall && 
			   GameController.Get.situation == EGameSituation.AttackB &&
			   GameController.Get.Joysticker.CanUseState(EPlayerState.Block)) {
				UIMaskState(UIController.Block);
				GameController.Get.DoBlock();
				GameController.Get.Joysticker.SetNoAiTime();
			}
			break;
		case UIController.Steal:
			if(isCanDefenceBtnPress && 
			   !GameController.Get.Joysticker.IsFall &&
			   GameController.Get.situation == EGameSituation.AttackB && 
			   GameController.Get.StealBtnLiftTime <= 0 && 
			   GameController.Get.Joysticker.CanUseState(EPlayerState.Steal)) {
				UIMaskState(UIController.Steal);
				GameController.Get.DoSteal();
				GameController.Get.Joysticker.SetNoAiTime();
			}
			break;
		case UIController.Shoot:
			if(GameController.Get.IsShooting) {
				if(state && UIDoubleClick.Visible){
					UIDoubleClick.Get.ClickStop ();
				}
			} else {
				if(GameController.Get.Joysticker.IsBallOwner &&
				   GameController.Get.situation == EGameSituation.AttackA && 
				   !GameController.Get.Joysticker.IsFall && 
				   !GameController.Get.Joysticker.CheckAnimatorSate(EPlayerState.MoveDodge0) && 
				   !GameController.Get.Joysticker.CheckAnimatorSate(EPlayerState.MoveDodge1) && 
				   !GameController.Get.Joysticker.CheckAnimatorSate(EPlayerState.Block)
				   ) {
					if(state && GameController.Get.Joysticker.IsFakeShoot && isShootAvailable) {
						isShootAvailable = false;
					}
					if(state)
						UIEffectState(UIController.Shoot);
					else 
					if(!state && shootBtnTime > 0 && isShootAvailable){
						if(GameController.Get.BallOwner != null) {
							if(GameController.Get.Joysticker.IsBallOwner) {
								UIMaskState(UIController.Shoot);
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
		case UIController.Pass:
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
				UIMaskState(UIController.Pass);
				if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(0)){
					GameController.Get.Joysticker.SetNoAiTime();
				}
			}
			break;
		case UIController.PassA:
			if(GameController.Get.GetBallOwner != 1) {
				UIMaskState(UIController.PassA);
				if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(1)){
					GameController.Get.Joysticker.SetNoAiTime();
				}
			}
			break;
		case UIController.PassB:
			if(GameController.Get.GetBallOwner != 2) {
				UIMaskState(UIController.PassB);
				if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(2)){
					GameController.Get.Joysticker.SetNoAiTime();
				}
			}
//			else 
//				uiShoot.SetActive(false);

			break;
		}
	}

	public void UIState(UISituation situation){
		switch(situation) {
		case UISituation.Start:
			viewStart.SetActive (false);
			uiScoreBar.SetActive (false);
			uiJoystick.Joystick.isActivated = true;
			
			CourtMgr.Get.SetBallState (EPlayerState.Start);
			GameController.Get.StartGame();
			break;
		case UISituation.Pause:
			if (!viewStart.activeInHierarchy) {
				Time.timeScale = 0;
				viewFinish.SetActive(false);
				viewTools.SetActive(true);
				viewPause.SetActive(true);
				
				uiContinue.SetActive(true);
				uiReselect.SetActive(true);
				uiScoreBar.SetActive(true);
				uiJoystick.Joystick.isActivated = false;
			}
			break;
		case UISituation.Continue:
			if (GameController.Get.IsStart) {
				Time.timeScale = 1;
				isShowOption = false;
				viewFinish.SetActive(false);
				viewTools.SetActive(false);
				viewPause.SetActive(false);
				
				uiContinue.SetActive(false);
				uiReselect.SetActive(false);
				uiScoreBar.SetActive(false);
				uiJoystick.Joystick.isActivated = true;
			}
			break;
		case UISituation.Finish:
			viewFinish.SetActive(true);
			uiScoreBar.SetActive(true);
//			uiJoystick.gameObject.SetActive(false);
			uiJoystick.Joystick.isActivated = false;
			break;

		case UISituation.Restart:
			ResetGame();
			Time.timeScale = 1;
			viewFinish.SetActive(false);
			viewTools.SetActive(false);
			viewPause.SetActive(false);
			uiReselect.SetActive(false);
			uiContinue.SetActive(false);
			break;
		case UISituation.Reset:
			GameController.Get.Reset ();
			InitData ();
			CourtMgr.Get.SetScoreboards (0, Scores [0]);
			CourtMgr.Get.SetScoreboards (1, Scores [1]);
			isShowOption = false;
			isShowScoreBar = false;
			isAngerFull = false;
			
			viewStart.SetActive (true);
			viewFinish.SetActive(false);
			viewTools.SetActive(false);
			viewPause.SetActive(false);
			
			uiReselect.SetActive(false);
			uiScoreBar.SetActive(true);
			uiJoystick.Joystick.isActivated = false;
			break;
		case UISituation.ReSelect:
			Time.timeScale = 1;
			SceneMgr.Get.ChangeLevel (SceneName.SelectRole);
			break;
		case UISituation.MainMenu:
			Time.timeScale = 1;
			SceneMgr.Get.ChangeLevel(SceneName.Lobby);
			break;
		case UISituation.EffectSwitch:
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
		case UISituation.OptionSelect:
			isShowOption = !isShowOption;
			viewOption.SetActive(isShowOption);
			break;
		}
		AudioMgr.Get.PauseGame();
	}

	public void UIEffectState(UIController effect){
		switch(effect){
		case UIController.Attack:
			buttonAttackFXTime = fxTime;
			buttonAttackFX.SetActive(true);
			break;
		case UIController.Block:
			buttonBlockFXTime = fxTime;
			buttonBlockFX.SetActive(true);
			break;
		case UIController.Shoot:
			buttonShootFXTime = fxTime;
			buttonShootFX.SetActive(true);
			break;
		case UIController.Steal:
			buttonStealFXTime = fxTime;
			buttonStealFX.SetActive(true);
			break;
		}
	}

	private void showScoreBar(){
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
			if(playerScreenPos.y > -330 && playerScreenPos.y < 330 && playerInCameraX >= 0) {
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
			   playerInCameraY > -50 &&
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
