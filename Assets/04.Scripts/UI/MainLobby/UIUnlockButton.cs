using System.Collections;
using GameEnum;
using UnityEngine;

/// <summary>
/// 控制按鈕的解鎖機制.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 設定 Icon, OpenID, Event 這 3 個屬性. </item>
/// </list>
[RequireComponent(typeof(UIButton))]
public class UIUnlockButton : MonoBehaviour
{
    [Tooltip("會變色的 Icon")]
    public UISprite Icon;

    [Tooltip("Limit 企劃表格的 OpenID 欄位")]
    public EOpenID OpenID;

    [Tooltip("按鈕解鎖後會被執行的事件")]
    public EventDelegate Event;

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
            if(GameData.Team.HighestLv == limit.Lv && hasLevelUpFlag())
            {
                PlaySFX();
                deleteLevelUpFlag();
            }
        }
    }

    private bool hasLevelUpFlag()
    {
        return PlayerPrefs.HasKey(ESave.LevelUpFlag.ToString());
    }

    private static void deleteLevelUpFlag()
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

            button.onClick.Clear();
            if(value)
                button.onClick.Add(Event);
            else
                button.onClick.Add(new EventDelegate(() =>
                {
                    string text = string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)OpenID)),
                        LimitTable.Ins.GetLv(OpenID));
                    UIHint.Get.ShowHint(text, Color.black);
                }));
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
