using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDailyLoginMain : MonoBehaviour
{
    public Action OnCloseClickListener;
    
    /// <summary>
    /// <para> 呼叫時機: 可領取按鈕按下. </para>
    /// <para> 參數(int): Year. </para>
    /// <para> 參數(int): Month. </para>
    /// </summary>
    public event Action<int, int> OnReceiveListener;
    public void FireReceiveClick()
    {
        if(OnReceiveListener != null)
            OnReceiveListener(Year, Month);
    }

    /// <summary>
    /// [0]:第 1 天, [1]: 第 2 天 ... [6]:第 7 天.
    /// </summary>
    public Transform[] DailyParents;
    public UIToggle[] Toggles;

    public UIButton CloseButton;

    public int Year { get; private set; }
    public int Month { get; private set; }

    /// <summary>
    /// key: 1 是第 1 週, 2 是第 2 週, 已此類推.
    /// </summary>
    private readonly Dictionary<int, List<DailyLoginReward>> mDailyRewards = new Dictionary<int, List<DailyLoginReward>>();

    private void Awake()
    {
        CloseButton.onClick.Add(new EventDelegate(() =>
        {
            if(OnCloseClickListener != null)
                OnCloseClickListener();
        }));

        Toggles[0].onChange.Add(new EventDelegate(() => { if(UIToggle.current.value) ShowWeek(1, false); }));
        Toggles[1].onChange.Add(new EventDelegate(() => { if(UIToggle.current.value) ShowWeek(2, false); }));
        Toggles[2].onChange.Add(new EventDelegate(() => { if(UIToggle.current.value) ShowWeek(3, false); }));
        Toggles[3].onChange.Add(new EventDelegate(() => { if(UIToggle.current.value) ShowWeek(4, false); }));
    }

    public void ShowWeek(int week, bool updateTab = true)
    {
        if(week > 4 || week < 1)
            return;

        // 如果只有 2 週的資料, 但是 week = 4, 那麼會校正 week 為 2, 然後顯示第二週的資料.
        var reviseWeek = week;
        while(!mDailyRewards.ContainsKey(reviseWeek))
        {
            --reviseWeek;
            if(reviseWeek < 1)
                return;
        }

        // Hide All Dialy Reward.
        foreach (var pair in mDailyRewards)
            foreach(DailyLoginReward reward in pair.Value)
                reward.gameObject.SetActive(false);

        if(mDailyRewards.ContainsKey(reviseWeek))
            foreach(DailyLoginReward reward in mDailyRewards[reviseWeek])
                reward.gameObject.SetActive(true);

        if(updateTab)
            Toggles[reviseWeek - 1].value = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="rewards"> [0]:第1天登入獎勵, [1]:第2天登入獎勵, 已此類推. </param>
    public void SetDayReward(int year, int month, DailyLoginReward.Data[] rewards)
    {
        Year = year;
        Month = month;

        ClearDailyRewards();
        createDailyRewardObjs(rewards);
        updateTabs(rewards.Length);
    }

    public void ClearDailyRewards()
    {
        foreach(var pair in mDailyRewards)
            foreach(DailyLoginReward reward in pair.Value)
                Destroy(reward.gameObject);
        mDailyRewards.Clear();
    }

    private void createDailyRewardObjs(DailyLoginReward.Data[] rewards)
    {
        int parentIndex = 0;
        int week = 1;
        foreach(var data in rewards)
        {
            string prefabName = parentIndex == 6 ? UIPrefabPath.UIDailyReward7 : UIPrefabPath.UIDailyReward;
            var obj = UIPrefabPath.LoadUI(prefabName, DailyParents[parentIndex]);
            var reward = obj.GetComponent<DailyLoginReward>();
            reward.Set(data);

            if(!mDailyRewards.ContainsKey(week))
                mDailyRewards.Add(week, new List<DailyLoginReward>());
            mDailyRewards[week].Add(reward);

            ++parentIndex;
            if(parentIndex >= DailyParents.Length)
            {
                parentIndex = 0;
                ++week;
            }
        }
    }

    private void updateTabs(int rewardNum)
    {
        Toggles[0].gameObject.SetActive(rewardNum > 0);
        Toggles[1].gameObject.SetActive(rewardNum > 7);
        Toggles[2].gameObject.SetActive(rewardNum > 14);
        Toggles[3].gameObject.SetActive(rewardNum > 21);
    }
} 