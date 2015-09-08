

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AI
{
    /// <summary>
    /// AI 框架的核心系統, 通過運用 Message 的方式實現事件驅動(event-driven) 的架構.
    /// 假設玩家要攻擊一群正在巡邏的守衛時, 只要玩家攻擊, 就會產生一個事件(這裡稱為 Message),
    /// 然後這群守衛收到玩家攻擊的事件後, 就可以做出對應的事情. 比如反擊(也就是攻擊玩家)等等.
    /// </summary>
    /// <typeparam name="TEnumMsg"> 必須是 Enum. </typeparam>
    /// <remarks>
    /// 使用方法:
    /// <list type="number">
    /// <item> 繼承(Singleton). </item>
    /// <item> call Update in every frame. </item>
    /// <item> call AddListener(), 向系統說明要監聽哪些事件. </item>
    /// <item> call RemoveListener(), ClearListeners() 向系統說明取消監聽哪些事件. </item>
    /// <item> call SendMesssage() 送出訊息. </item>
    /// </list>
    /// 
    /// 實作細節:
    /// <list type="number">
    /// <item> 內部會將使用者的操作都包裝成 Operation, 會這樣設計的原因是避免 InvalidOperationException. 
    /// "collection was modified in a foreach-loop". </item>
    /// </list>
    /// </remarks>
    public class MessageDispatcher<TEnumMsg> where TEnumMsg : struct, IConvertible, IComparable, IFormattable
    {
        private readonly Dictionary<TEnumMsg, List<ITelegraph<TEnumMsg>>> mListeners = new Dictionary<TEnumMsg, List<ITelegraph<TEnumMsg>>>();
//        private Telegram<TEnumMsg> mTelegram;

        private enum EOperation
        {
            AddListener, RemoveListener, ClearMsg, ClearAllMsg, SendMsg
        }
        private struct Operation
        {
            public EOperation Op;
            public TEnumMsg Msg;
            public ITelegraph<TEnumMsg> Listener;
            public Telegram<TEnumMsg> Tel;
        }

        private readonly List<Operation> mOperations = new List<Operation>();

        public void AddListeners(ITelegraph<TEnumMsg> listener, params TEnumMsg[] msgs)
        {
            foreach(TEnumMsg msg in msgs)
            {
                AddListener(listener, msg);
            }
        }

        public void AddListener(ITelegraph<TEnumMsg> listener, TEnumMsg msg)
        {
            mOperations.Add(new Operation {Op = EOperation.AddListener, Listener = listener, Msg = msg});
        }

        private void addListener(ITelegraph<TEnumMsg> listener, TEnumMsg msg)
        {
            if (!mListeners.ContainsKey(msg))
                mListeners.Add(msg, new List<ITelegraph<TEnumMsg>>());

            if (!mListeners[msg].Contains(listener))
                mListeners[msg].Add(listener);
        }

        public void RemoveListener(ITelegraph<TEnumMsg> listener, params TEnumMsg[] msgs)
        {
            foreach(TEnumMsg msg in msgs)
            {
                RemoveListener(listener, msg);
            }
        }

        public void RemoveListener(ITelegraph<TEnumMsg> listener, TEnumMsg msg)
        {
            mOperations.Add(new Operation {Op = EOperation.RemoveListener, Listener = listener, Msg = msg});
        }

        private void removeListener(ITelegraph<TEnumMsg> listener, TEnumMsg msg)
        {
            if(mListeners.ContainsKey(msg))
                mListeners[msg].Remove(listener);
        }

        public void ClearListeners(params TEnumMsg[] msgs)
        {
            foreach(TEnumMsg msg in msgs)
            {
                ClearListeners(msg);
            }
        }

        public void ClearListeners(TEnumMsg msg)
        {
            mOperations.Add(new Operation {Op = EOperation.ClearMsg, Msg = msg});
        }

//        private void clearListeners(TEnumMsg msg)
//        {
//            mListeners.Remove(msg);
//        }

        public void ClearAllListeners()
        {
            mOperations.Add(new Operation {Op = EOperation.ClearAllMsg});
//            mListeners.Clear();
        }

        public void Update()
        {
            if(mOperations.Count == 0)
                return;

            for(int i = 0; i < mOperations.Count; i++)
            {
                switch(mOperations[i].Op)
                {
                    case EOperation.AddListener:
                        addListener(mOperations[i].Listener, mOperations[i].Msg);
                        break;
                    case EOperation.RemoveListener:
                        removeListener(mOperations[i].Listener, mOperations[i].Msg);
                        break;
                    case EOperation.ClearMsg:
                        mListeners.Remove(mOperations[i].Msg);
                        break;
                    case EOperation.ClearAllMsg:
                        mListeners.Clear();
                        break;
                    case EOperation.SendMsg:
                        sendMessage(mOperations[i].Tel);
                        break;
                    default:
                        throw new InvalidEnumArgumentException(mOperations[i].Op.ToString());
                }
            }
            mOperations.Clear();
        }

        public void SendMesssage(TEnumMsg msg, Object extraInfo = null, 
                                 ITelegraph<TEnumMsg> sender = null, ITelegraph<TEnumMsg> receiver = null)
        {
            Telegram<TEnumMsg> telegram = new Telegram<TEnumMsg>
            {
                Sender = sender,
                Receiver = receiver,
                Msg = msg,
                ExtraInfo = extraInfo
            };
            
            mOperations.Add(new Operation {Op = EOperation.SendMsg, Tel = telegram});
        }

        private void sendMessage(Telegram<TEnumMsg> telegram)
        {
            if (telegram.Receiver != null)
                telegram.Receiver.HandleMessage(telegram);
            else
            {
                if(!mListeners.ContainsKey(telegram.Msg))
                    return;

                foreach(ITelegraph<TEnumMsg> receiver in mListeners[telegram.Msg])
                {
                    receiver.HandleMessage(telegram);
                }
            }
        }
    } // end of the class MessageDispatcher.
} // end of the namespace AI.
