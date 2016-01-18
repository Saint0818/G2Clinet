using GameStruct;

public class ShowLevelUp : ActionQueue.IAction
{
    private bool mIsDone;

    private readonly TItemData mBeforeItem;
    private readonly TItemData mNewItem;

    public ShowLevelUp(TItemData beforeItem, TItemData newItem)
    {
        mBeforeItem = beforeItem;
        mNewItem = newItem;
    }

    public void Do()
    {
        UILevelUp.Get.ShowEquip(mBeforeItem, mNewItem);

        mIsDone = true;
    }

    public bool IsDone()
    {
        return mIsDone;
    }

    public bool DoneResult()
    {
        return true;
    }
}