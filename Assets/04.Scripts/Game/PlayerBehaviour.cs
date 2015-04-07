using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RootMotion.FinalIK;

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
    BlockCatch = 11,
    Layup = 12, 
    Pass = 13,  
    Steal = 14, 
    MovingDefence = 23,
    RunAndDribble = 24,
    Shooting = 25,
    Catch = 26,
    DunkBasket = 27,
    RunningDefence = 28,
    FakeShoot = 29
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
    IsBlock = 3,
    IsBlockCatch = 4,
    IsDribble = 5,
    IsSteal = 6,
    IsPass = 7,
    IsShoot = 8,
    IsCatcher = 9,
    IsDunk = 10,
    IsShootIdle = 11,
    IsFakeShoot = 12
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

public class PlayerBehaviour : MonoBehaviour
{
    public OnPlayerAction OnShooting = null;
    public OnPlayerAction OnPass = null;
    public OnPlayerAction OnSteal = null;
    public OnPlayerAction OnBlockJump = null;
    public OnPlayerAction OnBlocking = null;
    public OnPlayerAction OnDunkBasket = null;
    public OnPlayerAction OnDunkJump = null;
    public OnPlayerAction OnBlockMoment = null;
    public Vector3 Translate;
    public float[] DunkHight = new float[2]{3, 5};
    private const float MoveCheckValue = 0.5f;
    private const int ChangeToAI = 4;
    public static string[] AnimatorStates = new string[] {
                "",
                "IsRun",
                "IsDefence",
                "IsBlock",
                "IsBlockCatch",
                "IsDribble",
                "IsSteal",
                "IsPass",
                "IsShoot",
                "IsCatcher",
                "IsDunk",
                "IsShootIdle",
                "IsFakeShoot"
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
    private float PassTime = 0;
    private float NoAiTime = 0;
    private float MoveStartTime = 0;
    private float ProactiveRate = 0;
    private float ProactiveTime = 0;
    private int smoothDirection = 0;
    private float animationSpeed = 0;
    private float AniWaitTime = 0;
    private Collider playerCollider;
    public Rigidbody PlayerRigidbody;
    private Animator animator;
    private GameObject selectTexture;
    public GameObject AIActiveHint = null;
    public GameObject DummyBall;
    public TeamKind Team;
    public int Index;
    public GameSituation situation = GameSituation.None;
    public PlayerState crtState = PlayerState.Idle;
    public Transform[] DefPointAy = new Transform[8];
    public float WaitMoveTime = 0;
    public float Invincible = 0;
    public float JumpHight = 450f;
    public float CoolDownSteal = 0;
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
    private bool isZmove = false;
    private float dunkCurveTime = 0;
	private GameObject dkPathGroup;
    private Vector3[] dunkPath = new Vector3[5];
    public AniCurve aniCurve;
    private TDunkCurve playerDunkCurve;

	//Block
	private float blockCurveTime = 0;
    private TBlockCurve playerBlockCurve;

	//Shooting
	private float shootJumpCurveTime = 0;
	private TShootCurve playerShootCurve;
	private bool isShootJump = false;


	//IK
	private AimIK aimIK;
	private Transform pinIKTransform;
	public Transform IKTarget;
	public bool isIKOpen = true;

    void initTrigger()
    {
        GameObject obj = Resources.Load("Prefab/Player/BodyTrigger") as GameObject;
        if (obj)
        {
            GameObject obj2 = Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
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

            GameObject obj4 = obj.transform.FindChild("DefRange").gameObject;
            if (obj4 != null)
				obj4.transform.localScale = new Vector3(GameData.AIlevelAy [Attr.AILevel].DefDistance, GameData.AIlevelAy [Attr.AILevel].DefDistance, GameData.AIlevelAy [Attr.AILevel].DefDistance);
            
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
		pinIKTransform = transform.FindChild("Pin");
		IKTarget = SceneMgr.Get.RealBall.transform;

        animator = gameObject.GetComponent<Animator>();
        playerCollider = gameObject.GetComponent<Collider>();
//        PlayerRigidbody = gameObject.GetComponent<Rigidbody>();
        DummyBall = gameObject.transform.FindChild("DummyBall").gameObject;
        aniCurve = gameObject.transform.FindChild("AniCurve").gameObject.GetComponent<AniCurve>();
        initTrigger();
    }

	void LateUpdate() {
		if(aimIK != null) {
			if(pinIKTransform != null) {
				if(IKTarget != null) {
//					Vector3 t_self = new Vector3(IKTarget.position.x, pinIKTransform.position.y, IKTarget.position.z);
					Vector3 t_self = new Vector3(IKTarget.position.x, IKTarget.position.y, IKTarget.position.z);
					aimIK.solver.transform.LookAt(pinIKTransform.position);
					if(GameStart.Get.IsOpenIKSystem) {
						if(isIKOpen) {
							aimIK.enabled = true;
							aimIK.solver.IKPosition = IKTarget.position;
						} else {
							aimIK.enabled = false;
						}
					} else 
						aimIK.enabled = false;
					for (int i = 0; i < aimIK.solver.bones.Length; i++) {
						if (aimIK.solver.bones[i].rotationLimit != null) {
							aimIK.solver.bones[i].rotationLimit.SetDefaultLocalRotation();
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


        switch (crtState)
        {
            case PlayerState.Dunk:
            case PlayerState.DunkBasket:
                CalculationDunkMove();
                break;

            case PlayerState.Block: 
                CalculationBlock();
				break;

			case PlayerState.Shooting:
				CalculationShootJump();
				break;
        }
        
        if (WaitMoveTime > 0 && Time.time >= WaitMoveTime)
            WaitMoveTime = 0;

        if (Invincible > 0 && Time.time >= Invincible)
            Invincible = 0;

        if (CoolDownSteal > 0 && Time.time >= CoolDownSteal)
            CoolDownSteal = 0;

        if (PassTime > 0 && Time.time >= PassTime)
        {
            PassTime = 0;
            DelActionFlag(ActionFlag.IsPass);
        }   

        if (FirstMoveQueue.Count > 0)
            MoveTo(FirstMoveQueue.Peek(), true);
        else 
        if (MoveQueue.Count > 0)
            MoveTo(MoveQueue.Peek());

        if (AniWaitTime > 0 && AniWaitTime <= Time.time)
        {
            AniWaitTime = 0;
            if (NeedResetFlag)
                ResetFlag();
        }

        if (isJoystick)
        {
            if (Time.time >= NoAiTime)
            {
                MoveQueue.Clear();
                NoAiTime = 0;
                isJoystick = false;
                DelActionFlag(ActionFlag.IsRun);

                if (AIActiveHint)
                    AIActiveHint.SetActive(true);
            }
        }

        if (CheckAction(ActionFlag.IsRun) && !IsDefence && 
		    situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && 
		    situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
        {
            if (Time.time >= MoveStartTime)
            {
                MoveStartTime = Time.time + 0.5f;
                GameController.Get.DefMove(this);
            }       
        }
		if(GameStart.Get.IsOpenIKSystem) {
			if(IsBallOwner) 
				isIKOpen = false;
			else 
				isIKOpen = true;
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
        isJoystick = true;
        NoAiTime = Time.time + ChangeToAI;

        if (AIActiveHint)
            AIActiveHint.SetActive(false);
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

            if (!isZmove && dunkCurveTime > playerDunkCurve.StartMoveTime)
            {
                isZmove = true;
                gameObject.transform.DOLocalMoveZ(dunkPath [4].z, playerDunkCurve.ToBasketTime - playerDunkCurve.StartMoveTime).SetEase(Ease.Linear);
                gameObject.transform.DOLocalMoveX(dunkPath [4].x, playerDunkCurve.ToBasketTime - playerDunkCurve.StartMoveTime).SetEase(Ease.Linear);
            }


            if (dunkCurveTime >= playerDunkCurve.LifeTime)
                isDunk = false;
        } else
        {
            isDunk = false;
            Debug.LogError("playCurve is null");
        }
    }

    private void CalculationBlock()
    {
        if (crtState == PlayerState.Block && playerBlockCurve != null)
        {
            blockCurveTime += Time.deltaTime;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, playerBlockCurve.aniCurve.Evaluate(blockCurveTime), gameObject.transform.position.z);

			if(blockCurveTime >= playerBlockCurve.LifeTime )
			{
				DelActionFlag(ActionFlag.IsBlock);
				isCheckLayerToReset = true;
			}
        }
    }

	private void CalculationShootJump()
	{
		if (isShootJump && crtState == PlayerState.Shooting && playerShootCurve != null) {
			shootJumpCurveTime += Time.deltaTime;
			gameObject.transform.position = new Vector3 (gameObject.transform.position.x, playerShootCurve.aniCurve.Evaluate (shootJumpCurveTime), gameObject.transform.position.z);
			Debug.Log("logo : " + gameObject.transform.position);
			if(shootJumpCurveTime >= playerShootCurve.LifeTime)
				isShootJump = false;
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
        if (CanMove || stop)
        {
            if (Mathf.Abs(move.joystickAxis.y) > 0 || Mathf.Abs(move.joystickAxis.x) > 0)
            {
                if (!isJoystick)
                    MoveStartTime = Time.time + 1;

                if (!CheckAction(ActionFlag.IsRun))
                    AddActionFlag(ActionFlag.IsRun);

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

    public void OnJoystickMoveEnd(MovingJoystick move, PlayerState ps)
    {
        isJoystick = false;
        SetNoAiTime();
        if (crtState != ps)
            AniState(ps);
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

        dunkPath [4] = SceneMgr.Get.DunkPoint [Team.GetHashCode()].transform.position;
        float dis = Vector3.Distance(gameObject.transform.position, dunkPath [4]);
        float maxH = DunkHight [0] + (DunkHight [1] - DunkHight [0] / (dis * 0.25f));
        dunkPath [0] = gameObject.transform.position;
        dunkPath [2] = new Vector3((dunkPath [dunkPath.Length - 1].x + dunkPath [0].x) / 2, maxH, (dunkPath [dunkPath.Length - 1].z + dunkPath [0].z) / 2);
        dunkPath [3] = new Vector3((dunkPath [dunkPath.Length - 1].x + dunkPath [2].x) / 2, DunkHight [1], (dunkPath [dunkPath.Length - 1].z + dunkPath [2].z) / 2);
        dunkPath [1] = new Vector3((dunkPath [2].x + dunkPath [0].x) / 2, 6, (dunkPath [2].z + dunkPath [0].z) / 2);

        playerDunkCurve = null;
        for (int i = 0; i < aniCurve.Dunk.Length; i++)
            if (aniCurve.Dunk [i].Name == "Dunk")
                playerDunkCurve = aniCurve.Dunk [i];

        isDunk = true;
        isZmove = false;
        dunkCurveTime = 0;
    }

    private void PathCallBack()
    {
        Vector3[] path2 = new Vector3[2];
        path2 = new Vector3[2]{dunkPath [3], dunkPath [4]};
        gameObject.transform.DOPath(path2, 0.4f, PathType.CatmullRom, PathMode.Full3D, 10, Color.red).SetEase(Ease.OutBack);
    }
    
    
    private int MinIndex(float[] floatAy)
    {
        int Result = 0;
        float Min = floatAy [0];
        
        for (int i = 1; i < floatAy.Length; i++)
        {
            if (floatAy [i] < Min)
            {
                Min = floatAy [i];
                Result = i;
            }
        }
        
        return Result;
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

            int mIndex = MinIndex(disAy);

            if (mIndex >= 0 && mIndex < disAy.Length)
            {
                Result = new Vector2(Data.DefPlayer.DefPointAy [mIndex].position.x, Data.DefPlayer.DefPointAy [mIndex].position.z);                 
                
				if (GameData.AIlevelAy [Attr.AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner || dis <= 6)
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
        if (CanMove && WaitMoveTime == 0)
        {
            Vector2 MoveTarget = GetMoveTarget(Data);

            if ((gameObject.transform.localPosition.x <= MoveTarget.x + MoveCheckValue && gameObject.transform.localPosition.x >= MoveTarget.x - MoveCheckValue) && 
                (gameObject.transform.localPosition.z <= MoveTarget.y + MoveCheckValue && gameObject.transform.localPosition.z >= MoveTarget.y - MoveCheckValue))
            {
                MoveTurn = 0;
                DelActionFlag(ActionFlag.IsRun);

                if (IsDefence)
                {
                    WaitMoveTime = 0;
                    if (Data.LookTarget == null)
                        rotateTo(MoveTarget.x, MoveTarget.y);
                    else
                        rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
                    
                    AniState(PlayerState.Defence);                          
                } else
                {
                    if (!IsBallOwner)
                        AniState(PlayerState.Idle);
                    
                    if (First || GameStart.Get.TestMode == GameTest.Edit)
                        WaitMoveTime = 0;
                    else if (situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
                        WaitMoveTime = Time.time + UnityEngine.Random.Range(0, 3);
                    
                    if (IsBallOwner)
                    {
                        if (Team == TeamKind.Self)
                            rotateTo(SceneMgr.Get.ShootPoint [0].transform.position.x, SceneMgr.Get.ShootPoint [0].transform.position.z);
                        else
                            rotateTo(SceneMgr.Get.ShootPoint [1].transform.position.x, SceneMgr.Get.ShootPoint [1].transform.position.z);

						if(Data.Shooting)
							GameController.Get.Shoot();
                    } else 
					{
                        if (Data.LookTarget == null)
                            rotateTo(MoveTarget.x, MoveTarget.y);
                        else
                            rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);

						if(Data.Catcher)
						{
							if(!GameController.Get.IsPassing)
							{
								GameController.Get.Pass(this);
								NeedShooting = Data.Shooting;
							}
						}
                    }
                }

                if (Data.MoveFinish != null)
                    Data.MoveFinish(this, Data.Speedup);

                if (First)
                    FirstMoveQueue.Dequeue();
                else
                    MoveQueue.Dequeue();
            } else 
            if (!CheckAction(ActionFlag.IsDefence) && MoveTurn >= 0 && MoveTurn <= 5)
            {
                AddActionFlag(ActionFlag.IsRun);
                MoveTurn++;
                rotateTo(MoveTarget.x, MoveTarget.y, 10);
                if (MoveTurn == 1)
                    MoveStartTime = Time.time + 1;
            } else
            {
                if (IsDefence)
                {
                    if (Data.DefPlayer != null)
                    {
                        float dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);
                        float dis2 = Vector3.Distance(transform.position, Data.DefPlayer.transform.position);
                        if (Data.LookTarget == null || dis > GameConst.TreePointDistance)
                            rotateTo(MoveTarget.x, MoveTarget.y);
                        else
                            rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);

                        dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);
                        dis2 = Vector3.Distance(new Vector3(MoveTarget.x, 0, MoveTarget.y), SceneMgr.Get.ShootPoint [Data.DefPlayer.Team.GetHashCode()].transform.position);

                        if (dis <= GameConst.TreePointDistance)
                        {
                            if (dis2 < dis)
                                AniState(PlayerState.MovingDefence);
                            else
                                AniState(PlayerState.RunningDefence);
                        } else
                            AniState(PlayerState.RunningDefence);
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
                    rotateTo(MoveTarget.x, MoveTarget.y, 10);

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
        }
    }

    public void rotateTo(float lookAtX, float lookAtZ, float time = 50)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, 
                             Quaternion.LookRotation(new Vector3(lookAtX, transform.localPosition.y, lookAtZ) - 
            transform.localPosition), time * Time.deltaTime);
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

    public bool CheckAction(ActionFlag Flag)
    {
        return GameFunction.CheckByteFlag(Flag.GetHashCode(), PlayerActionFlag);
    }
    
    public void ResetFlag(bool ClearMove = true)
    {
        if (AniWaitTime == 0)
        {
            for (int i = 0; i < PlayerActionFlag.Length; i++)
                PlayerActionFlag [i] = 0;
            

            AniState(PlayerState.Idle);
			if(ClearMove)
			{
				MoveQueue.Clear();
				FirstMoveQueue.Clear();
			}
            NoAiTime = 0;
            WaitMoveTime = 0;
			NeedShooting = false;
            isJoystick = false; 
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
        if (crtState == state)
            return false;

        switch (state)
        {

            case PlayerState.Catch:
				if (crtState != PlayerState.FakeShoot && 
			        crtState != PlayerState.Dunk && 
			    	crtState != PlayerState.Pass && 
			    	crtState != PlayerState.Steal)
                    return true;
                break;
            case PlayerState.Steal:
				if (crtState != PlayerState.FakeShoot && 
				    crtState != PlayerState.Dunk && 
				    crtState != PlayerState.Pass)
                    return true;
                break;
            case PlayerState.Pass:
				if (crtState != PlayerState.FakeShoot && 
			    	crtState != PlayerState.Dunk)
                    return true;
                break;
            case PlayerState.Block:
				if (crtState != PlayerState.Steal)
                    return true;
                break;
            case PlayerState.BlockCatch:
				if (crtState != PlayerState.FakeShoot && 
			    	crtState != PlayerState.Dunk && 
			    	crtState != PlayerState.Pass)
                    return true;
                break;
            case PlayerState.Shooting:
				if (crtState != PlayerState.Dunk && 
			    	crtState != PlayerState.Pass && 
			    	crtState != PlayerState.Catch)
                    return true;
                break;
            case PlayerState.FakeShoot:
				if (crtState != PlayerState.FakeShoot && 
			    	crtState != PlayerState.Dunk && 
			    	crtState != PlayerState.Pass)
                    return true;
                break;
            case PlayerState.Dunk:
				if (crtState != PlayerState.Dunk && 
			    	crtState != PlayerState.Pass)
                    return true;
                break;
            case PlayerState.Idle:
            case PlayerState.Run:
            case PlayerState.Dribble:
            case PlayerState.RunAndDribble:
            case PlayerState.RunningDefence:
            case PlayerState.Defence:
            case PlayerState.MovingDefence:
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
        
        switch (state)
        {
            case PlayerState.Block:
                if (!CheckAction(ActionFlag.IsBlock))
                {
					playerBlockCurve = null;
					for (int i = 0; i < aniCurve.Block.Length; i++)
						if (aniCurve.Block [i].Name == "Block")
						{
							playerBlockCurve = aniCurve.Block [i];
							blockCurveTime = 0;
						}

                    AddActionFlag(ActionFlag.IsBlock);
                    Result = true;
                }
                break;

            case PlayerState.BlockCatch:
                if (!CheckAction(ActionFlag.IsBlockCatch))
                {
                    AddActionFlag(ActionFlag.IsBlockCatch);
                    Result = true;      
                }
                break;

            case PlayerState.Catch:
                SetSpeed(0, -1);
                AddActionFlag(ActionFlag.IsCatcher);
                Result = true;
                break;

            case PlayerState.Defence:
                DelActionFlag(ActionFlag.IsRun);
                SetSpeed(0, -1);
                AddActionFlag(ActionFlag.IsDefence);
                Result = true;
                break;

            case PlayerState.Dunk:
                if (!CheckAction(ActionFlag.IsDunk) && IsBallOwner && 
                    Vector3.Distance(SceneMgr.Get.ShootPoint [Team.GetHashCode()].transform.position, gameObject.transform.position) < canDunkDis)
                {
                    gameObject.layer = LayerMask.NameToLayer("Shooter");
                    AddActionFlag(ActionFlag.IsDunk);
                    AniWaitTime = Time.time + 2.9f;
                    DunkTo();
                    DelActionFlag(ActionFlag.IsPass);
                    DelActionFlag(ActionFlag.IsFakeShoot);
                    DelActionFlag(ActionFlag.IsShoot);
                    Result = true;
                }
                break;

            case PlayerState.Dribble:
                SetSpeed(0, -1);
                AddActionFlag(ActionFlag.IsDribble);
                Result = true;
                break;

			case PlayerState.FakeShoot:
				UIGame.Get.DoPassNone();
                if (!CheckAction(ActionFlag.IsShoot) && IsBallOwner)
                {
                    AddActionFlag(ActionFlag.IsFakeShoot);
                    AddActionFlag(ActionFlag.IsShoot);
                    Result = true;
                }
                break;

            case PlayerState.Idle:
                SetSpeed(0, -1);
                for (int i = 1; i < AnimatorStates.Length; i++)
                    if (AnimatorStates [i] != string.Empty && animator.GetBool(AnimatorStates [i]))
                        animator.SetBool(AnimatorStates [i], false);
                Result = true;
                break;

            case PlayerState.MovingDefence:
                SetSpeed(1, 1);
                AddActionFlag(ActionFlag.IsRun);
                AddActionFlag(ActionFlag.IsDefence);
                Result = true;
                break;

            case PlayerState.Pass:
				UIGame.Get.DoPassNone();
                if (!CheckAction(ActionFlag.IsPass))
                {
                    PassTime = Time.time + 3;
                    AddActionFlag(ActionFlag.IsPass);
                    Result = true;
                }
                break;

            case PlayerState.Run:
                if (!isJoystick)
                    SetSpeed(1, 1); 
                AddActionFlag(ActionFlag.IsRun);
                DelActionFlag(ActionFlag.IsDefence);
                Result = true;
                break;

            case PlayerState.RunAndDribble:
                if (!isJoystick)
                    SetSpeed(1, 1);
                else
                    SetSpeed(1, 0);
                AddActionFlag(ActionFlag.IsDribble);
                AddActionFlag(ActionFlag.IsRun);
                Result = true;
                break;

            case PlayerState.RunningDefence:
                SetSpeed(1, 1);

                AddActionFlag(ActionFlag.IsRun);
                DelActionFlag(ActionFlag.IsDefence);
                Result = true;
                break;


            case PlayerState.Steal:
                if (!CheckAction(ActionFlag.IsSteal))
                {
                    AddActionFlag(ActionFlag.IsSteal);
                    Result = true;
                }
                break;

			case PlayerState.Shooting:
				UIGame.Get.DoPassNone();
                if (!CheckAction(ActionFlag.IsShoot) && IsBallOwner)
                {
					playerShootCurve = null;
					for (int i = 0; i < aniCurve.Shoot.Length; i++)
						if (aniCurve.Shoot [i].Name == "Shoot")
						{
							playerShootCurve = aniCurve.Shoot[i];
							shootJumpCurveTime = 0;
						}

                    gameObject.layer = LayerMask.NameToLayer("Shooter");
                    AddActionFlag(ActionFlag.IsShoot);
                    DelActionFlag(ActionFlag.IsShootIdle);
                    DelActionFlag(ActionFlag.IsRun);
                    DelActionFlag(ActionFlag.IsDribble);
                    Result = true;
                }
                break;
        }

        if (Result)
            crtState = state;

        return Result;
    }
    
    public void AnimationEvent(string animationName)
    {
        switch (animationName)
        {
            case "StealEnd":
                if (OnSteal != null)
                    OnSteal(this);

                DelActionFlag(ActionFlag.IsSteal);
                break;
            case "ShootDown":
                DelActionFlag(ActionFlag.IsShoot);
                DelActionFlag(ActionFlag.IsDribble);
                DelActionFlag(ActionFlag.IsRun);
                isCheckLayerToReset = true;
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
                if (OnBlocking != null)
                    OnBlocking(this);

                break;
            case "BlockEnd":
//                DelActionFlag(ActionFlag.IsBlock);
//                isCheckLayerToReset = true;
                break;
            case "Shooting":
                if (OnShooting != null)
                    OnShooting(this);
//              playerCollider.enabled = true;
                break;
            case "ShootJump":
				isShootJump = true;
//              playerCollider.enabled = false;
//                PlayerRigidbody.AddForce(JumpHight * transform.up + PlayerRigidbody.velocity.normalized / 2.5f, ForceMode.Force);
                break;
            case "Passing":         
                if (PassTime > 0)
                {                   
                    if (!SceneMgr.Get.RealBallTrigger.PassBall())
                    {
                        PassTime = 0;
                        DelActionFlag(ActionFlag.IsPass);
                    }
                }       
                break;
//          case "PassEnd":
//              PassTime = 0;
//              DelActionFlag(ActionFlag.IsPass);
//              DelActionFlag(ActionFlag.IsDribble);
//              break;
            case "DunkJump":
                
                DelActionFlag(ActionFlag.IsDribble);
                DelActionFlag(ActionFlag.IsRun);
                
                SceneMgr.Get.SetBallState(PlayerState.Dunk);
//              playerCollider.enabled = false;
                if (OnDunkJump != null)
                    OnDunkJump(this);

                break;
            case "DunkBasket":
                DelActionFlag(ActionFlag.IsDribble);
                DelActionFlag(ActionFlag.IsRun);
//                PlayerRigidbody.useGravity = false;
//                PlayerRigidbody.isKinematic = true;
                SceneMgr.Get.PlayDunk(Team.GetHashCode());
                break;
            case "DunkFallBall":
                if (OnDunkBasket != null)
                    OnDunkBasket(this);

                break;
            case "DunkFall":
                playerCollider.enabled = true;
//                PlayerRigidbody.useGravity = true;
//                PlayerRigidbody.isKinematic = false;
                break;
            case "DunkEnd":
                if (!NeedResetFlag)
                {
                    DelActionFlag(ActionFlag.IsDunk);
                    DelActionFlag(ActionFlag.IsShootIdle);
                    isCheckLayerToReset = true;
                }
                break;
            case "FakeShootStop":
                DelActionFlag(ActionFlag.IsShoot);
                AddActionFlag(ActionFlag.IsShootIdle);
                DelActionFlag(ActionFlag.IsDribble);
                DelActionFlag(ActionFlag.IsRun);
                DelActionFlag(ActionFlag.IsFakeShoot);
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
            if (!CheckAction(ActionFlag.IsSteal) && 
                !CheckAction(ActionFlag.IsDunk) && 
                !CheckAction(ActionFlag.IsBlock) && 
                !CheckAction(ActionFlag.IsPass) && 
                !CheckAction(ActionFlag.IsShoot) &&
                !CheckAction(ActionFlag.IsShootIdle) &&
                !CheckAction(ActionFlag.IsCatcher))
                return true;
            else
                return false;
        }
    }

    public void ClearIsCatcher()
    {
        DelActionFlag(ActionFlag.IsCatcher);
    }

    public bool IsCatcher
    {
        get{ return CheckAction(ActionFlag.IsCatcher);}
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
        get{ return gameObject.transform.localPosition.y > 1f;}
    }

    public bool IsBallOwner
    {
        get { return SceneMgr.Get.RealBall.transform.parent == DummyBall.transform;}
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
            if (FirstMoveQueue.Count == 0)
                FirstMoveQueue.Enqueue(value);
        }
    }

    private bool isTouchPalyer = false;

    void OnCollisionStay(Collision collisionInfo) {
        if (!isTouchPalyer && collisionInfo.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isTouchPalyer = true;
        }
    }

    void OnCollisionExit(Collision collisionInfo) {
        if (isTouchPalyer && collisionInfo.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isTouchPalyer = false;
        }
    }
}