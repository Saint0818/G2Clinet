using System;

namespace GameEnum {
	public enum ELanguage {
		TW = 0,
		EN = 1
	}

	public enum ELoadingGamePic {
		SelectRole = -1,
		Game = 1
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
}
