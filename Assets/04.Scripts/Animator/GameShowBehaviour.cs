using UnityEngine;

public class GameShowBehaviour : StateMachineBehaviour
{
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GameController.Get.ChangeSituation(EGameSituation.Presentation);
        AIController.Get.ChangeState(EGameSituation.Presentation);
	}
	
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!GameController.Get.IsStart && GameController.Get.Situation != EGameSituation.JumpBall)
        {
			GameController.Get.ChangeSituation(EGameSituation.CameraMovement);
            AIController.Get.ChangeState(EGameSituation.CameraMovement);
		}
	}
	
//	override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
//	{
//	}
//	
//	override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
//	{
//	}
//	
//	override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo ,int stateMachinePathHash)
//	{
//
//	}
//	
//	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//
//	}
}
