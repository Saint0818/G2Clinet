

using System;

namespace AI
{
    /// <summary>
    /// 實作此介面就可以接受訊息.
    /// </summary>
    public interface ITelegraph<TEnumMsg> where TEnumMsg : struct, IConvertible, IComparable, IFormattable
    {
        void HandleMessage(Telegram<TEnumMsg> msg);
    }
}

