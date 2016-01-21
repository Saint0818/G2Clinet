using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameEnum;

public struct TCourtInstant {
	public bool[] TimeInstant;
	public bool[] ScoreInstant;
	public bool[] Condition1Instant;
	public bool[] Condition2Instant;
	public TCourtInstant(int i) {
		TimeInstant = new bool[3];
		ScoreInstant = new bool[3];
		Condition1Instant = new bool[3];
		Condition2Instant = new bool[3];
	}

	public void ResetValue () {
		for (int i=0; i<TimeInstant.Length; i++) {
			TimeInstant[i] = false;
			ScoreInstant[i] = false;
			Condition1Instant[i] = false;
			Condition2Instant[i] = false;
		}
	}
}

public class MissionChecker {
	private static readonly MissionChecker INSTANCE = new MissionChecker();
	public static MissionChecker Get
	{
		get { return INSTANCE; }
	}

	public TCourtInstant CourtInstant;

	private PlayerBehaviour mPlayer;
	private TStageData mStageData;
	private float maxGameTime ;

	public void Init (TStageData stageData) {
		CourtInstant = new TCourtInstant(1);
		mStageData = stageData;
		maxGameTime = stageData.BitNum[0];
	}
	public void SetPlayer (PlayerBehaviour player) {
		mPlayer = player;
	}

	public void Reset () {
		CourtInstant.ResetValue();
	}

	public float MaxGameTime {
		get {return maxGameTime;}
	}

	public void CheckConditionText (int id)
	{
		if(StageTable.Ins.HasByID(id))
		{
			if(mStageData.HintBit[1] > 1) {
				if(mStageData.HintBit[1] == 2) { //Player get score
					if(!CourtInstant.ScoreInstant[0] && (UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] >= mStageData.BitNum[1]) ){
						showCourtInstant(2, mStageData.HintBit[1], 0, UIGame.Get.Scores[ETeamKind.Self.GetHashCode()]);
						CourtInstant.ScoreInstant[0] = true;
					}
					if(!CourtInstant.ScoreInstant[1] && (UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] >= mStageData.BitNum[1] * 0.5f) ){
						showCourtInstant(2, mStageData.HintBit[1], 1, (mStageData.BitNum[1] - UIGame.Get.Scores[ETeamKind.Self.GetHashCode()]));
						CourtInstant.ScoreInstant[1] = true;
					}
					if(!CourtInstant.ScoreInstant[2] && (UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] >= mStageData.BitNum[1] * 0.9f)) {
						if((mStageData.BitNum[1] - UIGame.Get.Scores[ETeamKind.Self.GetHashCode()]) > 0) {
							showCourtInstant(2, mStageData.HintBit[1], 2, (mStageData.BitNum[1] - UIGame.Get.Scores[ETeamKind.Self.GetHashCode()]));
							CourtInstant.ScoreInstant[2] = true;
						}
					}
				} else if(mStageData.HintBit[1] == 3) { //Enemy get score
					if(!CourtInstant.ScoreInstant[1] && (UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()] >= mStageData.BitNum[1] * 0.5f) ){
						showCourtInstant(2, mStageData.HintBit[1], 1, (mStageData.BitNum[1] - UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()]));
						CourtInstant.ScoreInstant[1] = true;
					}
					if(!CourtInstant.ScoreInstant[2] && (UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()] >= mStageData.BitNum[1] * 0.9f)) {
						if((mStageData.BitNum[1] - UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()]) > 0) {
							showCourtInstant(2, mStageData.HintBit[1], 2, (mStageData.BitNum[1] - UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()]));
							CourtInstant.ScoreInstant[2] = true;
						}
					}
				} else if(mStageData.HintBit[1] == 4) { //Player Score - Enemy Score
					if(!CourtInstant.ScoreInstant[1] && ((UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] - UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()]) >= mStageData.BitNum[1] * 0.5f) ){
						showCourtInstant(2, mStageData.HintBit[1], 1, mStageData.BitNum[1] - (UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] - UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()]));
						CourtInstant.ScoreInstant[1] = true;
					}
					if(!CourtInstant.ScoreInstant[2] && ((UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] - UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()]) >= mStageData.BitNum[1] * 0.9f)) {
						if(mStageData.BitNum[1] - (UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] - UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()]) > 0) {
							showCourtInstant(2, mStageData.HintBit[1], 2, mStageData.BitNum[1] - (UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] - UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()]));
							CourtInstant.ScoreInstant[2] = true;
						}
					}
				}
			}
			if(mStageData.HintBit[2] > 0) {
				if(checkCountEnough(mStageData.HintBit[2], mStageData.BitNum[2])) {
					if(!CourtInstant.Condition1Instant[0]) {
						if(mStageData.BitNum[2] >= 0){
							showCourtInstant(3, mStageData.HintBit[2], 0, mStageData.BitNum[2]);
							CourtInstant.Condition1Instant[0] = true;
						} 
					}
				}
				if(checkCountEnough(mStageData.HintBit[2], (int)(mStageData.BitNum[2] * 0.5f))) {
					if(!CourtInstant.Condition1Instant[1]) {
						if(mStageData.BitNum[2] * 0.5f >= 0){
							showCourtInstant(3, mStageData.HintBit[2], 1, mStageData.BitNum[2] - GetSelfTeamCondition(mStageData.HintBit[2]));
							CourtInstant.Condition1Instant[1] = true;
						} 
					}
				}
				if(checkCountEnough(mStageData.HintBit[2], (int)(mStageData.BitNum[2] * 0.1f))) {
					if(!CourtInstant.Condition1Instant[2]) {
						if(mStageData.BitNum[2] * 0.1f >= 0){
							showCourtInstant(3, mStageData.HintBit[2], 2, mStageData.BitNum[2] - GetSelfTeamCondition(mStageData.HintBit[2]));
							CourtInstant.Condition1Instant[2] = true;
						}
					}
				}
			}

			if(mStageData.HintBit[3] > 0) {
				if(checkCountEnough(mStageData.HintBit[3], mStageData.BitNum[3])) {
					if(!CourtInstant.Condition2Instant[0]) {
						if(mStageData.BitNum[3] >= 0){
							showCourtInstant(3, mStageData.HintBit[3], 0,  mStageData.BitNum[3]);
							CourtInstant.Condition2Instant[0] = true;
						}
					}
				}
				if(checkCountEnough(mStageData.HintBit[3], (int) (mStageData.BitNum[3] * 0.5f))) {
					if(!CourtInstant.Condition2Instant[1]) {
						if(mStageData.BitNum[3] * 0.5f >= 0){
							showCourtInstant(3, mStageData.HintBit[3], 1,  mStageData.BitNum[3] - GetSelfTeamCondition(mStageData.HintBit[3]));
							CourtInstant.Condition2Instant[1] = true;
						}
					}
				}
				if(checkCountEnough(mStageData.HintBit[3], (int) (mStageData.BitNum[3] * 0.1f))) {
					if(!CourtInstant.Condition2Instant[2]) {
						if(mStageData.BitNum[3] * 0.1f >= 0){
							showCourtInstant(3, mStageData.HintBit[3], 2, mStageData.BitNum[3] - GetSelfTeamCondition(mStageData.HintBit[3]));
							CourtInstant.Condition2Instant[2] = true;
						}
					}
				}
			}
		}
	}

	public bool IsTimePass(ref float gameTime) {
		gameTime -= Time.deltaTime;
		if(!CourtInstant.TimeInstant[1] && (gameTime <= maxGameTime * 0.5f)){
			showCourtInstant(1, 1, 1, (int)(maxGameTime * 0.5f));
			CourtInstant.TimeInstant[1] = true;
		}

		if(!CourtInstant.TimeInstant[2] && (gameTime <= maxGameTime * 0.1f)) {
			showCourtInstant(1, 1, 2, (int)(maxGameTime * 0.1f));
			CourtInstant.TimeInstant[2] = true;
		}

		if (gameTime <= 0) {
			gameTime = 0;
			return true;
		}
		return false;
	}

	//0: no check  1:self > enemy 2:self score 3:enemy score  4:self - enemy
	public bool IsScorePass(int team)
	{
		int self = team;
		int enemy = 0;
		if(self == (int) ETeamKind.Self)
			enemy = 1;
		if(StageTable.Ins.HasByID(mStageData.ID))
		{

			if (mStageData.HintBit[1] == 0)
				return true;
			else{ 
				if      (mStageData.HintBit[1] == 1 && (UIGame.Get.Scores[self] > UIGame.Get.Scores[enemy])) return true;
				else if (mStageData.HintBit[1] == 2 && (UIGame.Get.Scores[self] >= mStageData.WinValue)) return true;
				else if (mStageData.HintBit[1] == 3 && (UIGame.Get.Scores[enemy] <= mStageData.WinValue)) return true;
				else if (mStageData.HintBit[1] == 4 && (UIGame.Get.Scores[self] - UIGame.Get.Scores[enemy]) >= mStageData.WinValue) return true;
			}
		} else if(!GameStart.Get.ConnectToServer)
		{
			if (mStageData.HintBit[1] == 2 && (UIGame.Get.Scores[team] >= mStageData.WinValue)) return true;
		}
		return false;
	}

	public bool IsScoreFinish
	{
		get {
			if(StageTable.Ins.HasByID(mStageData.ID))
			{
				if (mStageData.HintBit[1] == 0)
					return true;
				else{ 
					if (mStageData.HintBit[1] == 2 && (UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] >= mStageData.WinValue)) return true;
					else if (mStageData.HintBit[1] == 3 && (UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()] > mStageData.WinValue)) return true;
				}
			} else if(!GameStart.Get.ConnectToServer)
			{
				if (mStageData.HintBit[1] == 2 && (UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] >= mStageData.WinValue)) return true;
			}
			return false;
		}
	}

	public bool IsConditionPass
	{
		get {
			if(StageTable.Ins.HasByID(mStageData.ID))
			{
				if(mStageData.HintBit[2] > 0) 
				if(!checkCountEnough(mStageData.HintBit[2], mStageData.BitNum[2]))
					return false;

				if(mStageData.HintBit[3] > 0) 
				if(!checkCountEnough(mStageData.HintBit[3], mStageData.BitNum[3]))
					return false;
				return true;
			} else if(!GameStart.Get.ConnectToServer)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsWinner (float gameTime) {
		if(mStageData.HintBit[0] == 0 || mStageData.HintBit[0] == 1) {
			if(IsScorePass(mPlayer.Team.GetHashCode()))
				if(IsConditionPass)
					return true;
		} else {
			if(gameTime <=0 && IsScorePass(mPlayer.Team.GetHashCode()))
			if(IsConditionPass)
				return true;
		}
		return false;
	}



	/// <summary>
	/// Index 			1:Time 2:Score 3:condition1 4:condition2//////
	/// Value 			now getValue //////
	/// Id 			    complete:0  1/2:1  1/10:2 //////
	/// VisibleValue	PlayerName
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="value">Value.</param>
	/// <param name="id">Identifier.</param>
	/// <param name="visibleValue">Visible value.</param>
	private void showCourtInstant (int index, int value, int id, int visibleValue){
		int baseValue = 2000000 + (int)(Mathf.Pow(10,index) * value) + id;
		string text = "";
		if(TextConst.S(baseValue).Contains("{1}"))
			text = string.Format(TextConst.S(baseValue), visibleValue, mPlayer.Attribute.Name);
		else 
			text = string.Format(TextConst.S(baseValue), visibleValue);
		UICourtInstant.UIShow(false);
		UICourtInstant.UIShow(true, text);
	}

	private bool checkCountEnough (int type, int count) {
		if(count > 0) {
			return (GetSelfTeamCondition(type) >= count);
		}
		return false;
	}

	public int GetSelfTeamCondition (int type) {
		int count = 0;
		for(int i=0 ;i<GameController.Get.GamePlayers.Count; i++) {
			if(GameController.Get.GamePlayers[i].Team == ETeamKind.Self) {
				switch (type){
				case 1://two score
					count += GameController.Get.GamePlayers[i].GameRecord.FGIn;
					break;
				case 2://three score
					count += GameController.Get.GamePlayers[i].GameRecord.FG3In;
					break;
				case 3://dunk
					count += GameController.Get.GamePlayers[i].GameRecord.Dunk;
					break;
				case 4://push
					count += GameController.Get.GamePlayers[i].GameRecord.Push;
					break;
				case 5://steal
					count += GameController.Get.GamePlayers[i].GameRecord.Steal;
					break;
				case 6://block
					count += GameController.Get.GamePlayers[i].GameRecord.Block;
					break;
				case 7://elbow
					count += GameController.Get.GamePlayers[i].GameRecord.Elbow;
					break;
				}
			}
		}
		return count;
	}
}
