using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 管控執行順序, 當 1 個 Action 執行完畢時, 才會執行下一個 Action. 如果 1 個 Action
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> new instance. </item>
/// <item> Call Clear(). </item>
/// <item> Call AddAction(). 多次 </item>
/// <item> Call Execute() 執行全部的命令. </item>
/// </list>
public class ActionQueue : MonoBehaviour
{
    private readonly List<IAction> mActions = new List<IAction>();
    private readonly Queue<IAction> mQueue = new Queue<IAction>();

    public interface IAction
    {
        void Do();
        bool IsDone();
        bool DoneResult();
    }

    public void AddAction([NotNull]IAction action)
    {
        mActions.Add(action);
    }

    public void Clear()
    {
        mActions.Clear();
    }

    public void Execute(CommonDelegateMethods.Bool1 callback)
    {
        if(mActions.Count == 0)
        {
            callback(true);
            return;
        }

        mQueue.Clear();
        for(var i = 0; i < mActions.Count; i++)
        {
            mQueue.Enqueue(mActions[i]);
        }
        StartCoroutine(execute(callback));
    }

    private IEnumerator execute(CommonDelegateMethods.Bool1 callback)
    {
        var action = mQueue.Dequeue();
        action.Do();
        while(true)
        {
            if(!action.IsDone())
            {
                yield return new WaitForEndOfFrame();
                continue;
            }

            if(!action.DoneResult())
            {
                callback(false);
                break;
            }

            if(mQueue.Count == 0)
            {
                callback(true);
                break;
            }

            action = mQueue.Dequeue();
            action.Do();    
        }
    } 
}