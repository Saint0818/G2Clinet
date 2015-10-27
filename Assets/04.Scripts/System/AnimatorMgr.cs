using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//新增的狀態必須往後加，否則會引響animator的功能
public enum EAnimatorState
{
	Block,
	Buff,
	Catch,
	Defence,
	Dribble,
	Dunk,
	Elbow,
	Fall,
	FakeShoot,
	GotSteal,
	HoldBall,
	Idle,
	Intercept,
	KnockDown,
	Layup,
	MoveDodge,
	Push,
	Pick,
	Pass,
	TipIn,
	Rebound,
	Run,
	Shoot,
	Steal,
	Show,
	BlockCatch
}

public enum EPlayerState
{
	Alleyoop,
	Block0,  
	Block1,  
	Block2,  
	Board, 
	BlockCatch,
	BasketAnimationStart,
	BasketActionEnd,
	BasketActionSwish,
	BasketActionSwishEnd,
	BasketActionNoScoreEnd,
	CatchFlat,
	CatchParabola,
	CatchFloor,
	Dribble0,
	Dribble1,
	Dribble2,
	Dribble3,
	Dunk0,
	Dunk1,
	Dunk2 = 611,
	Dunk4 = 613,
	Dunk6 = 615,
	Dunk20 = 15000,
	Dunk22 = 10600,
	DunkBasket,
	Defence0,    
	Defence1,
	Elbow0,
	Elbow1,
	Elbow2,
	Fall0,
	Fall1,
	Fall2,
	FakeShoot,
	GotSteal,
	HoldBall,
	Idle,
	Intercept0,
	Intercept1,
	Layup0, 
	Layup1 = 510, 
	Layup2 = 511, 
	Layup3 = 512, 
	MoveDodge0 = 1100,
	MoveDodge1,
	Pick0,
	Pick1,
	Pick2,
	Pass0,
	Pass1,
	Pass2,
	Pass3,
	Pass4,
	Pass5 = 1210,
	Pass6 = 1220,
	Pass7 = 1230,
	Pass8 = 1240,
	Pass9 = 1221,
	Pass50,
	Push0,
	Push1,
	Push2,
	Push20 = 11700,
	Run0,            
	Run1,            
	Run2,            
	RunningDefence,
	Rebound0,
	ReboundCatch,
	Reset,
	Start,
	Shoot0,
	Shoot1,
	Shoot2,
	Shoot3,
	Shoot4 = 410,
	Shoot5 = 411,
	Shoot6 = 412,
	Shoot7 = 413,
	Steal0,
	Steal1,
	Steal2,
	Steal20 = 11500,
	TipIn,
	JumpBall,
	Buff20 = 12100, 
	Buff21 = 12101,
	Shooting,
	Show1, 
	Show101, 
	Show102, 
	Show103, 
	Show104, 
	Show201, 
	Show202, 
	Show1001, 
	Show1003,
	Ending0,
	Ending10,
	KnockDown0,
	KnockDown1
}

public enum EAnimationEventString{
	ActiveSkillEnd,
	Stealing,
	GotStealing,
	FakeShootBlockMoment,
	BlockMoment,
	AirPassMoment,
	DoubleClickMoment,
	BlockCatchMomentStart,
	BlockCatchMomentEnd,
	BlockJump,
	Shooting,
	Passing,
	PickUp,
	PushCalculateStart,
	ElbowCalculateStart,
	BlockCalculateStart,
	BlockCalculateEnd,
	CloneMesh,
	DunkBasketStart,
	OnlyScore,
	DunkFallBall
}

public enum EanimationEventFunction
{
	AnimationEvent,
	TimeScale,
	ZoomIn,
	ZoomOut,
	MoveEvent,
	SetBallEvent,
	SkillEvent,
	EffectEvent,
	PlaySound
}

public static class StateChecker {
	private static bool isInit = false;
	public static Dictionary<EPlayerState, bool> StopStates = new Dictionary<EPlayerState, bool>();
	public static Dictionary<EPlayerState, bool> ShootStates = new Dictionary<EPlayerState, bool>();
	public static Dictionary<EPlayerState, bool> ShowStates = new Dictionary<EPlayerState, bool>();
	public static Dictionary<EPlayerState, bool> LoopStates = new Dictionary<EPlayerState, bool>();
	public static Dictionary<EPlayerState, bool> PassStates = new Dictionary<EPlayerState, bool>();
	
	public static void InitState() {
		if (!isInit) {
			isInit = true;
			
			ShootStates.Add(EPlayerState.Shoot0, true);
			ShootStates.Add(EPlayerState.Shoot1, true);
			ShootStates.Add(EPlayerState.Shoot2, true);
			ShootStates.Add(EPlayerState.Shoot3, true);
			ShootStates.Add(EPlayerState.Shoot4, true);
			ShootStates.Add(EPlayerState.Shoot5, true);
			ShootStates.Add(EPlayerState.Shoot6, true);
			ShootStates.Add(EPlayerState.Shoot7, true);
			ShootStates.Add(EPlayerState.TipIn, true);
			
			StopStates.Add(EPlayerState.Block0, true);
			StopStates.Add(EPlayerState.Block1, true);
			StopStates.Add(EPlayerState.Block2, true);
			StopStates.Add(EPlayerState.BlockCatch, true);
			StopStates.Add(EPlayerState.CatchFlat, true);
			StopStates.Add(EPlayerState.CatchFloor, true);
			StopStates.Add(EPlayerState.CatchParabola, true);
			StopStates.Add(EPlayerState.Alleyoop, true);
			StopStates.Add(EPlayerState.Elbow0, true);
			StopStates.Add(EPlayerState.Elbow1, true);
			StopStates.Add(EPlayerState.Elbow2, true);
			StopStates.Add(EPlayerState.FakeShoot, true);
			StopStates.Add(EPlayerState.HoldBall, true);
			StopStates.Add(EPlayerState.GotSteal, true);
			StopStates.Add(EPlayerState.Pass0, true);
			StopStates.Add(EPlayerState.Pass2, true);
			StopStates.Add(EPlayerState.Pass1, true);
			StopStates.Add(EPlayerState.Pass3, true);
			StopStates.Add(EPlayerState.Pass4, true);
			StopStates.Add(EPlayerState.Pass50, true);
			StopStates.Add(EPlayerState.Push0, true);
			StopStates.Add(EPlayerState.Push1, true);
			StopStates.Add(EPlayerState.Push2, true);
			StopStates.Add(EPlayerState.Push20, true);
			StopStates.Add(EPlayerState.Pick0, true);
			StopStates.Add(EPlayerState.Pick2, true);
			StopStates.Add(EPlayerState.Steal0, true);
			StopStates.Add(EPlayerState.Steal1, true);
			StopStates.Add(EPlayerState.Steal2, true);
			StopStates.Add(EPlayerState.Steal20, true);
			StopStates.Add(EPlayerState.Rebound0, true);
			StopStates.Add(EPlayerState.ReboundCatch, true);
			StopStates.Add(EPlayerState.TipIn, true);
			StopStates.Add(EPlayerState.Intercept0, true);
			StopStates.Add(EPlayerState.Intercept1, true);
			StopStates.Add(EPlayerState.MoveDodge0, true);
			StopStates.Add(EPlayerState.MoveDodge1, true);
			StopStates.Add(EPlayerState.Buff20, true);
			StopStates.Add(EPlayerState.Buff21, true);
			
			StopStates.Add(EPlayerState.Show1, true);
			StopStates.Add(EPlayerState.Show101, true);
			StopStates.Add(EPlayerState.Show102, true);
			StopStates.Add(EPlayerState.Show103, true);
			StopStates.Add(EPlayerState.Show104, true);
			StopStates.Add(EPlayerState.Show201, true);
			StopStates.Add(EPlayerState.Show202, true);
			StopStates.Add(EPlayerState.Show1001, true);
			StopStates.Add(EPlayerState.Show1003, true);
			StopStates.Add(EPlayerState.KnockDown0, true);
			StopStates.Add(EPlayerState.KnockDown1, true);
			
			StopStates.Add(EPlayerState.Ending0, true);
			StopStates.Add(EPlayerState.Ending10, true);
			
			ShowStates.Add(EPlayerState.Show1, true);
			ShowStates.Add(EPlayerState.Show101, true);
			ShowStates.Add(EPlayerState.Show102, true);
			ShowStates.Add(EPlayerState.Show103, true);
			ShowStates.Add(EPlayerState.Show104, true);
			ShowStates.Add(EPlayerState.Show201, true);
			ShowStates.Add(EPlayerState.Show202, true);
			ShowStates.Add(EPlayerState.Show1001, true);
			ShowStates.Add(EPlayerState.Show1003, true);
			
			PassStates.Add(EPlayerState.Pass0, true);
			PassStates.Add(EPlayerState.Pass1, true);
			PassStates.Add(EPlayerState.Pass2, true);
			PassStates.Add(EPlayerState.Pass3, true);
			PassStates.Add(EPlayerState.Pass4, true);
			PassStates.Add(EPlayerState.Pass5, true);
			PassStates.Add(EPlayerState.Pass6, true);
			PassStates.Add(EPlayerState.Pass7, true);
			PassStates.Add(EPlayerState.Pass8, true);
			PassStates.Add(EPlayerState.Pass9, true);
			PassStates.Add(EPlayerState.Pass50, true);
		}
	}
}

public class AnimatorMgr : KnightSingleton<AnimatorMgr>
{
	public static Dictionary<EAnimatorState, bool> LoopStates = new Dictionary<EAnimatorState, bool>();

	void Awake()
	{
		LoopStates.Add(EAnimatorState.Idle,true);
		LoopStates.Add(EAnimatorState.Run,true);
		LoopStates.Add(EAnimatorState.Defence,true);
		LoopStates.Add(EAnimatorState.Dribble,true);
	}

	public bool IsLoopState(EAnimatorState state)
	{
		return LoopStates.ContainsKey (state);
	}
}

