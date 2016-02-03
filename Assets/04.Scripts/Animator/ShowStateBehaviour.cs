using UnityEngine;
using System.Collections;

public class ShowStateBehaviour : StateMachineBehaviour
{
    public EAnimatorState State;
    private PlayerBehaviour player;
    private AnimationEvent skillEvent = new AnimationEvent();
    
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (animator.gameObject)
        {
            switch (State)
            {
                case EAnimatorState.Elbow:
                    animator.gameObject.SendMessage("AnimationEvent", "ElbowEnd");
                    break;
                case EAnimatorState.Fall:
                    animator.gameObject.SendMessage("AnimationEvent", "FallEnd");
                    break;
                case EAnimatorState.Show:
                    if (player.IsBallOwner)
                        player.AniState(EPlayerState.HoldBall);
                    else
                        player.AniState(EPlayerState.Idle);
                    break;
                case EAnimatorState.Pass:
                    animator.gameObject.SendMessage("AnimationEvent", "PassEnd");
                    break;
                case EAnimatorState.Dunk:
                case EAnimatorState.Shoot:
                case EAnimatorState.Layup:
                case EAnimatorState.KnockDown:
                case EAnimatorState.Steal:
                case EAnimatorState.Push:
                case EAnimatorState.GotSteal:
                case EAnimatorState.JumpBall:
                    animator.gameObject.SendMessage("AnimationEvent", "AnimationEnd");
                    break;
                case EAnimatorState.Intercept:
                case EAnimatorState.Catch:
                case EAnimatorState.Rebound:
                case EAnimatorState.Block:
                    animator.gameObject.SendMessage("AnimationEvent", "CatchEnd");
                    break;
                case EAnimatorState.Pick:
                    animator.gameObject.SendMessage("AnimationEvent", "PickEnd");
                    break;
                case EAnimatorState.Buff:
                    animator.gameObject.SendMessage("AnimationEvent", "PickEnd");
                    animator.gameObject.SendMessage("SkillEvent", skillEvent);
                    break;
                case EAnimatorState.MoveDodge:
                    animator.gameObject.SendMessage("AnimationEvent", "MoveDodgeEnd");
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
