using System;
using System.Collections.Generic;
using System.Linq;
using GameEnum;
using UnityEngine;

namespace GameStruct
{
    public struct TTeam
    {
        public string Identifier;
        public string sessionID;
        public string FBName;
        public string FBid;

        public DateTime LoginTime;
        public DateTime PowerCD;
        public DateTime FreshShopTime;
        public DateTime FreshFriendTime;
        public DateTime SocialEventTime;
        public DateTime WatchFriendsTime;
        public DateTime FreeLuckBox;
		public DateTime PVPCD;
		public DateTime[] LotteryFreeTime;

        public int StageTutorial;
        public int AvatarPotential;
        public int OccupyLv; //佔領球館等級
        public int StatiumLv; //經營球館等級
        public int SocialCoin; //社群幣

		public int SkillCardMax;//背包空間數
		/// <summary>
		/// 套卡有啟動的id就會紀錄id
		/// </summary>
		public int[] SuitCardCost; 

        public int[] TutorialFlags;
        public int[] Achievements;
        public Dictionary<int, int> GotItemCount; //key: item id, value: got number
        public Dictionary<int, int> GotAvatar; //key: item id, value: 1 : got already
        public Dictionary<int, int> MissionLv; //key: mission id, value: lv
        private bool needForSyncRecord;
        public TTeamRecord LifetimeRecord;
        public TDailyCount DailyCount;
        public TDailyRecord GroupRecord;
        public TDailyRecord DailyRecord;
        public TDailyRecord WeeklyRecord;
        public TDailyRecord MonthlyRecord;
        public TPlayer Player;
        public TItem[] Items;
        public TSellItem[] ShopItems1;
        public TSellItem[] ShopItems2;
        public TSellItem[] ShopItems3;
        public TValueItem[] ValueItems;
        public TMaterialItem[] MaterialItems;
        public TSkill[] SkillCards;
		public Dictionary<int, int> SkillCardCounts; //key: ID , value:num
        public TPlayerBank[] PlayerBank;
        public TMail[] Mails;
        public Dictionary<string, TFriend> Friends; //key: Identifier, value: TFriend
        public string[] EnemyIDs;

        /// <summary>
        /// [year][month][登入次數].
        /// [2016][2] 是 2016 年 2 月的登入次數.
        /// </summary>
        public Dictionary<int, Dictionary<int, int>> DailyLoginNums;

        public int GetDailyLoginNum(int year, int month)
        {
            if(DailyLoginNums != null && DailyLoginNums.ContainsKey(year) && DailyLoginNums[year].ContainsKey(month))
                return DailyLoginNums[year][month];
            return 0;
        }

        //PVP
        public int PVPCoin; //聯盟幣
        public int PVPLv;
        public int PVPIntegral;
		public int PVPDailyReaward;
        public int PVPEnemyIntegral;
		public int PVPTicket;
        public string LeagueName;
        public int LeagueIcon;

		public bool CheckFriend (string id) {
			if(Friends != null) {
				if(string.IsNullOrEmpty(id))
					return false;
				else 
					return Friends.ContainsKey(id); 
			} else 
				return false;
		}

        /// <summary>
        /// 玩家選擇的戰術.
        /// </summary>
        public ETacticalAuto AttackTactical;

        public bool HasMaterialItem(int itemID)
        {
            for(var i = 0; i < MaterialItems.Length; i++)
            {
                if (MaterialItems[i].ID == itemID)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="materialItem"></param>
        /// <returns> 材料在第幾個 Index.. </returns>
        public int FindMaterialItem(int itemID, ref TMaterialItem materialItem)
        {
            for(var i = 0; i < MaterialItems.Length; i++)
            {
                if(MaterialItems[i].ID == itemID)
                {
                    materialItem = MaterialItems[i];
                    return i;
                }
            }

            return -1;
        }

        public int FindMaterialItemIndex(int itemID)
        {
            for(var i = 0; i < MaterialItems.Length; i++)
            {
                if(MaterialItems[i].ID == itemID)
                    return i;
            }

            return -1;
        }


        public event Action<int> OnMoneyChangeListener;
        public int Money
        {
            get { return mMoney; }
            set
            {
                if(mMoney == value)
                    return;

                mMoney = value;

                PlayerPrefs.SetInt(ESave.MoneyChange.ToString(), 1);

                if(OnMoneyChangeListener != null)
                    OnMoneyChangeListener(mMoney);
            }
        }
        private int mMoney;

        public event Action<int> OnPowerChangeListener;
        public int Power
        {
            get { return mPower; }
            set
            {
                if(mPower == value)
                    return;

                mPower = value;

                PlayerPrefs.SetInt(ESave.PowerChange.ToString(), 1);

                if(OnPowerChangeListener != null)
                    OnPowerChangeListener(mPower);
            }
        }

        private int mPower;

        public event Action<int> OnDiamondChangeListener;
        public int Diamond
        {
            get { return mDiamond; }
            set
            {
                if(mDiamond == value)
                    return;
                mDiamond = value;

                PlayerPrefs.SetInt(ESave.DiamondChange.ToString(), 1);

                if(OnDiamondChangeListener != null)
                    OnDiamondChangeListener(mDiamond);
            }
        }
        private int mDiamond;

        public int MaxPlayerBank;

        public void Init() {
            if (Identifier == null)
                Identifier = "";

            if (sessionID == null)
                sessionID = "";

            if (FBName == null)
                FBName = "";

            if (FBid == null)
                FBid = "";

			if(LotteryFreeTime == null) {
				LotteryFreeTime = new DateTime[GameConst.MaxLotteryFreeTime];
				for(int i=0; i<GameConst.MaxLotteryFreeTime; i++){
					LotteryFreeTime[i] = DateTime.UtcNow;
				}
			}

            PlayerInit();
        }

		public void PlayerInit() {
			if (Player.Name == null)
				Player.Name = "";

			Player.SetAttribute(ESkillType.Player);
			Player.SetAvatar();
			AddSuitCardEffect(SuitCardCost, Player.Lv);
			AddSuitItemEffect(GotAvatar, Player.Lv);
		}

		/// <summary>
		/// Kind 0.Diamond 1.Money 2.PVPCoin 3.SocialCoin
		/// </summary>
		/// <returns><c>true</c>, if enough was coined, <c>false</c> otherwise.</returns>
		/// <param name="kind">Kind.</param>
		/// <param name="number">Number.</param>
        public bool CoinEnough(int kind, int number) {
            switch (kind) {
                case 0:
                    if (GameData.Team.Diamond < number)
                        return false;

                    break;
                case 1:
                    if (GameData.Team.Money < number)
                        return false;

                    break;
                case 2:
                    if (GameData.Team.PVPCoin < number)
                        return false;

                    break;
                case 3:
                    if (GameData.Team.SocialCoin < number)
                        return false;

                    break;
            }

            return true;
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

        public bool HaveAchievement(int id) {
            if (Achievements != null) {
                for (int i = 0; i < Achievements.Length; i++) 
                    if (Achievements[i] == id)
                        return true;
            }

            return false;
        }

        public void AddAchievement(int id) {
            if (Achievements == null) 
                Achievements = new int[0];

            Array.Resize(ref Achievements, Achievements.Length+1);
            Achievements[Achievements.Length-1] = id;
        }

        public void RemoveAchievement(int index) {
            if (Achievements == null) 
                Achievements = new int[0];

            if (index >= 0 && index < Achievements.Length)
                Achievements[index] = -1;
        }

        public void InitFriends() {
            if (Friends != null) {
                int count = 0;
                foreach (KeyValuePair<string, TFriend> item in Friends.ToList()) {
                    TFriend friend = item.Value;
                    friend.PlayerInit();
                    friend.Player.RoleIndex = count;
                    Friends[item.Key] = friend;
                    count++;
                }
            }
        }

        public int FindMissionLv(int id, int timeKind) {
            switch (timeKind) {
                case 0:
                    if (MissionLv != null && MissionLv.ContainsKey(id))
                        return MissionLv[id];

                    break;

                case 1:
                    if (DailyRecord.MissionLv != null && DailyRecord.MissionLv.ContainsKey(id))
                        return DailyRecord.MissionLv[id];

                    break;

                case 2:
                    if (WeeklyRecord.MissionLv != null && WeeklyRecord.MissionLv.ContainsKey(id))
                        return WeeklyRecord.MissionLv[id];

                    break;

                case 3:
                    if (MonthlyRecord.MissionLv != null && MonthlyRecord.MissionLv.ContainsKey(id))
                        return MonthlyRecord.MissionLv[id];

                    break;
            }

            return 0;
        }

        private string statsText(ref TDailyRecord record) {
            string str  = TextConst.S(3741) + record.PlayerRecord.GamePlayTime + "\n" +
            TextConst.S(3742) + record.PlayerRecord.GameCount + "\n" +
            TextConst.S(3743) + record.PlayerRecord.Score + "\n" +
            TextConst.S(3744) + record.PlayerRecord.FGIn + "\n" +
            TextConst.S(3745) + record.PlayerRecord.FG3In + "\n" +
            TextConst.S(3746) + record.PlayerRecord.Dunk + "\n" +
            TextConst.S(3747) + record.PlayerRecord.Rebound + "\n" +
            TextConst.S(3748) + record.PlayerRecord.Assist + "\n" +
            TextConst.S(3749) + record.PlayerRecord.Steal + "\n" +
            TextConst.S(3750) + record.PlayerRecord.Block + "\n" +
            TextConst.S(3751) + record.PlayerRecord.Push + "\n" +
            TextConst.S(3752) + record.PlayerRecord.Knock + "\n" +
            TextConst.S(3753) + record.PlayerRecord.DoubleClickPerfact + "\n" +
            TextConst.S(3761) + record.TeamRecord.Lv + "\n" + 
            TextConst.S(3762) + record.TeamRecord.PVPLv + "\n" + 
            TextConst.S(3763) + record.TeamRecord.StatiumLv + "\n" + 
            TextConst.S(3764) + record.TeamRecord.OccupyLv + "\n" + 
            TextConst.S(3765) + record.TeamRecord.AvatarCount + "\n" + 
            TextConst.S(3766) + 0 + "\n" + 
            TextConst.S(3767) + record.TeamRecord.SkillCount + "\n" + 
            TextConst.S(3768) + 0 + "\n" + 
            TextConst.S(3771) + record.TeamRecord.PVEWin + "\n" + 
            TextConst.S(3772) + record.TeamRecord.PVEKeepWin + "\n" + 
            TextConst.S(3773) + record.TeamRecord.InstanceWin + "\n" + 
            TextConst.S(3774) + record.TeamRecord.InstanceKeepWin + "\n" + 
            TextConst.S(3775) + record.TeamRecord.PVPWin + "\n" + 
            TextConst.S(3776) + record.TeamRecord.PVPKeepWin + "\n" + 
            TextConst.S(3777) + record.TeamRecord.OccupyWin + "\n" +
            TextConst.S(3778) + record.TeamRecord.OccupyKeepWin + "\n";

            return str;
        }

        public string StatsText {
            get {
                GroupRecord.TeamRecord.PVPLv = PVPLv;
                GroupRecord.TeamRecord.StatiumLv = StatiumLv;
                GroupRecord.TeamRecord.OccupyLv = OccupyLv;
                GroupRecord.TeamRecord = LifetimeRecord;
                GroupRecord.TeamRecord.Lv = Player.Lv;
                GroupRecord.PlayerRecord = Player.PlayerRecord;
                string str = statsText(ref GroupRecord);
                str += "\n" + "Daily Record\n" + statsText(ref DailyRecord);
                str += "\n" + "Weekly Record\n" + statsText(ref WeeklyRecord);
                str += "\n" + "Monthly Record\n" + statsText(ref MonthlyRecord);
                return str;
            }
        }

        private int getMissionValue(int kind, ref TDailyRecord record) {
            switch (kind) {
                case 1: return record.TeamRecord.Lv; //玩家等級
                case 2: return record.TeamRecord.PVPLv; //挑戰積分(PVP積分)
                case 3: return record.TeamRecord.StatiumLv; //球場等級
                case 4: return record.TeamRecord.OccupyLv; //踢館等級
                case 5: return record.TeamRecord.AvatarCount; //Avatar number
                case 6: return 0; //收集套裝
                case 7: return record.TeamRecord.SkillCount; //Ability number
                case 8: return 0; //收集套卡
                case 9: return record.TeamRecord.FriendCount; //好友數
                case 10: return record.TeamRecord.GoodCount; //按讚數
                case 11: return Player.NextMainStageID; //PVE通過某關
                case 12: return record.TeamRecord.PVEWin; //PVE獲勝數
                case 13: return record.TeamRecord.PVEKeepWin; //PVE連勝數
                case 14: return 0; //副本通過某關
                case 15: return record.TeamRecord.InstanceWin; //副本獲勝數
                case 16: return record.TeamRecord.InstanceKeepWin; //副本連勝數
                case 17: return record.TeamRecord.PVPWin; //PVP獲勝數
                case 18: return record.TeamRecord.PVPKeepWin; //PVP連勝數 
                case 19: return record.TeamRecord.OccupyWin; //踢館獲勝數
                case 20: return record.TeamRecord.OccupyKeepWin; //踢館連勝數
                case 31: return record.PlayerRecord.Score; //總得分
                case 32: return record.PlayerRecord.FGIn; //兩分球
                case 33: return record.PlayerRecord.FG3In; //三分球
                case 34: return record.PlayerRecord.Dunk; 
                case 35: return record.PlayerRecord.Rebound;
                case 36: return record.PlayerRecord.Assist;
                case 37: return record.PlayerRecord.Steal;
                case 38: return record.PlayerRecord.Block;
                case 39: return record.PlayerRecord.Push;
                case 40: return record.PlayerRecord.Knock; //擊倒
                case 41: return record.PlayerRecord.DoubleClickPerfact; //Perfect數
				case 42: return record.PlayerRecord.Alleyoop;

				case 81: return record.TeamRecord.SkillEvolution;;//技能卡升級每日合成次數
				case 82: return record.TeamRecord.SkillReinforce;//技能卡進階每日進階次數

				case 101: return record.TeamRecord.BuyStaminaQuantity;//每日購買體力次數
				case 102: return record.TeamRecord.BuyDiamondQuantity;//每日購買鑽石次數
				case 103: return record.TeamRecord.BuyCoinQuantity;//每日購買遊戲幣次數
				case 104: return DailyCount.BuyPVPTicketCount; //每日購買挑戰券(PVP)
                case 111: return DailyCount.FreshFriend;
                case 112: return DailyCount.FreshShop;
                case 120: return record.TeamRecord.TotalDelMoney;
                case 121: return record.TeamRecord.TotalDelDiamond;
                case 122: return record.TeamRecord.TotalDelPower;
                /*
                81 技能卡升級合成
                82 技能卡進階
                83 數值裝鑲嵌
                84 數值裝合成

                101 購買體力次數
                102 購買鑽石次數
                103 購買遊戲幣次數
                104 購買挑戰券(PVP)
                110 重置關卡次數
                */
            }

            return 0;
        }

        private int getLifetimeMissionValue(int kind) {
            GroupRecord.TeamRecord.PVPLv = PVPLv;
            GroupRecord.TeamRecord.StatiumLv = StatiumLv;
            GroupRecord.TeamRecord.OccupyLv = OccupyLv;
            GroupRecord.TeamRecord = LifetimeRecord;
            GroupRecord.TeamRecord.Lv = Player.Lv;
            GroupRecord.PlayerRecord = Player.PlayerRecord;

            return getMissionValue(kind, ref GroupRecord);
        }

        public int GetMissionValue(int kind, int timeKind, int timeValue) {
            switch (timeKind) {
                case 0: return getLifetimeMissionValue(kind);
                case 1: 
                    if (timeValue == -1) 
                        return getLifetimeMissionValue(kind);
                    else
                        return getMissionValue(kind, ref DailyRecord);
                    
                case 2: return getMissionValue(kind, ref WeeklyRecord);
                case 3: return getMissionValue(kind, ref MonthlyRecord);
            }

            return 0;
        }

        public bool MissionFinished(ref TMission mission) {
            if (mission.Value != null && Player.Lv >= mission.Lv && 
                FindMissionLv(mission.ID, mission.TimeKind) >= mission.Value.Length)
                return true;
            else
                return false;
        }

        public bool HaveMissionAward(ref TMission mission) {
            if (mission.Value != null && Player.Lv >= mission.Lv) {
                int mLv = FindMissionLv(mission.ID, mission.TimeKind);
                if (mLv < mission.Value.Length) {
                    int mValue = GetMissionValue(mission.Kind, mission.TimeKind, mission.TimeValue);
                    if (mValue >= mission.Value[mLv])
                        return true;
                }
            }

            return false;
        }
		//檢查空間是否有卡牌可以安裝
		public bool IsSurplusCost {
			get {
				int surplus = GameConst.Max_CostSpace - Player.GetSkillCost;
				if(SkillCards != null) {
					for(int i=0; i<SkillCards.Length; i++) {
						if(GameData.DSkillData.ContainsKey(SkillCards[i].ID)) {
							if(GameData.DSkillData[SkillCards[i].ID].Space(SkillCards[i].Lv) <= surplus)	
								return true;
						}
					}
				}
				return false;
			}
		}
		public bool CheckCardCost (TSkill skill) {
			if(GameData.DSkillData.ContainsKey(skill.ID)) {
				int surplus = GameConst.Max_CostSpace - Player.GetSkillCost;
				if(GameData.DSkillData[skill.ID].Space(skill.Lv) <= surplus)
					return true;
			}
			return false;
		}


		/// <summary>
		/// 除了自己，相同的卡片數量（for 進化）
		/// skill就是本身要進化   skillID是要被吃的卡
		/// </summary>
		/// <returns>The card list.</returns>
		/// <param name="skill">Skill.</param>
		/// <param name="skillID">Skill I.</param>
		public List<TSkill> GetCardList (TSkill skill , int skillID) {
			List<TSkill> skills = new List<TSkill>();

			if(SkillCards == null)
				SkillCards = new TSkill[0];

			if(SkillCards.Length > 0) {
				for (int i=0; i<SkillCards.Length; i++) {
					if(SkillCards[i].SN != skill.SN && SkillCards[i].ID == skill.ID) {
						SkillCards[i].Index = i;
						skills.Add(SkillCards[i]);
					}
				}
			}
			return skills;
		} 

		//檢查所有卡片是否有可以進化或合成(For GameLobby Skill RedPoint)
		public bool IsAnyCardReinEvo {
			get {
				if(LimitTable.Ins.HasByOpenID(EOpenID.SkillEvolution) && Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SkillEvolution)){
					if(SkillCards != null && SkillCards.Length > 0) {
						for(int i=0; i<SkillCards.Length; i++) {
							if(GameData.DSkillData.ContainsKey(SkillCards[i].ID)) {
								if(SkillCards[i].Lv == GameData.DSkillData[SkillCards[i].ID].MaxStar &&  IsEnoughMaterial(SkillCards[i])){
										return true;
								}
							}
						}
					}
				}

				if(LimitTable.Ins.HasByOpenID(EOpenID.SkillEvolution) && Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SkillEvolution)){
					if(Player.SkillCards != null && Player.SkillCards.Length > 0) {
						for (int i=0; i<Player.SkillCards.Length; i++) {
							if(GameData.DSkillData.ContainsKey(Player.SkillCards[i].ID )) {
								if(Player.SkillCards[i].Lv == GameData.DSkillData[Player.SkillCards[i].ID].MaxStar &&  IsEnoughMaterial(Player.SkillCards[i])) {
									return true; 
								}
							}
						}
					}
				}
					
				return false;
			}
		}

		//檢查是否有未安裝的卡
		public bool IsExtraCard {
			get {
				if(LimitTable.Ins.HasByOpenID(EOpenID.SkillReinforce) && Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SkillReinforce))
					if(SkillCards != null && SkillCards.Length > 0) 
						for (int i=0; i<SkillCards.Length; i++) 
							if(CheckNoInstallCard(SkillCards[i].SN))
								return true;
				
				return false;
			}	
		}
		//true : no Install
		public bool CheckNoInstallCard (int sn) {
			if(PlayerBank != null && PlayerBank.Length > 0) {
				for (int i=0; i<PlayerBank.Length; i++) {
					if(PlayerBank[i].SkillCardPages != null && PlayerBank[i].SkillCardPages.Length > 0) {
						for (int j=0; j<PlayerBank[i].SkillCardPages.Length; j++) {
							int[] SNs = PlayerBank[i].SkillCardPages[j].SNs;
							if (SNs.Length > 0) {
								for (int k=0; k<SNs.Length; k++)
									if (SNs[k] == sn)
										return false;
							}
						}
					}
				}
			}

			if(Player.SkillCardPages != null && Player.SkillCardPages.Length > 0) {
				for (int i=0; i<Player.SkillCardPages.Length; i++) {
					int[] SNs = Player.SkillCardPages[i].SNs;
					if (SNs.Length > 0) {
						for (int k=0; k<SNs.Length; k++)
							if (SNs[k] == sn)
								return false;
					}
				}
			}

			for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) 
				if(GameData.Team.Player.SkillCards[i].SN == sn)
					return false;

			return true;
		}

		//檢查該卡片是否有足夠材料
		public bool IsEnoughMaterial (TSkill skill) {
			TMaterialItem materialSkillCard = new TMaterialItem();
			bool flag1 = true;
			bool flag2 = true;
			bool flag3 = true;
			if(GameData.DSkillData.ContainsKey(skill.ID) || GameData.DSkillData[skill.ID].EvolutionSkill != 0) {
				if(!GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material1) && 
					!GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material2) &&
					!GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material3))
					return false;

				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material1)) {
					if(GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Kind == 21) {
						if(GameData.DSkillData.ContainsKey(GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Avatar))
							flag1 = (GetCardList(skill, GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Avatar).Count > 0);
						else 
							flag1 = false;
					} else {
						GameData.Team.FindMaterialItem(GameData.DSkillData[skill.ID].Material1, ref materialSkillCard);
						if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material1)) {
							if(materialSkillCard.Num >= GameData.DSkillData[skill.ID].MaterialNum1)
								flag1 = true;
							else 
								flag1 = false;
						}
					}
				} else 
					flag1 = true;

				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material2)) {
					if(GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Kind == 21) {
						if(GameData.DSkillData.ContainsKey(GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Avatar))
							flag2 = (GetCardList(skill, GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Avatar).Count > 0);
						else 
							flag2 = false;
					} else {
						GameData.Team.FindMaterialItem(GameData.DSkillData[skill.ID].Material2, ref materialSkillCard);
						if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material2)) {
							if(materialSkillCard.Num >= GameData.DSkillData[skill.ID].MaterialNum2)
								flag2 = true;
							else
								flag2 = false;
						} 
					}
				} else 
					flag2 = true;
				
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material3)) {
					if(GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Kind == 21) {
						if(GameData.DSkillData.ContainsKey(GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Avatar))
							flag3 = (GetCardList(skill, GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Avatar).Count > 0);
						else 
							flag3 = false;
					} else {
						GameData.Team.FindMaterialItem(GameData.DSkillData[skill.ID].Material3, ref materialSkillCard);
						if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material3)) {
							if(materialSkillCard.Num >= GameData.DSkillData[skill.ID].MaterialNum3)
								flag3 = true;
							else
								flag3 = false;
						} 
					}
				} else
					flag3 = true;

				if(flag1 && flag2 && flag3)
					return true;
				else
					return false;
			} else 
				return false;

		}

		public void InitSkillCardCount () {
			if(SkillCardCounts == null)
				SkillCardCounts = new Dictionary<int, int>();
			
			SkillCardCounts.Clear();

			if(SkillCards == null)
				SkillCards = new TSkill[0];

			if(SkillCards.Length > 0) {
				for (int i=0; i<SkillCards.Length; i++) {
					if(SkillCardCounts.ContainsKey(SkillCards[i].ID)) {
						SkillCardCounts[SkillCards[i].ID] += 1;
					} else {
						SkillCardCounts.Add(SkillCards[i].ID, 1);
					}
				}
			}

			if(PlayerBank != null && PlayerBank.Length > 0) {
				for (int i=0; i<PlayerBank.Length; i++) {
					if(PlayerBank[i].ID != Player.ID &&PlayerBank[i].SkillCards != null && PlayerBank[i].SkillCards.Length > 0) {
						for(int j=0; j<PlayerBank[i].SkillCards.Length; j++) {
							if(SkillCardCounts.ContainsKey(PlayerBank[i].SkillCards[j].ID)) {
								SkillCardCounts[PlayerBank[i].SkillCards[j].ID] += 1;
							} else {
								SkillCardCounts.Add(PlayerBank[i].SkillCards[j].ID, 1);
							}
						}
					}
				}
			}

			if(Player.SkillCards != null && Player.SkillCards.Length > 0) {
				for (int i=0; i<Player.SkillCards.Length; i++) {
					if(SkillCardCounts.ContainsKey(Player.SkillCards[i].ID )) {
						SkillCardCounts[Player.SkillCards[i].ID] += 1;
					} else {
						SkillCardCounts.Add(Player.SkillCards[i].ID , 1);
					}
				}
			}
		}

		public bool IsSkillCardFull {
			get {
				if(SkillCardMax == 0)
					SkillCardMax = 300;
				return (SkillCards.Length > SkillCardMax);
			}
		}

		/// <summary>
		/// 檢查身上卡片是否有
		/// </summary>
		/// <returns><c>true</c>, if skill cardis new was checked, <c>false</c> otherwise.</returns>
		/// <param name="id">Skill id.</param>
        public bool CheckSkillCardisNew (int id) {
            if(SkillCards == null)
                SkillCards = new TSkill[0];

            if(SkillCards.Length > 0) 
                for (int i=0; i<SkillCards.Length; i++) 
                    if(SkillCards[i].ID == id)
                        return false;

            if(PlayerBank != null && PlayerBank.Length > 0) 
                for (int i=0; i<PlayerBank.Length; i++) 
                    if(PlayerBank[i].ID != Player.ID &&PlayerBank[i].SkillCards != null && PlayerBank[i].SkillCards.Length > 0) 
                        for(int j=0; j<PlayerBank[i].SkillCards.Length; j++) 
                            if(PlayerBank[i].SkillCards[j].ID == id)
                                return false;

            if(Player.SkillCards != null && Player.SkillCards.Length > 0) 
                for (int i=0; i<Player.SkillCards.Length; i++) 
                    if (Player.SkillCards[i].ID == id)
                        return false;

            return true;
        }

		public bool CheckSkillCardItemIDGet (int itemID) {
			if(GameData.DItemData.ContainsKey(itemID)) {
				return !CheckSkillCardisNew(GameData.DItemData[itemID].Avatar);
			}
			return false;
		}

		public bool CheckSkillCardIDGet (int skillID) {
			if(GameData.DSkillData.ContainsKey(skillID)) {
				return !CheckSkillCardisNew(skillID);
			}
			return false;
		}

		public bool CheckSkillCardisNew (int id, TSkill[] SkillCardOld) {
			if(SkillCardOld != null) {
				if(SkillCardOld.Length > 0) 
					for (int i=0; i<SkillCardOld.Length; i++) 
						if(SkillCardOld[i].ID == id)
							return false;
			}


			if(PlayerBank != null && PlayerBank.Length > 0) 
				for (int i=0; i<PlayerBank.Length; i++) 
					if(PlayerBank[i].ID != Player.ID &&PlayerBank[i].SkillCards != null && PlayerBank[i].SkillCards.Length > 0) 
						for(int j=0; j<PlayerBank[i].SkillCards.Length; j++) 
							if(PlayerBank[i].SkillCards[j].ID == id)
								return false;

			if(Player.SkillCards != null && Player.SkillCards.Length > 0) 
				for (int i=0; i<Player.SkillCards.Length; i++) 
					if (Player.SkillCards[i].ID == id)
						return false;

			return true;
		}

		public int GetAvatarCount (int id) {
			int count = 0;
			int kind = 0;
			if (GameData.DItemData.ContainsKey (id)) 
            {
				kind = GameData.DItemData [id].Kind;
                if (kind < 8)
                {
                    if (Player.Items != null)
                        for (int i = 0; i < Player.Items.Length; i++)
                            if (Player.Items[i].ID == id)
                                count++; 

                    if (GameData.Team.Items != null)
                        for (int i = 0; i < GameData.Team.Items.Length; i++)
                            if (GameData.Team.Items[i].ID == id)
                                count++;
                    
                }else if (kind >= 11 && kind <= 17)
                {
                    if (Player.ValueItems != null)
                        foreach (KeyValuePair<int, TValueItem> item in Player.ValueItems)
                            if(item.Value.ID == id)
                                count++;  

                    if (GameData.Team.ValueItems != null)
                        for (int i = 0; i < GameData.Team.ValueItems.Length; i++)
                            if(GameData.Team.ValueItems[i].ID == id)
                                count++; 
				}
			}			
			return count;
		}

        /// <summary>
        /// 是否玩家身上的數值裝是最強的.
        /// </summary>
        /// <returns></returns>
        public bool IsPlayerAllBestValueItem()
        {
            for(int kind = 11; kind <= 18; kind++)
            {
                if(IsPlayerBestValueItem(kind))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 是否某個數值裝是最強的.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public bool IsPlayerBestValueItem(int kind)
        {
            if(11 <= kind && kind <= 18)
                return Player.GetValueItemTotalPoints(kind) < getStorageBestValueItemTotalPoints(kind);

            return false;
        }

        /// <summary>
        /// 從倉庫找出最強的數值裝數值總和.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public int getStorageBestValueItemTotalPoints(int kind)
        {
            int maxTotalPoint = Int32.MinValue;
            foreach(TValueItem valueItem in ValueItems)
            {
                if(!GameData.DItemData.ContainsKey(valueItem.ID))
                    continue;

                TItemData item = GameData.DItemData[valueItem.ID];
                if (item.Kind != kind)
                    continue;

                if(maxTotalPoint < valueItem.GetTotalPoint())
                    maxTotalPoint = valueItem.GetTotalPoint();
            }

            return maxTotalPoint;
        }
            
        public bool NeedForSyncRecord{
            get {return needForSyncRecord || Player.NeedForSyncRecord;}
            set {
                needForSyncRecord = value;
            }
        }
        //Setting LifetimeRecord have to use TeamRecor for sync data from server.
        public TTeamRecord TeamRecord {
            get {return LifetimeRecord;}
            set {
                needForSyncRecord = true;
                LifetimeRecord = value;
            }
        }

		/// <summary>
		/// 擁有過的item數量
		/// </summary>
		/// <returns><c>true</c> if this instance is get item the specified itemID; otherwise, <c>false</c>.</returns>
		/// <param name="itemID">Item I.</param>
		public bool IsGetItem (int itemID) {
			if(GotItemCount.ContainsKey(itemID)) 
				if(GotItemCount[itemID] != 0)
					return true;
			
			return false;
		}

		/// <summary>
		/// 擁有過的造型數量
		/// </summary>
		/// <returns><c>true</c> if this instance is get avatar the specified itemID; otherwise, <c>false</c>.</returns>
		/// <param name="itemID">Item I.</param>
		public bool IsGetAvatar (int itemID) {
			if(GotAvatar != null && GotAvatar.ContainsKey(itemID)) 
				if(GotAvatar[itemID] != 0)
					return true;
			
			return false;
		}

		public bool IsGetAvatarFriend (Dictionary<int, int> gotAvatar, int itemID) {
			if(gotAvatar != null && gotAvatar.ContainsKey(itemID)) 
				if(gotAvatar[itemID] != 0)
					return true;

			return false;
		}

		/// <summary>
		/// 某一列的套卡完成數量 （擁有過的卡牌）
		/// </summary>
		/// <returns>The card complete count.</returns>
		/// <param name="suitcardID">Suitcard I.</param>
		public int SuitCardCompleteCount (int suitcardID) {
			int count = 0;
			foreach(KeyValuePair<int, TSuitCard> item in GameData.DSuitCard) {
				if(item.Key == suitcardID) {
					for(int i=0; i<item.Value.Items.Length; i++) 
						if(IsGetItem(item.Value.Items[i]))
							count ++;
				}
			}
			return count;
		}

		/// <summary>
		/// 套卡啟動值使用的數量
		/// </summary>
		/// <value>The suit card execute cost.</value>
		public int SuitCardExecuteCost {
			get {
				int count = 0;
				if(SuitCardCost != null) 
					for(int i=0; i<SuitCardCost.Length; i++) 
						if(GameData.DSuitCard.ContainsKey(SuitCardCost[i]))
							count += GameData.DSuitCard[SuitCardCost[i]].CardPower;
				
				return count;
			}
		}

		/// <summary>
		/// 是否有啟動套卡技能.
		/// </summary>
		/// <returns><c>true</c> if this instance is execute suit card the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>

		public bool IsExecuteSuitCard (int suitcardID) {
			if(SuitCardCost != null) 
				for(int i=0; i<SuitCardCost.Length; i++) 
					if(SuitCardCost[i] == suitcardID)
						return true;	
			return false;
		}

		public int GetExecuteSuitCardIndex (int suitcardID) {
			if(SuitCardCost != null) 
				for(int i=0; i<SuitCardCost.Length; i++) 
					if(SuitCardCost[i] == suitcardID)
						return i;	
			return -1;
		}

		//AP (Ability Points)初始士氣值
		public int InitGetAP () {
			int ap = 0;
			if(LimitTable.Ins.HasByOpenID(EOpenID.SuitCard)) {
				if(Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SuitCard)) 
					if(SuitCardCost != null) 
						for(int i=0; i<SuitCardCost.Length; i++) 
							for (int j=0; j<GameData.DSuitCard[SuitCardCost[i]].AttrKind.Length; j++) 
								if(GameData.DSuitCard[SuitCardCost[i]].AttrKind[j] == 20) 
									ap += GameData.DSuitCard[SuitCardCost[i]].Value[j];
			}

			if(LimitTable.Ins.HasByOpenID(EOpenID.SuitItem)) {
				if(Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SuitItem)) {
					foreach (KeyValuePair<int, TSuitItem> item in GameData.DSuitItem) {
						int count = SuitItemCompleteCount(item.Key);
						if(count >= 2) {
							for(int i=0; i<item.Value.AttrKind.Length; i++) {
								if(GameData.DSuitItem[item.Key].AttrKind[i] == 20) 
									ap += GameData.DSuitItem[item.Key].Value[i];
							}
						}
					}
				}
			}

			return ap;
		}

		//加入套卡的效果
		public void AddSuitCardEffect (int[] suitCardCost, int lv) {
			if(LimitTable.Ins.HasByOpenID(EOpenID.SuitCard)) {
				if(lv >= LimitTable.Ins.GetLv(EOpenID.SuitCard)) {
					if(suitCardCost != null) {
						for(int i=0; i<suitCardCost.Length; i++) {
							for (int j=0; j<GameData.DSuitCard[suitCardCost[i]].AttrKind.Length; j++) {
								switch(GameData.DSuitCard[suitCardCost[i]].AttrKind[j]) {
								case (int)EBonus.Point2:
									Player.Point2 += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Point3:
									Player.Point3 += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Dunk:
									Player.Dunk += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Rebound:
									Player.Rebound += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Block:
									Player.Block += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Steal:
									Player.Steal += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Stamina:
									Player.Stamina += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Defence:
									Player.Defence += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Dribble:
									Player.Dribble += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Pass:
									Player.Pass += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Speed:
									Player.Speed += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								case (int)EBonus.Strength:
									Player.Strength += (float)GameData.DSuitCard[suitCardCost[i]].Value[j];
									break;
								}
							}
						}
					}
				}
			}
		}

		//套卡影響Cost的值總和
		public int SuitCardCostEffect (int id) {
			int count = 0;
			if(LimitTable.Ins.HasByOpenID(EOpenID.SuitCard) && Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SuitCard)) 
				if(SuitCardCost != null) 
					for(int i=0; i<SuitCardCost.Length; i++) 
						if(SuitCardCost[i] == id) 
							for (int j=0; j<GameData.DSuitCard[SuitCardCost[i]].AttrKind.Length; j++) 
								if(GameData.DSuitCard[SuitCardCost[i]].AttrKind[j] == 30) 
									count += GameData.DSuitCard[SuitCardCost[i]].Value[j];

			return count;
		}

		//某一列套裝完成的數量
		public int SuitItemCompleteCount (int id) {
			int count = 0;
			if(GameData.DSuitItem.ContainsKey(id)) 
				for(int i=0; i<GameData.DSuitItem[id].Items.Length; i++) 
					if(IsGetAvatar(GameData.DSuitItem[id].Items[i])) 
						count ++;
			return count;
		}

		//某一列套裝完成的數量(Friend)
		public int SuitItemCompleteCountFriend (Dictionary<int, int> gotAvatar, int id) {
			int count = 0;
			if(GameData.DSuitItem.ContainsKey(id)) 
				for(int i=0; i<GameData.DSuitItem[id].Items.Length; i++) 
					if(IsGetAvatarFriend(gotAvatar, GameData.DSuitItem[id].Items[i])) 
						count ++;
			return count;
		}

		//某一列套裝 卡片完成的數量 (影響Cost值)
		public int SuitItemCardCompleteCount (int id) {
			int count = 0;
			if(GameData.DSuitItem.ContainsKey(id)) 
				for(int i=0; i<GameData.DSuitItem[id].Card.Length; i++) 
					if(CheckSkillCardItemIDGet(GameData.DSuitItem[id].Card[i]))
						count ++;
			return count;
		}

		//加入套裝加成的效果
		public void AddSuitItemEffect (Dictionary<int, int> gotAvatar, int lv) {
			if(GotAvatar != null && LimitTable.Ins.HasByOpenID(EOpenID.SuitItem)) {
				if(lv >= LimitTable.Ins.GetLv(EOpenID.SuitItem)) {
					foreach (KeyValuePair<int, TSuitItem> item in GameData.DSuitItem) {
						int count = SuitItemCompleteCountFriend(gotAvatar, item.Key);
						if(count >= 2) {
							for(int i=0; i<item.Value.AttrKind.Length; i++) {
								if(i <= (count - 2)) {
									switch(item.Value.AttrKind[count-2]) {
									case (int)EBonus.Point2:
										Player.Point2 += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Point3:
										Player.Point3 += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Dunk:
										Player.Dunk += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Rebound:
										Player.Rebound += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Block:
										Player.Block += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Steal:
										Player.Steal += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Stamina:
										Player.Stamina += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Defence:
										Player.Defence += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Dribble:
										Player.Dribble += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Pass:
										Player.Pass += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Speed:
										Player.Speed += (float)item.Value.Value[count-2];
										break;
									case (int)EBonus.Strength:
										Player.Strength += (float)item.Value.Value[count-2];
										break;
									}
								}
							}
						}
					}
				}
			}
		}
    }
}