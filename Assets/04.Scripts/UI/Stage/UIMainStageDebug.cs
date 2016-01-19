using UnityEngine;

public class UIMainStageDebug
{
    private int mStageID;

    public void Send(int stageID)
    {
        mStageID = stageID;

        var start = new MainStageStartProtocol();
        start.Send(stageID, waitStart);
    }

    private void waitStart(bool ok, MainStageStartProtocol.Data data)
    {
        Debug.LogFormat("waitStart, ok:{0}", ok);
        Debug.Log(data);

        var win = new MainStageWinProtocol();
        win.Send(mStageID, waitMainStageWin);
    }

    private void waitMainStageWin(bool ok, TStageReward reward)
    {
        Debug.LogFormat("waitMainStageWin, ok:{0}", ok);
        Debug.Log(reward);

        var again = new MainStageRewardAgainProtocol();
        again.Send(mStageID, waitMainStageRewardAgain);
    }

    private void waitMainStageRewardAgain(bool ok, TStageRewardAgain reward)
    {
        Debug.LogFormat("waitMainStageRewardAgain, ok:{0}", ok);
        Debug.Log(reward);

        var again = new MainStageRewardAgainProtocol();
        again.Send(mStageID, waitMainStageRewardAgain2);
    }

    private void waitMainStageRewardAgain2(bool ok, TStageRewardAgain reward)
    {
        Debug.LogFormat("waitMainStageRewardAgain2, ok:{0}", ok);
        Debug.Log(reward);
    }
}