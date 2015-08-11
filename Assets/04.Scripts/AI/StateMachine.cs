
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
        private IState mGlobalState;
        private IState mCurrentState = new NullState();
        private readonly IStateMachineFactory<TEnum> mFactory;

        public StateMachine(IStateMachineFactory<TEnum> factory)
        {
            mFactory = factory;
        }

        public StateMachine(IState globalState, IStateMachineFactory<TEnum> factory)
        {
            mGlobalState = globalState;
            mFactory = factory;
        }

        public void Update()
        {
            if (mGlobalState != null)
                mGlobalState.Update();

            mCurrentState.Update();
        }

        public void ChangeState(TEnum newState)
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum must be an enum.");

            mCurrentState.Exit();
            mCurrentState = mFactory.CreateState(newState);
            mCurrentState.Enter();
        }

        public void SetGlobalState(IState state)
        {
            mGlobalState = state;
        }
    } // end of the class StateMachine.
} // end of the namespace AI.
