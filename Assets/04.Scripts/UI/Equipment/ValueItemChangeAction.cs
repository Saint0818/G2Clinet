public class ValueItemChangeAction : ActionQueue.IAction
{
    private readonly int[] mExChange;
    private readonly int[] mStackIndices;
    private bool mIsDone;
    private bool mDoneResult;

    public ValueItemChangeAction(int[] change, int[] stackIndices)
    {
        mExChange = change;
        mStackIndices = stackIndices;
    }

    public void Do()
    {
        mIsDone = false;

        var protocol = new ValueItemExchangeProtocol();
        protocol.Send(mExChange, mStackIndices, onChangeValueItem);
    }

    public bool IsDone()
    {
        return mIsDone;
    }

    public bool DoneResult()
    {
        return mDoneResult;
    }

    private void onChangeValueItem(bool ok)
    {
        mDoneResult = ok;
        mIsDone = true;
    }
}