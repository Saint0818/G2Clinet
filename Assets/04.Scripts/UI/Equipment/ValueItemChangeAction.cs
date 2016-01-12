public class ValueItemChangeAction : ActionQueue.IAction
{
    private readonly int[] mChange;
    private bool mIsDone;
    private bool mDoneResult;

    public ValueItemChangeAction(int[] change)
    {
        mChange = change;
    }

    public void Do()
    {
        mIsDone = false;

        var protocol = new ValueItemExchangeProtocol();
        protocol.Send(mChange, new []{-1, -1}, onChangeValueItem);
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