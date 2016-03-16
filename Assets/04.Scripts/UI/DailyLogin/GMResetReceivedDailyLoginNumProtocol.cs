using System;
using UnityEngine;

public class GMResetReceivedDailyLoginNumProtocol
{
    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// </summary>
    private Action<bool> mCallback;

    public void Send(Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        SendHttp.Get.Command(URLConst.GMResetReceivedDailyLoginNums, waitGMResetReceivedDailyLoginNums, form);
    }

    private void waitGMResetReceivedDailyLoginNums(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMResetReceivedDailyLoginNums, ok:{0}", ok);

        if(ok)
            GameData.Team.ReceivedDailyLoginNums.Clear();

        mCallback(ok);
    }
}