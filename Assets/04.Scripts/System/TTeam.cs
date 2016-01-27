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

        public int PlayerNum; // 玩家擁有幾位角色.
        public int StageTutorial;
        public int AvatarPotential;
        public int OccupyLv; //佔領球館等級
        public int StatiumLv; //經營球館等級
        public int SocialCoin; //社群幣

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

        //PVP
        public int PVPCoin; //聯盟幣
        public int PVPLv;
        public int PVPIntegral;
		public int PVPDailyReaward;
        public int PVPEnemyIntegral;
		public int PVPTicket;
        public string LeagueName;
        public int LeagueIcon;

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
                    friend.Player.Init();
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

		/// <summary>
		/// 檢查卡片是否有得到過
		/// </summary>
		/// <returns><c>true</c>, if skill cardis new was checked, <c>false</c> otherwise.</returns>
		/// <param name="id">Identifier.</param>
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
    }
}