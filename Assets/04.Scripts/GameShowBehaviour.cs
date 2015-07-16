using UnityEngine;
using System.Collections;

public class GameShowBehaviour : StateMachineBehaviour {
	
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

	}
	
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!GameController.Get.IsStart && GameController.Get.Situation != EGameSituation.JumpBall) {
			CourtMgr.Get.ShowEnd ();
		}
	}
	
	override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
	}
	
	override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
	{
	}
	
	override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo ,int stateMachinePathHash)
	{

	}
	
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

	}
}
