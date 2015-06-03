using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RootMotion.FinalIK;
using GameStruct;

public delegate bool OnPlayerAction(PlayerBehaviour player);
public delegate bool OnPlayerAction2(PlayerBehaviour player,bool speedup);
public delegate bool OnPlayerAction3(PlayerBehaviour player, PlayerState state);

public enum PlayerState
{
    Idle = 0,
    Run0 = 1,            
    Run1 = 2,            
    Block = 3,  
    Board = 4,  
    Defence0 = 5,    
	Defence1 = 6,
	RunningDefence = 7,
    
    Fall0 = 9,
    Fall1 = 10,
    Fall2 = 11,
    BlockCatch = 12,
    Layup = 13, 
    Steal = 14,
    GotSteal = 15,
    Pass0 = 16,
    Pass2 = 17,
    Pass1 = 18,
    Pass4 = 19,
    Pass5 = 20,
    Push = 21,

	Rebound = 25,
	ReboundCatch = 26,
	DunkBasket = 27,
   
    FakeShoot = 29,
    Reset = 30,
    Start = 31,
    Tee = 32,
    BasketAnimationStart = 33,
    BasketActionEnd = 34,
    BasketActionSwish = 35,
    BasketActionSwishEnd = 36,
    Elbow = 37,
    HoldBall = 38,
    CatchFlat = 39,
    CatchParabola = 40,
    CatchFloor = 41,

    Shoot0 = 43,
    Shoot1 = 44,
    Shoot2 = 45,
    Shoot3 = 46,

    Shoot6 = 49,
	BasketActionNoScoreEnd = 50,
	TipIn = 51,
	Alleyoop = 52,
	Intercept0 = 53,
	Intercept1 = 54,
	MoveDodge0 = 55,
	MoveDodge1 = 56,
	PickBall0 = 57,
	PickBall2 = 58,
	Dribble0 = 59,
	Dribble1 = 60,
	Dribble2 = 61,
	Dunk0 = 100,
	Dunk2 = 102,
	Dunk4 = 104,
	Dunk20 = 120
}

public enum TeamKind
{
    Self = 0,
    Npc = 1
}

public enum DefPointKind
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

public enum ActionFlag
{
	None = 0,
    IsRun = 1,
    IsDefence = 2,
    IsDribble = 3,
    IsHoldBall = 4,
}

public enum BallDirection
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

public struct PassiveSkill{
	public int ID;
	public int Kind;
	public string Name;
	public int Rate;
}

public class PlayerBehaviour : MonoBehaviour
{
	public string TacticalName = "";
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
	public OnPlayerAction OnUI = null;
	public OnPlayerAction OnUISkill = null;
	public OnPlayerAction OnUIMoveDodge = null;
	public OnPlayerAction3 OnDoubleClickMoment = null;

    public float[] DunkHight = new float[2]{3, 5};
    private const float MoveCheckValue = 1;
    public static string[] AnimatorStates = new string[] {"", "IsRun", "IsDefence", "IsDribble", "IsHoldBall"};
    private byte[] PlayerActionFlag = {0, 0, 0, 0, 0, 0, 0};
    private Vector2 drag = Vector2.zero;
    private bool stop = false;
    private bool NeedResetFlag = false;
    private int MoveTurn = 0;
    private float MoveStartTime = 0;
    private float TimeProactiveRate = 0;
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
    private GameObject pushTrigger;
    private GameObject elbowTrigger;
    private GameObject blockTrigger;
    private BlockCatchTrigger blockCatchTrigger;
    public GameObject AIActiveHint = null;
    public GameObject DummyBall;
	public GameObject DummyCatch;
	public UISprite SpeedUpView = null;
	public UISprite AngerView = null;
	public GameObject AngryFull = null;

    public TeamKind Team;
    public float NoAiTime = 0;
    public bool HaveNoAiTime = false;
    public int Index;
    public GameSituation situation = GameSituation.None;
    public PlayerState crtState = PlayerState.Idle;
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
	public GameStruct.TPlayerAttribute Attr;
	public GameStruct.TPlayer Player;

    //Dunk
    private bool isDunk = false;
    private bool isDunkZmove = false;
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
	private Dictionary<int, List<PassiveSkill>> passiveSkill = new Dictionary<int, List<PassiveSkill>>();
	private bool isHaveMoveDodge;
	private PassDirectState passDirect = PassDirectState.Forward;

	private bool firstDribble = true;
	public TScoreRate ScoreRate;
	private bool isCanCatchBall = true;
	
	private bool IsSpeedup = false;
	public float MovePower = 0;
	public float MaxMovePower = 0;
	private float MovePowerTime = 0;
	private Vector2 MoveTarget;
	private float dis;
	private bool CanSpeedup = true;
	public int AngerPower = 0;
	public Material BodyMaterial;
	private float SlowDownTime = 0;

	public void SetAnger(int Value)
	{
		AngerPower += Value;
		if(AngerPower > 100)
			AngerPower = 100;

		if(AngerPower < 0)
			AngerPower = 0;

		if(this == GameController.Get.Joysticker)
		{
			float temp = AngerPower;
			GameController.Get.Joysticker.AngerView.fillAmount = temp / 100;
			if(GameController.Get.Joysticker.AngerView.fillAmount == 1)
				OnUISkill(this);
		}
	}

	public void SetSlowDown(float Value)
	{
		if(SlowDownTime == 0)
		{
			SlowDownTime += Time.time + Value;
			Attr.SpeedValue = GameData.BaseAttr[Player.AILevel].SpeedValue * GameConst.SlowDownValue;
		}
	}
    
    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        gameObject.tag = "Player";

        animator = gameObject.GetComponent<Animator>();
        PlayerRigidbody = gameObject.GetComponent<Rigidbody>();
        DummyBall = gameObject.transform.FindChild("DummyBall").gameObject;
//		DummyCatch = gameObject.transform.FindChild("DummyCatch").gameObject;

        ScoreRate = GameStart.Get.ScoreRate;
    }

	public void InitAttr()
	{
		if (GameData.BaseAttr.Length > 0 && Player.AILevel >= 0 && Player.AILevel < GameData.BaseAttr.Length) 
		{
			Attr.PointRate2 = Math.Min(GameData.BaseAttr[Player.AILevel].PointRate2, 100);
			Attr.PointRate3 = Math.Min(GameData.BaseAttr[Player.AILevel].PointRate3, 100);
			Attr.StealRate = Math.Min(GameData.BaseAttr[Player.AILevel].StealRate + (Player.Steal / 12), 100);
			Attr.DunkRate = Math.Min(GameData.BaseAttr[Player.AILevel].DunkRate, 100);
			Attr.TipInRate = Math.Min(GameData.BaseAttr[Player.AILevel].TipInRate, 100);
			Attr.AlleyOopRate = Math.Min(GameData.BaseAttr[Player.AILevel].AlleyOopRate, 100);
			Attr.StrengthRate = Math.Min(GameData.BaseAttr[Player.AILevel].StrengthRate, 100);
			Attr.BlockPushRate = Math.Min(GameData.BaseAttr[Player.AILevel].BlockPushRate, 100);
			Attr.ElbowingRate = Math.Min(GameData.BaseAttr[Player.AILevel].ElbowingRate, 100);
			Attr.ReboundRate = Math.Min(GameData.BaseAttr[Player.AILevel].ReboundRate, 100);
			Attr.BlockRate = Math.Min(GameData.BaseAttr[Player.AILevel].BlockRate, 100);
			Attr.FaketBlockRate = Math.Min(GameData.BaseAttr[Player.AILevel].FaketBlockRate, 100);
			Attr.JumpBallRate = Math.Min(GameData.BaseAttr[Player.AILevel].JumpBallRate, 100);
			Attr.PushingRate = Math.Min(GameData.BaseAttr[Player.AILevel].PushingRate + (Player.Defence * 1.2f), 100);
			Attr.PassRate = Math.Min(GameData.BaseAttr[Player.AILevel].PassRate, 100);
			Attr.AlleyOopPassRate = Math.Min(GameData.BaseAttr[Player.AILevel].AlleyOopPassRate, 100);
			Attr.ReboundHeadDistance = GameData.BaseAttr[Player.AILevel].ReboundHeadDistance;
			Attr.ReboundHandDistance = GameData.BaseAttr[Player.AILevel].ReboundHandDistance;
			Attr.BlockDistance = GameData.BaseAttr[Player.AILevel].BlockDistance;
			Attr.DefDistance = GameData.BaseAttr[Player.AILevel].DefDistance + (Player.Defence / 10);
			Attr.SpeedValue = GameData.BaseAttr[Player.AILevel].SpeedValue;
			Attr.StaminaValue = GameData.BaseAttr[Player.AILevel].StaminaValue;
			Attr.AutoFollowTime = GameData.BaseAttr[Player.AILevel].AutoFollowTime;

			DefPoint.transform.localScale = new Vector3(Attr.DefDistance, Attr.DefDistance, Attr.DefDistance);
			if(Attr.StaminaValue > 0)
				SetMovePower(Attr.StaminaValue);

			//collect player's passiveSkill
			if(Player.Skills != null && Player.Skills.Length > 0) {
				for(int i=0; i<Player.Skills.Length; i++)  {
					if(Player.Skills[i].ID > 0) {
						if(GameData.SkillData.ContainsKey(Player.Skills[i].ID)) {
							if(GameData.SkillData[Player.Skills[i].ID].Kind == (int)TSkillKind.MoveDodge) 
								isHaveMoveDodge = true;
							int rate = GameData.SkillData[Player.Skills[i].ID].BaseRate + (GameData.SkillData[Player.Skills[i].ID].AddRate * Player.Skills[i].Lv);
							PassiveSkill ps = new PassiveSkill();
							ps.ID = Player.Skills[i].ID;
							ps.Name = GameData.SkillData[Player.Skills[i].ID].Animation;
							ps.Rate = rate;
							ps.Kind = GameData.SkillData[Player.Skills[i].ID].Kind;
							int key = GameData.SkillData[Player.Skills[i].ID].Kind ;
							if(key > 1000)
								key = (key / 100);
							if(passiveSkill.ContainsKey(key)) {
								List<PassiveSkill> pss = passiveSkill[key];
								pss.Add(ps);
								passiveSkill[key] = pss;
							} else {
								List<PassiveSkill> pss = new List<PassiveSkill>();
								pss.Add(ps);
								passiveSkill.Add(key, pss);
							}
						}
					}
				}
			}
		}
	}

	public void InitCurve(GameObject animatorCurve) {
		GameObject AnimatorCurveCopy = Instantiate(animatorCurve) as GameObject;
		AnimatorCurveCopy.transform.parent = gameObject.transform;
		AnimatorCurveCopy.name = "AniCurve";
		aniCurve = AnimatorCurveCopy.GetComponent<AniCurve>();
    }

	public void InitTrigger(GameObject defPoint)
	{
		GameObject obj = Resources.Load("Prefab/Player/BodyTrigger") as GameObject;
		if (obj)
		{
			GameObject obj2 = Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
			pushTrigger = obj2.transform.FindChild("Push").gameObject;
			elbowTrigger = obj2.transform.FindChild("Elbow").gameObject;
			blockTrigger = obj2.transform.FindChild("Block").gameObject;
			blockCatchTrigger = DummyBall.GetComponent<BlockCatchTrigger>();
			blockCatchTrigger.SetEnable(false);
			
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
			obj2.transform.parent = transform;
			obj2.transform.transform.localPosition = Vector3.zero;
			obj2.transform.transform.localScale = Vector3.one;

			Transform t = obj2.transform.FindChild("TriggerFinger").gameObject.transform;
			if (t) {
				t.name = Team.GetHashCode().ToString() + Index.ToString() + "TriggerFinger";
				t.parent = transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Bip01 R Finger2/Bip01 R Finger21/");
				t.localPosition = Vector3.zero;
				t.localScale = Vector3.one;
			}
		}
		
		if (defPoint != null) {
			GameObject DefPointCopy = Instantiate(defPoint, Vector3.zero, Quaternion.identity) as GameObject;
			DefPointCopy.transform.parent = gameObject.transform;
			DefPointCopy.name = "DefPoint";
			DefPointCopy.transform.localScale = Vector3.one;
			DefPointCopy.transform.localPosition = Vector3.zero;

			DefPointAy [DefPointKind.Front.GetHashCode()] = DefPointCopy.transform.Find ("Front").gameObject.transform;
			DefPointAy [DefPointKind.Back.GetHashCode()] = DefPointCopy.transform.Find ("Back").gameObject.transform;
			DefPointAy [DefPointKind.Right.GetHashCode()] = DefPointCopy.transform.Find ("Right").gameObject.transform;
			DefPointAy [DefPointKind.Left.GetHashCode()] = DefPointCopy.transform.Find ("Left").gameObject.transform;
			DefPointAy [DefPointKind.FrontSteal.GetHashCode()] = DefPointCopy.transform.Find ("FrontSteal").gameObject.transform;
			DefPointAy [DefPointKind.BackSteal.GetHashCode()] = DefPointCopy.transform.Find ("BackSteal").gameObject.transform;
			DefPointAy [DefPointKind.RightSteal.GetHashCode()] = DefPointCopy.transform.Find ("RightSteal").gameObject.transform;
			DefPointAy [DefPointKind.LeftSteal.GetHashCode()] = DefPointCopy.transform.Find ("LeftSteal").gameObject.transform;
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
		CalculationPush ();
		CalculationFall ();
		CalculationPick ();
		
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
			Attr.SpeedValue = GameData.BaseAttr[Player.AILevel].SpeedValue;
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
			}
        } else
        if (NoAiTime > 0 && Time.time >= NoAiTime)
        {
            MoveQueue.Clear();
            NoAiTime = 0;

            if (AIActiveHint)
                AIActiveHint.SetActive(true);

			if(SpeedUpView)
				SpeedUpView.enabled = false;

        }

//        if ((IsMoving || NoAiTime > 0) && !IsDefence && 
//            situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && 
//            situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
//        {
//            if (Time.time >= MoveStartTime)
//            {
//				MoveStartTime = Time.time + GameConst.DefMoveTime;
//				GameController.Get.DefMove(this);
//            }       
//        }

		if(situation == GameSituation.AttackA || situation == GameSituation.AttackB)
		{
			if(!IsDefence)
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
			if(IsSpeedup)
			{
				if(MovePower > 0)
				{
					MovePower -= 1;
					if(MovePower < 0)
						MovePower = 0;

					if(this == GameController.Get.Joysticker)
						GameController.Get.Joysticker.SpeedUpView.fillAmount = MovePower / MaxMovePower;

					if(MovePower == 0)
						CanSpeedup = false;
				}
			}
			else
			{
				if(MovePower < MaxMovePower)
				{
					MovePower += 2.5f;
					if(MovePower > MaxMovePower)
						MovePower = MaxMovePower;

					if(this == GameController.Get.Joysticker)
						GameController.Get.Joysticker.SpeedUpView.fillAmount = MovePower / MaxMovePower;

					if(MovePower == MaxMovePower)
						CanSpeedup = true;
				}
			}	
		}

        if (IsDefence)
        {
            if (Time.time >= ProactiveTime)
            {
                ProactiveTime = Time.time + 4;
				TimeProactiveRate = UnityEngine.Random.Range(0, 100) + 1;
            }

            if (AutoFollow)
            {
                Vector3 ShootPoint;
                if (Team == TeamKind.Self)
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
        if (situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
        {
            isJoystick = true;
			NoAiTime = Time.time + GameData.Setting.AIChangeTime;
            
            if (AIActiveHint)
                AIActiveHint.SetActive(false);

			if(SpeedUpView)
				SpeedUpView.enabled = true;
        } else
        {
            NoAiTime = 0;
            if (AIActiveHint)
                AIActiveHint.SetActive(true);

			if(SpeedUpView)
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

//			if (!isDunkZmove && 
			if(dunkCurveTime >= playerDunkCurve.StartMoveTime && dunkCurveTime <= playerDunkCurve.EndMoveTime)
            {
//                isDunkZmove = true;
                gameObject.transform.DOMoveZ(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.z, playerDunkCurve.ToBasketTime - playerDunkCurve.StartMoveTime).SetEase(Ease.Linear);
                gameObject.transform.DOMoveX(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.x, playerDunkCurve.ToBasketTime - playerDunkCurve.StartMoveTime).SetEase(Ease.Linear);
            }

			if(dunkCurveTime > playerDunkCurve.BlockMomentStartTime && dunkCurveTime <= playerDunkCurve.BlockMomentEndTime)
				IsCanBlock = true;
			else
				IsCanBlock = false;

            if (dunkCurveTime >= playerDunkCurve.LifeTime){
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
			layupCurveTime +=  Time.deltaTime;
			
			Vector3 position = gameObject.transform.position;
			position.y = playerLayupCurve.aniCurve.Evaluate(layupCurveTime);
			
			if (position.y < 0)
				position.y = 0;
			
			gameObject.transform.position = new Vector3(gameObject.transform.position.x, position.y, gameObject.transform.position.z);
			
			if (!isLayupZmove && layupCurveTime >= playerLayupCurve.StartMoveTime)
			{
				isLayupZmove = true;
				gameObject.transform.DOMoveZ(CourtMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.z, playerLayupCurve.ToBasketTime - playerLayupCurve.StartMoveTime).SetEase(Ease.Linear);
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

	private bool inReboundDistance() {
		return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), 
		                        new Vector2(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z)) <= 6;
    }
	
	private void CalculationRebound()
	{
		if (isRebound && playerReboundCurve != null) {
			reboundCurveTime += Time.deltaTime;
			if (reboundCurveTime < 0.7f && !IsBallOwner) {
				if (reboundMove != Vector3.zero) {
					transform.position = new Vector3(transform.position.x + reboundMove.x * Time.deltaTime * 2, 
		                                             playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
					                                 transform.position.z + reboundMove.z * Time.deltaTime * 2);
                } else
					transform.position = new Vector3(transform.position.x + transform.forward.x * 0.05f, 
		                                             playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
		                                             transform.position.z + transform.forward.z * 0.05f);
            } else
				transform.position = new Vector3(transform.position.x + transform.forward.x * 0.05f, 
				                                 playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
				                                 transform.position.z + transform.forward.z * 0.05f);
			
			if (reboundCurveTime >= playerReboundCurve.LifeTime) {
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

			if(pushCurveTime >= playerPushCurve.StartTime && pushCurveTime <= playerPushCurve.EndTime){
				switch(playerPushCurve.Dir){
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

			if(pushCurveTime >= playerPushCurve.LifeTime)
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
			
			if(fallCurveTime >= playerFallCurve.StartTime && fallCurveTime <= playerFallCurve.EndTime){
				switch(playerFallCurve.Dir){
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
			
			if(fallCurveTime >= playerFallCurve.LifeTime)
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
			
			if(pickCurveTime >= playerPickCurve.StartTime && pickCurveTime <= playerPickCurve.EndTime){
				switch(playerPickCurve.Dir){
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
			
			if(pickCurveTime >= playerPickCurve.LifeTime)
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
			switch(playerShootCurve.Dir){
				case AniCurveDirection.Forward:
					if(shootJumpCurveTime >= playerShootCurve.OffsetStartTime && shootJumpCurveTime < playerShootCurve.OffsetEndTime)
						gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * playerShootCurve.DirVaule), 
				                                            playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), 
					                                            gameObject.transform.position.z + (gameObject.transform.forward.z * playerShootCurve.DirVaule));
					else
						gameObject.transform.position = new Vector3(gameObject.transform.position.x, playerShootCurve.aniCurve.Evaluate(shootJumpCurveTime), gameObject.transform.position.z);
					break;
				case AniCurveDirection.Back:
					if(shootJumpCurveTime >= playerShootCurve.OffsetStartTime && shootJumpCurveTime < playerShootCurve.OffsetEndTime)
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

		if(isCheckLayerToReset){
			if (gameObject.transform.localPosition.y > 0.2f)
				isStartCheckLayer = true;

			if (isStartCheckLayer && isTouchPalyer <= 0 && gameObject.transform.localPosition.y < 0.2f)
			{
				gameObject.layer = LayerMask.NameToLayer("Player");
				isCheckLayerToReset = false;
				isStartCheckLayer = false;
			}

			if (gameObject.transform.localPosition.y > 0.2f && gameObject.layer == LayerMask.NameToLayer ("Player")) {
				Debug.Log("Error : " + gameObject.name + " . crtState : " + crtState);
			}
		}

    }

    public void OnJoystickMove(MovingJoystick move, PlayerState ps)
    {
        if (CanMove || stop || HoldBallCanMove)
        {
            if (situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && 
			    situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
            {
                if (Mathf.Abs(move.joystickAxis.y) > 0 || Mathf.Abs(move.joystickAxis.x) > 0)
                {
					if (GameController.Get.CoolDownCrossover == 0 && !IsDefence && GameController.Get.DoPassiveSkill(TSkillSituation.MoveDodge, this))
					{
					
					}
					else
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
							if(animationSpeed <= MoveMinSpeed)
								IsSpeedup = false;
							
							if (IsBallOwner){						
								Translate = Vector3.forward * Time.deltaTime * Attr.SpeedValue * GameConst.BallOwnerSpeedNormal;
								ps = PlayerState.Dribble1;
							}
							else
							{
								ps = PlayerState.Run0;
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
							if (IsBallOwner){
								Translate = Vector3.forward * Time.deltaTime * Attr.SpeedValue * GameConst.BallOwnerSpeedup;
								ps = PlayerState.Dribble2;
							}
							else
							{
								ps = PlayerState.Run1;
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

    public void OnJoystickMoveEnd(MovingJoystick move, PlayerState ps)
    {
		if (CanMove && 
		    situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && 
		    situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking) {
			SetNoAiTime();
			isJoystick = false;
			IsSpeedup = false;

            if (crtState != ps)
                AniState(ps);

			if(crtState == PlayerState.Dribble0){
	            if (situation == GameSituation.AttackA)
	                rotateTo(CourtMgr.Get.ShootPoint [0].transform.position.x, CourtMgr.Get.ShootPoint [0].transform.position.z);
	            else if (situation == GameSituation.AttackB)
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
			if(Data.DefPlayer.Index == Index && AutoFollow)
			{
				Result.x = Data.DefPlayer.transform.position.x;
				Result.y = Data.DefPlayer.transform.position.z;
				ResultBool = true;
			}
			else
			{
				Vector3 aP1 = Data.DefPlayer.transform.position;
				Vector3 aP2 = CourtMgr.Get.Hood[Data.DefPlayer.Team.GetHashCode()].transform.position;
				Result = GetStealPostion(aP1, aP2, Data.DefPlayer.Index);
				if(Vector2.Distance(Result, new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)) <= GameConst.StealBallDistance)
				{
					if(Math.Abs(GameController.Get.GetAngle(Data.DefPlayer, this)) >= 30 && 
					   Vector3.Distance(aP2, DefPlayer.transform.position) <= GameConst.TreePointDistance + 3)
					{
						ResultBool = true;
					}
					else
					{
						Result.x = gameObject.transform.position.x;
						Result.y = gameObject.transform.position.z;
					}
				}else
					ResultBool = true;
			}
        } 
		else if (Data.FollowTarget != null)
		{
			Result.x = Data.FollowTarget.position.x;
			Result.y = Data.FollowTarget.position.z;

			if(Vector2.Distance(Result, new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)) <= MoveCheckValue)
			{
				Result.x = gameObject.transform.position.x;
				Result.y = gameObject.transform.position.z;
			}else
				ResultBool = true;
		}
		else
		{
            Result = Data.Target;
			ResultBool = true;
		}

		return ResultBool;
    }
	
    public void MoveTo(TMoveData Data, bool First = false)
    {
		if ((CanMove || (NoAiTime == 0 && HoldBallCanMove)) && WaitMoveTime == 0 && GameStart.Get.TestMode != GameTest.Block)
		{
			bool DoMove = GetMoveTarget(ref Data, ref MoveTarget);
			TacticalName = Data.FileName;
			float temp = Vector2.Distance(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z), MoveTarget);
			SetSpeed(0.3f, 0);

			if(temp <= MoveCheckValue || !DoMove)
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
							else if(!DoMove)
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
					
					AniState(PlayerState.Defence0);                          
				} 
				else
				{
					if (!IsBallOwner)
						AniState(PlayerState.Idle);
					else if (situation == GameSituation.TeeA || situation == GameSituation.TeeB)
						AniState(PlayerState.Dribble0);
					
					if (First || GameStart.Get.TestMode == GameTest.Edit)
						WaitMoveTime = 0;
					else if (situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
					{
                        dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint [Team.GetHashCode()].transform.position);
						if (dis <= 8)
							WaitMoveTime = Time.time + UnityEngine.Random.Range(0, 1);
						else
							WaitMoveTime = Time.time + UnityEngine.Random.Range(0, 3);
					}
					
					if (IsBallOwner)
					{
						if (Team == TeamKind.Self)
                            rotateTo(CourtMgr.Get.ShootPoint [0].transform.position.x, CourtMgr.Get.ShootPoint [0].transform.position.z);
						else
                            rotateTo(CourtMgr.Get.ShootPoint [1].transform.position.x, CourtMgr.Get.ShootPoint [1].transform.position.z);
						
						if (Data.Shooting && NoAiTime == 0)
							GameController.Get.Shoot();
					} 
					else
					{
						if (Data.LookTarget == null)
						{
							if (GameController.Get.BallOwner != null)
							{
								rotateTo(GameController.Get.BallOwner.transform.position.x, GameController.Get.BallOwner.transform.position.z);
							} else
							{
								if (Team == TeamKind.Self)
                                    rotateTo(CourtMgr.Get.ShootPoint [0].transform.position.x, CourtMgr.Get.ShootPoint [0].transform.position.z);
								else
                                    rotateTo(CourtMgr.Get.ShootPoint [1].transform.position.x, CourtMgr.Get.ShootPoint [1].transform.position.z);
							}
						} 
						else
							rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
						
						if (Data.Catcher)
						{
							if ((situation == GameSituation.AttackA || situation == GameSituation.AttackB))
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
			} 
			else if ((IsDefence == false && MoveTurn >= 0 && MoveTurn <= 5) && GameController.Get.BallOwner != null)
			{                                          
				MoveTurn++;
				rotateTo(MoveTarget.x, MoveTarget.y);
				if (MoveTurn == 1)
					MoveStartTime = Time.time + GameConst.DefMoveTime;           
			} 
			else
			{
				if (IsDefence)
				{
					if (Data.DefPlayer != null)
					{
                        dis = Vector3.Distance(transform.position, CourtMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);
						
						if (dis <= GameConst.TreePointDistance + 4)
						{
							rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
						} 
						else
						{
							if (Vector3.Distance(transform.position, Data.LookTarget.position) <= 1.5f)
							{
								rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
							} 
							else
							{
								rotateTo(MoveTarget.x, MoveTarget.y);
							}
						}

						if(Math.Abs(GameController.Get.GetAngle(this, new Vector3(MoveTarget.x, 0, MoveTarget.y))) >= 90)
							AniState(PlayerState.Defence1);
						else
							AniState(PlayerState.RunningDefence);
					} 
					else
					{
						rotateTo(MoveTarget.x, MoveTarget.y);
						AniState(PlayerState.Run0);
					}
					
					isMoving = true;
					if (MovePower > 0 && CanSpeedup && this != GameController.Get.Joysticker && !IsTee)
					{
						SetSpeed(1, 0);
						transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * GameConst.DefSpeedup * Attr.SpeedValue);
						IsSpeedup = true;
					}
					else
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
							AniState(PlayerState.Dribble2);
							IsSpeedup = true;
						}
						else
						{
							transform.Translate(Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedNormal * Attr.SpeedValue);
							AniState(PlayerState.Dribble1);
							IsSpeedup = false;
						}
					} 
					else
					{
						if (Data.Speedup && MovePower > 0)
						{
							SetSpeed(1, 0);
							transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedup * Attr.SpeedValue);
							AniState(PlayerState.Run1);
							IsSpeedup = true;
						}
						else
						{
							transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedNormal * Attr.SpeedValue);
							AniState(PlayerState.Run0);
							IsSpeedup = false;
						}
					}
				}
			} 
        } 
		else
            isMoving = false;
    }

    public void rotateTo(float lookAtX, float lookAtZ)
    {
		if (isBlock)
			return;

		gameObject.transform.LookAt (new Vector3 (lookAtX, gameObject.transform.position.y, lookAtZ));

//		Debug.Log ("Roatte To .GameObject : " + gameObject.name);
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

    private void AddActionFlag(ActionFlag Flag)
    {
        GameFunction.Add_ByteFlag(Flag.GetHashCode(), ref PlayerActionFlag);
        animator.SetBool(Flag.ToString(), true);
    }

    public void DelActionFlag(ActionFlag Flag)
    {
        GameFunction.Del_ByteFlag(Flag.GetHashCode(), ref PlayerActionFlag);
        animator.SetBool(Flag.ToString(), false);
    }

    public bool CheckAnimatorSate(PlayerState state)
    {
        if (crtState == state)
            return true;
        else
            return false;
    }
    
    public void ResetFlag(bool ClearMove = true)
    {
        if (CheckAnimatorSate(PlayerState.Idle) || CheckAnimatorSate(PlayerState.Dribble1) || CheckAnimatorSate(PlayerState.Dribble0))
        {
            NeedResetFlag = false;
            for (int i = 0; i < PlayerActionFlag.Length; i++)
                PlayerActionFlag [i] = 0;
            
            AniState(PlayerState.Idle);
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

    public bool CanUseState(PlayerState state)
    {
//		Debug.Log ("Check ** " +gameObject.name + ".CrtState : " + crtState + "  : state : " + state);
        switch (state)
        {
            case PlayerState.Pass0:
            case PlayerState.Pass2:
            case PlayerState.Pass1:
            case PlayerState.Pass4:
            case PlayerState.Tee:
				if (!IsPass && (crtState == PlayerState.HoldBall || IsDribble)){
                    return true;
				}
                break;

			case PlayerState.Pass5:
				if((crtState == PlayerState.Shoot0 || crtState == PlayerState.Shoot2) && !GameController.Get.Shooter && IsPassAirMoment)
					return true;
				break;
            
            case PlayerState.BlockCatch:
				if (crtState == PlayerState.Block && crtState != PlayerState.BlockCatch) 
                    return true;
                break;

            case PlayerState.FakeShoot:
				if (IsBallOwner && (crtState == PlayerState.Idle || crtState == PlayerState.HoldBall || IsDribble))
					return true;
				break;

            case PlayerState.Shoot0:
            case PlayerState.Shoot1:
            case PlayerState.Shoot2:
            case PlayerState.Shoot3:
            case PlayerState.Shoot6:
            case PlayerState.Layup:
				if (IsBallOwner && !IsShoot && (crtState == PlayerState.Idle || crtState == PlayerState.HoldBall || IsDribble))
                    return true;
                break;

		case PlayerState.Dunk0:
		case PlayerState.Dunk2:
		case PlayerState.Dunk4:
		case PlayerState.Dunk20:
			if (IsBallOwner && !IsShoot && (crtState == PlayerState.Idle || crtState == PlayerState.HoldBall || IsDribble))
				if(Vector3.Distance(CourtMgr.Get.ShootPoint [Team.GetHashCode()].transform.position, gameObject.transform.position) < canDunkDis)
					return true;
			break;

			case PlayerState.Alleyoop:
			if (crtState != PlayerState.Alleyoop && !IsBallOwner && (GameStart.Get.TestMode == GameTest.Alleyoop || situation.GetHashCode() == (Team.GetHashCode()+3)))
					return true;

				break;
            case PlayerState.HoldBall:
                if (IsBallOwner && !IsPass)
                    return true;
                break;

			case PlayerState.Rebound:
				if(CanMove && crtState != PlayerState.Rebound)
					return true;
				break;

			case PlayerState.TipIn:
			if(crtState == PlayerState.Rebound && crtState != PlayerState.TipIn)
				return true;

				break;
           
			case PlayerState.PickBall0:
			case PlayerState.PickBall2:
			if (CanMove && !IsBallOwner && (crtState == PlayerState.Idle || crtState == PlayerState.Run0 || crtState == PlayerState.Run1 || crtState == PlayerState.Defence1 ||
			                                          crtState == PlayerState.Defence0 || crtState == PlayerState.RunningDefence))
					return true;
				break;

            case PlayerState.Push:
            case PlayerState.Steal:
			if (!IsTee && CanMove && !IsBallOwner && (crtState == PlayerState.Idle || crtState == PlayerState.Run0 || crtState == PlayerState.Run1 || crtState == PlayerState.Defence1 ||
                    crtState == PlayerState.Defence0 || crtState == PlayerState.RunningDefence))
                    return true;
                break;

			case PlayerState.Block:
			if (!IsTee && CanMove && !IsBallOwner && (crtState == PlayerState.Idle || crtState == PlayerState.Run0 || crtState == PlayerState.Run1 || crtState == PlayerState.Defence1 ||
			                                crtState == PlayerState.Defence0 || crtState == PlayerState.RunningDefence || IsDunk))
					return true;
				break;

            case PlayerState.Elbow:
				if (!IsTee && IsBallOwner && (crtState == PlayerState.Dribble0 || crtState == PlayerState.Dribble1 || crtState == PlayerState.HoldBall))
                    return true;
                break;

            case PlayerState.Fall0:
            case PlayerState.Fall1:
            case PlayerState.Fall2:
				if (!IsTee && crtState != state && crtState != PlayerState.Elbow && 
			    (crtState == PlayerState.Dribble0 || crtState == PlayerState.Dribble1 || crtState == PlayerState.HoldBall || IsDunk ||
			 	crtState == PlayerState.Idle || crtState == PlayerState.Run0 || crtState == PlayerState.Run1 || crtState == PlayerState.Defence0 || crtState == PlayerState.Defence1 || 
                    crtState == PlayerState.RunningDefence))
                    return true;
                break;

            case PlayerState.GotSteal:
				if (!IsTee && crtState != state && crtState != PlayerState.Elbow && 
                    (crtState == PlayerState.Dribble0 ||
                    crtState == PlayerState.Dribble1 || 
                    crtState == PlayerState.FakeShoot || 
                    crtState == PlayerState.HoldBall || 
                    crtState == PlayerState.Idle || 
                    crtState == PlayerState.Run0 ||
					crtState == PlayerState.Run1 ||
                    crtState == PlayerState.Defence0 || 
                    crtState == PlayerState.Defence1 || 
                    crtState == PlayerState.RunningDefence))
                    return true;
                break;

			case PlayerState.Dribble0:
            case PlayerState.Dribble1:
            case PlayerState.Dribble2:
				if (IsFirstDribble && !CanMove || (CanMove && crtState != state) || (crtState == PlayerState.MoveDodge0 || crtState == PlayerState.MoveDodge1))
				{
                    return true;
				}
                break;
            
            case PlayerState.Run0:   
            case PlayerState.Run1:   
            case PlayerState.RunningDefence:
            case PlayerState.Defence0:
            case PlayerState.Defence1:
			case PlayerState.MoveDodge0:
			case PlayerState.MoveDodge1:
				if(crtState != state)
				return true;
				break;
            case PlayerState.CatchFlat:
            case PlayerState.CatchFloor:
            case PlayerState.CatchParabola:
			case PlayerState.Intercept0:
			case PlayerState.Intercept1:
				if (CanMove)
                    return true;
                break;

            case PlayerState.Idle:
				return true;
        }

        return false;
    }

	public bool IsTee	
	{ 
		get
		{
			return (situation == GameSituation.TeeA || situation == GameSituation.TeeAPicking || situation == GameSituation.TeeB || situation == GameSituation.TeeBPicking);
		}
	}

    public bool AniState(PlayerState state, Vector3 v)
    {
        if (!CanUseState(state))
            return false;

        rotateTo(v.x, v.z);
        return AniState(state);
    }

    public bool AniState(PlayerState state)
    {
        if (!CanUseState(state))
            return false;

        bool Result = false;
        int stateNo = 0;
        string curveName;
		PlayerRigidbody.mass = 5;

//	Debug.Log("Do ** " + gameObject.name + ".AniState : " + state);
        
        switch (state)
        {
            case PlayerState.Block:
				SetShooterLayer();
                playerBlockCurve = null;
                for (int i = 0; i < aniCurve.Block.Length; i++)
                    if (aniCurve.Block [i].Name == "Block")
                        playerBlockCurve = aniCurve.Block [i];

                ClearAnimatorFlag();
                animator.SetTrigger("BlockTrigger");
                isCanCatchBall = false;
                blockCurveTime = 0;
                isBlock = true;
                Result = true;
                break;

            case PlayerState.BlockCatch:
                ClearAnimatorFlag();
                animator.SetTrigger("BlockCatchTrigger");
				IsPerfectBlockCatch = false;
                isCanCatchBall = false;
                Result = true;
                break;

            case PlayerState.CatchFlat:
				animator.SetInteger("StateNo", 0);
				SetSpeed(0, -1);
				ClearAnimatorFlag();
                animator.SetTrigger("CatchTrigger");
                Result = true;
                break;

			case PlayerState.CatchFloor:
				animator.SetInteger("StateNo", 2);
				SetSpeed(0, -1);
            	ClearAnimatorFlag();
                animator.SetTrigger("CatchTrigger");
                Result = true;
                break;
                
			case PlayerState.CatchParabola:
				animator.SetInteger("StateNo", 1);
				SetSpeed(0, -1);
                ClearAnimatorFlag();
                animator.SetTrigger("CatchTrigger");
                Result = true;
                break;

            case PlayerState.Defence0:
				PlayerRigidbody.mass = 50;
                ClearAnimatorFlag();
                SetSpeed(0, -1);
                AddActionFlag(ActionFlag.IsDefence);
                Result = true;
                break;
			case PlayerState.Alleyoop:
					PlayerRigidbody.useGravity = false;
					ClearAnimatorFlag();
					animator.SetTrigger("DunkTrigger");
					//isCanCatchBall = false;
					SetShooterLayer();
					DunkTo();
					Result = true;

				break;

            case PlayerState.Dunk0:
            case PlayerState.Dunk2:
            case PlayerState.Dunk4:
            case PlayerState.Dunk20:
					switch (state)
					{
						case PlayerState.Dunk0:
							stateNo = 0;
							break;
						case PlayerState.Dunk2:
							stateNo = 2;
							break;
						case PlayerState.Dunk4:
							stateNo = 4;
							break;
						case PlayerState.Dunk20:
							stateNo = 20;
							break;
					}
                    PlayerRigidbody.useGravity = false;
                    ClearAnimatorFlag();
					animator.SetInteger("StateNo", stateNo);
                    animator.SetTrigger("DunkTrigger");
                    isCanCatchBall = false;

					playerDunkCurve = null;
					for (int i = 0; i < aniCurve.Dunk.Length; i++)
						if (aniCurve.Dunk [i].Name == string.Format("Dunk{0}", stateNo)){
							playerDunkCurve = aniCurve.Dunk [i];
							isDunk = true;
							isDunkZmove = false;
							dunkCurveTime = 0;
						}
					SetShooterLayer();
                    Result = true;
                break;

            case PlayerState.Dribble0:
			case PlayerState.Dribble1:
			case PlayerState.Dribble2:
                if (GameController.Get.BallOwner == this)
                {
					switch (state)
					{
						case PlayerState.Dribble0:
								PlayerRigidbody.mass = 50;
								stateNo = 0;
								break;
						case PlayerState.Dribble1:
								stateNo = 1;
								break;
						case PlayerState.Dribble2:
								stateNo = 2;
								break;
					}
					PlayerRigidbody.mass = 3;
//                    if (!isJoystick)
//                        SetSpeed(0, -1);
                    ClearAnimatorFlag();
					animator.SetInteger("StateNo", stateNo);
					animator.SetTrigger("DribbleTrigger");
                    AddActionFlag(ActionFlag.IsDribble);
                    CourtMgr.Get.SetBallState(PlayerState.Dribble0, this);
                    isCanCatchBall = false;
                    IsFirstDribble = false;
                    Result = true;
                }
                break;

			case PlayerState.Elbow:
				PlayerRigidbody.mass = 50;
				ClearAnimatorFlag();
				animator.SetTrigger("ElbowTrigger");
				isCanCatchBall = false;
				Result = true;
				break;

            case PlayerState.FakeShoot:
                if (IsBallOwner)
                {
					PlayerRigidbody.mass = 50;
					ClearAnimatorFlag();
					animator.SetTrigger("FakeShootTrigger");
					isCanCatchBall = false;
					isFakeShoot = true;
                    Result = true;
                }
                break;

            case PlayerState.Fall0:
            case PlayerState.Fall1:
            case PlayerState.Fall2:
				switch (state)
				{
					case PlayerState.Fall0:
						stateNo = 0;
						break;
					case PlayerState.Fall1:
						stateNo = 1;
						break;
					case PlayerState.Fall2:
						stateNo = 2;
						break;
				}
				curveName = string.Format("Fall{0}", stateNo);
				playerFallCurve = null;

				for(int i = 0; i < aniCurve.Fall.Length; i++)
					if(curveName == aniCurve.Fall[i].Name){
						playerFallCurve = aniCurve.Fall[i];
						fallCurveTime = 0;
						isFall = true;
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

            case PlayerState.HoldBall:
				PlayerRigidbody.mass = 50;
                ClearAnimatorFlag();
                AddActionFlag(ActionFlag.IsHoldBall);
                isCanCatchBall = false;
                Result = true;
                break;
            
            case PlayerState.Idle:
				PlayerRigidbody.mass = 50;
                SetSpeed(0, -1);
                ClearAnimatorFlag();
                isCanCatchBall = true;
				isMoving = false;
                Result = true;
                break;

			case PlayerState.Intercept0:
				animator.SetInteger("StateNo", 0);
				animator.SetTrigger("InterceptTrigger");
				ClearAnimatorFlag();
				Result = true;
				break;

			case PlayerState.Intercept1:
				animator.SetInteger("StateNo", 1);
				animator.SetTrigger("InterceptTrigger");
				ClearAnimatorFlag();
				Result = true;
				break;
			
			case PlayerState.Defence1:
//				Rigi.mass = 500;
                isCanCatchBall = true;
                SetSpeed(1, 1);
				ClearAnimatorFlag(ActionFlag.IsDefence);
//				AddActionFlag(ActionFlag.IsDefence);
                Result = true;
                break;

			case PlayerState.MoveDodge0:
				animator.SetInteger("StateNo", 0);
				animator.SetTrigger("MoveDodge");
				OnUIMoveDodge(this);
				Result = true;
				break;

			case PlayerState.MoveDodge1:
				animator.SetInteger("StateNo", 1);
				animator.SetTrigger("MoveDodge");
				OnUIMoveDodge(this);
				Result = true;
				break;

            case PlayerState.Pass0:
			case PlayerState.Pass1:
			case PlayerState.Pass2:
			case PlayerState.Pass4:
			case PlayerState.Pass5:
				switch (state)
				{
					case PlayerState.Pass0:
						stateNo = 0;
						break;
					case PlayerState.Pass1:
						stateNo = 1;
						break;
					case PlayerState.Pass2:
						stateNo = 2;
						break;
					case PlayerState.Pass4:
						stateNo = 4;
						break;
					case PlayerState.Pass5:
						stateNo = 5;
						break;
				}
				ClearAnimatorFlag();
				PlayerRigidbody.mass = 50;
				animator.SetInteger("StateNo", stateNo);
                animator.SetTrigger("PassTrigger");
                Result = true;
                break;

            case PlayerState.Push:
                ClearAnimatorFlag();
				playerPushCurve = null;
				for(int i = 0; i < aniCurve.Push.Length; i++)
					if (aniCurve.Push [i].Name == "Push0"){
						playerPushCurve = aniCurve.Push [i];
						pushCurveTime = 0;
						isPush = true;
					}
                animator.SetTrigger("PushTrigger");
                Result = true;
                break;

            case PlayerState.PickBall0:
                isCanCatchBall = true;
                ClearAnimatorFlag();
				animator.SetInteger("StateNo", 0);
                animator.SetTrigger("PickTrigger");
                Result = true;
                break;

			case PlayerState.PickBall2:
				isCanCatchBall = true;
				ClearAnimatorFlag();
				for(int i = 0; i < aniCurve.Push.Length; i++)
				if (aniCurve.PickBall [i].Name == "PickBall2"){
					playerPickCurve = aniCurve.PickBall [i];
					pickCurveTime = 0;
					isPick = true;
				}
				animator.SetInteger("StateNo", 2);
				animator.SetTrigger("PickTrigger");
				Result = true;
				break;
            
            case PlayerState.Tee:
                isCanCatchBall = true;
                ClearAnimatorFlag();
                animator.SetInteger("StateNo", 1);
                animator.SetTrigger("PassTrigger");
                Result = true;
                break;
            
            case PlayerState.Run0:
            case PlayerState.Run1:
                if (!isJoystick)
                    SetSpeed(1, 1); 

				switch (state)
				{
					case PlayerState.Run0:
						stateNo = 0;
						break;
					case PlayerState.Run1:
						stateNo = 1;
						break;
				}
				animator.SetInteger("StateNo", stateNo);
				ClearAnimatorFlag(ActionFlag.IsRun);
				PlayerRigidbody.mass = 3;
				Result = true;
                break;
    
            case PlayerState.RunningDefence:
                SetSpeed(1, 1);
				ClearAnimatorFlag(ActionFlag.IsRun);
//				AddActionFlag(ActionFlag.IsRun);
				PlayerRigidbody.mass = 3;
                Result = true;
                break;

            case PlayerState.Steal:
				PlayerRigidbody.mass = 50;
                ClearAnimatorFlag();
                animator.SetTrigger("StealTrigger");
                isCanCatchBall = false;
                Result = true;
                break;

            case PlayerState.GotSteal:
//                  AniWaitTime = Time.time + 2.9f;
                ClearAnimatorFlag();
                animator.SetTrigger("GotStealTrigger");
                isCanCatchBall = false;
                Result = true;
                break;

            case PlayerState.Shoot0:
            case PlayerState.Shoot1:
            case PlayerState.Shoot2:
            case PlayerState.Shoot3:
			case PlayerState.Shoot6:
                if (IsBallOwner)
                {                   
                    playerShootCurve = null;
                    
                    switch (state)
                    {
                        case PlayerState.Shoot0:
							IsPassAirMoment = true;
                            stateNo = 0;
                            break;
                        case PlayerState.Shoot1:
                            stateNo = 1;
						break;
                        case PlayerState.Shoot2:
							IsPassAirMoment = true;
                            stateNo = 2;
                            break;
                        case PlayerState.Shoot3:
                            stateNo = 3;
							break;
						case PlayerState.Shoot6:
							stateNo = 6;
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
							continue;
                        }

					SetShooterLayer();
                    ClearAnimatorFlag();
                    animator.SetTrigger("ShootTrigger");
                    isCanCatchBall = false;
                    Result = true;
                }
                break;

		case PlayerState.Layup:
			if (IsBallOwner){
				playerLayupCurve = null;
				stateNo = 0;
				curveName = string.Format("Layup{0}", stateNo);

				for (int i = 0; i < aniCurve.Layup.Length; i++)
					if (aniCurve.Layup [i].Name == curveName)
					{
						playerLayupCurve = aniCurve.Layup [i];
						layupCurveTime = 0;
						isLayup = true;
						isLayupZmove = false;
					}
				SetShooterLayer();
				ClearAnimatorFlag();
				animator.SetTrigger("LayupTrigger");
				isCanCatchBall = false;
				Result = true;
			}
			break;

            case PlayerState.Rebound:
				playerReboundCurve = null;

				if (inReboundDistance()) 
					reboundMove = CourtMgr.Get.RealBall.transform.position - transform.position;
				else
					reboundMove = Vector3.zero;

				for (int i = 0; i < aniCurve.Rebound.Length; i++)
					if (aniCurve.Rebound [i].Name == "Rebound")
					{
						playerReboundCurve = aniCurve.Rebound [i];
						reboundCurveTime = 0;
						isRebound = true;
					}

				ClearAnimatorFlag();
				SetShooterLayer();
                animator.SetTrigger("ReboundTrigger");
                Result = true;
                break;

			case PlayerState.TipIn:
				ClearAnimatorFlag();
				SetShooterLayer();
				animator.SetTrigger("TipInTrigger");
				Result = true;
				break;

			case PlayerState.ReboundCatch:
				animator.SetTrigger("ReboundCatchTrigger");
				break;
		}
        
        if (Result)
        {
            crtState = state;
                        
            if (crtState == PlayerState.Idle && NeedResetFlag)
                ResetFlag();
        }

        return Result;
    }

	public void SetShooterLayer()
	{
		isStartCheckLayer = false;
		gameObject.layer = LayerMask.NameToLayer("Shooter");
		isCheckLayerToReset = true;
	}

	public void ClearAnimatorFlag(ActionFlag addFlag = ActionFlag.None)
	{
		if (addFlag == ActionFlag.None) {
			DelActionFlag (ActionFlag.IsDefence);
			DelActionFlag (ActionFlag.IsRun);
			DelActionFlag (ActionFlag.IsDribble);
			DelActionFlag (ActionFlag.IsHoldBall);
		}
		else{
			for(int i = 1; i < System.Enum.GetValues(typeof(ActionFlag)).Length; i++)
				if(i != (int)addFlag)
					DelActionFlag ((ActionFlag)i);
					
			AddActionFlag(addFlag);
		}
    }
    
    public void AnimationEvent(string animationName)
    {
        switch (animationName)
        {
            case "Stealing":
                if (OnStealMoment != null)
                    OnStealMoment(this);
                break;

            case "GotStealing":
                if (OnGotSteal != null)
                    OnGotSteal(this);
                break;  
                    
            case "FakeShootBlockMoment":
                if (!IsShoot && OnFakeShootBlockMoment != null)
                    OnFakeShootBlockMoment(this);
                break;

            case "BlockMoment":
                if (OnBlockMoment != null)
                    OnBlockMoment(this);
				break;

			case "DoubleClickMoment":
				if(OnDoubleClickMoment != null)
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
//				if(OnBlockCatching != null)
//					OnBlockCatching(this);
				break;

			case "BlockCatchingEnd":
				if(IsBallOwner){
					IsFirstDribble = true;
					AniState(PlayerState.HoldBall);
				}
				else
					AniState(PlayerState.Idle);

				IsPerfectBlockCatch = false;
				break;

            case "Blocking":
//                if (OnBlocking != null)
//                    OnBlocking(this);

                break;
            case "Shooting":
				IsPassAirMoment = false;
                if (OnShooting != null && crtState != PlayerState.Pass5)
                    OnShooting(this);
                break;

			case "MoveDodgeEnd": 
				OnUI(this);
				if(IsBallOwner)
					AniState(PlayerState.Dribble0);
				else
					AniState(PlayerState.Idle);
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
				
				if(!IsBallOwner)
					AniState(PlayerState.Idle);
				break;

            case "PickUp": 
                if (OnPickUpBall != null)
                    OnPickUpBall(this);
                break;
            case "PickEnd":
                AniState(PlayerState.Dribble0);
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
            
                CourtMgr.Get.SetBallState(PlayerState.Dunk0);
                if (OnDunkJump != null)
                    OnDunkJump(this);

                break;

            case "DunkBasket":
//                DelActionFlag(ActionFlag.IsDribble);
//                DelActionFlag(ActionFlag.IsRun);
                CourtMgr.Get.PlayDunk(Team.GetHashCode());

                break;
            case "DunkFallBall":
				OnUI(this);
                if (OnDunkBasket != null)
                    OnDunkBasket(this);

                break;

            case "ElbowEnd":
				OnUI(this);
				AniState(PlayerState.HoldBall);
				GameController.Get.RealBallFxTime = 1f;
				CourtMgr.Get.RealBallFX.SetActive(true);
                break;

            case "CatchEnd":
				if(situation == GameSituation.TeeA || situation == GameSituation.TeeB){
					if(IsBallOwner)
						AniState(PlayerState.Dribble0);
					else
						AniState(PlayerState.Idle);
				} else {
					OnUI(this);
					IsFirstDribble = true;
					if(!IsBallOwner)
					{					
						AniState(PlayerState.Idle);
					}else
					{
						if (NoAiTime == 0)
							AniState(PlayerState.Dribble0);
						else 
							AniState(PlayerState.HoldBall);
					}
				}
                break;

			case "FakeShootEnd":
				isFakeShoot = false;
				if(IsBallOwner)
                	AniState(PlayerState.HoldBall);
				else
					AniState(PlayerState.Idle);

				OnUI(this);
				GameController.Get.RealBallFxTime = 1f;
				CourtMgr.Get.RealBallFX.SetActive(true);
				break;

            case "AnimationEnd":
				OnUI(this);
                AniState(PlayerState.Idle);
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
//        DelActionFlag(ActionFlag.IsRun);
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

	public PlayerState PassiveSkill(TSkillSituation situation, TSkillKind kind, Vector3 v = default(Vector3)) {
		PlayerState playerState = PlayerState.Idle;
		playerState = (PlayerState)((int) situation);
		List<PassiveSkill> ps = new List<PassiveSkill>();
		if(passiveSkill.ContainsKey((int) kind)){
			ps = passiveSkill[(int)kind];
		}
		bool isPerformPassive = false;
		if(ps.Count > 0) {
			int passiveRate = 0;
			if(kind == TSkillKind.Pass) {
				float angle = GameFunction.GetPlayerToObjectAngleByVector(this.transform, v);
				if(angle < 45f && angle > -45f){
					passDirect = PassDirectState.Forward; 
				} else if(angle <= -45f && angle > -135f) {
					passDirect = PassDirectState.Left;
				} else if(angle < 135f && angle >= 45f) {
					passDirect = PassDirectState.Right;
				} else if(angle >= 135f && angle >= -135f) {
					passDirect = PassDirectState.Back;
				}
				for (int i=0; i<ps.Count; i++) {
					if((ps[i].Kind % 10) == (int)passDirect)
						passiveRate += ps[i].Rate;
				}
			} else {
				for (int i=0; i<ps.Count; i++)
					passiveRate += ps[i].Rate;
			}
			isPerformPassive = (UnityEngine.Random.Range(0,100) <= passiveRate)?true:false;
		}

		if(isPerformPassive) {
			if(ps.Count > 0) {
				string animationName = string.Empty;
				for(int i=0; i<ps.Count; i++) {
					if(UnityEngine.Random.Range(0,100) <= ps[i].Rate) {
						string[] enumName =  Enum.GetNames(typeof(PlayerState));
						bool isHave = false;
//						if(Enum.IsDefined (typeof(PlayerState), ps[i].Name) != null){
						for(int j=0; j<enumName.Length; j++) {
							if(enumName[i].Equals(ps[i].Name))
								isHave = true;
						}
						if(isHave){
							animationName = ps[i].Name;
							break;
						}
					}
				}
				if(animationName != string.Empty)
					return (PlayerState) System.Enum.Parse (typeof(PlayerState), animationName);
				else 
					return playerState;
			}
		} else
			return playerState;

		return playerState;
	}

	public bool IsHaveMoveDodge{
		get{
			return isHaveMoveDodge;
		}
	}

    public bool CanMove
    {
        get
        {
            PlayerState[] CheckAy = {
                PlayerState.Block,
                PlayerState.BlockCatch,
                PlayerState.CatchFlat,
                PlayerState.CatchFloor,
                PlayerState.CatchParabola,
                PlayerState.Dunk0,
				PlayerState.Alleyoop,
                PlayerState.Elbow,
                PlayerState.FakeShoot,
                PlayerState.Fall0,
                PlayerState.Fall1,
                PlayerState.Fall2,
                PlayerState.HoldBall,
                PlayerState.GotSteal,
                PlayerState.Pass0,
                PlayerState.Pass2,
                PlayerState.Pass1,
                PlayerState.Pass4,
                PlayerState.Pass5,
                PlayerState.Push,
                PlayerState.PickBall0,
				PlayerState.PickBall2,
                PlayerState.Shoot0,
                PlayerState.Shoot1,
                PlayerState.Shoot2,
                PlayerState.Shoot3,
                PlayerState.Shoot6,
				PlayerState.Steal,
                PlayerState.Layup,
                PlayerState.Tee,
                PlayerState.Rebound,
                PlayerState.ReboundCatch,
				PlayerState.TipIn,
				PlayerState.Intercept0,
				PlayerState.Intercept1,
				PlayerState.MoveDodge0,
				PlayerState.MoveDodge1
            };

            for (int i = 0; i < CheckAy.Length; i++)
                if (CheckAnimatorSate(CheckAy [i]))
                    return false;

            return true;
        }
    }

	public bool CanMoveFirstDribble{
		get{
			if(CheckAnimatorSate(PlayerState.HoldBall) && IsFirstDribble)
				return true;
			else
				return false;
		}
	}
	
	public bool HoldBallCanMove
	{
		get
		{
			if (CheckAnimatorSate(PlayerState.HoldBall) && IsFirstDribble)
				return true;
			else
				return false;
		}
	}
	
	public void ClearIsCatcher()
	{
		//        DelActionFlag(ActionFlag.IsCatcher);
	}
	
	public bool IsCatcher
	{
		get{ return CheckAnimatorSate(PlayerState.CatchFlat);}
    }

    public bool IsCanCatchBall
    {
        get{ return isCanCatchBall;}
    }

	public bool IsCanBlock
	{
		get{return isCanBlock;}
		set{
			isCanBlock = value;
			if(CourtMgr.Get.RealBallFX.activeSelf != value)
				CourtMgr.Get.RealBallFX.SetActive(value);
		}
	}

    public bool IsDefence
    {
        get
        {
            if ((situation == GameSituation.AttackA || situation == GameSituation.TeeA || situation == GameSituation.TeeAPicking) && Team == TeamKind.Npc)
                return true;
			else if ((situation == GameSituation.AttackB || situation == GameSituation.TeeB || situation == GameSituation.TeeBPicking) && Team == TeamKind.Self)
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

	public bool IsShoot
	{
		get{ return crtState == PlayerState.Shoot0 || crtState == PlayerState.Shoot1 || crtState == PlayerState.Shoot2 ||crtState == PlayerState.Shoot3 || 
			crtState == PlayerState.Shoot6 || crtState == PlayerState.Layup || IsDunk || crtState == PlayerState.DunkBasket ||  crtState == PlayerState.TipIn;}
	}

    public bool IsPass
    {
		get{ return crtState == PlayerState.Pass0 || crtState == PlayerState.Pass2 || crtState == PlayerState.Pass1 || crtState == PlayerState.Tee || crtState == PlayerState.Pass4;}
    }

	public bool IsDribble
	{
		get{ return crtState == PlayerState.Dribble0 || crtState == PlayerState.Dribble1 || crtState == PlayerState.Dribble2;}
	}

	public bool IsDunk
	{
		get{ return crtState == PlayerState.Dunk0 || crtState == PlayerState.Dunk2 || crtState == PlayerState.Dunk4 || crtState == PlayerState.Dunk20;}
	}

    private bool isMoving = false;

    public bool IsMoving
    {
        get{ return isMoving;}
    }

	public bool IsRebound 
	{
		get{return crtState == PlayerState.Rebound || crtState == PlayerState.ReboundCatch;}
	}

    public bool IsFall
    {
        get{ return crtState == PlayerState.Fall0 || crtState == PlayerState.Fall1;}
        
    }

    public bool IsCatch
    {
        get{ return crtState == PlayerState.CatchFlat || crtState == PlayerState.CatchFloor || crtState == PlayerState.CatchParabola;}
    }

	public bool IsFakeShoot
	{
		get{ return isFakeShoot;}
	}

	private bool isPerfectBlockCatch = false;

	public bool IsPerfectBlockCatch
	{
		get{return isPerfectBlockCatch;}
		set{
				isPerfectBlockCatch = value;

				if(!isPerfectBlockCatch){
					blockCatchTrigger.SetEnable(false);
				}
				else
				{
					if(OnDoubleClickMoment != null)
						OnDoubleClickMoment(this, crtState);
				}
//					EffectManager.Get.PlayEffect("DoubleClick01", Vector3.zero, null, gameObject, 1f);
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
			if(MoveQueue.Count == 0)
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
            if (FirstMoveQueue.Count < 2)
                FirstMoveQueue.Enqueue(value);
        }
    }

	public void SetMovePower(float Value)
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
		if(P1.x > 0)
			Result.x = P1.x - (Math.Abs(P1.x - P2.x) / 3);
		else
			Result.x = P1.x + (Math.Abs(P1.x - P2.x) / 3);

		if (GameController.Get.BallOwner && GameController.Get.BallOwner.DefPlayer && GameController.Get.BallOwner.Index != Index) 
		{
			float angle = Math.Abs(GameController.Get.GetAngle(GameController.Get.BallOwner, GameController.Get.BallOwner.DefPlayer));

			if(angle > 90)
			{
				P1 = GameController.Get.BallOwner.transform.position;
				if(P1.x > 0)
					Result.x = P1.x - (Math.Abs(P1.x - P2.x) / 3);
				else
					Result.x = P1.x + (Math.Abs(P1.x - P2.x) / 3);

				cover = true;
			}
		}

		if (mIndex != Index && !cover) 
		{
			switch(mIndex)
			{
			case 0:
				if(Index == 1)
					Result.x += 1.5f;
				else
					Result.x -= 1.5f;
				break;
			case 1:
				if(Index == 0)
					Result.x += 1.5f;
				else
					Result.x -= 1.5f;
				break;
			case 2:
				if(Index == 0)
					Result.x += 1.5f;
				else
					Result.x -= 1.5f;
				break;
			}
		}

		if(P2.z > 0)
			Result.y = P1.z + (Math.Abs(P1.z - P2.z) / 3);
		else
			Result.y = P1.z - (Math.Abs(P1.z - P2.z) / 3);
		return Result;
	}
}