using GameEnum;

public struct ChapterData
{
    public int ID;

    /// <summary>
    /// 章節. 1: 第一章, 2: 第二章.
    /// </summary>
    public int Chapter;
    
    public string NameTW;
    public string NameCN;
    public string NameEN;
    public string NameJP;
    public string ExplainTW;
    public string ExplainCN;
    public string ExplainEN;
    public string ExplainJP;

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