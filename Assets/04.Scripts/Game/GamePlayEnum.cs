using System;

namespace GamePlayEnum {
	public enum ECourtMode {
		Full = 0,
		Half = 1
	}

	public enum EBasketAnimationTest {
		Basket0=0,
		Basket1=1,
		Basket2=2,
		Basket3=3,
		Basket4=4,
		Basket5=5,
		Basket6=6,
		Basket7=7,
		Basket8=8,
		Basket9=9,
		Basket10=10,
		Basket11=11,
		Basket100=12,
		Basket101=13,
		Basket102=14,
		Basket103=15,
		Basket104=16,
		Basket105=17,
		Basket106=18,
		Basket107=19,
		Basket108=20,
		Basket109=21,
		Basket110=22,
		Basket111=23,
		Basket112=24,
	}
	
	public enum EScoreType {
		None,
		DownHand,
		UpHand,
		Normal,
		NearShot,
		LayUp
	}
	
	public enum EBasketSituation {
		Score = 0,
		Swish = 1,
		NoScore = 2,
		AirBall = 3
	}
	
	public enum EBasketDistanceAngle {
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
	
	public enum ESkillSituation {
		MoveDodge,
		Block,
		Dunk0,
		Fall1,
		Fall2,
		Layup0,
		Steal0,
		Pass0,
		Pass2,
		Pass1,
		Pass5,
		PickBall,
		Push0,
		Rebound,
		Elbow,
		Shoot0,
		Shoot1,
		Shoot2,
		Shoot3,
		ShowOwnIn,
		ShowOwnOut,
		JumpBall,
		KnockDown0
	}
	
	public enum ESkillKind {
		DownHand = 10,
		UpHand = 20,
		Shoot = 30,
		NearShoot = 40,
		Layup = 50,
		Dunk = 60,
		MoveDodge = 110,
		Pass = 120,
		Pick2 = 130,
		Rebound = 140,
		Steal = 150,
		Block0 = 160,
		Push = 170,
		Elbow0 = 180,
		Fall1 = 190,
		Fall2 = 200,
		ShowOwnIn = 220,
		ShowOwnOut = 230,
		JumpBall,
		KnockDown0,
		LayupSpecial
	}
	
	public static class EPassDirectState {
		public static int Forward = 1;
		public static int Back = 2;
		public static int Left = 3;
		public static int Right = 4;
	}

	public enum EDoubleType {
		None,
		Weak,
		Good,
		Perfect
	}

	public enum EShowWordType {
		Block,
		Dunk,
		NiceShot,
		Punch,
		Steal
	}
	//For ActiveSkill 
	public enum EBallState {
		CanSteal,
		CanBlock,
		CanRebound,
		CanDunkBlock,
		None

	}
}
