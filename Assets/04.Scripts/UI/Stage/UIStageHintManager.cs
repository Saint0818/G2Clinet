
public class UIStageHintManager{

	public static int UpdateHint (int stageID, ref UIStageHintTarget[] mTargets) {

		int hintIndex = 0;
		hideAllTargets(mTargets);
		if(StageTable.Ins.HasByID(stageID))
		{

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
				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(1, value, 9),
					GameFunction.GetHintText(1, value, 7),
					(minute * 60 + second).ToString(), "/" + stageData.Bit0Num.ToString(),
					false);
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
				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(2, hintBits[1], 9),
					GameFunction.GetHintText(2, hintBits[1], 7),
					score.ToString(), "/" + stageData.Bit1Num.ToString(),
					false);
				hintIndex++;
			}

			if(hintBits.Length > 2 && hintBits[2] > 0)
			{
				mTargets[hintIndex].Show();
				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(3, hintBits[2], 9),
					GameFunction.GetHintText(3, hintBits[2], 7),
					getConditionCountStr(hintBits[2]), "/" + stageData.Bit2Num.ToString(),
					false);
				hintIndex++;
			}

			if(hintBits.Length > 3 && hintBits[3] > 0)
			{
				mTargets[hintIndex].Show();
				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(3, hintBits[3], 9),
					GameFunction.GetHintText(3, hintBits[3], 7),
					getConditionCountStr(hintBits[3]), "/" + stageData.Bit3Num.ToString(),
					false);
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

				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(1, value, 9),
					GameFunction.GetHintText(1, value, 7),
					(minute * 60 + second).ToString(), "/" + GameController.Get.StageData.Bit0Num.ToString(),
					false);
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
				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(2, hintBits[1], 9),
					GameFunction.GetHintText(2, hintBits[1], 7),
					score.ToString(), "/" + GameController.Get.StageData.Bit1Num.ToString(),
					false);
			}
		}

		return hintIndex;
	}


	//For GameLose
	public static int UpdateHintLose (int stageID, ref GameStageTargetLose[] mTargets) {
		int hintIndex = 0;
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
				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(1, value, 9),
					GameFunction.GetHintText(1, value, 7),
					(minute * 60 + second).ToString(), "/" + stageData.Bit0Num.ToString(),
					true,
					false);
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
				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(2, hintBits[1], 9),
					GameFunction.GetHintText(2, hintBits[1], 7),
					score.ToString(), "/" + stageData.Bit1Num.ToString(),
					isFin,
					false);

				hintIndex++;
			}

			if(hintBits.Length > 2 && hintBits[2] > 0)
			{
				mTargets[hintIndex].Show();
				isFin = (getConditionCount(hintBits[2]) >=  stageData.Bit2Num);
				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(3, hintBits[2], 9),
					GameFunction.GetHintText(3, hintBits[2], 7),
					getConditionCount(hintBits[2]).ToString(), "/" + stageData.Bit2Num.ToString(),
					isFin,
					false);
				hintIndex++;
			}

			if(hintBits.Length > 3 && hintBits[3] > 0)
			{
				mTargets[hintIndex].Show();
				isFin = (getConditionCount(hintBits[3]) >=  stageData.Bit3Num);
				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(3, hintBits[3], 9),
					GameFunction.GetHintText(3, hintBits[3], 7),
					getConditionCount(hintBits[3]).ToString(), "/" + stageData.Bit3Num.ToString(),
					isFin,
					false);
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

				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(1, value, 9),
					GameFunction.GetHintText(1, value, 7),
					(minute * 60 + second).ToString(), "/" + GameController.Get.StageData.BitNum[0].ToString(),
					true,
					false);
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
				mTargets[hintIndex].UpdateUI(GameFunction.GetHintText(2, hintBits[1], 9),
					GameFunction.GetHintText(2, hintBits[1], 7),
					UIGame.Get.Scores[ETeamKind.Self.GetHashCode()].ToString(), "/" + GameController.Get.StageData.BitNum[1].ToString(),
					isFin,
					false);
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

	private static string getConditionCountStr(int type) {
		if(GameController.Visible)
			return GameController.Get.GetSelfTeamCondition(type).ToString();
		else 
			return "";
	}

	private static int getConditionCount(int type) {
		if(GameController.Visible)
			return GameController.Get.GetSelfTeamCondition(type);
		else 
			return 0;
	}
}
