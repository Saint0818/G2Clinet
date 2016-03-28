﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using GameEnum;

public class TItemSuitAvatarGroup {
	public GameObject mSelf;

	public UILabel SuitNameLabel;
	public UILabel CountLabel;
	public UISprite PositionIcon;
	public GameObject Select;

	public void Init (GameObject obj, GameObject parent, UIEventListener.VoidDelegate btnFun) {
		mSelf = obj;
		obj.transform.parent = parent.transform;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = Vector3.zero;
		SuitNameLabel = obj.transform.Find("SuitNameLabel").GetComponent<UILabel>();
		CountLabel = obj.transform.Find("CountLabel").GetComponent<UILabel>();
		PositionIcon = obj.transform.Find("PositionIcon").GetComponent<UISprite>();
		Select = obj.transform.Find("Select").gameObject;

		Select.SetActive(false);
		UIEventListener.Get(obj).onClick = btnFun;
	}

	public void UpdateView (int id, int index) {
		if(GameData.DSuitItem.ContainsKey(id)) {
			mSelf.name = id.ToString();
			mSelf.transform.localPosition = new Vector3(0, -60 * index, 0);
			SuitNameLabel.text = GameData.DSuitItem[id].SuitName;
			CountLabel.text = GameData.Team.SuitItemCompleteCount(id).ToString() + "/" + GameData.DSuitItem[id].ItemLength;
			PositionIcon.spriteName = GameFunction.PositionIcon(GameData.DSuitItem[id].Position);
		} 
	}

	public bool SelectActive {
		get {return Select.activeSelf;}
		set {Select.SetActive(value);}
	}
}

public struct TMiddleItemView {
	public ItemAwardGroup[] itemAwardGroup;
	public UILabel[] itemNameLabel;
	public GameObject[] SuitCover;

	private int mID;

	public void Init (GameObject obj) {
		itemAwardGroup = new ItemAwardGroup[7];
		itemNameLabel = new UILabel[7];
		SuitCover = new GameObject[7];
		for (int i=0; i<itemAwardGroup.Length; i++) {
			itemAwardGroup[i] = obj.transform.Find(i.ToString() + "/View/ItemAwardGroup").GetComponent<ItemAwardGroup>();
			itemNameLabel[i] = obj.transform.Find(i.ToString() + "/ItemNameLabel").GetComponent<UILabel>();
			SuitCover[i] = obj.transform.Find(i.ToString() + "/SuitCover").gameObject;
		}
	}

	//id是指套裝id
	public void UpdateView (int id) {
		if(GameData.DSuitItem.ContainsKey(id)) {
			mID = id;
			if(GameData.DSuitItem[id].Items.Length == itemAwardGroup.Length) {
				for (int i=0; i<itemAwardGroup.Length; i++) {
					if(GameData.DSuitItem[id].Items[i] != 0) {

						if(GameData.DItemData.ContainsKey(GameData.DSuitItem[id].Items[i])) {
							itemAwardGroup[i].Show(GameData.DItemData[GameData.DSuitItem[id].Items[i]]);
							itemNameLabel[i].text = GameData.DItemData[GameData.DSuitItem[id].Items[i]].Name;
							SuitCover[i].SetActive(!GameData.Team.IsGetAvatar(GameData.DSuitItem[id].Items[i]));
						}
					} else {
						itemAwardGroup[i].Hide();
						itemNameLabel[i].text = TextConst.S(8207);
					}
				}
			}
		} else 
			Debug.LogError("SuitItem can't find id:"+ id);
	}
}

public struct TMiddleBonusView {
	public UILabel[] BonusLabel;

	public void Init (GameObject obj) {
		BonusLabel = new UILabel[6];
		for (int i=0; i<BonusLabel.Length; i++) {
			BonusLabel[i] = obj.transform.Find(i.ToString()).GetComponent<UILabel>();
		}
	}

	public void UpdateView (int id) {
		if(GameData.DSuitItem.ContainsKey(id)) {
			if(GameData.DSuitItem[id].AttrKind.Length == BonusLabel.Length) {
				for(int i=0; i<BonusLabel.Length; i++) {
					BonusLabel[i].text = string.Format(TextConst.S(8205), i+2, TextConst.S(10500 + GameData.DSuitItem[id].AttrKind[i]) , GameData.DSuitItem[id].Value[i]);
					BonusLabel[i].gameObject.SetActive((GameData.DSuitItem[id].AttrKind[i] != 0 && GameData.DSuitItem[id].Value[i] != 0));
				}
			}
		}
	}

	private void darkAllBonus () {
		for(int i=0; i<BonusLabel.Length; i++) {
			BonusLabel[i].color = NotGetIt;
		}
	}

	public void SetColor (int count) {
		if(count > 1) {
			for(int i=0; i<BonusLabel.Length; i++) {
				if((i+2) <= count) 
					BonusLabel[i].color = GetIt;
				else 
					BonusLabel[i].color = NotGetIt;
			}
		} else 
			darkAllBonus();
	}

	public Color32 GetIt {
		get {return new Color32 (255, 0, 255, 255);}
	}

	public Color32 NotGetIt {
		get {return new Color32 (120, 0, 120, 255);}
	}
}

public struct TSuitItemRight {
	public GameObject NoCardLabel;
	public UILabel CostCaptionLabel;
	public TPassiveSkillCard[] itemCards;

	public void Init (GameObject obj) {
		NoCardLabel = obj.transform.Find("NoCardLabel").gameObject;
		CostCaptionLabel = obj.transform.Find("CostCaptionLabel").GetComponent<UILabel>();
		itemCards = new TPassiveSkillCard[5];
		for(int i=0; i<itemCards.Length; i++) {
			itemCards[i] = new TPassiveSkillCard();
			itemCards[i].InitSuitItem(obj.transform.Find("CardsView/" +i.ToString()).gameObject, ClickCard);
		}
		NoCardLabel.SetActive(false);
	}

	public void ClickCard (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name, out result)) {
			UIItemHint.Get.OnShowForSuit(result);
		}
	}
		

	private void hideAllCard () {
		for(int i=0; i<itemCards.Length; i++)
			itemCards[i].Enable = false;
	}

	public void UpdateView (int id) {
		if(GameData.DSuitItem.ContainsKey(id)) {
			NoCardLabel.SetActive((GameData.DSuitItem[id].CardLength == 0));
			CostCaptionLabel.gameObject.SetActive((GameData.DSuitItem[id].CardLength != 0));
			if(GameData.DSuitItem[id].CardLength == 0) {
				hideAllCard ();
			} else {
				if(itemCards.Length == GameData.DSuitItem[id].Card.Length) {
					for(int i=0; i<itemCards.Length; i++) {
						if(GameData.DSuitItem[id].Card[i] == 0)
							itemCards[i].Enable = false;
						else {
							itemCards[i].Enable = true;
							itemCards[i].UpdateViewSuitItem(GameData.DSuitItem[id].Card[i]);
						}
					}
					CostCaptionLabel.text = string.Format(TextConst.S(8203), (GameData.Team.SuitItemCompleteCount(id) / 2));//目前已取得的件數
				}
			}
		} else 
			Debug.LogError("SuitItem can't find id:"+ id);
	}
}

public class UISuitAvatar : UIBase {
	private static UISuitAvatar instance = null;
	private const string UIName = "UISuitAvatar";

	private GameObject itemAward;

    private Dictionary<int,TItemSuitAvatarGroup>  tItemSuitAvatarGroup = new Dictionary<int, TItemSuitAvatarGroup>();

	private UIScrollView leftScorllView;
	private TMiddleItemView middleItemView;
	private TMiddleBonusView middleBonusView;
	private TSuitItemRight suitItemRight;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

		set {
			if (instance) {
				if (!value)
                    RemoveUI(instance.gameObject);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}

	public static UISuitAvatar Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISuitAvatar;

			return instance;
		}
	}

	protected override void InitCom() {
		itemAward = Resources.Load(UIPrefabPath.ItemSuitAvatarGroup) as GameObject;

		leftScorllView = GameObject.Find(UIName + "/Window/Center/Left/ScrollView").GetComponent<UIScrollView>();
		middleItemView = new TMiddleItemView();
		middleItemView.Init(GameObject.Find(UIName + "/Window/Center/Middle/ItemsView"));
		middleBonusView = new TMiddleBonusView();
		middleBonusView.Init(GameObject.Find(UIName + "Window/Center/Middle/BonusLabelView"));
		suitItemRight = new TSuitItemRight();
		suitItemRight.Init(GameObject.Find(UIName + "/Window/Center/Right"));

		SetBtnFun(UIName + "/BG/NoBtn", OnClose);
	}

	public void OnClose () {
		Visible = false;
	}

	public void ShowView (int suitItemID = 1) {
		if(LimitTable.Ins.HasByOpenID(EOpenID.SuitItem) && GameData.Team.Player.Lv >= LimitTable.Ins.GetLv(EOpenID.SuitItem)) {
			Visible = true;
			initScrollView ();
			clickSuit (suitItemID);
			middleBonusView.SetColor(GameData.Team.SuitItemCompleteCount(suitItemID));
		} else 
			UIHint.Get.ShowHint(string.Format(TextConst.S(512),LimitTable.Ins.GetLv(EOpenID.SuitItem)) , Color.red);
	}

	private void initScrollView () {
		int index = 0;
		foreach(KeyValuePair<int, TSuitItem> item in GameData.DSuitItem) {
			TItemSuitAvatarGroup itemsuitItem = new TItemSuitAvatarGroup();
			itemsuitItem.Init(Instantiate(itemAward), leftScorllView.gameObject, OnClickSuit);
			itemsuitItem.UpdateView(item.Key, index);
			tItemSuitAvatarGroup.Add(item.Key, itemsuitItem);
			index ++;
		}
		leftScorllView.Scroll(0);
	}

	private void hideAllSelect () {
		foreach(KeyValuePair<int, TItemSuitAvatarGroup> item in tItemSuitAvatarGroup) {
			tItemSuitAvatarGroup[item.Key].SelectActive = false;
		}
	}

	private void clickSuit (int id) {
		middleItemView.UpdateView(id);
		middleBonusView.UpdateView(id);
		suitItemRight.UpdateView(id);
		middleBonusView.SetColor(GameData.Team.SuitItemCompleteCount(id));
		hideAllSelect();
		if(tItemSuitAvatarGroup.ContainsKey(id))
			tItemSuitAvatarGroup[id].SelectActive = true;

		if(id >= 200)
			leftScorllView.Scroll(-0.5f);
		else if(id >= 100)
			leftScorllView.Scroll(-0.25f);
		else 
			leftScorllView.Scroll(0);
	} 

	public void OnClickSuit (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name, out result)) {
			clickSuit(result);
		}
	}

	void OnDestroy() {
        if (tItemSuitAvatarGroup != null)
		    tItemSuitAvatarGroup.Clear();
	}
}
