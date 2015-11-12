using UnityEngine;
using GameStruct;

public class PersonalView
{
	private GameObject self;
	private UIButton changeHeadBtn;
	private UISprite headTex;
	private UILabel lv;
	private UILabel name;
	private UISlider expBar;
	private UILabel expValue;
	private UISprite powerBar;
	private UILabel powerValue;
	private UIButton group;
	private UILabel groupHead;
	private UILabel groupBody;
	public TValueAvater[] Avatars = new TValueAvater[8];

	public void Init(GameObject obj, GameObject[] itemEquipmentBtn)
	{
		self = obj;

		if (self) {
			changeHeadBtn = self.transform.FindChild("PlayerBt").gameObject.GetComponent<UIButton>();
			headTex = changeHeadBtn.transform.FindChild("PlayerIcon").gameObject.GetComponent<UISprite>();
			lv = changeHeadBtn.transform.FindChild("LevelLabel").gameObject.GetComponent<UILabel>();
			name = self.transform.FindChild("PlayerName/NameLabel").gameObject.GetComponent<UILabel>();
			expBar = self.transform.FindChild("EXPView/ProgressBar").gameObject.GetComponent<UISlider>();
			expValue = self.transform.FindChild("EXPView/ExpLabel").gameObject.GetComponent<UILabel>();
			powerBar = self.transform.FindChild("CombatView/CombatValue").gameObject.GetComponent<UISprite>();
			powerValue = self.transform.FindChild("CombatView/CombatLabel").gameObject.GetComponent<UILabel>();
			group = self.transform.FindChild("PlayerLeague").gameObject.GetComponent<UIButton>();
			groupHead = group.transform.FindChild("Label").gameObject.GetComponent<UILabel>();
			groupBody = group.transform.FindChild("LeagueID").gameObject.GetComponent<UILabel>();
			
			for(int i = 0; i < Avatars.Length; i++){
				GameObject go = self.transform.FindChild(string.Format("EquipmentView/PartSlot{0}/View", i)).gameObject;
				if((Avatars[i] == null || !Avatars[i].IsInit) && itemEquipmentBtn[i] != null)
				{
					Avatars[i] = new TValueAvater();
					Avatars[i].Init(itemEquipmentBtn[i], go, i);
				}
			}
		}
	}

	public void InitBtttonFunction(EventDelegate changeHeadFunc, EventDelegate groupFunc, EventDelegate itemHint)
	{
		changeHeadBtn.onClick.Add (changeHeadFunc);
		group.onClick.Add (groupFunc);

		for (int i = 0; i < Avatars.Length; i++)
			Avatars [i].InitBtttonFunction (itemHint);
	}

	public void Update(TPlayer player)
	{
		UpdatePlayerData (player);
//		UpdateAvatarData (player.EquipItems);
	}

	private void UpdateAvatarData(TEquipItem[] items)
	{
		for(int i = 0;i< Avatars.Length;i++)
		{
			if(items != null && i < items.Length)
			{
				if(GameData.DItemData.ContainsKey(items[i].ID))
				{
					Avatars[i].Enable = true;
					Avatars[i].Name = GameData.DItemData[items[i].ID].Name;
					Avatars[i].Pic = GameData.DItemData[items[i].ID].Icon;
					Avatars[i].Quality = GameData.DItemData[items[i].ID].Quality;
//					Avatars[i].Starts = items[i].Inlay.Length;
				}
			}
			else
				Avatars[i].Enable = false;
		}
	}

	private void UpdatePlayerData(TPlayer player)
	{
		name.text = player.Name;
		lv.text = player.Lv.ToString();

		if (GameData.DExpData.ContainsKey (player.Lv + 1)) 
		{
			expValue.text = string.Format ("{0}/{1}", player.Exp, GameData.DExpData[player.Lv + 1].LvExp);
			expBar.value = player.Exp / GameData.DExpData[player.Lv + 1].LvExp;
		}
		else{
			expValue.text = "Max";
			expBar.value = 1;
		}

		int count = 0;
		int average = 0;
		for (int i = 0; i < player.Potential.Length; i++)
			count += player.Potential[i];

		average = count / player.Potential.Length;
		powerValue.text = average.ToString();
		powerBar.fillAmount = average / 100;
	}
}

public class AbilityView
{
	private GameObject self;
	private UIButton skillPointBtn;
	private TAbilityItem[] Masteries = new TAbilityItem[12];
	private UIAttributes hexagon;

	public void Init(GameObject obj, GameObject hexgonObj)
	{
		self = obj;
		if(self)
		{
			GameObject go;
			for (int i = 0; i < Masteries.Length; i++) {
				Masteries[i] = new TAbilityItem();
				go = self.transform.FindChild(string.Format("AttrGroup/AttrKind{0}", i)).gameObject;
				Masteries[i].Init(go, i);
			}

			skillPointBtn = self.transform.FindChild("SkillPointBtn").gameObject.GetComponent<UIButton>();

			GameObject hexagonCenter = GameObject.Find("AttributeHexagonCenter").gameObject;
			hexagon = hexgonObj.GetComponent<UIAttributes>();
			hexagon.transform.parent = hexagonCenter.transform;
			hexagon.transform.localPosition = Vector3.zero;
		}
	}

	public void UpdateMasteries(int[] indexs)
	{
		if(Masteries.Length == indexs.Length){
			for(int i = 0;i < indexs.Length;i++){
				Masteries[i].Value.text = indexs[i].ToString();
			}
		}
	}

	public void InitBtttonFunction(EventDelegate skillFunc)
	{
		skillPointBtn.onClick.Add (skillFunc);
	}
}

[System.Serializable]
public class TValueAvater
{
	public int Index;
	private GameObject self;
	public UISprite BG;
	private UISprite pic;
	private UILabel name;
	public UIButton Btn;
	private UISprite[] stars = new UISprite[4];
	public GameObject Parent;
	public bool IsInit = false;

	public void Init(GameObject obj,GameObject parent, int index)
	{
		if (obj) {
			Index = index;
			self = obj;
			self.transform.parent = parent.transform;
			self.transform.localPosition = Vector3.zero;
			self.transform.localScale = Vector3.one;
			BG = self.GetComponent<UISprite>();

			pic = self.transform.FindChild("EquipmentPic").gameObject.GetComponent<UISprite>();
			name = self.transform.FindChild("NameLabel").gameObject.GetComponent<UILabel>();
			Btn = self.GetComponent<UIButton>();

			self.name = Index.ToString();

			for(int i = 0;i < stars.Length;i++)
				stars[i] = obj.transform.FindChild(string.Format("EquipmentStar/Empty{0}", i)).gameObject.GetComponent<UISprite>();

			IsInit = self || BG || Btn || stars[0] || stars[1] || stars[2] || stars[3] || pic;
		}
	}

	public void InitBtttonFunction(EventDelegate btnFunc)
	{
		if(Btn && btnFunc != null)
			Btn.onClick.Add(btnFunc);
	}

	public string Pic
	{
		set{pic.spriteName = string.Format("item_{0}", value);}
	}

	public string Name
	{
		set{name.text = value;}
	}

	public int Quality
	{
		set{pic.spriteName = string.Format("Equipment_{0:00}", value);}
	}

	public int Starts
	{
		set{
			for(int i = 0;i < stars.Length; i++)
				stars[i].enabled = i < value ? true : false;
		}
	}

	public bool Enable
	{
		set{self.SetActive(value);}
	}
}

public class TAbilityItem
{
	private GameObject go; 
	private UISprite pic;

	public int index;
	public UILabel Value;
	public bool IsInit = false;

	public void Init(GameObject obj, int index)
	{
		if (obj) {
			go = obj;
			pic = go.GetComponent<UISprite> ();
			pic.spriteName = string.Format ("AttrKind_{0}", index + 1);
			Value = go.transform.FindChild("ValueBaseLabel").gameObject.GetComponent<UILabel>();

			IsInit = go || pic || Value;
		}
	}
}

public class UIPlayerInfo : UIBase {
	private static UIPlayerInfo instance = null;
	private const string UIName = "UIPlayerInfo";
	private GameObject[] PageAy = new GameObject[3];
	private UIAttributes hexagon;

	//Page 0
	private PersonalView personalView = new PersonalView();
	private AbilityView abilityView = new AbilityView();
	
	//part4
	public UIButton SkillUp;
	public UIButton Back;

	//Page 1
	
	
	//Page 2
	
	private void Awake()
	{

	}
	
	public void UpdateAvatarModel(TItem[] items)
	{
		
	}
	
	public void ChangePlayerName()
	{
		if(UIInput.current.value.Length <= 0)
			return;
		
		//        if(ChangePlayerNameListener != null)
		//            ChangePlayerNameListener();
	}
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UIPlayerInfo Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIPlayerInfo;
			
			return instance;
		}
	}
	
	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow){
				RemoveUI(UIName);
				UIPlayerMgr.Get.Enable = false;
			}
			else
				instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}
	
	protected override void InitCom() {

		//P1
		for (int i = 0; i < PageAy.Length; i++)
			PageAy[i] = GameObject.Find(string.Format("Page{0}", i));

		GameObject[] itemEquipmentBtns = new GameObject[8];
		GameObject personalViewObj = GameObject.Find(UIName + string.Format("/Window/Center/View/PersonalView"));
		GameObject abilityViewObj = GameObject.Find(UIName + string.Format("/Window/Center/View/AbilityView"));
		for (int i = 0; i < itemEquipmentBtns.Length; i++) {
			itemEquipmentBtns[i] = Instantiate(Resources.Load ("Prefab/UI/Items/ItemEquipmentBtn")) as GameObject;	
		}

		personalView.Init(personalViewObj, itemEquipmentBtns);
		abilityView.Init(abilityViewObj, Instantiate(Resources.Load("Prefab/UI/UIattributeHexagon")) as GameObject);
		abilityView.InitBtttonFunction (new EventDelegate (OnUpgradingMasteries));
		SetBtnFun (UIName + "/Window/BottomLeft/BackBtn", OnReturn);
	}

	public void OnReturn()
	{
		UIShow (false);
		UIMainLobby.Get.Show();
	}

	public void OnUpgradingMasteries()
	{
		UIPlayerPotential.UIShow (true);
	}

	public void OnSwitchPage()
	{

	}
	
	protected override void InitData() {
		UpdatePage (0);
	}

	public void UpdatePage(int index)
	{
		switch(index)
		{
			case 0:
				personalView.Update(GameData.Team.Player);
				abilityView.UpdateMasteries(GameData.Team.Player.Potential);
				UIPlayerMgr.Get.ShowUIPlayer(EUIPlayerMode.UIPlayerInfo);
				break;
			case 1:
				break;
			case 2:
				break;
		}
	}

	protected override void OnShow(bool isShow) {
	}

	public void OnAvatarItemHint()
	{
		int index;

		if (int.TryParse (UIButton.current.name, out index))
			if (index < GameData.Team.Player.Items.Length && GameData.Team.Player.Items [index].ID > 0)
				Debug.Log ("index : " + index);
//				UIItemHint.Get.UIShow(GameData.Team.Player.Items [index].ID);
	}
}
