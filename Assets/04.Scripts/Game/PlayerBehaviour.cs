using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using DG.Tweening;
using GameStruct;
using GamePlayEnum;
using Chronos;
using JetBrains.Annotations;

public delegate bool OnPlayerAction(PlayerBehaviour player);

public delegate void OnPlayerAction1(PlayerBehaviour player);

public delegate void OnPlayerAction2(PlayerBehaviour player,bool speedup);

public delegate bool OnPlayerAction3(PlayerBehaviour player,bool speedup);

public delegate bool OnPlayerAction4(PlayerBehaviour player,EPlayerState state);

public delegate void OnPlayerAction5(float max,float anger,int count);

public enum EPlayerPostion
{
    C = 0, 
    F = 1, 
    G = 2
}

public enum ETeamKind
{
    Self = 0,
    Npc = 1
}

public enum EDefPointKind
{
    Front = 0,
    Back = 1,
    Right = 2,
    Left = 3,
    FrontSteal = 4,
    BackSteal = 5,
    RightSteal = 6,
    LeftSteal = 7
}

public enum EActionFlag
{
    None = 0,
    IsRun = 1,
    IsDefence = 2,
    IsDribble = 3,
    IsHoldBall = 4,
}

public enum EBallDirection
{
    Left,
    Middle,
    Right
}

public struct TMoveData
{
    public Vector2 Target
    {
        get { return mTarget; }
    }

    private Vector2 mTarget;
    public Transform LookTarget;
    public Transform FollowTarget;
    public PlayerBehaviour DefPlayer;
    public OnPlayerAction2 MoveFinish;
    public bool Speedup;
    public bool Catcher;
    public bool Shooting;
    public string TacticalName; // for debug.

    public void Clear()
    {
        mTarget = Vector2.zero;
        LookTarget = null;
        MoveFinish = null;
        FollowTarget = null;
        DefPlayer = null;
        Speedup = false;
        Catcher = false;
        Shooting = false;
        TacticalName = "";
    }

    public void SetTarget(float x, float y)
    {
        mTarget.Set(x, y);
    }
}

[System.Serializable]
public struct TScoreRate
{
    public int TwoScoreRate;
    public float TwoScoreRateDeviation;
    public int ThreeScoreRate;
    public float ThreeScoreRateDeviation;
    public int DownHandScoreRate;
    public int DownHandSwishRate;
    public int DownHandAirBallRate;
    public int UpHandScoreRate;
    public int UpHandSwishRate;
    public int UpHandAirBallRate;
    public int NormalScoreRate;
    public int NormalSwishRate;
    public int NormalAirBallRate;
    public int NearShotScoreRate;
    public int NearShotSwishRate;
    public int NearShotAirBallRate;
    public int LayUpScoreRate;
    public int LayUpSwishRate;
    public int LayUpAirBallRate;

    public TScoreRate(int flag)
    {
        TwoScoreRate = 70;
        TwoScoreRateDeviation = 0.8f;
        ThreeScoreRate = 50;
        ThreeScoreRateDeviation = 0.5f;
        DownHandScoreRate = 40;
        DownHandSwishRate = 50;
        DownHandAirBallRate = 35;
        UpHandScoreRate = 20;
        UpHandSwishRate = 30;
        UpHandAirBallRate = 15;
        NormalScoreRate = 0;
        NormalSwishRate = 0;
        NormalAirBallRate = 8;
        NearShotScoreRate = 10;
        NearShotSwishRate = 10;
        NearShotAirBallRate = 3;
        LayUpScoreRate = 20;
        LayUpSwishRate = 20;
        LayUpAirBallRate = 2;
    }
}

public class TSkillAttribute
{
    public int ID;
    public int Kind;
    public float Value;
    public float CDTime;
}

public class PlayerBehaviour : MonoBehaviour
{
    public Timeline Timer;
    public OnPlayerAction1 OnShooting = null;
    public OnPlayerAction OnPass = null;
    public OnPlayerAction OnStealMoment = null;
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
    public OnPlayerAction4 OnDoubleClickMoment = null;
    public OnPlayerAction3 OnUIJoystick = null;
    public bool IsJumpBallPlayer = false;
    public GameObject PlayerRefGameObject;
    public int ShowPos = -1;
    public string MoveName = "";
//    public float[] DunkHight = new float[2]{3, 5};
    private const float MoveCheckValue = 1;
//    public static string[] AnimatorStates = new string[] {"", "IsRun", "IsDefence", "IsDribble", "IsHoldBall"};
    private byte[] PlayerActionFlag = {0, 0, 0, 0, 0, 0, 0};
    private bool stop = false;
    private bool NeedResetFlag = false;
    private int MoveTurn = 0;
    private float moveStartTime = 0;
    private float ProactiveTime = 0;
    private int smoothDirection = 0;
    private float animationSpeed = 0;
    private float MoveMinSpeed = 0.5f;
    private float canDunkDis = 30f;
    private readonly Queue<TMoveData> moveQueue = new Queue<TMoveData>();
    private Vector3 translate;
    public Rigidbody PlayerRigidbody;
    public Animator AnimatorControl;
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

    /// <summary>
    /// 0: Center, 1:Forward, 2:Guard.
    /// </summary>
    public int Index;
//    private float aiTime = 0; // 0: AI 控制中; > 0 玩家控制中.
    private readonly StatusTimer mManually = new StatusTimer();
    public EGameSituation situation = EGameSituation.None;
    public EPlayerState crtState = EPlayerState.Idle;
    public EAnimatorState crtAnimatorState = EAnimatorState.Idle;
    public Transform[] DefPointAy = new Transform[8];
//    public float WaitMoveTime = 0;

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
    public float JumpHight = 450f;
    public float CoolDownElbow = 0;
//    public float AirDrag = 0f;
//    public float fracJourney = 0;
    public int MoveIndex = -1;
    public bool isJoystick = false;
    [CanBeNull]
    public PlayerBehaviour DefPlayer = null;
    public float CloseDef = 0;
    public bool AutoFollow = false;
    public bool NeedShooting = false;
    public EPlayerPostion Postion;

    //Dunk
    private bool isDunk = false;
//    private bool isDunkZmove = false;
    private float dunkCurveTime = 0;
    public AniCurve aniCurve;
    private TDunkCurve playerDunkCurve;

    //Layup
    private bool isLayup = false;
    private bool isLayupZmove = false;
    private float layupCurveTime = 0;
    private TLayupCurve playerLayupCurve;
        
    //Block
    private bool isCanBlock = false;
    private bool isBlock = false;
    private float blockCurveTime = 0;
    private TBlockCurve playerBlockCurve;
    private Vector3 skillMoveTarget;
    private Vector3 skillFaceTarget;
    private bool isShootBlock = false;
    private bool isDunkBlock = false;

    //Rebound
    private bool isRebound = false;
    private float reboundCurveTime = 0;
    private Vector3 reboundMove;
    private TReboundCurve playerReboundCurve;

    //Shooting
    private float shootJumpCurveTime = 0;
    private TShootCurve playerShootCurve;
    private bool isShootJump = false;
    private bool isFakeShoot = false;

    //Push
    private bool isPush = false;
    private float pushCurveTime = 0;
    private TSharedCurve playerPushCurve;

    //Steal
    private bool isSteal = false;
    private float stealCurveTime = 0;
    private TStealCurve playerStealCurve;

    //Fall
    private bool isFall = false;
    private float fallCurveTime = 0;
    private TSharedCurve playerFallCurve;

    //Pick
    private bool isPick = false;
    private float pickCurveTime = 0;
    private TSharedCurve playerPickCurve;

    //Skill
    private SkillController skillController;
    private ESkillKind skillKind; // For Shoot and Layup
    private bool isUsePass = false;

    //Active
    private bool isUseSkill = false;
    private int angerPower = 0;

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
    public ETimerKind CrtTimeKey = ETimerKind.Default;

    //SkillEvent
    private bool isSkillShow = false;

    //Camera
    private float yAxizOffset = 0;

    //Change Player Color Value
    private bool isChangeColor = false;
    private float changeTime;
    private Color colorStart = new Color32(150, 150, 150, 255);

    public void SetAnger(int value, GameObject target = null, GameObject parent = null)
    {
        int v = (int)(Mathf.Abs(value) / 10);
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
        angerPower += value;
        if (angerPower > Attribute.MaxAnger)
        {
            angerPower = Attribute.MaxAnger;
        }
        
        if (angerPower < 0)
            angerPower = 0;
        
        if (Team == ETeamKind.Self && Index == 0)
        {
            OnUIAnger(Attribute.MaxAnger, angerPower, v);
            if (value > 0)
                GameRecord.AngerAdd += value;
        }
    }

    public void SetSlowDown(float Value)
    {
        if (SlowDownTime == 0)
        {
            SlowDownTime += Time.time + Value;
            Attr.SpeedValue = GameData.BaseAttr [Attribute.AILevel].SpeedValue * GameConst.SlowDownValue;
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

        AnimatorControl = PlayerRefGameObject.GetComponent<Animator>();
        skillController = PlayerRefGameObject.GetComponent<SkillController>();
        PlayerRigidbody = PlayerRefGameObject.GetComponent<Rigidbody>();
        if (PlayerRigidbody == null)
            PlayerRigidbody = PlayerRefGameObject.AddComponent<Rigidbody>();

        PlayerRigidbody.mass = 0.1f;
        PlayerRigidbody.drag = 10f;
        PlayerRigidbody.freezeRotation = true;

        ScoreRate = new TScoreRate(1);
        DashEffectEnable(false);
    }

    private void manuallyTimeUp()
    {
        moveQueue.Clear();

        if (AIActiveHint)
            AIActiveHint.SetActive(true);

        if (SpeedUpView)
            SpeedUpView.enabled = false;
    }
    
    private void changePlayerColor()
    {
        if (isChangeColor)
        {
            changeTime += Time.deltaTime;
            float lerp = (Mathf.PingPong(changeTime, 0.5f * GameConst.PlayerShineTime / GameConst.PlayerShineCount) * 310 * GameConst.PlayerShineCount / GameConst.PlayerShineTime);
            if (Team == ETeamKind.Self)
                BodyMaterial.color = new Color32((byte)lerp, (byte)lerp, 255, 255);
            else
                BodyMaterial.color = new Color32(255, (byte)lerp, (byte)lerp, 255);
            if (changeTime >= GameConst.PlayerShineTime)
                isChangeColor = false;
        } else
        {
            changeTime = 0;
            BodyMaterial.color = colorStart;
        }
    }

    public void SetTimerKey(ETimerKind key)
    {
        CrtTimeKey = key;
        if (Timer == null)
        {
            Timer = PlayerRefGameObject.AddComponent<Timeline>();
            Timer.mode = TimelineMode.Global;
            Timer.globalClockKey = CrtTimeKey.ToString();
            Timer.recordTransform = false;
        }
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
//      setMovePower(100);
        GameRecord.Init();
        GameRecord.ID = Attribute.ID;

        if (Index == 0 && Team == ETeamKind.Self && GameStart.Get.ConnectToServer)
            Attribute.SetAttribute(GameEnum.ESkillType.Player);
        else
            Attribute.SetAttribute(GameEnum.ESkillType.NPC);

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

        Attr.PointRate2 = GameData.BaseAttr [Attribute.AILevel].PointRate2 + (Attribute.Point2 * 0.5f);
        Attr.PointRate3 = GameData.BaseAttr [Attribute.AILevel].PointRate3 + (Attribute.Point3 * 0.5f);
        Attr.StealRate = GameData.BaseAttr [Attribute.AILevel].StealRate + (Attribute.Steal * 0.1f);
        Attr.DunkRate = GameData.BaseAttr [Attribute.AILevel].DunkRate + (Attribute.Dunk * 0.9f);
        Attr.TipInRate = GameData.BaseAttr [Attribute.AILevel].TipInRate + (Attribute.Dunk * 0.9f);
        Attr.AlleyOopRate = GameData.BaseAttr [Attribute.AILevel].AlleyOopRate + (Attribute.Dunk * 0.6f);
        Attr.StrengthRate = GameData.BaseAttr [Attribute.AILevel].StrengthRate + (Attribute.Strength * 0.9f);
        Attr.BlockPushRate = GameData.BaseAttr [Attribute.AILevel].BlockPushRate + (Attribute.Strength * 0.5f);
        Attr.ElbowingRate = GameData.BaseAttr [Attribute.AILevel].ElbowingRate + (Attribute.Strength * 0.8f);
        Attr.ReboundRate = GameData.BaseAttr [Attribute.AILevel].ReboundRate + (Attribute.Rebound * 0.9f);
        Attr.BlockRate = GameData.BaseAttr [Attribute.AILevel].BlockRate + (Attribute.Block * 0.9f);
        Attr.FaketBlockRate = GameData.BaseAttr [Attribute.AILevel].FaketBlockRate + (100 - (Attribute.Block / 1.15f));
        Attr.JumpBallRate = GameData.BaseAttr [Attribute.AILevel].JumpBallRate;
        Attr.PushingRate = GameData.BaseAttr [Attribute.AILevel].PushingRate + (Attribute.Defence * 1);
        Attr.PassRate = GameData.BaseAttr [Attribute.AILevel].PassRate + (Attribute.Pass * 0.7f);
        Attr.AlleyOopPassRate = GameData.BaseAttr [Attribute.AILevel].AlleyOopPassRate + (Attribute.Pass * 0.6f);
        Attr.ReboundHeadDistance = GameData.BaseAttr [Attribute.AILevel].ReboundHeadDistance + (Attribute.Rebound * 0.005f);
        Attr.ReboundHandDistance = GameData.BaseAttr [Attribute.AILevel].ReboundHandDistance + (Attribute.Rebound * 0.02f);
        Attr.BlockDistance = GameData.BaseAttr [Attribute.AILevel].BlockDistance + (Attribute.Block * 0.01f);
        Attr.DefDistance = GameData.BaseAttr [Attribute.AILevel].DefDistance + (Attribute.Defence * 0.1f);
        Attr.SpeedValue = GameData.BaseAttr [Attribute.AILevel].SpeedValue + (Attribute.Speed * 0.002f);
        Attr.StaminaValue = GameData.BaseAttr [Attribute.AILevel].StaminaValue + (Attribute.Stamina * 1f);
        Attr.AutoFollowTime = GameData.BaseAttr [Attribute.AILevel].AutoFollowTime;
            
        DefPoint.transform.localScale = new Vector3(Attr.DefDistance, Attr.DefDistance, Attr.DefDistance);
        TopPoint.transform.localScale = new Vector3(4 + Attr.ReboundHeadDistance, TopPoint.transform.localScale.y, 4 + Attr.ReboundHeadDistance);
        FingerPoint.transform.localScale = new Vector3(Attr.ReboundHandDistance, Attr.ReboundHandDistance, Attr.ReboundHandDistance);
        blockTrigger.transform.localScale = new Vector3(blockTrigger.transform.localScale.x, 3.2f + Attr.BlockDistance, blockTrigger.transform.localScale.z);
      
    }

    private void initSkill()
    {
        skillController.initSkillController(Attribute, this, AnimatorControl);
        skillController.OnAddAttribute += SetAttribute;

        if (Team == ETeamKind.Npc) 
            skillController.HidePlayerName();
    }

    public void InitDoubleClick()
    {
        if (DoubleClick == null)
        {
            DoubleClick = Instantiate(Resources.Load("Effect/DoubleClick")) as GameObject;
            DoubleClick.name = "DoubleClick";
            DoubleClick.transform.parent = PlayerRefGameObject.transform;
            DoubleClick.transform.localPosition = Vector3.zero;
        } 
    }

    public void InitCurve(GameObject animatorCurve)
    {
        GameObject AnimatorCurveCopy = Instantiate(animatorCurve) as GameObject;
        AnimatorCurveCopy.transform.parent = PlayerRefGameObject.transform;
        AnimatorCurveCopy.name = "AniCurve";
        aniCurve = AnimatorCurveCopy.GetComponent<AniCurve>();
    }

    public void InitTrigger(GameObject defPoint)
    {
        SkinnedMeshRenderer render = PlayerRefGameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (render && render.material) 
            BodyMaterial = render.material;

        DummyBall = transform.FindChild("DummyBall").gameObject;
        
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
            blockTrigger = obj2.transform.FindChild("Block").gameObject;
            ShowWord = obj2.transform.FindChild("ShowWord").gameObject;
            
            obj2.name = "BodyTrigger";
            PlayerTrigger[] objs = obj2.GetComponentsInChildren<PlayerTrigger>();
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i ++)
                    objs [i].Player = this;
            }
            
            DefTrigger defTrigger = obj2.GetComponentInChildren<DefTrigger>(); 
            if (defTrigger != null)
                defTrigger.Player = this;
            
            DefPoint = obj2.transform.FindChild("DefRange").gameObject;          
            TopPoint = obj2.transform.FindChild("TriggerTop").gameObject; 
            CatchBallPoint = obj2.transform.FindChild("CatchBall").gameObject; 
            obj2.transform.parent = transform;
            obj2.transform.transform.localPosition = Vector3.zero;
            obj2.transform.transform.localScale = Vector3.one;

            Transform t = obj2.transform.FindChild("TriggerFinger");
            if (t)
            {
                FingerPoint = t.gameObject;
                t.name = Team.GetHashCode().ToString() + Index.ToString() + "TriggerFinger";
                t.parent = transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Bip01 R Finger2/Bip01 R Finger21/");
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

            DefPointAy [EDefPointKind.Front.GetHashCode()] = DefPointCopy.transform.Find("Front");
            DefPointAy [EDefPointKind.Back.GetHashCode()] = DefPointCopy.transform.Find("Back");
            DefPointAy [EDefPointKind.Right.GetHashCode()] = DefPointCopy.transform.Find("Right");
            DefPointAy [EDefPointKind.Left.GetHashCode()] = DefPointCopy.transform.Find("Left");
            DefPointAy [EDefPointKind.FrontSteal.GetHashCode()] = DefPointCopy.transform.Find("FrontSteal");
            DefPointAy [EDefPointKind.BackSteal.GetHashCode()] = DefPointCopy.transform.Find("BackSteal");
            DefPointAy [EDefPointKind.RightSteal.GetHashCode()] = DefPointCopy.transform.Find("RightSteal");
            DefPointAy [EDefPointKind.LeftSteal.GetHashCode()] = DefPointCopy.transform.Find("LeftSteal");
        }
    }

    void FixedUpdate()
    {
//      if (Timer.state == TimeState.Paused || GameController.Get.IsShowSituation) {
//          return;
//      }
        if (GameController.Get.IsShowSituation)
        {
            return;
        }
        changePlayerColor();
        CalculationPlayerHight();
        CalculationAnimatorSmoothSpeed();
        CalculationBlock();
        CalculationDunkMove();
        CalculationShootJump();
        CalculationRebound();
        CalculationLayupMove();
        CalculationPush();
        CalculationFall();
        CalculationPick();
        CalculationSteal();
        DebugTool();
        
        CantMoveTimer.Update(Time.deltaTime);
        Invincible.Update(Time.deltaTime);
        StealCD.Update(Time.deltaTime);
        PushCD.Update(Time.deltaTime);
        mManually.Update(Time.deltaTime);

        if (CoolDownElbow > 0 && Time.time >= CoolDownElbow)
            CoolDownElbow = 0;

        if (SlowDownTime > 0 && Time.time >= SlowDownTime)
        {
            SlowDownTime = 0;
            Attr.SpeedValue = GameData.BaseAttr [Attribute.AILevel].SpeedValue + (Attribute.Speed * 0.005f);
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
                if (IsDefence && (CheckAnimatorSate(EPlayerState.RunningDefence) || CheckAnimatorSate(EPlayerState.Defence1)))
                    AniState(EPlayerState.Defence0);
                else if (!IsDefence && !IsBallOwner && IsRun)
                    AniState(EPlayerState.Idle);
            }
        }
//        else if(aiTime > 0 && Time.time >= aiTime)
//        {
//            moveQueue.Clear();
////            Debug.Log("FixedUpdate(), moveQueue.Clear().");
//
//            aiTime = 0;
//
//            if (AIActiveHint)
//                AIActiveHint.SetActive(true);
//
//            if (SpeedUpView)
//                SpeedUpView.enabled = false;
//        }

        if (situation == EGameSituation.AttackGamer || situation == EGameSituation.AttackNPC)
        {
            if (!IsDefence)
            {
                if (Time.time >= moveStartTime)
                {
                    moveStartTime = Time.time + GameConst.DefMoveTime;
                    GameController.Get.DefMove(this);
                }
            }
        }
        
        if (Time.time >= mMovePowerTime)
        {
//            mMovePowerTime = Time.time + 0.15f;
            mMovePowerTime = Time.time + GameConst.MovePowerCheckTime;
            if (isSpeedup)
            {
                if (mMovePower > 0)
                {
//                    mMovePower -= 1;
                    mMovePower -= GameConst.MovePowerMoving;
                    if (mMovePower < 0)
                        mMovePower = 0;

                    if (this == GameController.Get.Joysticker)
                        GameController.Get.Joysticker.SpeedUpView.fillAmount = mMovePower / mMaxMovePower;

                    if (mMovePower == 0)
                        canSpeedup = false;
                }
            } else if (mMovePower < mMaxMovePower)
            {
//                mMovePower += 2.5f;
                mMovePower += GameConst.MovePowerRevive;
                if (mMovePower > mMaxMovePower)
                    mMovePower = mMaxMovePower;

                if (this == GameController.Get.Joysticker)
                    GameController.Get.Joysticker.SpeedUpView.fillAmount = mMovePower / mMaxMovePower;

                if (mMovePower == mMaxMovePower)
                    canSpeedup = true;
            }
        }

        if (IsDefence)
        {
            if (Time.time >= ProactiveTime)
            {
                ProactiveTime = Time.time + 4;
//                TimeProactiveRate = UnityEngine.Random.Range(0, 100) + 1;
            }

            if (AutoFollow)
            {
                Vector3 ShootPoint;
                if (Team == ETeamKind.Self)
                    ShootPoint = CourtMgr.Get.ShootPoint [1].transform.position;
                else
                    ShootPoint = CourtMgr.Get.ShootPoint [0].transform.position;    

                if (Vector3.Distance(ShootPoint, DefPlayer.transform.position) <= GameConst.TreePointDistance)
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

            if (IsDribble)
                DribbleTime += Time.deltaTime;
        }
    }

    public void SetSelectTexture(string name)
    {
        if (selectTexture)
        {

        } else
        {
            GameObject obj = Resources.Load("Prefab/Player/" + name) as GameObject;
            if (obj)
            {
                selectTexture = Instantiate(obj) as GameObject;
                selectTexture.name = "Select";
                selectTexture.transform.parent = transform;
                selectTexture.transform.localPosition = new Vector3(0, 0.05f, 0);
            }
        }
    }

    public void DashEffectEnable(bool isEnable)
    {
        if (dashSmoke == null)
            dashSmoke = EffectManager.Get.PlayEffect("DashSmoke", Vector3.zero, PlayerRefGameObject);

        if (dashSmoke)
            dashSmoke.SetActive(isEnable);
    }

    IEnumerator GetCurrentClipLength()
    {
        yield return new WaitForEndOfFrame();
        float aniTime = AnimatorControl.GetCurrentAnimatorStateInfo(0).length;
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

        if (SpeedUpView)
            SpeedUpView.enabled = true;
    }

    /// <summary>
    /// 進入手控狀態.
    /// </summary>
    public void SetManually()
    {
        if (Team == ETeamKind.Self && Index == 0)
        {
            if (situation == EGameSituation.AttackGamer || situation == EGameSituation.AttackNPC ||
                GameStart.Get.TestMode != EGameTest.None)
            {
                isJoystick = true;
//              aiTime = Time.time + GameData.Setting.AIChangeTime;
                mManually.StartCounting(GameData.Setting.AIChangeTime);
                StartCoroutine(GetCurrentClipLength());

                if (AIActiveHint)
                    AIActiveHint.SetActive(false);

                if (SpeedUpView)
                    SpeedUpView.enabled = true;
            } else
            {
//              aiTime = 0;
                mManually.Clear();
                if (AIActiveHint)
                    AIActiveHint.SetActive(true);

                if (SpeedUpView)
                    SpeedUpView.enabled = false;
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

    private bool isAnimatorMove = false;

    private bool IsAnimatorMove
    {
        get{ return isAnimatorMove;}
        set{ isAnimatorMove = value;}
    }

    private void CalculationDunkMove()
    {
//      if (!isDunk || Timer.timeScale == 0)
//            return;
        if (!isDunk)
            return;

        if (playerDunkCurve != null)
        {
            dunkCurveTime += Time.deltaTime * Timer.timeScale;
            

//          if (IsAnimatorMove == false && dunkCurveTime >= playerDunkCurve.StartMoveTime && dunkCurveTime <= playerDunkCurve.ToBasketTime && Timer.timeScale != 0)
//            {
//              IsAnimatorMove = true;
//              float t = (playerDunkCurve.ToBasketTime - playerDunkCurve.StartMoveTime);
//              PlayerRefGameObject.transform.DOMoveZ(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.z, t).SetEase(Ease.Linear);
//              PlayerRefGameObject.transform.DOMoveX(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.x, t).SetEase(Ease.Linear);
//              PlayerRefGameObject.transform.DORotate(new Vector3(0, Team == 0? 0 : 180, 0), playerDunkCurve.ToBasketTime, 0);
            //            }
            Vector3 position = PlayerRefGameObject.transform.position;
            if (Timer.timeScale != 0)
            { 
                position.y = playerDunkCurve.aniCurve.Evaluate(dunkCurveTime);
                
                if (position.y < 0)
                    position.y = 0; 
                if (IsAnimatorMove == false && dunkCurveTime >= playerDunkCurve.StartMoveTime && dunkCurveTime <= playerDunkCurve.ToBasketTime)
                { 
                    PlayerRefGameObject.transform.DOPlay(); 
                    IsAnimatorMove = true; 
                    float t = (playerDunkCurve.ToBasketTime - playerDunkCurve.StartMoveTime); 
                    PlayerRefGameObject.transform.DOMoveZ(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.z, t).SetEase(Ease.Linear); 
                    PlayerRefGameObject.transform.DOMoveX(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.x, t).SetEase(Ease.Linear); 
                    PlayerRefGameObject.transform.DORotate(new Vector3(0, Team == 0 ? 0 : 180, 0), playerDunkCurve.ToBasketTime, 0); 
                } 
            } else if (Timer.timeScale == 0)
            { 
                PlayerRefGameObject.transform.DOPause(); 
            }

            PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x, position.y, PlayerRefGameObject.transform.position.z);

            if (dunkCurveTime > playerDunkCurve.BlockMomentStartTime && dunkCurveTime <= playerDunkCurve.BlockMomentEndTime)
                IsCanBlock = true;
            else
                IsCanBlock = false;

            if (dunkCurveTime >= playerDunkCurve.LifeTime)
            {
                PlayerRefGameObject.transform.DOKill();
                isDunk = false;
                IsCanBlock = false;
                IsAnimatorMove = false;
            }
        } else
        {
            isDunk = false;
            IsAnimatorMove = false;
            LogMgr.Get.LogError("playCurve is null");
        }
    }

    public void DunkFall()
    {
        PlayerRefGameObject.transform.DOKill();
        isDunk = false;
        IsCanBlock = false;
        IsAnimatorMove = false;
    }

    private void CalculationLayupMove()
    {
        if (!isLayup)
            return;
        
        if (playerLayupCurve != null)
        {
            layupCurveTime += Time.deltaTime * Timer.timeScale;
            
            Vector3 position = PlayerRefGameObject.transform.position;
            position.y = playerLayupCurve.aniCurve.Evaluate(layupCurveTime);
            
            if (position.y < 0)
                position.y = 0;
            
            PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x, position.y, PlayerRefGameObject.transform.position.z);
            
            if (!isLayupZmove && layupCurveTime >= playerLayupCurve.StartMoveTime)
            {
                isLayupZmove = true;
                int add = (Team == 0 ? -1 : 1);
                float t = (playerLayupCurve.ToBasketTime - playerLayupCurve.StartMoveTime) * 1 / Timer.timeScale;
                PlayerRefGameObject.transform.DOMoveZ(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.z + add, t).SetEase(Ease.Linear);
                PlayerRefGameObject.transform.DOMoveX(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.x, t).SetEase(Ease.Linear);
            }
            
            if (layupCurveTime >= playerLayupCurve.LifeTime)
                isLayup = false;
        } else
        {
            isLayup = false;
            LogMgr.Get.LogError("playCurve is null");
        }
    }

    public  bool InReboundDistance
    {
        get
        {
            return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), 
                                     new Vector2(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z)) <= 6;
        }
    }
    
    private void CalculationRebound()
    {
        if (Timer.timeScale == 0)
            return;
        if (isRebound && playerReboundCurve != null)
        {
            reboundCurveTime += Time.deltaTime * Timer.timeScale;
            if (situation != EGameSituation.JumpBall)
            {
                if (playerReboundCurve.isSkill)
                {
                    PlayerRefGameObject.transform.LookAt(new Vector3(skillMoveTarget.x, PlayerRefGameObject.transform.position.y, skillMoveTarget.z));
                    if (reboundCurveTime < 0.7f)
                    {
                        if (skillMoveTarget.y > BodyHeight.transform.localPosition.y)
                        {
                            PlayerRefGameObject.transform.position = new Vector3(Mathf.Lerp(PlayerRefGameObject.transform.position.x, skillMoveTarget.x, reboundCurveTime), 
                                                                                 Mathf.Lerp(PlayerRefGameObject.transform.position.y, (skillMoveTarget.y - BodyHeight.transform.localPosition.y), reboundCurveTime), 
                                                                                 Mathf.Lerp(PlayerRefGameObject.transform.position.z, skillMoveTarget.z, reboundCurveTime));
                        } else
                        {
                            PlayerRefGameObject.transform.position = new Vector3(Mathf.Lerp(PlayerRefGameObject.transform.position.x, skillMoveTarget.x, reboundCurveTime), 
                                                                                 PlayerRefGameObject.transform.position.y, 
                                                                                 Mathf.Lerp(PlayerRefGameObject.transform.position.z, skillMoveTarget.z, reboundCurveTime));
                        }
                    } else
                    {
                        PlayerRefGameObject.transform.DOMoveY(0, 0.5f);
                    }
                } else
                {
                    if (reboundCurveTime < 0.7f && !IsBallOwner && reboundMove != Vector3.zero)
                    {
                        transform.position = new Vector3(transform.position.x + reboundMove.x * Time.deltaTime * 2 * Timer.timeScale, 
                                                         playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
                                                         transform.position.z + reboundMove.z * Time.deltaTime * 2 * Timer.timeScale);
                    } else
                        transform.position = new Vector3(transform.position.x + transform.forward.x * 0.05f, 
                                                         playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
                                                         transform.position.z + transform.forward.z * 0.05f);
                }
            } else
            {
                if (reboundCurveTime < 0.7f && !IsBallOwner && reboundMove != Vector3.zero)
                {
                    transform.position = new Vector3(transform.position.x, 
                                                     playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
                                                     transform.position.z);
                } else
                    transform.position = new Vector3(transform.position.x, 
                                                     playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
                                                     transform.position.z);
            }
            
            if (reboundCurveTime >= playerReboundCurve.LifeTime)
            {
                isRebound = false;
                isCheckLayerToReset = true;
            }
        } else
            isRebound = false;
    }

    private void CalculationPush()
    {
        if (!isPush)
            return;

        if (playerPushCurve != null)
        {
            pushCurveTime += Time.deltaTime * Timer.timeScale;

            if (pushCurveTime >= playerPushCurve.StartTime && pushCurveTime <= playerPushCurve.EndTime)
            {
                switch (playerPushCurve.Dir)
                {
                    case AniCurveDirection.Forward:
                        PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x + (PlayerRefGameObject.transform.forward.x * playerPushCurve.DirVaule * Timer.timeScale), 0, 
                                                           PlayerRefGameObject.transform.position.z + (PlayerRefGameObject.transform.forward.z * playerPushCurve.DirVaule));
                        break;
                    case AniCurveDirection.Back:
                        PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x + (PlayerRefGameObject.transform.forward.x * -playerPushCurve.DirVaule * Timer.timeScale), 0, 
                                                           PlayerRefGameObject.transform.position.z + (PlayerRefGameObject.transform.forward.z * -playerPushCurve.DirVaule));
                        break;
                }
            }
            
            if (pushCurveTime >= playerPushCurve.LifeTime)
                isPush = false;
        }
        
    }
    
    private void CalculationSteal()
    {
        if (!isSteal)
            return;
        
        if (playerStealCurve != null)
        {
            stealCurveTime += Time.deltaTime * Timer.timeScale;
            
            if (stealCurveTime >= playerStealCurve.StartTime)
            {
                if (GameController.Get.BallOwner != null)
                {
                    transform.DOMove((GameController.Get.BallOwner.transform.position + Vector3.forward * (-2)), playerStealCurve.LifeTime);
                    PlayerRefGameObject.transform.LookAt(new Vector3(GameController.Get.BallOwner.transform.position.x, PlayerRefGameObject.transform.position.y, GameController.Get.BallOwner.transform.position.z));
                    GameController.Get.BallOwner.AniState(EPlayerState.GotSteal);
                } else
                {
                    if (GameController.Get.Catcher != null)
                    {
                        transform.DOMove((GameController.Get.Catcher.transform.position + Vector3.forward * (-2)), playerStealCurve.LifeTime);
                        PlayerRefGameObject.transform.LookAt(new Vector3(GameController.Get.Catcher.transform.position.x, PlayerRefGameObject.transform.position.y, GameController.Get.Catcher.transform.position.z));
                        GameController.Get.Catcher.AniState(EPlayerState.GotSteal);
                    } else if (GameController.Get.Shooter != null)
                    {
                        transform.DOMove((GameController.Get.Shooter.transform.position + Vector3.forward * (-2)), playerStealCurve.LifeTime);
                        PlayerRefGameObject.transform.LookAt(new Vector3(GameController.Get.Shooter.transform.position.x, PlayerRefGameObject.transform.position.y, GameController.Get.Shooter.transform.position.z));
                        GameController.Get.Shooter.AniState(EPlayerState.GotSteal);
                    }
                }
                isSteal = false;
            }
        }
        
    }

    private void CalculationFall()
    {
        if (!isFall)
            return;
        
        if (playerFallCurve != null)
        {
            fallCurveTime += Time.deltaTime * Timer.timeScale;
            
            if (fallCurveTime >= playerFallCurve.StartTime && fallCurveTime <= playerFallCurve.EndTime)
            {
                switch (playerFallCurve.Dir)
                {
                    case AniCurveDirection.Forward:
                        PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x + (PlayerRefGameObject.transform.forward.x * playerFallCurve.DirVaule), 0, 
                                                           PlayerRefGameObject.transform.position.z + (PlayerRefGameObject.transform.forward.z * playerFallCurve.DirVaule));
                        break;
                    case AniCurveDirection.Back:
                        PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x + (PlayerRefGameObject.transform.forward.x * -playerFallCurve.DirVaule), 0, 
                                                           PlayerRefGameObject.transform.position.z + (PlayerRefGameObject.transform.forward.z * -playerFallCurve.DirVaule));
                        break;
                }
            }
            
            if (fallCurveTime >= playerFallCurve.LifeTime)
                isFall = false;
        }
        
    }

    private void CalculationPick()
    {
        if (!isPick)
            return;
        
        if (playerPickCurve != null)
        {
            pickCurveTime += Time.deltaTime * Timer.timeScale;
            
            if (pickCurveTime >= playerPickCurve.StartTime && pickCurveTime <= playerPickCurve.EndTime)
            {
                switch (playerPickCurve.Dir)
                {
                    case AniCurveDirection.Forward:
                        PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x + (PlayerRefGameObject.transform.forward.x * playerPickCurve.DirVaule), 0, 
                                                           PlayerRefGameObject.transform.position.z + (PlayerRefGameObject.transform.forward.z * playerPickCurve.DirVaule));
                        break;
                    case AniCurveDirection.Back:
                        PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x + (PlayerRefGameObject.transform.forward.x * -playerPickCurve.DirVaule), 0, 
                                                           PlayerRefGameObject.transform.position.z + (PlayerRefGameObject.transform.forward.z * -playerPickCurve.DirVaule));
                        break;
                }
            }
            
            if (pickCurveTime >= playerPickCurve.LifeTime)
                isPick = false;
        }
        
    }
    
    private void CalculationBlock()
    {
        if (!isBlock || Timer.timeScale == 0)
            return;

        if (playerBlockCurve != null)
        {
            blockCurveTime += Time.deltaTime * Timer.timeScale;

            if (playerBlockCurve.isSkill)
            {
                PlayerRefGameObject.transform.LookAt(new Vector3(skillMoveTarget.x, PlayerRefGameObject.transform.position.y, skillMoveTarget.z));
                if (blockCurveTime < 1f)
                {
                    if (GameController.Get.BallOwner == null)
                    {
                        if (isShootBlock)
                        {
                            PlayerRefGameObject.transform.position = new Vector3(Mathf.Lerp(PlayerRefGameObject.transform.position.x, skillMoveTarget.x, blockCurveTime), 
                                                                                 Mathf.Lerp(PlayerRefGameObject.transform.position.y, (skillMoveTarget.y - BodyHeight.transform.localPosition.y), blockCurveTime), 
                                                                                 Mathf.Lerp(PlayerRefGameObject.transform.position.z, skillMoveTarget.z, blockCurveTime));
                        }
                    } else
                    {
                        if (isDunkBlock && GameController.Get.GetDis(new Vector2(PlayerRefGameObject.transform.position.x, PlayerRefGameObject.transform.position.z),
                                                                    new Vector2(skillMoveTarget.x, skillMoveTarget.z)) > 1.5f)
                        {
                            PlayerRefGameObject.transform.position = new Vector3(Mathf.Lerp(PlayerRefGameObject.transform.position.x, skillMoveTarget.x, blockCurveTime), 
                                                                                 Mathf.Lerp(PlayerRefGameObject.transform.position.y, skillMoveTarget.y, blockCurveTime), 
                                                                                 Mathf.Lerp(PlayerRefGameObject.transform.position.z, skillMoveTarget.z, blockCurveTime));
                        }
                    }
                } else
                {
                    PlayerRefGameObject.transform.DOMoveY(0, 0.5f);
                }
                    
            } else
            {
                if (blockCurveTime < 1f)
                    PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x + (PlayerRefGameObject.transform.forward.x * 0.03f * Timer.timeScale), 
                                                                         playerBlockCurve.aniCurve.Evaluate(blockCurveTime), 
                                                                         PlayerRefGameObject.transform.position.z + (PlayerRefGameObject.transform.forward.z * 0.03f * Timer.timeScale));
                else
                    PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x, 
                                                                         playerBlockCurve.aniCurve.Evaluate(blockCurveTime), 
                                                                         PlayerRefGameObject.transform.position.z);
            }

            if (blockCurveTime >= playerBlockCurve.LifeTime)
            {
                isBlock = false;
                isShootBlock = false;
                isDunkBlock = false;
                isCheckLayerToReset = true;
            }
        }
    }

    private void CalculationShootJump()
    {
        if (isShootJump && playerShootCurve != null)
        {
            shootJumpCurveTime += Time.deltaTime * Timer.timeScale;
            switch (playerShootCurve.Dir)
            {
                case AniCurveDirection.Forward:
                    if (shootJumpCurveTime >= playerShootCurve.OffsetStartTime && shootJumpCurveTime < playerShootCurve.OffsetEndTime)
                        PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x + (PlayerRefGameObject.transform.forward.x * playerShootCurve.DirVaule * Timer.timeScale), 
                                                            playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), 
                                                           PlayerRefGameObject.transform.position.z + (PlayerRefGameObject.transform.forward.z * playerShootCurve.DirVaule * Timer.timeScale));
                    else
                        PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x, playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), PlayerRefGameObject.transform.position.z);
                    break;
                case AniCurveDirection.Back:
                    if (shootJumpCurveTime >= playerShootCurve.OffsetStartTime && shootJumpCurveTime < playerShootCurve.OffsetEndTime)
                        PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x + (PlayerRefGameObject.transform.forward.x * -playerShootCurve.DirVaule * Timer.timeScale), 
                                                            playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), 
                                                           PlayerRefGameObject.transform.position.z + (PlayerRefGameObject.transform.forward.z * -playerShootCurve.DirVaule * Timer.timeScale));
                    else
                        PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x, playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), PlayerRefGameObject.transform.position.z);
                    break;

                default : 
                    PlayerRefGameObject.transform.position = new Vector3(PlayerRefGameObject.transform.position.x, playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), PlayerRefGameObject.transform.position.z);
                    break;

            }

            //Debug.Log("H :: " + gameObject.transform.position);

            if (shootJumpCurveTime >= playerShootCurve.LifeTime)
            {
                isShootJump = false;
                shootJumpCurveTime = 0;
//                DelActionFlag(ActionFlag.IsDribble);
//                DelActionFlag(ActionFlag.IsRun);
                isCheckLayerToReset = true;
            }
        }
    }

    private void CalculationAnimatorSmoothSpeed()
    {
        if (smoothDirection != 0)
        {
            if (smoothDirection == 1)
            {
                animationSpeed += 0.1f;
                if (animationSpeed >= 1)
                {
                    animationSpeed = 1;
                    smoothDirection = 0;
                }
            } else
            {
                animationSpeed -= 0.1f;
                if (animationSpeed <= 0)
                {
                    animationSpeed = 0;
                    smoothDirection = 0;
                }
            }
            AnimatorControl.SetFloat("MoveSpeed", animationSpeed);
        }
    }

    private bool isCheckLayerToReset = false;
    private bool isStartCheckLayer = false;
    private bool isStartJump = false;

    private void CalculationPlayerHight()
    {
        AnimatorControl.SetFloat("CrtHight", PlayerRefGameObject.transform.localPosition.y);

        /*if (isCheckLayerToReset)
        {
            if (gameObject.transform.localPosition.y > 0.2f)
                isStartCheckLayer = true;

            if (isStartCheckLayer && isTouchPalyer <= 0 && gameObject.transform.localPosition.y <= 0)
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
                isCheckLayerToReset = false;
                isStartCheckLayer = false;
            }
        }*/

        //Effect Handel
        if (PlayerRefGameObject.transform.localPosition.y > 0.5f)
        {
            isStartJump = true;
        }

        if (isStartJump && PlayerRefGameObject.transform.localPosition.y <= 0.1f)
        {

            EffectManager.Get.PlayEffect("JumpDownFX", PlayerRefGameObject.transform.position, null, null, 3f);

            isStartJump = false;
        }
    }

    public void DebugTool()
    {
        if (!GameStart.Get.IsDebugAnimation)
            return;

        //LayerCheck
        if (PlayerRefGameObject.transform.localPosition.y > 0.2f && LayerMgr.Get.CheckLayer(PlayerRefGameObject, ELayer.Player))
        {
            LogMgr.Get.AnimationError((int)Team * 3 + Index, "Error Layer: " + PlayerRefGameObject.name + " . crtState : " + crtState);
        }

        //IdleAirCheck
//      if (PlayerTransform.localPosition.y > 0.2f && crtState == EPlayerState.Idle && situation != EGameSituation.End)
//      {
//          LogMgr.Get.AnimationError((int)Team * 3 + Index, PlayerGameObject.name + " : Error State : Idle in the Air ");
//      }

        //Idle ballowner
        if (crtState == EPlayerState.Idle && IsBallOwner && GameController.Get.Situation != EGameSituation.End)
        {
            LogMgr.Get.AnimationError((int)Team * 3 + Index, PlayerRefGameObject.name + " : Error State: Idle BallOWner");
        }

    }

    public void OnJoystickStart(MovingJoystick move)
    {
        yAxizOffset = CameraMgr.Get.CourtCamera.transform.eulerAngles.y - 90;
    }

    public void OnJoystickMove(MovingJoystick move, EPlayerState ps)
    {
        if (Timer.timeScale == 0)
            return;

        int moveKind = 0;
        float calculateSpeed = 1;

        if (CanMove || stop || HoldBallCanMove)
        {
            if (IsFall && GameStart.Get.IsDebugAnimation)
            {
                LogMgr.Get.LogError("CanMove : " + CanMove);
                LogMgr.Get.LogError("stop : " + stop);
                LogMgr.Get.LogError("HoldBallCanMove : " + HoldBallCanMove);
            }

            if (situation == EGameSituation.AttackGamer || situation == EGameSituation.AttackNPC || 
                GameStart.Get.TestMode != EGameTest.None)
            {
                if ((Mathf.Abs(move.joystickAxis.y) > 0 || Mathf.Abs(move.joystickAxis.x) > 0) &&
                    !(GameController.Get.CoolDownCrossover == 0 && !IsDefence && 
                    DoPassiveSkill(ESkillSituation.MoveDodge)))
                {
                    isMoving = true;
                    if (!isJoystick)
                        moveStartTime = Time.time + GameConst.DefMoveTime;

                    SetManually();
                    animationSpeed = Vector2.Distance(new Vector2(move.joystickAxis.x, 0), new Vector2(0, move.joystickAxis.y));
                    if (!IsPass)
                    {
                        float angle = move.Axis2Angle(true);
                        float a = 90 + yAxizOffset;
                        Vector3 rotation = new Vector3(0, angle + a, 0);
                        transform.rotation = Quaternion.Euler(rotation);
                    }

                    if (animationSpeed <= MoveMinSpeed)
                        moveKind = 0;
                    else
                    {
                        if (mMovePower == 0)
                            moveKind = 2;
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
                            } else
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
                            } else
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
//                  Debug.Log("MoveKind : " + moveKind);
                    if (GameStart.Get.TestMode == EGameTest.Skill || GameStart.Get.TestMode == EGameTest.PassiveSkill)
                        calculateSpeed = GameConst.AttackSpeedup;
                    translate = Vector3.forward * Time.deltaTime * Attr.SpeedValue * calculateSpeed * Timer.timeScale;
                    transform.Translate(translate); 
                    transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                    AniState(ps);
                }        
            }
        }
    }

    public void OnJoystickMoveEnd(MovingJoystick move, EPlayerState ps)
    {
        if (CanMove && 
            situation != EGameSituation.InboundsGamer && situation != EGameSituation.GamerPickBall && 
            situation != EGameSituation.InboundsNPC && situation != EGameSituation.NPCPickBall)
        {
            SetManually();
            isJoystick = false;
            isSpeedup = false;

            if (crtState != ps)
                AniState(ps);

            if (crtState == EPlayerState.Dribble0)
            {
                if (situation == EGameSituation.AttackGamer)
                    RotateTo(CourtMgr.Get.ShootPoint [0].transform.position.x, CourtMgr.Get.ShootPoint [0].transform.position.z);
                else 
                if (situation == EGameSituation.AttackNPC)
                    RotateTo(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);
            }
        }

        isMoving = false;
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
            } else
            {
                Vector3 aP1 = data.DefPlayer.transform.position;
                Vector3 aP2 = CourtMgr.Get.Hood [data.DefPlayer.Team.GetHashCode()].transform.position;
                result = GetStealPostion(aP1, aP2, data.DefPlayer.Index);
                if (Vector2.Distance(result, new Vector2(PlayerRefGameObject.transform.position.x, PlayerRefGameObject.transform.position.z)) <= GameConst.StealPushDistance)
                {
                    if (Math.Abs(GetAngle(data.DefPlayer.transform, this.transform)) >= 30 && 
                        Vector3.Distance(aP2, DefPlayer.transform.position) <= GameConst.TreePointDistance + 3)
                    {
                        resultBool = true;
                    } else
                    {
                        result.x = PlayerRefGameObject.transform.position.x;
                        result.y = PlayerRefGameObject.transform.position.z;
                    }
                } else
                    resultBool = true;
            }
        } else if (data.FollowTarget != null)
        {
            result.x = data.FollowTarget.position.x;
            result.y = data.FollowTarget.position.z;

            if (Vector2.Distance(result, new Vector2(PlayerRefGameObject.transform.position.x, PlayerRefGameObject.transform.position.z)) <= MoveCheckValue)
            {
                result.x = PlayerRefGameObject.transform.position.x;
                result.y = PlayerRefGameObject.transform.position.z;
            } else
                resultBool = true;
        } else
        {
            result = data.Target;
            resultBool = true;
        }

        return resultBool;
    }
    
    private void moveTo(TMoveData data, bool first = false)
    {
//        if((CanMove || (AIing && HoldBallCanMove)) && WaitMoveTime == 0 && 
        if ((CanMove || (AIing && HoldBallCanMove)) && CantMoveTimer.IsOff() && 
            GameStart.Get.TestMode != EGameTest.Block)
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
                    // 移動距離很短 or 不移動, 球員又是在防守狀態.
//                    WaitMoveTime = 0;
                    CantMoveTimer.Clear();
                    if (data.DefPlayer != null)
                    {
                        dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint [data.DefPlayer.Team.GetHashCode()].transform.position);
                        
                        if (data.LookTarget != null)
                        {
                            if (Vector3.Distance(this.transform.position, data.DefPlayer.transform.position) <= GameConst.StealPushDistance)
                                RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                            else 
                            if (!doMove)
                                RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                            else if (dis > GameConst.TreePointDistance + 4 && data.DefPlayer.AIing &&
//                                    (data.DefPlayer.WaitMoveTime == 0 || data.DefPlayer.TargetPosNum > 0))
                                (data.DefPlayer.CantMoveTimer.IsOff() || data.DefPlayer.TargetPosNum > 0))
                                RotateTo(MoveTarget.x, MoveTarget.y);
                            else
                                RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                        } else
                            RotateTo(MoveTarget.x, MoveTarget.y);
                    } else
                    {
                        if (data.LookTarget == null)
                            RotateTo(MoveTarget.x, MoveTarget.y);
                        else
                            RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                    }
                    
                    AniState(EPlayerState.Defence0);                          
                } else
                {
                    // 移動距離很短 or 不移動, 球員又是在進攻狀態.
                    if (!IsBallOwner)
                        AniState(EPlayerState.Idle);
                    else if (situation == EGameSituation.InboundsGamer || situation == EGameSituation.InboundsNPC)
                        AniState(EPlayerState.Dribble0);
                    
                    if (first || GameStart.Get.TestMode == EGameTest.Edit)
//                        WaitMoveTime = 0;
                        CantMoveTimer.Clear();
                    else if ((situation == EGameSituation.AttackGamer || situation == EGameSituation.AttackNPC) && 
                        GameController.Get.BallOwner && UnityEngine.Random.Range(0, 3) == 0)
                    {
                        // 目前猜測這段程式碼的功能是近距離防守時, 避免防守者不斷的轉向.
                        // 因為當初寫這段程式碼的時候, AI 做決策其實是 1 秒 30 次以上.
                        // 所以當 AI 做防守邏輯的時候, 會 1 秒下 30 的命令, 命令跑到某位球員的旁邊.
                        // 就會造成防守球員會一直的轉向.(因為距離很近的時候, 對方移動一點距離, 防守者就必須轉向很多度
                        // , 才可以正確的面相對方)
                        dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint [Team.GetHashCode()].transform.position);
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
                            RotateTo(CourtMgr.Get.ShootPoint [0].transform.position.x, CourtMgr.Get.ShootPoint [0].transform.position.z);
                        else
                            RotateTo(CourtMgr.Get.ShootPoint [1].transform.position.x, CourtMgr.Get.ShootPoint [1].transform.position.z);
                        
                        if (data.Shooting && AIing)
                            GameController.Get.DoShoot();
                    } else
                    {
                        if (data.LookTarget == null)
                        {
                            if (GameController.Get.BallOwner != null)
                                RotateTo(GameController.Get.BallOwner.transform.position.x, GameController.Get.BallOwner.transform.position.z);
                            else
                            {
                                if (Team == ETeamKind.Self)
                                    RotateTo(CourtMgr.Get.ShootPoint [0].transform.position.x, CourtMgr.Get.ShootPoint [0].transform.position.z);
                                else
                                    RotateTo(CourtMgr.Get.ShootPoint [1].transform.position.x, CourtMgr.Get.ShootPoint [1].transform.position.z);
                            }
                        } else
                            RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                        
                        if (data.Catcher)
                        {
                            if ((situation == EGameSituation.AttackGamer || situation == EGameSituation.AttackNPC))
                            {
                                if (GameController.Get.Pass(this, false, false, true))
                                    NeedShooting = data.Shooting;
                            }
                        }
                    }
                }
                
                if (data.MoveFinish != null)
                    data.MoveFinish(this, data.Speedup);

                // 移動到非常接近 target, 所以刪除這筆, 接著移動到下一個 target.
                if (moveQueue.Count > 0)
                {
                    moveQueue.Dequeue();
//                    Debug.LogFormat("moveTo(), moveQueue.Dequeue()");
                }
            } else if ((IsDefence == false && MoveTurn >= 0 && MoveTurn <= 5) && 
                GameController.Get.BallOwner != null)
            {
                // 這段應該是不做移動, 只做轉身, 然後用 6 個 frame 做轉身.
                MoveTurn++;
                RotateTo(MoveTarget.x, MoveTarget.y);
                if (MoveTurn == 1)
                    moveStartTime = Time.time + GameConst.DefMoveTime;           
            } else
            {
                if (IsDefence)
                {
                    // 防守移動.
                    if (data.DefPlayer != null)
                    {
                        dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint [data.DefPlayer.Team.GetHashCode()].transform.position);
                        
                        if (dis <= GameConst.TreePointDistance + 4 || Vector3.Distance(transform.position, data.LookTarget.position) <= 1.5f)
                            RotateTo(data.LookTarget.position.x, data.LookTarget.position.z);
                        else
                            RotateTo(MoveTarget.x, MoveTarget.y);

                        if (GetAngle(new Vector3(MoveTarget.x, 0, MoveTarget.y)) >= 90)
                            AniState(EPlayerState.Defence1);
                        else
                            AniState(EPlayerState.RunningDefence);
                    } else
                    {
                        RotateTo(MoveTarget.x, MoveTarget.y);
                        AniState(EPlayerState.Run0);
                    }
                    
                    isMoving = true;
                    if (mMovePower > 0 && canSpeedup && this != GameController.Get.Joysticker && !IsTee)
                    {
                        setSpeed(1, 0);
                        transform.position = Vector3.MoveTowards(transform.position, 
                                                new Vector3(MoveTarget.x, 0, MoveTarget.y), 
                                                Time.deltaTime * GameConst.DefSpeedup * Attr.SpeedValue);
                        isSpeedup = true;
                    } else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, 
                            new Vector3(MoveTarget.x, 0, MoveTarget.y), 
                            Time.deltaTime * GameConst.DefSpeedNormal * Attr.SpeedValue);
                        isSpeedup = false;
                    }
                } else
                {
                    // 進攻移動.
                    RotateTo(MoveTarget.x, MoveTarget.y);                   
                    isMoving = true;

                    if (IsBallOwner)
                    {
                        // 持球者移動.
                        if (data.Speedup && mMovePower > 0)
                        {
                            // 持球者加速移動.(因為球員已經轉身了, 所以往 forward 移動就可以了)
                            setSpeed(1, 0);
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedup * Attr.SpeedValue);
                            AniState(EPlayerState.Dribble2);
                            isSpeedup = true;
                        } else
                        {
                            // 持球者一般移動.
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedNormal * Attr.SpeedValue);
                            AniState(EPlayerState.Dribble1);
                            isSpeedup = false;
                        }
                    } else
                    {
                        // 未持球者移動.
                        if (data.Speedup && mMovePower > 0)
                        {
                            setSpeed(1, 0);
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedup * Attr.SpeedValue);
                            AniState(EPlayerState.Run1);
                            isSpeedup = true;
                        } else
                        {
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedNormal * Attr.SpeedValue);
                            AniState(EPlayerState.Run0);
                            isSpeedup = false;
                        }
                    }
                }
            } 
        } else
            isMoving = false;
    }

    public void RotateTo(float lookAtX, float lookAtZ)
    {
        if (isBlock || isSkillShow)
            return;

        PlayerRefGameObject.transform.LookAt(new Vector3(lookAtX, PlayerRefGameObject.transform.position.y, lookAtZ));

//      Debug.Log ("Roatte To .GameObject : " + PlayerGameObect.name);
//        transform.rotation = Quaternion.Lerp(transform.rotation, 
//                             Quaternion.LookRotation(new Vector3(lookAtX, transform.localPosition.y, lookAtZ) - 
//            transform.localPosition), time * Time.deltaTime);

//        Vector3 lookAtPos = new Vector3(lookAtX, PlayerTransform.position.y, lookAtZ);
//        Vector3 relative = PlayerTransform.InverseTransformPoint(lookAtPos);
//        float mangle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

//        if ((mangle > 15 && mangle < 180) || (mangle < -15 && mangle > -180))
//        {
//            PlayerTransform.DOLookAt(lookAtPos, 0.1f);
//        }
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
        //dir : 1 ++, -1 --, -2 : not smooth,  
//        if (dir == 0)
        AnimatorControl.SetFloat("MoveSpeed", value);
//        else
//        if (dir != -2)
//            smoothDirection = dir;
    }

    private void AddActionFlag(EActionFlag Flag)
    {
        GameFunction.Add_ByteFlag(Flag.GetHashCode(), ref PlayerActionFlag);
        AnimatorControl.SetBool(Flag.ToString(), true);
    }

    public void DelActionFlag(EActionFlag Flag)
    {
        GameFunction.Del_ByteFlag(Flag.GetHashCode(), ref PlayerActionFlag);
        AnimatorControl.SetBool(Flag.ToString(), false);
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

            for (int i = 0; i < PlayerActionFlag.Length; i++)
                PlayerActionFlag [i] = 0;

            if (!IsBallOwner)
                AniState(EPlayerState.Idle);
            else
                AniState(EPlayerState.Dribble0);

            if (clearMove)
            {
                moveQueue.Clear();
//                Debug.Log("ResetFlag(), moveQueue.Clear().");
            }

//            WaitMoveTime = 0;
            CantMoveTimer.Clear();
            NeedShooting = false;
            isJoystick = false; 
            isMoving = false;
            isSpeedup = false;
            canSpeedup = true;
        } else
            NeedResetFlag = true;
    }

    public void ResetSkill()
    {
        skillController.Reset();
    }

    public void ClearMoveQueue()
    {
        moveQueue.Clear();
//        Debug.Log("ClearMoveQueue()");
    }

    private bool IsPassAirMoment = false;

    public bool CanUseState(EPlayerState state)
    {
//      Debug.Log ("Check ** " +gameObject.name + ".CrtState : " + crtState + "  : state : " + state);
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
                if (IsBallOwner && !IsPass && !IsPickBall && !IsAllShoot && (crtState == EPlayerState.HoldBall || IsDribble))
                {
                    return true;
                }
                break;

            case EPlayerState.Pass4:
//          if (IsBallOwner && !IsLayup && !IsDunk && (crtState == EPlayerState.Shoot0 || crtState == EPlayerState.Shoot2) && !GameController.Get.Shooter && IsPassAirMoment && !IsPass)
                if (IsBallOwner && !IsLayup && !IsDunk && IsShoot && !GameController.Get.Shooter && IsPassAirMoment && !IsPass)
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
                if (Vector3.Distance(CourtMgr.Get.ShootPoint [Team.GetHashCode()].transform.position, PlayerRefGameObject.transform.position) < canDunkDis)
                    return true;
                break;

            case EPlayerState.Alleyoop:
                if (crtState != EPlayerState.Alleyoop && !IsBallOwner && (GameStart.Get.TestMode == EGameTest.Alleyoop || situation.GetHashCode() == (Team.GetHashCode() + 3)))
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
                if (IsRebound && crtState != EPlayerState.TipIn)
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
                if (!IsTee && IsBallOwner && (crtState == EPlayerState.Dribble0 || crtState == EPlayerState.Dribble1 || crtState == EPlayerState.HoldBall))
                    return true;
                break;

            case EPlayerState.Fall0:
            case EPlayerState.Fall1:
            case EPlayerState.Fall2:
            case EPlayerState.KnockDown0:
            case EPlayerState.KnockDown1:

                if (!IsTee && !IsFall && !IsUseSkill)
//                if (!IsTee && crtState != state && crtState != EPlayerState.Elbow && 
//                    (crtState == EPlayerState.Dribble0 || crtState == EPlayerState.Dribble1 || crtState == EPlayerState.Dribble2 || crtState == EPlayerState.HoldBall || IsDunk ||
//                    crtState == EPlayerState.Idle || crtState == EPlayerState.Run0 || crtState == EPlayerState.Run1 || crtState == EPlayerState.Defence0 || crtState == EPlayerState.Defence1 || 
//                    crtState == EPlayerState.RunningDefence) && Invincible == 0)
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
                if (IsBallOwner && !IsPickBall && !IsPass && !IsAllShoot)
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
                if (crtState != state)
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
                if (CanMove && !IsBallOwner && !IsAllShoot && situation != EGameSituation.GamerPickBall && situation != EGameSituation.NPCPickBall && situation != EGameSituation.InboundsGamer && situation != EGameSituation.InboundsNPC)
                    return true;
                break;

            case EPlayerState.Buff20:
            case EPlayerState.Buff21:
                if (!IsBuff)
                    return true;
                break;

            case EPlayerState.Idle:
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
                if (!IsShow)
                    return true;
                break;
        }
        
        return false;
    }
    
    public bool IsTee
    { 
        get
        {
            return (situation == EGameSituation.InboundsGamer || situation == EGameSituation.GamerPickBall || situation == EGameSituation.InboundsNPC || situation == EGameSituation.NPCPickBall);
        }
    }
    
    public bool IsKinematic
    {
        get{ return PlayerRigidbody.isKinematic;}
        set
        {
            PlayerRigidbody.isKinematic = value;
            Timer.rigidbody.isKinematic = value;
        }
    }

    public bool AniState(EPlayerState state, Vector3 lookAtPoint)
    {
        if (AniState(state))
        {
            RotateTo(lookAtPoint.x, lookAtPoint.z);
            if (GameStart.Get.TestMode == EGameTest.Pass)
                LogMgr.Get.Log("name:" + PlayerRefGameObject.name + "Rotate");

            return true;
        } else 
            return false;
    }

    private void AddTrigger(EAnimatorState state, int stateNo)
    {
        AnimatorControl.SetInteger("StateNo", stateNo);
        switch (state)
        {
            case EAnimatorState.Block:
            case EAnimatorState.Buff:
            case EAnimatorState.BlockCatch:
            case EAnimatorState.Catch:
            case EAnimatorState.Dunk:
            case EAnimatorState.End:
            case EAnimatorState.Elbow:
            case EAnimatorState.FakeShoot:
            case EAnimatorState.Fall:
            case EAnimatorState.KnockDown:
            case EAnimatorState.Intercept:
            case EAnimatorState.Pass:
            case EAnimatorState.Push:
            case EAnimatorState.Pick:
            case EAnimatorState.Steal:
            case EAnimatorState.GotSteal:
            case EAnimatorState.Shoot:
            case EAnimatorState.Layup:
            case EAnimatorState.Rebound:
            case EAnimatorState.JumpBall:
            case EAnimatorState.TipIn:
                ClearAnimatorFlag();
                AnimatorControl.SetTrigger(string.Format("{0}Trigger", state.ToString()));
                break;

            case EAnimatorState.Idle:
                ClearAnimatorFlag();
                break;
            case EAnimatorState.Defence:
                ClearAnimatorFlag(EActionFlag.IsDefence);
                break;
            case EAnimatorState.Dribble:
                ClearAnimatorFlag(EActionFlag.IsDribble);
                break;
            case EAnimatorState.HoldBall:
                ClearAnimatorFlag(EActionFlag.IsHoldBall);
                break;
            case EAnimatorState.Run:
                ClearAnimatorFlag(EActionFlag.IsRun);
                break;
            case EAnimatorState.MoveDodge:
            case EAnimatorState.Show:
                AnimatorControl.SetInteger("StateNo", stateNo);
                AnimatorControl.SetTrigger(string.Format("{0}Trigger", state.ToString()));
                break;
        }
    }

    private void InitAnimatorCurve(EAnimatorState state, int stateNo)
    {
        string curveName = string.Format("{0}{1}", state.ToString(), stateNo);
        Debug.Log("curveName : " + curveName);

        switch (state)
        {
            case EAnimatorState.Block:
                if (playerBlockCurve == null || (playerBlockCurve != null && playerBlockCurve.Name != curveName))
                {
                    playerBlockCurve = null;
                    for (int i = 0; i < aniCurve.Block.Length; i++)
                        if (aniCurve.Block [i].Name == curveName)
                        {
                            playerBlockCurve = aniCurve.Block [i];
                        }
                }
                blockCurveTime = 0;
                isBlock = true;
                break;
            case EAnimatorState.Dunk:
                if (playerDunkCurve == null || (playerDunkCurve != null && playerDunkCurve.Name != curveName))
                {
                    for (int i = 0; i < aniCurve.Dunk.Length; i++)
                        if (aniCurve.Dunk [i].Name == curveName)
                        {
                            playerDunkCurve = aniCurve.Dunk [i];
                        }
                }
                IsAnimatorMove = false;
                isDunk = true;
                dunkCurveTime = 0;

                break;

            case EAnimatorState.Fall:
                if (playerFallCurve == null || (playerFallCurve != null && playerFallCurve.Name != curveName))
                {
                    playerFallCurve = null;

                    for (int i = 0; i < aniCurve.Fall.Length; i++)
                        if (curveName == aniCurve.Fall [i].Name)
                            playerFallCurve = aniCurve.Fall [i];
                }
                fallCurveTime = 0;
                isFall = true;
                break;
            case EAnimatorState.Push:
                if (playerPushCurve == null || (playerPushCurve != null && playerPushCurve.Name != curveName))
                {
                    playerPushCurve = null;
                    for (int i = 0; i < aniCurve.Push.Length; i++)
                        if (aniCurve.Push [i].Name == curveName)
                            playerPushCurve = aniCurve.Push [i];
                }

                pushCurveTime = 0;
                isPush = true;
                break;

            case EAnimatorState.Pick:
                if (playerPickCurve == null || (playerPickCurve != null && playerPickCurve.Name != curveName))
                {
                    playerPickCurve = null;
                    for (int i = 0; i < aniCurve.PickBall.Length; i++)
                        if (aniCurve.PickBall [i].Name == curveName)
                        {
                            playerPickCurve = aniCurve.PickBall [i];
                        }
                }
                pickCurveTime = 0;
                isPick = true;
                break;
            case EAnimatorState.Steal:
                if (playerStealCurve == null || (playerStealCurve != null && playerStealCurve.Name != curveName))
                {
                    playerStealCurve = null;
                    for (int i = 0; i < aniCurve.Steal.Length; i++)
                        if (aniCurve.Steal [i].Name == curveName)
                            playerStealCurve = aniCurve.Steal [i];
                }
                stealCurveTime = 0;
                isSteal = true;
                break;
            case EAnimatorState.Shoot:
                if (playerShootCurve == null || (playerShootCurve != null && playerShootCurve.Name != curveName))
                {
                    for (int i = 0; i < aniCurve.Shoot.Length; i++)
                        if (aniCurve.Shoot [i].Name == curveName)
                            playerShootCurve = aniCurve.Shoot [i];
                }
            
                shootJumpCurveTime = 0;
                isShootJump = true;
            
                break;
            case EAnimatorState.Layup:
                if (playerLayupCurve == null || (playerLayupCurve != null && playerLayupCurve.Name != curveName))
                {
                    playerLayupCurve = null;
                    for (int i = 0; i < aniCurve.Layup.Length; i++)
                        if (aniCurve.Layup [i].Name == curveName)
                            playerLayupCurve = aniCurve.Layup [i];
                }

                layupCurveTime = 0;
                isLayup = true;
                isLayupZmove = false;
                break;

            case EAnimatorState.Rebound:
                if (playerLayupCurve == null || (playerLayupCurve != null && playerLayupCurve.Name != curveName))
                {
                    playerReboundCurve = null;
                    for (int i = 0; i < aniCurve.Rebound.Length; i++)
                        if (aniCurve.Rebound [i].Name == curveName)
                            playerReboundCurve = aniCurve.Rebound [i];
                }
                reboundCurveTime = 0;
                isRebound = true;
                break;
        }

//      if(curveName != string.Empty && !isFindCurve)
//          DebugAnimationCurve(curveName);
    }

    private void BlockStateHandle(int stateNo)
    {
        switch (stateNo)
        {
            case 20:
                GameRecord.Block ++;
                if (GameController.Get.BallState == EBallState.CanBlock)
                    isShootBlock = true;
                else if (GameController.Get.BallState == EBallState.CanDunkBlock)
                    isDunkBlock = true;
                break;
        }
        StartSkillCamera(stateNo);
        SetShooterLayer();
        
        PlayerRigidbody.useGravity = false;
        IsKinematic = true;
        
        if (GameController.Get.BallOwner == null)
        {
            skillMoveTarget = CourtMgr.Get.RealBall.transform.position;
        } else
        {
            skillMoveTarget = GameController.Get.BallOwner.transform.position;
        }
        
        AddTrigger(EAnimatorState.Block, stateNo);
        isCanCatchBall = false;
        GameRecord.BlockLaunch++;
    }

    private void BlockCatchStateHandle(int stateNo)
    {
        PlayerRigidbody.useGravity = false;
        IsKinematic = true;
        AddTrigger(EAnimatorState.BlockCatch, stateNo);
        IsPerfectBlockCatch = false;
        isCanCatchBall = false;
    }

    public void BuffStateHandle(int stateNo)
    {
        StartSkillCamera(stateNo);
        AddTrigger(EAnimatorState.Buff, stateNo);
    }

    public void CatchStateHandle(int stateNo)
    {
        setSpeed(0, -1);
        AddTrigger(EAnimatorState.Catch, stateNo);
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
    
        AddTrigger(EAnimatorState.Defence, stateNo);
    }

    public void DunkStateHandle(int stateNo)
    {
        StartSkillCamera(stateNo);
        PlayerRigidbody.useGravity = false;
        IsKinematic = true;
        AddTrigger(EAnimatorState.Dunk, stateNo);
        isCanCatchBall = false;

        InitAnimatorCurve(EAnimatorState.Dunk, stateNo);

        SetShooterLayer();
        CourtMgr.Get.SetBallState(EPlayerState.Dunk0, this);
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
            
        AddTrigger(EAnimatorState.Dribble, stateNo);
        CourtMgr.Get.SetBallState(EPlayerState.Dribble0, this);
        isCanCatchBall = false;
        IsFirstDribble = false;
    }

    public void ElbowStateHandle(int stateNo)
    {
        switch (stateNo)
        {
            case 20:
            case 21:
                GameRecord.Elbow ++;
                break;
        }
        StartSkillCamera(stateNo);
        PlayerRigidbody.mass = 5;
        AddTrigger(EAnimatorState.Elbow, stateNo);
        isCanCatchBall = false;
        GameRecord.ElbowLaunch++;
    }

    public void FakeShootStateHandle(int stateNo)
    {
        if (IsBallOwner)
        {
            PlayerRigidbody.mass = 5;
            AddTrigger(EAnimatorState.FakeShoot, stateNo);
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
            CourtMgr.Get.SetBallState(EPlayerState.KnockDown0);
        }
        if (IsDunk)
        {
            isDunk = false;
            IsAnimatorMove = false;
            PlayerRefGameObject.transform.DOKill();
            PlayerRefGameObject.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.Linear);
        }
        SetShooterLayer();
        
        isShootJump = false;
        AddTrigger(EAnimatorState.KnockDown, stateNo);
        isCanCatchBall = false;
    }

    public void FallStateHandle(int stateNo)
    {
        SetShooterLayer();
        InitAnimatorCurve(EAnimatorState.Fall, stateNo);
        isDunk = false;
        IsAnimatorMove = false;
        isShootJump = false;
        AddTrigger(EAnimatorState.Fall, stateNo);
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
            }
            PlayerRigidbody.useGravity = false;
            IsKinematic = true;
            InitAnimatorCurve(EAnimatorState.Shoot, stateNo);
            SetShooterLayer();
            AddTrigger(EAnimatorState.Shoot, stateNo);
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
            PlayerRigidbody.useGravity = false;
            IsKinematic = true;
            InitAnimatorCurve(EAnimatorState.Layup, stateNo);
            SetShooterLayer();
            AddTrigger(EAnimatorState.Layup, stateNo);
            isCanCatchBall = false;
        }
    }

    public bool AniState(EPlayerState state)
    {
        if (!CanUseState(state))
            return false;

        TAnimatorItem nextState = AnimatorMgr.Get.GetAnimatorStateType(state);

        bool Result = false;
        int stateNo = 0;

        PlayerRigidbody.mass = 0.1f;
        PlayerRigidbody.useGravity = true;
        IsKinematic = false;
        DribbleTime = 0;
        IsUseSkill = false;
        if (!isUsePass)
            isCanCatchBall = true;

        if (LayerMgr.Get.CheckLayer(PlayerRefGameObject, ELayer.Shooter))
            LayerMgr.Get.SetLayer(PlayerRefGameObject, ELayer.Player);

        if (GameStart.Get.IsDebugAnimation)
            Debug.LogWarning(PlayerRefGameObject.name + ".CrtState : " + crtState + ", NextState : " + state + ", Situation : " + GameController.Get.Situation);

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
            case EAnimatorState.Alleyoop:
                DunkStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.Dribble:
                DribbleStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.End:
                AddTrigger(EAnimatorState.End, nextState.StateNo);
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
                AddTrigger(EAnimatorState.HoldBall, nextState.StateNo); 
                isCanCatchBall = false;
                Result = true;
                break;
            case EAnimatorState.Idle:
                PlayerRigidbody.mass = 5;
                setSpeed(0, -1);
                AddTrigger(EAnimatorState.Idle, nextState.StateNo); 
                isMoving = false;
                Result = true;
                break;
            case EAnimatorState.Intercept:
                AddTrigger(EAnimatorState.Intercept, nextState.StateNo);
                Result = true;
                break;
            case EAnimatorState.MoveDodge:
                AddTrigger(EAnimatorState.MoveDodge, nextState.StateNo);
                OnUICantUse(this);
                if (moveQueue.Count > 0)
                    moveQueue.Dequeue();
                Result = true;
                break;
            case EAnimatorState.Pass:
                isCanCatchBall = false;
                AddTrigger(EAnimatorState.Pass, nextState.StateNo);
                if (nextState.StateNo == 5 || nextState.StateNo == 6 || nextState.StateNo == 7 || nextState.StateNo == 8 || nextState.StateNo == 9)
                    isUsePass = true;
                PlayerRigidbody.mass = 5;
                GameRecord.Pass++;
                Result = true;
                break;
            case EAnimatorState.Push:
                if (nextState.StateNo == 20)
                    GameRecord.Push ++;
                InitAnimatorCurve(EAnimatorState.Push, nextState.StateNo);
                StartSkillCamera(stateNo);
                AddTrigger(EAnimatorState.Push, nextState.StateNo);
                GameRecord.PushLaunch++;
                Result = true;
                break;
            case EAnimatorState.Pick:
                if (nextState.StateNo == 2)
                {
                    InitAnimatorCurve(EAnimatorState.Pick, nextState.StateNo);
                    GameRecord.SaveBallLaunch++;
                }
                AddTrigger(EAnimatorState.Pick, stateNo);
                Result = true;
                break;
            case EAnimatorState.Run:
                if (!isJoystick)
                    setSpeed(1, 1); 
                if (nextState.StateNo == 1)
                {
                    DashEffectEnable(true);
                }
                AddTrigger(EAnimatorState.Run, nextState.StateNo);
                Result = true;
                break;
            case EAnimatorState.Steal:
                if (nextState.StateNo == 20)
                {
                    GameRecord.Steal ++;
                    InitAnimatorCurve(EAnimatorState.Steal, nextState.StateNo);
                }
                StartSkillCamera(stateNo);
                PlayerRigidbody.mass = 5;
                AddTrigger(EAnimatorState.Steal, nextState.StateNo);
                isCanCatchBall = false;
                GameRecord.StealLaunch++;
                Result = true;
                break;
            case EAnimatorState.GotSteal:
                AddTrigger(EAnimatorState.GotSteal, nextState.StateNo);
                isCanCatchBall = false;
                Result = true;
                break;
            case EAnimatorState.Shoot:
                ShootStateHandle(nextState.StateNo);
                Result = true;
                break;
            case EAnimatorState.Show:
                AddTrigger(EAnimatorState.Show, nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.Layup:
                LayupStateHandle(nextState.StateNo);
                Result = true;
                break;

            case EAnimatorState.Rebound:
                if (nextState.StateNo == 20)
                    GameRecord.Rebound ++;

                PlayerRigidbody.useGravity = false;
                IsKinematic = true;
            
                StartSkillCamera(stateNo);
                skillMoveTarget = CourtMgr.Get.RealBall.transform.position;
                if (InReboundDistance)
                {
                    reboundMove = CourtMgr.Get.RealBall.transform.position - transform.position;
                    reboundMove += CourtMgr.Get.RealBallVelocity * 0.3f;
                } else
                    reboundMove = Vector3.zero;

                InitAnimatorCurve(EAnimatorState.Rebound, nextState.StateNo);
                SetShooterLayer();
                AddTrigger(EAnimatorState.Rebound, nextState.StateNo);
                GameRecord.ReboundLaunch++;
                Result = true;
                break;
            case EAnimatorState.JumpBall:
                AddTrigger(EAnimatorState.JumpBall, nextState.StateNo);
                SetShooterLayer();
                Result = true;
                break;
            case EAnimatorState.TipIn:
                AddTrigger(EAnimatorState.TipIn, nextState.StateNo);
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

    private void DebugAnimationCurve(string curveName)
    {
        if (GameStart.Get.IsDebugAnimation)
            LogMgr.Get.LogError("Can not Find aniCurve: " + curveName);
    }

    public void SetShooterLayer()
    {
        isStartCheckLayer = false;
        PlayerRefGameObject.layer = LayerMask.NameToLayer("Shooter");
        isCheckLayerToReset = true;
    }

    public void ClearAnimatorFlag(EActionFlag addFlag = EActionFlag.None)
    {
        if (addFlag == EActionFlag.None)
        {
            DelActionFlag(EActionFlag.IsDefence);
            DelActionFlag(EActionFlag.IsRun);
            DelActionFlag(EActionFlag.IsDribble);
            DelActionFlag(EActionFlag.IsHoldBall);
        } else
        {
            for (int i = 1; i < System.Enum.GetValues(typeof(EActionFlag)).Length; i++)
                if (i != (int)addFlag)
                    DelActionFlag((EActionFlag)i);
                    
            AddActionFlag(addFlag);
        }
    }
    
    public void AnimationEvent(string animationName)
    {
        if (GameController.Get.IsShowSituation)
            return;

        switch (animationName)
        {
            case "Stealing":
                if (OnStealMoment != null)
                {
                    if (OnStealMoment(this))
                    {
                        GameRecord.Steal++;
                        if (this == GameController.Get.Joysticker)
                            GameController.Get.IsGameFinish();
                    }
                }

                break;

            case "GotStealing":
                if (OnGotSteal != null)
                if (OnGotSteal(this))
                    GameRecord.BeSteal++;
                break;  
                    
            case "FakeShootBlockMoment":
                if (!IsAllShoot && OnFakeShootBlockMoment != null)
                    OnFakeShootBlockMoment(this);
                break;

            case "BlockMoment":
                if (OnBlockMoment != null)
                    OnBlockMoment(this);
                break;

            case "AirPassMoment":
                IsPassAirMoment = true;
                break;

            case "DoubleClickMoment":
                if (OnDoubleClickMoment != null)
                    OnDoubleClickMoment(this, crtState);
                break;

            case "BuffEnd":
                if (IsBallOwner)
                {
                    AniState(EPlayerState.HoldBall);
                    IsFirstDribble = true;
                } else
                    AniState(EPlayerState.Idle);
                break;
            
            case "BlockCatchMomentStart":
                blockCatchTrigger.SetEnable(true);
                break;
            
            case "BlockCatchMomentEnd":
                IsPerfectBlockCatch = false;
                break;

            case "BlockJump":
                if (OnBlockJump != null)
                    OnBlockJump(this);
                break;

            case "BlockCatchingEnd":
                if (IsBallOwner)
                {
                    IsFirstDribble = true;
                    AniState(EPlayerState.HoldBall);
                } else
                    AniState(EPlayerState.Idle);

                IsPerfectBlockCatch = false;
                break;

            case "Shooting":
                IsPassAirMoment = false;
                if (OnShooting != null)
                {
                    if (crtState != EPlayerState.Pass4)
                        OnShooting(this);
                    else 
                    if (crtState == EPlayerState.Layup0)
                    {
                        if (CourtMgr.Get.RealBall.transform.parent == DummyBall.transform)
                        {
                            LogMgr.Get.Log(PlayerRefGameObject.name + " layup no ball.");
                            GameController.Get.SetBall();
                        }
                    }
                }

                break;

            case "MoveDodgeEnd": 
                OnUI(this);
                if (IsBallOwner)
                    AniState(EPlayerState.Dribble0);
                else
                    AniState(EPlayerState.Idle);
                break;

            case "Passing": 
                //0.Flat
                //2.Floor
                //1 3.Parabola(Tee)
                if (IsBallOwner)
                {
                    if (GameController.Get.IsCatcherAlleyoop)
                    {
                        CourtMgr.Get.RealBallTrigger.PassBall(99);   
                    } else
                        CourtMgr.Get.RealBallTrigger.PassBall(AnimatorControl.GetInteger("StateNo"));

                    GameController.Get.IsCatcherAlleyoop = false;
                }
                break;

            case "PassEnd":
                OnUI(this);
                
                if (!IsBallOwner && PlayerRefGameObject.transform.localPosition.y < 0.2f)
                    AniState(EPlayerState.Idle);
                break;

            case "ShowEnd":
                if (!IsBallOwner)
                    AniState(EPlayerState.Idle);
                else
                    AniState(EPlayerState.HoldBall);
                break;
            
            case "PickUp": 
                if (OnPickUpBall != null)
                if (OnPickUpBall(this))
                    GameRecord.SaveBall++;

                break;
            case "PickEnd":
                if (IsBallOwner)
                {
                    IsFirstDribble = true;
                    AniState(EPlayerState.HoldBall);
                } else
                    AniState(EPlayerState.Idle);
                break;

            case "PushCalculateStart":
                GameController.Get.PushCalculate(this, GameConst.StealPushDistance, 30);
//              IsPushCalculate = true;
//                pushTrigger.gameObject.SetActive(true);
                break;

            case "PushCalculateEnd":
//              IsPushCalculate = false;
//                pushTrigger.SetActive(false);
                break;

            case "ElbowCalculateStart":
                GameController.Get.PushCalculate(this, GameConst.StealPushDistance, 270);
                break;
                
            case "ElbowCalculateEnd":
//                elbowTrigger.SetActive(false);
                break;

            case "BlockCalculateStart":
                blockTrigger.SetActive(true);
                break;

            case "BlockCalculateEnd":
                blockTrigger.SetActive(false);
                break;

            case "CloneMesh":
                if (!IsBallOwner)
                    EffectManager.Get.CloneMesh(PlayerRefGameObject, playerDunkCurve.CloneMaterial, 
                        playerDunkCurve.CloneDeltaTime, playerDunkCurve.CloneCount);
                break;

            case "DunkBasketStart":
                CourtMgr.Get.PlayDunk(Team.GetHashCode(), AnimatorControl.GetInteger("StateNo"));

                break;
            case "OnlyScore":
                if (OnOnlyScore != null) 
                    OnOnlyScore(this);
                break;

            case "DunkFallBall":
                OnUI(this);
                if (OnDunkBasket != null)
                    OnDunkBasket(this);

                break;

            case "ElbowEnd":
                OnUI(this);
                AniState(EPlayerState.HoldBall);
                CourtMgr.Get.ShowBallSFX(Attr.PunishTime);
                CourtMgr.Get.ShowBallSFX(Attr.PunishTime);
                break;

            case "CatchEnd":
                if (situation == EGameSituation.InboundsGamer || situation == EGameSituation.InboundsNPC)
                {
                    if (IsBallOwner)
                        AniState(EPlayerState.Dribble0);
                    else
                        AniState(EPlayerState.Idle);
                } else
                {
                    OnUI(this);
                    IsFirstDribble = true;
                    if (!IsBallOwner)
                    {                   
                        AniState(EPlayerState.Idle);
                    } else
                    {
                        if (AIing)
                            AniState(EPlayerState.Dribble0);
                        else 
                            AniState(EPlayerState.HoldBall);
                    }
                }
                break;

            case "FakeShootEnd":
                isFakeShoot = false;
                if (IsBallOwner)
                    AniState(EPlayerState.HoldBall);
                else
                    AniState(EPlayerState.Idle);

                OnUI(this);
                CourtMgr.Get.ShowBallSFX(Attr.PunishTime);
                break;

            case "AnimationEnd":
                OnUI(this);

                if (crtState == EPlayerState.Layup0 && CourtMgr.Get.RealBall.transform.parent == DummyBall.transform)
                {
                    LogMgr.Get.Log(PlayerRefGameObject.name + " AnimationEnd layup no ball.");
                    GameController.Get.SetBall();
                }

//              Debug.LogWarning(gameObject.name + ".AnimationEnd : " +crtState.ToString());


                if (!IsBallOwner)
                    AniState(EPlayerState.Idle);
                else
                {
                    if (firstDribble)
                        AniState(EPlayerState.Dribble0);
//                      else
//                          AniState(EPlayerState.HoldBall);
                }

                isUsePass = false;
                IsPassAirMoment = false;
                blockTrigger.SetActive(false);
                PlayerRigidbody.useGravity = true;
                IsKinematic = false;
                IsPerfectBlockCatch = false;
                isRebound = false;
                isPush = false; 
                IsUseSkill = false;
                blockCatchTrigger.enabled = false;

                if (!NeedResetFlag)
                    isCheckLayerToReset = true;
                else
                    ResetFlag();

                break;
        }
    }

    public void EffectEvent(string effectName)
    {
        switch (effectName)
        {
            case "FallDownFX":
                EffectManager.Get.PlayEffect(effectName, PlayerRefGameObject.transform.position, null, null, 3);
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

    public void TimeScale(AnimationEvent aniEvent)
    {
        float floatParam = aniEvent.floatParameter;
        int intParam = aniEvent.intParameter;

        switch (intParam)
        {
            case 0: //set all
                foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
                    TimerMgr.Get.ChangeTime(item, floatParam);
                break;
            case 1: //Set myself
                foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
                    if (item == CrtTimeKey)
                        TimerMgr.Get.ChangeTime(item, floatParam);
                break;
            case 2: //Set Other 
                foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
                    if (item != CrtTimeKey && item != ETimerKind.Default)
                        TimerMgr.Get.ChangeTime(item, floatParam);
                break;
            case 3: //Set Default
                TimerMgr.Get.ChangeTime(ETimerKind.Default, floatParam);
                break;
        }
    }

    public void ZoomIn(float t)
    {
        CameraMgr.Get.SkillShow(PlayerRefGameObject); 
        CameraMgr.Get.SetRoomMode(EZoomType.In, t); 
    }

    public void ZoomOut(float t)
    {
        CameraMgr.Get.SkillShow(PlayerRefGameObject);
        CameraMgr.Get.SetRoomMode(EZoomType.Out, t); 
    }

    //All Skill Event From this Function
    public void SkillEvent(AnimationEvent aniEvent)
    {
        float skillFloat = aniEvent.floatParameter;
        int skillInt = aniEvent.intParameter;
        string skillString = aniEvent.stringParameter;

        switch (skillString)
        {
            case "CameraBlur": 
                CameraMgr.Get.CourtCameraAnimator.SetTrigger("CameraAction_0");
                break;
            case "PushDistancePlayer":
                if (GameData.DSkillData.ContainsKey(ActiveSkillUsed.ID))
                {
                    GameRecord.Push ++;
                    if (this == GameController.Get.Joysticker)
                        GameController.Get.IsGameFinish();
                    for (int i=0; i<GameController.Get.GamePlayers.Count; i++)
                    {
                        if (GameController.Get.GamePlayers [i].Team != Team)
                        {
                            if (GameController.Get.GetDis(new Vector2(GameController.Get.GamePlayers [i].transform.position.x, GameController.Get.GamePlayers [i].transform.position.z), 
                                                     new Vector2(PlayerRefGameObject.transform.position.x, PlayerRefGameObject.transform.position.z)) <= GameData.DSkillData [ActiveSkillUsed.ID].Distance(ActiveSkillUsed.Lv))
                            {
                                GameController.Get.GamePlayers [i].AniState(EPlayerState.Fall1, PlayerRefGameObject.transform.position);
                            }
                        } 
                    }
                }
                break;
            case "SetBallEvent":
                GameController.Get.SetBall(this);
                if (this == GameController.Get.Joysticker)
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
                    foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
                        TimerMgr.Get.ChangeTime(item, 1);

                    if (isBlock)
                    {
                        if (GameController.Get.BallState == EBallState.CanBlock)
                        {
                            CourtMgr.Get.RealBallVelocity = Vector3.zero;
                            skillFaceTarget = judgePlayerFace(PlayerRefGameObject.transform.eulerAngles.y);
                            CourtMgr.Get.BlockRealBall(new Vector3(skillFaceTarget.x,
                                                               -1,
                                                               skillFaceTarget.z));
                        } else if (GameController.Get.BallState == EBallState.CanDunkBlock)
                        {
                            PlayerBehaviour p = GameController.Get.BallOwner;
                            GameController.Get.SetBall();
                            CourtMgr.Get.SetBallState(EPlayerState.Block0, p);
                            p.DoPassiveSkill(ESkillSituation.KnockDown0);
                        }
                        GameController.Get.BallState = EBallState.None;
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

    public void StartSkillCamera(int no)
    {
        if (no < 20)
            return;

        if (GameData.DSkillData.ContainsKey(ActiveSkillUsed.ID))
        {
            int skillEffectKind = GameData.DSkillData [ActiveSkillUsed.ID].ActiveCamera;
            float skillTime = GameData.DSkillData [ActiveSkillUsed.ID].ActiveCameraTime;
            if (this == GameController.Get.Joysticker && GameData.DSkillData.ContainsKey(ActiveSkillUsed.ID))
            {
                if (!isSkillShow)
                {
                    if (OnUIJoystick != null)
                        OnUIJoystick(this, false);
                    
                    if (UIPassiveEffect.Visible)
                        UIPassiveEffect.UIShow(false);
                    
                    isSkillShow = true;
                    string effectName = string.Format("UseSkillEffect_{0}", GameData.DSkillData [ActiveSkillUsed.ID].Kind);
                    EffectManager.Get.PlayEffect(effectName, transform.position, null, null, 1, false);
                    
                    if (GameController.Get.BallOwner != null && GameController.Get.BallOwner == GameController.Get.Joysticker)
                        LayerMgr.Get.SetLayerRecursively(CourtMgr.Get.RealBall, "SkillPlayer", "RealBall");
                    
                    CameraMgr.Get.SkillShowActive(skillEffectKind, skillTime);
                    if (GameData.DSkillData.ContainsKey(ActiveSkillUsed.ID))
                        UISkillEffect.UIShow(true, skillEffectKind, GameData.DSkillData [ActiveSkillUsed.ID].PictureNo, ActiveSkillUsed.Lv, GameData.DSkillData [ActiveSkillUsed.ID].Name);
                    
                    switch (skillEffectKind)
                    {
                        case 0://show self and rotate camera
                            Invoke("showActiveEffect", skillTime);
                            LayerMgr.Get.SetLayerRecursively(GameController.Get.Joysticker.PlayerRefGameObject, "SkillPlayer", "PlayerModel", "(Clone)");
                            foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind))) 
                                TimerMgr.Get.ChangeTime(item, 0);
                            break;
                        case 1://show self
                            showActiveEffect();
                            LayerMgr.Get.SetLayerRecursively(GameController.Get.Joysticker.PlayerRefGameObject, "SkillPlayer", "PlayerModel", "(Clone)");
                            foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind))) 
                                if (item != ETimerKind.Player0)
                                    TimerMgr.Get.ChangeTime(item, 0);
                            break;
                        case 2://show all Player
                            showActiveEffect();
                            GameController.Get.SetAllPlayerLayer("SkillPlayer");
                            foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind))) 
                                if (item != ETimerKind.Player0)
                                    TimerMgr.Get.ChangeTime(item, 0);
                            break;
                    }
                }
            } else
            {
                if (GameData.DSkillData [ActiveSkillUsed.ID].Kind == 140 ||
                    GameData.DSkillData [ActiveSkillUsed.ID].Kind == 150 ||
                    GameData.DSkillData [ActiveSkillUsed.ID].Kind == 160 ||
                    GameData.DSkillData [ActiveSkillUsed.ID].Kind == 161 ||
                    GameData.DSkillData [ActiveSkillUsed.ID].Kind == 170 ||
                    GameData.DSkillData [ActiveSkillUsed.ID].Kind == 171 ||
                    GameData.DSkillData [ActiveSkillUsed.ID].Kind == 180)
                {
                
                    if (!isSkillShow)
                    {
                        foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind))) 
                            if (item != CrtTimeKey)
                                TimerMgr.Get.ChangeTime(item, 0);

                        isSkillShow = true;
                    }
                }

                if (GameData.DSkillData.ContainsKey(ActiveSkillUsed.ID) && !IsUseSkill)
                    UIPassiveEffect.Get.ShowCard(this, ActiveSkillUsed.ID, ActiveSkillUsed.Lv);
                showActiveEffect();
            }

        }
    }
    
    public void StopSkill()
    {
        TimerMgr.Get.ChangeTime(CrtTimeKey, 1);
    }

    public void showActiveEffect()
    {
        SkillEffectManager.Get.OnShowEffect(this, false);
    }

    public void ResetMove()
    {
        moveQueue.Clear();
        CantMoveTimer.Clear();
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
    public bool DoPassiveSkill(ESkillSituation state, Vector3 v = default(Vector3))
    {
        return skillController.DoPassiveSkill(state, this, v);
    }

    public bool DoActiveSkill(GameObject target = null)
    {
        if (CanUseActiveSkill(ActiveSkillUsed) || GameStart.Get.TestMode == EGameTest.Skill)
        {
            GameRecord.Skill++;
            SetAnger(-Attribute.MaxAngerOne(ActiveSkillUsed.ID));

            if (Attribute.SkillAnimation(ActiveSkillUsed.ID) != "")
            {
                if (target)
                    return AniState((EPlayerState)System.Enum.Parse(typeof(EPlayerState), Attribute.SkillAnimation(ActiveSkillUsed.ID)), target.transform.position);
                else
                {
                    try
                    {
                        return AniState((EPlayerState)System.Enum.Parse(typeof(EPlayerState), Attribute.SkillAnimation(ActiveSkillUsed.ID)));
                    } catch
                    {
                        LogMgr.Get.LogError("Can't find SkillAnimation in EPlayerState");
                        return false;
                    }
                }
            }
        }
        return false;
    }

    public void AddSkillAttribute(int skillID, int kind, float value, float lifetime)
    {
        skillController.AddSkillAttribute(skillID, kind, value, lifetime);
    }

    public void CheckSkillValueAdd(TSkill activeSkill)
    {
        skillController.CheckSkillValueAdd(this, activeSkill);
    }

    public void SetAttribute(int kind, float value)
    {
        Attribute.AddAttribute(kind, value);
        initAttr();
    }

    public bool CheckSkillDistance(TSkill tSkill)
    {
        bool result = false;
        if (skillController.GetActiveSkillTarget(this, tSkill) != null && skillController.GetActiveSkillTarget(this, tSkill).Count > 0)
            for (int i=0; i<skillController.GetActiveSkillTarget(this, tSkill).Count; i++)
                if (skillController.CheckSkillDistance(this, tSkill, skillController.GetActiveSkillTarget(this, tSkill) [i])) 
                    result = true;
        return result;
    }

    public bool CheckSkillKind(TSkill tSkill)
    {
        return skillController.CheckSkillKind(tSkill);
    }

    public TSkill ActiveSkillUsed
    {
        get { return skillController.ActiveSkillUsed;}
        set { skillController.ActiveSkillUsed = value;}
    }

    public TSkill PassiveSkillUsed
    {
        get { return skillController.PassiveSkillUsed;}
        set { skillController.PassiveSkillUsed = value;}
    }

    public int MoveDodgeRate
    {
        get { return skillController.MoveDodgeRate;}
        set { skillController.MoveDodgeRate = value;}
    }

    public int MoveDodgeLv
    {
        get { return skillController.MoveDodgeLv;}
        set { skillController.MoveDodgeLv = value;}
    }

    public int PickBall2Rate
    {
        get { return skillController.PickBall2Rate;}
        set { skillController.PickBall2Rate = value;}
    }

    public int PickBall2Lv
    {
        get { return skillController.PickBall2Lv;}
        set { skillController.PickBall2Lv = value;}
    }

    public List<int> GetAllBuffs
    {
        get { return skillController.GetAllBuff();}
    }

    public ESkillKind GetSkillKind
    {
        get{ return skillKind;}
    }
                            
    public bool IsHaveMoveDodge
    {
        get { return skillController.DPassiveSkills.ContainsKey((int)ESkillKind.MoveDodge);}
    }

    public bool IsHavePickBall2
    {
        get { return skillController.DPassiveSkills.ContainsKey((int)ESkillKind.Pick2);}
    }

    public bool CanPressButton
    {
        get { return (Timer.timeScale == 1);}
    }

    public bool IsAlleyoopState
    {
        get
        {
            return CheckAnimatorSate(EPlayerState.Alleyoop);
        }
    }
    
    public bool CanMove
    {
        get
        {
            if (IsUseSkill || StateChecker.StopStates.ContainsKey(crtState) || IsFall || IsShoot || IsDunk || IsLayup)
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
        if ((CanMove || crtState == EPlayerState.HoldBall) &&
            !IsUseSkill && IsAngerFull(tSkill))
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

    public bool IsChangeColor
    {
        get{ return isChangeColor;}
        set{ isChangeColor = value;}
    }

    public bool IsSkillShow
    {
        get { return isSkillShow;}
    }
    
    public bool IsCatcher
    {
        get{ return CheckAnimatorSate(EPlayerState.CatchFlat);}
    }

    public bool IsCanCatchBall
    {
        get{ return isCanCatchBall;}
    }

    public bool IsCanBlock
    {
        get{ return isCanBlock;}
        set
        {
            isCanBlock = value;
//            if(CourtMgr.Get.RealBallFX.activeSelf != value)
//                CourtMgr.Get.RealBallFX.SetActive(value);
            if (CourtMgr.Get.IsBallSFXEnabled() != value)
                CourtMgr.Get.ShowBallSFX();
        }
    }

    public bool IsHoldBall
    {
        get{ return crtState == EPlayerState.HoldBall;}
    }

    public bool IsDefence
    {
        get
        {
            if (situation == EGameSituation.AttackGamer && Team == ETeamKind.Npc)
                return true;
            if (situation == EGameSituation.AttackNPC && Team == ETeamKind.Self)
                return true;
            return false;
        }
    }
    
    public bool IsJump
    {
        get{ return PlayerRefGameObject.transform.localPosition.y > 0.1f;}
    }

    public bool IsBallOwner
    {
        get { return AnimatorControl.GetBool("IsBallOwner");}
        set { AnimatorControl.SetBool("IsBallOwner", value);}
    }

    public bool IsBlock
    {
        get{ return crtState == EPlayerState.Block0 || crtState == EPlayerState.Block1 || crtState == EPlayerState.Block2 || crtState == EPlayerState.Block20;}
    }

    public bool IsBlockCatch
    {
        get{ return crtState == EPlayerState.BlockCatch;} 
    }

    public bool IsShoot
    {
        get { return StateChecker.ShootStates.ContainsKey(crtState); }
    }

    public bool IsShow
    {
        get{ return StateChecker.ShowStates.ContainsKey(crtState);}
    }

    public bool IsLoopState
    {
        get{ return StateChecker.LoopStates.ContainsKey(crtState);}
    }

    public bool IsBuff
    {
        get { return crtState == EPlayerState.Buff20 || crtState == EPlayerState.Buff21;}
    }

    public bool IsAllShoot
    {
        get{ return IsShoot || IsDunk || IsLayup;}
    }

    public bool IsIdle
    {
        get{ return crtState == EPlayerState.Idle;}
    }

    public bool IsDef
    {
        get{ return crtState == EPlayerState.Defence0 || crtState == EPlayerState.Defence1;}
    }

    public bool IsRun
    {
        get{ return crtState == EPlayerState.Run0 || crtState == EPlayerState.Run1 || crtState == EPlayerState.Run2;}
    }

    public bool IsPass
    {
        get{ return StateChecker.PassStates.ContainsKey(crtState);}
    }

    public bool IsPickBall
    {
        get{ return crtState == EPlayerState.Pick0 || crtState == EPlayerState.Pick1 || crtState == EPlayerState.Pick2;}
    }

    public bool IsDribble
    {
        get{ return crtState == EPlayerState.Dribble0 || crtState == EPlayerState.Dribble1 || crtState == EPlayerState.Dribble2 || crtState == EPlayerState.Dribble3;}
    }

    public bool IsDunk
    {
        get{ return crtState == EPlayerState.Dunk0 || crtState == EPlayerState.Dunk1 || crtState == EPlayerState.Dunk3 || crtState == EPlayerState.Dunk5 || crtState == EPlayerState.Dunk7 || crtState == EPlayerState.Dunk2 || crtState == EPlayerState.Dunk4 || crtState == EPlayerState.Dunk6 || crtState == EPlayerState.Dunk20 || crtState == EPlayerState.Dunk21 || crtState == EPlayerState.Dunk22;}
    }

    public bool IsLayup
    {
        get{ return crtState == EPlayerState.Layup0 || crtState == EPlayerState.Layup1 || crtState == EPlayerState.Layup2 || crtState == EPlayerState.Layup3;}
    }

    public bool IsMoveDodge
    {
        get{ return crtState == EPlayerState.MoveDodge0 || crtState == EPlayerState.MoveDodge1;}
    }

    public bool IsSteal
    {
        get{ return crtState == EPlayerState.Steal0 || crtState == EPlayerState.Steal1 || crtState == EPlayerState.Steal2 || crtState == EPlayerState.Steal20;}
    }

    public bool IsGotSteal
    {
        get{ return crtState == EPlayerState.GotSteal;}
    }

    public bool IsTipIn
    {
        get{ return crtState == EPlayerState.TipIn;}
    }

    public bool IsKnockDown
    {
        get{ return crtState == EPlayerState.KnockDown0 || crtState == EPlayerState.KnockDown1;}
    }

    public bool IsUseSkill //Only ActiveSkill
    {
        get{ return isUseSkill;}
        set{ isUseSkill = value;}
    }

    private bool isMoving = false;

    public bool IsMoving
    {
        get{ return isMoving;}
    }

    public bool IsRebound
    {
        get{ return crtState == EPlayerState.Rebound0 || crtState == EPlayerState.Rebound20 || crtState == EPlayerState.ReboundCatch;}
    }

    public bool IsFall
    {
        get{ return crtState == EPlayerState.Fall0 || crtState == EPlayerState.Fall1 || crtState == EPlayerState.Fall2 || crtState == EPlayerState.KnockDown0 || crtState == EPlayerState.KnockDown1;}
    }
    
    public bool IsCatch
    {
        get{ return crtState == EPlayerState.CatchFlat || crtState == EPlayerState.CatchFloor || crtState == EPlayerState.CatchParabola;}
    }

    public bool IsFakeShoot
    {
        get{ return isFakeShoot;}
    }

    public bool IsPush
    {
        get{ return crtState == EPlayerState.Push0 || crtState == EPlayerState.Push1 || crtState == EPlayerState.Push2 || crtState == EPlayerState.Push20;}
    }

    public bool IsIntercept
    {
        get{ return crtState == EPlayerState.Intercept0 || crtState == EPlayerState.Intercept1;}
    }

    public bool IsElbow
    {
        get{ return crtState == EPlayerState.Elbow0 || crtState == EPlayerState.Elbow1 || crtState == EPlayerState.Elbow2 || crtState == EPlayerState.Elbow20 || crtState == EPlayerState.Elbow21;}
    }

    private bool isPerfectBlockCatch = false;

    public bool IsPerfectBlockCatch
    {
        get{ return isPerfectBlockCatch;}
        set
        {
            isPerfectBlockCatch = value;

            if (!isPerfectBlockCatch)
            {
                blockCatchTrigger.SetEnable(false);
            } else
            {
                if (OnDoubleClickMoment != null)
                    OnDoubleClickMoment(this, crtState);
            }
//                  EffectManager.Get.PlayEffect("DoubleClick01", Vector3.zero, null, PlayerGameObject, 1f);
        }
    }

    public bool IsFirstDribble
    {
        get{ return firstDribble;}
        set{ firstDribble = value;}
    }

    public bool IsAngerFull(TSkill tSkill)
    {
        return Attribute.CheckIfMaxAnger(tSkill.ID, angerPower);
    }

    public bool AIing
    {
//      get { return PlayerRefGameObject.activeSelf && aiTime <= 0; }
        get { return PlayerRefGameObject.activeSelf && mManually.IsOff(); }
    }

    public int TargetPosNum
    {
        get { return moveQueue.Count;}
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

    public TMoveData TargetPos
    {
        set
        {
            MoveName = value.TacticalName;
            if (moveQueue.Count == 0)
                MoveTurn = 0;

            moveQueue.Enqueue(value);

            if (value.Target != Vector2.zero)
                GameRecord.PushMove(value.Target);
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

    public void ResetCurveFlag()
    {
        isDunk = false;
        isBlock = false;
        isLayup = false;
        isCanBlock = false;
        isRebound = false;
        isShootJump = false;
        isPush = false;
        isFall = false;
    }

    private float GetAngle(Vector3 target)
    {
        Vector3 relative = this.transform.InverseTransformPoint(target);
        return Math.Abs(Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg);
    }

    public float GetAngle(Transform t1, Transform t2)
    {
        Vector3 relative = t1.InverseTransformPoint(t2.position);
        return Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
    }

    public Vector2 GetStealPostion(Vector3 P1, Vector3 P2, int mIndex)
    {
        bool cover = false;
        Vector2 Result = Vector2.zero;
        if (P1.x > 0)
            Result.x = P1.x - (Math.Abs(P1.x - P2.x) / 3);
        else
            Result.x = P1.x + (Math.Abs(P1.x - P2.x) / 3);

        if (GameController.Get.BallOwner && GameController.Get.BallOwner.DefPlayer && GameController.Get.BallOwner.Index != Index)
        {
            float angle = Math.Abs(GetAngle(GameController.Get.BallOwner.transform, GameController.Get.BallOwner.DefPlayer.transform));

            if (angle > 90)
            {
                P1 = GameController.Get.BallOwner.transform.position;
                if (P1.x > 0)
                    Result.x = P1.x - (Math.Abs(P1.x - P2.x) / 3);
                else
                    Result.x = P1.x + (Math.Abs(P1.x - P2.x) / 3);

                cover = true;
            }
        }

        if (mIndex != Index && !cover)
        {
            switch (mIndex)
            {
                case 0:
                    if (Index == 1)
                        Result.x += 1.5f;
                    else
                        Result.x -= 1.5f;
                    break;
                case 1:
                    if (Index == 0)
                        Result.x += 1.5f;
                    else
                        Result.x -= 1.5f;
                    break;
                case 2:
                    if (Index == 0)
                        Result.x += 1.5f;
                    else
                        Result.x -= 1.5f;
                    break;
            }
        }

        if (P2.z > 0)
            Result.y = P1.z + (Math.Abs(P1.z - P2.z) / 3);
        else
            Result.y = P1.z - (Math.Abs(P1.z - P2.z) / 3);
        return Result;
    }
}
