using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public delegate bool OnPlayerAction(PlayerBehaviour player);
public delegate bool OnPlayerAction2(PlayerBehaviour player, bool speedup);
public enum PlayerState
{
    Idle = 0,
    Run = 2,            
    Block = 3,  
    Board = 4,  
    Catch = 5,  
    Defence = 6,    
    Dribble = 7,    
    Dunk = 8,   
    Fall = 9,   
    Hookshot = 10,   
    Layup = 12, 
    Pass = 13,  
    Steal = 14, 
    Underdunk = 15, 
    Pass2 = 20,
	AlleyOop_Pass = 21,
	AlleyOop_Dunk = 22,
	MovingDefence = 23,
	RunAndDribble = 24,
	Shooting = 25,
	Catcher = 26,
	DunkBasket = 27,
	RunningDefence = 28,
	FakeShoot = 29
}

public enum TeamKind{
	Self = 0,
	Npc = 1
}

public enum DefPoint{
	Front = 0,
	Back = 1,
	Right = 2,
	Left = 3,
	FrontSteal = 4,
	BackSteal = 5,
	RightSteal = 6,
	LeftSteal = 7
}

public static class ActionFlag{
	public const int IsRun = 1;
	public const int IsDefence = 2;
	public const int IsBlock = 3;
	public const int IsDribble = 5;
	public const int IsSteal = 6;
	public const int IsPass = 7;
	public const int IsShoot = 8;
	public const int IsCatcher = 9;
	public const int IsDunk = 10;
	public const int IsShootIdle = 11;
	public const int IsFakeShoot = 12;
}

public struct TMoveData
{
	public Vector2 Target;
	public Transform LookTarget;
	public Transform FollowTarget;
	public PlayerBehaviour DefPlayer;
	public OnPlayerAction2 MoveFinish;
	public bool Speedup;

	public TMoveData(int flag){
		Target = Vector2.zero;
		LookTarget = null;
		MoveFinish = null;
		FollowTarget = null;
		DefPlayer = null;
		Speedup = false;
	}
}

public class PlayerBehaviour : MonoBehaviour
{
	public OnPlayerAction OnShoot = null;
	public OnPlayerAction OnPass = null;
	public OnPlayerAction OnBlock = null;
	public OnPlayerAction OnDunkBasket = null;
	public OnPlayerAction OnDunkJump = null;
	
    public Vector3 Translate;
	private const float MoveCheckValue = 0.5f;
	private const int ChangeToAI = 4;
	public static string[] AnimatorStates = new string[]{"", "IsRun", "IsDefence","IsBlock", "", "IsDribble", "IsSteal", "IsPass", "IsShoot", "IsCatcher", "IsDunk", "IsShootIdle", "IsFakeShoot"};

	private Queue<TMoveData> MoveQueue = new Queue<TMoveData>();
	private Queue<TMoveData> FirstMoveQueue = new Queue<TMoveData>();
	private float canDunkDis = 30f;
	private byte[] PlayerActionFlag = {0, 0, 0, 0, 0, 0, 0};
	private float MoveMinSpeed = 0.5f;
	private Vector2 drag = Vector2.zero;
	private bool stop = false;
	private int MoveTurn = 0;
	private float PassTime = 0;
	private float NoAiTime = 0;
	private float MoveStartTime = 0;
	private float ProactiveRate = 0;
	private float ProactiveTime = 0;
	private int smoothDirection = 0;
	private float animationSpeed = 0;

	private Collider collider;
	private Rigidbody rigidbody;
	private Animator animator;
	private GameObject selectTexture;
	public GameObject DummyBall;

	public TeamKind Team;
	public GameSituation situation = GameSituation.None;
	public PlayerState crtState = PlayerState.Idle;
	public GamePostion Postion = GamePostion.G;
	public Transform [] DefPointAy = new Transform[8];

	public float WaitMoveTime = 0;
	public float Invincible = 0;
	public float JumpHight = 450f;
	public float CoolDownSteal = 0;
	public float AirDrag = 0f;
	public float fracJourney = 0;
	public int MoveIndex = -1;
	public bool isJoystick = false;
	public int AILevel = 1;
	public float CloseDef = 0;
	public PlayerBehaviour DefPlaeyr = null;
	public bool AutoFollow = false;

	void initTrigger() {
		GameObject obj = Resources.Load("Prefab/Player/BodyTrigger") as GameObject;
		if (obj) {
			GameObject obj2 = Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
			obj2.name = "BodyTrigger";
			PlayerTrigger[] objs = obj2.GetComponentsInChildren<PlayerTrigger>();
			if (objs != null) {
				for (int i = 0; i < objs.Length; i ++)
					objs[i].Player = this;
			}
			
			obj2.transform.parent = transform;
			obj2.transform.transform.localPosition = Vector3.zero;
			obj2.transform.transform.localScale = Vector3.one;
		}
	}

	void Awake()
	{
		rigidbody = gameObject.GetComponent<Rigidbody>();
		collider = gameObject.GetComponent<Collider>();
		animator = gameObject.GetComponent<Animator>();
		DummyBall = gameObject.transform.FindChild ("DummyBall").gameObject;
		
		initTrigger();
	}

	void FixedUpdate()
	{
		CalculationSmoothSpeed ();
//		CalculationAirResistance();
		animator.SetFloat ("CrtHight", gameObject.transform.localPosition.y);
		if (gameObject.transform.localPosition.y > 0.2f)
			collider.enabled = false;
		else 
		if (collider.enabled == false)
			collider.enabled = true;
		
		if(WaitMoveTime > 0 && Time.time >= WaitMoveTime)
			WaitMoveTime = 0;

		if(Invincible > 0 && Time.time >= Invincible)
			Invincible = 0;

		if(CoolDownSteal > 0 && Time.time >= CoolDownSteal)
			CoolDownSteal = 0;

		if (PassTime > 0 && Time.time >= PassTime) {
			PassTime = 0;
			DelActionFlag(ActionFlag.IsPass);
		}	

		if(FirstMoveQueue.Count > 0)
			MoveTo(FirstMoveQueue.Peek(), true);
		else if(MoveQueue.Count > 0)
			MoveTo(MoveQueue.Peek());

		if (isJoystick) {
			if(Time.time >= NoAiTime){
				MoveQueue.Clear();
				NoAiTime = 0;
				isJoystick = false;
				DelActionFlag (ActionFlag.IsRun);
				EffectManager.Get.SelectEffectScript.SetParticleColor(true);
			}
		}

		if (IsMove && !IsDefence) {//&& situation != GameSituation.Opening !IsDefence
			if(Time.time >= MoveStartTime){
				MoveStartTime = Time.time + 0.5f;
				GameController.Get.DefMove(this);
			}		
		}

		if (IsDefence) {
			if(Time.time >= ProactiveTime){
				ProactiveTime = Time.time + 4;
				ProactiveRate = UnityEngine.Random.Range(0, 100) + 1;
			}

			if (AutoFollow) {
				Vector3	ShootPoint;
				if(Team == TeamKind.Self)
					ShootPoint = SceneMgr.Get.ShootPoint[1].transform.position;		
				else
					ShootPoint = SceneMgr.Get.ShootPoint[0].transform.position;	

				if(Vector3.Distance(ShootPoint, DefPlaeyr.transform.position) <= 12){
					AutoFollow = false;
				}					
			}

			if(CloseDef > 0 && Time.time >= CloseDef){
				AutoFollow = true;
				CloseDef = 0;
			}
		}
	}

	public void SetSelectTexture(string name) {
		if (selectTexture) {

		} else {
			GameObject obj = Resources.Load("Prefab/Player/" + name) as GameObject;
			if (obj) {
				selectTexture = Instantiate(obj) as GameObject;
				selectTexture.name = "Select";
				selectTexture.transform.parent = transform;
				selectTexture.transform.localPosition = new Vector3(0, 0.05f, 0);
			}
		}
	}

	public void SetNoAiTime(){
		isJoystick = true;
		NoAiTime = Time.time + ChangeToAI;
	}
	
	private void CalculationAirResistance()
	{
		if (gameObject.transform.localPosition.y > 1f) {
			drag = Vector2.Lerp (Vector2.zero, new Vector2 (0, gameObject.transform.localPosition.y), 0.01f); 
			rigidbody.drag = drag.y;
		} else {
			drag = Vector2.Lerp (new Vector2 (0, gameObject.transform.localPosition.y),Vector2.zero, 0.01f); 
			if(drag.y >= 0)
				rigidbody.drag = drag.y;
			else
				rigidbody.drag = 0;
		}
	}

	private void CalculationSmoothSpeed()
	{
		if (smoothDirection != 0) {
			if(smoothDirection == 1)
			{
				animationSpeed += 0.1f;
				if(animationSpeed >=  1)
				{
					animationSpeed = 1;
					smoothDirection = 0;
				}
			}
			else{
				animationSpeed -=  0.1f;
				if(animationSpeed <= 0)
				{
					animationSpeed = 0;
					smoothDirection = 0;
				}
			}
			animator.SetFloat("MoveSpeed", animationSpeed);
		}
	}

	public void OnJoystickMove(MovingJoystick move, PlayerState ps)
	{
		if (CanMove || stop) {
			if (Mathf.Abs (move.joystickAxis.y) > 0 || Mathf.Abs (move.joystickAxis.x) > 0)
			{
				if(!isJoystick)
					MoveStartTime = Time.time + 1;

				if(!IsMove)
					AddActionFlag(ActionFlag.IsRun);

				isJoystick = true;
				NoAiTime = Time.time + ChangeToAI;
				EffectManager.Get.SelectEffectScript.SetParticleColor(false);
				animationSpeed = Vector2.Distance (new Vector2 (move.joystickAxis.x, 0), new Vector2 (0, move.joystickAxis.y));
				SetSpeed (animationSpeed, 0);
				AniState(ps);

				float angle = move.Axis2Angle (true);
				int a = 90;
				Vector3 rotation = new Vector3 (0, angle + a, 0);
				transform.rotation = Quaternion.Euler (rotation);
				
				if(animationSpeed <= MoveMinSpeed){
					if(IsBallOwner)
						Translate = Vector3.forward * Time.deltaTime * GameStart.Get.BasicMoveSpeed * GameStart.Get.BallOwnerSpeedNormal;
					else{
						if(IsDefence){
							Translate = Vector3.forward * Time.deltaTime * GameStart.Get.BasicMoveSpeed * GameStart.Get.DefSpeedNormal;
						}else
							Translate = Vector3.forward * Time.deltaTime * GameStart.Get.BasicMoveSpeed * GameStart.Get.AttackSpeedNormal;
					}						
				}else{
					if(IsBallOwner)
						Translate = Vector3.forward * Time.deltaTime * GameStart.Get.BasicMoveSpeed * GameStart.Get.BallOwnerSpeedup;
					else{
						if(IsDefence)
							Translate = Vector3.forward * Time.deltaTime * GameStart.Get.BasicMoveSpeed * GameStart.Get.DefSpeedup;
						else
							Translate = Vector3.forward * Time.deltaTime * GameStart.Get.BasicMoveSpeed * GameStart.Get.AttackSpeedup;
					}
				}
				
				transform.Translate (Translate);	
			}
		}
	}

	public void OnJoystickMoveEnd(MovingJoystick move, PlayerState ps)
	{
		isJoystick = false;

		if(crtState != ps)
			AniState(ps);
	}
	
	private void DoDunkJump()
	{
		float dis = Vector3.Distance (gameObject.transform.position, SceneMgr.Get.ShootPoint [Team.GetHashCode ()].transform.position);
		if (dis < 10)
			rigidbody.velocity = GameFunction.GetVelocity (gameObject.transform.position, SceneMgr.Get.DunkJumpPoint [Team.GetHashCode ()].transform.position, 70);
		else {
			gameObject.transform.LookAt(new Vector3(SceneMgr.Get.ShootPoint[Team.GetHashCode()].transform.position.x, 0, SceneMgr.Get.ShootPoint[Team.GetHashCode()].transform.position.z));
			rigidbody.velocity = GameFunction.GetVelocity (gameObject.transform.position, SceneMgr.Get.DunkJumpPoint [Team.GetHashCode ()].transform.position, 60);
		}
    }

	public void OnDunkInto()
	{
		if (CheckAction (ActionFlag.IsDunk))
			if (!animator.GetBool ("IsDunkInto")) {
				SceneMgr.Get.PlayDunk(Team.GetHashCode());
				rigidbody.useGravity = false;
				rigidbody.velocity = Vector3.zero;
				rigidbody.isKinematic = true;
				gameObject.transform.position = SceneMgr.Get.DunkPoint[Team.GetHashCode()].transform.position;
				if(IsBallOwner)
					SceneMgr.Get.RealBall.transform.position = SceneMgr.Get.ShootPoint[Team.GetHashCode()].transform.position;
				animator.SetBool("IsDunkInto", true); 
			}
	}
	
	public void DelPass(){
		DelActionFlag (ActionFlag.IsPass);
    }

	public void MoveTo(TMoveData Data, bool First = false){
		if (CanMove && WaitMoveTime == 0) {
			Vector2 MoveTarget = Vector2.zero;
			float dis = 0;
			if(Data.DefPlayer != null){
				Vector3	ShootPoint = SceneMgr.Get.ShootPoint[Data.DefPlayer.Team.GetHashCode()].transform.position;				
				float dis1 = Vector3.Distance(Data.DefPlayer.DefPointAy[DefPoint.Front.GetHashCode()].position, ShootPoint);
				float dis2 = Vector3.Distance(Data.DefPlayer.DefPointAy[DefPoint.Back.GetHashCode()].position, ShootPoint);
				float dis3 = Vector3.Distance(Data.DefPlayer.DefPointAy[DefPoint.Right.GetHashCode()].position, ShootPoint);
				float dis4 = Vector3.Distance(Data.DefPlayer.DefPointAy[DefPoint.Left.GetHashCode()].position, ShootPoint);

				dis = Vector3.Distance(transform.position, ShootPoint);

				if(dis1 <= dis2 && dis1 <= dis3 && dis1 <= dis4){
					MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.Front.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.Front.GetHashCode()].position.z);					
		
					if(ParameterConst.AIlevelAy[AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner || dis <= 6)
						MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.FrontSteal.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.FrontSteal.GetHashCode()].position.z);
				}else if(dis2 <= dis1 && dis2 <= dis3 && dis2 <= dis4){
					MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.Back.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.Back.GetHashCode()].position.z);

					if(ParameterConst.AIlevelAy[AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner || dis <= 6)
						MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.BackSteal.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.BackSteal.GetHashCode()].position.z);
				}else if(dis3 <= dis1 && dis3 <= dis2 && dis3 <= dis4){
					MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.Right.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.Right.GetHashCode()].position.z);

					if(ParameterConst.AIlevelAy[AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner || dis <= 6)
						MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.RightSteal.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.RightSteal.GetHashCode()].position.z);
				}else if(dis4 <= dis1 && dis4 <= dis2 && dis4 <= dis3){
					MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.Left.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.Left.GetHashCode()].position.z);

					if(ParameterConst.AIlevelAy[AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner || dis <= 6)
						MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.LeftSteal.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.LeftSteal.GetHashCode()].position.z);
				}
			}else if(Data.FollowTarget != null){
				MoveTarget = new Vector2(Data.FollowTarget.position.x, Data.FollowTarget.position.z);
			}else
				MoveTarget = Data.Target;


			if ((gameObject.transform.localPosition.x <= MoveTarget.x + MoveCheckValue && gameObject.transform.localPosition.x >= MoveTarget.x - MoveCheckValue) && 
			    (gameObject.transform.localPosition.z <= MoveTarget.y + MoveCheckValue && gameObject.transform.localPosition.z >= MoveTarget.y - MoveCheckValue)) {
				MoveTurn = 0;
				DelActionFlag(ActionFlag.IsRun);

				if(!IsDefence){
					if(!IsBallOwner)
						AniState(PlayerState.Idle);

					if(First)
						WaitMoveTime = 0;
					else
						WaitMoveTime = Time.time + UnityEngine.Random.Range(0, 3);
					
					if(IsBallOwner){
						if(Team == TeamKind.Self)
							rotateTo(SceneMgr.Get.ShootPoint[0].transform.position.x, SceneMgr.Get.ShootPoint[0].transform.position.z);
						else
							rotateTo(SceneMgr.Get.ShootPoint[1].transform.position.x, SceneMgr.Get.ShootPoint[1].transform.position.z);
					}else{
						if(Data.LookTarget == null)
							rotateTo(MoveTarget.x, MoveTarget.y);
						else
							rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
					}							
				}else{
					WaitMoveTime = 0;
					if(Data.LookTarget == null)
						rotateTo(MoveTarget.x, MoveTarget.y);
					else
						rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);

					AniState(PlayerState.Defence);
				}

				if(Data.MoveFinish != null)
					Data.MoveFinish(this, Data.Speedup);

				if(First)
					FirstMoveQueue.Dequeue();
				else
					MoveQueue.Dequeue();
			}else if(!CheckAction(ActionFlag.IsDefence) && MoveTurn >= 0 && MoveTurn <= 5){
				AddActionFlag(ActionFlag.IsRun);
				MoveTurn++;
				rotateTo(MoveTarget.x, MoveTarget.y, 10);
				if(MoveTurn == 1)
					MoveStartTime = Time.time + 1;
			}else{
				if(IsDefence){
					if(Data.DefPlayer != null){
						dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint[Data.DefPlayer.Team.GetHashCode()].transform.position);
						float dis2 = Vector3.Distance(transform.position, Data.DefPlayer.transform.position);
						if(Data.LookTarget == null || (dis > 10 && dis2 >= 2))
							rotateTo(MoveTarget.x, MoveTarget.y);
						else
							rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
					}else
						rotateTo(MoveTarget.x, MoveTarget.y);

					if(Data.DefPlayer != null){
						dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint[Data.DefPlayer.Team.GetHashCode()].transform.position);

						if(dis <= 10){
							//Move
							AniState(PlayerState.MovingDefence);
						}else{
							//Run
							AniState(PlayerState.RunningDefence);
						}
					}else
						AniState(PlayerState.Run);

					if(Data.Speedup)
						transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * GameStart.Get.DefSpeedup * GameStart.Get.BasicMoveSpeed);
					else
						transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * GameStart.Get.DefSpeedNormal * GameStart.Get.BasicMoveSpeed);
				}else{
					rotateTo(MoveTarget.x, MoveTarget.y, 10);

					if(IsBallOwner)
						AniState(PlayerState.RunAndDribble);
					else
						AniState(PlayerState.Run);

					if(IsBallOwner){
						if(Data.Speedup)
							transform.Translate (Vector3.forward * Time.deltaTime * GameStart.Get.BallOwnerSpeedup * GameStart.Get.BasicMoveSpeed);
						else
							transform.Translate (Vector3.forward * Time.deltaTime * GameStart.Get.BallOwnerSpeedNormal * GameStart.Get.BasicMoveSpeed);
					}else{
						if(Data.Speedup)
							transform.Translate (Vector3.forward * Time.deltaTime  * GameStart.Get.AttackSpeedup * GameStart.Get.BasicMoveSpeed);
						else
							transform.Translate (Vector3.forward * Time.deltaTime  * GameStart.Get.AttackSpeedNormal * GameStart.Get.BasicMoveSpeed);
					}
				}
			}		
		}
	}

    public void rotateTo(float lookAtX, float lookAtZ, float time = 50){
        transform.rotation = Quaternion.Lerp(transform.rotation, 
                             Quaternion.LookRotation(new Vector3 (lookAtX, transform.localPosition.y, lookAtZ) - 
                                transform.localPosition), time * Time.deltaTime);
    }
    
    public void SetInvincible(float time){
        if(Invincible == 0)
            Invincible = Time.time + time;
        else
            Invincible += time;
    }
    
    private void SetSpeed(float value, int dir = -2)
	{
		//dir : 1 ++, -1 --, -2 : not smooth,  
		if(dir == 0)
			animator.SetFloat("MoveSpeed", value);
		else
		if(dir != -2)
			smoothDirection = dir;
	}

	private void AddActionFlag(int Flag){
		GameFunction.Add_ByteFlag (Flag, ref PlayerActionFlag);
	}

	private void DelActionFlag(int Flag){
		GameFunction.Del_ByteFlag (Flag, ref PlayerActionFlag);
	}

	private bool CheckAction(int Flag){
		return GameFunction.CheckByteFlag (Flag, PlayerActionFlag);
	}

	public void ResetFlag(){
		for(int i = 0; i < PlayerActionFlag.Length; i++)
			PlayerActionFlag[i] = 0;

		AniState(PlayerState.Idle);
		MoveQueue.Clear ();
		FirstMoveQueue.Clear ();
		NoAiTime = 0;
		WaitMoveTime = 0;
		isJoystick = false;
	}

	public void ClearMoveQueue(){
		MoveQueue.Clear ();
		FirstMoveQueue.Clear ();
	}

	private bool CanUseState(PlayerState state)
	{
		if (state != PlayerState.FakeShoot && crtState != state)
			return true;
		else if(state == PlayerState.FakeShoot)
			return true;

		return false;
	}

	public void AniState(PlayerState state, bool DorotateTo = false, float lookAtX = -1, float lookAtZ = -1)
	{
		if (!CanUseState(state))
			return;

		crtState = state;
		
		if (DorotateTo)
			rotateTo(lookAtX, lookAtZ);
		
		switch (state) {
			case PlayerState.Idle:
				SetSpeed(0, -1);
				for (int i = 1; i < AnimatorStates.Length; i++)
					if(AnimatorStates[i] != string.Empty && animator.GetBool(AnimatorStates[i]))
						animator.SetBool(AnimatorStates[i], false);
				break;
			case PlayerState.Catcher:
				SetSpeed(0, -1);
				for (int i = 1; i < AnimatorStates.Length; i++)
					if(AnimatorStates[i] != string.Empty)
						animator.SetBool(AnimatorStates[i], false);
				AddActionFlag(ActionFlag.IsCatcher);
				break;
			case PlayerState.Run:
				if(!isJoystick)
					SetSpeed(1, 1);	
				animator.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				animator.SetBool(AnimatorStates[ActionFlag.IsDefence], false);
				break;
			case PlayerState.Dribble:
					SetSpeed(0, -1);
				AddActionFlag(ActionFlag.IsDribble);
				animator.SetBool(AnimatorStates[ActionFlag.IsDribble], true);
				break;
			case PlayerState.RunAndDribble:
				if(!isJoystick)
					SetSpeed(1, 1);
				else
					SetSpeed(1, 0);

				animator.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				animator.SetBool(AnimatorStates[ActionFlag.IsDribble], true);
				break;
			case PlayerState.RunningDefence:
				SetSpeed(1, 1);
				animator.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				animator.SetBool(AnimatorStates[ActionFlag.IsDefence], false);
				break;
			case PlayerState.Defence:
				animator.SetBool(AnimatorStates[ActionFlag.IsDefence], true);
				animator.SetBool(AnimatorStates[ActionFlag.IsRun], false);
				DelActionFlag(ActionFlag.IsRun);
				SetSpeed(0, -1);
				AddActionFlag (ActionFlag.IsDefence);
				break;
			case PlayerState.MovingDefence:
				SetSpeed(1, 1);
				animator.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				animator.SetBool(AnimatorStates[ActionFlag.IsDefence], true);
				break;
			case PlayerState.Steal:
				if(!CheckAction(ActionFlag.IsSteal)){
					animator.SetBool(AnimatorStates[ActionFlag.IsSteal], true);
					AddActionFlag(ActionFlag.IsSteal);
				}
				break;
			case PlayerState.Pass:
				if(!CheckAction(ActionFlag.IsPass)){
					PassTime = Time.time + 3;
					AddActionFlag(ActionFlag.IsPass);
					animator.SetBool(AnimatorStates[ActionFlag.IsPass], true);
				}
				break;
			case PlayerState.Block:
				if (!CheckAction(ActionFlag.IsBlock)){
	                    AddActionFlag(ActionFlag.IsBlock);
	                    animator.SetBool(AnimatorStates[ActionFlag.IsBlock], true);
	                    
	                    if (OnBlock != null)
	                        OnBlock(this);
	                }  
	                break;
			case PlayerState.Shooting:
				if(!CheckAction(ActionFlag.IsShoot) && IsBallOwner)
		        {
		            AddActionFlag(ActionFlag.IsShoot);
					DelActionFlag(ActionFlag.IsShootIdle);
					DelActionFlag(ActionFlag.IsRun);
					DelActionFlag(ActionFlag.IsDribble);
					animator.SetBool(AnimatorStates[ActionFlag.IsShootIdle], false);
		            animator.SetBool(AnimatorStates[ActionFlag.IsShoot], true);
					animator.SetBool(AnimatorStates[ActionFlag.IsRun], false);
					animator.SetBool(AnimatorStates[ActionFlag.IsDribble], false);
		        }
		        break;
			case PlayerState.FakeShoot:
				if(!CheckAction(ActionFlag.IsShoot) && IsBallOwner)
				{
					AddActionFlag(ActionFlag.IsFakeShoot);
					AddActionFlag(ActionFlag.IsShoot);
					animator.SetBool(AnimatorStates[ActionFlag.IsShoot], true);
					animator.SetBool(AnimatorStates[ActionFlag.IsFakeShoot], true);
				}
				break;
			case PlayerState.Dunk:
				if(!CheckAction(ActionFlag.IsDunk) && IsBallOwner && 
		   			Vector3.Distance (SceneMgr.Get.ShootPoint [Team.GetHashCode ()].transform.position, gameObject.transform.position) < canDunkDis)
				{
					AddActionFlag(ActionFlag.IsDunk);
					animator.SetBool(AnimatorStates[ActionFlag.IsDunk], true);
				}
			break;
        }
    }
    
    public void AnimationEvent(string animationName)
    {
        switch (animationName) 
		{
			case "StealEnd":
				DelActionFlag(ActionFlag.IsSteal);
				animator.SetBool(AnimatorStates[ActionFlag.IsSteal], false);
				break;
			case "ShootDown":
				animator.SetBool (AnimatorStates[ActionFlag.IsShoot], false);
				animator.SetBool (AnimatorStates[ActionFlag.IsDribble], false);
				animator.SetBool (AnimatorStates[ActionFlag.IsRun], false);
				DelActionFlag(ActionFlag.IsShoot);
				DelActionFlag(ActionFlag.IsDribble);
				DelActionFlag(ActionFlag.IsRun);
				break;
			case "BlockJump":
				rigidbody.AddForce (JumpHight * transform.up + rigidbody.velocity.normalized /2.5f, ForceMode.Force);
				break;
			case "Blocking":
				SceneMgr.Get.SetBallState(PlayerState.Block);
				break;
			case "BlockEnd":
				animator.SetBool(AnimatorStates[ActionFlag.IsBlock], false);
				DelActionFlag(ActionFlag.IsBlock);
				break;
			case "Shooting":
				if (OnShoot != null)
					OnShoot(this);
				
				break;
			case "ShootJump":
				rigidbody.AddForce (JumpHight * transform.up + rigidbody.velocity.normalized /2.5f, ForceMode.Force);
				break;
			case "Passing":			
				if (PassTime > 0) {
					PassTime = 0;
					if(!SceneMgr.Get.RealBallTrigger.PassBall())
						DelActionFlag(ActionFlag.IsPass);
				}		

				break;
			case "PassEnd":
				animator.SetBool (AnimatorStates[ActionFlag.IsDribble], false);
				animator.SetBool (AnimatorStates[ActionFlag.IsPass], false);
				DelActionFlag(ActionFlag.IsPass);
				DelActionFlag(ActionFlag.IsDribble);
				break;
			case "DunkJump":
				SceneMgr.Get.SetBallState(PlayerState.Dunk);
				collider.enabled = false;
				if(OnDunkJump != null)
					OnDunkJump(this);

				DoDunkJump();
				break;
			case "DunkBasket":
				rigidbody.useGravity = false;
				rigidbody.isKinematic = true;
				if(OnDunkBasket != null)
					OnDunkBasket(this);

				break;
			case "DunkFall":
				rigidbody.useGravity = true;
				rigidbody.isKinematic = false;
				break;
			case "DunkEnd":
				animator.SetBool (AnimatorStates[ActionFlag.IsDunk], false);
				animator.SetBool ("IsDunkInto", false);
				DelActionFlag(ActionFlag.IsDunk);
				break;
			case "FakeShootStop":
				DelActionFlag(ActionFlag.IsShoot);
				animator.SetBool (AnimatorStates[ActionFlag.IsShoot], false);
				AddActionFlag(ActionFlag.IsShootIdle);
				animator.SetBool (AnimatorStates[ActionFlag.IsShootIdle], true);
				DelActionFlag(ActionFlag.IsDribble);
				animator.SetBool (AnimatorStates[ActionFlag.IsDribble], false);
				DelActionFlag(ActionFlag.IsRun);
				animator.SetBool (AnimatorStates[ActionFlag.IsRun], false);
				DelActionFlag(ActionFlag.IsFakeShoot);
				animator.SetBool (AnimatorStates[ActionFlag.IsFakeShoot], false);
				break;
		}
	}

	public void ResetMove(){
		MoveQueue.Clear ();
		DelActionFlag (ActionFlag.IsRun);
		WaitMoveTime = 0;
	}

	public void SetAutoFollowTime(){
		if (CloseDef == 0 && AutoFollow == false) {
			CloseDef = Time.time + ParameterConst.AIlevelAy[AILevel].AutoFollowTime;
		}			
	}

	public bool CanMove
	{
		get{
			if (!CheckAction (ActionFlag.IsSteal) && 
			    !CheckAction (ActionFlag.IsDunk) && 
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

	public void ClearIsCatcher(){
		DelActionFlag (ActionFlag.IsCatcher);
	}

	public bool IsCatcher{
		get{return CheckAction(ActionFlag.IsCatcher);}
	}

	public bool IsDefence{
		get{
			if(situation == GameSituation.AttackA && Team == TeamKind.Npc)
				return true;
			else if(situation == GameSituation.AttackB && Team == TeamKind.Self)
				return true;
			else
				return false;
		}
	}

	public bool IsDribble{
		get{return CheckAction(ActionFlag.IsDribble);}
	}

    public bool IsMove{
		get{return CheckAction(ActionFlag.IsRun);}
	}
	
	public bool IsShooting{
		get{return CheckAction(ActionFlag.IsShoot);}
	}
	
	public bool IsBlock{
		get{return CheckAction(ActionFlag.IsBlock);}
	}
	
	public bool IsPass{
		get{return CheckAction(ActionFlag.IsPass);}
	}
	
	public bool IsJump{
		get{return gameObject.transform.localPosition.y > 1f;}
    }
    
    public bool IsSteal{
        get{return CheckAction(ActionFlag.IsSteal);}
    }

	public bool IsBallOwner {
		get {return SceneMgr.Get.RealBall.transform.parent == DummyBall.transform;}
	}

	public int TargetPosNum {
		get {return MoveQueue.Count;}
	}

	public TMoveData TargetPos{
		set{
			if(MoveQueue.Count == 0)
				MoveTurn = 0;
			MoveQueue.Enqueue(value);
		}
	}

	public TMoveData FirstTargetPos{
		set{
			if(FirstMoveQueue.Count == 0)
				FirstMoveQueue.Enqueue(value);
		}
	}
}