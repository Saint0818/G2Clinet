
public class OpenMainStageAction : UIItemSourceElement.IAction
{
    private readonly int mStageID;

    public OpenMainStageAction(int stageID)
    {
        mStageID = stageID;
    }

    public void Do()
    {
//        Debug.LogFormat("Show StageID({0})", mStageID);

        UIMainStage.Get.Show(mStageID);
    }
}