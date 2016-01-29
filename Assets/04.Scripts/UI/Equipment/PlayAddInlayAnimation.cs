public class PlayAddInlayAnimation : ActionQueue.IAction
{
    private bool mIsDone;
    private bool mDoneResult;

    private readonly UIEquipmentMain mMain;
    private readonly int mMaterialIndex;

    public PlayAddInlayAnimation(UIEquipmentMain main, int materialIndex)
    {
        mMain = main;
        mMaterialIndex = materialIndex;
    }

    public void Do()
    {
        mIsDone = false;

        mMain.PlayInlayAnimation(mMaterialIndex, onComplete);
    }

    public bool IsDone()
    {
        return mIsDone;
    }

    public bool DoneResult()
    {
        return mDoneResult;
    }

    private void onComplete()
    {
        AudioMgr.Get.PlaySound(SoundType.SD_ActiveLaunch);

        mDoneResult = true;
        mIsDone = true;
    }
}