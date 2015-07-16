using UnityEngine;
using System.Collections;

public class GameShoweBehaviour : StateMachineBehaviour {
	
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

	}
	
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log ("End : 1");

		if (!GameController.Get.IsStart && GameController.Get.Situation != EGameSituation.JumpBall) {
//			animator.SetBool("Start", false);
			CourtMgr.Get.ShowEnd ();
		}
	}
	
	override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
	}
	
	override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
	{
		Debug.Log ("End : 2");
	}
	
	override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo ,int stateMachinePathHash)
	{

	}
	
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

	}
}
