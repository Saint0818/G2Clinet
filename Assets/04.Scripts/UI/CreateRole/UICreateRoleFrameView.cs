using UnityEngine;
using JetBrains.Annotations;

[DisallowMultipleComponent]
public class UICreateRoleFrameView : MonoBehaviour
{
    public GameObject Window;
    public UICreateRolePlayerFrame[] Frames;

    public string[] PosSpriteNames;

    [UsedImplicitly]
	private void Awake()
    {
        for(int i = 0; i < Frames.Length; i++)
        {
            Frames[i].PosSpriteNames = PosSpriteNames;
//            Frames[i].SetPlayer(GameData.DPlayers[i]);
        }
    }

    public bool Visible
    {
        set { Window.SetActive(value);}
    }

    public void OnSlot1Clicked()
    {
        GetComponent<UICreateRole>().ShowPositionView();
    }

    public void OnSlot2Clicked()
    {
        GetComponent<UICreateRole>().ShowPositionView();
    }

    public void OnSlot3Clicked()
    {
        GetComponent<UICreateRole>().ShowPositionView();
    }
}
