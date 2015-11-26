using UnityEngine;
using System.Collections;
using GameStruct;

public class ItemAwardGroup : MonoBehaviour {
	public GameObject Window;

	private AwardAvatarView awardAvatarView;
	private AwardInlayView awardInlayView;
	private AwardSkillView awardSkillView;

	void Awake () {
		awardAvatarView = GetComponentInChildren<AwardAvatarView>();
		awardInlayView = GetComponentInChildren<AwardInlayView>();
		awardSkillView = GetComponentInChildren<AwardSkillView>();
	}

	public void Hide () {
		Window.SetActive(false);
	}

	private void hideAllGroup() {
		awardAvatarView.Hide ();
		awardInlayView.Hide ();
		awardSkillView.Hide ();
	}

	//kind 0:Money 1:EXP 2:Gem  ( Temp )
	public void Show(TItemData itemData/*, int kind = 0, int value = 0/*, bool isOther = false*/)
    {
		Window.SetActive(true);

        hideAllGroup();

//		if(!isOther) {
			if(itemData.Kind == 21) {
				awardSkillView.Show();
				awardSkillView.UpdateUI(itemData);
			} else if(itemData.Kind == 19) {
				awardInlayView.Show();
				awardInlayView.UpdateUI(itemData);
			} else {
				awardAvatarView.Show();
				awardAvatarView.UpdateUI(itemData);
			}
//		}
//        else {
//			awardAvatarView.Show ();
//			switch(kind) {
//			case 0:
//				awardAvatarView.UpdateMoney(value);
//				break;
//			case 1:
//				awardAvatarView.UpdateExp(value);
//				break;
//			case 2:
//				awardAvatarView.UpdateGem(value);
//				break;
//			}
//		}
	}

    public void ShowMoney(int value)
    {
        hideAllGroup();

        awardAvatarView.Show();
        awardAvatarView.UpdateMoney(value);
    }

    public void ShowExp(int value)
    {
        hideAllGroup();

        awardAvatarView.Show();
        awardAvatarView.UpdateExp(value);
    }

    public void ShowGem(int value)
    {
        hideAllGroup();

        awardAvatarView.Show();
        awardAvatarView.UpdateGem(value);
    }
}
