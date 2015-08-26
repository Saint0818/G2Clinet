using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRoleStyleView : MonoBehaviour
{
    public GameObject Window;
    public Transform ModelPreview;
    public UICreateRolePartButton HairButton;
    public UICreateRolePartButton ClothButton;
    public UICreateRolePartButton PantsButton;
    public UICreateRolePartButton ShoesButton;

    private GameObject mModel;

    [UsedImplicitly]
    private void Start()
    {
        HairButton.Play();
        ClothButton.Hide();
        PantsButton.Hide();
        ShoesButton.Hide();
    }

    public void Show(EPlayerPostion pos)
    {
        Window.SetActive(true);

        if(mModel)
            Destroy(mModel);
        mModel = UICreateRole.CreateModel(pos, ModelPreview);
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    public void OnHairClicked()
    {
        if(UIToggle.current.value)
        {
//            Debug.Log("OnHairClicked");
            HairButton.Play();
            ClothButton.Hide();
            PantsButton.Hide();
            ShoesButton.Hide();
        }
    }

    public void OnClothClicked()
    {
        if(UIToggle.current.value)
        {
//            Debug.Log("OnClothClicked");
            HairButton.Hide();
            ClothButton.Play();
            PantsButton.Hide();
            ShoesButton.Hide();
        }
    }

    public void OnPantsClicked()
    {
        if(UIToggle.current.value)
        {
//            Debug.Log("OnPantsClicked");
            HairButton.Hide();
            ClothButton.Hide();
            PantsButton.Play();
            ShoesButton.Hide();
        }
    }

    public void OnShoesClicked()
    {
        if(UIToggle.current.value)
        {
//            Debug.Log("OnShoesClicked");
            HairButton.Hide();
            ClothButton.Hide();
            PantsButton.Hide();
            ShoesButton.Play();
        }
    }

    public void OnBackClicked()
    {
        GetComponent<UICreateRole>().ShowPositionView();
    }

    public void OnNextClicked()
    {
        Debug.Log("Next Button Click!");
    }
}