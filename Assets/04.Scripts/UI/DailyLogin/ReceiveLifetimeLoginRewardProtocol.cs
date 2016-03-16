using System;
using GameStruct;
using UnityEngine;

/// <summary>
/// 領取終生登入獎勵.
/// </summary>
public class ReceiveLifetimeLoginRewardProtocol
{
    /// <summary>
    /// <para> 1) bool ok: 是否成功. </para>
    /// <para> 2) int LoginNum: 領取哪一次的終生登入獎勵. </para>
    /// </summary>
    private Action<bool, int> mCallback;

    private class Data
    {
        public int Diamond;
        public int Money;
        public int Power;
        public TItem[] Items;
        public TValueItem[] ValueItems;
        public TMaterialItem[] MaterialItems;
        public TSkill[] SkillCards;
        public TTeamRecord LifetimeRecord;

        public override string ToString()
        {
            return string.Format("Diamond: {0}, Money: {1}, Power: {2}", Diamond, Money, Power);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loginNum"> 領取哪一個終生獎勵. (這是對應表格的數值)</param>
    /// <param name="callback"></param>
    public void Send(int loginNum, Action<bool, int> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        form.AddField("LoginNum", loginNum);
        SendHttp.Get.Command(URLConst.ReceivedLifetimeLoginReward, waitReceivedLifetimeLoginReward, form);
    }

    private void waitReceivedLifetimeLoginReward(bool ok, WWW www)
    {
        Debug.LogFormat("waitReceivedLifetimeLoginReward, ok:{0}", ok);

        if(ok)
        {
            var data = JsonConvertWrapper.DeserializeObject<Data>(www.text);
            GameData.Team.Diamond = data.Diamond;
            GameData.Team.Money = data.Money;
            GameData.Team.Power = data.Power;
            GameData.Team.Items = data.Items;
            GameData.Team.ValueItems = data.ValueItems;
            GameData.Team.MaterialItems = data.MaterialItems;
            GameData.Team.SkillCards = data.SkillCards;
            GameData.Team.LifetimeRecord = data.LifetimeRecord;

//            Debug.Log(data);
//            Debug.LogFormat("Login:{0}, ReceivedLogin:{1}",
//                    GameData.Team.LifetimeRecord.LoginNum, 
//                    GameData.Team.LifetimeRecord.ReceivedLoginNum);

            mCallback(true, GameData.Team.LifetimeRecord.ReceivedLoginNum);
        }
        else
            mCallback(false, 0);
    }
}