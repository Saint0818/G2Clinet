using UnityEngine;
using System.Collections;

public class PlayerStateMachineBehaviour : StateMachineBehaviour {
	public EAnimatorState state = EAnimatorState.Idle;
	public bool IsLoop = false;
	private bool isStart = false;
	private PlayerBehaviour player;
	private int StateNo = 0;

	override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		IsLoop = AnimatorMgr.Get.IsLoopState(state);

		if(player == null)
			player = animator.gameObject.GetComponent<PlayerBehaviour> ();

		if (IsLoop)
			return;

		StateNo = animator.GetInteger("StateNo");

		switch (state) {
			case EAnimatorState.Block:
				break;
			case EAnimatorState.Catch:
				break;
			case EAnimatorState.Defence:
				break;
			case EAnimatorState.Dribble:
				break;
			case EAnimatorState.Dunk:
				break;
			case EAnimatorState.Elbow:
				break;
			case EAnimatorState.Fall:
				break;
			case EAnimatorState.FakeShoot:
				break;
			case EAnimatorState.GotSteal:
				break;
			case EAnimatorState.HoldBall:
				break;
			case EAnimatorState.Idle:
				break;
			case EAnimatorState.Intercept:
				break;
			case EAnimatorState.Layup:
				break;
			case EAnimatorState.MoveDodge:
				break;
			case EAnimatorState.Push:
				break;
			case EAnimatorState.Pick:
				break;
			case EAnimatorState.Pass:
				break;
			case EAnimatorState.Rebound:
				break;
			case EAnimatorState.Run:
				break;
			case EAnimatorState.Shoot:
			case EAnimatorState.TipIn:
				break;
			case EAnimatorState.Steal:
				break;
		}

//		Debug.Log (animator.name.ToString() + ".OnStateMachineEnter");
	}

//	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//	}

//	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//	}
	
	override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
	{
		if(IsLoop)
			return;

		if(player)
			Debug.LogWarning(player.name.ToString() + ".state : " + state.ToString() + " OnStateMachineExit");

		switch (state) {
			case EAnimatorState.Catch:
			case EAnimatorState.Defence:
			case EAnimatorState.Dribble:
			case EAnimatorState.Elbow:
			case EAnimatorState.FakeShoot:
			case EAnimatorState.HoldBall:
			case EAnimatorState.Idle:
			case EAnimatorState.Intercept:
			case EAnimatorState.MoveDodge:
			case EAnimatorState.Pick:
			case EAnimatorState.Pass:
			case EAnimatorState.Rebound:
			case EAnimatorState.Run:
				break;

			case EAnimatorState.Block:
			case EAnimatorState.Dunk:
			case EAnimatorState.GotSteal:
			case EAnimatorState.Shoot:
			case EAnimatorState.Steal:
			case EAnimatorState.TipIn:
			case EAnimatorState.Layup:
			case EAnimatorState.KnockDown:
			case EAnimatorState.Push:
			case EAnimatorState.Fall:
				if(player){
					player.AnimationEnd();
					Debug.LogWarning(player.name + ".state : " + state.ToString() + " AnimationEnd");
				}
				break;
		}
	}

//	override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo ,int stateMachinePathHash)
//	{
//			
//	}

//	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//		Debug.Log("On Attack Move ");
//	}
}
