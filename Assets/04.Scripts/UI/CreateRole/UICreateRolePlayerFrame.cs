﻿using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

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
    public struct Data
    {
        public int PlayerID;
        public int RoleIndex;
        public EPlayerPostion Position;
        public string Name;
        public int Level;

        public bool IsValid()
        {
            return PlayerID > 0 && RoleIndex >= 0 && !string.IsNullOrEmpty(Name) && Level >= 1;
        }
    }

    public delegate void Action(Data bank, bool isLock);
    public event Action OnClickListener;
    public event Action OnDeleteListener; // 刪除角色的按鈕按下.

    private readonly Dictionary<EPlayerPostion, string> mPosNames = new Dictionary<EPlayerPostion, string>
    {
        { EPlayerPostion.G, "Labelguard"},
        { EPlayerPostion.F, "Labelforward"},
        { EPlayerPostion.C, "Labelcenter"}
    };
        
    [FormerlySerializedAs("PlusButton")]
    public UISprite CenterButton;
    public GameObject RemoveButton;
    public UISprite BGLeft;
    public UISprite BGRight;

    public GameObject PlayerInfo;
    public UISprite PosSprite;
    public UILabel NameLabel;
    public UILabel LevelLabel;

    private const string LockSpriteName = "Icon_lock";
    private const string LockBGSpriteName = "BtnLocked";

    private bool mIsLock;

    private const string AddSpriteName = "Icon_Create";
    private const string AddBGSpriteName = "BtnEmpty";

    private readonly Dictionary<EPlayerPostion, string> mSelectSpriteNames = new Dictionary<EPlayerPostion, string>
    {
        {EPlayerPostion.C, "BtnCircle0"},
        {EPlayerPostion.F, "BtnCircle2"},
        {EPlayerPostion.G, "BtnCircle1"},
    };
    private readonly Dictionary<EPlayerPostion, string> mSelectBGSpriteNames = new Dictionary<EPlayerPostion, string>
    {
        {EPlayerPostion.C, "BtnCreatedCenter"},
        {EPlayerPostion.F, "BtnCreatedForward"},
        {EPlayerPostion.G, "BtnCreatedGuard"},
    };

//    private TPlayerBank mPlayerBank;
    private Data mData;

    [UsedImplicitly]
	private void Awake()
    {
	    Clear();
	}

    public void Clear()
    {
        CenterButton.spriteName = AddSpriteName;

        BGLeft.spriteName = AddBGSpriteName;
        BGRight.spriteName = AddBGSpriteName;

        RemoveButton.SetActive(false);
        PlayerInfo.SetActive(false);

//        mPlayerBank = new TPlayerBank();
        mData = new Data();

        mIsLock = false;
    }

    public void SetLock()
    {
        CenterButton.spriteName = LockSpriteName;
        BGLeft.spriteName = LockBGSpriteName;
        BGRight.spriteName = LockBGSpriteName;
        RemoveButton.SetActive(false);
        PlayerInfo.SetActive(false);

        mIsLock = true;
    }

    public void SetData(Data data)
    {
        mData = data;

        CenterButton.spriteName = mSelectSpriteNames[mData.Position];

        BGLeft.spriteName = mSelectBGSpriteNames[mData.Position];
        BGRight.spriteName = mSelectBGSpriteNames[mData.Position];

        RemoveButton.SetActive(true);
        PlayerInfo.SetActive(true);

        PosSprite.spriteName = mPosNames[mData.Position];
        NameLabel.text = mData.Name;
        LevelLabel.text = mData.Level.ToString();

        mIsLock = false;

//        int bodyType = GameData.DPlayers[mPlayerBank.ID].BodyType;
//        if(bodyType < 0)
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
            OnClickListener(mData, mIsLock);
    }

    /// <summary>
    /// 呼叫時機: X 按鈕按下.
    /// </summary>
    public void OnDeleteClick()
    {
//        Debug.Log("OnDeleteClick");

        if(OnDeleteListener != null)
            OnDeleteListener(mData, mIsLock);
    }
}
