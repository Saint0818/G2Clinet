using UnityEngine;
using System.Collections;
using GameStruct;

public class TeamValue : MonoBehaviour {
	public UISlider[] SliderAttrNPC = new UISlider[7];
	public UILabel[] LabelAttrSelf = new UILabel[7];
	public UILabel[] LabelAttrNPC = new UILabel[7];

	private int[] valueSelf = new int[7];
	private int[] valueNPC = new int[7];

	public void SetValue (TGameRecord record){
		for (int i=0; i<valueSelf.Length; i++) {
			for (int j=0; j<record.PlayerRecords.Length; j++) {
				if(j < 3) {
					if(i == 0)
						valueSelf[i] += record.PlayerRecords[j].FGIn;
					else if(i == 1)
						valueSelf[i] += record.PlayerRecords[j].FG3In;
					else if(i == 2)
						valueSelf[i] += record.PlayerRecords[j].Dunk;
					else if(i == 3)
						valueSelf[i] += record.PlayerRecords[j].Rebound;
					else if(i == 4)
						valueSelf[i] += record.PlayerRecords[j].Steal;
					else if(i == 5)
						valueSelf[i] += record.PlayerRecords[j].Assist;
					else if(i == 6)
						valueSelf[i] += record.PlayerRecords[j].Block;
				} else {
					if(i == 0)
						valueNPC[i] += record.PlayerRecords[j].FGIn;
					else if(i == 1)
						valueNPC[i] += record.PlayerRecords[j].FG3In;
					else if(i == 2)
						valueNPC[i] += record.PlayerRecords[j].Dunk;
					else if(i == 3)
						valueNPC[i] += record.PlayerRecords[j].Rebound;
					else if(i == 4)
						valueNPC[i] += record.PlayerRecords[j].Steal;
					else if(i == 5)
						valueNPC[i] += record.PlayerRecords[j].Assist;
					else if(i == 6)
						valueNPC[i] += record.PlayerRecords[j].Block;
				}
			}
			if(valueSelf[i] == 0 && valueNPC[i] == 0) 
				SliderAttrNPC[i].value = 0.5f;
			else 
				SliderAttrNPC[i].value = ((float)valueNPC[i] / (float)(valueSelf[i] + valueNPC[i]));
			LabelAttrSelf[i].text = valueSelf[i].ToString();
			LabelAttrNPC[i].text = valueNPC[i].ToString();
		}
	}
}
