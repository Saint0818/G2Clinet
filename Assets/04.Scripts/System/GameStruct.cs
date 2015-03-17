using UnityEngine;
using System.Collections;
using System;

public static class GameStruct
{
	public struct TPlayerAttribute
	{	
		public int Body;
		public int Hair;
		public int AHead;
		public int Cloth;
		public int Pants;
		public int Shoes;
		public int MHeadDress;
		public int ZBackEquip;

		public TPlayerAttribute (int i)
		{
			Body = 2;
			Hair = 2;
			AHead = 1;
			Cloth = 5;
			Pants = 6;
			Shoes = 1;
			MHeadDress = 2;
			ZBackEquip = 1;
		}
	}
}
