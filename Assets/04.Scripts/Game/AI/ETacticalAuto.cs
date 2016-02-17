/// <summary>
/// 戰術分類. 真正的戰術其實都是分類在 ETacticalKind.
/// </summary>
public enum ETacticalAuto
{
    // 進攻戰術.
    AttackNormal, // 一般進攻戰術.
    AttackShoot2, // 投 2 分球戰術.
    AttackShoot3, // 投 3 分球戰術.

    // 邊界傳球(進攻方).
    Inbounds, 

    // 邊界傳球(防守方).
    InboundsDef, 

    // 半場邊界傳球(進攻方).
    HalfInbounds, 

    // 半場邊界傳球(防守方).
    HalfInboundsDef, 

    // 這用在剛切換到 EGameSituation.GamerAttack or EGameSituation.NPCAttack 狀態時,
    // 要球員跑到前場的路線.
    MoveFrontCourt
}