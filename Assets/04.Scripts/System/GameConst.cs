public class GameConst
{
	public const string SceneLobby = "Lobby";
	public const string SceneGamePlay = "Court_0";

	//Sync with the server
	public const int Max_RoleNum = 3;
	public const int Default_RoleNum = 2;
	public const int Max_Money = 1000000;
	public const int Max_Power = 20;
	public const int Default_MainStageID = 101;
	public const int Max_ActiveSkill = 3;

	public static int PotentialCount = 6;
	public static int PreLvPotential = 10;
	public static int PotentialResetPrice = 100;
	public static int[] PotentialRule = new int[6]{5, 5, 5, 5, 5, 5};

	public static int RenamePrice = 100;
	
	public const int ID_LimitActive = 10000;

	//Game play move speed
	public const float DefSpeedup = 7.5f;
	public const float DefSpeedNormal = 6.2f;
	public const float BallOwnerSpeedup = 6.5f;
	public const float BallOwnerSpeedNormal = 5f;
	public const float WalkSpeed = 3.2f;
	public const float AttackSpeedup = 7;
	public const float AttackSpeedNormal = 5.7f;

    /// <summary>
    /// 每幾秒檢查 MovePower 的數值.(MovePower 就是球員的體力)
    /// </summary>
    public const float MovePowerCheckTime = 0.15f;

    /// <summary>
    /// 球員移動時, 扣的體力值.
    /// </summary>
    public const float MovePowerMoving = 1f;

    /// <summary>
    /// 球員靜止不動時, 體力回復的值.
    /// </summary>
    public const float MovePowerRevive = 2.5f;

    /// <summary>
    /// 超過此距離投籃, 會是撥長距離投籃的動作.
    /// </summary>
	public const float LongShootDistance = 15f;
	public const float Point3Distance = 10.6f;
	public const float Point2Distance = 7;
	public const float DunkDistance = 7; // 在此距離內, 才會做灌籃.
	public const float DunkDistanceNoMove = 3; // 在此距離內, 才會做灌籃.
	public const float LayupDistance = 7;
	public const float ShortShootDistance = 3;
	public const float PickBallDistance = 2.5f;
	public static float[] AITime = {1f, 3f, 5f, 15f, 30f, 999999f};

    /// <summary>
    /// 推人, 抄球的距離.
    /// </summary>
	public const float StealPushDistance = 3; 

    /// <summary>
    /// 抄截時, 抄截角度判定.(球員 +Z 軸的扇形區域)
    /// </summary>
	public const float StealFanAngle = 30; 

	public const float BlockDistance = 5;

    public const int FakeShootRate = 40; // 做假動作的機率.
	public const float DefMoveTime = 0.2f;
	public const float CrossOverDistance = 2.5f;
	public const float CoolDownPushTime = 3; // 推人冷卻時間, 單位:秒.
	public const float CoolDownElbowTime = 3; // Elbow 冷卻時間, 單位:秒.
    public const float CoolDownStealTime = 1.2f; // 抄截冷卻時間, 單位:秒.
//	public const float WaitStealTime = 0.5f;

    /// <summary>
    /// 當對手和球員在 TheatDistance and ThreatAngle 範圍內時, 表示雙方處理威脅狀態. 
    /// 會影響假動作, 投籃的邏輯.
    /// </summary>
	public const float ThreatDistance = 1.5f;
	public const float ThreatAngle = 40f;

    /// <summary>
    /// 這是避免一直傳球的參數, 傳球後, 經過此時間, 才可以做下一次傳球.
    /// </summary>
    public const float CoolDownPassTime = 2.0f;

    /// <summary>
    /// 預設的懲罰時間, 單位:秒. 
    /// 目前懲罰時間用在球員做 elbow 攻擊時(or 被抄截,但是未成功), 
    /// 球員身上的球會出現特效, 表示該球員目前被抄球的機率會增加.
    /// </summary>
    public const float DefaultPunishTime = 3;

    /// <summary>
    /// AI 執行站在原地的機率.
    /// </summary>
    public const float AIIdleRate = 30;

    /// <summary>
    /// AI 執行 Idle 的時間範圍.
    /// </summary>
    public const float AIMinIdleTime = 0.5f;
    public const float AIMaxIdleTime = 1.5f;

    /// <summary>
    /// AI 執行跑戰術機率.
    /// </summary>
    public const float AITacticalRate = 30;

    /// <summary>
    /// AI 執行跑戰術的時間範圍.
    /// </summary>
    public const float AIMinTacticalTime = 2.0f;
    public const float AIMaxTacticalTime = 4.5f;

    public const int AddAnger_PlusScore = 20;
	public const int AddAnger_Block = 30;
	public const int AddAnger_Steal = 20;
	public const int AddAnger_Push = 20;
	public const int AddAnger_Rebound = 20;
	public const int AddAnger_Perfect = 15;
	public const int DelAnger_Blocked = -10;
	public const int DelAnger_Fall1 = -10;
	public const int DelAnger_Stealed = -10;
	public const int DelAnger_Fall2 = -5;
	public const float SlowDownValue = 0f;
	public const float SlowDownTime = 0.1f;
	public const int SlowDownAngle = 90;

	public static int SelfAILevel = 0;
	public static int NpcAILevel = 0;
	public static float MiddleDistance = 12;
	public static float FastPassDistance = 2.5f;
	public static float CloseDistance = 4;
	public static float DefDistance = 3.5f;
	public static float ExtraScoreRate = 10;

	public const float AlleyoopPassTime = 0.5f;
	public const float PlayerShineTime = 0.5f;
	public const int PlayerShineCount = 3;
	public const float CrossTimeX = 0.5f;
	public const float CrossTimeZ = 1f;

	public const float HintLongPressTime = 0.3f;

	//Score
	public static string[] AngleScoreRightWing = {"0","3","10"};
	public static string[] AngleScoreRight = {"0","3","4","6","8","11"};
	public static string[] AngleScoreCenter = {"0","1","2","3","4"};
	public static string[] AngleScoreLeft = {"0","3","5","7","9","11"}; 
	public static string[] AngleScoreLeftWing = {"0","3","10"}; 

	public static string[] DistanceScoreShort = {"0","1","2","3","4","5","6","11"}; 
	public static string[] DistanceScoreMedium = {"0","2","3","4","5","6","7","8","9","10",}; 
	public static string[] DistanceScoreLong = {"0","2","3","4"}; 

	//No Score
	public static string[] AngleNoScoreRightWing = {"103","104","110","112"};
	public static string[] AngleNoScoreRight = {"101","102","103","106","108","110","112"};
	public static string[] AngleNoScoreCenter = {"100","101","102","103","104"};
	public static string[] AngleNoScoreLeft = {"101","102","103","105","107","109","111"}; 
	public static string[] AngleNoScoreLeftWing = {"103","104","109","111"}; 
	
	public static string[] DistanceNoScoreShort = {"100","101","102","103","104","105","106","112"}; 
	public static string[] DistanceNoScoreMedium = {"101","102","103","104","105","106","107","108","109","110","111","112"}; 
	public static string[] DistanceNoScoreLong = {"101","102","103","104"};

    /// <summary>
    /// 屬性全滿的數值.
    /// </summary>
    public const float AttributeMax = 100;
    public const float AttributeCount = 12;
}
