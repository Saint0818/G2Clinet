using GameStruct;
using UnityEngine;

public abstract class DailyLoginReward : MonoBehaviour
{
    public class Data
    {
        public string Day;
        public string Name;
        public UIDailyLoginMain.EStatus Status;
        public TItemData ItemData;
    }

    public abstract void Set(Data data);
}