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
			case EAnimatorState.Buff:
				if(StateNo == 20 || StateNo == 21) 
					player.StartActiveCamera();
				break;
			case EAnimatorState.Catch:
				break;
			case EAnimatorState.Defence:
				break;
			case EAnimatorState.Dribble:
				break;
			case EAnimatorState.Dunk:
				if(StateNo == 20 || StateNo == 22)
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
				if(StateNo == 20)
					player.StartActiveCamera();
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
				if(StateNo == 20)
					player.StartActiveCamera();
				break;
			case EAnimatorState.Show:
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

		//Block, Dunk, GotSteal, Shoot, Steal, TipIn, Layup, KnockDown, Push, Fall, Show, Pass, Intercept, MoveDodge, Elbow, Rebound, Catch, FakeShoot
		if(player){
			player.AnimationEnd(state);
			Debug.LogWarning(player.name + ".state : " + state.ToString() + " AnimationEnd");
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
