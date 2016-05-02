﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameEnum;
using JetBrains.Annotations;
using UnityEngine;

namespace GameStruct
{
    public struct TPlayerBank
    {
		public int RoleIndex;
		public int ID;
        public int Lv;
        public int HeadTextureNo;
		public string Name;
        public TSkill[] SkillCards;
		public TSkillCardPage[] SkillCardPages;
		public Dictionary<EAttribute, int> Potential;

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Name); }
        }

        public override string ToString()
        {
            return string.Format("ID:{0}, Name:{1}, Lv:{2}, RoleIndex:{3}", ID, Name, Lv, RoleIndex);
        }
    }

	public struct TFriend {
		public string Identifier;
		public int Kind; //1.auto deploy 2.follow 3.waiting answer 4.be friend 5.ask follow
        public int FightCount;
        public DateTime Time;
		public TPlayer Player;
		public int[] SuitCardCost;
		public Dictionary<int, int> GotAvatar; //key: item id, value: 1 : got already

		public void PlayerInit() {
			if (Player.Name == null)
				Player.Name = "";

			Player.SetAttribute(ESkillType.Player);
			Player.SetAvatar();
			if(!string.IsNullOrEmpty(GameData.Team.Identifier)) {
				GameData.Team.AddSuitCardEffect(SuitCardCost, Player.Lv);
				GameData.Team.AddSuitItemEffect(GotAvatar, Player.Lv);
			}
		}
	}

    public struct TSocialEvent {
        public string _id;
        public string Identifier;
        public string TargetID;
        public string Name;
        public TPlayer Player;
        public DateTime Time;
        public Dictionary<string, string> Good;
        public int Kind; //1.friend 2.stage 3.mission 4.item 5.pvp
        public int Cause;
        public int Value;
        public int Num;

        private int mGoodCount;
        public int GoodCount {
            get {
                mGoodCount = 0;
                if (Good != null) {
                    foreach (KeyValuePair<string, string> item in Good) {
                        if (!string.IsNullOrEmpty(item.Value))
                            mGoodCount++;
                    }
                }
                    
                return mGoodCount;
            }

            set {
                mGoodCount = value;
            }
        }
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

		public int Index; //目前為了給進化用

	    public override string ToString()
	    {
			return string.Format("ID: {0}, Lv: {1}, Exp: {2}, SN: {3}", ID, Lv, Exp, SN);
	    }

		public void Reset () {
			this.ID = 0;
			this.Lv = 0;
			this.Exp = 0;
			this.SN = 0;
		}
	}

    public struct TSkillRecommend {
        public int Rate;
        public int[] IDs;
    }

    /// <summary>
    /// 這對應到 baseattr 表格的數值.
    /// </summary>
	public class TPlayerAttribute
    {
		public float PointRate2;
		public float PointRate3;
		public float StealRate;
		public float DunkRate;
		public float TipInRate;
		public float AlleyOopRate;
		public float StrengthRate;
		public float BlockBePushRate;
		public float BlockPushRate;
		public float ElbowingRate;
		public float ReboundRate;
		public float BlockRate;
		public float FaketBlockRate;
//		public float JumpBallRate;
		public float PushingRate;
		public float PassRate;
		public float AlleyOopPassRate;
		public float ShootingRate;

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

		public int APMax;
		public int APBegin;
		public float APGrowth ;

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
		BlockBePushRate,
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
		public int HeadTextureNo;
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
        public int GameKind; //0.pve, 1:instance
		public int GamePlayTime;
        public int ExitCount;
        public int PauseCount;
        public bool HaveFriend;
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
            HaveFriend = false;
			Done = false;
            IsWin = false;
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
        public int FriendCount;
        public int GoodCount;
        public int RenameCount;
        public int PVECount;
        public int PVEWin;
        public int PVEKeepWin;
        public int PVPCount;
        public int PVPWin;
        public int PVPKeepWin;
		public int PVPIntegral;
		public int PVPTicket;
        public int InstanceCount;
        public int InstanceWin;
        public int InstanceKeepWin;
        public int OccupyCount;
        public int OccupyWin;
        public int OccupyKeepWin;

		public int SkillEvolution;
		public int SkillReinforce;

		public int BuyStaminaQuantity;
		public int BuyDiamondQuantity;
		public int BuyCoinQuantity;

        public int ValueItemInlayNum; // 數值裝鑲嵌次數.
        public int ValueItemUpgradeNum; // 數值裝升級次數.

        public int LoginNum; // 終生登入次數.
        public int ReceivedLoginNum; // 終生登入的領獎記錄.

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
                newRecord.FriendCount != FriendCount ||
                newRecord.GoodCount != GoodCount ||
                newRecord.PVECount != PVECount ||
                newRecord.PVEWin != PVEWin ||
                newRecord.PVEKeepWin != PVEKeepWin ||
                newRecord.PVPCount != PVPCount ||
                newRecord.PVPWin != PVPWin ||
                newRecord.PVPKeepWin != PVPKeepWin ||
                newRecord.InstanceCount != InstanceCount ||
                newRecord.InstanceWin != InstanceWin ||
                newRecord.InstanceKeepWin != InstanceKeepWin ||
                newRecord.OccupyCount != OccupyCount ||
                newRecord.OccupyWin != OccupyWin ||
                newRecord.OccupyKeepWin != OccupyKeepWin ||
				newRecord.SkillEvolution != SkillEvolution ||
				newRecord.SkillReinforce != SkillReinforce ||
				newRecord.BuyCoinQuantity != BuyCoinQuantity ||
				newRecord.BuyDiamondQuantity != BuyDiamondQuantity ||
				newRecord.BuyDiamondQuantity != BuyDiamondQuantity)
                return true;
            else
                return false;
        }
    }
        
	public struct TGamePlayerRecord {
        public string Identifier;
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
            Identifier = "";
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

    public struct TDailyCount {
        public int FreshShop;
        public int FreshFriend;
		public int BuyPowerOne;
		public int BuyPowerTwo;
        public int BuyPVPCDCount;
        public int BuyPVPTicketCount;
		public int PVPReaward;
    }

	public struct TGameSetting {
		public int AnnouncementDate;
		public int AnnouncementDaily;
		public int AIChangeTimeLv;
		public int Quality;
		public bool GameMove;
		public bool GameRotation;
		public bool Effect;
		public bool Music;
		public bool Sound;
		public bool ShowEvent;
        public bool ShowWatchFriend;
		public ELanguage Language;
        public DateTime SocialEventTime;
        public DateTime WatchFriendTime;
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
		public int MaxStar;
		public int PictureNo;
		public string MiniPicture;
		public string RectanglePicture;
		public string NameTW;
		public int SuitCard;
		public int Suititem;
		public string Animation;
		public string Situation;
		public int Sell;
		public int Money;
		public int space;
		public int spaceAdd;
		public int[] UpgradeMoney;
		public int[] UpgradeExp;
		public int[] UpgradeMaterial;//該卡牌能合成的卡牌
		public int Exp;
		public int AddExp;
		public int EvolutionSkill;
		public int EvolutionMoney;
		public int Material1;
		public int MaterialNum1;
		public int Material2;
		public int MaterialNum2;
		public int Material3;
		public int MaterialNum3;
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
		public int MaxAngerBase;
		public int MaxAngerAdd;
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

		public int[] TargetKinds{
			get {
				int[] targetkind = new int[3];
				targetkind[0] = TargetKind1;
				targetkind[1] = TargetKind2;
				targetkind[2] = TargetKind3;
				return targetkind;
			}
		}

		public int[] TargetEffects{
			get {
				int[] targeteffect = new int[3];
				targeteffect[0] = TargetEffect1;
				targeteffect[1] = TargetEffect2;
				targeteffect[2] = TargetEffect3;
				return targeteffect;
			}
		}

		public int[] EffectParents{
			get {
				int[] effectparent = new int[3];
				effectparent[0] = EffectParent1;
				effectparent[1] = EffectParent2;
				effectparent[2] = EffectParent3;
				return effectparent;
			}
		}

		public float[] DelayTimes{
			get {
				float[] delay = new float[3];
				delay[0] = DelayTime1;
				delay[1] = DelayTime2;
				delay[2] = DelayTime3;
				return delay;
			}
		}

		public float[] Durations{
			get {
				float[] duration = new float[3];
				duration[0] = Duration1;
				duration[1] = Duration2;
				duration[2] = Duration3;
				return duration;
			}
		}

		public int MaxAnger (int lv) {
			return MaxAngerBase + lv * MaxAngerAdd;
		}

		public int GetUpgradeExp(int lv) {
			if(lv < 0)
				lv = 0;
			if(lv >= UpgradeExp.Length)
				lv = UpgradeExp.Length - 1;
			return UpgradeExp[lv];
		}

		public int GetUpgradeMoney(int lv) {
			if(lv < 0)
				lv = 0;
			if(lv > UpgradeExp.Length)
				lv = UpgradeExp.Length;
			return UpgradeMoney[lv];
		}

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
			return Mathf.Max((space + lv * spaceAdd) - (GameData.Team.SuitItemCompleteCount(Suititem) / 2) + GameData.Team.SuitCardCostEffect(SuitCard), 1) ;
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

    public struct TMaterialItem
    {
        public int ID;
        public int Num;
    }

	public enum EAttribute
	{
		Point2,//命中率
		Point3,//腕力＝計算投籃命中衰減的加成
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

		//Record Build's Index in ItemArray
		public int Index;

        // 0:Body, 1:Hair, 2:AHandDress, 3:Cloth, 4:Pants, 5:Shoes, 6:MHeadDress, 7:ZBackEquip.
        // 11 ~ 16: 數值裝.
        // 17,18: 次數型數值裝.
        // 19:鑲嵌材料.
        // 21:技能卡牌.
        // 31.錢,
        // 32.鑽石.
        // 33.經驗值.
        // 34.體力.
        // 35.PVP使用次數.
        // 39.聯盟幣.
        // 40.社群幣.
        public int Kind;
		public int LV;
		public int Atlas;

		public int Value;

		public int Position;
		public int Avatar;
		public int MaxStack;
		public int UseTime;
		public int Potential;

		public int SuitCard;
		public int SuitItem;

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

        public TMaterialItem[] ReviseMaterials
        {
            get
            {
                if(mMaterials == null)
                {
                    List<TMaterialItem> materials = new List<TMaterialItem>();

                    if(GameData.DItemData.ContainsKey(Material1) && MaterialNum1 > 0)
                        materials.Add(new TMaterialItem { ID = Material1, Num = MaterialNum1});

                    if(GameData.DItemData.ContainsKey(Material2) && MaterialNum2 > 0)
                        materials.Add(new TMaterialItem { ID = Material2, Num = MaterialNum2 });

                    if(GameData.DItemData.ContainsKey(Material3) && MaterialNum3 > 0)
                        materials.Add(new TMaterialItem { ID = Material3, Num = MaterialNum3 });

                    if(GameData.DItemData.ContainsKey(Material4) && MaterialNum4 > 0)
                        materials.Add(new TMaterialItem { ID = Material4, Num = MaterialNum4 });

                    mMaterials = materials.ToArray();
                }

                return mMaterials;
            }
        }

        private TMaterialItem[] mMaterials;

//        private int[] MaterialItemIDs
//        {
//            get { return mMaterialItemIDs ?? (mMaterialItemIDs = new []{Material1, Material2, Material3, Material4}); }
//        }
//
//        private int[] MaterialNums
//        {
//            get { return mMaterialNums ?? (mMaterialNums = new []{MaterialNum1, MaterialNum2, MaterialNum3, MaterialNum4}); }
//        }

//        public int AvailableMaterialNum
//        {
//            get
//            {
//                int num = 0;
//                for(var i = 0; i < MaterialItemIDs.Length; i++)
//                {
//                    if(MaterialItemIDs[i] > 0 && MaterialNums[i] > 0)
//                        ++num;
//                }
//
//                return num;
//            }
//        }

//        private int[] mMaterialItemIDs;
//        private int[] mMaterialNums;
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
        [UsedImplicitly]
        public int UpgradeLv { get; private set; }

        [UsedImplicitly, CanBeNull]
        public int[] StageSource { get; private set; }
        [UsedImplicitly, CanBeNull]
        public int[] UISource { get; private set; }

		/// <summary>
		/// 主要是開關特效 1是打開
		/// </summary>
		public int Flag;
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
                string name;
                switch(GameData.Setting.Language)
                {
                    case ELanguage.TW:
                        name = NameTW;
                        break;
                    case ELanguage.CN:
                        name = NameCN;
                        break;
                    case ELanguage.JP:
                        name = NameJP;
                        break;
                    default:
                        name = NameEN;
                        break;
                }

                if(Kind == 31 || Kind == 32 || Kind == 33 || Kind == 39 || Kind == 40)
                    return string.Format(name, Value);
                return name;
            }
        }

        public string Explain
        {
            get
            {
				string explain;
                switch(GameData.Setting.Language)
                {
				case ELanguage.TW: 
					explain = ExplainTW;
					break;
				case ELanguage.CN: 
					explain = ExplainCN;
					break;
				case ELanguage.JP: 
					explain = ExplainJP;
					break;
                default: 
					explain = ExplainEN;
					break;
                }
				if(Kind == 31 || Kind == 32 || Kind == 33 || Kind == 39 || Kind == 40)
					return string.Format(explain, Value);
				return explain;
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
        public float Wait;
        public int Value;
		public int Offsetx;
		public int Offsety;
		public int TalkIndex;
		public int TalkL;
		public int TalkR;
		public int ActionL;
		public int ActionR;
		public string UIPath;
        public string HintPath;
        public string ScalePath;
		public string TextTW;
		public string TextCN;
		public string TextJP;
		public string TextEN;
        public string HintTW;
        public string HintCN;
        public string HintJP;
        public string HintEN;
		
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

        public string Hint {
            get{
                switch(GameData.Setting.Language){
                    case ELanguage.TW: return HintTW;
                    case ELanguage.CN: return HintCN;
                    case ELanguage.JP: return HintJP;
                    default: return HintEN;
                }
            }
        }
	}

    public struct TMission {
        public int ID;
        public int Kind;
        public int Lv;
        public int Color;
        public int Score;
		public int PrivousID;
		public int Final;
        public int TimeKind;
        public int TimeValue;
        public int SpendKind;

        public int[] Value;

        public int GetAppropriateValue(int value)
        {
            for(var i = 0; i < Value.Length; i++)
            {
                if(value < Value[i])
                    return Value[i];
            }

            return Value[Value.Length - 1];
        }

        public int[] Money;
        public int[] Exp;
        public int[] Diamond;
        public int[] AwardID;
        public int[] AwardNum;
        public string OpenUI;

        [UsedImplicitly]
        private string nameTW;
        [UsedImplicitly]
        private string nameCN;
        [UsedImplicitly]
        private string nameEN;
        [UsedImplicitly]
        private string nameJP;
        [UsedImplicitly]
        private string explainTW;
        [UsedImplicitly]
        private string explainCN;
        [UsedImplicitly]
        private string explainEN;
        [UsedImplicitly]
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
		public string TacticalName; // for debug.

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
		public int ID;
		public int Order;
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

    /// <summary>
    /// SpendKind
    /// 0.鑽石
    /// 1.遊戲幣
    /// 2.聯盟幣
    /// 3.社群幣
    /// </summary>

    public struct TSellItem {
        public int ID;
        public int Num;
        public int SpendKind;
        public int Price;
    }

	public struct TShop {
		public int Kind;
        public int Kind2;
		public int SpendKind;
		public int Price;
		/// <summary>
		/// length次數限制
		/// []代表價格
		/// </summary>
		public int[] Limit;
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
		public int Kind;
		public int Rate;
		public int Order;
		public int Sale;
		public string pricetw;
		public string priceen;
		public string pricecn;
		public string pricejp;
		public string Nametw;
		public string Nameen;
		public string Namecn;
		public string Namejp;

		public int Diamond {
			get
			{
				switch(GameData.Setting.Language)
				{
				case ELanguage.TW: return Diamonds;
				case ELanguage.CN: return Diamondcn;
				case ELanguage.JP: return Diamonds;
				default : return Diamonds;
				}
			}
		}

		public string Price {
			get
			{
				switch(GameData.Setting.Language)
				{
				case ELanguage.TW: return pricetw;
				case ELanguage.CN: return pricecn;
				case ELanguage.JP: return pricejp;
				default : return priceen;
				}
			}
		}

		public string Name {
			get
			{
				switch(GameData.Setting.Language)
				{
				case ELanguage.TW: return Nametw;
				case ELanguage.CN: return Namecn;
				case ELanguage.JP: return Namejp;
				default : return Nameen;
				}
			}
		}
	}
        
	public struct TPVPData
	{
        public int Lv;
        public string NameTW;
        public string NameCN;
        public string NameEN;
        public string NameJP;
		public int LowScore;
		public int HighScore;
		public int BasicScore;
		public int Calculate;
        public int PVPCoin;
        public int PVPCoinDaily;
        public int Stage;
        public int SearchCost;

        public string Name
        {
            get{
                switch(GameData.Setting.Language)
                {
                    case ELanguage.TW: return NameTW;
                    case ELanguage.CN: return NameCN;
                    case ELanguage.JP: return NameJP;
                    default : return NameEN;
                }
            }
        }
	}

    public struct TTeamRank
    {  
        public int Index;
        public TTeam Team;
    }

    public struct TPVPStart
    {
		public int PVPTicket;
		public DateTime PVPCD;
    }

	public struct TPVPResult
	{
        public int PVPLv;
		public int PVPIntegral;
        public int PVPCoin;
		public TTeamRecord LifetimeRecord;
	}

    public struct TPVPEnemyTeams
    {
        public int Money;
        public TTeam[] Teams;
        public int PVPEnemyIntegral;
    }

    public struct TPVPBuyResult
    {
        public int Diamond;
        public int PVPTicket;
        public DateTime PVPCD;
        public TDailyCount DailyCount;
    }

	public struct TPVPReward
	{
		public int PVPCoin;
		public TDailyCount DailyCount;		
	}

	public struct TSuitCard 
	{
		public int ID;
		public string SuitNameTW;
		public string SuitNameCN;
		public string SuitNameEN;
		public string SuitNameJP;
		public int[] Item1;
		public int[] Item2;
		public int[] Item3;
		public int CardPower;
		public int[] StarNum;
		public int[] AttrKind1;
		public int[] Value1;
		public int[] AttrKind2;
		public int[] Value2;

		public int ItemCount {
			get {
				return 3;
			}
		}

		public int[][] Items {
			get 
			{
				int[][] items = new int[3][];
				items[0] = Item1;
				items[1] = Item2;
				items[2] = Item3;
				return items;
			}
		}

		public struct TAttrSet {
			public int AttrKind;
			public int AttrValue;
		}

		public int[][] AttrKinds {
			get 
			{
				int[][] attrKind = new int[2][];
				attrKind[0] = AttrKind1;
				attrKind[1] = AttrKind2;
				return attrKind;
			}
		}

		public int[][] Values {
			get 
			{
				int[][] values = new int[2][];
				values[0] = Value1;
				values[1] = Value2;
				return values;
			}
		}


		public string SuitName {
			get
			{
				switch(GameData.Setting.Language)
				{
				case ELanguage.TW: return SuitNameTW;
				case ELanguage.CN: return SuitNameCN;
				case ELanguage.JP: return SuitNameJP;
				default : return SuitNameEN;
				}
			}
		}
	}

	public struct TSuitItem {
		public int ID;
		public string SuitNameTW;
		public string SuitNameCN;
		public string SuitNameEN;
		public string SuitNameJP;
		public int Position;
		public int Card1;
		public int Card2;
		public int Card3;
		public int Card4;
		public int Card5;
		public int CardLength;
		public int Item1;
		public int Item2;
		public int Item3;
		public int Item4;
		public int Item5;
		public int Item6;
		public int Item7;
		public int ItemLength;
		public int AttrKind1;
		public int Value1;
		public int AttrKind2;
		public int Value2;
		public int AttrKind3;
		public int Value3;
		public int AttrKind4;
		public int Value4;
		public int AttrKind5;
		public int Value5;
		public int AttrKind6;
		public int Value6;

		public int[] Card;
		public int[] Items;
		public int[] AttrKind;
		public int[] Value;

		public void LengthCalCulate () {

			Card = new int[5];
			Card[0] = Card1;
			Card[1] = Card2;
			Card[2] = Card3;
			Card[3] = Card4;
			Card[4] = Card5;

			Items = new int[7];
			Items[0] = Item1;
			Items[1] = Item2;
			Items[2] = Item3;
			Items[3] = Item4;
			Items[4] = Item5;
			Items[5] = Item6;
			Items[6] = Item7;

			AttrKind = new int[6];
			AttrKind[0] = AttrKind1;
			AttrKind[1] = AttrKind2;
			AttrKind[2] = AttrKind3;
			AttrKind[3] = AttrKind4;
			AttrKind[4] = AttrKind5;
			AttrKind[5] = AttrKind6;

			Value = new int[6];
			Value[0] = Value1;
			Value[1] = Value2;
			Value[2] = Value3;
			Value[3] = Value4;
			Value[4] = Value5;
			Value[5] = Value6;

			ItemLength = 0;
			for (int i=0; i<Items.Length; i++) 
				if(Items[i] != 0)
					ItemLength ++;

			CardLength = 0;
			for (int i=0; i<Card.Length; i++) 
				if(Card[i] != 0)
					CardLength ++;
		}

		public string SuitName {
			get
			{
				switch(GameData.Setting.Language)
				{
				case ELanguage.TW: return SuitNameTW;
				case ELanguage.CN: return SuitNameCN;
				case ELanguage.JP: return SuitNameJP;
				default : return SuitNameEN;
				}
			}
		}
	}

    public struct TPotentital
    {
        public int ID;
        public int Point2;
        public int Point3;
        public int Dunk;    
        public int Rebound; 
        public int Block; 
        public int Steal;
	}

	public struct TArchitectureExp {
		public int LV;

		public float GymTime;
		public int GymSpendKind;
		public int GymCost;
		public int GymAttrKind;
		public float GymAttrValue;

		public float BasketTime;
		public int BasketSpendKind;
		public int BasketCost;
		public int BasketAttrKind;
		public float BasketAttrValue;

		public float AdTime;
		public int AdSpendKind;
		public int AdCost;
		public int AdAttrKind;
		public float AdAttrValue;

		public float StoreTime;
		public int StoreSpendKind;
		public int StoreCost;
		public int StoreAttrKind;
		public float StoreAttrValue;

		public float ChairTime;
		public int ChairSpendKind;
		public int ChairCost;
		public int ChairAttrKind;
		public float ChairAttrValue;

		public float DoorTime;
		public int DoorSpendKind;
		public int DoorCost;
		public int DoorAttrKind;
		public float DoorAttrValue;

		public float LogoTime;
		public int LogoSpendKind;
		public int LogoCost;
		public int LogoAttrKind;
		public float LogoAttrValue;

		public float CalendarTime;
		public int CalendarSpendKind;
		public int CalendarCost;
		public int CalendarAttrKind;
		public float CalendarAttrValue;

		public float MailTime;
		public int MailSpendKind;
		public int MailCost;
		public int MailAttrKind;
		public float MailAttrValue;
	}
		
	public struct TGymBuild {
		public int LV;
		public int ItemID;
		public DateTime Time;
	} 

	public struct TGymQueue {
		public int Index;//紀錄位置
		public bool IsOpen;
		public int BuildIndex;

		public void ChangePos(int buildIndex) {
			BuildIndex = buildIndex;
		}
	}

	public struct TMailGift{
		public int Kind;
		public int Param1;
		public int Param2;
	}


	public struct TMailInfo{
		public string _id;
		public DateTime Time;
		public string FromIdentifier;
		public string FromName;
		public string FromLv;
		public int FromHeadTextureNo;
		public string ToIdentifier;
		public int MailKind; // 1=prize, 2=social
		public int ContextType; // 1=native string, 2=string id
		public string Context;
		public TMailGift[] Gifts;

	}
}
