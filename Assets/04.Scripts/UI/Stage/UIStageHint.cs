using JetBrains.Annotations;
using UnityEngine;

public class UIStageHint : MonoBehaviour
{
    public GameObject Window;
	public UIAnchor WindowAnchor;

    private UIStageHintTarget[] mTargets;

    [UsedImplicitly]
    private void Awake()
    {
        mTargets = GetComponentsInChildren<UIStageHintTarget>();
    }

	public void Init () {
		for (int i=0 ;i<mTargets.Length; i++) {
			mTargets[i].MoveCurrentLabel();
			mTargets[i].MoveGoalLabel();
		}
	}

    /// <summary>
    /// 設定目標的間隔.
    /// </summary>
    /// <param name="yInterval"> 單位: Pixel. </param>
    public void SetInterval(float yInterval)
    {
        // 因為介面的配置是左邊有 3 個目標, 右邊有 3 個目標, 所以才有以下的魔術數字 3 和 6.
        float y = 0;
        for (int i = 0; i < 3; i++)
        {
            var localPos = mTargets[i].transform.localPosition;
            localPos.y -= y;
            mTargets[i].transform.localPosition = localPos;

            y -= yInterval;
        }

        y = 0;
        for (int i = 3; i < 6; i++)
        {
            var localPos = mTargets[i].transform.localPosition;
            localPos.y -= y;
            mTargets[i].transform.localPosition = localPos;

            y -= yInterval;
        }
    }

	public void SetInterval(float startY, float yInterval)
	{
		// 因為介面的配置是左邊有 3 個目標, 右邊有 3 個目標, 所以才有以下的魔術數字 3 和 6.
		float y = 0;
		for (int i = 0; i < 3; i++)
		{
			var localPos = mTargets[i].transform.localPosition;
			if(i == 0) {
				y = startY;
				localPos.y = startY;
			} else
				localPos.y = y;
			mTargets[i].transform.localPosition = localPos;
			
			y -= yInterval;
		}
		
		y = 0;
		for (int i = 3; i < 6; i++)
		{
			var localPos = mTargets[i].transform.localPosition;
			if(i == 3) {
				y = startY;
				localPos.y = startY;
			} else
				localPos.y = y;
			mTargets[i].transform.localPosition = localPos;
			
			y -= yInterval;
		}
	}

    public void Show()
    {
        Window.SetActive(true);
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    public void UpdateUI(int stageID)
    {
        if(!StageTable.Ins.HasByID(stageID))
            return;

        hideAllTargets();
        
        TStageData stageData = StageTable.Ins.GetByID(stageID);
        int[] hintBits = stageData.HintBit;
        int index = 0;
        if(hintBits.Length > 0 && hintBits[0] > 0)
        {
			mTargets[index].Show();
			int value = 0;
			if(hintBits[0] == 1 || hintBits[0] == 2)
				value = 1;
			mTargets[index].UpdateUI(getText(index + 1, value, 9),
			                         getText(index + 1, value, 8),
//                "/" + stageData.Bit0Num, stageData.Bit0Num.ToString());
                "", stageData.Bit0Num.ToString());
            index++;
        }
        
        if(hintBits.Length > 1 && hintBits[1] > 0)
        {
            mTargets[index].Show();
            mTargets[index].UpdateUI(getText(index + 1, hintBits[1], 9),
                getText(index + 1, hintBits[1], 8),
//                "/" + stageData.Bit1Num, stageData.Bit1Num.ToString());
                "", stageData.Bit1Num.ToString());

            index++;
        }
        
        if(hintBits.Length > 2 && hintBits[2] > 0)
        {
            mTargets[index].Show();
            mTargets[index].UpdateUI(getText(3, hintBits[2], 9),
                getText(3, hintBits[2], 8),
//                "/" + stageData.Bit2Num, stageData.Bit2Num.ToString());
                "", stageData.Bit2Num.ToString());
            index++;
        }
        
        if(hintBits.Length > 3 && hintBits[3] > 0)
        {
            mTargets[index].Show();
            mTargets[index].UpdateUI(getText(3, hintBits[3], 9),
                getText(3, hintBits[3], 8),
//                "/" + hintBits[3],  stageData.Bit3Num.ToString());
                "",  stageData.Bit3Num.ToString());
        }
    }

	public void UpdateValue(int stageID)
	{
		if(StageTable.Ins.HasByID(stageID))
		{
			hideAllTargets();
			
			TStageData stageData = StageTable.Ins.GetByID(stageID);
			int[] hintBits = stageData.HintBit;
			int index = 0;

			int minute = (int) (GameController.Get.GameTime / 60f);
			int second = (int) (GameController.Get.GameTime % 60f);

			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[index].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;
				mTargets[index].UpdateUI(getText(1, value, 9),
				                         getText(1, value, 8),
				                         (minute * 60 + second).ToString(), "/" + stageData.Bit0Num.ToString(),
				                         (GameController.Get.GameTime <= 0));
				index ++;
			}
			
			if(hintBits.Length > 1 && hintBits[1] > 0)
			{
				mTargets[index].Show();
				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];
				bool isFin = (UIGame.Get.Scores[(int) ETeamKind.Self] > UIGame.Get.Scores[(int) ETeamKind.Npc]);
				if(hintBits[1] == 2){
					isFin = (score >= stageData.Bit1Num);
				} else if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
					isFin = (score <= stageData.Bit1Num);
				} else if(hintBits[1] == 4){
					score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
					isFin = (score >= stageData.Bit1Num);
				}

				mTargets[index].UpdateUI(getText(2, hintBits[1], 9),
				                         getText(2, hintBits[1], 8),
				                         score.ToString(), "/" + stageData.Bit1Num.ToString(),
				                         isFin);
				index++;
			}
			
			if(hintBits.Length > 2 && hintBits[2] > 0)
			{
				bool isFin = (getConditionCount(hintBits[2]) >=  stageData.Bit2Num);
				mTargets[index].Show();
				mTargets[index].UpdateUI(getText(3, hintBits[2], 9),
				                         getText(3, hintBits[2], 8),
				                         getConditionCount(hintBits[2]).ToString(), "/" + stageData.Bit2Num.ToString(),
				                         isFin);
				index++;
			}
			
			if(hintBits.Length > 3 && hintBits[3] > 0)
			{
				bool isFin = (getConditionCount(hintBits[3]) >=  stageData.Bit3Num);
				mTargets[index].Show();
				mTargets[index].UpdateUI(getText(3, hintBits[3], 9),
				                         getText(3, hintBits[3], 8),
				                         getConditionCount(hintBits[3]).ToString(), "/" + stageData.Bit3Num.ToString(),
				                         isFin);
			}
		} else 
		{
			int[] hintBits = GameController.Get.StageData.HintBit;
			int index = 0;
			if(hintBits.Length > 0 && hintBits[0] > 0)
			{
				mTargets[index].Show();
				int value = 0;
				if(hintBits[0] == 1 || hintBits[0] == 2)
					value = 1;

				mTargets[index].UpdateUI(getText(1, value, 9),
				                         getText(1, value, 8),
				                         (Mathf.RoundToInt(GameController.Get.GameTime)).ToString(), "/" + GameController.Get.StageData.BitNum[0].ToString(),
				                         (GameController.Get.GameTime <= 0));
				index++;
			}
			
			if(hintBits.Length > 1 && hintBits[1] > 1)
			{
				mTargets[index].Show();
				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];
				bool isFin = (score >= GameController.Get.StageData.BitNum[1]);
				if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
					isFin = (score <= GameController.Get.StageData.BitNum[1]);
				} else {
					if(hintBits[1] == 4)
						score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
					isFin = (score >= GameController.Get.StageData.BitNum[1]);
				}

				mTargets[index].UpdateUI(getText(2, hintBits[1], 9),
				                         getText(2, hintBits[1], 8),
				                         score.ToString(), "/" + GameController.Get.StageData.BitNum[1].ToString(),
				                         isFin);
			}
		}
	}

    private string getText(int index, int value, int id)
    {
        int baseValue = 2000000 + (int)(Mathf.Pow(10,index) * value) + id;
        return TextConst.S(baseValue);
    }

    private void hideAllTargets()
    {
        for(int i = 0; i < mTargets.Length; i++)
        {
            mTargets[i].Hide();
        }
    }

	private int getConditionCount(int type){
		return GameController.Get.GetSelfTeamCondition(type);
	}
}