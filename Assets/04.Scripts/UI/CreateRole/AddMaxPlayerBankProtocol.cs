using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class AddMaxPlayerBankProtocol
{
    private class Data
    {
        [UsedImplicitly]
        public int Diamond;

        [UsedImplicitly]
        public int MaxPlayerBank;

        public override string ToString()
        {
            return string.Format("Diamond: {0}, MaxPlayerBank: {1}", Diamond, MaxPlayerBank);
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
        SendHttp.Get.Command(URLConst.AddMaxPlayerBank, waitAddMaxPlayerBank, form);
    }

    private void waitAddMaxPlayerBank(bool ok, WWW www)
    {
        Debug.LogFormat("waitAddMaxPlayerBank, ok:{0}", ok);

        if(ok)
        {
			var data = JsonConvert.DeserializeObject<Data>(www.text, SendHttp.Get.JsonSetting);

            Debug.Log(data);

            GameData.Team.Diamond = data.Diamond;
            GameData.Team.MaxPlayerBank = data.MaxPlayerBank;
        }
        else
            UIHint.Get.ShowHint("AddMaxPlayerBank fail!", Color.red);

        mCallback(ok);
    }
}