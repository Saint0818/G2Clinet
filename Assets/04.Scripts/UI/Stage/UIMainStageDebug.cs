using Newtonsoft.Json;
using UnityEngine;

public class UIMainStageDebug
{
    private int mStageID;

    public void SendCommand(int stageID)
    {
        mStageID = stageID;

        mainStageWin();
    }

    private void mainStageWin()
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", mStageID);
        SendHttp.Get.Command(URLConst.MainStageWin, waitMainStageWin, form);
    }

    private void waitMainStageWin(bool ok, WWW www)
    {
        Debug.LogFormat("waitMainStageWin, ok:{0}", ok);

        if(ok)
        {
            TStageRewardStart reward = JsonConvert.DeserializeObject<TStageRewardStart>(www.text);

            Debug.LogFormat("waitMainStageWin:{0}", reward);

            GameData.Team.Power = reward.Power;
            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Player = reward.Player;
            GameData.Team.Player.Init();
            GameData.Team.Items = reward.Items;

            stageRewardAgain();
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }

    private void stageRewardAgain()
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", mStageID);
        SendHttp.Get.Command(URLConst.MainStageRewardAgain, waitMainStageRewardAgain, form);
    }

    private void waitMainStageRewardAgain(bool ok, WWW www)
    {
        Debug.LogFormat("waitMainStageRewardAgain, ok:{0}", ok);

        if(ok)
        {
            var reward = JsonConvert.DeserializeObject<TStageRewardAgain>(www.text);
            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Player.Lv = reward.PlayerLv;
            GameData.Team.Player.Exp = reward.PlayerExp;
            GameData.Team.Items = reward.Items;
            GameData.Team.SkillCards = reward.SkillCards;

            Debug.LogFormat("waitMainStageRewardAgain:{0}", reward);

            stageRewardAgain2();
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }

    private void stageRewardAgain2()
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", mStageID);
        SendHttp.Get.Command(URLConst.MainStageRewardAgain, waitMainStageRewardAgain2, form);
    }

    private void waitMainStageRewardAgain2(bool ok, WWW www)
    {
        Debug.LogFormat("waitMainStageRewardAgain2, ok:{0}", ok);

        if (ok)
        {
            var reward = JsonConvert.DeserializeObject<TStageRewardAgain>(www.text);

            Debug.LogFormat("waitMainStageRewardAgain2:{0}", reward);

            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Player.Lv = reward.PlayerLv;
            GameData.Team.Player.Exp = reward.PlayerExp;
            GameData.Team.Items = reward.Items;
            GameData.Team.SkillCards = reward.SkillCards;
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }
}