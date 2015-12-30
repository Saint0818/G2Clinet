using System;
using System.Collections.Generic;
using GameStruct;
using UnityEngine;

public class UIEquipUtility
{
    public static UIValueItemData Build(TItemData item, int[] inlayItemIDs)
    {
        return build(item, inlayItemIDs, findStorageMaterialNums(item));
    }

    private static int[] findStorageMaterialNums(TItemData item)
    {
        List<int> storageMaterialNums = new List<int>();

        for(var i = 0; i < item.Materials.Length; i++)
        {
            if(GameData.Team.HasMaterialItemByItemID(item.Materials[i]))
            {
                TMaterialItem materialItem = GameData.Team.FindMaterialItemByItemID(item.Materials[i]);
                storageMaterialNums.Add(materialItem.Num);
            }
            else
            {
                storageMaterialNums.Add(0);
            }
        }

        return storageMaterialNums.ToArray();
    }

    /// <summary>
    /// 將玩家的某件數值裝資料轉換成介面使用的資料.
    /// </summary>
    /// <param name="item"> 數值裝的道具資料. </param>
    /// <param name="inlayItemIDs"> 數值裝鑲嵌的狀態(4 個 element), [0]:Material1 鑲嵌材料; 
    ///                             [1]:Material2 鑲嵌材料, 以此類推.</param>
    /// <param name="storageMaterialNums"> 倉庫材料數目(4 個 element), [0]:Material1 擁有的數量, 
    ///                                    [1]:Material2 擁有的數量, 以此類推. </param>
    /// <returns></returns>
    private static UIValueItemData build(TItemData item, int[] inlayItemIDs, int[] storageMaterialNums)
    {
        UIValueItemData valueItem = new UIValueItemData
        {
            Name = item.Name,
            Atlas = GameData.DItemAtlas.ContainsKey(GameData.AtlasName(item.Atlas))
                    ? GameData.DItemAtlas[GameData.AtlasName(item.Atlas)]
                    : null,
            Icon = string.Format("Item_{0}", item.Icon),
            Frame = string.Format("Equipment_{0}", item.Quality),
            Desc = item.Explain,
            Values = convertBonus(item.Bonus, item.BonusValues)
        };

        buildInlays(item, inlayItemIDs, storageMaterialNums, ref valueItem);

        return valueItem;
    }
    
    private static void buildInlays(TItemData item, int[] inlayItemIDs, int[] storageMaterialNums,
                                    ref UIValueItemData valueItem)
    {
        valueItem.Inlays.Clear();

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
                Icon = string.Format("Item_{0}", materialItem.Icon),
                NeedValue = item.MaterialNums[i],
                Values = convertBonus(materialItem.Bonus, materialItem.BonusValues)
            };
            valueItem.Inlays.Add(data);

            if(i < inlayItemIDs.Length && inlayItemIDs[i] > 0)
            {
                // 有鑲嵌物.
                data.Status = UIEquipMaterialItem.EStatus.Inlayed;
                data.RealValue = data.NeedValue;
            }
            else
            {
                // 沒有鑲嵌.
                data.Status = storageMaterialNums[i] >= item.MaterialNums[i] ? UIEquipMaterialItem.EStatus.Enough : UIEquipMaterialItem.EStatus.Lack;
                data.RealValue = storageMaterialNums[i];
            }
        }
    }

    private static Dictionary<EAttribute, UIValueItemData.BonusData> 
            convertBonus(EBonus[] bonus, int[] bonusValues)
    {
        Dictionary<EAttribute, UIValueItemData.BonusData> values = new Dictionary<EAttribute, UIValueItemData.BonusData>();
        for(int i = 0; i < bonus.Length; i++)
        {
            if(bonus[i] == EBonus.None)
                continue;

            var data = new UIValueItemData.BonusData
            {
                Icon = string.Format("AttrKind_{0}", bonus[i].GetHashCode()),
                Value = bonusValues[i]
            };
            values.Add(convert(bonus[i]), data);
        }
        return values;
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
}
