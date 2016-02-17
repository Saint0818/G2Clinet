
/// <summary>
/// 狀態流程:
/// <list type="number">
/// <item> 剛進入遊戲: Presentation -> CameraMovement -> InitCourt -> Opening -> JumpBall. </item>
/// <item> 玩家跳球得到球: GamerAttack. </item>
/// <item> 玩家得分: SpecialAction -> NPCPickBall -> NPCInbounds -> NPCAttack. </item>
/// <item> 玩家掉球, 對手撿到球: NPCAttack. </item>
/// <item> 對手得分: SpecialAction -> GamerPickBall -> GamerInbounds -> GamerAttack. </item>
/// <item> 對手掉球, 玩家撿到球: GamerAttack. </item>
/// </list>
/// </summary>
public enum EGameSituation
{
    Presentation   = -3, // 演出, 也就是雙方球員出場, 互相叫囂.
    CameraMovement = -2, // 運鏡.
    InitCourt      = -1,
    None           = 0,
    Opening        = 1, // 開球.(球員站在中場, 等待裁判發球)
    JumpBall       = 2, // 裁判發球, 中間球員跳球, 其餘球員撿球.
    GamerAttack    = 3, // 玩家球隊進攻.
    NPCAttack      = 4, // 電腦球隊進攻.
    GamerPickBall  = 5, // 對方得分後, 玩家球隊撿球.
    GamerInbounds  = 6, // 玩家球隊做邊界發球.
    NPCPickBall    = 7, // 對方得分後, 電腦球隊撿球.
    NPCInbounds    = 8, // 電腦球隊做邊界發球.
    End            = 9, // 比賽結束.
    SpecialAction  = 10// 球員特殊演出, 球員得分後會進到此狀態. 比如像 Jason Terry 雙手張開像飛機的動作.
//	GamePlayEvent  = 11 // 比賽中觸發事件
}