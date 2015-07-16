using UnityEngine;
using System.Collections;

public class PlayerStateMachineBehaviour : StateMachineBehaviour {

	public EPlayerState state = EPlayerState.Idle;
	public float currentTime;
	public float checkTime = 5f;
	public bool isOnce = false;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
//		Debug.Log (state.ToString() + ".OnStateEnter");
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
//		Debug.Log (state.ToString() + ".OnStateExit");
	}

	override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		isOnce = GameController.Get.IsOnceAnimation (state);

		if(isOnce){
			currentTime = Time.time;
		}

//		Debug.Log (state.ToString() + ".OnStateMachineEnter");
	}

	override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
	{
//		Debug.Log (state.ToString() + ".OnStateMachineExit");
	}

	override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo ,int stateMachinePathHash)
	{
		if (isOnce && Time.time - currentTime > checkTime)
			Debug.LogError ("Animator Stuck : ");
	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
//		Debug.Log("On Attack Move ");
	}
}
