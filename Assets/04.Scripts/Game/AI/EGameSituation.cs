
public enum EGameSituation
{
//    Loading           =-4,
    Presentation   = -3, // 演出, 也就是雙方球員出場, 互相叫囂.
    CameraMovement = -2, // 運鏡.
    InitCourt      = -1,
    None           = 0,
    Opening        = 1, // 開球.
    JumpBall       = 2,
    AttackA        = 3, // A 隊進攻.
    AttackB        = 4, // B 隊進攻.
    APickBallAfterScore    = 5, // 對方得分後, A 隊(玩家) 撿球.
    InboundsA      = 6, // A 隊(玩家) 邊界發球.
    BPickBallAfterScore    = 7, // 對方得分後, B 隊(電腦) 撿球.
    InboundsB      = 8, // B 隊(電腦) 邊界發球.
    End            = 9, // 比賽結束.
    SpecialAction  = 10 // 球員特殊演出, 球員得分後會進到此狀態. 比如像 Jason Terry 雙手張開像飛機的動作.
}