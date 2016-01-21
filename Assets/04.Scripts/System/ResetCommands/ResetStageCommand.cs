using System;
using UnityEngine;

public class ResetStageCommand : ICommand
{
    private int mLastYear;
    private int mLastMonth;
    private int mLastDay;

    public ResetStageCommand()
    {
        recordCurrentTime();
    }

    private void recordCurrentTime()
    {
        mLastYear = DateTime.Now.Year;
        mLastMonth = DateTime.Now.Month;
        mLastDay = DateTime.Now.Day;
    }

    public bool IsTimeUp()
    {
        return DateTime.Now.Year != mLastYear ||
               DateTime.Now.Month != mLastMonth ||
               DateTime.Now.Day != mLastDay;
    }

    public void Execute()
    {
        WWWForm form = new WWWForm();
        SendHttp.Get.Command(URLConst.CheckResetStage, waitCheckResetStage, form);

        recordCurrentTime();
    }

    private void waitCheckResetStage(bool ok, WWW www)
    {
        Debug.LogFormat("waitCheckResetStage, ok:{0}", ok);

        if(ok)
        {
            if(www.text.Length > 0)
                GameData.Team.Player.DailyStageChallengeNums.Clear();
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.CheckResetStage);
    }
}
