using JetBrains.Annotations;

namespace AI
{
    public class PlayerDefenseState : State<EPlayerAIState, EGameMsg>
    {
        public override EPlayerAIState ID
        {
            get { return EPlayerAIState.Defense; }
        }

        private readonly PlayerAI mPlayerAI;
        private readonly PlayerBehaviour mPlayer;

        private readonly StartActiveSkillAction mStartActiveSkillAction;

        private readonly ActionRandomizer mActions = new ActionRandomizer();

        public PlayerDefenseState([NotNull]PlayerAI playerAI, [NotNull] PlayerBehaviour player)
        {
            mPlayerAI = playerAI;
            mPlayer = player;

            mStartActiveSkillAction = new StartActiveSkillAction(mPlayer);

            mActions.Add(new CloseDefPlayerAction(playerAI, mPlayer));
            mActions.Add(new PushAction(playerAI, mPlayer));
            mActions.Add(new StealAction(playerAI, mPlayer));
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", ID, mActions);
        }

        public void Init(PlayerBehaviour[] players)
        {
            mStartActiveSkillAction.Init(players, false);
        }

        public override void Enter(object extraInfo)
        {
            mPlayer.ResetMove();
        }

        public override void Exit()
        {
        }

        public override void UpdateAI()
        {
            if(!mPlayer.AIing)
                return;

            if(mStartActiveSkillAction.Do())
                return; // 真的有做主動技, 會結束此次的 AI 判斷.

            doDefenseAction();
        }

        public override void Update()
        {
        }

        private void doDefenseAction()
        {
            if(GameController.Get.IsDunk || GameController.Get.IsShooting)
                return;

            mActions.Do();
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }
    } // end of the class PlayerDefenseState.
} // end of the namespace AI.
