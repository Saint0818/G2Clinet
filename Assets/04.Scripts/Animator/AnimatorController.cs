﻿using System;
using UnityEngine;
using GameEnum;
using JetBrains.Annotations;

public delegate void ZoomDelegate(float speed);
public delegate void SkillDelegate(AnimationEvent aniEvent);

/// <summary>
/// <para> 方便控制 Animator 的控制器. 每位球員身上都會有此元件. </para>
/// 
/// 用法：主要用來控制Animator跟Curve的播放.
/// 1.addComponent(AnimatorBehavior) 到一個角色身上.
/// 2.先Init(Animator)
/// 3.再塞各自動作委託(可省略)
/// 4.播動作 AddTrigger
/// </summary>

public class AnimatorController : MonoBehaviour
{
    public Animator Controler;
    public int StateNo = -1;

    private readonly BlockCurveCounter blockCurveCounter = new BlockCurveCounter();
    private readonly DunkCurveCounter dunkCurveCounter = new DunkCurveCounter();
    private readonly SharedCurveCounter fallCurveCounter = new SharedCurveCounter();
    private readonly SharedCurveCounter pushCurveCounter = new SharedCurveCounter();
    private readonly SharedCurveCounter pickCurveCounter = new SharedCurveCounter();
    private readonly StealCurveCounter stealCurveCounter = new StealCurveCounter();
    private readonly ShootCurveCounter shootCurveCounter = new ShootCurveCounter();
    private readonly LayupCurveCounter layupCurveCounter = new LayupCurveCounter();
    private readonly ReboundCurveCounter reboundCurveCounter = new ReboundCurveCounter();
    //    private JumpBallCurveCounter jumpBallCurveCounter = new JumpBallCurveCounter();
    private float headHeight;

    public Action GotStealingDel;
    public Action FakeShootBlockMomentDel;
    public Action BlockMomentDel;

    /// <summary>
    /// 呼叫時機: 跳投的空中傳球.(Shoot0 or Shoot2 Animation)
    /// </summary>
    public Action AirPassMomentDel;
    public Action DoubleClickMomentDel;
    public Action<EAnimatorState> BuffEndDel;
    public Action BlockCatchMomentStartDel;
    public Action BlockCatchMomentEndDel;
    public Action BlockJumpDel;
    public Action BlockCatchingEndDel;

    /// <summary>
    /// 呼叫時機: 投籃, 球投出的瞬間.
    /// </summary>
    public Action ShootingDel;
    public Action<EAnimatorState> MoveDodgeEndDel;

    /// <summary>
    /// 呼叫時機: 傳球動作中, 球傳出去的瞬間.
    /// </summary>
    public Action PassingDel;

    /// <summary>
    /// 呼叫時機: 傳球動作中, 傳球動作結束.
    /// </summary>
    public Action<EAnimatorState> PassEndDel;
    public Action<EAnimatorState> ShowEndDel;
    public Action PickUpDel;
    public Action<EAnimatorState> PickEndDel;
    public Action StealingDel;
    public Action StealingEndDel;
    public Action PushCalculateStartDel;
    public Action PushCalculateEndDel;
    public Action ElbowCalculateStartDel;
    public Action ElbowCalculateEndDel;
    public Action BlockCalculateStartDel;
    public Action BlockCalculateEndDel;
	public Action ReboundCalculateStartDel;
	public Action ReboundCalculateEndDel;
    public Action CloneMeshDel;
    public Action DunkBasketStartDel;
    public Action OnlyScoreDel;
    public Action DunkFallBallDel;
    public Action<EAnimatorState> ElbowEndDel;
    public Action<EAnimatorState> FallEndDel;
    public Action FakeShootEndDel;
    public Action TipInStartDel;
    public Action TipInEndDel;
    public Action<EAnimatorState> AnimationEndDel;
    public Action ShowDel;
    public Action<EAnimatorState> CatchDel;

    public ZoomDelegate ZoomInDel;
    public ZoomDelegate ZoomOutDel;
    public SkillDelegate SkillDel;

    public void Init(Animator ani)
    {
        Controler = ani;
    }

    public void Play(EAnimatorState state, int stateNo, int team)
    {
        addTrigger(state, stateNo, team);
    }

    public void Play(EPlayerState state, int team)
    {
        TAnimatorItem findType = AnimatorMgr.Get.GetAnimatorState(state);
        Play(findType.AnimatorState, findType.StateNo, team);
    }

    public void Play(EAnimatorState state, int stateNo, int team, Vector3 skillMoveTarget)
    {
        addTrigger(state, stateNo, team, false, skillMoveTarget);
    }

    public void Play(EAnimatorState state, int stateNo, int team, Vector3 skillMoveTarget, Vector3 reboundMove)
    {
        addTrigger(state, stateNo, team, false, skillMoveTarget, reboundMove);
    }

    public void Play(EAnimatorState state, int stateNo, int team, bool isDunkBlock, Vector3 skillMoveTarget)
    {
        addTrigger(state, stateNo, team, isDunkBlock, skillMoveTarget);
    }

    private void InitCurve(EAnimatorState state, int stateNo, int team, bool isDunkBlock = false,
                           Vector3 skillMoveTarget = default(Vector3), Vector3 reboundMove = default(Vector3))
    {
        int angle;
        if (SceneMgr.Get.IsCourt)
            angle = team == 0 ? 0 : 180;
        else
            angle = 90;		
        switch (state)
        {
            case EAnimatorState.Shoot:
                InitShootCurve(stateNo, CourtMgr.Get.ShootPoint[team].transform.position);
                break;
            case EAnimatorState.Layup:
                Vector3 layupPoint = CourtMgr.Get.DunkPoint[team].transform.position;
                layupPoint.z += (team == 0 ? -1 : 1);
                InitLayupCurve(stateNo, layupPoint);
                break;
            case EAnimatorState.Dunk:
				//TODO : unity bug prefab位置小數點第二位會被無條件進位
                Vector3 transPos = new Vector3(CourtMgr.Get.DunkPoint[team].transform.position.x, 
                           CourtMgr.Get.DunkPoint[team].transform.position.y, 
                           CourtMgr.Get.DunkPoint[team].transform.position.z); 
                InitDunkCurve(stateNo, transPos, angle);
                break;
            case EAnimatorState.Fall:
                InitFallCurve(stateNo);
                break;
            case EAnimatorState.Rebound:
                InitReboundCurve(stateNo, skillMoveTarget, reboundMove);
                break;

            case EAnimatorState.Block:
                InitBlockCurve(stateNo, skillMoveTarget, isDunkBlock);
                break;
        }
    }

	/// <summary>
	/// Animator播動作的方法
	/// </summary>
	/// <param name="state">State.</param>
	/// <param name="stateNo">State no.</param>
	/// <param name="team">Team.</param>
	/// <param name="isDunkBlock">If set to <c>true</c> is dunk block.</param>
	/// <param name="skillMoveTarget">Skill move target.</param>
	/// <param name="reboundMove">Rebound move.</param>
    private void addTrigger(EAnimatorState state, int stateNo, int team, bool isDunkBlock = false,
                            Vector3 skillMoveTarget = default(Vector3), Vector3 reboundMove = default(Vector3))
    {
        Controler.SetInteger("StateNo", stateNo);
        StateNo = stateNo;

        InitCurve(state, stateNo, team, isDunkBlock, skillMoveTarget, reboundMove);

        switch (state)
        {
            case EAnimatorState.Block:
            case EAnimatorState.Buff:
            case EAnimatorState.BlockCatch:
            case EAnimatorState.Catch:
            case EAnimatorState.Dunk:
            case EAnimatorState.End:
            case EAnimatorState.Elbow:
            case EAnimatorState.FakeShoot:
            case EAnimatorState.Intercept:
            case EAnimatorState.Push:
            case EAnimatorState.Pick:
            case EAnimatorState.Steal:
            case EAnimatorState.GotSteal:
            case EAnimatorState.Shoot:
            case EAnimatorState.Layup:
            case EAnimatorState.Rebound:    
            case EAnimatorState.JumpBall:
            case EAnimatorState.TipIn:
            case EAnimatorState.MoveDodge:
                ClearAnimatorFlag();
                Controler.SetTrigger(string.Format("{0}Trigger", state.ToString()));
                break;

            case EAnimatorState.Fall:
            case EAnimatorState.KnockDown:
            case EAnimatorState.Pass:
                ClearAnimatorFlag();
                Controler.Play(string.Format("{0}{1}", state.ToString(), stateNo));
                break;

            case EAnimatorState.Idle:
                ClearAnimatorFlag();
                break;
            case EAnimatorState.Defence:
                ClearAnimatorFlag(EActionFlag.IsDefence);
                break;
            case EAnimatorState.Dribble:
                ClearAnimatorFlag(EActionFlag.IsDribble);
                break;
            case EAnimatorState.HoldBall:
                ClearAnimatorFlag(EActionFlag.IsHoldBall);
                break;
            case EAnimatorState.Run:
                ClearAnimatorFlag(EActionFlag.IsRun);
                break;
            case EAnimatorState.Show:
                Controler.SetInteger("StateNo", stateNo);
                Controler.SetTrigger(string.Format("{0}Trigger", state.ToString()));
                break;
        }
    }

    public float Speed
    {
        set{ Controler.speed = value;
            TimeScaleTime = Controler.speed;
        }
        get{ return Controler.speed;} 
    }

    public void ClearAnimatorFlag(EActionFlag addFlag = EActionFlag.None)
    {
        if (addFlag == EActionFlag.None)
        {
            DelActionFlag(EActionFlag.IsDefence);
            DelActionFlag(EActionFlag.IsRun);
            DelActionFlag(EActionFlag.IsDribble);
            DelActionFlag(EActionFlag.IsHoldBall);
        }
        else
        {
            for (int i = 1; i < Enum.GetValues(typeof(EActionFlag)).Length; i++)
                if (i != (int)addFlag)
                    DelActionFlag((EActionFlag)i);

            AddActionFlag(addFlag);
        }
    }

    void OnDestroy()
    {
        if (Controler)
            Destroy(Controler);

        Controler = null;
    }

    private void AddActionFlag(EActionFlag Flag)
    {
        Controler.SetBool(Flag.ToString(), true);
    }

    public void DelActionFlag(EActionFlag Flag)
    {
        Controler.SetBool(Flag.ToString(), false);
    }

    public void SetSpeed(float value, int dir = -2)
    {
        Controler.SetFloat("MoveSpeed", value); 
    }

    public void SetTrigger(string name)
    {
        Controler.SetTrigger(name);
    }

    public void Play(string Name)
    {
        Controler.Play(Name);
    }

    public void InitDunkCurve(int stateNo, Vector3 dunkPoint, float rotateAngle)
    {	
        dunkCurveCounter.Init(stateNo, gameObject, dunkPoint, rotateAngle);
    }

    public void InitBlockCurve(int stateNo, Vector3 skillmovetarget, bool isdunkblock = false)
    {
        blockCurveCounter.Init(stateNo, gameObject, skillmovetarget, isdunkblock); 
    }

    public void InitFallCurve(int stateNo)
    {
        InitSharedCurve(EAnimatorState.Fall, stateNo);
    }

    public void InitPushCurve(int stateNo)
    {
        InitSharedCurve(EAnimatorState.Push, stateNo);
    }

    public void InitPickCurve(int stateNo)
    {
        InitSharedCurve(EAnimatorState.Pick, stateNo);
    }

    private void InitSharedCurve(EAnimatorState state, int stateNo)
    {
        switch (state)
        {
            case EAnimatorState.Fall:
                fallCurveCounter.Init(state, stateNo, gameObject);
                break;
            case EAnimatorState.Push:
                pushCurveCounter.Init(state, stateNo, gameObject);
                break;
            case EAnimatorState.Pick:
                pickCurveCounter.Init(state, stateNo, gameObject);
                break;
        }
    }

    public void InitStealCurve(int stateNo)
    {
        stealCurveCounter.Init(stateNo, gameObject);
    }

    public void InitShootCurve(int stateNo, Vector3 rotateto)
    {
        shootCurveCounter.Apply(stateNo, transform, rotateto);
    }

    public void InitLayupCurve(int stateNo, Vector3 layupPoint)
    {
        layupCurveCounter.Init(stateNo, gameObject, layupPoint);
    }

    public void InitReboundCurve(int stateNo, Vector3 skillmovetarget, Vector3 reboundMove)
    {
        headHeight = gameObject.transform.GetComponent<CapsuleCollider>().height + 0.2f;
        reboundCurveCounter.Init(gameObject, stateNo, skillmovetarget, headHeight, reboundMove);
    }

    public bool IsCanBlock
    {
        get{ return dunkCurveCounter.IsCanBlock; }
    }

    void FixedUpdate()
    {
        Controler.SetFloat("CrtHight", transform.localPosition.y);
        blockCurveCounter.FixedUpdate();
        shootCurveCounter.FixedUpdate();
        reboundCurveCounter.FixedUpdate();
        layupCurveCounter.FixedUpdate();
        pushCurveCounter.FixedUpdate();
        fallCurveCounter.FixedUpdate();
        pickCurveCounter.FixedUpdate();
        stealCurveCounter.FixedUpdate();
        dunkCurveCounter.FixedUpdate();
    }

    private bool flag;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="crtstate"></param>
    /// <returns> true:  </returns>
	public bool IsEqual(EPlayerState crtstate)
	{
	    return Controler.GetCurrentAnimatorStateInfo(0).IsName(crtstate.ToString());
	}

    private float timescale = 1;

    public float TimeScaleTime
    {
        set
        {
            timescale = value;
            blockCurveCounter.Timer = value; 
            dunkCurveCounter.Timer = value; 		
            shootCurveCounter.Timer = value;   
            reboundCurveCounter.Timer = value;   
            layupCurveCounter.Timer = value;   
            pushCurveCounter.Timer = value;   
            fallCurveCounter.Timer = value;   
            pickCurveCounter.Timer = value;   
            stealCurveCounter.Timer = value;
        }

        get{ return timescale; }
    }

    public void Reset()
    {
        dunkCurveCounter.IsPlaying = false;
        fallCurveCounter.IsPlaying = false;
    }

    public void Reset(EAnimatorState state)
    {
        switch (state)
        {
            case EAnimatorState.Dunk:
                dunkCurveCounter.IsPlaying = false;
                break;
            case EAnimatorState.Push:
                pushCurveCounter.IsPlaying = false;
                break;
        }
    }

    public void PlayDunkCloneMesh()
    {
        dunkCurveCounter.CloneMesh();
    }

    /// <summary>
    /// 呼叫時機: Animator 的 Sub-StateMachine 離開時被呼叫.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="animationName"></param>
    public void AnimatorEndEvent(EAnimatorState state, string animationName)
    {
        switch (animationName)
        {
            case "AnimationEnd":
                if(AnimationEndDel != null)
                    AnimationEndDel(state);
                break;
            case "BuffEnd":
                if (BuffEndDel != null)
                    BuffEndDel(state);
                break;
            case "CatchEnd":
                if (CatchDel != null)
                    CatchDel(state);
                break;
            case "FallEnd":
                if (FallEndDel != null)
                    FallEndDel(state);
                break;
            case "ElbowEnd":
                if (ElbowEndDel != null)
                    ElbowEndDel(state);
                break;
            case "ShowEnd":
                if (ShowEndDel != null)
                    ShowEndDel(state);
                break;
            case "PassEnd":
                if (PassEndDel != null)
                    PassEndDel(state);
                break;
            case "PickEnd":
                if (PickEndDel != null)
                    PickEndDel(state);
                break;
            case "MoveDodgeEnd":
                if (MoveDodgeEndDel != null)
                    MoveDodgeEndDel(state);
                break;
        }
    }

    /// <summary>
    /// 呼叫時機: Animation Clip 發出的 event.
    /// </summary>
    /// <param name="animationName"></param>
    [UsedImplicitly]
    private void AnimationEvent(string animationName)
    {
        switch (animationName)
        {
            case "GotStealing":
                if (GotStealingDel != null)
                    GotStealingDel();
                break;
            case "FakeShootBlockMoment":
                if (FakeShootBlockMomentDel != null)
                    FakeShootBlockMomentDel();
                break;
            case "BlockMoment":
                if (BlockMomentDel != null)
                    BlockMomentDel();
                break;
            case "AirPassMoment":
                if (AirPassMomentDel != null)
                    AirPassMomentDel();
                break;
            case "DoubleClickMoment":
                if (DoubleClickMomentDel != null)
                    DoubleClickMomentDel();
                break;
            case "BlockCatchMomentStart":
                if (BlockCatchMomentStartDel != null)
                    BlockCatchMomentStartDel();
                break;
            case "BlockCatchMomentEnd":
                if (BlockCatchMomentEndDel != null)
                    BlockCatchMomentEndDel();
                break;
            case "BlockJump":
                if (BlockJumpDel != null)
                    BlockJumpDel();
                break;
            case "BlockCatchingEnd":
                if (BlockCatchingEndDel != null)
                    BlockCatchingEndDel();
                break;
            case "Shooting":
                if (ShootingDel != null)
                    ShootingDel();
                break;
            case "Passing":
                if (PassingDel != null)
                    PassingDel();
                break;
            case "PickUp":
                if (PickUpDel != null)
                    PickUpDel();
                break;        
            case "Stealing":
                if (StealingDel != null)
                    StealingDel();
                break;
            case "StealingEnd":
                if (StealingEndDel != null)
                    StealingEndDel();
                break;
            case "PushCalculateStart":
                if (PushCalculateStartDel != null)
                    PushCalculateStartDel();
                break;
            case "PushCalculateEnd":
                if (PushCalculateEndDel != null)
                    PushCalculateEndDel();
                break;
            case "ElbowCalculateStart":
                if (ElbowCalculateStartDel != null)
                    ElbowCalculateStartDel();
                break;
            case "ElbowCalculateEnd":
                if (ElbowCalculateEndDel != null)
                    ElbowCalculateEndDel();
                break;
            case "BlockCalculateStart":
                if (BlockCalculateStartDel != null)
                    BlockCalculateStartDel();
                break;
            case "BlockCalculateEnd":
                if (BlockCalculateEndDel != null)
                    BlockCalculateEndDel();
                break;
            case "ReboundCalculateStart":
                if (ReboundCalculateStartDel != null)
                    ReboundCalculateStartDel();
                break;
            case "ReboundCalculateEnd":
                if (ReboundCalculateEndDel != null)
                    ReboundCalculateEndDel();
                break;
            case "CloneMesh":
                if (CloneMeshDel != null)
                    CloneMeshDel();
                break;
            case "DunkBasketStart":
                if (DunkBasketStartDel != null)
                    DunkBasketStartDel();
                break;
            case "OnlyScore":
                if (OnlyScoreDel != null)
                    OnlyScoreDel();
                break;
            case "DunkFallBall":
                if (DunkFallBallDel != null)
                    DunkFallBallDel();
                break;
            case "FakeShootEnd":
                if (FakeShootEndDel != null)
                    FakeShootEndDel();
                break;
            case "TipInStart":
                if (TipInStartDel != null)
                    TipInStartDel();
                break;
            case "TipInEnd":
                if (TipInEndDel != null)
                    TipInEndDel();
                break;
            case "Show": 
                if (ShowDel != null)
                    ShowDel();
                break;
        }
    }

    public void ZoomIn(float t)
    {
        if (ZoomInDel != null)
            ZoomInDel(t);
    }

    public void ZoomOut(float t)
    {
        if (ZoomOutDel != null)
            ZoomOutDel(t);
    }

    public void SkillEvent(AnimationEvent aniEvent)
    {
        if (SkillDel != null)
            SkillDel(aniEvent);
    }

}
