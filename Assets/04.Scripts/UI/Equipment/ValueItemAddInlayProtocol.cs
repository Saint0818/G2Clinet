using GameStruct;
using UnityEngine;

public class ValueItemAddInlayProtocol
{
    private CommonDelegateMethods.Bool1 mCallback;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerValueItemKind"></param>
    /// <param name="storageMaterialItemIndex"> 倉庫內的索引. </param>
    /// <param name="callback"></param>
    public void Send(int playerValueItemKind, int storageMaterialItemIndex, 
                     CommonDelegateMethods.Bool1 callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("ValueItemKind", playerValueItemKind);
        form.AddField("MaterialItemIndex", storageMaterialItemIndex);
        SendHttp.Get.Command(URLConst.ValueItemAddInlay, waitAddValueItemInlay, form);
    }

    private void waitAddValueItemInlay(bool ok, WWW www)
    {
        Debug.LogFormat("ValueItemAddInlay, ok:{0}", ok);

        if(ok)
        {
            TTeam team = JsonConvertWrapper.DeserializeObject<TTeam>(www.text);
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