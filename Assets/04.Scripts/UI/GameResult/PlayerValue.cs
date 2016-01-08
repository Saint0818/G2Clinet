using GameStruct;
using UnityEngine;

public class PlayerValue : MonoBehaviour {
	public UILabel[] Value = new UILabel[7];

	public void SetValue (TGamePlayerRecord record) {
		Value[0].text = record.FGIn.ToString();
		Value[1].text = record.FG3In.ToString();
		Value[2].text = record.Dunk.ToString();
		Value[3].text = record.Rebound.ToString();
		Value[4].text = record.Steal.ToString();
		Value[5].text = record.Assist.ToString();
		Value[6].text = record.Block.ToString();
	}
}
