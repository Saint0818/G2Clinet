using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

public class ItemAwardGroup : MonoBehaviour
{
	public GameObject Window;

	public AwardAvatarView awardAvatarView;
	public AwardInlayView awardInlayView;
	public AwardSkillView awardSkillView;

	public  TItemData mItemData;
	private TSkill mSkill;

	private int otherKind = -1;
	private int otherValue = 0;

	private bool isFromSuit;

    [UsedImplicitly]
	void Awake()
    {
		if(awardAvatarView == null)
			awardAvatarView = transform.GetComponentInChildren<AwardAvatarView>();
		
		if(awardInlayView == null)
			awardInlayView = transform.GetComponentInChildren<AwardInlayView>();
		
		if(awardSkillView == null)
			awardSkillView = transform.GetComponentInChildren<AwardSkillView>();
	}

	public void Hide () {
		Window.SetActive(false);
	}

	private void hideAllGroup() {
		awardAvatarView.Hide ();
		awardInlayView.Hide ();
		awardSkillView.Hide ();
	}

	public void Show(TItemData itemData, bool isSuit = false)
    {
		isFromSuit = isSuit;
		Window.SetActive(true);

	    mItemData = itemData;

        hideAllGroup();

		if(itemData.Kind == 21)
        {
			awardSkillView.Show();
			awardSkillView.UpdateUI(itemData);
			mSkill.ID = itemData.Avatar;
			mSkill.Lv = itemData.LV;
		}
        else if(itemData.Kind == 19)
        {
			awardInlayView.Show();
			awardInlayView.UpdateUI(itemData);
		} 
        else
        {
			awardAvatarView.Show();
			awardAvatarView.UpdateUI(itemData);
		}
	}

	public void ShowSkill (TSkill skill) {
		Window.SetActive(true);
		hideAllGroup();
		awardSkillView.Show();
		awardSkillView.UpdateUI(skill);
		mSkill = skill;
	}

	/// <summary>
	///  kind 1:Money 2:Gem 3:EXP 4:PVP 5:Social
	/// </summary>
	/// <param name="kind">Kind.</param>
	/// <param name="value">Value.</param>
	public void ShowOther (int kind, int value) {
		otherKind = kind;
		otherValue = value;
		Window.SetActive(true);
		hideAllGroup();

		awardAvatarView.Show();
		awardAvatarView.UpdateOther(kind, value);

		mItemData = new TItemData();
	}

    public void NotifyClick()
    {
		if(isFromSuit) {
			if(mSkill.ID > 0) 
				UISkillInfo.Get.ShowFromNewCard(mSkill);
			else
				UIItemHint.Get.OnShowForSuit(mItemData.ID);
		} else {
			if(otherKind > -1) {
				UIItemHint.Get.OnShowOther(otherKind, otherValue);
			} else
				if(mItemData.ID > 0 && !UILevelUp.Visible) {
					if(mItemData.Kind == 21) {
						UISkillInfo.Get.ShowFromNewCard(mSkill);
					}else
						UIItemHint.Get.OnShow(mItemData.ID);
				}
		}
			
    }

    public void SetAmountText(string amount) {
        if(mItemData.Kind == 21)
        {
            if (awardSkillView)
                awardSkillView.AmountLabel.text = amount;
        }
        else if(mItemData.Kind == 19)
        {
            if (awardInlayView)
                awardInlayView.AmountLabel.text = amount;
        }
        else
        {
            if (awardAvatarView)
                awardAvatarView.AmountLabel.text = amount;
        }
    }
}
