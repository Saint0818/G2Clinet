using System;
using GameStruct;
using UnityEngine;

public class ValueItemUpgradeProtocol
{
    private Action<bool> mCallback;

    private int mValueItemKind;

    public void Send(int valueItemKind, Action<bool> callback)
    {
        mCallback = callback;
        mValueItemKind = valueItemKind;

        WWWForm form = new WWWForm();
        form.AddField("ValueItemKind", mValueItemKind);
        SendHttp.Get.Command(URLConst.ValueItemUpgrade, waitValueItemUpgrade, form);
    }

    private void waitValueItemUpgrade(bool ok, WWW www)
    {
//        Debug.LogFormat("waitValueItemUpgrade, ok:{0}", ok);

        if(ok)
        {
            var team = JsonConvertWrapper.DeserializeObject<TTeam>(www.text);

            TValueItem valueItem = team.Player.GetValueItem(mValueItemKind);
            if(valueItem != null)
                Statistic.Ins.LogEvent(203, valueItem.ID.ToString(), GameData.Team.Money - team.Money);

            GameData.Team.Player.ValueItems = team.Player.ValueItems;
            GameData.Team.LifetimeRecord = team.LifetimeRecord;
            GameData.Team.Money = team.Money;
        }
        else
            UIHint.Get.ShowHint(www.text, Color.red);

        mCallback(ok);
    }
}