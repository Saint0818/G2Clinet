
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    /// <item> call AddState() and ChangeState() in setup state machine. </item>
    /// <item> call MessageDispatcher.AddListener to receive message. </item>
    /// <item> (Optional) Call SetGlobalState. </item>
    /// <item> (Optional) 用 indexer property 取出 State. </item>
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

        private readonly Dictionary<TEnumState, State<TEnumState, TEnumMsg>> mStates = new Dictionary<TEnumState, State<TEnumState, TEnumMsg>>();

        /// <summary>
        /// 下一次更新 AI 邏輯的時間. 單位:秒.
        /// </summary>
        private float mNextUpdateAITime;

        /// <summary>
        /// 幾秒更新一次.
        /// </summary>
        private readonly float mUpdateInterval;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateInterval"> 1 秒更新 5 次. </param>
        public StateMachine(float updateInterval = 1/5f)
        {
            mUpdateInterval = updateInterval;
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

        [CanBeNull]
        public State<TEnumState, TEnumMsg> this[TEnumState index]
        {
            get
            {
                if(mStates.ContainsKey(index))
                    return mStates[index];
                return null;
            }
        }

        [CanBeNull]
        public State<TEnumState, TEnumMsg> CurrentState { get; private set; }

        /// <summary>
        /// 每個 Frame 都要呼叫一次.
        /// </summary>
        public void Update()
        {
            if(Time.time >= mNextUpdateAITime)
            {
                mNextUpdateAITime = Time.time + mUpdateInterval;

                if(mGlobalState != null)
                    mGlobalState.UpdateAI();

                if(CurrentState != null)
                    CurrentState.UpdateAI();
            }

            if(mGlobalState != null)
                mGlobalState.Update();

            if(CurrentState != null)
                CurrentState.Update();
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

            if(CurrentState != null)
                CurrentState.Exit();
            CurrentState = mStates[newState];
            CurrentState._Enter(this, extraInfo);
        }

        public void SetGlobalState(State<TEnumState, TEnumMsg> state)
        {
            mGlobalState = state;
        }

        public void HandleMessage(Telegram<TEnumMsg> msg)
        {
            if(CurrentState != null)
                CurrentState.HandleMessage(msg);
        }
    } // end of the class StateMachine.
} // end of the namespace AI.
