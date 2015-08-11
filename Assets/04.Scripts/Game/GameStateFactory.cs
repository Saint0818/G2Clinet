
using System;
using System.Collections.Generic;
using AI;

public class GameStateFactory : IStateMachineFactory<EGameSituation>
{
    private readonly Dictionary<EGameSituation, State<EGameSituation>> mStates = new Dictionary<EGameSituation, State<EGameSituation>>();

    public State<EGameSituation> CreateState(EGameSituation e)
    {
        if(!mStates.ContainsKey(e))
            mStates.Add(e, createStateImpl(e));

        return mStates[e];
    }

    private State<EGameSituation> createStateImpl(EGameSituation e)
    {
        switch(e)
        {
            case EGameSituation.None: return new NullState();
            case EGameSituation.InitShowContorl: return new InitShowControlState();
            case EGameSituation.ShowOne: return new ShowOneState();
        }

        throw new NotImplementedException(String.Format("Value:{0}", e));
    }
}
