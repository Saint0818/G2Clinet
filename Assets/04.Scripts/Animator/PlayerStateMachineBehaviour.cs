using UnityEngine;
using System.Collections;

public class PlayerStateMachineBehaviour : StateMachineBehaviour {
	#if UNITY_EDITOR
	public EAnimatorState state = EAnimatorState.Idle;
	public float currentTime;
	private float checkTime = 10f;
	public bool isOnce = false;
	private PlayerBehaviour pb;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (pb == null)
			pb = animator.gameObject.GetComponent<PlayerBehaviour> ();

		currentTime = Time.time;
		isOnce = GameController.Get.IsOnceAnimation (state);
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
			if (pb && isOnce && Time.time - currentTime > checkTime && !pb.IsLoopState)
			{
				if(GameStart.Get.IsDebugAnimation)
				{
					string message = "Animator Stuck : Player  " + animator.gameObject.name + " .State : " + pb.crtState.ToString() + "GameCount : " + GameController.Get.PlayCount.ToString();
					Debug.LogError (message);
					MailMgr.Get.BugReport(message);
				}
				else
				{
					//Mistake-proofing
					if(pb.IsBallOwner)
						GameController.Get.SetBall();
					
					pb.AniState(EPlayerState.Idle);
				}
			}
				
		}
	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
//		Debug.Log("On Attack Move ");
	}
	#endif
}
