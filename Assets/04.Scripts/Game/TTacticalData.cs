using System;
using System.Collections.Generic;

public struct TTacticalAction
{
    /// <summary>
    /// 球員要跑的位置.
    /// </summary>
    public float x;
    public float z;

    public bool Speedup;

    /// <summary>
    /// true: 球員跑到定點時, 會向持球者要球.
    /// </summary>
    public bool Catcher;

    /// <summary>
    /// true: 球員跑到定點時, 投籃.
    /// </summary>
    public bool Shooting;
}

/// <summary>
/// 某個戰術的全部資料.
/// </summary>
public struct TTacticalData
{
    public string FileName; // 戰術名稱.
    public TTacticalAction[] PosAy1; // 中鋒的資料.
    public TTacticalAction[] PosAy2; // 前鋒的資料.
    public TTacticalAction[] PosAy3; // 後衛的資料.

    public bool IsValid { get { return !string.IsNullOrEmpty(FileName); } }

    public ETacticalKind Kind
    {
        get
        {
            if(mKind == ETacticalKind.None)
                convert();

            return mKind;
        }
    }

    private ETacticalKind mKind;

    private void convert()
    {
        foreach(KeyValuePair<string, ETacticalKind> pair in TacticalTable.PrefixNameTacticalPairs)
        {
            if(FileName.StartsWith(pair.Key))
                mKind = pair.Value;
        }
    }
		
    public override string ToString()
    {
        return String.Format("Name:{0}", FileName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public TTacticalAction[] GetActions(EPlayerPostion pos)
    {
        if(pos == EPlayerPostion.C)
            return PosAy1;
        if(pos == EPlayerPostion.F)
            return PosAy2;
        if(pos == EPlayerPostion.G)
            return PosAy3;

        throw new NotImplementedException();
    }
}
