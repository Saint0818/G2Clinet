using AI;
using JetBrains.Annotations;
using UnityEngine;

public enum EPlayerAIState
{
    None,
    Attack,
    Defense
}

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerBehaviour))]
public class PlayerAI : MonoBehaviour, ITelegraph<EGameMsg>
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

        GameMsgDispatcher.Ins.AddListener(this, EGameMsg.GamePlayersCreated);
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

    public void HandleMessage(Telegram<EGameMsg> msg)
    {
        if(msg.Msg == EGameMsg.GamePlayersCreated)
        {
//            Debug.Log("Receiving [EGameMsg.GamePlayersCreated]...");

            PlayerBehaviour[] players = (PlayerBehaviour[])msg.ExtraInfo;
            var attack = (PlayerAttackState)(mFSM[EPlayerAIState.Attack]);
            attack.Init(players);

            var defense = (PlayerDefenseState)(mFSM[EPlayerAIState.Defense]);
            defense.Init(players);

            GameMsgDispatcher.Ins.AddListener(mFSM, EGameMsg.CoachOrderAttackTactical);

            GameMsgDispatcher.Ins.RemoveListener(this, EGameMsg.GamePlayersCreated);
        }
    }
}
