using JetBrains.Annotations;
using UnityEngine;

public class UIStageHint2 : MonoBehaviour
{
    public GameObject Window;

    private UIStageHintTarget[] mTargets;

    [UsedImplicitly]
    private void Awake()
    {
        mTargets = GetComponentsInChildren<UIStageHintTarget>();
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
}