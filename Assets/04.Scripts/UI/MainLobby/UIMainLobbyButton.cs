using System;
using System.Collections;
using System.Collections.Generic;
using GameEnum;
using GameStruct;
using UnityEngine;
using JetBrains.Annotations;

public class UIMainLobbyButton : MonoBehaviour
{
//    public int OpenIndex;

    private const float EffectDelayTime = 3f;

//    private int OpenLv
//    {
//        get
//        {
//            if(mOpenLv == Int32.MaxValue)
//            {
//                foreach (KeyValuePair<int, TExpData> pair in GameData.DExpData)
//                {
//                    if (pair.Value.OpenIndex == OpenIndex)
//                    {
//                        mOpenLv = pair.Value.Lv;
//                        break;
//                    }
//                }
//            }
//
//            return mOpenLv;
//        }
//    }

    // 球員等級大於等於此數值, 表示此按鈕必須要打開.
    private int mOpenLv = Int32.MaxValue;

    private GameObject mEffect;

    public bool IsEnable
    {
        set
        {
            var uiButton = GetComponent<UIButton>();
            if(uiButton)
                uiButton.isEnabled = value;
        }
    }

    public void PlaySFX()
    {
        StartCoroutine(playSFX(EffectDelayTime));
    }

//    [UsedImplicitly]
//	private void OnEnable()
//    {
//        GetComponent<UIButton>().isEnabled = GameData.Team.Player.Lv >= OpenLv;
//
//        PlayerPrefs.SetInt(ESave.LevelUpFlag.ToString(), 1);
//        if(PlayerPrefs.HasKey(ESave.LevelUpFlag.ToString()))
//        {
//            StartCoroutine(playSFX(EffectDelayTime));
//
//            PlayerPrefs.DeleteKey(ESave.LevelUpFlag.ToString());
//            PlayerPrefs.Save();
//        }
//	}

    private IEnumerator playSFX(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if(mEffect != null)
            Destroy(mEffect);

        mEffect = UIPrefabPath.LoadUI("Effect/UnlockEffect", transform);
    }

    [UsedImplicitly]
    private void OnDisable()
    {
        if(mEffect != null)
        {
            Destroy(mEffect);
            mEffect = null;
        }
    }
}
