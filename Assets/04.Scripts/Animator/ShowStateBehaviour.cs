using UnityEngine;
using GameEnum;
using JetBrains.Annotations;

[UsedImplicitly]
public class ShowStateBehaviour : StateMachineBehaviour
{
    public EAnimatorState State;
    private AnimationEvent skillEvent = new AnimationEvent();

    /// <summary>
    /// 本來用 OnStateMachineExit, 改用 OnStateExit. 因為經過我測試，我發現 Animator 裡面若是
    /// 沒有拉連線到 Exit Node，那麼 OnStateMachineExit 有可能會發生錯誤，但是 OnStateExit 不會
    /// 發生錯誤，一樣會正常呼叫。
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
//    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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
                controller.AnimationEvent("ElbowEnd");
                break;
            case EAnimatorState.Fall:
                controller.AnimationEvent("FallEnd");
                break;
            case EAnimatorState.Show:
                controller.AnimationEvent("ShowEnd");
                break;
            case EAnimatorState.Pass:
                controller.AnimationEvent("PassEnd");
                break;
            case EAnimatorState.Dunk:
            case EAnimatorState.Shoot:
            case EAnimatorState.Layup:
            case EAnimatorState.KnockDown:
            case EAnimatorState.Steal:
            case EAnimatorState.Push:
            case EAnimatorState.GotSteal:
            case EAnimatorState.JumpBall:
                controller.AnimationEvent("AnimationEnd");
                break;
            case EAnimatorState.Intercept:
            case EAnimatorState.Catch:
            case EAnimatorState.Rebound:
            case EAnimatorState.Block:
                controller.AnimationEvent("CatchEnd");
                break;
            case EAnimatorState.Pick:
                controller.AnimationEvent("PickEnd");
                break;
            case EAnimatorState.Buff:
                controller.AnimationEvent("BuffEnd");
                controller.SkillEvent(skillEvent);
                break;
            case EAnimatorState.MoveDodge:
                controller.AnimationEvent("MoveDodgeEnd");
                break;
        }
    }
}
