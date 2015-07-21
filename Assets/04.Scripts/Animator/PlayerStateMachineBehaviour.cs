using UnityEngine;
using System.Collections;

public class PlayerStateMachineBehaviour : StateMachineBehaviour {

	public EAnimatorState state = EAnimatorState.Idle;
	public float currentTime;
	private float checkTime = 20f;
	public bool isOnce = false;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		currentTime = Time.time;
		isOnce = GameController.Get.IsOnceAnimation (state);
//		Debug.Log (state.ToString() + ".OnStateEnter");
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		currentTime = Time.time;
//		Debug.Log (state.ToString() + ".OnStateExit");
	}

	override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		currentTime = Time.time;
//		Debug.Log (state.ToString() + ".OnStateMachineEnter");
	}

	override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
	{
		currentTime = Time.time;
//		Debug.Log (state.ToString() + ".OnStateMachineExit");
	}

	override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo ,int stateMachinePathHash)
	{
		if(!GameController.Get.IsStart)
			currentTime = Time.time;
		else{
			if (isOnce && Time.time - currentTime > checkTime)
				Debug.LogError ("Animator Stuck : " + "Player : " + animator.gameObject.name + " .State : " + state.ToString());
		}
	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
//		Debug.Log("On Attack Move ");
	}
}
