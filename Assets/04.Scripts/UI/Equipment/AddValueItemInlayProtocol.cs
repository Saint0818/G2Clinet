using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class AddValueItemInlayProtocol
{
    private CommonDelegateMethods.Bool1 mCallback;

    public void Send(int valueItemKind, int materialItemIndex, CommonDelegateMethods.Bool1 callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("ValueItemKind", valueItemKind);
        form.AddField("MaterialItemIndex", materialItemIndex);
        SendHttp.Get.Command(URLConst.AddValueItemInlay, waitAddValueItemInlay, form);
    }

    private void waitAddValueItemInlay(bool ok, WWW www)
    {
        Debug.LogFormat("AddValueItemInlay, ok:{0}", ok);

        if(ok)
        {
            TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Player = team.Player;
            GameData.Team.MaterialItems = team.MaterialItems;
            GameData.Team.Player.Init();
            
//            UIHint.Get.ShowHint(TextConst.S(531), Color.black);
        }
        else
            UIHint.Get.ShowHint(www.text, Color.red);

        mCallback(ok);
    }
}