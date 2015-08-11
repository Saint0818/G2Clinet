

using System;
using System.Collections.Generic;

namespace AI
{
    /// <summary>
    /// AI 框架的核心系統, 通過運用 Message 的方式實現事件驅動(event-driven) 的架構.
    /// 假設玩家要攻擊一群正在巡邏的守衛時, 只要玩家攻擊, 就會產生一個事件(這裡稱為 Message),
    /// 然後這群守衛收到玩家攻擊的事件後, 就可以做出對應的事情. 比如反擊(也就是攻擊玩家)等等.
    /// </summary>
    /// <typeparam name="TEnum"> 必須是 Enum. </typeparam>
    /// <remarks>
    /// 使用方法:
    /// <list type="number">
    /// <item> new instance. </item>
    /// <item> call AddListener(), 和系統說明要監聽哪些事件. </item>
    /// <item> call SendMesssage() 送出訊息. </item>
    /// </list>
    /// </remarks>
    public class MessageDispatcher<TEnum> where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        private readonly Dictionary<TEnum, List<ITelegraph<TEnum>>> mListeners = new Dictionary<TEnum, List<ITelegraph<TEnum>>>();
        private Telegram<TEnum> mTelegram = new Telegram<TEnum>();

        public void AddListeners(ITelegraph<TEnum> listener, params TEnum[] msgs)
        {
            foreach(TEnum msg in msgs)
            {
                AddListener(listener, msg);
            }
        }

        public void AddListener(ITelegraph<TEnum> listener, TEnum msg)
        {
            if(!mListeners.ContainsKey(msg))
                mListeners.Add(msg, new List<ITelegraph<TEnum>>());

            mListeners[msg].Add(listener);
        }

        public void RemoveListener(ITelegraph<TEnum> listener, params TEnum[] msgs)
        {
            foreach(TEnum msg in msgs)
            {
                RemoveListener(listener, msg);
            }
        }

        public void RemoveListener(ITelegraph<TEnum> listener, TEnum msg)
        {
            if(mListeners.ContainsKey(msg))
                mListeners[msg].Remove(listener);
        }

        public void ClearListeners(params TEnum[] msgs)
        {
            foreach(TEnum msg in msgs)
            {
                ClearListeners(msg);
            }
        }

        public void ClearListeners(TEnum msg)
        {
            mListeners.Remove(msg);
        }

        public void ClearListeners()
        {
            mListeners.Clear();
        }

        public void SendMesssage(ITelegraph<TEnum> sender, ITelegraph<TEnum> receiver, TEnum msg, Object extraInfo)
        {
            mTelegram.Clear();
            mTelegram.Sender = sender;
            mTelegram.Receiver = receiver;
            mTelegram.Message = msg;
            mTelegram.ExtraInfo = extraInfo;

            if(mTelegram.Receiver != null)
                mTelegram.Receiver.HandleMessage(mTelegram);
            else
            {
                if(!mListeners.ContainsKey(msg))
                    return;

                foreach(ITelegraph<TEnum> r in mListeners[msg])
                {
                    r.HandleMessage(mTelegram);
                }
            }
        }
    } // end of the class MessageDispatcher.
} // end of the namespace AI.
