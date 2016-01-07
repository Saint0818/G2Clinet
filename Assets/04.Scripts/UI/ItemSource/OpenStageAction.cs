using UnityEngine;

public class OpenStageAction : UIItemSourceElement.IAction
{
    private readonly int mStageID;

    public OpenStageAction(int stageID)
    {
        mStageID = stageID;
    }

    public void Do()
    {
        Debug.LogFormat("Show StageID({0})", mStageID);
    }
}