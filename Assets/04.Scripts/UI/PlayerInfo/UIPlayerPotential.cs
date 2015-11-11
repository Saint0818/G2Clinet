using UnityEngine;

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
		CanUse = demandValue > GameData.Team.MasteriesPoint? true : false;
	}

	public int Add
	{
		set{ 
			addPoint += value;
			BaseValueLabel.text = curPoint +"[ABFF83FF]"+ addPoint + "[-]";
		}
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
	}

	public void OnReturn()
	{
		UIShow (false);
	}

	protected override void InitData() {

	}


	protected override void OnShow(bool isShow) {

	}
}
