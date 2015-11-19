using System.Collections.Generic;
using GameEnum;
using UnityEngine;

namespace GameStruct
{
    /// <summary>
    /// 這是對應到 Server 端 Team.Player 的資料結構. 但也用在 NPC 的資料.
    /// 如果是 Team.Player, 必須要呼叫 Init() 做資料初始化.
    /// 如果是 NPC, 要呼叫 SetID() 做資料初始化.
    /// </summary>
    public struct TPlayer {
        public int RoleIndex;
        public int ID;
        public string Name;
        public int Lv;

        public int Exp;

        public int AILevel;

        /// <summary>
        /// 球員的能力值(目前主要是表格的數值 + 數值裝的數值)
        /// </summary>
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

        public int GetPotentialValue(EAttribute attr)
        {
            return Potential.ContainsKey(attr) ? Potential[attr] : 0;
        }
		public Dictionary<EAttribute, int> Potential;
        /// <summary>
        /// 玩家當日的挑戰次數. key: stageID, value: 挑戰次數.
        /// </summary>
        public Dictionary<int, int> StageChallengeNums;

        public TAvatar Avatar;
        public List<TSkill> ActiveSkills;
        public TSkill[] SkillCards;
        public TSkillCardPage[] SkillCardPages;

        /// <summary>
        /// 玩家身上的外觀裝備.(Avatar). key: item.kind.
        /// </summary>
        public TItem[] Items;

        /// <summary>
        /// 玩家身上的數值裝備. key: TItemData.Kind.
        /// </summary>
        public Dictionary<int, TValueItem> ValueItems;

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
            Exp = 0;

            Avatar = new TAvatar(1);
            ActiveSkills = new List<TSkill>();
            SkillCards = new TSkill[0];
            SkillCardPages = new TSkillCardPage[0];
            Items = new TItem[0];
            //			EquipItems = new TEquipItem[0];
            ValueItems = new Dictionary<int, TValueItem>();
			Potential = new Dictionary<EAttribute, int> ();
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

        /// <summary>
        /// only for npc.
        /// </summary>
        /// <param name="id"></param>
        public bool SetID(int id) {
            ID = id;
            if(ID > 0 && GameData.DPlayers.ContainsKey(ID)) {
                SetAttribute(ESkillType.NPC);
                SetAvatar();
                return true;
            } else
                return false;
        }

        public void SetAttribute(ESkillType type = ESkillType.Player)
        {
            if(ID > 0 && GameData.DPlayers.ContainsKey(ID))
            {
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

                addEquipValues();
            }
        }

        private void addEquipValues()
        {
            if(ValueItems == null)
                return;

            foreach(KeyValuePair<int, TValueItem> pair in ValueItems)
            {
                if(!GameData.DItemData.ContainsKey(pair.Value.ID))
                {
                    Debug.LogErrorFormat("Can't find ItemData({0})", pair.Value.ID);
                    continue;
                }

                TItemData data = GameData.DItemData[pair.Value.ID];
                for(int i = 0; i < data.AttrKinds.Length; i++)
                {
                    switch(data.AttrKinds[i])
                    {
						case EAttributeKind.Point2:
                            Point2 += data.AttrValues[i];
                            break;
						case EAttributeKind.Point3:
                            Point3 += data.AttrValues[i];
                            break;
						case EAttributeKind.Speed:
                            Speed += data.AttrValues[i];
                            break;
						case EAttributeKind.Stamina:
                            Stamina += data.AttrValues[i];
                            break;
						case EAttributeKind.Strength:
                            Strength += data.AttrValues[i];
                            break;
						case EAttributeKind.Dunk:
                            Dunk += data.AttrValues[i];
                            break;
						case EAttributeKind.Rebound:
                            Rebound += data.AttrValues[i];
                            break;
						case EAttributeKind.Block:
                            Block += data.AttrValues[i];
                            break;
						case EAttributeKind.Defence:
                            Defence += data.AttrValues[i];
                            break;
						case EAttributeKind.Steal:
                            Steal += data.AttrValues[i];
                            break;
						case EAttributeKind.Dribble:
                            Dribble += data.AttrValues[i];
                            break;
						case EAttributeKind.Pass:
                            Pass += data.AttrValues[i];
                            break;
                    }
                }
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

        public int GetSkillCount (int skillID) {
            int count = 0;
            for(int i=0; i<SkillCards.Length; i++) {
                if(SkillCards[i].ID == skillID)
                    count ++;
            }
            return count;
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

        /// <summary>
        /// 某個屬性, 全部數值裝影響的數值總和.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
		public int GetSumValueItems(EAttributeKind kind)
        {
			CommonDelegateMethods.RInt1Int1Kind1 findSumValue = delegate(int itemID, EAttributeKind attrKind)
            {
                if(!GameData.DItemData.ContainsKey(itemID))
                {
                    Debug.LogErrorFormat("Can't find TItemData: {0}", itemID);
                    return 0;
                }

                return GameData.DItemData[itemID].GetSumAttrValue(attrKind);
            };

            if(ValueItems == null)
                return 0;

            int sum = 0;
            foreach(KeyValuePair<int, TValueItem> pair in ValueItems)
            {
                sum += findSumValue(pair.Value.ID, kind);

                if(pair.Value.InlayItemIDs == null)
                    continue;

                for(var i = 0; i < pair.Value.InlayItemIDs.Length; i++)
                {
                    sum += findSumValue(pair.Value.InlayItemIDs[i], kind);
                }
            }

            return sum;
        }
    } // end of the struct.
} // end of the namespace.