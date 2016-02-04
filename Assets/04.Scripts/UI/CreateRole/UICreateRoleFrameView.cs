using System;
using System.Collections;
using GameEnum;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// FrameView 會列出玩家擁有的全部角色.
/// </summary>
/// <remarks>
/// <list type="number">
/// <item> Call ShowXXX() or Hide() 控制 UI 的顯示邏輯. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UICreateRoleFrameView : MonoBehaviour
{
    public GameObject Window;
    public UICreateRolePlayerSlot[] Slots;

    public string LockButtonSpriteName;
    public string LockBGSpriteName;
    public GameObject FullScreenBlock;

    /// <summary>
    /// 呼叫時機: 解鎖球員 Slot.
    /// </summary>
    public event Action UnlockPlayerListener;

    /// <summary>
    /// 離開 FrameView, 進入 PositionView 的等待時間. 這個等待時間是要等 Slot 撥完離開的 Animation.
    /// </summary>
    private const float ExitAnimationTime = 0.9f;

    /// <summary>
    /// 剛顯示頁面時, Slot 延遲幾秒才進入畫面的時間.
    /// </summary>
    private readonly float[] slotDelayTimes = {0, 0.15f, 0.3f};

    [UsedImplicitly]
	private void Awake()
    {
        for(int i = 0; i < Slots.Length; i++)
        {
            Slots[i].OnClickListener += onSlotClick;
            Slots[i].OnDeleteListener += onDeletePlayer;
        }

        FullScreenBlock.SetActive(false);
    }

    public void Show()
    {
        Window.SetActive(true);
        UI3DCreateRole.Get.Hide();
        playEnterAnimations();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="selectedIndex"> 哪一個 Slot 是玩家正在使用的球員. </param>
    public void Show([NotNull] UICreateRolePlayerSlot.Data[] data, int selectedIndex)
    {
        Window.SetActive(true);
        UI3DCreateRole.Get.Hide();

        setData(data);
        setSelected(selectedIndex);
        playEnterAnimations();
    }

    private void setSelected(int selectedIndex)
    {
        for(int i = 0; i < Slots.Length; i++)
        {
            Slots[i].Selected = i == selectedIndex;
        }
    }

    private void setData(UICreateRolePlayerSlot.Data[] data)
    {
        for(int i = 0; i < Slots.Length; i++)
        {
            Slots[i].SetData(data[i]);
        }
    }

    private void playEnterAnimations()
    {
        for(int i = 0; i < Slots.Length; i++)
        {
            Slots[i].SetVisible(false);
            Slots[i].PlayEnterAnimation(slotDelayTimes[i]);
        }
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"> 哪一個 slot 被點擊. </param>
    /// <param name="data"> 該 Slot 的角色資訊. </param>
    private void onSlotClick(int index, UICreateRolePlayerSlot.Data data)
    {
//        Debug.LogFormat("onSlotClick, index:{0}, isLock:{1}, data:{2}", index, isLock, data);

        if(data.Status == UICreateRolePlayerSlot.Data.EStatus.Valid || data.Status == UICreateRolePlayerSlot.Data.EStatus.Empty)
            StartCoroutine(runNexAction(index, data));
        else if(data.Status == UICreateRolePlayerSlot.Data.EStatus.LockLv)
            UIHint.Get.ShowHint(data.Message, Color.red);
        else if(data.Status == UICreateRolePlayerSlot.Data.EStatus.LockDiamond)
        {
            UIMessage.Get.ShowMessage(TextConst.S(205), string.Format(TextConst.S(207), LimitTable.Ins.GetDiamond(EOpenID.CreateRole)),
                () =>
                {
                    if(UnlockPlayerListener != null)
                        UnlockPlayerListener();
                });
        }
        else
            throw new NotImplementedException(data.Status.ToString());
    }


    private IEnumerator runNexAction(int index, UICreateRolePlayerSlot.Data data)
    {
        UICreateRole.Get.EnableBlock(true);
        playExitAnimation(index);

        yield return new WaitForSeconds(ExitAnimationTime);

        UICreateRole.Get.EnableBlock(false);

        if(data.Status == UICreateRolePlayerSlot.Data.EStatus.Empty)
        {
            // 沒有資料, 所以進入創角流程.
            GetComponent<UICreateRole>().ShowPositionView();
            UI3DCreateRole.Get.PositionView.PlayDropAnimation();
        }
        else if(GameData.Team.Player.RoleIndex == data.RoleIndex)
        {
            // 是相同的角色, 直接進入大廳.
            UICreateRole.Get.Hide();
			if (SceneMgr.Get.CurrentScene != ESceneName.Lobby)
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            else
                LobbyStart.Get.EnterLobby();

            AudioMgr.Get.PlayMusic(EMusicType.MU_ThemeSong);
        }
        else
        {
            // 通知 Server 切換角色.
            WWWForm form = new WWWForm();
            form.AddField("RoleIndex", data.RoleIndex);
            SendHttp.Get.Command(URLConst.SelectRole, waitSelectPlayer, form);
        }
    }

    /// <summary>
    /// 播放離開 FrameView 時的 Animation.
    /// </summary>
    /// <param name="slotOpenIndex"> 哪一個 slot 被點擊. </param>
    private void playExitAnimation(int slotOpenIndex)
    {
        for(int i = 0; i < Slots.Length; i++)
        {
            if(i == slotOpenIndex)
                Slots[i].PlayOpenAnimation();
            else
                Slots[i].PlayExitAnimation();
        }
    }

    private void waitSelectPlayer(bool ok, WWW www)
    {
        Debug.LogFormat("waitSelectPlayer, ok:{0}", ok);

        UICreateRole.Get.Hide();

        if(ok)
        {
            var team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Player = team.Player;
            GameData.Team.Player.Init();

            UICreateRole.Get.Hide();
			if (SceneMgr.Get.CurrentScene != ESceneName.Lobby)
				SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            else
                LobbyStart.Get.EnterLobby();

            AudioMgr.Get.PlayMusic(EMusicType.MU_ThemeSong);
        }
    }

    public void OnBackClick()
    {
//        Debug.Log("OnBackClick");

        UICreateRole.Get.Hide();

		if(SceneMgr.Get.CurrentScene != ESceneName.Lobby)
			SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
        else
            LobbyStart.Get.EnterLobby();
    }

    private void onDeletePlayer(int index, UICreateRolePlayerSlot.Data data)
    {
//        Debug.LogFormat("onDeletePlayer, isLock:{0}", isLock);

        if(data.Status == UICreateRolePlayerSlot.Data.EStatus.Valid || data.Status == UICreateRolePlayerSlot.Data.EStatus.Empty)
            UIMessage.Get.ShowMessage(TextConst.S(205), TextConst.S(206), onConfirmDelete, null, data);
    }

    private void onConfirmDelete(object extraInfo)
    {
//        Debug.Log("onConfirmDelete");
        
        // 做刪除角色流程.
        WWWForm form = new WWWForm();
        UICreateRolePlayerSlot.Data data = (UICreateRolePlayerSlot.Data)extraInfo;
        form.AddField("RoleIndex", data.RoleIndex);

        SendHttp.Get.Command(URLConst.DeleteRole, waitDeletePlayer, form);
    }

    private void waitDeletePlayer(bool ok, WWW www)
    {
        Debug.LogFormat("waitDeletePlayer, ok:{0}", ok);

        if(ok)
        {
			TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
			GameData.Team.Player = team.Player;
            GameData.Team.Player.Init();

            var data = UICreateRoleBuilder.Build(team.PlayerBank);
            if (data != null)
                setData(data);
            else
                Debug.LogError("Data Error!");
        }
    }
}
