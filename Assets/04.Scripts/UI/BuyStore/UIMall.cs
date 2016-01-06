using UnityEngine;
using System.Collections;

public class UIMall : UIBase {
	private static UIMall instance = null;
	private const string UIName = "UIMall";

	private GameObject table;
	private GameObject skillCard;
	private GameObject itemIcon;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);	
	}

	public static UIMall Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMall;

			return instance;
		}
	}

	protected override void InitCom() {
		skillCard = Resources.Load(UIPrefabPath.ItemSkillCard) as GameObject;
		itemIcon = Resources.Load(UIPrefabPath.ItemAwardGroup) as GameObject;
		table = GameObject.Find(UIName + "/Center/Window/ScrollView/Table");
		SetBtnFun(UIName + "/BottomLeft/BackBtn", OnClose);
	}

	public void Show () {//420
		UIShow(true);
		for (int i=0; i<GameData.DPickCost.Length; i++) {
			TMallBox mallBox = new TMallBox();
			GameObject prefab = Instantiate(Resources.Load("Prefab/UI/Items/" + GameData.DPickCost[i].Prefab)) as GameObject;
			setParentInit(prefab, table);
			mallBox.Init(prefab);
			mallBox.UpdateView(i, GameData.DPickCost[i]);
			if(GameData.DPickCost[i].ShowCard != null && GameData.DPickCost[i].ShowCard.Length > 0) {
				for(int j=0; j<GameData.DPickCost[i].ShowCard.Length; j++) {
					if(GameData.DItemData.ContainsKey(GameData.DPickCost[i].ShowCard[j])) {
						TActiveSkillCard activeSkillCard = new TActiveSkillCard();
						GameObject obj = Instantiate(skillCard) as GameObject;
						setParentInit(obj, mallBox.DiskScrollView);
						obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
						activeSkillCard.Init(obj,new EventDelegate(ShowSkillCardHint), false);
						activeSkillCard.UpdateViewItemData(GameData.DItemData[GameData.DPickCost[i].ShowCard[j]]);
						mallBox.UpdataCards(j, activeSkillCard.MySkillCard);
					}
				}
			}
			if(GameData.DPickCost[i].ShowItem != null && GameData.DPickCost[i].ShowItem.Length > 0) {
				for(int j=0; j<GameData.DPickCost[i].ShowItem.Length; j++) {
					if(GameData.DItemData.ContainsKey(GameData.DPickCost[i].ShowItem[j])) {
						ItemAwardGroup item = (Instantiate(itemIcon) as GameObject ).GetComponent<ItemAwardGroup>();
						setParentInit(item.gameObject, mallBox.ItemScrollView);
						item.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 1);
						item.gameObject.transform.localPosition = new Vector3(150 * j, 0, 0);
						item.Show(GameData.DItemData[GameData.DPickCost[i].ShowItem[j]]);
					}
				}
			}
		}
	}

	private void setParentInit (GameObject obj, GameObject parent) {
		obj.transform.parent = parent.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localScale = Vector3.one;
	}

	public void OnClose () {
		UIShow(false);
	}

	public void ShowSkillCardHint () {
		int result = -1;
		if(int.TryParse(UIButton.current.name,out result)) {
			if(GameData.DItemData.ContainsKey(result))
				UIItemHint.Get.OnShow(GameData.DItemData[result]);
		}
	}
}
