using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIResourceView : MonoBehaviour
{
    public GameObject FullScreenBlock;

    public GameObject MoneyObj;
    public UILabel MoneyLabel;
    public GameObject DiamondObj;
    public UILabel DiamondLabel;
    public GameObject PowerObj;
    public UILabel PowerLabel; // 體力.
    public UILabel PowerCountDownLabel; // 體力倒數計時.
    public UISlider PowerSlider; 
    public GameObject SocialObj;
    public UILabel SocialLabel;
    public GameObject PVPObj;
    public UILabel PVPLabel;

    private TweenScale mMoneyTweenScale;
    private TweenScale mDiamondTweenScale;
    private TweenScale mPowerTweenScale;
    private TweenScale mPVPTweenScale;
    private TweenScale mSocialTweenScale;

    [UsedImplicitly]
    private void Awake()
    {
        mMoneyTweenScale = MoneyLabel.GetComponent<TweenScale>();
        mDiamondTweenScale = DiamondLabel.GetComponent<TweenScale>();
        mPowerTweenScale = PowerLabel.GetComponent<TweenScale>();
        mPVPTweenScale = PowerLabel.GetComponent<TweenScale>();
        mSocialTweenScale = PowerLabel.GetComponent<TweenScale>();
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
        mMoneyTweenScale.delay = delay;
        mMoneyTweenScale.PlayForward();
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
        mDiamondTweenScale.delay = delay;
        mDiamondTweenScale.PlayForward();
    }

    public int PVPCoin
    {
        set { PVPLabel.text = NumFormater.Convert(value); }
    }

    public void PlayPVPAnimation(float delay = 0)
    {
        mPVPTweenScale.delay = delay;
        mPVPTweenScale.PlayForward();
    }

    public int SocialCoin
    {
        set { SocialLabel.text = NumFormater.Convert(value); }
    }

    public void PlaySocialAnimation(float delay = 0)
    {
        mSocialTweenScale.delay = delay;
        mSocialTweenScale.PlayForward();
    }

    public int Power
    {
        set
        {
            PowerLabel.text = string.Format("{0}/{1}", value, GameConst.Max_Power);
            PowerSlider.value = (float)value / GameConst.Max_Power;
        }
    }

    public void PlayPowerAnimation(float delay = 0)
    {
        mPowerTweenScale.delay = delay;
        mPowerTweenScale.PlayForward();
    }

    public string PowerCountDown
    {
        set { PowerCountDownLabel.text = value; }
    }

    public bool PowerCountDownVisible
    {
        set { PowerCountDownLabel.gameObject.SetActive(value);}
    }

    public void Show(UIResource.EMode mode)
    {
//        DiamondObj.SetActive(kind >= 1);
//        MoneyObj.SetActive(kind >= 2);
//        PowerObj.SetActive(kind >= 3);

        DiamondObj.SetActive(true);
        MoneyObj.SetActive(true);

        PowerObj.SetActive(mode == UIResource.EMode.Power);
        PVPObj.SetActive(mode == UIResource.EMode.PVP || mode == UIResource.EMode.PvpSocial);
        SocialObj.SetActive(mode == UIResource.EMode.PvpSocial);
    }

    public void Hide()
    {
        EnableFullScreenBlock();

        MoneyObj.SetActive(false);
        DiamondObj.SetActive(false);
        PowerObj.SetActive(false);
    }

    /// <summary>
    /// Block 的目的是避免使用者點擊任何 UI 元件.(內部使用, 一般使用者不要使用)
    /// </summary>
    /// <param name="lockTime"> 單位: 秒. </param>
    public void EnableFullScreenBlock(float lockTime = 0.5f)
    {
        if (gameObject.activeSelf)
            StartCoroutine(enableFullScreenBlock(lockTime));
    }

    private IEnumerator enableFullScreenBlock(float lockTime)
    {
        FullScreenBlock.SetActive(true);
        yield return new WaitForSeconds(lockTime);
        FullScreenBlock.SetActive(false);
    }
}