using System.Collections;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// FrameView 會列出玩家擁有的全部角色.
/// </summary>
/// <remarks>
/// <list type="number">
/// <item> Call Show() or Hide() 控制 UI 的顯示邏輯. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UICreateRoleFrameView : MonoBehaviour
{
    public GameObject Window;
    public UICreateRolePlayerSlot[] Slots;
    public UIConfirmDialog ConfirmDialog;

    public string LockButtonSpriteName;
    public string LockBGSpriteName;

    /// <summary>
    /// 離開 FrameView, 進入 PositionView 的等待時間. 這個等待時間是要等 Slot 撥完離開的 Animation.
    /// </summary>
    private const float ExitAnimationTime = 0.9f;

    /// <summary>
    /// 預設顯示幾位球員. 超過的部分會用 lock 來顯示.
    /// </summary>
    private const int DefaultShowNum = 2;

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

        ConfirmDialog.OnYesListener += onConfirmDelete;

        ShowNum = DefaultShowNum;
    }

    /// <summary>
    /// 最多顯示幾位球員. 超過的部分會用 lock 來顯示.
    /// </summary>
    private int ShowNum
    {
        get { return mShowNum; }

        set
        {
            if(value < 0)
                return;

            mShowNum = value;
            updateLockUI(mShowNum);
        }
    }

    private void updateLockUI(int showNum)
    {
        for(int i = showNum; i < Slots.Length; i++)
        {
            Slots[i].SetLock();
        }
    }

    private int mShowNum;

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
    /// <param name="showNum"> 最多顯示幾位球員. 超過的部分會用 lock 來顯示. </param>
    public void Show([NotNull] UICreateRolePlayerSlot.Data[] data, int selectedIndex, int showNum)
    {
        Window.SetActive(true);
        UI3DCreateRole.Get.Hide();

        setData(data, selectedIndex);
        playEnterAnimations();

        ShowNum = showNum;
    }

    private void setData(UICreateRolePlayerSlot.Data[] data, int selectedIndex)
    {
        for(int i = 0; i < Slots.Length; i++)
        {
            Slots[i].Clear();

            if(i >= data.Length)
                continue;

            Slots[i].SetData(data[i]);
            if(selectedIndex == i)
                Slots[i].SetSelected();
        }

        updateLockUI(ShowNum);
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
    /// <param name="isLock"> 該 Slot 是否是鎖住的狀態. </param>
    private void onSlotClick(int index, UICreateRolePlayerSlot.Data data, bool isLock)
    {
//        Debug.LogFormat("onSlotClick, index:{0}, isLock:{1}, data:{2}", index, isLock, data);

        if(isLock)
            return;

        StartCoroutine(runNexAction(index, data));
    }

    private IEnumerator runNexAction(int index, UICreateRolePlayerSlot.Data data)
    {
        UICreateRole.Get.EnableBlock(true);
        playExitAnimation(index);

        yield return new WaitForSeconds(ExitAnimationTime);

        UICreateRole.Get.EnableBlock(false);

        if(!data.IsValid())
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
            GameData.SaveTeam();

            UICreateRole.Get.Hide();
            if (SceneMgr.Get.CurrentScene != ESceneName.Lobby)
                SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            else
                LobbyStart.Get.EnterLobby();
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

    private void onDeletePlayer(int index, UICreateRolePlayerSlot.Data data, bool isLock)
    {
        Debug.LogFormat("onDeletePlayer, isLock:{0}", isLock);

        if(isLock)
            return;

        ConfirmDialog.Show(data);
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

            var data = UICreateRole.Convert(team.PlayerBank);
            if (data != null)
                setData(data, GameData.Team.Player.RoleIndex);
            else
                Debug.LogError("Data Error!");
        }
    }
}
