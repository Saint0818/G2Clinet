using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRolePositionView : MonoBehaviour
{
    public GameObject Window;
    public Transform ModelPreview;

//    public UILabel PosGLabel;
//    public UILabel PosFLabel;
//    public UILabel PosCLabel;
    public UILabel PosNameLabel;
    public UILabel PosDescriptionLabel;

//    private GameObject mGuardModel;
//    private GameObject mForwardModel;
//    private GameObject mCenterModel;

    private EPlayerPostion mCurrentPostion = EPlayerPostion.G;

    [UsedImplicitly]
    private void Awake()
    {
        Visible = false;

//        // 現在的版本是讓玩家可以選擇 ID: 1, 2, 3 的角色.
//        mGuardModel = UICreateRole.CreateModel(ModelPreview, "G", 1,
//            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.G)[0],
//            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.G)[0],
//            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.G)[0],
//            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.G)[0],
//            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.G)[0]);
//        mForwardModel = UICreateRole.CreateModel(ModelPreview, "F", 2,
//            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.F)[0],
//            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.F)[0],
//            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.F)[0],
//            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.F)[0],
//            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.F)[0]);
//        mCenterModel = UICreateRole.CreateModel(ModelPreview, "C", 3,
//            CreateRoleDataMgr.Ins.GetBody(EPlayerPostion.C)[0],
//            CreateRoleDataMgr.Ins.GetHairs(EPlayerPostion.C)[0],
//            CreateRoleDataMgr.Ins.GetCloths(EPlayerPostion.C)[0],
//            CreateRoleDataMgr.Ins.GetPants(EPlayerPostion.C)[0],
//            CreateRoleDataMgr.Ins.GetShoes(EPlayerPostion.C)[0]);
    }

    [UsedImplicitly]
    private void Start()
    {
        mCurrentPostion = EPlayerPostion.G;

        updateUI(mCurrentPostion);
//        updateModel(mCurrentPostion);
    }

    private void updateUI(EPlayerPostion pos)
    {
        switch(pos)
        {
            case EPlayerPostion.G:
//                PosGLabel.text = TextConst.S(21);
                PosNameLabel.text = TextConst.S(15);
                PosDescriptionLabel.text = TextConst.S(18);
                break;
            case EPlayerPostion.F:
//                PosFLabel.text = TextConst.S(22);
                PosNameLabel.text = TextConst.S(16);
                PosDescriptionLabel.text = TextConst.S(19);
                break;
            case EPlayerPostion.C:
//                PosCLabel.text = TextConst.S(23);
                PosNameLabel.text = TextConst.S(17);
                PosDescriptionLabel.text = TextConst.S(20);
                break;

            default:
                throw new InvalidEnumArgumentException(pos.ToString());
        }
    }

    public bool Visible
    {
        set
        {
            Window.SetActive(value);
        }
    }

    public void OnGuardClick()
    {
        if(UIToggle.current.value)
        {
            mCurrentPostion = EPlayerPostion.G;
            updateUI(mCurrentPostion);

            UI3DCreateRole.Get.Select(EPlayerPostion.G);
        }
    }

    public void OnForwardClick()
    {
        if(UIToggle.current.value)
        {
            mCurrentPostion = EPlayerPostion.F;

            updateUI(mCurrentPostion);

            UI3DCreateRole.Get.Select(EPlayerPostion.F);
        }
    }

    public void OnCenterClick()
    {
        if(UIToggle.current.value)
        {
            mCurrentPostion = EPlayerPostion.C;

            updateUI(mCurrentPostion);

            UI3DCreateRole.Get.Select(EPlayerPostion.C);
        }
    }

    public void OnBackClick()
    {
        GetComponent<UICreateRole>().ShowFrameView();
    }

    public void OnNextClick()
    {
        GetComponent<UICreateRole>().ShowStyleView(mCurrentPostion);
    }
}