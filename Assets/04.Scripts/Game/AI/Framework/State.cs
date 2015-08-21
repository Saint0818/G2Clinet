using System;

namespace AI
{
    /// <summary>
    /// StateMachine 中的某一個狀態.
    /// </summary>
    public abstract class State<TEnumState, TEnumMsg> 
        where TEnumState : struct, IConvertible, IComparable, IFormattable
        where TEnumMsg : struct, IConvertible, IComparable, IFormattable
    {
        public StateMachine<TEnumState, TEnumMsg> Parent { get; private set; }
        public MessageDispatcher<TEnumMsg> Dispatcher { get; private set; }

        public abstract TEnumState ID { get; }

        /// <summary>
        /// 呼叫時機: FSM 改變狀態時, 表示此是新的狀態.
        /// </summary>
        public void Enter(StateMachine<TEnumState, TEnumMsg> machine, MessageDispatcher<TEnumMsg> dispatcher, 
                          Object extraInfo)
        {
            Parent = machine;
            Dispatcher = dispatcher;

            EnterImpl(extraInfo);
        }

        public abstract void EnterImpl(object extraInfo);

        /// <summary>
        /// 呼叫時機: FSM 改變狀態時, 表示此狀態要結束.
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// 呼叫時機: 每個 frame.
        /// </summary>
        public abstract void Update();
    }
}

