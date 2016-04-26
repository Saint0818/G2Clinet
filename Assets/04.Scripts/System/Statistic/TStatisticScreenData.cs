using JetBrains.Annotations;

/// <summary>
/// Statistic(Screen) 表格中的某一筆資料.
/// </summary>
public class TStatisticScreenData
{
    [UsedImplicitly]
    public int ID { get; private set; }

    [UsedImplicitly]
    public string Name { get; private set; }

    public override string ToString()
    {
        return string.Format("ID: {0}, Name: {1}", ID, Name);
    }
}