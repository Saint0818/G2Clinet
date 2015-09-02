using AI;

/// <summary>
/// 某隊得分後, 另一隊執行撿球.
/// </summary>
public class APickBallAfterScoreState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.APickBallAfterScore; }
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
