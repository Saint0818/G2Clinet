using UnityEngine;
using System.Collections;
using System;

public struct TAIlevel
{
	public int Level;
	public int ProactiveRate;
	public int AutoFollowTime;
	public float DefDistance;
}

public static class GameStruct
{
	public struct TAvatar
	{	
		public int Body;
		public int Hair;
		public int AHead;
		public int Cloth;
		public int Pants;
		public int Shoes;
		public int MHandDress;
		public int ZBackEquip;

		public TAvatar (int i)
		{
			Body = 2;
			Hair = 2;
			AHead = 1;
			Cloth = 5;
			Pants = 6;
			Shoes = 1;
			MHandDress = 2;
			ZBackEquip = 1;

		}
	}
}
