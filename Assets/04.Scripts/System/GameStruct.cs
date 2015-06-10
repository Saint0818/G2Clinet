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
		public float Point2;	
		public float Point3;
		public float Steal;	
		public float Speed;
		public float Dunk;
		public float Strength;
		public float Rebound;
		public float Block;	
		public float Stamina;	
		public float Dribble;	
		public float Defence;
		public float Pass;
		public int BodyType;

		public TAvatar Avatar;
		public TSkill ActiveSkill;
		public TSkill[] Skills;

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
			ActiveSkill = new TSkill();
			Skills = new TSkill[0];
		}

		public void SetAttribute() {
			if (GameData.DPlayers.ContainsKey(ID)) {
				Point2 = GameData.DPlayers[ID].Point2;
				Point3 = GameData.DPlayers[ID].Point3;
				Steal = GameData.DPlayers[ID].Steal;
				Speed = GameData.DPlayers[ID].Speed;
				Dunk = GameData.DPlayers[ID].Dunk;
				Strength = GameData.DPlayers[ID].Strength;
				Rebound = GameData.DPlayers[ID].Rebound;
				Block = GameData.DPlayers[ID].Block;
				Stamina = GameData.DPlayers[ID].Stamina;
				Dribble = GameData.DPlayers[ID].Dribble;
				Defence = GameData.DPlayers[ID].Defence;
				Pass = GameData.DPlayers[ID].Pass;
				BodyType = GameData.DPlayers[ID].BodyType;
				AILevel = GameData.DPlayers[ID].AILevel;
				ActiveSkill.ID = GameData.DPlayers[ID].ActiveSkill;
				ActiveSkill.Lv = GameData.DPlayers[ID].ActiveSkillLV;
				Skills = new TSkill[14];
				Skills[0].ID =  GameData.DPlayers[ID].Skill1;
				Skills[0].Lv =  GameData.DPlayers[ID].SkillLV1;
				Skills[1].ID =  GameData.DPlayers[ID].Skill2;
				Skills[1].Lv =  GameData.DPlayers[ID].SkillLV2;
				Skills[2].ID =  GameData.DPlayers[ID].Skill3;
				Skills[2].Lv =  GameData.DPlayers[ID].SkillLV3;
				Skills[3].ID =  GameData.DPlayers[ID].Skill4;
				Skills[3].Lv =  GameData.DPlayers[ID].SkillLV4;
				Skills[4].ID =  GameData.DPlayers[ID].Skill5;
				Skills[4].Lv =  GameData.DPlayers[ID].SkillLV5;
				Skills[5].ID =  GameData.DPlayers[ID].Skill6;
				Skills[5].Lv =  GameData.DPlayers[ID].SkillLV6;
				Skills[6].ID =  GameData.DPlayers[ID].Skill7;
				Skills[6].Lv =  GameData.DPlayers[ID].SkillLV7;
				Skills[7].ID =  GameData.DPlayers[ID].Skill8;
				Skills[7].Lv =  GameData.DPlayers[ID].SkillLV8;
				Skills[8].ID =  GameData.DPlayers[ID].Skill9;
				Skills[8].Lv =  GameData.DPlayers[ID].SkillLV9;
				Skills[9].ID =  GameData.DPlayers[ID].Skill10;
				Skills[9].Lv =  GameData.DPlayers[ID].SkillLV10;
				Skills[10].ID =  GameData.DPlayers[ID].Skill11;
				Skills[10].Lv =  GameData.DPlayers[ID].SkillLV11;
				Skills[11].ID =  GameData.DPlayers[ID].Skill12;
				Skills[11].Lv =  GameData.DPlayers[ID].SkillLV12;
				Skills[12].ID =  GameData.DPlayers[ID].Skill13;
				Skills[12].Lv =  GameData.DPlayers[ID].SkillLV13;
				Skills[13].ID =  GameData.DPlayers[ID].Skill14;
				Skills[13].Lv =  GameData.DPlayers[ID].SkillLV14;
			}
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

	public struct TSkill {
		public int ID;
		public int Lv;
		public int Exp;
	}

	public struct TPlayerAttribute
	{
		public float PointRate2;
		public float PointRate3;
		public float StealRate;
		public float DunkRate;
		public float TipInRate;
		public float AlleyOopRate;
		public float StrengthRate;
		public float BlockPushRate;
		public float ElbowingRate;
		public float ReboundRate;
		public float BlockRate;
		public float FaketBlockRate;
		public float JumpBallRate;
		public float PushingRate;
		public float PassRate;
		public float AlleyOopPassRate;

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

		public int ActiveSkill;
		public int ActiveSkillLV;
		public int Skill1;
		public int SkillLV1;
		public int Skill2;
		public int SkillLV2;
		public int Skill3;
		public int SkillLV3;
		public int Skill4;
		public int SkillLV4;
		public int Skill5;
		public int SkillLV5;
		public int Skill6;
		public int SkillLV6;
		public int Skill7;
		public int SkillLV7;
		public int Skill8;
		public int SkillLV8;
		public int Skill9;
		public int SkillLV9;
		public int Skill10;
		public int SkillLV10;
		public int Skill11;
		public int SkillLV11;
		public int Skill12;
		public int SkillLV12;
		public int Skill13;
		public int SkillLV13;
		public int Skill14;
		public int SkillLV14;
	}

	public struct TScenePlayer {
		public float X;
		public float Z;
		public float TX;
		public float TZ;
		public float Dir;
		public float Speed;
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

	public struct TPlayerPackage{

	}

	public struct TPlayerSkill{
	}
	
	public struct TSkillData{
		public int ID;
		public int Kind; 
		public int Star;
		public int MaxStar;
		public int PictureNo;
		public string NameTW;
		public string NameEN;
		private string name;
		public string ExplainTW;
		public string ExplainEN;
		private string explain;
		public int Space;
		public int AddSpace;
		public float LifeTime;
		public int Role;
		public int AttrKind;
		public float BaseValue;
		public int BaseRate;
		public int AddRate;
		public string Animation;
		public int Effect;
		public int TargetEffect;
		public int TargetAnimation;
		
		
		public string Name
		{
			get {
				switch(GameData.Setting.Language) {
				case Language.TW:
					name =  NameTW;
					break;
				case Language.EN:
					name =  NameEN;
					break;
				default:
					name =  NameEN;
					break;
				}
				return name;
			}
		}
		
		public string Explain
		{
			get{
				switch(GameData.Setting.Language) {
				case Language.TW:
					explain =  ExplainTW;
					break;
				case Language.EN:
					explain =  ExplainEN;
					break;
				default:
					explain =  ExplainEN;
					break;
				}
				return explain;
			}
		}
	}
}
