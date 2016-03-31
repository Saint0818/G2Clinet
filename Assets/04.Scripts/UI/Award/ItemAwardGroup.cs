using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

public class ItemAwardGroup : MonoBehaviour
{
	public GameObject Window;

	private AwardAvatarView awardAvatarView;
	private AwardInlayView awardInlayView;
	private AwardSkillView awardSkillView;

	public  TItemData mItemData;

	private int otherKind = -1;
	private int otherValue = 0;

    [UsedImplicitly]
	void Awake()
    {
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

	public void Show(TItemData itemData)
    {
		Window.SetActive(true);

	    mItemData = itemData;

        hideAllGroup();

		if(itemData.Kind == 21)
        {
			awardSkillView.Show();
			awardSkillView.UpdateUI(itemData);
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
		if(otherKind > -1) {
			UIItemHint.Get.OnShowOther(otherKind, otherValue);
		} else
			if(mItemData.ID > 0 && !UILevelUp.Visible)
				UIItemHint.Get.OnShow(mItemData.ID);
			
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
