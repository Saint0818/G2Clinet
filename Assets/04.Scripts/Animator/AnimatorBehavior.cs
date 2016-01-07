using UnityEngine;
using System.Collections;
using GameEnum;

public class AnimatorBehavior : MonoBehaviour 
{
    public Animator Controler;
    public int StateNo = -1;

    public void Init(Animator ani)
    {
        Controler = ani;
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


}
