using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

public static class UIValueItemDataBuilder
{
    public static UIValueItemData BuildEmpty()
    {
        return new UIValueItemData
        {
            Atlas = Resources.Load<UIAtlas>("UI/UIGame"),
            Icon = "Icon_Create",
            Frame = "Equipment_1",
            Status = UIValueItemData.EStatus.CannotDemount
        };
    }

    public static UIValueItemData BuildDemount()
    {
        return new UIValueItemData
        {
            Atlas = Resources.Load<UIAtlas>("UI/UIGame"),
            Icon = "Icon_Create",
            Frame = "Equipment_1",
            Status = UIValueItemData.EStatus.CannotDemount,
            StorageIndex = UIValueItemData.StorageIndexDemount
        };
    }

    public static UIValueItemData Build(TItemData item, [NotNull] int[] playerInlayItemIDs, int num)
    {
        return build(item, playerInlayItemIDs, findStorageMaterials(item), num);
    }

    private class MaterialItemInfo
    {
        /// <summary>
        /// -1: 在倉庫找不到材料. >0: 在倉庫的索引.
        /// </summary>
        public int Index = -1;

        /// <summary>
        /// 在倉庫有幾個材料.
        /// </summary>
        public int Num;
    }

    private static MaterialItemInfo[] findStorageMaterials(TItemData item)
    {
        List<MaterialItemInfo> infos = new List<MaterialItemInfo>();

        for(var i = 0; i < item.Materials.Length; i++)
        {
            if(GameData.Team.HasMaterialItem(item.Materials[i]))
            {
                TMaterialItem materialItem = new TMaterialItem();
                int materialItemIndex = GameData.Team.FindMaterialItem(item.Materials[i], ref materialItem);
                MaterialItemInfo info = new MaterialItemInfo
                {
                    Index = materialItemIndex,
                    Num = materialItem.Num
                };
                infos.Add(info);
            }
            else
            {
                infos.Add(new MaterialItemInfo());
            }
        }

        return infos.ToArray();
    }

    /// <summary>
    /// 將玩家的某件數值裝資料轉換成介面使用的資料.
    /// </summary>
    /// <param name="item"> 數值裝的道具資料. </param>
    /// <param name="playerInlayItemIDs"> 數值裝鑲嵌的狀態(4 個 element), [0]:Material1 鑲嵌材料; 
    /// [1]:Material2 鑲嵌材料, 以此類推.</param>
    /// <param name="storageMaterials"> 倉庫材料數目(4 個 element), [0]:Material1 擁有的數量, 
    /// [1]:Material2 擁有的數量, 以此類推. </param>
    /// <param name="num"></param>
    /// <returns></returns>
    private static UIValueItemData build(TItemData item, int[] playerInlayItemIDs, 
                                         MaterialItemInfo[] storageMaterials, int num)
    {
        UIValueItemData valueItem = new UIValueItemData
        {
            Name = item.Name,
            NameColor = TextConst.Color(item.Quality),
            Atlas = GameData.DItemAtlas.ContainsKey(GameData.AtlasName(item.Atlas))
                    ? GameData.DItemAtlas[GameData.AtlasName(item.Atlas)]
                    : null,
            Icon = string.Format("Item_{0}", item.Icon),
            Frame = string.Format("Equipment_{0}", item.Quality),
			Quality = item.Quality,
            Desc = item.Explain,
            Values = convertBonus(item.Bonus, item.BonusValues),
            Inlay = convertInlayStatus(playerInlayItemIDs),
            InlayValues = convertInlayBonus(playerInlayItemIDs),
            Status = UIEquipChecker.FindStatus(item, playerInlayItemIDs),
            LevelNotEnoughText = UIEquipChecker.IsInlayFull(item, playerInlayItemIDs) && 
                                 UIEquipChecker.HasUpgradeMoney(item) &&
                                 UIEquipChecker.HasUpgradeItem(item) &&
                                 !UIEquipChecker.IsLevelEnough(item) 
                                 ? String.Format(TextConst.S(6010), item.UpgradeLv)
                                 : String.Empty,
            Num = num,
            UpgradeMoney = item.UpgradeMoney
        };

        buildMaterials(item, playerInlayItemIDs, storageMaterials, ref valueItem);

        return valueItem;
    }

    private static void buildMaterials(TItemData item, int[] playerInlayItemIDs, 
                                    MaterialItemInfo[] storageMaterials,
                                    ref UIValueItemData valueItem)
    {
        valueItem.Materials.Clear();

        for(var i = 0; i < item.Materials.Length; i++)
        {
            if(item.Materials[i] <= 0)
                continue;

            if(!GameData.DItemData.ContainsKey(item.Materials[i]))
            {
                Debug.LogWarningFormat("Can't Find ItemData by ItemID({0})", item.Materials[i]);
                continue;
            }
            TItemData materialItem = GameData.DItemData[item.Materials[i]];

            UIEquipMaterialItem.Data data = new UIEquipMaterialItem.Data
            {
                Name = materialItem.Name,
                NameColor = TextConst.Color(materialItem.Quality),
                Icon = string.Format("Item_{0}", materialItem.Icon),
                IconBGColor = TextConst.ColorBG(materialItem.Quality),
                Frame = string.Format("Equipment_{0}", item.Quality),
                NeedValue = item.MaterialNums[i],
                StorageIndex = storageMaterials[i].Index,
                ItemID = materialItem.ID,
                Values = convertBonus(materialItem.Bonus, materialItem.BonusValues)
            };
            valueItem.Materials.Add(data);
            
            if(playerInlayItemIDs != null && i < playerInlayItemIDs.Length && playerInlayItemIDs[i] > 0)
            {
                // 有鑲嵌物.
                data.Status = UIEquipMaterialItem.EStatus.Inlayed;
                data.RealValue = data.NeedValue;
            }
            else
            {
                // 沒有鑲嵌.
                data.Status = storageMaterials[i].Num >= item.MaterialNums[i] ? UIEquipMaterialItem.EStatus.Enough : UIEquipMaterialItem.EStatus.Lack;
                data.RealValue = storageMaterials[i].Num;
            }
        }
    }

    private static Dictionary<EAttribute, UIValueItemData.BonusData>
            convertBonus(EBonus[] bonus, int[] bonusValues)
    {
        Dictionary<EAttribute, UIValueItemData.BonusData> bonusData = new Dictionary<EAttribute, UIValueItemData.BonusData>();
        convertBonus(bonus, bonusValues, ref bonusData);

        return bonusData;
    }

    private static void convertBonus(EBonus[] bonus, int[] bonusValues, 
            ref Dictionary<EAttribute, UIValueItemData.BonusData> boundsData)
    {
        for(int i = 0; i < bonus.Length; i++)
        {
            if(bonus[i] == EBonus.None)
                continue;

            EAttribute attribute = convert(bonus[i]);
            if(boundsData.ContainsKey(attribute))
            {
                boundsData[attribute].Value += bonusValues[i];
            }
            else
            {
                var data = new UIValueItemData.BonusData
                {
                    Icon = string.Format("AttrKind_{0}", bonus[i].GetHashCode()),
                    Name = convertBounsName(bonus[i]),
                    Value = bonusValues[i],
                    Bonus = bonus[i]
                };
                boundsData.Add(attribute, data);
            }
        }
    }

    private static string convertBounsName(EBonus bonus)
    {
        switch(bonus)
        {
            case EBonus.Point2: return TextConst.S(10501);
            case EBonus.Point3: return TextConst.S(10502);
            case EBonus.Speed: return TextConst.S(10503);
            case EBonus.Stamina: return TextConst.S(10504);
            case EBonus.Strength: return TextConst.S(10505);
            case EBonus.Dunk: return TextConst.S(10506);
            case EBonus.Rebound: return TextConst.S(10507);
            case EBonus.Block: return TextConst.S(10508);
            case EBonus.Defence: return TextConst.S(10509);
            case EBonus.Steal: return TextConst.S(10510);
            case EBonus.Dribble: return TextConst.S(10511);
            case EBonus.Pass: return TextConst.S(10512);
        }

        throw new NotImplementedException(bonus.ToString());
    }

    private static Dictionary<EAttribute, UIValueItemData.BonusData>
            convertInlayBonus(int[] playerInlayItemIDs)
    {
        Dictionary<EAttribute, UIValueItemData.BonusData> bonusData = new Dictionary<EAttribute, UIValueItemData.BonusData>();

        if (playerInlayItemIDs != null) {
            foreach(int inlayItemID in playerInlayItemIDs)
            {
                if(!GameData.DItemData.ContainsKey(inlayItemID))
                    continue;

                TItemData item = GameData.DItemData[inlayItemID];
                convertBonus(item.Bonus, item.BonusValues, ref bonusData);
            }
        }
        
        return bonusData;
    }

    private static EAttribute convert(EBonus bonus)
    {
        switch(bonus)
        {
            case EBonus.Point2: return EAttribute.Point2;
            case EBonus.Point3: return EAttribute.Point3;
            case EBonus.Speed: return EAttribute.Speed;
            case EBonus.Stamina: return EAttribute.Stamina;
            case EBonus.Strength: return EAttribute.Strength;
            case EBonus.Dunk: return EAttribute.Dunk;
            case EBonus.Rebound: return EAttribute.Rebound;
            case EBonus.Block: return EAttribute.Block;
            case EBonus.Defence: return EAttribute.Defence;
            case EBonus.Steal: return EAttribute.Steal;
            case EBonus.Dribble: return EAttribute.Dribble;
            case EBonus.Pass: return EAttribute.Pass;
        }

        throw new NotImplementedException(string.Format("Bouns:{0}", bonus));
    }

    private static bool[] convertInlayStatus(int[] inlayItemIDs)
    {
        if (inlayItemIDs != null) {
            bool[] status = new bool[inlayItemIDs.Length];

            for(var i = 0; i < inlayItemIDs.Length; i++)
            {
                status[i] = inlayItemIDs[i] > 0;
            }

            return status;
        } else
            return new bool[0];
    }
}
