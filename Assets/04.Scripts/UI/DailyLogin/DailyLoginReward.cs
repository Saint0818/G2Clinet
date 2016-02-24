using System;
using GameStruct;
using UnityEngine;

public abstract class DailyLoginReward : MonoBehaviour
{
    public enum EStatus
    {
        Received, // 已領取.
        Receivable, // 可領取.
        NoReceive // 不可領取.
    }

    public class Data
    {
        public string Day;
        public string Name;
        public EStatus Status;
        public TItemData ItemData;
    }

    public abstract void Set(Data data);
}