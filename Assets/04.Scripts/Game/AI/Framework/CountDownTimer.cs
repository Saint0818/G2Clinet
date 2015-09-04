
/// <summary>
/// 
/// </summary>
/// How to use:
/// <list type="number">
/// <item> new instance. </item>
/// <item> call Update in every frame. </item>
/// <item> register listener. </item>
/// <item> call RunOnce or RunForever. </item>
/// </list>
public class CountDownTimer
{
    public delegate void Action(float elapsedTime);
    public event Action Listener;

    private float mElapsedTime;
    private float mCountDownTime;
    private bool mRunning;
    private bool mIsRunOnce;

    public void RunOnce(float countDownTime)
    {
        mElapsedTime = 0;
        mCountDownTime = countDownTime;
        mRunning = true;
        mIsRunOnce = true;
    }

    public void RunForever(float countDownTime)
    {
        mElapsedTime = 0;
        mCountDownTime = countDownTime;
        mRunning = true;
        mIsRunOnce = false;
    }

    public void Stop()
    {
        mRunning = false;
    }

    public void Update(float elapsedTime)
    {
        if(!mRunning)
            return;

        mElapsedTime += elapsedTime;

        if(mElapsedTime < mCountDownTime)
            return;

        if(Listener != null)
            Listener(mElapsedTime);

        mElapsedTime = 0;

        if(mIsRunOnce)
            mRunning = false;
    }
}
