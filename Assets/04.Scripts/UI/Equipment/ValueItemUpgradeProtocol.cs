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
			TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.Player.ValueItems = team.Player.ValueItems;
            GameData.Team.Money = team.Money;

//            UIHint.Get.ShowHint(TextConst.S(531), Color.black);
        }
        else
            UIHint.Get.ShowHint(www.text, Color.red);

        mCallback(ok);
    }
}