using System;
using GameStruct;
using UnityEngine;

public class StageWinProtocol
{
    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// <para>[TStageReward]: 關卡獲得的獎勵. </para>
    /// </summary>
    private Action<bool, TStageReward> mCallback;

    private int mStageID;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stageID"></param>
    /// <param name="points"> 玩家得分. </param>
    /// <param name="lostPoints"> 玩家失分. </param>
    /// <param name="callback"></param>
    public void Send(int stageID, int points, int lostPoints, Action<bool, TStageReward> callback)
    {
        mCallback = callback;
        mStageID = stageID;

        WWWForm form = new WWWForm();
        form.AddField("StageID", mStageID);
        form.AddField("Points", points);
        form.AddField("LostPoints", lostPoints);
        SendHttp.Get.Command(URLConst.StageWin, waitMainStageWin, form);
    }

    private void waitMainStageWin(bool ok, WWW www)
    {
//        Debug.LogFormat("waitMainStageWin, ok:{0}", ok);

        if(ok)
        {
            TStageReward reward = JsonConvertWrapper.DeserializeObject<TStageReward>(www.text);

//            Debug.LogFormat("waitMainStageWin:{0}", reward);

            trySendStatisticEvents(reward);

            GameData.Team.Power = reward.Power;
            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Player = reward.Player;
            GameData.Team.PlayerInit();
            GameData.Team.Items = reward.Items;
            GameData.Team.ValueItems = reward.ValueItems;
            GameData.Team.MaterialItems = reward.MaterialItems;

            if (reward.GotAvatar != null)
                GameData.Team.GotAvatar = reward.GotAvatar;
            
            mCallback(true, reward);
        }
        else
        {
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
            mCallback(false, new TStageReward());
        }
    }

    private void trySendStatisticEvents(TStageReward reward)
    {
        TStageData stageData = StageTable.Ins.GetByID(mStageID);
        if(stageData.IsValid() && stageData.HasRandomRewards())
            Statistic.Ins.LogEvent(59, mStageID.ToString(), 0);

        if(GameData.DItemData.ContainsKey(reward.RandomItemID))
        {
            TItemData item = GameData.DItemData[reward.RandomItemID];
            if(item.Kind == 31)
                Statistic.Ins.LogEvent(60, mStageID.ToString(), item.Value);

            if(item.Kind == 32)
                Statistic.Ins.LogEvent(61, mStageID.ToString(), item.Value);
        }
    }
}