using Newtonsoft.Json;
using UnityEngine;

public struct TRenameResult {
	public string Name;
	public int Diamond;
    public int RenameCount;
}

public class TNameView
{
	private GameObject self;
	private UIInput nameInput;
	private UILabel nameLabel;
	private UIButton randomBtn;
	private UILabel tip;
	private UILabel price;
	private bool isInit = false;

    public void Init(GameObject go, EventDelegate randomFunction = null, EventDelegate changeFunction = null)
	{
		if (go) {
			self =  go;
			nameInput = self.transform.FindChild("PlayerName").gameObject.GetComponent<UIInput>();
			nameLabel = nameInput.transform.FindChild("NameLabel").gameObject.GetComponent<UILabel>();
			randomBtn = self.transform.FindChild("RandomBtn").gameObject.GetComponent<UIButton>();
			tip = self.transform.FindChild("WarningLabel").gameObject.GetComponent<UILabel>();
			price = self.transform.FindChild("SpendGemIcon/PriceLabel").gameObject.GetComponent<UILabel>();
            isInit = self && nameInput && nameLabel && randomBtn && tip && price;

			if(isInit)
			{
                UpdateUI();
				if(randomFunction != null)
					randomBtn.onClick.Add(randomFunction);

                nameInput.onChange.Add(changeFunction);
			}
		}
	}

    public void UpdateUI() {
        int diamond = 0;
        if (!string.IsNullOrEmpty(GameData.Team.Player.Name))
            diamond = (GameData.Team.LifetimeRecord.RenameCount+1) * GameConst.RenamePrice;

        bool flag = GameData.Team.CoinEnough(0, diamond);
        price.color = GameData.CoinEnoughTextColor(flag);
        price.text = diamond.ToString();
    }

	public void UpdateView()
	{
		if (GameData.Team.Player.Name != string.Empty)
			nameLabel.text = GameData.Team.Player.Name;
		else
			UINamed.Get.OnRandomName ();
	}

	public string Name
	{
		set{nameInput.value = value;}
		get{return nameLabel.text;}
	}

	public bool IsChange
	{
		get{return GameData.Team.Player.Name != Name;}
	}

	public bool IsLegal
	{
		get{ 
			if(Name.Length >= 2 && Name.Length <= 16)
			{
				return true;
			}
			else
			{
				UIHint.Get.ShowHint(TextConst.S(3407), Color.red);
				return false;
			}
		}
	}
}

public class UINamed : UIBase {
	private static UINamed instance = null;
	private const string UIName = "UINamed";
	private TNameView nameView = new TNameView();
    private UISprite spriteOKBtn;
	private UIButton yesBtn;
	private UIButton noBtn;

	public static int OpenKind = 1;

	public static void UIShow(bool isShow){
		if(instance) {
            if (!isShow)
                RemoveUI(UIName);
            else
			    instance.Show(isShow);
        }
		else
		if(isShow)
			Get.Show(isShow);
	}

	public static UINamed Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UINamed;
			
			return instance;
		}
	}

	protected override void InitCom() {
		GameObject obj = GameObject.Find (UIName + "/Center/NamedView");
        spriteOKBtn = GameObject.Find (UIName + "/Center/CheckBtn/Icon/Btn").GetComponent<UISprite>();
        spriteOKBtn.spriteName = "buttor_gray";

        nameView.Init (obj, new EventDelegate(OnRandomName), new EventDelegate(OnChange));
		SetBtnFun(UIName + "/Center/CheckBtn", OnCheckBtn);
		SetBtnFun(UIName + "/Center/NoBtn", OnCancelChange);
	}

	protected override void OnShow(bool isShow) {
        base.OnShow(isShow);

		if (isShow) {
			nameView.UpdateView ();
            if (string.IsNullOrEmpty(GameData.Team.Player.Name))
                SetLabel (UIName + "/Center/NamedView/SpendGemIcon/PriceLabel", "0");
            else
                SetLabel (UIName + "/Center/NamedView/SpendGemIcon/PriceLabel", (300 * (GameData.Team.LifetimeRecord.RenameCount + 1)).ToString());
		}
	}

	public void OnRandomName()
	{
		if (TextConst.TeamNameAy != null && TextConst.TeamNameAy.Length > 0) {
			int index1 = UnityEngine.Random.Range (0, TextConst.TeamNameAy.Length - 1);
			int index2 = UnityEngine.Random.Range (0, TextConst.TeamNameAy.Length - 1);
			int index3 = UnityEngine.Random.Range (0, TextConst.TeamNameAy.Length - 1);
			nameView.Name = TextConst.TeamNameAy [index1].TeamName1 + TextConst.TeamNameAy [index2].TeamName2 + TextConst.TeamNameAy [index3].TeamName3;
		}
	}

	private void OnCheckBtn()
	{
        if (nameView.IsChange && nameView.IsLegal) {
            int diamond = 0;
            if (!string.IsNullOrEmpty(GameData.Team.Player.Name))
                diamond = (GameData.Team.LifetimeRecord.RenameCount+1) * 300;

            if (diamond > 0)
                CheckDiamond(diamond, true, string.Format(TextConst.S(3406), diamond), changePlayerName, nameView.UpdateUI);
            else
                changePlayerName();
        }
	}

    public void OnChange() {
        if (nameView.IsChange && nameView.IsLegal)
            spriteOKBtn.spriteName = "button_orange1";
        else
            spriteOKBtn.spriteName = "button_gray";
    }

	private void OnCancelChange()
	{
		if (nameView.IsChange) {
			nameView.UpdateView();
		}

		UIShow(false);
	}

	private void changePlayerName()
	{
		WWWForm form = new WWWForm();
		form.AddField("NewPlayerName", nameView.Name);
		SendHttp.Get.Command(URLConst.ChangePlayerName, waitChangePlayerName, form, true);
	}

	private void waitChangePlayerName(bool ok, WWW www)
	{
		if (ok)
		{
            if (SendHttp.Get.CheckServerMessage(www.text)) {
				TRenameResult result = JsonConvert.DeserializeObject<TRenameResult>(www.text, SendHttp.Get.JsonSetting);
    			GameData.Team.Player.Name = result.Name;
    			GameData.Team.Diamond = result.Diamond;
                GameData.Team.LifetimeRecord.RenameCount = result.RenameCount;
    			UIMainLobby.Get.UpdateUI();
                if (UIPlayerInfo.Visible) {
    				UIPlayerInfo.UIShow(true,ref GameData.Team);
                    UIPlayerInfo.Get.UpdatePage (0);
                }

                UIShow (false);
            }
        } else
            UIHint.Get.ShowHint(TextConst.S(3409), Color.red);
	}
}
