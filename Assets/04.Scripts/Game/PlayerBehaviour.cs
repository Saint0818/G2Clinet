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

public enum BodyType{
	Big = 0,
	Mid = 1,
	Small = 2
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
	public const int Action_Def = 1;
	public const int Action_Move = 2;
}

public class PlayerBehaviour : MonoBehaviour
{
	private const float MoveCheckValue = 1.5f;
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
	private bool canJump = true;
	private bool canResetJump = false;
	public BodyType Body;
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
		Control.SetFloat ("CrtHight", gameObject.transform.localPosition.y);

		if (canResetJump && Control.GetBool ("IsJump") && gameObject.transform.localPosition.y < 0.1f) {
			canJump = true;
			Control.SetBool ("IsJump", false);
			canResetJump = false;
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
		if (CheckCanUseControl()) {
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

	public void MoveTo(float X, float Z){
		if ((gameObject.transform.localPosition.x <= X + MoveCheckValue && gameObject.transform.localPosition.x >= X - MoveCheckValue) && 
		    (gameObject.transform.localPosition.z <= Z + MoveCheckValue && gameObject.transform.localPosition.z >= Z - MoveCheckValue)) {
			SetSpeed(0);
			DelActionFlag(ActionFlag.Action_Move);
			MoveTurn = 0;
			AniState(PlayerState.Idle);
			TargetPos = Vector2.zero;
			WaitMoveTime = (float)UnityEngine.Random.Range(0, 3);
			gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(new Vector3 (X, gameObject.transform.localPosition.y, Z) - gameObject.transform.localPosition), 30 * Time.deltaTime);
		}else if(!CheckAction(ActionFlag.Action_Def) && MoveTurn >= 0 && MoveTurn <= 5){
			AddActionFlag(ActionFlag.Action_Move);
			MoveTurn++;
			gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(new Vector3 (X, gameObject.transform.localPosition.y, Z) - gameObject.transform.localPosition), 10 * Time.deltaTime);
		}else{
			SetSpeed(1);
			if(CheckAction(ActionFlag.Action_Def))
				AniState(PlayerState.RunAndDefence);
			else
				AniState(PlayerState.Run);
			gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(new Vector3 (X, gameObject.transform.localPosition.y, Z) - gameObject.transform.localPosition), 10 * Time.deltaTime);
			gameObject.transform.localPosition = Vector3.Lerp (gameObject.transform.localPosition, new Vector3 (X, gameObject.transform.localPosition.y, Z), 0.045f);
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

		switch (state) {
			case PlayerState.Idle:
				CloseAllState();
			break;
			case PlayerState.Walk:
				
			break;
			case PlayerState.Run:
				Control.SetBool("IsRun", true);
			break;
			case PlayerState.Defence:
				Control.SetBool("IsDefence", true);
				break;
			case PlayerState.RunAndDefence:
				Control.SetBool("IsRun", true);
				Control.SetBool("IsDefence", true);
				break;
			case PlayerState.Jumper:
				Jump();
				break;
			case PlayerState.Dribble:
				Control.SetBool("IsDrible", true);
				break;
		}
	}

	public void SetDef(){
		SetSpeed(0);
		AniState(PlayerState.Defence);
		AddActionFlag (ActionFlag.Action_Def);
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

	private void Jump()
	{
		Control.SetBool("IsJump", true);
		if (canJump)
		{
			gameObject.rigidbody.AddForce (jumpHight * transform.up + gameObject.rigidbody.velocity.normalized /2.5f, ForceMode.VelocityChange);
			canJump = false;
			StartCoroutine ("JumpCoolDown", 1f);
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
		if (canJump)
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
		for(int i = 0; i < PlayerActionFlag.Length; i++)
			PlayerActionFlag[i] = 0;

		SetSpeed(0);
		AniState(PlayerState.Idle);
	}

	public bool Move{
		get{return CheckAction(ActionFlag.Action_Move);}
	}
}