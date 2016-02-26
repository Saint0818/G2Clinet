using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public enum EPotential
{
	none,
	adding,
}

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
		if (add > 0)
				BaseValueLabel.text = string.Format ("{0}[ABFF83FF]+{1}[-]", curt, add);
		else {
				BaseValueLabel.text = string.Format("{0}", curt);
		}
						
	}

//	public void DotSave()
//	{
//		curPoint = curPoint + addPoint;
//		addPoint = 0;
//		BaseValueLabel.text = "[FFFFFFFF]" + curPoint + "[-]";
//	}

	public void InitBtttonFunction(EventDelegate addBtnFunc)
	{
		btn.onClick.Add (addBtnFunc);
	}
}

public class UpgradeView
{
	private GameObject self;
	// sort Blk = 0, Stl = 1, 2PT = 2, 3PT = 3, Dnk = 4, Reb = 5
    private TUpgradeBtn[] upgradeBtns  = new TUpgradeBtn[GameConst.PotentialCount];
	public UIAttributes hexagon;
    public int[] AddPotential = new int[GameConst.PotentialCount];
	private int useLvPotential;
	private int useAvatarPotential;
    private int bodytype = 0;

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
	}
	
	public void UpdatePotential(TPlayer player)
	{
		hexagon.enabled = true;
        bodytype = player.BodyType;
        int index = 0;
        float basic = 0;

        for (int i = 0; i < upgradeBtns.Length; i++)
            upgradeBtns[i].DemandValue = GameFunction.GetPotentialRule(bodytype, i);

        if (player.Potential == null || player.Potential.Count == 0)
        {
            for (int i = 0; i < GameConst.PotentialCount; i++)
            {
                EAttribute att = GameFunction.GetAttributeKind(i);
                if (!player.Potential.ContainsKey(att))
                    player.Potential.Add(att, 0);
            }
        }

        foreach (var item in GameData.Team.Player.Potential)
        {
            index = GameFunction.GetAttributeIndex(item.Key);

            switch (item.Key)
            {
                case EAttribute.Point2:
                    basic = player.Point2;
                    break;
                case EAttribute.Point3:
                    basic = player.Point3;
                    break;
                case EAttribute.Dunk:
                    basic = player.Dunk;
                    break;
                case EAttribute.Rebound:
                    basic = player.Rebound;
                    break;
                case EAttribute.Block:
                    basic = player.Block;
                    break;
                case EAttribute.Steal:
                    basic = player.Steal;
                    break;
            }
            upgradeBtns[index].SetValue((int)basic, AddPotential[index]);
        }
	}

	public void OnAdd()
	{
		int index;
		if (int.TryParse (UIButton.current.name, out index)) {
			if(CanUsePotential(index)){
				AddPotential[index]++;
				UIPlayerPotential.Get.UpdateView();
				UIPlayerPotential.Get.SetUseState (EPotential.adding);
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
        return UIPlayerPotential.Get.CrtAvatarPotential + GameFunction.GetCurrentLvPotential(GameData.Team.Player) >= 
            useLvPotential + useAvatarPotential + GameFunction.GetPotentialRule(bodytype, index);
	}

	private void CalculateAddPotential()
	{
		int count = 0;
		for (int i = 0; i < AddPotential.Length; i++) {
            count += AddPotential[i] * GameFunction.GetPotentialRule(bodytype, i);
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

	public void EnableHexagon(bool show)
	{
		hexagon.SetVisible (show);
	}

}

public class PointView 
{
	private GameObject self;
//	private UILabel title;
//	private UILabel lvtitle;
	private UILabel lvVaule;
//	private UILabel avatartitle;
	private UILabel avatarVaule;
//	private UILabel explanation;

	public void Init(GameObject go)
	{
		if (go) {
			self = go;
//			title = self.transform.FindChild("HeadingLabel").gameObject.GetComponent<UILabel>();
//			lvtitle = self.transform.FindChild("Top/SubheadLabel1").gameObject.GetComponent<UILabel>();
			lvVaule = self.transform.FindChild("Top/LevelPointsLabel").gameObject.GetComponent<UILabel>();
//			avatartitle = self.transform.FindChild("Top/SubheadLabel2").gameObject.GetComponent<UILabel>();
			avatarVaule = self.transform.FindChild("Top/AvatarPointsLabel").gameObject.GetComponent<UILabel>();
//			explanation = self.transform.FindChild("Bottom/WarningLabel").gameObject.GetComponent<UILabel>();
		}
	}

	public void SetLvPotential(int current, int use)
	{
        lvVaule.text = string.Format ("[3EBBBCFF]{0}[-]", current - use);
	}

	public void SetAvatarPotential(int current, int use)
	{
        avatarVaule.text = string.Format ("[3EBBBCFF]{0}[-]", current - use);
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
	private UIButton resetBtn;
	private UIButton saveBtn;
	private UIButton cancelBtn;
	
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

		resetBtn = GameObject.Find(UIName + "/Window/Center/ResetBtn").gameObject.GetComponent<UIButton>();
		saveBtn = GameObject.Find(UIName + "/Window/Center/CheckBtn").gameObject.GetComponent<UIButton>();
		cancelBtn = GameObject.Find(UIName + "/Window/Center/CancelBtn").gameObject.GetComponent<UIButton>();

		SetBtnFun (UIName + "/Window/Center/NoBtn", OnReturn);
		SetBtnFun (ref resetBtn, OnReset);
		SetBtnFun (ref saveBtn, OnCheck);
		SetBtnFun (ref cancelBtn, OnCancel);
		SetUseState (EPotential.none);

		resetLabel = GameObject.Find (UIName + "/Window/Center/ResetBtn/PriceLabel").GetComponent<UILabel> ();
		ResetPrice = GameConst.PotentialResetPrice;
	}

	public void SetUseState(EPotential state)
	{
		switch(state)
		{
			case EPotential.none:
				saveBtn.isEnabled = false;
				cancelBtn.isEnabled = false;
				resetBtn.isEnabled = CanUseReset();
				break;
			case EPotential.adding:
				saveBtn.isEnabled = true;
				cancelBtn.isEnabled = true;
				resetBtn.isEnabled = false;
				break;
		}
	}

	public int ResetPrice
	{
		set{resetLabel.text = value.ToString();}
	}

	public void OnReturn()
	{
		UIShow (false);
		GameData.Team.Player.SetAttribute (GameEnum.ESkillType.Player);
		UIPlayerMgr.Get.Enable = false;
		UIMainLobby.Get.Show();
//		UIPlayerInfo.UIShow (true, ref GameData.Team);
//		UIPlayerInfo.Get.UpdatePage (0);
//		UIPlayerInfo.Get.UpdateHexagon(true);
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
			foreach (KeyValuePair<EAttribute, int> item in GameData.Team.Player.Potential) {
				if(item.Value > 0)
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
			GameData.Team.Player.SetAttribute(GameEnum.ESkillType.Player);
			upgradeView.OnCancel();
			UpdateView();
			SetUseState (EPotential.none);
		}
	}

	public void waitResetPotential(bool ok, WWW www)
	{
		if (ok) {
			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			GameData.Team.Player.Potential = team.Player.Potential;
			GameData.Team.Diamond = team.Diamond;
			GameData.Team.Player.SetAttribute(GameEnum.ESkillType.Player);
			upgradeView.OnCancel();
			UpdateView();
			UIMainLobby.Get.UpdateUI();
			SetUseState (EPotential.none);
		}
	}

	public void OnCheck()
	{
		WWWForm form = new WWWForm();
		Dictionary<EAttribute, int> save = new Dictionary<EAttribute, int>();
		save = GameFunction.SumAttribute (GameData.Team.Player.Potential, upgradeView.AddPotential);

		form.AddField("Kind", 1);
		form.AddField("Potential", JsonConvert.SerializeObject(save));
		SendHttp.Get.Command(URLConst.Potential, waitPotential, form);
	}

	public void OnCancel()
	{
		//cancel this time;
		upgradeView.OnCancel ();
		UpdateView ();
		SetUseState (EPotential.none);
	}
	
	protected override void OnShow(bool isShow) {
        base.OnShow(isShow);
		if (isShow) {
			UpdateView ();
			upgradeView.EnableHexagon(true);
//			UIPlayerInfo.Get.UpdateHexagon(false);
		}
	}

	public void UpdateView()
	{
		upgradeView.UpdatePotential(GameData.Team.Player);
		GameFunction.UpdateAttrHexagon (upgradeView.hexagon, GameData.Team.Player, upgradeView.AddPotential);
		CrtLvPotential = GameFunction.GetCurrentLvPotential (GameData.Team.Player);
		CrtAvatarPotential = GameFunction.GetAllPlayerTotalUseAvatarPotential ();
		upgradeView.UpdateBtnSate();
		pointView.SetLvPotential (CrtLvPotential, upgradeView.UseLvPotential);
		pointView.SetAvatarPotential (CrtAvatarPotential, upgradeView.UseAvatarPotential);
	}
}
