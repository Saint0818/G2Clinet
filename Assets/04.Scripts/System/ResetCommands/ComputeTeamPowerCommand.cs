using System;

public class ComputeTeamPowerCommand : ICommand
{
    private bool mIsWaitingServer;

    public bool IsTimeUp()
    {
        return GameData.Team.Power < GameConst.Max_Power &&
            DateTime.UtcNow.Subtract(GameData.Team.PowerCD.ToUniversalTime()).TotalSeconds >=
               GameConst.AddPowerTimeInSeconds;
    }

    public void Execute()
    {
        if(mIsWaitingServer)
            return;

        mIsWaitingServer = true;

        RequireComputeTeamPowerProtocol protocol = new RequireComputeTeamPowerProtocol();
        protocol.Send(ok => { mIsWaitingServer = false; });
    }
}