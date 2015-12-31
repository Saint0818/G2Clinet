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
        public DateTime LookFriendTime;
        public DateTime FreeLuckBox;
        public int PlayerNum; // 玩家擁有幾位角色.
        public int StageTutorial;
        public int AvatarPotential;
        public int PVPLv;
        public int OccupyLv; //佔領球館等級
        public int StatiumLv; //經營球館等級

        public int[] TutorialFlags;
        public int[] Achievements;
        public Dictionary<int, int> GotItemCount; //key: item id, value: got number
        public Dictionary<int, int> GotAvatar; //key: item id, value: 1 : got already
        public Dictionary<int, int> MissionLv; //key: mission id, value: lv
        public TTeamRecord LifetimeRecord;
        public TPlayer Player;
        public TItem[] Items;
        public TMaterialItem[] MaterialItems;
        public TSkill[] SkillCards;
        public TPlayerBank[] PlayerBank;
        public TMail[] Mails;
        public TFriend[] Friends;

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

        public event CommonDelegateMethods.Int1 OnMoneyChangeListener;
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

        public event CommonDelegateMethods.Int1 OnPowerChangeListener;
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

        public event CommonDelegateMethods.Int1 OnDiamondChangeListener;
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

        public int FindMissionLv(int id) {
            if (MissionLv != null && MissionLv.ContainsKey(id))
                return MissionLv[id];
            else
                return 0;
        }

        public string StatsText {
            get {
                string str = TextConst.S(3741) + Player.LifetimeRecord.GamePlayTime + "\n" +
                    TextConst.S(3742) + Player.LifetimeRecord.GameCount + "\n" +
                    TextConst.S(3743) + Player.LifetimeRecord.Score + "\n" +
                    TextConst.S(3744) + Player.LifetimeRecord.FGIn + "\n" +
                    TextConst.S(3745) + Player.LifetimeRecord.FG3In + "\n" +
                    TextConst.S(3746) + Player.LifetimeRecord.Dunk + "\n" +
                    TextConst.S(3747) + Player.LifetimeRecord.Rebound + "\n" +
                    TextConst.S(3748) + Player.LifetimeRecord.Assist + "\n" +
                    TextConst.S(3749) + Player.LifetimeRecord.Steal + "\n" +
                    TextConst.S(3750) + Player.LifetimeRecord.Block + "\n" +
                    TextConst.S(3751) + Player.LifetimeRecord.Push + "\n" +
                    TextConst.S(3752) + Player.LifetimeRecord.Knock + "\n" +
                    TextConst.S(3753) + Player.LifetimeRecord.DoubleClickPerfact + "\n" +
                    TextConst.S(3761) + Player.Lv + "\n" + 
                    TextConst.S(3762) + PVPLv + "\n" + 
                    TextConst.S(3763) + StatiumLv + "\n" + 
                    TextConst.S(3764) + OccupyLv + "\n" + 
                    TextConst.S(3765) + LifetimeRecord.AvatarCount + "\n" + 
                    TextConst.S(3766) + 0 + "\n" + 
                    TextConst.S(3767) + LifetimeRecord.SkillCount + "\n" + 
                    TextConst.S(3768) + 0 + "\n" + 
                    TextConst.S(3771) + LifetimeRecord.PVEWin + "\n" + 
                    TextConst.S(3772) + LifetimeRecord.PVEKeepWin + "\n" + 
                    TextConst.S(3773) + LifetimeRecord.SubTextWin + "\n" + 
                    TextConst.S(3774) + LifetimeRecord.SubTextKeepWin + "\n" + 
                    TextConst.S(3775) + LifetimeRecord.PVPWin + "\n" + 
                    TextConst.S(3776) + LifetimeRecord.PVPKeepWin + "\n" + 
                    TextConst.S(3777) + LifetimeRecord.OccupyWin + "\n" +
                    TextConst.S(3778) + LifetimeRecord.OccupyKeepWin + "\n";

                return str;
            }
        }

        public int GetMissionValue(int kind) {
            switch (kind) {
                case 1: return Player.Lv; //玩家等級
                case 2: return PVPLv; //挑戰積分(PVP積分)
                case 3: return StatiumLv; //球場等級
                case 4: return OccupyLv; //踢館等級
                case 5: return LifetimeRecord.AvatarCount; //Avatar number
                case 6: return 0; //收集套裝
                case 7: return LifetimeRecord.SkillCount; //Ability number
                case 8: return 0; //收集套卡
                case 11: return Player.NextMainStageID; //PVE通過某關
                case 12: return LifetimeRecord.PVEWin; //PVE獲勝數
                case 13: return LifetimeRecord.PVEKeepWin; //PVE連勝數
                case 14: return 0; //副本通過某關
                case 15: return LifetimeRecord.SubTextWin; //副本獲勝數
                case 16: return LifetimeRecord.SubTextKeepWin; //副本連勝數
                case 17: return LifetimeRecord.PVPWin; //PVP獲勝數
                case 18: return LifetimeRecord.PVPKeepWin; //PVP連勝數 
                case 19: return LifetimeRecord.OccupyWin; //踢館獲勝數
                case 20: return LifetimeRecord.OccupyKeepWin; //踢館連勝數
                case 31: return Player.LifetimeRecord.Score; //總得分
                case 32: return Player.LifetimeRecord.FGIn; //兩分球
                case 33: return Player.LifetimeRecord.FG3In; //三分球
                case 34: return Player.LifetimeRecord.Dunk; 
                case 35: return Player.LifetimeRecord.Rebound;
                case 36: return Player.LifetimeRecord.Assist;
                case 37: return Player.LifetimeRecord.Steal;
                case 38: return Player.LifetimeRecord.Block;
                case 39: return Player.LifetimeRecord.Push;
                case 40: return Player.LifetimeRecord.Knock; //擊倒
                case 41: return Player.LifetimeRecord.DoubleClickPerfact; //Perfect數
                case 42: return Player.LifetimeRecord.Alleyoop;
            }

            return 0;
        }

        public bool MissionFinished(ref TMission mission) {
            if (mission.Value != null && FindMissionLv(mission.ID) >= mission.Value.Length)
                return true;
            else
                return false;
        }

        public bool HaveMissionAward(ref TMission mission) {
            if (mission.Value != null) {
                int mLv = FindMissionLv(mission.ID);
                if (mLv < mission.Value.Length) {
                    int mValue = GetMissionValue(mission.Kind);
                    if (mValue >= mission.Value[mLv])
                        return true;
                }
            }

            return false;
        }

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

        /// <summary>
        /// 是否玩家身上的數值裝是最強的.
        /// </summary>
        /// <returns></returns>
        public bool IsPlayerAllBestValueItem()
        {
            for(int kind = 11; kind < 19; kind++)
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
            if(11 <= kind && kind <= 19)
                return Player.GetValueItemTotalPoints(kind) < getBestValueItemTotalPointsFromStorage(kind);

            return false;
        }

        /// <summary>
        /// 從倉庫找出最強的數值裝數值總和.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public int getBestValueItemTotalPointsFromStorage(int kind)
        {
            int maxTotalPoint = Int32.MinValue;
            for(var i = 0; i < Items.Length; i++)
            {
                if(!GameData.DItemData.ContainsKey(Items[i].ID))
                    continue;

                TItemData item = GameData.DItemData[Items[i].ID];
                if(item.Kind != kind)
                    continue;

                if(maxTotalPoint < item.BonusValues.Sum())
                    maxTotalPoint = item.BonusValues.Sum();
            }

            return maxTotalPoint;
        }
    }
}