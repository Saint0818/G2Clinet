using System;
using System.Collections.Generic;
using System.Linq;
using GameEnum;
using JetBrains.Annotations;
using UnityEngine;

namespace GameStruct
{
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
		public TSkill[] SkillCards;
		public TSkillCardPage[] SkillCardPages;
		public TItem[] Items;
		public Dictionary<EAttribute, int> Potential;

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Name); }
        }

        public override string ToString()
        {
            return string.Format("ID:{0}, Name:{1}, RoleIndex:{2}", ID, Name, RoleIndex);
        }
    }

	public struct TFriend {
		public string Identifier;
		public int Kind;
		public TPlayer Player;
	}

	public struct TSkillCardPage {
		public int[] SNs;

		public TSkillCardPage(int i) {
			SNs = new int[0];
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

    public struct TSkillRecommend {
        public int Rate;
        public int[] IDs;
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

		public float PushDistance;
		public float PushExtraAngle;
		public float StealDistance;
		public float StealExtraAngle;
		public float ElbowDistance;
		public float ElbowExtraAngle;

        /// <summary>
        /// 懲罰時間, 單位:秒. 
        /// </summary>
	    public float PunishTime = GameConst.DefaultPunishTime;
    }

	public enum EPlayerAttributeRate
	{
		Point2Rate,
		Point3Rate,
		SpeedValue,
		StaminaValue,
		StrengthRate,
		BlockPushRate,
		ElbowingRate,
		DunkRate,
		TipInRate,
		AlleyOopRate,
		ReboundRate,
		ReboundHeadDistance,
		ReboundHandDistance,
		BlockRate,
		FakeBlockrate,
		BlockDistance,
		DefDistance,
		PushingRate,
		StealRate,
		PassRate,
		AlleyOopPassRate,
		PushDistance,
		PushExtraAngle,
		ElbowDistance,
		ElbowExtraAngle,
		StealDistance,
		StealExtraAngle
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

        public bool HaveAvatar {
            get { return Body > 0 || Hair > 0 || AHeadDress > 0 || Cloth > 0 ||
                         Pants > 0 || Shoes > 0 || MHandDress > 0 || ZBackEquip > 0; }
        }
	}

    /// <summary>
    /// 這是直接對應到企劃表格(GreatPlayer)的資料結構.
    /// </summary>
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
        public int GameKind; //0.pve
		public int GamePlayTime;
        public int ExitCount;
        public int PauseCount;
		public bool Done;
        public bool IsWin;
        public int StageID;
		public int Score1;
		public int Score2;
		
		public string[] ButtonTrace;
		public TGamePlayerRecord[] PlayerRecords;

		public void Init(int playerNumber) {
			Identifier = "";
			Version = 0;
			Start = DateTime.UtcNow;
			GamePlayTime = 0;
			ExitCount = 0;
			PauseCount = 0;
			Done = false;
			Score1 = 0;
			Score2 = 0;
			ButtonTrace = new string[0];
			PlayerRecords = new TGamePlayerRecord[playerNumber];
			for (int i = 0; i < playerNumber; i ++)
				PlayerRecords[i].Init();
		}
	}

    public struct TDailyRecord {
        public Dictionary<int, int> MissionLv; //key: mission id, value: lv
        public TTeamRecord TeamRecord;
        public TGamePlayerRecord PlayerRecord;
    }

    public struct TTeamRecord
    {
        public int TotalAddDiamond; //kind 1
        public int TotalDelDiamond;
        public int BuyDiamond;
        public int TotalAddMoney; //kind 2   
        public int TotalDelMoney;
        public int BuyMoney;
        public int TotalAddPower; //kind 3
        public int TotalDelPower;
        public int BuyPower;
        public int GameTime;
        public int GamePlayTime;
        public int Lv;
        public int Exp;
        public int PVPLv; //挑戰積分(PVP積分) kind 5
        public int StatiumLv; //球場等級 kind 6
        public int OccupyLv; //踢館等級 kind 7
        public int AvatarCount; //kind 8
        public int SkillCount; //kind 9
        public int PVECount;
        public int PVEWin;
        public int PVEKeepWin;
        public int PVPCount;
        public int PVPWin;
        public int PVPKeepWin;
        public int SubTextCount;
        public int SubTextWin;
        public int SubTextKeepWin;
        public int OccupyCount;
        public int OccupyWin;
        public int OccupyKeepWin;

        public bool NeedForSync(ref TTeamRecord newRecord) {
            if (/*newRecord.TotalAddDiamond != TotalAddDiamond ||
                newRecord.TotalDelDiamond != TotalDelDiamond ||
                newRecord.BuyDiamond != BuyDiamond ||
                newRecord.TotalAddMoney != TotalAddMoney ||
                newRecord.TotalDelMoney != TotalDelMoney ||
                newRecord.BuyMoney != BuyMoney ||
                newRecord.GameTime != GameTime ||
                newRecord.GamePlayTime != GamePlayTime ||*/
                newRecord.Lv != Lv ||
                newRecord.PVPLv != PVPLv ||
                newRecord.StatiumLv != StatiumLv ||
                newRecord.OccupyLv != OccupyLv ||
                newRecord.AvatarCount != AvatarCount ||
                newRecord.SkillCount != SkillCount ||
                newRecord.PVECount != PVECount ||
                newRecord.PVEWin != PVEWin ||
                newRecord.PVEKeepWin != PVEKeepWin ||
                newRecord.PVPCount != PVPCount ||
                newRecord.PVPWin != PVPWin ||
                newRecord.PVPKeepWin != PVPKeepWin ||
                newRecord.SubTextCount != SubTextCount ||
                newRecord.SubTextWin != SubTextWin ||
                newRecord.SubTextKeepWin != SubTextKeepWin ||
                newRecord.OccupyCount != OccupyCount ||
                newRecord.OccupyWin != OccupyWin ||
                newRecord.OccupyKeepWin != OccupyKeepWin)
                return true;
            else
                return false;
        }
    }
        
	public struct TGamePlayerRecord {
		public int ID;
        public int GameCount;
        public int GamePlayTime;
		public int FG;
		public int FGIn;
		public int FG3;
		public int FG3In;
        public int Score;
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
        public int DoubleClickLaunch;
        public int DoubleClickPerfact;

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
            DoubleClickLaunch = 0;
            DoubleClickPerfact = 0;

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
		public int AnnouncementDate;
		public int AnnouncementDaily;
		public int AIChangeTimeLv;
		public ELanguage Language;
        public int Quality;
		public bool Effect;
		public bool Music;
		public bool Sound;
		public Dictionary<int, int> NewAvatar;
	}

	public struct TPlayerPackage {

	}

	public struct TPlayerSkill {

	}
	
	public struct TSkillData {
		public int ID;
		public int Kind; 
		public int Quality; //Card Color
//		public int Star; // Space
		public int MaxStar;
		public int PictureNo;
		public string NameTW;
		public string Animation;
		public string Situation;
		public int Sell;
		public int Money;
		public int space;
		public int spaceAdd;
		public int[] UpgradeMoney;
		public int[] UpgradeExp;
		public int Exp;
		public int AddExp;
		public float lifeTime;
		public float lifeTimeAdd;
		public int AttrKind;
		public float valueBase;
		public float valueAdd;
		public int rate;
		public int rateAdd;
		public int AniRateKind;
		public int aniRate;
		public int aniRateAdd;
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
		public int Open; //skillheaditem
		
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
			return Mathf.Max(space + lv * spaceAdd, 1);
		}

		public float LifeTime(int lv) {
			return lifeTime + lv * lifeTimeAdd;
		}

		public int Rate(int lv) {
			return rate + lv * rateAdd;
		}

		public int AniRate(int lv) {
			return aniRate + lv * aniRateAdd;
		}

		public float Distance(int lv) {
			return distance + lv * distanceAdd;
		}

		public int Angle(int lv) {
			return angle + lv * angleAdd;
		} 

		public int ExpInlay (int lv) {
			return Exp + lv * AddExp;
		}
	}

	public struct TItem
    {
		public int ID;
		public int UseKind;
		public DateTime UseTime;

        public override string ToString()
	    {
	        return string.Format("ID: {0}, UseKind: {1}, UseTime: {2}", ID, UseKind, UseTime);
	    }
	}

	public struct TValueItem
    {
		public int ID;

        /// <summary>
        /// 鑲嵌物品的 ItemID.(這個只是用在接 Json 讀出的資料, 不要使用, 改使用 RealInlayItemIDs)
        /// </summary>
        [CanBeNull]public int[] InlayItemIDs;

	    [NotNull]
	    public int[] RevisionInlayItemIDs
	    {
	        get
	        {
                if(!GameData.DItemData.ContainsKey(ID))
                    return new int[0];
                TItemData item = GameData.DItemData[ID];

                if(InlayItemIDs == null)
	                InlayItemIDs = new int[item.AvailableMaterialNum];
                else if(InlayItemIDs.Length < item.AvailableMaterialNum)
                {
                    // 不足的部份補上 0.
                    int[] revisionInlayItemIDs = new int[item.AvailableMaterialNum];
                    Array.Copy(InlayItemIDs, revisionInlayItemIDs, InlayItemIDs.Length);
                    InlayItemIDs = revisionInlayItemIDs;
                }
                return InlayItemIDs;
	        }
	    }

	    public bool HasInlay(int itemID)
	    {
	        for(var i = 0; i < RevisionInlayItemIDs.Length; i++)
	        {
	            if(RevisionInlayItemIDs[i] == itemID)
	                return true;
	        }
	        return false;
	    }

	    public override string ToString()
	    {
	        return string.Format("ID: {0}, InlayItemIDs: {1}", ID, InlayItemIDs);
	    }
    }

    public struct TMaterialItem
    {
        public int ID;
        public int Num;
    }

	public enum EAttribute
	{
		Point2,
		Point3,
		Dunk,
		Rebound,
		Block,
		Steal,
		Speed,
		Stamina,
		Strength,
		Defence,
		Dribble,
		Pass
	}

    /// <summary>
    /// 道具額外的屬性加成 or 比賽優勢加成.
    /// </summary>
	public enum EBonus
    {
        None = 0,
        Point2 = 1,
        Point3 = 2,
        Speed = 3,
        Stamina = 4,
        Strength = 5,
        Dunk = 6,
        Rebound = 7,
        Block = 8,
        Defence = 9,
        Steal = 10,
        Dribble = 11,
        Pass = 12,
        V13 = 13, // 13.士氣 (單次）.
        V14 = 14, // 14.士氣 (時間內+%)時間內獲得到的  士氣都會額外乘上一個％
        Score = 15, // 15.分數
        SumScore = 16, // 16.總分數
        Time = 17 // 17.時間
    }

    /// <summary>
    /// 對應到企劃表格 item.json.
    /// </summary>
	public struct TItemData
    {
		public int ID;

        // [0]:Body, [1]:Hair, [2]:AHandDress, [3]:Cloth, [4]:Pants
        // [5]:Shoes, [6]:MHeadDress, [7]:ZBackEquip.
        // [11] ~ [18] 數值裝備(企劃尚未定義).
        // [19]:鑲嵌材料, [21]:技能卡牌
        public int Kind;
		public int LV;
		public int Atlas;

		public int Value;

		public int Position;
		public int Avatar;
		public int MaxStack;
		public int UseTime;
		public int Potential;

        public EBonus[] Bonus
        {
            get
            {
                // https://msdn.microsoft.com/zh-tw/library/ms173224.aspx
                return mAttrKinds ?? (mAttrKinds = new[] {AttrKind1, AttrKind2, AttrKind3});
            }
        }

        public int[] BonusValues
        {
            get { return mAttrValues ?? (mAttrValues = new []{AttrValue1, AttrValue2, AttrValue3}); }
        }

        private EBonus[] mAttrKinds;
        private int[] mAttrValues;
        public EBonus AttrKind1;
        public int AttrValue1;
        public EBonus AttrKind2;
        public int AttrValue2;
        public EBonus AttrKind3;
        public int AttrValue3;

        public int[] Materials
        {
            get { return mMaterials ?? (mMaterials = new []{Material1, Material2, Material3, Material4}); }
        }

        public int[] MaterialNums
        {
            get { return mMaterialNums ?? (mMaterialNums = new []{MaterialNum1, MaterialNum2, MaterialNum3, MaterialNum4}); }
        }

        public int AvailableMaterialNum
        {
            get
            {
                int num = 0;
                for(var i = 0; i < Materials.Length; i++)
                {
                    if(Materials[i] > 0 && MaterialNums[i] > 0)
                        ++num;
                }

                return num;
            }
        }

        public bool HasMaterial(int itemID)
        {
            for(var i = 0; i < Materials.Length; i++)
            {
                if(Materials[i] == itemID)
                    return true;
            }
            return false;
        }

        public int FindMaterialNum(int itemID)
        {
            for(var i = 0; i < Materials.Length; i++)
            {
                if(Materials[i] == itemID)
                    return MaterialNums[i];
            }

            return 0;
        }

        private int[] mMaterials;
        private int[] mMaterialNums;
        [UsedImplicitly]
        public int Material1 { get; private set; }
        [UsedImplicitly]
        public int MaterialNum1 { get; private set; }
        [UsedImplicitly]
        public int Material2 { get; private set; }
        [UsedImplicitly]
        public int MaterialNum2 { get; private set; }
        [UsedImplicitly]
        public int Material3 { get; private set; }
        [UsedImplicitly]
        public int MaterialNum3 { get; private set; }
        [UsedImplicitly]
        public int Material4 { get; private set; }
        [UsedImplicitly]
        public int MaterialNum4 { get; private set; }

        [UsedImplicitly]
        public int UpgradeItem { get; private set; }
        [UsedImplicitly]
        public int UpgradeMoney { get; private set; }

        [UsedImplicitly, CanBeNull]
        public int[] StageSource { get; private set; }
        [UsedImplicitly, CanBeNull]
        public int[] UISource { get; private set; }

        public int Buy;
        public int Sell;
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

        /// <summary>
        /// 取得道具全部的數值加成.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public int GetSumBonusValue(EBonus kind)
        {
            int sum = 0;
            for(var i = 0; i < Bonus.Length; i++)
            {
                if(Bonus[i] == kind)
                    sum += BonusValues[i];
            }

            return sum;
        }

        public int GetTotalPoints()
        {
            return BonusValues.Sum();

            /*
            int sum = 0;
            for(var i = 0; i < BonusValues.Length; i++)
            {
                sum += BonusValues[i];
            }

            return sum;
            */
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

    /// <summary>
    /// 這是 Exp 企劃表格內的某一列資料.
    /// </summary>
	public struct TExpData
	{
		public int Lv;
		public int LvUpExp;
		public int OpenIndex;
		public int UnlockHint;
		public int UI;
		public int UnlockName;
		public int TutorialID;
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
		public int ActionL;
		public int ActionR;
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

    public struct TMission {
        public int ID;
        public int Kind;
        public int Color;
        public int Score;
		public int PrivousID;
		public int Final;
        public int TimeKind;
        public int TimeValue;
        public int[] Value;
        public int[] Money;
        public int[] Exp;
        public int[] Diamond;
        public int[] AwardID;
        public int[] AwardNum;
        public string OpenUI;
        private string nameTW;
        private string nameCN;
        private string nameEN;
        private string nameJP;
        private string explainTW;
        private string explainCN;
        private string explainEN;
        private string explainJP;

        public string Name
        {
            get
            {
                switch(GameData.Setting.Language)
                {
                    case ELanguage.TW: return nameTW;
                    case ELanguage.CN: return nameCN;
                    case ELanguage.JP: return nameJP;
                    default : return nameEN;
                }
            }
        }

        public string Explain
        {
            get
            {
                switch(GameData.Setting.Language)
                {
                    case ELanguage.TW: return explainTW;
                    case ELanguage.CN: return explainCN;
                    case ELanguage.JP: return explainJP;
                    default : return explainEN;
                }
            }
        }
    }

	public struct TMoveData
	{
			public Vector2 Target
			{
					get { return mTarget; }
			}

			private Vector2 mTarget;

			/// <summary>
			/// 當移動到定點的時候, 要往哪個方向看.
			/// </summary>
			[CanBeNull]
			public Transform LookTarget;
			public Transform FollowTarget;
			public PlayerBehaviour DefPlayer;
			public OnPlayerAction2 MoveFinish;
			public bool Speedup;
			public bool Catcher;
			public bool Shooting;
			public string TacticalName;
			// for debug.

			public void Clear()
			{
					mTarget = Vector2.zero;
					LookTarget = null;
					MoveFinish = null;
					FollowTarget = null;
					DefPlayer = null;
					Speedup = false;
					Catcher = false;
					Shooting = false;
					TacticalName = "";
			}

			public void SetTarget(float x, float y)
			{
					mTarget.Set(x, y);
			}
	}

	[System.Serializable]
	public struct TScoreRate
	{
			public int TwoScoreRate;
			public float TwoScoreRateDeviation;
			public int ThreeScoreRate;
			public float ThreeScoreRateDeviation;
			public int DownHandScoreRate;
			public int DownHandSwishRate;
			public int DownHandAirBallRate;
			public int UpHandScoreRate;
			public int UpHandSwishRate;
			public int UpHandAirBallRate;
			public int NormalScoreRate;
			public int NormalSwishRate;
			public int NormalAirBallRate;
			public int NearShotScoreRate;
			public int NearShotSwishRate;
			public int NearShotAirBallRate;
			public int LayUpScoreRate;
			public int LayUpSwishRate;
			public int LayUpAirBallRate;

			public TScoreRate(int flag)
			{
					TwoScoreRate = 70;
					TwoScoreRateDeviation = 0.8f;
					ThreeScoreRate = 50;
					ThreeScoreRateDeviation = 0.5f;
					DownHandScoreRate = -60;
					DownHandSwishRate = -50;
					DownHandAirBallRate = 35;
					UpHandScoreRate = 20;
					UpHandSwishRate = -30;
					UpHandAirBallRate = 15;
					NormalScoreRate = 0;
					NormalSwishRate = 0;
					NormalAirBallRate = 8;
					NearShotScoreRate = 10;
					NearShotSwishRate = -10;
					NearShotAirBallRate = 3;
					LayUpScoreRate = 20;
					LayUpSwishRate = -20;
					LayUpAirBallRate = 2;
			}
	}

	public class TSkillAttribute
	{
			public int ID;
			public int Kind;
			public float Value;
			public float CDTime;
	}

	public struct TPickCost {
		public int Order;
		public int Kind;
		public int SpendKind;
		public int OnePick;
		public int FivePick;
		public int TenPick;
		public int[] ShowCard;
		public int[] ShowItem;
		public string Prefab;
		public int StartTimeYear;
		public int StartTimeMonth;
		public int StartTimeDay;
		public int FinishTimeYear;
		public int FinishTimeMonth;
		public int FinishTimeDay;
		public int FreeTime;
		public string nameTW;
		public string nameCN;
		public string nameEN;
		public string nameJP;
		public string explainTW;
		public string explainCN;
		public string explainEN;
		public string explainJP;

		public string Name
		{
			get
			{
				switch(GameData.Setting.Language)
				{
				case ELanguage.TW: return nameTW;
				case ELanguage.CN: return nameCN;
				case ELanguage.JP: return nameJP;
				default : return nameEN;
				}
			}
		}

		public string Explain
		{
			get
			{
				switch(GameData.Setting.Language)
				{
				case ELanguage.TW: return explainTW;
				case ELanguage.CN: return explainCN;
				case ELanguage.JP: return explainJP;
				default : return explainEN;
				}
			}
		}
	}

	public struct TShop {
		public int Kind;
		public int SpendKind;
		public string Price;
		public int Sale;//銷售語的種類
		public int ItemID;
		public int Pic;
		public int Order;
		public int StartTimeYear;
		public int StartTimeMonth;
		public int StartTimeDay;
		public int FinishTimeYear;
		public int FinishTimeMonth;
		public int FinishTimeDay;
	}

	public struct TMall {
		public int Diamonds;
		public int Diamondcn;
		public string Android;
		public string Ios;
		public int Order;
		public int Sale;
		public float pricetw;
		public float priceen;
		public float pricecn;
		public float pricejp;

		public string Price {
			get
			{
				switch(GameData.Setting.Language)
				{
				case ELanguage.TW: return "NT " + pricetw.ToString();
				case ELanguage.CN: return "RMB¥" + pricecn.ToString();
				case ELanguage.JP: return "¥" + pricejp.ToString();
				default : return "$" + priceen.ToString();
				}
			}
		}
	}
}
