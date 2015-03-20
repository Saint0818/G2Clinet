using UnityEngine;
using System.Collections;

public class AddExampleWeapons : MonoBehaviour {
	
	[SerializeField] private SpaceD_WeaponSwitch weaponSwitch;
	[SerializeField] private GameObject[] exampleWeapons;
	[SerializeField] private UILabel tip;

	void Start()
	{
		if (this.weaponSwitch == null)
			return;
		
		// Add the example weapons
		foreach (GameObject obj in this.exampleWeapons)
		{
			// Instantiate the weapon avatar prefab
			// and add it to the container
			this.weaponSwitch.AddWeapon(obj);
		}
	}

	void Update()
	{
		if (this.weaponSwitch == null)
			return;

		UIPanel panel = this.weaponSwitch.transform.GetComponent<UIPanel>();

		if (this.tip != null && panel != null)
		{
			this.tip.alpha = Mathf.Lerp(this.tip.alpha, ((panel.alpha == 0f) ? 1f : 0f), (Time.deltaTime * ((panel.alpha == 0f) ? 4f : 10f)));
		}

		// Navigate through items
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			if (this.weaponSwitch.currentIndex > 0)
				this.weaponSwitch.FocusWeapon(this.weaponSwitch.currentIndex - 1);
			else
				this.weaponSwitch.FocusWeapon(this.weaponSwitch.lastIndex);
		}
		
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			if (this.weaponSwitch.currentIndex < this.weaponSwitch.lastIndex)
				this.weaponSwitch.FocusWeapon(this.weaponSwitch.currentIndex + 1);
			else
				this.weaponSwitch.FocusWeapon(0);
		}
	}
}
