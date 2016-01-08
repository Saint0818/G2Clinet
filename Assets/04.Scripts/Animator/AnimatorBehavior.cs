using UnityEngine;
using System.Collections;
using GameEnum;

public class AnimatorBehavior : MonoBehaviour
{
    public Animator Controler;
    public int StateNo = -1;

    private BlockCurveCounter blockCurveCounter = new BlockCurveCounter();
	private DunkCurveCounter dunkCurveCounter;
    private SharedCurveCounter fallCurveCounter = new SharedCurveCounter();
    private SharedCurveCounter pushCurveCounter = new SharedCurveCounter();
    private SharedCurveCounter pickCurveCounter = new SharedCurveCounter();
    private StealCurveCounter stealCurveCounter = new StealCurveCounter();
    private ShootCurveCounter shootCurveCounter = new ShootCurveCounter();
    private LayupCurveCounter layupCurveCounter = new LayupCurveCounter();
    private ReboundCurveCounter reboundCurveCounter = new ReboundCurveCounter();
//    private JumpBallCurveCounter jumpBallCurveCounter = new JumpBallCurveCounter();
    private float headHeight;

    public void Init(Animator ani)
    {
        Controler = ani;
		if (dunkCurveCounter == null) 
			dunkCurveCounter = gameObject.AddComponent<DunkCurveCounter> ();
    }

    public void AddTrigger(EAnimatorState state, int stateNo)
    {
        Controler.SetInteger("StateNo", stateNo);
        StateNo = stateNo;

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
            case EAnimatorState.MoveDodge:
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

    public void InitShootCurve(int stateNo)
    {
        shootCurveCounter.Init(stateNo, gameObject);
    }

    public void InitLayupCurve(int stateNo, Vector3 layupPoint)
    {
        layupCurveCounter.Init(stateNo, gameObject, layupPoint);
    }

    public void InitReboundCurve(int stateNo, Vector3 skillmovetarget,Vector3 reboundMove)
    {
        headHeight = gameObject.transform.GetComponent<CapsuleCollider>().height + 0.2f;
        reboundCurveCounter.Init(gameObject, stateNo, skillmovetarget, headHeight,reboundMove);
    }

    /*
    public void InitAnimatorCurve(EAnimatorState state, int stateNo, ETeamKind team, Vector3 skillmovetarget, bool isdunkblock = false)
    {
        headHeight = new Vector3(0, gameObject.transform.GetComponent<CapsuleCollider>().height + 0.2f, 0);

        switch (state)
        {
            case EAnimatorState.Steal:
                stealCurveCounter.Init(stateNo, gameObject);
                break;
			case EAnimatorState.Shoot:
                shootCurveCounter.Init (stateNo, gameObject);
                break;
            case EAnimatorState.Layup:
                layupCurveCounter.Init(stateNo, gameObject);
                break;

            case EAnimatorState.Rebound:
                //ToDO:Test
                Vector3 reboundMove = Vector3.zero;
                reboundCurveCounter.Init(gameObject, stateNo, skillmovetarget, headHeight,reboundMove);
                break;
            case EAnimatorState.JumpBall:
				//reboundMove??
				reboundMove = Vector3.zero;
				jumpBallCurveCounter.Init(gameObject, stateNo, reboundMove);
                break;
        }
    }
    */

    public bool IsCanBlock
    {
        get{ return dunkCurveCounter.IsCanBlock;}
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
    }

    private float timescale = 1;

    public float TimeScale
    {
        set{
            timescale = value;
            blockCurveCounter.Timer = timescale; 

			if(dunkCurveCounter)
           		dunkCurveCounter.Timer = timescale; 
						
            shootCurveCounter.Timer = timescale;   
            reboundCurveCounter.Timer = timescale;   
            layupCurveCounter.Timer = timescale;   
            pushCurveCounter.Timer = timescale;   
            fallCurveCounter.Timer = timescale;   
            pickCurveCounter.Timer = timescale;   
            stealCurveCounter.Timer = timescale;
        }

        get{ return timescale;}
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

}
