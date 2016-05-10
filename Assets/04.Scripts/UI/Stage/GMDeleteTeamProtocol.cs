using UnityEngine;

public class GMDeleteTeamProtocol
{
    public void Send(string machineCode)
    {
        WWWForm form = new WWWForm();
        form.AddField("Identifier", machineCode);
        SendHttp.Get.Command(URLConst.GMDeleteTeam, waitDeleteTeam, form);
    }

    private void waitDeleteTeam(bool ok, WWW www)
    {
        Debug.LogFormat("waitDeleteTeam, ok:{0}", ok);

        if(ok)
        {
            if(Application.platform == RuntimePlatform.WindowsEditor ||
               Application.platform == RuntimePlatform.OSXEditor)
                UnityEditor.EditorApplication.isPlaying = false;
            else
                Application.Quit();
        }
    }
}