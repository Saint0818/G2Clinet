using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 負責對 UIScrollView 做移動. 主要是模擬章節的切換, 比如切換主線和副本的章節.
/// </summary>
public class UIMoveScrollView : MonoBehaviour
{
    /// <summary>
    /// 幾個 Frame, ScrollView 捲動完畢.
    /// </summary>
    private const int MoveStep = 10;

    private UIScrollView mScrollView;

    private Action mOnMoveFinish;
    private Action mOnMoving;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scrollView"> 要捲動的 UI 元件. </param>
    /// <param name="targetPos"> ScrollView 要捲動到到的目的位置. </param>
    /// <param name="onMoveFinish"> 呼叫時機: 移動完成後. </param>
    /// <param name="onMoving"> 呼叫時機: ScrollView 移動 1 步. </param>
    public void Move(UIScrollView scrollView, Vector3 targetPos, Action onMoveFinish = null, Action onMoving = null)
    {
        mScrollView = scrollView;
        mOnMoveFinish = onMoveFinish;
        mOnMoving = onMoving;

        Vector3 moveAmount = targetPos - mScrollView.transform.localPosition;

//        Debug.LogFormat("MoveToChapter, Chapter:{0}, TargetPos:{1}, MoveAmount:{2}", reviseChapter, targetPos, moveAmount);

        StartCoroutine(moveChapter(moveAmount));
    }

    private IEnumerator moveChapter(Vector3 moveAmount)
    {
        var centerOnChild = mScrollView.GetComponent<UICenterOnChild>();
        if(centerOnChild)
            centerOnChild.enabled = false;

        Vector3 stepMoveAmount = moveAmount / MoveStep;

//        Debug.LogFormat("moveChapter, stepMoveAmount:{0}", stepMoveAmount);

        for(int i = 0; i < MoveStep; i++)
        {
            mScrollView.MoveRelative(stepMoveAmount);

            if(mOnMoving != null)
                mOnMoving();

            yield return new WaitForEndOfFrame();
        }

        if(centerOnChild)
            centerOnChild.enabled = true;

        if(mOnMoveFinish != null)
            mOnMoveFinish();
    }

}
