
namespace GameEnum {
	public enum ELanguage {
		EN = 0,
		TW = 1,
		CN = 2,
		JP = 3
	}

	public static class EOS {
		public const string Editor = "0";
		public const string IOS = "1";
		public const string Android = "2";
		public const string Other = "3";
	}

	public static class ECompany {
		public const string NiceMarket = "NiceMarket";
		public const string PubGame = "PubGame";
		public const string R2 = "R2";
		public const string MyGamez = "MyGamez";
	}

	public enum ELoadingGamePic {
		Null = -2,
		SelectRole = -1,
		Login = 0,
		CreateRole = 1,
		Lobby = 2,
		Game = 3,
		Stage = 4
	}

	public enum EModelTest {
		None = -1,
		Center = 0,
		Forward = 1,
		Defender = 2
	}

	public enum ECameraTest {
		None,
		RGB
	}

	public enum ESkillType {
		NPC,
		Player
	}

	public enum ESave{
		AvatarSort,
		AvatarFilter,
		SkillCardCondition,
		SkillCardFilter,
		MusicOn,
		SoundOn,
		EffectOn,
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
		NewAvatar7
	}

	public enum ETestActive {
		Block20 = 11600,
		Buff20 = 12100,
		Buff21 = 12150,
		Elbow20 = 11800,
		Dunk20 = 10700,
		Dunk22 = 10600,
		Push20 = 11700,
		Steal20 = 11500,
		Rebound20 = 11400,
		Shooting20 = 10499
	}
}
