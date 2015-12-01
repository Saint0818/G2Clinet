using System;
using System.Collections;
using UnityEngine;
using JetBrains.Annotations;

public class UIMainLobbyButton : MonoBehaviour
{
    private const float EffectDelayTime = 3f;

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
