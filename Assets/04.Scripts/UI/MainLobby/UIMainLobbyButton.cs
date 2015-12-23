using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class UIMainLobbyButton : MonoBehaviour
{
    public UISprite Icon;

    private const float EffectDelayTime = 3f;
    private readonly Color mGrayColor = new Color(50/255f, 50/255f, 50/255f, 1);

    private GameObject mEffect;

    public bool IsEnable
    {
        set
        {
            var uiButton = GetComponent<UIButton>();
            if(uiButton)
                uiButton.isEnabled = value;

            if(Icon)
                Icon.color = value ? Color.white : mGrayColor;
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
