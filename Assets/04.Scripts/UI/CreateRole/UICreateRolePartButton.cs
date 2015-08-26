using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRolePartButton : MonoBehaviour
{
    public UISprite Slider;
    public UILabel Label;

    public const float ShowTime = 0.15f; // 單位: 秒.
    private float mElapsedTime;
    private bool mIsPlaying;

    [UsedImplicitly]
    private void Awake()
    {
    }

    [UsedImplicitly]
    private void Update()
    {
        if(!mIsPlaying)
            return;

        if(mElapsedTime >= ShowTime)
        {
            mIsPlaying = false;
            Slider.fillAmount = 1;
            return;
        }

        mElapsedTime += Time.deltaTime;
        Slider.fillAmount = mElapsedTime / ShowTime;

//        Debug.LogFormat("ElapsedTime:{0}, FillAmount:{1}", mElapsedTime, Slider.fillAmount);
    }

    public void Play()
    {
        Slider.gameObject.SetActive(true);
        Slider.fillAmount = 0;

        mElapsedTime = 0;
        mIsPlaying = true;
    }

    public void Hide()
    {
        Slider.gameObject.SetActive(false);

        mIsPlaying = false;
    }

    public string Name
    {
        set { Label.text = value; }
    }
}