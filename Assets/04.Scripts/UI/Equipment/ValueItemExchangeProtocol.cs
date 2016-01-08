using System;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class ValueItemExchangeProtocol
{
    private Action<bool> mCallback;

    public void Send(int[] changeData, Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("ValueItems", JsonConvert.SerializeObject(changeData));
        SendHttp.Get.Command(URLConst.ValueItemExchange, waitChangeValueItems, form);
    }

    private void waitChangeValueItems(bool ok, WWW www)
    {
        if(ok)
        {
            TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Player = team.Player;
            GameData.Team.ValueItems = team.ValueItems;
            GameData.Team.Player.Init();
            
            UIHint.Get.ShowHint(TextConst.S(531), Color.black);
        }
        else
            UIHint.Get.ShowHint(TextConst.S(534), Color.red);

        mCallback(ok);
    }
}