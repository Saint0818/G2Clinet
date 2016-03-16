using System;
using UnityEngine;

public class GMResetLifetimeReceivedLoginNumProtocol
{
    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// </summary>
    private Action<bool> mCallback;

    public void Send(Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        SendHttp.Get.Command(URLConst.GMResetLifetimeReceivedLoginNum, waitGMSetLifetimeReceivedLoginNum, form);
    }

    private void waitGMSetLifetimeReceivedLoginNum(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMSetLifetimeReceivedLoginNum, ok:{0}", ok);

        if(ok)
        {
            GameData.Team.LifetimeRecord.ReceivedLoginNum = 0;

            Debug.LogFormat("Lifetime, LoginNum:{0}, ReceivedLoginNum:{1}", 
                GameData.Team.LifetimeRecord.LoginNum,
                GameData.Team.LifetimeRecord.ReceivedLoginNum);
        }

        mCallback(ok);
    }
}