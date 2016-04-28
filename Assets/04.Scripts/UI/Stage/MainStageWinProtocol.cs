using System;
using UnityEngine;

public class MainStageWinProtocol
{
    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// <para>[TStageReward]: 關卡獲得的獎勵. </para>
    /// </summary>
    private Action<bool, TStageReward> mCallback;

    private int mStageID;

    public void Send(int stageID, Action<bool, TStageReward> callback)
    {
        mCallback = callback;
        mStageID = stageID;

        WWWForm form = new WWWForm();
        form.AddField("StageID", mStageID);
        SendHttp.Get.Command(URLConst.StageWin, waitMainStageWin, form);
    }

    private void waitMainStageWin(bool ok, WWW www)
    {
//        Debug.LogFormat("waitMainStageWin, ok:{0}", ok);

        if(ok)
        {
            TStageReward reward = JsonConvertWrapper.DeserializeObject<TStageReward>(www.text);

//            Debug.LogFormat("waitMainStageWin:{0}", reward);

            TStageData stageData = StageTable.Ins.GetByID(mStageID);
            if(stageData.IsValid() && stageData.HasRandomRewards()) 
                Statistic.Ins.LogEvent(59, mStageID.ToString(), GameData.Team.Diamond - reward.Diamond);

            GameData.Team.Power = reward.Power;
            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Player = reward.Player;
            GameData.Team.PlayerInit();
            GameData.Team.Items = reward.Items;
            GameData.Team.ValueItems = reward.ValueItems;
            GameData.Team.MaterialItems = reward.MaterialItems;

            mCallback(true, reward);
        }
        else
        {
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
            mCallback(false, new TStageReward());
        }
    }
}