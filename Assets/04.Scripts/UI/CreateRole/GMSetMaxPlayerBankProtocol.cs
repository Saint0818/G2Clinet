using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class GMSetMaxPlayerBankProtocol
{
    private class Data
    {
        [UsedImplicitly]
        public int MaxPlayerBank;

        public override string ToString()
        {
            return string.Format("MaxPlayerBank: {0}", MaxPlayerBank);
        }
    }

    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// </summary>
    private Action<bool> mCallback;

    public void Send(int value, Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("MaxPlayerBank", value);
        SendHttp.Get.Command(URLConst.GMSetMaxPlayerBank, waitSetMaxPlayerBank, form);
    }

    private void waitSetMaxPlayerBank(bool ok, WWW www)
    {
        Debug.LogFormat("waitSetMaxPlayerBank, ok:{0}", ok);

        if(ok)
        {
            var data = JsonConvert.DeserializeObject<Data>(www.text);

            Debug.Log(data);

            GameData.Team.MaxPlayerBank = data.MaxPlayerBank;
        }
        else
            UIHint.Get.ShowHint("GMSetMaxPlayerBank fail!", Color.red);

        mCallback(ok);
    }
}