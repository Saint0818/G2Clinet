using UnityEngine;
using System.Collections;

public class ShowStateBehaviour : StateMachineBehaviour
{
    public EAnimatorState State;
    private AnimationEvent skillEvent = new AnimationEvent();
    
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        AnimatorBehavior ani = animator.gameObject.GetComponent<AnimatorBehavior>();
        if (ani)
        {
            switch (State)
            {
                case EAnimatorState.Elbow:
                    ani.AnimationEvent("ElbowEnd");
                    break;
                case EAnimatorState.Fall:
                    ani.AnimationEvent("FallEnd");
                    break;
                case EAnimatorState.Show:
                    ani.AnimationEvent("Show");
                    break;
                case EAnimatorState.Pass:
                    ani.AnimationEvent("PassEnd");
                    break;
                case EAnimatorState.Dunk:
                case EAnimatorState.Shoot:
                case EAnimatorState.Layup:
                case EAnimatorState.KnockDown:
                case EAnimatorState.Steal:
                case EAnimatorState.Push:
                case EAnimatorState.GotSteal:
                case EAnimatorState.JumpBall:
                    ani.AnimationEvent("AnimationEnd");
                    break;
                case EAnimatorState.Intercept:
                case EAnimatorState.Catch:
                case EAnimatorState.Rebound:
                case EAnimatorState.Block:
                    ani.AnimationEvent("CatchEnd");
                    break;
                case EAnimatorState.Pick:
                    ani.AnimationEvent("PickEnd");
                    break;
                case EAnimatorState.Buff:
                    ani.AnimationEvent("PickEnd");
                    ani.SkillEvent(skillEvent);
                    break;
                case EAnimatorState.MoveDodge:
                    ani.AnimationEvent("MoveDodgeEnd");
                    break;
            }
        }
//        else
//        {
//            Debug.LogError("player == null");
//        }

        //Debug.Log (animator.gameObject.name + " .OnStateMachineExit : " + State.ToString());
    }
}
