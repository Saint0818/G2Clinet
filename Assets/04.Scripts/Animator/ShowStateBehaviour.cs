using UnityEngine;
using System.Collections;

public class ShowStateBehaviour : StateMachineBehaviour
{
    public EAnimatorState State;
    private PlayerBehaviour player;
    private AnimationEvent skillEvent = new AnimationEvent();

    //    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //    {
    //Debug.Log ("OnStateMachineEnter : " + State.ToString());
    //    }

    //  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //  {
    //  }

    //  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //  {
    //  }
    
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (player == null)
            player = animator.gameObject.GetComponent<PlayerBehaviour>();

        if (player)
        {
            switch (State)
            {
                case EAnimatorState.Elbow:
                    player.AnimationEvent("ElbowEnd");
                    break;
                case EAnimatorState.Fall:
                    player.AnimationEvent("FallEnd");
                    break;
                case EAnimatorState.Show:
                    if (player.IsBallOwner)
                        player.AniState(EPlayerState.HoldBall);
                    else
                        player.AniState(EPlayerState.Idle);
                    break;
                case EAnimatorState.Pass:
                    player.AnimationEvent("PassEnd");
                    break;
                case EAnimatorState.Dunk:
                case EAnimatorState.Shoot:
                case EAnimatorState.Layup:
                case EAnimatorState.KnockDown:
                case EAnimatorState.Steal:
                case EAnimatorState.Push:
                case EAnimatorState.GotSteal:
                case EAnimatorState.JumpBall:
                    player.AnimationEvent("AnimationEnd");
                    break;
                case EAnimatorState.Intercept:
                case EAnimatorState.Catch:
                case EAnimatorState.Rebound:
                case EAnimatorState.Block:
                    player.AnimationEvent("CatchEnd");
                    break;
                case EAnimatorState.Pick:
                    player.AnimationEvent("PickEnd");
                    break;
                case EAnimatorState.Buff:
                    player.AnimationEvent("BuffEnd");
                    skillEvent.stringParameter = "ActiveSkillEnd";
                    player.SkillEvent(skillEvent);
                    break;
                case EAnimatorState.MoveDodge:
                    player.AnimationEvent("MoveDodgeEnd");
                    break;
            }
        }
        else
        {
            Debug.LogError("player == null");
        }

        //Debug.Log (animator.gameObject.name + " .OnStateMachineExit : " + State.ToString());
    }

    //  override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo ,int stateMachinePathHash)
    //  {
    //
    //  }

    //  override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //  {
    //  }
}
