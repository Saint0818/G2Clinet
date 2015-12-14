
using JetBrains.Annotations;

namespace AI
{
    /// <summary>
    /// 每一個 Action 對應到某個 AI 行為.
    /// </summary>
    public abstract class PlayerAIAction
    {
        public PlayerAI PlayerAI { get; private set; }
        public PlayerBehaviour Player { get; private set; }

        /// <summary>
        /// 此行為發生的機率.
        /// </summary>
        public abstract float Probability { get; }

        /// <summary>
        /// 是否該 Action 執行完畢.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsDone { get; }

        /// <summary>
        /// 該行為是否真的可以執行. 比如目前是投籃中, 是不能做轉身運球動作. 假如假如是 StealAction,
        /// 意思就是目前可以執行 Steal Animation 嗎?
        /// </summary>
        /// <returns></returns>
        public abstract bool IsValid();

        protected PlayerAIAction([CanBeNull]PlayerAI playerAI, [CanBeNull] PlayerBehaviour player)
        {
            PlayerAI = playerAI;
            Player = player;
        }

        /// <summary>
        /// 真的做撥動作的行為.
        /// </summary>
        public abstract void Do();
    }
}


