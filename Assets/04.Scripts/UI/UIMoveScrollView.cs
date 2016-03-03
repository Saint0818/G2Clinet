using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 負責對 UIScrollView 做移動. 主要是模擬章節的切換, 比如切換主線和副本的章節.
/// </summary>
public class UIMoveScrollView : MonoBehaviour
{
    /// <summary>
    /// 捲動時間, 單位: 秒.
    /// </summary>
    private const float MoveTime = 0.3f;

    private UIScrollView mScrollView;
    private Vector3 mScrollViewStartPos;

    private Action mOnMoveFinish;
    private Action mOnMoving;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scrollView"> 要捲動的 UI 元件. </param>
    /// <param name="targetPos"> ScrollView 要捲動到到的目的位置. </param>
    /// <param name="onMoveFinish"> 呼叫時機: 移動完成後. </param>
    /// <param name="onMoving"> 呼叫時機: ScrollView 移動 1 步. </param>
    public void Move(UIScrollView scrollView, Vector3 targetPos, Action onMoveFinish = null, 
                     Action onMoving = null)
    {
        mScrollView = scrollView;
        mOnMoveFinish = onMoveFinish;
        mOnMoving = onMoving;

        mScrollViewStartPos = mScrollView.transform.localPosition;

        Vector3 moveAmount = targetPos - mScrollView.transform.localPosition;

//        Debug.LogFormat("MoveToChapter, Chapter:{0}, TargetPos:{1}, MoveAmount:{2}", reviseChapter, targetPos, sumMoveAmount);

        StartCoroutine(moving(moveAmount));
    }

    private IEnumerator moving(Vector3 sumMoveAmount)
    {
        var centerOnChild = mScrollView.GetComponentInChildren<UICenterOnChild>();
        if(centerOnChild)
            centerOnChild.enabled = false;

        float elapsedTime = 0;
//        Vector3 stepMoveAmount = sumMoveAmount / MoveStep;

//        Debug.LogFormat("moving, stepMoveAmount:{0}", stepMoveAmount);

        while(elapsedTime <= MoveTime)
        {
            yield return new WaitForEndOfFrame();

            elapsedTime += Time.deltaTime;

            float percent = elapsedTime / MoveTime;
            if(percent > 1)
                percent = 1;

            Vector3 currentTargetPos = sumMoveAmount * percent + mScrollViewStartPos;
            Vector3 currentMoveValue = currentTargetPos - mScrollView.transform.localPosition;

//            Debug.LogFormat("CurTargetPos:{0}, CurMoveValue:{1}", currentTargetPos, currentMoveValue);

            mScrollView.MoveRelative(currentMoveValue);

            if(mOnMoving != null)
                mOnMoving();
        }

        if(centerOnChild)
            centerOnChild.enabled = true;

        if(mOnMoveFinish != null)
            mOnMoveFinish();
    }
}
