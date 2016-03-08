using System;
using System.Linq;
using GameStruct;
using JetBrains.Annotations;

public struct TValueItem
{
    public int ID;

    /// <summary>
    /// 鑲嵌物品的 ItemID.(這個只是用在接 Json 讀出的資料, 不要使用, 改使用 RealInlayItemIDs)
    /// </summary>
    [CanBeNull]public int[] InlayItemIDs;

    /// <summary>
    /// 堆疊數量.
    /// </summary>
    public int Num;

    /// <summary>
    /// 經過校正的鑲嵌資訊.
    /// </summary>
    [NotNull]
    public int[] RevisionInlayItemIDs
    {
        get
        {
            if(!GameData.DItemData.ContainsKey(ID))
                return new int[0];

            TItemData item = GameData.DItemData[ID];

            if(InlayItemIDs == null)
                InlayItemIDs = new int[item.ReviseMaterials.Length];
            else if(InlayItemIDs.Length < item.ReviseMaterials.Length)
            {
                // 不足的部份補上 0.
                int[] revisionInlayItemIDs = new int[item.ReviseMaterials.Length];
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

    public int GetTotalPoint()
    {
        int totalPoints = getSumAttriValues(ID);

        if(InlayItemIDs != null)
        {
            foreach(int inlayItemID in InlayItemIDs)
            {
                totalPoints += getSumAttriValues(inlayItemID);
            }
        }

        return totalPoints;
    }

    private int getSumAttriValues(int itemID)
    {
        if(GameData.DItemData.ContainsKey(itemID))
            return GameData.DItemData[itemID].BonusValues.Sum();
        return 0;
    }

    public override string ToString()
    {
        return string.Format("ID: {0}, InlayItemIDs: {1}", ID, InlayItemIDs);
    }
}
