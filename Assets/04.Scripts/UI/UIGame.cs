using G2;
using UnityEngine;

public enum EUISituation
{
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

public enum EUIControl
{
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

public enum EUIPassType
{
    MeBallOwner = 0,
    ABallOwner = 1,
    BBallOwner = 2
}

public enum EUIRangeType
{
    Skill,
    Push,
    Elbow,
    Steal
}

public class UIGame : UIBase
{
    private static UIGame instance = null;
    private const string UIName = "UIGame";

    private PlayerBehaviour PlayerMe;
    //Game const
    public float ButtonBTime = 0.08f;
    //Fake to shoot time
    public int[] MaxScores = { 13, 13 };
    public int[] Scores = { 0, 0 };

    private float shootBtnTime = 0;

    private bool isPressElbowBtn = true;
    private bool isCanDefenceBtnPress = true;
    private bool isPressShootBtn = false;
    private bool isShootAvailable = true;

    // GoldFinger
    private bool isPressA = false;
	 
    //GameJoystick
    private GameJoystick gameJoystick = null;
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
    private GameObject[] uiDefenceGroup = new GameObject[2];
    // 0:ButtonSteal 1:ButtonBlock
    private GameObject[] controlButtonGroup = new GameObject[2];
// 0:ViewAttack 1:ViewDefence
    private GameObject[] uiPassObjectGroup = new GameObject[3];
// 0:SpriteMe 1:SpriteA 2:SpriteB
    private GameObject[] uiTutorial = new GameObject[9];
//set button for tutorial
    private GameObject[] uiTutorial2 = new GameObject[2];
//set button for tutorial

    //TopLeft
    private GameObject uiSpeed;
	private UILabel labelSpeed;
    private GameObject uiPause;
    private GameObject viewTopLeft;
    private UILabel[] labelTopLeftScore = new UILabel[2];
    private GameObject uiLimitTime;
    private UILabel labelLimitTime;
    public UILabel LabelStrategy;

    //Right
    private GameObject uiPlayerLocation;

    //TopRight
    private GameObject viewTopRight;
    private GameObject[] uiButtonSkill = new GameObject[3];
    private GameObject[] uiSkillEnables = new GameObject[3];
    private GameObject[] uiDCs = new GameObject[3];
    private UISprite[] spriteSkills = new UISprite[3];
    private UISprite[] spriteEmptys = new UISprite[3];
    private UISprite spriteForce;
    private UISprite spriteForceFirst;
    private GameObject uiSpriteFull;
    private UILabel uiForceNum;

    //DC
    private int dcCount = 0;
    private float baseForceValue;
    private float oldForceValue;
    private float newForceValue;
    private float timeForce;

    //	private DrawLine drawLine;
	
    private bool isShowSkillRange;
    private bool isShowPushRange;
    private bool isShowElbowRange;
    private bool isShowStealRange;
    private Transform skillRangeTarget;
    private PlayerBehaviour nearP;
    private float eulor;

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

    //JoySticker
    private Vector2 mPosition = Vector2.zero;
    private Vector2 screenPosition = Vector2.zero;
	
    private ItemSkillHint skillHint;
    private float skillHintTime;
    private int skillHintIndex;

    public static UIGame Get
    {
        get
        {
            if (!instance)
                instance = LoadUI(UIName) as UIGame;

            return instance;
        }
    }

    public static bool Visible
    {
        get
        {
            if (instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }

        set
        {
            if (instance)
            {
                if (!value)
                    RemoveUI(UIName);
                else
                    instance.Show(value);
            }
            else if (value)
                Get.Show(value);
        }
    }

    public static void UIShow(bool isShow)
    {
        if (instance)
            instance.Show(isShow);
        else if (isShow)
            Get.Show(isShow);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x < (Screen.width * 0.5f) && Input.mousePosition.y < (Screen.height * 0.55f))
        {
            screenPosition = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameJoystick.GetCanvas.rectTransform(), screenPosition, gameJoystick.GetCanvas.worldCamera, out mPosition);
            gameJoystick.GetTranform.anchoredPosition = mPosition;
        }
    }

    void FixedUpdate()
    {
        if (GameController.Get.IsStart)
        {
            if (PlayerMe && IsPlayerAttack)
            {
                if (isShowSkillRange || isShowElbowRange || isShowPushRange || isShowStealRange)
                {
                    if (isShowPushRange)
                    {
						CourtMgr.Get.RangeOfActionEuler(0);
                    }
                    else if (isShowStealRange)
                    {
						if (GameController.Get.BallOwner != null) {
							CourtMgr.Get.RangeOfActionEuler(0);
						}
                    }
                    if (skillRangeTarget != null)
                        CourtMgr.Get.RangeOfActionPosition(skillRangeTarget.position);
                }
            }
			
            if (skillHintTime > 0)
            {
                skillHintTime -= Time.deltaTime;
                if (skillHintTime <= 0)
                {
                    skillHint.Show();
                    skillHint.UpdateUI(skillHintIndex);
                }
            }
			
            runForceValue();
            if (isPressShootBtn && shootBtnTime > 0)
            {
                shootBtnTime -= Time.deltaTime;
                if (shootBtnTime <= 0)
                {
                    isPressShootBtn = false;
                    if (PlayerMe && GameController.Get.BallOwner == PlayerMe)
                    {
                        if (GameController.Get.DoShoot(true))
                        {
                            PlayerMe.SetManually();
                            spriteAttack.gameObject.SetActive(false);
                            ShowSkillEnableUI(false);
                        }
						
                        if (GameController.Get.IsCanPassAir)
                        {
                            SetPassButton();
                        }
                    }
                }
            }
			
            judgePlayerScreenPosition();
            setGameTime();
        }
    }

    protected override void InitCom()
    {
        GameController.Get.onSkillDCComplete += AddForceValue;
        SetBtnFun(UIName + "/TopLeft/ButtonSpeed", OnSpeed);

        //GameJoystick
        GameObject obj2 = GameObject.Find("GameJoystick");
        if (!obj2)
        {
            GameObject obj = Resources.Load("Prefab/GameJoystick") as GameObject;
        
            if (obj)
            {
                obj2 = Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
                obj2.name = "GameJoystick";
                //joystickController = obj.AddComponent<JoystickController> ();
            }
        }

        if (obj2)
        {
            gameJoystick = obj2.GetComponentInChildren<GameJoystick>();
            if (gameJoystick)
                gameJoystick.visible = false;
        }

        gameJoystick.SetJoystickType(ETCJoystick.JoystickType.Dynamic);

        gameJoystick.GetTranform.anchorMin = new Vector2(0.5f, 0.5f);
        gameJoystick.GetTranform.anchorMax = new Vector2(0.5f, 0.5f);
        gameJoystick.GetTranform.SetAsLastSibling();

        //Center
        viewStart = GameObject.Find(UIName + "/Center/ViewStart");
		
        //BottomRight
        viewBottomRight = GameObject.Find(UIName + "/BottomRight");
        uiShoot = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonShoot/SpriteShoot");
        viewPass = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass");
        uiPassA = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA");
        uiPassB = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB");
        uiAlleyoopA = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/AlleyoopA");
        uiAlleyoopB = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/AlleyoopB");
        spriteAttack = GameObject.Find(UIName + "/BottomRight/ButtonAttack/SpriteAttack").GetComponent<UISprite>();
        uiDefenceGroup[0] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal/SpriteSteal");
        uiDefenceGroup[1] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonBlock/SpriteBlock");
        controlButtonGroup[0] = GameObject.Find(UIName + "/BottomRight/ViewAttack");
        controlButtonGroup[1] = GameObject.Find(UIName + "/BottomRight/ViewDefance");
        uiPassObjectGroup[0] = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonPass/SpriteMe");
        uiPassObjectGroup[1] = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectA/SpriteA");
        uiPassObjectGroup[2] = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonObjectB/SpriteB");
		
        //TopLeft
        uiSpeed = GameObject.Find(UIName + "/TopLeft/ButtonSpeed");
		labelSpeed = GameObject.Find(UIName + "/TopLeft/ButtonSpeed/SpeedLabel").GetComponent<UILabel>();
        uiPause = GameObject.Find(UIName + "/TopLeft/ButtonPause");
        viewTopLeft = GameObject.Find(UIName + "TopLeft");
        labelTopLeftScore[0] = GameObject.Find(UIName + "TopLeft/ButtonPause/ScoreBoard/Home").GetComponent<UILabel>();
        labelTopLeftScore[1] = GameObject.Find(UIName + "TopLeft/ButtonPause/ScoreBoard/Away").GetComponent<UILabel>();
        uiLimitTime = GameObject.Find(UIName + "/TopLeft/Countdown");
        labelLimitTime = GameObject.Find(UIName + "/TopLeft/Countdown/TimeLabel").GetComponent<UILabel>();
        LabelStrategy = GameObject.Find(UIName + "/TopLeft/StrategyLabel").GetComponent<UILabel>();
        SetBtnFun(UIName + "/TopLeft/ButtonPause", OnPause);
		
        //Right
        uiPlayerLocation = GameObject.Find(UIName + "/Right");

        //TopRight
        viewTopRight = GameObject.Find(UIName + "/TopRight");
        skillHint = GameObject.Find(UIName + "/TopRight/ItemSkillHint").GetComponent<ItemSkillHint>();
        skillHint.transform.parent = viewTopRight.transform;
        skillHint.transform.localPosition = new Vector3(-300, -185, 0);
        skillHint.transform.localScale = Vector3.one;
        uiForceNum = GameObject.Find(UIName + "/TopRight/ViewForceBar/ForceNum").GetComponent<UILabel>();

        spriteForce = GameObject.Find(UIName + "/TopRight/ViewForceBar/Forcebar/SpriteForce").GetComponent<UISprite>();
        spriteForceFirst = GameObject.Find(UIName + "/TopRight/ViewForceBar/Forcebar/SpriteForceFrist").GetComponent<UISprite>();
        uiSpriteFull = GameObject.Find(UIName + "/TopRight/ViewForceBar/ForcebarFull");
        for (int i = 0; i < uiSkillEnables.Length; i++)
        {
            uiButtonSkill[i] = GameObject.Find(UIName + "/TopRight/ButtonSkill" + i.ToString());
            uiButtonSkill[i].name = i.ToString();
            uiSkillEnables[i] = GameObject.Find(uiButtonSkill[i].name + "/SpriteFull");
            uiDCs[i] = GameObject.Find(uiButtonSkill[i].name + "/GetDCSoul");
            uiDCs[i].SetActive(false);
            spriteSkills[i] = GameObject.Find(uiButtonSkill[i].name + "/SpriteSkill").GetComponent<UISprite>();
            spriteEmptys[i] = GameObject.Find(uiButtonSkill[i].name + "/SkillEmpty").GetComponent<UISprite>();
            UIEventListener.Get(uiButtonSkill[i]).onPress = DoSkill;
            UIEventListener.Get(uiButtonSkill[i]).onDragOver = DoSkillOut;
            uiButtonSkill[i].SetActive(false);
            uiSkillEnables[i].SetActive(false);
        }

        UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonShoot")).onPress = DoShoot;
        UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonPass")).onPress = DoPassChoose;
        UIEventListener.Get(uiPassA).onPress = DoPassTeammateA;
        UIEventListener.Get(uiPassB).onPress = DoPassTeammateB;
        UIEventListener.Get(uiAlleyoopA).onPress = DoPassTeammateA;
        UIEventListener.Get(uiAlleyoopB).onPress = DoPassTeammateB;

        SetBtnFun(UIName + "/Center/ViewStart/ButtonStart", StartGame);
        SetBtnFun(UIName + "/BottomRight/ViewDefance/ButtonBlock", DoBlock);
        UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/ButtonAttack")).onPress = DoAttack;
        UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/ButtonAttack")).onDragOver = DoAttackOut;
        UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal")).onPress = DoSteal;
        UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal")).onDragOver = DoStealOut;
		
//		drawLine = gameObject.AddComponent<DrawLine>();
//		uiScoreBar.SetActive(false);
        uiAlleyoopA.SetActive(false);
        uiAlleyoopB.SetActive(false);
        uiSpriteFull.SetActive(false);
        showViewForceBar(false);

        //toturial button
        uiTutorial2[0] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonBlock");
        uiTutorial2[1] = GameObject.Find(UIName + "/BottomRight/ViewDefance/ButtonSteal");

        uiTutorial[0] = GameObject.Find(UIName + "/BottomRight/ViewAttack/ButtonShoot");
        uiTutorial[1] = GameObject.Find(UIName + "/BottomRight/ViewAttack/ViewPass/ButtonPass");
        uiTutorial[2] = GameObject.Find(UIName + "/BottomRight/ButtonAttack"); //push
        uiTutorial[3] = uiPassA;
        uiTutorial[4] = uiPassB;
        uiTutorial[5] = uiButtonSkill[0];
        uiTutorial[6] = uiButtonSkill[1];
        uiTutorial[7] = uiButtonSkill[2];
        uiTutorial[8] = gameJoystick.gameObject;
    }

    public void InitUI()
    {
//		isShowScoreBar = false;
        Scores[0] = 0;
        Scores[1] = 0;
        labelTopLeftScore[0].text = "0";
        labelTopLeftScore[1].text = "0";
        uiForceNum.text = "0/[13CECEFF]" + GameData.Team.Player.MaxAnger + "[-]";
        LabelStrategy.text = string.Format(TextConst.S(9602), GameData.Team.Player.StrategyText);
        spriteForce.fillAmount = 0;
        spriteForceFirst.fillAmount = 0;
        showUITime();
		
        uiPlayerLocation.SetActive(false);
        uiAlleyoopA.SetActive(false);
        uiAlleyoopB.SetActive(false);
        viewTopLeft.SetActive(false);
        uiSpriteFull.SetActive(false);
        if (PlayerMe && PlayerMe.Attribute.IsHaveActiveSkill)
        {
            for (int i = 0; i < PlayerMe.Attribute.ActiveSkills.Count; i++)
            {
                uiButtonSkill[i].SetActive((i < PlayerMe.Attribute.ActiveSkills.Count));
            }
        }
        ChangeControl(true);
        showViewForceBar(true);
		ShowSkillEnableUI(false);
		refreshSpeedLabel ();
//		drawLine.IsShow = false;
    }

    public void InitTutorialUI()
    {
        uiSpeed.SetActive(false);
        uiPause.SetActive(false);
    }

    private void initJoystickPos()
    {
        gameJoystick.transform.localPosition = new Vector3(-245, -130f);
    }

    private void showGameJoystick(bool isShow)
    {
        gameJoystick.visible = isShow;
        gameJoystick.activated = isShow;
    }

    private void showUITime()
    {
        if (GameController.Get.StageData.HintBit[0] == 0)
            uiLimitTime.SetActive(false);
        else
        {
            uiLimitTime.SetActive(true);
            labelLimitTime.text = GameController.Get.GameTime.ToString();
        }
    }

    private void setGameTime()
    {
        int minute = (int)(GameController.Get.GameTime / 60f);
        int second = (int)(GameController.Get.GameTime % 60f);
        if (second < 10)
            labelLimitTime.text = minute.ToString() + ":0" + second.ToString();
        else
            labelLimitTime.text = minute.ToString() + ":" + second.ToString();
    }

    public void ResetRange()
    {
        isShowElbowRange = false;
        isShowPushRange = false;
        isShowSkillRange = false;
        isShowStealRange = false;
        CourtMgr.Get.ShowRangeOfAction(false);
        CourtMgr.Get.ShowArrowOfAction(false);
        showSkillHint(false);
    }

    public void InitPlayerSkillUI(PlayerBehaviour p)
    {
        PlayerMe = p;
        if (PlayerMe.Attribute.IsHaveActiveSkill)
        {
            for (int i = 0; i < PlayerMe.Attribute.ActiveSkills.Count; i++)
            {
                if (IsPlayerMe && GameData.DSkillData.ContainsKey(PlayerMe.Attribute.ActiveSkills[i].ID))
                {
                    if (spriteSkills[i] != null)
                        spriteSkills[i].spriteName = GameData.DSkillData[PlayerMe.Attribute.ActiveSkills[i].ID].PictureNo + "s";
                }
            }
        }

//		initLine();
    }

    //	public void ClearLine() {
    //		drawLine.ClearTarget();
    //	}
	
    //	private void initLine() {
    //		drawLine.ClearTarget();
    //			GameObject obj = GameObject.Find("PlayerInfoModel/Self0/PassMe");
    //			if (obj)
    //				drawLine.AddTarget(uiPassObjectGroup[0], obj);
    //
    //		drawLine.Show(true);
    //	}

    private GameObject getSkillRangeTarget()
    {
        if (GameStart.Get.TestMode == EGameTest.AttackA)
            return PlayerMe.PlayerRefGameObject;
        if (IsPlayerMe && GameData.DSkillData.ContainsKey(PlayerMe.ActiveSkillUsed.ID))
        {
            switch (GameData.DSkillData[PlayerMe.ActiveSkillUsed.ID].TargetKind)
            {
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

    private bool isCircleRange
    {
        get
        {
            if (IsPlayerMe && GameData.DSkillData.ContainsKey(PlayerMe.ActiveSkillUsed.ID))
            {
                if (GameData.DSkillData[PlayerMe.ActiveSkillUsed.ID].Kind == 171)
                    return false;
            }
            return true;
        }
    }

    private void showSkillHint(bool isShow, int index = 0)
    {
        if (isShow)
        {
            skillHintTime = GameConst.HintLongPressTime;
        }
        else
        {
            skillHint.Hide();
            skillHintTime = 0;
        }
        skillHintIndex = index;
    }

    private void showRange(EUIRangeType type, bool state)
    {
        skillRangeTarget = null;

        if (state && IsPlayerMe && !PlayerMe.IsUseActiveSkill && (GameController.Get.Situation == EGameSituation.GamerAttack ||
        GameController.Get.Situation == EGameSituation.NPCAttack))
        {
            switch (type)
            {
                case EUIRangeType.Skill:
                    isShowSkillRange = state;
                    isShowElbowRange = !state;
                    isShowPushRange = !state;
                    isShowStealRange = !state;

                    if (getSkillRangeTarget() != null)
                    {
                        skillRangeTarget = getSkillRangeTarget().transform;
                        if (GameData.DSkillData.ContainsKey(PlayerMe.ActiveSkillUsed.ID))
                        {
                            if (isCircleRange)
                            {
                                CourtMgr.Get.ShowRangeOfAction(state, 
                                    skillRangeTarget, 
                                    360, 
                                    GameData.DSkillData[PlayerMe.ActiveSkillUsed.ID].Distance(PlayerMe.ActiveSkillUsed.Lv)); 
                            }
                            else
                            {
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
                        PlayerMe.Attr.ElbowExtraAngle, 
                        PlayerMe.Attr.ElbowDistance); 
                    break;
                case EUIRangeType.Push:
                    isShowSkillRange = !state;
                    isShowElbowRange = !state;
                    isShowPushRange = state;
                    isShowStealRange = !state;
                    skillRangeTarget = PlayerMe.PlayerRefGameObject.transform;
                    eulor = 0;
                    nearP = GameController.Get.NpcSelectMe;
                    if (nearP)
                        eulor = MathUtils.FindAngle(PlayerMe.PlayerRefGameObject.transform, nearP.PlayerRefGameObject.transform.position);
                    CourtMgr.Get.ShowRangeOfAction(state, 
                        PlayerMe.PlayerRefGameObject.transform, 
                        PlayerMe.Attr.PushExtraAngle, 
                        PlayerMe.Attr.PushDistance,
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
                        PlayerMe.Attr.StealExtraAngle, 
                        PlayerMe.Attr.StealDistance,
                        eulor); 
                    break;
            }
        }
        else
        {
            CourtMgr.Get.ShowRangeOfAction(state);
            CourtMgr.Get.ShowArrowOfAction(state);
        }
    }

    public void DoAttackOut(GameObject go)
    {
        if (IsPlayerMe && !IsPlayerAttack)
        {
            if (PlayerMe.IsBallOwner)
            {
                //Elbow
                if (isShowElbowRange)
                    ResetRange();
            }
            else
            {
                //Push
                if (isShowPushRange)
                    ResetRange();
            }
        }
    }

    public void DoAttack(GameObject go, bool state)
    {
        if (IsPlayerMe && !IsPlayerAttack)
        {
            if (PlayerMe.IsBallOwner)
            {
                //Elbow
                if (!state)
                {
                    if (isShowElbowRange)
                        UIControllerState(EUIControl.Attack);
                }
                else
                {
                    ResetRange();
                    showRange(EUIRangeType.Elbow, true);
                }
            }
            else
            {
                //Push
                if (!state)
                {
                    if (isShowPushRange)
                        UIControllerState(EUIControl.Attack);
                }
                else
                {
                    ResetRange();
                    showRange(EUIRangeType.Push, true);
                }
            }
        }
    }

    //Defence
    public void DoBlock()
    {
        UIControllerState(EUIControl.Block);
    }

    public void DoStealOut(GameObject go)
    {
        if (isShowStealRange && !IsPlayerAttack)
            ResetRange();
    }

    public void DoSteal(GameObject go, bool state)
    {
        if (!IsPlayerAttack)
        {
            if (!state)
            {
                if (isShowStealRange)
                    UIControllerState(EUIControl.Steal);
            }
            else
            {
                ResetRange();
                showRange(EUIRangeType.Steal, true);
            }
			
        }
    }
	
    //Attack
    public void DoSkillOut(GameObject go)
    {
        if (isShowSkillRange)
            ResetRange();
    }

    public void DoSkill(GameObject go, bool state)
    {
        if (PlayerMe.Attribute.IsHaveActiveSkill && go && IsPlayerMe)
        {
            int id = -1;
            if (int.TryParse(go.name, out id) && id >= 0 && id < PlayerMe.Attribute.ActiveSkills.Count)
            {
                PlayerMe.ActiveSkillUsed = PlayerMe.Attribute.ActiveSkills[id];
                if (!state)
                if (isShowSkillRange)
                    UIControllerState(EUIControl.Skill);
                else
                    ResetRange();
				
                showRange(EUIRangeType.Skill, state);
                showSkillHint(state, id);
            }
        }
        else
            ResetRange();
    }

    public void OnSpeed()
    {
        #if DEBUG
        if (Time.timeScale == 1)
            Time.timeScale = 2;
        else if (Time.timeScale == 2)
            Time.timeScale = 4;
        else if (Time.timeScale == 4)
            Time.timeScale = 8;
        else if (Time.timeScale == 8)
            Time.timeScale = 16;
        else if (Time.timeScale == 16)
            Time.timeScale = 1;
        #else
        if (Time.timeScale == 1) 
            Time.timeScale = 2;
        else if (Time.timeScale == 2) 
			Time.timeScale = 4;
		else if (Time.timeScale == 4) 
			Time.timeScale = 0.5f;
		else if (Time.timeScale == 0.5f) 
            Time.timeScale = 1;
        #endif
		refreshSpeedLabel ();
        GameStart.Get.GameSpeed = Time.timeScale;
        GameController.Get.RecordTimeScale = Time.timeScale;
    }

	private void refreshSpeedLabel () {
		labelSpeed.text = "X" + Time.timeScale.ToString();
//		if(Time.timeScale == 1) {
//			labelSpeed.text = TextConst.S(10206);
//		} else if(Time.timeScale == 0.5f) {
//			labelSpeed.text = TextConst.S(10207);
//		} else if(Time.timeScale == 2) {
//			labelSpeed.text = TextConst.S(10208);
//		} else if(Time.timeScale == 4) {
//			labelSpeed.text = TextConst.S(10209);
//		}
	}

    public void DoShoot(GameObject go, bool state)
    {
        UIControllerState(EUIControl.Shoot, go, state);
    }

    public void DoPassChoose(GameObject obj, bool state)
    {
        UIControllerState(EUIControl.Pass, obj, state);
    }

    public void DoPassNone()
    {
        SetPassButton();
    }

    public void DoPassTeammateA(GameObject obj, bool state)
    {
        isPressA = state;
        UIControllerState(EUIControl.PassA, obj, state);
    }

    public void DoPassTeammateB(GameObject obj, bool state)
    {
        if (IsPlayerMe && isPressA)
        {
            PlayerMe.SetAnger(PlayerMe.Attribute.MaxAnger);
        }
        UIControllerState(EUIControl.PassB, obj, state);
    }

    public void OnReselect()
    {
        UIState(EUISituation.ReSelect);
    }

    public void OnPause()
    {
        if (Time.timeScale == 0)
            UIState(EUISituation.Continue);
        else
            UIState(EUISituation.Pause);
    }

    public void ResetGame()
    {
        UIState(EUISituation.Reset);
    }

    public void StartGame()
    {
        UIState(EUISituation.Start);
    }

    public void GameOver()
    {
        UIState(EUISituation.Finish);
    }

    public void RefreshSkillUI()
    {
        ShowSkillEnableUI(true);
        SetAngerUI(PlayerMe.Attribute.MaxAnger, PlayerMe.AngerPower, 0);
        runSkillValue();
    }

    public void ShowSkillEnableUI(bool isShow, int index = 0, bool isAngerFull = false, bool canUse = false)
    {
        if (IsPlayerMe)
        {
            if (PlayerMe.Attribute.IsHaveActiveSkill && index < PlayerMe.Attribute.ActiveSkills.Count)
            {
                if (isShow)
                {
                    if (GameController.Get.IsStart)
                        uiSkillEnables[index].SetActive((canUse && isAngerFull));
                    else
                        uiSkillEnables[index].SetActive(false);
                }
                else
                {
                    for (int i = 0; i < uiSkillEnables.Length; i++)
                    {
                        uiSkillEnables[i].SetActive(false);
                    }
                }
            }
        }
    }

    public void ShowAlleyoop(bool isShow, int teammate = 1)
    {
        if (!GameController.Get.StageData.IsTutorial)
        {
            if (isShow && GameController.Get.Situation == EGameSituation.GamerAttack)
            {
                if (teammate == 1)
                {
                    uiPassA.SetActive(!isShow);
                    uiAlleyoopA.SetActive(isShow);
                }
                else if (teammate == 2)
                {
                    uiPassB.SetActive(!isShow);
                    uiAlleyoopB.SetActive(isShow);
                }
            }
            else
            {
                uiPassA.SetActive(true);
                uiAlleyoopA.SetActive(false);
                uiPassB.SetActive(true);
                uiAlleyoopB.SetActive(false);
            }
        }
    }

    public void SetAngerUI(float max, float anger, int count)
    {
        timeForce = 0;
        uiSpriteFull.SetActive(false);
        showViewForceBar(GameController.Get.IsStart);

        if (max > 0)
        {
            uiForceNum.text = anger + "/[13CECEFF]" + max + "[-]";
            oldForceValue = spriteForce.fillAmount;
            newForceValue = anger / max;
            if (count != 0)
            {
                dcCount += count;
                baseForceValue = (newForceValue - oldForceValue) / dcCount;
            }
            if (newForceValue >= 1)
            {
                uiSpriteFull.SetActive(true);
            }
            else if (newForceValue <= 0)
            {
                oldForceValue = 0;
                newForceValue = 0;
                spriteForce.fillAmount = 0;
                spriteForceFirst.fillAmount = 0;
            }
        }
        runSkillValue();
    }

    private void runSkillValue()
    {
        if (IsPlayerMe && PlayerMe.Attribute.IsHaveActiveSkill)
            for (int i = 0; i < PlayerMe.Attribute.ActiveSkills.Count; i++)
                if (uiButtonSkill[i].activeSelf)
					spriteEmptys[i].fillAmount = 1 - PlayerMe.Attribute.MaxAngerPercent(PlayerMe.Attribute.ActiveSkills[i].ID, newForceValue * PlayerMe.Attribute.MaxAnger, PlayerMe.Attribute.ActiveSkills[i].Lv);
    }

    public void PlusScore(int team, int score)
    {
        Scores[team] += score;
        if (!GameController.Get.IsFinish)
            CourtMgr.Get.SetScoreboards(team, Scores[team]);

        TweenRotation rotateTopScore = TweenRotation.Begin(labelTopLeftScore[team].gameObject, 0.5f / Time.timeScale, Quaternion.identity);
        rotateTopScore.delay = 1f / Time.timeScale;
        rotateTopScore.from = Vector3.zero;
        rotateTopScore.to = new Vector3(0, 720, 0);
        labelTopLeftScore[team].text = Scores[team].ToString();
    }

    public void ChangeControl(bool IsAttack)
    {
        if (IsAttack)
            UIMaskState(EUIControl.AttackA);
        else
            UIMaskState(EUIControl.AttackB);
    }

    public bool SetUIJoystick(PlayerBehaviour p = null, bool isShow = false)
    {
        if (IsPlayerMe && p == PlayerMe)
        {
            if (GameController.Get.IsStart)
            {
                return true;
            }
        }
        return false;
    }

    public void AddForceReviveValue(float max, float anger, int count)
    {
        oldForceValue = (anger / max);
        newForceValue = oldForceValue;
        spriteForce.fillAmount = oldForceValue;
        uiForceNum.text = anger + "/[13CECEFF]" + max + "[-]";
        runSkillValue();
    }

    public bool AddForceValue()
    {
        oldForceValue += baseForceValue;
        if (dcCount >= 0)
            dcCount--;

        oldForceValue = Mathf.Clamp(oldForceValue, 0, newForceValue);
        spriteForce.fillAmount = oldForceValue;
        if (!SkillDCExplosion.Get.IsHaveDC)
        {
            dcCount = 0;
            spriteForce.fillAmount = newForceValue;
        }
		
        runSkillValue();
        return true;
    }

    public bool UICantUse(PlayerBehaviour p = null)
    {
        if (IsPlayerMe)
        {
            if (p == PlayerMe)
            {
                if (GameController.Get.IsStart)
                {
                    SetPassButton();
                    ShowSkillEnableUI(false);
                    ResetRange();
                    spriteAttack.gameObject.SetActive(true);
                }
                return true;
            }
            else
            {
                if (p.Team == PlayerMe.Team && p.crtState == EPlayerState.Alleyoop)
                    SetPassButton();
				
                return false;
            }
        }
        return false;
    }

    public bool OpenUIMask(PlayerBehaviour p = null)
    {
        if (IsPlayerMe && p == PlayerMe)
        {
            if (GameController.Get.IsStart)
            { 
                ShowAlleyoop(false);
                CourtMgr.Get.ShowRangeOfAction(false);
                CourtMgr.Get.ShowArrowOfAction(false);
                if (GameController.Get.Situation == EGameSituation.GamerAttack)
                    UIMaskState(EUIControl.AttackA);
                else
                    UIMaskState(EUIControl.AttackB);
            }

            return true;
        }
        else
        {
            if (IsPlayerMe && p.Team == PlayerMe.Team && p.crtState == EPlayerState.Alleyoop)
                ShowAlleyoop(false);

            return false;
        }
    }

    public void SetPassButton()
    {
        if (GameStart.Get.TestMode != EGameTest.None && GameStart.Get.TestMode != EGameTest.Pass && GameStart.Get.TestMode != EGameTest.Alleyoop)
            return;

        if (GameController.Get.IsShowSituation)
            return;

        int who = GameController.Get.GetBallOwner;
        switch (who)
        {
            case (int)EUIPassType.MeBallOwner:
                ResetRange();
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
                if (GameController.Get.Situation == EGameSituation.NPCAttack || GameController.Get.Situation == EGameSituation.NPCPickBall)
                {
                    viewPass.SetActive(false);
                }
                else
                {
                    viewPass.SetActive(true);
                }
                break;
        }
    }

    public void UIMaskState(EUIControl controllerState)
    {
        if (joystickController)
            joystickController.UIMaskState(controllerState);
        
        switch (controllerState)
        {
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
                if (IsPlayerMe && PlayerMe.IsBallOwner)
                {
                    //Elbow Attack
                    uiShoot.SetActive(false);
                    spriteAttack.gameObject.SetActive(true);
                    uiDefenceGroup[0].SetActive(false);
                    uiDefenceGroup[1].SetActive(false);
                    uiPassObjectGroup[0].SetActive(false);
                    uiPassObjectGroup[1].SetActive(false);
                    uiPassObjectGroup[2].SetActive(false);
                }
                else
                {
                    //Push Deffence
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
                ShowSkillEnableUI(false);
                spriteAttack.gameObject.SetActive(false);
                uiDefenceGroup[0].SetActive(false);
                uiDefenceGroup[1].SetActive(true);
                break;
            case EUIControl.Steal:
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

    public void UIControllerState(EUIControl controllerState, GameObject go = null, bool state = false)
    {
        if (GameController.Get.IsShowSituation || !PlayerMe.CanPressButton)
            return;
			
        if (IsPlayerMe && (GameController.Get.Situation == EGameSituation.GamerAttack || GameController.Get.Situation == EGameSituation.NPCAttack))
        {
            bool noAI = false;
            switch (controllerState)
            {
                case EUIControl.Skill:
                    if (PlayerMe.Attribute.IsHaveActiveSkill)
                        noAI = GameController.Get.OnSkill(PlayerMe.ActiveSkillUsed);
                    if (noAI)
                        UIMaskState(EUIControl.Skill);
					
                    break;

                case EUIControl.Attack:
                    if (PlayerMe.IsBallOwner)
                    {
                        //Elbow
                        if (isPressElbowBtn &&
                        !PlayerMe.IsFall &&
                        GameController.Get.Situation == EGameSituation.GamerAttack)
                        {
                            noAI = GameController.Get.DoElbow();
                            if (noAI)
                            {
                                UIMaskState(EUIControl.Attack);
								isShowElbowRange = true;
                            }
                            else
                                ResetRange();
                        }
                    }
                    else
                    {
                        //Push
                        if (isCanDefenceBtnPress &&
                        !PlayerMe.IsFall &&
                        (GameController.Get.Situation == EGameSituation.NPCAttack || GameController.Get.Situation == EGameSituation.GamerAttack) &&
                        PlayerMe.CanUseState(EPlayerState.Push0))
                        {
                            noAI = GameController.Get.DoPush(nearP);
                            if (noAI)
                            {
                                UIMaskState(EUIControl.Attack);
								isShowPushRange = true;
                            }
                            else
                                ResetRange();
                        }
                    }

                    break;
                case EUIControl.Block:
                    if (isCanDefenceBtnPress &&
                    !PlayerMe.IsFall &&
                    GameController.Get.Situation == EGameSituation.NPCAttack)
                    {
                        noAI = GameController.Get.DoBlock();
                        if (noAI)
                            UIMaskState(EUIControl.Block);
                    }

                    break;
                case EUIControl.Steal:
                    if (isCanDefenceBtnPress &&
                    !PlayerMe.IsFall &&
                    GameController.Get.Situation == EGameSituation.NPCAttack &&
                    GameController.Get.StealBtnLiftTime <= 0 &&
                    PlayerMe.CanUseState(EPlayerState.Steal0))
                    {
                        noAI = GameController.Get.DoSteal();
                        if (noAI)
                        {
                            UIMaskState(EUIControl.Steal);
							isShowStealRange = true;
                        }
                        else
                            ResetRange();
                    }
                    break;
                case EUIControl.Shoot:
                    if (GameController.Get.IsCanUseShootDoubleClick())
                    {
                        if (state)
                        {
                            int index = GameController.Get.GetShootPlayerIndex();
                            if (index != -1 && index < UIDoubleClick.Get.DoubleClicks.Length && UIDoubleClick.Get.DoubleClicks[index].Enable)
                            {
                                UIDoubleClick.Get.ClickStop(index);
                            }
                        }
                    }
                    else
                    {
                        if (PlayerMe.IsBallOwner &&
                        GameController.Get.Situation == EGameSituation.GamerAttack &&
                        !PlayerMe.IsFall &&
                        !PlayerMe.CheckAnimatorSate(EPlayerState.MoveDodge0) &&
                        !PlayerMe.CheckAnimatorSate(EPlayerState.MoveDodge1) &&
                        !PlayerMe.IsBlock)
                        {
                            if (state && PlayerMe.IsFakeShoot && isShootAvailable)
                                isShootAvailable = false;


                            if (!state && shootBtnTime > 0 && isShootAvailable)
                            {
                                if (GameController.Get.BallOwner != null)
                                {
                                    if (PlayerMe.IsBallOwner)
                                        UIMaskState(EUIControl.Shoot);

                                    shootBtnTime = ButtonBTime;
                                }

                                noAI = GameController.Get.DoShoot(false);
                            }
                            else if (!state && !isShootAvailable)
                                isShootAvailable = true;
						
                            isPressShootBtn = state;
                        }
                        else if (!PlayerMe.IsBallOwner)
                            noAI = GameController.Get.DoShoot(true);
                    }

                    break;
                case EUIControl.Pass:
                    if (!PlayerMe.IsBallOwner)
                    {
                        if ((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(0))
                        {
                            UIMaskState(EUIControl.Pass);
                            noAI = true;
                        }
                    }

                    break;
                case EUIControl.PassA:
                    if (GameController.Get.GetBallOwner != 1)
                    if ((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(1))
                        UIMaskState(EUIControl.PassA);

                    break;
                case EUIControl.PassB:
                    if (GameController.Get.GetBallOwner != 2)
                    if ((!GameController.Get.IsShooting || GameController.Get.IsCanPassAir) && GameController.Get.DoPass(2))
                        UIMaskState(EUIControl.PassB);

                    break;
            }

            if (noAI)
                PlayerMe.SetManually();
        }
    }

    public void UIState(EUISituation situation)
    {
        ResetRange();
        switch (situation)
        {
            case EUISituation.ShowTwo:
                viewPass.SetActive(false);
                viewStart.SetActive(true);
                viewTopLeft.SetActive(false);
                viewBottomRight.SetActive(false);
                gameJoystick.visible = false;
                controlButtonGroup[0].SetActive(false);
                controlButtonGroup[1].SetActive(false);
//			drawLine.IsShow = false;
                break;
            case EUISituation.Opening:
                showUITime();
                viewPass.SetActive(true);
                viewTopLeft.SetActive(true);
                viewBottomRight.SetActive(true);
                controlButtonGroup[0].SetActive(true);
                controlButtonGroup[1].SetActive(false);
                gameJoystick.visible = true;
                gameJoystick.activated = false;
//			drawLine.IsShow = true;
                showViewForceBar(true);
                if (PlayerMe && PlayerMe.Attribute.IsHaveActiveSkill)
                {
                    for (int i = 0; i < PlayerMe.Attribute.ActiveSkills.Count; i++)
                    {
                        uiButtonSkill[i].SetActive((i < PlayerMe.Attribute.ActiveSkills.Count));
                    }
                }
                break;
            case EUISituation.Start:
                AudioMgr.Get.PlaySound(SoundType.SD_BattleStart_Btn);
                GameController.Get.PlayCount++;
                InitUI();

                viewStart.SetActive(false);
                viewTopLeft.SetActive(true);
                viewBottomRight.SetActive(true);

                if (!GameController.Get.StageData.IsTutorial || !GameStart.Get.ConnectToServer)
                {
                    showGameJoystick(true);
                }

                viewPass.SetActive(GameController.Get.Situation == EGameSituation.GamerAttack);
                controlButtonGroup[0].SetActive(GameController.Get.Situation == EGameSituation.GamerAttack);
                controlButtonGroup[1].SetActive(GameController.Get.Situation != EGameSituation.GamerAttack);

                SetPassButton();
                CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.Start);
                GameController.Get.StartGame();
                initJoystickPos();
//			drawLine.IsShow = false;
                break;
            case EUISituation.Pause:
                if (!viewStart.activeInHierarchy)
                {
                    Time.timeScale = 0;
                    viewBottomRight.SetActive(false);
                    showViewForceBar(false);

                    showGameJoystick(false);
                    ShowSkillEnableUI(false);

                    if (UIPassiveEffect.Visible)
                        UIPassiveEffect.UIShow(false);

                    for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
                    {
                        if (GameController.Get.GameRecord.PlayerRecords != null)
                        {
                            if (i < GameController.Get.GameRecord.PlayerRecords.Length)
                            {
                                GameController.Get.GameRecord.PlayerRecords[i] = GameController.Get.GamePlayers[i].GameRecord;
                            }
                        }
                    }
                    UIGamePause.Get.SetGameRecord(ref GameController.Get.GameRecord);
                    AudioMgr.Get.PauseGame(true);
                }
                break;
            case EUISituation.Continue:
                if (GameController.Get.IsStart)
                {
                    Time.timeScale = 1;
                    GameController.Get.RecordTimeScale = Time.timeScale;
                    viewBottomRight.SetActive(true);
                    showViewForceBar(true);

                    showGameJoystick(true);
                    ShowSkillEnableUI(true);
			
                    UIPassiveEffect.UIShow(!UIPassiveEffect.Visible);
                    UIGamePause.UIShow(false);
                    initJoystickPos();
                    AudioMgr.Get.PauseGame(false);
                }
                break;
            case EUISituation.Finish:
                Time.timeScale = 1;
                GameController.Get.RecordTimeScale = Time.timeScale;
                viewBottomRight.SetActive(false);
                viewTopLeft.SetActive(false);

//			showGameJoystick(false);
                gameJoystick.gameObject.SetActive(false);
                showViewForceBar(false);
                GameController.Get.IsStart = false;

                break;
            case EUISituation.Reset:
                UIShow(false);
                InitUI();
                GameController.Get.Reset();
                UIPassiveEffect.Get.Reset();
                CourtMgr.Get.SetScoreboards(0, Scores[0]);
                CourtMgr.Get.SetScoreboards(1, Scores[1]);
			
                SetPassButton();
                viewStart.SetActive(true);
                viewBottomRight.SetActive(false);
                showGameJoystick(false);

                dcCount = 0;
                if (IsPlayerMe && PlayerMe.Attribute.IsHaveActiveSkill)
                {
                    for (int i = 0; i < PlayerMe.Attribute.ActiveSkills.Count; i++)
                    {
                        spriteEmptys[i].fillAmount = 0;
                        uiSkillEnables[i].SetActive(false);
                    }
                }
			//no tutorial
                if (GameController.Get.Situation == EGameSituation.Opening)
                {
                    UIState(EUISituation.Opening);
                }
			
                if (GameData.DStageTutorial.ContainsKey(GameController.Get.StageData.ID))
                    GamePlayTutorial.Get.SetTutorialData(GameController.Get.StageData.ID);
			
                initJoystickPos();
                break;
            case EUISituation.ReSelect:
                Time.timeScale = 1;
                UIGameResult.UIShow(false);
                SceneMgr.Get.ChangeLevel(ESceneName.SelectRole);
                break;
        }
    }

    private void runForceValue()
    {
        if (IsPlayerMe && spriteForce.fillAmount != newForceValue)
        {
            timeForce += Time.fixedDeltaTime;
            if (newForceValue > oldForceValue)
                spriteForceFirst.fillAmount = Mathf.Lerp(oldForceValue, newForceValue, timeForce);
            else
            {
                spriteForce.fillAmount = newForceValue;
                spriteForceFirst.fillAmount = newForceValue;
            }
        }
    }

    private void showViewForceBar(bool isShow)
    {
        if (!IsPlayerMe)
        {
            viewTopRight.SetActive(false);
        }
        else
        {
            if (PlayerMe.Attribute.IsHaveActiveSkill)
            {
                viewTopRight.SetActive(isShow);
            }
            else
            {
                viewTopRight.SetActive(false);
            }
        }
    }

    private void showSkillRefUI(bool isShow)
    {
        if (IsPlayerMe)
        {
            if (PlayerMe.Attribute.IsHaveActiveSkill)
                showViewForceBar(isShow);
            else
                showViewForceBar(false);
        }
    }

    private void judgePlayerScreenPosition()
    {
        if (GameController.Get.IsStart && IsPlayerMe &&
        (GameController.Get.Situation == EGameSituation.GamerAttack || GameController.Get.Situation == EGameSituation.NPCAttack))
        {

            playerInCameraX = CameraMgr.Get.CourtCamera.WorldToScreenPoint(PlayerMe.PlayerRefGameObject.transform.position).x;
            playerInCameraY = CameraMgr.Get.CourtCamera.WorldToScreenPoint(PlayerMe.PlayerRefGameObject.transform.position).y;

            if (playerInCameraX > -50 && playerInCameraX < Screen.width + 100 &&
            playerInCameraY > -90 && playerInCameraY < Screen.height + 100)
            {
                uiPlayerLocation.SetActive(false);
            }
            else
            {
                uiPlayerLocation.SetActive(true);
                playerInBoardX = PlayerMe.PlayerRefGameObject.transform.position.z;
                playerInBoardY = PlayerMe.PlayerRefGameObject.transform.position.x;

                playerX = 15 - playerInBoardX;
                playerY = 11 - playerInBoardY;

                playerScreenPos = new Vector2((playerX * baseValueX) - 640, (playerY * baseValueY) * (-1));
                if (playerScreenPos.y > -330 && playerScreenPos.y < 330 && playerInCameraX < 0)
                    playerScreenPos.x = -610;
                else if (playerScreenPos.y > -330 && playerScreenPos.y < 330 && playerInCameraX >= Screen.width)
                    playerScreenPos.x = 610;
                else if (playerScreenPos.x > 610)
                    playerScreenPos.x = 610;
                else if (playerScreenPos.x < -610)
                    playerScreenPos.x = -610;

                if (playerScreenPos.x > -610 && playerScreenPos.x < 610 && playerInCameraY < 0)
                    playerScreenPos.y = -330;
                else if (playerScreenPos.y < -330)
                    playerScreenPos.y = -330;

                float angle = 0f;

                if (playerScreenPos.x == -610 && playerScreenPos.y == -330)
                    angle = -135;
                else if (playerScreenPos.x == 610 && playerScreenPos.y == -330)
                    angle = -45;
                else if (playerScreenPos.x == 610)
                    angle = 0;
                else if (playerScreenPos.x == -610)
                    angle = 180;
                else if (playerScreenPos.y == -330)
                    angle = -90;
                uiPlayerLocation.transform.localPosition = new Vector3(playerScreenPos.x, playerScreenPos.y, 0);
                uiPlayerLocation.transform.localEulerAngles = new Vector3(0, 0, angle);
            }
        }
        else
            uiPlayerLocation.SetActive(false);
    }

    public void CloseStartButton()
    {
//		drawLine.IsShow = false;
        viewStart.SetActive(false);
    }

    public void TutorialUI(int flag)
    {
        if (flag == 0)
        {
            for (int i = 0; i < uiTutorial.Length; i++)
                uiTutorial[i].SetActive(false);

            viewTopRight.SetActive(false);
//			ClearLine();
        }
        else
        {
            int[] temp = AI.BitConverter.Convert(flag.ToString());
            if (temp != null)
            {
                for (int i = 0; i < uiTutorial.Length; i++)
                {
                    bool v = i < temp.Length && temp[i] > 0;

                    if (i >= 5 && i <= 7 && v)
                        viewTopRight.SetActive(true);

                    uiTutorial[i].SetActive(v);

                    if (i < uiTutorial2.Length)
                        uiTutorial2[i].SetActive(v);
                }

//				if (!uiPassObjectGroup[0].activeInHierarchy)
//					ClearLine();

                if (uiTutorial[8].activeInHierarchy)
                {
                    gameJoystick.visible = true;
                    gameJoystick.activated = true;
                }

                for (int i = 0; i < uiButtonSkill.Length; i++)
                    if (uiButtonSkill[i].activeInHierarchy)
                        return;

                viewTopRight.SetActive(false);
            }
            if (PlayerMe.IsElbow)
                uiPassA.SetActive(true);
        }
    }

    public void SetJoystick(PlayerBehaviour player)
    {
        gameJoystick.AttachPlayer(player);
        if (joystickController)
            joystickController.Joysticker = player;
    }

    public bool IsPlayerMe
    {
        get { return (PlayerMe != null); }
    }

    public bool IsPlayerAttack
    {
        get
        {
            if (IsPlayerMe)
            {
                if (PlayerMe.IsPush || PlayerMe.IsSteal || PlayerMe.IsElbow)
                    return true;
            } 
            return false;
        }
    }

    public bool isStage
    {
        get { return StageTable.Ins.HasByID(GameData.StageID); }
    }
}
