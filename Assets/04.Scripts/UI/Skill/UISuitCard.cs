using UnityEngine;
using GameStruct;
using System.Collections.Generic;
using Newtonsoft.Json;

public struct TSuitCardResult {
	public int[] SuitCardCost;
}

public struct TSuitCardCaption {
	public GameObject mSelf;
	public UIScrollView ScrollView;
	public UILabel SuitName;
}

public struct TSuitCardLaunch {
	public GameObject mSelf;
	public UILabel LabelCount;
	public UILabel[] LabelBonus;
	public UISprite BG;
	public UISprite SpriteIcon;

	public void Init (GameObject obj, Transform parent) {
		mSelf = obj;
		mSelf.transform.SetParent(parent);
		mSelf.transform.localScale = Vector3.one;
		LabelBonus = new UILabel[2];
	}

	public void UpdateStar(int starIndex) {
		SpriteIcon.spriteName = "Staricon" + starIndex.ToString();
	}

	public void UpdateView (int index, int starCount, int attrKind1, int attrValue1, int attrKind2, int attrValue2) {
		mSelf.transform.localPosition = new Vector3(0, 220 - 80 * index, 0);
		LabelCount.text = starCount.ToString();
		checkKind(0, attrKind1, attrValue1);
		checkKind(1, attrKind2, attrValue2);
	}

	private void checkKind (int index, int attrKind1, int attrValue1) {
		if(attrKind1 != 0) {
			if(attrValue1 > 0)
				LabelBonus[index].text = TextConst.S(10500 + attrKind1) +"[00ff00] + "+ attrValue1 +"[-]";
			else
				LabelBonus[index].text = TextConst.S(10500 + attrKind1) +"[ff0000] "+ attrValue1 +"[-]";
		} else {
			LabelBonus[index].gameObject.SetActive(false);
		}
	}

	public void SetNowOpen (bool isOpen) {
		if(isOpen)
			BG.spriteName = "MissionBoard";
		else
			BG.spriteName = "MissionBoardMask";
	}
}

public struct TSuitCardGroupValue {
	public int Index;
	public TSuitCard SuitCard;
	public TItemData[] Items;
	public int[] ItemsLv;
	public bool[] IsGetItem;
	public int[] AttrKinds;
	public int[] AttrValues;
	public bool IsExecute;
	public bool IsGetAllCard;
	public int NowStar;
	public int NextStar;
	public int ExecuteLevel;
	public bool IsHighLevel;

	public void Init () {
		Items = new TItemData[3];
		ItemsLv = new int[3];
		IsGetItem = new bool[3];
		AttrKinds = new int[2];
		AttrValues = new int[2];
		IsGetAllCard = true;
	}

}

public struct TSuitCardGroup {
	public GameObject mSelf;
	public GameObject LaunchSelect;
	public UISprite Background;
	public UILabel SuitNameLabel;
	public GameObject LaunchButton;
	public UILabel LabelButton;
	public UISprite LabelButtonBG;
	public UILabel LabelDemand;
	public TActiveSkillCard[] SuitCards;
	public UILabel[] AttrBonus;
	public UILabel NowCountLabel;
	public UISprite NowStarSprite;
	public UILabel NextCountLabel;
	public UISprite NextStarSprite;
	public GameObject MoreButton;

	private TSuitCard mSuitCard;
	public TSuitCardGroupValue SuitCardGroupValue;

	public void Init () {
		SuitCards = new TActiveSkillCard[3];
		AttrBonus = new UILabel[2];
	}

	public void UpdateStar(int starIndex) {
		NowStarSprite.spriteName = "Staricon" + starIndex.ToString();
		NextStarSprite.spriteName = "Staricon" + starIndex.ToString();
	}

	public void UpdateView (GameObject obj, Transform parent, TSuitCardGroupValue value) {
		SuitCardGroupValue = value;
		mSelf = obj;
		mSelf.transform.parent = parent;
		mSelf.transform.localScale = Vector3.one;
		mSelf.transform.localPosition = new Vector3 (0, value.Index * -345, 0);
		mSuitCard = value.SuitCard;
		mSelf.name = mSuitCard.ID.ToString();
		LaunchButton.name = value.Index.ToString();
		SuitNameLabel.text = mSuitCard.SuitName;

		for(int i=0; i<value.Items.Length; i++) {
			SuitCards[i].UpdateViewItemDataForSuit(value.Items[i], value.ItemsLv[i]);
			SuitCards[i].IsCanUseForSuit = !value.IsGetItem[i];
		}

		for(int i=0; i<value.AttrKinds.Length; i++) {
			if(value.AttrKinds[i] != 0) {
				if(value.AttrValues[i] > 0)
					AttrBonus[i].text = TextConst.S(10500 + value.AttrKinds[i]) +"[00ff00] + "+ value.AttrValues[i] +"[-]";
				else
					AttrBonus[i].text = TextConst.S(10500 + value.AttrKinds[i]) +"[ff0000] "+ value.AttrValues[i] +"[-]";
			} else {
				AttrBonus[i].gameObject.SetActive(false);
			}
		}

		SetUseColor(value.IsGetAllCard, value.IsExecute);
		NowCountLabel.text = value.NowStar.ToString();
		NextCountLabel.text = value.NextStar.ToString();
		if(value.IsHighLevel)
			NextCountLabel.gameObject.SetActive(false);
		LabelDemand.text = mSuitCard.CardPower.ToString();
	}

	public void LaunchSuit (bool isHave, bool isExecute) {
		SetUseColor(isHave, isExecute);
	}

	public bool IsAllGet {
		get {return SuitCardGroupValue.IsGetAllCard;}
	}

	public TSuitCard SuitCard {
		get {return mSuitCard;}
	}

	public void SetUseColor (bool isHave, bool isLaunch) {
		if(!isHave)
		{
			SuitNameLabel.color = colorNo;
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
	private UISuitAvatar mSelf;
	private GameObject itemSuitCardGroup;
	private GameObject itemSuitCardLaunch;

	private UILabel labelSuitCost;
	private UIScrollView scrollView;
	private GameObject viewCaption;
	private UILabel labelCaptionSuitName;
	private UIScrollView scrollViewCaption;

	private List<TSuitCardGroup> itemSuitCards = new List<TSuitCardGroup>();
	private TSuitCardLaunch[] suitCardCaption;

	private int costNow;
	private int costMax;//啟動值最大值

	private int starIndex = 1;

	public void OnDestroy() {
		itemSuitCards.Clear();
	}

	public void InitCom (UISuitAvatar suitAvatar, string UIName) {
		if(GameData.IsOpenUIEnableByPlayer(GameEnum.EOpenID.SuitCard)) {
			costNow = GameData.Team.SuitCardExecuteCost;
			costMax = 10;
			mSelf = suitAvatar;
			itemSuitCardGroup = Resources.Load(UIPrefabPath.ItemSuitCardGroup) as GameObject;
			itemSuitCardLaunch = Resources.Load(UIPrefabPath.ItemSuitCardLaunch) as GameObject;
			
			labelSuitCost = GameObject.Find(UIName + "/Window1/SuitCardsView/Top/SuitCostLabel").GetComponent<UILabel>();
			scrollView = GameObject.Find(UIName + "/Window1/SuitCardsView/MainView/ScrollView").GetComponent<UIScrollView>();
			viewCaption = GameObject.Find(UIName + "/Window1/SuitCardsView/CaptionView");
			UIEventListener.Get(GameObject.Find(UIName + "/Window1/SuitCardsView/CaptionView/CoverCollider")).onClick = HideCaption;
			labelCaptionSuitName = GameObject.Find(UIName + "/Window1/SuitCardsView/CaptionView/View/SuitNameLabel").GetComponent<UILabel>();
			scrollViewCaption = GameObject.Find(UIName + "/Window1/SuitCardsView/CaptionView/View/ScrollView").GetComponent<UIScrollView>();
			viewCaption.SetActive(false);

			itemSuitCards = new List<TSuitCardGroup>();
			initSuitCard() ;
		}
	}

	private void initSuitCard() {
		int index = 0;
		foreach (KeyValuePair<int, TSuitCard> suitcard in GameData.DSuitCard) {
			TSuitCardGroup item = new TSuitCardGroup();
			item.Init();
			GameObject obj = mSelf.Duplicate(itemSuitCardGroup);
			item.Background = obj.transform.Find("Background").GetComponent<UISprite>();
			item.SuitNameLabel = obj.transform.Find("SuitNameLabel").GetComponent<UILabel>();
			for (int i=0; i<item.SuitCards.Length; i++) {
				item.SuitCards[i] = new TActiveSkillCard();
				item.SuitCards[i].Init(obj.transform.Find("SuitGroup/" + i.ToString() + "/View/ItemSkillCard").gameObject);
				UIEventListener.Get(item.SuitCards[i].MySkillCard).onClick = ClickCard;
			}
			item.LaunchSelect = obj.transform.Find("LaunchSelect").gameObject;
			item.LaunchButton = obj.transform.Find("LaunchBtn").gameObject;
			UIEventListener.Get(item.LaunchButton).onClick = OnLaunch;
			item.LabelButton = obj.transform.Find("LaunchBtn/LabelBG/Label").GetComponent<UILabel>();
			item.LabelButtonBG = obj.transform.Find("LaunchBtn/LabelBG").GetComponent<UISprite>();
			item.LabelDemand = obj.transform.Find("LaunchBtn/DemandLabel").GetComponent<UILabel>();
			for(int i=0; i<item.AttrBonus.Length; i++) 
				item.AttrBonus[i] = obj.transform.Find("AttriGroup/BonusLabel" + i.ToString()).GetComponent<UILabel>();
			
			item.NowCountLabel = obj.transform.Find("AttriGroup/NowLaunch/CountLabel").GetComponent<UILabel>();
			item.NowStarSprite = obj.transform.Find("AttriGroup/NowLaunch/LabelIcon").GetComponent<UISprite>();
			item.NextCountLabel = obj.transform.Find("AttriGroup/NextLaunch/CountLabel").GetComponent<UILabel>();
			item.NextStarSprite = obj.transform.Find("AttriGroup/NextLaunch/LabelIcon").GetComponent<UISprite>();
			item.MoreButton = obj.transform.Find("More/MoreBtn").gameObject;
			item.MoreButton.name = index.ToString();
			UIEventListener.Get(item.MoreButton).onClick = OnCaption;

			TSuitCardGroupValue groupValue = new TSuitCardGroupValue();
			groupValue.Init();
			groupValue.Index = index;
			groupValue.SuitCard = suitcard.Value;

			for(int i=0; i<groupValue.SuitCard.Items.Length; i++) {
				findItemIDMax(groupValue.SuitCard.Items[i], ref groupValue.Items[i], ref groupValue.ItemsLv[i], ref groupValue.NowStar);
				groupValue.IsGetItem[i] = (groupValue.Items[i].ID != 0);
				if(groupValue.IsGetAllCard)
					groupValue.IsGetAllCard = (groupValue.Items[i].ID != 0);
				
				if(groupValue.Items[i].ID == 0) {
					groupValue.Items[i] = GameData.DItemData[groupValue.SuitCard.Items[i][0]];
					groupValue.ItemsLv[i] = 0;
				}
			}

			int executeLevel = 0;

			if(groupValue.IsGetAllCard) 
				executeLevel = GameData.Team.GetStarLevel(groupValue.NowStar, groupValue.SuitCard);

			for (int i=0; i<groupValue.SuitCard.AttrKinds.Length; i++) {
				groupValue.AttrKinds[i] = groupValue.SuitCard.AttrKinds[i][executeLevel];
				groupValue.AttrValues[i] = groupValue.SuitCard.Values[i][executeLevel];
			}

			groupValue.ExecuteLevel = executeLevel;
			groupValue.IsHighLevel = (executeLevel == groupValue.SuitCard.StarNum.Length -1);
			groupValue.IsExecute = GameData.Team.IsExecuteSuitCard(suitcard.Key);
			if(executeLevel != groupValue.SuitCard.StarNum.Length -1) 
				groupValue.NextStar = groupValue.SuitCard.StarNum[executeLevel + 1];

			item.UpdateView(obj, scrollView.transform, groupValue);
			itemSuitCards.Add(item);
			index ++;
		}

		labelSuitCost.text = costNow + " / " + costMax.ToString();
	}

	private void findItemIDMax (int[] itemids, ref TItemData itemdata, ref int itemlv , ref int count) {
		for(int i = itemids.Length - 1; i >= 0; i--) {
			if(GameData.DItemData.ContainsKey(itemids[i])) {
				if(GameData.DSkillData.ContainsKey(GameData.DItemData[itemids[i]].Avatar)) {
					if(GameData.Team.IsGetItem(itemids[i])) {
						itemdata = GameData.DItemData[itemids[i]];
						itemlv = GameData.Team.GetSkillCardStar(GameData.DItemData[itemids[i]].Avatar);
						count += findItemIDStars(i, itemids) + GameData.Team.GetSkillCardStar(GameData.DItemData[itemids[i]].Avatar);
						return ;
					}
				}
			}
		}
	}
	 //加總前面卡牌的星星數
	private int findItemIDStars (int index, int[] itemids) {
		int count = 0;
		for (int i=0; i<itemids.Length; i++) 
			if(i<index)
				count += GameData.DSkillData[GameData.DItemData[itemids[i]].Avatar].MaxStar;
			
		return count;
	}

	public void MoveToID (int id) {
		if(id >0) {
			scrollView.gameObject.transform.localPosition = new Vector3(15, 75 + (200 * (id-1)), 0);
			scrollView.panel.clipOffset = new Vector2(0, -(scrollView.gameObject.transform.localPosition.y - 70));
		}
	}

	public void ClickCard (GameObject go) {
		string[] value = go.name.Split("_"[0]);
		UIItemHint.Get.OnShowForSuit(int.Parse(value[0]), int.Parse(value[1]));
	}

	private bool checkCost (int currentCost) {
		return ((currentCost + costNow) <= costMax);
	}

	private void refreshUI () {
		mSelf.RefreshTabsRedPoint();
		if(UIMainLobby.Get.IsVisible)
			UIMainLobby.Get.UpdateButtonStatus();
		
		costNow = GameData.Team.SuitCardExecuteCost;
		labelSuitCost.text = costNow + " / " + costMax.ToString();

		for(int i=0; i<itemSuitCards.Count; i++)
			itemSuitCards[i].LaunchSuit(itemSuitCards[i].IsAllGet ,GameData.Team.IsExecuteSuitCard(itemSuitCards[i].SuitCard.ID));
	}

	public void OnLaunch (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name,out result)) {
			if(result >=0 && result < itemSuitCards.Count) {
				if(itemSuitCards[result].IsAllGet) {
					if(GameData.Team.IsExecuteSuitCard(itemSuitCards[result].SuitCard.ID)) {
						//關掉
						int index = GameData.Team.GetExecuteSuitCardIndex(itemSuitCards[result].SuitCard.ID);
						if(index != -1)
							SendRemoveSuitCardExecute(index);
						else 
							Debug.LogError("index -1:" + itemSuitCards[result].SuitCard.ID);
					} else {
						//開啟
						if(checkCost (itemSuitCards[result].SuitCard.CardPower))
							SendAddSuitCardExecute(result);
						else 
							UIHint.Get.ShowHint(TextConst.S(7708), Color.black);
					}
				}else 
					UIHint.Get.ShowHint(TextConst.S(7706), Color.black);
			}
		}
	}

	public void RunStar () {
		starIndex ++;
		if(starIndex > 5)
			starIndex = 1;
		
		if(viewCaption.activeSelf) 
			if(suitCardCaption != null) 
				for(int i=0; i<suitCardCaption.Length; i++) 
					suitCardCaption[i].UpdateStar(starIndex);

		for(int i=0; i<itemSuitCards.Count; i++)
			itemSuitCards[i].UpdateStar(starIndex);
	}

	private void createCaption (int index) {
		suitCardCaption = new TSuitCardLaunch[4];
		for(int i=0; i<suitCardCaption.Length; i++) {
			suitCardCaption[i].Init(mSelf.Duplicate(itemSuitCardLaunch), scrollViewCaption.transform);
			suitCardCaption[i].LabelCount = suitCardCaption[i].mSelf.transform.Find("CountLabel").GetComponent<UILabel>();
			for(int j=0; j<suitCardCaption[i].LabelBonus.Length; j++)
				suitCardCaption[i].LabelBonus[j] = suitCardCaption[i].mSelf.transform.Find("BonusLabel" + j.ToString()).GetComponent<UILabel>();
			
			suitCardCaption[i].BG = suitCardCaption[i].mSelf.transform.Find("BG").GetComponent<UISprite>();
			suitCardCaption[i].SpriteIcon = suitCardCaption[i].mSelf.transform.Find("LabelIcon").GetComponent<UISprite>();
		}
		refreshCaption(index);
	}

	private void refreshCaption (int index) {
		if(index >=0 && index < itemSuitCards.Count) {
			TSuitCardGroup suitCardGroup = itemSuitCards[index];
			for(int i=0; i<suitCardCaption.Length; i++) {
				labelCaptionSuitName.text = suitCardGroup.SuitCard.SuitName;
				suitCardCaption[i].UpdateView(i, suitCardGroup.SuitCard.StarNum[i],
					suitCardGroup.SuitCard.AttrKind1[i],
					suitCardGroup.SuitCard.Value1[i],
					suitCardGroup.SuitCard.AttrKind2[i],
					suitCardGroup.SuitCard.Value2[i]
				);

				if(suitCardGroup.IsAllGet) 
					suitCardCaption[i].SetNowOpen (suitCardGroup.SuitCardGroupValue.ExecuteLevel == i);
				else
					suitCardCaption[i].SetNowOpen (false);
			}
			viewCaption.SetActive(true);
		}
	}

	//go.name = index (可以取itemSuitCards)
	public void OnCaption (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name, out result)) {
			if(suitCardCaption == null) 
				createCaption(result);
			else
				refreshCaption(result);
		}
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
            TSuitCardResult result = JsonConvertWrapper.DeserializeObject <TSuitCardResult>(www.text); 
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
            TSuitCardResult result = JsonConvertWrapper.DeserializeObject <TSuitCardResult>(www.text); 
			GameData.Team.SuitCardCost = result.SuitCardCost;
			GameData.Team.Player.SetAttribute(GameEnum.ESkillType.Player);

			refreshUI ();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}


}
