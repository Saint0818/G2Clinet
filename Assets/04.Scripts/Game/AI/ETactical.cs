
/// <summary>
/// <para> 這是戰術類型, 要搭配 TacticalTable 一起使用. </para>
/// <para> Tactical Category, used in TacticalTable. </para>
/// </summary>
public enum ETactical
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

//public static class ETacticalExtensions
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="kind"></param>
//    /// <param name="index"> 球員在球隊打的位置. 0: C, 1:F, 2:G. </param>
//    /// <returns></returns>
//    public static int GetPosNameIndex(this ETactical kind, int index = -1)
//    {
//        switch (kind)
//        {
//            case ETactical.Attack:
//                return 2;
//            case ETactical.Inbounds:
//                if (index == 0)
//                    return 3;
//                if (index == 1)
//                    return 4;
//                if (index == 2)
//                    return 5;
//                    return -1;
//            case ETactical.InboundsDefence:
//                if (index == 0)
//                    return 6;
//                if (index == 1)
//                    return 7;
//                if (index == 2)
//                    return 8;
//                return -1;
//            case ETactical.HalfTee:
//                if (index == 0)
//                    return 15;
//                if (index == 1)
//                    return 16;
//                if (index == 2)
//                    return 17;
//                return -1;
//            case ETactical.HalfTeeDefence:
//                if (index == 0)
//                    return 18;
//                if (index == 1)
//                    return 19;
//                if (index == 2)
//                    return 20;
//                return -1;
//            case ETactical.Fast:
//                if (index == 0)
//                    return 9;
//                if (index == 1)
//                    return 10;
//                if (index == 2)
//                    return 11;
//                return -1;
//            case ETactical.Center:
//                return 12;
//            case ETactical.Forward:
//                return 13;
//            case ETactical.Guard:
//                return 14;
//            default:
//                return -1;
//        }
//    }
//}