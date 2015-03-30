using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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
	BlockCatch = 11,
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
	public const int IsBlockCatch = 4;
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
	public OnPlayerAction OnSteal = null;
	public OnPlayerAction OnBlockJump = null;
	public OnPlayerAction OnBlocking = null;
	public OnPlayerAction OnDunkBasket = null;
	public OnPlayerAction OnDunkJump = null;
	
    public Vector3 Translate;
	public float[] DunkHight = new float[2]{3, 5};
	private const float MoveCheckValue = 0.5f;
	private const int ChangeToAI = 4;
	public static string[] AnimatorStates = new string[]{"", "IsRun", "IsDefence","IsBlock", "IsBlockCatch", "IsDribble", "IsSteal", "IsPass", "IsShoot", "IsCatcher", "IsDunk", "IsShootIdle", "IsFakeShoot"};

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
	public PlayerBehaviour DefPlayer = null;
	public bool AutoFollow = false;

	//Dunk
	private bool isDunk = false;
	private bool isZmove = false;
	private float dunkTime = 0;
	private Vector3[] dunkPath = new Vector3[5];
	public AniCurve aniCurve;
	private TDunkCurve playDunkCurve;

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

			DefTrigger obj3 = obj2.GetComponentInChildren<DefTrigger>(); 
			if(obj3 != null)
				obj3.Player = this;

			GameObject obj4 = obj.transform.FindChild("DefRange").gameObject;
			if(obj4 != null){
				BoxCollider b = obj4.GetComponent<BoxCollider>();
				b.size = new Vector3(GameConst.AIlevelAy[AILevel].DefDistance, 1, GameConst.AIlevelAy[AILevel].DefDistance);
			}
			
			obj2.transform.parent = transform;
			obj2.transform.transform.localPosition = Vector3.zero;
			obj2.transform.transform.localScale = Vector3.one;
		}
	}

	void Awake()
	{
		gameObject.layer = LayerMask.NameToLayer ("Player");
		gameObject.tag = "Player";

		animator = gameObject.GetComponent<Animator>();
		playerCollider = gameObject.GetComponent<Collider>();
		PlayerRigidbody = gameObject.GetComponent<Rigidbody>();
		DummyBall = gameObject.transform.FindChild ("DummyBall").gameObject;
		aniCurve = gameObject.transform.FindChild ("AniCurve").gameObject.GetComponent<AniCurve>();
		initTrigger();
	}

	void FixedUpdate()
	{
		CalculationPlayerHight ();
		CalculationAnimatorSmoothSpeed ();

		switch (crtState) {
			case PlayerState.Dunk:
			case PlayerState.DunkBasket:
				CalculationDunkMove ();
			break;
		}
		
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
		else 
		if(MoveQueue.Count > 0)
			MoveTo(MoveQueue.Peek());

		if (AniWaitTime > 0 && AniWaitTime <= Time.time) {
			AniWaitTime = 0;
			if(NeedResetFlag)
				ResetFlag();
		}

		if (isJoystick) {
			if(Time.time >= NoAiTime){
				MoveQueue.Clear();
				NoAiTime = 0;
				isJoystick = false;
				DelActionFlag (ActionFlag.IsRun);

				if (AIActiveHint)
					AIActiveHint.SetActive(true);
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

				if(Vector3.Distance(ShootPoint, DefPlayer.transform.position) <= 12){
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

		if (AIActiveHint)
					AIActiveHint.SetActive(false);
	}
	
	private void CalculationAirResistance()
	{
		if (gameObject.transform.localPosition.y > 1f) {
			drag = Vector2.Lerp (Vector2.zero, new Vector2 (0, gameObject.transform.localPosition.y), 0.01f); 
			PlayerRigidbody.drag = drag.y;
		} else {
			drag = Vector2.Lerp (new Vector2 (0, gameObject.transform.localPosition.y),Vector2.zero, 0.01f); 
			if(drag.y >= 0)
				PlayerRigidbody.drag = drag.y;
			else
				PlayerRigidbody.drag = 0;
		}
	}

	private void CalculationDunkMove()
	{
		if (!isDunk)
			return;

		if (playDunkCurve != null) {
			gameObject.transform.position = new Vector3 (gameObject.transform.position.x, playDunkCurve.aniCurve.Evaluate (dunkTime), gameObject.transform.position.z);

			if (!isZmove && dunkTime > playDunkCurve.StartMoveTime) {
				isZmove = true;
				gameObject.transform.DOLocalMoveZ (dunkPath [4].z, playDunkCurve.ToBasketTime - playDunkCurve.StartMoveTime).SetEase (Ease.Linear);
				gameObject.transform.DOLocalMoveX (dunkPath [4].x, playDunkCurve.ToBasketTime - playDunkCurve.StartMoveTime).SetEase (Ease.Linear);
			}

			dunkTime += Time.deltaTime;
			if (dunkTime >= playDunkCurve.LifeTime)
				isDunk = false;
		} else {
			isDunk = false;
			Debug.LogError ("playCurve is null");
		}
	}

	private void CalculationAnimatorSmoothSpeed()
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

	private void CalculationPlayerHight()
	{
		animator.SetFloat ("CrtHight", gameObject.transform.localPosition.y);
		if (gameObject.transform.localPosition.y > 0.2f) {
			playerCollider.enabled = false;
		} else 
			if(playerCollider.enabled == false)
				playerCollider.enabled = true;
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

				SetNoAiTime();

				animationSpeed = Vector2.Distance (new Vector2 (move.joystickAxis.x, 0), new Vector2 (0, move.joystickAxis.y));
				SetSpeed (animationSpeed, 0);
				AniState(ps);

				float angle = move.Axis2Angle (true);
				int a = 90;
				Vector3 rotation = new Vector3 (0, angle + a, 0);
				transform.rotation = Quaternion.Euler (rotation);
				
				if(animationSpeed <= MoveMinSpeed){
					if(IsBallOwner)
						Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.BallOwnerSpeedNormal;
					else{
						if(IsDefence){
							Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.DefSpeedNormal;
						}else
							Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.AttackSpeedNormal;
					}						
				}else{
					if(IsBallOwner)
						Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.BallOwnerSpeedup;
					else{
						if(IsDefence)
							Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.DefSpeedup;
						else
							Translate = Vector3.forward * Time.deltaTime * GameConst.BasicMoveSpeed * GameConst.AttackSpeedup;
					}
				}
				
				transform.Translate (Translate);	
			}
		}
	}

	public void OnJoystickMoveEnd(MovingJoystick move, PlayerState ps)
	{
		isJoystick = false;
		SetNoAiTime();
		if(crtState != ps)
			AniState(ps);
	}

	private void DunkTo()
	{
		if (GameStart.Get.TestMode == GameTest.Dunk) {
			if(dkPathGroup)
				Destroy(dkPathGroup);

			dkPathGroup = new GameObject();
			dkPathGroup.name = "pathGroup";
		}

		PlayerRigidbody.useGravity = false;
		PlayerRigidbody.isKinematic = true;

		dunkPath [4] = SceneMgr.Get.DunkPoint [Team.GetHashCode ()].transform.position;
		float dis = Vector3.Distance(gameObject.transform.position, dunkPath [4]);
		float maxH = DunkHight[0] + (DunkHight[1] - DunkHight[0] / (dis * 0.25f));
		dunkPath [0] = gameObject.transform.position;
		dunkPath [2] = new Vector3 ((dunkPath [dunkPath.Length - 1].x + dunkPath [0].x) / 2, maxH, (dunkPath [dunkPath.Length - 1].z + dunkPath [0].z) / 2);
		dunkPath [3] = new Vector3 ((dunkPath [dunkPath.Length - 1].x + dunkPath [2].x) / 2, DunkHight[1], (dunkPath [dunkPath.Length - 1].z + dunkPath [2].z) / 2);
		dunkPath [1] = new Vector3 ((dunkPath [2].x + dunkPath [0].x) / 2, 6, (dunkPath [2].z + dunkPath [0].z) / 2);

		playDunkCurve = null;
		for (int i = 0; i < aniCurve.Dunk.Length; i++) {
			if(aniCurve.Dunk[i].Name == "Dunk")
				playDunkCurve = aniCurve.Dunk[i];
		}

		isDunk = true;
		isZmove = false;
		dunkTime = 0;
    }

	private void PathCallBack() {
		Vector3[] path2 = new Vector3[2];
		path2 = new Vector3[2]{dunkPath [3], dunkPath [4]};
		gameObject.transform.DOPath(path2, 0.4f, PathType.CatmullRom, PathMode.Full3D, 10, Color.red).SetEase(Ease.OutBack);
	}
	
	private GameObject dkPathGroup;
	
	public void OnDunkInto()
	{
		if (CheckAction (ActionFlag.IsDunk))
			if (!animator.GetBool ("IsDunkInto")) {
				PlayerRigidbody.useGravity = false;
				PlayerRigidbody.velocity = Vector3.zero;
				PlayerRigidbody.isKinematic = true;
				gameObject.transform.position = SceneMgr.Get.DunkPoint[Team.GetHashCode()].transform.position;
				if(IsBallOwner)
					SceneMgr.Get.RealBall.transform.position = SceneMgr.Get.ShootPoint[Team.GetHashCode()].transform.position;
				
				animator.SetBool("IsDunkInto", true); 
			}
	}
	
	public void DelPass(){
		DelActionFlag (ActionFlag.IsPass);
    }

	private int MinIndex (float [] floatAy) {
		int Result = 0;
		float Min = floatAy[0];
		
		for (int i = 1; i < floatAy.Length; i++) {
			if (floatAy[i] < Min) {
				Min = floatAy[i];
				Result = i;
			}
		}
		
		return Result;
	}

	private Vector2 GetMoveTarget (TMoveData Data){
		Vector2 Result = Vector2.zero;

		if(Data.DefPlayer != null){
			float dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint[Data.DefPlayer.Team.GetHashCode()].transform.position);
			float [] disAy = new float[4];
			for(int i = 0; i < disAy.Length; i++)
				disAy[i] = Vector3.Distance(Data.DefPlayer.DefPointAy[i].position, SceneMgr.Get.ShootPoint[Data.DefPlayer.Team.GetHashCode()].transform.position);

			int mIndex = MinIndex(disAy);

			if(mIndex >= 0 && mIndex < disAy.Length){
				Result = new Vector2(Data.DefPlayer.DefPointAy[mIndex].position.x, Data.DefPlayer.DefPointAy[mIndex].position.z);					
				
				if(GameConst.AIlevelAy[AILevel].ProactiveRate >= ProactiveRate && Data.DefPlayer.IsBallOwner || dis <= 6)
					Result = new Vector2(Data.DefPlayer.DefPointAy[mIndex + 4].position.x, Data.DefPlayer.DefPointAy[mIndex + 4].position.z);
			}
		}else if(Data.FollowTarget != null)
			Result = new Vector2(Data.FollowTarget.position.x, Data.FollowTarget.position.z);
		else
			Result = Data.Target;

		return Result;
	}

	public void MoveTo(TMoveData Data, bool First = false){
		if (CanMove && WaitMoveTime == 0) {
			Vector2 MoveTarget = GetMoveTarget(Data);

			if ((gameObject.transform.localPosition.x <= MoveTarget.x + MoveCheckValue && gameObject.transform.localPosition.x >= MoveTarget.x - MoveCheckValue) && 
			    (gameObject.transform.localPosition.z <= MoveTarget.y + MoveCheckValue && gameObject.transform.localPosition.z >= MoveTarget.y - MoveCheckValue)) {
				MoveTurn = 0;
				DelActionFlag(ActionFlag.IsRun);

				if(IsDefence){
					WaitMoveTime = 0;
					if(Data.LookTarget == null)
						rotateTo(MoveTarget.x, MoveTarget.y);
					else
						rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);
					
					AniState(PlayerState.Defence);							
				}else{
					if(!IsBallOwner)
						AniState(PlayerState.Idle);
					
					if(First)
						WaitMoveTime = 0;
					else 
						if(situation != GameSituation.TeeA && situation != GameSituation.TeeAPicking && situation != GameSituation.TeeB && situation != GameSituation.TeeBPicking)
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
				}

				if(Data.MoveFinish != null)
					Data.MoveFinish(this, Data.Speedup);

				if(First)
					FirstMoveQueue.Dequeue();
				else
					MoveQueue.Dequeue();
			}else 
			if(!CheckAction(ActionFlag.IsDefence) && MoveTurn >= 0 && MoveTurn <= 5){
				AddActionFlag(ActionFlag.IsRun);
				MoveTurn++;
				rotateTo(MoveTarget.x, MoveTarget.y, 10);
				if(MoveTurn == 1)
					MoveStartTime = Time.time + 1;
			}else{
				if(IsDefence){
					if(Data.DefPlayer != null){
						float dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint[Data.DefPlayer.Team.GetHashCode()].transform.position);
						float dis2 = Vector3.Distance(transform.position, Data.DefPlayer.transform.position);
						if(Data.LookTarget == null || dis > GameConst.TreePointDistance)
							rotateTo(MoveTarget.x, MoveTarget.y);
						else
							rotateTo(Data.LookTarget.position.x, Data.LookTarget.position.z);

						dis = Vector3.Distance(transform.position, SceneMgr.Get.ShootPoint[Data.DefPlayer.Team.GetHashCode()].transform.position);
						dis2 = Vector3.Distance(new Vector3(MoveTarget.x, 0, MoveTarget.y), SceneMgr.Get.ShootPoint[Data.DefPlayer.Team.GetHashCode()].transform.position);

						if(dis <= GameConst.TreePointDistance){
							if(dis2 < dis)
								AniState(PlayerState.MovingDefence);							
							else
								AniState(PlayerState.RunningDefence);
						}else
							AniState(PlayerState.RunningDefence);
					}else{
						rotateTo(MoveTarget.x, MoveTarget.y);
						AniState(PlayerState.Run);
					}

					if(Data.Speedup)
						transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * GameConst.DefSpeedup * GameConst.BasicMoveSpeed);
					else
						transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveTarget.x, 0, MoveTarget.y), Time.deltaTime * GameConst.DefSpeedNormal * GameConst.BasicMoveSpeed);
				}else{
					rotateTo(MoveTarget.x, MoveTarget.y, 10);

					if(IsBallOwner)
						AniState(PlayerState.RunAndDribble);
					else
						AniState(PlayerState.Run);

					if(IsBallOwner){
						if(Data.Speedup)
							transform.Translate (Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedup * GameConst.BasicMoveSpeed);
						else
							transform.Translate (Vector3.forward * Time.deltaTime * GameConst.BallOwnerSpeedNormal * GameConst.BasicMoveSpeed);
					}else{
						if(Data.Speedup)
							transform.Translate (Vector3.forward * Time.deltaTime  * GameConst.AttackSpeedup * GameConst.BasicMoveSpeed);
						else
							transform.Translate (Vector3.forward * Time.deltaTime  * GameConst.AttackSpeedNormal * GameConst.BasicMoveSpeed);
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
		if (AniWaitTime == 0) {
			for(int i = 0; i < PlayerActionFlag.Length; i++)
				PlayerActionFlag[i] = 0;
			
			AniState(PlayerState.Idle);
			MoveQueue.Clear ();
			FirstMoveQueue.Clear ();
			NoAiTime = 0;
			WaitMoveTime = 0;
			isJoystick = false;	
		}else
			NeedResetFlag = true;
	}

	public void ClearMoveQueue(){
		MoveQueue.Clear ();
		FirstMoveQueue.Clear ();
	}

	private bool CanUseState(PlayerState state)
	{
		if (state != PlayerState.FakeShoot && crtState != state)
			return true;
		else 
		if(state == PlayerState.FakeShoot)
			return true;

		return false;
	}

	public void AniState(PlayerState state, Vector3 v) {
		if (!CanUseState(state))
			return;

		rotateTo(v.x, v.z);
		AniState(state);
	}

	public void AniState(PlayerState state)
	{
		if (!CanUseState(state))
			return;

		crtState = state;
		
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

				AddActionFlag(ActionFlag.IsDribble);
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
	                }

	                break;
			case PlayerState.BlockCatch:
				if (!CheckAction(ActionFlag.IsBlockCatch)){
					AddActionFlag(ActionFlag.IsBlockCatch);
					animator.SetBool(AnimatorStates[ActionFlag.IsBlockCatch], true);
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
					AniWaitTime = Time.time + 2.9f;
					DunkTo();
				}
				break;
        }
    }
    
    public void AnimationEvent(string animationName)
    {
        switch (animationName) 
		{
			case "StealEnd":
				if (OnSteal != null)
					OnSteal(this);

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
				if (OnBlockJump != null)
					OnBlockJump(this);

				break;
			case "Blocking":
				if (OnBlocking != null)
					OnBlocking(this);

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
				PlayerRigidbody.AddForce (JumpHight * transform.up + PlayerRigidbody.velocity.normalized /2.5f, ForceMode.Force);
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
				playerCollider.enabled = false;
				if(OnDunkJump != null)
					OnDunkJump(this);

				break;
			case "DunkBasket":
				DelActionFlag(ActionFlag.IsDribble);
				animator.SetBool(AnimatorStates[ActionFlag.IsDribble], false);
				DelActionFlag(ActionFlag.IsRun);
				animator.SetBool(AnimatorStates[ActionFlag.IsRun], false);
				PlayerRigidbody.useGravity = false;
				PlayerRigidbody.isKinematic = true;
				SceneMgr.Get.PlayDunk(Team.GetHashCode());
				break;
			case "DunkFallBall":
				if(OnDunkBasket != null)
					OnDunkBasket(this);

				break;
			case "DunkFall":
				PlayerRigidbody.useGravity = true;
				PlayerRigidbody.isKinematic = false;
				break;
			case "DunkEnd":
				animator.SetBool (AnimatorStates[ActionFlag.IsDunk], false);
//				animator.SetBool ("IsDunkInto", false);
				if(!NeedResetFlag){
					DelActionFlag(ActionFlag.IsDunk);
					DelActionFlag(ActionFlag.IsShootIdle);
				}
				animator.SetBool(AnimatorStates[ActionFlag.IsShootIdle], false);
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
			CloseDef = Time.time + GameConst.AIlevelAy[AILevel].AutoFollowTime;
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

	public bool IsFakeShoot{
		get{return CheckAction(ActionFlag.IsFakeShoot);}
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