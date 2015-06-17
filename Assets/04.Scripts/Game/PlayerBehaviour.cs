using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameStruct;

public delegate bool OnPlayerAction(PlayerBehaviour player);

public delegate bool OnPlayerAction2(PlayerBehaviour player,bool speedup);

public delegate bool OnPlayerAction3(PlayerBehaviour player,EPlayerState state);

public delegate void OnPlayerAction4(PlayerBehaviour player, float anger);

public enum EPlayerState
{
	Alleyoop,
	Block,  
	Board, 
	BlockCatch,
	BasketAnimationStart,
	BasketActionEnd,
	BasketActionSwish,
	BasketActionSwishEnd,
	BasketActionNoScoreEnd,
	CatchFlat,
	CatchParabola,
	CatchFloor,
	Dribble0,
	Dribble1,
	Dribble2,
	Dunk0,
	Dunk2,
	Dunk4,
	Dunk6,
	Dunk20,
	DunkBasket,
	Defence0,    
	Defence1,
	Elbow,
	Fall0,
	Fall1,
	Fall2,
	FakeShoot,
	GotSteal,
	HoldBall,
    Idle,
	Intercept0,
	Intercept1,
	Layup0, 
	Layup1, 
	Layup2, 
	Layup3, 
	MoveDodge0,
	MoveDodge1,
	PickBall0,
	PickBall2,
	Pass0,
	Pass1,
	Pass2,
	Pass3,
	Pass4,
	Pass5,
	Pass6,
	Pass7,
	Pass8,
	Pass9,
	Push,
    Run0,            
    Run1,            
    RunningDefence,
	Rebound,
	ReboundCatch,
	Reset,
	Start,
	Shoot0,
	Shoot1,
	Shoot2,
	Shoot3,
	Shoot4,
	Shoot5,
	Shoot6,
	Shoot7,
	Steal,
	TipIn,
	JumpBall  
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
    public Vector2 Target;
    public Transform LookTarget;
    public Transform FollowTarget;
    public PlayerBehaviour DefPlayer;
    public OnPlayerAction2 MoveFinish;
    public bool Speedup;
    public bool Catcher;
    public bool Shooting;
    public string FileName;

    public TMoveData(int flag)
    {
        Target = Vector2.zero;
        LookTarget = null;
        MoveFinish = null;
        FollowTarget = null;
        DefPlayer = null;
        Speedup = false;
        Catcher = false;
        Shooting = false;
        FileName = "";
    }
}

[System.Serializable]
public struct TShootAngle {
	public int CenterShootAngle;
	public int ForwardShootAngle;
	public int GuardShootAngle;
	public int CenterTipInAngle;
	public int ForwardTipInAngle;
	public int GuardTipInAngle;
	public TShootAngle (int flag){
		CenterShootAngle = 55;
		ForwardShootAngle = 55;
		GuardShootAngle = 55;
		CenterTipInAngle = 75;
		ForwardTipInAngle = 75;
		GuardTipInAngle = 75;
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
        NearShotScoreRate = 0;
        NearShotSwishRate = 10;
        NearShotAirBallRate = 3;
        LayUpScoreRate = 0;
        LayUpSwishRate = 20;
        LayUpAirBallRate = 2;

    }
}

public struct TPassiveSkill
{
    public int ID;
    public int Kind;
    public string Name;
    public int Rate;
}

public enum EActiveDistanceType 
{
	AttackHalfCount,
	DeffenceHalfCount,
	AllCount
}

public struct TActiveSkill 
{
	public int ID;
	public int Kind;
	public string Name;
	public EActiveDistanceType type;
}

public class PlayerBehaviour : MonoBehaviour
{
    public OnPlayerAction OnShooting = null;
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
	public OnPlayerAction4 OnUIAnger = null;
    public OnPlayerAction3 OnDoubleClickMoment = null;

    public float[] DunkHight = new float[2]{3, 5};
    private const float MoveCheckValue = 1;
    public static string[] AnimatorStates = new string[] {"", "IsRun", "IsDefence", "IsDribble", "IsHoldBall"};
    private byte[] PlayerActionFlag = {0, 0, 0, 0, 0, 0, 0};

//    private Vector2 drag = Vector2.zero;
    private bool stop = false;
    private bool NeedResetFlag = false;
    private int MoveTurn = 0;
    private float MoveStartTime = 0;
//    private float TimeProactiveRate = 0;
    private float ProactiveTime = 0;
    private int smoothDirection = 0;
    private float animationSpeed = 0;
    private float MoveMinSpeed = 0.5f;
    private float canDunkDis = 30f;

    public Queue<TMoveData> MoveQueue = new Queue<TMoveData>();
    private Queue<TMoveData> FirstMoveQueue = new Queue<TMoveData>();
    public Vector3 Translate;
    public Rigidbody PlayerRigidbody;
    private Animator animator;
    private GameObject selectTexture;
    private GameObject DefPoint;
	private GameObject TopPoint;
	private GameObject FingerPoint;
    private GameObject pushTrigger;
    private GameObject elbowTrigger;
    private GameObject blockTrigger;
    private BlockCatchTrigger blockCatchTrigger;
    public GameObject AIActiveHint = null;
    public GameObject DummyBall;
    public UISprite SpeedUpView = null;
    public UISprite AngerView = null;
    public GameObject AngryFull = null;
	public Material BodyMaterial;

	public GameStruct.TPlayerAttribute Attr;
	public GameStruct.TPlayer Player;
	public TScoreRate ScoreRate;
	public TGamePlayerRecord GameRecord = new TGamePlayerRecord();

    public ETeamKind Team;
	public int Index;
    public float NoAiTime = 0;
    public bool HaveNoAiTime = false;
    public EGameSituation situation = EGameSituation.None;
    public EPlayerState crtState = EPlayerState.Idle;
	private EPassDirectState passDirect = EPassDirectState.Forward;
    public Transform[] DefPointAy = new Transform[8];
    public float WaitMoveTime = 0;
    public float Invincible = 0;
    public float JumpHight = 450f;
    public float CoolDownSteal = 0;
    public float CoolDownPush = 0;
    public float CoolDownElbow = 0;
    public float AirDrag = 0f;
    public float fracJourney = 0;
    public int MoveIndex = -1;
    public bool isJoystick = false;
    public PlayerBehaviour DefPlayer = null;
    public float CloseDef = 0;
    public bool AutoFollow = false;
    public bool NeedShooting = false;

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

    //Fall
    private bool isFall = false;
    private float fallCurveTime = 0;
    private TSharedCurve playerFallCurve;

    //Pick
    private bool isPick = false;
    private float pickCurveTime = 0;
    private TSharedCurve playerPickCurve;

    //PassiveSkill
	private Dictionary<int, List<TPassiveSkill>> passiveSkills = new Dictionary<int, List<TPassiveSkill>>(); // key:TSkillKind  value:List<PassiveSkill>  
	private Dictionary<int, List<TPassiveSkill>> passivePassDirects = new Dictionary<int, List<TPassiveSkill>>();
	//ActiveSkill
	public TActiveSkill activeSkill = new TActiveSkill();
//	private float activeTime  = 0;
    private bool isHaveMoveDodge = false;
	private bool isHavePickBall2 = false;
	private bool firstDribble = true;
    private bool isCanCatchBall = true;
    private bool IsSpeedup = false;
    public float MovePower = 0;
    public float MaxMovePower = 0;
    private float MovePowerTime = 0;
    private Vector2 MoveTarget;
    private float dis;
    private bool CanSpeedup = true;
    private float SlowDownTime = 0;
	public float DribbleTime = 0;
	private int angerPower = 0;

    public void SetAnger(int value)
    {
        angerPower += value;
        if (angerPower > 100)
            angerPower = 100;

        if (angerPower < 0)
            angerPower = 0;

        if (this == GameController.Get.Joysticker)
        {
            float temp = angerPower;
			OnUIAnger(this, temp);
//            GameController.Get.Joysticker.AngerView.fillAmount = temp / 100;
//			if (GameController.Get.Joysticker.AngerView.fillAmount == 1) {
//			} else {
//				GameController.Get.Joysticker.AngryFull.SetActive (false);
//			}
        }

		GameRecord.AngerAdd += value;
    }

    public void SetSlowDown(float Value)
    {
        if (SlowDownTime == 0)
        {
            SlowDownTime += Time.time + Value;
            Attr.SpeedValue = GameData.BaseAttr [Player.AILevel].SpeedValue * GameConst.SlowDownValue;
        }
    }
    
    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        gameObject.tag = "Player";

        animator = gameObject.GetComponent<Animator>();
        PlayerRigidbody = gameObject.GetComponent<Rigidbody>();

        ScoreRate = GameStart.Get.ScoreRate;
    }

    public void InitAttr()
    {
		setMovePower(100);
		GameRecord.Init();

        if (GameData.BaseAttr.Length > 0 && Player.AILevel >= 0 && Player.AILevel < GameData.BaseAttr.Length)
        {
            Attr.PointRate2 = Math.Min(GameData.BaseAttr [Player.AILevel].PointRate2 + (Player.Point2 * 0.5F), 100);
            Attr.PointRate3 = Math.Min(GameData.BaseAttr [Player.AILevel].PointRate3 + (Player.Point3 * 0.5F), 100);
            Attr.StealRate = Math.Min(GameData.BaseAttr [Player.AILevel].StealRate + (Player.Steal / 12), 100);
            Attr.DunkRate = Math.Min(GameData.BaseAttr[Player.AILevel].DunkRate + (Player.Dunk * 0.9f), 100);
            Attr.TipInRate = Math.Min(GameData.BaseAttr[Player.AILevel].TipInRate + (Player.Dunk * 0.9f), 100);
            Attr.AlleyOopRate = Math.Min(GameData.BaseAttr[Player.AILevel].AlleyOopRate + (Player.Dunk * 0.7f), 100);
            Attr.StrengthRate = Math.Min(GameData.BaseAttr[Player.AILevel].StrengthRate + (Player.Strength * 0.9f), 100);
            Attr.BlockPushRate = Math.Min(GameData.BaseAttr[Player.AILevel].BlockPushRate + (Player.Strength * 0.5f), 100);
            Attr.ElbowingRate = Math.Min(GameData.BaseAttr[Player.AILevel].ElbowingRate + (Player.Strength * 0.8f), 100);
            Attr.ReboundRate = Math.Min(GameData.BaseAttr [Player.AILevel].ReboundRate + (Player.Rebound * 0.9f), 100);
            Attr.BlockRate = Math.Min(GameData.BaseAttr[Player.AILevel].BlockRate + (Player.Block * 0.9f), 100);
            Attr.FaketBlockRate = Math.Min(GameData.BaseAttr[Player.AILevel].FaketBlockRate + (100-(Player.Block / 1.15f)), 100);
            Attr.JumpBallRate = Math.Min(GameData.BaseAttr [Player.AILevel].JumpBallRate, 100);
            Attr.PushingRate = Math.Min(GameData.BaseAttr [Player.AILevel].PushingRate + (Player.Defence * 1), 100);
			Attr.PassRate = Math.Min(GameData.BaseAttr[Player.AILevel].PassRate + (Player.Pass * 0.8f), 100);
			Attr.AlleyOopPassRate = Math.Min(GameData.BaseAttr[Player.AILevel].AlleyOopPassRate + (Player.Pass * 0.6f), 100);
			Attr.ReboundHeadDistance = GameData.BaseAttr [Player.AILevel].ReboundHeadDistance + (Player.Rebound / 200);
			Attr.ReboundHandDistance = GameData.BaseAttr [Player.AILevel].ReboundHandDistance + (Player.Rebound / 200);
			Attr.BlockDistance = GameData.BaseAttr [Player.AILevel].BlockDistance + (Player.Block / 100);
            Attr.DefDistance = GameData.BaseAttr [Player.AILevel].DefDistance + (Player.Defence * 0.1f);
            Attr.SpeedValue = GameData.BaseAttr [Player.AILevel].SpeedValue + (Player.Speed * 0.005f);
            Attr.StaminaValue = GameData.BaseAttr[Player.AILevel].StaminaValue + (Player.Stamina * 1.2f);
            Attr.AutoFollowTime = GameData.BaseAttr [Player.AILevel].AutoFollowTime;

            DefPoint.transform.localScale = new Vector3(Attr.DefDistance, Attr.DefDistance, Attr.DefDistance);
			TopPoint.transform.localScale = new Vector3(4 + Attr.ReboundHeadDistance, TopPoint.transform.localScale.y, 4 + Attr.ReboundHeadDistance);
			FingerPoint.transform.localScale = new Vector3(Attr.ReboundHandDistance,Attr.ReboundHandDistance,Attr.ReboundHandDistance);
			blockTrigger.transform.localScale = new Vector3( blockTrigger.transform.localScale.x, 3.2f + Attr.BlockDistance, blockTrigger.transform.localScale.z);
            if (Attr.StaminaValue > 0)
                setMovePower(Attr.StaminaValue);

			initSkill ();
        }
    }

	private void initSkill (){
		//Active
		if(Player.ActiveSkill.ID > 0) {
			activeSkill.ID = Player.ActiveSkill.ID;
			activeSkill.Name = GameData.SkillData [Player.ActiveSkill.ID].Animation;
			activeSkill.Kind = GameData.SkillData [Player.ActiveSkill.ID].Kind;
			int keyActive = GameData.SkillData [Player.ActiveSkill.ID].Kind;
			if(keyActive % 10 == 1 || keyActive % 10 == 4)
				activeSkill.type = EActiveDistanceType.AttackHalfCount;
			else 
			if(keyActive % 10 == 2 || keyActive % 10 == 5) 
				activeSkill.type = EActiveDistanceType.DeffenceHalfCount;
			else 
			if(keyActive % 10 == 3 || keyActive % 10 == 6) 
				activeSkill.type = EActiveDistanceType.AllCount;
		}
		//Passive
		if (Player.Skills != null && Player.Skills.Length > 0)
		{
			for (int i=0; i<Player.Skills.Length; i++)
			{
				if (Player.Skills [i].ID > 0)
				{
					if (GameData.SkillData.ContainsKey(Player.Skills [i].ID))
					{
						if (GameData.SkillData [Player.Skills [i].ID].Kind == (int)ESkillKind.MoveDodge) 
							isHaveMoveDodge = true;
						if (GameData.SkillData [Player.Skills [i].ID].Animation == ESkillKind.Pick2.ToString())
							isHavePickBall2 = true;
						int rate = GameData.SkillData [Player.Skills [i].ID].BaseRate + (GameData.SkillData [Player.Skills [i].ID].AddRate * Player.Skills [i].Lv); // BaseRate + ( AddRate * LV)
						TPassiveSkill ps = new TPassiveSkill();
						ps.ID = Player.Skills [i].ID;
						ps.Name = GameData.SkillData [Player.Skills [i].ID].Animation;
						ps.Rate = rate;
						ps.Kind = GameData.SkillData [Player.Skills [i].ID].Kind;


						int key = GameData.SkillData [Player.Skills [i].ID].Kind;
						if (key > 1000){
							key = (key / 100);
							int direct = 0;
							if(key % 10 == (int)EPassDirectState.Forward) 
								direct = (int)EPassDirectState.Forward;
							else if(key % 10 == (int)EPassDirectState.Back) 
								direct = (int)EPassDirectState.Back;
							else if(key % 10 == (int)EPassDirectState.Left) 
								direct = (int)EPassDirectState.Left;
							else if(key % 10 == (int)EPassDirectState.Right) 
								direct = (int)EPassDirectState.Right;

							if(passivePassDirects.ContainsKey(direct)) {
								List<TPassiveSkill> psTemps = passivePassDirects[direct];
								psTemps.Add(ps);
								passivePassDirects[direct] = psTemps;
							} else {
								List<TPassiveSkill> psTemps = new List<TPassiveSkill>();
								psTemps.Add(ps);
								passivePassDirects.Add(direct, psTemps);
							}

						}
						if(key >= 100 && key < 1000) {
							activeSkill.ID = Player.Skills[i].ID;
							activeSkill.Name = GameData.SkillData [Player.Skills [i].ID].Animation;
							activeSkill.Kind = GameData.SkillData [Player.Skills [i].ID].Kind;
							if(key % 10 == 1 || key % 10 == 4)
								activeSkill.type = EActiveDistanceType.AttackHalfCount;
							else 
							if(key % 10 == 2 || key % 10 == 5) 
								activeSkill.type = EActiveDistanceType.DeffenceHalfCount;
							else 
							if(key % 10 == 3 || key % 10 == 6) 
									activeSkill.type = EActiveDistanceType.AllCount;
						}
						if (passiveSkills.ContainsKey(key))
						{
							List<TPassiveSkill> pss = passiveSkills [key];
							pss.Add(ps);
							passiveSkills [key] = pss;
						} else
						{
							List<TPassiveSkill> pss = new List<TPassiveSkill>();
							pss.Add(ps);
							passiveSkills.Add(key, pss);
						}
					}
				}
			}
		}
		
//		float time = 0;
//		AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
//		if (clips != null && clips.Length > 0)
//		{
//			for (int i=0; i<clips.Length; i++)
//			{
//				for(int j=0; j<activeSkills.Count; j++) {
//					if (clips [i].name.Equals(activeSkills[j]))
//					if(clips[i].name.Equals(activeSkill.Name))
//					{
//						Debug.Log("clips[i].name:" + clips[i].name);
//						Debug.Log("clips[i].length:" + clips[i].length);
//						activeTime.Add(clips[i].name, clips[i].length);
//						activeTime = clips[i].length;
//					}
//				}
//			}
//		}
	}

    public void InitCurve(GameObject animatorCurve)
    {
        GameObject AnimatorCurveCopy = Instantiate(animatorCurve) as GameObject;
        AnimatorCurveCopy.transform.parent = gameObject.transform;
        AnimatorCurveCopy.name = "AniCurve";
        aniCurve = AnimatorCurveCopy.GetComponent<AniCurve>();
    }

    public void InitTrigger(GameObject defPoint)
    {
        SkinnedMeshRenderer render = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (render && render.material) 
            BodyMaterial = render.material;

        DummyBall = transform.FindChild("DummyBall").gameObject;
        
        if (DummyBall != null)
        {
            blockCatchTrigger = DummyBall.GetComponent<BlockCatchTrigger>();
            if (blockCatchTrigger == null)
                blockCatchTrigger = DummyBall.gameObject.AddComponent<BlockCatchTrigger>();
            
            blockCatchTrigger.SetEnable(false);
        }

		GameObject obj = null;
		switch (Player.BodyType) 
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

        if (obj)
        {
            GameObject obj2 = Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
            pushTrigger = obj2.transform.FindChild("Push").gameObject;
            elbowTrigger = obj2.transform.FindChild("Elbow").gameObject;
            blockTrigger = obj2.transform.FindChild("Block").gameObject;
            
            obj2.name = "BodyTrigger";
            PlayerTrigger[] objs = obj2.GetComponentsInChildren<PlayerTrigger>();
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; i ++)
                    objs [i].Player = this;
            }
            
            DefTrigger obj3 = obj2.GetComponentInChildren<DefTrigger>(); 
            if (obj3 != null)
                obj3.Player = this;
            
            DefPoint = obj2.transform.FindChild("DefRange").gameObject;          
			TopPoint = obj2.transform.FindChild("TriggerTop").gameObject; 
            obj2.transform.parent = transform;
            obj2.transform.transform.localPosition = Vector3.zero;
            obj2.transform.transform.localScale = Vector3.one;

            Transform t = obj2.transform.FindChild("TriggerFinger").gameObject.transform;
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
            DefPointCopy.transform.parent = gameObject.transform;
            DefPointCopy.name = "DefPoint";
            DefPointCopy.transform.localScale = Vector3.one;
            DefPointCopy.transform.localPosition = Vector3.zero;

            DefPointAy [EDefPointKind.Front.GetHashCode()] = DefPointCopy.transform.Find("Front").gameObject.transform;
            DefPointAy [EDefPointKind.Back.GetHashCode()] = DefPointCopy.transform.Find("Back").gameObject.transform;
            DefPointAy [EDefPointKind.Right.GetHashCode()] = DefPointCopy.transform.Find("Right").gameObject.transform;
            DefPointAy [EDefPointKind.Left.GetHashCode()] = DefPointCopy.transform.Find("Left").gameObject.transform;
            DefPointAy [EDefPointKind.FrontSteal.GetHashCode()] = DefPointCopy.transform.Find("FrontSteal").gameObject.transform;
            DefPointAy [EDefPointKind.BackSteal.GetHashCode()] = DefPointCopy.transform.Find("BackSteal").gameObject.transform;
            DefPointAy [EDefPointKind.RightSteal.GetHashCode()] = DefPointCopy.transform.Find("RightSteal").gameObject.transform;
            DefPointAy [EDefPointKind.LeftSteal.GetHashCode()] = DefPointCopy.transform.Find("LeftSteal").gameObject.transform;
        }
    }

    void FixedUpdate()
    {
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
		DebugTool ();
        
        if (WaitMoveTime > 0 && Time.time >= WaitMoveTime)
            WaitMoveTime = 0;

        if (Invincible > 0 && Time.time >= Invincible)
            Invincible = 0;

        if (CoolDownSteal > 0 && Time.time >= CoolDownSteal)
            CoolDownSteal = 0;

        if (CoolDownPush > 0 && Time.time >= CoolDownPush)
            CoolDownPush = 0;

        if (CoolDownElbow > 0 && Time.time >= CoolDownElbow)
            CoolDownElbow = 0;

        if (SlowDownTime > 0 && Time.time >= SlowDownTime)
        {
            SlowDownTime = 0;
			Attr.SpeedValue = GameData.BaseAttr [Player.AILevel].SpeedValue + (Player.Speed * 0.005f);
        }

        if (NoAiTime == 0)
        {
            if (FirstMoveQueue.Count > 0)
                MoveTo(FirstMoveQueue.Peek(), true);
            else 
            if (MoveQueue.Count > 0)
                MoveTo(MoveQueue.Peek());
            else
            {
                isMoving = false;
                if (IsDefence && (CheckAnimatorSate(EPlayerState.RunningDefence) || CheckAnimatorSate(EPlayerState.Defence1)))
                    AniState(EPlayerState.Defence0);
                else if (!IsDefence && !IsBallOwner && (CheckAnimatorSate(EPlayerState.Run0) || CheckAnimatorSate(EPlayerState.Run1)))
                    AniState(EPlayerState.Idle);
            }
        } else
        if (NoAiTime > 0 && Time.time >= NoAiTime)
        {
            MoveQueue.Clear();
            NoAiTime = 0;

            if (AIActiveHint)
                AIActiveHint.SetActive(true);

            if (SpeedUpView)
                SpeedUpView.enabled = false;

        }

//        if ((IsMoving || NoAiTime > 0) && !IsDefence && 
//            situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && 
//            situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
//        {
//            if (Time.time >= MoveStartTime)
//            {
//              MoveStartTime = Time.time + GameConst.DefMoveTime;
//              GameController.Get.DefMove(this);
//            }       
//        }

        if (situation == EGameSituation.AttackA || situation == EGameSituation.AttackB)
        {
            if (!IsDefence)
            {
                if (Time.time >= MoveStartTime)
                {
                    MoveStartTime = Time.time + GameConst.DefMoveTime;
                    GameController.Get.DefMove(this);
                }
            }
        }
        
        if (Time.time >= MovePowerTime)
        {
            MovePowerTime = Time.time + 0.05f;
            if (IsSpeedup)
            {
                if (MovePower > 0)
                {
                    MovePower -= 1;
                    if (MovePower < 0)
                        MovePower = 0;

                    if (this == GameController.Get.Joysticker)
                        GameController.Get.Joysticker.SpeedUpView.fillAmount = MovePower / MaxMovePower;

                    if (MovePower == 0)
                        CanSpeedup = false;
                }
            } else
            {
                if (MovePower < MaxMovePower)
                {
                    MovePower += 2.5f;
                    if (MovePower > MaxMovePower)
                        MovePower = MaxMovePower;

                    if (this == GameController.Get.Joysticker)
                        GameController.Get.Joysticker.SpeedUpView.fillAmount = MovePower / MaxMovePower;

                    if (MovePower == MaxMovePower)
                        CanSpeedup = true;
                }
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

    public void SetNoAiTime()
    {
        if (situation != EGameSituation.TeeA && situation != EGameSituation.TeeAPicking && situation != EGameSituation.TeeB && situation != EGameSituation.TeeBPicking)
        {
            isJoystick = true;
            NoAiTime = Time.time + GameData.Setting.AIChangeTime;
            
            if (AIActiveHint)
                AIActiveHint.SetActive(false);

            if (SpeedUpView)
                SpeedUpView.enabled = true;
        } else
        {
            NoAiTime = 0;
            if (AIActiveHint)
                AIActiveHint.SetActive(true);

            if (SpeedUpView)
                SpeedUpView.enabled = false;
        }
    }

    public void SetAiTime()
    {
        NoAiTime = 0;
        if (AIActiveHint)
            AIActiveHint.SetActive(true);
    }
    
//    private void CalculationAirResistance()
//    {
//        if (gameObject.transform.localPosition.y > 1f)
//        {
//            drag = Vector2.Lerp(Vector2.zero, new Vector2(0, gameObject.transform.localPosition.y), 0.01f); 
//            PlayerRigidbody.drag = drag.y;
//        } else
//        {
//            drag = Vector2.Lerp(new Vector2(0, gameObject.transform.localPosition.y), Vector2.zero, 0.01f); 
//            if (drag.y >= 0)
//                PlayerRigidbody.drag = drag.y;
//            else
//                PlayerRigidbody.drag = 0;
//        }
//    }

    private void CalculationDunkMove()
    {
        if (!isDunk)
            return;

        if (playerDunkCurve != null)
        {
            dunkCurveTime += Time.deltaTime;

            Vector3 position = gameObject.transform.position;
            position.y = playerDunkCurve.aniCurve.Evaluate(dunkCurveTime);

            if (position.y < 0)
                position.y = 0;

            gameObject.transform.position = new Vector3(gameObject.transform.position.x, position.y, gameObject.transform.position.z);

//          if (!isDunkZmove && 
            if (dunkCurveTime >= playerDunkCurve.StartMoveTime && dunkCurveTime <= playerDunkCurve.EndMoveTime)
            {
//                isDunkZmove = true;
                gameObject.transform.DOMoveZ(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.z, playerDunkCurve.ToBasketTime - playerDunkCurve.StartMoveTime).SetEase(Ease.Linear);
                gameObject.transform.DOMoveX(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.x, playerDunkCurve.ToBasketTime - playerDunkCurve.StartMoveTime).SetEase(Ease.Linear);
            }

            if (dunkCurveTime > playerDunkCurve.BlockMomentStartTime && dunkCurveTime <= playerDunkCurve.BlockMomentEndTime)
                IsCanBlock = true;
            else
                IsCanBlock = false;

            if (dunkCurveTime >= playerDunkCurve.LifeTime)
            {
                isDunk = false;
                IsCanBlock = false;
            }
        } else
        {
            isDunk = false;
            Debug.LogError("playCurve is null");
        }
    }

    private void CalculationLayupMove()
    {
        if (!isLayup)
            return;
        
        if (playerLayupCurve != null)
        {
            layupCurveTime += Time.deltaTime;
            
            Vector3 position = gameObject.transform.position;
            position.y = playerLayupCurve.aniCurve.Evaluate(layupCurveTime);
            
            if (position.y < 0)
                position.y = 0;
            
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, position.y, gameObject.transform.position.z);
            
            if (!isLayupZmove && layupCurveTime >= playerLayupCurve.StartMoveTime)
            {
                isLayupZmove = true;
				int add = (Team == 0? -1 : 1);
				gameObject.transform.DOMoveZ(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.z + add, playerLayupCurve.ToBasketTime - playerLayupCurve.StartMoveTime).SetEase(Ease.Linear);
                gameObject.transform.DOMoveX(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.x, playerLayupCurve.ToBasketTime - playerLayupCurve.StartMoveTime).SetEase(Ease.Linear);
            }
            
            if (layupCurveTime >= playerLayupCurve.LifeTime)
                isLayup = false;
        } else
        {
            isLayup = false;
            Debug.LogError("playCurve is null");
        }
    }

    private bool inReboundDistance()
    {
        return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), 
                                new Vector2(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z)) <= 6;
    }
    
    private void CalculationRebound()
    {
        if (isRebound && playerReboundCurve != null)
        {
            reboundCurveTime += Time.deltaTime;
			if (reboundCurveTime < 0.7f && !IsBallOwner && reboundMove != Vector3.zero)
            {
	            transform.position = new Vector3(transform.position.x + reboundMove.x * Time.deltaTime * 2, 
	                                             playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
	                                             transform.position.z + reboundMove.z * Time.deltaTime * 2);
            } else
                transform.position = new Vector3(transform.position.x + transform.forward.x * 0.05f, 
                                                 playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
                                                 transform.position.z + transform.forward.z * 0.05f);
            
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
            pushCurveTime += Time.deltaTime;

            if (pushCurveTime >= playerPushCurve.StartTime && pushCurveTime <= playerPushCurve.EndTime)
            {
                switch (playerPushCurve.Dir)
                {
                    case AniCurveDirection.Forward:
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * playerPushCurve.DirVaule), 0, 
                                                                gameObject.transform.position.z + (gameObject.transform.forward.z * playerPushCurve.DirVaule));
                        break;
                    case AniCurveDirection.Back:
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * -playerPushCurve.DirVaule), 0, 
                                                                gameObject.transform.position.z + (gameObject.transform.forward.z * -playerPushCurve.DirVaule));
                        break;
                }
            }

            if (pushCurveTime >= playerPushCurve.LifeTime)
                isPush = false;
        }
        
    }

    private void CalculationFall()
    {
        if (!isFall)
            return;
        
        if (playerFallCurve != null)
        {
            fallCurveTime += Time.deltaTime;
            
            if (fallCurveTime >= playerFallCurve.StartTime && fallCurveTime <= playerFallCurve.EndTime)
            {
                switch (playerFallCurve.Dir)
                {
                    case AniCurveDirection.Forward:
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * playerFallCurve.DirVaule), 0, 
                                                                gameObject.transform.position.z + (gameObject.transform.forward.z * playerFallCurve.DirVaule));
                        break;
                    case AniCurveDirection.Back:
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * -playerFallCurve.DirVaule), 0, 
                                                                gameObject.transform.position.z + (gameObject.transform.forward.z * -playerFallCurve.DirVaule));
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
            pickCurveTime += Time.deltaTime;
            
            if (pickCurveTime >= playerPickCurve.StartTime && pickCurveTime <= playerPickCurve.EndTime)
            {
                switch (playerPickCurve.Dir)
                {
                    case AniCurveDirection.Forward:
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * playerPickCurve.DirVaule), 0, 
                                                                gameObject.transform.position.z + (gameObject.transform.forward.z * playerPickCurve.DirVaule));
                        break;
                    case AniCurveDirection.Back:
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * -playerPickCurve.DirVaule), 0, 
                                                                gameObject.transform.position.z + (gameObject.transform.forward.z * -playerPickCurve.DirVaule));
                        break;
                }
            }
            
            if (pickCurveTime >= playerPickCurve.LifeTime)
                isPick = false;
        }
        
    }
    
    private void CalculationBlock()
    {
        if (!isBlock)
            return;

        if (playerBlockCurve != null)
        {
            blockCurveTime += Time.deltaTime;

            if (blockCurveTime < 1f)
                gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * 0.05f), 
                                                        playerBlockCurve.aniCurve.Evaluate(blockCurveTime), 
                                                        gameObject.transform.position.z + (gameObject.transform.forward.z * 0.05f));
            else
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 
                                                            playerBlockCurve.aniCurve.Evaluate(blockCurveTime), 
                                                            gameObject.transform.position.z);

            if (blockCurveTime >= playerBlockCurve.LifeTime)
            {
                isBlock = false;
                isCheckLayerToReset = true;
            }
        }
    }

    private void CalculationShootJump()
    {
        if (isShootJump && playerShootCurve != null)
        {
            shootJumpCurveTime += Time.deltaTime;
            switch (playerShootCurve.Dir)
            {
                case AniCurveDirection.Forward:
                    if (shootJumpCurveTime >= playerShootCurve.OffsetStartTime && shootJumpCurveTime < playerShootCurve.OffsetEndTime)
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * playerShootCurve.DirVaule), 
                                                            playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), 
                                                                gameObject.transform.position.z + (gameObject.transform.forward.z * playerShootCurve.DirVaule));
                    else
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x, playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), gameObject.transform.position.z);
                    break;
                case AniCurveDirection.Back:
                    if (shootJumpCurveTime >= playerShootCurve.OffsetStartTime && shootJumpCurveTime < playerShootCurve.OffsetEndTime)
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * -playerShootCurve.DirVaule), 
                                                            playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), 
                                                                gameObject.transform.position.z + (gameObject.transform.forward.z * -playerShootCurve.DirVaule));
                    else
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x, playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), gameObject.transform.position.z);
                    break;

                default : 
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), gameObject.transform.position.z);
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
            animator.SetFloat("MoveSpeed", animationSpeed);
        }
    }

    private bool isCheckLayerToReset = false;
    private bool isStartCheckLayer = false;

    private void CalculationPlayerHight()
    {
        animator.SetFloat("CrtHight", gameObject.transform.localPosition.y);

        if (isCheckLayerToReset)
        {
            if (gameObject.transform.localPosition.y > 0.2f)
                isStartCheckLayer = true;

            if (isStartCheckLayer && isTouchPalyer <= 0 && gameObject.transform.localPosition.y <= 0)
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
                isCheckLayerToReset = false;
                isStartCheckLayer = false;
            }
        }
    }

	public void DebugTool()
	{
		if(!GameStart.Get.IsDebugAnimation)
			return;

		//LayerCheck
		if (gameObject.transform.localPosition.y > 0.2f && gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Debug.LogError("Error Layer: " + gameObject.name + " . crtState : " + crtState);
		}

		//IdleAirCheck
		if (gameObject.transform.localPosition.y > 0.2f && crtState == EPlayerState.Idle && situation != EGameSituation.End)
		{
			Debug.LogError(gameObject.name + " : Error State : Idle in the Air ");
		}

		//Idle ballowner
		if(crtState == EPlayerState.Idle && IsBallOwner)
		{
			Debug.LogError(gameObject.name + " : Error State: Idle BallOWner");
		}

	}

    public void OnJoystickMove(MovingJoystick move, EPlayerState ps)
    {
        if (CanMove || stop || HoldBallCanMove)
        {
			if(IsFall && GameStart.Get.IsDebugAnimation){
				Debug.LogError("CanMove : " + CanMove);
				Debug.LogError("stop : " + stop);
				Debug.LogError("HoldBallCanMove : " + HoldBallCanMove);
			}

            if (situation != EGameSituation.TeeA && situation != EGameSituation.TeeAPicking && 
                situation != EGameSituation.TeeB && situation != EGameSituation.TeeBPicking)
            {
                if (Mathf.Abs(move.joystickAxis.y) > 0 || Mathf.Abs(move.joystickAxis.x) > 0)
                {
                    if (GameController.Get.CoolDownCrossover == 0 && !IsDefence && GameController.Get.DoPassiveSkill(ESkillSituation.MoveDodge, this))
                    {
                    
                    } else
                    {
                        isMoving = true;
                        if (!isJoystick)
                            MoveStartTime = Time.time + GameConst.DefMoveTime;

                        SetNoAiTime();
                        animationSpeed = Vector2.Distance(new Vector2(move.joystickAxis.x, 0), new Vector2(0, move.joystickAxis.y));
                        float angle = move.Axis2Angle(true);
                        int a = 90;
                        Vector3 rotation = new Vector3(0, angle + a, 0);
                        transform.rotation = Quaternion.Euler(rotation);

                        if (animationSpeed <= MoveMinSpeed || MovePower == 0)
                        {
                            
                            SetSpeed(0.3f, 0);
                            if (animationSpeed <= MoveMinSpeed)
                                IsSpeedup = false;
                            
                            if (IsBallOwner)
                            {                       
                                Translate = Vector3.forward * Time.deltaTime * Attr.SpeedValue * GameConst.BallOwnerSpeedNormal;
                                ps = EPlayerState.Dribble1;
                            } else
                            {
                                ps = EPlayerState.Run0;
                                if (IsDefence)
                                {
                                    Translate = Vector3.forward * Time.deltaTime * Attr.SpeedValue * GameConst.DefSpeedNormal;
                                } else
                                    Translate = Vector3.forward * Time.deltaTime * Attr.SpeedValue * GameConst.AttackSpeedNormal;
                            }                       
                        } else
                        {
                            IsSpeedup = true;
                            SetSpeed(1, 0);
                            if (IsBallOwner)
                            {
                                Translate = Vector3.forward * Time.deltaTime * Attr.SpeedValue * GameConst.BallOwnerSpeedup;
                                ps = EPlayerState.Dribble2;
                            } else
                            {
                                ps = EPlayerState.Run1;
                                if (IsDefence)
                                    Translate = Vector3.forward * Time.deltaTime * Attr.SpeedValue * GameConst.DefSpeedup;
                                else
                                    Translate = Vector3.forward * Time.deltaTime * Attr.SpeedValue * GameConst.AttackSpeedup;
                            }
                        }

                        transform.Translate(Translate); 
                        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                        
                        AniState(ps);
                    }
                }
            }            
        }
    }

    public void OnJoystickMoveEnd(MovingJoystick move, EPlayerState ps)
    {
        if (CanMove && 
            situation != EGameSituation.TeeA && situation != EGameSituation.TeeAPicking && 
            situation != EGameSituation.TeeB && situation != EGameSituation.TeeBPicking)
        {
            SetNoAiTime();
            isJoystick = false;
            IsSpeedup = false;

            if (crtState != ps)
                AniState(ps);

            if (crtState == EPlayerState.Dribble0)
            {
                if (situation == EGameSituation.AttackA)
                    rotateTo(CourtMgr.Get.ShootPoint [0].transform.position.x, CourtMgr.Get.ShootPoint [0].transform.position.z);
                else if (situation == EGameSituation.AttackB)
                    rotateTo(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);
            }
        }

        isMoving = false;
    }

    private bool GetMoveTarget(ref TMoveData Data, ref Vector2 Result)
    {
        bool ResultBool = false;
        Result = Vector2.zero;

        if (Data.DefPlayer != null)
        {
            if (Data.DefPlayer.Index == Index && AutoFollow)
            {
                Result.x = Data.DefPlayer.transform.position.x;
                Result.y = Data.DefPlayer.transform.position.z;
                ResultBool = true;
            } else
            {
                Vector3 aP1 = Data.DefPlayer.transform.position;
                Vector3 aP2 = CourtMgr.Get.Hood [Data.DefPlayer.Team.GetHashCode()].transform.position;
                Result = GetStealPostion(aP1, aP2, Data.DefPlayer.Index);
                if (Vector2.Distance(Result, new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)) <= GameConst.StealBallDistance)
                {
                    if (Math.Abs(GameController.Get.GetAngle(Data.DefPlayer, this)) >= 30 && 
                        Vector3.Distance(aP2, DefPlayer.transform.position) <= GameConst.TreePointDistance + 3)
                    {
                        ResultBool = true;
                    } else
                    {
                        Result.x = gameObject.transform.position.x;
                        Result.y = gameObject.transform.position.z;
                    }
                } else
                    ResultBool = true;
            }
        } else if (Data.FollowTarget != null)
        {
            Result.x = Data.FollowTarget.position.x;
            Result.y = Data.FollowTarget.position.z;

            if (Vector2.Distance(Result, new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)) <= MoveCheckValue)
            {
                Result.x = gameObject.transform.position.x;
                Result.y = gameObject.transform.position.z;
            } else
                ResultBool = true;
        } else
        {
            Result = Data.Target;
            ResultBool = true;
        }

        return ResultBool;
    }
    
    public void MoveTo(TMoveData Data, bool First = false)
    {
        if ((CanMove || (NoAiTime == 0 && HoldBallCanMove)) && WaitMoveTime == 0 && GameStart.Get.TestMode != EGameTest.Block)
        {
            bool DoMove = GetMoveTarget(ref Data, ref MoveTarget);
            float temp = Vector2.Distance(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z), MoveTarget);
            SetSpeed(0.3f, 0);

            if (temp <= MoveCheckValue || !DoMove)
            {
                MoveTurn = 0;
                isMoving = false;
                
                if (IsDefence)
                {
                    WaitMoveTime = 0;
                    if (Data.DefPlayer != null)
                    {
                        dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);
                        
                        if (Data.LookTarget != null)
                        {
                            if (Vector3.Distance(this.transform.position, Data.DefPlayer.transform.position) <= GameConst.StealBallDistance)
                                rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
                            else if (!DoMove)
                                rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
                            else if (dis > GameConst.TreePointDistance + 4 && (Data.DefPlayer.NoAiTime == 0 && (Data.DefPlayer.WaitMoveTime == 0 || Data.DefPlayer.TargetPosNum > 0)))
                                rotateTo(MoveTarget.x, MoveTarget.y);
                            else
                                rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
                        } else
                            rotateTo(MoveTarget.x, MoveTarget.y);
                    } else
                    {
                        if (Data.LookTarget == null)
                            rotateTo(MoveTarget.x, MoveTarget.y);
                        else
                            rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
                    }
                    
                    AniState(EPlayerState.Defence0);                          
                } else
                {
                    if (!IsBallOwner)
                        AniState(EPlayerState.Idle);
                    else if (situation == EGameSituation.TeeA || situation == EGameSituation.TeeB)
                        AniState(EPlayerState.Dribble0);
                    
                    if (First || GameStart.Get.TestMode == EGameTest.Edit)
                        WaitMoveTime = 0;
                    else if (situation != EGameSituation.TeeA && situation != EGameSituation.TeeAPicking && situation != EGameSituation.TeeB && situation != EGameSituation.TeeBPicking)
                    {
                        dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint [Team.GetHashCode()].transform.position);
                        if (dis <= 8)
                            WaitMoveTime = Time.time + UnityEngine.Random.Range(0, 1);
                        else
                            WaitMoveTime = Time.time + UnityEngine.Random.Range(0, 3);
                    }
                    
                    if (IsBallOwner)
                    {
                        if (Team == ETeamKind.Self)
                            rotateTo(CourtMgr.Get.ShootPoint [0].transform.position.x, CourtMgr.Get.ShootPoint [0].transform.position.z);
                        else
                            rotateTo(CourtMgr.Get.ShootPoint [1].transform.position.x, CourtMgr.Get.ShootPoint [1].transform.position.z);
                        
                        if (Data.Shooting && NoAiTime == 0)
                            GameController.Get.Shoot();
                    } else
                    {
                        if (Data.LookTarget == null)
                        {
                            if (GameController.Get.BallOwner != null)
                            {
                                rotateTo(GameController.Get.BallOwner.transform.position.x, GameController.Get.BallOwner.transform.position.z);
                            } else
                            {
                                if (Team == ETeamKind.Self)
                                    rotateTo(CourtMgr.Get.ShootPoint [0].transform.position.x, CourtMgr.Get.ShootPoint [0].transform.position.z);
                                else
                                    rotateTo(CourtMgr.Get.ShootPoint [1].transform.position.x, CourtMgr.Get.ShootPoint [1].transform.position.z);
                            }
                        } else
                            rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
                        
                        if (Data.Catcher)
                        {
                            if ((situation == EGameSituation.AttackA || situation == EGameSituation.AttackB))
                            {
                                if (GameController.Get.Pass(this, false, false, true))
                                    NeedShooting = Data.Shooting;
                            }
                        }
                    }
                }
                
                if (Data.MoveFinish != null)
                    Data.MoveFinish(this, Data.Speedup);
                
                if (First && FirstMoveQueue.Count > 0)
                    FirstMoveQueue.Dequeue();
                else if (MoveQueue.Count > 0)
                    MoveQueue.Dequeue();
            } else if ((IsDefence == false && MoveTurn >= 0 && MoveTurn <= 5) && GameController.Get.BallOwner != null)
            {                                          
                MoveTurn++;
                rotateTo(MoveTarget.x, MoveTarget.y);
                if (MoveTurn == 1)
                    MoveStartTime = Time.time + GameConst.DefMoveTime;           
            } else
            {
                if (IsDefence)
                {
                    if (Data.DefPlayer != null)
                    {
                        dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);
                        
                        if (dis <= GameConst.TreePointDistance + 4)
                        {
                            rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
                        } else
                        {
                            if (Vector3.Distance(transform.position, Data.LookTarget.position) <= 1.5f)
                            {
                                rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
                            } else
                            {
                                rotateTo(MoveTarget.x, MoveTarget.y);
                            }
                        }

                        if (Math.Abs(GameController.Get.GetAngle(this, new Vector3(MoveTarget.x, 0, MoveTarget.y))) >= 90)
                            AniState(EPlayerState.Defence1);
                        else
                            AniState(EPlayerState.RunningDefence);
                    } else
                    {
                        rotateTo(MoveTarget.x, MoveTarget.y);
                        AniState(EPlayerState.Run0);
                    }
                    
                    isMoving = true;
                    if (MovePower > 0 && CanSpeedup && this != GameController.Get.Joysticker && !IsTee)
                    {
                        SetSpeed(1, 0);
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * GameConst.DefSpeedup * Attr.SpeedValue);
                        IsSpeedup = true;
                    } else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * GameConst.DefSpeedNormal * Attr.SpeedValue);
                        IsSpeedup = false;
                    }
                } else
                {
                    rotateTo(MoveTarget.x, MoveTarget.y);                   
                    isMoving = true;

                    if (IsBallOwner)
                    {
                        if (Data.Speedup && MovePower > 0)
                        {
                            SetSpeed(1, 0);
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedup * Attr.SpeedValue);
                            AniState(EPlayerState.Dribble2);
                            IsSpeedup = true;
                        } else
                        {
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedNormal * Attr.SpeedValue);
                            AniState(EPlayerState.Dribble1);
                            IsSpeedup = false;
                        }
                    } else
                    {
                        if (Data.Speedup && MovePower > 0)
                        {
                            SetSpeed(1, 0);
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedup * Attr.SpeedValue);
                            AniState(EPlayerState.Run1);
                            IsSpeedup = true;
                        } else
                        {
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedNormal * Attr.SpeedValue);
                            AniState(EPlayerState.Run0);
                            IsSpeedup = false;
                        }
                    }
                }
            } 
        } else
            isMoving = false;
    }

    public void rotateTo(float lookAtX, float lookAtZ)
    {
        if (isBlock)
            return;

        gameObject.transform.LookAt(new Vector3(lookAtX, gameObject.transform.position.y, lookAtZ));

//      Debug.Log ("Roatte To .GameObject : " + gameObject.name);
//        transform.rotation = Quaternion.Lerp(transform.rotation, 
//                             Quaternion.LookRotation(new Vector3(lookAtX, transform.localPosition.y, lookAtZ) - 
//            transform.localPosition), time * Time.deltaTime);

//        Vector3 lookAtPos = new Vector3(lookAtX, gameObject.transform.position.y, lookAtZ);
//        Vector3 relative = gameObject.transform.InverseTransformPoint(lookAtPos);
//        float mangle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

//        if ((mangle > 15 && mangle < 180) || (mangle < -15 && mangle > -180))
//        {
//            gameObject.transform.DOLookAt(lookAtPos, 0.1f);
//        }
    }
    
    public void SetInvincible(float time)
    {
        if (Invincible == 0)
            Invincible = Time.time + time;
        else
            Invincible += time;
    }
    
    private void SetSpeed(float value, int dir = -2)
    {
        //dir : 1 ++, -1 --, -2 : not smooth,  
//        if (dir == 0)
        animator.SetFloat("MoveSpeed", value);
//        else
//        if (dir != -2)
//            smoothDirection = dir;
    }

    private void AddActionFlag(EActionFlag Flag)
    {
        GameFunction.Add_ByteFlag(Flag.GetHashCode(), ref PlayerActionFlag);
        animator.SetBool(Flag.ToString(), true);
    }

    public void DelActionFlag(EActionFlag Flag)
    {
        GameFunction.Del_ByteFlag(Flag.GetHashCode(), ref PlayerActionFlag);
        animator.SetBool(Flag.ToString(), false);
    }

    public bool CheckAnimatorSate(EPlayerState state)
    {
        if (crtState == state)
            return true;
        else
            return false;
    }
    
    public void ResetFlag(bool ClearMove = true)
    {
        if (CheckAnimatorSate(EPlayerState.Idle) || CheckAnimatorSate(EPlayerState.Dribble1) || CheckAnimatorSate(EPlayerState.Dribble0))
        {
            NeedResetFlag = false;

			for (int i = 0; i < PlayerActionFlag.Length; i++)
				PlayerActionFlag [i] = 0;

			if(!IsBallOwner){
            	AniState(EPlayerState.Idle);
			}
			else
				AniState(EPlayerState.Dribble0);

            if (ClearMove)
            {
                MoveQueue.Clear();
                FirstMoveQueue.Clear();
            }

            WaitMoveTime = 0;
            NeedShooting = false;
            isJoystick = false; 
            isMoving = false;
            IsSpeedup = false;
            CanSpeedup = true;
        } else
            NeedResetFlag = true;
    }

    public void ClearMoveQueue()
    {
        MoveQueue.Clear();
        FirstMoveQueue.Clear();
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
			if (!IsPass && !IsAllShoot && (crtState == EPlayerState.HoldBall || IsDribble))
                {
                    return true;
                }
                break;

            case EPlayerState.Pass4:
				if ((crtState == EPlayerState.Shoot0 || crtState == EPlayerState.Shoot2) && !GameController.Get.Shooter && IsPassAirMoment && crtState != state)
                    return true;
                break;
            
            case EPlayerState.BlockCatch:
                if (crtState == EPlayerState.Block && crtState != EPlayerState.BlockCatch) 
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
				if (IsBallOwner && !IsAllShoot && (crtState == EPlayerState.Idle || crtState == EPlayerState.HoldBall || IsDribble))
					return true;
				break;

            case EPlayerState.Layup0:
            case EPlayerState.Layup1:
            case EPlayerState.Layup2:
            case EPlayerState.Layup3:
                if (IsBallOwner && !IsAllShoot && (crtState == EPlayerState.Idle || crtState == EPlayerState.HoldBall || IsDribble))
                    return true;
                break;

            case EPlayerState.Dunk0:
            case EPlayerState.Dunk2:
            case EPlayerState.Dunk4:
            case EPlayerState.Dunk6:
            case EPlayerState.Dunk20:
                if (IsBallOwner && !IsAllShoot && (crtState == EPlayerState.Idle || crtState == EPlayerState.HoldBall || IsDribble))
                if (Vector3.Distance(CourtMgr.Get.ShootPoint [Team.GetHashCode()].transform.position, gameObject.transform.position) < canDunkDis)
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

            case EPlayerState.Rebound:
                if (CanMove && crtState != EPlayerState.Rebound)
                    return true;
                break;

            case EPlayerState.TipIn:
                if (crtState == EPlayerState.Rebound && crtState != EPlayerState.TipIn)
                    return true;

                break;
           
            case EPlayerState.PickBall0:
            case EPlayerState.PickBall2:
                if (CanMove && !IsBallOwner && (crtState == EPlayerState.Idle || crtState == EPlayerState.Run0 || crtState == EPlayerState.Run1 || crtState == EPlayerState.Defence1 ||
                    crtState == EPlayerState.Defence0 || crtState == EPlayerState.RunningDefence))
                    return true;
                break;

            case EPlayerState.Push:
            case EPlayerState.Steal:
                if (!IsTee && CanMove && !IsBallOwner && (crtState == EPlayerState.Idle || crtState == EPlayerState.Run0 || crtState == EPlayerState.Run1 || crtState == EPlayerState.Defence1 ||
                    crtState == EPlayerState.Defence0 || crtState == EPlayerState.RunningDefence))
                    return true;
                break;

            case EPlayerState.Block:
                if (!IsTee && CanMove && !IsBallOwner && (crtState == EPlayerState.Idle || crtState == EPlayerState.Run0 || crtState == EPlayerState.Run1 || crtState == EPlayerState.Defence1 ||
                    crtState == EPlayerState.Defence0 || crtState == EPlayerState.RunningDefence || IsDunk))
                    return true;
                break;

            case EPlayerState.Elbow:
                if (!IsTee && IsBallOwner && (crtState == EPlayerState.Dribble0 || crtState == EPlayerState.Dribble1 || crtState == EPlayerState.HoldBall))
                    return true;
                break;

            case EPlayerState.Fall0:
            case EPlayerState.Fall1:
            case EPlayerState.Fall2:
                if (!IsTee && crtState != state && crtState != EPlayerState.Elbow && 
                    (crtState == EPlayerState.Dribble0 || crtState == EPlayerState.Dribble1 || crtState == EPlayerState.Dribble2 || crtState == EPlayerState.HoldBall || IsDunk ||
                    crtState == EPlayerState.Idle || crtState == EPlayerState.Run0 || crtState == EPlayerState.Run1 || crtState == EPlayerState.Defence0 || crtState == EPlayerState.Defence1 || 
                    crtState == EPlayerState.RunningDefence) && Invincible == 0)
                    return true;
                break;

            case EPlayerState.GotSteal:
                if (!IsTee && crtState != state && crtState != EPlayerState.Elbow && 
                    (crtState == EPlayerState.Dribble0 ||
                    crtState == EPlayerState.Dribble1 || 
                    crtState == EPlayerState.Dribble2 || 
                    crtState == EPlayerState.FakeShoot || 
                    crtState == EPlayerState.HoldBall || 
                    crtState == EPlayerState.Idle || 
                    crtState == EPlayerState.Run0 ||
                    crtState == EPlayerState.Run1 ||
                    crtState == EPlayerState.Defence0 || 
                    crtState == EPlayerState.Defence1 || 
                    crtState == EPlayerState.RunningDefence))
                    return true;
                break;

            case EPlayerState.Dribble0:
            case EPlayerState.Dribble1:
            case EPlayerState.Dribble2:
			if (IsFirstDribble && !IsAllShoot && !CanMove || (CanMove && crtState != state && (!IsPass)) || (crtState == EPlayerState.MoveDodge0 || crtState == EPlayerState.MoveDodge1))
                {
                    return true;
                }
                break;
            
            case EPlayerState.Run0:   
            case EPlayerState.Run1:   
            case EPlayerState.RunningDefence:
            case EPlayerState.Defence0:
            case EPlayerState.Defence1:
				if (crtState != state)
					return true;
				break;
            case EPlayerState.MoveDodge0:
            case EPlayerState.MoveDodge1:
                if (crtState != state && !IsPass && !IsAllShoot)
                    return true;
                break;
            case EPlayerState.CatchFlat:
            case EPlayerState.CatchFloor:
            case EPlayerState.CatchParabola:
            case EPlayerState.Intercept0:
            case EPlayerState.Intercept1:
                if (CanMove)
                    return true;
                break;

            case EPlayerState.Idle:
                return true;
        }

        return false;
    }

    public bool IsTee
    { 
        get
        {
            return (situation == EGameSituation.TeeA || situation == EGameSituation.TeeAPicking || situation == EGameSituation.TeeB || situation == EGameSituation.TeeBPicking);
        }
    }

    public bool AniState(EPlayerState state, Vector3 v)
    {
        if (!CanUseState(state))
            return false;

        rotateTo(v.x, v.z);
        return AniState(state);
    }

    public bool AniState(EPlayerState state)
    {
        if (!CanUseState(state))
            return false;

        bool Result = false;
        int stateNo = 0;
        string curveName = string.Empty;
		bool isFindCurve = false;
        PlayerRigidbody.mass = 0;
		DribbleTime = 0;

		if(GameStart.Get.IsDebugAnimation)
			Debug.Log ("Do ** " + gameObject.name + ".CrtState : " + crtState + "  : state : " + state);
        
        switch (state)
        {
            case EPlayerState.Block:
                SetShooterLayer();
                playerBlockCurve = null;
				curveName = "Block";

                for (int i = 0; i < aniCurve.Block.Length; i++)
					if (aniCurve.Block [i].Name == curveName)
					{
                        playerBlockCurve = aniCurve.Block [i];
						isFindCurve = true;
						blockCurveTime = 0;
						isBlock = true;
					}

                ClearAnimatorFlag();
                animator.SetTrigger("BlockTrigger");
                isCanCatchBall = false;
				GameRecord.BlockLaunch++;
                Result = true;
                break;

            case EPlayerState.BlockCatch:
                ClearAnimatorFlag();
                animator.SetTrigger("BlockCatchTrigger");
                IsPerfectBlockCatch = false;
                isCanCatchBall = false;
                Result = true;
                break;

            case EPlayerState.CatchFlat:
				PlayerRigidbody.useGravity = true;
                animator.SetInteger("StateNo", 0);
                SetSpeed(0, -1);
                ClearAnimatorFlag();
                animator.SetTrigger("CatchTrigger");
                Result = true;
                break;

            case EPlayerState.CatchFloor:
				PlayerRigidbody.useGravity = true;
                animator.SetInteger("StateNo", 2);
                SetSpeed(0, -1);
                ClearAnimatorFlag();
                animator.SetTrigger("CatchTrigger");
                Result = true;
                break;
                
            case EPlayerState.CatchParabola:
				PlayerRigidbody.useGravity = true;
                animator.SetInteger("StateNo", 1);
                SetSpeed(0, -1);
                ClearAnimatorFlag();
                animator.SetTrigger("CatchTrigger");
                Result = true;
                break;

            case EPlayerState.Defence0:
				PlayerRigidbody.useGravity = true;
                PlayerRigidbody.mass = 5;
                ClearAnimatorFlag();
                SetSpeed(0, -1);
                AddActionFlag(EActionFlag.IsDefence);
                Result = true;
                break;

            case EPlayerState.Defence1:
				PlayerRigidbody.useGravity = true;
                isCanCatchBall = true;
                SetSpeed(1, 1);
                ClearAnimatorFlag(EActionFlag.IsDefence);
                Result = true;
                break;

            case EPlayerState.Alleyoop:
            case EPlayerState.Dunk0:
            case EPlayerState.Dunk2:
            case EPlayerState.Dunk4:
            case EPlayerState.Dunk6:
            case EPlayerState.Dunk20:
                switch (state)
                {
                    case EPlayerState.Dunk0:
                    case EPlayerState.Alleyoop:
                        stateNo = 0;
                        break;
                    case EPlayerState.Dunk2:
                        stateNo = 2;
                        break;
                    case EPlayerState.Dunk4:
                        stateNo = 4;
                        break;
					case EPlayerState.Dunk6:
						stateNo = 6;
						break;
                    case EPlayerState.Dunk20:
                        stateNo = 20;
                        break;
                }
                PlayerRigidbody.useGravity = false;
                ClearAnimatorFlag();
                animator.SetInteger("StateNo", stateNo);
                animator.SetTrigger("DunkTrigger");
                isCanCatchBall = false;

                playerDunkCurve = null;

				curveName = string.Format("Dunk{0}", stateNo);

                for (int i = 0; i < aniCurve.Dunk.Length; i++)
					if (aniCurve.Dunk [i].Name == curveName)
                    {
                        playerDunkCurve = aniCurve.Dunk [i];
                        isDunk = true;
//                        isDunkZmove = false;
                        dunkCurveTime = 0;
						isFindCurve = true;
                    }
                SetShooterLayer();
                Result = true;
                break;

            case EPlayerState.Dribble0:
            case EPlayerState.Dribble1:
            case EPlayerState.Dribble2:
				PlayerRigidbody.useGravity = true;
                if (GameController.Get.BallOwner == this)
                {
                    switch (state)
                    {
                        case EPlayerState.Dribble0:
                            PlayerRigidbody.mass = 5;
                            stateNo = 0;
                            break;
                        case EPlayerState.Dribble1:
                            PlayerRigidbody.mass = 0;
                            stateNo = 1;
                            break;
                        case EPlayerState.Dribble2:
                            PlayerRigidbody.mass = 0;
                            stateNo = 2;
                            break;
                    }
//                    if (!isJoystick)
//                        SetSpeed(0, -1);
                    ClearAnimatorFlag();
                    animator.SetInteger("StateNo", stateNo);
                    AddActionFlag(EActionFlag.IsDribble);
                    CourtMgr.Get.SetBallState(EPlayerState.Dribble0, this);
                    isCanCatchBall = false;
                    IsFirstDribble = false;
                    Result = true;
                }
                break;

            case EPlayerState.Elbow:
				PlayerRigidbody.useGravity = true;
                PlayerRigidbody.mass = 5;
                ClearAnimatorFlag();
                animator.SetTrigger("ElbowTrigger");
                isCanCatchBall = false;
				GameRecord.ElbowLaunch++;
                Result = true;
                break;

            case EPlayerState.FakeShoot:
				PlayerRigidbody.useGravity = true;
                if (IsBallOwner)
                {
                    PlayerRigidbody.mass = 5;
                    ClearAnimatorFlag();
                    animator.SetTrigger("FakeShootTrigger");
                    isCanCatchBall = false;
                    isFakeShoot = true;
					GameRecord.Fake++;
                    Result = true;
                }
                break;

            case EPlayerState.Fall0:
            case EPlayerState.Fall1:
            case EPlayerState.Fall2:
                switch (state)
                {
                    case EPlayerState.Fall0:
                        stateNo = 0;
                        break;
                    case EPlayerState.Fall1:
                        stateNo = 1;
                        break;
                    case EPlayerState.Fall2:
                        stateNo = 2;
                        break;
                }
				SetShooterLayer();
                curveName = string.Format("Fall{0}", stateNo);
                playerFallCurve = null;

                for (int i = 0; i < aniCurve.Fall.Length; i++)
                    if (curveName == aniCurve.Fall [i].Name)
                    {
                        playerFallCurve = aniCurve.Fall [i];
                        fallCurveTime = 0;
                        isFall = true;
						isFindCurve = true;
                    }

                isDunk = false;
                isShootJump = false;
                ClearAnimatorFlag();
                animator.SetInteger("StateNo", stateNo);
                animator.SetTrigger("FallTrigger");
                isCanCatchBall = false;
                gameObject.transform.DOLocalMoveY(0, 1f);
                if (OnFall != null)
                    OnFall(this);
                Result = true;
                break;

            case EPlayerState.HoldBall:
				PlayerRigidbody.useGravity = true;
                PlayerRigidbody.mass = 5;
                ClearAnimatorFlag();
                AddActionFlag(EActionFlag.IsHoldBall);
                isCanCatchBall = false;
                Result = true;
                break;
            
            case EPlayerState.Idle:
				PlayerRigidbody.useGravity = true;
                PlayerRigidbody.mass = 5;
                SetSpeed(0, -1);
                ClearAnimatorFlag();
                isCanCatchBall = true;
                isMoving = false;
                Result = true;
                break;

            case EPlayerState.Intercept0:
                animator.SetInteger("StateNo", 0);
                animator.SetTrigger("InterceptTrigger");
                ClearAnimatorFlag();
                Result = true;
                break;

            case EPlayerState.Intercept1:
                animator.SetInteger("StateNo", 1);
                animator.SetTrigger("InterceptTrigger");
                ClearAnimatorFlag();
                Result = true;
                break;
            
            case EPlayerState.MoveDodge0:
				PlayerRigidbody.useGravity = true;
                animator.SetInteger("StateNo", 0);
                animator.SetTrigger("MoveDodge");
                OnUICantUse(this);
                Result = true;
                break;

            case EPlayerState.MoveDodge1:
				PlayerRigidbody.useGravity = true;
                animator.SetInteger("StateNo", 1);
                animator.SetTrigger("MoveDodge");
                OnUICantUse(this);
                Result = true;
                break;

            case EPlayerState.Pass0:
            case EPlayerState.Pass1:
            case EPlayerState.Pass2:
            case EPlayerState.Pass3:
            case EPlayerState.Pass4:
            case EPlayerState.Pass5:
            case EPlayerState.Pass6:
            case EPlayerState.Pass7:
            case EPlayerState.Pass8:
            case EPlayerState.Pass9:
                switch (state)
                {
                    case EPlayerState.Pass0:
                        stateNo = 0;
                        break;
                    case EPlayerState.Pass1:
                        stateNo = 1;
                        break;
                    case EPlayerState.Pass2:
                        stateNo = 2;
                        break;
                    case EPlayerState.Pass3:
                        stateNo = 3;
                        break;
                    case EPlayerState.Pass4:
                        stateNo = 4;
                        break;
					case EPlayerState.Pass5:
						stateNo = 5;
						break;
					case EPlayerState.Pass6:
						stateNo = 6;
						break;
					case EPlayerState.Pass7:
						stateNo = 7;
						break;
					case EPlayerState.Pass8:
						stateNo = 8;
						break;
					case EPlayerState.Pass9:
						stateNo = 9;
						break;
                }
                ClearAnimatorFlag();
				PlayerRigidbody.useGravity = true;
                PlayerRigidbody.mass = 5;
                animator.SetInteger("StateNo", stateNo);
                animator.SetTrigger("PassTrigger");
				GameRecord.Pass++;
                Result = true;
                break;

            case EPlayerState.Push:
                ClearAnimatorFlag();
                playerPushCurve = null;
				curveName = "Push0";
                for (int i = 0; i < aniCurve.Push.Length; i++)
					if (aniCurve.Push [i].Name == curveName)
                    {
                        playerPushCurve = aniCurve.Push [i];
                        pushCurveTime = 0;
                        isPush = true;
						isFindCurve = true;
                    }
				PlayerRigidbody.useGravity = true;
                animator.SetTrigger("PushTrigger");
				GameRecord.PushLaunch++;
                Result = true;
                break;

            case EPlayerState.PickBall0:
                isCanCatchBall = true;
                ClearAnimatorFlag();
				PlayerRigidbody.useGravity = true;
                animator.SetInteger("StateNo", 0);
                animator.SetTrigger("PickTrigger");
                Result = true;
                break;

            case EPlayerState.PickBall2:
                isCanCatchBall = true;
                ClearAnimatorFlag();
				curveName = "PickBall2";
				PlayerRigidbody.useGravity = true;

                for (int i = 0; i < aniCurve.Push.Length; i++)
					if (aniCurve.PickBall [i].Name == curveName)
                    {
                        playerPickCurve = aniCurve.PickBall [i];
                        pickCurveTime = 0;
                        isPick = true;
						isFindCurve = true;
                    }
                animator.SetInteger("StateNo", 2);
                animator.SetTrigger("PickTrigger");
				GameRecord.SaveBallLaunch++;
                Result = true;
                break;

            case EPlayerState.Run0:
            case EPlayerState.Run1:
                if (!isJoystick)
                    SetSpeed(1, 1); 

                switch (state)
                {
                    case EPlayerState.Run0:
                        stateNo = 0;
                        break;
                    case EPlayerState.Run1:
                        stateNo = 1;
                        break;
                }
				PlayerRigidbody.useGravity = true;
                animator.SetInteger("StateNo", stateNo);
                ClearAnimatorFlag(EActionFlag.IsRun);
                Result = true;
                break;
    
            case EPlayerState.RunningDefence:
                SetSpeed(1, 1);
				PlayerRigidbody.useGravity = true;
                ClearAnimatorFlag(EActionFlag.IsRun);
                Result = true;
                break;

            case EPlayerState.Steal:
				PlayerRigidbody.useGravity = true;
                PlayerRigidbody.mass = 5;
                ClearAnimatorFlag();
                animator.SetTrigger("StealTrigger");
                isCanCatchBall = false;
				GameRecord.StealLaunch++;
                Result = true;
                break;

            case EPlayerState.GotSteal:
                ClearAnimatorFlag();
                animator.SetTrigger("GotStealTrigger");
                isCanCatchBall = false;
                Result = true;
                break;

            case EPlayerState.Shoot0:
            case EPlayerState.Shoot1:
            case EPlayerState.Shoot2:
            case EPlayerState.Shoot3:
            case EPlayerState.Shoot4:
            case EPlayerState.Shoot5:
            case EPlayerState.Shoot6:
			case EPlayerState.Shoot7:
                if (IsBallOwner)
                {                   
                    playerShootCurve = null;
                    
                    switch (state)
                    {
                        case EPlayerState.Shoot0:
                            IsPassAirMoment = true;
                            stateNo = 0;
                            break;
                        case EPlayerState.Shoot1:
                            stateNo = 1;
                            break;
                        case EPlayerState.Shoot2:
                            IsPassAirMoment = true;
                            stateNo = 2;
                            break;
                        case EPlayerState.Shoot3:
                            stateNo = 3;
                            break;
						case EPlayerState.Shoot4:
							stateNo = 4;
							break;
						case EPlayerState.Shoot5:
							stateNo = 5;
							break;
                        case EPlayerState.Shoot6:
                            stateNo = 6;
                            break;
						case EPlayerState.Shoot7:
							stateNo = 7;
							break;
                    }
                
                    animator.SetInteger("StateNo", stateNo);
                    curveName = string.Format("Shoot{0}", stateNo);
					
                    for (int i = 0; i < aniCurve.Shoot.Length; i++)
                        if (aniCurve.Shoot [i].Name == curveName)
                        {
                            playerShootCurve = aniCurve.Shoot [i];
                            shootJumpCurveTime = 0;
                            isShootJump = true;
							isFindCurve = true;
                            continue;
                        }
                    SetShooterLayer();
                    ClearAnimatorFlag();
                    animator.SetTrigger("ShootTrigger");
                    isCanCatchBall = false;
                    Result = true;
                }
                break;

            case EPlayerState.Layup0:
            case EPlayerState.Layup1:
            case EPlayerState.Layup2:
            case EPlayerState.Layup3:
                if (IsBallOwner)
                {
                    playerLayupCurve = null;

					switch (state)
					{
						case EPlayerState.Layup0:
							stateNo = 0;
							break;
						case EPlayerState.Layup1:
							stateNo = 1;
							break;
						case EPlayerState.Layup2:
							stateNo = 2;
							break;
						case EPlayerState.Layup3:
							stateNo = 3;
							break;
					}

					curveName = string.Format("Layup{0}", stateNo);
					
					for (int i = 0; i < aniCurve.Layup.Length; i++)
						if (aniCurve.Layup [i].Name == curveName)
					{
						playerLayupCurve = aniCurve.Layup [i];
						layupCurveTime = 0;
						isLayup = true;
						isLayupZmove = false;
						isFindCurve = true;
					}
					SetShooterLayer();
					ClearAnimatorFlag();
					animator.SetTrigger("LayupTrigger");
					isCanCatchBall = false;
	                Result = true;
                }
                break;

            case EPlayerState.Rebound:
                playerReboundCurve = null;

                if (inReboundDistance()) {
                    reboundMove = CourtMgr.Get.RealBall.transform.position - transform.position;
				    reboundMove += CourtMgr.Get.RealBallRigidbody.velocity * 0.3f;
				} else
                    reboundMove = Vector3.zero;

				curveName = "Rebound0";

                for (int i = 0; i < aniCurve.Rebound.Length; i++)
					if (aniCurve.Rebound [i].Name == curveName)
                    {
                        playerReboundCurve = aniCurve.Rebound [i];
                        reboundCurveTime = 0;
                        isRebound = true;
						isFindCurve = true;
                    }

                ClearAnimatorFlag();
                SetShooterLayer();
                animator.SetTrigger("ReboundTrigger");
				GameRecord.ReboundLaunch++;
                Result = true;
                break;

            case EPlayerState.TipIn:
                ClearAnimatorFlag();
                SetShooterLayer();
                animator.SetTrigger("TipInTrigger");
                Result = true;
                break;

            case EPlayerState.ReboundCatch:
                animator.SetTrigger("ReboundCatchTrigger");
                break;
        }

		if(curveName != string.Empty && !isFindCurve)
			DebugAnimationCurve(curveName);
        
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
		if(GameStart.Get.IsDebugAnimation)
			Debug.LogError("Can not Find aniCurve: " + curveName);
	}

    public void SetShooterLayer()
    {
        isStartCheckLayer = false;
        gameObject.layer = LayerMask.NameToLayer("Shooter");
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
        switch (animationName)
        {
            case "Stealing":
                if (OnStealMoment != null)
                    if (OnStealMoment(this))
						GameRecord.Steal++;

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

            case "DoubleClickMoment":
                if (OnDoubleClickMoment != null)
                    OnDoubleClickMoment(this, crtState);
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

            case "BlockCatching":
//              if(OnBlockCatching != null)
//                  OnBlockCatching(this);
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

            case "Blocking":
//                if (OnBlocking != null)
//                    OnBlocking(this);

                break;
            case "Shooting":
                IsPassAirMoment = false;
                if (OnShooting != null && crtState != EPlayerState.Pass4)
                    OnShooting(this);
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
                    CourtMgr.Get.RealBallTrigger.PassBall(animator.GetInteger("StateNo"));      
                break;
            case "PassEnd":
                OnUI(this);
                
                if (!IsBallOwner && gameObject.transform.localPosition.y < 0.2f)
                    AniState(EPlayerState.Idle);
                break;

            case "PickUp": 
                if (OnPickUpBall != null)
                    if (OnPickUpBall(this))
						GameRecord.SaveBall++;

                break;
            case "PickEnd":
                AniState(EPlayerState.Dribble0);
                break;

            case "PushCalculateStart":
                pushTrigger.gameObject.SetActive(true);
                break;

            case "PushCalculateEnd":
                pushTrigger.SetActive(false);
                break;

            case "ElbowCalculateStart":
                elbowTrigger.gameObject.SetActive(true);
                break;
                
            case "ElbowCalculateEnd":
                elbowTrigger.SetActive(false);
                break;

            case "BlcokCalculateStart":
                blockTrigger.gameObject.SetActive(true);
                break;

            case "BlcokCalculateEnd":
                blockTrigger.gameObject.SetActive(false);
                break;
            case "CloneMesh":
                if (!IsBallOwner)
                    EffectManager.Get.CloneMesh(gameObject, playerDunkCurve.CloneMaterial, 
                        playerDunkCurve.CloneDeltaTime, playerDunkCurve.CloneCount);

                break;

            case "DunkJump":
//                DelActionFlag(ActionFlag.IsDribble);
//                DelActionFlag(ActionFlag.IsRun);
            
                CourtMgr.Get.SetBallState(EPlayerState.Dunk0);
                if (OnDunkJump != null)
                    OnDunkJump(this);

                break;
            case "OnlyScore":
                if (OnOnlyScore != null)
                    OnOnlyScore(this);
                CourtMgr.Get.PlayDunk(Team.GetHashCode(), animator.GetInteger("StateNo"));
                break;
            case "DunkBasket":
//                DelActionFlag(ActionFlag.IsDribble);
//                DelActionFlag(ActionFlag.IsRun);
                CourtMgr.Get.PlayDunk(Team.GetHashCode(), animator.GetInteger("StateNo"));

                break;
            case "DunkFallBall":
                OnUI(this);
                if (OnDunkBasket != null)
                    OnDunkBasket(this);

                break;

            case "ElbowEnd":
                OnUI(this);
                AniState(EPlayerState.HoldBall);
                GameController.Get.RealBallFxTime = 1f;
                CourtMgr.Get.RealBallFX.SetActive(true);
                break;

            case "CatchEnd":
                if (situation == EGameSituation.TeeA || situation == EGameSituation.TeeB)
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
                        if (NoAiTime == 0)
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
                GameController.Get.RealBallFxTime = 1f;
                CourtMgr.Get.RealBallFX.SetActive(true);
                break;

            case "AnimationEnd":
                OnUI(this);

				if(!IsBallOwner)
               	 AniState(EPlayerState.Idle);
				else{
					if(firstDribble)
						AniState(EPlayerState.Dribble0);
					else
						AniState(EPlayerState.HoldBall);
				}

                blockTrigger.SetActive(false);
                pushTrigger.SetActive(false);
                elbowTrigger.SetActive(false);
                isCanCatchBall = true;
                PlayerRigidbody.useGravity = true;
                IsPerfectBlockCatch = false;
                isRebound = false;
                isPush = false;
                blockCatchTrigger.enabled = false;

                if (!NeedResetFlag)
                    isCheckLayerToReset = true;
                else
                    ResetFlag();

                break;
        }
    }

    public void ResetMove()
    {
        MoveQueue.Clear();
        WaitMoveTime = 0;
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

    public EPlayerState PassiveSkill(ESkillSituation situation, ESkillKind kind, Vector3 v = default(Vector3)) {
        EPlayerState playerState = EPlayerState.Idle;
		playerState = (EPlayerState)System.Enum.Parse(typeof(EPlayerState), situation.ToString());
        bool isPerformPassive = false;
		float angle = GameFunction.GetPlayerToObjectAngleByVector(this.transform, v);
		if(passiveSkills.ContainsKey((int)kind)) {
			if (passiveSkills[(int)kind].Count > 0){
	            int passiveRate = 0;
	            if (kind == ESkillKind.Pass) {
					if (angle < 60f && angle > -60f)
						passDirect = EPassDirectState.Forward;
					else if (angle <= -60f && angle > -120f)
						passDirect = EPassDirectState.Left;
	                else if (angle < 120f && angle >= 60f)
						passDirect = EPassDirectState.Right;
					else if (angle >= 120f && angle >= -120f)
						passDirect = EPassDirectState.Back; 
	                
					for (int i=0; i<passiveSkills[(int)kind].Count; i++) {
						if ((passiveSkills[(int)kind][i].Kind % 10) == (int)passDirect){
							passiveRate += passiveSkills[(int)kind] [i].Rate;
						}
	                }
	            } else {
					for (int i=0; i<passiveSkills[(int)kind].Count; i++)
						passiveRate += passiveSkills[(int)kind] [i].Rate;
	            }
	            isPerformPassive = (UnityEngine.Random.Range(0, 100) <= passiveRate) ? true : false;
	        }
		}

        if (isPerformPassive){
			string animationName = string.Empty;
			if (kind == ESkillKind.Pass){
				if (passivePassDirects.ContainsKey((int) passDirect)){
					for (int i=0; i<passivePassDirects [(int)passDirect].Count; i++)
					{
						if (UnityEngine.Random.Range(0, 100) <= passivePassDirects [(int)passDirect][i].Rate)
						{
							animationName = passivePassDirects [(int)passDirect][i].Name;
							break;       
						}
					}
				}
			} else 
			if (passiveSkills.ContainsKey((int)kind)) {
				for (int i=0; i<passiveSkills[(int)kind].Count; i++) {
					if (UnityEngine.Random.Range(0, 100) <= passiveSkills[(int)kind] [i].Rate){
						animationName = passiveSkills[(int)kind] [i].Name;
						break;       
					}
				}
			}

			if (animationName != string.Empty) {
				if(IsBallOwner && GameController.Get.Joysticker == this)
					GameController.Get.ShowPassiveEffect();

				GameRecord.PassiveSkill++;
				return (EPlayerState)System.Enum.Parse(typeof(EPlayerState), animationName);
			} else 
				return playerState;
        } else
            return playerState;

//        return playerState;
    }

    public float GetActiveTime(string name)
    {
        float time = 0;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        if (clips != null && clips.Length > 0)
        {
            for (int i=0; i<clips.Length; i++)
            {
                if (clips [i].name.Equals(name))
                {
                    time = clips [i].length;
                }
            }
		}
//        return activeTime;
		return time;
    }

    public bool IsHaveMoveDodge
    {
        get
        {
            return isHaveMoveDodge;
        }
    }

	public bool IsHavePickBall2
	{
		get
		{
			return isHavePickBall2;
		}
	}
	
	public bool CanMove
	{
		get
		{
            EPlayerState[] CheckAy = {
                EPlayerState.Block,
                EPlayerState.BlockCatch,
                EPlayerState.CatchFlat,
                EPlayerState.CatchFloor,
                EPlayerState.CatchParabola,
                EPlayerState.Alleyoop,
                EPlayerState.Elbow,
                EPlayerState.FakeShoot,
                EPlayerState.HoldBall,
                EPlayerState.GotSteal,
                EPlayerState.Pass0,
                EPlayerState.Pass2,
                EPlayerState.Pass1,
                EPlayerState.Pass3,
                EPlayerState.Pass4,
                EPlayerState.Push,
                EPlayerState.PickBall0,
                EPlayerState.PickBall2,
                EPlayerState.Steal,
                EPlayerState.Rebound,
                EPlayerState.ReboundCatch,
                EPlayerState.TipIn,
                EPlayerState.Intercept0,
                EPlayerState.Intercept1,
                EPlayerState.MoveDodge0,
                EPlayerState.MoveDodge1
            };

            for (int i = 0; i < CheckAy.Length; i++)
                if (CheckAnimatorSate(CheckAy [i]))
                    return false;

			if(IsFall || IsShoot || IsDunk || IsLayup)
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
            if (CourtMgr.Get.RealBallFX.activeSelf != value)
                CourtMgr.Get.RealBallFX.SetActive(value);
        }
    }

    public bool IsDefence
    {
        get
        {
            if ((situation == EGameSituation.AttackA || situation == EGameSituation.TeeA || situation == EGameSituation.TeeAPicking) && Team == ETeamKind.Npc)
                return true;
            else if ((situation == EGameSituation.AttackB || situation == EGameSituation.TeeB || situation == EGameSituation.TeeBPicking) && Team == ETeamKind.Self)
                return true;
            else
                return false;
        }
    }
    
    public bool IsJump
    {
        get{ return gameObject.transform.localPosition.y > 0.1f;}
    }

    public bool IsBallOwner
    {
//        get { return SceneMgr.Get.RealBall.transform.parent == DummyBall.transform;}
        get { return animator.GetBool("IsBallOwner");}
        set { animator.SetBool("IsBallOwner", value);}
    }

	public bool IsBlock
	{
		get{ return crtState == EPlayerState.Block ;}
	}

    public bool IsShoot
    {
        get
        {
            return crtState == EPlayerState.Shoot0 || crtState == EPlayerState.Shoot1 || crtState == EPlayerState.Shoot2 || crtState == EPlayerState.Shoot3 ||
				crtState == EPlayerState.Shoot4 || crtState == EPlayerState.Shoot5 || crtState == EPlayerState.Shoot6 || crtState == EPlayerState.Shoot7 || crtState == EPlayerState.TipIn;
        }
    }

	public bool IsAllShoot
	{
		get{ return IsShoot || IsDunk || IsLayup;}
	}

	public bool IsIdle
	{
		get{ return crtState == EPlayerState.Idle;}
	}

	public bool IsRun
	{
		get{ return crtState == EPlayerState.Run0 || crtState == EPlayerState.Run1;}
	}

    public bool IsPass
    {
        get{ return crtState == EPlayerState.Pass0 || crtState == EPlayerState.Pass2 || crtState == EPlayerState.Pass1 || crtState == EPlayerState.Pass3 ||
			crtState == EPlayerState.Pass5 || crtState == EPlayerState.Pass6 || crtState == EPlayerState.Pass7 || crtState == EPlayerState.Pass8 || crtState == EPlayerState.Pass9;}
    }

    public bool IsDribble
    {
        get{ return crtState == EPlayerState.Dribble0 || crtState == EPlayerState.Dribble1 || crtState == EPlayerState.Dribble2;}
    }

    public bool IsDunk
    {
		get{ return crtState == EPlayerState.Dunk0 || crtState == EPlayerState.Dunk2 || crtState == EPlayerState.Dunk4 || crtState == EPlayerState.Dunk6 || crtState == EPlayerState.Dunk20;}
    }

	public bool IsLayup
	{
		get{ return crtState == EPlayerState.Layup0 || crtState == EPlayerState.Layup1 || crtState == EPlayerState.Layup2 || crtState == EPlayerState.Layup3;}
	}

    public bool IsUseSkill
    {
        get{ return crtState == EPlayerState.Dunk20;}
    }

    private bool isMoving = false;

    public bool IsMoving
    {
        get{ return isMoving;}
    }

    public bool IsRebound
    {
        get{ return crtState == EPlayerState.Rebound || crtState == EPlayerState.ReboundCatch;}
    }

    public bool IsFall
    {
        get{ return crtState == EPlayerState.Fall0 || crtState == EPlayerState.Fall1 || crtState == EPlayerState.Fall2;}
        
    }

    public bool IsCatch
    {
        get{ return crtState == EPlayerState.CatchFlat || crtState == EPlayerState.CatchFloor || crtState == EPlayerState.CatchParabola;}
    }

    public bool IsFakeShoot
    {
        get{ return isFakeShoot;}
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
//                  EffectManager.Get.PlayEffect("DoubleClick01", Vector3.zero, null, gameObject, 1f);
        }
    }

    public bool IsFirstDribble
    {
        get{ return firstDribble;}
        set{ firstDribble = value;}
    }

    public int TargetPosNum
    {
        get { return MoveQueue.Count;}
    }

    public string TargetPosName
    {
        get
        { 
            if (MoveQueue.Count == 0)
                return "";
            else
                return MoveQueue.Peek().FileName;
        }
    }

    public TMoveData TargetPos
    {
        set
        {
            if (MoveQueue.Count == 0)
                MoveTurn = 0;

            MoveQueue.Enqueue(value);

			if (value.Target != Vector2.zero)
				GameRecord.PushMove(value.Target);
        }
    }

    public int FirstTargetPosNum
    {
        get { return FirstMoveQueue.Count;}
    }

    public TMoveData FirstTargetPos
    {
        set
        {
            if (FirstMoveQueue.Count < 2) {
                FirstMoveQueue.Enqueue(value);
				GameRecord.PushMove(value.Target);
			}
        }
    }

    private void setMovePower(float Value)
    {
        MaxMovePower = Value;
        MovePower = Value;
    }

    private int isTouchPalyer = 0;

    public void IsTouchPlayerForCheckLayer(int index)
    {
        isTouchPalyer += index;
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
            float angle = Math.Abs(GameController.Get.GetAngle(GameController.Get.BallOwner, GameController.Get.BallOwner.DefPlayer));

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