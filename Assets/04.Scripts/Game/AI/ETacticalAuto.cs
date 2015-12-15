/// <summary>
/// </summary>
public enum ETacticalAuto
{
    AttackNormal,

    // 邊界傳球(進攻方).
    Inbounds, 

    // 邊界傳球(防守方).
    InboundsDef, 

    // 半場邊界傳球(進攻方).
    HalfInbounds, 

    // 半場邊界傳球(防守方).
    HalfInboundsDef, 

    // 這用在剛切換到 EGameSituation.AttackGamer or EGameSituation.AttackNPC 狀態時,
    // 要球員跑到前場的路線.
    MoveFrontCourt
}