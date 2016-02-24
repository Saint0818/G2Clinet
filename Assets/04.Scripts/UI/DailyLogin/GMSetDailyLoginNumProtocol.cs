using System;
using System.Collections.Generic;
using UnityEngine;

public class GMSetDailyLoginNumProtocol
{
    public class Data
    {
        public Dictionary<int, Dictionary<int, int>> DailyLoginNums;
    }

    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// </summary>
    private Action<bool> mCallback;

    public void Send(int year, int month, int value, Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("Year", year);
        form.AddField("Month", month);
        form.AddField("LoginNum", value);
        SendHttp.Get.Command(URLConst.GMSetDailyLoginNums, waitGMSetDailyLoginNums, form);
    }

    private void waitGMSetDailyLoginNums(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMSetDailyLoginNums, ok:{0}", ok);

        if(ok)
        {
            var data = JsonConvertWrapper.DeserializeObject<Data>(www.text);
            GameData.Team.DailyLoginNums = data.DailyLoginNums;
        }

        mCallback(ok);
    }
}