using GameStruct;
using UnityEngine;

public class GetOneItem : MonoBehaviour {
	public ItemAwardGroup itemAwardGroup;

	private TItemData mItemData;
//	void Start () {
//		Reset();
//	}

	public void Reset () {
		itemAwardGroup.gameObject.SetActive(false);
	}

	public void Show (TItemData itemData) {
		itemAwardGroup.gameObject.SetActive(true);
		itemAwardGroup.Show(itemData);
		mItemData = itemData;
			
	}

	public void ShowNew () {
		if(GameData.Team.CheckSkillCardisNew(mItemData.Avatar))
			UIGetSkillCard.Get.Show(mItemData.ID);
	}
}
