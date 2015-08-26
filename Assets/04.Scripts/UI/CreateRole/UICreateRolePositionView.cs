using System.ComponentModel;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRolePositionView : MonoBehaviour
{
    public GameObject Window;
    public Transform ModelPreview;

    public UILabel PosGLabel;
    public UILabel PosFLabel;
    public UILabel PosCLabel;
    public UILabel PosNameLabel;
    public UILabel PosDescriptionLabel;
//    public UIToggle GuardToggle;

    private GameObject mGuardModel;
    private GameObject mForwardModel;
    private GameObject mCenterModel;

    private EPlayerPostion mCurrentPostion = EPlayerPostion.G;

    [UsedImplicitly]
    private void Awake()
    {
        mGuardModel = UICreateRole.CreateModel(EPlayerPostion.G, ModelPreview);
        mGuardModel.SetActive(false);
        mForwardModel = UICreateRole.CreateModel(EPlayerPostion.F, ModelPreview);
        mForwardModel.SetActive(false);
        mCenterModel = UICreateRole.CreateModel(EPlayerPostion.C, ModelPreview);
        mCenterModel.SetActive(false);
    }

    [UsedImplicitly]
    private void Start()
    {
        selectPos(EPlayerPostion.G);
    }

    private void selectPos(EPlayerPostion pos)
    {
        mGuardModel.SetActive(false);
        mForwardModel.SetActive(false);
        mCenterModel.SetActive(false);
        mCurrentPostion = pos;

	    if(pos == EPlayerPostion.G)
	    {
            mGuardModel.SetActive(true);
	        PosGLabel.text = TextConst.S(21);
	        PosNameLabel.text = TextConst.S(15);
	        PosDescriptionLabel.text = TextConst.S(18);
	    }
        else if(pos == EPlayerPostion.F)
        {
            mForwardModel.SetActive(true);
            PosFLabel.text = TextConst.S(22);
            PosNameLabel.text = TextConst.S(16);
            PosDescriptionLabel.text = TextConst.S(18);
        }
        else if (pos == EPlayerPostion.C)
        {
            mCenterModel.SetActive(true);
            PosFLabel.text = TextConst.S(23);
            PosNameLabel.text = TextConst.S(17);
            PosDescriptionLabel.text = TextConst.S(19);
        }
        else
            throw new InvalidEnumArgumentException(pos.ToString());
    }

    public bool Visible
    {
        set
        {
            Window.SetActive(value);

//            GuardToggle.Set(true);
//            selectPos(EPlayerPostion.G);
        }
    }

    public void OnGuardClicked()
    {
//        Debug.LogFormat("Guard, Value:{0}", UIToggle.current.value);
        if(UIToggle.current.value)
            selectPos(EPlayerPostion.G);
    }

    public void OnForwardClicked()
    {
        //        Debug.LogFormat("Forward, Value:{0}", UIToggle.current.value);
        if(UIToggle.current.value)
            selectPos(EPlayerPostion.F);
    }

    public void OnCenterClicked()
    {
        //        Debug.LogFormat("Center, Value:{0}", UIToggle.current.value);

        if(UIToggle.current.value)
            selectPos(EPlayerPostion.C);
    }

    public void OnBackClicked()
    {
        GetComponent<UICreateRole>().ShowFrameView();
    }

    public void OnNextClicked()
    {
        GetComponent<UICreateRole>().ShowStyleView(mCurrentPostion);
    }
}