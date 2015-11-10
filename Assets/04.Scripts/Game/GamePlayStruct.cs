using System;
using System.Collections.Generic;

namespace GamePlayStruct {
	public struct TTacticalAction {
		public float x;
		public float z;
		public bool Speedup;
		public bool Catcher;
		public bool Shooting;
	}
	
	public struct TTacticalData
    {
		public string FileName; // 戰術名稱.
		public TTacticalAction[] PosAy1; // 中鋒的資料.
		public TTacticalAction[] PosAy2; // 前鋒的資料.
		public TTacticalAction[] PosAy3; // 後衛的資料.

	    public ETactical Tactical
	    {
	        get
	        {
	            if(mTactical == ETactical.None)
                    convert();

	            return mTactical;
	        }
	    }

	    private ETactical mTactical;

	    private void convert()
	    {
	        foreach(KeyValuePair<string, ETactical> pair in TacticalTable.PrefixNameTacticalPairs)
	        {
	            if(FileName.StartsWith(pair.Key))
	                mTactical = pair.Value;
	        }
	    }
		
		public override string ToString()
		{
			return String.Format("Name:{0}", FileName);
		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="posIndex"> 0:C, 1:F, 2:G </param>
	    /// <returns></returns>
	    public TTacticalAction[] GetActions(int posIndex)
	    {
	        if(posIndex == 0)
	            return PosAy1;
	        if(posIndex == 1)
	            return PosAy2;
	        if(posIndex == 2)
	            return PosAy3;

	        return null;
	    }
    }
	
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
		public int Kind;
		public int Value1;
		public int Value2;
		public int Value3;
		public int FinishCondition;
		public int ConditionKind;
		public int ConditionValue;
		public int ConditionOperator;
		public int NextEventID;

		public TToturialAction[] Actions;

		public TGamePlayEvent(int i) {
			Kind = 0;
			Value1 = 0;
			Value2 = 0;
			Value3 = 0;
			FinishCondition = 0;
			ConditionKind = 0;
			ConditionOperator = 0;
			ConditionValue = 0;
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
