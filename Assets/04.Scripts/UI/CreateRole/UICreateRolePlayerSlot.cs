using System.Collections;
using System.Collections.Generic;
using GameEnum;
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
/// <item> Call SetVisible() 控制要不要顯示. </item>
/// <item> Call PlayEnterAnimation() 控制 Slot 進入畫面的時間. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UICreateRolePlayerSlot : MonoBehaviour
{
    public struct Data
    {
        public int PlayerID;
        public int RoleIndex;
        public EPlayerPostion Position;
        [NotNull]public string Name;
        public int Level;

        public bool IsValid()
        {
            return PlayerID > 0 && RoleIndex >= 0 && Level >= 1;
        }

        public override string ToString()
        {
            return string.Format("PlayerID: {0}, RoleIndex: {1}, Position: {2}, Name: {3}, Level: {4}", PlayerID, RoleIndex, Position, Name, Level);
        }
    }

    public delegate void Action(int index, Data data, bool isLock);
    public event Action OnClickListener;
    public event Action OnDeleteListener; // 刪除角色的按鈕按下.

    private readonly Dictionary<EPlayerPostion, int> mPosTextIndices = new Dictionary<EPlayerPostion, int>
    {
        { EPlayerPostion.G, 21},
        { EPlayerPostion.F, 22},
        { EPlayerPostion.C, 23}
    };

    private readonly Dictionary<EPlayerPostion, string> mPosBGs = new Dictionary<EPlayerPostion, string>
    {
        { EPlayerPostion.G, "Nameplate_guard"},
        { EPlayerPostion.F, "Nameplate_forward"},
        { EPlayerPostion.C, "Nameplate_center"}
    };

    public GameObject Window;
    public UISprite CenterButton;
    public GameObject RemoveButton;
    public UISprite[] BGSprites;

    public GameObject PlayerInfo;
    public UILabel PosNameLabel;
    public UISprite PosBG;
    public UILabel NameLabel;
    public UILabel LevelLabel;
    public GameObject SelectedIcon; // 用來標示哪一位球員是目前正在使用的球員.
    public UISprite[] LeftRightMarks; // 左右兩邊的三角形圖示.

    /// <summary>
    /// 方便外部使用者可以得知哪一個 Slot 被點擊了.
    /// </summary>
    public int Index;

    private const string LockSpriteName = "Icon_lock";
    private const string LockBGSpriteName = "BtnLocked";

    private bool mIsLock;

    private const string AddSpriteName = "Icon_Create";
    private const string AddBGSpriteName = "BtnEmpty";

    private readonly Dictionary<EPlayerPostion, string> mSelectSpriteNames = new Dictionary<EPlayerPostion, string>
    {
        {EPlayerPostion.C, "BtnCircle0"},
        {EPlayerPostion.F, "BtnCircle2"},
        {EPlayerPostion.G, "BtnCircle1"}
    };
//    private readonly Dictionary<EPlayerPostion, string> mSelectBGSpriteNames = new Dictionary<EPlayerPostion, string>
//    {
//        {EPlayerPostion.C, "BtnCreatedCenter"},
//        {EPlayerPostion.F, "BtnCreatedForward"},
//        {EPlayerPostion.G, "BtnCreatedGuard"},
//    };

    private Data mData;

    [UsedImplicitly]
	private void Awake()
    {
	    Clear();
	}

    public void Clear()
    {
        CenterButton.spriteName = AddSpriteName;

        foreach(UISprite sprite in BGSprites)
        {
            sprite.spriteName = AddBGSpriteName;
        }
//        BGLeft.spriteName = AddBGSpriteName;
//        BGRight.spriteName = AddBGSpriteName;

        foreach(UISprite sprite in LeftRightMarks)
        {
            sprite.color = Color.white;
        }

        RemoveButton.SetActive(false);
        PlayerInfo.SetActive(false);
        SelectedIcon.SetActive(false);

        mData = new Data();

        mIsLock = false;

        GetComponent<Animator>().enabled = false;
    }

    private IEnumerator playAnimation(float delayTime, string animName = null)
    {
        var animator = GetComponent<Animator>();
        animator.enabled = false;
        yield return new WaitForSeconds(delayTime);
        SetVisible(true);
        animator.enabled = true;

        if(!string.IsNullOrEmpty(animName))
            animator.SetTrigger(animName);
    }

    public void PlayEnterAnimation(float delayTime)
    {
        StartCoroutine(playAnimation(delayTime, "Start"));
    }

    public void PlayOpenAnimation()
    {
//        GetComponent<Animator>().SetTrigger("Open");

        // 0.05 只是我 try and error 的數值. 只是要讓 Animator 關閉後, 再打開, 行為才是正常的.
        StartCoroutine(playAnimation(0.05f, "Open")); 
    }

    public void PlayExitAnimation()
    {
//        GetComponent<Animator>().SetTrigger("Down");

        // 0.05 只是我 try and error 的數值. 只是要讓 Animator 關閉後, 再打開, 行為才是正常的.
        StartCoroutine(playAnimation(0.05f, "Down"));
    }

    public void SetLock()
    {
        CenterButton.spriteName = LockSpriteName;
        foreach(UISprite sprite in BGSprites)
        {
            sprite.spriteName = LockBGSpriteName;
        }
//        BGLeft.spriteName = LockBGSpriteName;
//        BGRight.spriteName = LockBGSpriteName;

        foreach(UISprite sprite in LeftRightMarks)
        {
            sprite.color = Color.white;
        }

        RemoveButton.SetActive(false);
        PlayerInfo.SetActive(false);

        mIsLock = true;
    }

    public void SetData(Data data)
    {
        mData = data;

        CenterButton.spriteName = mSelectSpriteNames[mData.Position];

//        BGLeft.spriteName = mSelectBGSpriteNames[mData.Position];
//        BGRight.spriteName = mSelectBGSpriteNames[mData.Position];

        foreach(UISprite sprite in LeftRightMarks)
        {
            sprite.color = UICreateRole.Get.PosInfos[mData.Position].TextColor;
        }

        RemoveButton.SetActive(true);
        PlayerInfo.SetActive(true);

        PosNameLabel.text = TextConst.S(mPosTextIndices[mData.Position]);
        PosBG.spriteName = mPosBGs[mData.Position];
        NameLabel.text = mData.Name;
        LevelLabel.text = mData.Level.ToString();

        mIsLock = false;
    }

    public void SetSelected()
    {
        SelectedIcon.SetActive(true);
    }

    public void SetVisible(bool visible)
    {
        Window.SetActive(visible);
    }

    /// <summary>
    /// NGUI 會自動偵測的 callback.
    /// </summary>
    public void OnClick()
    {
        if(OnClickListener != null)
            OnClickListener(Index, mData, mIsLock);
    }

    /// <summary>
    /// 呼叫時機: X 按鈕按下.
    /// </summary>
    public void OnDeleteClick()
    {
//        Debug.Log("OnDeleteClick");

        if(OnDeleteListener != null)
            OnDeleteListener(Index, mData, mIsLock);
    }
}
