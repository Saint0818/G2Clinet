using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

/// <summary>
/// Diamonds 表格中的某一筆資料.
/// </summary>
public class TDiamondData
{
    public enum EKind
    {
        Unknown = 0,
        ResetDailyChallenge = 1
    }

    [Description("改用 GetReviseNum().")]
    public int[] Nums
    {
        get
        {
            if(mNums == null)
            {
                Action<List<int>, int> checkAndAdd = (collections, value) =>
                {
                    if(value > 0)
                        collections.Add(value);
                };
                List<int> nums = new List<int>();
                checkAndAdd(nums, Num1);
                checkAndAdd(nums, Num2);
                checkAndAdd(nums, Num3);
                checkAndAdd(nums, Num4);
                checkAndAdd(nums, Num5);
                checkAndAdd(nums, Num6);
                checkAndAdd(nums, Num7);
                checkAndAdd(nums, Num8);
                mNums = nums.ToArray();
            }
            return mNums;
        }
    }

    public int GetReviseNum(int index)
    {
        if(0 <= index && index < Nums.Length)
            return Nums[index];
        return Nums[Nums.Length - 1]; // 回傳最後一個數值.
    }
    private int[] mNums;

    [UsedImplicitly]
    public EKind Kind { get; private set; }

    [UsedImplicitly]
    public int Num1 { get; private set; }
    [UsedImplicitly]
    public int Num2 { get; private set; }
    [UsedImplicitly]
    public int Num3 { get; private set; }
    [UsedImplicitly]
    public int Num4 { get; private set; }
    [UsedImplicitly]
    public int Num5 { get; private set; }
    [UsedImplicitly]
    public int Num6 { get; private set; }
    [UsedImplicitly]
    public int Num7 { get; private set; }
    [UsedImplicitly]
    public int Num8 { get; private set; }
}