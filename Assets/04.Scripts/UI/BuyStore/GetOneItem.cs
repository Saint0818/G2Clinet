using UnityEngine;
using System.Collections;
using GameStruct;

public class GetOneItem : MonoBehaviour {
	public ItemAwardGroup itemAwardGroup;

//	void Start () {
//		Reset();
//	}

	public void Reset () {
		itemAwardGroup.gameObject.SetActive(false);
	}

	public void Show (TItemData itemData) {
		itemAwardGroup.gameObject.SetActive(true);
		itemAwardGroup.Show(itemData);
	}
}
