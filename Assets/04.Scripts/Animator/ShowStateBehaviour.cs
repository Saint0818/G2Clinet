using UnityEngine;
using System.Collections;

public class ShowStateBehaviour : StateMachineBehaviour
{
	public EAnimatorState State;
    private PlayerBehaviour player;

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
		//Debug.Log ("OnStateMachineEnter : " + State.ToString());
    }

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
				player.AnimationEvent ("ElbowEnd");
				break;
			case EAnimatorState.Fall:
				player.AnimationEvent ("AnimationEnd");
				break;
			case EAnimatorState.Show:
				if (player.IsBallOwner)
					player.AniState (EPlayerState.HoldBall);
				else
					player.AniState (EPlayerState.Idle);
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
