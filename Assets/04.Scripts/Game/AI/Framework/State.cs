using System;

namespace AI
{
    /// <summary>
    /// StateMachine 中的某一個狀態.
    /// </summary>
    public abstract class State<TEnumState> 
        where TEnumState : struct, IConvertible, IComparable, IFormattable
    {
        public StateMachine<TEnumState> Parent { get; private set; }
//        public MessageDispatcher<TEnumMsg> Dispatcher { get; private set; }

        public abstract TEnumState ID { get; }

        /// <summary>
        /// 呼叫時機: FSM 改變狀態時, 表示此是新的狀態.
        /// </summary>
        public void _Enter(StateMachine<TEnumState> machine, //MessageDispatcher<TEnumMsg> dispatcher, 
                          Object extraInfo)
        {
            Parent = machine;
//            Dispatcher = dispatcher;

            Enter(extraInfo);
        }

        public abstract void Enter(object extraInfo);

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

