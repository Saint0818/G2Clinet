using UnityEngine;

public class UICreateRoleStyleViewGroupButton : MonoBehaviour
{
    public UILabel NameLabel;

    public void Show(string buttonName)
    {
        gameObject.SetActive(true);

        NameLabel.text = buttonName;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetSelected()
    {
//        if(gameObject.activeInHierarchy)
            GetComponent<UIToggle>().Set(true);
    }

    public void SetUnSelected()
    {
        GetComponent<UIToggle>().Set(false);
    }
}
