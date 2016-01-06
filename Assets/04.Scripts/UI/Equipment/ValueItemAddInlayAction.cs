using GameStruct;
using UnityEngine;

public class ValueItemAddInlayAction : ActionQueue.IAction
{
    private readonly int mPlayerValueItemKind;
    private readonly int mStorageMaterialItemIndex;
    private bool mIsDone;
    private bool mDoneResult;

    public ValueItemAddInlayAction(int playerValueItemKind, int storageMaterialItemIndex)
    {
        mPlayerValueItemKind = playerValueItemKind;
        mStorageMaterialItemIndex = storageMaterialItemIndex;
    }

    public void Do()
    {
        mIsDone = false;

        TValueItem valueItem = GameData.Team.Player.ValueItems[mPlayerValueItemKind];
        if (!GameData.DItemData.ContainsKey(valueItem.ID))
        {
            Debug.LogErrorFormat("Can't find ItemData, ItemID:{0}", valueItem.ID);
            mIsDone = true;
            return;
        }

        if(mStorageMaterialItemIndex >= GameData.Team.MaterialItems.Length)
        {
            Debug.LogErrorFormat("Can't find MaterialItem. Index:{0}", mStorageMaterialItemIndex);
            mIsDone = true;
            return;
        }
        TMaterialItem materialItem = GameData.Team.MaterialItems[mStorageMaterialItemIndex];

        if(!GameData.DItemData.ContainsKey(valueItem.ID))
        {
            Debug.LogErrorFormat("Can't found ItemData, ID:{0}", valueItem.ID);
            mIsDone = true;
            return;
        }
        TItemData itemData = GameData.DItemData[valueItem.ID];
        if (!itemData.HasMaterial(materialItem.ID))
        {
            Debug.LogErrorFormat("Material is not the inlay. ItemID:{0}, MaterailItemID:{1}", itemData.ID, materialItem.ID);
            mIsDone = true;
            return;
        }

        if (valueItem.HasInlay(materialItem.ID))
        {
            // 已鑲嵌, 點擊不做任何事情.
            Debug.Log("Alreay Inaly");
            mIsDone = true;
            return;
        }

        if (materialItem.Num >= itemData.FindMaterialNum(materialItem.ID))
        {
            // 材料足夠.
            var protocol = new ValueItemAddInlayProtocol();
            protocol.Send(mPlayerValueItemKind, mStorageMaterialItemIndex, onAddInlay);
        }
        else
        {
            // 材料不夠.
            mIsDone = true;
            Debug.Log("Show Navigation Window!");
        }
    }

    public bool IsDone()
    {
        return mIsDone;
    }

    public bool DoneResult()
    {
        return mDoneResult;
    }

    private void onAddInlay(bool ok)
    {
        mDoneResult = ok;
        mIsDone = true;
    }
}