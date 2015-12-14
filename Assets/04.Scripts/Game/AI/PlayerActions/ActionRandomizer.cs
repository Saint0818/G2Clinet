using System.Collections.Generic;
using JetBrains.Annotations;

namespace AI
{
    /// <summary>
    /// AI 行為選擇器.
    /// </summary>
    /// 使用方法:
    /// <list type="number">
    /// <item> new instance. </item>
    /// <item> Call Add() 多次. </item>
    /// <item> Call Do() 執行亂數行為. </item>
    /// <item> (Optional) Call Clear(). </item>
    /// </list>
    public class ActionRandomizer
    {
        private readonly List<PlayerAIAction> mActions = new List<PlayerAIAction>();

        private readonly WeightedRandomizer<PlayerAIAction> mRandomizer = new WeightedRandomizer<PlayerAIAction>();

        private readonly PlayerAIAction mNullAction = new NullAction();

        private PlayerAIAction mCurrentAction;

        public ActionRandomizer()
        {
            mCurrentAction = mNullAction;
        }

        public void Add([NotNull]PlayerAIAction action)
        {
            mActions.Add(action);
        }

        public void Clear()
        {
            mActions.Clear();
        }

        public void Do()
        {
            if(mCurrentAction.IsDone)
            {
                mCurrentAction = findValidAction();
                mCurrentAction.Do();
            }
        }

        [NotNull]
        private PlayerAIAction findValidAction()
        {
            mRandomizer.Clear();

            for (int i = 0; i < mActions.Count; i++)
            {
                if(mActions[i].IsValid())
                    mRandomizer.AddOrUpdate(mActions[i], mActions[i].Probability);
            }

            return mRandomizer.IsEmpty() ? mNullAction : mRandomizer.GetNext();
        }
    }
}
