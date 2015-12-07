using System;
using System.Collections.Generic;
using GameEnum;
using JetBrains.Annotations;

/// <summary>
/// 某個小關卡的相關資料.
/// </summary>
public class TStageData
{
    public int ID { get; private set; }
    public int Chapter { get; private set; }
    public int Order { get; private set; }
	public int CourtNo { get; private set; }

    [UsedImplicitly]
    public int CourtMode { get; private set; }

	public int FriendKind { get; private set; }

    /// <summary>
    /// 1.傳統
    /// 2.得分
    /// 3.防守
    /// 4.攻擊
    /// 9.挑戰魔王對手
    /// </summary>
    public int Kind { get; private set; }

    public int KindTextIndex
    {
        get { return mStageKindMapping[Kind]; }
    }
     
    public string Hint = "";
    public int Bit0Num;
    public int Bit1Num;
    public int Bit2Num;
    public int Bit3Num;
    private int[] bitNum = new int[4];
	private int[] hintBit = new int[0];

    public enum ECostKind
    {
        Stamina = 0, // 體力.
        Activity = 1, // 活動.
        Challenger = 2 // 踢館.
    }
    /// <summary>
    /// <para> 進入關卡要消耗的數值種類. </para>
    /// </summary>
    [UsedImplicitly]
    public ECostKind CostKind { get; private set; }

    /// <summary>
    /// 進入關卡要消耗的數值.
    /// </summary>
    public int CostValue { get; private set; }

    /// <summary>
    /// 每日關卡挑戰次數.
    /// </summary>
    public int ChallengeNum { get; private set; }

    /// <summary>
    /// 進入關卡的等級限制.
    /// </summary>
    public int LimitLevel { get; private set; }
    /// <summary>
    /// 進入關卡的戰鬥力限制. 因為戰鬥力是根據屬性算出總和數值, 其實非常像 2K 的總評分數, 所以才
    /// 取名為 Evaluation.
    /// </summary>
    public int LimitEvaluation { get; private set; }

    /// <summary>
    /// 亂數獎勵.
    /// </summary>
    [UsedImplicitly]
    [CanBeNull] public int[] Rewards { get; private set; } // 獎勵 ItemID.

    [UsedImplicitly]
    [CanBeNull] public int[] RewardRates { get; private set; } // 獎勵機率.

    public int WinValue;
    public int FriendNumber { get; private set; }

    /// <summary>
    /// 小關卡在章節上的位置.
    /// </summary>
    public float PositionX { get; private set; }
    public float PositionY { get; private set; }

    public int[] PlayerID;
    public int[] FriendID;

    public string NameTW { get; private set; }
    public string NameCN { get; private set; }
    public string NameEN { get; private set; }
    public string NameJP { get; private set; }
    public string ExplainTW { get; private set; }
    public string ExplainCN { get; private set; }
    public string ExplainEN { get; private set; }
    public string ExplainJP { get; private set; }

    public void Clear()
    {
        ID = 0;
		Chapter = 0;
		Order = 0;
		CourtNo = 0;
		FriendKind = 0;
		Kind = 0;
		Bit0Num = 0;
		Bit1Num = 0;
		Bit2Num = 0;
		Bit3Num = 0;
		CostValue = 0;
		ChallengeNum = 0;
		LimitLevel = 0;
		LimitEvaluation = 0;
		WinValue = 0;
		FriendNumber = 0;
		PositionX = 0;
		PositionY = 0;
		
		Hint = "";
		NameTW = "";
		NameCN = "";
		NameEN = "";
		NameJP = "";
		ExplainTW = "";
		ExplainCN = "";
		ExplainEN = "";
		ExplainJP = "";

		if (hintBit != null)
			Array.Resize(ref hintBit, 0);

		if (PlayerID != null)
			for (int i = 0; i < PlayerID.Length; i++)
				PlayerID[i] = 0;

		if (FriendID != null)
			for (int i = 0; i < FriendID.Length; i++)
				FriendID[i] = 0;

		if(Rewards != null)
			for (int i = 0; i < Rewards.Length; i++)
				Rewards[i] = 0;

		if(RewardRates != null)
			for (int i = 0; i < RewardRates.Length; i++)
				RewardRates[i] = 0;
    }

    public bool IsValid()
    {
        return ID >= 1;
    }

    /// <summary>
    /// 驗證自己的資料是否正確.
    /// </summary>
    /// <returns> 空字串表示沒有問題. </returns>
    public string verify()
    {
        if(ID <= 0)
            return String.Format("ID:{0}, ID must be great than zero.", ID);

        if(Rewards == null || Rewards.Length != 5)
            return string.Format("ID:{0}, Rewards must be equal 5.", ID);

        if(RewardRates == null || RewardRates.Length != 5)
            return string.Format("ID:{0}, RewardRates must be equal 5.", ID);

        return String.Empty;
    }

	public bool IsTutorial {
		get {return Chapter == 0;}
	}

	public bool IsOnlineFriend {
		get {return FriendKind == 1 || FriendKind == 2;}
	}

    public override string ToString()
    {
        return String.Format("ID: {0}, Chapter: {1}, Order: {2}", ID, Chapter, Order);
    }

    public int[] HintBit
    {
        get
        {
			int minLength = 4;
			if (hintBit != null) {
				if (hintBit.Length == 0) {
					hintBit = AI.BitConverter.Convert(Hint);
					if (hintBit == null)
						hintBit = new int[minLength];
				}

				if (hintBit.Length < minLength)
					Array.Resize(ref hintBit, minLength);
			} else
				hintBit = new int[minLength];

			return hintBit;
        }
    }

	public int[] BitNum
	{
		get
		{
			bitNum[0] = Bit0Num;
			bitNum[1] = Bit1Num;
			bitNum[2] = Bit2Num;
			bitNum[3] = Bit3Num;
			return bitNum;
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

    /// <summary>
    /// <para> 這是 Stage.Kind 參數的對照表. [Key:Kind, Value:TextConst Index]</para>
    /// <para> 目前的規則是對照表找出的數值, 就是關卡的圖片, 也就是該關卡的類型文字(比如:傳統, 計時賽等等). </para>
    /// </summary>
    private readonly Dictionary<int, int> mStageKindMapping = new Dictionary<int, int>
    {
        {1, 2000001},
        {2, 2000002},
        {3, 2000003},
        {4, 2000004},
        {9, 2000009}
    };
}