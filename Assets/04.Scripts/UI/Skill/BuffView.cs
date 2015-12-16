using UnityEngine;
using System.Collections;

public class BuffView : MonoBehaviour {
	public GameObject Distance;
	public GameObject LifeTime;
	public UILabel LabelTitle;
	public UILabel LabelDistance;
	public UISprite AttrKind;
	public UILabel KindLabel;
	public UILabel LabelTime;
	public UILabel LabelTimeValue;
	public UILabel LabelAttrKind;

	private int myKind;

	void Awake () {
		LabelTime.text = TextConst.S(7210);
		LabelTitle.text = TextConst.S(7211);
		Distance.SetActive(false);
		LifeTime.SetActive(false);
		UIEventListener.Get(AttrKind.gameObject).onClick = OnOpenHint;
	}

	public void ShowDistance (float distance) {
		Distance.SetActive(true);
		LabelDistance.text = distance.ToString();
	}

	public void ShowTime (int kind, float lifetime, float value) {
		LifeTime.SetActive(true);
		myKind = kind;
		KindLabel.text = TextConst.S(3005 + kind);
		AttrKind.spriteName = "AttrKind_" + kind.ToString();
		LabelTimeValue.text = lifetime.ToString();
		LabelAttrKind.text = value.ToString();
	}

	public void OnOpenHint (GameObject go) {
		UIAttributeHint.Get.UpdateView(myKind);
	}

}
