using System;

namespace AI
{
    /// <summary>
    /// <para> StateMachine 中的某一個狀態. </para>
    /// <para> 特別注意: State 不要實作 ITelegraph, 是 StateMachine 才要實作. 這麼做的原因是
    /// 1 個 StateMachine 只會有 1 個 State 是運作中, 如果 State 實作 ITelegraph, 那麼 State 即使不是
    /// 運作中, 也可能會收到訊息, 這會造成無法預期的錯誤. </para>
    /// </summary>
    public abstract class State<TEnumState, TEnumMsg> 
        where TEnumState : struct, IConvertible, IComparable, IFormattable
        where TEnumMsg : struct, IConvertible, IComparable, IFormattable
    {
        public StateMachine<TEnumState, TEnumMsg> Parent { get; private set; }

        public abstract TEnumState ID { get; }

        /// <summary>
        /// 呼叫時機: FSM 改變狀態時, 表示此是新的狀態.
        /// </summary>
        public void _Enter(StateMachine<TEnumState, TEnumMsg> machine, Object extraInfo)
        {
            Parent = machine;

            Enter(extraInfo);
        }

        public abstract void Enter(object extraInfo);

        /// <summary>
        /// 呼叫時機: FSM 改變狀態時, 表示此狀態要結束.
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// 呼叫時機: 每經過 StateMachine.updateInterval 秒後, 才會呼叫一次.
        /// 假如 lag 的時間是 StateMachine.updateInterval 的 n 倍, 也只會呼叫一次, 不會呼叫多次.
        /// </summary>
        public abstract void UpdateAI();

        /// <summary>
        /// 呼叫時機: 每個 frame.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// 呼叫時機: 收到訊息.
        /// </summary>
        /// <param name="msg"></param>
        public abstract void HandleMessage(Telegram<TEnumMsg> msg);
    }
}

