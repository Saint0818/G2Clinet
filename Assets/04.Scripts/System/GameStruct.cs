using UnityEngine;
using System.Collections;
using System;

namespace GameStruct
{
	public struct TTeam 
	{
		public string Identifier;
		public string FBName;
		public string FBid;
		public string SessionID;
		public DateTime CreateTime;
		public DateTime LoginTime;
		public DateTime PowerCD;
		public DateTime FreeLuckBox;

		public int Money;
		public int Power;
		public int Diamond;

		public int PlayerIndex;
		public TPlayer[] Players;
	}

    public struct TPlayer {
        public int ID;
        public string Name;
        public int BodyType;
        public int AILevel;
        public int Steal;
		public int Control;
		public int Body;
		public int Hair;
		public int Cloth;
		public int Pants;
		public int Shoes;
		public int MHandDress;
		public int AHeadDress;
		public int ZBackEquip;

		public TPlayerAttribute Attr;
		public TAvatar Avatar;

		//
		public int ProactiveRate;
		public int StealRate;
		public int AutoFollowTime;
		public float DefDistance;
		public int BlockRate;
		public int FaketBlockRate;
		public int PushingRate;
		public int ElbowingRate;
		public int BlockDunk;
		public int JumpBallRate;
		public int BlockPushRate;
		public int ReboundRate;
		public int TipIn;
		public int AlleyOop;

		public TPlayer(int Level)
		{
			ID = 0;
			Name = "";
			BodyType = 0;
			AILevel = Level;
			Steal = 0;
			Control = 0;
			Body = 2001;
			Hair = 2001;
			Cloth = 5001;
			Pants = 6001;
			Shoes = 1001;
			MHandDress = 0;
			AHeadDress = 0;
			ZBackEquip = 0;

			ProactiveRate = 0;
			StealRate = 0;
			AutoFollowTime = 0;
			DefDistance = 0;
			BlockRate = 0;
			FaketBlockRate = 0;
			PushingRate = 0;
			ElbowingRate = 0;
			BlockDunk = 0;
			JumpBallRate = 0;
			BlockPushRate = 0;
			ReboundRate = 0;
			TipIn = 0;
			AlleyOop = 0;

			Attr = new TPlayerAttribute();
			Avatar = new TAvatar(0);
		}
    }

	public struct TPlayerAttribute
	{
		public float STR;
	}

	public struct TAvatar
	{	
		public int Body;
		public int Hair;
		public int AHeadDress;
		public int Cloth;
		public int Pants;
		public int Shoes;
		public int MHandDress;
		public int ZBackEquip;
		
		public TAvatar (int i){
			Body = 2001;
			Hair = 2001;
			Cloth = 5001;
			Pants = 6001;
			Shoes = 1001;
			MHandDress = 0;
			AHeadDress = 0;
			ZBackEquip = 0;
		}
	}

	public struct TAIlevel
	{
		public int ProactiveRate;
		public int StealRate;
		public int AutoFollowTime;
		public float DefDistance;
		public int BlockRate;
		public int FaketBlockRate;
		public int PushingRate;
		public int ElbowingRate;
		public int BlockDunk;
		public int JumpBallRate;
		public int BlockPushRate;
		public int ReboundRate;
		public int TipIn;
		public int AlleyOop;
	}

	public enum Language
	{
		TW = 0,
		EN = 1
	}

	public struct TGameSetting
	{
		public float AIChangeTime;
		public Language Language;
		public bool Effect;
	}
}
