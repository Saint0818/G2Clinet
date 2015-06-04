using UnityEngine;
using System.Collections;

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

public enum UIEffect {
	Pass,
	Shoot,
	Block,
	Steal,
	Attack,
	Skill
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
	PassB
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
	private GameObject uiAttackPush;
	private GameObject uiPlayerLocation;

	private GameObject[] attackGroup = new GameObject[2];
	private GameObject[] defenceGroup = new GameObject[2];
	private GameObject[] controlButtonGroup= new GameObject[2];
	private GameObject[] passObjectGroup = new GameObject[2];
	private GameObject[] effectGroup = new GameObject[2];

	private UISprite spritePass;
	private UILabel[] labelScores = new UILabel[2];
	private UIScrollBar[] aiLevelScrollBar = new UIScrollBar[3];

	//FX
	private float fxTime = 0.3f;
	private GameObject buttonPassFX;
	private float buttonPassFXTime;
	private GameObject buttonShootFX;
	private float buttonShootFXTime;
	private GameObject buttonBlockFX;
	private float buttonBlockFXTime;
	private GameObject buttonStealFX;
	private float buttonStealFXTime;
	private GameObject buttonAttackFX;
	private float buttonAttackFXTime;
	private GameObject buttonSkillFX;
	private float buttonSkillFXTime;

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
		if (Input.GetMouseButtonUp(0)) {
			isPressShootBtn = false;
			if(UICamera.hoveredObject.name.Equals("ButtonObjectA")) {
				DoPassTeammateA();
			} else if (UICamera.hoveredObject.name.Equals("ButtonObjectB")) {
				DoPassTeammateB();
			}
		}

		if (isPressShootBtn && shootBtnTime > 0) {
			shootBtnTime -= Time.deltaTime;
			if(shootBtnTime <= 0){
				isPressShootBtn = false;
				if (GameController.Get.BallOwner == GameController.Get.Joysticker) {
					GameController.Get.DoShoot(true);
					GameController.Get.Joysticker.SetNoAiTime();
					uiAttackPush.SetActive(false);
					attackGroup[0].SetActive(false);
					ShowSkillUI(false);

					if(GameController.Get.IsCanPassAir) {
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
		uiAttackPush = GameObject.Find(UIName + "/BottomRight/ButtonAttack");
		uiPlayerLocation = GameObject.Find (UIName + "/Right");

		controlButtonGroup [0] = GameObject.Find (UIName + "/BottomRight/ViewAttack");
		controlButtonGroup [1] = GameObject.Find (UIName + "/BottomRight/ViewDefance");

		attackGroup[0] = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonPass");
		attackGroup[1] = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonShoot");

		defenceGroup[0] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal");
		defenceGroup[1] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonBlock");

		labelScores [0] = GameObject.Find (UIName + "/Top/UIScoreBar/LabelScore1").GetComponent<UILabel>();
		labelScores [1] = GameObject.Find (UIName + "/Top/UIScoreBar/LabelScore2").GetComponent<UILabel>();

		spritePass = attackGroup[0].GetComponent<UISprite>();

		passObjectGroup [0] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA");
		passObjectGroup [1] = GameObject.Find (UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB");

		aiLevelScrollBar [0] = GameObject.Find(UIName + "/Center/ViewStart/AISelect/HomeScrollBar").GetComponent<UIScrollBar>();
		aiLevelScrollBar [1] = GameObject.Find(UIName + "/Center/ViewStart/AISelect/AwayScrollBar").GetComponent<UIScrollBar>();
		aiLevelScrollBar [2] = GameObject.Find(UIName + "/Center/ViewStart/AISelect/AIControlScrollBar").GetComponent<UIScrollBar>();

		effectGroup[0] = GameObject.Find (UIName + "/Center/ViewTools/ViewOption/ButtonEffect/LabelON");
		effectGroup[1] = GameObject.Find (UIName + "/Center/ViewTools/ViewOption/ButtonEffect/LabelOff");
		effectGroup [0].SetActive (GameData.Setting.Effect);
		effectGroup [1].SetActive (!GameData.Setting.Effect);


		buttonPassFX = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonPass/UI_FX_A_21");
		buttonShootFX = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonShoot/UI_FX_A_21");
		buttonBlockFX = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonBlock/UI_FX_A_21");
		buttonStealFX = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal/UI_FX_A_21");
		buttonAttackFX = GameObject.Find(UIName + "/BottomRight/ButtonAttack/UI_FX_A_21");
		buttonSkillFX = GameObject.Find(UIName + "/BottomRight/ButtonSkill/UI_FX_A_21");
		buttonPassFX.SetActive(false);
		buttonShootFX.SetActive(false);
		buttonBlockFX.SetActive(false);
		buttonStealFX.SetActive(false);
		buttonAttackFX.SetActive(false);
		buttonSkillFX.SetActive(false);

		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewAttack/ButtonShoot")).onPress = DoShoot;
		UIEventListener.Get (GameObject.Find (UIName + "/BottomRight/ViewAttack/ButtonPass")).onPress = DoPassChoose;

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
		SetPassButton(0);

		ChangeControl(false);
		
		uiJoystick.gameObject.SetActive(false);
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

	IEnumerator WaitForVirtualScreen(){
		yield return new WaitForSeconds(1);
		
		uiJoystick.CheckVirtualScreen();
	}

	private void showAttack(bool isShow) {
		for (int i=0; i<attackGroup.Length; i++) {
			attackGroup[i].SetActive(isShow);
		}
	}

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

	public void DoPassTeammateA() {
		UIControllerState(UIController.PassA);
	}

	public void DoPassTeammateB() {
		UIControllerState(UIController.PassB);
	}

	public void DoPassNone() {
		SetPassButton(0);
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

	public bool ShowSkill(PlayerBehaviour p = null){
		if(p == GameController.Get.Joysticker) {
			GameController.Get.Joysticker.AngryFull.SetActive(true);
			ShowSkillUI(true);
			return true;
		} else {
			return false;
		}
	}

	public void ShowSkillUI (bool isShow){
		if(GameController.Get.Joysticker != null && 
		   GameController.Get.Joysticker.AngerView.fillAmount == 1) 
			uiSkill.SetActive(isShow);
		else
			uiSkill.SetActive(false);
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

	public void SetPassButton(int kind) {
		switch (kind) {
		case 1:
			spritePass.alpha = 0f;
			viewPass.SetActive(true);
			passObjectGroup[0].SetActive(true);
			passObjectGroup[1].SetActive(false);
			break;
		case 2:
			spritePass.alpha = 0f;
			viewPass.SetActive(true);
			passObjectGroup[0].SetActive(false);
			passObjectGroup[1].SetActive(true);
            break;
		case 3:
			spritePass.alpha = 0f;
			viewPass.SetActive(true);
			passObjectGroup[0].SetActive(true);
			passObjectGroup[1].SetActive(true);
			break;
		case 4:
			spritePass.alpha = 1f;
			viewPass.SetActive(true);
			passObjectGroup[0].SetActive(true);
			passObjectGroup[1].SetActive(true);
			break;
		default:
			spritePass.alpha = 1f;
			viewPass.SetActive(false);
			passObjectGroup[0].SetActive(true);
			passObjectGroup[1].SetActive(true);
			break;
        }
		GameController.Get.SetBodyMaterial(kind);
	}


	public void ChangeControl(bool IsAttack) {
		shootBtnTime = ButtonBTime;
		controlButtonGroup[0].SetActive(IsAttack);
		showAttack(IsAttack);
		controlButtonGroup[1].SetActive(!IsAttack);
		showDefence(!IsAttack);
		uiAttackPush.SetActive(true);
		isAttackState = IsAttack;
	}

	public bool UICantUse(PlayerBehaviour p = null) {
		if(p == GameController.Get.Joysticker) {
			controlButtonGroup[0].SetActive(false);
			controlButtonGroup[1].SetActive(false);
			uiAttackPush.SetActive(false);
			SetPassButton(0);
			ShowSkillUI(false);
			return true;
		} else {
			if (p.Team == GameController.Get.Joysticker.Team && p.crtState == PlayerState.Alleyoop)
				SetPassButton(0);
			
			return false;
		}
	}

	public bool OpenUIMask(PlayerBehaviour p = null){
		if(p == GameController.Get.Joysticker) {
			if(p.NoAiTime > 0)
				p.SetNoAiTime();

			shootBtnTime = ButtonBTime;
			controlButtonGroup[0].SetActive(isAttackState);
			showAttack(isAttackState);
			controlButtonGroup[1].SetActive(!isAttackState);
			showDefence(!isAttackState);
			uiAttackPush.SetActive(true);
			ShowSkillUI(true);

			isCanDefenceBtnPress = true;
			isPressElbowBtn = true;

			return true;
		} else {
			if (p.Team == GameController.Get.Joysticker.Team && p.crtState == PlayerState.Alleyoop)
				SetPassButton(0);

			return false;
		}
	}

	public void UIControllerState (UIController controllerState, GameObject go = null, bool state = false) {
		switch(controllerState) {
		case UIController.Skill:
			if(!GameController.Get.Joysticker.IsFall && 
			   GameController.Get.Joysticker.IsBallOwner &&
			   GameController.Get.Joysticker.AngerView.fillAmount == 1) 
			{
				GameController.Get.DoSkill();
				GameController.Get.Joysticker.SetNoAiTime();
				GameController.Get.Joysticker.AngerView.fillAmount = 0;
				GameController.Get.Joysticker.AngryFull.SetActive(false);
				ShowSkillUI(false);
			}
			break;
		case UIController.Attack:
			if(GameController.Get.Joysticker.IsBallOwner) {
				//Elbow
				if(isPressElbowBtn && 
				   !GameController.Get.Joysticker.IsFall && 
				   GameController.Get.situation == GameSituation.AttackA &&
				   GameController.Get.Joysticker.CanUseState(PlayerState.Elbow)) {
					UIEffectState(UIEffect.Attack);
					showAttack(false);
					ShowSkillUI(false);
					GameController.Get.DoElbow ();
					GameController.Get.Joysticker.SetNoAiTime();
				}
			} else {
				//Push
				if(isCanDefenceBtnPress && 
				   !GameController.Get.Joysticker.IsFall &&
				   (GameController.Get.situation == GameSituation.AttackB || GameController.Get.situation == GameSituation.AttackA) &&
				   GameController.Get.Joysticker.CanUseState(PlayerState.Push)) {
					UIEffectState(UIEffect.Attack);
					showDefence(false);
					ShowSkillUI(false);
					GameController.Get.DoPush();
					GameController.Get.Joysticker.SetNoAiTime();
				}
			}
			break;
		case UIController.Block:
			if(isCanDefenceBtnPress && 
			   !GameController.Get.Joysticker.IsFall && 
			   GameController.Get.situation == GameSituation.AttackB &&
			   GameController.Get.Joysticker.CanUseState(PlayerState.Block)
			   ) {
				UIEffectState(UIEffect.Block);
				uiAttackPush.SetActive(false);
				defenceGroup[0].SetActive(false);
				ShowSkillUI(false);
				GameController.Get.DoBlock();
				GameController.Get.Joysticker.SetNoAiTime();
			}
			break;
		case UIController.Steal:
			if(isCanDefenceBtnPress && 
			   !GameController.Get.Joysticker.IsFall &&
			   GameController.Get.situation == GameSituation.AttackB && 
			   GameController.Get.StealBtnLiftTime <= 0 && 
			   GameController.Get.Joysticker.CanUseState(PlayerState.Steal)
			   ) {
				UIEffectState(UIEffect.Steal);
				uiAttackPush.SetActive(false);
				defenceGroup[1].SetActive(false);
				ShowSkillUI(false);
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
				   GameController.Get.situation == GameSituation.AttackA && 
				   !GameController.Get.Joysticker.IsFall && 
				   !GameController.Get.Joysticker.CheckAnimatorSate(PlayerState.MoveDodge0) && 
				   !GameController.Get.Joysticker.CheckAnimatorSate(PlayerState.MoveDodge1) && 
				   !GameController.Get.Joysticker.CheckAnimatorSate(PlayerState.Block)
				   ) {
					if(state && GameController.Get.Joysticker.IsFakeShoot && isShootAvailable) {
						isShootAvailable = false;
					}
					if(state)
						UIEffectState(UIEffect.Shoot);
					else 
					if(!state && shootBtnTime > 0 && isShootAvailable){
						if(GameController.Get.BallOwner != null) {
							if(GameController.Get.Joysticker.IsBallOwner) {
								uiAttackPush.SetActive(false);
								attackGroup[0].SetActive(false);
								ShowSkillUI(false);
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
			if(GameController.Get.Joysticker.IsBallOwner && 
			   !GameController.Get.Joysticker.IsFall && 
			   GameController.Get.situation == GameSituation.AttackA) {
				if(!GameController.Get.IsCanPassAir){
					if (state) 
						SetPassButton(4);
				}
			} else {
				GameController.Get.DoPass(0);
				GameController.Get.Joysticker.SetNoAiTime();
			}
			
			if(!state)
				SetPassButton(0);
			break;
		case UIController.PassA:
			SetPassButton(0);
			attackGroup[1].SetActive(false);
			uiAttackPush.SetActive(false);
			ShowSkillUI(false);
			
			if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(1)){
				UIEffectState(UIEffect.Pass);
				GameController.Get.Joysticker.SetNoAiTime();
			}else
				showAttack(true);
			break;
		case UIController.PassB:
			SetPassButton(0);
			attackGroup[1].SetActive(false);
			uiAttackPush.SetActive(false);
			ShowSkillUI(false);
			
			if((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(2)){
				UIEffectState(UIEffect.Pass);
				GameController.Get.Joysticker.SetNoAiTime();
			}else 
				showAttack(true);

			break;
		}
	}

	public void UIState(UISituation situation){
		switch(situation) {
		case UISituation.Start:
			viewStart.SetActive (false);
			uiScoreBar.SetActive (false);
			uiJoystick.gameObject.SetActive(true);
			
			CourtMgr.Get.SetBallState (PlayerState.Start);
			GameController.Get.StartGame();
			
			StartCoroutine("WaitForVirtualScreen");
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
				uiJoystick.gameObject.SetActive(false);
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
				uiJoystick.gameObject.SetActive(true);
			}
			break;
		case UISituation.Finish:
			viewFinish.SetActive(true);
			uiScoreBar.SetActive(true);
			uiJoystick.gameObject.SetActive(false);
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
			
			viewStart.SetActive (true);
			viewFinish.SetActive(false);
			viewTools.SetActive(false);
			viewPause.SetActive(false);
			
			uiReselect.SetActive(false);
			uiScoreBar.SetActive(true);
			uiJoystick.gameObject.SetActive(false);
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
	}

	public void UIEffectState(UIEffect effect){
		switch(effect){
		case UIEffect.Attack:
			buttonAttackFXTime = fxTime;
			buttonAttackFX.SetActive(true);
			break;
		case UIEffect.Block:
			buttonBlockFXTime = fxTime;
			buttonBlockFX.SetActive(true);
			break;
		case UIEffect.Pass:
			buttonPassFXTime = fxTime;
			buttonPassFX.SetActive(true);
			break;
		case UIEffect.Shoot:
			buttonShootFXTime = fxTime;
			buttonShootFX.SetActive(true);
			break;
		case UIEffect.Steal:
			buttonStealFXTime = fxTime;
			buttonStealFX.SetActive(true);
			break;
		case UIEffect.Skill:
			buttonSkillFXTime = fxTime;
			buttonSkillFX.SetActive(true);
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
		
		if(buttonSkillFXTime > 0) {
			buttonSkillFXTime -= Time.deltaTime;
			if(buttonSkillFXTime <= 0) {
				buttonSkillFXTime = 0;
				buttonSkillFX.SetActive(false);
			}
		}
	}
}
