using System;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class DeleteRoleProtocol
{
    public class Data
    {
        public TPlayer Player;
        public TPlayerBank[] PlayerBank;
        public TSkill[] SkillCards;
    }

    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// </summary>
    private Action<bool> mCallback;

    public void Send(int roleIndex, Action<bool> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("RoleIndex", roleIndex);

        SendHttp.Get.Command(URLConst.DeleteRole, waitDeleteRole, form);
    }

    private void waitDeleteRole(bool ok, WWW www)
    {
        Debug.LogFormat("waitDeleteRole, ok:{0}", ok);

        if(ok)
        {
            var data = JsonConvert.DeserializeObject<Data>(www.text);
            GameData.Team.Player = data.Player;
            GameData.Team.Player.Init();
            GameData.Team.PlayerBank = data.PlayerBank;
            GameData.Team.SkillCards = data.SkillCards;
        }

        mCallback(ok);
    }
}