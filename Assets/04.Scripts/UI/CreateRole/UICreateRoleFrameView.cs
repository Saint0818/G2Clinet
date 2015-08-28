using GameStruct;
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
            Frames[i].OnClickListener += OnSlotClick;
        }
    }

    public void Show()
    {
        Window.SetActive(true);
    }

    public void Show([NotNull] TPlayerBank[] playerBanks)
    {
        Window.SetActive(true);

        for(int i = 0; i < Frames.Length; i++)
        {
            Frames[i].Clear();

            if(i >= playerBanks.Length)
                continue;

            Frames[i].SetData(playerBanks[i]);
        }
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    public void OnSlotClick(TPlayerBank playerBank)
    {
        if(playerBank.IsValid)
        {
            
        }
        else
            GetComponent<UICreateRole>().ShowPositionView();
    }
}
