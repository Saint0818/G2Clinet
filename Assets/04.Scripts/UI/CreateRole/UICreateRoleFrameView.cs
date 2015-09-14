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

//    public string[] PosSpriteNames;

    public string LockButtonSpriteName;
    public string LockBGSpriteName;

//    private TPlayerBank mDeletePlayerBank;
    private UICreateRolePlayerFrame.Data mDeleteData;

    private const int DefaultShowNum = 2;

    [UsedImplicitly]
	private void Awake()
    {
        for(int i = 0; i < Frames.Length; i++)
        {
//            Frames[i].PosSpriteNames = PosSpriteNames;
//            Frames[i].LockButtonSpriteName = LockButtonSpriteName;
//            Frames[i].LockBGSpriteName = LockBGSpriteName;
            Frames[i].OnClickListener += onSlotClick;
            Frames[i].OnDeleteListener += onDeleteClick;
        }

        ConfirmDialog.OnYesListener += onConfirmDelete;
        ConfirmDialog.OnCancelClickListener += onCancelDelete;

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
//    public void Show([NotNull] TPlayerBank[] playerBanks)
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
//    public void Show([NotNull] TPlayerBank[] playerBanks, int showNum)
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

//        if(data.IsValid)
//        {
            // 切換角色.
//        }
//        else
            GetComponent<UICreateRole>().ShowPositionView();
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

    private void onDeleteClick(UICreateRolePlayerFrame.Data data, bool isLock)
    {
//        Debug.Log("onDeleteClick");

//        mDeletePlayerBank = data;
        mDeleteData = data;
        
        ConfirmDialog.Show();
    }

    private void onConfirmDelete()
    {
        //        Debug.Log("onConfirmDelete");
        // 刪除角色.

//        if(mDeletePlayerBank.IsValid)
//        {
            WWWForm form = new WWWForm();
//            form.AddField("RoleIndex", mDeletePlayerBank.RoleIndex);
            form.AddField("RoleIndex", mDeleteData.RoleIndex);

            SendHttp.Get.Command(URLConst.DeleteRole, waitDeleteRole, form, true);
//        }
//        else
//            Debug.LogError("Flow is error....");
    }

    private void onCancelDelete()
    {
        Debug.Log("onCancelDelete");
//        mDeletePlayerBank = new TPlayerBank();
        mDeleteData = new UICreateRolePlayerFrame.Data();
    }

    private void waitDeleteRole(bool ok, WWW www)
    {
        Debug.LogFormat("waitDeleteRole, ok:{0}", ok);

        if(ok)
        {
			TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
			GameData.Team.Player = team.Player;
//			Show(team.PlayerBank);
        }
    }

//    private void waitLookPlayerBank(bool ok, WWW www)
//    {
//        if(ok)
//        {
//            TPlayerBank[] playerBank = JsonConvert.DeserializeObject<TPlayerBank[]>(www.text);
//            Show(playerBank);
//        }
//        else
//            Debug.LogErrorFormat("Protocol:{0}", URLConst.LookPlayerBank);
//    }
}
