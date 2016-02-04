using System;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class LookupPlayerBanksProtocol
{
    public class Data
    {
        public int SelectedRoleIndex;
        public TPlayerBank[] PlayerBank;

        public override string ToString()
        {
            return string.Format("SelectedRoleIndex: {0}", SelectedRoleIndex);
        }
    }

    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// </summary>
    private Action<bool, Data> mCallback;

    public void Send(Action<bool, Data> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        SendHttp.Get.Command(URLConst.LookPlayerBank, waitLookPlayerBank, form);
    }

    private void waitLookPlayerBank(bool isSuccess, WWW www)
    {
        Data data = null;
        if(isSuccess)
        {
            data = JsonConvert.DeserializeObject<Data>(www.text);
            GameData.Team.PlayerBank = data.PlayerBank;
        }
        else
            Debug.LogErrorFormat("Protocol:{0}, request data fail.", URLConst.LookPlayerBank);

        mCallback(isSuccess, data);
    }
}