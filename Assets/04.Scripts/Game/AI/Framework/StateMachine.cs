
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
    /// <item> call MessageDispatcher.AddListener to receive message. </item>
    /// <item> (Optional) Call SetGlobalState. </item>
    /// </list>
    /// 
    /// 設計決策:
    /// <para> StateMachine 實作 ITelegraph 的原因是我不希望每個 State 自己向 MessageDispatcher 註冊事件.
    /// 如果每個 State 可以向 MessageDispatcher 註冊事件, 那麼就可能(比如程式寫錯)某個 State 並不是 current state 時,
    /// 結果還是可以接收到事件. 所以我改為 StateMachine 去註冊事件來避免這個問題. </para>
    /// 
    /// <para> Global State 主要是要避免 State 處理 Message 時的重複程式碼. </para>
    /// </remarks>
    /// 
    /// <para> where TEnum : struct, IConvertible, IComparable, IFormattable 是限制 TEnum 必須是 Enum. </para>
    public class StateMachine<TEnumState, TEnumMsg> : ITelegraph<TEnumMsg> 
        where TEnumState : struct, IConvertible, IComparable, IFormattable
        where TEnumMsg : struct, IConvertible, IComparable, IFormattable
    {
        private State<TEnumState, TEnumMsg> mGlobalState;
        private State<TEnumState, TEnumMsg> mCurrentState;

        private readonly Dictionary<TEnumState, State<TEnumState, TEnumMsg>> mStates = new Dictionary<TEnumState, State<TEnumState, TEnumMsg>>();

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
            if (mGlobalState != null)
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
            mCurrentState._Enter(this, extraInfo);
        }

        public void SetGlobalState(State<TEnumState, TEnumMsg> state)
        {
            mGlobalState = state;
        }

        public void HandleMessage(Telegram<TEnumMsg> msg)
        {
            if(mCurrentState != null)
                mCurrentState.HandleMessage(msg);
        }
    } // end of the class StateMachine.
} // end of the namespace AI.
