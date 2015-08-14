using AI;
using UnityEngine;

/// <summary>
/// 這個狀態是用在球員得分後, 得分球員做的特寫 or 特殊動作. 比如 Jason Terry 得分後的招牌動作是滑翔翼.
/// </summary>
public class SpecialActionState : State<EGameSituation, EGameMsg>
{
    /// <summary>
    /// 幾秒後會切換到下一個狀態.
    /// </summary>
    private const float NextStateTime = 0;

    private float mChangeStateTime;

    /// <summary>
    /// 時間到後, 切換的新狀態.
    /// </summary>
    private EGameSituation mNextState;

    public override void EnterImpl(object extraInfo)
    {
        mChangeStateTime = Time.time + NextStateTime;
        mNextState = (EGameSituation)extraInfo;

        foreach(PlayerBehaviour player in GameController.Get.GamePlayerList)
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
