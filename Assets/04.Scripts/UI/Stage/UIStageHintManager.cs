using GameEnum;

public class UIStageHintManager{
	/*
	0: 顯示kind條件的內容
	1: 顯示達成條件的對象
	2: 目前達成數量
	*/


	/// <summary>
	/// Updates the hint normal. Use No. 8 （無分子分母）
	/// </summary>
	/// <returns>The hint normal.</returns>
	/// <param name="stageID">Stage I.</param>
	/// <param name="mTargets">M targets.</param>
	public static int UpdateHintNormal (int stageID, ref UIStageHintTarget[] mTargets) {

		int hintIndex = 0;
		string describe = "";
		hideAllTargets(mTargets);
		if(StageTable.Ins.HasByID(stageID)) {
			TStageData stageData = StageTable.Ins.GetByID(stageID);
			int[] hintBits = stageData.HintBit;
			if(hintBits.Length > 0 && hintBits[0] > 0){
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;
				
				describe = string.Format (GameFunction.GetHintText(1, value, 8), stageData.Bit0Num);
				mTargets[hintIndex].UpdateUI(describe, false);
				hintIndex ++;
			}

			if(hintBits.Length > 1 && hintBits[1] > 0)
			{
				mTargets[hintIndex].Show();
				describe = string.Format (GameFunction.GetHintText(2, hintBits[1], 8), stageData.Bit1Num);
				mTargets[hintIndex].UpdateUI(describe, false);
				hintIndex++;
			}

			if(hintBits.Length > 2 && hintBits[2] > 0)
			{
				mTargets[hintIndex].Show();
				describe = string.Format (GameFunction.GetHintText(3, hintBits[2], 8), stageData.Bit2Num);
				mTargets[hintIndex].UpdateUI(describe, false);
				hintIndex++;
			}

			if(hintBits.Length > 3 && hintBits[3] > 0)
			{
				mTargets[hintIndex].Show();
				describe = string.Format (GameFunction.GetHintText(3, hintBits[3], 8), stageData.Bit3Num);
				mTargets[hintIndex].UpdateUI(describe, false);
			}
		} else 
		{
			int[] hintBits = GameController.Get.StageData.HintBit;
			hintIndex = 0;

			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;

				describe = string.Format (GameFunction.GetHintText(1, value, 8), GameStart.Get.GameWinTimeValue);
				mTargets[hintIndex].UpdateUI(describe, false);
				hintIndex++;
			}

			if(hintBits.Length > 1 && hintBits[1] > 0)
			{
				mTargets[hintIndex].Show();
				describe = string.Format (GameFunction.GetHintText(2, hintBits[1], 8), GameStart.Get.GameWinValue);
				mTargets[hintIndex].UpdateUI(describe, false);
			}
		}

		return hintIndex;
	}

	/// <summary>
	/// Updates the hint result. Use No. 7 （有分子分母）
	/// </summary>
	/// <returns>The hint result.</returns>
	/// <param name="stageID">Stage I.</param>
	/// <param name="mTargets">M targets.</param>
	public static int UpdateHintResult (int stageID, ref UIStageHintTarget[] mTargets) {

		int hintIndex = 0;
		string describe = "";
		hideAllTargets(mTargets);
		if(StageTable.Ins.HasByID(stageID)){
			TStageData stageData = StageTable.Ins.GetByID(stageID);
			int[] hintBits = stageData.HintBit;
			int minute = (int) (GameController.Get.GameTime / 60f);
			int second = (int) (GameController.Get.GameTime % 60f);

			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;
				
				describe = string.Format (GameFunction.GetHintText(1, value, 7), stageData.Bit0Num, "", (minute * 60 + second));
				mTargets[hintIndex].UpdateUI(describe, false);
				hintIndex ++;
			}

			if(hintBits.Length > 1 && hintBits[1] > 0)
			{
				mTargets[hintIndex].Show();
				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];

				if(hintBits[1] == 2) {
				} else if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
				} else if(hintBits[1] == 4) {
					score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
				}

				describe = string.Format (GameFunction.GetHintText(2, hintBits[1], 7), stageData.Bit1Num, "", score);
				mTargets[hintIndex].UpdateUI(describe, false);
				hintIndex++;
			}

			if(hintBits.Length > 2 && hintBits[2] > 0)
			{
				mTargets[hintIndex].Show();
				describe = string.Format (GameFunction.GetHintText(3, hintBits[2], 7), stageData.Bit2Num, "", getConditionCount(hintBits[2]));
				mTargets[hintIndex].UpdateUI(describe, false);
				hintIndex++;
			}

			if(hintBits.Length > 3 && hintBits[3] > 0)
			{
				mTargets[hintIndex].Show();
				describe = string.Format (GameFunction.GetHintText(3, hintBits[3], 7), stageData.Bit3Num, "", getConditionCount(hintBits[3]));
				mTargets[hintIndex].UpdateUI(describe, false);
			}
		} else 
		{
			int[] hintBits = GameController.Get.StageData.HintBit;
			hintIndex = 0;

			int minute = (int) (GameController.Get.GameTime / 60f);
			int second = (int) (GameController.Get.GameTime % 60f);

			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;
				describe = string.Format (GameFunction.GetHintText(1, value, 7), GameController.Get.StageData.Bit0Num, "", (minute * 60 + second));
				mTargets[hintIndex].UpdateUI(describe, false);
				hintIndex++;
			}

			if(hintBits.Length > 1 && hintBits[1] > 0)
			{
				mTargets[hintIndex].Show();

				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];

				if(hintBits[1] == 2) {
				} else if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
				} else if(hintBits[1] == 4) {
					score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
				}
				describe = string.Format (GameFunction.GetHintText(2, hintBits[1], 7), GameController.Get.StageData.Bit1Num, "", score);
				mTargets[hintIndex].UpdateUI(describe, false);
			}
		}

		return hintIndex;
	}

	/// <summary>
	/// Updates the hint in game. Use No. 9 （有分子分母）
	/// </summary>
	/// <returns>The hint in game.</returns>
	/// <param name="stageID">Stage I.</param>
	/// <param name="mTargets">M targets.</param>

	public static int UpdateHintInGame (int stageID, ref UIStageHintTarget[] mTargets) {

		int hintIndex = 0;
		string describe = "";
		bool isFin = false;
		hideAllTargets(mTargets);
		if(StageTable.Ins.HasByID(stageID)) {
			TStageData stageData = StageTable.Ins.GetByID(stageID);
			int[] hintBits = stageData.HintBit;
			int minute = (int) (GameController.Get.GameTime / 60f);
			int second = (int) (GameController.Get.GameTime % 60f);
			if(hintBits.Length > 0 && hintBits[0] > 0){
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;

				describe = string.Format (GameFunction.GetHintText(1, value, 9), stageData.Bit0Num, "", (minute * 60 + second));
				mTargets[hintIndex].UpdateUI(describe, true);
				hintIndex ++;
			}

			if(hintBits.Length > 1 && hintBits[1] > 0)
			{
				mTargets[hintIndex].Show();
				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];

				isFin = (UIGame.Get.Scores[(int) ETeamKind.Self] > UIGame.Get.Scores[(int) ETeamKind.Npc]);
				if(hintBits[1] == 2){
					isFin = (score >= stageData.Bit1Num);
				}else if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
					isFin = (score < stageData.Bit1Num);
				} else if(hintBits[1] == 4) {
					score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
					isFin = (score >= stageData.Bit1Num);
				}
				describe = string.Format (GameFunction.GetHintText(2, hintBits[1], 9), stageData.Bit1Num, "", score);
				mTargets[hintIndex].UpdateUI(describe, isFin);
				hintIndex++;
			}

			if(hintBits.Length > 2 && hintBits[2] > 0)
			{
				mTargets[hintIndex].Show();
				isFin = (getConditionCount(hintBits[2]) >=  stageData.Bit2Num);
				describe = string.Format (GameFunction.GetHintText(3, hintBits[2], 9), stageData.Bit2Num, "", getConditionCount(hintBits[2]));
				mTargets[hintIndex].UpdateUI(describe, isFin);
				hintIndex++;
			}

			if(hintBits.Length > 3 && hintBits[3] > 0)
			{
				mTargets[hintIndex].Show();
				isFin = (getConditionCount(hintBits[3]) >=  stageData.Bit3Num);
				describe = string.Format (GameFunction.GetHintText(3, hintBits[3], 9), stageData.Bit3Num);
				mTargets[hintIndex].UpdateUI(describe, isFin);
			}
		} else 
		{
			int[] hintBits = GameController.Get.StageData.HintBit;
			int minute = (int) (GameController.Get.GameTime / 60f);
			int second = (int) (GameController.Get.GameTime % 60f);
			hintIndex = 0;

			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;

				describe = string.Format (GameFunction.GetHintText(1, value, 9), GameController.Get.StageData.Bit0Num, "", (minute * 60 + second));
				mTargets[hintIndex].UpdateUI(describe, true);
				hintIndex++;
			}

			if(hintBits.Length > 1 && hintBits[1] > 0)
			{
				mTargets[hintIndex].Show();
				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];

				isFin = (UIGame.Get.Scores[(int) ETeamKind.Self] > UIGame.Get.Scores[(int) ETeamKind.Npc]);
				if(hintBits[1] == 2){
					isFin = (score >= GameController.Get.StageData.Bit1Num);
				}else if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
					isFin = (score < GameController.Get.StageData.Bit1Num);
				} else if(hintBits[1] == 4) {
					score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
					isFin = (score >= GameController.Get.StageData.Bit1Num);
				}
				describe = string.Format (GameFunction.GetHintText(2, hintBits[1], 9), GameController.Get.StageData.Bit1Num, "", score);
				mTargets[hintIndex].UpdateUI(describe, isFin);
			}
		}

		return hintIndex;
	}


	//For GameLose
	public static int UpdateHintLose (int stageID, ref GameStageTargetLose[] mTargets) {
		int hintIndex = 0;
		string describe = "";
		hideAllTargets(mTargets);
		if(StageTable.Ins.HasByID(stageID))
		{

			TStageData stageData = StageTable.Ins.GetByID(stageID);
			int[] hintBits = stageData.HintBit;
			hintIndex = 0;
			bool isFin = false;
			int minute = (int) (GameController.Get.GameTime / 60f);
			int second = (int) (GameController.Get.GameTime % 60f);

			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;

				describe = string.Format (GameFunction.GetHintText(1, value, 7), stageData.Bit0Num, "", (minute * 60 + second));
				mTargets[hintIndex].UpdateUI(describe, true, false);
				hintIndex ++;
			}

			if(hintBits.Length > 1 && hintBits[1] > 0)
			{
				mTargets[hintIndex].Show();
				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];

				isFin = (UIGame.Get.Scores[(int) ETeamKind.Self] > UIGame.Get.Scores[(int) ETeamKind.Npc]);
				if(hintBits[1] == 2){
					isFin = (score >= stageData.Bit1Num);
				}else if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
					isFin = (score < stageData.Bit1Num);
				} else if(hintBits[1] == 4) {
					score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
					isFin = (score >= stageData.Bit1Num);
				}
				describe = string.Format (GameFunction.GetHintText(2, hintBits[1], 7), stageData.Bit1Num, "", score);
				mTargets[hintIndex].UpdateUI(describe, isFin, false);

				hintIndex++;
			}

			if(hintBits.Length > 2 && hintBits[2] > 0)
			{
				mTargets[hintIndex].Show();
				isFin = (getConditionCount(hintBits[2]) >=  stageData.Bit2Num);
				describe = string.Format (GameFunction.GetHintText(3, hintBits[2], 7), stageData.Bit2Num, "", getConditionCount(hintBits[2]));
				mTargets[hintIndex].UpdateUI(describe, isFin, false);
				hintIndex++;
			}

			if(hintBits.Length > 3 && hintBits[3] > 0)
			{
				mTargets[hintIndex].Show();
				isFin = (getConditionCount(hintBits[3]) >=  stageData.Bit3Num);
				describe = string.Format (GameFunction.GetHintText(3, hintBits[3], 7), stageData.Bit3Num, "", getConditionCount(hintBits[3]));
				mTargets[hintIndex].UpdateUI(describe, isFin, false);
				hintIndex++;
			}
		} else 
		{
			int minute = (int) (GameController.Get.GameTime / 60f);
			int second = (int) (GameController.Get.GameTime % 60f);
			int[] hintBits = GameController.Get.StageData.HintBit;
			hintIndex = 0;
			bool isFin = false;
			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[hintIndex].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;
				
				describe = string.Format (GameFunction.GetHintText(1, value, 7), GameController.Get.StageData.Bit0Num, "", (minute * 60 + second));
				mTargets[hintIndex].UpdateUI(describe, true, false);
				hintIndex++;
			}

			if(hintBits.Length > 1 && hintBits[1] > 1)
			{
				mTargets[hintIndex].Show();
				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];
				isFin = (score >= GameController.Get.StageData.Bit1Num);
				if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
					isFin = (score <= GameController.Get.StageData.Bit1Num);
				} else {
					if(hintBits[1] == 4)
						score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
					isFin = (score >= GameController.Get.StageData.Bit1Num);
				}
				describe = string.Format (GameFunction.GetHintText(2, hintBits[1], 7), GameController.Get.StageData.Bit1Num, "", score);
				mTargets[hintIndex].UpdateUI(describe, isFin, false);
				hintIndex ++;
			}
		}

		return hintIndex;
	}

	private static void hideAllTargets(UIStageHintTarget[] target)
	{
		for(int i = 0; i < target.Length; i++)
		{
			target[i].Hide();
		}
	}

	private static void hideAllTargets(GameStageTargetLose[] target)
	{
		for(int i = 0; i < target.Length; i++)
		{
			target[i].Hide();
		}
	}

	private static int getConditionCount(int type) {
		if(GameController.Visible)
			return MissionChecker.Get.GetSelfTeamCondition(type);
		else 
			return 0;
	}
}
