using UnityEngine;

public class ValueItemAddInlayAction : ActionQueue.IAction
{
    private readonly int mPlayerValueItemKind;
    private readonly int mStorageMaterialItemIndex;
    private bool mIsDone;
    private bool mDoneResult;

    public ValueItemAddInlayAction(int playerValueItemKind, int storageMaterialItemIndex)
    {
        mPlayerValueItemKind = playerValueItemKind;
        mStorageMaterialItemIndex = storageMaterialItemIndex;
    }

    public void Do()
    {
        mIsDone = false;

        var protocol = new ValueItemAddInlayProtocol();
        protocol.Send(mPlayerValueItemKind, mStorageMaterialItemIndex, onAddInlay);
    }

    public bool IsDone()
    {
        return mIsDone;
    }

    public bool DoneResult()
    {
        return mDoneResult;
    }

    private void onAddInlay(bool ok)
    {
        if(ok)
            UIHint.Get.ShowHint(TextConst.S(554), Color.white);

        mDoneResult = ok;
        mIsDone = true;
    }
}