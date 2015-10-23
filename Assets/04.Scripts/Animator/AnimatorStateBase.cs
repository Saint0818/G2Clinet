using UnityEngine;
using System.Collections;
//using AnimatorStateMachineUtil;

public class AnimatorStateBase : AnimatorStateMachineUtil 
{
//	[StateMachineEnter("BaseState.ShootState")]
//	public void ShootEnter(){
//		Debug.Log ("ShootState Enter");
//	}

	[StateUpdateMethod("Layer0.Idle")]
	public void UpdateOnIdle()
	{
		Debug.Log ("IdleUpdate");
	}

	[StateEnterMethod("Layer0.Idle")]
	public void EnteredIdle()
	{
		Debug.Log ("IdleEnter");
	}

	[StateUpdateMethod("Layer0.Idle")]
	public void Drrible()
	{

	}


}
