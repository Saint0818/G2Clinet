using System;
using UnityEngine;

/// <summary>
/// 領取關卡星等獎勵.
/// </summary>
public class StageStarRewardProtocol
{
    /// <summary>
    /// <para>[bool]: true 為 server command 成功.</para>
    /// <para>[TStageReward]: 關卡星等獲得的獎勵. </para>
    /// </summary>
    private Action<bool, TStageStarReward> mCallback;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="index"></param>
    /// <param name="callback"></param>
    public void Send(int chapter, int index, Action<bool, TStageStarReward> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("Chapter", chapter);
        form.AddField("Index", index);
        SendHttp.Get.Command(URLConst.StageStarReward, waitStageStarReward, form);
    }

    private void waitStageStarReward(bool ok, WWW www)
    {
        Debug.LogFormat("waitStageStarReward, ok:{0}", ok);

        if(ok)
        {
            var reward = JsonConvertWrapper.DeserializeObject<TStageStarReward>(www.text);

            Debug.Log(reward);

            GameData.Team.Power = reward.Power;
            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Player.MainStageStarReceived = reward.MainStageStarReceived;
            GameData.Team.Items = reward.Items;
            GameData.Team.ValueItems = reward.ValueItems;
            GameData.Team.MaterialItems = reward.MaterialItems;

            if(reward.GotAvatar != null)
                GameData.Team.GotAvatar = reward.GotAvatar;
            
            mCallback(true, reward);
        }
        else
        {
            UIHint.Get.ShowHint("Stage Star Reward fail!", Color.red);
            mCallback(false, new TStageStarReward());
        }
    }
}