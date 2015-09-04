

using System;

namespace AI
{
    public struct Telegram<TEnum> where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// 哪一個 Agent 送訊息.
        /// </summary>
        public ITelegraph<TEnum> Sender;

        /// <summary>
        /// 哪一個 Agent 接受訊息.
        /// </summary>
        public ITelegraph<TEnum> Receiver;

        public TEnum Msg;

        /// <summary>
        /// 訊息額外的資訊.
        /// </summary>
        public Object ExtraInfo;

        public void Clear()
        {
            Sender = null;
            Receiver = null;
            ExtraInfo = null;
        }

        public override string ToString()
        {
            return String.Format("Msg:{0}, Sender:{1}, Receiver:{2}, ExtraInfo:{3}", Msg, Sender, Receiver, ExtraInfo);
        }
    }
}


