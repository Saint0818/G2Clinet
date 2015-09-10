using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// FrameView 會列出玩家擁有的全部角色.
/// </summary>
[DisallowMultipleComponent]
public class UICreateRoleFrameView : MonoBehaviour
{
    public GameObject Window;
    public UICreateRolePlayerFrame[] Frames;
    public UIConfirmDialog ConfirmDialog;

    public string[] PosSpriteNames;

    private TPlayerBank mDeletePlayerBank;

    [UsedImplicitly]
	private void Awake()
    {
        for(int i = 0; i < Frames.Length; i++)
        {
            Frames[i].PosSpriteNames = PosSpriteNames;
            Frames[i].OnClickListener += onSlotClick;
            Frames[i].OnDeleteListener += onDeleteClick;
        }

        ConfirmDialog.OnYesListener += onConfirmDelete;
        ConfirmDialog.OnCancelClickListener += onCancelDelete;
    }

    public void Show()
    {
        Window.SetActive(true);
    }

    public void Show([NotNull] TPlayerBank[] playerBanks)
    {
        Window.SetActive(true);

        for(int i = 0; i < Frames.Length; i++)
        {
            Frames[i].Clear();

            if(i >= playerBanks.Length)
                continue;

            Frames[i].SetData(playerBanks[i]);
        }
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    private void onSlotClick(TPlayerBank playerBank)
    {
        if(playerBank.IsValid)
        {
            // 切換角色.
        }
        else
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

    private void onDeleteClick(TPlayerBank playerBank)
    {
//        Debug.Log("onDeleteClick");

        mDeletePlayerBank = playerBank;
        
        ConfirmDialog.Show();
    }

    private void onConfirmDelete()
    {
        //        Debug.Log("onConfirmDelete");
        // 刪除角色.

        if(mDeletePlayerBank.IsValid)
        {
            WWWForm form = new WWWForm();
            form.AddField("RoleIndex", mDeletePlayerBank.RoleIndex);

            SendHttp.Get.Command(URLConst.DeleteRole, waitDeleteRole, form, true);
        }
        else
            Debug.LogError("Flow is error....");
    }

    private void onCancelDelete()
    {
        Debug.Log("onCancelDelete");
        mDeletePlayerBank = new TPlayerBank();
    }

    private void waitDeleteRole(bool ok, WWW www)
    {
        Debug.LogFormat("waitDeleteRole, ok:{0}", ok);

        if(ok)
        {
			TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
			GameData.Team.Player = team.Player;
			Show(team.PlayerBank);
        }
    }

    private void waitLookPlayerBank(bool ok, WWW www)
    {
        if(ok)
        {
            TPlayerBank[] playerBank = JsonConvert.DeserializeObject<TPlayerBank[]>(www.text);
            Show(playerBank);
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.LookPlayerBank);
    }
}
