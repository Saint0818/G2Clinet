using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// UICreateRoleFrameView 會使用的元件, 專門用來顯示某位角色的相關資訊.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> Call Clear() 重置. </item>
/// <item> Call SetData() 設定要顯示的資料. </item>
/// <item> Call SetLock() 將 Frame 設定為鎖定狀態. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UICreateRolePlayerFrame : MonoBehaviour
{
    public delegate void Action(TPlayerBank bank, bool isLock);
    public event Action OnClickListener;
    public event Action OnDeleteListener;

    public UISprite PlusButton;
    public GameObject RemoveButton;
    public UISprite BGLeft;
    public UISprite BGRight;

    public GameObject PlayerInfo;
    public UISprite PosSprite;
    public UILabel NameLabel;
    public UILabel LevelLabel;

    [HideInInspector]
    public string[] PosSpriteNames;

    [HideInInspector]
    public string LockButtonSpriteName;

    [HideInInspector]
    public string LockBGSpriteName;

    private TPlayerBank mPlayerBank;

    private bool mIsLock;

	[UsedImplicitly]
	private void Awake()
    {
	    Clear();
	}

    public void Clear()
    {
        PlusButton.gameObject.SetActive(true);
        RemoveButton.SetActive(false);
        PlayerInfo.SetActive(false);

        mPlayerBank = new TPlayerBank();

        mIsLock = false;
    }

    public void SetLock()
    {
        PlusButton.spriteName = LockButtonSpriteName;
        BGLeft.spriteName = LockBGSpriteName;
        BGRight.spriteName = LockBGSpriteName;
        RemoveButton.SetActive(false);
        PlayerInfo.SetActive(false);

        mIsLock = true;
    }

    public void SetData(TPlayerBank player)
    {
        if(!player.IsValid || !GameData.DPlayers.ContainsKey(player.ID))
        {
            Debug.LogErrorFormat("PlayerID({0}) don't exit.", mPlayerBank.ID);
            return;
        }

        PlusButton.gameObject.SetActive(false);
        RemoveButton.SetActive(true);
        PlayerInfo.SetActive(true);

        mIsLock = false;

        mPlayerBank = player;
        
//        int bodyType = GameData.DPlayers[mPlayerBank.ID].BodyType;
//        if(bodyType < 0 || bodyType >= PosSpriteNames.Length)
//        {
//            Debug.LogErrorFormat("BodyType({0}) error.", bodyType);
//            return;
//        }
    }

    /// <summary>
    /// NGUI 會自動偵測的 callback.
    /// </summary>
    public void OnClick()
    {
        if(OnClickListener != null)
            OnClickListener(mPlayerBank, mIsLock);
    }

    /// <summary>
    /// 呼叫時機: X 按鈕按下.
    /// </summary>
    public void OnDeleteClick()
    {
        if(OnDeleteListener != null)
            OnDeleteListener(mPlayerBank, mIsLock);
    }
}
