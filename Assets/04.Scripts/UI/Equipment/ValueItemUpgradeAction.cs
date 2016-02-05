using System;
using GameStruct;
using UnityEngine;

public class ValueItemUpgradeAction : ActionQueue.IAction
{
    private readonly int mPlayerValueItemKind;
    private bool mIsDone;
    private bool mDoneResult;

    public ValueItemUpgradeAction(int playerValueItemKind)
    {
        mPlayerValueItemKind = playerValueItemKind;
    }

    public void Do()
    {
        mIsDone = false;

        TValueItem valueItem = GameData.Team.Player.ValueItems[mPlayerValueItemKind];
        TItemData item = GameData.DItemData[valueItem.ID];
        if(UIEquipChecker.FindStatus(item, valueItem.RevisionInlayItemIDs) == UIValueItemData.EStatus.Upgradeable)
        {
            var upgradeCommand = new ValueItemUpgradeProtocol();
            upgradeCommand.Send(mPlayerValueItemKind, onUpgrade);
        }
        else if(!UIEquipChecker.HasUpgradeItem(item))
        {
            // 是最高等級, 所以不能升級.
            Debug.Log("Top Level Item.");
            UIHint.Get.ShowHint(TextConst.S(553), Color.white);
        }
        else if(!UIEquipChecker.IsInlayFull(item, valueItem.RevisionInlayItemIDs))
        {
            // 材料沒有鑲嵌完畢.
            Debug.Log("Inlay not full.");
            UIHint.Get.ShowHint(TextConst.S(551), Color.white);
        }
        else if(!UIEquipChecker.HasUpgradeMoney(item))
        {
            // 沒錢.
            Debug.Log("Money not enoguh.");
            UIHint.Get.ShowHint(TextConst.S(552), Color.white);
        }
        else if(!UIEquipChecker.IsLevelEnough(item))
        {
            // 等級不足.
            Debug.Log("Level not enough.");
            UIHint.Get.ShowHint(String.Format(TextConst.S(6010), item.UpgradeLv), Color.white);
        }
        else
            Debug.LogError("Not Implemented...");
    }

    public bool IsDone()
    {
        return mIsDone;
    }

    public bool DoneResult()
    {
        return mDoneResult;
    }

    private void onUpgrade(bool ok)
    {
        if (ok)
            UIHint.Get.ShowHint(TextConst.S(555), Color.white);

        mDoneResult = ok;
        mIsDone = true;
    }
}