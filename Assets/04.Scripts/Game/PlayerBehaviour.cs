using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using DG.Tweening;
using GameStruct;
using GameEnum;
using Chronos;
using G2;
using JetBrains.Annotations;

public delegate bool OnPlayerAction(PlayerBehaviour player);

public delegate void OnPlayerAction1(PlayerBehaviour player,bool isActive);

public delegate void OnPlayerAction2(PlayerBehaviour player,bool speedup);

public delegate bool OnPlayerAction3(PlayerBehaviour player,bool speedup);

public delegate bool OnPlayerAction4(PlayerBehaviour player,EPlayerState state);

public delegate void OnPlayerAction5(float max,float anger,int count);


public class PlayerBehaviour : MonoBehaviour
{
    public ETimerKind TimerKind;
    public OnPlayerAction1 OnShooting = null;
    public OnPlayerAction OnPass = null;
    public OnPlayerAction OnBlockJump = null;
    public OnPlayerAction OnBlocking = null;
    public OnPlayerAction OnBlockCatching = null;
    public OnPlayerAction OnDunkBasket = null;
    public OnPlayerAction OnDunkJump = null;
    public OnPlayerAction OnBlockMoment = null;
    public OnPlayerAction OnFakeShootBlockMoment = null;
    public OnPlayerAction OnFall = null;
    public OnPlayerAction OnPickUpBall = null;
    public OnPlayerAction OnGotSteal = null;
    public OnPlayerAction OnOnlyScore = null;
    public OnPlayerAction OnUI = null;
    public OnPlayerAction OnUICantUse = null;
    public OnPlayerAction5 OnUIAnger = null;
    public OnPlayerAction5 OnReviveAnger = null;
    public OnPlayerAction4 OnDoubleClickMoment = null;
    public OnPlayerAction3 OnUIJoystick = null;
    public bool IsJumpBallPlayer = false;
    public GameObject PlayerRefGameObject;
    public int ShowPos = -1;

    [Tooltip("Just for Debug.")]
    public string TacticalName = "";

    private const float MoveCheckValue = 1;
    private bool stop = false;
    private bool NeedResetFlag = false;
    public bool CanUseTipIn = false;
    private int MoveTurn = 0;
    private float moveStartTime = 0;
    //    private float ProactiveTime = 0;
    private float animationSpeed = 0;
    private float MoveMinSpeed = 0.5f;
    private float canDunkDis = 30f;
    private readonly Queue<TMoveData> moveQueue = new Queue<TMoveData>();
    private Vector3 translate;
    public Rigidbody PlayerRigidbody;
    private GameObject selectTexture;
    private GameObject DefPoint;
    private GameObject TopPoint;
    public GameObject CatchBallPoint;
    private GameObject FingerPoint;
    private GameObject blockTrigger;
    private GameObject dashSmoke;
    private BlockCatchTrigger blockCatchTrigger;
    public GameObject AIActiveHint = null;
    public GameObject DoubleClick = null;
    public GameObject DummyBall;
    public UISprite SpeedUpView = null;
    public Animator SpeedAnimator = null;
    private bool isSpeedStay = true;
    public UISprite AngerView = null;
    public GameObject AngryFull = null;
    public Material BodyMaterial;
    [HideInInspector]
    public GameObject BodyHeight;
    public TPlayerAttribute Attr = new TPlayerAttribute();
    public TPlayer Attribute;
    [HideInInspector]
    public TScoreRate ScoreRate;
    public TGamePlayerRecord GameRecord = new TGamePlayerRecord();
    public ETeamKind Team;

    public EPlayerPostion Index;
    public EPlayerPostion Postion;

    private readonly StatusTimer mManually = new StatusTimer();

    public EGameSituation situation = EGameSituation.None;
    public EPlayerState crtState = EPlayerState.Idle;
    public EAnimatorState crtAnimatorState = EAnimatorState.Idle;
    public Transform[] DefPointAy = new Transform[8];

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
    public bool isJoystick = false;
    [CanBeNull]
    public PlayerAI AI = null;
    private PlayerBehaviour defencePlayer = null;
    public float CloseDef = 0;
    public bool AutoFollow = false;
    public bool NeedShooting = false;
        
    //Block
    private bool isCanBlock = false;
    private bool isBlock = false;
    private Vector3 skillMoveTarget;
    private Vector3 skillFaceTarget;
    private bool isDunkBlock;

    //Rebound
    private Vector3 reboundMove;
    private bool isShootJumpActive = false;
    //For Active
    private bool isFakeShoot = false;

    //Push
    public bool IsPushCalculate = false;

    //Elbow
    public bool IsElbowCalculate = false;

    //Steal
    public bool IsStealCalculate = false;

    //Skill
    public SkillController PlayerSkillController;
    private ESkillKind skillKind;
    // For Shoot and Layup
    private bool isUsePass = false;
    private AnimationEvent animatorEvent;

    //Active
    private float angerValue = 0;

    //ShowWord
    public GameObject ShowWord;
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
    private float dis;
    private bool canSpeedup = true;
    private float SlowDownTime = 0;
    public float DribbleTime = 0;
	private float holdBallTime = 0;

    //SkillEvent
    private bool isSkillShow = false;

    //Camera
    private float yAxizOffset = 0;

    //Select
    public GameObject SelectMe;

    private bool IsPassAirMoment = false;

    //ReadyToNextState : 強制動作使用，為下一個loop動作作緩衝
    private bool ReadyToNextState = true;
    public AnimatorBehavior AnimatorControl;

    public void SetAnger(int value, GameObject target = null, GameObject parent = null)
    {
        int v = (int)(Mathf.Abs(value) / 5);
        if (v <= 0)
            v = 0;
        if (GameController.Get.Situation != EGameSituation.End && Attribute.ActiveSkills.Count > 0)
        {
            if (this == GameController.Get.Joysticker && value > 0)
            {
                if (target)
                    SkillDCExplosion.Get.BornDC(v, target, CameraMgr.Get.SkillDCTarget, parent);
            }
        }
        angerValue += value;
        if (angerValue > Attribute.MaxAnger)
        {
            angerValue = Attribute.MaxAnger;
        }
        
        if (angerValue < 0)
            angerValue = 0;
        
        if (Team == ETeamKind.Self && Index == 0)
        {
            if (OnUIAnger != null)
                OnUIAnger(Attribute.MaxAnger, angerValue, v);
            if (value > 0)
                GameRecord.AngerAdd += value;
        }
    }

    public void ReviveAnger(float value)
    {
        angerValue += value;
        if (angerValue > Attribute.MaxAnger)
        {
            angerValue = Attribute.MaxAnger;
        }
        if (OnReviveAnger != null)
            OnReviveAnger(Attribute.MaxAnger, angerValue, 0);
    }

    public void SetSlowDown(float Value)
    {
        if (SlowDownTime == 0)
        {
            SlowDownTime += Time.time + Value;
//            Attr.SpeedValue = GameData.BaseAttr[Attribute.AILevel].SpeedValue * GameConst.SlowDownValue;
        }
    }

    void OnDestroy()
    {
        if (AnimatorControl)
            Destroy(AnimatorControl);

        AnimatorControl = null;

        if (BodyMaterial)
            Destroy(BodyMaterial);

        BodyMaterial = null;
        Destroy(PlayerRefGameObject);
    }

    void Awake()
    {
        mManually.TimeUpListener += manuallyTimeUp;

        PlayerRefGameObject = gameObject;
        LayerMgr.Get.SetLayerAndTag(PlayerRefGameObject, ELayer.Player, ETag.Player);

//        PlayerSkillController = new SkillController();
        PlayerSkillController = gameObject.AddComponent<SkillController>();

        InitAnmator();
        PlayerRigidbody = PlayerRefGameObject.GetComponent<Rigidbody>();
        ScoreRate = new TScoreRate(1);
        animatorEvent = new UnityEngine.AnimationEvent();
        DashEffectEnable(false);
    }

    void Start()
    {
        TestGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        TestGameObject.name = gameObject.name + ".Target";
        if (TestGameObject.GetComponent<SphereCollider>())
            TestGameObject.GetComponent<SphereCollider>().enabled = false;
				
        TestGameObject.GetComponent<MeshRenderer>().enabled = GameStart.Get.IsDebugAnimation;
    }

    private void manuallyTimeUp()
    {
        RemoveMoveData();

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
        if (Attr.StaminaValue > 0)
            setMovePower(Attr.StaminaValue);

        initAttr();
    }

    private void initAttr()
    {
        if (GameData.BaseAttr.Length <= 0 || Attribute.AILevel < 0 || Attribute.AILevel >= GameData.BaseAttr.Length)
        {
            Debug.LogErrorFormat("initialize attributes fail, BaseAttr:{0}, AILevel:{1}.", GameData.BaseAttr.Length, Attribute.AILevel);
            return;
        }
		Attr.PointRate2 = GameFunction.GetAttributeFormula(EPlayerAttributeRate.Point2Rate, (Attribute.Point2 + GameData.BaseAttr[Attribute.AILevel].PointRate2));
		Attr.PointRate3 = GameFunction.GetAttributeFormula(EPlayerAttributeRate.Point3Rate, (Attribute.Point3 + GameData.BaseAttr[Attribute.AILevel].PointRate3));
		Attr.StealRate =  GameFunction.GetAttributeFormula(EPlayerAttributeRate.StealRate, (Attribute.Steal + GameData.BaseAttr[Attribute.AILevel].StealRate));
		Attr.DunkRate =   GameFunction.GetAttributeFormula(EPlayerAttributeRate.DunkRate, (Attribute.Dunk + GameData.BaseAttr[Attribute.AILevel].DunkRate));
		Attr.TipInRate =  GameFunction.GetAttributeFormula(EPlayerAttributeRate.TipInRate, (Attribute.Dunk + GameData.BaseAttr[Attribute.AILevel].TipInRate));
		Attr.AlleyOopRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.AlleyOopRate, (Attribute.Dunk + GameData.BaseAttr[Attribute.AILevel].AlleyOopRate));
		Attr.StrengthRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.StrengthRate, (Attribute.Strength + GameData.BaseAttr[Attribute.AILevel].StrengthRate));
		Attr.BlockPushRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.BlockPushRate, (Attribute.Strength + GameData.BaseAttr[Attribute.AILevel].BlockPushRate));
		Attr.ElbowingRate =  GameFunction.GetAttributeFormula(EPlayerAttributeRate.ElbowingRate, (Attribute.Strength + GameData.BaseAttr[Attribute.AILevel].ElbowingRate));
		Attr.ReboundRate =  GameFunction.GetAttributeFormula(EPlayerAttributeRate.ReboundRate, (Attribute.Rebound + GameData.BaseAttr[Attribute.AILevel].ReboundRate));
		Attr.BlockRate =    GameFunction.GetAttributeFormula(EPlayerAttributeRate.BlockRate, (Attribute.Block + GameData.BaseAttr[Attribute.AILevel].BlockRate));
		Attr.FaketBlockRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.FakeBlockrate, (Attribute.Block + GameData.BaseAttr[Attribute.AILevel].FaketBlockRate));
//        Attr.JumpBallRate = GameData.BaseAttr[Attribute.AILevel].JumpBallRate;//沒用到（已有技能）
		Attr.PushingRate =  GameFunction.GetAttributeFormula(EPlayerAttributeRate.PushingRate, (Attribute.Defence + GameData.BaseAttr[Attribute.AILevel].PushingRate));
		Attr.PassRate =     GameFunction.GetAttributeFormula(EPlayerAttributeRate.PassRate, (Attribute.Pass + GameData.BaseAttr[Attribute.AILevel].PassRate));
		Attr.AlleyOopPassRate = GameFunction.GetAttributeFormula(EPlayerAttributeRate.AlleyOopPassRate, (Attribute.Pass + GameData.BaseAttr[Attribute.AILevel].AlleyOopPassRate));
		Attr.ReboundHeadDistance = GameFunction.GetAttributeFormula(EPlayerAttributeRate.ReboundHeadDistance, (Attribute.Rebound + GameData.BaseAttr[Attribute.AILevel].ReboundHeadDistance));
		Attr.ReboundHandDistance = GameConst.ReboundHandDistanceMin + GameFunction.GetAttributeFormula(EPlayerAttributeRate.ReboundHandDistance, (Attribute.Rebound + GameData.BaseAttr[Attribute.AILevel].ReboundHandDistance));
		Attr.BlockDistance =  GameFunction.GetAttributeFormula(EPlayerAttributeRate.BlockDistance, (Attribute.Block + GameData.BaseAttr[Attribute.AILevel].BlockDistance));
		Attr.DefDistance =  GameFunction.GetAttributeFormula(EPlayerAttributeRate.DefDistance, (Attribute.Defence + GameData.BaseAttr[Attribute.AILevel].DefDistance));
		Attr.SpeedValue =  GameConst.SpeedValueMin + GameFunction.GetAttributeFormula(EPlayerAttributeRate.SpeedValue, (Attribute.Speed + GameData.BaseAttr[Attribute.AILevel].SpeedValue));
		Attr.StaminaValue = GameFunction.GetAttributeFormula(EPlayerAttributeRate.StaminaValue, (Attribute.Stamina + GameData.BaseAttr[Attribute.AILevel].StaminaValue));

        Attr.StealDistance = 	GameConst.StealPushDistance + GameFunction.GetAttributeFormula(EPlayerAttributeRate.StealDistance, Attribute.Steal);
        Attr.StealExtraAngle =  GameConst.StealFanAngle + GameFunction.GetAttributeFormula(EPlayerAttributeRate.StealExtraAngle, Attribute.Steal);
        Attr.PushDistance = 	GameConst.StealPushDistance + GameFunction.GetAttributeFormula(EPlayerAttributeRate.PushDistance, Attribute.Defence);
        Attr.PushExtraAngle = 	GameConst.PushFanAngle + GameFunction.GetAttributeFormula(EPlayerAttributeRate.PushExtraAngle, Attribute.Defence);
        Attr.ElbowDistance = 	GameConst.StealPushDistance + GameFunction.GetAttributeFormula(EPlayerAttributeRate.ElbowDistance, Attribute.Strength);
        Attr.ElbowExtraAngle =  GameConst.ElbowFanAngle + GameFunction.GetAttributeFormula(EPlayerAttributeRate.ElbowExtraAngle, Attribute.Strength);
		
        Attr.AutoFollowTime =  GameData.BaseAttr[Attribute.AILevel].AutoFollowTime;
            
        DefPoint.transform.localScale = new Vector3(Attr.DefDistance, Attr.DefDistance, Attr.DefDistance);
        TopPoint.transform.localScale = new Vector3(4 + Attr.ReboundHeadDistance, TopPoint.transform.localScale.y, 4 + Attr.ReboundHeadDistance);
        FingerPoint.transform.localScale = new Vector3(Attr.ReboundHandDistance, Attr.ReboundHandDistance, Attr.ReboundHandDistance);
        blockTrigger.transform.localScale = new Vector3(blockTrigger.transform.localScale.x, 3.2f + Attr.BlockDistance, blockTrigger.transform.localScale.z);
      
    }

    private void initSkill()
    {
        isSkillShow = false;
        PlayerSkillController.initSkillController(Attribute, this, AnimatorControl.Controler);

        if (Team == ETeamKind.Npc)
            PlayerSkillController.HidePlayerName();
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

    public void InitTrigger(GameObject defPoint)
    {
        SkinnedMeshRenderer render = PlayerRefGameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (render && render.material)
            BodyMaterial = render.material;
				
        DummyBall = null;
        Transform find = transform.Find("DummyBall");

        if (find)
            DummyBall = find.gameObject;
				
        if (DummyBall != null)
        {
            blockCatchTrigger = DummyBall.GetComponent<BlockCatchTrigger>();
            if (blockCatchTrigger == null)
                blockCatchTrigger = DummyBall.AddComponent<BlockCatchTrigger>();

            blockCatchTrigger.SetEnable(false);	
        } 

        GameObject obj = null;
        switch (Attribute.BodyType)
        {
            case 0:
                obj = Resources.Load("Prefab/Player/BodyTrigger0") as GameObject;
                break;
            case 1:
                obj = Resources.Load("Prefab/Player/BodyTrigger1") as GameObject;
                break;
            case 2:
                obj = Resources.Load("Prefab/Player/BodyTrigger2") as GameObject;
                break;
            default:
                obj = Resources.Load("Prefab/Player/BodyTrigger2") as GameObject;
                break;
        }

        if (BodyHeight == null)
            BodyHeight = new GameObject();
        BodyHeight.name = "BodyHeight";
        BodyHeight.transform.parent = transform;
        BodyHeight.transform.localPosition = new Vector3(0, PlayerRefGameObject.transform.GetComponent<CapsuleCollider>().height + 0.2f, 0);

        if (obj)
        {
            GameObject obj2 = Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
            blockTrigger = obj2.transform.Find("Block").gameObject;
            ShowWord = obj2.transform.Find("ShowWord").gameObject;
            
            obj2.name = "BodyTrigger";
            PlayerTrigger[] objs = obj2.GetComponentsInChildren<PlayerTrigger>();
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i++)
                    objs[i].Player = this;
            }
            
            DefTrigger defTrigger = obj2.GetComponentInChildren<DefTrigger>(); 
            if (defTrigger != null)
                defTrigger.Player = this;
            
            DefPoint = obj2.transform.Find("DefRange").gameObject;          
            TopPoint = obj2.transform.Find("TriggerTop").gameObject; 
            CatchBallPoint = obj2.transform.Find("CatchBall").gameObject; 
            obj2.transform.parent = transform;
            obj2.transform.transform.localPosition = Vector3.zero;
            obj2.transform.transform.localScale = Vector3.one;

            Transform t = obj2.transform.Find("TriggerFinger");
            if (t)
            {
                FingerPoint = t.gameObject;
				t.name = Team.GetHashCode().ToString() + Index.GetHashCode().ToString() + "TriggerFinger";
                t.parent = transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Bip01 R Finger2/Bip01 R Finger21/");
                t.localPosition = Vector3.zero;
                t.localScale = Vector3.one;
            }
        }
        
        if (defPoint != null)
        {
            GameObject DefPointCopy = Instantiate(defPoint, Vector3.zero, Quaternion.identity) as GameObject;
            DefPointCopy.transform.parent = PlayerRefGameObject.transform;
            DefPointCopy.name = "DefPoint";
            DefPointCopy.transform.localScale = Vector3.one;
            DefPointCopy.transform.localPosition = Vector3.zero;

            DefPointAy[EDefPointKind.Front.GetHashCode()] = DefPointCopy.transform.Find("Front");
            DefPointAy[EDefPointKind.Back.GetHashCode()] = DefPointCopy.transform.Find("Back");
            DefPointAy[EDefPointKind.Right.GetHashCode()] = DefPointCopy.transform.Find("Right");
            DefPointAy[EDefPointKind.Left.GetHashCode()] = DefPointCopy.transform.Find("Left");
            DefPointAy[EDefPointKind.FrontSteal.GetHashCode()] = DefPointCopy.transform.Find("FrontSteal");
            DefPointAy[EDefPointKind.BackSteal.GetHashCode()] = DefPointCopy.transform.Find("BackSteal");
            DefPointAy[EDefPointKind.RightSteal.GetHashCode()] = DefPointCopy.transform.Find("RightSteal");
            DefPointAy[EDefPointKind.LeftSteal.GetHashCode()] = DefPointCopy.transform.Find("LeftSteal");
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
        if (this == GameController.Get.Joysticker)
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
        if (GameStart.Get.IsDebugAnimation)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gameObject.transform.position, TestGameObject.transform.position);
        }
    }

    void FixedUpdate()
    {
        if (GameController.Get.IsShowSituation || !GameController.Get.IsStart)
        {
            return;
        }

        timeScale = TimerMgr.Get.CrtTime;

        if (AnimatorControl.TimeScaleTime != timeScale)
            AnimatorControl.TimeScaleTime = timeScale;

//		if (IsAllShoot || IsRebound || IsSteal || IsFall || IsBlock || IsBuff || IsPush) {
//
//				AnimatorControl.TimeScaleTime = timeScale;
//		} else {
//		}			
				
        DebugTool();
        
        CantMoveTimer.Update(Time.deltaTime);
        Invincible.Update(Time.deltaTime);
        StealCD.Update(Time.deltaTime);
        PushCD.Update(Time.deltaTime);
        ElbowCD.Update(Time.deltaTime);
        mManually.Update(Time.deltaTime);

        if (IsPushCalculate)
            GameController.Get.PushCalculate(this, Attr.PushDistance, Attr.PushExtraAngle);

        if (IsElbowCalculate)
            GameController.Get.PushCalculate(this, Attr.ElbowDistance, Attr.ElbowExtraAngle);

        if (IsStealCalculate)
            GameController.Get.OnStealMoment(this, Attr.StealDistance, Attr.StealExtraAngle);

//        if(CoolDownElbow > 0 && Time.time >= CoolDownElbow)
//            CoolDownElbow = 0;

        if (SlowDownTime > 0 && Time.time >= SlowDownTime)
        {
            SlowDownTime = 0;
//            Attr.SpeedValue = GameData.BaseAttr[Attribute.AILevel].SpeedValue + (Attribute.Speed * 0.005f);
        }

//        if(aiTime == 0)
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

        if (situation == EGameSituation.GamerAttack || situation == EGameSituation.NPCAttack)
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

			if (IsIdle && IsBallOwner) {
				holdBallTime += Time.deltaTime;
				if (holdBallTime >= 2) {
					AniState(EPlayerState.Dribble0);
					holdBallTime = 0;
					Debug.Log("Exception idle become dribble");
				}
			}
        }
        
        if (Time.time >= mMovePowerTime)
        {
            mMovePowerTime = Time.time + GameConst.MovePowerCheckTime;
            if (isSpeedup)
            {
                if (mMovePower > 0 && (GameController.Get.Situation == EGameSituation.GamerAttack || GameController.Get.Situation == EGameSituation.NPCAttack))
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
//      aiTime += aniTime;
        mManually.StartCounting(mManually.RemainTime + aniTime);
    }

    //    public float AIRemainTime
    //    {
    //        get
    //        {
    //            if(aiTime <= 0)
    //                return 0;
    //            return aiTime - Time.time;
    //        }
    //    }

    public void SetAITime(float time)
    {
//        aiTime = Time.time + time;
        mManually.StartCounting(time);
        StartCoroutine(GetCurrentClipLength());

        if (AIActiveHint)
            AIActiveHint.SetActive(false);
    }

    /// <summary>
    /// 進入手控狀態.
    /// </summary>
    public void SetManually()
    {
        if (Team == ETeamKind.Self && GameController.Get.Joysticker == this)
        {
            if (situation == EGameSituation.GamerAttack || situation == EGameSituation.NPCAttack ||
                GameStart.Get.TestMode != EGameTest.None)
            {
                isJoystick = true;
//              aiTime = Time.time + GameData.Setting.AIChangeTime;
                mManually.StartCounting(GameConst.AITime[GameData.Setting.AIChangeTimeLv]);
                StartCoroutine(GetCurrentClipLength());

                if (AIActiveHint)
                    AIActiveHint.SetActive(false);
            }
            else
            {
//              aiTime = 0;
                mManually.Clear();
                if (AIActiveHint)
                    AIActiveHint.SetActive(true);
            }
        }
    }

    public void SetToAI()
    {
//        aiTime = 0;
        mManually.Clear();
        if (AIActiveHint)
            AIActiveHint.SetActive(true);
    }

    public float timeScale = 1;

    public  bool InReboundDistance
    {
        get
        {
            return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), 
                new Vector2(CourtMgr.Get.RealBallObj.transform.position.x, CourtMgr.Get.RealBallObj.transform.position.z)) <= 6;
        }
    }

    private float proofingTime = 2f;

    public void DebugTool()
    {	
        if (GameController.Get.IsStart && TimerMgr.Get.CrtTime > 0 && 
            (GameController.Get.Situation == EGameSituation.GamerAttack || 
                GameController.Get.Situation == EGameSituation.NPCAttack))
        {
            if (crtState == EPlayerState.Idle && IsBallOwner)
            {
                proofingTime -= Time.deltaTime;
                if (proofingTime <= 0)
                {
                    if (GameStart.Get.IsDebugAnimation)
                        Debug.LogError("proofing1");
                    
                    if (IsBallOwner)
                        AniState(EPlayerState.Dribble0);
                    else
                        AniState(EPlayerState.Fall0);
    									
                    proofingTime = 2f;
                }
            }
            else if (IsElbow && crtAnimatorState == EAnimatorState.Idle)
            {
                proofingTime -= Time.deltaTime;
                if (proofingTime <= 0)
                {
                    if (GameStart.Get.IsDebugAnimation)
                        Debug.LogError("proofing2");
                    
                    if (IsBallOwner)
                        AniState(EPlayerState.HoldBall);
                    else
                        AniState(EPlayerState.Fall0);
                    
                    proofingTime = 2f;
                }
            }
            else
                proofingTime = 2f; 
        }
        else
        {
            proofingTime = 2f;
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
            if (situation == EGameSituation.GamerAttack || situation == EGameSituation.NPCAttack ||
                GameStart.Get.TestMode != EGameTest.None)
            {
                EPlayerState ps = EPlayerState.Run0;
                if (IsBallOwner)
                    ps = EPlayerState.Dribble1;

                int moveKind = 0;
                float calculateSpeed = 1;
                RemoveMoveData();

                #if UNITY_EDITOR
                if (IsFall && GameStart.Get.IsDebugAnimation)
                {
                    LogMgr.Get.LogError("CanMove : " + CanMove);
                    LogMgr.Get.LogError("stop : " + stop);
                    LogMgr.Get.LogError("HoldBallCanMove : " + HoldBallCanMove);
                }
                #endif

                if (!(GameController.Get.CoolDownCrossover == 0 && !IsDefence &&
                    PlayerSkillController.DoPassiveSkill(ESkillSituation.MoveDodge)))
                {
                    isMoving = true;
                    if (!isJoystick)
                        moveStartTime = Time.time + GameConst.DefMoveTime;

                    SetManually();
                    animationSpeed = Vector2.Distance(new Vector2(v.x, 0), new Vector2(0, v.y));
                    if (!IsPass)
                    {
                        float angle = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
                        float a = 90 + yAxizOffset;
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

                    if (GameStart.Get.TestMode == EGameTest.Skill || GameStart.Get.TestMode == EGameTest.PassiveSkill)
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
            if (crtState == EPlayerState.Elbow0)
                ps = EPlayerState.Elbow0;
            else if (crtState == EPlayerState.HoldBall)
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

            if (crtState != ps)
                AniState(ps);

            if (crtState == EPlayerState.Dribble0)
            {
                if (situation == EGameSituation.GamerAttack)
                    RotateTo(CourtMgr.Get.ShootPoint[0].transform.position.x, CourtMgr.Get.ShootPoint[0].transform.position.z);
                else if (situation == EGameSituation.NPCAttack)
                    RotateTo(CourtMgr.Get.RealBallObj.transform.position.x, CourtMgr.Get.RealBallObj.transform.position.z);
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
//            if (IsDribble)
//                Debug.Log("catch you");
        }

        return resultBool;
    }

    private GameObject TestGameObject;


    private void AttackerMove(bool first, TMoveData data, bool isshort)
    {
        if (isshort)
        {
            // 移動距離很短 or 不移動, 球員又是在進攻狀態.
            if (!IsBallOwner)
                AniState(EPlayerState.Idle);
            else if (situation == EGameSituation.GamerInbounds || situation == EGameSituation.NPCInbounds)
                AniState(EPlayerState.Dribble0);

            if (first || GameStart.Get.TestMode == EGameTest.Edit)
						//                        WaitMoveTime = 0;
						CantMoveTimer.Clear();
            else if ((situation == EGameSituation.GamerAttack || situation == EGameSituation.NPCAttack) &&
                     GameController.Get.BallOwner && UnityEngine.Random.Range(0, 3) == 0)
            {
                // 目前猜測這段程式碼的功能是近距離防守時, 避免防守者不斷的轉向.
                // 因為當初寫這段程式碼的時候, AI 做決策其實是 1 秒 30 次以上.
                // 所以當 AI 做防守邏輯的時候, 會 1 秒下 30 的命令, 命令跑到某位球員的旁邊.
                // 就會造成防守球員會一直的轉向.(因為距離很近的時候, 對方移動一點距離, 防守者就必須轉向很多度
                // , 才可以正確的面相對方)
                dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint[Team.GetHashCode()].transform.position);
                if (dis <= 8)
								//                          WaitMoveTime = Time.time + UnityEngine.Random.Range(0.3f, 1.1f);
								CantMoveTimer.StartCounting(UnityEngine.Random.Range(0.3f, 1.1f));
                else
								//                          WaitMoveTime = Time.time + UnityEngine.Random.Range(0.3f, 2.1f);
								CantMoveTimer.StartCounting(UnityEngine.Random.Range(0.3f, 2.1f));
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
                    if (situation == EGameSituation.GamerAttack || situation == EGameSituation.NPCAttack)
                    {
                        if (GameController.Get.Pass(this, false, false, true))
                            NeedShooting = data.Shooting;
                    }
                }
            }	
        }
        else
        {
            // 進攻移動.                 
            RotateTo(MoveTarget.x, MoveTarget.y); 
			MoveTargetPos( new Vector3(MoveTarget.x, 0, MoveTarget.y));
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
                    //                            transform.Translate(transform.forward * Time.deltaTime * GameConst.AttackSpeedNormal * Attr.SpeedValue * timeScale);
                    //                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedNormal * Attr.SpeedValue * timeScale);
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
                dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint[data.DefPlayer.Team.GetHashCode()].transform.position);

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
                dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint[data.DefPlayer.Team.GetHashCode()].transform.position);

                if (dis <= GameConst.Point3Distance + 4 || Vector3.Distance(transform.position, data.LookTarget.position) <= 1.5f)
                {
					MoveTargetPos( new Vector3(data.LookTarget.position.x, 0, data.LookTarget.position.z));
                    RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                }
                else
                {
					MoveTargetPos( new Vector3(MoveTarget.x, 0, MoveTarget.y));
                    RotateTo(MoveTarget.x, MoveTarget.y);
                }

                if (MathUtils.FindAngle(PlayerRefGameObject.transform, new Vector3(MoveTarget.x, 0, MoveTarget.y)) > 90)
                    AniState(EPlayerState.Defence1);
                else
                    AniState(EPlayerState.RunningDefence);
            }
            else
            {
				MoveTargetPos(new Vector3(MoveTarget.x, 0, MoveTarget.y));
                RotateTo(MoveTarget.x, MoveTarget.y);
                AniState(EPlayerState.Run0);
            }

            isMoving = true;
            if (mMovePower > 0 && canSpeedup && this != GameController.Get.Joysticker && !IsTee)
            {
                setSpeed(1, 0);
                transform.position = Vector3.MoveTowards(transform.position, 
                    new Vector3(MoveTarget.x, 0, MoveTarget.y), 
                    Time.deltaTime * GameConst.DefSpeedup * Attr.SpeedValue * timeScale);
                isSpeedup = true;

				MoveTargetPos(new Vector3(MoveTarget.x, 0, MoveTarget.y));
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, 
                    new Vector3(MoveTarget.x, 0, MoveTarget.y), 
                    Time.deltaTime * GameConst.DefSpeedNormal * Attr.SpeedValue * timeScale);
                isSpeedup = false;
				MoveTargetPos (new Vector3 (MoveTarget.x, 0, MoveTarget.y));
            }
        }
    }

	private void MoveTargetPos(Vector3 pos)
	{
		TestGameObject.transform.position = pos;
	}

    private void moveTo(TMoveData data, bool first = false)
    {
        if ((CanMove || (AIing && HoldBallCanMove)) && CantMoveTimer.IsOff() && GameController.Get.Situation != EGameSituation.End && !IsTimePause)
        {
            bool doMove = GetMoveTarget(ref data, out MoveTarget);
            float temp = Vector2.Distance(new Vector2(PlayerRefGameObject.transform.position.x, PlayerRefGameObject.transform.position.z), MoveTarget);
            setSpeed(0.3f, 0);

            if (temp <= MoveCheckValue || !doMove)
            {
                // 移動距離太短 or 不移動.
                MoveTurn = 0;
                isMoving = false;

                if (IsDefence)
                {
                    DefenderMove(data, doMove, true);                       
                }
                else
                {
                    AttackerMove(first, data, true);
                }

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
//        if (Invincible == 0)
//            Invincible = Time.time + time;
//        else
//            Invincible += time;
        Invincible.StartCounting(time);
    }

    private void setSpeed(float value, int dir = -2)
    {
        AnimatorControl.SetSpeed(value);
    }

    public bool CheckAnimatorSate(EPlayerState state)
    {
        if (crtState == state)
            return true;
        else
            return false;
    }

    public void ResetFlag(bool clearMove = true)
    {
        if (GameController.Get.IsShowSituation)
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
            if (clearMove && AI.enabled)
            {
                RemoveMoveData();
            }

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
        isCanBlock = false;
        isShootJumpActive = false;
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
                if (!IsShow && !IsDunk && !IsAlleyoopState && IsBallOwner && !IsPass && !IsPickBall && !IsAllShoot && (crtState == EPlayerState.HoldBall || IsDribble))
                {
                    return true;
                }
                break;

            case EPlayerState.Pass4:
                if (IsBallOwner && !IsLayup && !IsDunk && !IsAlleyoopState && IsShoot && !GameController.Get.Shooter && IsPassAirMoment && !IsPass)
                    return true;
                break;
            case EPlayerState.BlockCatch:
                if (IsBlock && crtState != EPlayerState.BlockCatch)
                    return true;
                break;

            case EPlayerState.FakeShoot:
                if (IsBallOwner && (crtState == EPlayerState.Idle || crtState == EPlayerState.HoldBall || IsDribble))
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
                if (IsBallOwner && !IsPickBall && !IsIntercept && !IsAllShoot && (crtState == EPlayerState.HoldBall || IsDribble))
                    return true;
                break;

            case EPlayerState.Layup0:
            case EPlayerState.Layup1:
            case EPlayerState.Layup2:
            case EPlayerState.Layup3:
                if (IsBallOwner && !IsPickBall && !IsIntercept && !IsAllShoot && (crtState == EPlayerState.HoldBall || IsDribble))
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
                if (IsBallOwner && !IsIntercept && !IsPickBall && !IsAllShoot && (crtState == EPlayerState.HoldBall || IsDribble))
                if (Vector3.Distance(CourtMgr.Get.ShootPoint[Team.GetHashCode()].transform.position, PlayerRefGameObject.transform.position) < canDunkDis)
                    return true;
                break;

            case EPlayerState.Alleyoop:
                if (crtState != EPlayerState.Alleyoop && !GameController.Get.CheckOthersUseSkill(TimerKind.GetHashCode()) && !IsBallOwner && (GameStart.Get.TestMode == EGameTest.Alleyoop || situation.GetHashCode() == (Team.GetHashCode() + 3)))
                    return true;

                break;
            case EPlayerState.HoldBall:
                if (IsBallOwner && !IsPass && !IsAllShoot && crtState != EPlayerState.GotSteal)
                    return true;
                break;

            case EPlayerState.Rebound0:
            case EPlayerState.Rebound20:
                if (CanMove && !IsRebound)
                    return true;
                break;

            case EPlayerState.JumpBall:
                if (CanMove && crtState != EPlayerState.JumpBall)
                    return true;
                break;

            case EPlayerState.TipIn:
                if (IsRebound && CanUseTipIn && crtState != EPlayerState.TipIn)
                    return true;

                break;
           
            case EPlayerState.Pick0:
            case EPlayerState.Pick1:
            case EPlayerState.Pick2:
                if (CanMove && !IsBallOwner && (crtState == EPlayerState.Idle || IsRun || crtState == EPlayerState.Defence1 ||
                    crtState == EPlayerState.Defence0 || crtState == EPlayerState.RunningDefence))
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
                if (!IsTee && !IsBallOwner && !IsSteal && (crtState == EPlayerState.Idle || IsSteal || IsRun || crtState == EPlayerState.Defence1 ||
                    crtState == EPlayerState.Defence0 || crtState == EPlayerState.RunningDefence))
                    return true;
                break;

            case EPlayerState.Block0:
            case EPlayerState.Block1:
            case EPlayerState.Block2:
            case EPlayerState.Block20:
                if (!IsTee && !IsBlock && CanMove && !IsBallOwner && (crtState == EPlayerState.Idle || IsRun || crtState == EPlayerState.Defence1 ||
                    crtState == EPlayerState.Defence0 || crtState == EPlayerState.RunningDefence || IsDunk))
                    return true;
                break;

            case EPlayerState.Elbow0:
            case EPlayerState.Elbow1:
            case EPlayerState.Elbow2:
            case EPlayerState.Elbow20:
            case EPlayerState.Elbow21:
                if (!IsTee && !IsElbow && IsBallOwner && (IsDribble || crtState == EPlayerState.HoldBall))
                    return true;
                break;

            case EPlayerState.Fall0:
            case EPlayerState.Fall1:
            case EPlayerState.Fall2:
            case EPlayerState.KnockDown0:
            case EPlayerState.KnockDown1:

                if (!IsTee && !IsFall && !IsUseActiveSkill)
                    return true;
                break;

            case EPlayerState.GotSteal:
                if (!IsTee && !IsAllShoot && crtState != state && crtState != EPlayerState.Elbow0 &&
                    (IsDribble ||
                    crtState == EPlayerState.FakeShoot ||
                    crtState == EPlayerState.HoldBall ||
                    crtState == EPlayerState.Idle ||
                    crtState == EPlayerState.Defence0 ||
                    crtState == EPlayerState.Defence1 ||
                    crtState == EPlayerState.RunningDefence))
                    return true;
                break;

            case EPlayerState.Dribble0:
            case EPlayerState.Dribble1:
            case EPlayerState.Dribble2:
            case EPlayerState.Dribble3:
                if (ReadyToNextState && IsBallOwner && !IsPickBall && !IsPass && !IsAllShoot && !IsElbow && !IsFall)
                if ((!CanMove && IsFirstDribble) || (CanMove && crtState != state) || (crtState == EPlayerState.MoveDodge0 || crtState == EPlayerState.MoveDodge1))
                {
                    return true;
                }
                break;
            
            case EPlayerState.Run0:   
            case EPlayerState.Run1:   
            case EPlayerState.Run2:   
            case EPlayerState.RunningDefence:
            case EPlayerState.Defence0:
            case EPlayerState.Defence1:
                if (ReadyToNextState && crtState != state && !IsAllShoot && !IsFall && !IsJump)
                    return true;
                break;
            case EPlayerState.MoveDodge0:
            case EPlayerState.MoveDodge1:
                if (crtState != state && !IsPass && !IsAllShoot && !IsBlock && !IsFall)
                    return true;
                break;
            case EPlayerState.CatchFlat:
            case EPlayerState.CatchFloor:
            case EPlayerState.CatchParabola:
            case EPlayerState.Intercept0:
            case EPlayerState.Intercept1:
                if (CanMove && !IsBallOwner && !IsAllShoot && situation != EGameSituation.GamerPickBall && situation != EGameSituation.NPCPickBall && situation != EGameSituation.GamerInbounds && situation != EGameSituation.NPCInbounds)
                    return true;
                break;

            case EPlayerState.Buff20:
            case EPlayerState.Buff21:
                if (!IsBuff)
                    return true;
                break;
            case EPlayerState.Idle:
                if (ReadyToNextState)
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
            return (situation == EGameSituation.SpecialAction || situation == EGameSituation.GamerInbounds || situation == EGameSituation.GamerPickBall || situation == EGameSituation.NPCInbounds || situation == EGameSituation.NPCPickBall);
        }
    }

    public bool IsKinematic
    {
        get{ return PlayerRigidbody.isKinematic; }
        set
        {
            PlayerRigidbody.isKinematic = value;
//            Timer.rigidbody.isKinematic = value;
        }
    }

    public bool AniState(EPlayerState state, Vector3 lookAtPoint)
    {
//        Debug.Log("state : " + state.ToString() + ". lookAtPoint : " + lookAtPoint);
        if (AniState(state))
        {
            RotateTo(lookAtPoint.x, lookAtPoint.z);
            if (GameStart.Get.TestMode == EGameTest.Pass)
                LogMgr.Get.Log("name:" + PlayerRefGameObject.name + "Rotate");

            return true;
        }
        else
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
        if (GameController.Get.BallOwner == null)
        {
            skillMoveTarget = CourtMgr.Get.RealBallObj.transform.position;
        }
        else
        {
            skillMoveTarget = GameController.Get.BallOwner.FindNearBlockPoint(PlayerRefGameObject.transform.position);
        }

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
//        StartSkillCamera(stateNo);
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
       
        CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.Dunk0, this);

        if (stateNo == 0 || stateNo == 1 || stateNo == 3 || stateNo == 5 || stateNo == 7)
        {
            GameController.Get.BallState = EBallState.CanDunkBlock;
        }
            

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
        CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.Dribble0, this);
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
            CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.KnockDown0);
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
//                    RotateTo(CourtMgr.Get.GetHoodPosition(Team).x, CourtMgr.Get.GetHoodPosition(Team).z);
                    isShootJumpActive = true;
                    break;
            }
			
            //Debug.LogError("Shoot show camera: " + stateNo);
            CourtMgr.Get.RealBallCompoment.SetBallState(EPlayerState.Shoot0, this);
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

    public bool AniState(EPlayerState state)
    {
        if (!CanUseState(state))
            return false;

        ReadyToNextState = !AnimatorMgr.Get.IsForciblyStates(state);
        TAnimatorItem nextState = AnimatorMgr.Get.GetAnimatorStateType(state);

//		if(nextState.Type != EAnimatorState.End && GameController.Get.Situation == EGameSituation.End)
//			return false;

        bool Result = false;
//        int stateNo = 0;

        PlayerRigidbody.mass = 0.1f;
        UseGravity = true;
        IsKinematic = false;
        if (!isUsePass)
            isCanCatchBall = true;

        if (LayerMgr.Get.CheckLayer(PlayerRefGameObject, ELayer.Shooter))
            LayerMgr.Get.SetLayer(PlayerRefGameObject, ELayer.Player);

        if (GameStart.Get.IsDebugAnimation)
            Debug.LogWarning(PlayerRefGameObject.name + ", CrtState : " + crtState + ", NextState : " + state + ", Situation : " + GameController.Get.Situation + ", Time : " + Time.time);

        DashEffectEnable(false);

        
        switch (nextState.Type)
        {
            case EAnimatorState.Block:  
                BlockStateHandle(nextState.StateNo);
                Result = true;
                break;
            case EAnimatorState.BlockCatch: 
                BlockCatchStateHandle(nextState.StateNo);
                Result = true;
                break;
            case EAnimatorState.Buff:   
                BuffStateHandle(nextState.StateNo);
                showActiveEffect(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.Catch:
                CatchStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.Defence:
                DefenceStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.Dunk:
                DunkStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.Alleyoop:
                AlleyoopStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.Dribble:
                DribbleStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.End:
                AnimatorControl.Play(EAnimatorState.End, nextState.StateNo, Team.GetHashCode());
                Result = true;
                break;
            case EAnimatorState.Elbow:
                ElbowStateHandle(nextState.StateNo);
                Result = true;
                break;
            case EAnimatorState.FakeShoot:
                FakeShootStateHandle(nextState.StateNo);
                Result = true;
                break;
            case EAnimatorState.KnockDown:
                KnockDownStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.Fall:
                FallStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.HoldBall:
                PlayerRigidbody.mass = 5;
                AnimatorControl.Play(EAnimatorState.HoldBall, nextState.StateNo, Team.GetHashCode()); 
                isCanCatchBall = false;
                Result = true;
                break;
            case EAnimatorState.Idle:
                PlayerRigidbody.mass = 5;
                setSpeed(0, -1);
                AnimatorControl.Play(EAnimatorState.Idle, nextState.StateNo, Team.GetHashCode()); 
                isMoving = false;
                Result = true;
                isCanCatchBall = true;
                break;
            case EAnimatorState.Intercept:
                AnimatorControl.Play(EAnimatorState.Intercept, nextState.StateNo, Team.GetHashCode());
                Result = true;
                break;
            case EAnimatorState.MoveDodge:
                AnimatorControl.Play(EAnimatorState.MoveDodge, nextState.StateNo, Team.GetHashCode());
                OnUICantUse(this);
                if (moveQueue.Count > 0)
                    moveQueue.Dequeue();
                Result = true;
                break;
            case EAnimatorState.Pass:
                isCanCatchBall = false;
                AnimatorControl.Play(EAnimatorState.Pass, nextState.StateNo, Team.GetHashCode());
                if (nextState.StateNo == 5 || nextState.StateNo == 6 || nextState.StateNo == 7 || nextState.StateNo == 8 || nextState.StateNo == 9)
                    isUsePass = true;
                PlayerRigidbody.mass = 5;
                GameRecord.Pass++;
                Result = true;
                break;
            case EAnimatorState.Push:
                if (nextState.StateNo == 20)
                    GameRecord.Push++;
                AnimatorControl.InitPushCurve(nextState.StateNo);
                StartSkillCamera(nextState.StateNo);
                AnimatorControl.Play(EAnimatorState.Push, nextState.StateNo, Team.GetHashCode());
                GameRecord.PushLaunch++;
                Result = true;
                AudioMgr.Get.PlaySound(SoundType.SD_Punch);
                break;
            case EAnimatorState.Pick:
                if (nextState.StateNo == 2)
                {
                    AnimatorControl.InitPickCurve(nextState.StateNo);
                    GameRecord.SaveBallLaunch++;
                }
                AnimatorControl.Play(EAnimatorState.Pick, nextState.StateNo, Team.GetHashCode());
                Result = true;
                break;
            case EAnimatorState.Run:
                if (!isJoystick)
                    setSpeed(1, 1); 
                if (nextState.StateNo == 1)
                {
                    DashEffectEnable(true);
                }
                AnimatorControl.Play(EAnimatorState.Run, nextState.StateNo, Team.GetHashCode());
                Result = true;
                break;
            case EAnimatorState.Steal:
                if (nextState.StateNo == 20)
                {
                    GameRecord.Steal++;
                    AnimatorControl.InitStealCurve(nextState.StateNo);
                }
                StartSkillCamera(nextState.StateNo);
                PlayerRigidbody.mass = 5;
                AnimatorControl.Play(EAnimatorState.Steal, nextState.StateNo, Team.GetHashCode());
                isCanCatchBall = false;
                GameRecord.StealLaunch++;
                AudioMgr.Get.PlaySound(SoundType.SD_Steal);
                Result = true;
                break;
            case EAnimatorState.GotSteal:
                AnimatorControl.Play(EAnimatorState.GotSteal, nextState.StateNo, Team.GetHashCode());
                isCanCatchBall = false;
                Result = true;
                break;
            case EAnimatorState.Shoot:
                ShootStateHandle(nextState.StateNo);
                Result = true;
                break;
            case EAnimatorState.Show:
                AnimatorControl.Play(EAnimatorState.Show, nextState.StateNo, Team.GetHashCode());
                Result = true;
                break;

            case EAnimatorState.Layup:
                LayupStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.Rebound:
                if (nextState.StateNo == 20)
                    GameRecord.Rebound++;

                UseGravity = false;
                IsKinematic = true;
            
                StartSkillCamera(nextState.StateNo);
                skillMoveTarget = CourtMgr.Get.RealBallObj.transform.position;
                if (InReboundDistance)
                {
                    reboundMove = CourtMgr.Get.RealBallObj.transform.position - transform.position;
                    reboundMove += CourtMgr.Get.RealBallCompoment.MoveVelocity * 0.3f;
                }
                else
                    reboundMove = Vector3.zero;
         
                SetShooterLayer();
                AnimatorControl.Play(EAnimatorState.Rebound, nextState.StateNo, Team.GetHashCode(), skillMoveTarget, reboundMove);
                GameRecord.ReboundLaunch++;
                Result = true;
                break;
            case EAnimatorState.JumpBall:
                AnimatorControl.Play(EAnimatorState.JumpBall, nextState.StateNo, Team.GetHashCode());
                SetShooterLayer();
                Result = true;
                break;
            case EAnimatorState.TipIn:
                AnimatorControl.Play(EAnimatorState.TipIn, nextState.StateNo, Team.GetHashCode());
                SetShooterLayer();
                Result = true;
                break;
        }
        
        if (Result)
        {
            crtState = state;
                        
            if (crtState == EPlayerState.Idle && NeedResetFlag)
                ResetFlag();
        }

        return Result;
    }

    public void SetShooterLayer()
    {
        PlayerRefGameObject.layer = LayerMask.NameToLayer("Shooter");
    }

    #region 動作委託

    private void InitAnmator()
    {
        AnimatorControl = PlayerRefGameObject.AddComponent<AnimatorBehavior>();
        AnimatorControl.Init(PlayerRefGameObject.GetComponent<Animator>());
        AnimatorControl.GotStealingDel = GotStealing;
        AnimatorControl.FakeShootBlockMomentDel = FakeShootBlockMoment;
        AnimatorControl.BlockMomentDel = BlockMoment;
        AnimatorControl.AirPassMomentDel = AirPassMoment;
        AnimatorControl.DoubleClickMomentDel = DoubleClickMoment;
        AnimatorControl.BuffEndDel = BuffEnd;
        AnimatorControl.BlockCatchMomentStartDel = BlockCatchMomentStart;
        AnimatorControl.BlockCatchMomentEndDel = BlockCatchMomentEnd;
        AnimatorControl.BlockJumpDel = BlockJump;
        AnimatorControl.BlockCatchingEndDel = BlockCatchingEnd;
        AnimatorControl.ShootingDel = Shooting;
        AnimatorControl.MoveDodgeEndDel = MoveDodgeEnd;
        AnimatorControl.PassingDel = Passing;
        AnimatorControl.PassEndDel = PassEnd;
        AnimatorControl.ShowEndDel = ShowEnd;
        AnimatorControl.PickUpDel = PickUp;
        AnimatorControl.PickEndDel = PickEnd;
        AnimatorControl.StealingDel = Stealing;
        AnimatorControl.StealingEndDel = StealingEnd;
        AnimatorControl.PushCalculateStartDel = PushCalculateStart;
        AnimatorControl.PushCalculateEndDel = PushCalculateEnd;
        AnimatorControl.ElbowCalculateStartDel = ElbowCalculateStart;
        AnimatorControl.ElbowCalculateEndDel = ElbowCalculateEnd;
        AnimatorControl.BlockCalculateStartDel = BlockCalculateStart;
        AnimatorControl.BlockCalculateEndDel = BlockCalculateEnd;
        AnimatorControl.CloneMeshDel = CloneMesh;
        AnimatorControl.DunkBasketStartDel = DunkBasketStart;
        AnimatorControl.OnlyScoreDel = OnlyScore;
        AnimatorControl.DunkFallBallDel = DunkFallBall;
        AnimatorControl.ElbowEndDel = ElbowEnd;
        AnimatorControl.FallEndDel = FallEnd;
        AnimatorControl.FakeShootEndDel = FakeShootEnd;
        AnimatorControl.TipInStartDel = TipInStart;
        AnimatorControl.TipInEndDel = TipInEnd;
        AnimatorControl.AnimationEndDel = AnimationEnd;
        AnimatorControl.ShowDel = OnShowCallBack;
        AnimatorControl.SkillDel = SkillEventCallBack;
        AnimatorControl.ZoomInDel = OnZoomIn;
        AnimatorControl.ZoomOutDel = OnZoomOut;
        AnimatorControl.CatchDel = CatchEnd;
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
        if (GameController.Get.IsShowSituation)
            return;
        if (OnGotSteal != null)
        if (OnGotSteal(this))
            GameRecord.BeSteal++; 
    }

    private void FakeShootBlockMoment()
    {
        if (GameController.Get.IsShowSituation)
            return;
        if (!IsAllShoot && OnFakeShootBlockMoment != null)
            OnFakeShootBlockMoment(this);
    }

    private void BlockMoment()
    {
        if (GameController.Get.IsShowSituation)
            return;
        if (OnBlockMoment != null)
            OnBlockMoment(this);
    }

    private void AirPassMoment()
    {
        if (GameController.Get.IsShowSituation)
            return;
        IsPassAirMoment = true;
    }

    private void DoubleClickMoment()
    {
        if (GameController.Get.IsShowSituation)
            return;
        if (OnDoubleClickMoment != null)
            OnDoubleClickMoment(this, crtState);
    }

    private void BuffEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        if (IsBallOwner)
        {
            AniState(EPlayerState.HoldBall);
            IsFirstDribble = true;
        }
        else
            AniState(EPlayerState.Idle);
    }

    private void BlockCatchMomentStart()
    {
        if (GameController.Get.IsShowSituation)
            return;
        blockCatchTrigger.SetEnable(true); 
    }

    private void BlockCatchMomentEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        IsPerfectBlockCatch = false;
    }

    private void BlockJump()
    {
        if (GameController.Get.IsShowSituation)
            return;
        if (OnBlockJump != null)
            OnBlockJump(this); 
    }

    private void BlockCatchingEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
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
        if (GameController.Get.IsShowSituation)
            return;
        IsPassAirMoment = false;
        if (OnShooting != null)
        {
            if (crtState != EPlayerState.Pass4)
                OnShooting(this, false);
            else if (crtState == EPlayerState.Layup0)
            {
                if (CourtMgr.Get.RealBallObj.transform.parent == DummyBall.transform)
                {
                    LogMgr.Get.Log(PlayerRefGameObject.name + " layup no ball.");
                    GameController.Get.SetBall();
                }
            }
        }
    }

    private void MoveDodgeEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        OnUI(this);
        if (IsBallOwner)
            AniState(EPlayerState.Dribble0);
        else
            AniState(EPlayerState.Idle);
    }

    private void Passing()
    {
        if (GameController.Get.IsShowSituation)
            return;
        //0.Flat
        //2.Floor
        //1 3.Parabola(Tee)
        if (IsBallOwner)
        {
            if (GameController.Get.IsCatcherAlleyoop)
            {
                CourtMgr.Get.RealBallCompoment.Trigger.PassBall(99);   
            }
            else
                CourtMgr.Get.RealBallCompoment.Trigger.PassBall(AnimatorControl.StateNo);

            GameController.Get.IsCatcherAlleyoop = false;
        } 
    }

    private void PassEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        OnUI(this);

        if (!IsBallOwner && PlayerRefGameObject.transform.localPosition.y < 0.2f)
            AniState(EPlayerState.Idle); 
    }

    private void ShowEnd()
    {
        ReadyToNextState = true;
        if (!IsBallOwner)
            AniState(EPlayerState.Idle);
        else
            AniState(EPlayerState.HoldBall);
    }

    private void PickUp()
    {
        if (GameController.Get.IsShowSituation)
            return;
        if (OnPickUpBall != null)
        if (OnPickUpBall(this))
            GameRecord.SaveBall++;
    }

    private void PickEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        if (IsBallOwner)
        {
            IsFirstDribble = true;
            AniState(EPlayerState.HoldBall);
        }
        else
            AniState(EPlayerState.Idle); 
    }

    private void Stealing()
    {
        if (GameController.Get.IsShowSituation)
            return;
        IsStealCalculate = true; 
    }

    private void StealingEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        IsStealCalculate = false; 
    }

    private void PushCalculateStart()
    {
        if (GameController.Get.IsShowSituation)
            return;
        IsPushCalculate = true;
    }

    private void PushCalculateEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        IsPushCalculate = false; 
    }

    private void ElbowCalculateStart()
    {
        if (GameController.Get.IsShowSituation)
            return;
        IsElbowCalculate = true;  
    }

    private void ElbowCalculateEnd()
    {
        IsElbowCalculate = false;
    }

    private void BlockCalculateStart()
    {
        if (GameController.Get.IsShowSituation)
            return;
        blockTrigger.SetActive(true);
    }

    private void BlockCalculateEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        blockTrigger.SetActive(false); 
    }

    private void CloneMesh()
    {
        if (GameController.Get.IsShowSituation)
            return;
        if (!IsBallOwner)
            AnimatorControl.PlayDunkCloneMesh();   
    }

    private void DunkBasketStart()
    {
        if (GameController.Get.IsShowSituation)
            return;
        CourtMgr.Get.PlayDunk(Team.GetHashCode(), AnimatorControl.StateNo); 
    }

    private void OnlyScore()
    {
        if (GameController.Get.IsShowSituation)
            return;
        if (OnOnlyScore != null)
            OnOnlyScore(this);
        CourtMgr.Get.IsBallOffensive = true;
    }

    private void DunkFallBall()
    {
        OnUI(this);
        if (OnDunkBasket != null)
            OnDunkBasket(this);
        CourtMgr.Get.IsBallOffensive = false;
    }

    private void ElbowEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        OnUI(this);
        if (IsBallOwner)
            AniState(EPlayerState.HoldBall);
        else
            AniState(EPlayerState.Idle);
        CourtMgr.Get.RealBallCompoment.ShowBallSFX(Attr.PunishTime); 
    }

    private void FallEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        OnUI(this);
        InitFlag();
        AniState(EPlayerState.Idle); 
    }

    private void CatchEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
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

    private void FakeShootEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        isFakeShoot = false;
        if (IsBallOwner)
            AniState(EPlayerState.HoldBall);
        else
            AniState(EPlayerState.Idle);

        OnUI(this);
        CourtMgr.Get.RealBallCompoment.ShowBallSFX(Attr.PunishTime);
    }

    private void TipInStart()
    {
        if (GameController.Get.IsShowSituation)
            return;
        CanUseTipIn = true;
    }

    private void TipInEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        CanUseTipIn = false;
    }

    private void AnimationEnd()
    {
        if (GameController.Get.IsShowSituation)
            return;
        ReadyToNextState = true;
        OnUI(this);
        PlayerSkillController.ResetUseSkill();

        if (!IsBallOwner)
            AniState(EPlayerState.Idle);
        else
        {
            if (firstDribble)
                AniState(EPlayerState.Dribble0);
        }

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
        IsPassAirMoment = false;
        blockTrigger.SetActive(false);
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
        }
    }

    public void PlaySound(string soundName)
    {
        AudioMgr.Get.PlaySound(soundName);
    }

    public void TimeScaleCallBack(AnimationEvent aniEvent, PlayerBehaviour player)
    {
        return;
//        if (player != TimerMgr.Get.TimeController)
//            return;
//        
//        float floatParam = aniEvent.floatParameter;
//        int intParam = aniEvent.intParameter;
//			
//		Debug.LogWarning ("aniEvent : " + aniEvent.ToString ());
//
//		if(floatParam <= GameConst.Min_TimePause)
//			floatParam = GameConst.Min_TimePause;
//
//        switch (intParam)
//        {
//            case 0: //set all
//                foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
//                    TimerMgr.Get.ChangeTime(item, floatParam);
//
//				TimerMgr.Get.PauseBall(IsTimePause);
//                break;
//            case 1: //Set myself
//                foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
//					if (item == TimerKind)
//                       TimerMgr.Get.ChangeTime(item, floatParam);
//                break;
//            case 2: //Set Other 
//                foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
//					if (item != TimerKind && item != ETimerKind.Default)
//                        TimerMgr.Get.ChangeTime(item, floatParam);
//
//				TimerMgr.Get.PauseBall(IsTimePause);
//                break;
//            case 3: //Set Default
//                TimerMgr.Get.ChangeTime(ETimerKind.Default, floatParam);
//                break;
//            case 4: //Set Other Without Ball
//                foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
//					if (item != TimerKind && item != ETimerKind.Default)
//                        TimerMgr.Get.ChangeTime(item, floatParam);
//                break;
//        }
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
                GameController.Get.BallState = EBallState.None;
                if (OnShooting != null)
                    OnShooting(this, true);
                break;
            case "PushDistancePlayer":
                if (GameData.DSkillData.ContainsKey(ActiveSkillUsed.ID) && (GameController.Get.Situation == EGameSituation.GamerAttack ||
                    GameController.Get.Situation == EGameSituation.NPCAttack))
                {
                    GameController.Get.CheckConditionText();
                    GameController.Get.IsGameFinish();
                    for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
                    {
                        if (GameController.Get.GamePlayers[i].Team != Team)
                        {
                            if (GameController.Get.GetDis(new Vector2(GameController.Get.GamePlayers[i].transform.position.x, GameController.Get.GamePlayers[i].transform.position.z), 
                                    new Vector2(PlayerRefGameObject.transform.position.x, PlayerRefGameObject.transform.position.z)) <= GameData.DSkillData[ActiveSkillUsed.ID].Distance(ActiveSkillUsed.Lv))
                            {
                                if (GameController.Get.GamePlayers[i].IsAllShoot || GameController.Get.GamePlayers[i].IsDunk)
                                    GameController.Get.GamePlayers[i].AniState(EPlayerState.KnockDown0, PlayerRefGameObject.transform.position);
                                else
                                    GameController.Get.GamePlayers[i].AniState(EPlayerState.Fall1, PlayerRefGameObject.transform.position);
								
                            }
                        } 
                    }
                }
                break;
            case "SetBallEvent":
                GameController.Get.SetBall(this);
                GameController.Get.CheckConditionText();
                GameController.Get.IsGameFinish();
            
                if (GameController.Get.Catcher != null)
                    GameController.Get.Catcher = null;
                if (GameController.Get.Passer != null)
                    GameController.Get.Passer = null;
                if (GameController.Get.Shooter != null)
                    GameController.Get.Shooter = null;
                break;
            case "ActiveSkillEnd":
                if (isSkillShow)
                {
                    if (OnUIJoystick != null)
                        OnUIJoystick(this, true);
    				
                    isSkillShow = false;
                    UISkillEffect.UIShow(false);
                    if (isShootJumpActive)
                    {
                        isShootJumpActive = false;
                        animatorEvent.intParameter = 4;
                    }
                    else
                        animatorEvent.intParameter = 0;
				
                    animatorEvent.floatParameter = 1;
                    TimeScaleCallBack(animatorEvent, this);
					
//					PlayerSkillController.ResetUseSkill();
                    if (isBlock)
                    {
                        if (GameController.Get.BallState == EBallState.CanBlock)
                        {
                            skillFaceTarget = judgePlayerFace(PlayerRefGameObject.transform.eulerAngles.y);
                            Vector3 pos = new Vector3(skillFaceTarget.x, -1, skillFaceTarget.z) * 40;
                            CourtMgr.Get.RealBallCompoment.AddForce(pos, ForceMode.VelocityChange);
                        }
                        else if (GameController.Get.BallState == EBallState.CanDunkBlock)
                        {
                            if (GameController.Get.BallOwner != null)
                                GameController.Get.BallOwner.AniState(EPlayerState.KnockDown0);
                        }
                        GameController.Get.BallState = EBallState.None;
                        GameController.Get.CheckConditionText();
                        GameController.Get.IsGameFinish();
                    }
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
        {
            TimerMgr.Get.ResetTime();  
        }
    }

    public void StartSkillCamera(int no)
    {
        if (no >= 20)
        if (GameData.DSkillData.ContainsKey(PassiveSkillUsed.ID))
            UIPassiveEffect.Get.ShowView(PassiveSkillUsed, this);
			
		
        if (no < 20 && GameController.Get.CheckOthersUseSkill(TimerKind.GetHashCode()))
        {
            return;
        }
	
        if (GameData.DSkillData.ContainsKey(ActiveSkillUsed.ID))
        {
            //Debug.LogError(gameObject.name + " . Skill Start");

            int skillEffectKind = GameData.DSkillData[ActiveSkillUsed.ID].ActiveCamera;
            float skillTime = GameData.DSkillData[ActiveSkillUsed.ID].ActiveCameraTime;
			TimerMgr.Get.PauseTimeByUseSkill(skillTime, ResetTimeCallBack);
//            if (this == GameController.Get.Joysticker)
//            {
            if (!isSkillShow)
            {
                if (OnUIJoystick != null)
                    OnUIJoystick(this, false);
                
                if (UIPassiveEffect.Visible)
                    UIPassiveEffect.UIShow(false);
                
                isSkillShow = true;
//					string effectName = string.Format("UseSkillEffect_{0}", GameData.DSkillData[ActiveSkillUsed.ID].Kind);
                string effectName = string.Format("UseSkillEffect_{0}", 0);
                EffectManager.Get.PlayEffect(effectName, transform.position, null, null, 1, false);
                
//                    if (GameController.Get.BallOwner != null && GameController.Get.BallOwner == GameController.Get.Joysticker)
                if (GameController.Get.BallOwner != null)
                    LayerMgr.Get.SetLayerRecursively(CourtMgr.Get.RealBallObj, "SkillPlayer", "RealBall");
                
                CameraMgr.Get.SkillShowActive(this, skillEffectKind, skillTime);
                UISkillEffect.Get.ShowView(ActiveSkillUsed);
                AudioMgr.Get.PlaySound(SoundType.SD_ActiveLaunch);
                
                switch (skillEffectKind)
                {
                    case 0://show self and rotate camera
                        Invoke("showEffect", skillTime);
//					LayerMgr.Get.SetLayerRecursively(GameController.Get.Joysticker.PlayerRefGameObject, "SkillPlayer", "PlayerModel", "(Clone)");
                        LayerMgr.Get.SetLayerRecursively(PlayerRefGameObject, "SkillPlayer", "PlayerModel", "(Clone)");
					
//                        animatorEvent.floatParameter = GameConst.Min_TimePause;
//                        animatorEvent.intParameter = 0;
                        //TODO: TimePause(true)
//                        TimerMgr.Get.PauseTime(true);
//                        TimeScaleCallBack(animatorEvent);   
                        break;
                    case 1://show self
//                        showActiveEffect();
//					LayerMgr.Get.SetLayerRecursively(GameController.Get.Joysticker.PlayerRefGameObject, "SkillPlayer", "PlayerModel", "(Clone)");
//                        LayerMgr.Get.SetLayerRecursively(PlayerRefGameObject, "SkillPlayer", "PlayerModel", "(Clone)");
//                        animatorEvent.floatParameter = GameConst.Min_TimePause;
//                        animatorEvent.intParameter = 2;
//                        TimeScaleCallBack(animatorEvent); 
                        break;
                    case 2://show all Player
//                        showActiveEffect();
//                        GameController.Get.SetAllPlayerLayer("SkillPlayer");
//                        animatorEvent.floatParameter = GameConst.Min_TimePause;
//                        animatorEvent.intParameter = 2;
//                        TimeScaleCallBack(animatorEvent); 
                        break;
                }
            }
//            }
//            else
//            {
//                if (GameData.DSkillData[ActiveSkillUsed.ID].Kind == 140 ||
//                    GameData.DSkillData[ActiveSkillUsed.ID].Kind == 150 ||
//                    GameData.DSkillData[ActiveSkillUsed.ID].Kind == 160 ||
//                    GameData.DSkillData[ActiveSkillUsed.ID].Kind == 161 ||
//                    GameData.DSkillData[ActiveSkillUsed.ID].Kind == 170 ||
//                    GameData.DSkillData[ActiveSkillUsed.ID].Kind == 171 ||
//                    GameData.DSkillData[ActiveSkillUsed.ID].Kind == 180)
//                {
//                
//                    if (!isSkillShow)
//                    {
//						animatorEvent.floatParameter = GameConst.Min_TimePause;
//                        animatorEvent.intParameter = 2;
//                        TimeScaleCallBack(animatorEvent); 
//
//                        isSkillShow = true;
//                    }
//                }
//
//                if (!IsUseActiveSkill)
//					UIPassiveEffect.Get.ShowView(ActiveSkillUsed, this);
//                showActiveEffect();
//            }
        }
    }

    public void StopSkill()
    {
        //TODO: TimePause(false)
//        Debug.LogError(gameObject.name + " . Skill Stop");
//        TimerMgr.Get.PauseTime(false);
//        animatorEvent.floatParameter = 1;
//        animatorEvent.intParameter = 1;
//        TimeScaleCallBack(animatorEvent); 
//		animatorEvent.floatParameter = 1;
//		animatorEvent.intParameter = 3;
//		TimeScaleCallBack(animatorEvent); 
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
        if (AI.enabled)
        {
            RemoveMoveData();
            CantMoveTimer.Clear();
        }
    }

    private void RemoveMoveData()
    {
        moveQueue.Clear(); 
		MoveTargetPos(gameObject.transform.position);
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
        if (CanUseActiveSkill(ActiveSkillUsed) || GameStart.Get.TestMode == EGameTest.Skill)
        {
            if (GameData.DSkillData[ActiveSkillUsed.ID].Kind == 210 && PlayerSkillController.IsGetBuff(ActiveSkillUsed.ID))
                return false;

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
            if (IsUseActiveSkill || StateChecker.StopStates.ContainsKey(crtState) || IsFall || IsShoot || IsDunk || IsLayup)
                return false;
            else
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
        if ((CanMove || crtState == EPlayerState.HoldBall) && IsAngerFull(tSkill))
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
        get{ return crtState == EPlayerState.HoldBall; }
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
            return (crtState == EPlayerState.Buff20 || crtState == EPlayerState.Buff21 || crtState == EPlayerState.Block20 ||
            crtState == EPlayerState.Dunk20 || crtState == EPlayerState.Dunk21 || crtState == EPlayerState.Dunk22 ||
            crtState == EPlayerState.Elbow20 || crtState == EPlayerState.Elbow21 ||
            crtState == EPlayerState.Push20 ||
            crtState == EPlayerState.Rebound20 ||
            crtState == EPlayerState.Shoot20 ||
            crtState == EPlayerState.Steal20);
        }
    }

    public bool IsBlock
    {
        get{ return crtState == EPlayerState.Block0 || crtState == EPlayerState.Block1 || crtState == EPlayerState.Block2 || crtState == EPlayerState.Block20; }
    }

    public bool IsBlockCatch
    {
        get{ return crtState == EPlayerState.BlockCatch; } 
    }

    public bool IsShoot
    {
        get { return StateChecker.ShootStates.ContainsKey(crtState); }
    }

    public bool IsShow
    {
        get{ return StateChecker.ShowStates.ContainsKey(crtState); }
    }

    public bool IsLoopState
    {
        get{ return StateChecker.LoopStates.ContainsKey(crtState); }
    }

    public bool IsBuff
    {
        get { return crtState == EPlayerState.Buff20 || crtState == EPlayerState.Buff21; }
    }

    public bool IsAllShoot
    {
        get{ return IsShoot || IsDunk || IsLayup; }
    }

    public bool IsIdle
    {
        get{ return crtState == EPlayerState.Idle; }
    }

    public bool IsDef
    {
        get{ return crtState == EPlayerState.Defence0 || crtState == EPlayerState.Defence1; }
    }

    public bool IsRun
    {
        get{ return crtState == EPlayerState.Run0 || crtState == EPlayerState.Run1 || crtState == EPlayerState.Run2; }
    }

    public bool IsPass
    {
        get{ return StateChecker.PassStates.ContainsKey(crtState); }
    }

    public bool IsPickBall
    {
        get{ return crtState == EPlayerState.Pick0 || crtState == EPlayerState.Pick1 || crtState == EPlayerState.Pick2; }
    }

    public bool IsDribble
    {
        get{ return crtState == EPlayerState.Dribble0 || crtState == EPlayerState.Dribble1 || crtState == EPlayerState.Dribble2 || crtState == EPlayerState.Dribble3; }
    }

    public bool IsDunk
    {
        get{ return crtState == EPlayerState.Dunk0 || crtState == EPlayerState.Dunk1 || crtState == EPlayerState.Dunk3 || crtState == EPlayerState.Dunk5 || crtState == EPlayerState.Dunk7 || crtState == EPlayerState.Dunk2 || crtState == EPlayerState.Dunk4 || crtState == EPlayerState.Dunk6 || crtState == EPlayerState.Dunk20 || crtState == EPlayerState.Dunk21 || crtState == EPlayerState.Dunk22; }
    }

    public bool IsLayup
    {
        get{ return crtState == EPlayerState.Layup0 || crtState == EPlayerState.Layup1 || crtState == EPlayerState.Layup2 || crtState == EPlayerState.Layup3; }
    }

    public bool IsMoveDodge
    {
        get{ return crtState == EPlayerState.MoveDodge0 || crtState == EPlayerState.MoveDodge1; }
    }

    public bool IsSteal
    {
        get{ return crtState == EPlayerState.Steal0 || crtState == EPlayerState.Steal1 || crtState == EPlayerState.Steal2 || crtState == EPlayerState.Steal20; }
    }

    public bool IsGotSteal
    {
        get{ return crtState == EPlayerState.GotSteal; }
    }

    public bool IsTipIn
    {
        get{ return crtState == EPlayerState.TipIn; }
    }

    public bool IsKnockDown
    {
        get{ return crtState == EPlayerState.KnockDown0 || crtState == EPlayerState.KnockDown1; }
    }

    private bool isMoving = false;

    public bool IsMoving
    {
        get{ return isMoving; }
    }

    public bool IsRebound
    {
        get{ return crtState == EPlayerState.Rebound0 || crtState == EPlayerState.Rebound20 || crtState == EPlayerState.ReboundCatch; }
    }

    public bool IsFall
    {
        get{ return crtState == EPlayerState.Fall0 || crtState == EPlayerState.Fall1 || crtState == EPlayerState.Fall2 || crtState == EPlayerState.KnockDown0 || crtState == EPlayerState.KnockDown1; }
    }

    public bool IsCatch
    {
        get{ return crtState == EPlayerState.CatchFlat || crtState == EPlayerState.CatchFloor || crtState == EPlayerState.CatchParabola; }
    }

    public bool IsFakeShoot
    {
        get{ return isFakeShoot; }
    }

    public bool IsPush
    {
        get{ return crtState == EPlayerState.Push0 || crtState == EPlayerState.Push1 || crtState == EPlayerState.Push2 || crtState == EPlayerState.Push20; }
    }

    public bool IsIntercept
    {
        get{ return crtState == EPlayerState.Intercept0 || crtState == EPlayerState.Intercept1; }
    }

    public bool IsElbow
    {
        get{ return crtState == EPlayerState.Elbow0 || crtState == EPlayerState.Elbow1 || crtState == EPlayerState.Elbow2 || crtState == EPlayerState.Elbow20 || crtState == EPlayerState.Elbow21; }
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
                    OnDoubleClickMoment(this, crtState);
            }
//                  EffectManager.Get.PlayEffect("DoubleClick01", Vector3.zero, null, PlayerGameObject, 1f);
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
//      get { return PlayerRefGameObject.activeSelf && aiTime <= 0; }
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
            else
                return moveQueue.Peek().TacticalName;
        }
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

    public PlayerBehaviour DefPlayer
    {
        get
        {
            if (AI.enabled)
                return defencePlayer;
            else
                return null;
        }

        set
        {
            defencePlayer = value;
        }
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

    public bool IsTimePause
    {
        get
        {
            return (TimerMgr.Get.CrtTime <= GameConst.Min_TimePause);
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
//                    if (Index == 1)
                    if (Index == EPlayerPostion.F)
                        result.x += 1.5f;
                    else
                        result.x -= 1.5f;
                    break;
                case EPlayerPostion.F:
//                    if (Index == 0)
                    if (Index == EPlayerPostion.C)
                        result.x += 1.5f;
                    else
                        result.x -= 1.5f;
                    break;
                case EPlayerPostion.G:
//                    if (Index == 0)
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
