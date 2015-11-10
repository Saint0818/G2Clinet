using System;
using System.Collections.Generic;
using GameEnum;
using UnityEngine;

namespace GameStruct {
	public struct TTeam
    {
		public string Identifier;
		public string sessionID;
		public string FBName;
		public string FBid;
		public DateTime LoginTime;
		public DateTime PowerCD;
		public DateTime FreeLuckBox;
	    public int PlayerNum; // 玩家擁有幾位角色.

		public int Money;
		public int Power;
		public int Diamond;
		public int MasteriesPoint;

		public int[] TutorialFlags;
		public TPlayer Player;
		public TItem[] Items;
		public TSkill[] SkillCards;
		public TPlayerBank[] PlayerBank;
		public TMail[] Mails;

		public void Init() {
			if (Identifier == null)
				Identifier = "";

			if (sessionID == null)
				sessionID = "";

			if (FBName == null)
				FBName = "";

			if (FBid == null)
				FBid = "";

			Player.Init();
		}

	    public override string ToString()
	    {
	        return string.Format("PlayerNum:{0}", PlayerNum);
	    }

		public bool HaveTutorialFlag(int id) {
			if (TutorialFlags != null) {
				for (int i = 0; i < TutorialFlags.Length; i++) 
					if (TutorialFlags[i] == id)
						return true;
			}

			return false;
		}

		public void AddTutorialFlag(int id) {
			if (TutorialFlags == null) 
				TutorialFlags = new int[0];

			Array.Resize(ref TutorialFlags, TutorialFlags.Length+1);
			TutorialFlags[TutorialFlags.Length-1] = id;
		}

		public void RemoveTutorialFlag(int index) {
			if (TutorialFlags == null) 
				TutorialFlags = new int[0];
			
			if (index >= 0 && index < TutorialFlags.Length)
				TutorialFlags[index] = -1;
		}
    }

    public struct TLookUpData
    {
        public int SelectedRoleIndex;
        public TPlayerBank[] PlayerBanks;

        public override string ToString()
        {
            return string.Format("SelectedRoleIndex: {0}", SelectedRoleIndex);
        }
    }

    public struct TPlayerBank
    {
		public int RoleIndex;
		public int ID;
        public int Lv;
		public string Name;
		public TItem[] Items;

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Name); }
        }

        public override string ToString()
        {
            return string.Format("ID:{0}, Name:{1}, RoleIndex:{2}", ID, Name, RoleIndex);
        }
    }

	public struct TSkillCardPage {
		public int[] SNs;

		public TSkillCardPage(int i) {
			SNs = new int[0];
		}
	}

    public struct TPlayer {
		public int RoleIndex;
        public int ID;
        public string Name;
		public int Lv;
        public int AILevel;
		public float Point2; //kind1
		public float Point3; //kind2
		public float Speed; //kind3
		public float Stamina; //kind4
		public float Strength; //kind5
		public float Dunk; //kind6
		public float Rebound; //kind7
		public float Block;	//kind8
		public float Defence; //kind9
		public float Steal;	//kind10
		public float Dribble; //kind11
		public float Pass; //kind12
		public int BodyType; // 2:後衛, 1:前鋒, 0:中鋒.
        public int MaxSkillSpace;
        public int AISkillLv;
		public int SkillPage;// 0 1 2 3 4
		public int NowStageID;
		public int[] Masteries;
        /// <summary>
        /// 玩家當日的挑戰次數. key: stageID, value: 挑戰次數.
        /// </summary>
        public Dictionary<int, int> StageChallengeNums;

        public TAvatar Avatar;
		public List<TSkill> ActiveSkills;
		public TSkill[] SkillCards;
		public TSkillCardPage[] SkillCardPages;
		public TItem[] Items;
		public TEquipItem[] EquipItems;

        /// <summary>
        /// 主線關卡進度(下一個可以打的關卡). 範圍是 101 ~ 2000.
        /// 101: 剛開始, 沒打過任何關卡.
        /// 102: StageID = 101 的關卡已經過關了.
        /// 105: StageID = 101,102,103,104 的關卡已經過關了.
        /// </summary>
        public int NextMainStageID;

		public TPlayer(int level)
		{
			AILevel = level;
			Name = "";
			RoleIndex = 0;
			ID = 0;
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
			MaxSkillSpace = 0;
		    AISkillLv = 0;
			SkillPage = 0;
			NowStageID = 0;

			Avatar = new TAvatar(1);
			ActiveSkills = new List<TSkill>();
			SkillCards = new TSkill[0];
			SkillCardPages = new TSkillCardPage[0];
			Items = new TItem[0];
			EquipItems = new TEquipItem[0];
			Masteries = new int[0];
		    NextMainStageID = StageTable.MinMainStageID;
            StageChallengeNums = new Dictionary<int, int>();
		}

        public override string ToString()
        {
            return string.Format("ID:{0}, Name:{1}", ID, Name);
        }

        public void Init() {
			if (Name == null)
				Name = "";
			
			SetAttribute();
			SetAvatar();
		} 

		public void SetID(int id) {
			ID = id;
			SetAttribute(ESkillType.NPC);
			SetAvatar();
		}

		public void SetAttribute(ESkillType type = ESkillType.Player) {
			if (ID > 0 && GameData.DPlayers.ContainsKey(ID)) {
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
			    AISkillLv = GameData.DPlayers[ID].AISkillLv;
				SetSkill(type);
			}
		}

		public void SetSkill (ESkillType type){
			if(ActiveSkills == null)
				ActiveSkills = new List<TSkill>();

			switch (type) {
				case ESkillType.NPC:
				if(GameData.DPlayers.ContainsKey(ID)) {
					if(SkillCards == null || SkillCards.Length == 0) 
						SkillCards = new TSkill[14];
					SkillCards[0].ID = GameData.DPlayers[ID].Skill1;
					SkillCards[0].Lv = GameData.DPlayers[ID].SkillLV1;
					SkillCards[1].ID = GameData.DPlayers[ID].Skill2;
					SkillCards[1].Lv = GameData.DPlayers[ID].SkillLV2;
					SkillCards[2].ID = GameData.DPlayers[ID].Skill3;
					SkillCards[2].Lv = GameData.DPlayers[ID].SkillLV3;
					SkillCards[3].ID = GameData.DPlayers[ID].Skill4;
					SkillCards[3].Lv = GameData.DPlayers[ID].SkillLV4;
					SkillCards[4].ID = GameData.DPlayers[ID].Skill5;
					SkillCards[4].Lv = GameData.DPlayers[ID].SkillLV5;
					SkillCards[5].ID = GameData.DPlayers[ID].Skill6;
					SkillCards[5].Lv = GameData.DPlayers[ID].SkillLV6;
					SkillCards[6].ID = GameData.DPlayers[ID].Skill7;
					SkillCards[6].Lv = GameData.DPlayers[ID].SkillLV7;
					SkillCards[7].ID = GameData.DPlayers[ID].Skill8;
					SkillCards[7].Lv = GameData.DPlayers[ID].SkillLV8;
					SkillCards[8].ID = GameData.DPlayers[ID].Skill9;
					SkillCards[8].Lv = GameData.DPlayers[ID].SkillLV9;
					SkillCards[9].ID = GameData.DPlayers[ID].Skill10;
					SkillCards[9].Lv = GameData.DPlayers[ID].SkillLV10;
					SkillCards[10].ID = GameData.DPlayers[ID].Skill11;
					SkillCards[10].Lv = GameData.DPlayers[ID].SkillLV11;
					SkillCards[11].ID = GameData.DPlayers[ID].Skill12;
					SkillCards[11].Lv = GameData.DPlayers[ID].SkillLV12;
					SkillCards[12].ID = GameData.DPlayers[ID].Skill13;
					SkillCards[12].Lv = GameData.DPlayers[ID].SkillLV13;
					SkillCards[13].ID = GameData.DPlayers[ID].Skill14;
					SkillCards[13].Lv = GameData.DPlayers[ID].SkillLV14;

					ActiveSkills.Clear();
					for(int i=0; i<SkillCards.Length; i++) {
						if(SkillCards[i].ID > GameConst.ID_LimitActive && SkillCards[i].ID > 0){
							if(!ActiveSkills.Contains(SkillCards[i]))
								ActiveSkills.Add(SkillCards[i]);
						}
					}
				}
					break;
				case ESkillType.Player:
					ActiveSkills.Clear();
					if(SkillCards == null)
						SkillCards = new TSkill[0];
					if(SkillCards.Length > 0) {
						for(int i=0; i<SkillCards.Length; i++) {
							if(SkillCards[i].ID >= GameConst.ID_LimitActive){
								if(!ActiveSkills.Contains(SkillCards[i]))
									ActiveSkills.Add(SkillCards[i]);
							}
						}
					}
				break;

			}
		}
		
		public void SetAvatar() {
			if (ID > 0 && GameData.DPlayers.ContainsKey(ID)) {
				Avatar.Body = GameData.DPlayers[ID].Body;
				Avatar.Hair = GameData.DPlayers[ID].Hair;
				Avatar.AHeadDress = GameData.DPlayers[ID].AHeadDress;
				Avatar.Cloth = GameData.DPlayers[ID].Cloth;
				Avatar.Pants = GameData.DPlayers[ID].Pants;
				Avatar.Shoes = GameData.DPlayers[ID].Shoes;
				Avatar.MHandDress = GameData.DPlayers[ID].MHandDress;
				Avatar.ZBackEquip = GameData.DPlayers[ID].ZBackEquip;

				if (Items != null) {
					for (int i = 0; i < Items.Length; i ++) {
						if (GameData.DItemData.ContainsKey(Items[i].ID)) {
							TItemData item = GameData.DItemData[Items[i].ID];
							switch (item.Kind) {
							case 0:
								Avatar.Body = item.Avatar;
								break;
							case 1:
								Avatar.Hair = item.Avatar;
								break;
							case 2:
								Avatar.MHandDress = item.Avatar;
								break;
							case 3:
								Avatar.Cloth = item.Avatar;
								break;
							case 4:
								Avatar.Pants = item.Avatar;
								break;
							case 5:
								Avatar.Shoes = item.Avatar;
								break;
							case 6:
								Avatar.AHeadDress = item.Avatar;
								break;
							case 7:
								Avatar.ZBackEquip = item.Avatar;
								break;
							}
						}
					}
				}
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns>-1: can't find.</returns>
        public int GetBodyItemID()
        {
            return getItemID(0);
        }

        public int GetHairItemID()
        {
            return getItemID(1);
        }

        public int GetHeadItemID()
        {
            return getItemID(6);
        }

        public int GetClothItemID()
        {
            return getItemID(3);
        }

        public int GetPantsItemID()
        {
            return getItemID(4);
        }

        public int GetShoesItemID()
        {
            return getItemID(5);
        }

        public int GetHandItemID()
        {
            return getItemID(2);
        }

        public int GetBackItemID()
        {
            return getItemID(7);
        }

        private int getItemID(int kind)
        {
            if(Items == null)
                return -1;

            for(int i = 0; i < Items.Length; i++)
            {
                if(!GameData.DItemData.ContainsKey(Items[i].ID))
                    continue;

                TItemData item = GameData.DItemData[Items[i].ID];
                if(item.Kind == kind)
                    return item.ID;
            }

            return -1;
        }

		public void AddAttribute(int kind, float value) {
			switch (kind) {
			case 1:
				Point2 += value;
				break;
			case 2:
				Point3 += value;
				break;
			case 3:
				Speed += value;
				break;
			case 4:
				Stamina += value;
				break;
			case 5:
				Strength += value;
				break;
			case 6:
				Dunk += value;
				break;
			case 7:
				Rebound += value;
				break;
			case 8:
				Block += value;
				break;
			case 9:
				Defence += value;
				break;
			case 10:
				Steal += value;
				break;
			case 11:
				Dribble += value;
				break;
			case 12:
				Pass += value;
				break;
			}
		}

		public int MaxAnger{
			get{
//				if(ActiveSkills.Count > 0) {
//					int maxAnger = 0;
//					for (int i=0; i<ActiveSkills.Count; i++) {
//						if (GameData.DSkillData.ContainsKey(ActiveSkills[i].ID))
//							maxAnger += GameData.DSkillData[ActiveSkills[i].ID].MaxAnger;
//					}
//					return maxAnger;
//				} else
//					return 0;
				return 100;
			}
		}

		public bool CheckIfMaxAnger (int activeID, int value) {
			if(ActiveSkills.Count > 0) 
				return (value >= MaxAngerOne(activeID));
	
			return false;
		}

		public int MaxAngerOne(int activeID){
			if(ActiveSkills.Count > 0) 
				if (GameData.DSkillData.ContainsKey(activeID))
					return GameData.DSkillData[activeID].MaxAnger;

			return 0;
		}

		public float MaxAngerPercent(int activeID, float value){
			if(ActiveSkills.Count > 0) 
				if (GameData.DSkillData.ContainsKey(activeID)){
				float p = (value / GameData.DSkillData[activeID].MaxAnger);
					if( p > 1)
						return 1;
					else
						return p;
				}

			return 0;
		}

		public string SkillAnimation (int activeID) {
			if (GameData.DSkillData.ContainsKey(activeID))
				return GameData.DSkillData[activeID].Animation;
			else
				return string.Empty;
				
		}
	}

	public struct TSkill {
		public int ID;
		public int Lv;
		public int Exp;
		public int SN;

	    public override string ToString()
	    {
			return string.Format("ID: {0}, Lv: {1}, Exp: {2}, SN: {3}", ID, Lv, Exp, SN);
	    }
	}

	public class TPlayerAttribute
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

        /// <summary>
        /// 懲罰時間, 單位:秒. 
        /// </summary>
	    public float PunishTime = GameConst.DefaultPunishTime;
    }

	public struct TAvatar {	
		public int Body;
		public int Hair; // Item.Avatar
		public int AHeadDress; // 頭飾(共用).
        public int Cloth;
		public int Pants;
		public int Shoes;
		public int MHandDress; // 手飾.
        public int ZBackEquip; // 背飾(共用).

        public TAvatar (int id){
			if (GameData.DPlayers.ContainsKey(id)) {
				Body = GameData.DPlayers[id].Body;
				Hair = GameData.DPlayers[id].Hair;
				Cloth = GameData.DPlayers[id].Cloth;
				Pants = GameData.DPlayers[id].Pants;
				Shoes = GameData.DPlayers[id].Shoes;
				MHandDress = GameData.DPlayers[id].MHandDress;
				AHeadDress = GameData.DPlayers[id].AHeadDress;
				ZBackEquip = GameData.DPlayers[id].ZBackEquip;
			} else {
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

	    public override string ToString()
	    {
	        return string.Format("Body:{0}, Hair:{1}, Cloth:{2}, Pants:{3}, Shoes:{4}", Body, Hair, Cloth, Pants, Shoes);
	    }
	}

	public struct TGreatPlayer {
		public int ID;
		public string NameTW;
		public string NameCN;
		public string NameEN;
		public string NameJP;
		public string ExplainTW;
		public string ExplainCN;
		public string ExplainEN;
		public string ExplainJP;
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
		public int BodyType; // 0:C, 1:F, 2:G.
		public int AILevel;
		public int AISkillLv;
		public int Body;
		public int Hair;
		public int Cloth;
		public int Pants;
		public int Shoes;
		public int MHandDress;
		public int AHeadDress;
		public int ZBackEquip;

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

		public string Name {
			get {
				switch (GameData.Setting.Language) {
				case ELanguage.TW: return NameTW;
				case ELanguage.CN: return NameCN;
				case ELanguage.JP: return NameJP;
				default : return NameEN;
				}
			}
		}

		public string Explain {
			get {
				switch (GameData.Setting.Language) {
				case ELanguage.TW: return ExplainTW;
				case ELanguage.CN: return ExplainCN;
				case ELanguage.JP: return ExplainJP;
				default : return ExplainEN;
				}
			}
		}
	}

	public struct TScenePlayer {
		public float X;
		public float Z;
		public float TX;
		public float TZ;
		public float Dir;
		public float Speed;
	}

	public struct TMoveRecord {
		public float X;
		public float Z;
	}

	public struct TShotRecord {
		public bool In;
		public int Kind;
		public float Rate;
		public TMoveRecord Move;
	}

	public struct TGameRecord {
		public string Identifier;
		public float Version;
		public DateTime Start;
		public DateTime End;
		public float GameTime;
		public float ExitCount;
		public float PauseCount;
		public bool Done;
		public int Score1;
		public int Score2;
		public int DoubleClickLaunch;
		public int DoubleClickLv1;
		public int DoubleClickLv2;
		public int DoubleClickLv3;
		public string[] ButtonTrace;
		public TGamePlayerRecord[] PlayerRecords;

		public void Init(int playerNumber) {
			Identifier = "";
			Version = 0;
			Start = DateTime.UtcNow;
			GameTime = 0;
			ExitCount = 0;
			PauseCount = 0;
			Done = false;
			Score1 = 0;
			Score2 = 0;
			DoubleClickLaunch = 0;
			DoubleClickLv1 = 0;
			DoubleClickLv2 = 0;
			DoubleClickLv3 = 0;
			ButtonTrace = new string[0];
			PlayerRecords = new TGamePlayerRecord[playerNumber];
			for (int i = 0; i < playerNumber; i ++)
				PlayerRecords[i].Init();
		}
	}

	public struct TGamePlayerRecord {
		public int ID;
		public int FG;
		public int FGIn;
		public int FG3;
		public int FG3In;
		public int ShotError;
		public int Fake;
		public int BeFake;
		public int ReboundLaunch;
		public int Rebound;
		public int Assist;
		public int BeIntercept;
		public int Pass;
		public int StealLaunch;
		public int Steal;
		public int BeSteal;
		public int Intercept;
		public int BlockLaunch;
		public int Block;
		public int BeBlock;
		public int PushLaunch;
		public int Push;
		public int BePush;
		public int ElbowLaunch;
		public int Elbow;
		public int BeElbow;
		public int Knock;
		public int BeKnock;
		public int AlleyoopLaunch;
		public int Alleyoop;
		public int TipinLaunch;
		public int Tipin;
		public int DunkLaunch;
		public int Dunk;
		public int SaveBallLaunch;
		public int SaveBall;
		public int AngerAdd;
		public int PassiveSkill;
		public int Skill;
		public TMoveRecord[] MoveRecords;
		public TShotRecord[] ShotRecords;

		public void Init() {
			FG = 0;
			FGIn = 0;
			FG3 = 0;
			FG3In = 0;
			ShotError = 0;
			Fake = 0;
			BeFake = 0;
			ReboundLaunch = 0;
			Rebound = 0;
			Assist = 0;
			BeIntercept = 0;
			Pass = 0;
			StealLaunch = 0;
			Steal = 0;
			BeSteal = 0;
			Intercept = 0;
			BlockLaunch = 0;
			Block = 0;
			BeBlock = 0;
			PushLaunch = 0;
			Push = 0;
			BePush = 0;
			ElbowLaunch = 0;
			Elbow = 0;
			BeElbow = 0;
			Knock = 0;
			BeKnock = 0;
			AlleyoopLaunch = 0;
			Alleyoop = 0;
			TipinLaunch = 0;
			Tipin = 0;
			DunkLaunch = 0;
			Dunk = 0;
			SaveBallLaunch = 0;
			SaveBall = 0;
			AngerAdd = 0;
			PassiveSkill = 0;
			Skill = 0;
			MoveRecords = new TMoveRecord[0];
			ShotRecords = new TShotRecord[0];
		}

		public void PushMove(Vector2 pos) {
			/*Array.Resize(ref MoveRecords, MoveRecords.Length + 1);
			MoveRecords[MoveRecords.Length-1].X = Mathf.Round(pos.x * 10) / 10;
			MoveRecords[MoveRecords.Length-1].Z = Mathf.Round(pos.y * 10) / 10;*/
		}

		public void PushShot(Vector2 pos, int kind, float rate) {
			/*Array.Resize(ref ShotRecords, ShotRecords.Length + 1);
			ShotRecords[ShotRecords.Length-1].Move.X = Mathf.Round(pos.x * 10) / 10;
			ShotRecords[ShotRecords.Length-1].Move.Z = Mathf.Round(pos.y * 10) / 10;
			ShotRecords[ShotRecords.Length-1].Kind = kind;
			ShotRecords[ShotRecords.Length-1].Rate = rate;*/
		}
	}

	public struct TGameSetting {
		public float AIChangeTime;
		public ELanguage Language;
		public bool Effect;
	}

	public struct TPlayerPackage {

	}

	public struct TPlayerSkill {

	}
	
	public struct TSkillData {
		public int ID;
		public int Kind; 
		public int Quality; //Card Color
		public int Star; // Space
		public int MaxStar;
		public int PictureNo;
		public string NameTW;
		public string Animation;
		public string Situation;
		public int Sell;
		public int Money;
		public int space;
		public int spaceAdd;
		public int UpgradeExp;
		public int ExtraExp;
		public float lifeTime;
		public float lifeTimeAdd;
		public int AttrKind;
		public float valueBase;
		public float valueAdd;
		public int rate;
		public int rateAdd;
		public int Direct;
		public float distance;
		public float distanceAdd;
		public int angle;
		public int angleAdd;
		public int MaxAnger;
		public int ActiveCamera;
		public float ActiveCameraTime;
		public int TargetKind;
		public int TargetKind1;
		public int TargetEffect1;
		public int EffectParent1;
		public float DelayTime1;
		public float Duration1;
		public int TargetKind2;
		public int TargetEffect2;
		public int EffectParent2;
		public float DelayTime2;
		public float Duration2;
		public int TargetKind3;
		public int TargetEffect3;
		public int EffectParent3;
		public float DelayTime3;
		public float Duration3;
		public string NameCN;
		public string NameEN;
		public string NameJP;
		public string ExplainTW;
		public string ExplainCN;
		public string ExplainEN;
		public string ExplainJP;
		
		public string Name {
			get {
				switch(GameData.Setting.Language) {
				case ELanguage.TW: return NameTW;
				case ELanguage.CN: return NameCN;
				case ELanguage.JP: return NameJP;
				default: return NameEN;
				}
			}
		}
		
		public string Explain {
			get{
				switch(GameData.Setting.Language) {
				case ELanguage.TW:return ExplainTW;
				case ELanguage.CN:return ExplainCN;
				case ELanguage.JP:return ExplainJP;
				default:return ExplainEN;
				}
			}
		}

		public float Value(int lv) {
			return valueBase + lv * valueAdd;
		}

		public int Space(int lv) {
			return Mathf.Max(space + Star * spaceAdd, 1);
		}

		public float LifeTime(int lv) {
			return lifeTime + lv * lifeTimeAdd;
		}

		public int Rate(int lv) {
			return rate + lv * rateAdd;
		}

		public float Distance(int lv) {
			return distance + lv * distanceAdd;
		}

		public int Angle(int lv) {
			return angle + lv * angleAdd;
		} 
	}

	public struct TItem {
		public int ID;
		public int UseKind;
		public DateTime UseTime;
	}

	public struct TEquipItem {
		public int ID;
		public int[] Inlay;
	}

	public struct TItemData
    {
		public int ID;

        // [0]:Body, [1]:Hair, [2]:AHandDress, [3]:Cloth, [4]:Pants
        // [5]:Shoes, [6]:MHeadDress, [7]:ZBackEquip
        public int Kind;

		public int Position;
		public int Avatar;
		public int MaxStack;
		public int UseTime;
		public int Money;
		public int Quality;
		public string Icon;
		public string NameTW;
		public string NameCN;
		public string NameEN;
		public string NameJP;
		public string ExplainTW;
		public string ExplainCN;
		public string ExplainEN;
		public string ExplainJP;

	    public override string ToString()
	    {
	        return string.Format("ID:{0}, Name:{1}, Explain:{2}", ID, Name, Explain);
	    }

	    public string Name
        {
            get
            {
                switch(GameData.Setting.Language)
                {
                    case ELanguage.TW: return NameTW;
                    case ELanguage.CN: return NameCN;
                    case ELanguage.JP: return NameJP;
                    default: return NameEN;
                }
            }
        }

        public string Explain
        {
            get
            {
                switch(GameData.Setting.Language)
                {
                    case ELanguage.TW: return ExplainTW;
                    case ELanguage.CN: return ExplainCN;
                    case ELanguage.JP: return ExplainJP;
                    default: return ExplainEN;
                }
            }
        }
    }

    public struct TPreloadEffect {
		public string Name;
	}

	public class TTutorial {
		public int ID;
		public int Line;
		public int Kind;
		public string Title;
		public string UIName;
		public int Value;
		public int Offsetx;
		public int Offsety;
		public int TalkIndex;
		public int TalkL;
		public int TalkR;
		public string UIpath;
		public string TextTW;
		public string TextCN;
		public string TextJP;
		public string TextEN;
		
		public string Text {
			get{
				switch(GameData.Setting.Language){
				case ELanguage.TW: return TextTW;
				case ELanguage.CN: return TextCN;
				case ELanguage.JP: return TextJP;
				default: return TextEN;
				}
			}
		}
	}
}
