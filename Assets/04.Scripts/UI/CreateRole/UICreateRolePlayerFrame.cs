using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// UICreateRoleFrameView 會使用的元件, 專門用來顯示某位角色的相關資訊.
/// </summary>
[DisallowMultipleComponent]
public class UICreateRolePlayerFrame : MonoBehaviour
{
    public delegate void Action(TPlayerBank bank);
    public event Action OnClickListener;
    public event Action OnDeleteListener;

    public GameObject PlusButton;
    public GameObject RemoveButton;
    public UISprite PosSprite;

    [HideInInspector]
    public string[] PosSpriteNames;

    private TPlayerBank mPlayerBank;

	[UsedImplicitly]
	private void Awake()
    {
	    Clear();
	}

    public void Clear()
    {
        PlusButton.SetActive(true);
        RemoveButton.SetActive(false);
        PosSprite.gameObject.SetActive(false);

        mPlayerBank = new TPlayerBank();
    }

    public void SetData(TPlayerBank player)
    {
        if(!player.IsValid || !GameData.DPlayers.ContainsKey(player.ID))
        {
            Debug.LogErrorFormat("PlayerID({0}) don't exit.", mPlayerBank.ID);
            return;
        }

        PlusButton.SetActive(false);
        RemoveButton.SetActive(true);
        PosSprite.gameObject.SetActive(true);

        mPlayerBank = player;
        
        int bodyType = GameData.DPlayers[mPlayerBank.ID].BodyType;
        if(bodyType < 0 || bodyType >= PosSpriteNames.Length)
        {
            Debug.LogErrorFormat("BodyType({0}) error.", bodyType);
            return;
        }

        PosSprite.spriteName = PosSpriteNames[bodyType];
    }

    public void OnClick()
    {
        if(OnClickListener != null)
            OnClickListener(mPlayerBank);
    }

    public void OnDeleteClick()
    {
        if(OnDeleteListener != null)
            OnDeleteListener(mPlayerBank);
    }
}
