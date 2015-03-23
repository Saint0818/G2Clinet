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
		public int AHeadDress;
		public int Cloth;
		public int Pants;
		public int Shoes;
		public int MHandDress;
		public int ZBackEquip;

		public TAvatar (int i){
			Body = 2;
			Hair = 2;
			Cloth = 5;
			Pants = 6;
			Shoes = 1;
			MHandDress = 0;
			AHeadDress = 0;
			ZBackEquip = 0;
		}
	}
	public struct TAvatarTexture{
		public string BTexture;
		public string HTexture;
		public string ATexture;
		public string CTexture;
		public string PTexture;
		public string STexture;
		public string MTexture;
		public string ZTexture;

		public TAvatarTexture(int i){
			BTexture = "2_B_0_0";
			CTexture = "2_C_5_0";
			HTexture = "2_H_2_0";
			MTexture = "2_M_2_0";
			PTexture = "2_P_6_0";
			STexture = "2_S_1_0";
			ATexture = "3_A_1_0";
			ZTexture = "3_Z_1_0";
		}
	}
}

public static class GameData 
{
	public static TTactical[] TacticalData;
}
