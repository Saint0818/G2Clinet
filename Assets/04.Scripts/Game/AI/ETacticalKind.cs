
/// <summary>
/// <para> 這是戰術類型, 要搭配 TacticalTable 一起使用. </para>
/// <para> Tactical Kind, used in TacticalTable. </para>
/// </summary>
public enum ETacticalKind
{
    Unknown,
    AttackNormal,
    AttackNormalC,
    AttackNormalF,
    AttackNormalG,

    // 邊界傳球(進攻方).
    Inbounds, // 必須搭配 PlayerBehavior.Index 一起使用.
    InboundsC, 
    InboundsF, 
    InboundsG,

    // 邊界傳球(防守方).
    InboundsDefence, // 必須搭配 PlayerBehavior.Index 一起使用.
    InboundsDefC,
    InboundsDefF,
    InboundsDefG,

    // 半場邊界傳球(進攻方).
    HalfInbounds, // 必須搭配 PlayerBehavior.Index 一起使用.
    HalfInboundsC, 
    HalfInboundsF, 
    HalfInboundsG,

    // 半場邊界傳球(防守方).
    HalfInboundsDefence, // 必須搭配 PlayerBehavior.Index 一起使用.
    HalfInboundsDefC,
    HalfInboundsDefF,
    HalfInboundsDefG,

    // 這用在剛切換到 EGameSituation.AttackGamer or EGameSituation.AttackNPC 狀態時,
    // 要球員跑到前場的路線.
    Fast, // 必須搭配 PlayerBehavior.Index 一起使用.
    MoveFrontCourtC, // 中鋒持球路線.
    MoveFrontCourtF, // 前鋒持球路線.
    MoveFrontCourtG // 後衛持球路線.
}
