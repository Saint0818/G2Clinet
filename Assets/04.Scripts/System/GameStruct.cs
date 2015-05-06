using UnityEngine;
using System.Collections;
using System;

namespace GameStruct
{
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

	public static class GameSetting
	{
		public static bool Effect = true;
	}

    public struct TPlayer
    {
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
		}
    }
}
