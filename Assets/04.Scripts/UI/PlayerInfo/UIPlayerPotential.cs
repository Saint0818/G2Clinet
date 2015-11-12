using UnityEngine;
using GameStruct;

public class TUpgradeBtn
{
	private GameObject self;
	private UILabel BaseValueLabel;
	private UILabel demandValueLabel;
	private GameObject addBtn;
	private UIButton btn;
	private int demandValue;
	private int curPoint;
	private int addPoint;

	public void Init(GameObject go)
	{
		if(go){
			self = go;
			BaseValueLabel = self.transform.FindChild("BaseValueLabel").gameObject.GetComponent<UILabel>();
			demandValueLabel = self.transform.FindChild("DemandValueLabel").gameObject.GetComponent<UILabel>();
			addBtn = self.transform.FindChild("AddBtn").gameObject;
		}

	}

	public bool CanUse
	{
		set
		{
			addBtn.SetActive(value);
			btn.enabled = value;
		}
	}

	public int DemandValue
	{
		set
		{
			demandValue = value;
			demandValueLabel.text = demandValue.ToString();
		}
	}

	public void Update()
	{
		CanUse = demandValue > GameData.Team.AvatarPotential? true : false;
	}

	public void SetValue(int curt, int add)
	{
		BaseValueLabel.text = string.Format("{0}[ABFF83FF]{1}[-]", curt, add);
	}

	public void DoSave()
	{
		curPoint = curPoint + addPoint;
		addPoint = 0;
		BaseValueLabel.text = "[FFFFFFFF]" + curPoint + "[-]";
	}

}

public class UpgradeView
{
	private GameObject self;
	// sort Blk = 0, Stl = 1, 2PT = 2, 3PT = 3, Dnk = 4, Reb = 5
	private TUpgradeBtn[] upgradeBtns  = new TUpgradeBtn[6];
	private UIAttributes hexagon;
	private int[] addRules = new int[6];
	private int[] addIndexs = new int[6];

	public void Init(GameObject go, GameObject hexgonObj)
	{
		if(go){
			self = go;
			GameObject btn;
			for (int i = 0; i < upgradeBtns.Length; i++) {
				btn = self.transform.FindChild(string.Format("Attribute{0}", i)).gameObject;
				if(btn){
					upgradeBtns[i] = new TUpgradeBtn();
					upgradeBtns[i].Init(btn);
				}
			}
		}

		GameObject hexagonCenter = GameObject.Find("AttrView").gameObject;
		hexagon = hexgonObj.GetComponent<UIAttributes>();
		hexagon.transform.parent = hexagonCenter.transform;
		hexagon.transform.localPosition = Vector3.zero;
		hexagon.transform.localScale = Vector3.one;
		hexagon.EnableTitle = false;

		for (int i = 0; i < addIndexs.Length; i++)
			addIndexs [i] = 0;

		for (int i = 0; i <addRules.Length; i++)
			addRules [i] = 5;
	}

	public void UpdatePotential(TPlayer player)
	{
		if(player.Potential.Length == addIndexs.Length)
		{
			for(int i = 0;i < player.Potential.Length;i++){
				upgradeBtns.SetValue(player.Potential[i], 0);
			}
		}	
	}

}

public class PointView 
{
	private GameObject self;
	private UILabel title;
	private UILabel lvtitle;
	private UILabel lvVaule;
	private UILabel avatartitle;
	private UILabel avatarVaule;
	private UILabel explanation;
	private int[] addRules = new int[6];

	private void Init(GameObject go)
	{
		if (go) {
			self = go;
			title = self.transform.FindChild("HeadingLabel").gameObject.GetComponent<UILabel>();
			lvtitle = self.transform.FindChild("Top/SubheadLabel1").gameObject.GetComponent<UILabel>();
			lvVaule = self.transform.FindChild("Top/LevelPointsLabel").gameObject.GetComponent<UILabel>();
			avatartitle = self.transform.FindChild("Top/SubheadLabel2").gameObject.GetComponent<UILabel>();
			avatarVaule = self.transform.FindChild("Top/AvatarPointsLabel").gameObject.GetComponent<UILabel>();
			explanation = self.transform.FindChild("Bottom/WarningLabel").gameObject.GetComponent<UILabel>();
		}
	}

	public void SetLvPotential(int current, int use)
	{
		lvVaule.text = string.Format ("[3EBBBCFF][b]{0}[ff0000]-{1}[-]", current, use);
	}

	public void SetAvatarPotential(int current, int use)
	{
		avatarVaule.text = string.Format ("[3EBBBCFF][b]{0}[ff0000]-{1}[-]", current, use);
	}

}

public class UIPlayerPotential : UIBase {
	private static UIPlayerPotential instance = null;
	private const string UIName = "UIPlayerPotential";
	private UpgradeView upgradeView;

	private void Awake()
	{

	}
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UIPlayerPotential Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIPlayerPotential;
			
			return instance;
		}
	}
	
	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow){
				RemoveUI(UIName);
				UIPlayerMgr.Get.Enable = true;
			}
			else{
				instance.Show(isShow);
				UIPlayerMgr.Get.Enable = false;
			}
		} else
			if (isShow){
				Get.Show(isShow);
				UIPlayerMgr.Get.Enable = false;
			}
	}
	
	protected override void InitCom() {
		GameObject upgradeViewObj = GameObject.Find(UIName + "Window/Center/UpgradeView");

		if (upgradeViewObj) {
			upgradeView = new UpgradeView ();
			upgradeView.Init(upgradeViewObj, Instantiate(Resources.Load("Prefab/UI/UIattributeHexagon")) as GameObject);
		}

		SetBtnFun (UIName + "/Window/Center/NoBtn", OnReturn);
		SetBtnFun (UIName + "/Window/Center/ResetBtn", OnReset);
		SetBtnFun (UIName + "/Window/Center/CheckBtn", OnCheck);
		SetBtnFun (UIName + "/Window/Center/CancelBtn", OnCancel);
	}

	public void OnReturn()
	{
		UIShow (false);
	}

	public void OnReset()
	{
		//Reset all potential
	}

	public void OnCheck()
	{
		//Save potential
	}

	public void OnCancel()
	{
		//cancel this time;
	}

	protected override		 void InitData() {

	}


	protected override void OnShow(bool isShow) {

	}
}
