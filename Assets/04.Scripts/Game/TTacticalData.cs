using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameEnum;

public struct TTacticalAction
{
    /// <summary>
    /// 球員要跑的位置.
    /// </summary>
    public float X;
    public float Z;

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
    public string Name; // 戰術名稱.
    public TTacticalAction[] CActions; // 中鋒的資料.
    public TTacticalAction[] FActions; // 前鋒的資料.
    public TTacticalAction[] GActions; // 後衛的資料.

    [JsonIgnore]
    public bool IsValid { get { return !string.IsNullOrEmpty(Name); } }

    [JsonIgnore]
    public ETacticalKind Kind
    {
        get
        {
            if(mKind == ETacticalKind.Unknown)
                convert();

            return mKind;
        }
    }

    private ETacticalKind mKind;

    private void convert()
    {
        foreach(KeyValuePair<string, ETacticalKind> pair in TacticalTable.PrefixNames)
        {
            if(Name.StartsWith(pair.Key))
                mKind = pair.Value;
        }
    }
		
    public override string ToString()
    {
        return String.Format("Name:{0}", Name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public TTacticalAction[] GetActions(EPlayerPostion pos)
    {
        if(pos == EPlayerPostion.C)
            return CActions;
        if(pos == EPlayerPostion.F)
            return FActions;
        if(pos == EPlayerPostion.G)
            return GActions;

        throw new NotImplementedException();
    }
}
