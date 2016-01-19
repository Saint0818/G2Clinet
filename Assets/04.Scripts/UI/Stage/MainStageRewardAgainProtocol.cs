using Newtonsoft.Json;
using UnityEngine;

public class MainStageRewardAgainProtocol
{
    public delegate void Action(bool ok, TStageRewardAgain reward);
    private Action mCallback;

    public void Send(int stageID, Action callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        SendHttp.Get.Command(URLConst.StageRewardAgain, waitMainStageRewardAgain, form);
    }

    private void waitMainStageRewardAgain(bool ok, WWW www)
    {
//        Debug.LogFormat("waitMainStageRewardAgain, ok:{0}", ok);

        if(ok)
        {
            var reward = JsonConvert.DeserializeObject<TStageRewardAgain>(www.text);

//            Debug.LogFormat("waitMainStageRewardAgain:{0}", reward);

            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Player.Lv = reward.PlayerLv;
            GameData.Team.Player.Exp = reward.PlayerExp;
            GameData.Team.Items = reward.Items;
            GameData.Team.ValueItems = reward.ValueItems;
            GameData.Team.MaterialItems = reward.MaterialItems;
			GameData.Team.SkillCards = reward.SkillCards;
			GameData.Team.InitSkillCardCount();

            mCallback(true, reward);
        }
        else
        {
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
            mCallback(false, new TStageRewardAgain());
        }
    }
}