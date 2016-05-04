using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using DG.Tweening;
using GameStruct;
using GameEnum;
using G2;
using JetBrains.Annotations;

public delegate void OnPlayerAction1(PlayerBehaviour player,bool isActive);

//public delegate void OnPlayerAction2(PlayerBehaviour player,bool speedup);

public delegate bool OnPlayerAction3(PlayerBehaviour player,bool speedup);

public delegate bool OnPlayerAction4(PlayerBehaviour player,EPlayerState state);

public delegate void OnPlayerAction5(float max,float anger,int count);


public class PlayerBehaviour : MonoBehaviour
{
    public EGameTest TestMode = EGameTest.None;
    public ETimerKind TimerKind;
    public bool IsDebugAnimation;

    public OnPlayerAction1 OnShooting = null;
    public Func<PlayerBehaviour, bool> OnPass = null;
    public Func<PlayerBehaviour, bool> OnBlockJump = null;
    public Func<PlayerBehaviour, bool> OnBlocking = null;
    public Func<PlayerBehaviour, bool> OnBlockCatching = null;
    public Func<PlayerBehaviour, bool> OnDunkBasket = null;
    public Func<PlayerBehaviour, bool> OnDunkJump = null;
    public Func<PlayerBehaviour, bool> OnBlockMoment = null;
    public Func<PlayerBehaviour, bool> OnFakeShootBlockMoment = null;
    public Func<PlayerBehaviour, bool> OnFall = null;
    public Func<PlayerBehaviour, bool> OnPickUpBall = null;
    public Func<PlayerBehaviour, bool> OnGotSteal = null;
    public Func<PlayerBehaviour, bool> OnOnlyScore = null;
    public Func<PlayerBehaviour, bool> OnUI = null;
    public Func<PlayerBehaviour, bool> OnUICantUse = null;
    public OnPlayerAction5 OnUIAnger = null;
    public OnPlayerAction5 OnReviveAnger = null;
    public OnPlayerAction4 OnDoubleClickMoment = null;
    public OnPlayerAction3 OnUIJoystick = null;
    [HideInInspector]public bool IsJumpBallPlayer = false;
    [HideInInspector]public GameObject PlayerRefGameObject;
    [HideInInspector]public int ShowPos = -1;

    [Tooltip("Just for Debug."), UsedImplicitly]
    public string TacticalName = "";

    private const float MoveCheckValue = 1;
    private bool stop = false;
    private bool NeedResetFlag = false;
    [HideInInspector]public bool CanUseTipIn = false;
    private int MoveTurn = 0;
    private float moveStartTime = 0;
    private float animationSpeed = 0;
    private float MoveMinSpeed = 0.5f;
    private float canDunkDis = 30f;
    private readonly Queue<TMoveData> moveQueue = new Queue<TMoveData>();
    private Vector3 translate;
    [HideInInspector]public Rigidbody PlayerRigidbody;
    private GameObject selectTexture;
    private GameObject DefPoint;
    private GameObject TopPoint;
    [HideInInspector]public GameObject CatchBallPoint;
	private GameObject blockTrigger;
	private GameObject pushThroughTigger;
	private GameObject interceptTrigger;
    private GameObject dashSmoke;
	private GameObject reboundTrigger;
    private BlockCatchTrigger blockCatchTrigger;
    [HideInInspector]public GameObject AIActiveHint = null;
    [HideInInspector]public GameObject DoubleClick = null;
    [HideInInspector]public GameObject DummyBall;
    [HideInInspector]public UISprite SpeedUpView = null;
    [HideInInspector]public Animator SpeedAnimator = null;
    private bool isSpeedStay = true;
    [HideInInspector]public UISprite AngerView = null;
    [HideInInspector]public GameObject AngryFull = null;
    [HideInInspector]public GameObject BodyHeight;
    private GameObject bodyTrigger;
    private GameObject mDebugSphere;

	[HideInInspector]public Transform Pelvis;
    public TPlayerAttribute Attr = new TPlayerAttribute(); // 球員最終的數值.
	public TPlayer Attribute; // 對應到 Server 的 Team.Player 資料結構, greatplayer 表格 + 數值裝 + 潛能點數.
	public TPlayer BaseAttribute = new TPlayer(); //球員沒加入base跟buff的值 
	private int shootNoScore;
    public int shootNoScoreLimit;
	private int shootScore;
    public int shootScoreLimit;
    [HideInInspector]public TScoreRate ScoreRate;
    public TGamePlayerRecord GameRecord = new TGamePlayerRecord();
    [HideInInspector] public ETeamKind Team;

    public EPlayerPostion Index;
    public EPlayerPostion Postion;

    private readonly StatusTimer mManually = new StatusTimer();

    public EGameSituation situation = EGameSituation.None;

    public EPlayerState CurrentState = EPlayerState.Idle;
    public EAnimatorState CurrentAnimatorState = EAnimatorState.Idle;

    private Transform[] DefPointAy = new Transform[8];
    private GameObject defPointCopy;
    /// <summary>
    /// 這是避免近距離時, 人物不斷轉向的問題, 而設計的解決方案.(這不好, 應該要換作法才對)
    /// </summary>
    public readonly StatusTimer CantMoveTimer = new StatusTimer();
    public readonly StatusTimer Invincible = new StatusTimer();

    /// <summary>
    /// 抄截冷卻時間.
    /// </summary>
    public readonly CountDownTimer StealCD = new CountDownTimer(GameConst.CoolDownStealTime);

    /// <summary>
    /// 推人冷卻時間.
    /// </summary>
    public readonly CountDownTimer PushCD = new CountDownTimer(GameConst.CoolDownPushTime);

    /// <summary>
    /// Elbow 冷卻時間.
    /// </summary>
    public readonly CountDownTimer ElbowCD = new CountDownTimer(GameConst.CoolDownElbowTime);

    public float JumpHight = 450f;
    public int MoveIndex = -1;
	public bool isJoystick = false; //搖桿的判斷，不是指玩家
	public bool IsGameJoysticker = false;//玩家的判斷，不是指搖桿
//    [CanBeNull]
//    [HideInInspector]public PlayerAI AI = null;
    private PlayerBehaviour defencePlayer = null;
    [HideInInspector]public float CloseDef = 0;
    public bool AutoFollow = false;
    public bool NeedShooting = false;
        
    //Block
    private bool isBlock = false;
    private Vector3 skillMoveTarget;
    private Vector3 skillFaceTarget;
    private bool isDunkBlock;

    //For Active
    private bool isFakeShoot = false;
    //Push
    public bool IsPushCalculate = false;
    //Elbow
    public bool IsElbowCalculate = false;
    //Steal
    public bool IsStealCalculate = false;
	//MoveDodge CoolDown
	public float CoolDownCrossover = 0;

    //Skill
    [HideInInspector]public SkillController PlayerSkillController;
    private ESkillKind skillKind;
    // For Shoot and Layup
    private bool isUsePass = false;
    private AnimationEvent animatorEvent;

    //Active
    private float angerValue = 0;

    //ShowWord
	[HideInInspector]public GameObject ShowWord;
    private bool firstDribble = true;
    private bool isCanCatchBall = true;
    private bool isSpeedup = false;

    /// <summary>
    /// 移動體力.
    /// </summary>
    private float mMovePower = 0;
    private float mMaxMovePower = 0;
    private float mMovePowerTime = 0;
    private Vector2 MoveTarget;
//    private float dis;
    private bool canSpeedup = true;
    private float SlowDownTime = 0;
    public float DribbleTime = 0;

    //SkillEvent
    private bool isSkillShow = false;

    //Camera
    private float yAxizOffset = 0;

    //Select
	[HideInInspector]public GameObject SelectMe;

    /// <summary>
    /// true: 可以做空中傳球;
    /// </summary>
    private bool mIsPassAirMoment = false;

    /// <summary>
    /// <para> 控制某些動作一定要撥完, 比如 Push, Block ... </para>
    /// <para> true: 可以切換到下一個動作; false: 不能切換到下一個動作 </para>
    /// <para> 當強制要撥完的動作撥完後, 就會設值為 true. </para>
    /// </summary>
    private bool mStateChangable = true;

	[HideInInspector]public AnimatorController AnimatorControl;

	private float recoverTime = 1;
	/// <summary>
	/// 如果是玩家的話會有建築物影響
	/// </summary>
	public int SelfAPMax = 0;
	public float SelfAPGrowth = 0;
	public int SelfAPBegin = 0;

	//建築物＋BaseAttr
	private int apMax = 0;
	//每秒回復士氣的值
	private float apGrowth = 0.5f;
	//起手士氣
	private int apBegin = 0;
	public int APBegin {
		get{return apBegin;}
	}

	/// <summary>
	/// 倒數一分鐘AP成長速度會改變
	/// </summary>
	private float extraAPIncrease = 0;
	public float ExtraAPIncrease {
		set {extraAPIncrease = value;}
	}

	public void ReviveAnger(float value)
	{
		angerValue += value;
		if (angerValue > TotalMaxAnger)
		{
			angerValue = TotalMaxAnger;
		}
		if (OnReviveAnger != null)
			OnReviveAnger(TotalMaxAnger, angerValue, 0);
	}

	private void angerRecoveryUpdate () {
		if(IsGameJoysticker) {
			if(Attribute.IsHaveActiveSkill && AngerPower < TotalMaxAnger) {
				if(recoverTime > 0) {
					recoverTime -= Time.deltaTime;	
					if(recoverTime <= 0) {
						recoverTime = GameConst.AngerReviveTime;
						ReviveAnger(apGrowth + extraAPIncrease);
					}
				}
			}
		}
	}

	//會另外寫在這是因為建築物也會影響士氣最大值
	public int TotalMaxAnger {
		get {
			return Attribute.BaseMaxAnger + apMax;
		}
	}

    public void SetAnger(int value, GameObject target = null, GameObject parent = null)
    {
        int v = (int)(Mathf.Abs(value) / 2);
        if (v <= 0)
            v = 0;
        
        if (situation != EGameSituation.End && Attribute.ActiveSkills.Count > 0)
        {
			if (IsGameJoysticker && value > 0)
            {
                if (target)
                    SkillDCExplosion.Get.BornDC(v, target, CameraMgr.Get.SkillDCTarget, parent);
            }
        }
        angerValue += value;
		if (angerValue > TotalMaxAnger)
        {
			angerValue = TotalMaxAnger;
        }
        
        if (angerValue < 0)
            angerValue = 0;
        
        if (Team == ETeamKind.Self && Index == 0)
        {
            if (OnUIAnger != null)
				OnUIAnger(TotalMaxAnger, angerValue, v);
            if (value > 0)
                GameRecord.AngerAdd += value;
        }
    }

	public int ShootNoScore {
		get {return shootNoScore;}
		set { shootNoScore = value;}
	}

	public bool IsNeedScore {
		get {
			if(shootNoScore > shootNoScoreLimit) {
				shootNoScore = 0;
				return true;
			} else 
				return false;
			
		}
	}

	public int ShootScore {
		get {return shootScore;}
		set {shootScore = value;}
	}

	public bool IsNeedNoScore {
		get {
			if(shootScore > shootScoreLimit) {
				shootScore = 0;
				return true;
			} else 
				return false;
			
		}
	}

    public bool Pause
    {
        set{ AnimatorControl.Speed = value == true ? GameConst.Min_TimePause : 1;}
        get{ return AnimatorControl.Speed == GameConst.Min_TimePause;}
    }

    public float TimeSpeed
    {
        set{ AnimatorControl.Speed = value;}
        get{ return AnimatorControl.Speed;}
    }

    public void SetSlowDown(float Value)
    {
        if (SlowDownTime == 0)
        {
            SlowDownTime += Time.time + Value;
			Attr.SpeedValue = Mathf.Max(GameData.BaseAttr[Attribute.AILevel].SpeedValue * GameConst.SlowDownValue, GameConst.SlowDownValueMin);
        }
    }

    void OnDestroy()
    {
        moveQueue.Clear();

        if (DoubleClick)
            Destroy(DoubleClick);

        if (defPointCopy)
            Destroy(defPointCopy);

        if (blockTrigger)
            Destroy(blockTrigger);

		if(pushThroughTigger)
			Destroy (pushThroughTigger);

		if (interceptTrigger)
			Destroy(interceptTrigger);

		if (reboundTrigger)
			Destroy(reboundTrigger);		
        
        if (AnimatorControl)
            Destroy(AnimatorControl);

        if (animatorEvent != null)
            animatorEvent = null;

        if (BodyHeight)
            Destroy(BodyHeight);

        if (bodyTrigger)
            Destroy(bodyTrigger);
    }

    void Awake()
    {
        mManually.TimeUpListener += manuallyTimeUp;

        PlayerRefGameObject = gameObject;
        LayerMgr.Get.SetLayerAndTag(PlayerRefGameObject, ELayer.Player, ETag.Player);
        PlayerSkillController = gameObject.AddComponent<SkillController>();

        InitAnmator();
        PlayerRigidbody = PlayerRefGameObject.GetComponent<Rigidbody>();
        ScoreRate = new TScoreRate(1);
        animatorEvent = new UnityEngine.AnimationEvent();
        DashEffectEnable(false);
    }

    void Start()
    {
        mDebugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mDebugSphere.name = gameObject.name + ".Target";
        if (mDebugSphere.GetComponent<SphereCollider>())
            mDebugSphere.GetComponent<SphereCollider>().enabled = false;
				
        mDebugSphere.GetComponent<MeshRenderer>().enabled = IsDebugAnimation;
    }

    private void manuallyTimeUp()
    {
        clearMoveData();

        if (AIActiveHint)
            AIActiveHint.SetActive(true);
    }

    public void SetTimerTime(float time)
    {
        if (time == 0)
            PlayerRefGameObject.transform.DOPause();
        else
            PlayerRefGameObject.transform.DOPlay();
    }

    public void InitAttr()
    {
        GameRecord.Init();
        GameRecord.ID = Attribute.ID;

        initSkill();
		BaseAttribute = Attribute;
		initAttr();
		/*
		 * 必進的校正公式(記錄不進)    100 / (玩家換算後的  “實際命中率”  )
		   必不進的校正公式（紀錄進球）   (玩家換算後的  “實際命中率”  ) / 10
		*/
		shootNoScoreLimit = Mathf.RoundToInt(100f / (GameFunction.ShootingCalculate(this, Attr.PointRate2, 0, 0, 11)));
		shootScoreLimit = Mathf.RoundToInt((GameFunction.ShootingCalculate(this, Attr.PointRate2, 0, 0, 11)) / 10f);

		if (Attr.StaminaValue > 0)
			setMovePower(Attr.StaminaValue);
    }

    private void initAttr()
    {
        if (GameData.BaseAttr.Length <= 0 || Attribute.AILevel < 0 || Attribute.AILevel >= GameData.BaseAttr.Length)
        {
            Debug.LogErrorFormat("initialize attributes fail, BaseAttr:{0}, AILevel:{1}.", GameData.BaseAttr.Length, Attribute.AILevel);
            return;
        }

		/*
		 * 必進的校正公式(記錄不進)    100 / (玩家換算後的  “實際命中率”  )
		   必不進的校正公式（紀錄進球）   (玩家換算後的  “實際命中率”  ) / 10
		*/
		shootNoScoreLimit = Mathf.RoundToInt( 100f / (GameFunction.ShootingCalculate(this, GameFunction.GetAttributeFormula(EPlayerAttributeRate.Point2Rate, (BaseAttribute.Point2 + GameData.BaseAttr[Attribute.AILevel].PointRate2)), 0, 0, 11)));
		shootScoreLimit = Mathf.RoundToInt((GameFunction.ShootingCalculate(this, GameFunction.GetAttributeFormula(EPlayerAttributeRate.Point2Rate, (BaseAttribute.Point2 + GameData.BaseAttr[Attribute.AILevel].PointRate2)), 0, 0, 11)) / 10 );

        Attr.ShootingRate = GameData.BaseAttr[Attribute.AILevel].ShootingRate;
        Attr.PointRate2 = GameFunction.GetAttributeFormula(EPlayerAttributeRate.Point2Rate, (Attribute.Point2 + GameData.BaseAttr[Attribute.AILevel].PointRate2));
        Attr.PointRate3 = GameFunction.GetAttributeFormula(EPlayerAttributeRate.Point3Rate, (Attribute.Point3 + GameData.BaseAttr[Attribute.AILevel].PointRate3));
        Attr.StealRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.StealRate, (Attribute.Steal + GameData.BaseAttr[Attribute.AILevel].StealRate));
        Attr.DunkRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.DunkRate, (Attribute.Dunk + GameData.BaseAttr[Attribute.AILevel].DunkRate));
        Attr.TipInRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.TipInRate, (Attribute.Dunk + GameData.BaseAttr[Attribute.AILevel].TipInRate));
        Attr.AlleyOopRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.AlleyOopRate, (Attribute.Dunk + GameData.BaseAttr[Attribute.AILevel].AlleyOopRate));
        Attr.StrengthRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.StrengthRate, (Attribute.Strength + GameData.BaseAttr[Attribute.AILevel].StrengthRate));
        Attr.ElbowingRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.ElbowingRate, (Attribute.Strength + GameData.BaseAttr[Attribute.AILevel].ElbowingRate));
		Attr.BlockBePushRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.BlockBePushRate, (Attribute.Strength + GameData.BaseAttr[Attribute.AILevel].BlockBePushRate));
        Attr.ReboundRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.ReboundRate, (Attribute.Rebound + GameData.BaseAttr[Attribute.AILevel].ReboundRate));
        Attr.BlockRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.BlockRate, (Attribute.Block + GameData.BaseAttr[Attribute.AILevel].BlockRate));
		Attr.FaketBlockRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.FakeBlockrate, (Attribute.Block + GameData.BaseAttr[Attribute.AILevel].FaketBlockRate));
		Attr.BlockPushRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.BlockPushRate, (Attribute.Block + GameData.BaseAttr[Attribute.AILevel].BlockPushRate));
        Attr.PushingRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.PushingRate, (Attribute.Defence + GameData.BaseAttr[Attribute.AILevel].PushingRate));
        Attr.PassRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.PassRate, (Attribute.Pass + GameData.BaseAttr[Attribute.AILevel].PassRate));
        Attr.AlleyOopPassRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.AlleyOopPassRate, (Attribute.Pass + GameData.BaseAttr[Attribute.AILevel].AlleyOopPassRate));
        Attr.DefDistance = GameFunction.GetAttributeFormula(EPlayerAttributeRate.DefDistance, (Attribute.Defence + GameData.BaseAttr[Attribute.AILevel].DefDistance));
        Attr.StaminaValue = GameFunction.GetAttributeFormula(EPlayerAttributeRate.StaminaValue, (Attribute.Stamina + GameData.BaseAttr[Attribute.AILevel].StaminaValue));

		//其他算法
		Attr.ReboundHeadDistance = Attribute.Rebound + GameData.BaseAttr[Attribute.AILevel].ReboundHeadDistance;
		Attr.ReboundHandDistance = Attribute.Rebound + GameData.BaseAttr[Attribute.AILevel].ReboundHandDistance;
		Attr.BlockDistance = Attribute.Block + GameData.BaseAttr[Attribute.AILevel].BlockDistance;

		//有最小值
		Attr.SpeedValue = GameConst.SpeedValueMin + GameFunction.GetAttributeFormula(EPlayerAttributeRate.SpeedValue, (Attribute.Speed + GameData.BaseAttr[Attribute.AILevel].SpeedValue));
        Attr.StealDistance = GameConst.StealPushDistance + GameFunction.GetAttributeFormula(EPlayerAttributeRate.StealDistance, Attribute.Steal);
        Attr.StealExtraAngle = GameConst.StealFanAngle + GameFunction.GetAttributeFormula(EPlayerAttributeRate.StealExtraAngle, Attribute.Steal);
        Attr.PushDistance = GameConst.StealPushDistance + GameFunction.GetAttributeFormula(EPlayerAttributeRate.PushDistance, Attribute.Defence);
        Attr.PushExtraAngle = GameConst.PushFanAngle + GameFunction.GetAttributeFormula(EPlayerAttributeRate.PushExtraAngle, Attribute.Defence);
        Attr.ElbowDistance = GameConst.StealPushDistance + GameFunction.GetAttributeFormula(EPlayerAttributeRate.ElbowDistance, Attribute.Strength);
        Attr.ElbowExtraAngle = GameConst.ElbowFanAngle + GameFunction.GetAttributeFormula(EPlayerAttributeRate.ElbowExtraAngle, Attribute.Strength);
		
        Attr.AutoFollowTime = GameData.BaseAttr[Attribute.AILevel].AutoFollowTime;

		apMax = SelfAPMax + GameData.BaseAttr[Attribute.AILevel].APMax;
		apBegin = SelfAPBegin + GameData.BaseAttr[Attribute.AILevel].APBegin;
		apGrowth = SelfAPGrowth + GameData.BaseAttr[Attribute.AILevel].APGrowth;
   
        DefPoint.transform.localScale = new Vector3(Attr.DefDistance, Attr.DefDistance, Attr.DefDistance);
		interceptTrigger.transform.localScale = new Vector3(1, 1.5f, 1); //因還未有成長先(1, 1.5, 1)， 有成長就變成(0,5 1 0,5)
		if(Attribute.BodyType == 0){
			TopPoint.transform.localScale = new Vector3(GameConst.ReboundHeadXC + (Attr.ReboundHeadDistance* 0.03f) , 
														GameConst.ReboundHeadYC + (Attr.ReboundHeadDistance* 0.04f) , 
														GameConst.ReboundHeadZC + (Attr.ReboundHeadDistance* 0.03f));
			blockTrigger.transform.localScale = new Vector3(GameConst.ReboundBlockXC + (Attr.BlockDistance * 0.04f), 
															GameConst.ReboundBlockYC + (Attr.BlockDistance * 0.05f), 
															GameConst.ReboundBlockZC + (Attr.BlockDistance * 0.04f));
			pushThroughTigger.transform.localScale = new Vector3(GameConst.PushThroughX + (Attr.BlockDistance * 0.04f), 
																 GameConst.PushThroughY + (Attr.BlockDistance * 0.05f), 
																 GameConst.PushThroughZ + (Attr.BlockDistance * 0.04f));
			reboundTrigger.transform.localScale = new Vector3(GameConst.ReboundBlockXC + (Attr.ReboundHandDistance * 0.04f), 
															  GameConst.ReboundBlockYC + (Attr.ReboundHandDistance * 0.05f),
															  GameConst.ReboundBlockZC + (Attr.ReboundHandDistance * 0.04f));
		} else if(Attribute.BodyType == 1) {
			TopPoint.transform.localScale = new Vector3(GameConst.ReboundHeadXF + (Attr.ReboundHeadDistance* 0.02f), 
														GameConst.ReboundHeadYF + (Attr.ReboundHeadDistance* 0.03f) , 
														GameConst.ReboundHeadZF + (Attr.ReboundHeadDistance* 0.02f));
			blockTrigger.transform.localScale = new Vector3(GameConst.ReboundBlockXF + (Attr.BlockDistance * 0.02f), 
															GameConst.ReboundBlockYF + (Attr.BlockDistance * 0.03f), 
															GameConst.ReboundBlockZF + (Attr.BlockDistance * 0.02f));
			pushThroughTigger.transform.localScale = new Vector3(GameConst.PushThroughX + (Attr.BlockDistance * 0.02f), 
																 GameConst.PushThroughY + (Attr.BlockDistance * 0.03f), 
																 GameConst.PushThroughZ + (Attr.BlockDistance * 0.02f));
			reboundTrigger.transform.localScale = new Vector3(GameConst.ReboundBlockXF + (Attr.ReboundHandDistance * 0.03f), 
															  GameConst.ReboundBlockYF + (Attr.ReboundHandDistance * 0.04f),
															  GameConst.ReboundBlockZF + (Attr.ReboundHandDistance * 0.03f));
		} else {
			TopPoint.transform.localScale = new Vector3(GameConst.ReboundHeadXG + (Attr.ReboundHeadDistance* 0.01f) , 
														GameConst.ReboundHeadYG + (Attr.ReboundHeadDistance* 0.02f) , 
														GameConst.ReboundHeadZG + (Attr.ReboundHeadDistance* 0.01f));
			blockTrigger.transform.localScale = new Vector3(GameConst.ReboundBlockXG + (Attr.BlockDistance * 0.03f), 
															GameConst.ReboundBlockYG + (Attr.BlockDistance * 0.04f), 
															GameConst.ReboundBlockZG + (Attr.BlockDistance * 0.03f));
			pushThroughTigger.transform.localScale = new Vector3(GameConst.PushThroughX + (Attr.BlockDistance * 0.03f), 
															 	 GameConst.PushThroughY + (Attr.BlockDistance * 0.04f), 
																 GameConst.PushThroughZ + (Attr.BlockDistance * 0.03f));
			reboundTrigger.transform.localScale = new Vector3(GameConst.ReboundBlockXG + (Attr.ReboundHandDistance * 0.02f), 
															  GameConst.ReboundBlockYG + (Attr.ReboundHandDistance * 0.03f),
															  GameConst.ReboundBlockZG + (Attr.ReboundHandDistance * 0.02f));
		}
    }

    private void initSkill()
    {
        isSkillShow = false;
        PlayerSkillController.initSkillController(Attribute, this, AnimatorControl.Controler);

        if (Team == ETeamKind.Npc)
            PlayerSkillController.HidePlayerName();
    }
		
	public Color NameColor {
		set {
			PlayerSkillController.NameColor = value;
		}
	}

    public void InitDoubleClick()
    {
        if (Team == ETeamKind.Self && DoubleClick == null)
        {
            DoubleClick = Instantiate(Resources.Load("Effect/DoubleClick")) as GameObject;
            DoubleClick.name = "DoubleClick";
            DoubleClick.transform.parent = PlayerRefGameObject.transform;
            DoubleClick.transform.localPosition = Vector3.zero;
        } 
    }

    public void InitTrigger(GameObject defPoint, GameObject trigger)
    {
        DummyBall = null;
        Transform find = transform.Find("DummyBall");

        if (find)
            DummyBall = find.gameObject;
				
        if (DummyBall != null) {
            blockCatchTrigger = DummyBall.GetComponent<BlockCatchTrigger>();
            if (blockCatchTrigger == null)
                blockCatchTrigger = DummyBall.AddComponent<BlockCatchTrigger>();

            blockCatchTrigger.SetEnable(false);	
        }

        if (BodyHeight == null)
            BodyHeight = new GameObject();
        
        BodyHeight.name = "BodyHeight";
        BodyHeight.transform.parent = transform;
        BodyHeight.transform.localPosition = new Vector3(0, PlayerRefGameObject.transform.GetComponent<CapsuleCollider>().height + 0.2f, 0);

		if(Pelvis == null)
			Pelvis = transform.Find("Bip01");

        if (trigger) {
            bodyTrigger = Instantiate(trigger, Vector3.zero, Quaternion.identity) as GameObject;
            interceptTrigger= bodyTrigger.transform.Find("Intercept").gameObject;
            blockTrigger = bodyTrigger.transform.Find("Block").gameObject;
            pushThroughTigger = bodyTrigger.transform.Find("TriggerPushThrough").gameObject;
            reboundTrigger = bodyTrigger.transform.Find("TriggerRebound").gameObject;
            ShowWord = bodyTrigger.transform.Find("ShowWord").gameObject;
            
            bodyTrigger.name = "BodyTrigger";
            PlayerTrigger[] objs = bodyTrigger.GetComponentsInChildren<PlayerTrigger>();
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i++)
                    objs[i].Player = this;
            }
            
            DefTrigger defTrigger = bodyTrigger.GetComponentInChildren<DefTrigger>(); 
            if (defTrigger != null)
                defTrigger.Player = this;
            
            DefPoint = bodyTrigger.transform.Find("DefRange").gameObject;          
            TopPoint = bodyTrigger.transform.Find("TriggerTop").gameObject; 
            CatchBallPoint = bodyTrigger.transform.Find("CatchBall").gameObject; 
            bodyTrigger.transform.parent = transform;
            bodyTrigger.transform.transform.localPosition = Vector3.zero;
            bodyTrigger.transform.transform.localScale = Vector3.one;

            Transform t = bodyTrigger.transform.Find("TriggerFinger");
            if (t)
            {
//                FingerPoint = t.gameObject;
                t.name = Team.GetHashCode().ToString() + Index.GetHashCode().ToString() + "TriggerFinger";
                t.parent = transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Bip01 R Finger2/Bip01 R Finger21/");
                t.localPosition = Vector3.zero;
                t.localScale = Vector3.one;
            }
        }
        
        if (defPoint != null)
        {
            defPointCopy = Instantiate(defPoint, Vector3.zero, Quaternion.identity) as GameObject;
            defPointCopy.transform.parent = PlayerRefGameObject.transform;
            defPointCopy.name = "DefPoint";
            defPointCopy.transform.localScale = Vector3.one;
            defPointCopy.transform.localPosition = Vector3.zero;

            DefPointAy[EDefPointKind.Front.GetHashCode()] = defPointCopy.transform.Find("Front");
            DefPointAy[EDefPointKind.Back.GetHashCode()] = defPointCopy.transform.Find("Back");
            DefPointAy[EDefPointKind.Right.GetHashCode()] = defPointCopy.transform.Find("Right");
            DefPointAy[EDefPointKind.Left.GetHashCode()] = defPointCopy.transform.Find("Left");
            DefPointAy[EDefPointKind.FrontSteal.GetHashCode()] = defPointCopy.transform.Find("FrontSteal");
            DefPointAy[EDefPointKind.BackSteal.GetHashCode()] = defPointCopy.transform.Find("BackSteal");
            DefPointAy[EDefPointKind.RightSteal.GetHashCode()] = defPointCopy.transform.Find("RightSteal");
            DefPointAy[EDefPointKind.LeftSteal.GetHashCode()] = defPointCopy.transform.Find("LeftSteal");
        }
    }

    public Vector3 FindNearBlockPoint(Vector3 source)
    {
        float dis = 0;
        int kind = EDefPointKind.FrontSteal.GetHashCode();
        dis = Vector3.Distance(source, DefPointAy[kind].position);
        float tempDis = Vector3.Distance(source, DefPointAy[EDefPointKind.BackSteal.GetHashCode()].position);
        if (dis > tempDis)
        {
            dis = tempDis;
            kind = EDefPointKind.BackSteal.GetHashCode();
        }
        tempDis = Vector3.Distance(source, DefPointAy[EDefPointKind.RightSteal.GetHashCode()].position);
        if (dis > tempDis)
        {
            dis = tempDis;
            kind = EDefPointKind.RightSteal.GetHashCode();
        }
        tempDis = Vector3.Distance(source, DefPointAy[EDefPointKind.LeftSteal.GetHashCode()].position);
        if (dis > tempDis)
        {
            kind = EDefPointKind.LeftSteal.GetHashCode();
        }

        return DefPointAy[kind].position;
    }

    private void speedBarColor()
    {
		if (SpeedUpView != null && IsGameJoysticker)
        {
            SpeedUpView.fillAmount = mMovePower / mMaxMovePower;
            SpeedUpView.color = new Color32(255, (byte)(200 * SpeedUpView.fillAmount), (byte)(15 * SpeedUpView.fillAmount), 255);
            if (isSpeedStay && SpeedUpView.fillAmount <= 0.2f)
            {
                isSpeedStay = false;
                SpeedAnimator.SetTrigger("SelectMe");
            }
            else if (!isSpeedStay && SpeedUpView.fillAmount > 0.2f)
            {
                isSpeedStay = true;
                SpeedAnimator.SetTrigger("SelectMeStay");
            }
        }
    }

    void OnDrawGizmos()
    {
        if (IsDebugAnimation)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gameObject.transform.position, mDebugSphere.transform.position);
        }
    }

    void FixedUpdate()
    {
        if (IsShowSituation || !GameController.Get.IsStart)
            return;
        
		angerRecoveryUpdate ();
        CantMoveTimer.Update(Time.deltaTime);
        Invincible.Update(Time.deltaTime);
        StealCD.Update(Time.deltaTime);
        PushCD.Update(Time.deltaTime);
        ElbowCD.Update(Time.deltaTime);
        mManually.Update(Time.deltaTime);

		if (CoolDownCrossover > 0) {
			CoolDownCrossover -= Time.deltaTime;
			if(CoolDownCrossover <= 0)
				CoolDownCrossover = 0;
		}

        if (IsPushCalculate)
            GameController.Get.PushCalculate(this, Attr.PushDistance, Attr.PushExtraAngle);

        if (IsElbowCalculate)
            GameController.Get.PushCalculate(this, Attr.ElbowDistance, Attr.ElbowExtraAngle);

        if (IsStealCalculate)
            GameController.Get.OnStealMoment(this, Attr.StealDistance, Attr.StealExtraAngle);

        if (SlowDownTime > 0 && Time.time >= SlowDownTime)
        {
            SlowDownTime = 0;
//            Attr.SpeedValue = GameData.BaseAttr[Attribute.AILevel].SpeedValue + (Attribute.Speed * 0.005f);
			Attr.SpeedValue = GameConst.SpeedValueMin + GameFunction.GetAttributeFormula(EPlayerAttributeRate.SpeedValue, (Attribute.Speed + GameData.BaseAttr[Attribute.AILevel].SpeedValue));
        }

        if (mManually.IsOff())
        {
            // AI 控制中.
            if (moveQueue.Count > 0) // 移動到某點.
                moveTo(moveQueue.Peek());
            else
            {
                // 移動完畢時. 若是防守方, 要撥防守動作; 若是進攻方, 要撥 Idle.
                isMoving = false;
                if (IsDefence)
                {
                    if (!CheckAnimatorSate(EPlayerState.Defence0))
                    {
                        AniState(EPlayerState.Defence0);		
                    }
                }
                else
                {
                    if (!IsBallOwner && IsRun)
                    {
                        AniState(EPlayerState.Idle); 
                    }
                }

//                if (IsDefence && (CheckAnimatorSate(EPlayerState.RunningDefence) || CheckAnimatorSate(EPlayerState.Defence1)))
//                    AniState(EPlayerState.Defence0);
//                else if (!IsDefence && !IsBallOwner && IsRun)
//                    AniState(EPlayerState.Idle);
            }
        }

		if (IsGameAttack)
        {
            if (!IsDefence)
            {
                //每0.2秒移動一次, 用進攻球員去控制防守球員
                if (Time.time >= moveStartTime)
                {
                    moveStartTime = Time.time + GameConst.DefMoveTime;
                    GameController.Get.MoveDefPlayer(this);
                }
            }
            else
            {
                if (AutoFollow)
                {
                    Vector3 ShootPoint;
                    if (Team == ETeamKind.Self)
                        ShootPoint = CourtMgr.Get.ShootPoint[1].transform.position;
                    else
                        ShootPoint = CourtMgr.Get.ShootPoint[0].transform.position;    

                    if (DefPlayer != null && Vector3.Distance(ShootPoint, DefPlayer.transform.position) <= GameConst.Point3Distance)
                    {
                        AutoFollow = false;
                        SetAutoFollowTime();
                    }                   
                }

                if (CloseDef > 0 && Time.time >= CloseDef)
                {
                    AutoFollow = true;
                    CloseDef = 0;
                }
            }
        }
        
        if (Time.time >= mMovePowerTime)
        {
            mMovePowerTime = Time.time + GameConst.MovePowerCheckTime;
            if (isSpeedup)
            {
				if (mMovePower > 0 && !IsFall && GameController.Get.IsGameAttack)
                {
                    mMovePower -= GameConst.MovePowerMoving;
                    if (mMovePower < 0)
                        mMovePower = 0;

                    speedBarColor();

                    if (mMovePower == 0)
                        canSpeedup = false;
                }
            }
            else
            {
                if (mMovePower <= mMaxMovePower)
                {
                    mMovePower += GameConst.MovePowerRevive;
                    if (mMovePower > mMaxMovePower)
                        mMovePower = mMaxMovePower;
					
                    speedBarColor();
					
                    if (mMovePower == mMaxMovePower)
                        canSpeedup = true;
                }
            }
        }
            
        if (isMoving)
            DribbleTime += Time.deltaTime;

        tryReviseAnimation();
    }

    public void DashEffectEnable(bool isEnable)
    {
        if (dashSmoke == null)
            dashSmoke = EffectManager.Get.PlayEffect("DashSmoke", Vector3.zero, PlayerRefGameObject, null, 0, true, false);

        if (dashSmoke)
        {
            dashSmoke.transform.parent = PlayerRefGameObject.transform;
            dashSmoke.transform.localPosition = Vector3.zero;
            dashSmoke.transform.localEulerAngles = Vector3.zero;
            dashSmoke.transform.localScale = Vector3.one;
            dashSmoke.SetActive(isEnable);
        }
    }

    IEnumerator GetCurrentClipLength()
    {
        yield return new WaitForEndOfFrame();
        float aniTime = AnimatorControl.Controler.GetCurrentAnimatorStateInfo(0).length;
        mManually.StartCounting(mManually.RemainTime + aniTime);
    }

    public void SetAITime(float time)
    {
        mManually.StartCounting(time);
        StartCoroutine(GetCurrentClipLength());

        if (AIActiveHint)
            AIActiveHint.SetActive(false);
    }

    /// <summary>
    /// 進入手控狀態.
    /// </summary>
    /// <param name="aiAddTime"> 微調手動切換成 AI 的時間. 單位: 秒. </param>
    public void SetManually(float aiAddTime = 0)
    {
		if (Team == ETeamKind.Self && IsGameJoysticker)
        {
			if (IsGameAttack || TestMode != EGameTest.None)
            {
                isJoystick = true;
                mManually.StartCounting(GameConst.AITime[GameData.Setting.AIChangeTimeLv] + aiAddTime);
                StartCoroutine(GetCurrentClipLength());

                if (AIActiveHint)
                    AIActiveHint.SetActive(false);
            }
            else
            {
                mManually.Clear();
                if (AIActiveHint)
                    AIActiveHint.SetActive(true);
            }
        }
    }

    public void SetToAI()
    {
        mManually.Clear();
        if (AIActiveHint)
            AIActiveHint.SetActive(true);
    }

    public float timeScale = 1;

    public bool InReboundDistance
    {
        get
        {
            return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), 
                                    new Vector2(CourtMgr.Get.RealBall.transform.position.x, 
                                                CourtMgr.Get.RealBall.transform.position.z)) <= GameConst.ReboundDistance;
        }
    }
	
	private const float ReviseAnimationInterval = 1f;
	private float mReviseElapsedTime = ReviseAnimationInterval;

    private void tryReviseAnimation()
    {	
        if(GameController.Get.IsStart && 
           TimerMgr.Get.CrtTime > GameConst.Min_TimePause && 
			IsGameAttack &&
			(!PlayerSkillController.IsActiveUse && !PlayerSkillController.IsPassiveUse))
        {
			if(!AnimatorControl.IsEqual(CurrentState) && !AnimatorMgr.Get.IsLoopState(CurrentState))
                synchronousAnimation();
            else
				mReviseElapsedTime = ReviseAnimationInterval;
        }
        else
			mReviseElapsedTime = ReviseAnimationInterval;
    }

    private void synchronousAnimation()
    {
        mReviseElapsedTime -= Time.deltaTime; 
        if (mReviseElapsedTime <= 0)
        {
            if(IsDebugAnimation)
				Debug.LogErrorFormat("Animation stuck(asynchronous). {0} - {1}", name, CurrentState);

            AniState(IsBallOwner ? EPlayerState.HoldBall : EPlayerState.Idle, false);

            mReviseElapsedTime = ReviseAnimationInterval;
        }
    }

    public void OnJoystickStart()
    {
        yAxizOffset = CameraMgr.Get.CourtCamera.transform.eulerAngles.y - 90;
    }

    public void OnJoystickMove(Vector2 v)
    {
        if (timeScale > 0 && (CanMove || HoldBallCanMove))
        {
			if (IsGameAttack || TestMode != EGameTest.None)
            {
				EPlayerState ps = EPlayerState.Run0;
                if (IsBallOwner)
                    ps = EPlayerState.Dribble1;

                int moveKind = 0;
                float calculateSpeed = 1;
                clearMoveData();

                #if UNITY_EDITOR
                if (IsFall && IsDebugAnimation)
                {
                    LogMgr.Get.LogError("CanMove : " + CanMove);
                    LogMgr.Get.LogError("stop : " + stop);
                    LogMgr.Get.LogError("HoldBallCanMove : " + HoldBallCanMove);
                }
                #endif

				if (!(CoolDownCrossover == 0 && !IsDefence && IsMoveDodge && IsPush))
				{
					isMoving = true;
					if (!isJoystick)
						moveStartTime = Time.time + GameConst.DefMoveTime;

					SetManually();
					animationSpeed = Vector2.Distance(new Vector2(v.x, 0), new Vector2(0, v.y));
					if (!IsPass)
					{
						float angle = Mathf.Atan2 (v.x, v.y) * Mathf.Rad2Deg;
						float a = 90;
						if (!GameData.Setting.GameMove)
							a += yAxizOffset;
						
						Vector3 rotation = new Vector3(0, angle + a, 0);
						transform.rotation = Quaternion.Euler(rotation);
					}

					if (animationSpeed <= MoveMinSpeed)
						moveKind = 0;
					else
					{
						if (mMovePower == 0)
							moveKind = 0;
						else
							moveKind = 1;
					}

					switch (moveKind)
					{
					case 0://run
						if (animationSpeed <= MoveMinSpeed)
							isSpeedup = false;

						setSpeed(0.3f, 0);
						if (IsBallOwner)
						{  
							calculateSpeed = GameConst.BallOwnerSpeedNormal;
							ps = EPlayerState.Dribble1;
						}
						else
						{
							ps = EPlayerState.Run0;
							if (IsDefence)
								calculateSpeed = GameConst.DefSpeedNormal;
							else
								calculateSpeed = GameConst.AttackSpeedNormal;
						}
						break;

					case 1://dash
						isSpeedup = true;
						setSpeed(1f, 0);

						if (IsBallOwner)
						{  
							calculateSpeed = GameConst.BallOwnerSpeedup;
							ps = EPlayerState.Dribble2;
						}
						else
						{
							ps = EPlayerState.Run1;
							if (IsDefence)
								calculateSpeed = GameConst.DefSpeedup;
							else
								calculateSpeed = GameConst.AttackSpeedup;
						}

						break;

					case 2://walk
						setSpeed(0.2f, 0);
						if (IsBallOwner)
							ps = EPlayerState.Dribble3;
						else
							ps = EPlayerState.Run2;

						calculateSpeed = GameConst.WalkSpeed;
						break;
					}

					if (TestMode == EGameTest.Skill || TestMode == EGameTest.PassiveSkill)
						calculateSpeed = GameConst.AttackSpeedup;
					
					translate = Vector3.forward * Time.deltaTime * Attr.SpeedValue * calculateSpeed * timeScale;
					transform.Translate(translate); 
					transform.position = new Vector3(transform.position.x, 0, transform.position.z);
					AniState(ps);
				}     
            }
        }
    }

    public void OnJoystickMoveEnd()
    {
        if (timeScale > 0 && (CanMove || HoldBallCanMove))
        {
            EPlayerState ps;

            if (IsBallOwner)
            {
                if (CurrentState == EPlayerState.Elbow0)
                    ps = EPlayerState.Elbow0;
                else if (CurrentState == EPlayerState.HoldBall)
                    ps = EPlayerState.HoldBall;
                else
                    ps = EPlayerState.Dribble0;
            }
            else
                ps = EPlayerState.Idle;

            if (CanMove &&
                situation != EGameSituation.GamerInbounds && situation != EGameSituation.GamerPickBall &&
                situation != EGameSituation.NPCInbounds && situation != EGameSituation.NPCPickBall)
            {
                SetManually();
                isJoystick = false;
                isSpeedup = false;

                if (CurrentState != ps)
                    AniState(ps);

                if (CurrentState == EPlayerState.Dribble0)
                {
                    if (situation == EGameSituation.GamerAttack)
                        RotateTo(CourtMgr.Get.ShootPoint[0].transform.position.x, CourtMgr.Get.ShootPoint[0].transform.position.z);
                    else if (situation == EGameSituation.NPCAttack)
                        RotateTo(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);
                }
            }

            isMoving = false;
        }
    }

    private bool GetMoveTarget(ref TMoveData data, out Vector2 result)
    {
        bool resultBool = false;
        result = Vector2.zero;

        if (data.DefPlayer != null)
        {
            if (data.DefPlayer.Index == Index && AutoFollow)
            {
                result.x = data.DefPlayer.transform.position.x;
                result.y = data.DefPlayer.transform.position.z;
                resultBool = true;
//                if (IsDribble)
//                    Debug.Log("catch you");
            }
            else
            {
                Vector3 aP1 = data.DefPlayer.transform.position;
                Vector3 aP2 = CourtMgr.Get.Hood[data.DefPlayer.Team.GetHashCode()].transform.position;
                result = GetStealPostion(aP1, aP2, data.DefPlayer.Index);
                if (Vector2.Distance(result, new Vector2(PlayerRefGameObject.transform.position.x, PlayerRefGameObject.transform.position.z)) <= GameConst.StealPushDistance)
                {
                    if (DefPlayer != null && MathUtils.FindAngle(data.DefPlayer.transform, PlayerRefGameObject.transform.position) >= 30 &&
                        Vector3.Distance(aP2, DefPlayer.transform.position) <= GameConst.Point3Distance + 3)
                    {
                        resultBool = true;
                    }
                    else
                    {
                        result.x = PlayerRefGameObject.transform.position.x;
                        result.y = PlayerRefGameObject.transform.position.z;
                    }
                }
                else
                    resultBool = true;
            }
        }
        else if (data.FollowTarget != null)
        {
            result.x = data.FollowTarget.position.x;
            result.y = data.FollowTarget.position.z;

            if (Vector2.Distance(result, new Vector2(PlayerRefGameObject.transform.position.x, PlayerRefGameObject.transform.position.z)) <= MoveCheckValue)
            {
                result.x = PlayerRefGameObject.transform.position.x;
                result.y = PlayerRefGameObject.transform.position.z;
            }
            else
                resultBool = true;
        }
        else
        {
            result = data.Target;
            resultBool = true;
        }

        return resultBool;
    }

    private void AttackerMove(bool first, TMoveData data, bool isShort)
    {
        if(isShort)
        {
            // 移動距離很短 or 不移動, 球員又是在進攻狀態.
            if(!IsBallOwner)
                AniState(EPlayerState.Idle);
            else if(situation == EGameSituation.GamerInbounds || situation == EGameSituation.NPCInbounds)
                AniState(EPlayerState.Dribble0);

            if(first || TestMode == EGameTest.Edit)
                CantMoveTimer.Clear();
            else if(IsGameAttack && GameController.Get.BallOwner && UnityEngine.Random.Range(0, 3) == 0)
            {
                // 目前猜測這段程式碼的功能是近距離防守時, 避免防守者不斷的轉向.
                // 因為當初寫這段程式碼的時候, AI 做決策其實是 1 秒 30 次以上.
                // 所以當 AI 做防守邏輯的時候, 會 1 秒下 30 的命令, 命令跑到某位球員的旁邊.
                // 就會造成防守球員會一直的轉向.(因為距離很近的時候, 對方移動一點距離, 防守者就必須轉向很多度
                // , 才可以正確的面相對方)
//                var dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint[Team.GetHashCode()].transform.position);
//                if(dis <= 8)
//                    CantMoveTimer.StartCounting(UnityEngine.Random.Range(0.3f, 1.1f));
//                else
//                    CantMoveTimer.StartCounting(UnityEngine.Random.Range(0.3f, 1.1f));

                CantMoveTimer.StartCounting(UnityEngine.Random.Range(0.3f, 0.7f));
            }

            if (IsBallOwner)
            {
                if (Team == ETeamKind.Self)
                    RotateTo(CourtMgr.Get.ShootPoint[0].transform.position.x, CourtMgr.Get.ShootPoint[0].transform.position.z);
                else
                    RotateTo(CourtMgr.Get.ShootPoint[1].transform.position.x, CourtMgr.Get.ShootPoint[1].transform.position.z);

                if (data.Shooting && AIing)
                    GameController.Get.DoShoot();
            }
            else
            {
                if (data.LookTarget == null)
                {
                    if (GameController.Get.BallOwner != null)
                        RotateTo(GameController.Get.BallOwner.transform.position.x, GameController.Get.BallOwner.transform.position.z);
                    else
                    {
                        if (Team == ETeamKind.Self)
                            RotateTo(CourtMgr.Get.ShootPoint[0].transform.position.x, CourtMgr.Get.ShootPoint[0].transform.position.z);
                        else
                            RotateTo(CourtMgr.Get.ShootPoint[1].transform.position.x, CourtMgr.Get.ShootPoint[1].transform.position.z);
                    }
                }
                else
                    RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);

                if (data.Catcher)
                {
					if (IsGameAttack)
                    {
                        if (GameController.Get.TryPass(this, false, false, true))
                            NeedShooting = data.Shooting;
                    }
                }
            }	
        }
        else
        {
            // 進攻移動.                 
            RotateTo(MoveTarget.x, MoveTarget.y); 
            setDebugSpherePos(new Vector3(MoveTarget.x, 0, MoveTarget.y));
            isMoving = true;

            if (IsBallOwner)
            {
                // 持球者移動.
                if (data.Speedup && mMovePower > 0)
                {
                    // 持球者加速移動.(因為球員已經轉身了, 所以往 forward 移動就可以了)
                    setSpeed(1, 0);
                    transform.Translate(Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedup * Attr.SpeedValue * timeScale);
                    AniState(EPlayerState.Dribble2);
                    isSpeedup = true;
                }
                else
                {
                    // 持球者一般移動.
                    transform.Translate(Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedNormal * Attr.SpeedValue * timeScale);
                    AniState(EPlayerState.Dribble1);
                    isSpeedup = false;
                }
            }
            else
            {
                // 未持球者移動.
                if (data.Speedup && mMovePower > 0)
                {
                    setSpeed(1, 0);
                    transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedup * Attr.SpeedValue * timeScale);
                    AniState(EPlayerState.Run1);
                    isSpeedup = true;
                }
                else
                {
                    transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedNormal * Attr.SpeedValue * timeScale);
                    AniState(EPlayerState.Run0);
                    isSpeedup = false;
                }
            } 
        }
    }

    private void DefenderMove(TMoveData data, bool doMove, bool isshort)
    {
        if (isshort)
        {
            // 移動距離很短 or 不移動, 球員又是在防守狀態.
            //                    WaitMoveTime = 0;
            CantMoveTimer.Clear();
            if (data.DefPlayer != null)
            {
                var dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint[data.DefPlayer.Team.GetHashCode()].transform.position);

                if (data.LookTarget != null)
                {
                    if (Vector3.Distance(transform.position, data.DefPlayer.transform.position) <= GameConst.StealPushDistance)
                        RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                    else if (!doMove)
                        RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                    else if (dis > GameConst.Point3Distance + 4 && data.DefPlayer.AIing &&
                             (data.DefPlayer.CantMoveTimer.IsOff() || data.DefPlayer.TargetPosNum > 0))
                        RotateTo(MoveTarget.x, MoveTarget.y);
                    else
                        RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                }
                else
                    RotateTo(MoveTarget.x, MoveTarget.y);
            }
            else
            {
                if (data.LookTarget == null)
                    RotateTo(MoveTarget.x, MoveTarget.y);
                else
                    RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
            }

            AniState(EPlayerState.Defence0);   
        }
        else
        {
            // 防守移動.
            if (data.DefPlayer != null)
            {
                var dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint[data.DefPlayer.Team.GetHashCode()].transform.position);

                if(dis <= GameConst.Point3Distance + 4 || Vector3.Distance(transform.position, data.LookTarget.position) <= 1.5f)
                {
                    setDebugSpherePos(new Vector3(data.LookTarget.position.x, 0, data.LookTarget.position.z));
                    RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                }
                else
                {
                    setDebugSpherePos(new Vector3(MoveTarget.x, 0, MoveTarget.y));
                    RotateTo(MoveTarget.x, MoveTarget.y);
                }

                if (MathUtils.FindAngle(PlayerRefGameObject.transform, new Vector3(MoveTarget.x, 0, MoveTarget.y)) > 90)
                    AniState(EPlayerState.Defence1);
                else
                    AniState(EPlayerState.RunningDefence);
            }
            else
            {
                setDebugSpherePos(new Vector3(MoveTarget.x, 0, MoveTarget.y));
                RotateTo(MoveTarget.x, MoveTarget.y);
                AniState(EPlayerState.Run0);
            }

            isMoving = true;
			if (mMovePower > 0 && canSpeedup && !IsGameJoysticker && !IsTee)
            {
                setSpeed(1, 0);
                transform.position = Vector3.MoveTowards(transform.position, 
                    new Vector3(MoveTarget.x, 0, MoveTarget.y), 
                    Time.deltaTime * GameConst.DefSpeedup * Attr.SpeedValue * timeScale);
                isSpeedup = true;

                setDebugSpherePos(new Vector3(MoveTarget.x, 0, MoveTarget.y));
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, 
                    new Vector3(MoveTarget.x, 0, MoveTarget.y), 
                    Time.deltaTime * GameConst.DefSpeedNormal * Attr.SpeedValue * timeScale);
                isSpeedup = false;
                setDebugSpherePos(new Vector3(MoveTarget.x, 0, MoveTarget.y));
            }
        }
    }

    private void setDebugSpherePos(Vector3 pos)
    {
        if(mDebugSphere)
            mDebugSphere.transform.position = pos;
    }

    private void moveTo(TMoveData data, bool first = false)
    {
		if((CanMove || (AIing && HoldBallCanMove)) && CantMoveTimer.IsOff() && !Pause)
        {
            bool doMove = GetMoveTarget(ref data, out MoveTarget);
            float temp = Vector2.Distance(new Vector2(PlayerRefGameObject.transform.position.x, PlayerRefGameObject.transform.position.z), MoveTarget);
            setSpeed(0.3f, 0);

            if (temp <= MoveCheckValue || !doMove)
            {
                // 移動距離太短 or 不移動.
                MoveTurn = 0;
                isMoving = false;

                if(IsDefence)
                    DefenderMove(data, doMove, true);
                else
                    AttackerMove(first, data, true);

                // 移動到非常接近 target, 所以刪除這筆, 接著移動到下一個 target.
                if (moveQueue.Count > 0)
                {
                    moveQueue.Dequeue();
                    //                    Debug.LogFormat("moveTo(), moveQueue.Dequeue()");
                }

                if (data.MoveFinish != null)
                    data.MoveFinish(this, data.Speedup);
            }
            else
            {
                if (MoveTurn >= 0 && MoveTurn <= 5 && GameController.Get.BallOwner != null)
                {
                    MoveTurn++;
                    RotateTo(MoveTarget.x, MoveTarget.y);
                    if (MoveTurn == 1)
                        moveStartTime = Time.time + GameConst.DefMoveTime;  	
                }
                else
                {
                    if (IsDefence)
                    {
                        DefenderMove(data, doMove, false);
                    }
                    else
                    {
                        AttackerMove(first, data, false);
                    }
                }

//                if (MoveTurn >= 0 && MoveTurn <= 5 && GameController.Get.BallOwner != null)
//                {
//                    MoveTurn++;
//                    RotateTo(MoveTarget.x, MoveTarget.y);
//                    if (MoveTurn == 1)
//                        moveStartTime = Time.time + GameConst.DefMoveTime;           
//                }
            } 
        }
        else
            isMoving = false;
    }

    public void RotateTo(float lookAtX, float lookAtZ)
    {
        if (isBlock || isSkillShow)
            return;
				
//        PlayerRefGameObject.transform.DOLookAt(new Vector3(lookAtX, PlayerRefGameObject.transform.position.y, lookAtZ), 0, AxisConstraint.Y);
        PlayerRefGameObject.transform.LookAt(new Vector3(lookAtX, PlayerRefGameObject.transform.position.y, lookAtZ));
    }

    public void SetInvincible(float time)
    {
        Invincible.StartCounting(time);
    }

    private void setSpeed(float value, int dir = -2)
    {
        AnimatorControl.SetSpeed(value);
    }

    public bool CheckAnimatorSate(EPlayerState state)
    {
        return CurrentState == state;
    }

    public void ResetFlag(bool clearMove = true)
    {
        if (IsShowSituation)
            return;

        if (CheckAnimatorSate(EPlayerState.Idle) || CheckAnimatorSate(EPlayerState.Dribble1) || CheckAnimatorSate(EPlayerState.Dribble0))
        {
            NeedResetFlag = false;

//            for (int i = 0; i < PlayerActionFlag.Length; i++)
//                PlayerActionFlag[i] = 0;

            if (!IsBallOwner)
                AniState(EPlayerState.Idle);
            else
                AniState(EPlayerState.Dribble0);

            //When AI disabled player has to run all path
            if(clearMove)
                clearMoveData();

            CantMoveTimer.Clear();
            NeedShooting = false;
            isJoystick = false; 
            isMoving = false;
            isSpeedup = false;
            canSpeedup = true;
        }
        else
            NeedResetFlag = true;
    }

    public void Reset()
    {
        PlayerSkillController.Reset();
        angerValue = 0;
        AnimatorControl.Reset();
        isBlock = false;
//        isShootJumpActive = false;
        isSkillShow = false;
        ResetFlag();
    }

    public bool CanUseState(EPlayerState state)
    {
        switch (state)
        {
            case EPlayerState.Pass0:
            case EPlayerState.Pass2:
            case EPlayerState.Pass1:
            case EPlayerState.Pass3:
            case EPlayerState.Pass5:
            case EPlayerState.Pass6:
            case EPlayerState.Pass7:
            case EPlayerState.Pass8:
            case EPlayerState.Pass9:
            case EPlayerState.Pass50:
                if(!IsShow && !IsDunk && !IsAlleyoopState && IsBallOwner && !IsPass && !IsPickBall &&
                   !IsAllShoot && (CurrentState == EPlayerState.HoldBall || IsDribble))
                    return true;
                break;

            case EPlayerState.Pass4:
                if(IsBallOwner && !IsLayup && !IsDunk && !IsAlleyoopState && IsShoot && 
                   !GameController.Get.Shooter && mIsPassAirMoment && !IsPass)
                    return true;
                break;
            case EPlayerState.BlockCatch:
                if(IsBlock && CurrentState != EPlayerState.BlockCatch)
                    return true;
                break;

            case EPlayerState.FakeShoot:
                if(IsBallOwner && 
                   (CurrentState == EPlayerState.Idle || CurrentState == EPlayerState.HoldBall || IsDribble))
                    return true;
                break;

            case EPlayerState.Shoot0:
            case EPlayerState.Shoot1:
            case EPlayerState.Shoot2:
            case EPlayerState.Shoot3:
            case EPlayerState.Shoot4:
            case EPlayerState.Shoot5:
            case EPlayerState.Shoot6:
            case EPlayerState.Shoot7:
            case EPlayerState.Shoot20:
                if (IsBallOwner && !IsPickBall && !IsIntercept && !IsAllShoot && (CurrentState == EPlayerState.HoldBall || IsDribble))
                    return true;
                break;

            case EPlayerState.Layup0:
            case EPlayerState.Layup1:
            case EPlayerState.Layup2:
            case EPlayerState.Layup3:
                if (IsBallOwner && !IsPickBall && !IsIntercept && !IsAllShoot && (CurrentState == EPlayerState.HoldBall || IsDribble))
                    return true;
                break;

            case EPlayerState.Dunk0:
            case EPlayerState.Dunk1:
            case EPlayerState.Dunk2:
            case EPlayerState.Dunk3:
            case EPlayerState.Dunk4:
            case EPlayerState.Dunk5:
            case EPlayerState.Dunk6:
            case EPlayerState.Dunk7:
            case EPlayerState.Dunk20:
            case EPlayerState.Dunk21:
            case EPlayerState.Dunk22:
                if (IsBallOwner && !IsIntercept && !IsPickBall && !IsAllShoot && (CurrentState == EPlayerState.HoldBall || IsDribble))
                if (Vector3.Distance(CourtMgr.Get.ShootPoint[Team.GetHashCode()].transform.position, PlayerRefGameObject.transform.position) < canDunkDis)
                    return true;
                break;

            case EPlayerState.Alleyoop:
                if(CurrentState != EPlayerState.Alleyoop && 
                   !GameController.Get.CheckOthersUseSkill(TimerKind.GetHashCode()) && 
                   !IsBallOwner && 
                   (TestMode == EGameTest.Alleyoop || situation.GetHashCode() == Team.GetHashCode() + 3))
                    return true;

                break;
            case EPlayerState.HoldBall:
                if (IsBallOwner && !IsPass && !IsAllShoot && CurrentState != EPlayerState.GotSteal && IsInGround)
                    return true;
                break;

            case EPlayerState.Rebound0:
            case EPlayerState.Rebound20:
                if (CanMove && !IsRebound)
                    return true;
                break;

            case EPlayerState.JumpBall:
                if (CanMove && CurrentState != EPlayerState.JumpBall)
                    return true;
                break;

            case EPlayerState.TipIn:
                if (IsRebound && CanUseTipIn && CurrentState != EPlayerState.TipIn)
                    return true;

                break;
           
            case EPlayerState.Pick0:
            case EPlayerState.Pick1:
            case EPlayerState.Pick2:
                if (CanMove && !IsBallOwner && (CurrentState == EPlayerState.Idle || IsRun || CurrentState == EPlayerState.Defence1 ||
                    CurrentState == EPlayerState.Defence0 || CurrentState == EPlayerState.RunningDefence))
                    return true;
                break;

            case EPlayerState.Push0:
            case EPlayerState.Push1:
            case EPlayerState.Push2:
            case EPlayerState.Push20:
            case EPlayerState.Steal0:
            case EPlayerState.Steal1:
            case EPlayerState.Steal2:
            case EPlayerState.Steal20:
                if(!IsTee && !IsBallOwner && !IsSteal && IsInGround &&
                   (CurrentState == EPlayerState.Idle || IsSteal || IsRun || CurrentState == EPlayerState.Defence1 ||
                    CurrentState == EPlayerState.Defence0 || CurrentState == EPlayerState.RunningDefence))
                    return true;
                break;

            case EPlayerState.Block0:
            case EPlayerState.Block1:
            case EPlayerState.Block2:
            case EPlayerState.Block20:
                if (!IsTee && !IsBlock && CanMove && !IsBallOwner && (CurrentState == EPlayerState.Idle || IsRun || CurrentState == EPlayerState.Defence1 ||
                    CurrentState == EPlayerState.Defence0 || CurrentState == EPlayerState.RunningDefence || IsDunk))
                    return true;
                break;

            case EPlayerState.Elbow0:
            case EPlayerState.Elbow1:
            case EPlayerState.Elbow2:
            case EPlayerState.Elbow20:
            case EPlayerState.Elbow21:
                if(!IsTee && !IsElbow && IsInGround && IsBallOwner && 
                   (IsDribble || CurrentState == EPlayerState.HoldBall))
                    return true;
                break;

            case EPlayerState.Fall0:
            case EPlayerState.Fall1:
            case EPlayerState.Fall2:
            case EPlayerState.KnockDown0:
            case EPlayerState.KnockDown1:
				if(!IsTee && !IsFall) //&& !IsUseActiveSkill (主動技可以被蓋)
                    return true;
                break;

            case EPlayerState.GotSteal:
                if (!IsTee && !IsAllShoot && CurrentState != state && CurrentState != EPlayerState.Elbow0 &&
                    (IsDribble ||
                    CurrentState == EPlayerState.FakeShoot ||
                    CurrentState == EPlayerState.HoldBall ||
                    CurrentState == EPlayerState.Idle ||
                    CurrentState == EPlayerState.Defence0 ||
                    CurrentState == EPlayerState.Defence1 ||
                    CurrentState == EPlayerState.RunningDefence))
                    return true;
                break;

            case EPlayerState.Dribble0:
            case EPlayerState.Dribble1:
            case EPlayerState.Dribble2:
            case EPlayerState.Dribble3:
                if(IsInGround && mStateChangable && IsBallOwner && 
                   !IsPickBall && !IsPass && !IsAllShoot && !IsElbow && !IsFall)
                if((!CanMove && IsFirstDribble) || 
                    (CanMove && CurrentState != state) || 
                    CurrentState == EPlayerState.MoveDodge0 || 
                    CurrentState == EPlayerState.MoveDodge1)
                    return true;
                break;
            
            case EPlayerState.Run0:   
            case EPlayerState.Run1:   
            case EPlayerState.Run2:   
            case EPlayerState.RunningDefence:
            case EPlayerState.Defence0:
            case EPlayerState.Defence1:
                if(IsInGround && mStateChangable && CurrentState != state && !IsAllShoot && !IsFall && !IsJump)
                    return true;
                break;
            case EPlayerState.MoveDodge0:
            case EPlayerState.MoveDodge1:
				if (CurrentState != state && !IsPass && !IsAllShoot && !IsBlock && !IsFall && IsInGround)
                    return true;
                break;
            case EPlayerState.CatchFlat:
            case EPlayerState.CatchFloor:
            case EPlayerState.CatchParabola:
            case EPlayerState.Intercept0:
            case EPlayerState.Intercept1:
				if (CanMove && !IsBallOwner && !IsAllShoot && IsInGround && situation != EGameSituation.GamerPickBall && situation != EGameSituation.NPCPickBall && situation != EGameSituation.GamerInbounds && situation != EGameSituation.NPCInbounds)
                    return true;
                break;

            case EPlayerState.Buff20:
            case EPlayerState.Buff21:
                if (!IsBuff)
                    return true;
                break;
            case EPlayerState.Idle:
                if (mStateChangable)
                    return true;
                break;

            case EPlayerState.Show1:
            case EPlayerState.Show1001:
            case EPlayerState.Show1003:
            case EPlayerState.Show101:
            case EPlayerState.Show102:
            case EPlayerState.Show103:
            case EPlayerState.Show201:
            case EPlayerState.Show202:
            case EPlayerState.Ending0:
            case EPlayerState.Ending10:
                if (!IsShow && !IsFall && !IsAllShoot && !IsBlock && !IsRebound)
                    return true;
                break;
        }
        
        return false;
    }

    public bool IsTee
    { 
        get
        {
            return situation == EGameSituation.SpecialAction || 
                   situation == EGameSituation.GamerInbounds || 
                   situation == EGameSituation.GamerPickBall || 
                   situation == EGameSituation.NPCInbounds || 
                   situation == EGameSituation.NPCPickBall;
        }
    }

    public bool IsKinematic
    {
        get{ return PlayerRigidbody.isKinematic; }
        set
        {
            PlayerRigidbody.isKinematic = value;
        }
    }

    public bool AniState(EPlayerState newState, Vector3 lookAtPoint)
    {
        if(AniState(newState))
        {
            RotateTo(lookAtPoint.x, lookAtPoint.z);
            if (TestMode == EGameTest.Pass)
                LogMgr.Get.Log("name:" + PlayerRefGameObject.name + "Rotate");
            
            return true;
        }
        return false;
    }

    private void BlockStateHandle(int stateNo)
    {
        switch (stateNo)
        {
            case 20:
                GameRecord.Block++;
                if (GameController.Get.BallState == EBallState.CanDunkBlock)
                    isDunkBlock = true;
                break;
        }

        if(GameController.Get.BallOwner == null)
            skillMoveTarget = CourtMgr.Get.RealBall.transform.position;
        else
            skillMoveTarget = GameController.Get.BallOwner.FindNearBlockPoint(PlayerRefGameObject.transform.position);

//        AnimatorControl.InitBlockCurve(stateNo, skillMoveTarget, isDunkBlock);
        StartSkillCamera(stateNo);
        SetShooterLayer();
        
        UseGravity = false;
        IsKinematic = true;
        
        AnimatorControl.Play(EAnimatorState.Block, stateNo, Team.GetHashCode(), isDunkBlock, skillMoveTarget);
        isCanCatchBall = false;
        if (stateNo >= 20)
            isBlock = true;
        GameRecord.BlockLaunch++;
    }

    private void BlockCatchStateHandle(int stateNo)
    {
        UseGravity = false;
        IsKinematic = true;
        AnimatorControl.Play(EAnimatorState.BlockCatch, stateNo, Team.GetHashCode());
        IsPerfectBlockCatch = false;
        isCanCatchBall = false;
    }

    public void BuffStateHandle(int stateNo)
    {
        StartSkillCamera(stateNo);
        AnimatorControl.Play(EAnimatorState.Buff, stateNo, Team.GetHashCode());
    }

    public void CatchStateHandle(int stateNo)
    {
        setSpeed(0, -1);
        AnimatorControl.Play(EAnimatorState.Catch, stateNo, Team.GetHashCode());
    }

    public void DefenceStateHandle(int stateNo)
    {
        switch (stateNo)
        {
            case 0:
                PlayerRigidbody.mass = 5;
                setSpeed(0, -1);
                break;
            case 1:
                setSpeed(1, 1);
                break;
        }
    
        AnimatorControl.Play(EAnimatorState.Defence, stateNo, Team.GetHashCode());
    }

    public void AlleyoopStateHandle(int stateNo)
    {
        UseGravity = false;
        IsKinematic = true;
        AnimatorControl.Play(EAnimatorState.Dunk, stateNo, Team.GetHashCode());
        isCanCatchBall = true;
        AnimatorControl.InitDunkCurve(stateNo, CourtMgr.Get.DunkPoint[Team.GetHashCode()].transform.position, Team == 0 ? 0 : 180);
        SetShooterLayer();
        if (OnDunkJump != null)
            OnDunkJump(this);
    }

    public void DunkStateHandle(int stateNo)
    {
        StartSkillCamera(stateNo);
        UseGravity = false;
        IsKinematic = true;
        AnimatorControl.Play(EAnimatorState.Dunk, stateNo, Team.GetHashCode());
        isCanCatchBall = false;
//		AnimatorControl.InitDunkCurve(stateNo, CourtMgr.Get.DunkPoint [Team.GetHashCode ()].transform.position, Team == 0 ? 0 : 180);

        SetShooterLayer();
       
        CourtMgr.Get.RealBall.SetBallState(EPlayerState.Dunk0, this);

//        if (stateNo == 0 || stateNo == 1 || stateNo == 3 || stateNo == 5 || stateNo == 7)
//        {
            GameController.Get.BallState = EBallState.CanDunkBlock;
//        }
            

        if (OnDunkJump != null)
            OnDunkJump(this);
    }

    public void DribbleStateHandle(int stateNo)
    {
        switch (stateNo)
        {
            case 0:
                PlayerRigidbody.mass = 5;
                break;
            case 2:
                DashEffectEnable(true);
                break;
            case 3:
                DashEffectEnable(false);
                break;
        }
            
        AnimatorControl.Play(EAnimatorState.Dribble, stateNo, Team.GetHashCode());
        CourtMgr.Get.RealBall.SetBallState(EPlayerState.Dribble0, this);
        isCanCatchBall = false;
        IsFirstDribble = false;
    }

    public void ElbowStateHandle(int stateNo)
    {
        switch (stateNo)
        {
            case 20:
            case 21:
                GameRecord.Elbow++;
                break;
        }
        StartSkillCamera(stateNo);
        PlayerRigidbody.mass = 5;
        AnimatorControl.Play(EAnimatorState.Elbow, stateNo, Team.GetHashCode());
        isCanCatchBall = false;
        GameRecord.ElbowLaunch++;
    }

    public void FakeShootStateHandle(int stateNo)
    {
        if (IsBallOwner)
        {
            PlayerRigidbody.mass = 5;
            AnimatorControl.Play(EAnimatorState.FakeShoot, stateNo, Team.GetHashCode());
            isCanCatchBall = false;
            isFakeShoot = true;
            GameRecord.Fake++;
        }
    }

    public void KnockDownStateHandle(int stateNo)
    {
        if (IsBallOwner && (situation != EGameSituation.GamerPickBall || situation != EGameSituation.NPCPickBall))
        {
            GameController.Get.SetBall();
            CourtMgr.Get.RealBall.SetBallState(EPlayerState.KnockDown0);
        }
        if (IsDunk)
        {
            AnimatorControl.Reset(EAnimatorState.Dunk);
//            IsAnimatorMove = false;
            PlayerRefGameObject.transform.DOKill();
            PlayerRefGameObject.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.Linear);
        }
        SetShooterLayer();
        
//        isShootJump = false;
        AnimatorControl.Play(EAnimatorState.KnockDown, stateNo, Team.GetHashCode());
        isCanCatchBall = false;
    }

    public void FallStateHandle(int stateNo)
    {
        SetShooterLayer();
//        AnimatorControl.InitFallCurve(stateNo);
        AnimatorControl.Reset(EAnimatorState.Dunk);
//        IsAnimatorMove = false;
//        isShootJump = false;
        AnimatorControl.Play(EAnimatorState.Fall, stateNo, Team.GetHashCode());	
        isCanCatchBall = false;
        PlayerRefGameObject.transform.DOLocalMoveY(0, 1f);
        if (OnFall != null)
            OnFall(this);
    }

    public void ShootStateHandle(int stateNo)
    {
        if (IsBallOwner)
        {
            switch (stateNo)
            {
                case 0:     
                case 4:     
                case 5:     
                case 6:     
                case 7:     
                    skillKind = ESkillKind.Shoot;
                    break;
                case 1:     
                    skillKind = ESkillKind.NearShoot;
                    break;
                case 2:     
                    skillKind = ESkillKind.UpHand;
                    break;
                case 3:     
                    skillKind = ESkillKind.DownHand;
                    break;
                case 20:
//                    isShootJumpActive = true;
                    break;
            }
			
            //Debug.LogError("Shoot show camera: " + stateNo);
            CourtMgr.Get.RealBall.SetBallState(EPlayerState.Shoot0, this);
            StartSkillCamera(stateNo);
            UseGravity = false;
            IsKinematic = true;
//            AnimatorControl.InitShootCurve(stateNo);
            SetShooterLayer();
            AnimatorControl.Play(EAnimatorState.Shoot, stateNo, Team.GetHashCode());
            isCanCatchBall = false;
        }
    }

    public void LayupStateHandle(int stateNo)
    {
        if (IsBallOwner)
        {
            switch (stateNo)
            {
                case 0:
                    skillKind = ESkillKind.Layup;
                    break;
                case 1:
                case 2:
                case 3:
                    skillKind = ESkillKind.LayupSpecial;
                    break;
            }
            UseGravity = false;
            IsKinematic = true;
//            Vector3 layupPoint = CourtMgr.Get.DunkPoint[Team.GetHashCode()].transform.position;
//            layupPoint.z += (Team == 0 ? -1 : 1);
//            AnimatorControl.InitLayupCurve(stateNo, layupPoint);
            SetShooterLayer();
            AnimatorControl.Play(EAnimatorState.Layup, stateNo, Team.GetHashCode());
            isCanCatchBall = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="checkUseState"> true: 要檢查動作可不可以使用; false: 不檢查動作是否可以使用, 動作直接切過去. </param>
    /// <returns></returns>
    public bool AniState(EPlayerState newState, bool checkUseState = true)
    {
        if(checkUseState && !CanUseState(newState))
            return false;

        mStateChangable = !AnimatorMgr.Get.IsForciblyStates(newState);
        TAnimatorItem nextAnimatorState = AnimatorMgr.Get.GetAnimatorState(newState);

        PlayerRigidbody.mass = 0.1f;
        UseGravity = true;
        IsKinematic = false;
        if (!isUsePass)
            isCanCatchBall = true;

        if (LayerMgr.Get.CheckLayer(PlayerRefGameObject, ELayer.Shooter))
            LayerMgr.Get.SetLayer(PlayerRefGameObject, ELayer.Player);

        if (IsDebugAnimation)
            Debug.LogWarningFormat("{0}, CurrentState:{1}, NewState:{2}, Time:{3}, StateChangable:{4}", 
                                   name, CurrentState, newState, Time.time, mStateChangable);

        DashEffectEnable(false);

        bool result = false;
        switch (nextAnimatorState.AnimatorState)
        {
            case EAnimatorState.Block:  
                BlockStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;
            case EAnimatorState.BlockCatch: 
                BlockCatchStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;
            case EAnimatorState.Buff:   
                BuffStateHandle(nextAnimatorState.StateNo);
                showActiveEffect(nextAnimatorState.StateNo);
                result = true;
                break;

            case EAnimatorState.Catch:
                CatchStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;

            case EAnimatorState.Defence:
                DefenceStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;

            case EAnimatorState.Dunk:
                DunkStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;

            case EAnimatorState.Alleyoop:
                AlleyoopStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;

            case EAnimatorState.Dribble:
                DribbleStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;

            case EAnimatorState.End:
                AnimatorControl.Play(EAnimatorState.End, nextAnimatorState.StateNo, Team.GetHashCode());
                result = true;
                break;
            case EAnimatorState.Elbow:
                ElbowStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;
            case EAnimatorState.FakeShoot:
                FakeShootStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;
            case EAnimatorState.KnockDown:
                KnockDownStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;

            case EAnimatorState.Fall:
                FallStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;

            case EAnimatorState.HoldBall:
                PlayerRigidbody.mass = 5;
                AnimatorControl.Play(EAnimatorState.HoldBall, nextAnimatorState.StateNo, Team.GetHashCode()); 
                isCanCatchBall = false;
                result = true;
                break;
            case EAnimatorState.Idle:
                PlayerRigidbody.mass = 5;
                setSpeed(0, -1);
                AnimatorControl.Play(EAnimatorState.Idle, nextAnimatorState.StateNo, Team.GetHashCode()); 
                isMoving = false;
                result = true;
                isCanCatchBall = true;
                break;
            case EAnimatorState.Intercept:
                AnimatorControl.Play(EAnimatorState.Intercept, nextAnimatorState.StateNo, Team.GetHashCode());
                result = true;
                break;
            case EAnimatorState.MoveDodge:
                AnimatorControl.Play(EAnimatorState.MoveDodge, nextAnimatorState.StateNo, Team.GetHashCode());
                OnUICantUse(this);
                if (moveQueue.Count > 0)
                    moveQueue.Dequeue();
                result = true;
                break;
            case EAnimatorState.Pass:
                isCanCatchBall = false;
                AnimatorControl.Play(EAnimatorState.Pass, nextAnimatorState.StateNo, Team.GetHashCode());
                if (nextAnimatorState.StateNo == 5 || nextAnimatorState.StateNo == 6 || nextAnimatorState.StateNo == 7 || nextAnimatorState.StateNo == 8 || nextAnimatorState.StateNo == 9)
                    isUsePass = true;
                PlayerRigidbody.mass = 5;
                GameRecord.Pass++;
                result = true;
                break;
            case EAnimatorState.Push:
                if (nextAnimatorState.StateNo == 20)
                    GameRecord.Push++;
                AnimatorControl.InitPushCurve(nextAnimatorState.StateNo);
                StartSkillCamera(nextAnimatorState.StateNo);
                AnimatorControl.Play(EAnimatorState.Push, nextAnimatorState.StateNo, Team.GetHashCode());
                GameRecord.PushLaunch++;
                result = true;
                AudioMgr.Get.PlaySound(SoundType.SD_Punch);
                break;
            case EAnimatorState.Pick:
                if (nextAnimatorState.StateNo == 2)
                {
                    AnimatorControl.InitPickCurve(nextAnimatorState.StateNo);
                    GameRecord.SaveBallLaunch++;
                }
                AnimatorControl.Play(EAnimatorState.Pick, nextAnimatorState.StateNo, Team.GetHashCode());
                result = true;
                break;
            case EAnimatorState.Run:
                if (!isJoystick)
                    setSpeed(1, 1); 
                if (nextAnimatorState.StateNo == 1)
                    DashEffectEnable(true);
                AnimatorControl.Play(EAnimatorState.Run, nextAnimatorState.StateNo, Team.GetHashCode());
                result = true;
                break;
            case EAnimatorState.Steal:
                if (nextAnimatorState.StateNo == 20)
                {
                    GameRecord.Steal++;
                    AnimatorControl.InitStealCurve(nextAnimatorState.StateNo);
                }
                StartSkillCamera(nextAnimatorState.StateNo);
                PlayerRigidbody.mass = 5;
                AnimatorControl.Play(EAnimatorState.Steal, nextAnimatorState.StateNo, Team.GetHashCode());
                isCanCatchBall = false;
                GameRecord.StealLaunch++;
                AudioMgr.Get.PlaySound(SoundType.SD_Steal);
                result = true;
                break;
            case EAnimatorState.GotSteal:
                AnimatorControl.Play(EAnimatorState.GotSteal, nextAnimatorState.StateNo, Team.GetHashCode());
                isCanCatchBall = false;
                result = true;
                break;
            case EAnimatorState.Shoot:
                ShootStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;
            case EAnimatorState.Show:
                AnimatorControl.Play(EAnimatorState.Show, nextAnimatorState.StateNo, Team.GetHashCode());
                result = true;
                break;

            case EAnimatorState.Layup:
                LayupStateHandle(nextAnimatorState.StateNo);
                result = true;
                break;

            case EAnimatorState.Rebound:
                if (nextAnimatorState.StateNo == 20)
                    GameRecord.Rebound++;

                UseGravity = false;
                IsKinematic = true;
            
                StartSkillCamera(nextAnimatorState.StateNo);
                skillMoveTarget = CourtMgr.Get.RealBall.transform.position;

                Vector3 horizontalMove = CourtMgr.Get.RealBall.transform.position - transform.position;
//                horizontalMove += CourtMgr.Get.RealBall.MoveVelocity * 0.3f;
         
                SetShooterLayer();
                AnimatorControl.Play(EAnimatorState.Rebound, nextAnimatorState.StateNo, Team.GetHashCode(), skillMoveTarget, horizontalMove);
                GameRecord.ReboundLaunch++;
                result = true;
                break;
            case EAnimatorState.JumpBall:
                AnimatorControl.Play(EAnimatorState.JumpBall, nextAnimatorState.StateNo, Team.GetHashCode());
                SetShooterLayer();
                result = true;
                break;
            case EAnimatorState.TipIn:
                AnimatorControl.Play(EAnimatorState.TipIn, nextAnimatorState.StateNo, Team.GetHashCode());
                SetShooterLayer();
                result = true;
                break;
        }
        
        if (result)
        {
            CurrentState = newState;
            CurrentAnimatorState = nextAnimatorState.AnimatorState;

            if (CurrentState == EPlayerState.Idle && NeedResetFlag)
                ResetFlag();
        }

        return result;
    }

    public void SetShooterLayer()
    {
        PlayerRefGameObject.layer = LayerMask.NameToLayer("Shooter");
    }

    #region 動作委託

    private void InitAnmator()
    {
        AnimatorControl = PlayerRefGameObject.AddComponent<AnimatorController>();
        AnimatorControl.Init(PlayerRefGameObject.GetComponent<Animator>());
        AnimatorControl.GotStealingDel = GotStealing;
        AnimatorControl.FakeShootBlockMomentDel = FakeShootBlockMoment;
        AnimatorControl.BlockMomentDel = BlockMoment;
        AnimatorControl.AirPassMomentDel = AirPassMoment;
        AnimatorControl.DoubleClickMomentDel = DoubleClickMoment;
        AnimatorControl.BuffEndDel = buffEnd;
        AnimatorControl.BlockCatchMomentStartDel = BlockCatchMomentStart;
        AnimatorControl.BlockCatchMomentEndDel = blockCatchMomentEnd;
        AnimatorControl.BlockJumpDel = BlockJump;
        AnimatorControl.BlockCatchingEndDel = blockCatchingEnd;
        AnimatorControl.ShootingDel = Shooting;
        AnimatorControl.MoveDodgeEndDel = moveDodgeEnd;
        AnimatorControl.PassingDel = Passing;
        AnimatorControl.PassEndDel = passEnd;
        AnimatorControl.ShowEndDel = showEnd;
        AnimatorControl.PickUpDel = PickUp;
        AnimatorControl.PickEndDel = pickEnd;
        AnimatorControl.StealingDel = stealing;
        AnimatorControl.StealingEndDel = stealingEnd;
        AnimatorControl.PushCalculateStartDel = PushCalculateStart;
        AnimatorControl.PushCalculateEndDel = PushCalculateEnd;
        AnimatorControl.ElbowCalculateStartDel = ElbowCalculateStart;
        AnimatorControl.ElbowCalculateEndDel = ElbowCalculateEnd;
        AnimatorControl.BlockCalculateStartDel = BlockCalculateStart;
        AnimatorControl.BlockCalculateEndDel = BlockCalculateEnd;
		AnimatorControl.ReboundCalculateStartDel = ReboundCalculateStart;
		AnimatorControl.ReboundCalculateEndDel = ReboundCalculateEnd;			
        AnimatorControl.CloneMeshDel = CloneMesh;
        AnimatorControl.DunkBasketStartDel = DunkBasketStart;
        AnimatorControl.OnlyScoreDel = OnlyScore;
        AnimatorControl.DunkFallBallDel = DunkFallBall;
        AnimatorControl.ElbowEndDel = elbowEnd;
        AnimatorControl.FallEndDel = fallEnd;
        AnimatorControl.FakeShootEndDel = fakeShootEnd;
        AnimatorControl.TipInStartDel = tipInStart;
        AnimatorControl.TipInEndDel = tipInEnd;
        AnimatorControl.AnimationEndDel = animationEnd;
        AnimatorControl.ShowDel = OnShowCallBack;
        AnimatorControl.SkillDel = SkillEventCallBack;
        AnimatorControl.ZoomInDel = OnZoomIn;
        AnimatorControl.ZoomOutDel = OnZoomOut;
        AnimatorControl.CatchDel = catchEnd;
    }

    private void OnShowCallBack()
    {
        if (IsBallOwner)
            AniState(EPlayerState.HoldBall);
        else
            AniState(EPlayerState.Idle);
    }

    private void GotStealing()
    {
        if (IsShowSituation)
            return;
        
        if (OnGotSteal != null && OnGotSteal(this))
            GameRecord.BeSteal++; 
    }

    private void FakeShootBlockMoment()
    {
        if (IsShowSituation)
            return;
        
        if (!IsAllShoot && OnFakeShootBlockMoment != null)
            OnFakeShootBlockMoment(this);
    }

    private void BlockMoment()
    {
        if (IsShowSituation)
            return;
        
        if (OnBlockMoment != null)
            OnBlockMoment(this);
    }

    private void AirPassMoment()
    {
        if (IsShowSituation)
            return;
        
        mIsPassAirMoment = true;
    }

    private void DoubleClickMoment()
    {
        if (IsShowSituation)
            return;
        
        if (OnDoubleClickMoment != null)
            OnDoubleClickMoment(this, CurrentState);
    }

    private void buffEnd(EAnimatorState state)
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. buffEnd. Time:{1}", name, Time.time);

        if (state == CurrentAnimatorState)
            mStateChangable = true;

        PlayerSkillController.ResetUseSkill();
        if (IsBallOwner)
        {
            AniState(EPlayerState.HoldBall);
            IsFirstDribble = true;
        } else
            AniState(EPlayerState.Idle);
    }

    private void BlockCatchMomentStart()
    {
        if (IsShowSituation)
            return;
        
        blockCatchTrigger.SetEnable(true); 
    }

    private void blockCatchMomentEnd()
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. blockCatchMomentEnd. Time:{1}", name, Time.time);

        mStateChangable = true;
        IsPerfectBlockCatch = false;
    }

    private void BlockJump()
    {
        if (IsShowSituation)
            return;
        if (OnBlockJump != null)
            OnBlockJump(this); 
    }

    private void blockCatchingEnd()
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. blockCatchingEnd. Time:{1}", name, Time.time);

        mStateChangable = true;
        if (IsBallOwner)
        {
            IsFirstDribble = true;
            AniState(EPlayerState.HoldBall);
        }
        else
            AniState(EPlayerState.Idle);

        IsPerfectBlockCatch = false;
    }

    private void Shooting()
    {
        if (IsShowSituation)
            return;
        
        mIsPassAirMoment = false;
        if (OnShooting != null)
        {
            if (CurrentState != EPlayerState.Pass4)
                OnShooting(this, false);
            else 
            if (CurrentState == EPlayerState.Layup0)
            {
                if (CourtMgr.Get.RealBall.transform.parent == DummyBall.transform)
                {
                    LogMgr.Get.Log(PlayerRefGameObject.name + " layup no ball.");
                    GameController.Get.SetBall();
                }
            }
        }
    }

    private void moveDodgeEnd(EAnimatorState state)
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. moveDodgeEnd. Time:{1}", name, Time.time);

        if (state == CurrentAnimatorState)
            mStateChangable = true;

        OnUI(this);
        PlayerSkillController.ResetUseSkill();
        if (IsBallOwner)
            AniState(EPlayerState.Dribble0);
        else
            AniState(EPlayerState.Idle);
    }

    private void Passing()
    {
        if (IsShowSituation)
            return;
        
        //0.Flat
        //2.Floor
        //1 3.Parabola(Tee)
        if (IsBallOwner)
        {
            if(GameController.Get.IsCatcherAlleyoop)
                CourtMgr.Get.RealBall.Trigger.PassBall(99);   
            else
                CourtMgr.Get.RealBall.Trigger.PassBall(AnimatorControl.StateNo);

            PlayerSkillController.ResetUseSkill();
            GameController.Get.IsCatcherAlleyoop = false;
        } 
    }

    private void passEnd(EAnimatorState state)
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. passEnd. Time:{1}", name, Time.time);

        if (state == CurrentAnimatorState)
            mStateChangable = true;

        OnUI(this);

        if (!IsBallOwner && PlayerRefGameObject.transform.localPosition.y < 0.2f)
            AniState(EPlayerState.Idle); 
    }

    private void showEnd(EAnimatorState state)
    {
        if (IsDebugAnimation)
            Debug.LogFormat("{0}. showEnd. Time:{1}", name, Time.time);

        if (state == CurrentAnimatorState)
            mStateChangable = true;

        AniState(!IsBallOwner ? EPlayerState.Idle : EPlayerState.HoldBall);
    }

    private void PickUp()
    {
        if (IsShowSituation)
            return;
        
        if (OnPickUpBall != null && OnPickUpBall(this))
            GameRecord.SaveBall++;
    }

    private void pickEnd(EAnimatorState state)
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. pickEnd. Time:{1}", name, Time.time);

        if (state == CurrentAnimatorState)
            mStateChangable = true;

        if (IsBallOwner)
        {
            IsFirstDribble = true;
            AniState(EPlayerState.HoldBall);
        } else
            AniState(EPlayerState.Idle); 
    }

    private void stealing()
    {
        if (IsShowSituation)
            return;
        
        IsStealCalculate = true; 
    }

    private void stealingEnd()
    {
        if (IsShowSituation)
			return;
		
		PlayerSkillController.ResetUseSkill();

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. stealingEnd. Time:{1}", name, Time.time);

        mStateChangable = true;
        IsStealCalculate = false; 
    }

    private void PushCalculateStart()
    {
        if (IsShowSituation)
            return;
        
        IsPushCalculate = true;
    }

    private void PushCalculateEnd()
    {
        if (IsShowSituation)
            return;
        
        IsPushCalculate = false; 
    }

    private void ElbowCalculateStart()
    {
        if (IsShowSituation)
            return;
        
        IsElbowCalculate = true;  
    }

    private void ElbowCalculateEnd()
    {
        IsElbowCalculate = false;
    }

    private void BlockCalculateStart()
    {
        if (IsShowSituation)
            return;
        
        blockTrigger.SetActive(true);
    }

    private void BlockCalculateEnd()
    {
        if (IsShowSituation)
            return;
        
        blockTrigger.SetActive(false); 
    }

	private void ReboundCalculateStart()
	{
		reboundTrigger.SetActive (true);
	}

	private void ReboundCalculateEnd()
	{
		reboundTrigger.SetActive (false);
	}

    private void CloneMesh()
    {
        if (IsShowSituation)
            return;
        
        if (!IsBallOwner)
            AnimatorControl.PlayDunkCloneMesh();   
    }

    private void DunkBasketStart()
    {
        if (IsShowSituation)
            return;
        
        CourtMgr.Get.PlayDunk(Team.GetHashCode(), AnimatorControl.StateNo); 
    }

    private void OnlyScore()
    {
        if (IsShowSituation)
            return;
        
        if (OnOnlyScore != null)
            OnOnlyScore(this);
        
        CourtMgr.Get.IsBallOffensive = true;
    }

    private void DunkFallBall()
    {
		OnUI(this);
		PlayerSkillController.ResetUseSkill();
        if (OnDunkBasket != null)
            OnDunkBasket(this);
        
        CourtMgr.Get.IsBallOffensive = false;
    }

    private void elbowEnd(EAnimatorState state)
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. elbowEnd. Time:{1}", name, Time.time);

        if(state == CurrentAnimatorState)
            mStateChangable = true;

        PlayerSkillController.ResetUseSkill();
        OnUI(this);
        if (IsBallOwner)
            AniState(EPlayerState.HoldBall);
        else
            AniState(EPlayerState.Idle);
        CourtMgr.Get.RealBall.ShowBallSFX(Attr.PunishTime); 
    }

    private void fallEnd(EAnimatorState state)
    {
        if (IsShowSituation)
			return;
		PlayerSkillController.ResetUseSkill();

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. fallEnd. Time:{1}", name, Time.time);

        if (state == CurrentAnimatorState)
            mStateChangable = true;

        OnUI(this);
        InitFlag();
        AniState(EPlayerState.Idle); 
    }
				
    private void catchEnd(EAnimatorState state)
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. catchEnd. Time:{1}", name, Time.time);

		PlayerSkillController.ResetUseSkill();
        if (state == CurrentAnimatorState)
            mStateChangable = true;

        if (situation == EGameSituation.GamerInbounds || situation == EGameSituation.NPCInbounds)
        {
            if (IsBallOwner)
                AniState(EPlayerState.Dribble0);
            else
                AniState(EPlayerState.Idle);
        }
        else
        {
            OnUI(this);
            IsFirstDribble = true;
            if (!IsBallOwner)
            {                   
                AniState(EPlayerState.Idle);
            }
            else
            {
                if (AIing)
                    AniState(EPlayerState.Dribble0);
                else
                    AniState(EPlayerState.HoldBall);
            }
        }
    }

    private void fakeShootEnd()
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. fakeShootEnd. Time:{1}", name, Time.time);

        mStateChangable = true;
        isFakeShoot = false;
        if (IsBallOwner)
            AniState(EPlayerState.HoldBall);
        else
            AniState(EPlayerState.Idle);

        OnUI(this);
        CourtMgr.Get.RealBall.ShowBallSFX(Attr.PunishTime);
    }

    private void tipInStart()
    {
        if (IsShowSituation)
            return;
        
        CanUseTipIn = true;
    }

    private void tipInEnd()
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. tipInEnd. Time:{1}", name, Time.time);

        mStateChangable = true;
		CanUseTipIn = false;
		PlayerSkillController.ResetUseSkill();
    }

    private void animationEnd(EAnimatorState animatorState)
    {
        if (IsShowSituation)
            return;

        if (IsDebugAnimation)
            Debug.LogFormat("{0}. animationEnd. Time:{1}", name, Time.time);

        if(animatorState == CurrentAnimatorState)
            mStateChangable = true;

        OnUI(this);
		pushThroughTigger.SetActive(false);
		isBlock = false;
        PlayerSkillController.ResetUseSkill();

		if(!IsBallOwner) 
			AniState(EPlayerState.Idle);
        else if(IsAllShoot)
            AniState(EPlayerState.Fall0);
		else if(firstDribble)
            AniState(EPlayerState.Dribble0);

        if (LayerMgr.Get.CheckLayer(PlayerRefGameObject, ELayer.Shooter))
            LayerMgr.Get.SetLayer(PlayerRefGameObject, ELayer.Player);

        InitFlag();
    }

    private void OnZoomIn(float t)
    {
        CameraMgr.Get.SkillShow(gameObject); 
        CameraMgr.Get.SetRoomMode(EZoomType.In, t); 
    }

    private void OnZoomOut(float t)
    {
        CameraMgr.Get.SkillShow(gameObject);
        CameraMgr.Get.SetRoomMode(EZoomType.Out, t); 
    }

    #endregion

    public void InitFlag()
    {
        CanUseTipIn = false;
        isUsePass = false;
        isCanCatchBall = true;
        mIsPassAirMoment = false;
        blockTrigger.SetActive(false);
		pushThroughTigger.SetActive(false);
		reboundTrigger.SetActive (false);
        UseGravity = true;
        IsKinematic = false;
        IsPerfectBlockCatch = false;
//        isRebound = false;
        blockCatchTrigger.enabled = false;
		
        if (NeedResetFlag)
            ResetFlag();
    }

    public void EffectEvent(string effectName)
    {
        switch (effectName)
        {
            case "FallDownFX":
                EffectManager.Get.PlayEffect(effectName, PlayerRefGameObject.transform.position, null, null, 3);
                AudioMgr.Get.PlaySound(SoundType.SD_Fall);
                break;
            case "ShakeFX_0":
                EffectManager.Get.PlayEffect(effectName, new Vector3(PlayerRefGameObject.transform.position.x, 1.5f, PlayerRefGameObject.transform.position.z), null, null, 0.5f);
                break;
            case "JumpFX":
                EffectManager.Get.PlayEffect(effectName, new Vector3(PlayerRefGameObject.transform.position.x, 0f, PlayerRefGameObject.transform.position.z), null, null, 0.5f);
                break;
        }
    }

    public void PlaySound(string soundName)
    {
        AudioMgr.Get.PlaySound(soundName);
    }
       
    //All Skill Event From this Function
    public void SkillEventCallBack(AnimationEvent aniEvent)
    {
		string skillString = aniEvent.stringParameter;
		int skillInt = aniEvent.intParameter;
		
		switch (skillString)
		{
		case "CameraAction": 
			CameraMgr.Get.CourtCameraAnimator.SetTrigger("CameraAction_" + skillInt);
			break;
		case "Shooting":
			CourtMgr.Get.IsRealBallActive = true;
			//主動技也可以被蓋 (20160323)
			//                GameController.Get.BallState = EBallState.None;//主動技要可以被蓋所以註解
			GameController.Get.BallState = EBallState.CanBlock;
			if (GameController.Get.ShootDistance >= GameConst.Point3Distance)
				GameRecord.FG3++;
			else
				GameRecord.FG++;
			
			if (OnShooting != null)
				OnShooting(this, true);
			break;
		case "PushDistancePlayer":
			if (GameData.DSkillData.ContainsKey(ActiveSkillUsed.ID) && IsGameAttack)
			{
				GameRecord.PushLaunch++;
				for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
				{
					if (GameController.Get.GamePlayers[i].Team != Team)
					{
						if(GameData.DSkillData[ActiveSkillUsed.ID].Kind == 171) {
							//直線碰撞
							pushThroughTigger.SetActive(true);
							transform.DOMove(CourtMgr.Get.GetArrowPosition(PlayerRefGameObject, GameData.DSkillData[ActiveSkillUsed.ID].Distance(ActiveSkillUsed.Lv)), 0.5f);
						} else {
							//圓形碰撞
							if (GameController.Get.GetDis(new Vector2(GameController.Get.GamePlayers[i].transform.position.x, GameController.Get.GamePlayers[i].transform.position.z), 
								new Vector2(PlayerRefGameObject.transform.position.x, PlayerRefGameObject.transform.position.z)) <= GameData.DSkillData[ActiveSkillUsed.ID].Distance(ActiveSkillUsed.Lv))
							{
								if (GameController.Get.GamePlayers[i].IsAllShoot || GameController.Get.GamePlayers[i].IsDunk){ 
									GameController.Get.GamePlayers[i].AniState(EPlayerState.KnockDown0, PlayerRefGameObject.transform.position);
								} else {
									GameController.Get.GamePlayers[i].PlayerSkillController.DoPassiveSkill(ESkillSituation.Fall1, PlayerRefGameObject.transform.position);
								}
								GameRecord.Push++;
							}
						}
					} 
				}
				GameController.Get.IsGameFinish();
			}
			break;
		case "SetBallEvent":
			//抄截後有１秒無敵
			SetInvincible(1);
			GameController.Get.SetBall(this);
			GameController.Get.IsGameFinish();
			
			if (GameController.Get.Catcher != null)
				GameController.Get.Catcher = null;
			if (GameController.Get.Passer != null)
				GameController.Get.Passer = null;
			if (GameController.Get.Shooter != null)
				GameController.Get.Shooter = null;
			break;
		case "ReboundSetBall":
			if(GameController.Get.BallOwner == null && PlayerSkillController.IsActiveUse) {
				GameController.Get.SetBall(this);
				GameController.Get.IsGameFinish();
			}
			break;
		case "ActiveSkillEnd":
			if (OnUIJoystick != null)
				OnUIJoystick(this, true);
			
			UISkillEffect.UIShow(false);
			
			if (isBlock)
			{
				if (GameController.Get.BallState == EBallState.CanBlock)
				{
					TimerMgr.Get.PauseBall(false);
					skillFaceTarget = judgePlayerFace(PlayerRefGameObject.transform.eulerAngles.y);
					Vector3 pos = new Vector3(skillFaceTarget.x, -1, skillFaceTarget.z) * 20;
					CourtMgr.Get.RealBall.AddForce(pos, ForceMode.VelocityChange);
				}
				else if (GameController.Get.BallState == EBallState.CanDunkBlock)
				{
					if (GameController.Get.BallOwner != null)
						GameController.Get.BallOwner.AniState(EPlayerState.KnockDown0);
				}
				GameController.Get.BallState = EBallState.None;
				GameController.Get.IsGameFinish();
			}
			break;
		}
    }

    private Vector3 judgePlayerFace(float angle)
    {
        if (angle <= 22.5f && angle >= -22.5) //Left
            return Vector3.forward;
        else if (angle > 22.5f && angle <= 67.5f) //LeftUp
            return new Vector3(1, 0, 1);
        else if (angle < -22.5f && angle >= -67.5f)
            return new Vector3(-1, 0, 1);//LeftDown
        else if (angle > 67.5f && angle <= 112.5f)
            return Vector3.right;//Up
        else if (angle < -67.5f && angle >= -112.5f)
            return Vector3.left;//Down
        else if (angle > 112.5f && angle <= 157.5f)
            return new Vector3(1, 0, -1);//Right UP
        else if (angle < -112.5f && angle >= -157.5f)
            return new Vector3(-1, 0, -1);//Right Down
        else if (angle > 157.5f || angle < -157.5f)
            return Vector3.back;//Right 
        return Vector3.zero;
    }

    private void ResetTimeCallBack()
    {
        if (GameController.Get.IsStart)
            TimerMgr.Get.ResetTime();  
    }
		
	//TODO:因為規則改成被動技跟主動技會用相同動作，因此不能用20以上做判斷
    public void StartSkillCamera(int no)
    {
		if (no >= 20) {
			if (GameData.DSkillData.ContainsKey(PassiveSkillUsed.ID) && !PlayerSkillController.IsActiveUse) {
            	UIPassiveEffect.Get.ShowView(PassiveSkillUsed, this);
				return;
			} else
				PlayerSkillController.ResetUsePassive();
		}
	
        if (GameData.DSkillData.ContainsKey(ActiveSkillUsed.ID))
        {
            int skillEffectKind = GameData.DSkillData[ActiveSkillUsed.ID].ActiveCamera;
            float skillTime = GameData.DSkillData[ActiveSkillUsed.ID].ActiveCameraTime;
            TimerMgr.Get.PauseTimeByUseSkill(skillTime, ResetTimeCallBack);
            if (!isSkillShow)
            {
                if (OnUIJoystick != null)
                    OnUIJoystick(this, false);
                
                isSkillShow = true;
                if (GameController.Get.BallOwner != null)
                    LayerMgr.Get.SetLayerRecursively(CourtMgr.Get.RealBall.gameObject, "SkillPlayer", "RealBall");
				
                CameraMgr.Get.SkillShowActive(this, skillEffectKind, skillTime);
                AudioMgr.Get.PlaySound(SoundType.SD_ActiveLaunch);
                switch (skillEffectKind)
                {
                    case 0://show self and rotate camera
						string effectName = string.Format("UseSkillEffect_{0}", 0);
						EffectManager.Get.PlayEffect(effectName, transform.position, null, null, 1, false);
						Invoke("showEffect", skillTime);
						if (UIPassiveEffect.Visible)
							UIPassiveEffect.UIShow(false);
						UISkillEffect.Get.ShowView(ActiveSkillUsed);
						TimerMgr.Get.PauseTimeByUseSkill(skillTime, ResetTimeCallBack);
                        LayerMgr.Get.SetLayerRecursively(PlayerRefGameObject, "SkillPlayer", "PlayerModel", "(Clone)");
                        break;
                    case 1://show self
                    case 2://show all Player
						// kind= 2, time = 0.5f
							showActiveEffect();
							UIPassiveEffect.Get.ShowView(ActiveSkillUsed, this);
							GameController.Get.SetAllPlayerLayer("SkillPlayer");
							TimerMgr.Get.PauseTimeByUseSkill(0.5f, ResetTimeCallBack);
							Pause = false;
                        break;
                }
            } else 
				PlayerSkillController.ResetUseActive();
        }
    }

    public void StopSkill()
    {
        isSkillShow = false;
    }

    public void showEffect()
    {
        SkillEffectManager.Get.OnShowEffect(this, false);
    }

    public void showActiveEffect(int no = 20)
    {
        if (no >= 20)
            SkillEffectManager.Get.OnShowEffect(this, false);
    }

    public void ResetMove()
    {
        DribbleTime = 0;
        clearMoveData();
        CantMoveTimer.Clear();
    }

    private void clearMoveData()
    {
        moveQueue.Clear(); 
        setDebugSpherePos(transform.position);
    }

    public void SetAutoFollowTime()
    {
        if (CloseDef == 0 && AutoFollow == false)
        {
            CloseDef = Time.time + Attr.AutoFollowTime;
        }           
    }

    public void ClearAutoFollowTime()
    {
        CloseDef = 0;
        AutoFollow = false;
    }

    //=====Skill=====
    public bool DoActiveSkill(GameObject target = null)
    {
        if (CanUseActiveSkill(ActiveSkillUsed) || TestMode == EGameTest.Skill)
        {
            GameRecord.Skill++;
            SetAnger(-Attribute.MaxAngerOne(ActiveSkillUsed.ID, ActiveSkillUsed.Lv));

            if (Attribute.SkillAnimation(ActiveSkillUsed.ID) != "")
            {
                if (target)
                    return AniState((EPlayerState)System.Enum.Parse(typeof(EPlayerState), Attribute.SkillAnimation(ActiveSkillUsed.ID)), target.transform.position);
                else
                {
                    try
                    {
                        return AniState((EPlayerState)System.Enum.Parse(typeof(EPlayerState), Attribute.SkillAnimation(ActiveSkillUsed.ID)));
                    }
                    catch
                    {
                        LogMgr.Get.LogError("Can't find SkillAnimation in EPlayerState");
                        return false;
                    }
                }
            }
        }
        return false;
    }

    public void SetAttribute(int kind, float value)
    {
        Attribute.AddAttribute(kind, value);
        initAttr();
    }

    public bool CheckSkillDistance(TSkill tSkill)
    {
        bool result = false;
        if (PlayerSkillController.GetActiveSkillTarget(tSkill) != null && PlayerSkillController.GetActiveSkillTarget(tSkill).Count > 0)
            for (int i = 0; i < PlayerSkillController.GetActiveSkillTarget(tSkill).Count; i++)
                if (PlayerSkillController.CheckSkillDistance(tSkill, PlayerSkillController.GetActiveSkillTarget(tSkill)[i]))
                    result = true;
        
        return result;
    }

	public float GetPushThroughW {
		get {return pushThroughTigger.transform.localScale.x;}
	}

    public TSkill ActiveSkillUsed
    {
        get { return PlayerSkillController.ActiveSkillUsed; }
        set { PlayerSkillController.ActiveSkillUsed = value; }
    }

    public TSkill PassiveSkillUsed
    {
        get { return PlayerSkillController.PassiveSkillUsed; }
        set { PlayerSkillController.PassiveSkillUsed = value; }
    }

    public ESkillKind GetSkillKind
    {
        get{ return skillKind; }
    }

    public int GetPassiveAniRate(int kind, float shootDistance, float baseDistance)
    {
        if (PlayerSkillController.DExtraPassiveSkills.ContainsKey(kind))
        {
            int rate = 0;
            for (int i = 0; i < PlayerSkillController.DExtraPassiveSkills[kind].Count; i++)
            {
                if ((GameData.DSkillData[PlayerSkillController.DExtraPassiveSkills[kind][i].Tskill.ID].Distance(PlayerSkillController.DExtraPassiveSkills[kind][i].Tskill.Lv) + baseDistance) >= shootDistance)
                    rate += GameData.DSkillData[PlayerSkillController.DExtraPassiveSkills[kind][i].Tskill.ID].AniRate(PlayerSkillController.DExtraPassiveSkills[kind][i].Tskill.Lv);
            }
            return rate;
        }
        else
            return 0;
    }

    public bool CanPressButton
    {
        get { return (timeScale == 1); }
    }

    public bool IsAlleyoopState
    {
        get{ return CheckAnimatorSate(EPlayerState.Alleyoop); }
    }

    public bool CanMove
    {
        get
        {
            if(IsUseActiveSkill || 
               StateChecker.StopStates.ContainsKey(CurrentState) || 
               IsFall || IsShoot || IsDunk || IsLayup)
                return false;
            return true;
        }
    }

    public bool CanMoveFirstDribble
    {
        get
        {
            if (CheckAnimatorSate(EPlayerState.HoldBall) && IsFirstDribble)
                return true;
            else
                return false;
        }
    }

    public bool CanUseActiveSkill(TSkill tSkill)
    {
        if ((CanMove || CurrentState == EPlayerState.HoldBall) && IsAngerFull(tSkill))
            return true;

        return false;
    }

    public bool HoldBallCanMove
    {
        get
        {
            if (CheckAnimatorSate(EPlayerState.HoldBall) && IsFirstDribble)
                return true;
            else
                return false;
        }
    }

    public bool IsSkillShow
    {
        get { return isSkillShow; }
    }

    public bool IsCatcher
    {
        get{ return CheckAnimatorSate(EPlayerState.CatchFlat); }
    }

    /// <summary>
    ///動作期間是否可以接球.
    /// </summary>
    /// <value><c>true</c> if this instance is can catch ball; otherwise, <c>false</c>.</value>
    public bool IsCanCatchBall
    {
        get{ return isCanCatchBall; }
        set{ isCanCatchBall = value; }
    }

    public bool IsCanBlock
    {
        get
        { 
            return GameController.Get.BallState == EBallState.CanDunkBlock;
        }
    }

    public bool IsHoldBall
    {
        get{ return CurrentState == EPlayerState.HoldBall; }
    }

    public bool IsDefence
    {
        get
        {
            if (situation == EGameSituation.GamerAttack && Team == ETeamKind.Npc)
                return true;
            if (situation == EGameSituation.NPCAttack && Team == ETeamKind.Self)
                return true;
            return false;
        }
    }

    public bool IsJump
    {
        get{ return PlayerRefGameObject.transform.localPosition.y > 0.1f; }
    }

    public bool IsBallOwner
    {
        get { return AnimatorControl.Controler.GetBool("IsBallOwner"); }
        set { AnimatorControl.Controler.SetBool("IsBallOwner", value); }
    }

    public bool IsUseActiveSkill
    {
        get
        {
            return (CurrentState == EPlayerState.Buff20 || CurrentState == EPlayerState.Buff21 || CurrentState == EPlayerState.Block20 ||
            CurrentState == EPlayerState.Dunk20 || CurrentState == EPlayerState.Dunk21 || CurrentState == EPlayerState.Dunk22 ||
            CurrentState == EPlayerState.Elbow20 || CurrentState == EPlayerState.Elbow21 ||
            CurrentState == EPlayerState.Push20 ||
            CurrentState == EPlayerState.Rebound20 ||
            CurrentState == EPlayerState.Shoot20 ||
            CurrentState == EPlayerState.Steal20);
        }
    }

    public bool IsBlock
    {
        get{ return CurrentState == EPlayerState.Block0 || CurrentState == EPlayerState.Block1 || CurrentState == EPlayerState.Block2 || CurrentState == EPlayerState.Block20; }
    }

    public bool IsBlockCatch
    {
        get{ return CurrentState == EPlayerState.BlockCatch; } 
    }

    public bool IsShoot
    {
        get { return StateChecker.ShootStates.ContainsKey(CurrentState); }
    }

    public bool IsShow
    {
        get{ return StateChecker.ShowStates.ContainsKey(CurrentState); }
    }

    public bool IsLoopState
    {
        get{ return StateChecker.LoopStates.ContainsKey(CurrentState); }
    }

    public bool IsBuff
    {
        get { return CurrentState == EPlayerState.Buff20 || CurrentState == EPlayerState.Buff21; }
    }

    public bool IsAllShoot
    {
        get{ return IsShoot || IsDunk || IsLayup; }
    }

    public bool IsIdle
    {
        get{ return CurrentState == EPlayerState.Idle; }
    }

    public bool IsDef
    {
        get{ return CurrentState == EPlayerState.Defence0 || CurrentState == EPlayerState.Defence1; }
    }

    public bool IsRun
    {
        get{ return CurrentState == EPlayerState.Run0 || CurrentState == EPlayerState.Run1 || CurrentState == EPlayerState.Run2; }
    }

    public bool IsPass
    {
        get{ return StateChecker.PassStates.ContainsKey(CurrentState); }
    }

    public bool IsPickBall
    {
        get{ return CurrentState == EPlayerState.Pick0 || CurrentState == EPlayerState.Pick1 || CurrentState == EPlayerState.Pick2; }
    }

    public bool IsDribble
    {
        get{ return CurrentState == EPlayerState.Dribble0 || CurrentState == EPlayerState.Dribble1 || CurrentState == EPlayerState.Dribble2 || CurrentState == EPlayerState.Dribble3; }
    }

    public bool IsDunk
    {
        get{ return CurrentState == EPlayerState.Dunk0 || CurrentState == EPlayerState.Dunk1 || CurrentState == EPlayerState.Dunk3 || CurrentState == EPlayerState.Dunk5 || CurrentState == EPlayerState.Dunk7 || CurrentState == EPlayerState.Dunk2 || CurrentState == EPlayerState.Dunk4 || CurrentState == EPlayerState.Dunk6 || CurrentState == EPlayerState.Dunk20 || CurrentState == EPlayerState.Dunk21 || CurrentState == EPlayerState.Dunk22; }
    }

    public bool IsLayup
    {
        get{ return CurrentState == EPlayerState.Layup0 || CurrentState == EPlayerState.Layup1 || CurrentState == EPlayerState.Layup2 || CurrentState == EPlayerState.Layup3; }
    }

    public bool IsMoveDodge
    {
        get{ return CurrentState == EPlayerState.MoveDodge0 || CurrentState == EPlayerState.MoveDodge1; }
    }

    public bool IsSteal
    {
        get{ return CurrentState == EPlayerState.Steal0 || CurrentState == EPlayerState.Steal1 || CurrentState == EPlayerState.Steal2 || CurrentState == EPlayerState.Steal20; }
    }

    public bool IsGotSteal
    {
        get{ return CurrentState == EPlayerState.GotSteal; }
    }

    public bool IsTipIn
    {
        get{ return CurrentState == EPlayerState.TipIn; }
    }

    public bool IsKnockDown
    {
        get{ return CurrentState == EPlayerState.KnockDown0 || CurrentState == EPlayerState.KnockDown1; }
    }

    private bool isMoving = false;

    public bool IsMoving
    {
        get{ return isMoving; }
    }

    public bool IsRebound
    {
        get
        {
            return CurrentState == EPlayerState.Rebound0 || 
                   CurrentState == EPlayerState.Rebound20 || 
                   CurrentState == EPlayerState.ReboundCatch;
        }
    }

    public bool IsFall
    {
        get
        {
            return CurrentState == EPlayerState.Fall0 || 
                   CurrentState == EPlayerState.Fall1 || 
                   CurrentState == EPlayerState.Fall2 || 
                   CurrentState == EPlayerState.KnockDown0 || 
                   CurrentState == EPlayerState.KnockDown1;
        }
    }

    /// <summary>
    /// true: 人在空中; false: 人在地面.
    /// </summary>
    public bool IsInAir
    {
        get
        {
            // 用 0.03 來判斷, 是因為我認為這樣才比較不會有數值誤差的問題.
            return transform.position.y >= 0.03f;
        }
    }

    public bool IsInGround
    {
        get { return !IsInAir; }
    }

    public bool IsCatch
    {
        get{ return CurrentState == EPlayerState.CatchFlat || CurrentState == EPlayerState.CatchFloor || CurrentState == EPlayerState.CatchParabola; }
    }

    public bool IsFakeShoot
    {
        get{ return isFakeShoot; }
    }

    public bool IsPush
    {
        get{ return CurrentState == EPlayerState.Push0 || CurrentState == EPlayerState.Push1 || CurrentState == EPlayerState.Push2 || CurrentState == EPlayerState.Push20; }
    }

	public bool IsSkillPushThrough 
	{
		get {
			if(GameData.DSkillData.ContainsKey(PlayerSkillController.ActiveSkillUsed.ID))
				if(GameData.DSkillData[PlayerSkillController.ActiveSkillUsed.ID].Kind == 171)
					return true;
			
			return false;
		}
	}

    public bool IsIntercept
    {
        get{ return CurrentState == EPlayerState.Intercept0 || CurrentState == EPlayerState.Intercept1; }
    }

    public bool IsElbow
    {
        get{ return CurrentState == EPlayerState.Elbow0 || CurrentState == EPlayerState.Elbow1 || CurrentState == EPlayerState.Elbow2 || CurrentState == EPlayerState.Elbow20 || CurrentState == EPlayerState.Elbow21; }
    }

    private bool isPerfectBlockCatch = false;

    public bool IsPerfectBlockCatch
    {
        get{ return isPerfectBlockCatch; }
        set
        {
            isPerfectBlockCatch = value;

            if (!isPerfectBlockCatch)
            {
                blockCatchTrigger.SetEnable(false);
            }
            else
            {
                if (OnDoubleClickMoment != null)
                    OnDoubleClickMoment(this, CurrentState);
            }
        }
    }

    public bool IsFirstDribble
    {
        get{ return firstDribble; }
        set{ firstDribble = value; }
    }

    public float AngerPower
    {
        get{ return angerValue; }
    }

    public bool IsAngerFull(TSkill tSkill)
    {
        return Attribute.CheckIfMaxAnger(tSkill.ID, angerValue, tSkill.Lv);
    }

    public bool AIing
    {
        get { return PlayerRefGameObject.activeSelf && mManually.IsOff(); }
    }

    public int TargetPosNum
    {
        get { return moveQueue.Count; }
    }

    public string TargetPosName
    {
        get
        { 
            if (moveQueue.Count == 0)
                return "";
            return moveQueue.Peek().TacticalName;
        }
    }

    public Vector3 CurrentTargetPos
    {
        get { return mDebugSphere.transform.position; }
    }

    public bool UseGravity
    {
        set
        { 
            if (PlayerRigidbody)
                PlayerRigidbody.useGravity = value;
        }			
    }

    public TMoveData TargetPos
    {
        set
        {
            TacticalName = value.TacticalName;
            if (moveQueue.Count == 0)
                MoveTurn = 0;

            moveQueue.Enqueue(value);

            if (value.Target != Vector2.zero)
                GameRecord.PushMove(value.Target);
        }
    }

    [CanBeNull]
    public PlayerBehaviour DefPlayer
    {
//        get { return AI.enabled ? defencePlayer : null; }
        get { return defencePlayer; }
        set { defencePlayer = value; }
    }

    private void setMovePower(float value)
    {
        mMaxMovePower = value;
        mMovePower = value;
    }

    private int isTouchPalyer = 0;

    public void IsTouchPlayerForCheckLayer(int index)
    {
        isTouchPalyer += index;
    }

	public bool IsGameAttack {
		get {
			return (situation == EGameSituation.GamerAttack || situation == EGameSituation.NPCAttack);
		}
	}

    public bool IsShowSituation
    {
        get{
            if(situation <= EGameSituation.Opening)
                return true;
            else
                return false;
        }
    }

    public Vector2 GetStealPostion(Vector3 p1, Vector3 p2, EPlayerPostion index)
    {
        bool cover = false;
        Vector2 result = Vector2.zero;
        if (p1.x > 0)
            result.x = p1.x - (Math.Abs(p1.x - p2.x) / 3);
        else
            result.x = p1.x + (Math.Abs(p1.x - p2.x) / 3);

        if (GameController.Get.BallOwner && GameController.Get.BallOwner.DefPlayer && GameController.Get.BallOwner.Index != Index)
        {
            float angle = Math.Abs(MathUtils.FindAngle(GameController.Get.BallOwner.transform, GameController.Get.BallOwner.DefPlayer.transform.position));
            if (angle > 90)
            {
                p1 = GameController.Get.BallOwner.transform.position;
                if (p1.x > 0)
                    result.x = p1.x - (Math.Abs(p1.x - p2.x) / 3);
                else
                    result.x = p1.x + (Math.Abs(p1.x - p2.x) / 3);

                cover = true;
            }
        }

        if (index != Index && !cover)
        {
            switch (index)
            {
                case EPlayerPostion.C:
                    if (Index == EPlayerPostion.F)
                        result.x += 1.5f;
                    else
                        result.x -= 1.5f;
                    break;
                case EPlayerPostion.F:
                    if (Index == EPlayerPostion.C)
                        result.x += 1.5f;
                    else
                        result.x -= 1.5f;
                    break;
                case EPlayerPostion.G:
                    if (Index == EPlayerPostion.C)
                        result.x += 1.5f;
                    else
                        result.x -= 1.5f;
                    break;
            }
        }

        if (p2.z > 0)
            result.y = p1.z + (Math.Abs(p1.z - p2.z) / 3);
        else
            result.y = p1.z - (Math.Abs(p1.z - p2.z) / 3);
        return result;
    }
}
