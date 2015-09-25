﻿using AI;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// <para> 這就像是一個神物件, 對遊戲中的全部球員下達命令. 比如要球員執行什麼戰術, 並根據情況命令球員要做什麼動作
/// (傳球, 投籃等等). </para>
/// <para> It's like a god of the game. For example: AIController order all players to execute tactics
/// under the circumstances(passing, shooting, etc.). </para>
/// <para></para>
/// </summary>
/// <remarks>
/// How to add State:
/// <list type="number">
/// <item> inherit AI.State. </item>
/// <item> call StateMachine.AddState() in setup StateMachine. </item>
/// </list>
/// 
/// <para> 預期未來這個類別的責任會寫到 Team, 各 Team 對自己的球員下達命令. </para>
/// </remarks>
[DisallowMultipleComponent]
public class AIController : KnightSingleton<AIController>, ITelegraph<EGameMsg>
{
    private StateMachine<EGameSituation, EGameMsg> mFSM;

    public Team PlayerTeam
    {
        get { return mPlayerTeam; }
    }

    public Team NpcTeam
    {
        get { return mNpcTeam; }
    }

    private readonly Team mPlayerTeam = new Team(ETeamKind.Self);
    private readonly Team mNpcTeam = new Team(ETeamKind.Npc);

    [UsedImplicitly]
    private void Awake()
    {
        mFSM = new StateMachine<EGameSituation, EGameMsg>();
        mFSM.AddState(new NoneState());
        mFSM.AddState(new PresentationState());
        mFSM.AddState(new CameraMovementState());
        mFSM.AddState(new InitCourtState());
        mFSM.AddState(new OpeningState());
        mFSM.AddState(new JumpBallState());
        mFSM.AddState(new AttackAState());
        mFSM.AddState(new AttackBState());
        mFSM.AddState(new APickBallAfterScoreState());
        mFSM.AddState(new InboundsAState());
        mFSM.AddState(new BPickBallAfterScoreState());
        mFSM.AddState(new InboundsBState());
        mFSM.AddState(new SpecialActionState());
        mFSM.AddState(new EndState());
        mFSM.ChangeState(EGameSituation.None);

        GameMsgDispatcher.Ins.AddListener(mFSM, EGameMsg.UISkipClickOnGaming);
        GameMsgDispatcher.Ins.AddListener(this, EGameMsg.GamePlayersCreated);
    }

    public void HandleMessage(Telegram<EGameMsg> e)
    {
        if(e.Msg == EGameMsg.GamePlayersCreated)
        {
            // 這段是整個 AI 的框架的初始化過程.
            mPlayerTeam.Clear();
            mNpcTeam.Clear();

            PlayerBehaviour[] players = (PlayerBehaviour[])e.ExtraInfo;
            foreach(var player in players)
            {
                if(player.Team == ETeamKind.Self)
                {
                    mPlayerTeam.AddPlayer(player.GetComponent<PlayerAI>());
                    mNpcTeam.AddOpponentPlayer(player.GetComponent<PlayerAI>());
                }
                else if(player.Team == ETeamKind.Npc)
                {
                    mPlayerTeam.AddOpponentPlayer(player.GetComponent<PlayerAI>());
                    mNpcTeam.AddPlayer(player.GetComponent<PlayerAI>());
                }
            }
        }
    }

    [UsedImplicitly]
    private void FixedUpdate()
    {
        mFSM.Update();

        // 暫時在 AIController 做更新, 往後如果有更適當的地方再改變.
        GameMsgDispatcher.Ins.Update();
    }

    public void ChangeState(EGameSituation newState)
    {
        mFSM.ChangeState(newState);
    }

    public void ChangeState(EGameSituation newState, object extraInfo)
    {
        mFSM.ChangeState(newState, extraInfo);
    }
}
