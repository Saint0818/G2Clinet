using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRoleStyleView : MonoBehaviour
{
    public GameObject Window;
    public Transform ModelPreview;

    private GameObject mModel;

    [UsedImplicitly]
    private void Start()
    {
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

    public void OnBackClicked()
    {
        GetComponent<UICreateRole>().ShowPositionView();
    }

    public void OnNextClicked()
    {
        Debug.Log("Next Button Click!");
    }
}