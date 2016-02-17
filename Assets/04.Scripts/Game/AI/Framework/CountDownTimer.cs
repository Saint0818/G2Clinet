

using System;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// 
    /// </summary>
    /// How to use:
    /// <list type="number">
    /// <item> new instance. </item>
    /// <item> Call Update in every frame. </item>
    /// <item> Call Start() or StartAgain(). </item>
    /// <item> Call IsTimeUp() 檢查是否倒數完畢. </item>
    /// <item> 向 TimeUpListener 註冊倒數完畢事件. </item>
    /// <item> (Optinal) 用 Resume(), Pause(), Stop() 做額外的控制. </item>
    /// </list>
    public class CountDownTimer
    {
        /// <summary>
        /// 呼叫時機: 倒數完畢.
        /// </summary>
        public event Action TimeUpListener;

        /// <summary>
        /// 倒數完畢時的數值.
        /// </summary>
        private const float TimeUpTime = -1;

        /// <summary>
        /// 剩下的倒數時間(單位:秒), 當小於等於 0 時, 表示倒數完畢.
        /// </summary>
        public float RemainTime
        {
            get { return mRemainTime; }
        }
        private float mRemainTime = TimeUpTime;

        private bool mLoop;

        /// <summary>
        /// 前一次的倒數時間.
        /// </summary>
        private float mLastRemainTime;
        private bool mIsUpdating;

        public CountDownTimer(float defaultRemainTime)
        {
            if(defaultRemainTime <= 0)
            {
                Debug.LogErrorFormat("defaultRemainTime({0}) must be great than zero.", defaultRemainTime);
                return;
            }

            mLastRemainTime = defaultRemainTime;
        }

        public void Start(float remainSecond, bool loop = false)
        {
            mLastRemainTime = remainSecond;
            mRemainTime = remainSecond;
            mIsUpdating = true;
            mLoop = loop;
        }

        public void StartAgain(bool loop = false)
        {
            mRemainTime = mLastRemainTime;
            mIsUpdating = true;
            mLoop = loop;
        }

        public void Resume()
        {
            mIsUpdating = true;
        }

        public void Pause()
        {
            mIsUpdating = false;
        }

        public void Stop()
        {
            mRemainTime = TimeUpTime;
            mIsUpdating = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> true: 倒數完畢; false: 還在倒數中. </returns>
        public bool IsTimeUp()
        {
            return mRemainTime <= 0;
        }

        public void Update(float elapsedTime)
        {
            if(!mIsUpdating)
                return;

            mRemainTime -= elapsedTime;
            if(mRemainTime <= 0)
            {
                mRemainTime = TimeUpTime; // 表示時間已經到了.
                mIsUpdating = mLoop;

                fireTimeUp();
            }
        }

        private void fireTimeUp()
        {
            if(TimeUpListener != null)
                TimeUpListener();
        }
    } // end of the class.
} // end of the namespace.


