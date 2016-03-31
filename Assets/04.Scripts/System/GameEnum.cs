namespace GameEnum
{
    public enum ELanguage
    {
        EN = 0,
        TW = 1,
        CN = 2,
        JP = 3
    }

    public static class EOS
    {
        public const string Editor = "0";
        public const string IOS = "1";
        public const string Android = "2";
        public const string Other = "3";
    }

    public static class ECompany
    {
        public const string NiceMarket = "NiceMarket";
        public const string Canada = "Canada";
        public const string PubGame = "PubGame";
    }

    public enum ELoading
    {
        Null = -2,
        SelectRole = -1,
        Login = 0,
        CreateRole = 1,
        Lobby = 2,
        Game = 3,
        Stage = 4
    }

    public enum EModelTest
    {
        None = -1,
        Center = 0,
        Forward = 1,
        Defender = 2
    }

    public enum ECameraTest
    {
        None,
        RGB
    }

    public enum ESkillType
    {
        NPC,
        Player
    }

    public enum ESave
    {
        AvatarSort,
        AvatarFilter,
        SkillCardCondition,
        SkillCardFilter,
        MusicOn,
        SoundOn,
        AIChangeTimeLv,
        UserLanguage,
        MoneyChange,
        DiamondChange,
        PowerChange,
        NewAvatar1,
        NewAvatar2,
        NewAvatar3,
        NewAvatar4,
        NewAvatar5,
        NewAvatar6,
        NewAvatar7,
        LevelUpFlag,
        // 1:LevelUp
        NewCardFlag,
        // 2:Get New Card
        NoticDate,
        NoticDaily,
        Quality,
        ShowEvent,
        ShowWatchFriend,
        SocialEventTime,
        WatchFriendTime
    }

    public enum EQualityType
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public enum ETestActive
    {
		Block20 = 11600,
		DunkBlock20 = 11610,
        Buff20 = 12100,
        Buff21 = 12150,
        Elbow20 = 11800,
        Dunk20 = 10700,
        Dunk22 = 10600,
		Push20 = 11700,
		PushThrough20 = 11710,
        Steal20 = 11500,
        Rebound20 = 11400,
        Shooting20 = 10499,
        Dunk21 = 10799
    }

    public enum EOpenID
    {
        Equipment = 1,
        Avatar = 2,
        Shop = 3,
        Social = 4,
        Ability = 5,
        Mission = 6,
        Mall = 7,
        PVP = 8,
        Instance = 9,
        CreateRole = 10,
        SuitCard = 26,
        SuitItem = 27,
        SkillEvolution = 21,
        SkillReinforce = 22,
		ThirdActive = 11,
		OperateBasket = 51,
		OperateAdvertisement = 52,
		OperateStore = 53,
		OperateGym = 54,
		OperateDoor = 55,
		OperateLogo = 56,
		OperateChair = 57,
		OperateCalendar = 58,
		OperateMail = 59
    }

    public enum EPlayerPostion
    {
        C = 0,
        F = 1,
        G = 2
    }

    public enum ETeamKind
    {
        Self = 0,
        Npc = 1
    }

    public enum EDefPointKind
    {
        Front = 0,
        Back = 1,
        Right = 2,
        Left = 3,
        FrontSteal = 4,
        BackSteal = 5,
        RightSteal = 6,
        LeftSteal = 7
    }

    public enum EActionFlag
    {
        None = 0,
        IsRun = 1,
        IsDefence = 2,
        IsDribble = 3,
        IsHoldBall = 4,
    }

    public enum EBallDirection
    {
        Left,
        Middle,
        Right
    }

    public static class ECourtMode
    {
        public const int Full = 0;
        public const int Half = 1;
    }

    public enum EBasketAnimationTest
    {
        BasketballAction_0 = 0,
        BasketballAction_1 = 1,
        BasketballAction_2 = 2,
        BasketballAction_3 = 3,
        BasketballAction_4 = 4,
        BasketballAction_5 = 5,
        BasketballAction_6 = 6,
        BasketballAction_7 = 7,
        BasketballAction_8 = 8,
        BasketballAction_9 = 9,
        BasketballAction_10 = 10,
        BasketballAction_11 = 11,
        BasketballAction_100 = 12,
        BasketballAction_101 = 13,
        BasketballAction_102 = 14,
        BasketballAction_103 = 15,
        BasketballAction_104 = 16,
        BasketballAction_105 = 17,
        BasketballAction_106 = 18,
        BasketballAction_107 = 19,
        BasketballAction_108 = 20,
        BasketballAction_109 = 21,
        BasketballAction_110 = 22,
        BasketballAction_111 = 23,
        BasketballAction_112 = 24,
    }

    public enum EScoreType
    {
        None,
        DownHand,
        UpHand,
        Normal,
        NearShot,
        LayUp
    }

    public enum EBasketSituation
    {
        Score = 0,
        Swish = 1,
        NoScore = 2,
        AirBall = 3
    }

    public enum EBasketDistanceAngle
    {
        ShortRightWing = 0,
        ShortRight = 1,
        ShortCenter = 2,
        ShortLeft = 3,
        ShortLeftWing = 4,
        MediumRightWing = 5,
        MediumRight = 6,
        MediumCenter = 7,
        MediumLeft = 8,
        MediumLeftWing = 9,
        LongRightWing = 10,
        LongRight = 11,
        LongCenter = 12,
        LongLeft = 13,
        LongLeftWing = 14,
    }

    public enum ESkillSituation
    {
        MoveDodge,
        Block0,
        Dunk0,
        Fall1,
        Fall2,
        Layup0,
        Steal0,
        Pass0,
        Pass2,
        Pass1,
        Pass5,
        Pick0,
        Push0,
        Rebound0,
        Elbow0,
        Shoot0,
        Shoot1,
        Shoot2,
        Shoot3,
        ShowOwnIn,
        ShowOwnOut,
        JumpBall,
        KnockDown0
    }

    public enum ESkillKind
    {
        DownHand = 10,
        UpHand = 20,
        Shoot = 30,
        NearShoot = 40,
        Layup = 50,
        Dunk = 60,
        MoveDodge0 = 110,
        Pass = 120,
        Pick2 = 130,
        Rebound = 140,
        Steal = 150,
        Block0 = 160,
        Push = 170,
        Elbow0 = 180,
        Fall1 = 190,
//        Fall2 = 200,
        ShowOwnIn = 220,
        ShowOwnOut = 230,
        JumpBall,
        KnockDown0,
        LayupSpecial
    }

    public static class EPassDirectState
    {
        public static int Forward = 1;
        public static int Back = 2;
        public static int Left = 3;
        public static int Right = 4;
    }

    public enum EDoubleType
    {
        None,
        Weak,
        Good,
        Perfect
    }

    public enum EShowWordType
    {
        Block,
        Dunk,
        NiceShot,
        Punch,
        Steal,
        GetTwo,
        GetThree,
        Assistant,
        DC5,
        DC10,
        DC15,
        DC20,
        TipShot,
        Catch
    }
    //For ActiveSkill
    public enum EBallState
    {
        CanSteal,
        CanBlock,
        CanRebound,
        CanDunkBlock,
        None

    }

    public enum EPlayerState
    {
        Alleyoop,
        Block0,
        Block1,
        Block2,
        Block20,
        BlockCatch,
        BasketAnimationStart,
        BasketActionEnd,
        BasketActionSwish,
        BasketActionSwishEnd,
        BasketActionNoScoreEnd,
        CatchFlat,
        CatchParabola,
        CatchFloor,
        Dribble0,
        Dribble1,
        Dribble2,
        Dribble3,
        Dunk0,
        Dunk1,
        Dunk2,
        Dunk3,
        Dunk4,
        Dunk5,
        Dunk6,
        Dunk7,
        Dunk20,
        Dunk21,
        Dunk22,
        DunkBasket,
        Defence0,
        Defence1,
        Elbow0,
        Elbow1,
        Elbow2,
        Elbow20,
        Elbow21,
        Fall0,
        Fall1,
        Fall2,
        FakeShoot,
        GotSteal,
        HoldBall,
        Idle,
        Intercept0,
        Intercept1,
        Layup0,
        Layup1,
        Layup2,
        Layup3,
        MoveDodge0,
        MoveDodge1,
        Pick0,
        Pick1,
        Pick2,
        Pass0,
        Pass1,
        Pass2,
        Pass3,
        Pass4,
        Pass5,
        Pass6,
        Pass7,
        Pass8,
        Pass9,
        Pass50,
        Push0,
        Push1,
        Push2,
        Push20,
        Run0,
        Run1,
        Run2,
        RunningDefence,
        Rebound0,
        Rebound20,
        ReboundCatch,
        Reset,
        Start,
        Shoot0,
        Shoot1,
        Shoot2,
        Shoot3,
        Shoot4,
        Shoot5,
        Shoot6,
        Shoot7,
        Shoot20,
        Steal0,
        Steal1,
        Steal2,
        Steal20,
        TipIn,
        JumpBall,
        Buff20,
        Buff21,
        Shooting,
        Show1,
        Show101,
        Show102,
        Show103,
        Show104,
        Show201,
        Show202,
        Show1001,
        Show1003,
        Ending0,
        Ending10,
        KnockDown0,
        KnockDown1
    }

    public enum EAnimatorState
    {
        Block,
        Catch,
        Defence,
        Dribble,
        Dunk,
        Elbow,
        Fall,
        FakeShoot,
        GotSteal,
        HoldBall,
        Idle,
        Intercept,
        Layup,
        MoveDodge,
        Push,
        Pick,
        Pass,
        Rebound,
        Run,
        Shoot,
        Steal,
        Buff,
        BlockCatch,
        End,
        KnockDown,
        JumpBall,
        Show,
        TipIn,
        Alleyoop
    }

    public enum EanimationEventFunction
    {
        AnimationEvent,
        TimeScale,
        ZoomIn,
        ZoomOut,
        MoveEvent,
        SetBallEvent,
        SkillEvent,
        EffectEvent,
        PlaySound
    }

    public enum EAnimationEventString{
        ActiveSkillEnd,
        Stealing,
        StealingEnd,
        GotStealing,
        FakeShootBlockMoment,
        BlockMoment,
        AirPassMoment,
        DoubleClickMoment,
        BlockCatchMomentStart,
        BlockCatchMomentEnd,
        BlockJump,
        Shooting,
        Passing,
        PickUp,
        PushCalculateStart,
        ElbowCalculateStart,
        BlockCalculateStart,
        BlockCalculateEnd,
        CloneMesh,
        DunkBasketStart,
        OnlyScore,
        DunkFallBall,
        AnimationEnd,
        PassEnd,
        CatchEnd,
        BlockCatchEnd,
        PickEnd,
        MoveDodgeEnd,
        BuffEnd,
        FakeShootEnd,
        BlockCatchingEnd,
        ReboundCalculateStart,
        ReboundCalculateEnd,
        TipInStart,
        TipInEnd,
		PushCalculateEnd,
		ElbowCalculateEnd
    }

    public enum ESkillEventString{
        CameraAction,
        Shooting,
        PushDistancePlayer,
        SetBallEvent,
        ActiveSkillEnd
    }
}
