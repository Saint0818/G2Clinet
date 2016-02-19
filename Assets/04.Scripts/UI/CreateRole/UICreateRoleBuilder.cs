using System;
using GameEnum;
using GameStruct;

public static class UICreateRoleBuilder
{
    public static UICreateRolePlayerSlot.Data[] Build(TPlayerBank[] bank)
    {
        UICreateRolePlayerSlot.Data[] data = new UICreateRolePlayerSlot.Data[UICreateRole.Get.FrameView.Slots.Length];
        for(int i = 0; i < data.Length; i++)
        {
            if(bank.Length > i)
                data[i] = build(bank[i]);
            else if(GameData.Team.MaxPlayerBank > i)
                data[i] = buildEmpty();
            else if(GameData.Team.Player.Lv < LimitTable.Ins.GetLv(EOpenID.CreateRole))
                data[i] = buildLockLv();
            else
                data[i] = buildLockDiamond();
        }

        return data;
    }

    private static UICreateRolePlayerSlot.Data build(TPlayerBank bank)
    {
        return new UICreateRolePlayerSlot.Data
        {
            Status = UICreateRolePlayerSlot.Data.EStatus.Valid,
            PlayerID = bank.ID,
            RoleIndex = bank.RoleIndex,
            Position = (EPlayerPostion)GameData.DPlayers[bank.ID].BodyType,
            Name = bank.Name ?? String.Empty,
            Lv = bank.Lv
        };
    }

    private static UICreateRolePlayerSlot.Data buildEmpty()
    {
        return new UICreateRolePlayerSlot.Data
        {
            Status = UICreateRolePlayerSlot.Data.EStatus.Empty
        };
    }

    private static UICreateRolePlayerSlot.Data buildLockLv()
    {
        return new UICreateRolePlayerSlot.Data
        {
            Status = UICreateRolePlayerSlot.Data.EStatus.LockLv,
            Message = string.Format(TextConst.S(512), LimitTable.Ins.GetLv(EOpenID.CreateRole)) 
        };
    }

    private static UICreateRolePlayerSlot.Data buildLockDiamond()
    {
        Func<string> needDiamond = () =>
        {
            int diamond = 0;
            TLimitData data = LimitTable.Ins.GetByOpenID(EOpenID.CreateRole);
            if(data != null)
                diamond = data.Diamond;
            return string.Format(TextConst.S(513), diamond);
        };

        return new UICreateRolePlayerSlot.Data
        {
            Status = UICreateRolePlayerSlot.Data.EStatus.LockDiamond,
            Message = needDiamond()
        };
    }
}