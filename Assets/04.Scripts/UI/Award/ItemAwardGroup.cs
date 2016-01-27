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

    public void ShowMoney(int value)
	{
		Window.SetActive(true);
        hideAllGroup();

        awardAvatarView.Show();
        awardAvatarView.UpdateMoney(value);

        mItemData = new TItemData();
    }

    public void ShowExp(int value)
	{
		Window.SetActive(true);
        hideAllGroup();

        awardAvatarView.Show();
        awardAvatarView.UpdateExp(value);

        mItemData = new TItemData();
    }

    public void ShowGem(int value)
	{
		Window.SetActive(true);
        hideAllGroup();

        awardAvatarView.Show();
        awardAvatarView.UpdateGem(value);

        mItemData = new TItemData();
    }

    public void NotifyClick()
    {
		if(mItemData.ID > 0 && !UILevelUp.Visible)
			UIItemHint.Get.OnShow(mItemData.ID);
    }
}
