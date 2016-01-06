using System.Collections.Generic;
using AI;
using JetBrains.Annotations;
using UnityEngine;
using GameEnum;

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
    // todo 這個是暫時的作法, 以後要記得拿掉. 這主要是要解決換場時, 被 AI 控管的時間不要被計算.
    // todo 如果沒有這樣做, 就可能換場時, 球員就被 AI 控制, 然後就馬上發動主動技.
    public float AIRemainTime { get; set; }

    /// <summary>
    /// 玩家選擇的進攻戰術.
    /// </summary>
    public ETacticalAuto PlayerAttackTactical { get; set; }

    private StateMachine<EGameSituation, EGameMsg> mFSM;

    /// <summary>
    /// 防守時的 Home Position.
    /// </summary>
    private readonly Dictionary<ETeamKind, Dictionary<EPlayerPostion, Vector2>> mHomePositions = new Dictionary<ETeamKind, Dictionary<EPlayerPostion, Vector2>>
    {
        {
            ETeamKind.Npc, new Dictionary<EPlayerPostion, Vector2>
            {
                {EPlayerPostion.G, new Vector2(-5.3f, 11)},
                {EPlayerPostion.F, new Vector2(5.3f, 11)},
                {EPlayerPostion.C, new Vector2(0, 14.5f)}
            }
        
        },

        {
            ETeamKind.Self, new Dictionary<EPlayerPostion, Vector2>
            {
                {EPlayerPostion.G, new Vector2(-5.3f, -11)},
                {EPlayerPostion.F, new Vector2(5.3f, -11)},
                {EPlayerPostion.C, new Vector2(0, -14.5f)}
            }
        }
    };

    private readonly Dictionary<ETeamKind, Team> mTeams = new Dictionary<ETeamKind, Team>();

    [UsedImplicitly]
    private void Awake()
    {
        mTeams.Add(ETeamKind.Self, new Team(ETeamKind.Self, mHomePositions[ETeamKind.Self]));
        mTeams.Add(ETeamKind.Npc, new Team(ETeamKind.Npc, mHomePositions[ETeamKind.Npc]));

        mFSM = new StateMachine<EGameSituation, EGameMsg>();
        mFSM.AddState(new NoneState());
        mFSM.AddState(new PresentationState());
        mFSM.AddState(new CameraMovementState());
        mFSM.AddState(new InitCourtState());
        mFSM.AddState(new OpeningState());
        mFSM.AddState(new JumpBallState());
        mFSM.AddState(new AttackGamerState());
        mFSM.AddState(new AttackNPCState());
        mFSM.AddState(new GamerPickBallState());
        mFSM.AddState(new InboundsGamerState());
        mFSM.AddState(new NPCPickBallState());
        mFSM.AddState(new InboundsNPCState());
        mFSM.AddState(new SpecialActionState());
        mFSM.AddState(new EndState());
        mFSM.ChangeState(EGameSituation.None);

        GameMsgDispatcher.Ins.AddListener(mFSM, EGameMsg.UISkipClickOnGaming);
        GameMsgDispatcher.Ins.AddListener(mFSM, EGameMsg.PlayerTouchBallWhenJumpBall);
        GameMsgDispatcher.Ins.AddListener(mFSM, EGameMsg.PlayerTacticalDone);
        GameMsgDispatcher.Ins.AddListener(this, EGameMsg.GamePlayersCreated);
    }

    public void HandleMessage(Telegram<EGameMsg> e)
    {
        if(e.Msg == EGameMsg.GamePlayersCreated)
        {
            // 這段是整個 AI 的框架的初始化過程.
            mTeams[ETeamKind.Self].Clear();
            mTeams[ETeamKind.Npc].Clear();

            PlayerBehaviour[] players = (PlayerBehaviour[])e.ExtraInfo;
            foreach(var player in players)
            {
                if(player.Team == ETeamKind.Self)
                {
                    mTeams[ETeamKind.Self].AddPlayer(player.GetComponent<PlayerAI>());
                    mTeams[ETeamKind.Npc].AddOpponentPlayer(player.GetComponent<PlayerAI>());
                }
                else if(player.Team == ETeamKind.Npc)
                {
                    mTeams[ETeamKind.Self].AddOpponentPlayer(player.GetComponent<PlayerAI>());
                    mTeams[ETeamKind.Npc].AddPlayer(player.GetComponent<PlayerAI>());
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

    [UsedImplicitly]
    private void OnGUI()
    {
        if(AIDebugger.Get.ShowStatus)
        {
            GUI.Label(new Rect(100, 100, 600, 100), string.Format("GameController:{0}", GameController.Get.Situation));
            GUI.Label(new Rect(100, 130, 600, 100), string.Format("{0}", mTeams[ETeamKind.Self]));
            GUI.Label(new Rect(100, 160, 600, 100), string.Format("{0}", mTeams[ETeamKind.Npc]));
        }
    }

    public void ChangeState(EGameSituation newState)
    {
        mFSM.ChangeState(newState);
    }

    public void ChangeState(EGameSituation newState, object extraInfo)
    {
        mFSM.ChangeState(newState, extraInfo);
    }

    public Team GetTeam(ETeamKind team)
    {
        return mTeams[team];
    }
}
