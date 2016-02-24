using UnityEngine;

public class UILifetimeLoginMain : MonoBehaviour
{
    public UILabel LifetimeLoginNumLabel;
    public Transform[] RewardParents;
    private readonly UILifetimeReward[] mRewards = new UILifetimeReward[3];

    public int LifetimeLoginNum
    {
        set { LifetimeLoginNumLabel.text = string.Format(TextConst.S(3806), value); }
    }

    private void Awake()
    {
        for(var i = 0; i < RewardParents.Length; i++)
        {
            var obj = UIPrefabPath.LoadUI(UIPrefabPath.UILifetimeReward, RewardParents[i]);
            mRewards[i] = obj.GetComponent<UILifetimeReward>();
            obj.SetActive(false);
        }
    }

    public void HideAllRewards()
    {
        foreach(var reward in mRewards)
            reward.gameObject.SetActive(false);
    }

    public void SetRewards(UILifetimeReward.Data[] rewards)
    {
        HideAllRewards();

        for(var i = 0; i < mRewards.Length; i++)
        {
            if(i >= rewards.Length)
                break;
            
            mRewards[i].Set(rewards[i]);
            mRewards[i].gameObject.SetActive(true);
        }
    }
}