public enum EPosKind
{
    None,
    Attack,
    Tee, 
    TeeDefence,
    HalfTee, // 半場邊界傳球.
    HalfTeeDefence,
    Fast,
    Center,
    Forward,
    Guard
}

public static class EPosKindExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="index"> 球員在球隊打的位置. 0: C, 1:F, 2:G. </param>
    /// <returns></returns>
    public static int GetPosNameIndex(this EPosKind kind, int index = -1)
    {
        switch (kind)
        {
            case EPosKind.Attack:
                return 2;
            case EPosKind.Tee:
                if (index == 0)
                    return 3;
                if (index == 1)
                    return 4;
                if (index == 2)
                    return 5;
                    return -1;
            case EPosKind.TeeDefence:
                if (index == 0)
                    return 6;
                if (index == 1)
                    return 7;
                if (index == 2)
                    return 8;
                return -1;
            case EPosKind.HalfTee:
                if (index == 0)
                    return 15;
                if (index == 1)
                    return 16;
                if (index == 2)
                    return 17;
                return -1;
            case EPosKind.HalfTeeDefence:
                if (index == 0)
                    return 18;
                if (index == 1)
                    return 19;
                if (index == 2)
                    return 20;
                return -1;
            case EPosKind.Fast:
                if (index == 0)
                    return 9;
                if (index == 1)
                    return 10;
                if (index == 2)
                    return 11;
                return -1;
            case EPosKind.Center:
                return 12;
            case EPosKind.Forward:
                return 13;
            case EPosKind.Guard:
                return 14;
            default:
                return -1;
        }
    }
}