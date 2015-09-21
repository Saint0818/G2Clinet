using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 負責創角時 3D 相關的東西(模型等等).
/// </summary>
/// <remarks>
/// <list type="number">
/// <item> Call Get 取得 instance. </item>
/// <item> Call Show() or Hide() 控制 UI 要不要顯示. </item>
/// <item> Call Select() 通知某個 3D 模型被選擇. </item>
/// <item> Call GetPlayerID() 取得資訊. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UI3DCreateRole : UIBase
{
    private static UI3DCreateRole instance = null;
    private const string UIName = "UI3DCreateRole";

    private readonly Dictionary<EPlayerPostion, GameObject> mModels = new Dictionary<EPlayerPostion, GameObject>();
    private readonly Dictionary<EPlayerPostion, int> mPlayerIDs = new Dictionary<EPlayerPostion, int>
    {
        {EPlayerPostion.G, 1},
        {EPlayerPostion.F, 2},
        {EPlayerPostion.C, 3}
    };

    private UI3DCreateRoleHelp mHelp;

    [UsedImplicitly]
    private void Awake()
    {
        mHelp = GetComponent<UI3DCreateRoleHelp>();

        // 現在的版本是讓玩家可以選擇 ID: 1, 2, 3 的角色.
        var obj = UICreateRole.CreateModel(mHelp.GuardParent, "GuardModel", mPlayerIDs[EPlayerPostion.G],
            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.G)[0],
            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.G)[0],
            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.G)[0],
            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.G)[0],
            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.G)[0]);
        obj.AddComponent<SelectEvent>(); // 避免發生 Error.
        mModels.Add(EPlayerPostion.G, obj);

        obj = UICreateRole.CreateModel(mHelp.ForwardParent, "ForwardModel", mPlayerIDs[EPlayerPostion.F],
            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.F)[0],
            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.F)[0],
            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.F)[0],
            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.F)[0],
            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.F)[0]);
        obj.AddComponent<SelectEvent>(); // 避免發生 Error.
        mModels.Add(EPlayerPostion.F, obj);

        obj = UICreateRole.CreateModel(mHelp.CenterParent, "CenterModel", mPlayerIDs[EPlayerPostion.C],
            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.C)[0],
            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.C)[0],
            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.C)[0],
            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.C)[0],
            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.C)[0]);
        obj.AddComponent<SelectEvent>(); // 避免發生 Error.
        mModels.Add(EPlayerPostion.C, obj);
    }

    [UsedImplicitly]
    private void Start()
    {
    }

    public void Show()
    {
        Show(true);

        foreach(KeyValuePair<EPlayerPostion, GameObject> pair in mModels)
        {
            pair.Value.GetComponent<Animator>().SetTrigger("SelectDown");
        }
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

    public int GetPlayerID(EPlayerPostion pos)
    {
        return mPlayerIDs[pos];
    }
}
