using GameStruct;
using UnityEngine;

public class GetTenItem : MonoBehaviour {
	private bool isShow = false;
	private bool isShowPause = false;
	private bool isShowFin = false;
	private float showTime = 0;
	private float showInterval = 0.5f;
	private int index = 0;
	private bool[] isNeedPause;

	public ItemAwardGroup[] itemAwardGroups;


//	void Start () {
//		Reset ();
//	}

	public void Reset () {
		isShow = false;
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
				if(!isShowPause) {
					if(index < 10) {
						showTime -= Time.deltaTime;
						if(showTime <=0) {
							showTime = showInterval;
							itemAwardGroups[index].gameObject.SetActive(true);
							if(isNeedPause[index]){
								isNeedPause[index] = false;
								isShowPause = true;
								UIGetSkillCard.Get.Show(itemAwardGroups[index].mItemData.ID);
							}
							index ++;
						} 
					}else {
						isShowFin = true;
						UIBuyStore.Get.FinishDrawLottery();
					}
				}
			}
		}
	}

	public void GoAhead () {
		isShowPause = false;
	}

	public void ShowAni(bool isshow) {
		isShow = isshow;
	}

	public void Show (TItemData[] itemData) {
		index = 0;
		isNeedPause = new bool[itemData.Length];
		if(itemData.Length > 0 && itemData.Length == 10 && itemAwardGroups.Length == 10) {
			for(int i=0; i<itemData.Length; i++) {
				if(itemData[i].Kind == 21)
					isNeedPause[i] = GameData.Team.CheckSkillCardisNew(itemData[i].Avatar);
				else 
					isNeedPause[i] = false;
				
				itemAwardGroups[i].Show(itemData[i]);
			}
		} else  {
			Debug.LogError("itemData Lengh is not 10." + itemData.Length);
			UIBuyStore.Get.FinishDrawLottery();
		}

		showTime = showInterval;
	}
}
