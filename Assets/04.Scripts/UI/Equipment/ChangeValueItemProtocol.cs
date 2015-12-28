using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class ChangeValueItemProtocol
{
    private CommonDelegateMethods.Action mCallback;

    public void Send(int[] changeData, CommonDelegateMethods.Action callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("ValueItems", JsonConvert.SerializeObject(changeData));
        SendHttp.Get.Command(URLConst.ChangeValueItems, waitChangeValueItems, form);
    }

    private void waitChangeValueItems(bool ok, WWW www)
    {
        if(ok)
        {
            TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Player = team.Player;
            GameData.Team.Items = team.Items;
            GameData.Team.Player.Init();
            
            UIHint.Get.ShowHint(TextConst.S(531), Color.black);
        }
        else
            UIHint.Get.ShowHint(TextConst.S(534), Color.red);

        mCallback();
    }
}