using UnityEngine;
using JetBrains.Annotations;

[DisallowMultipleComponent]
public class UICreateRoleFrameView : MonoBehaviour
{
    public GameObject Window;
    public UICreateRolePlayerFrame Frame1;
    public UICreateRolePlayerFrame Frame2;
    public UICreateRolePlayerFrame Frame3;

    public string[] PosSpriteNames;

    [UsedImplicitly]
	private void Awake()
    {
        Frame1.PosSpriteNames = PosSpriteNames;
        Frame2.PosSpriteNames = PosSpriteNames;
        Frame3.PosSpriteNames = PosSpriteNames;
    }

    [UsedImplicitly]
    // Update is called once per frame
    private void Update()
    {
	
	}

    public bool Visible
    {
        set { Window.SetActive(value);}
    }
}
