using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class GMSetStageDailyChallengeNumProtocol
{
    private class Data
    {
        [UsedImplicitly]
        public Dictionary<int, int> StageDailyChallengeNums;
    }

    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// <para>[TStageReward]: 關卡獲得的獎勵. </para>
    /// </summary>
    private Action<bool> mCallback;

    public void Send(int stageID, int value, Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        form.AddField("Value", value);
        SendHttp.Get.Command(URLConst.GMSetStageDailyChallengeNum, waitGMSetStageDailyChallengeNum, form);
    }

    private void waitGMSetStageDailyChallengeNum(bool ok, WWW www)
    {
//        Debug.LogFormat("waitGMSetStageDailyChallengeNum, ok:{0}", ok);

        if(ok)
        {
            var data = JsonConvert.DeserializeObject<Data>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.Player.StageDailyChallengeNums = data.StageDailyChallengeNums;
        }

        mCallback(ok);
    }
}