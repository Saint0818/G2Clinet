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
    public UICreateRolePlayerFrame[] Frames;
    public UIConfirmDialog ConfirmDialog;

    public string LockButtonSpriteName;
    public string LockBGSpriteName;

//    private UICreateRolePlayerFrame.Data mDeleteData;

    /// <summary>
    /// 預設顯示幾位球員. 超過的部分會用 lock 來顯示.
    /// </summary>
    private const int DefaultShowNum = 2;

    [UsedImplicitly]
	private void Awake()
    {
        for(int i = 0; i < Frames.Length; i++)
        {
            Frames[i].OnClickListener += onSlotClick;
            Frames[i].OnDeleteListener += onDeletePlayer;
        }

        ConfirmDialog.OnYesListener += onConfirmDelete;
//        ConfirmDialog.OnCancelClickListener += onCancelDelete;

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
        for(int i = showNum; i < Frames.Length; i++)
        {
            Frames[i].SetLock();
        }
    }

    private int mShowNum;

    public void Show()
    {
        Window.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void Show([NotNull] UICreateRolePlayerFrame.Data[] data)
    {
        Window.SetActive(true);

        for(int i = 0; i < Frames.Length; i++)
        {
            Frames[i].Clear();

            if(i >= data.Length)
                continue;

            Frames[i].SetData(data[i]);
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

    private void onSlotClick(UICreateRolePlayerFrame.Data data, bool isLock)
    {
        if(isLock)
            return;

        if(!data.IsValid())
        {
            // 沒有資料, 所以進入創角流程.
            GetComponent<UICreateRole>().ShowPositionView();
            return;
        }

        if(GameData.Team.Player.RoleIndex == data.RoleIndex)
        {
            // 是相同的角色, 直接進入大廳.
            UICreateRole.Get.Hide();;
            if (SceneMgr.Get.CurrentScene != SceneName.Lobby)
                SceneMgr.Get.ChangeLevel(SceneName.Lobby);
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
            if (SceneMgr.Get.CurrentScene != SceneName.Lobby)
                SceneMgr.Get.ChangeLevel(SceneName.Lobby);
            else
                LobbyStart.Get.EnterLobby();
        }
    }

    public void OnBackClick()
    {
//        Debug.Log("OnBackClick");

        UICreateRole.Get.Hide();

        if(SceneMgr.Get.CurrentScene != SceneName.Lobby)
            SceneMgr.Get.ChangeLevel(SceneName.Lobby);
        else
            LobbyStart.Get.EnterLobby();
    }

    private void onDeletePlayer(UICreateRolePlayerFrame.Data data, bool isLock)
    {
        Debug.LogFormat("onDeletePlayer, isLock:{0}", isLock);

        if(isLock)
            return;

//        mDeleteData = data;
        
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

//    private void onCancelDelete()
//    {
//        Debug.Log("onCancelDelete");
//        mDeleteData = new UICreateRolePlayerFrame.Data();
//    }

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
