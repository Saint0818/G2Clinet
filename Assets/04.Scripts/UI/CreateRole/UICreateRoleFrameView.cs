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
    public UICreateRolePlayerFrame[] Slots;
    public UIConfirmDialog ConfirmDialog;

    public string LockButtonSpriteName;
    public string LockBGSpriteName;

    /// <summary>
    /// Slot 點擊時, 撥動作的等待時間.(要等動作撥完, 才進入下一個階段)
    /// </summary>
    private const float SlotAnimationTime = 0.9f;

    /// <summary>
    /// 預設顯示幾位球員. 超過的部分會用 lock 來顯示.
    /// </summary>
    private const int DefaultShowNum = 2;

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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void Show([NotNull] UICreateRolePlayerFrame.Data[] data)
    {
        Window.SetActive(true);
        UI3DCreateRole.Get.Hide();

        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].Clear();

            if(i >= data.Length)
                continue;

            Slots[i].SetData(data[i]);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="showNum"> 最多顯示幾位球員. 超過的部分會用 lock 來顯示. </param>
    public void Show([NotNull] UICreateRolePlayerFrame.Data[] data, int showNum)
    {
        Show(data);

        ShowNum = showNum;
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
    private void onSlotClick(int index, UICreateRolePlayerFrame.Data data, bool isLock)
    {
        if(isLock)
            return;

        StartCoroutine(runNexAction(index, data));
    }

    private IEnumerator runNexAction(int index, UICreateRolePlayerFrame.Data data)
    {
        playAnimation(index);

        yield return new WaitForSeconds(SlotAnimationTime);

        if(!data.IsValid())
        {
            // 沒有資料, 所以進入創角流程.
            GetComponent<UICreateRole>().ShowPositionView();
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
            SendHttp.Get.Command(URLConst.SelectRole, waitSelectPlayer, form, true);
        }
    }

    private void playAnimation(int slotIndex)
    {
        for(int i = 0; i < Slots.Length; i++)
        {
            if(i == slotIndex)
                Slots[i].GetComponent<Animator>().SetTrigger("Open");
            else
                Slots[i].GetComponent<Animator>().SetTrigger("Down");
        }
    }

    private void waitSelectPlayer(bool ok, WWW www)
    {
        Debug.LogFormat("waitSelectPlayer, ok:{0}", ok);

        if (ok)
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

    private void onDeletePlayer(int index, UICreateRolePlayerFrame.Data data, bool isLock)
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
        UICreateRolePlayerFrame.Data data = (UICreateRolePlayerFrame.Data)extraInfo;
        form.AddField("RoleIndex", data.RoleIndex);

        SendHttp.Get.Command(URLConst.DeleteRole, waitDeletePlayer, form, true);
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
                Show(data);
            else
                Debug.LogError("Data Error!");
        }
    }
}
