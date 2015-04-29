using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RootMotion.FinalIK;
using DG.Tweening;

public delegate bool OnPlayerAction(PlayerBehaviour player);

public delegate bool OnPlayerAction2(PlayerBehaviour player,bool speedup);

public enum PlayerState
{
    Idle = 0,
    Run = 2,            
    Block = 3,  
    Board = 4,  
    Defence = 6,    
    Dribble = 7,    
    Dunk = 8,
    Fall0 = 9,
    Fall1 = 10,
    BlockCatch = 11,
    Layup = 12, 
    Steal = 14,
    GotSteal = 15,
    PassFlat = 16,
    PassFloor = 17,
    PassParabola = 18,
    Push = 20,
    MovingDefence = 23,
    RunAndDribble = 24,
	Rebound = 25,
	ReboundCatch = 26,
	DunkBasket = 27,
    RunningDefence = 28,
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
    PickBall = 42,
    Shoot0 = 43,
    Shoot1 = 44,
    Shoot2 = 45,
    Shoot3 = 46,
    Shoot6 = 49,
	BasketActionNoScoreEnd = 50
}

public enum TeamKind
{
    Self = 0,
    Npc = 1
}

public enum DefPoint
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
        DownHandAirBallRate = 60;
        UpHandScoreRate = 20;
        UpHandSwishRate = 30;
        UpHandAirBallRate = 40;
        NormalScoreRate = 0;
        NormalSwishRate = 0;
        NormalAirBallRate = 20;
        NearShotScoreRate = 0;
        NearShotSwishRate = 10;
        NearShotAirBallRate = 10;
        LayUpScoreRate = 0;
        LayUpSwishRate = 20;
        LayUpAirBallRate = 5;

    }
}

public class PlayerBehaviour : MonoBehaviour
{
    public OnPlayerAction OnShooting = null;
    public OnPlayerAction OnPass = null;
    public OnPlayerAction OnStealMoment = null;
    public OnPlayerAction OnBlockJump = null;
    public OnPlayerAction OnBlocking = null;
    public OnPlayerAction OnDunkBasket = null;
    public OnPlayerAction OnDunkJump = null;
    public OnPlayerAction OnBlockMoment = null;
    public OnPlayerAction OnFakeShootBlockMoment = null;
    public OnPlayerAction OnFall = null;
    public OnPlayerAction OnPickUpBall = null;
    public OnPlayerAction OnGotSteal = null;
    public Vector3 Translate;
    public float[] DunkHight = new float[2]{3, 5};
    private const float MoveCheckValue = 1;
    private const int ChangeToAI = 1;
    public static string[] AnimatorStates = new string[] {
                "",
                "IsRun",
                "IsDefence",
                "IsDribble",
                "IsHoldBall"
        };
    private Queue<TMoveData> MoveQueue = new Queue<TMoveData>();
    private Queue<TMoveData> FirstMoveQueue = new Queue<TMoveData>();
    private float canDunkDis = 30f;
    private byte[] PlayerActionFlag = {0, 0, 0, 0, 0, 0, 0};
    private float MoveMinSpeed = 0.5f;
    private Vector2 drag = Vector2.zero;
    private bool stop = false;
    private bool NeedResetFlag = false;
    private int MoveTurn = 0;
    private float MoveStartTime = 0;
    private float ProactiveRate = 0;
    private float ProactiveTime = 0;
    private int smoothDirection = 0;
    private float animationSpeed = 0;
    public Rigidbody PlayerRigidbody;
    private Animator animator;
    private GameObject selectTexture;
    private GameObject DefPoint;
    private GameObject pushTrigger;
    private GameObject elbowTrigger;
    private GameObject blockTrigger;
    public GameObject AIActiveHint = null;
    public GameObject DummyBall;
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
    public float CloseDef = 0;
    public PlayerBehaviour DefPlayer = null;
    public bool AutoFollow = false;
    public bool NeedShooting = false;
    public GameStruct.TPlayer Attr;

    //Dunk
    private bool isDunk = false;
    private bool isDunkZmove = false;
    private float dunkCurveTime = 0;
    private GameObject dkPathGroup;
//    private Vector3[] dunkPath = new Vector3[5];
    public AniCurve aniCurve;
    private TDunkCurve playerDunkCurve;

	//Layup
	private bool isLayup = false;
	private bool isLayupZmove = false;
	private float layupCurveTime = 0;
//	private GameObject dkPathGroup;
//	private Vector3[] dunkPath = new Vector3[5];
	private TLayupCurve playerLayupCurve;
		
		//Block
    private bool isBlock = false;
    private float blockCurveTime = 0;
    private TBlockCurve playerBlockCurve;

	//Rebound
    private bool isRebound = false;
    private float reboundCurveTime = 0;
    private TReboundCurve playerReboundCurve;

    //Shooting
    private float shootJumpCurveTime = 0;
    private TShootCurve playerShootCurve;
    private bool isShootJump = false;

    //IK
    private AimIK aimIK;
    private FullBodyBipedIK fullBodyBipedIK;
    private Transform pinIKTransform;
    public Transform IKTarget;
    public bool isIKOpen = false;
    public bool isIKLook = false;
    public bool isIKCatchBall = false;
    private bool firstDribble = true;
    private RotationLimitAngle[] ikRotationLimits;
    public TScoreRate ScoreRate;
    private bool isCanCatchBall = true;

    void initTrigger()
    {
        GameObject obj = Resources.Load("Prefab/Player/BodyTrigger") as GameObject;
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

            DefPoint = obj.transform.FindChild("DefRange").gameObject;            
            obj2.transform.parent = transform;
            obj2.transform.transform.localPosition = Vector3.zero;
            obj2.transform.transform.localScale = Vector3.one;
        }
    }

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        gameObject.tag = "Player";

        //IK
        aimIK = gameObject.GetComponent<AimIK>();
        fullBodyBipedIK = gameObject.GetComponent<FullBodyBipedIK>();
        aimIK.enabled = GameStart.Get.IsOpenIKSystem;
        fullBodyBipedIK.enabled = GameStart.Get.IsOpenIKSystem;
        pinIKTransform = transform.FindChild("Pin");
        IKTarget = SceneMgr.Get.RealBall.transform;

        ikRotationLimits = gameObject.GetComponentsInChildren<RotationLimitAngle>();
        fullBodyBipedIK.enabled = GameStart.Get.IsOpenIKSystem;
        for (int i = 0; i < ikRotationLimits.Length; i++)
            ikRotationLimits [i].enabled = GameStart.Get.IsOpenIKSystem;

        animator = gameObject.GetComponent<Animator>();
        PlayerRigidbody = gameObject.GetComponent<Rigidbody>();
        DummyBall = gameObject.transform.FindChild("DummyBall").gameObject;
        aniCurve = gameObject.transform.FindChild("AniCurve").gameObject.GetComponent<AniCurve>();
        initTrigger();

        ScoreRate = GameStart.Get.ScoreRate;
    }

    public void Init()
    {
        if (DefPoint != null)
            DefPoint.transform.localScale = new Vector3(GameData.AIlevelAy [Attr.AILevel].DefDistance, GameData.AIlevelAy [Attr.AILevel].DefDistance, GameData.AIlevelAy [Attr.AILevel].DefDistance);
    }

    void LateUpdate()
    {
        if (aimIK != null)
        {
            if (pinIKTransform != null)
            {
                if (IKTarget != null)
                {
                    if (GameStart.Get.IsOpenIKSystem)
                    {
                        Vector3 t_self = new Vector3(IKTarget.position.x, IKTarget.position.y, IKTarget.position.z);
                        aimIK.solver.transform.LookAt(pinIKTransform.position);
                        if (isIKOpen)
                        {
                            if (isIKLook)
                            {
                                aimIK.enabled = true;
                                for (int i = 0; i < ikRotationLimits.Length; i++)
                                    ikRotationLimits [i].enabled = true;
                                aimIK.solver.IKPosition = IKTarget.position;
                            } else
                            {
                                aimIK.enabled = false;
                                for (int i = 0; i < ikRotationLimits.Length; i++)
                                    ikRotationLimits [i].enabled = false;
                            }

                            if (isIKCatchBall)
                            {
                                if (Mathf.Abs(GameFunction.GetPlayerToObjectAngle(this.gameObject.transform, SceneMgr.Get.RealBall.transform)) < 100)
                                {
                                    if (Vector3.Distance(this.gameObject.transform.position, SceneMgr.Get.RealBall.transform.position) < 3)
                                    {
                                        fullBodyBipedIK.enabled = true;
                                        fullBodyBipedIK.solver.leftHandEffector.position = SceneMgr.Get.RealBall.transform.position;
                                        fullBodyBipedIK.solver.rightHandEffector.position = SceneMgr.Get.RealBall.transform.position;
                                    } else
                                    {
                                        fullBodyBipedIK.enabled = false;
                                    }
                                } else
                                {
                                    fullBodyBipedIK.enabled = false;
                                }
                            } else
                            {
                                fullBodyBipedIK.enabled = false;
                            }
                        } else
                        {
                            aimIK.enabled = false;
                            for (int i = 0; i < ikRotationLimits.Length; i++)
                                ikRotationLimits [i].enabled = false;
                            fullBodyBipedIK.enabled = false;
                        }
                    } else
                    {
                        aimIK.enabled = false;
                        for (int i = 0; i < ikRotationLimits.Length; i++)
                            ikRotationLimits [i].enabled = false;
                        fullBodyBipedIK.enabled = false;
                    }
                    for (int i = 0; i < aimIK.solver.bones.Length; i++)
                    {
                        if (aimIK.solver.bones [i].rotationLimit != null)
                        {
                            aimIK.solver.bones [i].rotationLimit.SetDefaultLocalRotation();
                        }
                    }
                }
            }
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

        if (NoAiTime == 0)
        {
            if (FirstMoveQueue.Count > 0)
                MoveTo(FirstMoveQueue.Peek(), true);
            else 
            if (MoveQueue.Count > 0)
                MoveTo(MoveQueue.Peek());
        } 
            
        if (NoAiTime > 0 && Time.time >= NoAiTime)
        {
            MoveQueue.Clear();
            NoAiTime = 0;

            if (AIActiveHint)
                AIActiveHint.SetActive(true);
        }

        if ((IsMoving || NoAiTime > 0) && !IsDefence && 
            situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && 
            situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
        {
            if (Time.time >= MoveStartTime)
            {
				MoveStartTime = Time.time + GameConst.DefMoveTime;
				GameController.Get.DefMove(this);
            }       
        }

        if (IsDefence)
        {
            if (Time.time >= ProactiveTime)
            {
                ProactiveTime = Time.time + 4;
                ProactiveRate = UnityEngine.Random.Range(0, 100) + 1;
            }

            if (AutoFollow)
            {
                Vector3 ShootPoint;
                if (Team == TeamKind.Self)
                    ShootPoint = SceneMgr.Get.ShootPoint [1].transform.position;
                else
                    ShootPoint = SceneMgr.Get.ShootPoint [0].transform.position;    

                if (Vector3.Distance(ShootPoint, DefPlayer.transform.position) <= 12)
                {
                    AutoFollow = false;
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
            NoAiTime = Time.time + ChangeToAI;

            if (AIActiveHint)
                AIActiveHint.SetActive(false);
        } else
        {
            NoAiTime = 0;
            if (AIActiveHint)
                AIActiveHint.SetActive(true);
        }
    }

    public void SetAiTime()
    {
        NoAiTime = 0;
        if (AIActiveHint)
            AIActiveHint.SetActive(true);
    }
    
    private void CalculationAirResistance()
    {
        if (gameObject.transform.localPosition.y > 1f)
        {
            drag = Vector2.Lerp(Vector2.zero, new Vector2(0, gameObject.transform.localPosition.y), 0.01f); 
            PlayerRigidbody.drag = drag.y;
        } else
        {
            drag = Vector2.Lerp(new Vector2(0, gameObject.transform.localPosition.y), Vector2.zero, 0.01f); 
            if (drag.y >= 0)
                PlayerRigidbody.drag = drag.y;
            else
                PlayerRigidbody.drag = 0;
        }
    }

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

            if (!isDunkZmove && dunkCurveTime >= playerDunkCurve.StartMoveTime)
            {
                isDunkZmove = true;
                gameObject.transform.DOMoveZ(SceneMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.z, playerDunkCurve.ToBasketTime - playerDunkCurve.StartMoveTime).SetEase(Ease.Linear);
                gameObject.transform.DOMoveX(SceneMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.x, playerDunkCurve.ToBasketTime - playerDunkCurve.StartMoveTime).SetEase(Ease.Linear);
            }

            if (dunkCurveTime >= playerDunkCurve.LifeTime)
                isDunk = false;
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
				gameObject.transform.DOMoveZ(SceneMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.z, playerLayupCurve.ToBasketTime - playerLayupCurve.StartMoveTime).SetEase(Ease.Linear);
				gameObject.transform.DOMoveX(SceneMgr.Get.DunkPoint [Team.GetHashCode()].transform.position.x, playerLayupCurve.ToBasketTime - playerLayupCurve.StartMoveTime).SetEase(Ease.Linear);
			}
			
			if (layupCurveTime >= playerLayupCurve.LifeTime)
				isLayup = false;
		} else
		{
			isLayup = false;
			Debug.LogError("playCurve is null");
		}
	}
	
	private void CalculationRebound()
	{
		if (!isRebound)
			return;
		
		if (playerReboundCurve != null)
		{
			reboundCurveTime += Time.deltaTime;
			
			if (reboundCurveTime < 1f)
				gameObject.transform.position = new Vector3(gameObject.transform.position.x + (gameObject.transform.forward.x * 0.05f), 
				                                            playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
				                                            gameObject.transform.position.z + (gameObject.transform.forward.z * 0.05f));
			else
				gameObject.transform.position = new Vector3(gameObject.transform.position.x, 
				                                            playerReboundCurve.aniCurve.Evaluate(reboundCurveTime), 
				                                            gameObject.transform.position.z);
			
			if (reboundCurveTime >= playerReboundCurve.LifeTime)
			{
				isRebound = false;
				isCheckLayerToReset = true;
			}
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
                DelActionFlag(ActionFlag.IsDribble);
                DelActionFlag(ActionFlag.IsRun);
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

    private void CalculationPlayerHight()
    {
        animator.SetFloat("CrtHight", gameObject.transform.localPosition.y);

        if (isCheckLayerToReset && !isTouchPalyer)
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            isCheckLayerToReset = false;
        }

    }

    public void OnJoystickMove(MovingJoystick move, PlayerState ps)
    {
        if (CanMove || stop || HoldBallCanMove)
        {
            if (situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
            {
                if (Mathf.Abs(move.joystickAxis.y) > 0 || Mathf.Abs(move.joystickAxis.x) > 0)
                {
                    isMoving = true;
                    if (!isJoystick)
						MoveStartTime = Time.time + GameConst.DefMoveTime;
					
//                  if (!CheckAction(ActionFlag.IsRun))
//                      AddActionFlag(ActionFlag.IsRun);
                    
                    SetNoAiTime();
                    
                    animationSpeed = Vector2.Distance(new Vector2(move.joystickAxis.x, 0), new Vector2(0, move.joystickAxis.y));
                    SetSpeed(animationSpeed, 0);
                    AniState(ps);
                    
                    float angle = move.Axis2Angle(true);
                    int a = 90;
                    Vector3 rotation = new Vector3(0, angle + a, 0);
                    transform.rotation = Quaternion.Euler(rotation);
                    
                    if (animationSpeed <= MoveMinSpeed)
                    {
                        if (IsBallOwner)
                            Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.BallOwnerSpeedNormal;
                        else
                        {
                            if (IsDefence)
                            {
                                Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.DefSpeedNormal;
                            } else
                                Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.AttackSpeedNormal;
                        }                       
                    } else
                    {
                        if (IsBallOwner)
                            Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.BallOwnerSpeedup;
                        else
                        {
                            if (IsDefence)
                                Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.DefSpeedup;
                            else
                                Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.AttackSpeedup;
                        }
                    }
                    
                    transform.Translate(Translate); 
                }
            }            
        }
    }

    public void OnJoystickMoveEnd(MovingJoystick move, PlayerState ps)
    {
        if (situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
        {
            isMoving = false;
            SetNoAiTime();
            isJoystick = false;
            if (crtState != ps)
                AniState(ps);

            if (situation == GameSituation.AttackA)
                rotateTo(SceneMgr.Get.ShootPoint [0].transform.position.x, SceneMgr.Get.ShootPoint [0].transform.position.z);
            else if (situation == GameSituation.AttackB)
                rotateTo(SceneMgr.Get.RealBall.transform.position.x, SceneMgr.Get.RealBall.transform.position.z);
        }
    }

    private void DunkTo()
    {
        if (GameStart.Get.TestMode == GameTest.Dunk)
        {
            if (dkPathGroup)
                Destroy(dkPathGroup);

            dkPathGroup = new GameObject();
            dkPathGroup.name = "pathGroup";
        }

//        PlayerRigidbody.useGravity = false;
//        PlayerRigidbody.isKinematic = true;

//        dunkPath [4] = SceneMgr.Get.DunkPoint [Team.GetHashCode()].transform.position;
//        float dis = Vector3.Distance(gameObject.transform.position, dunkPath [4]);
//        float maxH = DunkHight [0] + (DunkHight [1] - DunkHight [0] / (dis * 0.25f));
//        dunkPath [0] = gameObject.transform.position;
//        dunkPath [2] = new Vector3((dunkPath [dunkPath.Length - 1].x + dunkPath [0].x) / 2, maxH, (dunkPath [dunkPath.Length - 1].z + dunkPath [0].z) / 2);
//        dunkPath [3] = new Vector3((dunkPath [dunkPath.Length - 1].x + dunkPath [2].x) / 2, DunkHight [1], (dunkPath [dunkPath.Length - 1].z + dunkPath [2].z) / 2);
//        dunkPath [1] = new Vector3((dunkPath [2].x + dunkPath [0].x) / 2, 6, (dunkPath [2].z + dunkPath [0].z) / 2);

        playerDunkCurve = null;
        for (int i = 0; i < aniCurve.Dunk.Length; i++)
            if (aniCurve.Dunk [i].Name == "Dunk")
                playerDunkCurve = aniCurve.Dunk [i];

        isDunk = true;
        isDunkZmove = false;
        dunkCurveTime = 0;
    }

//    private void PathCallBack()
//    {
//        Vector3[] path2 = new Vector3[2];
//        path2 = new Vector3[2]{dunkPath [3], dunkPath [4]};
//        gameObject.transform.DOPath(path2, 0.4f, PathType.CatmullRom, PathMode.Full3D, 10, Color.red).SetEase(Ease.OutBack);
//    }
    
    private int MinIndex(float[] floatAy, bool getmin = false)
    {
        int Result = 0;
        int Result2 = floatAy.Length - 1;

        float Min = floatAy [0];
        
        for (int i = 1; i < floatAy.Length; i++)
        {
            if (floatAy [i] < Min)
            {
                Min = floatAy [i];
                Result2 = Result;
                Result = i;
            }
        }
        
        if (getmin)
            return Result;
        else
            return Result2;
    }

    private Vector2 GetMoveTarget(TMoveData Data)
    {
        Vector2 Result = Vector2.zero;

        if (Data.DefPlayer != null)
        {
            float dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);
            float [] disAy = new float[4];
            for (int i = 0; i < disAy.Length; i++)
                disAy [i] = Vector3.Distance(Data.DefPlayer.DefPointAy [i].position, SceneMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);

            int mIndex = MinIndex(disAy, Data.DefPlayer == DefPlayer);

            if (mIndex >= 0 && mIndex < disAy.Length)
            {
                Result = new Vector2(Data.DefPlayer.DefPointAy [mIndex].position.x, Data.DefPlayer.DefPointAy [mIndex].position.z);                 
                
                if ((GameData.AIlevelAy [Attr.AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner && dis <= GameConst.TreePointDistance) || dis <= 6 && Data.DefPlayer == DefPlayer)
                    Result = new Vector2(Data.DefPlayer.DefPointAy [mIndex + 4].position.x, Data.DefPlayer.DefPointAy [mIndex + 4].position.z);
            }
        } else if (Data.FollowTarget != null)
            Result = new Vector2(Data.FollowTarget.position.x, Data.FollowTarget.position.z);
        else
            Result = Data.Target;

        return Result;
    }

    public void MoveTo(TMoveData Data, bool First = false)
    {
        if ((CanMove || (NoAiTime == 0 && HoldBallCanMove)) && WaitMoveTime == 0)
        {
            isMoving = true;
            Vector2 MoveTarget = GetMoveTarget(Data);

            if ((gameObject.transform.localPosition.x <= MoveTarget.x + MoveCheckValue && gameObject.transform.localPosition.x >= MoveTarget.x - MoveCheckValue) && 
                (gameObject.transform.localPosition.z <= MoveTarget.y + MoveCheckValue && gameObject.transform.localPosition.z >= MoveTarget.y - MoveCheckValue))
            {
                MoveTurn = 0;
				isMoving = false;

                if (IsDefence)
                {
                    WaitMoveTime = 0;
                    if (Data.DefPlayer != null)
                    {
                        float dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);

                        if (Data.LookTarget != null)
                        {
                            if (Vector3.Distance(this.transform.position, Data.DefPlayer.transform.position) <= GameConst.StealBallDistance)
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

                    AniState(PlayerState.Defence);                          
                } else
                {
                    if (!IsBallOwner)
                        AniState(PlayerState.Idle);
                    else if (situation == GameSituation.TeeA || situation == GameSituation.TeeB)
                        AniState(PlayerState.Dribble);
                    
                    if (First || GameStart.Get.TestMode == GameTest.Edit)
                        WaitMoveTime = 0;
                    else if (situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
                    {
                        float dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint [Team.GetHashCode()].transform.position);
                        if (dis <= 8)
                            WaitMoveTime = Time.time + UnityEngine.Random.Range(0, 1);
                        else
                            WaitMoveTime = Time.time + UnityEngine.Random.Range(0, 3);
                    }
                    
                    if (IsBallOwner)
                    {
                        if (Team == TeamKind.Self)
                            rotateTo(SceneMgr.Get.ShootPoint [0].transform.position.x, SceneMgr.Get.ShootPoint [0].transform.position.z);
                        else
                            rotateTo(SceneMgr.Get.ShootPoint [1].transform.position.x, SceneMgr.Get.ShootPoint [1].transform.position.z);

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
                                if (Team == TeamKind.Self)
                                    rotateTo(SceneMgr.Get.ShootPoint [0].transform.position.x, SceneMgr.Get.ShootPoint [0].transform.position.z);
                                else
                                    rotateTo(SceneMgr.Get.ShootPoint [1].transform.position.x, SceneMgr.Get.ShootPoint [1].transform.position.z);
                            }
                        } else
                            rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);

                        if (Data.Catcher)
                        {
                            if ((situation == GameSituation.AttackA || situation == GameSituation.AttackB) && NoAiTime == 0)
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
            } else 
            if ((IsDefence == false && MoveTurn >= 0 && MoveTurn <= 5) && GameController.Get.BallOwner != null)
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
                        float dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);
                        float dis2 = Vector3.Distance(transform.position, Data.DefPlayer.transform.position);
//                        if (Data.LookTarget == null || dis > GameConst.TreePointDistance + 4)
//                            rotateTo(MoveTarget.x, MoveTarget.y);
//                        else
//                            rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);

                        dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);
                        dis2 = Vector3.Distance(new Vector3(MoveTarget.x, 0, MoveTarget.y), SceneMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);
                        float dis3 = Vector3.Distance(Data.DefPlayer.transform.position, SceneMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);

                        if (dis <= GameConst.TreePointDistance + 4)
                        {
                            if (dis2 < dis)
                            {
                                if (dis > dis3)
                                {
                                    rotateTo(MoveTarget.x, MoveTarget.y);
                                    AniState(PlayerState.RunningDefence);
                                } else
                                {
                                    rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
                                    AniState(PlayerState.MovingDefence);
                                }
                            } else
                            {
                                rotateTo(MoveTarget.x, MoveTarget.y);
                                AniState(PlayerState.RunningDefence);
                            }
                        } else
                        {
                            if (Data.LookTarget == null)
                            {
                                rotateTo(MoveTarget.x, MoveTarget.y);
                                AniState(PlayerState.RunningDefence);
                            } else if (Vector3.Distance(transform.position, Data.LookTarget.position) <= 1.5f)
                            {
                                rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
                                AniState(PlayerState.MovingDefence);
                            } else
                            {
                                rotateTo(MoveTarget.x, MoveTarget.y);
                                AniState(PlayerState.RunningDefence);
                            }
                        }
                    } else
                    {
                        rotateTo(MoveTarget.x, MoveTarget.y);
                        AniState(PlayerState.Run);
                    }

                    if (Data.Speedup)
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * GameConst.DefSpeedup * GameConst.BasicMoveSpeed);
                    else
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * GameConst.DefSpeedNormal * GameConst.BasicMoveSpeed);
                } else
                {
                    rotateTo(MoveTarget.x, MoveTarget.y);

                    if (IsBallOwner)
                        AniState(PlayerState.RunAndDribble);
                    else
                        AniState(PlayerState.Run);

                    if (IsBallOwner)
                    {
                        if (Data.Speedup)
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedup * GameConst.BasicMoveSpeed);
                        else
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedNormal * GameConst.BasicMoveSpeed);
                    } else
                    {
                        if (Data.Speedup)
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedup * GameConst.BasicMoveSpeed);
                        else
                            transform.Translate(Vector3.forward * Time.deltaTime * GameConst.AttackSpeedNormal * GameConst.BasicMoveSpeed);
                    }
                }
            }       
        } else
            isMoving = false;
    }

    public void rotateTo(float lookAtX, float lookAtZ)
    {
		if (IsShoot || isRebound || isBlock)
			return;
//        transform.rotation = Quaternion.Lerp(transform.rotation, 
//                             Quaternion.LookRotation(new Vector3(lookAtX, transform.localPosition.y, lookAtZ) - 
//            transform.localPosition), time * Time.deltaTime);

        Vector3 lookAtPos = new Vector3(lookAtX, 0, lookAtZ);
        Vector3 relative = gameObject.transform.InverseTransformPoint(lookAtPos);
        float mangle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

        if ((mangle > 15 && mangle < 180) || (mangle < -15 && mangle > -180))
        {
            gameObject.transform.DOLookAt(lookAtPos, 0.1f);
        }
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
        if (dir == 0)
            animator.SetFloat("MoveSpeed", value);
        else
        if (dir != -2)
            smoothDirection = dir;
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
        if (CheckAnimatorSate(PlayerState.Idle) || CheckAnimatorSate(PlayerState.RunAndDribble) || CheckAnimatorSate(PlayerState.Dribble))
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
        } else
            NeedResetFlag = true;
    }

    public void ClearMoveQueue()
    {
        MoveQueue.Clear();
        FirstMoveQueue.Clear();
    }

    public bool CanUseState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.PassFlat:
            case PlayerState.PassFloor:
            case PlayerState.PassParabola:
            case PlayerState.Tee:
                if (crtState == PlayerState.HoldBall || crtState == PlayerState.Dribble || crtState == PlayerState.RunAndDribble)
                    return true;
                break;
            
            case PlayerState.BlockCatch:
                if (crtState == PlayerState.Block) 
                    return true;
                break;

            case PlayerState.FakeShoot:
            case PlayerState.Shoot0:
            case PlayerState.Shoot1:
            case PlayerState.Shoot2:
            case PlayerState.Shoot3:
            case PlayerState.Shoot6:
            case PlayerState.Dunk:
            case PlayerState.Layup:
                if (IsBallOwner && (crtState == PlayerState.HoldBall || crtState == PlayerState.Dribble || crtState == PlayerState.RunAndDribble))
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
           
            case PlayerState.Push:
            case PlayerState.PickBall:
            case PlayerState.Steal:
            case PlayerState.Block:
                if (CanMove && !IsBallOwner && (crtState == PlayerState.Idle || crtState == PlayerState.Run || crtState == PlayerState.MovingDefence ||
                    crtState == PlayerState.Defence || crtState == PlayerState.RunningDefence))
                    return true;
                break;

            case PlayerState.Elbow:
                if (IsBallOwner && (crtState == PlayerState.Dribble || crtState == PlayerState.RunAndDribble || crtState == PlayerState.HoldBall))
                    return true;
                break;

            case PlayerState.Fall0:
            case PlayerState.Fall1:
                if (crtState != state && crtState != PlayerState.Elbow && 
                    (crtState == PlayerState.Dribble || crtState == PlayerState.RunAndDribble || crtState == PlayerState.HoldBall || 
                    crtState == PlayerState.Idle || crtState == PlayerState.Run || crtState == PlayerState.Defence || crtState == PlayerState.MovingDefence || 
                    crtState == PlayerState.RunningDefence))
                    return true;
                break;

            case PlayerState.GotSteal:
                if (crtState != state && crtState != PlayerState.Elbow && 
                    (crtState == PlayerState.Dribble ||
                    crtState == PlayerState.RunAndDribble || 
                    crtState == PlayerState.FakeShoot || 
                    crtState == PlayerState.HoldBall || 
                    crtState == PlayerState.Idle || 
                    crtState == PlayerState.Run || 
                    crtState == PlayerState.Defence || 
                    crtState == PlayerState.MovingDefence || 
                    crtState == PlayerState.RunningDefence))
                    return true;
                break;

            case PlayerState.RunAndDribble:
            case PlayerState.Dribble:
                if (IsFirstDribble && !CanMove || CanMove)
                    return true;
                break;
            
            case PlayerState.Run:        
            case PlayerState.RunningDefence:
            case PlayerState.Defence:
            case PlayerState.MovingDefence:
            case PlayerState.CatchFlat:
            case PlayerState.CatchFloor:
            case PlayerState.CatchParabola:
                if (CanMove)
                    return true;
                break;

            case PlayerState.Idle:
                return true;
        }

        return false;
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
        
        switch (state)
        {
            case PlayerState.Block:
                gameObject.layer = LayerMask.NameToLayer("Shooter");
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

            case PlayerState.Defence:
                ClearAnimatorFlag();
                SetSpeed(0, -1);
                AddActionFlag(ActionFlag.IsDefence);
                Result = true;
                break;

            case PlayerState.Dunk:
                if (IsBallOwner && Vector3.Distance(SceneMgr.Get.ShootPoint [Team.GetHashCode()].transform.position, gameObject.transform.position) < canDunkDis)
                {
                    PlayerRigidbody.useGravity = false;
                    ClearAnimatorFlag();
                    animator.SetTrigger("DunkTrigger");
                    isCanCatchBall = false;
                    gameObject.layer = LayerMask.NameToLayer("Shooter");
                    DunkTo();
                    Result = true;
                }
                break;

            case PlayerState.Dribble:
                if (GameController.Get.BallOwner == this)
                {
                    if (!isJoystick)
                        SetSpeed(0, -1);
                    ClearAnimatorFlag();
                    AddActionFlag(ActionFlag.IsDribble);
                    SceneMgr.Get.SetBallState(PlayerState.Dribble, this);
                    isCanCatchBall = false;
                    IsFirstDribble = false;
                    Result = true;
                }
                break;

			case PlayerState.Elbow:
				ClearAnimatorFlag();
				animator.SetTrigger("ElbowTrigger");
				isCanCatchBall = false;
				GameController.Get.RealBallFxTime = 1f;
				SceneMgr.Get.RealBallFX.SetActive(true);
				Result = true;
				break;

            case PlayerState.FakeShoot:
                UIGame.Get.DoPassNone();
                if (IsBallOwner)
                {
					ClearAnimatorFlag();
					animator.SetTrigger("FakeShootTrigger");
					isCanCatchBall = false;
					GameController.Get.RealBallFxTime = 1f;
					SceneMgr.Get.RealBallFX.SetActive(true);
                    Result = true;
                }
                break;

            case PlayerState.Fall0:
                isDunk = false;
                isShootJump = false;
                ClearAnimatorFlag();
                animator.SetInteger("StateNo", 0);
                animator.SetTrigger("FallTrigger");
                isCanCatchBall = false;
                gameObject.transform.DOLocalMoveY(0, 1f);
                if (OnFall != null)
                    OnFall(this);
                Result = true;
                break;

            case PlayerState.Fall1:
                isDunk = false;
                isShootJump = false;
                ClearAnimatorFlag();
                animator.SetInteger("StateNo", 1);
                animator.SetTrigger("FallTrigger");
                isCanCatchBall = false;
                gameObject.transform.DOLocalMoveY(0, 1f);
                if (OnFall != null)
                    OnFall(this);
                Result = true;
                break;

            case PlayerState.HoldBall:
                ClearAnimatorFlag();
                AddActionFlag(ActionFlag.IsHoldBall);
                isCanCatchBall = false;
                Result = true;
                break;
            
            case PlayerState.Idle:
                SetSpeed(0, -1);
                ClearAnimatorFlag();
                isCanCatchBall = true;
                Result = true;
                break;

            case PlayerState.MovingDefence:
                isCanCatchBall = true;
                SetSpeed(1, 1);
                ClearAnimatorFlag();
                AddActionFlag(ActionFlag.IsDefence);
                Result = true;
                break;

            case PlayerState.PassFlat:
                animator.SetInteger("StateNo", 0);
                UIGame.Get.DoPassNone();
                ClearAnimatorFlag();
                animator.SetTrigger("PassTrigger");
                Result = true;
                break;

            case PlayerState.PassFloor:
                animator.SetInteger("StateNo", 2);
                UIGame.Get.DoPassNone();
                ClearAnimatorFlag();
                animator.SetTrigger("PassTrigger");
                Result = true;
                break;
            
            case PlayerState.PassParabola:
                animator.SetInteger("StateNo", 1);
                UIGame.Get.DoPassNone();
                ClearAnimatorFlag();
                animator.SetTrigger("PassTrigger");
                Result = true;
                break;

            case PlayerState.Push:
                ClearAnimatorFlag();
                animator.SetTrigger("PushTrigger");
                Result = true;
                break;

            case PlayerState.PickBall:
                isCanCatchBall = true;
                ClearAnimatorFlag();
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
            
            case PlayerState.Run:
                if (!isJoystick)
                    SetSpeed(1, 1);
                ClearAnimatorFlag();
                AddActionFlag(ActionFlag.IsRun);
                Result = true;
                break;

            case PlayerState.RunAndDribble:
                if (!isJoystick)
                    SetSpeed(1, 1);
                else
                    SetSpeed(1, 0);
                ClearAnimatorFlag();
                AddActionFlag(ActionFlag.IsDribble);
                IsFirstDribble = false;
                Result = true;
                break;

            case PlayerState.RunningDefence:
                SetSpeed(1, 1);
                ClearAnimatorFlag();
                AddActionFlag(ActionFlag.IsRun);
                Result = true;
                break;

            case PlayerState.Steal:
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
                UIGame.Get.DoPassNone();
                
                if (IsBallOwner)
                {     
                    playerShootCurve = null;
                    
                    switch (state)
                    {
                        case PlayerState.Shoot0:
                            stateNo = 0;
                            break;
                        case PlayerState.Shoot1:
                            stateNo = 1;
                            break;
                        case PlayerState.Shoot2:
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

                    gameObject.layer = LayerMask.NameToLayer("Shooter");
                    ClearAnimatorFlag();
                    animator.SetTrigger("ShootTrigger");
                    isCanCatchBall = false;
                    Result = true;
                }
                break;

		case PlayerState.Layup:
			if (IsBallOwner){
				UIGame.Get.DoPassNone();
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

				gameObject.layer = LayerMask.NameToLayer("Shooter");
				ClearAnimatorFlag();
				animator.SetTrigger("LayupTrigger");
				isCanCatchBall = false;
				Result = true;
			}
			break;

            case PlayerState.Rebound:
				UIGame.Get.DoPassNone();
				playerReboundCurve = null;
				for (int i = 0; i < aniCurve.Rebound.Length; i++)
				if (aniCurve.Rebound [i].Name == "Rebound")
				{
					playerReboundCurve = aniCurve.Rebound [i];
					reboundCurveTime = 0;
					isRebound = true;
				}
				ClearAnimatorFlag();
                gameObject.layer = LayerMask.NameToLayer("Shooter");
                animator.SetTrigger("ReboundTrigger");
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

    public void ClearAnimatorFlag()
    {
        DelActionFlag(ActionFlag.IsDefence);
        DelActionFlag(ActionFlag.IsRun);
        DelActionFlag(ActionFlag.IsDribble);
        DelActionFlag(ActionFlag.IsHoldBall);
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
            case "BlockJump":
                if (OnBlockJump != null)
                    OnBlockJump(this);

                break;
            case "Blocking":
//                if (OnBlocking != null)
//                    OnBlocking(this);

                break;
            case "Shooting":
                if (OnShooting != null)
                    OnShooting(this);
                break;

            case "Passing": 
                //0.Flat
                //2.Floor
                //1 3.Parabola(Tee)
                if (IsBallOwner)
                    SceneMgr.Get.RealBallTrigger.PassBall(animator.GetInteger("StateNo"));      
                break;
            case "PickUp": 
                if (OnPickUpBall != null)
                    OnPickUpBall(this);
                break;
            case "PickEnd":
                AniState(PlayerState.Dribble);
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

            case "DunkJump":
                DelActionFlag(ActionFlag.IsDribble);
                DelActionFlag(ActionFlag.IsRun);
            
                SceneMgr.Get.SetBallState(PlayerState.Dunk);
                if (OnDunkJump != null)
                    OnDunkJump(this);
                break;

            case "DunkBasket":
                DelActionFlag(ActionFlag.IsDribble);
                DelActionFlag(ActionFlag.IsRun);
                SceneMgr.Get.PlayDunk(Team.GetHashCode());
                break;
            case "DunkFallBall":
                if (OnDunkBasket != null)
                    OnDunkBasket(this);
                break;

            case "ElbowEnd":
                UIGame.Get.OpenUIMask(this);
                AniState(PlayerState.HoldBall);
                break;

            case "CatchEnd":
                UIGame.Get.OpenUIMask(this);
                IsFirstDribble = true;
                
                if (NoAiTime == 0)
                    AniState(PlayerState.Dribble);
                else 
                    AniState(PlayerState.HoldBall);
                break;

            case "FakeShootEnd":
                AniState(PlayerState.HoldBall);
                UIGame.Get.OpenUIMask(this);
                break;

            case "AnimationEnd":
                AniState(PlayerState.Idle);
                UIGame.Get.OpenUIMask(this);
                blockTrigger.SetActive(false);
                pushTrigger.SetActive(false);
                elbowTrigger.SetActive(false);
                isCanCatchBall = true;
                PlayerRigidbody.useGravity = true;

                if (!NeedResetFlag)
                    isCheckLayerToReset = true;

                if (NeedResetFlag)
                    ResetFlag();
                break;
        }
    }

    public void ResetMove()
    {
        MoveQueue.Clear();
        DelActionFlag(ActionFlag.IsRun);
        WaitMoveTime = 0;
    }

    public void SetAutoFollowTime()
    {
        if (CloseDef == 0 && AutoFollow == false)
        {
            CloseDef = Time.time + GameData.AIlevelAy [Attr.AILevel].AutoFollowTime;
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
                PlayerState.Dunk,
                PlayerState.Elbow,
                PlayerState.FakeShoot,
                PlayerState.Fall0,
                PlayerState.Fall1,
                PlayerState.HoldBall,
                PlayerState.GotSteal,
                PlayerState.PassFlat,
                PlayerState.PassFloor,
                PlayerState.PassParabola,
                PlayerState.Push,
                PlayerState.PickBall,
                PlayerState.Shoot0,
                PlayerState.Shoot1,
                PlayerState.Shoot2,
                PlayerState.Shoot3,
                PlayerState.Shoot6,
				PlayerState.Steal,
                PlayerState.Layup,
                PlayerState.Tee,
                PlayerState.Rebound
            };

            for (int i = 0; i < CheckAy.Length; i++)
                if (CheckAnimatorSate(CheckAy [i]))
                    return false;
        
            return true;
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

    public bool IsDefence
    {
        get
        {
            if (situation == GameSituation.AttackA && Team == TeamKind.Npc)
                return true;
            else if (situation == GameSituation.AttackB && Team == TeamKind.Self)
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
        get { return SceneMgr.Get.RealBall.transform.parent == DummyBall.transform;}
        set { animator.SetBool("IsBallOwner", value);}
    }

	public bool IsShoot
	{
		get{ return crtState == PlayerState.Shoot0 || crtState == PlayerState.Shoot1 || crtState == PlayerState.Shoot2 || 
			crtState == PlayerState.Shoot3 || crtState == PlayerState.Shoot6 || crtState == PlayerState.Layup || crtState == PlayerState.Dunk;}
	}

    public bool IsPass
    {
        get{ return crtState == PlayerState.PassFlat || crtState == PlayerState.PassFloor || crtState == PlayerState.PassParabola || crtState == PlayerState.Tee;}

    }

    private bool isMoving = false;

    public bool IsMoving
    {
        get{ return isMoving;}
    }

    public bool IsFall
    {
        get{ return crtState == PlayerState.Fall0 || crtState == PlayerState.Fall1;}
        
    }

    public bool IsCatch
    {
        get{ return crtState == PlayerState.CatchFlat || crtState == PlayerState.CatchFloor || crtState == PlayerState.CatchParabola;}
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

    public TMoveData TargetPos
    {
        set
        {
            if (MoveQueue.Count == 0)
                MoveTurn = 0;

            MoveQueue.Enqueue(value);
        }
    }

    public TMoveData FirstTargetPos
    {
        set
        {
            if (FirstMoveQueue.Count < 2)
                FirstMoveQueue.Enqueue(value);
        }
    }

    private bool isTouchPalyer = false;

    void OnCollisionStay(Collision collisionInfo)
    {
        if (!isTouchPalyer && collisionInfo.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isTouchPalyer = true;
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (isTouchPalyer && collisionInfo.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isTouchPalyer = false;
        }
    }
}