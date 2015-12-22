using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class UIMainStageDebug
{
    private int mStageID;

    public void SendCommand(int stageID)
    {
        mStageID = stageID;

        WWWForm form = new WWWForm();
        form.AddField("StageID", mStageID);
        SendHttp.Get.Command(URLConst.PVEEnd, waitPVEEnd, form);
    }

    private void waitPVEEnd(bool ok, WWW www)
    {
        Debug.LogFormat("waitPVEEnd, ok:{0}", ok);

        if(ok)
        {
            TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Player = team.Player;
            GameData.Team.Player.Init();

            stageRewardStart();
        }
        else
            UIHint.Get.ShowHint("PVE End fail!", Color.red);
    }

    private void stageRewardStart()
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", mStageID);
        SendHttp.Get.Command(URLConst.StageRewardStart, waitStageRewardStart, form);
    }

    private void waitStageRewardStart(bool ok, WWW www)
    {
        Debug.LogFormat("waitStageRewardStart, ok:{0}", ok);

        if(ok)
        {
            TStageRewardStart reward = JsonConvert.DeserializeObject<TStageRewardStart>(www.text);

            Debug.LogFormat("waitStageRewardStart:{0}", reward);

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
        SendHttp.Get.Command(URLConst.StageRewardAgain, waitStageRewardAgain, form);
    }

    private void waitStageRewardAgain(bool ok, WWW www)
    {
        Debug.LogFormat("waitStageRewardAgain, ok:{0}", ok);

        if(ok)
        {
            var reward = JsonConvert.DeserializeObject<TStageRewardAgain>(www.text);
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Items = reward.Items;

            Debug.LogFormat("waitStageRewardAgain:{0}", reward);

            stageRewardAgain2();
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }

    private void stageRewardAgain2()
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", mStageID);
        SendHttp.Get.Command(URLConst.StageRewardAgain, waitStageRewardAgain2, form);
    }

    private void waitStageRewardAgain2(bool ok, WWW www)
    {
        Debug.LogFormat("waitStageRewardAgain2, ok:{0}", ok);

        if (ok)
        {
            var reward = JsonConvert.DeserializeObject<TStageRewardAgain>(www.text);

            Debug.LogFormat("waitStageRewardAgain2:{0}", reward);

            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Items = reward.Items;
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }
}