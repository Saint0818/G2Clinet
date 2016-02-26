using UnityEngine;
using GameStruct;
using System.Collections.Generic;
using Newtonsoft.Json;

public struct TSuitCardResult {
	public int[] SuitCardCost;
}

public struct TItemSuitCardGroup {
	public GameObject mSelf;
	public UISprite Background;
	public UILabel SuitNameLabel;
	public TActiveSkillCard[] SuitCards;
	public GameObject LaunchSelect;
	public GameObject LaunchButton;
	public UISprite LabelButtonBG;
	public UILabel LabelButton;
	public UILabel LabelDemandTitle;
	public UILabel LabelDemand;
	public UILabel[] AttrBonus;

	private int suitID;
	private bool isAllGet;

	public void Init (GameObject obj, UIEventListener.VoidDelegate callback) {
		mSelf = obj;
		Background = obj.transform.Find("Background").GetComponent<UISprite>();
		SuitNameLabel = obj.transform.Find("SuitNameLabel").GetComponent<UILabel>();
		SuitCards = new TActiveSkillCard[3];
		for (int i=0; i<SuitCards.Length; i++) {
			SuitCards[i] = new TActiveSkillCard();
			SuitCards[i].Init(obj.transform.Find("SuitGroup/" + i.ToString() + "/View/ItemSkillCard").gameObject);
		}
		LaunchSelect = obj.transform.Find("LaunchSelect").gameObject;
		LaunchButton = obj.transform.Find("LaunchBtn").gameObject;
		UIEventListener.Get(LaunchButton).onClick = callback;
		LabelButton = obj.transform.Find("LaunchBtn/LabelBG/Label").GetComponent<UILabel>();
		LabelButtonBG = obj.transform.Find("LaunchBtn/LabelBG").GetComponent<UISprite>();
		LabelDemandTitle = obj.transform.Find("LaunchBtn/DemandLabel/Label").GetComponent<UILabel>();
		LabelDemand = obj.transform.Find("LaunchBtn/DemandLabel").GetComponent<UILabel>();
		AttrBonus = new UILabel[3];
		for(int i=0; i<AttrBonus.Length; i++) {
			AttrBonus[i] = obj.transform.Find("AttriGroup/BonusLabel" + i.ToString()).GetComponent<UILabel>();
		}

		isAllGet = true;
	}

	public void OnShowCardInfo (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name, out result)) {
			UIItemHint.Get.OnShowForSuit(result);
		}
	}

	public bool UpdateView (int id, int index, Transform parent, bool isExecute) {
		if(GameData.DSuitCard.ContainsKey(id)) {
			mSelf.transform.parent = parent;
			mSelf.transform.localScale = Vector3.one;
			mSelf.transform.localPosition = new Vector3 (0, index * -200, 0);
			mSelf.name = id.ToString();
			LaunchButton.name = index.ToString();
			suitID = id;
			SuitNameLabel.text = GameData.DSuitCard[id].SuitName;
			for (int i=0; i<GameData.DSuitCard[id].Items.Length;i ++) {
				if(GameData.DItemData.ContainsKey(GameData.DSuitCard[id].Items[i])) { 
					SuitCards[i].UpdateViewItemDataForSuit(GameData.DItemData[GameData.DSuitCard[id].Items[i]]);
					UIEventListener.Get(SuitCards[i].MySkillCard).onClick = OnShowCardInfo;
					SuitCards[i].IsCanUseForSuit = !GameData.Team.IsGetItem(GameData.DSuitCard[id].Items[i]);
					if(!GameData.Team.IsGetItem(GameData.DSuitCard[id].Items[i]))
						isAllGet = false;
				} 
				else {
					Debug.LogError("no ID:" + GameData.DSuitCard[id].Items[i]);
					SuitCards[i].Enable = false;
				}
			}
			if(GameData.DSuitCard[id].AttrKind.Length == GameData.DSuitCard[id].Value.Length) {
				for(int i=0; i<GameData.DSuitCard[id].AttrKind.Length; i++) {
					if(GameData.DSuitCard[id].AttrKind[i] != 0) {
						if(GameData.DSuitCard[id].Value[i] > 0)
							AttrBonus[i].text = TextConst.S(10500 + GameData.DSuitCard[id].AttrKind[i]) +"[ff0000] + "+ GameData.DSuitCard[id].Value[i] +"[-]";
						else
							AttrBonus[i].text = TextConst.S(10500 + GameData.DSuitCard[id].AttrKind[i]) +"[ff0000] "+ GameData.DSuitCard[id].Value[i] +"[-]";
					} else {
						AttrBonus[i].gameObject.SetActive(false);
					}
				}
			}

			SetUseColor(isAllGet, isExecute);

			LabelDemand.text = GameData.DSuitCard[id].CardPower.ToString();
			return true;
		}
		return false;
	}

	public void LaunchSuit (bool isHave, bool isExecute) {
		SetUseColor(isHave, isExecute);
	}

	public bool IsAllGet {
		get {return isAllGet;}
	}

	public int SuitID {
		get {return suitID;}
	}

	public void SetUseColor (bool isHave, bool isLaunch) {
		if(!isHave)
		{
			SuitNameLabel.color = colorNo;
			LabelDemandTitle.color = colorNo;
			LabelDemand.color = colorNo;
			Background.color = colorNo;
			for (int i=0; i<AttrBonus.Length; i++) {
				AttrBonus[i].color = colorNo;
			}
			LabelButtonBG.spriteName = "button_gray";
			LabelButton.text = TextConst.S(7704);
			LaunchSelect.SetActive(false);
		} else 
		if(isHave && !isLaunch) 
		{
			SuitNameLabel.color = colorNormal;
			LabelDemandTitle.color = colorNormal;
			LabelDemand.color = colorNormal;
			Background.color = colorNormal;
			for (int i=0; i<AttrBonus.Length; i++) {
				AttrBonus[i].color = colorNormal;
			}
			LabelButtonBG.spriteName = "button_orange1";
			LabelButton.text = TextConst.S(7702);
			LaunchSelect.SetActive(false);
		} else 
		{
			SuitNameLabel.color = colorNormal;
			LabelDemandTitle.color = colorNormal;
			LabelDemand.color = colorNormal;
			Background.color = colorLaunch;
			for (int i=0; i<AttrBonus.Length; i++) {
				AttrBonus[i].color = colorNormal;
			}
			LabelButtonBG.spriteName = "ButtonY";
			LabelButton.text = TextConst.S(7703);
			LaunchSelect.SetActive(true);
		}
	}

	private Color32 colorNo {
		get {return new Color32(184, 184, 184, 255);}
	}

	private Color32 colorNormal {
		get {return Color.white;}
	}

	private Color32 colorLaunch {
		get {return new Color32(30, 170, 220, 255);}
	}
}

public class UISuitCard {
	private UISkillFormation mSelf;
	private GameObject itemSuitCardGroup;

	private UILabel labelSuitCost;
	private GameObject viewCaption;
	private UIScrollView scrollView;

	private List<TItemSuitCardGroup> itemSuitCards;

	private int costNow;
	private int costMax;//啟動值最大值

	public void InitCom (UISkillFormation skillFormation, string UIName) {
		if(LimitTable.Ins.HasByOpenID(GameEnum.EOpenID.SuitCard) && GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(GameEnum.EOpenID.SuitCard)) {
			costNow = GameData.Team.SuitCardExecuteCost;
			costMax = 10;
			mSelf = skillFormation;
			itemSuitCardGroup = Resources.Load(UIPrefabPath.ItemSuitCardGroup) as GameObject;
			
			labelSuitCost = GameObject.Find(UIName + "/SuitCardsView/Top/SuitCostLabel").GetComponent<UILabel>();
			scrollView = GameObject.Find(UIName + "/SuitCardsView/MainView/ScrollView").GetComponent<UIScrollView>();
			viewCaption = GameObject.Find(UIName + "/SuitCardsView/CaptionView");
			
			UIEventListener.Get(GameObject.Find(UIName + "/SuitCardsView/CaptionView/CoverCollider")).onClick = HideCaption;
			
			mSelf.SetBtn(UIName + "/SuitCardsView/Top/CaptionBtn", OnCaption);
			
			viewCaption.SetActive(false);
			itemSuitCards = new List<TItemSuitCardGroup>();
			initSuitCard() ;
		}
	}

	private void initSuitCard() {
		int index = 0;
		foreach (KeyValuePair<int, TSuitCard> suitcard in GameData.DSuitCard) {
			TItemSuitCardGroup item = new TItemSuitCardGroup();
			item.Init(mSelf.Duplicate(itemSuitCardGroup), OnLaunch);
			if(item.UpdateView(suitcard.Key, index, scrollView.transform, GameData.Team.IsExecuteSuitCard(suitcard.Key))){
				itemSuitCards.Add(item);
				index ++;
			}
		}

		labelSuitCost.text = costNow + " / " + costMax.ToString();
	}

	public void MoveToID (int id) {
		if(id >0) {
			scrollView.gameObject.transform.localPosition = new Vector3(15, 75 + (200 * (id-1)), 0);
			scrollView.panel.clipOffset = new Vector2(0, -(scrollView.gameObject.transform.localPosition.y - 70));
		}
	}

	private bool checkCost (int currentCost) {
		return ((currentCost + costNow) <= costMax);
	}

	private void refreshUI () {
		costNow = GameData.Team.SuitCardExecuteCost;
		labelSuitCost.text = costNow + " / " + costMax.ToString();
		UISkillFormation.Get.RefreshTabsRedPoint();

		for(int i=0; i<itemSuitCards.Count; i++)
			itemSuitCards[i].LaunchSuit(itemSuitCards[i].IsAllGet ,GameData.Team.IsExecuteSuitCard(itemSuitCards[i].SuitID));
	}

	public void OnLaunch (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name,out result)) {
			if(result >=0 && result < itemSuitCards.Count) {
				if(itemSuitCards[result].IsAllGet) {
					if(GameData.DSuitCard.ContainsKey(itemSuitCards[result].SuitID)) {
						if(GameData.Team.IsExecuteSuitCard(itemSuitCards[result].SuitID)) {
							//關掉
							int index = GameData.Team.GetExecuteSuitCardIndex(itemSuitCards[result].SuitID);
							if(index != -1)
								SendRemoveSuitCardExecute(index);
							else 
								Debug.LogError("index -1:" + itemSuitCards[result].SuitID);
						} else {
							//開啟
							if(checkCost (GameData.DSuitCard[itemSuitCards[result].SuitID].CardPower))
								SendAddSuitCardExecute(result);
							else 
								UIHint.Get.ShowHint(TextConst.S(7708), Color.red);
						}
					}
				}else 
					UIHint.Get.ShowHint(TextConst.S(7706), Color.red);
			}
		}
	}

	public bool CheckRedPoint {
		get {
			if(LimitTable.Ins.HasByOpenID(GameEnum.EOpenID.SuitCard) && GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(GameEnum.EOpenID.SuitCard)) {
				for (int i=0; i<itemSuitCards.Count; i++) 
					if(itemSuitCards[i].IsAllGet && !GameData.Team.IsExecuteSuitCard(itemSuitCards[i].SuitID))
						return true;
			}
			return false;
		}
	}

	public void OnCaption () {
		viewCaption.SetActive(true);
	}

	public void HideCaption (GameObject go) {
		viewCaption.SetActive(false);
	}

	public void SendAddSuitCardExecute(int index) {
		WWWForm form = new WWWForm();
		form.AddField("Index", index);
		SendHttp.Get.Command(URLConst.AddSuitCardExecute, waitAddSuitCardExecute, form);
	}

	private void waitAddSuitCardExecute(bool ok, WWW www) {
		if (ok) {
			TSuitCardResult result = JsonConvert.DeserializeObject <TSuitCardResult>(www.text, SendHttp.Get.JsonSetting); 
			GameData.Team.SuitCardCost = result.SuitCardCost;
			GameData.Team.Player.SetAttribute(GameEnum.ESkillType.Player);

			refreshUI ();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}

	public void SendRemoveSuitCardExecute(int index) {
		WWWForm form = new WWWForm();
		form.AddField("Index", index);
		SendHttp.Get.Command(URLConst.RemoveSuitCardExecute, waitRemoveSuitCardExecute, form);
	}

	private void waitRemoveSuitCardExecute(bool ok, WWW www) {
		if (ok) {
			TSuitCardResult result = JsonConvert.DeserializeObject <TSuitCardResult>(www.text, SendHttp.Get.JsonSetting); 
			GameData.Team.SuitCardCost = result.SuitCardCost;
			GameData.Team.Player.SetAttribute(GameEnum.ESkillType.Player);

			refreshUI ();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}


}
