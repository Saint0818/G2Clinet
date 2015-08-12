using System;

namespace AI
{
    /// <summary>
    /// StateMachine 中的某一個狀態.
    /// </summary>
    public abstract class State<TEnum> where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        public StateMachine<TEnum> Parent { get; private set; }
        public MessageDispatcher<TEnum> MessageDispatcher { get; private set; }

        /// <summary>
        /// 呼叫時機: FSM 改變狀態時, 表示此是新的狀態.
        /// </summary>
        public void Enter(StateMachine<TEnum> machine, MessageDispatcher<TEnum> dispatcher, Object extraInfo)
        {
            Parent = machine;
            MessageDispatcher = dispatcher;

            EnterImpl(extraInfo);
        }

        public abstract void EnterImpl(object extraInfo);

        /// <summary>
        /// 呼叫時機: 每個 frame.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// 呼叫時機: FSM 改變狀態時, 表示此狀態要結束.
        /// </summary>
        public abstract void Exit();
    }
}

