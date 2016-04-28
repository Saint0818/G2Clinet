using System;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

public class ValueItemAddInlayProtocol
{
    private Action<bool> mCallback;

    private class Data
    {
        [UsedImplicitly]
        public TPlayer Player;
        [UsedImplicitly]
        public TMaterialItem[] MaterialItems;
        [UsedImplicitly]
        public TTeamRecord LifetimeRecord;
    }

    private int mPlayerValueItemKind;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerValueItemKind"></param>
    /// <param name="storageMaterialItemIndex"> 倉庫內的索引. </param>
    /// <param name="callback"></param>
    public void Send(int playerValueItemKind, int storageMaterialItemIndex,
                     Action<bool> callback)
    {
        mCallback = callback;
        mPlayerValueItemKind = playerValueItemKind;

        WWWForm form = new WWWForm();
        form.AddField("ValueItemKind", mPlayerValueItemKind);
        form.AddField("MaterialItemIndex", storageMaterialItemIndex);
        SendHttp.Get.Command(URLConst.ValueItemAddInlay, waitAddValueItemInlay, form);
    }

    private void waitAddValueItemInlay(bool ok, WWW www)
    {
//        Debug.LogFormat("waitAddValueItemInlay, ok:{0}", ok);

        if(ok)
        {
            var data = JsonConvertWrapper.DeserializeObject<Data>(www.text);
            GameData.Team.Player = data.Player;
            GameData.Team.MaterialItems = data.MaterialItems;
            GameData.Team.LifetimeRecord = data.LifetimeRecord;
            GameData.Team.PlayerInit();

            TValueItem valueItem = GameData.Team.Player.GetValueItem(mPlayerValueItemKind);
            if(valueItem != null)
                Statistic.Ins.LogEvent(204, valueItem.ID.ToString());
        }
        else
            UIHint.Get.ShowHint(www.text, Color.red);

        mCallback(ok);
    }
}