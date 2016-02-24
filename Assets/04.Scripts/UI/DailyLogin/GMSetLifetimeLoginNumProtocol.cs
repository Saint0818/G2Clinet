using System;
using UnityEngine;

public class GMSetLifetimeLoginNumProtocol
{
    public class Data
    {
        public int LifetimeLoginNum;
    }

    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// </summary>
    private Action<bool> mCallback;

    public void Send(int value, Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("LoginNum", value);
        SendHttp.Get.Command(URLConst.GMSetLifeTimeLoginNum, waitGMSetLifetimeLoginNum, form);
    }

    private void waitGMSetLifetimeLoginNum(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMSetLifetimeLoginNum, ok:{0}", ok);

        if(ok)
        {
            var data = JsonConvertWrapper.DeserializeObject<Data>(www.text);
            GameData.Team.LifetimeRecord.LoginNum = data.LifetimeLoginNum;
        }

        mCallback(ok);
    }
}