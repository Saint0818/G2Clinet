using System;
using UnityEngine;

public class GMResetDailyLoginNumProtocol
{
    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// </summary>
    private Action<bool> mCallback;

    public void Send(Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        SendHttp.Get.Command(URLConst.GMResetDailyLoginNums, waitGMResetDailyLoginNums, form);
    }

    private void waitGMResetDailyLoginNums(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMResetDailyLoginNums, ok:{0}", ok);

        if(ok)
            GameData.Team.DailyLoginNums.Clear();

        mCallback(ok);
    }
}