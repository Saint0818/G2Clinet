using JetBrains.Annotations;

/// <summary>
/// Statistic(Event) 表格中的某一筆資料.
/// </summary>
public class TStatisticEventData
{
    [UsedImplicitly]
    public int ID { get; private set; }

    [UsedImplicitly]
    public string Category { get; private set; }

    [UsedImplicitly]
    public string Action { get; private set; }

    [UsedImplicitly]
    public string Label { get; private set; }

    [UsedImplicitly]
    public int Value { get; private set; }

    public override string ToString()
    {
        return string.Format("ID: {0}, Category: {1}, Action: {2}, Label: {3}, Value: {4}", ID, Category, Action, Label, Value);
    }
}