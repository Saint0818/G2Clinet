using System;
using System.Collections.Generic;
using GameStruct;
using UnityEngine;

public class ReceiveDailyLoginRewardProtocol
{
    /// <summary>
    /// <para> 1) bool: 是否成功. </para>
    /// <para> 2) Year </para>
    /// <para> 3) Month </para>
    /// </summary>
    private Action<bool, int, int> mCallback;

    private class Data
    {
        public int Diamond;
        public int Money;
        public int Power;
        public TItem[] Items;
        public TValueItem[] ValueItems;
        public TMaterialItem[] MaterialItems;
        public TSkill[] SkillCards;
        public int Year;
        public int Month;
        public Dictionary<int, Dictionary<int, int>> ReceivedDailyLoginNums;

        public override string ToString()
        {
            return string.Format("Diamond: {0}, Money: {1}, Power: {2}, Year: {3}, Month: {4}", Diamond, Money, Power, Year, Month);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callback"></param>
    public void Send(Action<bool, int, int> callback)
    {
        mCallback = callback;

        WWWForm form = new WWWForm();
        SendHttp.Get.Command(URLConst.ReceivedDailyLoginReward, waitReceivedDailyLoginReward, form);
    }

    private void waitReceivedDailyLoginReward(bool ok, WWW www)
    {
        Debug.LogFormat("waitReceivedDailyLoginReward, ok:{0}", ok);

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
            GameData.Team.ReceivedDailyLoginNums = data.ReceivedDailyLoginNums;

//            Debug.Log(data);
//
//            Debug.LogFormat("{0}-{1}, DailyLogin:{2}, ReceivedDailyLogin:{3}",
//                    data.Year, data.Month,
//                    GameData.Team.GetDailyLoginNum(data.Year, data.Month), 
//                    GameData.Team.GetReceivedDailyLoginNum(data.Year, data.Month));

            mCallback(true, data.Year, data.Month);
        }
        else
            mCallback(false, 0, 0);
    }
}