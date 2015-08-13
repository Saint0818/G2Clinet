using System;

public struct TTactical
{
    public string FileName; // 戰術名稱.
    public TActionPosition[] PosAy1; // 中鋒的資料.
    public TActionPosition[] PosAy2; // 前鋒的資料.
    public TActionPosition[] PosAy3; // 後衛的資料.

    public TTactical(bool flag)
    {
        FileName = "";
        PosAy1 = new TActionPosition[0];
        PosAy2 = new TActionPosition[1];
        PosAy3 = new TActionPosition[2];
    }

    public override string ToString()
    {
        return String.Format("Name:{0}", FileName);
    }
}