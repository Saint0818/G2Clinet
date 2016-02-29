using UnityEngine;
using System.Collections;
using GameEnum;


public delegate void AnimationDelegate();
public delegate void TimeScaleDelegate(AnimationEvent aniEvent);
public delegate void ZoomDelegate(float speed);
public delegate void SkillDelegate(AnimationEvent aniEvent);
//TimeScale

/// <summary>
/// 用法：
/// 1.addComponent<AnimatorBehavior>到一個角色身上 
/// 2.先Init(Animator)
/// 3.再塞各自動作委託(可省略)
/// 4.播動作 AddTrigger
/// </summary>

public class AnimatorBehavior : MonoBehaviour
{
    public Animator Controler;
    public int StateNo = -1;

    private BlockCurveCounter blockCurveCounter = new BlockCurveCounter();
    private DunkCurveCounter dunkCurveCounter = new DunkCurveCounter();
    private SharedCurveCounter fallCurveCounter = new SharedCurveCounter();
    private SharedCurveCounter pushCurveCounter = new SharedCurveCounter();
    private SharedCurveCounter pickCurveCounter = new SharedCurveCounter();
    private StealCurveCounter stealCurveCounter = new StealCurveCounter();
    private ShootCurveCounter shootCurveCounter = new ShootCurveCounter();
    private LayupCurveCounter layupCurveCounter = new LayupCurveCounter();
    private ReboundCurveCounter reboundCurveCounter = new ReboundCurveCounter();
    //    private JumpBallCurveCounter jumpBallCurveCounter = new JumpBallCurveCounter();
    private float headHeight;

    public AnimationDelegate GotStealingDel = null;
    public AnimationDelegate FakeShootBlockMomentDel = null;
    public AnimationDelegate BlockMomentDel = null;
    public AnimationDelegate AirPassMomentDel = null;
    public AnimationDelegate DoubleClickMomentDel = null;
    public AnimationDelegate BuffEndDel = null;
    public AnimationDelegate BlockCatchMomentStartDel = null;
    public AnimationDelegate BlockCatchMomentEndDel = null;
    public AnimationDelegate BlockJumpDel = null;
    public AnimationDelegate BlockCatchingEndDel = null;
    public AnimationDelegate ShootingDel = null;
    public AnimationDelegate MoveDodgeEndDel = null;
    public AnimationDelegate PassingDel = null;
    public AnimationDelegate PassEndDel = null;
    public AnimationDelegate ShowEndDel = null;
    public AnimationDelegate PickUpDel = null;
    public AnimationDelegate PickEndDel = null;
    public AnimationDelegate StealingDel = null;
    public AnimationDelegate StealingEndDel = null;
    public AnimationDelegate PushCalculateStartDel = null;
    public AnimationDelegate PushCalculateEndDel = null;
    public AnimationDelegate ElbowCalculateStartDel = null;
    public AnimationDelegate ElbowCalculateEndDel = null;
    public AnimationDelegate BlockCalculateStartDel = null;
    public AnimationDelegate BlockCalculateEndDel = null;
    public AnimationDelegate CloneMeshDel = null;
    public AnimationDelegate DunkBasketStartDel = null;
    public AnimationDelegate OnlyScoreDel = null;
    public AnimationDelegate DunkFallBallDel = null;
    public AnimationDelegate ElbowEndDel = null;
    public AnimationDelegate FallEndDel = null;
    public AnimationDelegate FakeShootEndDel = null;
    public AnimationDelegate TipInStartDel = null;
    public AnimationDelegate TipInEndDel = null;
    public AnimationDelegate AnimationEndDel = null;
    public AnimationDelegate ShowDel = null;
    public AnimationDelegate CatchDel = null;

    public TimeScaleDelegate TimeScaleCallBack = null;
    public ZoomDelegate ZoomInDel = null;
    public ZoomDelegate ZoomOutDel = null;
    public SkillDelegate SkillDel = null;

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
        TAnimatorItem findType = AnimatorMgr.Get.GetAnimatorStateType(state);
        Play(findType.Type, findType.StateNo, team);
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
            for (int i = 1; i < System.Enum.GetValues(typeof(EActionFlag)).Length; i++)
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
        shootCurveCounter.Init(stateNo, gameObject, rotateto);
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

    private float timescale = 1;

    public float TimeScaleTime
    {
        set
        {
            timescale = value;
            blockCurveCounter.Timer = timescale; 
            dunkCurveCounter.Timer = timescale; 		
            shootCurveCounter.Timer = timescale;   
            reboundCurveCounter.Timer = timescale;   
            layupCurveCounter.Timer = timescale;   
            pushCurveCounter.Timer = timescale;   
            fallCurveCounter.Timer = timescale;   
            pickCurveCounter.Timer = timescale;   
            stealCurveCounter.Timer = timescale;
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

    public void AnimationEvent(string animationName)
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
            case "BuffEnd":
                if (BuffEndDel != null)
                    BuffEndDel();
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
            case "MoveDodgeEnd":
                if (MoveDodgeEndDel != null)
                    MoveDodgeEndDel();
                break;
            case "Passing":
                if (PassingDel != null)
                    PassingDel();
                break;
            case "PassEnd":
                if (PassEndDel != null)
                    PassEndDel();
                break;
            case "ShowEnd":
                if (ShowEndDel != null)
                    ShowEndDel();
                break;
            case "PickUp":
                if (PickUpDel != null)
                    PickUpDel();
                break;
            case "PickEnd":
                if (PickEndDel != null)
                    PickEndDel();
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
            case "ElbowEnd":
                if (ElbowEndDel != null)
                    ElbowEndDel();
                break;
            case "FallEnd":
                if (FallEndDel != null)
                    FallEndDel();
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
            case "AnimationEnd":
                if (AnimationEndDel != null)
                    AnimationEndDel();
                break;
            case "Show": 
                if (ShowDel != null)
                    ShowDel();
                break;
            case "CatchEnd":
                if (CatchDel != null)
                    CatchDel();
                break;
        }
    }

    public void TimeScale(AnimationEvent aniEvent)
    {
        if (TimeScaleCallBack != null)
            TimeScaleCallBack(aniEvent);
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
