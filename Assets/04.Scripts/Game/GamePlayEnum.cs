using System;

namespace GamePlayEnum {
	public enum ECourtMode {
		Full = 0,
		Half = 1
	}
	
	public enum EWinMode {
		Score = 0,
		Time = 1
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
		Pass4,
		PickBall,
		Push0,
		Rebound,
		Elbow,
		Shoot0,
		Shoot1,
		Shoot2,
		Shoot3
	}
	
	public enum ESkillKind {
		DownHand = 1,
		UpHand = 2,
		Shoot = 3,
		NearShoot = 4,
		Layup = 5,
		Dunk = 6,
		MoveDodge = 11,
		Pass = 12,
		Pick2 = 13,
		Rebound = 14,
		Steal = 15,
		Block = 16,
		Push = 17,
		Elbow = 18,
		Fall1 = 19,
		Fall2 = 20,
		Special1 = 101,
		Special2 = 102,
		Special3 = 103,
		Special4 = 104,
		Special5 = 105,
		Special6 = 106
	}
	
	public enum EPassDirectState {
		Forward = 1,
		Back = 2,
		Left = 3,
		Right = 4
	}
}
