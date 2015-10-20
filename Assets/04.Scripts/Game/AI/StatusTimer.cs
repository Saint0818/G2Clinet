
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
    /// <item>  </item>
    /// </list>
    public class StatusTimer
    {
        private readonly CountDownTimer mTimer = new CountDownTimer(1);

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
    } // end of the class.
} // end of the namespace.


