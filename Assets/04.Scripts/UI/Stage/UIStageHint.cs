using JetBrains.Annotations;
using UnityEngine;

public class UIStageHint : MonoBehaviour
{
    public GameObject Window;

    private UIStageHintTarget[] mTargets;

    [UsedImplicitly]
    private void Awake()
    {
        mTargets = GetComponentsInChildren<UIStageHintTarget>();
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
        
        StageData stageData = StageTable.Ins.GetByID(stageID);
        int[] hintBits = stageData.HintBit;
        int index = 0;
        if(hintBits.Length > 0 && hintBits[0] > 0)
        {
            mTargets[index].Show();
            mTargets[index].UpdateUI(getText(index + 1, hintBits[0], 9),
                getText(index + 1, hintBits[0], 8),
                stageData.Bit0Num.ToString(), "/" + stageData.Bit0Num);
            // 左邊的文字
//            gameTargets[index].LabelCaption.text = ;
            // 中間的文字
//            gameTargets[index].LabelTargetName.text = ;
            // 下面的文字.
//            gameTargets[index].LabelTargetValue.text = "/" + GameController.Get.StageBitNum[0];
            // 上面的文字.
//            gameTargets[index].LabelValue.text = GameController.Get.GameTime.ToString();
            index++;
        }
        
//        if(GameController.Get.StageHintBit.Length > 1 && GameController.Get.StageHintBit[1] > 0)
        if(hintBits.Length > 1 && hintBits[1] > 0)
        {
            mTargets[index].Show();
            mTargets[index].UpdateUI(getText(index + 1, hintBits[1], 9),
                getText(index + 1, hintBits[1], 8),
                stageData.Bit1Num.ToString(), "/" + stageData.Bit1Num);

//            gameTargets[index].Self.SetActive(true);
//            gameTargets[index].LabelCaption.text = getText(index + 1, GameController.Get.StageHintBit[1], 8);
//            gameTargets[index].LabelTargetName.text = getText(index + 1, GameController.Get.StageHintBit[1], 9);
//            gameTargets[index].LabelTargetValue.text = "/" + GameController.Get.StageBitNum[1];
//            gameTargets[index].LabelValue.text = UIGame.Get.Scores[(int)ETeamKind.Self].ToString();
            index++;
        }
        
        if(hintBits.Length > 2 && hintBits[2] > 0)
        {
            mTargets[index].Show();
            mTargets[index].UpdateUI(getText(3, hintBits[2], 9),
                getText(3, hintBits[2], 8),
                stageData.Bit2Num.ToString(), "/" + stageData.Bit2Num);
//            gameTargets[index].Self.SetActive(true);
//            gameTargets[index].LabelCaption.text = getText(3, GameController.Get.StageHintBit[2], 8);
//            gameTargets[index].LabelTargetName.text = getText(3, GameController.Get.StageHintBit[2], 9);
//            gameTargets[index].LabelTargetValue.text = "/" + GameController.Get.StageBitNum[2];
//            gameTargets[index].LabelValue.text = getConditionCount(GameController.Get.StageHintBit[2]).ToString();
            index++;
        }
        
        if(hintBits.Length > 3 && hintBits[3] > 0)
        {
            mTargets[index].Show();
            mTargets[index].UpdateUI(getText(3, hintBits[3], 9),
                getText(3, hintBits[3], 8),
                hintBits[3].ToString(), "/" + stageData.Bit3Num);
            //            gameTargets[index].Self.SetActive(true);
            //            gameTargets[index].LabelCaption.text = getText(3, GameController.Get.StageHintBit[3], 8);
            //            gameTargets[index].LabelTargetName.text = getText(3, GameController.Get.StageHintBit[3], 9);
            //            gameTargets[index].LabelTargetValue.text = "/" + GameController.Get.StageBitNum[3];
            //            gameTargets[index].LabelValue.text = getConditionCount(GameController.Get.StageHintBit[3]).ToString();
            //            index++;
        }
    }

	public void UpdateValue(int stageID)
	{
		if(!StageTable.Ins.HasByID(stageID))
			return;
		
		hideAllTargets();
		
		StageData stageData = StageTable.Ins.GetByID(stageID);
		int[] hintBits = stageData.HintBit;
		int index = 0;
		if(hintBits.Length > 0 && hintBits[0] > 0)
		{
			mTargets[index].Show();
			mTargets[index].UpdateUI(getText(index + 1, hintBits[0], 9),
			                         getText(index + 1, hintBits[0], 8),
			                         GameController.Get.GameTime.ToString(), "/" + stageData.Bit0Num,
			                         (GameController.Get.GameTime <= 0));
			index++;
		}

		if(hintBits.Length > 1 && hintBits[1] > 0)
		{
			mTargets[index].Show();
			int team = (int) ETeamKind.Self;
			int score = UIGame.Get.Scores[team];
			bool isFin = (score >= stageData.Bit1Num);
			if(hintBits[1] == 2){
				team = (int) ETeamKind.Npc;
				score = UIGame.Get.Scores[team];
				isFin = (score <= stageData.Bit1Num);
			} else {
			if(hintBits[1] == 3)
				score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
				isFin = (score >= stageData.Bit1Num);
			}

			mTargets[index].UpdateUI(getText(index + 1, hintBits[1], 9),
			                         getText(index + 1, hintBits[1], 8),
			                         score.ToString(), "/" + stageData.Bit1Num,
			                         isFin);
			index++;
		}
		
		if(hintBits.Length > 2 && hintBits[2] > 0)
		{
			bool isFin = (getConditionCount(hintBits[2]) >=  stageData.Bit2Num);
			mTargets[index].Show();
			mTargets[index].UpdateUI(getText(3, hintBits[2], 9),
			                         getText(3, hintBits[2], 8),
			                         getConditionCount(hintBits[2]).ToString(), "/" + stageData.Bit2Num,
			                         isFin);
			index++;
		}
		
		if(hintBits.Length > 3 && hintBits[3] > 0)
		{
			bool isFin = (getConditionCount(hintBits[3]) >=  stageData.Bit3Num);
			mTargets[index].Show();
			mTargets[index].UpdateUI(getText(3, hintBits[3], 9),
			                         getText(3, hintBits[3], 8),
			                         getConditionCount(hintBits[3]).ToString(), "/" + stageData.Bit3Num,
			                         isFin);
		}
	}

    private string getText(int index, int value, int id)
    {
//        return string.Empty;
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
		switch (type){
		case 1://two score
			return GameController.Get.Joysticker.GameRecord.FGIn;
		case 2://three score
			return GameController.Get.Joysticker.GameRecord.FG3In;
		case 3://dunk
			return GameController.Get.Joysticker.GameRecord.Dunk;
		case 4://push
			return GameController.Get.Joysticker.GameRecord.Push;
		case 5://steal
			return GameController.Get.Joysticker.GameRecord.Steal;
		case 6://block
			return GameController.Get.Joysticker.GameRecord.Block;
		}
		return 0;
	}
}