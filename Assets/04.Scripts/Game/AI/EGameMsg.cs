
/// <summary>
/// 籃球比賽會傳遞的訊息(GameMsgDispatcher).
/// </summary>
public enum EGameMsg
{
    UISkipClickOnGaming,

    /// <summary>
    /// <para> 教練下達進攻戰術給球員. </para>
    /// <para> Telegram.ExtraInfo:TTacticalData. </para>
    /// </summary>
    CoachOrderAttackTactical
}