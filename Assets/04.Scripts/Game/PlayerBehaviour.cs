using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState
{
    Idle = 0,
    Walk = 1,   
    Run = 2,            
    Block = 3,  
    Board = 4,  
    Catch = 5,  
    Defence = 6,    
    Dribble = 7,    
    Dunk = 8,   
    Fall = 9,   
    Hookshot = 10,  
    Jumper = 11,    
    Layup = 12, 
    Pass = 13,  
    Steal = 14, 
    Underdunk = 15, 
    Dunk1 = 17,
    ShootStart = 18,
    Shoot = 19,
    Pass2 = 20,
	AlleyOop_Pass = 21,
	AlleyOop_Dunk = 22,
	RunAndDefence = 23
}

public enum TeamKind{
	None = 0,
	Self = 1,
	Npc = 2
}

public enum RunDistanceType{
	Short = 0,
	Mid = 1,
	Long = 2
}

public enum MoveType{
	BackAndForth = 0,
	Cycle = 1,
	Random = 2
}

public static class PlayerActions
{
    public const string Idle = "StayIdle";
    public const string Walk = "Walk";
    public const string Run = "Run";
    public const string Block = "Block";
    public const string Board = "Board";
    public const string Catch = "Catch";
    public const string Defence = "Defence";
    public const string Dribble = "Dribble";
    public const string Dunk = "Dunk2";
    public const string Underdunk = "Underdunk";
    public const string Fall = "Fall";
    public const string Hookshot = "HookShot";
    public const string Jumper = "ShootStart";
    public const string Layup = "Layup";
    public const string Pass = "Pass";
    public const string Steal = "Steal";
    public const string Gangnamstyle = "gangnamstyle";
    public const string Stay = "Stay";
    public const string FakeJumper = "FakeJumper";
    public const string Shoot = "Shoot";
    public const string ShootStart = "ShootStart";
    public const string Pass2 = "Pass2";
}

public static class ActionFlag{
	public const int Action_IsRun = 1;
	public const int Action_IsDefence = 2;
	public const int Action_IsBlock = 3;
	public const int Action_IsJump = 4;
	public const int Action_IsDrible = 5;
	public const int Action_IsSteal = 6;
	public const int Action_IsPass = 7;
}

public class PlayerBehaviour : MonoBehaviour
{
	public static string[] AnimatorStates = new string[]{"", "IsRun", "IsDefence","IsBlock", "IsJump", "IsDrible", "IsSteal", "IsPass"};
	private bool canSteal = true;
	private bool canJump = true;
	private bool canResetJump = false;
	private bool stop = false;

	private const float MoveCheckValue = 1;
	private byte[] PlayerActionFlag = {0, 0, 0, 0, 0, 0, 0};
	private int MoveTurn = 0;
	private float Timer = 0;
	private Vector2 mTargetPos = Vector2.zero;
	public Animator Control;
	public PlayerState crtState = PlayerState.Idle;
	public float basicMoveSpeed = 10;
	public float curSpeed = 0;
	public TeamKind Team;
	public GameObject DummyBall;
	public float jumpHight = 8;
	public RunDistanceType RunArea;
	public int MoveIndex = -1;
	public float WaitMoveTime = 0;
	public MoveType MoveKind = MoveType.BackAndForth;
	public int Postion = 0;

	void Awake()
	{
		Control = gameObject.GetComponent<Animator>();
		DummyBall = gameObject.transform.FindChild ("DummyBall").gameObject;
	}

	void Update()
	{
		if (!canSteal && Control.GetCurrentAnimatorStateInfo (0).IsName ("Steal"))
		{
			Control.SetFloat("StealTime", Control.GetCurrentAnimatorStateInfo (0).normalizedTime);
			if(Control.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.8f) {
				Control.SetBool ("IsSteal", false);
				canSteal = true;
				stop = true;
				DelActionFlag(ActionFlag.Action_IsSteal);
			}
		}

		Control.SetFloat ("CrtHight", gameObject.transform.localPosition.y);

		if (canResetJump && Control.GetBool ("IsJump") && gameObject.transform.localPosition.y < 0.1f) {
			canJump = true;
			Control.SetBool ("IsJump", false);
			canResetJump = false;
			DelActionFlag(ActionFlag.Action_IsJump);
		}

		if (Time.time - Timer >= 1){
			Timer = Time.time;
			
			if(WaitMoveTime > 0){
				WaitMoveTime--;
			}
		}
	}

	public void OnJoystickMove(MovingJoystick move)
	{
		if (CheckCanUseControl() || stop) {
			curSpeed = Vector2.Distance (new Vector2 (move.joystickAxis.x, 0), new Vector2 (0, move.joystickAxis.y));
			SetSpeed (curSpeed);

			if (Mathf.Abs (move.joystickAxis.y) > 0)
					AniState (PlayerState.Run);

			float angle = move.Axis2Angle (true);
			int a = 90;
			Vector3 rotation = new Vector3 (0, angle + a, 0);
			transform.rotation = Quaternion.Euler (rotation);
			Vector3 translate = Vector3.forward * Time.deltaTime * curSpeed * basicMoveSpeed;
			transform.Translate (translate);	
		}
	}

	public void MoveTo(float X, float Z, float lookAtX, float loolAtZ){
		if (!CheckAction (ActionFlag.Action_IsSteal) && !CheckAction (ActionFlag.Action_IsJump)) {
			if ((gameObject.transform.localPosition.x <= X + MoveCheckValue && gameObject.transform.localPosition.x >= X - MoveCheckValue) && 
			    (gameObject.transform.localPosition.z <= Z + MoveCheckValue && gameObject.transform.localPosition.z >= Z - MoveCheckValue)) {
				SetSpeed(0);
				DelActionFlag(ActionFlag.Action_IsRun);
				MoveTurn = 0;
				AniState(PlayerState.Idle);
				TargetPos = Vector2.zero;
				if(!CheckAction(ActionFlag.Action_IsDefence)){
					WaitMoveTime = (float)UnityEngine.Random.Range(0, 3);
					if(Team == TeamKind.Self)
						rotateTo(SceneMgr.Inst.ShootPoint[0].transform.position.x, SceneMgr.Inst.ShootPoint[0].transform.position.z);
					else
						rotateTo(SceneMgr.Inst.ShootPoint[1].transform.position.x, SceneMgr.Inst.ShootPoint[1].transform.position.z);
				}else{
					WaitMoveTime = 0;
					rotateTo(lookAtX, loolAtZ);
				}
			}else if(!CheckAction(ActionFlag.Action_IsDefence) && MoveTurn >= 0 && MoveTurn <= 5){
				AddActionFlag(ActionFlag.Action_IsRun);
				MoveTurn++;
				rotateTo(X, Z, 10);
			}else{
				SetSpeed(1);
				if(CheckAction(ActionFlag.Action_IsDefence))
					AniState(PlayerState.RunAndDefence);
				else
					AniState(PlayerState.Run);
				rotateTo(X, Z, 10);
				gameObject.transform.localPosition = Vector3.Lerp (gameObject.transform.localPosition, new Vector3 (X, gameObject.transform.localPosition.y, Z), 0.045f);
			}		
		}
	}

	public void OnJoystickMoveEnd(MovingJoystick move)
	{
		SetSpeed(0);
		AniState(PlayerState.Idle);
	}

	public void AniState(PlayerState state)
	{
		CloseAllState();
		crtState = state;
		switch (state) {
			case PlayerState.Idle:
				CloseAllState();
			break;
			case PlayerState.Walk:
				
			break;
			case PlayerState.Run:
			Control.SetBool(AnimatorStates[ActionFlag.Action_IsRun], true);
			break;
			case PlayerState.Defence:
				Control.SetBool("IsDefence", true);
				break;
			case PlayerState.RunAndDefence:
			Control.SetBool(AnimatorStates[ActionFlag.Action_IsRun], true);
			Control.SetBool(AnimatorStates[ActionFlag.Action_IsDefence], true);
				break;
			case PlayerState.Jumper:
				Jump();
				break;
			case PlayerState.Dribble:
			Control.SetBool(AnimatorStates[ActionFlag.Action_IsDrible], true);
				break;
			case PlayerState.Steal:
				Steal(-1, -1);
				break;
			case PlayerState.Pass:
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsPass], true);
				break;
		}
	}

	public void SetDef(){
		SetSpeed(0);
		AniState(PlayerState.Defence);
		AddActionFlag (ActionFlag.Action_IsDefence);
	}

	public void SetSpeed(float value)
	{
		Control.SetFloat("Speed", value);
		Control.SetFloat("DribleMoveSpeed", value);
	}

	private void CloseAllState()
	{
		Control.SetBool("IsRun", false);
		Control.SetBool("IsDefence", false);
	}

	public void Jump()
	{
		Control.SetBool("IsJump", true);
		if (canJump)
		{
			gameObject.rigidbody.AddForce (jumpHight * transform.up + gameObject.rigidbody.velocity.normalized /2.5f, ForceMode.VelocityChange);
			canJump = false;
			AddActionFlag(ActionFlag.Action_IsJump);
			StartCoroutine ("JumpCoolDown", 1f);
		}
	}

	public void Steal(float lookAtX, float lookAtZ)
	{
		if (canSteal) 
		{
			stop = true;
			Control.SetBool("IsSteal", true);
			AddActionFlag(ActionFlag.Action_IsSteal);
			if(lookAtX != -1 && lookAtZ != -1)
				rotateTo(lookAtX, lookAtZ);
			canSteal = false;
		}
	}

	IEnumerator JumpCoolDown(float cdtime)
	{
		yield return new WaitForSeconds (cdtime);
		canResetJump = true;
		yield return new WaitForSeconds (0.5f);
		canJump = true;

	}

	private bool CheckCanUseControl()
	{
		if (canJump && canSteal)
			return true;
		else
			return false;
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

	public Vector2 TargetPos{
		set{
			mTargetPos = value;
			MoveTurn = 0;
		}
		get{
			return mTargetPos;
		}
	}

	public void ResetFlag(){
		for (int i = 1; i < AnimatorStates.Length; i++)
			if(AnimatorStates[i] != string.Empty)
				Control.SetBool(AnimatorStates[i], false);

		for(int i = 0; i < PlayerActionFlag.Length; i++)
			PlayerActionFlag[i] = 0;

		SetSpeed(0);
		AniState(PlayerState.Idle);
	}

	private void rotateTo(float lookAtX, float lookAtZ, float time = 30){
		gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(new Vector3 (lookAtX, gameObject.transform.localPosition.y, lookAtZ) - gameObject.transform.localPosition), time * Time.deltaTime);
	}

	public bool IsMove{
		get{return CheckAction(ActionFlag.Action_IsRun);}
	}

	public bool IsJump{
		get{return CheckAction(ActionFlag.Action_IsJump);}
	}
}