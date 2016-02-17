using System;

public class ComputeTeamPowerCommand : ICommand
{
    private bool mIsWaitingServer;

    public bool IsTimeUp()
    {
        DateTime utcPowerCD = GameData.Team.PowerCD.ToUniversalTime();
        DateTime utcUpdatePowerTime = utcPowerCD.AddSeconds(GameConst.AddPowerTimeInSeconds);
        bool isPowerLack = GameData.Team.Power < GameConst.Max_Power;
        bool isUpdateTimeUp = DateTime.UtcNow.Subtract(utcUpdatePowerTime).TotalSeconds >= 0;

//        Debug.LogFormat("PowerCD:{0}, UpdatePowerCD:{1}, isPowerLack:{2}, isNeedUpdate:{3}, IsWaitingServer:{4}", 
//                        utcPowerCD, utcUpdatePowerTime, isPowerLack, isNeedUpdate, mIsWaitingServer);

        return isPowerLack && isUpdateTimeUp && !mIsWaitingServer;
    }

    public void Execute()
    {
//        Debug.Log("Send Command");
        mIsWaitingServer = true;

        RequireComputeTeamPowerProtocol protocol = new RequireComputeTeamPowerProtocol();
        protocol.Send(ok =>
        {
            mIsWaitingServer = false;
//            Debug.Log("Receive Command");
        });
    }
}