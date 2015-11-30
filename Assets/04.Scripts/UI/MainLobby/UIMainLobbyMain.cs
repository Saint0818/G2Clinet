﻿using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIMainLobbyMain : MonoBehaviour
{
    /// <summary>
    /// 呼叫時機: 當玩家更改球員名稱時.
    /// </summary>
    public event CommonDelegateMethods.Action ChangePlayerNameListener;

    public GameObject FullScreenBlock;

    public GameObject MoneyObj;
    public UILabel MoneyLabel;
    public GameObject DiamondObj;
    public UILabel DiamondLabel;
    public GameObject PowerObj;
    public UILabel PowerLabel; // 體力.

    public UISprite PlayerIconSprite;
    public UIButton PlayerButton;

    public TweenScale MoneyTweenScale;
    public TweenScale DiamondTweenScale;
    public TweenScale PowerTweenScale;

    public GameObject Settings;

    public UILabel NameLabel;

    // 紅點.
    public GameObject EquipmentNoticeObj;
    public GameObject AvatarNoticeObj;
    public GameObject SkillNoticeObj;

    // 畫面下方的主要功能按鈕.
    public UIMainLobbyButton AvatarButton;
    public UIMainLobbyButton EquipButton;
    public UIMainLobbyButton SkillButton;
    public UIMainLobbyButton ShopButton;
    public UIMainLobbyButton SocialButton;

    [UsedImplicitly]
    private void Awake()
    {
        FullScreenBlock.SetActive(false);
    }

    public int Money
    {
        set { MoneyLabel.text = value.ToString(); }
    }

    public void PlayMoneyAnimation(float delay = 0)
    {
        MoneyTweenScale.delay = delay;
        MoneyTweenScale.PlayForward();
    }

    public int Diamond
    {
        set { DiamondLabel.text = value.ToString(); }
    }

    public void PlayDiamondAnimation(float delay = 0)
    {
        DiamondTweenScale.delay = delay;
        DiamondTweenScale.PlayForward();
    }

    public int Power
    {
        set { PowerLabel.text = value.ToString(); }
    }

    public void PlayPowerAnimation(float delay = 0)
    {
        PowerTweenScale.delay = delay;
        PowerTweenScale.PlayForward();
    }

    public string PlayerName
    {
        set { NameLabel.text = value; }
    }

    public string PlayerIcon
    {
        set
        {
            PlayerIconSprite.spriteName = value;
            PlayerButton.normalSprite = value;
        }
    }

    public bool EquipmentNotice
    {
        set { EquipmentNoticeObj.SetActive(value); }
    }

	public bool AvatarNotice
	{
		set{ AvatarNoticeObj.SetActive(value); }
	}

    public bool SkillNotice
    {
        set { SkillNoticeObj.SetActive(value); }
    }

    /// <summary>
    /// Block 的目的是避免使用者點擊任何 UI 元件.(內部使用, 一般使用者不要使用)
    /// </summary>
    /// <param name="enable"></param>
    public void EnableBlock(bool enable)
    {
        FullScreenBlock.SetActive(enable);
    }

    public bool IsShow { get { return Settings.activeSelf; } }

    public void Show()
    {
        PlayEnterAnimation();
        Settings.SetActive(true);

        MoneyObj.SetActive(true);
        DiamondObj.SetActive(true);
        PowerObj.SetActive(true);
    }

    public void Hide()
    {
        PlayExitAnimation();
        Settings.SetActive(false);

        MoneyObj.SetActive(true);
        DiamondObj.SetActive(true);
        PowerObj.SetActive(true);
    }

    public void HideAll()
    {
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

    public void ChangePlayerName()
    {
        if(UIInput.current.value.Length <= 0)
            return;

        if(ChangePlayerNameListener != null)
            ChangePlayerNameListener();
    }
}