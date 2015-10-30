using UnityEngine;
using System.Collections;

public class PlayerStateMachineBehaviour : StateMachineBehaviour
{
    /*public EAnimatorState state = EAnimatorState.Idle;
    public bool IsLoop = false;
    private bool isStart = false;
    private PlayerBehaviour player;
    private int StateNo = 0;

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        IsLoop = AnimatorMgr.Get.IsLoopState(state);

        if (player == null)
            player = animator.gameObject.GetComponent<PlayerBehaviour>();

		player.AnimationStart (state, animator.GetInteger("StateNo"));

        if (IsLoop)
        {
            switch (state)
            {
                case EAnimatorState.Dribble:
                    animator.ResetTrigger(EActionTrigger.ReleaseDefence.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseHoldBall.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseRun.ToString());
                    break;
                case EAnimatorState.Defence:
                    animator.ResetTrigger(EActionTrigger.ReleaseDribble.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseHoldBall.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseRun.ToString());
                    break;
                case EAnimatorState.HoldBall:
                    animator.ResetTrigger(EActionTrigger.ReleaseDribble.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseHoldBall.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseRun.ToString());
                    break;
                case EAnimatorState.Idle:
                    animator.ResetTrigger(EActionTrigger.ReleaseDribble.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseDefence.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseHoldBall.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseRun.ToString());
                    break;
                case EAnimatorState.Run:
                    animator.ResetTrigger(EActionTrigger.ReleaseDribble.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseDefence.ToString());
                    animator.ResetTrigger(EActionTrigger.ReleaseHoldBall.ToString());
                    break;
            }
            return;
        } else
        {
            animator.ResetTrigger(EActionTrigger.ReleaseDribble.ToString());
            animator.ResetTrigger(EActionTrigger.ReleaseDefence.ToString());
            animator.ResetTrigger(EActionTrigger.ReleaseHoldBall.ToString());
            animator.ResetTrigger(EActionTrigger.ReleaseRun.ToString());
        }

        StateNo = animator.GetInteger("StateNo");

        switch (state)
        {
            case EAnimatorState.Block:
                break;
            case EAnimatorState.Buff:
                if (StateNo == 20 || StateNo == 21)
                {
                    player.StartActiveCamera();
                    CameraMgr.Get.CourtCameraAnimator.SetTrigger("CameraAction_0");
                }
                break;
            case EAnimatorState.Catch:
                break;
            case EAnimatorState.Dunk:
                if (StateNo == 20 || StateNo == 22)
                    player.StartActiveCamera();
                break;
            case EAnimatorState.Elbow:
                break;
            case EAnimatorState.Fall:
                break;
            case EAnimatorState.FakeShoot:
                break;
            case EAnimatorState.GotSteal:
                break;
            case EAnimatorState.Intercept:
                break;
            case EAnimatorState.Layup:
                break;
            case EAnimatorState.MoveDodge:
                break;
            case EAnimatorState.Push:
                if (StateNo == 20)
                    player.StartActiveCamera();
                break;
            case EAnimatorState.Pick:
                break;
            case EAnimatorState.Pass:
                break;
            case EAnimatorState.Rebound:
                break;
            case EAnimatorState.Shoot:
            case EAnimatorState.TipIn:
                break;
            case EAnimatorState.Steal:
                if (StateNo == 20)
                    player.StartActiveCamera();
                break;
            case EAnimatorState.Show:
                break;
        }

//      Debug.Log (animator.name.ToString() + ".OnStateMachineEnter");
    }

//  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//  {
//  }

//  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//  {
//  }
    
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (IsLoop)
            return;

        //Block, Dunk, GotSteal, Shoot, Steal, TipIn, Layup, KnockDown, Push, Fall, Show, Pass, Intercept, MoveDodge, Elbow, Rebound, Catch, FakeShoot
        if (player)
        {
			Debug.LogWarning(player.name + ".state : " + state.ToString() + " AnimationEnd");
            player.AnimationEnd(state);
        }
		else
		{
			Debug.LogError("player" + player.name);
		}
    }*/

//  override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo ,int stateMachinePathHash)
//  {
//          
//  }

//  override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//  {
//  }
}
