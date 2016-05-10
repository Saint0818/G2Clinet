using UnityEngine;

public class UIMainStageDebug
{
    private int mStageID;
    private int mWinPoints;
    private int mLostPoints;

    public void Send(int stageID, int winPoints, int lostPoints)
    {
        mStageID = stageID;
        mWinPoints = winPoints;
        mLostPoints = lostPoints;

        var start = new MainStageStartProtocol();
        start.Send(stageID, waitStart);
    }

    private void waitStart(bool ok, MainStageStartProtocol.Data data)
    {
        Debug.LogFormat("waitStart, ok:{0}", ok);
        Debug.Log(data);

        var win = new StageWinProtocol();
        win.Send(mStageID, mWinPoints, mLostPoints, waitMainStageWin);
    }

    private void waitMainStageWin(bool ok, TStageReward reward)
    {
        Debug.LogFormat("waitMainStageWin, ok:{0}", ok);
        Debug.Log(reward);

//        var again = new StageRewardAgainProtocol();
//        again.Send(mStageID, waitMainStageRewardAgain);
    }

    private void waitMainStageRewardAgain(bool ok, TStageRewardAgain reward)
    {
        Debug.LogFormat("waitMainStageRewardAgain, ok:{0}", ok);
        Debug.Log(reward);

        var again = new StageRewardAgainProtocol();
        again.Send(mStageID, waitMainStageRewardAgain2);
    }

    private void waitMainStageRewardAgain2(bool ok, TStageRewardAgain reward)
    {
        Debug.LogFormat("waitMainStageRewardAgain2, ok:{0}", ok);
        Debug.Log(reward);
    }
}