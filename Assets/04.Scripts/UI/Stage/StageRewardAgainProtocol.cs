using System;
using GameStruct;
using UnityEngine;

public class StageRewardAgainProtocol
{
    private Action<bool, TStageRewardAgain> mCallback;

    private int mStageID;

    public void Send(int stageID, Action<bool, TStageRewardAgain> callback)
    {
        mCallback = callback;
        mStageID = stageID;

        WWWForm form = new WWWForm();
        form.AddField("StageID", mStageID);
        SendHttp.Get.Command(URLConst.StageRewardAgain, waitMainStageRewardAgain, form);
    }

    private void waitMainStageRewardAgain(bool ok, WWW www)
    {
//        Debug.LogFormat("waitMainStageRewardAgain, ok:{0}", ok);

        if(ok)
        {
            var reward = JsonConvertWrapper.DeserializeObject<TStageRewardAgain>(www.text);

//            Debug.LogFormat("waitMainStageRewardAgain:{0}", reward);

            trySendStatisticEvents(reward);

            GameData.Team.Power = reward.Power;
            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Player.Lv = reward.PlayerLv;
            GameData.Team.Player.Exp = reward.PlayerExp;
            GameData.Team.Items = reward.Items;
            GameData.Team.ValueItems = reward.ValueItems;
            GameData.Team.MaterialItems = reward.MaterialItems;
			GameData.Team.SkillCards = reward.SkillCards;
			GameData.Team.GotItemCount = reward.GotItemCount;
			GameData.Team.InitSkillCardCount();

            mCallback(true, reward);
        }
        else
        {
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
            mCallback(false, new TStageRewardAgain());
        }
    }

    private void trySendStatisticEvents(TStageRewardAgain reward)
    {
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