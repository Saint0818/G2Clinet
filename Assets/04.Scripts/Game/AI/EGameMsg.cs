
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
    /// <para> Telegram.ExtraInfo: PlayerBehaviour[]. </para>
    /// </summary>
    GamePlayersCreated,

    /// <summary>
    /// <para> 跳球時, 第一個碰到球的球員, 應該要送出此訊息. </para>
    /// <para> Telegram.ExtraInfo: PlayerBehaviour, 跳球時, 第一個碰到球的球員. </para>
    /// </summary>
    PlayerTouchBallWhenJumpBall
}