using System;
using System.Collections.Generic;
using GameEnum;
using JetBrains.Annotations;

/// <summary>
/// 某個小關卡的相關資料.
/// </summary>
public class TStageData
{
	/// <summary>
	/// PVP ID 的範圍.
	/// </summary>
	public const int MinPVPStageID = 10;
	public const int MaxPVPStageID = 100;

    /// <summary>
    /// 主線關卡 ID 的範圍.
    /// </summary>
    public const int MinMainStageID = 101;
    public const int MaxMainStageID = 2000;
    public static bool IsMainStage(int stageID)
    {
        return MinMainStageID <= stageID && stageID <= MaxMainStageID;
    }

    /// <summary>
    /// 副本的 ID 範圍.
    /// </summary>
    public const int MinInstanceID = 2001;
    public const int MaxInstanceID = 4000;
    public static bool IsInstance(int stageID)
    {
        return MinInstanceID <= stageID && stageID <= MaxInstanceID;
    }

    public enum EKind
    {
        Undefined,
        MainStage, // 主線關卡.
        Instance, // 副本.
		PVP
    }

    public int ID { get; private set; }

    public EKind IDKind
    {
        get
        {
			if (MinPVPStageID <= ID && ID <= MaxPVPStageID)
				return EKind.PVP;				
            if(MinMainStageID <= ID && ID <= MaxMainStageID)
                return EKind.MainStage;
            if(MinInstanceID <= ID && ID <= MaxInstanceID)
                return EKind.Instance;
            return EKind.Undefined;
        }
    }

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

	/// <summary>
	/// 玩家本身該有的AI等級
	/// </summary>
	public int PlayerAI;

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
    /// 球員每日關卡挑戰次數.
    /// </summary>
    public int DailyChallengeNum { get; private set; }

    /// <summary>
    /// true: 關卡只可以挑戰 1 次. false: 關卡可以挑戰無限次.
    /// </summary>
    public bool ChallengeOnlyOnce { get; private set; }

    public int Diamond { get; private set; }
    public int Exp { get; private set; }
    public int Money { get; private set; }

    public bool HasSurelyRewards(EPlayerPostion pos)
    {
        switch(pos)
        {
            case EPlayerPostion.C:
                return SurelyRewardsC != null && SurelyRewardsC.Length > 0;
            case EPlayerPostion.F:
                return SurelyRewardsF != null && SurelyRewardsF.Length > 0;
            case EPlayerPostion.G:
                return SurelyRewardsG != null && SurelyRewardsG.Length > 0;
        }

        throw new NotImplementedException(string.Format("Position:{0}", pos));
    }

    public int[] GetSurelyRewards(EPlayerPostion pos)
    {
        switch (pos)
        {
            case EPlayerPostion.C:
                return SurelyRewardsC;
            case EPlayerPostion.F:
                return SurelyRewardsF;
            case EPlayerPostion.G:
                return SurelyRewardsG;
        }

        throw new NotImplementedException(string.Format("Position:{0}", pos));
    }

    /// <summary>
    /// 必給獎勵.
    /// </summary>
    [UsedImplicitly, CanBeNull]
    public int[] SurelyRewardsC { get; private set; } // 獎勵 ItemID.

    [UsedImplicitly, CanBeNull]
    public int[] SurelyRewardsF { get; private set; } // 獎勵 ItemID.

    [UsedImplicitly, CanBeNull]
    public int[] SurelyRewardsG { get; private set; } // 獎勵 ItemID.

    /// <summary>
    /// 亂數獎勵.
    /// </summary>
    [UsedImplicitly, CanBeNull]
    public int[] Rewards { get; private set; } // 獎勵 ItemID.

    [UsedImplicitly, CanBeNull]
    public int[] RewardRates { get; private set; } // 獎勵機率.

    public int[] Tips;
    public int WinValue;
    public int FriendNumber { get; private set; }

    /// <summary>
    /// 小關卡在章節上的位置.
    /// </summary>
    public float PositionX { get; private set; }
    public float PositionY { get; private set; }

    [CanBeNull]
    public int[] PlayerID;
    public int[] FriendID;

    private string nameTW;
    private string nameCN;
    private string nameEN;
    private string nameJP;
    private string explainTW;
    private string explainCN;
    private string explainEN;
    private string explainJP;

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
		DailyChallengeNum = 0;
        ChallengeOnlyOnce = false;
		WinValue = 0;
		FriendNumber = 0;
		PositionX = 0;
		PositionY = 0;

        Diamond = 0;
        Exp = 0;
        Money = 0;
		
		Hint = "";
		nameTW = "";
		nameCN = "";
		nameEN = "";
		nameJP = "";
		explainTW = "";
		explainCN = "";
		explainEN = "";
		explainJP = "";

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

	public bool IsTutorial {
        get {return ID < 10;}
	}

	public bool IsOnlineFriend {
        get {return FriendKind == 1 || FriendKind == 2 || FriendKind == 3;}
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
                case ELanguage.TW: return nameTW;
                case ELanguage.CN: return nameCN;
                case ELanguage.JP: return nameJP;
                default : return nameEN;
            }
        }
    }

    public string Explain
    {
        get
        {
            switch(GameData.Setting.Language)
            {
                case ELanguage.TW: return explainTW;
                case ELanguage.CN: return explainCN;
                case ELanguage.JP: return explainJP;
                default : return explainEN;
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