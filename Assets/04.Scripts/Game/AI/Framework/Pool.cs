
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// How to use:
/// <list type="number">
/// <item> new instance. </item>
/// <item> call CreateOrGet() to acquire an instance. </item>
/// <item> call Free() to release instance. </item>
/// </list>
/// </remarks>
public class Pool<T> where T:class
{
    public delegate T Action();
    public delegate void Action2(T obj);

    private readonly Action mCreateMethod;
    private readonly Action2 mResetMethod;

    private readonly Queue<T> mQueue = new Queue<T>();

    private int mSize;
    private readonly int mMaxSize;

    public Pool(Action createMethod, Action2 resetMethod, int defaultSize = 100, int maxSize = 2000)
    {
        mCreateMethod = createMethod;
        mResetMethod = resetMethod;

        mMaxSize = maxSize;

        for(int i = 0; i < defaultSize; i++)
        {
            var obj = CreateOrGet();
            if(obj == null)
                break;
            Free(obj);
        }
    } 

    [CanBeNull]
    public T CreateOrGet()
    {
        if(mQueue.Count > 0)
        {
            var obj = mQueue.Dequeue();
            mResetMethod(obj);
            return obj;
        }

        if(mSize > mMaxSize)
        {
            Debug.LogErrorFormat("Pool is full... {0}/{1}", mSize, mMaxSize);
            return null;
        }

        var newObj = mCreateMethod();
        ++mSize;
        return newObj;
    }

    public void Free([NotNull]T obj)
    {
        mQueue.Enqueue(obj);
    }
}
