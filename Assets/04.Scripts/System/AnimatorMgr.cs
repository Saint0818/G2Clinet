using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class AnimatorMgr : KnightSingleton<AnimatorMgr>
{
	public static Dictionary<EAnimatorState, bool> LoopStates = new Dictionary<EAnimatorState, bool>();

	void Awake()
	{
		LoopStates.Add(EAnimatorState.Idle,true);
		LoopStates.Add(EAnimatorState.Run,true);
		LoopStates.Add(EAnimatorState.Defence,true);
		LoopStates.Add(EAnimatorState.Dribble,true);
		LoopStates.Add(EAnimatorState.HoldBall,true);
	}

	public bool IsLoopState(EAnimatorState state)
	{
		return LoopStates.ContainsKey (state);
	}
}

