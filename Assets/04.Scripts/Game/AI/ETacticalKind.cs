
/// <summary>
/// <para> 這是戰術類型, 要搭配 TacticalTable 一起使用. </para>
/// <para> Tactical Kind, used in TacticalTable. </para>
/// </summary>
public enum ETacticalKind
{
    Unknown,

    AttackNormalC,
    AttackNormalF,
    AttackNormalG,

    AttackShoot2C,
    AttackShoot2F,
    AttackShoot2G,

    AttackShoot3C,
    AttackShoot3F,
    AttackShoot3G,

    // 邊界傳球(進攻方).
    InboundsC, 
    InboundsF, 
    InboundsG,

    // 邊界傳球(防守方).
    InboundsDefC,
    InboundsDefF,
    InboundsDefG,

    // 半場邊界傳球(進攻方).
    HalfInboundsC, 
    HalfInboundsF, 
    HalfInboundsG,

    // 半場邊界傳球(防守方).
    HalfInboundsDefC,
    HalfInboundsDefF,
    HalfInboundsDefG,

    // 對方得分後, 攻守轉換, 邊界發球結束, 剛切換到 EGameSituation.GamerAttack or EGameSituation.NPCAttack 狀態時,
    // 要球員跑到前場的路線.
    MoveFrontCourtC, // 中鋒持球路線.
    MoveFrontCourtF, // 前鋒持球路線.
    MoveFrontCourtG // 後衛持球路線.
}
