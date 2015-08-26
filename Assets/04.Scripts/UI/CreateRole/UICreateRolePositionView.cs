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

    private GameObject mGuardModel;
    private GameObject mForwardModel;
    private GameObject mCenterModel;

    [UsedImplicitly]
    private void Awake()
    {
        mGuardModel = createModel(EPlayerPostion.G);
        mGuardModel.SetActive(false);
        mForwardModel = createModel(EPlayerPostion.F);
        mForwardModel.SetActive(false);
        mCenterModel = createModel(EPlayerPostion.C);
        mCenterModel.SetActive(false);
    }

    [UsedImplicitly]
    private void Start()
    {
        updateUI(EPlayerPostion.G);
    }

    private GameObject createModel(EPlayerPostion pos)
    {
        TPlayer p;
        if(pos == EPlayerPostion.G)
            p = new TPlayer(0) {ID = 10};
        else if(pos == EPlayerPostion.F)
            p = new TPlayer(0) { ID = 20 };
        else if(pos == EPlayerPostion.C)
            p = new TPlayer(0) { ID = 30 };
        else
            throw new InvalidEnumArgumentException(pos.ToString());
        p.SetAvatar();

        GameObject model = new GameObject {name = pos.ToString()};
        ModelManager.Get.SetAvatar(ref model, p.Avatar, GameData.DPlayers[p.ID].BodyType, false);

        model.transform.parent = ModelPreview;
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;
        model.layer = LayerMask.NameToLayer("UI");
        foreach(Transform child in model.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
        }

        return model;
    }

    private void updateUI(EPlayerPostion pos)
    {
        mGuardModel.SetActive(false);
        mForwardModel.SetActive(false);
        mCenterModel.SetActive(false);

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
        set { Window.SetActive(value); }
    }

    public void OnGuardClicked()
    {
//        Debug.LogFormat("Guard, Value:{0}", UIToggle.current.value);
        if(UIToggle.current.value)
            updateUI(EPlayerPostion.G);
    }

    public void OnForwardClicked()
    {
        //        Debug.LogFormat("Forward, Value:{0}", UIToggle.current.value);
        if(UIToggle.current.value)
            updateUI(EPlayerPostion.F);
    }

    public void OnCenterClicked()
    {
        //        Debug.LogFormat("Center, Value:{0}", UIToggle.current.value);

        if(UIToggle.current.value)
            updateUI(EPlayerPostion.C);
    }

    public void OnBackClicked()
    {
        GetComponent<UICreateRole>().ShowFrameView();
    }

    public void OnNextClicked()
    {
        GetComponent<UICreateRole>().ShowStyleView();
    }
}