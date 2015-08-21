
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// How to use:
    /// <list type="number">
    /// <item> new instance. </item>
    /// <item> call Update() in every frame. </item>
    /// <item> call AddState(). </item>
    /// <item> call ChangeState() in setup state machine. </item>
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
        private readonly MessageDispatcher<TEnumMsg> mDispatcher;

        private readonly Dictionary<TEnumState, State<TEnumState, TEnumMsg>> mStates = new Dictionary<TEnumState, State<TEnumState, TEnumMsg>>();

        public StateMachine(MessageDispatcher<TEnumMsg> dispatcher)
        {
            mDispatcher = dispatcher;
        }

        public bool AddState(State<TEnumState, TEnumMsg> state)
        {
            if(mStates.ContainsKey(state.ID))
            {
                Debug.LogWarningFormat("State({0}) already exist!", state.ID);
                return false;
            }

            mStates.Add(state.ID, state);
            return true;
        }

        public void Update()
        {
            if(mGlobalState != null)
                mGlobalState.Update();

            mCurrentState.Update();
        }

        public void ChangeState(TEnumState newState, object extraInfo = null)
        {
            if(!typeof(TEnumState).IsEnum)
                throw new ArgumentException("TEnum must be an enum.");

            if(!mStates.ContainsKey(newState))
            {
                Debug.LogErrorFormat("State({0}) instance don't exist!", newState);
                return;
            }

            if(mCurrentState != null)
                mCurrentState.Exit();
            mCurrentState = mStates[newState];
            mCurrentState.Enter(this, mDispatcher, extraInfo);
        }

        public void SetGlobalState(State<TEnumState, TEnumMsg> state)
        {
            mGlobalState = state;
        }
    } // end of the class StateMachine.
} // end of the namespace AI.
