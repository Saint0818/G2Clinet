
/// <summary>
/// <para> 這是戰術類型, 要搭配 TacticalTable 一起使用. </para>
/// <para> Tactical Kind, used in TacticalTable. </para>
/// </summary>
public enum ETacticalKind
{
    None,
    Attack,
    Center,
    Forward,
    Guard,

    // 邊界傳球(進攻方).
    Inbounds, // 必須搭配 PlayerBehavior.Index 一起使用.
    InboundsCenter, 
    InboundsForward, 
    InboundsGuard,

    // 邊界傳球(防守方).
    InboundsDefence, // 必須搭配 PlayerBehavior.Index 一起使用.
    InboundsDefenceCenter,
    InboundsDefenceForward,
    InboundsDefenceGuard,

    // 半場邊界傳球(進攻方).
    HalfInbounds, // 必須搭配 PlayerBehavior.Index 一起使用.
    HalfInboundsCenter, 
    HalfInboundsForward, 
    HalfInboundsGuard,

    // 半場邊界傳球(防守方).
    HalfInboundsDefence, // 必須搭配 PlayerBehavior.Index 一起使用.
    HalfInboundsDefenceCenter,
    HalfInboundsDefenceForward,
    HalfInboundsDefenceGuard,

    Fast, // 必須搭配 PlayerBehavior.Index 一起使用.
    FastCenter,
    FastForward,
    FastGuard
}
