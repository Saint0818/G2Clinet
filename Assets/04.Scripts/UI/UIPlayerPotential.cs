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
	private GameObject addBtnObj;
	private GameObject minusBtnObj;
	private UIButton btnAdd;
	private UIButton btnMinus;
	private int demandValue;
	private int curPoint;

	public void Init(GameObject go)
	{
		if(go){
			self = go;
			BaseValueLabel = self.transform.FindChild("BaseValueLabel").gameObject.GetComponent<UILabel>();
			demandValueLabel = self.transform.FindChild("DemandValueLabel").gameObject.GetComponent<UILabel>();
			addBtnObj = self.transform.FindChild("AddBtn").gameObject;
			minusBtnObj = self.transform.FindChild ("MinusBtn").gameObject;
			btnAdd = self.GetComponent<UIButton>();
			btnMinus = minusBtnObj.GetComponent<UIButton>();
		}

	}

	public bool CanUseAdd
	{
		set
		{
			addBtnObj.SetActive(value);
			btnAdd.enabled = value;
			btnAdd.defaultColor = btnAdd.enabled == true? Color.white : Color.grey;
		}
	}

	public bool CanUseMinus
	{
		set
		{
			minusBtnObj.SetActive(value);
			btnMinus.enabled = value;
			btnMinus.defaultColor = btnMinus.enabled == true? Color.white : Color.grey;
		}
	}

	public int DemandValue
	{
		set
		{
			demandValue = value;
            demandValueLabel.text = (demandValue).ToString();
		}
	}

	public void Update()
	{
		CanUseAdd = demandValue > GameData.Team.AvatarPotential? true : false;
		CanUseMinus = false;
	}

	public void SetValue(int curt, int add)
	{
		if (add > 0)
				BaseValueLabel.text = string.Format ("{0}[ABFF83FF]+{1}[-]", curt, add);
		else {
				BaseValueLabel.text = string.Format("{0}", curt);
		}
						
	}

	public void InitBtttonFunction(EventDelegate addBtnFunc, EventDelegate minusBtnFunc)
	{
		btnAdd.onClick.Add (addBtnFunc);
		btnMinus.onClick.Add (minusBtnFunc);

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
					upgradeBtns[i].InitBtttonFunction(new EventDelegate(OnAdd), new EventDelegate(OnMinus));
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
				AddPotential[index]+= AddLevel;
				UIPlayerPotential.Get.UpdateView();
				UIPlayerPotential.Get.SetUseState (EPotential.adding);
			}
		}
	}
	public void OnMinus()
	{
		int index;
		if (int.TryParse (UIButton.current.transform.parent.name, out index)) {
			if(AddPotential[index]>0){
				AddPotential[index]-= AddLevel;
				UIPlayerPotential.Get.UpdateView();
				UIPlayerPotential.Get.SetUseState (EPotential.adding);
			}
		}
	}
	public int AddLevel = 1;
	
	public void OnChangeAddLevle()
	{
		if (AddLevel == 1)
			AddLevel = 5;
		else if (AddLevel == 5)
			AddLevel = 10;
		else
			AddLevel = 1;
		UpdateBtnSate ();			
	}

	public void UpdateBtnSate()
	{
		CalculateAddPotential ();
		for (int i = 0; i < upgradeBtns.Length; i++) {
			upgradeBtns[i].CanUseAdd = CanUsePotential(i);	
			upgradeBtns [i].CanUseMinus = CanMinusPotential (i);
		}
	}

    //是否可以升級此屬性
	public bool CanUsePotential(int index)
	{
        return UIPlayerPotential.Get.CrtAvatarPotential + GameFunction.GetCurrentLvPotential(GameData.Team.Player) >= 
			useLvPotential + useAvatarPotential + (GameFunction.GetPotentialRule(bodytype, index) * AddLevel);
	}

	public bool CanMinusPotential(int index)
	{
		return AddPotential[index]> 0;
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
	}

	public void EnableHexagon(bool show)
	{
		hexagon.SetVisible (show);
	}

}

public class UIPlayerPotential : UIBase {
	private static UIPlayerPotential instance = null;
	private const string UIName = "UIPlayerPotential";

    public int CrtAvatarPotential = 0; //can use avatarPotential
    public int CrtLvPotential = 0; //can use lvPotential
	private UpgradeView upgradeView = new UpgradeView();
	private UILabel labelPotential;
	private UILabel resetLabel;
	private UIButton resetBtn;
	private UIButton saveBtn;
	private UIButton cancelBtn;

	private float bCombatPower;
	private float aCombatPower;
	
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
	
	public static void UIShow(bool isShow)
	{
	    if (instance) {
			if (!isShow){
                RemoveUI(instance.gameObject);
			}
			else{
				instance.Show(isShow);
			}
		} else
		if (isShow){
			Get.Show(isShow);
		}

	    if(isShow)
	    {
	        Statistic.Ins.LogScreen(21);
	        Statistic.Ins.LogEvent(501);
	    }
	}

    protected override void InitCom() {
		GameObject obj = GameObject.Find(UIName + "Window/Center/UpgradeView");

		if (obj) 
			upgradeView.Init (obj, Instantiate (Resources.Load ("Prefab/UI/UIattributeHexagon")) as GameObject);

		resetBtn = GameObject.Find(UIName + "/Window/Center/ResetBtn").gameObject.GetComponent<UIButton>();
		saveBtn = GameObject.Find(UIName + "/Window/Center/CheckBtn").gameObject.GetComponent<UIButton>();
		cancelBtn = GameObject.Find(UIName + "/Window/Center/CancelBtn").gameObject.GetComponent<UIButton>();

        SetBtnFun (UIName + "/BottomLeft/BackBtn", OnReturn);
		SetBtnFun (ref resetBtn, OnReset);
		SetBtnFun (ref saveBtn, OnCheck);
		SetBtnFun (ref cancelBtn, OnCancel);
		SetBtnFun (UIName + "/Window/Center/ExplainBtn", OnExplain);
		SetUseState (EPotential.none);

        labelPotential = GameObject.Find (UIName + "/Window/Center/PointView/Top/LevelPointsLabel").GetComponent<UILabel> (); 
		resetLabel = GameObject.Find (UIName + "/Window/Center/ResetBtn/PriceLabel").GetComponent<UILabel> ();
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

    public void SetLvPotential(int lvPoint, int avatarPoint)
    {
        labelPotential.text = string.Format ("[3EBBBCFF]{0}[-]", lvPoint);
        if (avatarPoint > 0)
            labelPotential.text += " + " + string.Format ("[3EBBBCFF]{0}[-]", avatarPoint);
    }

	public void OnExplain () {
		UIAttributeExplain.UIShow(true);
	}

	public void OnReturn()
	{
		UIShow (false);
		GameData.Team.Player.SetAttribute (GameEnum.ESkillType.Player);
		UIMainLobby.Get.Show();
	}

    private void initResetDiamond() {
        bool flag = GameData.Team.CoinEnough(0, GameConst.PotentialResetPrice);
        resetLabel.color = GameData.CoinEnoughTextColor(flag);
        resetLabel.text = GameConst.PotentialResetPrice.ToString();
    }

    private void askReset() {
        WWWForm form = new WWWForm();
        form.AddField("Kind", 2);
        SendHttp.Get.Command(URLConst.Potential, waitResetPotential, form);
    }

	public void OnReset()
	{
		if(CanUseReset()) {
            CheckDiamond(GameConst.PotentialResetPrice, true, string.Format(TextConst.S(3209), GameConst.PotentialResetPrice), askReset, initResetDiamond);
		}
	}

	private bool CanUseReset()
	{
		foreach (KeyValuePair<EAttribute, int> item in GameData.Team.Player.Potential) {
			if(item.Value > 0)
				return true;
		}

		return false;
	}

	public void waitPotential(bool ok, WWW www)
	{
		if (ok) {
            TTeam team = JsonConvertWrapper.DeserializeObject<TTeam>(www.text);
            GameData.Team.PlayerInit();
			bCombatPower = GameData.Team.Player.CombatPower();
			GameData.Team.Player.Potential = team.Player.Potential;
            GameData.Team.PlayerInit();
			//GameData.Team.Player.SetAttribute(GameEnum.ESkillType.Player);
			aCombatPower = GameData.Team.Player.CombatPower();;
			UIOverallHint.Get.ShowView(bCombatPower, aCombatPower);
			upgradeView.OnCancel();
			UpdateView();
			SetUseState (EPotential.none);
		}
	}

    private void waitResetPotential(bool ok, WWW www)
	{
        if(ok)
        {
            TTeam team = JsonConvertWrapper.DeserializeObject<TTeam>(www.text);

            Statistic.Ins.LogEvent(502, GameData.Team.Diamond - team.Diamond);

            GameData.Team.Player.Potential = team.Player.Potential;
            GameData.Team.Diamond = team.Diamond;
            GameData.Team.Player.SetAttribute(GameEnum.ESkillType.Player);
            upgradeView.OnCancel();
            UpdateView();
            UIMainLobby.Get.UpdateUI();
            SetUseState(EPotential.none);
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

	public void OnChangeLvel()
	{
		upgradeView.OnChangeAddLevle();
	}
	
	protected override void OnShow(bool isShow) {
        base.OnShow(isShow);
		if (isShow) {
            GameData.Team.PlayerInit();
            initResetDiamond();
			UpdateView();
			upgradeView.EnableHexagon(true);
		}
	}

	public void UpdateView()
	{
		upgradeView.UpdatePotential(GameData.Team.Player);
		GameFunction.UpdateAttrHexagon (upgradeView.hexagon, GameData.Team.Player, upgradeView.AddPotential);
		CrtLvPotential = GameFunction.GetCurrentLvPotential (GameData.Team.Player);
		CrtAvatarPotential = GameFunction.GetAllPlayerTotalUseAvatarPotential ();
		upgradeView.UpdateBtnSate();
        SetLvPotential (CrtLvPotential - upgradeView.UseLvPotential, CrtAvatarPotential - upgradeView.UseAvatarPotential);
	}

	public int AddLevel
	{
		get{ return upgradeView.AddLevel; }
	}
}