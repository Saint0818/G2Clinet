using UnityEngine;
using GameStruct;
using Newtonsoft.Json;

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
			btn = self.GetComponent<UIButton>();
		}

	}

	public bool CanUse
	{
		set
		{
			addBtn.SetActive(value);
			btn.enabled = value;
			btn.defaultColor = btn.enabled == true? Color.white : Color.grey;
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
		BaseValueLabel.text = string.Format("{0}[ABFF83FF]+{1}[-]", curt, add);
	}

	public void DoSave()
	{
		curPoint = curPoint + addPoint;
		addPoint = 0;
		BaseValueLabel.text = "[FFFFFFFF]" + curPoint + "[-]";
	}

	public void InitBtttonFunction(EventDelegate addBtnFunc)
	{
		btn.onClick.Add (addBtnFunc);
	}
}

public class UpgradeView
{
	private GameObject self;
	// sort Blk = 0, Stl = 1, 2PT = 2, 3PT = 3, Dnk = 4, Reb = 5
	private TUpgradeBtn[] upgradeBtns  = new TUpgradeBtn[6];
	private UIAttributes hexagon;
	private int[] addRules = new int[6];
	public int[] AddPotential = new int[6];

	private int useLvPotential;
	private int useAvatarPotential;

	public int UseAvatarPotential
	{
		set{useAvatarPotential = value;}
		get{return useAvatarPotential;}
	}

	public int UseLvPotential
	{
		set{useLvPotential = value;}
		get{return useLvPotential;}
	}

	public void Init(GameObject go, GameObject hexgonObj)
	{
		if(go){
			self = go;
			GameObject btn;
			for (int i = 0; i < upgradeBtns.Length; i++) {
				btn = self.transform.FindChild(string.Format("AttributeBtns/{0}", i)).gameObject;
				if(btn){
					upgradeBtns[i] = new TUpgradeBtn();
					upgradeBtns[i].Init(btn);
					upgradeBtns[i].InitBtttonFunction(new EventDelegate(OnAdd));
					upgradeBtns[i].DemandValue = GameConst.PotentialRule[i];
				}
			}
		}

		GameObject hexagonCenter = GameObject.Find("AttrView").gameObject;
		hexagon = hexgonObj.GetComponent<UIAttributes>();
		hexagon.transform.parent = hexagonCenter.transform;
		hexagon.transform.localPosition = Vector3.zero;
		hexagon.transform.localScale = Vector3.one;
		hexagon.EnableTitle = false;

		for (int i = 0; i < AddPotential.Length; i++)
			AddPotential [i] = 0;

		for (int i = 0; i <addRules.Length; i++)
			addRules [i] = 5;
	}
	
	public void UpdatePotential(TPlayer player)
	{
		if(player.Potential != null && player.Potential.Length == AddPotential.Length)
		{
			for(int i = 0;i < player.Potential.Length;i++){
				upgradeBtns[i].SetValue(player.Potential[i], AddPotential [i]);
			}
		}	
	}

	public void OnAdd()
	{
		int index;
		if (int.TryParse (UIButton.current.name, out index)) {
			if(CanUsePotential(index)){
				AddPotential[index]++;
//				upgradeBtns[index].SetValue(GameData.Team.Player.Potential[index], );

				UIPlayerPotential.Get.UpdateView();
			}
		}
	}

	public void UpdateBtnSate()
	{
		CalculateAddPotential ();
		for (int i = 0; i < upgradeBtns.Length; i++) {
			upgradeBtns[i].CanUse = CanUsePotential(i);	
		}
	}

	public bool CanUsePotential(int index)
	{
		return UIPlayerPotential.Get.CrtAvatarPotential + UIPlayerPotential.Get.CrtLvPotential >= useLvPotential + useAvatarPotential + GameConst.PotentialRule [index];
	}

	private void CalculateAddPotential()
	{
		int count = 0;
		for (int i = 0; i < AddPotential.Length; i++) {
			count += AddPotential[i] * GameConst.PotentialRule[i];
		}
		
		if (UIPlayerPotential.Get.CrtLvPotential >= count) {
			useLvPotential = count;
			useAvatarPotential = 0;
		}
		else {
			useLvPotential = UIPlayerPotential.Get.CrtLvPotential;
			useAvatarPotential = count - UIPlayerPotential.Get.CrtLvPotential;
		}
	}

	public void OnCancel()
	{
		for(int i = 0;i < AddPotential.Length; i++)
			AddPotential[i] = 0;
		
		CalculateAddPotential();
		//cancel this time;
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

	public void Init(GameObject go)
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

	private UpgradeView upgradeView = new UpgradeView();
	private PointView pointView = new PointView();

	//can use avatarPotential
	public int CrtAvatarPotential = 0;
	//can use lvPotential
	public int CrtLvPotential = 0;
	private UILabel lvPotentialLabel;
	private UILabel avatarPotentialLabel;
	private UILabel resetLabel;

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
		GameObject obj = GameObject.Find(UIName + "Window/Center/UpgradeView");

		if (obj) {
			upgradeView.Init (obj, Instantiate (Resources.Load ("Prefab/UI/UIattributeHexagon")) as GameObject);

		}

		obj = GameObject.Find(UIName + "Window/Center/PointView");
		pointView.Init (obj);

		SetBtnFun (UIName + "/Window/Center/NoBtn", OnReturn);
		SetBtnFun (UIName + "/Window/Center/ResetBtn", OnReset);
		SetBtnFun (UIName + "/Window/Center/CheckBtn", OnCheck);
		SetBtnFun (UIName + "/Window/Center/CancelBtn", OnCancel);

		resetLabel = GameObject.Find (UIName + "/Window/Center/ResetBtn/PriceLabel").GetComponent<UILabel> ();
		ResetPrice = GameConst.PotentialResetPrice;
	}

	public int ResetPrice
	{
		set{resetLabel.text = value.ToString();}

	}

	public void OnReturn()
	{
		UIShow (false);
	}

	public void OnReset()
	{
		if(CanUseReset()){
			WWWForm form = new WWWForm();
			form.AddField("Kind", 2);
			SendHttp.Get.Command(URLConst.Potential, waitResetPotential, form);
		}
	}

	private bool CanUseReset()
	{
		if (GameData.Team.Diamond >= GameConst.PotentialResetPrice) {
			for(int i = 0; i < GameData.Team.Player.Potential.Length;i++){
				if(GameData.Team.Player.Potential[i] > 0)
					return true;
			}
		}

		return false;
	}

	public void waitPotential(bool ok, WWW www)
	{
		if (ok) {
			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			GameData.Team.Player.Potential = team.Player.Potential;
			upgradeView.OnCancel();
			UpdateView();
		}
	}

	public void waitResetPotential(bool ok, WWW www)
	{
		if (ok) {
			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			GameData.Team.Player = team.Player;
			GameData.Team.Diamond = team.Diamond;
			upgradeView.OnCancel();
			UpdateView();
			UIMainLobby.Get.UpdateUI();
		}
	}

	public void OnCheck()
	{
		WWWForm form = new WWWForm();
		int[] save = new int[GameConst.PotentialCount];
		
		for(int i = 0;i< save.Length; i++)
			save[i] = GameData.Team.Player.Potential[i] + upgradeView.AddPotential[i];

		form.AddField("Kind", 1);
		form.AddField("Potential", JsonConvert.SerializeObject(save));
		SendHttp.Get.Command(URLConst.Potential, waitPotential, form);
		//Save potential
	}

	public void OnCancel()
	{
		//cancel this time;
		upgradeView.OnCancel ();
		UpdateView ();
	}

	protected override void InitData() {
	}
	
	protected override void OnShow(bool isShow) {
		if(isShow)
			UpdateView ();
	}

	public int GetCrtAvatarPotential()
	{
		int use = 0;
		if (GameData.Team.PlayerBank != null && GameData.Team.PlayerBank.Length > 1) {
			for(int i = 0;i< GameData.Team.PlayerBank.Length; i++){
				if(GameData.Team.PlayerBank[i].RoleIndex != GameData.Team.Player.RoleIndex){
					use += GetUseAvatarPotentialFromBank(GameData.Team.PlayerBank[i]);
				}
			}
		}
		
		use += GetUseAvatarPotential (GameData.Team.Player);
		
		return GameData.Team.AvatarPotential - use;
	}

	public int GetCurrentLvPotential(TPlayer player)
	{
		int lvpoint = GetLvPotential (player.Lv);
		int use = 0;

		if(player.Potential != null)
			for(int i = 0; i < player.Potential.Length; i++){
				use += player.Potential[i] * GameConst.PotentialRule[i]; 
			}
		
		if (lvpoint > use)
			return lvpoint - use;
		else
			return 0;
	}

	public int GetUseAvatarPotentialFromBank(TPlayerBank player)
	{
		int lvpoint = GetLvPotential (player.Lv);
		int use = 0;
		
		for(int i = 0; i < player.Potential.Length; i++){
			use += player.Potential[i] * GameConst.PotentialRule[i]; 
		}
		
		if (use > lvpoint)
			return use - lvpoint;
		else
			return 0;
	}
	
	public int GetUseAvatarPotential(TPlayer player)
	{
		int lvpoint = GetLvPotential (player.Lv);
		int use = 0;

		if(player.Potential != null)
			for(int i = 0; i < player.Potential.Length; i++){
				use += player.Potential[i] * GameConst.PotentialRule[i]; 
			}
		
		if (use > lvpoint)
			return use - lvpoint;
		else
			return 0;
	}

	public void UpdateView()
	{
		upgradeView.UpdatePotential(GameData.Team.Player);

		CrtLvPotential = GetCurrentLvPotential (GameData.Team.Player);
		pointView.SetLvPotential (CrtLvPotential, upgradeView.UseLvPotential);

		CrtAvatarPotential = GetCrtAvatarPotential ();
		pointView.SetAvatarPotential (CrtAvatarPotential, upgradeView.UseAvatarPotential);

		upgradeView.UpdateBtnSate();
	}

	public int GetLvPotential(int lv)
	{
		return (lv - 1) * GameConst.PreLvPotential;
	}


}
