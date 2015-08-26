using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRoleStyleView : MonoBehaviour
{
    public GameObject Window;

    [UsedImplicitly]
    private void Start()
    {
    }

    public bool Visible
    {
        set { Window.SetActive(value); }
    }
}