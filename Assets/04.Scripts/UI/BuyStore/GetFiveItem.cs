using UnityEngine;
using System.Collections;
using GameStruct;

public class GetFiveItem : MonoBehaviour {
	private bool isShow = false;
	private bool isShowPause = false;
	private bool isShowFin = false;
	private float showTime = 0;
	private float showInterval = 0.5f;
	private int index = 0;
	private bool[] isNeedPause;


	public ItemAwardGroup[] itemAwardGroups;

	public void Reset () {
		isShowFin = false;
		isShowPause = false;
		showTime = 0;
		index = 0;

		for(int i=0; i<itemAwardGroups.Length; i++) {
			itemAwardGroups[i].gameObject.SetActive(false);
		}
	}

	void FixedUpdate () {
		if(isShow) {
			if(!isShowFin) {
				if(!isShowPause && index < 5) {
					showTime -= Time.deltaTime;
					if(showTime <=0) {
						showTime = showInterval;
						itemAwardGroups[index].gameObject.SetActive(true);
						//				if(isNeedPause[index]){
						//					isShowPause = true;
						//					TSkill skill = new TSkill();
						//					skill.ID = itemAwardGroups[index].mItemData.Avatar;
						//					skill.Lv = itemAwardGroups[index].mItemData.LV;
						//					UISkillInfo.Get.ShowFromNewCard(skill);
						//				}
						index ++;
					}
				} else {
					isShowFin = true;
					UIBuyStore.Get.FinishDrawLottery();
				}
			}
		}
	}


	public void GoAhead () {
		isShowPause = true;
	}

	public void ShowAni(bool isshow) {
		isShow = isshow;
	}

	public void Show (TItemData[] itemData) {
		index = 0;
		isNeedPause = new bool[itemData.Length];
		if(itemData.Length > 0 && itemData.Length == 5 && itemAwardGroups.Length == 5) {
			for(int i=0; i<itemData.Length; i++) {
				if(itemData[i].Kind == 21)
					isNeedPause[i] = GameData.Team.CheckSkillCardisNew(itemData[i].Avatar);
				else 
					isNeedPause[i] = false;

				itemAwardGroups[i].Show(itemData[i]);
			}
		} else  {
			Debug.LogError("itemData Lengh is not 5." + itemData.Length);
			UIBuyStore.Get.FinishDrawLottery();
		}

		showTime = showInterval;
	}
}
