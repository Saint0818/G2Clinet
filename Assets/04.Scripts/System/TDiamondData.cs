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

    public int[] Nums
    {
        get { return mNums ?? (mNums = new [] {Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8}); }
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