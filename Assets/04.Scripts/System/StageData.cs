using GameEnum;

public struct StageData
{
    public int ID;
    public int Chapter;
    public int Order;

    /// <summary>
    /// 1.�ǲ�
    /// 2.�o��
    /// 3.���u
    /// 4.����
    /// 9.�D���]�����
    /// </summary>
    public int Kind;
     
    public string Hint;
    public int Bit0Num;
    public int Bit1Num;
    public int Bit2Num;
    public int Bit3Num;
    public int CourtMode;
    public int WinMode;
    public int WinValue;
    public int FriendNumber;

    public int PlayerID1;
    public int PlayerID2;
    public int PlayerID3;
    public int PlayerID4;
    public int PlayerID5;
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
        return ID >= 1;
    }

    public override string ToString()
    {
        return string.Format("ID: {0}, Chapter: {1}, Order: {2}", ID, Chapter, Order);
    }

    public int[] HintBit
    {
        get
        {
            return AI.BitConverter.Convert(Hint);
        }
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