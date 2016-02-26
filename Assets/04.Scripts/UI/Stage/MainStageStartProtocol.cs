using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class MainStageStartProtocol
{
    private Action<bool, Data> mCallback;

    public class Data
    {
        [UsedImplicitly]
        public int Power;
        [UsedImplicitly]
        public DateTime PowerCD;
        [UsedImplicitly]
        public int[] ConsumeValueItems;
        [UsedImplicitly]
        public Dictionary<int, TValueItem> PlayerValueItems;

        public bool IsPlayerValueItemChanged()
        {
            return ConsumeValueItems.Length > 0;
        }

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

    public void Send(int stageID, Action<bool, Data> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        SendHttp.Get.Command(URLConst.StageStart, waitMainStageStart, form);
    }

    private void waitMainStageStart(bool ok, WWW www)
    {
        var data = new Data();
        if(ok)
        {
			data = JsonConvert.DeserializeObject<Data>(www.text, SendHttp.Get.JsonSetting);

//            Debug.Log(data);

            GameData.Team.Power = data.Power;
            GameData.Team.PowerCD = data.PowerCD;
            GameData.Team.Player.ConsumeValueItems = data.ConsumeValueItems;

            if(data.IsPlayerValueItemChanged())
                GameData.Team.Player.ValueItems = data.PlayerValueItems;
        }

        mCallback(ok, data);
    }
}