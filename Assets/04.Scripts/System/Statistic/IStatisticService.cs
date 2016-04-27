
public interface IStatisticService
{
    void LogScreen(int id, string name);
    void LogEvent(int id, string category, string action, string label, int value);
}