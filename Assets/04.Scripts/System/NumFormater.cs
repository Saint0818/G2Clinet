
using System;
using System.Globalization;

public static class NumFormater
{
    public static string Convert(int num)
    {
        return String.Format(CultureInfo.InvariantCulture, "{0:#,#}", num);
    }
}
