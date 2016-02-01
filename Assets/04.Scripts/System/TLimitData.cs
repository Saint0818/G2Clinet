
using GameEnum;
using JetBrains.Annotations;

/// <summary>
/// Limit 表格中的某一筆資料.
/// </summary>
public class TLimitData
{
    [UsedImplicitly]
    public EOpenID OpenID { get; private set; }

    [UsedImplicitly]
    public int Lv { get; private set; }

    [UsedImplicitly]
    public int Diamond { get; private set; }

    public override string ToString()
    {
        return string.Format("OpenID: {0}, Lv: {1}, Diamond: {2}", OpenID, Lv, Diamond);
    }
}