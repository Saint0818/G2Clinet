using UnityEngine;
using System.Collections;

public class ShowStateBehaviour : StateMachineBehaviour
{
    private PlayerBehaviour player;

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (player == null)
            player = animator.gameObject.GetComponent<PlayerBehaviour>();

//		Debug.Log ("Showin");
    }

//  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//  {
//  }

//  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//  {
//  }
    
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
		if (player != null) {
			if(player.IsBallOwner)
				player.AniState(EPlayerState.HoldBall);
			else
				player.AniState(EPlayerState.Idle);
		}

//		Debug.Log ("ShowOut");
    }

//  override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo ,int stateMachinePathHash)
//  {
//          
//  }

//  override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//  {
//  }
}
