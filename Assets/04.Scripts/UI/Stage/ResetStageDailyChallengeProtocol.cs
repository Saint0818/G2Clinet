using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class ResetStageDailyChallengeProtocol
{
    [UsedImplicitly]
    private class Data
    {
        [UsedImplicitly]
        public int Diamond;

        [UsedImplicitly]
        public Dictionary<int, int> StageDailyChallengeNums;

        [UsedImplicitly]
        public Dictionary<int, int> ResetStageDailyChallengeNums;
    }

    private Action<bool> mCallback;

    public void Send(int stageID, Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        SendHttp.Get.Command(URLConst.ResetStageDailyChallenge, waitResetStageDailyChallenge, form);
    }

    private void waitResetStageDailyChallenge(bool ok, WWW www)
    {
        Debug.LogFormat("waitResetStageDailyChallenge, ok:{0}", ok);

        if(ok)
        {
            var data = JsonConvert.DeserializeObject<Data>(www.text, SendHttp.Get.JsonSetting);

            GameData.Team.Diamond = data.Diamond;
            GameData.Team.Player.StageDailyChallengeNums = data.StageDailyChallengeNums;
            GameData.Team.Player.ResetStageDailyChallengeNums = data.ResetStageDailyChallengeNums;
        }

        mCallback(ok);
    }
}