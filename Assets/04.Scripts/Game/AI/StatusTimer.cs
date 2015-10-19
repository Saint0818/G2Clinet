
namespace AI
{
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


