using System;

namespace GameEnum {
	public enum ELanguage {
		EN = 0,
		TW = 1,
		CN = 2,
		JP = 3
	}

	public enum EOS {
		EDITOR = 0,
		IOS = 1,
		ANDROID = 2
	}

	public enum ECompany {
		NiceMarket = 0,
		PubGame = 1,
		R2 = 2,
		MyGamez = 3
	}

	public enum ELoadingGamePic {
		SelectRole = -1,
		Game = 1,
		Stage = 2
	}

	public enum ESceneTest
	{
		Single,
		Multi,
		Release
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
		AvatarFilter
	}
}
