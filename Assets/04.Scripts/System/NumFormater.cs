
using System;
using System.Globalization;

public static class NumFormater
{
    public static string Convert(int num)
    {
        // https://msdn.microsoft.com/zh-tw/library/0c899ak8%28v=vs.110%29.aspx
        return String.Format(CultureInfo.InvariantCulture, "{0:#,0}", num);
    }
}
