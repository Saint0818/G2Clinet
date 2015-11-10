using UnityEngine;
using System.Collections;

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
		                           GameController.Get.Joysticker.Attribute.ActiveSkills[index].Lv.ToString(),
		                           GameData.DSkillData[GameController.Get.Joysticker.Attribute.ActiveSkills[index].ID].Quality.ToString(),
		                           GameController.Get.Joysticker.AngerPower.ToString(),
		                           "/" + GameController.Get.Joysticker.Attribute.MaxAngerOne(GameController.Get.Joysticker.Attribute.ActiveSkills[index].ID).ToString(),
		                           "");
	}
}
