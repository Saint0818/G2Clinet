using GameEnum;
using JetBrains.Annotations;

public struct ChapterData
{
    public int ID;

    /// <summary>
    /// 章節. 1: 第一章, 2: 第二章.
    /// </summary>
    [UsedImplicitly]
    public int Chapter { get; private set; }

    [UsedImplicitly]
    public int StarNum1 { get; private set; }
    [UsedImplicitly]
    public int StarNum2 { get; private set; }
    [UsedImplicitly]
    public int StarReward1 { get; private set; }
    [UsedImplicitly]
    public int StarReward2 { get; private set; }

    [UsedImplicitly]
    public string NameTW { get; private set; }
    [UsedImplicitly]
    public string NameCN { get; private set; }
    [UsedImplicitly]
    public string NameEN { get; private set; }
    [UsedImplicitly]
    public string NameJP { get; private set; }
    [UsedImplicitly]
    public string ExplainTW { get; private set; }
    [UsedImplicitly]
    public string ExplainCN { get; private set; }
    [UsedImplicitly]
    public string ExplainEN { get; private set; }
    [UsedImplicitly]
    public string ExplainJP { get; private set; }

    public bool IsValid()
    {
        return ID >= 1 && Chapter >= 1;
    }

    public override string ToString()
    {
        return string.Format("ID: {0}, Chapter: {1}", ID, Chapter);
    }

    public string Name
    {
        get
        {
            switch(GameData.Setting.Language)
            {
                case ELanguage.TW: return NameTW;
                case ELanguage.CN: return NameCN;
                case ELanguage.JP: return NameJP;
                default : return NameEN;
            }
        }
    }

    public string Explain
    {
        get
        {
            switch(GameData.Setting.Language)
            {
                case ELanguage.TW: return ExplainTW;
                case ELanguage.CN: return ExplainCN;
                case ELanguage.JP: return ExplainJP;
                default : return ExplainEN;
            }
        }
    }
}