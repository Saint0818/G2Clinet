using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDailyLoginMain : MonoBehaviour
{
    public Action OnCloseClickListener;

    /// <summary>
    /// [0]:第 1 天, [1]: 第 2 天 ... [6]:第 7 天.
    /// </summary>
    public Transform[] DailyParents;

    public UIButton CloseButton;

    /// <summary>
    /// key: 1 是第 1 週, 2 是第 2 週, 已此類推.
    /// </summary>
    private readonly Dictionary<int, List<UIDailyLoginReward>> mDailyRewards = new Dictionary<int, List<UIDailyLoginReward>>();

    private void Awake()
    {
        CloseButton.onClick.Add(new EventDelegate(() =>
        {
            if(OnCloseClickListener != null)
                OnCloseClickListener();
        }));
    }

    public void ShowWeek(int week)
    {
        foreach(var pair in mDailyRewards)
        {
            foreach(UIDailyLoginReward reward in pair.Value)
            {
                reward.gameObject.SetActive(false);
            }
        }

        if(!mDailyRewards.ContainsKey(week))
            return;

        foreach(UIDailyLoginReward reward in mDailyRewards[week])
        {
            reward.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rewards"> [0]:第1天登入獎勵, [1]:第2天登入獎勵, 已此類推. </param>
    public void SetDayReward(UIDailyLoginReward.Data[] rewards)
    {
        ClearDailyRewards();
        createDailyRewardObjs(rewards);
    }

    public void ClearDailyRewards()
    {
        foreach(var pair in mDailyRewards)
        {
            foreach(UIDailyLoginReward reward in pair.Value)
            {
                Destroy(reward.gameObject);
            }
        }
        mDailyRewards.Clear();
    }

    private void createDailyRewardObjs(UIDailyLoginReward.Data[] rewards)
    {
        int parentIndex = 0;
        int week = 1;
        foreach(var data in rewards)
        {
            var obj = UIPrefabPath.LoadUI(UIPrefabPath.UIDailyReward, DailyParents[parentIndex]);
            UIDailyLoginReward reward = obj.GetComponent<UIDailyLoginReward>();
            reward.Set(data);

            if(!mDailyRewards.ContainsKey(week))
                mDailyRewards.Add(week, new List<UIDailyLoginReward>());
            mDailyRewards[week].Add(reward);

            ++parentIndex;
            if(parentIndex >= DailyParents.Length)
            {
                parentIndex = 0;
                ++week;
            }
        }
    }
} 