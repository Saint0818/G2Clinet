using GameStruct;
using UnityEngine;

public abstract class IDailyLoginReward : MonoBehaviour
{
    public class Data
    {
        public string Day;
        public string Name;
        public bool ShowClear;
        public TItemData ItemData;
    }

    public abstract void Set(Data data);
}