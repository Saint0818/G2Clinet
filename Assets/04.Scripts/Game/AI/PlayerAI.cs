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
    private StateMachine<EPlayerAIState, EGameMsg> mFSM;
        
    [UsedImplicitly]
	private void Awake()
    {
	    mFSM = new StateMachine<EPlayerAIState, EGameMsg>();
        mFSM.AddState(new PlayerNoneState());
        mFSM.AddState(new PlayerAttackState(GetComponent<PlayerBehaviour>()));
        mFSM.AddState(new PlayerDefenseState(GetComponent<PlayerBehaviour>()));
        mFSM.ChangeState(EPlayerAIState.None);

        GameMsgDispatcher.Ins.AddListener(mFSM, EGameMsg.CoachOrderAttackTactical);
    }

    [UsedImplicitly]
    private void FixedUpdate()
    {
	    mFSM.Update();
	}

    public void ChangeState(EPlayerAIState newState, object extraInfo = null)
    {
        mFSM.ChangeState(newState, extraInfo);
    }
}
