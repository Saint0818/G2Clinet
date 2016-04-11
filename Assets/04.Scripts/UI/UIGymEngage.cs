using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;

public class TITemGymObj {
	public GameObject ItemObj;
	public GameObject View;
	public UILabel Name;
	public GameObject Selected;
	public GameObject Lock;
	public UILabel Condition;
	public GameObject Buy;
	public UILabel Price;

	public string BuildName;

	public void Init (GameObject go, string name) {
		ItemObj = go;
		View = go.transform.Find("3DView").gameObject;
		Name = go.transform.Find("NameLabel").GetComponent<UILabel>();
		Selected = go.transform.Find("Selected").gameObject;
		Lock = go.transform.Find("Lock").gameObject;
		Condition = go.transform.Find("Lock/ConditionLabel").GetComponent<UILabel>();
		Buy = go.transform.Find("Buy").gameObject;
		Price = go.transform.Find("Buy/Btn/BtnLabel").GetComponent<UILabel>();

		BuildName = name;
	}

	//先判斷等級是否達到，再判斷是否可以購買，最後判斷是否已購買 isOwn是否有擁有
	public void UpdateUI(TItemData data, bool isSelect, bool isOwn) {
		ItemObj.transform.localScale = Vector3.one;
//		GameObject go = UIPrefabPath.LoadUI("Prefab/Stadium/StadiumItem/" + BuildName + data.Avatar, View.transform, Vector3.zero);
//		LayerMgr.Get.SetLayer(go, ELayer.UI);
//		go.transform.parent = View.transform;
//		go.transform.localScale = Vector3.one;
		Lock.SetActive(GameData.Team.HighestLv < data.LV);
		Buy.SetActive(!Lock.activeSelf);
		if(isOwn) 
			Buy.SetActive(false);
		
		if(GameData.Team.IsGetItem(data.ID)) {
			Buy.SetActive(false);	
		}
		Name.text = data.Name;
		Price.text = data.Buy.ToString();

		Selected.SetActive(isSelect);
		Condition.text =  string.Format(TextConst.S(11021) ,data.LV.ToString());
	}

}

public struct TArchitectureValue {
	public float Time;
	public int SpendKind;
	public int Cost;
	public int AttrKind;
	public float AttrValue;
}

public class UIGymEngage : UIBase {
	private static UIGymEngage instance = null;
	private const string UIName = "UIGymEngage";

	private const int TimeDiamondD2 = 1;
	private const int TimeDiamondD3 = 20;
	private const int TimeDiamondD4 = 260;
	private const int TimeDiamondD5 = 1000;

	private GameObject itemBuild;// Resource.Load
	private GameObject window;
	private GameObject windowNormal;
	private GameObject windowHighest;

	private UILabel labelBuildName;

	//Highest Level
	private UILabel labelHighestLV;
	private UILabel labelHighestEffect;

	//Normal Level
	private UILabel labelNowLevel;
	private UILabel labelNowAttr;
	private UILabel labelNextLevel;
	private UILabel labelNextAttr;

	private GameObject goUpgrade;
	private UILabel labelUpgradePrice;
	private UISprite spriteUpgradeIcon;
	private UILabel labelUpgradeCD;

	private GameObject goBuyCD;
	private UISlider sliderCDBar;
	private UILabel labelTime;
	private UILabel labelPrice;
//	private UISprite spriteBuyCDIcon;

	private ItemAwardGroup[] awardGroup = new ItemAwardGroup[2];

	private TArchitectureValue architectureValue = new TArchitectureValue();
	private TArchitectureValue architectureNextValue = new TArchitectureValue();

	private List<TITemGymObj> itemGymObjs = new List<TITemGymObj>();

	private UIScrollView scrollView;

	private int mBuildIndex;

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

	public static UIGymEngage Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGymEngage;

			return instance;
		}
	}

	void FixedUpdate () {
		updateCDUI();
	}

	protected override void InitCom() {
		itemBuild = Resources.Load(UIPrefabPath.ItemGymEngage) as GameObject;
		window = GameObject.Find(UIName + "/Window");
		windowNormal = GameObject.Find(UIName + "/Window/Center/MainView/Normal");
		windowHighest = GameObject.Find(UIName + "/Window/Center/MainView/Highest");

		labelBuildName = GameObject.Find (UIName + "/Window/Center/MainView/NameLabel").GetComponent<UILabel>();

		//Highest Lv
		labelHighestLV = GameObject.Find (UIName + "/Window/Center/MainView/Highest/Info/HighestLV").GetComponent<UILabel>();
		labelHighestEffect = GameObject.Find (UIName + "/Window/Center/MainView/Highest/Info/HighestEffect").GetComponent<UILabel>();

		//Normal Lv
		labelNowLevel = GameObject.Find (UIName + "/Window/Center/MainView/Normal/Info/NowLV").GetComponent<UILabel>();
		labelNowAttr = GameObject.Find (UIName + "/Window/Center/MainView/Normal/Info/NowEffect").GetComponent<UILabel>();
		labelNextLevel = GameObject.Find (UIName + "/Window/Center/MainView/Normal/Info/NextLV").GetComponent<UILabel>();
		labelNextAttr = GameObject.Find (UIName + "/Window/Center/MainView/Normal/Info/NextEffect").GetComponent<UILabel>();

		goUpgrade = GameObject.Find (UIName + "/Window/Center/MainView/Normal/UpgradeBtn");
		labelUpgradePrice = GameObject.Find (UIName + "/Window/Center/MainView/Normal/UpgradeBtn/PriceLabel").GetComponent<UILabel>();
		spriteUpgradeIcon = GameObject.Find (UIName + "/Window/Center/MainView/Normal/UpgradeBtn/PriceLabel/Icon").GetComponent<UISprite>();
		labelUpgradeCD = GameObject.Find (UIName + "/Window/Center/MainView/Normal/UpgradeBtn/CD/CDLabel").GetComponent<UILabel>();

		goBuyCD = GameObject.Find (UIName + "/Window/Center/MainView/Normal/BuyCDBtn");
		sliderCDBar = GameObject.Find (UIName + "/Window/Center/MainView/Normal/BuyCDBtn/CDBar").GetComponent<UISlider>();
		labelTime = GameObject.Find (UIName + "/Window/Center/MainView/Normal/BuyCDBtn/CDBar/TimeLabel").GetComponent<UILabel>();
		labelPrice = GameObject.Find (UIName + "/Window/Center/MainView/Normal/BuyCDBtn/Btn/PriceLabel").GetComponent<UILabel>();
//		spriteBuyCDIcon = GameObject.Find (UIName + "/Window/Center/MainView/Normal/BuyCDBtn/Btn/Icon").GetComponent<UISprite>();

		for(int i=0; i<awardGroup.Length; i++) 
			awardGroup[i] = GameObject.Find (UIName + "/Window/Center/MainView/Normal/NextCondition/View/ItemAwardGroup" + i.ToString()).GetComponent<ItemAwardGroup>();

		scrollView = GameObject.Find (UIName + "/Window/Bottom/BuildingView/ScrollView").GetComponent<UIScrollView>();

		SetBtnFun (UIName + "/Window/BottomLeft/BackBtn", OnClose);
		SetBtnFun (UIName + "/Window/Center/MainView/Normal/UpgradeBtn", OnUpgrade);
		SetBtnFun (UIName + "/Window/Center/MainView/Normal/BuyCDBtn", OnBuyCD);
	}

	public void OnClose () {
		window.SetActive(false);
		if(UI3DMainLobby.Visible)
			UI3DMainLobby.Get.mImpl.OnSelect(mBuildIndex);
		StartCoroutine(showGymCenter());
	}

	private IEnumerator showGymCenter() {
		yield return new WaitForSeconds(1);
		UIGym.Get.CenterVisible = true;
		Visible = false;
	}

	public void OnUpgrade () {
		if(getIdleIndex == -1) {
			UIHint.Get.ShowHint(TextConst.S(11015), Color.red);
		} else {
			if(mBuildIndex >= 0 && mBuildIndex < GameData.Team.GymBuild.Length && architectureValue.Cost > 0) {
				if(architectureValue.SpendKind == 0)
					CheckDiamond(architectureValue.Cost, true, string.Format(TextConst.S(11014), architectureValue.Cost), SendUpdateBuild, refreshLabelColor);
				else if(architectureValue.SpendKind == 1)
					CheckMoney(architectureValue.Cost, true, string.Format(TextConst.S(11014), architectureValue.Cost), SendUpdateBuild, refreshLabelColor);
			}
		}
	}

	public void OnBuyCD () {
		if(getNowCDIndex(mBuildIndex) != -1) {
			if(mBuildIndex >= 0 && mBuildIndex < GameData.Team.GymBuild.Length) {
				int price = computeTimeDiamond(secToMin((int)GameData.Team.GymBuild[mBuildIndex].Time.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds));
				CheckDiamond(price, true, string.Format(TextConst.S(11010), price), SendBuyBuildCD, refreshLabelColor);
			}
		}
	}

	public void ShowView (int index) {
		Visible = true;
		mBuildIndex = index;
		setTitle(index);
		refreshUI(index);
		setScrollView ();
	}

	private void refreshUI (int index) {
		if(isHighestLevel (index)) {
			windowHighest.SetActive(true);
			windowNormal.SetActive(false);
			setTempValue(index, true);
			setHighLvInfo(index);
		} else {
			windowHighest.SetActive(false);
			windowNormal.SetActive(true);
			goUpgrade.SetActive(false);
			goBuyCD.SetActive(false);
			setTempValue(index, false);
			setInfo(index);
			setUpgrade();
		}
	}

	private void setTitle (int index) {
		if(index >= 0 && index < GameData.Team.GymBuild.Length) 
			labelBuildName.text = GameFunction.GetBuildName(index);
	}

	private void setHighLvInfo(int index) {
		if(index >= 0 && index < GameData.Team.GymBuild.Length && GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV)) {
			labelHighestLV.text = TextConst.S(11012);
			if(index == 3) { //gym
				labelHighestEffect.text = string.Format(TextConst.S(11023), architectureValue.AttrValue);
			} else {
				if(architectureValue.AttrValue > 0)
					labelHighestEffect.text = TextConst.S(10500 + architectureValue.AttrKind) +"[00ff00] + "+ architectureValue.AttrValue +"[-]";
				else 
					labelHighestEffect.text = TextConst.S(10500 + architectureValue.AttrKind) +"[ff0000] "+ architectureValue.AttrValue +"[-]";
			}
		}
	}

	private void setInfo (int index) {
		if(index >= 0 && index < GameData.Team.GymBuild.Length) {
			labelNowLevel.text = string.Format(TextConst.S(11021), GameData.Team.GymBuild[index].LV);
			labelNextLevel.text = string.Format(TextConst.S(11021), GameData.Team.GymBuild[index].LV + 1);
			if(index == 3) { //gym
				labelNowAttr.text = string.Format(TextConst.S(11023), architectureValue.AttrValue);
				labelNextAttr.text = string.Format(TextConst.S(11023), architectureNextValue.AttrValue);
			} else {
				if(architectureValue.AttrValue > 0)
					labelNowAttr.text = TextConst.S(10500 + architectureValue.AttrKind) +"[00ff00] + "+ architectureValue.AttrValue +"[-]";
				else 
					labelNowAttr.text = TextConst.S(10500 + architectureValue.AttrKind) +"[ff0000] "+ architectureValue.AttrValue +"[-]";
				
				if(architectureNextValue.AttrValue > 0)
					labelNextAttr.text = TextConst.S(10500 + architectureNextValue.AttrKind) +"[00ff00] + "+ architectureNextValue.AttrValue +"[-]";
				else 
					labelNextAttr.text = TextConst.S(10500 + architectureNextValue.AttrKind) +"[ff0000] "+ architectureNextValue.AttrValue +"[-]";
			}
		}
	}

	private void setUpgrade() {
		awardGroup[0].gameObject.SetActive(false);
		awardGroup[1].gameObject.SetActive(false);
		if(!IsWaitCD) {
			goUpgrade.SetActive(true);
			goBuyCD.SetActive(false);
			labelUpgradePrice.text = architectureValue.Cost.ToString();
			spriteUpgradeIcon.spriteName = GameFunction.SpendKindTexture(architectureValue.SpendKind);
			labelUpgradeCD.text = timeConvert(architectureValue.Time);

		} else {
			goUpgrade.SetActive(false);
			goBuyCD.SetActive(true);
			
		}
			
		refreshLabelColor ();
	}

	private void updateCDUI () {
		if(goBuyCD.activeSelf && mBuildIndex >= 0 && mBuildIndex < GameData.Team.GymBuild.Length) {
			sliderCDBar.value = TextConst.DeadlineStringPercent(GameFunction.GetOriTime(mBuildIndex, GameFunction.GetBuildLv(mBuildIndex) - 1, GameFunction.GetBuildTime(mBuildIndex).ToUniversalTime()),  GameFunction.GetBuildTime(mBuildIndex).ToUniversalTime());
			labelTime.text = TextConst.SecondString((int)(new System.TimeSpan(GameData.Team.GymBuild[mBuildIndex].Time.ToUniversalTime().Ticks - DateTime.UtcNow.Ticks).TotalSeconds));
			int price = computeTimeDiamond(secToMin((int)GameData.Team.GymBuild[mBuildIndex].Time.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds));
			labelPrice.text = price.ToString();
			labelPrice.color = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(0, price), 0);
		}
	}

	private bool IsWaitCD {
		get {
			for(int i=0; i<GameData.Team.GymQueue.Length; i++)
				if(GameData.Team.GymQueue[i].BuildIndex == mBuildIndex) 
					return true;
			
			return false;
		}
	}

	private void setScrollView () {
		int kind = 51 + mBuildIndex;
		if(GameData.DBuildData.ContainsKey(kind)) 
			for(int i=0; i<GameData.DBuildData[kind].Count; i++) 
				itemGymObjs.Add(addItems(i, GameData.DBuildData[kind][i]));
			
	}

	private TITemGymObj addItems (int index, TItemData data) {
		bool isSelect = false;
		if(mBuildIndex >= 0 && mBuildIndex < GameData.Team.GymBuild.Length)
			isSelect = (GameData.Team.GymBuild[mBuildIndex].Type == index);
		
		GameObject go = Instantiate(itemBuild) as GameObject;
		go.transform.parent = scrollView.gameObject.transform;
		go.transform.localPosition = new Vector3(170 * index, 0, 0);
		TITemGymObj obj = new TITemGymObj();
		obj.Init(go, GetBuildEnName(mBuildIndex));
		obj.UpdateUI(data, isSelect, IsOwn(mBuildIndex, index));
		UIEventListener.Get(go).onClick = OnChooseBuildType;
		return obj;
	}

	public void OnChooseBuildType (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name, out result)) {
			if(result >= 0 && result < itemGymObjs.Count) {
				
			}
//			itemGymObjs[result]	
		}
	}

	private void refreshLabelColor () {
		labelUpgradePrice.color = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(architectureValue.Cost, architectureValue.SpendKind), architectureValue.SpendKind);
	}

	private int secToMin(int Sec){
		decimal temp =  (decimal)Sec / 60;
		return Convert.ToInt16(Math.Ceiling(temp));
	}

	private int computeTimeDiamond(int time) {
		//		1	1	0	60
		//		(D3-D2)/3540*(T-60)+D2	     20	      60	3600
		//		(D4-D3)/82800*(T-3600)+D3	260	    3600	86400
		//		(D5-D4)/518400*(T-86400)+D4	1000	86400	

		float temp;
		time = time * 60;
		if(time <= 60){
			return TimeDiamondD2;
		}else if(time > 60 && time <= 3600){
			temp = TimeDiamondD3;
			return Convert.ToInt32((temp - TimeDiamondD2) / 3540 * (time - 60) + TimeDiamondD2);
		}else if(time > 3600 && time <= 86400){
			temp = TimeDiamondD4;
			return Convert.ToInt32((temp - TimeDiamondD3) / 82800 * (time - 3600) + TimeDiamondD3);
		}else{
			temp = TimeDiamondD5;
			return Convert.ToInt32((temp - TimeDiamondD4) / 518400 * (time - 86400) + TimeDiamondD4);
		}
	}

	private string GetBuildEnName (int index) {
		switch (index) {
		case 0:
			return "Basket";
		case 1:
			return "Advertisement";
		case 2:
			return "Store";
		case 3:
			return "Gym";
		case 4:
			return "Door";
		case 5:
			return "Logo";
		case 6:
			return "Chair";
		case 7:
			return "Calendar";
		case 8:
			return "Mail";
		}
		return "";
	}

	private bool IsOwn (int buildIndex, int index) {
		if(buildIndex >= 0 && buildIndex < GameData.Team.GymOwn.Length) 
			if(index < GameData.Team.GymOwn[buildIndex])
				return true;
		
		return false;
	}

	private bool isHighestLevel (int index) {
		if(index >= 0 && index < GameData.Team.GymBuild.Length  && index < GameData.DBuildHightestLvs.Length) {
			return (GameData.Team.GymBuild[index].LV >= GameData.DBuildHightestLvs[index]);
		}
		return true;
	}

	private string timeConvert (float time) {
		int d = 0;
		int m = 0; 
		int h = 0; 
		string sResult = "";

		if (time.ToString().IndexOf(".") == -1) {
			d = (int) (time / 24);
			h = (int) (time % 24);
		} else {
			float floor = Mathf.Floor(time);
			d = (int) (floor / 24);
			h = (int) (floor % 24);
			floor = time - floor;
			m = (int)(floor * 60);
		}

		if (d > 0)
			sResult = string.Format(TextConst.S(243), d, h);
		else 
			if (h > 0)
				sResult = string.Format(TextConst.S(244), h, m);
			else
				if (m > 0)
					sResult = string.Format(TextConst.S(261), m);
		return sResult;
	}

	private void setTempValue (int index, bool isHighest) {
		if(index >= 0 && index < GameData.Team.GymBuild.Length && 
			GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV)) {
			switch (index) {
			case 0://Basket
				architectureValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].BasketAttrKind;
				architectureValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].BasketAttrValue;
				architectureValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].BasketCost;
				architectureValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].BasketSpendKind;
				architectureValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].BasketTime;

				if(GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV + 1) && !isHighest) {
					architectureNextValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].BasketAttrKind;
					architectureNextValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].BasketAttrValue;
					architectureNextValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].BasketCost;
					architectureNextValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].BasketSpendKind;
					architectureNextValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].BasketTime;
				}
				break;
			case 1://Ad
				architectureValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].AdAttrKind;
				architectureValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].AdAttrValue;
				architectureValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].AdCost;
				architectureValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].AdSpendKind;
				architectureValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].AdTime;

				if(GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV + 1) && !isHighest) {
					architectureNextValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].AdAttrKind;
					architectureNextValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].AdAttrValue;
					architectureNextValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].AdCost;
					architectureNextValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].AdSpendKind;
					architectureNextValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].AdTime;
				}
				break;
			case 2://Store
				architectureValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].StoreAttrKind;
				architectureValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].StoreAttrValue;
				architectureValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].StoreCost;
				architectureValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].StoreSpendKind;
				architectureValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].StoreTime;

				if(GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV + 1) && !isHighest) {
					architectureNextValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].StoreAttrKind;
					architectureNextValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].StoreAttrValue;
					architectureNextValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].StoreCost;
					architectureNextValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].StoreSpendKind;
					architectureNextValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].StoreTime;
				}
				break;
			case 3://Gym
				architectureValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].GymAttrKind;
				architectureValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].GymAttrValue;
				architectureValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].GymCost;
				architectureValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].GymSpendKind;
				architectureValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].GymTime;

				if(GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV + 1) && !isHighest) {
					architectureNextValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].GymAttrKind;
					architectureNextValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].GymAttrValue;
					architectureNextValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].GymCost;
					architectureNextValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].GymSpendKind;
					architectureNextValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].GymTime;
				}
				break;
			case 4://Door
				architectureValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].DoorAttrKind;
				architectureValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].DoorAttrValue;
				architectureValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].DoorCost;
				architectureValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].DoorSpendKind;
				architectureValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].DoorTime;

				if(GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV + 1) && !isHighest) {
					architectureNextValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].DoorAttrKind;
					architectureNextValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].DoorAttrValue;
					architectureNextValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].DoorCost;
					architectureNextValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].DoorSpendKind;
					architectureNextValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].DoorTime;
				}
				break;
			case 5://Logo
				architectureValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].LogoAttrKind;
				architectureValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].LogoAttrValue;
				architectureValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].LogoCost;
				architectureValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].LogoSpendKind;
				architectureValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].LogoTime;

				if(GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV + 1) && !isHighest) {
					architectureNextValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].LogoAttrKind;
					architectureNextValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].LogoAttrValue;
					architectureNextValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].LogoCost;
					architectureNextValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].LogoSpendKind;
					architectureNextValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].LogoTime;
				}
				break;
			case 6://Chair
				architectureValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].ChairAttrKind;
				architectureValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].ChairAttrValue;
				architectureValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].ChairCost;
				architectureValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].ChairSpendKind;
				architectureValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].ChairTime;

				if(GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV + 1) && !isHighest) {
					architectureNextValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].ChairAttrKind;
					architectureNextValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].ChairAttrValue;
					architectureNextValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].ChairCost;
					architectureNextValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].ChairSpendKind;
					architectureNextValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].ChairTime;
				}
				break;
			case 7://Calendar
				architectureValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].CalendarAttrKind;
				architectureValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].CalendarAttrValue;
				architectureValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].CalendarCost;
				architectureValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].CalendarSpendKind;
				architectureValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].CalendarTime;

				if(GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV + 1) && !isHighest) {
					architectureNextValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].CalendarAttrKind;
					architectureNextValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].CalendarAttrValue;
					architectureNextValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].CalendarCost;
					architectureNextValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].CalendarSpendKind;
					architectureNextValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].CalendarTime;
				}
				break;
			case 8://Mail
				architectureValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].MailAttrKind;
				architectureValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].MailAttrValue;
				architectureValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].MailCost;
				architectureValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].MailSpendKind;
				architectureValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV].MailTime;

				if(GameData.DArchitectureExp.ContainsKey(GameData.Team.GymBuild[index].LV + 1) && !isHighest) {
					architectureNextValue.AttrKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].MailAttrKind;
					architectureNextValue.AttrValue = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].MailAttrValue;
					architectureNextValue.Cost = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].MailCost;
					architectureNextValue.SpendKind = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].MailSpendKind;
					architectureNextValue.Time = GameData.DArchitectureExp[GameData.Team.GymBuild[index].LV + 1].MailTime;
				}
				break;
			}
		}
	}

	private int getIdleIndex {
		get {
			for(int i=0; i<GameData.Team.GymQueue.Length; i++) 
				if(GameData.Team.GymQueue[i].IsOpen && GameData.Team.GymQueue[i].BuildIndex == -1)
					return i;
			
			return -1;
		}
	}

	private int getNowCDIndex (int buildIndex) {
		for(int i=0; i<GameData.Team.GymQueue.Length; i++) 
			if(GameData.Team.GymQueue[i].IsOpen && GameData.Team.GymQueue[i].BuildIndex == buildIndex)
				return i;

		return -1;
	}

	private void SendUpdateBuild () {
		WWWForm form = new WWWForm();
		form.AddField("Index", getIdleIndex);
		form.AddField("BuildIndex", mBuildIndex);
		SendHttp.Get.Command(URLConst.GymUpdateBuild, waitUpdateBuild, form);
	}

	private void waitUpdateBuild(bool ok, WWW www) {
		if (ok) {
			TGymResult result = JsonConvert.DeserializeObject <TGymResult>(www.text, SendHttp.Get.JsonSetting); 
			GameData.Team.Money = result.Money;
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.GymBuild = result.GymBuild;
			GameData.Team.GymQueue = result.GymQueue;

			UIMainLobby.Get.UpdateUI();
			refreshUI(mBuildIndex);
			UIGym.Get.RefreshQueue();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}

	private void SendBuyBuildCD () {
		WWWForm form = new WWWForm();
		form.AddField("Index", getNowCDIndex(mBuildIndex));
		form.AddField("BuildIndex", mBuildIndex);
		SendHttp.Get.Command(URLConst.GymBuyCD, waitBuyBuildCD, form);
	}

	private void waitBuyBuildCD(bool ok, WWW www) {
		if (ok) {
			TGymResult result = JsonConvert.DeserializeObject <TGymResult>(www.text, SendHttp.Get.JsonSetting); 
			GameData.Team.Diamond = result.Diamond;
			GameData.Team.GymBuild = result.GymBuild;
			GameData.Team.GymQueue = result.GymQueue;

			UIMainLobby.Get.UpdateUI();
			refreshUI(mBuildIndex);
			UIGym.Get.RefreshQueue();
		} else {
			Debug.LogError("text:"+www.text);
		} 
	}
}
