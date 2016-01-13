using System;
using System.Text;

public static class DebugerString
{
    private static readonly StringBuilder Builder = new StringBuilder();

    public static string Convert(int[] data)
    {
        if(data == null)
            return String.Empty;

        Builder.Length = 0;
        Builder.Append("[");
        for(var i = 0; i < data.Length; i++)
        {
            Builder.Append(data[i]);
            if(i + 1 < data.Length) // 不是最後一個
                Builder.Append(",");
        }

        Builder.Append("]");
        return Builder.ToString();
    }
}