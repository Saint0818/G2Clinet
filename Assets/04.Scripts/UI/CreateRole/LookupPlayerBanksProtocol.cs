using System;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class LookupPlayerBanksProtocol
{
    public class TPlayerBanks
    {
        public int SelectedRoleIndex;
        public TPlayerBank[] PlayerBanks;

        public override string ToString()
        {
            return string.Format("SelectedRoleIndex: {0}", SelectedRoleIndex);
        }
    }

    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// </summary>
    private Action<bool, TPlayerBanks> mCallback;

    public void Send(Action<bool, TPlayerBanks> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        SendHttp.Get.Command(URLConst.LookPlayerBank, waitLookPlayerBank, form);
    }

    private void waitLookPlayerBank(bool isSuccess, WWW www)
    {
        TPlayerBanks playerBanks = null;
        if(isSuccess)
            playerBanks = JsonConvert.DeserializeObject<TPlayerBanks>(www.text);
        else
            Debug.LogErrorFormat("Protocol:{0}, request data fail.", URLConst.LookPlayerBank);

        mCallback(isSuccess, playerBanks);
    }
}