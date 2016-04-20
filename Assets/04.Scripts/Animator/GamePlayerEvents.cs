using UnityEngine;
using GameEnum;
using JetBrains.Annotations;

[UsedImplicitly]
public class GamePlayerEvents : StateMachineBehaviour
{
    [UsedImplicitly]
    public EAnimatorState State;

    private readonly AnimationEvent mSkillEvent = new AnimationEvent();

    /// <summary>
    /// OnStateExit 是 StateMachine 的某個子狀態結束時會被呼叫.
    /// OnStateMachineExit 是 StateMache 整個跳出才會被呼叫.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateMachinePathHash"></param>
    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
//        Debug.LogFormat("OnStateMachineExit, stateMachinePathHash:{0}, State:{1}", stateMachinePathHash, State);

        AnimatorController controller = animator.GetComponent<AnimatorController>();
        if(!controller)
        {
            Debug.LogErrorFormat("OnStateMachineExit, State:{0}, can't found AnimatorController.", State);
            return;
        }

        switch(State)
        {
            case EAnimatorState.Elbow:
                controller.AnimatorEndEvent(State, "ElbowEnd");
                break;
            case EAnimatorState.Fall:
                controller.AnimatorEndEvent(State, "FallEnd");
                break;
            case EAnimatorState.Show:
                controller.AnimatorEndEvent(State, "ShowEnd");
                break;
            case EAnimatorState.Pass:
                controller.AnimatorEndEvent(State, "PassEnd");
                break;
            case EAnimatorState.Dunk:
            case EAnimatorState.Shoot:
            case EAnimatorState.Layup:
            case EAnimatorState.KnockDown:
            case EAnimatorState.Steal:
            case EAnimatorState.Push:
            case EAnimatorState.GotSteal:
            case EAnimatorState.JumpBall:
//                Debug.LogFormat("GamePlayerEvents.OnStateExit, Event:AnimationEnd, State:{0}", State);
                controller.AnimatorEndEvent(State, "AnimationEnd");
                break;
            case EAnimatorState.Intercept:
            case EAnimatorState.Catch:
            case EAnimatorState.Rebound:
            case EAnimatorState.Block:
                controller.AnimatorEndEvent(State, "CatchEnd");
                break;
            case EAnimatorState.Pick:
                controller.AnimatorEndEvent(State, "PickEnd");
                break;
            case EAnimatorState.Buff:
                controller.AnimatorEndEvent(State, "BuffEnd");
                controller.SkillEvent(mSkillEvent);
                break;
            case EAnimatorState.MoveDodge:
                controller.AnimatorEndEvent(State, "MoveDodgeEnd");
                break;
        }
    }
}
