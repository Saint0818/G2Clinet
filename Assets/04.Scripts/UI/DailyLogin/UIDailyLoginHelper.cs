﻿using System;

public static class UIDailyLoginHelper
{
    public static bool HasTodayDailyLoginReward()
    {
        return HasDailyLoginReward(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
    }

    public static bool HasDailyLoginReward(int year, int month)
    {
        int currentLoginNum = GameData.Team.GetDailyLoginNum(year, month);
        int receivedLoginNum = GameData.Team.GetReceivedDailyLoginNum(year, month);

        TDailyData data = DailyTable.Ins.GetByDate(year, month);
        // +1 是已經收獎勵的下一天, 如果下一天沒獎勵, 就表示沒有每日獎勵了.
        return data != null && currentLoginNum > receivedLoginNum && data.HasRewardByDay(receivedLoginNum + 1);
    }

    public static bool HasLifetimeLoginReward()
    {
        var currentLoginNum = GameData.Team.LifetimeRecord.LoginNum;
        var receiveLoginNum = GameData.Team.LifetimeRecord.ReceivedLoginNum;

        var currentIndex = LifetimeTable.Ins.FindIndex(currentLoginNum);
        // +1 是下一個未領取的獎勵.
        var receiveIndex = LifetimeTable.Ins.FindIndex(receiveLoginNum) + 1;

        return currentIndex >= receiveIndex;
    }
}
