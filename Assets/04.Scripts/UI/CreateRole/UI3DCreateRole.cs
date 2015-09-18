using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UI3DCreateRole : UIBase
{
    private static UI3DCreateRole instance = null;
    private const string UIName = "UI3DCreateRole";

    private readonly Dictionary<EPlayerPostion, GameObject> mModels = new Dictionary<EPlayerPostion, GameObject>();

    private UI3DCreateRoleHelp mHelp;

    [UsedImplicitly]
    private void Awake()
    {
        mHelp = GetComponent<UI3DCreateRoleHelp>();
    }

    [UsedImplicitly]
    private void Start()
    {
        // 現在的版本是讓玩家可以選擇 ID: 1, 2, 3 的角色.
        var obj = UICreateRole.CreateModel(mHelp.GuardParent, "GuardModel", 1,
            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.G)[0],
            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.G)[0],
            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.G)[0],
            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.G)[0],
            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.G)[0]);
        mModels.Add(EPlayerPostion.G, obj);

        obj = UICreateRole.CreateModel(mHelp.ForwardParent, "ForwardModel", 2,
            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.F)[0],
            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.F)[0],
            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.F)[0],
            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.F)[0],
            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.F)[0]);
        mModels.Add(EPlayerPostion.F, obj);

        obj = UICreateRole.CreateModel(mHelp.CenterParent, "CenterModel", 3,
            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.C)[0],
            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.C)[0],
            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.C)[0],
            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.C)[0],
            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.C)[0]);
        mModels.Add(EPlayerPostion.C, obj);
    }

    public void Show()
    {
        Show(true);
    }

    public void Hide()
    {
        RemoveUI(UIName);
    }

    public void Select(EPlayerPostion pos)
    {
        mHelp.SelectSFX.localPosition = mHelp.GetTransformByPos(pos).localPosition;
    }

    public static UI3DCreateRole Get
    {
        get
        {
            if(!instance)
            {
                UI3D.UIShow(true);
                instance = Load3DUI(UIName) as UI3DCreateRole;
            }

            return instance;
        }
    }
}
