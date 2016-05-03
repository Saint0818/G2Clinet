using UnityEngine;

public class StatisticNodeJs : IStatisticService
{
    public StatisticNodeJs()
    {
    }

    public void LogScreen(int id, string name)
    {
        WWWForm form = new WWWForm();
        form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
        form.AddField("ID", id);
        SendHttp.Get.Command(URLConst.LastScreenID, null, form, false);
    }

    public void LogEvent(int id, string category, string action, string label, int value)
    {
        WWWForm form = new WWWForm();
        form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
        form.AddField("ID", id);
        SendHttp.Get.Command(URLConst.LastEventID, null, form, false);
    }
}