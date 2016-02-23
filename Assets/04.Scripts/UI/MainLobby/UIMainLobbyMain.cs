using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIMainLobbyMain : MonoBehaviour
{
    public GameObject FullScreenBlock;

    public GameObject MoneyObj;
    public UILabel LevelLabel;
    public UILabel MoneyLabel;
    public GameObject DiamondObj;
    public UILabel DiamondLabel;
    public GameObject PowerObj;
    public UILabel PowerLabel; // 體力.
    public UILabel PowerCountDownLabel; // 體力倒數計時.

    public UISprite PlayerIconSprite;
    public UISprite PlayerPositionSprite;

    public TweenScale MoneyTweenScale;
    public TweenScale DiamondTweenScale;
    public TweenScale PowerTweenScale;

    public GameObject Settings;
    public GameObject MainMenu;

    public UILabel NameLabel;

    public UIButton DailyLoginButton;

    // 紅點.
    public GameObject EquipmentNoticeObj;
    public GameObject AvatarNoticeObj;
    public GameObject SkillNoticeObj;
    public GameObject SocialNoticeObj;
    public GameObject ShopNoticeObj;
    public GameObject MissionNoticeObj;
    public GameObject PlayerNoticeObj;
    public GameObject MallNoticeObj;

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
        MallNotice = false;
    }

    public int Level
    {
        set { LevelLabel.text = value.ToString(); }
    }

    public int Money
    {
        set { MoneyLabel.text = NumFormater.Convert(value); }
    }

    public bool MoneyVisible
    {
        set { MoneyObj.SetActive(value);}
        get { return MoneyObj.activeSelf; }
    }

    public void PlayMoneyAnimation(float delay = 0)
    {
        MoneyTweenScale.delay = delay;
        MoneyTweenScale.PlayForward();
    }

    public int Diamond
    {
        set { DiamondLabel.text = NumFormater.Convert(value); }
    }

    public bool DiamondVisible
    {
        set { DiamondObj.SetActive(value); }
        get { return DiamondObj.activeSelf; }
    }

    public void PlayDiamondAnimation(float delay = 0)
    {
        DiamondTweenScale.delay = delay;
        DiamondTweenScale.PlayForward();
    }

    public int Power
    {
        set { PowerLabel.text = string.Format("{0}/{1}", value, GameConst.Max_Power); }
    }

    public void PlayPowerAnimation(float delay = 0)
    {
        PowerTweenScale.delay = delay;
        PowerTweenScale.PlayForward();
    }

    public string PowerCountDown
    {
        set { PowerCountDownLabel.text = value; }
    }

    public bool PowerCountDownVisible
    {
        set { PowerCountDownLabel.gameObject.SetActive(value);}
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

    /// <summary>
    /// Block 的目的是避免使用者點擊任何 UI 元件.(內部使用, 一般使用者不要使用)
    /// </summary>
    /// <param name="enable"></param>
    public void EnableBlock(bool enable)
    {
        FullScreenBlock.SetActive(enable);
    }

    public bool IsShow
    {
        get { return Settings.activeSelf; }
    }

    public void Show()
    {
        PlayEnterAnimation();
        Settings.SetActive(true);

        MoneyObj.SetActive(true);
        DiamondObj.SetActive(true);
        PowerObj.SetActive(true);
    }

    public void Hide(int kind = 3, bool playAnimation = true)
    {
        if(playAnimation)
            PlayExitAnimation();

        Settings.SetActive(false);

        DiamondObj.SetActive(kind >= 1);
        MoneyObj.SetActive(kind >= 2);
        PowerObj.SetActive(kind >= 3);
    }

    public void HideAll(bool playAnimation = true)
    {
        if(playAnimation)
            PlayExitAnimation();
        Settings.SetActive(false);

        MoneyObj.SetActive(false);
        DiamondObj.SetActive(false);
        PowerObj.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        GetComponent<Animator>().SetTrigger("MainLobby_Up");
    }

    public void PlayExitAnimation()
    {
        GetComponent<Animator>().SetTrigger("MainLobby_Down");
    }
}