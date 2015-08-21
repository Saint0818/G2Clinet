using AI;
using UnityEngine;

/// <summary>
/// <para> 這個狀態是用在球員得分後, 得分球員做的特寫 or 特殊動作. 比如 Jason Terry 得分後的招牌動作是滑翔翼. </para>
/// </summary>
public class SpecialActionState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.SpecialAction; }
    }

    /// <summary>
    /// <para> 幾秒後會切換到下一個狀態. </para>
    /// <para> switch to the next state, after a few seconds. </para>
    /// </summary>
    /// <remarks>
    /// <para> 注意, 這不能設定為 0, 否則會發生 1 vs 1 的時候, 對方得分後, 沒人撿球的問題. </para>
    /// <para> Note: this can't be zero, otherwise nobody picking ball will occur when 1 vs 1. </para>
    /// </remarks>
    private const float NextStateTime = 2;

    private float mChangeStateTime;

    /// <summary>
    /// 時間到後, 切換的新狀態.
    /// </summary>
    private EGameSituation mNextState;

    public override void EnterImpl(object extraInfo)
    {
        mChangeStateTime = Time.time + NextStateTime;
        mNextState = (EGameSituation)extraInfo;

        foreach(PlayerBehaviour player in GameController.Get.GamePlayers)
        {
            player.ResetFlag();
        }

//        Debug.LogFormat("SpecialActionState.Enter. Time.time:{0}, mChangeStateTime:{1}, NextState:{2}", Time.time, mChangeStateTime, mNextState);
    }

    public override void Update()
    {
        if(Time.time >= mChangeStateTime)
        {
//            Debug.LogFormat("SpecialActionState.ChangeState: {0}", mNextState);

            Parent.ChangeState(mNextState);
            GameController.Get.ChangeSituation(mNextState);
        }
    }

    public override void Exit()
    {
    }
}
