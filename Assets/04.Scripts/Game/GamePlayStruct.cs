namespace GamePlayStruct
{
    public struct TBasketShootPositionData {
		public string AnimationName;
		public float ShootPositionX;
		public float ShootPositionY;
		public float ShootPositionZ;
	}

	public struct TToturialAction {
		public int Team;
		public int Index;
		public int MoveKind;
		public TTacticalAction Action;
	}

	public struct TGamePlayEvent {
		public int ID;
		public int Kind;
		public int Value1;
		public int Value2;
		public int Value3;
		public int FinishCondition;
		public int ConditionKind;
		public int ConditionValue;
		public int ConditionValue2;
		public int OtherEventID;
		public int NextEventID;

		public TToturialAction[] Actions;

		public TGamePlayEvent(int i) {
			ID = 0;
			Kind = 0;
			Value1 = 0;
			Value2 = 0;
			Value3 = 0;
			FinishCondition = 0;
			ConditionKind = 0;
			ConditionValue = 0;
			ConditionValue2 = 0;
			OtherEventID = 0;
			NextEventID = 0;
			
			Actions = new TToturialAction[0];
		}
	}

	public struct TStageToturial {
		public int ID;
		public TGamePlayEvent[] Events;

		public TStageToturial(int i) {
			ID = 0;
			Events = new TGamePlayEvent[0];
		}
	}
}
