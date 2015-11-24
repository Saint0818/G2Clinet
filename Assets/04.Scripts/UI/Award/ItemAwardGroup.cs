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

	private void hideAllGroup () {
		awardAvatarView.Hide ();
		awardInlayView.Hide ();
		awardSkillView.Hide ();
	}

	public void Show(TItemData itemData) {
		Window.SetActive(true);
		if(itemData.Kind == 21) {
			awardSkillView.Show ();
			awardSkillView.UpdateUI(itemData);
		} else if(itemData.Kind == 19) {
			awardInlayView.Show ();
			awardInlayView.UpdateUI(itemData);
		} else {
			awardAvatarView.Show ();
			awardAvatarView.UpdateUI(itemData);
		}
	}
}
