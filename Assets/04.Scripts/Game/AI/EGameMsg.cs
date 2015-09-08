
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
    CoachOrderAttackTactical,

    /// <summary>
    /// <para> 遊戲裡的球員創建完畢. </para>
    /// <para> Telegram.ExtraInfo: PlayerBehavior[]. </para>
    /// </summary>
    GamePlayersCreated
}