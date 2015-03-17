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
	RunAndDrible = 24,
	Shooting = 25,
	Catcher = 26,
	DunkBasket = 27,
	RunningDefence = 28
}

public enum TeamKind{
	Self = 0,
	Npc = 1
}

public enum MoveType{
	PingPong = 0,
	Cycle = 1,
	Random = 2,
	Once = 3,
	Idle = 4
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
	public static string[] AnimatorStates = new string[]{"", "IsRun", "IsDefence","IsBlock", "", "IsDribble", "IsSteal", "IsPass", "IsShoot", "IsCatcher", "IsDunk", "IsShootIdle"};

	private Queue<TMoveData> MoveQueue = new Queue<TMoveData>();
	private Queue<TMoveData> FirstMoveQueue = new Queue<TMoveData>();
	private float canDunkDis = 30f;
	private byte[] PlayerActionFlag = {0, 0, 0, 0, 0, 0, 0};
	private float MoveMinSpeed = 0.5f;
	private float dashSpeed = 0.8f;
	private Vector2 drag = Vector2.zero;
	private bool stop = false;
	private bool JoystickEnd = false;
	private int MoveTurn = 0;
	private float PassTime = 0;
	private float NoAiTime = 0;
	private float MoveStartTime = 0;
	private float ProactiveRate = 0;
	private float ProactiveTime = 0;

    public Animator Control;
	public GameObject DummyBall;

	public TeamKind Team;
	public GameSituation situation = GameSituation.None;
	public PlayerState crtState = PlayerState.Idle;
	public MoveType MoveKind = MoveType.PingPong;
	public GamePostion Postion = GamePostion.G;
	public Transform [] DefPointAy = new Transform[8];
	public Vector2 [] RunPosAy;
	public float BasicMoveSpeed = 1f;
	public float WaitMoveTime = 0;
	public float Invincible = 0;
	public float JumpHight = 550f;
	public float CoolDownSteal = 0;
	public float AirDrag = 0f;
	public float fracJourney = 0;
	public int MoveIndex = -1;
	public bool isJoystick = false;
	public float DefSpeedup = 10;
	public float DefSpeedNormal = 7;
	public float BallOwnerSpeedup = 6;
	public float BallOwnerSpeedNormal = 8;
	public float AttackSpeedup = 10;
	public float AttackSpeedNormal = 8;

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
		Control = gameObject.GetComponent<Animator>();
		DummyBall = gameObject.transform.FindChild ("DummyBall").gameObject;
		
		initTrigger();
	}

	void FixedUpdate()
	{
//		CalculationAirResistance();
		Control.SetFloat ("CrtHight", gameObject.transform.localPosition.y);
		if (gameObject.transform.localPosition.y > 0.2f) {
			gameObject.GetComponent<Collider>().enabled = false;
		} else if(gameObject.GetComponent<Collider>().enabled == false)
			gameObject.GetComponent<Collider>().enabled = true;
		
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

		if (IsMove) {
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

	public void SetNoAiTime(){
		isJoystick = true;
		JoystickEnd = false;
		NoAiTime = Time.time + ChangeToAI;
	}
	
	private void CalculationAirResistance()
	{
		if (gameObject.transform.localPosition.y > 1f) {
			drag = Vector2.Lerp (Vector2.zero, new Vector2 (0, gameObject.transform.localPosition.y), 0.01f); 
			gameObject.GetComponent<Rigidbody>().drag = drag.y;
		} else {
			drag = Vector2.Lerp (new Vector2 (0, gameObject.transform.localPosition.y),Vector2.zero, 0.01f); 
			if(drag.y >= 0)
				gameObject.GetComponent<Rigidbody>().drag = drag.y;
			else
				gameObject.GetComponent<Rigidbody>().drag = 0;
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
				JoystickEnd = false;
				NoAiTime = Time.time + ChangeToAI;
				EffectManager.Get.SelectEffectScript.SetParticleColor(false);
				float AnimationSpeed = Vector2.Distance (new Vector2 (move.joystickAxis.x, 0), new Vector2 (0, move.joystickAxis.y));
				SetSpeed (AnimationSpeed);
				AniState(ps);

				float angle = move.Axis2Angle (true);
				int a = 90;
				Vector3 rotation = new Vector3 (0, angle + a, 0);
				transform.rotation = Quaternion.Euler (rotation);
				
				if(AnimationSpeed <= MoveMinSpeed){
					if(IsBallOwner)
						Translate = Vector3.forward * Time.deltaTime * MoveMinSpeed * 10 * BasicMoveSpeed * BallOwnerSpeedNormal;
					else{
						if(IsDefence){
							Translate = Vector3.forward * Time.deltaTime * MoveMinSpeed * 10 * BasicMoveSpeed * DefSpeedNormal;
						}else
							Translate = Vector3.forward * Time.deltaTime * MoveMinSpeed * 10 * BasicMoveSpeed * AttackSpeedNormal;
					}						
				}else{
					if(IsBallOwner)
						Translate = Vector3.forward * Time.deltaTime * AnimationSpeed * 10 * BasicMoveSpeed * BallOwnerSpeedup;
					else{
						if(IsDefence)
							Translate = Vector3.forward * Time.deltaTime * AnimationSpeed * 10 * BasicMoveSpeed * DefSpeedup;
						else
							Translate = Vector3.forward * Time.deltaTime * AnimationSpeed * 10 * BasicMoveSpeed * AttackSpeedup;
					}
				}
				
				transform.Translate (Translate);	
			}
		}
	}

	public void OnJoystickMoveEnd(MovingJoystick move, PlayerState ps)
	{
		JoystickEnd = true;
		AniState(ps);
	}
	
	private void DoDunkJump()
	{
		float dis = Vector3.Distance (gameObject.transform.position, SceneMgr.Get.ShootPoint [Team.GetHashCode ()].transform.position);
		if (dis < 10)
			gameObject.GetComponent<Rigidbody>().velocity = GameFunction.GetVelocity (gameObject.transform.position, SceneMgr.Get.DunkJumpPoint [Team.GetHashCode ()].transform.position, 70);
		else {
			gameObject.transform.LookAt(new Vector3(SceneMgr.Get.ShootPoint[Team.GetHashCode()].transform.position.x, 0, SceneMgr.Get.ShootPoint[Team.GetHashCode()].transform.position.z));
			gameObject.GetComponent<Rigidbody>().velocity = GameFunction.GetVelocity (gameObject.transform.position, SceneMgr.Get.DunkJumpPoint [Team.GetHashCode ()].transform.position, 60);
		}
    }

	public void OnDunkInto()
	{
		if (CheckAction (ActionFlag.IsDunk))
			if (!Control.GetBool ("IsDunkInto")) {
				SceneMgr.Get.PlayDunk(Team.GetHashCode());
				gameObject.GetComponent<Rigidbody>().useGravity = false;
				gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
				gameObject.GetComponent<Rigidbody>().isKinematic = true;
				gameObject.transform.position = SceneMgr.Get.DunkPoint[Team.GetHashCode()].transform.position;
				if(IsBallOwner)
					SceneMgr.Get.RealBall.transform.position = SceneMgr.Get.ShootPoint[Team.GetHashCode()].transform.position;
				Control.SetBool("IsDunkInto", true); 
			}
	}
	
	public void DelPass(){
		DelActionFlag (ActionFlag.IsPass);
    }

	public void MoveTo(TMoveData Data, bool First = false){
		if (CanMove) {
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
		
					if(ParameterConst.AIlevelAy[AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner || dis <= 7)
						MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.FrontSteal.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.FrontSteal.GetHashCode()].position.z);
				}else if(dis2 <= dis1 && dis2 <= dis3 && dis2 <= dis4){
					MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.Back.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.Back.GetHashCode()].position.z);

					if(ParameterConst.AIlevelAy[AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner || dis <= 7)
						MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.BackSteal.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.BackSteal.GetHashCode()].position.z);
				}else if(dis3 <= dis1 && dis3 <= dis2 && dis3 <= dis4){
					MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.Right.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.Right.GetHashCode()].position.z);

					if(ParameterConst.AIlevelAy[AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner || dis <= 7)
						MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.RightSteal.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.RightSteal.GetHashCode()].position.z);
				}else if(dis4 <= dis1 && dis4 <= dis2 && dis4 <= dis3){
					MoveTarget = new Vector2(Data.DefPlayer.DefPointAy[DefPoint.Left.GetHashCode()].position.x, Data.DefPlayer.DefPointAy[DefPoint.Left.GetHashCode()].position.z);

					if(ParameterConst.AIlevelAy[AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner || dis <= 7)
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
						transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * DefSpeedup * BasicMoveSpeed);
					else
						transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * DefSpeedNormal * BasicMoveSpeed);
				}else{
					rotateTo(MoveTarget.x, MoveTarget.y, 10);

					if(IsBallOwner)
						AniState(PlayerState.RunAndDrible);
					else
						AniState(PlayerState.Run);

					if(IsBallOwner){
						if(Data.Speedup)
							transform.Translate (Vector3.forward * Time.deltaTime * MoveMinSpeed * BallOwnerSpeedup * BasicMoveSpeed);
						else
							transform.Translate (Vector3.forward * Time.deltaTime * MoveMinSpeed * BallOwnerSpeedNormal * BasicMoveSpeed);
					}else{
						if(Data.Speedup)
							transform.Translate (Vector3.forward * Time.deltaTime * MoveMinSpeed * AttackSpeedup * BasicMoveSpeed);
						else
							transform.Translate (Vector3.forward * Time.deltaTime * MoveMinSpeed * AttackSpeedNormal * BasicMoveSpeed);
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
    
    private void SetSpeed(float value)
	{
		Control.SetFloat("MoveSpeed", value);
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

		SetSpeed(0);
		AniState(PlayerState.Idle);
		MoveQueue.Clear ();
		FirstMoveQueue.Clear ();
		NoAiTime = 0;
		isJoystick = false;
	}

	public void ClearMoveQueue(){
		MoveQueue.Clear ();
		FirstMoveQueue.Clear ();
	}

	public void AniState(PlayerState state, bool DorotateTo = false, float lookAtX = -1, float lookAtZ = -1)
	{
		crtState = state;
		
		if (DorotateTo)
			rotateTo(lookAtX, lookAtZ);
		
		switch (state) {
			case PlayerState.Idle:
				SetSpeed(0);
				for (int i = 1; i < AnimatorStates.Length; i++)
					if(AnimatorStates[i] != string.Empty && Control.GetBool(AnimatorStates[i]))
						Control.SetBool(AnimatorStates[i], false);
				break;
			case PlayerState.Catcher:
				SetSpeed(0);
				for (int i = 1; i < AnimatorStates.Length; i++)
					if(AnimatorStates[i] != string.Empty)
						Control.SetBool(AnimatorStates[i], false);
				AddActionFlag(ActionFlag.IsCatcher);
				break;
			case PlayerState.Run:
				Control.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				Control.SetBool(AnimatorStates[ActionFlag.IsDefence], false);
				break;
			case PlayerState.Dribble:
				SetSpeed(0);
				AddActionFlag(ActionFlag.IsDribble);
				Control.SetBool(AnimatorStates[ActionFlag.IsDribble], true);
				break;
			case PlayerState.RunAndDrible:
				SetSpeed(1);
				Control.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				Control.SetBool(AnimatorStates[ActionFlag.IsDribble], true);
				break;
			case PlayerState.RunningDefence:
				SetSpeed(1);
				Control.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				Control.SetBool(AnimatorStates[ActionFlag.IsDefence], false);
				break;
			case PlayerState.Defence:
				Control.SetBool(AnimatorStates[ActionFlag.IsDefence], true);
				Control.SetBool(AnimatorStates[ActionFlag.IsRun], false);
				SetSpeed(0);
				AddActionFlag (ActionFlag.IsDefence);
				break;
			case PlayerState.MovingDefence:
				SetSpeed(1);
				Control.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				Control.SetBool(AnimatorStates[ActionFlag.IsDefence], true);
				break;
			case PlayerState.Steal:
				if(!CheckAction(ActionFlag.IsSteal)){
					Control.SetBool(AnimatorStates[ActionFlag.IsSteal], true);
					AddActionFlag(ActionFlag.IsSteal);
				}
				break;
			case PlayerState.Pass:
				if(!CheckAction(ActionFlag.IsPass)){
					PassTime = Time.time + 3;
					AddActionFlag(ActionFlag.IsPass);
					Control.SetBool(AnimatorStates[ActionFlag.IsPass], true);
				}
				break;
			case PlayerState.Block:
				if (!CheckAction(ActionFlag.IsBlock)){
	                    AddActionFlag(ActionFlag.IsBlock);
	                    Control.SetBool(AnimatorStates[ActionFlag.IsBlock], true);
	                    
	                    if (OnBlock != null)
	                        OnBlock(this);
	                }
	                
	                break;
			case PlayerState.Shooting:
				if(!CheckAction(ActionFlag.IsShoot) && IsBallOwner)
		        {
		            AddActionFlag(ActionFlag.IsShoot);
					DelActionFlag(ActionFlag.IsShootIdle);
					Control.SetBool(AnimatorStates[ActionFlag.IsShootIdle], false);
		            Control.SetBool(AnimatorStates[ActionFlag.IsShoot], true);
		        }
		        break;
			case PlayerState.Dunk:
				if(!CheckAction(ActionFlag.IsDunk) && IsBallOwner && 
		   			Vector3.Distance (SceneMgr.Get.ShootPoint [Team.GetHashCode ()].transform.position, gameObject.transform.position) < canDunkDis)
				{
					AddActionFlag(ActionFlag.IsDunk);
					Control.SetBool(AnimatorStates[ActionFlag.IsDunk], true);
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
				Control.SetBool(AnimatorStates[ActionFlag.IsSteal], false);
				break;
			case "ShootDown":
				DelActionFlag(ActionFlag.IsShoot);
				DelActionFlag(ActionFlag.IsDribble);
				DelActionFlag(ActionFlag.IsRun);
				Control.SetBool (AnimatorStates[ActionFlag.IsShoot], false);
				Control.SetBool (AnimatorStates[ActionFlag.IsDribble], false);
				Control.SetBool (AnimatorStates[ActionFlag.IsRun], false);
				break;
			case "Blocking":
				SceneMgr.Get.SetBallState(PlayerState.Block);
				break;
			case "BlockEnd":
				Control.SetBool(AnimatorStates[ActionFlag.IsBlock], false);
				DelActionFlag(ActionFlag.IsBlock);
				break;
			case "Shooting":
				if (OnShoot != null)
					OnShoot(this);
				
				break;
			case "ShootJump":
				gameObject.GetComponent<Rigidbody>().AddForce (JumpHight * transform.up + gameObject.GetComponent<Rigidbody>().velocity.normalized /2.5f, ForceMode.Force);
				break;
			case "Passing":			
				if (PassTime > 0) {
					PassTime = 0;
					if(!SceneMgr.Get.RealBallTrigger.PassBall())
						DelActionFlag(ActionFlag.IsPass);
				}		

				break;
			case "PassEnd":
				Control.SetBool (AnimatorStates[ActionFlag.IsDribble], false);
				Control.SetBool (AnimatorStates[ActionFlag.IsPass], false);
				DelActionFlag(ActionFlag.IsPass);
				DelActionFlag(ActionFlag.IsDribble);
				break;
			case "DunkJump":
				SceneMgr.Get.SetBallState(PlayerState.Dunk);
				gameObject.GetComponent<Rigidbody>().GetComponent<Collider>().enabled = false;
				if(OnDunkJump != null)
					OnDunkJump(this);
				DoDunkJump();
				break;
			case "DunkBasket":
				if(OnDunkBasket != null)
					OnDunkBasket(this);
				break;
			case "DunkFall":
				gameObject.GetComponent<Rigidbody>().useGravity = true;
				gameObject.GetComponent<Rigidbody>().isKinematic = false;
				break;
			case "DunkEnd":
				Control.SetBool (AnimatorStates[ActionFlag.IsDunk], false);
				Control.SetBool ("IsDunkInto", false);
				DelActionFlag(ActionFlag.IsDunk);
				break;
			case "FakeShootStop":
				Control.SetBool (AnimatorStates[ActionFlag.IsShootIdle], true);
				AddActionFlag(ActionFlag.IsShootIdle);
				DelActionFlag(ActionFlag.IsShoot);
				DelActionFlag(ActionFlag.IsDribble);
				DelActionFlag(ActionFlag.IsRun);
				Control.SetBool (AnimatorStates[ActionFlag.IsShoot], false);
				Control.SetBool (AnimatorStates[ActionFlag.IsDribble], false);
				Control.SetBool (AnimatorStates[ActionFlag.IsRun], false);
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