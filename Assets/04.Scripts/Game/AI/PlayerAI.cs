using AI;
using JetBrains.Annotations;
using UnityEngine;

public enum EPlayerAIState
{
    None,
    Attack,
    Defense
}

public class PlayerAI : MonoBehaviour
{
    private StateMachine<EPlayerAIState> mMachine;
        
    [UsedImplicitly]
	private void Awake()
    {
	    mMachine = new StateMachine<EPlayerAIState>();
        mMachine.AddState(new PlayerNoneState());
        mMachine.AddState(new PlayerAttackState());
        mMachine.AddState(new PlayerDefenseState());
        mMachine.ChangeState(EPlayerAIState.None);
    }

    [UsedImplicitly]
    private void FixedUpdate()
    {
	    mMachine.Update();
	}
}
