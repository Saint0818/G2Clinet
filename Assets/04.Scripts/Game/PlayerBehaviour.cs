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
	RunAndDefence = 23,
	RunAndDrible = 24,
	Shootting = 25
}

public enum TeamKind{
	Self = 0,
	Npc = 1
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
	public const int Action_IsDribble = 5;
	public const int Action_IsSteal = 6;
	public const int Action_IsPass = 7;
}

public class PlayerBehaviour : MonoBehaviour
{
	public static string[] AnimatorStates = new string[]{"", "IsRun", "IsDefence","IsBlock", "IsJump", "IsDribble", "IsSteal", "IsPass"};
	private bool canSteal = true;
//	private bool canJump = true;
	private bool canResetJump = false;
	private bool stop = false;

	private const float MoveCheckValue = 1;
	private byte[] PlayerActionFlag = {0, 0, 0, 0, 0, 0, 0};
	private int MoveTurn = 0;
	private float Timer = 0;
	private Vector2 mTargetPos = Vector2.zero;
	public Animator Control;
	public PlayerState crtState = PlayerState.Idle;
	public float basicMoveSpeed = 0.5f;
	public float curSpeed = 0;
	public TeamKind Team;
	public GameObject DummyBall;
	public float jumpHight = 8;
	public RunDistanceType RunArea;
	public int MoveIndex = -1;
	public float WaitMoveTime = 0;
	public float Invincible = 0;
	public MoveType MoveKind = MoveType.BackAndForth;
	public int Postion = 0;
	private float startMoveTime = 0;
	private float journeyLength = 0;

	void Awake()
	{
		Control = gameObject.GetComponent<Animator>();
		DummyBall = gameObject.transform.FindChild ("DummyBall").gameObject;
	}

	void Update()
	{
		Control.SetFloat ("CrtHight", gameObject.transform.localPosition.y);

		if (Time.time - Timer >= 1){
			Timer = Time.time;
			
			if(WaitMoveTime > 0){
				WaitMoveTime--;
			}
		}

		if(Time.time >= Invincible)
			Invincible = 0;
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
			Vector3 translate = Vector3.forward * Time.deltaTime * curSpeed * 10 * basicMoveSpeed;
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
				startMoveTime = 0;
				journeyLength = 0;
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
			}else if(!CheckAction(ActionFlag.Action_IsDefence) && UIGame.Get.Game.ballController != null && MoveTurn >= 0 && MoveTurn <= 5){
				AddActionFlag(ActionFlag.Action_IsRun);
				MoveTurn++;
				rotateTo(X, Z, 10);
			
				if(MoveTurn == 1){
					startMoveTime = Time.time;
					journeyLength = Vector3.Distance(gameObject.transform.localPosition, new Vector3 (X, gameObject.transform.localPosition.y, Z));
				}
			}else{
				float fracJourney = 0;
				SetSpeed(1);
				rotateTo(X, Z, 10);
				if(CheckAction(ActionFlag.Action_IsDefence)){
					AniState(PlayerState.RunAndDefence);
					fracJourney = 0.045f;
				}else{
					if(UIGame.Get.Game.ballController && UIGame.Get.Game.ballController.gameObject == gameObject)
						AniState(PlayerState.RunAndDrible);
					else
						AniState(PlayerState.Run);
					fracJourney = ((Time.time - startMoveTime) * basicMoveSpeed) / journeyLength;
				}

				gameObject.transform.localPosition = Vector3.Lerp (gameObject.transform.localPosition, new Vector3 (X, gameObject.transform.localPosition.y, Z), fracJourney);
			}		
		}
	}

	public void OnJoystickMoveEnd(MovingJoystick move)
	{
		SetSpeed(0);
		if (UIGame.Get.Game.ballController && UIGame.Get.Game.ballController.gameObject == gameObject)
			AniState(PlayerState.Dribble);
		else
			AniState (PlayerState.Idle);
	}

	public void AniState(PlayerState state, bool DorotateTo = false, float lookAtX = -1, float lookAtZ = -1)
	{
		crtState = state;
		for (int i = 1; i < AnimatorStates.Length; i++)
			if(AnimatorStates[i] != string.Empty)
				Control.SetBool(AnimatorStates[i], false);

		if(DorotateTo)
			rotateTo(lookAtX, lookAtZ);

		switch (state) {
			case PlayerState.Idle:
				break;
			case PlayerState.Walk:
				break;
			case PlayerState.Run:
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsRun], true);
				break;
			case PlayerState.Dribble:
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsDribble], true);
				break;
			case PlayerState.RunAndDrible:
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsRun], true);
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsDribble], true);
				break;
			case PlayerState.Defence:
				Control.SetBool("IsDefence", true);
				SetSpeed(0);
				AddActionFlag (ActionFlag.Action_IsDefence);
				break;
			case PlayerState.RunAndDefence:
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsRun], true);
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsDefence], true);
				break;
			case PlayerState.Jumper:
				if(!CheckAction(ActionFlag.Action_IsJump))
				{
					Control.SetBool(AnimatorStates[ActionFlag.Action_IsJump], true);
					gameObject.rigidbody.AddForce (jumpHight * transform.up + gameObject.rigidbody.velocity.normalized /2.5f, ForceMode.VelocityChange);
					AddActionFlag(ActionFlag.Action_IsJump);
				}
				break;
			case PlayerState.Steal:
				if(!CheckAction(ActionFlag.Action_IsSteal)){
					Control.SetBool(AnimatorStates[ActionFlag.Action_IsSteal], true);
					AddActionFlag(ActionFlag.Action_IsSteal);
					Debug.LogWarning(1111);
				}
				break;
			case PlayerState.Pass:
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsPass], true);
				break;
			case PlayerState.Block:
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsBlock], true);
				if(!CheckAction(ActionFlag.Action_IsBlock))
				{
					if(DorotateTo)
						gameObject.rigidbody.velocity = GetVelocity (gameObject.transform.position, new Vector3(lookAtX, 3, lookAtZ), 60);
					else
						gameObject.rigidbody.AddForce (jumpHight * transform.up + gameObject.rigidbody.velocity.normalized /2.5f, ForceMode.VelocityChange);

					AddActionFlag(ActionFlag.Action_IsBlock);
				}
				break;

		case PlayerState.Shootting:
			Control.SetBool(AnimatorStates[ActionFlag.Action_IsDribble], true);
			Control.SetBool(AnimatorStates[ActionFlag.Action_IsJump], true);
			gameObject.rigidbody.AddForce (jumpHight * transform.up + gameObject.rigidbody.velocity.normalized /2.5f, ForceMode.VelocityChange);

			break;
		}
	}

	private void SetSpeed(float value)
	{
		Control.SetFloat("Speed", value);
		Control.SetFloat("DribleMoveSpeed", value);
	}

	private bool CheckCanUseControl()
	{
		if (!CheckAction(ActionFlag.Action_IsJump) && canSteal)
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

	private void rotateTo(float lookAtX, float lookAtZ, float time = 30){
		gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(new Vector3 (lookAtX, gameObject.transform.localPosition.y, lookAtZ) - gameObject.transform.localPosition), time * Time.deltaTime);
	}

	public bool IsMove{
		get{return CheckAction(ActionFlag.Action_IsRun);}
	}
	
	public bool IsJump{
		get{return CheckAction(ActionFlag.Action_IsJump);}
	}

	public bool IsSteal{
		get{return CheckAction(ActionFlag.Action_IsSteal);}
	}

	public void DoDunk()
	{
		if (Vector3.Distance (SceneMgr.Inst.ShootPoint [Team.GetHashCode ()].transform.position, gameObject.transform.position) < 7f) {
			float ang = ElevationAngle(gameObject.transform.position, SceneMgr.Inst.ShootPoint[Team.GetHashCode()].transform.position);  
			gameObject.rigidbody.velocity = GetVelocity (gameObject.transform.position, SceneMgr.Inst.ShootPoint [Team.GetHashCode ()].transform.position, ang);
		}
		else
			Debug.Log("distance is no enght");
	}

	float ElevationAngle(Vector3 source, Vector3 target)
	{
		// find the cannon->target vector:
		Vector3 dir = target - source;
		// create a horizontal version of it:
		Vector3 dirH = new Vector3(dir.x, 0, dir.y);
		// measure the unsigned angle between them:
		float ang = Vector3.Angle(dir, dirH);
		// add the signal (negative is below the cannon):
		if (dir.y < 0)
		{ 
			ang = -ang;
		}
		
		return ang;
	}

	Vector3 GetVelocity(Vector3 source, Vector3 target, float angle)
	{
		try
		{
			Vector3 dir = target - source;  // get target direction
			float h = dir.y;  // get height difference
			dir.y = 0;  // retain only the horizontal direction
			float dist = dir.magnitude;  // get horizontal distance
			float a = angle * Mathf.Deg2Rad;  // convert angle to radians
			float tan = Mathf.Tan(a);
			dir.y = dist * tan;  // set dir to the elevation angle
			if (Mathf.Abs(tan) >= 0.01f)
			{
				dist += h / tan;
			}  // correct for small height differences
			
			// calculate the velocity magnitude
			float sin = Mathf.Sin(2 * a);
			float vel = 1;
			if (sin != 0)
			{
				float value = Mathf.Abs(dist) * Physics.gravity.magnitude;

				vel = Mathf.Sqrt(value / sin);
			}
			
			return vel * dir.normalized;
		} catch (Exception e)
		{
			Debug.Log(e.ToString());
			return Vector3.one;
		}
	}

	public void AnimationEvent(string animationName)
	{
		switch (animationName) 
		{
			case "Jumper":
				Control.SetBool ("IsJump", false);
				break;
			case "StealEnd":
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsSteal], false);
				DelActionFlag(ActionFlag.Action_IsSteal);
				break;
			case "ShootDown":
				UIGame.Get.Game.SetballController();
				DelActionFlag(ActionFlag.Action_IsJump);
				DelActionFlag(ActionFlag.Action_IsDribble);
				Control.SetBool (AnimatorStates[ActionFlag.Action_IsDribble], false);
				Control.SetBool (AnimatorStates[ActionFlag.Action_IsJump], false);
				Debug.Log("ShootDown");
				break;
			case "BlockEnd":
				Control.SetBool(AnimatorStates[ActionFlag.Action_IsBlock], false);
				DelActionFlag(ActionFlag.Action_IsBlock);
				break;
			case "Shotting":
				if (UIGame.Get.Game.ballController.gameObject == gameObject) {
					SceneMgr.Inst.RealBall.transform.localEulerAngles = Vector3.zero;                                                                                                                        
					UIGame.Get.Game.SetBallState(PlayerState.Shoot);
					SceneMgr.Inst.RealBall.rigidbody.velocity = GetVelocity(SceneMgr.Inst.RealBall.transform.position, SceneMgr.Inst.ShootPoint[Team.GetHashCode()].transform.position, 60);
				}
				break;
		}
	}
}