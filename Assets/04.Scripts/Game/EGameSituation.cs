
public enum EGameSituation
{
//    Loading           =-4,
    InitShowContorl   = -3,
    ShowOne           = -2,
    ShowTwo           = -1,
    None           = 0,
    Opening        = 1, // 開球.
    JumpBall       = 2,
    AttackA        = 3,
    AttackB        = 4,
    TeeAPicking    = 5, // A 隊(玩家) 撿球.
    TeeA           = 6, // A 隊(玩家) 邊界發球.
    TeeBPicking    = 7, // B 隊(電腦) 撿球.
    TeeB           = 8, // B 隊(電腦) 邊界發球.
    End            = 9, // 比賽結束.
    SpecialAction  = 10 // 球員特殊演出, 球員得分後會進到此狀態. 比如像 Jason Terry 雙手張開像飛機的動作.
}