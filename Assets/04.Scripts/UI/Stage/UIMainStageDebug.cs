using UnityEngine;

public class UIMainStageDebug
{
    private int mStageID;

    public void SendCommand(int stageID)
    {
        mStageID = stageID;

        var win = new MainStageWinProtocol();
        win.Send(mStageID, waitMainStageWin);
    }

    private void waitMainStageWin(bool ok, TMainStageWin reward)
    {
        Debug.LogFormat("waitMainStageWin, ok:{0}", ok);

        if(ok)
        {
//            TStageRewardStart reward = JsonConvert.DeserializeObject<TStageRewardStart>(www.text);

//            Debug.LogFormat("waitMainStageWin:{0}", reward);

//            GameData.Team.Power = reward.Power;
//            GameData.Team.Money = reward.Money;
//            GameData.Team.Diamond = reward.Diamond;
//            GameData.Team.Player = reward.Player;
//            GameData.Team.Player.Init();
//            GameData.Team.Items = reward.Items;

            var again = new MainStageRewardAgainProtocol();
            again.Send(mStageID, waitMainStageRewardAgain);
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }

    private void waitMainStageRewardAgain(bool ok, TMainStageRewardAgain reward)
    {
        Debug.LogFormat("waitMainStageRewardAgain, ok:{0}", ok);

        if(ok)
        {
//            var reward = JsonConvert.DeserializeObject<TMainStageRewardAgain>(www.text);
//            GameData.Team.Money = reward.Money;
//            GameData.Team.Diamond = reward.Diamond;
//            GameData.Team.Player.Lv = reward.PlayerLv;
//            GameData.Team.Player.Exp = reward.PlayerExp;
//            GameData.Team.Items = reward.Items;
//            GameData.Team.SkillCards = reward.SkillCards;

//            Debug.LogFormat("waitMainStageRewardAgain:{0}", reward);

            var again = new MainStageRewardAgainProtocol();
            again.Send(mStageID, waitMainStageRewardAgain2);
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }

    private void waitMainStageRewardAgain2(bool ok, TMainStageRewardAgain reward)
    {
        Debug.LogFormat("waitMainStageRewardAgain2, ok:{0}", ok);

        if (ok)
        {
//            var reward = JsonConvert.DeserializeObject<TMainStageRewardAgain>(www.text);

//            Debug.LogFormat("waitMainStageRewardAgain2:{0}", reward);

//            GameData.Team.Money = reward.Money;
//            GameData.Team.Diamond = reward.Diamond;
//            GameData.Team.Player.Lv = reward.PlayerLv;
//            GameData.Team.Player.Exp = reward.PlayerExp;
//            GameData.Team.Items = reward.Items;
//            GameData.Team.SkillCards = reward.SkillCards;
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }
}