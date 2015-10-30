using System;
using System.Collections.Generic;
using GameEnum;
using GamePlayEnum;

/// <summary>
/// 某個小關卡的相關資料.
/// </summary>
public class StageData
{
    public int ID;
    public int Chapter;
    public int Order;

    /// <summary>
    /// 1.傳統
    /// 2.得分
    /// 3.防守
    /// 4.攻擊
    /// 9.挑戰魔王對手
    /// </summary>
    public int Kind;

    public int KindTextIndex
    {
        get { return StageKindMapping[Kind]; }
    }
     
    public string Hint;
    public int Bit0Num;
    public int Bit1Num;
    public int Bit2Num;
    public int Bit3Num;
    public int CourtMode;

    public enum ECostKind
    {
        Stamina = 0, // 體力.
        Activity = 1, // 活動.
        Challenger = 2 // 踢館.
    }
    /// <summary>
    /// <para> 進入關卡要消耗的數值種類. </para>
    /// </summary>
    public ECostKind CostKind;

    /// <summary>
    /// 進入關卡要消耗的數值.
    /// </summary>
    public int CostValue;

    public int WinMode;
    public int WinValue;
    public int FriendNumber;

    /// <summary>
    /// 小關卡在章節上的位置.
    /// </summary>
    public float PositionX;
    public float PositionY;

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

    public void Clear()
    {
        ID = 0;
    }

    public bool IsValid()
    {
        return ID >= 1;
    }

    public override string ToString()
    {
        return String.Format("ID: {0}, Chapter: {1}, Order: {2}", ID, Chapter, Order);
    }

    public int[] HintBit
    {
        get
        {
            return AI.BitConverter.Convert(Hint);
        }
    }

    public int ConvertWinMode()
    {
		var hintBits = HintBit;
		if(hintBits[0] == 0 && hintBits[1] == 0 && (hintBits[2] > 0 || hintBits[3] > 0))
			return (int)EWinMode.NoneCondition;
        if(hintBits[0] == 0 && hintBits[1] == 0)
			return (int)EWinMode.None;
        if(hintBits[0] == 0 && hintBits[1] == 1)
			return (int)EWinMode.NoTimeScore;
        if(hintBits[0] == 0 && hintBits[1] == 2)
			return (int)EWinMode.NoTimeLostScore;
        if(hintBits[0] == 0 && hintBits[1] == 3)
			return (int)EWinMode.NoTimeScoreCompare;

		if(hintBits[0] == 1 && hintBits[1] == 0 && (hintBits[2] > 0 || hintBits[3] > 0))
			return (int)EWinMode.TimeNoScoreCondition;
        if(hintBits[0] == 1 && hintBits[1] == 0)
			return (int)EWinMode.TimeNoScore;
        if(hintBits[0] == 1 && hintBits[1] == 1)
			return (int)EWinMode.TimeScore;
        if(hintBits[0] == 1 && hintBits[1] == 2)
			return (int)EWinMode.TimeLostScore;
        if(hintBits[0] == 1 && hintBits[1] == 3)
			return (int)EWinMode.TimeScoreCompare;

		return (int)EWinMode.None;
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

    /// <summary>
    /// <para> 這是 Stage.Kind 參數的對照表. [Key:Kind, Value:TextConst Index]</para>
    /// <para> 目前的規則是對照表找出的數值, 就是關卡的圖片, 也就是該關卡的類型文字(比如:傳統, 計時賽等等). </para>
    /// </summary>
    private readonly Dictionary<int, int> StageKindMapping = new Dictionary<int, int>
    {
        {1, 2000001},
        {2, 2000002},
        {3, 2000003},
        {4, 2000004},
        {9, 2000009}
    };
}