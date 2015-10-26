using UnityEngine;
using System.Collections;

public class UIStageHint : UIBase {
	private static UIStageHint instance = null;
	private const string UIName = "UIStageHint";

	private TGameTarget[] gameTargets = new TGameTarget[5];
	private int stageKind = 0; //0:For GamePause 1:For Stage
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
		
		set {
			if (instance) {
				if (!value)
					RemoveUI(UIName);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}
	
	public static UIStageHint Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIStageHint;
			
			return instance;
		}
	}
	
	public static void UIShow(bool isShow) {
		if(instance)
			instance.Show(isShow);
		else
			if(isShow)
				Get.Show(isShow);
	}
	
	protected override void InitCom() {
		for(int i=0; i<gameTargets.Length; i++) {
			gameTargets[i].Self = GameObject.Find(UIName + "/Center/GameTarget/Target"+i.ToString());
			gameTargets[i].LabelTargetName = GameObject.Find(UIName + "/Center/GameTarget/Target"+i.ToString()+"/TargetLabel").GetComponent<UILabel>();
			gameTargets[i].LabelTargetValue = GameObject.Find(UIName + "/Center/GameTarget/Target"+i.ToString()+"/BitNumLabel").GetComponent<UILabel>();
			gameTargets[i].LabelValue = GameObject.Find(UIName + "/Center/GameTarget/Target"+i.ToString()+"/BitValueLabel").GetComponent<UILabel>();
			gameTargets[i].LabelCaption = GameObject.Find(UIName + "/Center/GameTarget/Target"+i.ToString()+"/CaptionLabel").GetComponent<UILabel>();
			gameTargets[i].Self.SetActive(false);
		}
	}

	public void ShowTarget (bool isShow, int kind) {
		UIShow(isShow);
		Get.SetKind(kind);
		initTarget();
	}

	private void initTarget() {
		switch (stageKind) {
		case 0:
			int index = 0;
			if(GameController.Get.StageHintBit.Length > 0 && GameController.Get.StageHintBit[0] > 0) {
				gameTargets[index].Self.SetActive(true);
				gameTargets[index].LabelCaption.text = setText(index + 1, GameController.Get.StageHintBit[0], 8);
				gameTargets[index].LabelTargetName.text = setText(index + 1, GameController.Get.StageHintBit[0], 9);
				gameTargets[index].LabelTargetValue.text = "/"+  GameController.Get.StageBitNum[0];
				gameTargets[index].LabelValue.text = GameController.Get.GameTime.ToString();
				index ++;
			}
			
			if(GameController.Get.StageHintBit.Length > 1 && GameController.Get.StageHintBit[1] > 0) {
				gameTargets[index].Self.SetActive(true);
				gameTargets[index].LabelCaption.text = setText(index + 1, GameController.Get.StageHintBit[1], 8);
				gameTargets[index].LabelTargetName.text = setText(index + 1, GameController.Get.StageHintBit[1], 9);
				gameTargets[index].LabelTargetValue.text = "/"+ GameController.Get.StageBitNum[1];
				gameTargets[index].LabelValue.text = UIGame.Get.Scores[(int) ETeamKind.Self].ToString();
				index ++;
			}
			
			if(GameController.Get.StageHintBit.Length > 2 && GameController.Get.StageHintBit[2] > 0) {
				gameTargets[index].Self.SetActive(true);
				gameTargets[index].LabelCaption.text = setText(3, GameController.Get.StageHintBit[2], 8);
				gameTargets[index].LabelTargetName.text = setText(3, GameController.Get.StageHintBit[2], 9);
				gameTargets[index].LabelTargetValue.text = "/"+ GameController.Get.StageBitNum[2];
				gameTargets[index].LabelValue.text = getConditionCount(GameController.Get.StageHintBit[2]).ToString();
				index ++;
			}
			
			if(GameController.Get.StageHintBit.Length > 3 && GameController.Get.StageHintBit[3] > 0) {
				gameTargets[index].Self.SetActive(true);
				gameTargets[index].LabelCaption.text = setText(3, GameController.Get.StageHintBit[3], 8);
				gameTargets[index].LabelTargetName.text = setText(3, GameController.Get.StageHintBit[3], 9);
				gameTargets[index].LabelTargetValue.text = "/"+ GameController.Get.StageBitNum[3];
				gameTargets[index].LabelValue.text = getConditionCount(GameController.Get.StageHintBit[3]).ToString();
				index ++;
			}
			break;
		case 1:
			break;
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

	private string setText (int index, int value, int id){
		int baseValue = 2000000 + (int)(Mathf.Pow(10,index) * value) + id;
		return TextConst.S(baseValue);
	}

	public void SetKind(int kind) {
		stageKind = kind;
	}
}
