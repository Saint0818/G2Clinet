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
	        foreach(KeyValuePair<string, ETactical> pair in TacticalMgr.PrefixNameTacticalPairs)
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
}
