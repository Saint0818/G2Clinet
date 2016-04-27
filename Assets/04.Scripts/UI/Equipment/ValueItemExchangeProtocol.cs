using System;
using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class ValueItemExchangeProtocol
{
    private Action<bool> mCallback;

    public void Send(int[] exchangeIndices, int[] stackIndices, Action<bool> callback)
    {
//        Debug.LogFormat("Exchange:{0}, Stack:{1}", DebugerString.Convert(exchangeIndices), DebugerString.Convert(stackIndices));

        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("TeamValueItemIndices", JsonConvert.SerializeObject(exchangeIndices));
        form.AddField("TeamStackValueItemIndices", JsonConvert.SerializeObject(stackIndices));
        SendHttp.Get.Command(URLConst.ValueItemExchange, waitChangeValueItems, form);
    }

    private void waitChangeValueItems(bool ok, WWW www)
    {
        if(ok)
        {
			var team = JsonConvert.DeserializeObject<TTeam>(www.text, SendHttp.Get.JsonSetting);
            trySendNewValueTimeEvent(GameData.Team.Player.ValueItems, team.Player.ValueItems);
            GameData.Team.Player = team.Player;
            GameData.Team.ValueItems = team.ValueItems;
            GameData.Team.PlayerInit();
            
            UIHint.Get.ShowHint(TextConst.S(531), Color.black);
        }
        else
            UIHint.Get.ShowHint(TextConst.S(534), Color.red);

        mCallback(ok);
    }

    private void trySendNewValueTimeEvent(Dictionary<int, TValueItem> oldValueItems, 
                                          Dictionary<int, TValueItem> newValueItems)
    {
        for(var kind = TPlayer.MinValueItemKind; kind <= TPlayer.MaxValueItemKind; kind++)
        {
            if(!newValueItems.ContainsKey(kind))
                continue;

            var oldValueItemID = -1;
            if(oldValueItems.ContainsKey(kind))
                oldValueItemID = oldValueItems[kind].ID;

            if(oldValueItemID != newValueItems[kind].ID)
                Statistic.Ins.LogEvent(16, newValueItems[kind].ID.ToString(), -1);
        } 
    }
}