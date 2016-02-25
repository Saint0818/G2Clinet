using UnityEngine;

public class ItemSkillHint : MonoBehaviour {
	public GameObject Window;
	public ItemSkillHintView itemSkillHintView;

	// Use this for initialization
	void Awake () {
		itemSkillHintView = GetComponentInChildren<ItemSkillHintView>();
		Hide ();
	}
	
	public void Show () {
		Window.SetActive(true);
	}

	public void Hide () {
		Window.SetActive(false);
	}

	public void UpdateUI (int index) {
		itemSkillHintView.UpdateUI(GameData.DSkillData[GameController.Get.Joysticker.Attribute.ActiveSkills[index].ID].Name,
		                           GameController.Get.Joysticker.Attribute.ActiveSkills[index].Lv,
		                           GameData.DSkillData[GameController.Get.Joysticker.Attribute.ActiveSkills[index].ID].Quality,
		                           GameController.Get.Joysticker.AngerPower.ToString(),
			"/" + GameController.Get.Joysticker.Attribute.MaxAngerOne(GameController.Get.Joysticker.Attribute.ActiveSkills[index].ID, GameController.Get.Joysticker.Attribute.ActiveSkills[index].Lv).ToString(),
		                           GameData.DSkillData[GameController.Get.Joysticker.Attribute.ActiveSkills[index].ID].PictureNo);
	}
}
