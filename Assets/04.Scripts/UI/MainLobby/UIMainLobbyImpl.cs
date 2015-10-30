using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

[DisallowMultipleComponent]
public class UIMainLobbyImpl : MonoBehaviour
{
    /// <summary>
    /// 呼叫時機: 當玩家更改球員名稱時.
    /// </summary>
    public event CommonDelegateMethods.Action ChangePlayerNameListener;

    public GameObject FullScreenBlock;
    public UILabel MoneyLabel;
    public UILabel DiamondLabel;
    public UILabel PowerLabel; // 體力.
    public UIInput NameInput;
    public UISprite PlayerIconSprite;
    public GameObject Settings;
//	public Animator UIAnimator;
//	private bool isEnable;

    [UsedImplicitly]
    private void Awake()
    {
        FullScreenBlock.SetActive(false);
    }

    public int Money
    {
        set { MoneyLabel.text = value.ToString(); }
    }

    public int Diamond
    {
        set { DiamondLabel.text = value.ToString(); }
    }

    public int Power
    {
        set { PowerLabel.text = value.ToString(); }
    }

    public string PlayerName
    {
        set { NameInput.value = value; }
    }

    public string PlayerIcon
    {
        set { PlayerIconSprite.spriteName = value; }
    }

    /// <summary>
    /// Block 的目的是避免使用者點擊任何 UI 元件.(內部使用, 一般使用者不要使用)
    /// </summary>
    /// <param name="enable"></param>
    public void EnableBlock(bool enable)
    {
        FullScreenBlock.SetActive(enable);
    }

    public void Show()
    {
        PlayEnterAnimation();
        Settings.SetActive(true);
    }

    public void Hide()
    {
        PlayExitAnimation();
        Settings.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        GetComponent<Animator>().SetTrigger("MainLobby_Up");
    }

    public void PlayExitAnimation()
    {
        GetComponent<Animator>().SetTrigger("MainLobby_Down");
    }

    public void ShowCreateRole()
    {
        WWWForm form = new WWWForm();
        SendHttp.Get.Command(URLConst.LookPlayerBank, waitLookPlayerBank, form);
    }

    private void waitLookPlayerBank(bool isSuccess, WWW www)
    {
        if (!isSuccess)
        {
            Debug.LogErrorFormat("Protocol:{0}, request data fail.", URLConst.LookPlayerBank);
            return;
        }

        TLookUpData lookUpData = JsonConvert.DeserializeObject<TLookUpData>(www.text);
        var data = UICreateRole.Convert(lookUpData.PlayerBanks);
        if (data != null)
        {
            UICreateRole.Get.ShowFrameView(data, lookUpData.SelectedRoleIndex, GameData.Team.PlayerNum);
            UIMainLobby.Get.Hide();
        }
        else
            Debug.LogError("Data Error!");
    }

    public void ShowAvatarFitted()
    {
        UIAvatarFitted.UIShow(true);
        UIMainLobby.Get.Hide();
    }

    public void ShowStage()
    {
        UIGameLobby.Get.Show();
        UIMainLobby.Get.Hide();
    }

    public void ShowSkillFormation()
    {
        UISkillFormation.UIShow(true);
        UIMainLobby.Get.Hide();
    }

    public void ChangePlayerName()
    {
        if(UIInput.current.value.Length <= 0)
            return;

        if(ChangePlayerNameListener != null)
            ChangePlayerNameListener();
    }
}