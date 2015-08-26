using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRolePositionView : MonoBehaviour
{
    public GameObject Window;
    public UILabel PosGLabel;
    public UILabel PosFLabel;
    public UILabel PosCLabel;
    public UILabel PosNameLabel;
    public UILabel PosDescriptionLabel;

    [UsedImplicitly]
    private void Start()
    {
        updateUI(EPlayerPostion.G);
    }

    [UsedImplicitly]
    private void updateUI(EPlayerPostion pos)
    {
	    if(pos == EPlayerPostion.G)
	    {
	        PosGLabel.text = TextConst.S(21);
	        PosNameLabel.text = TextConst.S(15);
	        PosDescriptionLabel.text = TextConst.S(18);
	    }
        else if(pos == EPlayerPostion.F)
        {
            PosFLabel.text = TextConst.S(22);
            PosNameLabel.text = TextConst.S(16);
            PosDescriptionLabel.text = TextConst.S(18);
        }
        else if (pos == EPlayerPostion.C)
        {
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
        updateUI(EPlayerPostion.G);
    }

    public void OnForwardClicked()
    {
        updateUI(EPlayerPostion.F);
    }

    public void OnCenterClicked()
    {
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