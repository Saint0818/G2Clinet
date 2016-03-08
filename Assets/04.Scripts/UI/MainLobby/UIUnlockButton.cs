﻿using System.Collections;
using GameEnum;
using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class UIUnlockButton : MonoBehaviour
{
    public UISprite Icon;

    [Tooltip("Exp 企劃表格的 OpenIndex 欄位")]
    public EOpenID OpenID;

    private const float EffectDelayTime = 3f;
    private readonly Color mGrayColor = new Color(50/255f, 50/255f, 50/255f, 1);

    private GameObject mEffect;

    private void OnEnable()
    {
        CheckEnable();
    }

    public void CheckEnable()
    {
//        PlayerPrefs.SetInt(ESave.LevelUpFlag.ToString(), 0);

        TLimitData limit = LimitTable.Ins.GetByOpenID(OpenID);
        if(limit != null)
        {
            IsVisible = GameData.Team.HighestLv >= limit.VisibleLv;
            IsEnable = GameData.Team.HighestLv >= limit.Lv;
            if(GameData.Team.HighestLv == limit.Lv && hasFlag())
            {
                PlaySFX();
                deleteFlag();
            }
        }
    }

    private bool hasFlag()
    {
        return PlayerPrefs.HasKey(ESave.LevelUpFlag.ToString());
    }

    private static void deleteFlag()
    {
        if(PlayerPrefs.HasKey(ESave.LevelUpFlag.ToString()))
        {
            PlayerPrefs.DeleteKey(ESave.LevelUpFlag.ToString());
            PlayerPrefs.Save();
        }
    }

    public bool IsEnable
    {
        get
        {
            TLimitData limit = LimitTable.Ins.GetByOpenID(OpenID);
            if(limit == null)
                return true;

            return GameData.Team.HighestLv >= limit.Lv;
        }
        set
        {
            var button = GetComponent<UIButton>();
            Color color = value ? Color.white : mGrayColor;
            button.defaultColor = color;
            button.hover = color;
            button.pressed = color;
            Icon.color = color;
        }
    }

    public bool IsVisible
    {
        set { gameObject.SetActive(value); }
    }

    public void PlaySFX()
    {
        StartCoroutine(playSFX(EffectDelayTime));
    }

    private IEnumerator playSFX(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if(mEffect != null)
            Destroy(mEffect);

        mEffect = UIPrefabPath.LoadUI("Effect/UnlockEffect", transform);
    }

    private void OnDisable()
    {
        if(mEffect != null)
        {
            Destroy(mEffect);
            mEffect = null;
        }
    }
}
