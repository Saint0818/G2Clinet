using System;
using Newtonsoft.Json;
using UnityEngine;

public class RequireComputeTeamPowerProtocol
{
    public class Data
    {
        public int Power;
        public DateTime PowerCD;

        public override string ToString()
        {
            return string.Format("Power: {0}, PowerCD: {1}", Power, PowerCD.ToUniversalTime());
        }
    }

    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// </summary>
    private Action<bool> mCallback;

    public void Send(Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        SendHttp.Get.Command(URLConst.RequireComputeTeamPower, waitRequireComputeTeamPower, form);
    }

    private void waitRequireComputeTeamPower(bool ok, WWW www)
    {
        Debug.LogFormat("waitRequireComputeTeamPower, ok:{0}", ok);

        if(ok)
        {
            var data = JsonConvert.DeserializeObject<Data>(www.text);

            Debug.Log(data);

            GameData.Team.Power = data.Power;
            GameData.Team.PowerCD = data.PowerCD;
        }

        mCallback(ok);
    }
}