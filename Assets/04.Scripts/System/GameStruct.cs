using UnityEngine;
using System.Collections;
using System;

namespace GameStruct
{
	public struct TTeam 
	{
		public string Identifier;
		public string sessionID;
		public string FBName;
		public string FBid;
		public DateTime PowerCD;
		public DateTime FreeLuckBox;

		public int Money;
		public int Power;
		public int Diamond;

		public TPlayer Player;

		public void Init() {
			if (Identifier == null)
				Identifier = "";

			if (sessionID == null)
				sessionID = "";

			if (FBName == null)
				FBName = "";

			if (FBid == null)
				FBid = "";

			Player.SetAvatar();
		} 
	}

    public struct TPlayer {
        public int ID;
        public string Name;
		public int Lv;
        public int AILevel;
		public int Point2;	
		public int Point3;
		public int Steal;	
		public int Speed;
		public int Dunk;
		public int Strength;
		public int Rebound;
		public int Block;	
		public int Stamina;	
		public int Dribble;	
		public int Defence;
		public int Pass;
		public int BodyType;

		public TAvatar Avatar;

		public TPlayer(int Level)
		{
			AILevel = Level;
			ID = 0;
			Name = "";
			Lv = 0;
			Point2 = 0;
			Point3 = 0;
			Steal = 0;
			Speed = 0;
			Dunk = 0;
			Strength = 0;
			Rebound = 0;
			Block = 0;
			Stamina = 0;
			Dribble = 0;
			Defence = 0;
			Pass = 0;
			BodyType = 0;
			Avatar = new TAvatar(0);
		}

		public void SetAvatar() {
			if (GameData.DPlayers.ContainsKey(ID)) {
				Avatar.Body = GameData.DPlayers[ID].Body;
				Avatar.Hair = GameData.DPlayers[ID].Hair;
				Avatar.AHeadDress = GameData.DPlayers[ID].AHeadDress;
				Avatar.Cloth = GameData.DPlayers[ID].Cloth;
				Avatar.Pants = GameData.DPlayers[ID].Pants;
				Avatar.Shoes = GameData.DPlayers[ID].Shoes;
				Avatar.MHandDress = GameData.DPlayers[ID].MHandDress;
				Avatar.ZBackEquip = GameData.DPlayers[ID].ZBackEquip;
			}
		}
    }

	public struct TPlayerAttribute
	{
		public int PointRate2;
		public int PointRate3;
		public int StealRate;
		public int DunkRate;
		public int TipInRate;
		public int AlleyOopRate;
		public int StrengthRate;
		public int BlockPushRate;
		public int ElbowingRate;
		public int ReboundRate;
		public int BlockRate;
		public int FaketBlockRate;
		public int JumpBallRate;
		public int PushingRate;
		public int PassRate;
		public int AlleyOopPassRate;

		public float ReboundHeadDistance;
		public float ReboundHandDistance;
		public float BlockDistance;
		public float DefDistance;
		public float SpeedValue;
		public float StaminaValue;
		public float AutoFollowTime;
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

	public struct TGreatPlayer {
		public int ID;
		public string Name;
		public int Point2;
		public int Point3;
		public int Steal;
		public int Speed;
		public int Dunk;
		public int Strength;
		public int Rebound;
		public int Block;
		public int Stamina;
		public int Dribble;
		public int Defence;
		public int Pass;
		public int BodyType;
		public int AILevel;
		public int Body;
		public int Hair;
		public int Cloth;
		public int Pants;
		public int Shoes;
		public int MHandDress;
		public int AHeadDress;
		public int ZBackEquip;
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
