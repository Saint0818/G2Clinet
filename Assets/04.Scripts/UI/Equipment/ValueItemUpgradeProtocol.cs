using System;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class ValueItemUpgradeProtocol
{
    private Action<bool> mCallback;

    public void Send(int valueItemKind, Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("ValueItemKind", valueItemKind);
        SendHttp.Get.Command(URLConst.ValueItemUpgrade, waitValueItemUpgrade, form);
    }

    private void waitValueItemUpgrade(bool ok, WWW www)
    {
//        Debug.LogFormat("waitValueItemUpgrade, ok:{0}", ok);

        if(ok)
        {
            var team = JsonConvertWrapper.DeserializeObject<TTeam>(www.text);
            GameData.Team.Player.ValueItems = team.Player.ValueItems;
            GameData.Team.LifetimeRecord = team.LifetimeRecord;
            GameData.Team.Money = team.Money;
        }
        else
            UIHint.Get.ShowHint(www.text, Color.red);

        mCallback(ok);
    }
}