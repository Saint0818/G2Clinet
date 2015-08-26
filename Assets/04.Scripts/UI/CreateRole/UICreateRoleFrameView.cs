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
