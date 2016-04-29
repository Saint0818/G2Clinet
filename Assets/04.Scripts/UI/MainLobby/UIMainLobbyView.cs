using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class UIMainLobbyView : MonoBehaviour
{
    public GameObject FullScreenBlock;

    public GameObject TopLeftGroup;
    public GameObject BottomGroup;

    public UILabel LevelLabel;

    public UISprite PlayerIconSprite;
    public UISprite PlayerPositionSprite;

    public UILabel NameLabel;

    public GameObject Settings;

    // 紅點.
    public GameObject EquipmentNoticeObj;
    public GameObject AvatarNoticeObj;
    public GameObject SkillNoticeObj;
    public GameObject SocialNoticeObj;
    public GameObject ShopNoticeObj;
    public GameObject MissionNoticeObj;
    public GameObject PlayerNoticeObj;
    public GameObject MallNoticeObj;
	public GameObject QueueNoticeObj;
	public GameObject AlbumNoticeObj;

    // 畫面下方的主要功能按鈕.
    public UIButton RaceButton;
    public UIUnlockButton AvatarButton;
    public UIUnlockButton EquipButton;
    public UIUnlockButton SkillButton;
    public UIUnlockButton ShopButton;
    public UIUnlockButton SocialButton;
    public UIUnlockButton MissionButton;
	public UIUnlockButton MallButton;

	public TGymQueueObj[] GymQueueObj = new TGymQueueObj[3];
	public GameObject QueueButton;
	public GameObject QueueGroup;
	public GameObject QueueLockObj;
	public UILabel QueueLockPrice;

    [UsedImplicitly]
    private void Awake()
    {
        FullScreenBlock.SetActive(false);
    }

    public int Level
    {
        set { LevelLabel.text = value.ToString(); }
    }

    public string PlayerName
    {
        set { NameLabel.text = value; }
    }

    public string PlayerIcon
    {
        set { PlayerIconSprite.spriteName = value; }
    }

    public string PlayerPosition
    {
        set { PlayerPositionSprite.spriteName = value; }
    }

    public bool IsSettingsVisible
    {
        get { return Settings.activeSelf; }
    }

    public bool EquipmentNotice
    {
        set { EquipmentNoticeObj.SetActive(value); }
    }

    public bool AvatarNotice
    {
        set { AvatarNoticeObj.SetActive(value); }
    }

    public bool SkillNotice
    {
        set { SkillNoticeObj.SetActive(value); }
    }

    public bool SocialNotice
    {
        set { 
            if (GameData.IsOpenUIEnable(GameEnum.EOpenID.Social))
                SocialNoticeObj.SetActive(value); 
            else
                SocialNoticeObj.SetActive(false); 
        }
    }

    public bool ShopNotice
    {
        set { ShopNoticeObj.SetActive(value); }
    }

    public bool MissionNotice
    {
        set { MissionNoticeObj.SetActive(value); }
    }

    public bool PlayerNotice
    {
        set { PlayerNoticeObj.SetActive(value); }
    }

    public bool MallNotice
    {
        set { MallNoticeObj.SetActive(value); }
    }

	public bool QueueNotice
	{
		set {QueueNoticeObj.SetActive(value);} 
	}

	public bool AlbumNotice 
	{
		set {AlbumNoticeObj.SetActive(value);}
	}

	public bool QueueLock
	{
		set {QueueLockObj.SetActive(value);}
	}

	public bool IsQueueOpen 
	{
		get {return QueueGroup.activeSelf;}
	}

	public string QueuePrice 
	{
		set {QueueLockPrice.text = value;}
	}

	public Color QueuePriceColor
	{
		set {QueueLockPrice.color = value;}
	}

//    public bool LoginNotice
//    {
//        set { LoginNoticeObj.SetActive(value); }
//    }

	public void SetQueueState (int index, string name, float cd = 0, string time = "", bool isEmpty = true, string emptyText = "", int buildIndex = -1) {
		if(index >= 0 && index < GymQueueObj.Length) {
			GymQueueObj[index].NameLabel.text = name;
			GymQueueObj[index].CDBar.gameObject.SetActive(!isEmpty);
			GymQueueObj[index].GoEmpty.SetActive(isEmpty);
			if(isEmpty) {
				GymQueueObj[index].EmptyLabel.text = emptyText;
			} else {
				GymQueueObj[index].CDBar.value = cd;
				GymQueueObj[index].TimeLabel.text = time;
			}
			GymQueueObj[index].ToolSprite.spriteName = GameFunction.GetBuildSpriteName(buildIndex);
		}
	}

	//閒置中
	public void SetQueueBreak (int index) {
		if(index >= 0 && index < GymQueueObj.Length) {
			GymQueueObj[index].NameLabel.text = string.Format(TextConst.S(11018), index + 1);
			GymQueueObj[index].CDBar.gameObject.SetActive(false);
			GymQueueObj[index].CDBar.value = 0;
			GymQueueObj[index].TimeLabel.text = "";
			GymQueueObj[index].GoEmpty.SetActive(true);
			GymQueueObj[index].EmptyLabel.text = TextConst.S(11002);
			GymQueueObj[index].ToolSprite.spriteName = GameFunction.GetBuildSpriteName(-1);
		}
	}

	//update 跑ＣＤ
	public void SetQueueBuildState (int index, int buildIndex) {
		if(index >=0 && index < GymQueueObj.Length) {
			GymQueueObj[index].NameLabel.text = GameFunction.GetBuildName(buildIndex);
			GymQueueObj[index].CDBar.gameObject.SetActive(true);
			GymQueueObj[index].CDBar.value = TextConst.DeadlineStringPercent(GameFunction.GetOriTime(buildIndex, GameFunction.GetBuildLv(buildIndex) - 1, GameFunction.GetBuildTime(buildIndex).ToUniversalTime()) ,GameFunction.GetBuildTime(buildIndex).ToUniversalTime());
			GymQueueObj[index].TimeLabel.text = TextConst.SecondString((int)(new System.TimeSpan(GameData.Team.GymBuild[buildIndex].Time.ToUniversalTime().Ticks - DateTime.UtcNow.Ticks).TotalSeconds));
			GymQueueObj[index].GoEmpty.SetActive(false);
			GymQueueObj[index].ToolSprite.spriteName = GameFunction.GetBuildSpriteName(buildIndex);
		}
	}

    /// <summary>
    /// Block 的目的是避免使用者點擊任何 UI 元件.(內部使用, 一般使用者不要使用)
    /// </summary>
    /// <param name="lockTime"> 單位: 秒. </param>
    public void EnableFullScreenBlock(float lockTime = 0.5f)
    {
        if(gameObject.activeSelf)
            StartCoroutine(enableFullScreenBlock(lockTime));
    }

    private IEnumerator enableFullScreenBlock(float lockTime)
    {
        FullScreenBlock.SetActive(true);
        yield return new WaitForSeconds(lockTime);
        FullScreenBlock.SetActive(false);
    }

    public void Show()
    {
        Settings.SetActive(true);
        PlayEnterAnimation();
    }

    public void Hide(bool playAnimation = true)
    {
        Settings.SetActive(false);
        if(playAnimation)
            PlayExitAnimation();
    }

    public void PlayEnterAnimation()
    {
        GetComponent<Animator>().SetTrigger("MainLobby_Up");
    }

    public void PlayExitAnimation()
    {
        GetComponent<Animator>().SetTrigger("MainLobby_Down");
        EnableFullScreenBlock();
    }
}