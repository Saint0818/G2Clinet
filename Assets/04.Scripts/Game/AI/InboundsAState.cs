﻿using AI;

public class InboundsAState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.InboundsA; }
    }

    public override void EnterImpl(object extraInfo)
    {
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
