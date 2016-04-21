using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

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

//    public UIButton DailyLoginButton;

    // 紅點.
    public GameObject EquipmentNoticeObj;
    public GameObject AvatarNoticeObj;
    public GameObject SkillNoticeObj;
    public GameObject SocialNoticeObj;
    public GameObject ShopNoticeObj;
    public GameObject MissionNoticeObj;
    public GameObject PlayerNoticeObj;
    public GameObject MallNoticeObj;
//    public GameObject LoginNoticeObj;

    // 畫面下方的主要功能按鈕.
    public UIUnlockButton AvatarButton;
    public UIUnlockButton EquipButton;
    public UIUnlockButton SkillButton;
    public UIUnlockButton ShopButton;
    public UIUnlockButton SocialButton;
    public UIUnlockButton MissionButton;
    public UIUnlockButton MallButton;

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

//    public bool LoginNotice
//    {
//        set { LoginNoticeObj.SetActive(value); }
//    }

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
        PlayEnterAnimation();
    }

    public void Hide(bool playAnimation = true)
    {
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