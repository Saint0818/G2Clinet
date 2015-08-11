

namespace AI
{
    /// <summary>
    /// StateMachine 中的某一個狀態.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// 呼叫時機: FSM 改變狀態時, 表示此是新的狀態.
        /// </summary>
        void Enter();

        /// <summary>
        /// 呼叫時機: 每個 frame.
        /// </summary>
        void Update();

        /// <summary>
        /// 呼叫時機: FSM 改變狀態時, 表示此狀態要結束.
        /// </summary>
        void Exit();

//        bool OnMessage(Telegram telegram);
    }
}

