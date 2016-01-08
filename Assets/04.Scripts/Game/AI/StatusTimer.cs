
using System;

namespace AI
{
    /// <summary>
    /// 用在有冷卻時間的狀態.
    /// </summary>
    /// 使用方法:
    /// <list type="number">
    /// <item> new instance. </item>
    /// <item> 每個 frame 呼叫 Update(). </item>
    /// <item> Call StartCounting() 開始倒數計時. </item>
    /// <item> Call IsOn() or IsOff() 檢查狀態. </item>
    /// <item> (Optinal) 向 TimeUpListener 註冊事件. </item>
    /// </list>
    public class StatusTimer
    {
        /// <summary>
        /// 呼叫時機: 倒數完畢時.
        /// </summary>
        public event Action TimeUpListener;

        private readonly CountDownTimer mTimer = new CountDownTimer(1);

        public float RemainTime
        {
            get { return mTimer.RemainTime; }
        }

        public StatusTimer()
        {
            mTimer.TimeUpListener += timeUp;
        }

        public void StartCounting(float remainSecond)
        {
            mTimer.Start(remainSecond);
        }

        public bool IsOn()
        {
            return !mTimer.IsTimeUp();
        }

        public bool IsOff()
        {
            return mTimer.IsTimeUp();
        }

        public void Clear()
        {
            mTimer.Stop();
        }

        public void Update(float elapsedTime)
        {
            mTimer.Update(elapsedTime);
        }

        private void timeUp()
        {
            if(TimeUpListener != null)
                TimeUpListener();
        }
    } // end of the class.
} // end of the namespace.


