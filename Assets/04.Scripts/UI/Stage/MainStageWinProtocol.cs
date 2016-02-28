using System;
using Newtonsoft.Json;
using UnityEngine;

public class MainStageWinProtocol
{
    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// <para>[TStageReward]: 關卡獲得的獎勵. </para>
    /// </summary>
    private Action<bool, TStageReward> mCallback;

    public void Send(int stageID, Action<bool, TStageReward> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        SendHttp.Get.Command(URLConst.StageWin, waitMainStageWin, form);
    }

    private void waitMainStageWin(bool ok, WWW www)
    {
//        Debug.LogFormat("waitMainStageWin, ok:{0}", ok);

        if(ok)
        {
			TStageReward reward = JsonConvert.DeserializeObject<TStageReward>(www.text, SendHttp.Get.JsonSetting);

//            Debug.LogFormat("waitMainStageWin:{0}", reward);

            GameData.Team.Power = reward.Power;
            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Player = reward.Player;
            GameData.Team.PlayerInit();
            GameData.Team.Items = reward.Items;
            GameData.Team.ValueItems = reward.ValueItems;
            GameData.Team.MaterialItems = reward.MaterialItems;

            mCallback(true, reward);
        }
        else
        {
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
            mCallback(false, new TStageReward());
        }
    }
}