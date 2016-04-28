using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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

    private int mStageID;

    public void Send(int stageID, Action<bool> callback)
    {
        mCallback = callback;
        mStageID = stageID;

        WWWForm form = new WWWForm();
        form.AddField("StageID", mStageID);
        SendHttp.Get.Command(URLConst.ResetStageDailyChallenge, waitResetStageDailyChallenge, form);
    }

    private void waitResetStageDailyChallenge(bool ok, WWW www)
    {
        Debug.LogFormat("waitResetStageDailyChallenge, ok:{0}", ok);

        if(ok)
        {
            var data = JsonConvertWrapper.DeserializeObject<Data>(www.text);

            Statistic.Ins.LogEvent(52, mStageID.ToString(), GameData.Team.Diamond - data.Diamond);

            GameData.Team.Diamond = data.Diamond;
            GameData.Team.Player.StageDailyChallengeNums = data.StageDailyChallengeNums;
            GameData.Team.Player.ResetStageDailyChallengeNums = data.ResetStageDailyChallengeNums;
        }

        mCallback(ok);
    }
}