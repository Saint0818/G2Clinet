
using System;
using System.Collections.Generic;
using AI;

public class GameStateFactory : IStateMachineFactory<EGameSituation>
{
    private readonly Dictionary<EGameSituation, IState> mStates = new Dictionary<EGameSituation, IState>();

    public IState CreateState(EGameSituation e)
    {
        if(!mStates.ContainsKey(e))
            mStates.Add(e, createStateImpl(e));

        return mStates[e];
    }

    private IState createStateImpl(EGameSituation e)
    {
        switch(e)
        {
            case EGameSituation.None: return new NullState();
        }

        throw new NotImplementedException(String.Format("Value:{0}", e));
    }
}
