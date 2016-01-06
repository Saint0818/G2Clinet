using Newtonsoft.Json;
using UnityEngine;

public class MainStageWinProtocol
{
    public delegate void Action(bool ok, TMainStageWin reward);
    private Action mCallback;

    public void Send(int stageID, Action callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        SendHttp.Get.Command(URLConst.MainStageWin, waitMainStageWin, form);
    }

    private void waitMainStageWin(bool ok, WWW www)
    {
        Debug.LogFormat("waitMainStageWin, ok:{0}", ok);

        if(ok)
        {
            TMainStageWin reward = JsonConvert.DeserializeObject<TMainStageWin>(www.text);

            Debug.LogFormat("waitMainStageWin:{0}", reward);

            GameData.Team.Power = reward.Power;
            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Player = reward.Player;
            GameData.Team.Player.Init();
            GameData.Team.Items = reward.Items;
            GameData.Team.ValueItems = reward.ValueItems;
            GameData.Team.MaterialItems = reward.MaterialItems;

            mCallback(true, reward);
        }
        else
        {
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
            mCallback(false, new TMainStageWin());
        }
    }
}