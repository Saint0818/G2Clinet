
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
    public class StateMachine<TEnumState, TEnumMsg> 
        where TEnumState : struct, IConvertible, IComparable, IFormattable
        where TEnumMsg : struct, IConvertible, IComparable, IFormattable
    {
        public MessageDispatcher<TEnumMsg> Dispatcher
        {
            get { return mDispatcher; }
        }

        private State<TEnumState, TEnumMsg> mGlobalState;
        private State<TEnumState, TEnumMsg> mCurrentState;
        private readonly IStateMachineFactory<TEnumState, TEnumMsg> mFactory;
        private readonly MessageDispatcher<TEnumMsg> mDispatcher;

        public StateMachine(IStateMachineFactory<TEnumState, TEnumMsg> factory, 
                            MessageDispatcher<TEnumMsg> dispatcher, 
                            TEnumState initState)
        {
            mFactory = factory;
            mDispatcher = dispatcher;
            mCurrentState = mFactory.CreateState(initState);
        }

        public StateMachine(IStateMachineFactory<TEnumState, TEnumMsg> factory, 
                            MessageDispatcher<TEnumMsg> dispatcher,
                            TEnumState initState, State<TEnumState, TEnumMsg> globalState)
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

        public void ChangeState(TEnumState newState)
        {
            ChangeState(newState, null);
        }

        public void ChangeState(TEnumState newState, Object extraInfo)
        {
            if(!typeof(TEnumState).IsEnum)
                throw new ArgumentException("TEnum must be an enum.");

            mCurrentState.Exit();
            mCurrentState = mFactory.CreateState(newState);
            mCurrentState.Enter(this, mDispatcher, extraInfo);
        }

        public void SetGlobalState(State<TEnumState, TEnumMsg> state)
        {
            mGlobalState = state;
        }
    } // end of the class StateMachine.
} // end of the namespace AI.
