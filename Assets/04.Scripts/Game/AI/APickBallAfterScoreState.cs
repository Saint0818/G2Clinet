using AI;

/// <summary>
/// 某隊得分後, 另一隊執行撿球.
/// </summary>
public class APickBallAfterScoreState : State<EGameSituation>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.APickBallAfterScore; }
    }

    public override void Enter(object extraInfo)
    {
        
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
