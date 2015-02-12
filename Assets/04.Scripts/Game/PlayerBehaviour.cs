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
    Pass2 = 20,
	AlleyOop_Pass = 21,
	AlleyOop_Dunk = 22,
	RunAndDefence = 23,
	RunAndDrible = 24,
	Shooting = 25
}

public enum TeamKind{
	Self = 0,
	Npc = 1
}

public enum MoveType{
	PingPong = 0,
	Cycle = 1,
	Random = 2
}

public static class ActionFlag{
	public const int IsRun = 1;
	public const int IsDefence = 2;
	public const int IsBlock = 3;
	public const int IsJump = 4;
	public const int IsDribble = 5;
	public const int IsSteal = 6;
	public const int IsPass = 7;
}

public class PlayerBehaviour : MonoBehaviour
{
	private const float MoveCheckValue = 1;
	public static string[] AnimatorStates = new string[]{"", "IsRun", "IsDefence","IsBlock", "IsJump", "IsDribble", "IsSteal", "IsPass"};

	private byte[] PlayerActionFlag = {0, 0, 0, 0, 0, 0, 0};
	private Vector2 drag = Vector2.zero;
	private Vector2 mTargetPos = Vector2.zero;
	private bool stop = false;
	private float startMoveTime = 0;
	private float journeyLength = 0;
	private float Timer = 0;
	private int MoveTurn = 0;

	public Animator Control;
	public TeamKind Team;
	public GameObject DummyBall;
	public PlayerState crtState = PlayerState.Idle;
	public MoveType MoveKind = MoveType.PingPong;
	public GamePostion Postion = GamePostion.PG;
	public Vector2 [] RunPosAy;
	public bool ReadyTee = false;
	public float basicMoveSpeed = 0.5f;
	public float curSpeed = 0;
	public float WaitMoveTime = 0;
	public float Invincible = 0;
	public float jumpHight = 12;
	public float CoolDownSteal = 0;
	public float AirDrag = 0f;
	public int MoveIndex = -1;


	void Awake()
	{
		Control = gameObject.GetComponent<Animator>();
		DummyBall = gameObject.transform.FindChild ("DummyBall").gameObject;
	}

	void Update()
	{
		if (UIGame.Get.IsStart) {
			Control.SetFloat ("CrtHight", gameObject.transform.localPosition.y);
			if (gameObject.transform.localPosition.y > 0.2f) {
					gameObject.collider.enabled = false;
			} else {
				if(gameObject.collider.enabled == false)
					gameObject.collider.enabled = true;
			}
		}

		if (gameObject.transform.localPosition.y > 1f) {
			drag = Vector2.Lerp (Vector2.zero, new Vector2 (0, gameObject.transform.localPosition.y), 0.01f); 
			gameObject.rigidbody.drag = drag.y;
		} else {
			drag = Vector2.Lerp (new Vector2 (0, gameObject.transform.localPosition.y),Vector2.zero, 0.01f); 
			if(drag.y >= 0)
				gameObject.rigidbody.drag = drag.y;
			else
				gameObject.rigidbody.drag = 0;
		}

		if (Time.time - Timer >= 1){
			Timer = Time.time;
			
			if(WaitMoveTime > 0){
				WaitMoveTime--;
			}
		}

		if(Time.time >= Invincible)
			Invincible = 0;

		if(CoolDownSteal > 0 && Time.time >= CoolDownSteal)
			CoolDownSteal = 0;
	}

	public void OnJoystickMove(MovingJoystick move)
	{
		if (CheckCanUseControl() || stop) {
			EffectManager.Get.SelectEffectScript.SetParticleColor(false);
			curSpeed = Vector2.Distance (new Vector2 (move.joystickAxis.x, 0), new Vector2 (0, move.joystickAxis.y));
			SetSpeed (curSpeed);

			if (Mathf.Abs (move.joystickAxis.y) > 0)
			{
				if(UIGame.Get.Game.BallController && UIGame.Get.Game.BallController.gameObject == gameObject)
					AniState(PlayerState.RunAndDrible);
				else
					AniState(PlayerState.Run);
			}

			float angle = move.Axis2Angle (true);
			int a = 90;
			Vector3 rotation = new Vector3 (0, angle + a, 0);
			transform.rotation = Quaternion.Euler (rotation);
			Vector3 translate = Vector3.forward * Time.deltaTime * curSpeed * 10 * basicMoveSpeed;
			transform.Translate (translate);	
		}
	}

	public void MoveTo(float X, float Z, float lookAtX, float loolAtZ){
		if (!CheckAction (ActionFlag.IsSteal) && !CheckAction (ActionFlag.IsJump) && !CheckAction(ActionFlag.IsBlock)) {
			if ((gameObject.transform.localPosition.x <= X + MoveCheckValue && gameObject.transform.localPosition.x >= X - MoveCheckValue) && 
			    (gameObject.transform.localPosition.z <= Z + MoveCheckValue && gameObject.transform.localPosition.z >= Z - MoveCheckValue)) {
				SetSpeed(0);
				DelActionFlag(ActionFlag.IsRun);
				MoveTurn = 0;
				startMoveTime = 0;
				journeyLength = 0;
				TargetPos = Vector2.zero;
				if(!CheckAction(ActionFlag.IsDefence)){
					if(UIGame.Get.Game.situation == GameSituation.TeeA){
						if(Postion == GamePostion.PF){
							//Pass ball to PG
							ReadyTee = true;
						}else
							AniState(PlayerState.Idle);
					}else if(UIGame.Get.Game.situation == GameSituation.TeeB){
						if(Postion == GamePostion.PF){
							//Pass ball to PG
							ReadyTee = true;
						}else
							AniState(PlayerState.Idle);
					}else if(UIGame.Get.Game.situation == GameSituation.TeeAPicking){
						AniState(PlayerState.Idle);
					}else if(UIGame.Get.Game.situation == GameSituation.TeeBPicking){
						AniState(PlayerState.Idle);
					}else{
						WaitMoveTime = (float)UnityEngine.Random.Range(0, 3);
						
						if(UIGame.Get.Game.BallController && UIGame.Get.Game.BallController.gameObject == gameObject){
							if(Team == TeamKind.Self)
								rotateTo(SceneMgr.Inst.ShootPoint[0].transform.position.x, SceneMgr.Inst.ShootPoint[0].transform.position.z);
							else
								rotateTo(SceneMgr.Inst.ShootPoint[1].transform.position.x, SceneMgr.Inst.ShootPoint[1].transform.position.z);
						}else
							rotateTo(lookAtX, loolAtZ);
					}
				}else{
					WaitMoveTime = 0;
					rotateTo(lookAtX, loolAtZ);
				}
			}else if(!CheckAction(ActionFlag.IsDefence) && MoveTurn >= 0 && MoveTurn <= 5 && CanMoverotateTo()){
				AddActionFlag(ActionFlag.IsRun);
				MoveTurn++;
				rotateTo(X, Z, 10);
			
				if(MoveTurn == 1){
					startMoveTime = Time.time;
					journeyLength = Vector3.Distance(gameObject.transform.localPosition, new Vector3 (X, gameObject.transform.localPosition.y, Z));
				}
			}else{
				float fracJourney = 0.045f;
				SetSpeed(1);
				rotateTo(X, Z, 10);
				if(CheckAction(ActionFlag.IsDefence)){
					AniState(PlayerState.RunAndDefence);
				}else{
					if(UIGame.Get.Game.BallController && UIGame.Get.Game.BallController.gameObject == gameObject)
						AniState(PlayerState.RunAndDrible);
					else
						AniState(PlayerState.Run);

					if(journeyLength != 0)
						fracJourney = ((Time.time - startMoveTime) * basicMoveSpeed) / journeyLength;
				}

				gameObject.transform.localPosition = Vector3.Lerp (gameObject.transform.localPosition, new Vector3 (X, gameObject.transform.localPosition.y, Z), fracJourney);
			}		
		}
	}

	private bool CanMoverotateTo(){
		if(UIGame.Get.Game.BallController != null)
			return true;
		else if(Team == TeamKind.Self && (UIGame.Get.Game.situation == GameSituation.TeeB || UIGame.Get.Game.situation == GameSituation.TeeBPicking))
			return true;
		else if(Team == TeamKind.Npc && (UIGame.Get.Game.situation == GameSituation.TeeA || UIGame.Get.Game.situation == GameSituation.TeeAPicking))
			return true;
		else
			return false;
	}

	public void OnJoystickMoveEnd(MovingJoystick move)
	{
		SetSpeed(0);
		if (UIGame.Get.Game.BallController && UIGame.Get.Game.BallController.gameObject == gameObject)
			AniState(PlayerState.Dribble);
		else
			AniState (PlayerState.Idle);
	}

	public void AniState(PlayerState state, bool DorotateTo = false, float lookAtX = -1, float lookAtZ = -1)
	{
		crtState = state;

		if(DorotateTo)
			rotateTo(lookAtX, lookAtZ);

		switch (state) {
			case PlayerState.Idle:
				for (int i = 1; i < AnimatorStates.Length; i++)
					if(AnimatorStates[i] != string.Empty)
						Control.SetBool(AnimatorStates[i], false);
				break;
			case PlayerState.Walk:
				break;
			case PlayerState.Run:
				Control.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				break;
			case PlayerState.Dribble:
				AddActionFlag(ActionFlag.IsDribble);
				Control.SetBool(AnimatorStates[ActionFlag.IsDribble], true);
				break;
			case PlayerState.RunAndDrible:
				Control.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				Control.SetBool(AnimatorStates[ActionFlag.IsDribble], true);
				break;
			case PlayerState.Defence:
				Control.SetBool("IsDefence", true);
				SetSpeed(0);
				AddActionFlag (ActionFlag.IsDefence);
				break;
			case PlayerState.RunAndDefence:
				Control.SetBool(AnimatorStates[ActionFlag.IsRun], true);
				Control.SetBool(AnimatorStates[ActionFlag.IsDefence], true);
				break;
			case PlayerState.Jumper:
				if(!CheckAction(ActionFlag.IsJump)){
					Control.SetBool(AnimatorStates[ActionFlag.IsJump], true);
					gameObject.rigidbody.AddForce (jumpHight * transform.up + gameObject.rigidbody.velocity.normalized /2.5f, ForceMode.VelocityChange);
					AddActionFlag(ActionFlag.IsJump);
				}
				break;
			case PlayerState.Steal:
				if(!CheckAction(ActionFlag.IsSteal)){
					Control.SetBool(AnimatorStates[ActionFlag.IsSteal], true);
					AddActionFlag(ActionFlag.IsSteal);
				}
				break;
			case PlayerState.Pass:
				if(!CheckAction(ActionFlag.IsPass))
				{
					AddActionFlag(ActionFlag.IsPass);
					Control.SetBool(AnimatorStates[ActionFlag.IsPass], true);
					UIGame.Get.Game.Passing = true;
				}
				break;
			case PlayerState.Block:
				if(!CheckAction(ActionFlag.IsBlock)){
					AddActionFlag(ActionFlag.IsBlock);
					Control.SetBool(AnimatorStates[ActionFlag.IsBlock], true);

					if (UIGame.Get.Game.BallController) 
					{
						if(Vector3.Distance(UIGame.Get.targetPlayer.gameObject.transform.position, UIGame.Get.Game.BallController.gameObject.transform.position) < 5f)
							gameObject.rigidbody.AddForce (jumpHight * transform.up + gameObject.rigidbody.velocity.normalized /2.5f, ForceMode.VelocityChange);
						else
							gameObject.rigidbody.velocity = GameFunction.GetVelocity (gameObject.transform.position, new Vector3(UIGame.Get.Game.BallController.transform.position.x, 7, UIGame.Get.Game.BallController.transform.position.z), 70);
					} 
					else 
					{
						if(UIGame.Get.Game.ShootController && Vector3.Distance(gameObject.transform.position, SceneMgr.Inst.RealBall.transform.position) < 5)
							gameObject.rigidbody.velocity = GameFunction.GetVelocity (gameObject.transform.position, new Vector3(SceneMgr.Inst.RealBall.transform.position.x, 5, SceneMgr.Inst.RealBall.transform.position.z), 70);
					}
				}
				break;
			case PlayerState.Shooting:
				if(!CheckAction(ActionFlag.IsJump) && UIGame.Get.Game.BallController == this)
				{
					AddActionFlag(ActionFlag.IsJump);
					AddActionFlag(ActionFlag.IsDribble);
					Control.SetBool(AnimatorStates[ActionFlag.IsDribble], true);
					Control.SetBool(AnimatorStates[ActionFlag.IsJump], true);
				}
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
		if (!CheckAction(ActionFlag.IsJump) &&
		    UIGame.Get.Game.situation != GameSituation.None &&
		    UIGame.Get.Game.situation != GameSituation.TeeA &&
		    UIGame.Get.Game.situation != GameSituation.TeeB && 
		    UIGame.Get.Game.situation != GameSituation.TeeBPicking &&
		    UIGame.Get.Game.situation != GameSituation.End)
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
			startMoveTime = 0;
			journeyLength = 0;
			DelActionFlag(ActionFlag.IsRun);
		}
		get{
			return mTargetPos;
		}
	}

	public void ResetFlag(){
		for(int i = 0; i < PlayerActionFlag.Length; i++)
			PlayerActionFlag[i] = 0;

		ReadyTee = false;
		SetSpeed(0);
		AniState(PlayerState.Idle);
	}

	private void rotateTo(float lookAtX, float lookAtZ, float time = 30){
		gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(new Vector3 (lookAtX, gameObject.transform.localPosition.y, lookAtZ) - gameObject.transform.localPosition), time * Time.deltaTime);
	}

	public bool IsMove{
		get{return CheckAction(ActionFlag.IsRun);}
	}

	public bool IsJump{
		get{return CheckAction(ActionFlag.IsJump);}
	}

	public bool IsSteal{
		get{return CheckAction(ActionFlag.IsSteal);}
	}

	public void DoDunk()
	{
		if (Vector3.Distance (SceneMgr.Inst.ShootPoint [Team.GetHashCode ()].transform.position, gameObject.transform.position) < 7f) {
			float ang = GameFunction.ElevationAngle(gameObject.transform.position, SceneMgr.Inst.ShootPoint[Team.GetHashCode()].transform.position);  
			gameObject.rigidbody.velocity = GameFunction.GetVelocity (gameObject.transform.position, SceneMgr.Inst.ShootPoint [Team.GetHashCode ()].transform.position, ang);
		}
		else
			Debug.Log("distance is no enght");
	}

	public void AnimationEvent(string animationName)
	{
		switch (animationName) 
		{
			case "Jumper":
				Control.SetBool ("IsJump", false);
				DelActionFlag(ActionFlag.IsJump);
				break;
			case "StealEnd":
				DelActionFlag(ActionFlag.IsSteal);
				Control.SetBool(AnimatorStates[ActionFlag.IsSteal], false);
				break;
			case "ShootDown":
				UIGame.Get.Game.SetBall();
				DelActionFlag(ActionFlag.IsJump);
				DelActionFlag(ActionFlag.IsDribble);
				Control.SetBool (AnimatorStates[ActionFlag.IsDribble], false);
				Control.SetBool (AnimatorStates[ActionFlag.IsJump], false);
				DelActionFlag(ActionFlag.IsJump);
				UIGame.Get.Game.Shooting = false;
				break;
			case "BlockEnd":
				Control.SetBool(AnimatorStates[ActionFlag.IsBlock], false);
				DelActionFlag(ActionFlag.IsBlock);
				break;
			case "Shooting":
				if (UIGame.Get.Game.BallController.gameObject == gameObject) {
					UIGame.Get.Game.ShootController = this;
					SceneMgr.Inst.RealBall.transform.localEulerAngles = Vector3.zero;                                                                                                                        
					UIGame.Get.Game.SetBallState(PlayerState.Shooting);
					SceneMgr.Inst.RealBall.rigidbody.velocity = GameFunction.GetVelocity(SceneMgr.Inst.RealBall.transform.position, SceneMgr.Inst.ShootPoint[Team.GetHashCode()].transform.position, 60);
				}
				break;
			case "ShootJump":
				gameObject.rigidbody.AddForce (jumpHight * transform.up + gameObject.rigidbody.velocity.normalized /2.5f, ForceMode.VelocityChange);
				break;
			case "Passing":
				UIGame.Get.Game.SetBallState(PlayerState.Pass);
				SceneMgr.Inst.RealBallTrigger.PassBall(gameObject, UIGame.Get.Game.Catcher.gameObject); 
				break;
			case "PassEnd":
				Control.SetBool (AnimatorStates[ActionFlag.IsDribble], false);
				Control.SetBool (AnimatorStates[ActionFlag.IsPass], false);
				DelActionFlag(ActionFlag.IsPass);
				DelActionFlag(ActionFlag.IsDribble);
				break;
		}
	}

	public void SetInvincible(float time){
		if(Invincible == 0)
			Invincible = Time.time + time;
		else
			Invincible += time;
	}
}