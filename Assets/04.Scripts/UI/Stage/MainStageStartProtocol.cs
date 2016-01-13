using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class MainStageStartProtocol
{
    private Action<bool> mCallback;

    private class Data
    {
        [UsedImplicitly]
        public int Power;
        [UsedImplicitly]
        public DateTime PowerCD;
        [UsedImplicitly]
        public int[] ConsumeValueItems;
        [UsedImplicitly]
        public Dictionary<int, TValueItem> PlayerValueItems;

        public override string ToString()
        {
            return string.Format("Power: {0}, PowerCD: {1}, ConsumeValueItems: {2}, PlayerValueItem17:[{3}:{4}], PlayerValueItem18:[{5}:{6}]", 
                                 Power, PowerCD, DebugerString.Convert(ConsumeValueItems), getItemID(17), getItemNum(17), getItemID(18), getItemNum(18));
        }

        private int getItemID(int kind)
        {
            if(PlayerValueItems.ContainsKey(kind))
                return PlayerValueItems[kind].ID;
            return 0;
        }

        private int getItemNum(int kind)
        {
            if(PlayerValueItems.ContainsKey(kind))
                return PlayerValueItems[kind].Num;
            return 0;
        }
    }

    public void Send(int stageID, Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        SendHttp.Get.Command(URLConst.MainStageStart, waitMainStageStart, form);
    }

    private void waitMainStageStart(bool ok, WWW www)
    {
        if(ok)
        {
            var data = JsonConvert.DeserializeObject<Data>(www.text);

            Debug.Log(data);

            GameData.Team.Power = data.Power;
            GameData.Team.PowerCD = data.PowerCD;
            GameData.Team.Player.ConsumeValueItems = data.ConsumeValueItems;

            // 數值裝是否改變?
            if(data.ConsumeValueItems.Length > 0)
                GameData.Team.Player.ValueItems = data.PlayerValueItems;
        }

        mCallback(ok);
    }
}