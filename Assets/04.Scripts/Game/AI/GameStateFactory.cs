
using System;
using System.Collections.Generic;
using AI;

public class GameStateFactory : IStateMachineFactory<EGameSituation, EGameMsg>
{
    private readonly Dictionary<EGameSituation, State<EGameSituation, EGameMsg>> mStates = new Dictionary<EGameSituation, State<EGameSituation, EGameMsg>>();

    public State<EGameSituation, EGameMsg> CreateState(EGameSituation e)
    {
        if(!mStates.ContainsKey(e))
            mStates.Add(e, createStateImpl(e));

        return mStates[e];
    }

    private State<EGameSituation, EGameMsg> createStateImpl(EGameSituation situation)
    {
        switch(situation)
        {
            case EGameSituation.None: return new NullState();
            case EGameSituation.Presentation: return new PresentationState();
            case EGameSituation.ShowOne: return new ShowOneState();
            case EGameSituation.SpecialAction: return new SpecialActionState();
            case EGameSituation.TeeAPicking: return new TeeAPickingState();
            case EGameSituation.TeeA: return new TeeAState();
            case EGameSituation.TeeBPicking: return new TeeBPickingState();
            case EGameSituation.TeeB: return new TeeBState();
        }

        throw new NotImplementedException(String.Format("Value:{0}", situation));
    }
}
