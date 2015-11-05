using System.Collections.Generic;
using JetBrains.Annotations;

/// <summary>
/// 負責固定時間送 Command 給 Server 的工具類別.
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Run() 啟動檢查. </item>
/// <item> Call Stop() 停止檢查. </item>
/// </list>
public class ResetCommands : KnightSingleton<ResetCommands>
{
    private readonly List<ICommand> mCommands = new List<ICommand>();
    private bool mIsRunning;

    [UsedImplicitly]
    private void Awake()
    {
        mCommands.Add(new ResetStageCommand());
    }

    [UsedImplicitly]
    private void Update()
    {
        if(!mIsRunning)
            return;

        for(int i = 0; i < mCommands.Count; i++)
        {
            if(mCommands[i].IsTimeUp())
                mCommands[i].Execute();
        }
    }

    public void Run()
    {
        mIsRunning = true;
    }

    public void Stop()
    {
        mIsRunning = false;
//        DestroyInst();
    }
}
