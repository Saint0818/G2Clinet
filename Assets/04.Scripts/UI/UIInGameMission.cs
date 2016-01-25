using System.Collections.Generic;
using GameEnum;
using UnityEngine;

public class UIInGameMission : UIBase {
	private static UIInGameMission instance = null;
	private const string UIName = "UIInGameMission";

	private TStageData stageData;
	private int[] hintBits;
	private int hintIndex;
	private string describe;
	private Animator missionAnimator;
	private UIInGameMissionView[] missionViews;
	private Dictionary<int, UIInGameMissionView> missionDic = new Dictionary<int, UIInGameMissionView>();
	private bool isShow = true;
	private bool isFin = false;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static void UIShow(bool isShow){
		if (instance)
			instance.Show(isShow);
		else
			if (isShow)
				Get.Show(isShow);
	}

	public static UIInGameMission Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIInGameMission;

			return instance;
		}
	}

	protected override void InitCom() {
		missionAnimator = GetComponent<Animator>();
		missionViews = GetComponentsInChildren<UIInGameMissionView>(true);

		SetBtnFun(UIName + "/TopLeft/ButtomMission", ShowView);
	}

	private void initPosition () {
		for (int i=0; i<missionViews.Length; i++) {
			missionViews[i].gameObject.transform.localPosition = new Vector3(18, i * (-30), 0);
		}
	}

	private void hideAllTargets() {
		for(int i = 0; i < missionViews.Length; i++) {
			missionViews[i].Hide();
		}
	}

	private int getConditionCount(int type) {
		return MissionChecker.Get.GetSelfTeamCondition(type);
	}

	public void InitView (int stageID) {
		UIShow(true);
		initPosition ();
		hideAllTargets();
		if(StageTable.Ins.HasByID(stageID)) {
			stageData = StageTable.Ins.GetByID(stageID);
			hintBits = stageData.HintBit;
			hintIndex = 0;

			if(hintBits.Length > 1 && hintBits[1] > 0)
			{
				missionViews[hintIndex].Show();
				int team = (int) ETeamKind.Self;
				int score = UIGame.Get.Scores[team];

				if(hintBits[1] == 2) {
				} else if(hintBits[1] == 3){
					team = (int) ETeamKind.Npc;
					score = UIGame.Get.Scores[team];
				} else if(hintBits[1] == 4) {
					score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
				}

				describe = string.Format (GameFunction.GetHintText(2, hintBits[1], 9), stageData.Bit1Num, "", score);
				missionViews[hintIndex].UpdateUI(describe);
				missionDic.Add(1, missionViews[hintIndex]);
				hintIndex++;
			}

			if(hintBits.Length > 2 && hintBits[2] > 0)
			{
				missionViews[hintIndex].Show();
				describe = string.Format (GameFunction.GetHintText(3, hintBits[2], 9), stageData.Bit2Num, "", getConditionCount(hintBits[2]));
				missionViews[hintIndex].UpdateUI(describe);
				missionDic.Add(2, missionViews[hintIndex]);
				hintIndex++;
			}

			if(hintBits.Length > 3 && hintBits[3] > 0)
			{
				missionViews[hintIndex].Show();
				describe = string.Format (GameFunction.GetHintText(3, hintBits[3], 9), stageData.Bit3Num, "", getConditionCount(hintBits[3]));
				missionViews[hintIndex].UpdateUI(describe);
				missionDic.Add(3, missionViews[hintIndex]);
			}
		}
	}

	public void CheckMisstion () {
		isFin = false;
		hintIndex = 0;
		if(missionDic.ContainsKey(1) && !missionDic[1].IsFinish) {
			int score = UIGame.Get.Scores[ETeamKind.Self.GetHashCode()];
//			isFin = (UIGame.Get.Scores[ETeamKind.Self.GetHashCode()] > UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()]);
			isFin = false;
			if(hintBits[1] == 2){
				isFin = (score >= stageData.Bit1Num);
			} else if(hintBits[1] == 3){
//				score = UIGame.Get.Scores[ETeamKind.Npc.GetHashCode()];
//				isFin = (score < stageData.Bit1Num);
				isFin = false;
			} else if(hintBits[1] == 4){
				score = UIGame.Get.Scores[(int) ETeamKind.Self] - UIGame.Get.Scores[(int) ETeamKind.Npc];
				isFin = (score >= stageData.Bit1Num);
			}
			describe = string.Format (GameFunction.GetHintText(2, hintBits[1], 9), stageData.Bit1Num, "", score);
			missionViews[hintIndex].UpdateUI(describe);
			hintIndex ++;
			if((hintBits[1] == 3 || hintBits[1] == 4))
				isFin = false;
			
			if(isFin)
				missionDic[1].UpdateFin();
		}

		if(isFin)
			Invoke("refresh", 1);
		
		isFin = false;
		if(missionDic.ContainsKey(2) && !missionDic[2].IsFinish) {
			isFin = (getConditionCount(hintBits[2]) >=  stageData.Bit2Num);
			describe = string.Format (GameFunction.GetHintText(3, hintBits[2], 9), stageData.Bit2Num, "", getConditionCount(hintBits[2]));
			missionViews[hintIndex].UpdateUI(describe);
			hintIndex ++;
			if(isFin)
				missionDic[2].UpdateFin();
		}

		if(isFin)
			Invoke("refresh", 1);

		isFin = false;
		if(missionDic.ContainsKey(3) && !missionDic[3].IsFinish) {
			isFin = (getConditionCount(hintBits[3]) >=  stageData.Bit2Num);
			describe = string.Format (GameFunction.GetHintText(3, hintBits[3], 9), stageData.Bit3Num, "", getConditionCount(hintBits[3]));
			missionViews[hintIndex].UpdateUI(describe);
			if(isFin)
				missionDic[3].UpdateFin();
		}
	}

	private void refresh () {
		for(int i=1; i<missionViews.Length; i++) {
			if(missionViews[i - 1].IsUse && missionViews[i].IsUse) {
				if(missionViews[i -1].IsFinish) {
					for(int j=1; j<missionViews.Length; j++) {
						if(missionViews[j].IsUse) {
							missionViews[j].gameObject.transform.localPosition = new Vector3(18, missionViews[j].gameObject.transform.localPosition.y + 30, 0);
						}
					}
				}
			}
		}
	}

	public void ShowView () {
		if(isShow) {
			isShow = false;
			missionAnimator.SetTrigger("Close");
		} else  {
			isShow = true;
			missionAnimator.SetTrigger("Open");
		}
	}
}
