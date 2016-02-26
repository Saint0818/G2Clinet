public class OpenInstanceAction : UIItemSourceElement.IAction
{
    private readonly int mStageID;

    public OpenInstanceAction(int stageID)
    {
        mStageID = stageID;
    }

    public void Do()
    {
        UIInstance.Get.ShowByStageID(mStageID);
    }
}