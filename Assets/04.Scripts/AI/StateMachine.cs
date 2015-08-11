
using System;

namespace AI
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 使用方法:
    /// <list type="number">
    /// <item> new instance. </item>
    /// <item> call update(). </item>
    /// <item> call ChangeState 改變狀態. </item>
    /// <item> (Optional) Call SetGlobalState. </item>
    /// </list>
    /// 
    /// Global State 主要是要避免 State 處理 Message 時的重複程式碼.
    /// </remarks>
    /// 
    /// where TEnum : struct, IConvertible, IComparable, IFormattable 是限制 TEnum 必須是 Enum.
    public class StateMachine<TEnum> where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        private State<TEnum> mGlobalState;
        private State<TEnum> mCurrentState;
        private readonly IStateMachineFactory<TEnum> mFactory;
        private readonly MessageDispatcher<TEnum> mDispatcher;

        public StateMachine(IStateMachineFactory<TEnum> factory, MessageDispatcher<TEnum> dispatcher, 
                            TEnum initState)
        {
            mFactory = factory;
            mDispatcher = dispatcher;
            mCurrentState = mFactory.CreateState(initState);
        }

        public StateMachine(IStateMachineFactory<TEnum> factory, MessageDispatcher<TEnum> dispatcher,
                            TEnum initState, State<TEnum> globalState)
        {
            mFactory = factory;
            mDispatcher = dispatcher;
            mCurrentState = mFactory.CreateState(initState);
            mGlobalState = globalState;
        }

        public void Update()
        {
            if(mGlobalState != null)
                mGlobalState.Update();

            mCurrentState.Update();
        }

        public void ChangeState(TEnum newState)
        {
            if(!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum must be an enum.");

            mCurrentState.Exit();
            mCurrentState = mFactory.CreateState(newState);
            mCurrentState.Enter(this, mDispatcher);
        }

        public void SetGlobalState(State<TEnum> state)
        {
            mGlobalState = state;
        }
    } // end of the class StateMachine.
} // end of the namespace AI.
