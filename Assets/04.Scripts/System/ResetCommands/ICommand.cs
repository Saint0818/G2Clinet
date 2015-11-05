
/// <summary>
/// 搭配 ResetCommands 一起使用, 這是某一個命令.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// 命令執行的時間是否到了.
    /// </summary>
    /// <returns> true: 時間到了. </returns>
    bool IsTimeUp();

    /// <summary>
    /// 執行命令. 使用者也順便更新時間, 避免不斷的送出 Command.
    /// </summary>
    void Execute();
}
