
public enum EGameSituation
{
    Presentation   = -3, // 演出, 也就是雙方球員出場, 互相叫囂.
    CameraMovement = -2, // 運鏡.
    InitCourt      = -1,
    None           = 0,
    Opening        = 1, // 開球.(球員站在中場, 等待裁判發球)
    JumpBall       = 2, // 裁判發球, 中間球員跳球, 其餘球員撿球.
    AttackGamer    = 3, // 玩家球隊進攻.
    AttackNPC      = 4, // 電腦球隊進攻.
    GamerPickBall  = 5, // 對方得分後, 玩家球隊撿球.
    InboundsGamer  = 6, // 玩家球隊做邊界發球.
    NPCPickBall    = 7, // 對方得分後, 電腦球隊撿球.
    InboundsNPC    = 8, // 電腦球隊做邊界發球.
    End            = 9, // 比賽結束.
    SpecialAction  = 10 // 球員特殊演出, 球員得分後會進到此狀態. 比如像 Jason Terry 雙手張開像飛機的動作.
}